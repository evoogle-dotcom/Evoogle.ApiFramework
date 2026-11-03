# Repository

This repository is home to the following **Evoogle** projects. These projects are maintained by [Evoogle](https://github.com/evoogle-dotcom) and licensed under the [MIT License](License.txt).

- [Evoogle.ApiFramework](#Evoogle.ApiFramework)

## Evoogle.ApiFramework

TBD: Need a high-level description.

- **Schema**. Library that represents an API schema where a schema defines a type system that describes what data can be queried or mutated from an API.

### Declarative POCO Models

Reference `Evoogle.ApiFramework.Annotations` from a POCO-only domain-model project when that
project should declare API-schema metadata without depending on the schema runtime. Its public
types share the root `Evoogle.ApiFramework` namespace:

```csharp
using Evoogle.ApiFramework;

[ApiObject]
public sealed class Customer
{
    [ApiKey]
    public Guid Id { get; set; }

    [ApiVersion]
    public long Version { get; set; }

    [ApiRelationship
    (
        ApiName = "CustomerOrders",
        Kind = ApiRelationshipKind.OneToMany,
        DeleteBehavior = ApiRelationshipDeleteBehavior.Delete
    )]
    public ICollection<Order> Orders { get; } = [];
}
```

`Evoogle.ApiFramework` references the annotations package transitively and provides schema
construction, discovery, validation, JSON, and runtime materialization.

### Configuring a Schema

Use `ApiSchemaBuilder` when defining schemas in code. The fluent API is designed around familiar .NET patterns:

- Register scalar, enum, and object CLR types with `AddScalar<T>()`, `AddEnum<T>()`, and `AddObject<T>()`.
- Prefer expression-based overloads such as `AddProperty(c => c.Name)` and `AddKey("PK_Customer", c => c.Id)` so CLR member names are refactor-safe.
- Use `WithVersion(c => c.Version)` for a non-nullable property-backed version, or
  `WithRepositoryVersion<TVersion>()` when repository metadata supplies it.
- With default annotations enabled, apply `[ApiVersion]` to a property or field for a
  property-backed version, or apply `[ApiVersion(ClrType = typeof(TVersion))]` to an object type
  for a repository-backed version. Member annotations must omit `ClrType`, and the annotation does
  not register the scalar or property.
- Use `AddProperty` to infer nullability from CLR members; use `AsNullable()` or `AsNonNullable()`
  in its configuration callback to override it explicitly.
- Configure larger schemas with `IApiObjectTypeConfiguration<T>` and relationship configuration classes when inline lambdas become too large.
- Use relationship shortcuts for common cases, or the full relationship builders when you need named principal keys, composite keys, or extensions.

```csharp
var schema = new ApiSchemaBuilder()
    .WithName("Commerce")
    .WithVersion("v1")
    .WithOptions(o => o.ThrowOnNullKeyPart())
    .AddScalar<Guid>()
    .AddScalar<long>()
    .AddScalar<string>()
    .AddEnum<OrderStatus>(e => e.AddAllValues())
    .AddObject<Customer>(o => o
        .AddProperty(c => c.Id)
        .AddProperty(c => c.Name)
        .AddProperty(c => c.Version)
        .AddProperty(c => c.Orders)
        .WithVersion(c => c.Version)
        .AddKey("PK_Customer", c => c.Id))
    .AddObject<Order>(order => order
        .AddProperty(o => o.Id)
        .AddProperty(o => o.CustomerId)
        .AddKey("PK_Order", o => o.Id))
    .AddOneToManyRelationship<Customer, Order>("CustomerOrders", o => o.CustomerId)
    .Build();
```

Schema-wide options can be overridden on individual object types:

```csharp
.AddObject<Customer>(o => o
    .WithOptions(options => options.UseEmptyKeyOnNullKeyPart())
    .AddProperty(c => c.Id)
    .AddKey("PK_Customer", c => c.Id))
```

`Build()` compiles, validates, freezes, and returns the schema. A returned schema is immutable and
safe for concurrent runtime reads.

An object type can explicitly select one of its named keys with `WithCanonicalKey("CustomerId")`.
After compilation, `ApiObjectType.ApiCanonicalKey` references that existing definition and can use
its normal `MaterializeKey` operation. Selection may precede key declaration; an unknown name is a
schema compilation error. No key is selected implicitly, even when only one key exists. Schema JSON
stores `ApiCanonicalKey` as the selected key's API name. Canonical identity does not imply query or
mutation support and does not change relationship principal-key inference.

Each `ApiObjectType` may expose one optional `ApiVersion` member whose value is an
`ApiVersionDefinition`. A property-backed definition references an existing non-nullable scalar
`ApiProperty`; a repository-backed definition has no CLR member.
`ApiVersion` is an opaque equality token whose exact CLR representation matters. Strings use
ordinal equality, and recognized binary values are defensively copied.

Custom reference-type scalar versions are supported for third-party integrations as an explicit
opt-in. The framework retains the supplied reference, so it must remain immutable and provide
stable equality and hash-code behavior for its full lifetime as an `ApiVersion`. The framework
cannot enforce deep immutability for arbitrary custom objects.

This release supplies the schema metadata, builders, JSON contract, compilation, and value
materialization foundation. Repository retrieval records, command concurrency policies, atomic
mutation enforcement, provider mappings, and client state reconciliation remain operation-runtime
work.

### Validation and Error Reporting

Schema construction is a one-way compilation lifecycle: mutable builder or JSON state becomes one
unpublished graph, that graph is resolved and validated once, and a frozen `ApiSchema` is published
only when it is valid. `ApiSchemaBuilder.Build()` throws one `ApiSchemaCompilationException` for
expected schema errors. `BuildResult()` returns an immutable `ApiSchemaCompilationResult` instead, with a
non-null `Schema` for success or warnings and a null `Schema` for errors. Both expose non-default
immutable `Issues`, `Errors`, and `Warnings` arrays. Malformed JSON and unexpected programming or
infrastructure exceptions still propagate normally.

`ApiSchema` has no public constructor, `Compile`, or instance-based factory. Each builder call
materializes a fresh graph; failed graphs are discarded and cannot be retried. Updating application
metadata means building a separate schema and atomically exchanging the application-held reference.
Root JSON deserialization follows the same compiler path. Warning-only JSON remains deserializable,
although `JsonSerializer` does not return the warnings.

This aggregation applies to schema validity problems such as duplicate API names, unresolved CLR
types, invalid key paths, missing properties, invalid relationship definitions, and other
whole-schema consistency issues. Malformed JSON or incompatible JSON token shapes may still be
rejected by the JSON serializer before schema compilation runs. The [enum handling policy for JSON
deserialization](docs/EnumJsonDeserializationPolicy.md) defines which well-formed enum errors are
reported during compilation and which remain JSON parsing errors.

Schema-element traversal diagnostics use fully qualified paths. Every schema-element path begins
with the schema, such as `ApiSchema["Store"].ApiObjectType["Order"]`, and continues through the
complete structural location. Relationship children use semantic roles such as
`ApiPrincipalEndA` and `ApiForeignKeyB`; ordered key paths and segments include their
zero-based position and an available label. Issues produced while traversing schema elements are
also logged once through the schema context logger with their severity, compilation code,
path, description, and optional remediation.

Before element compilation begins, the framework builds an ownership tree rooted at
`ApiSchema`. Every `ApiSchemaElement` exposes its parent, children, siblings, and root through the
read-only NTree node interface, so callers can navigate or traverse the compiled schema without
reconstructing containment from its specialized collections. Concrete schema-element variables can
use the same children, descendant, path, and visitor operations directly through
`ApiSchemaElementExtensions`; those operations delegate to NTree. Cross-references, such as resolved
named types and relationship targets, remain outside this ownership tree.

Ownership is exclusive: one schema-element instance cannot belong to two schema trees. Structural
constructor inputs are defensively copied into non-default immutable arrays, so changing a source
collection after construction cannot change the model. Construct a new model to change its
structural shape. Inline scalar, enum, object, and collection types are owned and compiled in
place, including nested inline collections and keys owned by inline object types; inline named types
are not added to schema-level lookup registries. Relationship ends and many-to-many associations
derive their owner from the ownership tree. Lookup maps are frozen before publication, key names are
precomputed, and structural and reverse-reference collections use immutable arrays.

`ApiSchemaElement.Kind` is a runtime, cast-safe discriminator for built-in schema families. The
specialized type, relationship, and relationship-end kind properties remain their authoritative
domain discriminators. `ApiKeyDefinition` is the built-in anonymous key representation and
`ApiNamedKeyDefinition` reports `NamedKeyDefinition`; callers cannot construct or derive new key
metadata outside the framework assembly.

Schema metadata CLR types use the `Definition` suffix, including `ApiKeyDefinition`,
`ApiNamedKeyDefinition`, and `ApiVersionDefinition`. Contextual members, JSON, fluent methods, and
annotations use concise domain roles: `ApiKeys`, `ApiVersion`, `ApiForeignKey`,
`ApiPrincipalKey`, `AddKey`, `WithVersion`, `ApiKeyAttribute`, and `ApiVersionAttribute`.

Schema extensions implement `IApiSchemaExtension.CreateFrozenSnapshot()`. Compilation requires a
distinct, non-null immutable snapshot assignable to the registered extension key type. The snapshot
may use thread-safe caches, but it must not expose semantic mutation. Builder and JSON state remain
exclusive and are not thread-safe; the successfully returned schema, its descendants, traversal,
lookups, serialization, relationship/key resolution, compiled property accessors, and extension
lookups are safe for concurrent use. `ApiKeyMaterializationContext` remains request-scoped and must
not be shared by concurrent operations. Caller-provided CLR objects, loggers, format providers, and
the application reference used to replace a schema remain the caller's synchronization
responsibility.

Fluent builder methods are stricter because they are explicit authoring APIs. A builder method may fail fast with standard argument exceptions when the method call itself violates its parameter contract, such as passing a `null` callback, `null` configuration object, `null` `Type`, blank name, invalid expression, or invalid extension metadata. Those precondition failures are treated as programmer errors and are separate from schema compilation diagnostics.

## Schema Authoring and Build Sessions

`ApiSchemaBuilder` retains fluent authoring state. Direct configuration callbacks execute immediately
and are not replayed by later builds. Each `BuildResult()` copies the complete builder graph into an
isolated session, recomputes discovery, annotations, conventions, and inference, then constructs and
compiles one fresh schema graph. Successful and failed sessions are discarded. Changes to conventions,
annotation results, discovery filters, or retained fluent builders affect subsequent builds.

Schema and object options callbacks retain their deferred timing. Assembly configurations are activated
once per configuration CLR type in each build, and each supported configuration role runs once even
when scans overlap. Directly supplied non-generic configuration instances remain independent declarations.
The builder does not support concurrent authoring; overlapping and reentrant builds fail immediately.
Compiled schemas retain their existing immutability and concurrent-read contracts.

## Builder Configuration Sources

All stored builder configuration sources are non-nullable `ApiConfigurationSource` values.
Defaults and untouched settings start at `Convention`; constructors that receive a configured
value and its actual source retain that source. Optional configuration is represented by its
value, such as a null builder, reference, or delegate. There is no separate unconfigured source.

Precedence is `Convention < DataAnnotation < Explicit`. Accept a setting when the incoming
source is greater than or equal to its stored source, updating the value and source together:

```csharp
if (source >= _state.NullabilitySource)
{
    _state.ApiNullability = apiNullability;
    _state.NullabilitySource = source;
}
```

Later settings at equal precedence win. Repeated registrations retain the highest source,
including when an existing builder is reused. An unregistered builder's registration source
starts at `Convention`; its owning registration path records the actual source. Merging an
optional setting requires an incoming value as well as sufficient precedence, so an absent
traversal or foreign key does not erase an existing configuration.

`ApiConfigurationSourceScope` describes the operation currently executing. It starts at
`Explicit` so ordinary fluent calls are explicit configuration. A convention or annotation
callback temporarily changes the active source, then restores the previous source in `finally`,
including when the callback throws. Stored state sources describe retained configuration and
are updated only when the corresponding setting or registration is accepted.

## API Nullability

`ApiNullability` declares whether a value permits null in a particular API context:
`Nullable = 0` or `NonNullable = 1`. It is not a flags enumeration and has no unknown state.
Properties and query-source parameters expose `ApiNullability`; collections expose
`ApiItemNullability`; query sources expose `ApiOutputNullability`. Collection nullability
and item nullability are independent, including for nested collections.

Property builders infer nullability from CLR members. Unknown CLR nullability falls back
to nullable. Use `WithNullability(ApiNullability.NonNullable)`, `AsNonNullable()`, or
`AsNullable()` for an explicit override. Argument presence remains a separate constraint.

## API Presence

`ApiPresence` is a non-flags enumeration with `Optional = 0` and `Required = 1`.
Query-source parameters expose `ApiPresence` independently of `ApiNullability` and provide
computed `IsRequired` and `IsOptional` predicates. Parameters default to required, non-nullable inputs.
Configure presence with `WithPresence(ApiPresence.Optional)`, `AsOptional()`, or `AsRequired()`.

This replaces the former Boolean argument-presence API and JSON property without compatibility aliases.
Schema JSON writes `ApiPresence` as `"Optional"` or `"Required"` (subject to naming policy);
omission defaults to required. Explicit null and invalid enum values produce compilation issues.

Properties do not yet declare universal presence. Create/update and object-input contracts will
establish their applicable requirements; query projections derive their result shapes.
Collection types retain item nullability, and query sources retain output nullability without
introducing an output-omission state. See ADR 0002 for the operation-oriented contract boundaries.

## Query Source Declarations

Register query sources explicitly; registering an object type does not expose it for querying.
For example, after registering `Customer` and the `Int32` scalar type:

```csharp
builder.AddQuerySource("CustomerById", source => source
    .AddInputParameter<int>("id")
    .WithOutput<Customer>());
```

The output is nullable by default, parameters require supplied non-null arguments by default,
and source composition defaults to projection. `WithPresence(...)`, `AsRequired()`, and `AsOptional()`
control whether an argument must be supplied. `AsNullable()` and `AsNonNullable()`
independently control whether its supplied value may be null. `WithNullability(...)` remains available
for explicit nullability.

`WithOutputComposition(...)` replaces the permitted operations; it does not execute them.
Filtering, ordering, and offset pagination require collection output, configured with
`WithCollectionOutput<TItem>()`. Collection and item nullability are independent:

```csharp
builder.AddQuerySource("Orders", source => source
    .AddInputParameter<OrderStatus>("status", parameter => parameter
        .AsOptional()
        .AsNonNullable())
    .WithCollectionOutput<Order>(
        apiCollectionNullability: ApiNullability.NonNullable,
        apiItemNullability: ApiNullability.NonNullable)
    .WithOutputComposition(ApiQueryComposition.Projection | ApiQueryComposition.Filtering));
```

Each name-only `AddQuerySource` call declares an independent source. Two declarations with the same
final API name produce a schema compilation error. Retain a `DeclareQuerySource` handle to configure
one fluent declaration repeatedly, or use a delegate type as its stable locator.

Within a pure fluent source, repeated `AddInputParameter` calls with the same original declaration name
and equal type reference configure one parameter builder, including after `WithName` renames its API
facet. A type-reference change at equal or higher precedence replaces that
parameter's settings and extensions in its original position; a lower-precedence type change
is ignored. Type-reference equality compares the declaration, so CLR-backed and API-named
references are distinct even when they resolve to the same schema type.

Configuration precedence is convention, then data annotation, then explicit configuration;
later settings at the same precedence win. `WithOutputNullability` and `WithOutputItemNullability`
configure output facets independently of its type. Parameter `WithNullability`, `WithItemNullability`,
and `WithPresence` configure separate facets. Collection-item setters require collection declarations.
The initial fluent source name is a convention candidate; `WithName` pins an explicit API name.
Extensions follow the existing builder behavior: the last registration for an extension type wins.

Reusable delegate source configurations implement `IApiQuerySourceConfiguration<TDelegate>` from
`Evoogle.ApiFramework.Schema.Configuration.Query`:

```csharp
public delegate Customer CustomerById(int id);

public sealed class CustomerByIdConfiguration : IApiQuerySourceConfiguration<CustomerById>
{
    public void Configure(ApiQuerySourceBuilder builder) => builder
        .WithName("customer")
        .ConfigureInputParameter("id", parameter => parameter.AsRequired());
}
```

Register an instance with `builder.AddQuerySource(new CustomerByIdConfiguration())`, or use
`builder.UseConfigurationsFromAssembly(typeof(CustomerByIdConfiguration).Assembly)` to discover
public implementations with public parameterless constructors. Discovered configurations run
at explicit precedence and are deduplicated by configuration type and closed role. The non-generic
`IApiQuerySourceConfiguration` remains available for independent fluent declarations. Referenced schema
types must still be registered.

```csharp
var registration = builder.DeclareQuerySource("customerSearch", source => source
    .AddInputParameter<string>("text")
    .WithCollectionOutput<Customer>());

builder.ConfigureQuerySource(registration, source => source.WithName("customers"));
builder.AddQuerySource<CustomerById>(source => source.WithName("customer"));
```

Registrations are immutable, owned by their creating builder, valid across sequential builds, and
unsupported for serialization. Passing a registration to another builder fails immediately. `Declare`
methods issue durable handles before a build; build-time discovery and conventions use `Add` methods
for declarations local to their session. Configuring an existing durable handle during a build targets
its copied declaration.

The builders have public constructors; `ApiQuerySourceParameterBuilder` accepts either an
`ApiTypeReference` or a CLR `Type` alongside its API name. `AddInputParameter`, `WithOutput`,
and `WithCollectionOutput` also accept CLR `Type` arguments as alternatives to their generic
overloads. Typed `AddQuerySourceExtension` and `AddParameterExtension` methods attach schema
extension metadata. Each schema build creates independent source, parameter, and inline-type
instances from the retained builder configuration.

`ApiSchema.ApiQuerySources` and each source's `ApiInputParameters` are immutable schema-owned collections.
Use `GetQuerySourceByApiName` / `TryGetQuerySourceByApiName` and the corresponding
`GetParameterByApiName` / `TryGetParameterByApiName` methods for ordinal, case-sensitive lookups.
Schema JSON preserves declarations and omits `ApiQuerySources` when no sources are declared.
These declarations establish schema contracts; operation compilation, argument preparation,
service bindings, and repository execution are subsequent runtime work. An operation **binds**
to a source; the source does not itself materialize a query.

Query sources may also be declared with public delegate signatures. The delegate is reflection-only
schema metadata: ApiFramework never creates an instance, assigns a method, or treats the declaration
as an execution binding. Delegates may be namespace-level or nested in a public class or static class:

```csharp
public static class CustomerQuerySources
{
    [ApiQuerySource]
    public delegate Customer? CustomerById(int id);

    [ApiQuerySource(ApiOutputComposition =
        ApiQueryComposition.Projection | ApiQueryComposition.Filtering)]
    public delegate IReadOnlyList<Customer> Customers
    (
        [ApiQuerySourceParameter(ApiPresence = ApiPresence.Optional)]
        string? name
    );
}
```

Configure `UseDefaultAnnotations()` and `UseAssemblyAnnotationScanning(...)` to discover these
declarations. Every delegate parameter becomes an input declaration; its attribute is needed only
for name, nullability, or presence overrides. CLR names and nullable metadata supply convention
candidates, while attribute values apply at data-annotation precedence and explicit fluent settings
remain authoritative. Naming conventions can independently target `QuerySource` and
`QuerySourceParameter`. Repeated discovery and generic registration of the same delegate target one
declaration. Explicitly registered delegates also read configured annotations without requiring an
assembly discovery scan. Different delegates and independent fluent registrations remain distinct, even with
equivalent signatures or names; duplicate final names produce schema compilation diagnostics.

The delegate signature fixes parameter count, order, CLR types, and output CLR type. Configure
API-facing names, nullability, argument presence, composition, and extensions through that declaration.
Use `ConfigureInputParameter` with the original CLR parameter name after any API renaming.

Annotated delegates must be public, closed, return a value, and avoid pointer or by-reference
signatures. `Task`, task-derived, `ValueTask`, and `IAsyncEnumerable`-implementing outputs are rejected
because asynchronous execution and streaming are not schema output types. Signature types must still
be registered in the schema; discovery does not expose or register them automatically. Annotation
discovery is recomputed for each build, so changing naming conventions does not retain obsolete
annotation-derived query sources.

`ApiNullability`, `ApiPresence`, and `ApiQueryComposition` are defined by the annotations assembly
so annotations-only POCO projects can use the exact schema contract enums. Their namespaces and
values are unchanged, but consumers compiled against an earlier assembly location must rebuild.

## Relationship Declaration Identity

Name-only relationship calls declare independent relationships. Keep a kind-safe registration handle
when a relationship has no navigation locator and needs later configuration:

```csharp
var registration = builder.DeclareOneToManyRelationship("CustomerOrders", relationship => relationship
    .From<Customer>()
    .To<Order>());

builder.ConfigureRelationship(registration, relationship => relationship.WithName("Purchases"));
```

Member-based declarations identify relationships through explicit CLR navigation members:

```csharp
builder.AddOneToManyRelationship<Customer, Order>(
    "CustomerOrders",
    customer => customer.Orders,
    order => order.Customer);

builder.ConfigureOneToManyRelationship<Order>(
    order => order.Customer,
    relationship => relationship.WithDeleteBehavior(ApiRelationshipDeleteBehavior.None));
```

The same member-based overloads and kind-safe handles are available for one-to-one and many-to-many
relationships. Supply `null` for an absent selector; at least one selector is required. A selector
identifies one direct public instance property or field. Use `AssociateInverseNavigation` to attach
an unclaimed opposite member to an existing member-located relationship. Member-targeted configuration
requires an existing declaration.

Member attributes also support explicit inverse association:

```csharp
[ApiRelationship(
    ApiName = "CustomerOrders",
    Kind = ApiRelationshipKind.OneToMany,
    InverseNavigation = nameof(Order.Customer))]
public List<Order> Orders { get; set; } = [];
```

`ApiManyToManyRelationshipAttribute.InverseNavigation` provides the same capability. The inverse value
names one member on the opposite CLR endpoint. Existing principal-end annotation placement rules remain
in force for one-to-one and one-to-many relationships.

Navigation locators establish authoritative CLR binding, target type, and cardinality. Traversal names
remain configurable, including through the `RelationshipTraversal` naming target. Endpoints, foreign
keys, association types, and traversal bindings added later through builders do not establish identity.
Explicit locators already owned by different relationships produce a declaration conflict and are never
merged. Independent declarations with equal final names produce the existing schema compilation error.

Migration from repeated same-name configuration requires a delegate locator, navigation locator, or
registration handle. Public builder/configuration consumers must rebuild. Schema JSON names, defaults,
metadata types, and runtime name lookups retain their contracts; registration identity is never serialized.

## Naming Conventions

Built-in API naming conventions are opt-in through `ApiSchemaBuilder` extension methods. They apply to
convention-configurable API names for types, enum values, properties, query sources, parameters, and relationship
traversals. Explicit API names remain unchanged.

| Method | Format | Example for `PersonWithId` |
| --- | --- | --- |
| `UseCamelCaseNaming()` | camelCase | `personWithId` |
| `UseKebabCaseNaming()` | kebab-case | `person-with-id` |
| `UseLowerCaseNaming()` | lower case without separators | `personwithid` |
| `UsePascalCaseNaming()` | PascalCase | `PersonWithId` |
| `UsePluralizeNaming()` | pluralized | `PersonWithIds` |
| `UseSingularizeNaming()` | singularized | `PersonWithId` |
| `UseUpperCaseNaming()` | upper case without separators | `PERSONWITHID` |

The built-in conventions use Humanizer for their transformations. Multiple naming conventions may
be registered and are applied in registration order. Each method accepts an optional
`ApiNamingConventionTargets` value. Existing casing methods default to `All`; pluralize and
singularize default to `ObjectType`. Any convention can be limited to specific schema elements,
and `None` makes that naming convention a no-op. For example:

```csharp
builder.UseKebabCaseNaming
(
    ApiNamingConventionTargets.ObjectType |
    ApiNamingConventionTargets.Property
);
```

The available targets are object types, scalar types, enum types, enum values, properties, query sources,
query-source parameters, and relationship traversals. Naming contexts have a nullable `ClrType` because
unanchored fluent declarations have no CLR locator.

Relationship traversal conventions implement `IApiRelationshipTraversalConvention` and receive an
`ApiRelationshipTraversalBuilder` plus `ApiRelationshipTraversalConventionContext`. Register them with
`UseConventions(conventions => conventions.AddConvention(...))`. They use the `Configuration` phase and
run in registration order after relationship annotations, inference, and navigation binding. Naming conventions
participate through the same traversal convention collection; query-source registration is independent.
Explicit and annotation-configured traversal names retain precedence over convention-derived names.

Lower-case and upper-case naming change the casing of the whole name without inserting separators.

### Prefix Standard: `Api`, `Clr`, and No Prefix

Use `Api` and `Clr` to mark the boundary between the framework/API-schema model and the
CLR/BCL backing model. Do not use them as general decoration, and do not add `Api` merely
because the containing type or property type starts with `Api`.

Use `Api` when a value is the API/schema-side representation of a concept that also has,
or can be confused with, a CLR/BCL-side representation. Use `Clr` for the CLR/BCL-side
representation.

```csharp
public string ApiName { get; }
public string ClrName { get; }

public string ApiPath { get; }
public string ClrPath { get; }

public Type ClrType { get; }
```

Some schema concepts are canonical `Api*` terms and should stay that way everywhere.
Examples include `ApiName`, `ApiPath`, and `ApiKind`. `ApiPath` is intentionally prefixed
because path-like values can mean schema paths, CLR member paths, JSON paths, or
file-system paths.

Use no prefix when the containing type or local scope already makes the domain clear and
there is no API-vs-CLR distinction to communicate.

```csharp
// On ApiSchemaCompilationIssue
public string ApiPath { get; }
public ApiSchemaCompilationSeverity Severity { get; }
public string Description { get; }
public string? Remediation { get; }

// On ApiKeyMaterializationContext
public ApiKeyPartNameFormat PartNameFormat { get; init; }
public ApiKeyPartNameFormatterDelegate? PartNameFormatter { get; init; }
public ApiKeyNullHandling NullHandling { get; init; }

// Computed predicates are self-describing regardless of context.
public bool IsValid { get; }
```

#### Summary

| Situation | Prefix |
| --- | --- |
| API/schema side of an API-vs-CLR boundary | `Api` |
| CLR/BCL side of an API-vs-CLR boundary | `Clr` |
| Canonical schema term such as `ApiName`, `ApiPath`, or `ApiKind` | `Api` |
| Type or containing scope already makes the domain clear | *(none)* |
| Diagnostic, formatting, option, predicate, or derived state | *(none)* |
