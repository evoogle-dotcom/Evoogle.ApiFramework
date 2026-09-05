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
- Use `AddRequiredProperty` or `AddOptionalProperty` only when the API contract should override CLR nullable reference type inference.
- Configure larger schemas with `IApiObjectTypeConfiguration<T>` and relationship configuration classes when inline lambdas become too large.
- Use relationship shortcuts for common cases, or the full relationship builders when you need named principal keys, composite keys, or extensions.

```csharp
var schema = new ApiSchemaBuilder()
    .WithName("Commerce")
    .WithVersion("v1")
    .WithOptions(o => o.ThrowOnNullKeyPart())
    .AddScalar<Guid>()
    .AddScalar<string>()
    .AddEnum<OrderStatus>(e => e.AddAllValues())
    .AddObject<Customer>(o => o
        .AddProperty(c => c.Id)
        .AddProperty(c => c.Name)
        .AddProperty(c => c.Orders)
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
    .WithOptions(options => options.UseDefaultOnNullKeyPart())
    .AddProperty(c => c.Id)
    .AddKey("PK_Customer", c => c.Id))
```

`Build()` compiles, validates, freezes, and returns the schema. A returned schema is immutable and
safe for concurrent runtime reads.

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
`ApiPrincipalEndA` and `ApiForeignKeyTypeB`; ordered key paths and segments include their
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
domain discriminators. `ApiKeyType` is the built-in anonymous key representation and
`ApiNamedKeyType` reports `NamedKeyType`; callers cannot construct or derive new key metadata
outside the framework assembly.

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

## Naming Conventions

Built-in API naming conventions are opt-in through `ApiSchemaBuilder` extension methods. They
apply to convention-configurable schema type, enum-value, and property API names. Explicit API
names remain unchanged.

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

The available targets are object types, scalar types, enum types, enum values, and properties.
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
because path-like values can mean schema paths, CLR property paths, JSON paths, or
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
