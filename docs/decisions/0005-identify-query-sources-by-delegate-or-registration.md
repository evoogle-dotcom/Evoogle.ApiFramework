# Identify Query Sources by Delegate or Registration

## Status

Accepted and implemented on 2026-11-01.

## Context and Problem Statement

[ADR 0002](0002-use-an-operation-oriented-api-runtime.md) defines `ApiQuerySource` as schema-owned
metadata independently of service execution. [ADR 0004](0004-separate-schema-authoring-from-build-sessions.md)
separates durable authoring from build-local symbols and prohibits mutable API names from serving as
declaration identity.

An annotated query source has a natural CLR declaration: its delegate type. A pure fluent query
source has no corresponding CLR type. Requiring a delegate for every fluent declaration would add
artificial ceremony, while continuing to key every builder by API name would make naming conventions
and explicit renaming unsafe.

## Decision Drivers

- Use the delegate type when a natural CLR declaration exists.
- Preserve pure fluent query-source authoring without requiring a delegate.
- Keep `AddQuerySource` convenient for the dominant use case.
- Allow annotations and explicit configuration to target the same declaration safely.
- Infer delegate input and output declarations consistently.
- Preserve configuration-source precedence and source/parameter ordering.
- Permit multiple query sources with identical CLR signatures when they are distinct declarations.
- Keep query-source execution bindings outside schema declaration identity.

## Considered Options

1. Continue keying query-source builders by API name.
2. Require every query source to have a distinct CLR delegate type.
3. Use user-maintained marker types or global enum identifiers.
4. Use a delegate-type locator when present and an opaque registration otherwise.

## Decision Outcome

Choose option 4. Every query source resolves to one build-session query-source symbol. A
delegate-backed declaration is located by its CLR delegate type. A pure fluent declaration is
located by an opaque durable registration.

```text
Delegate-backed declaration:
    typeof(CustomerById) ──> QuerySourceSymbol

Pure fluent declaration:
    QuerySourceRegistration ──> QuerySourceSymbol
```

One delegate type identifies at most one query source within one schema. Independent fluent
registrations can declare multiple sources with equivalent input and output signatures.

## Delegate-Backed Authoring

Given:

```csharp
public delegate Customer CustomerById(Guid id);
```

Explicit generic registration uses the delegate type as locator and does not require an attribute:

```csharp
schemaBuilder.AddQuerySource<CustomerById>();
```

The framework reflects the delegate's `Invoke` method to infer input parameters, output type,
nullability candidates, and ordering. The delegate type name and CLR parameter names are convention
naming candidates.

The delegate signature is authoritative declaration structure. Configuration cannot replace the
delegate output CLR type, add or remove parameters, reorder parameters, or change parameter CLR
types. Such a contribution is incompatible with the declaration and produces a configuration
diagnostic.

Configuration may override API-facing facets of the reflected declaration:

- Query-source and parameter API names.
- Output, parameter, and collection-item nullability where supported.
- Parameter argument presence.
- Output composition permissions.
- Extension metadata.

Pure fluent registration remains structurally configurable because it has no delegate signature.

An optional explicit callback configures the same declaration:

```csharp
schemaBuilder.AddQuerySource<CustomerById>
(
    source => source
        .WithName("customer")
        .WithOutputComposition(ApiQueryComposition.Projection)
);
```

Repeated generic calls resolve the same symbol because the delegate type is stable:

```csharp
schemaBuilder.AddQuerySource<CustomerById>(source => source.WithName("customer"));
schemaBuilder.AddQuerySource<CustomerById>
(
    source => source.WithOutputComposition(ApiQueryComposition.Projection)
);
```

## Annotation Discovery

`ApiQuerySourceAttribute` remains the opt-in mechanism for assembly annotation discovery:

```csharp
[ApiQuerySource]
public delegate Customer CustomerById(Guid id);
```

Configured annotation readers also apply to explicitly registered delegates without requiring an
assembly discovery scan. The attribute remains optional for explicit generic registration.

Annotation discovery and generic fluent configuration resolve through `typeof(CustomerById)` and
therefore contribute to the same symbol. Structurally similar delegates remain distinct because
their CLR types differ.

If two annotation readers contribute to the same delegate, their contributions target the same
symbol. Equivalent contributions are idempotent; incompatible contributions are annotation
configuration conflicts rather than separate query sources.

## Pure Fluent Authoring

Name-only fluent authoring creates a new registration-backed declaration on every call:

```csharp
schemaBuilder.AddQuerySource
(
    "customerSearch",
    source => source
        .AddInputParameter<string>("text")
        .WithCollectionOutput<Customer>()
);
```

Conceptually, `AddQuerySource` calls `DeclareQuerySource` and discards the returned registration.
Advanced callers can retain the handle:

```csharp
var registration = schemaBuilder.DeclareQuerySource
(
    "customerSearch",
    source => source.WithCollectionOutput<Customer>()
);

schemaBuilder.ConfigureQuerySource
(
    registration,
    source => source.AddInputParameter<string>("text")
);
```

Two name-only calls with the same name create two symbols. The declarations are not merged; final
schema compilation reports the duplicate API name.

## Naming and Configuration Precedence

`ApiQuerySourceBuilder` exposes precedence-aware explicit name configuration such as `WithName`.
Name resolution follows:

```text
Delegate type or fluent candidate    Convention
ApiQuerySourceAttribute override     DataAnnotation
WithName override                    Explicit
```

Parameter naming follows the corresponding CLR parameter, annotation, and explicit precedence.
Changing a name never changes the delegate locator or registration.

## Configuration Interfaces and Discovery

A generic configuration interface targets a delegate-backed declaration:

```csharp
public sealed class CustomerByIdConfiguration :
    IApiQuerySourceConfiguration<CustomerById>
{
    public void Configure(ApiQuerySourceBuilder builder)
    {
        builder.WithName("customer");
    }
}
```

Assembly discovery identifies this contribution through the delegate type. A discovered
non-generic query-source configuration declares a registration-backed source and is idempotent by
configuration CLR type and query-source role within the build session.

A directly supplied non-generic configuration instance creates an independent registration. This
allows multiple instances of one parameterized configuration type to declare distinct sources.

## Add and Configure Semantics

- `AddQuerySource<TDelegate>` gets or creates the symbol located by `TDelegate` and applies the
  supplied contribution.
- Name-only `AddQuerySource` always declares a new registration-backed source.
- `DeclareQuerySource` declares a registration-backed source and returns its opaque handle.
- `ConfigureQuerySource` with a registration handle configures that declaration.
- Contributions targeting a delegate use the delegate type rather than API name.

The registration handle is immutable, builder-scoped, non-serializable, and valid across sequential
builds from its owning builder.

## Validation and Diagnostics

The configuration and compilation workflow diagnoses at least:

- A generic query-source type that is not a supported public closed delegate.
- An invalid delegate input or output signature.
- An attempt to replace or extend the authoritative structure of a delegate-backed source.
- Incompatible contributions targeting one delegate symbol.
- A registration handle used with a different schema builder.
- Duplicate final query-source API names across independent symbols.

Final name duplication remains an `ApiSchema` compilation concern and never causes declaration
merging.

## Compatibility

- Existing name-only `AddQuerySource` syntax remains available and convenient.
- Repeated name-only calls no longer configure one builder; they declare independent sources.
- Delegate-generic authoring and opaque registration handles add new public authoring surfaces.
- `ApiQuerySourceBuilder` gains explicit name configuration.
- Configuration and annotation-reader contracts may require coordinated source and binary changes.
- `ApiQuerySource`, query operation binding, JSON, defaults, ordering, and runtime lookup contracts
  remain unchanged.

## Consequences

### Positive

- Delegate-backed declarations have stable, reflection-rich CLR identity.
- Pure fluent declarations remain concise and do not require artificial delegate or marker types.
- Annotation and explicit contributions converge without relying on API names.
- Naming conventions can safely recompute source and parameter names on every build.

### Negative

- Name-only repeated configuration must move to a delegate locator or registration handle.
- One delegate type cannot represent multiple sources in one schema.
- Query-source builder and configuration interfaces require coordinated evolution.

### Neutral and Follow-up Work

- Exact registration-handle type names remain implementation-level decisions, but handles must be
  query-source-specific and builder-scoped.
- Service execution delegates and binding remain outside this decision under ADR 0002.

## Confirmation

Implemented and covered by the [query-source declaration regression suite](../../Tests/Evoogle.ApiFramework.Tests/Schema/Configuration/Query/ApiQuerySourceDeclarationTests.cs).
Final validation passed 1,664 runtime tests and 117 annotation tests with no skips. Production and test
builds completed with zero warnings and errors; diff, UTF-8 BOM, CRLF, and final-newline checks passed.

The decision is confirmed when tests demonstrate:

- Generic registration infers delegate inputs, output, nullability, and ordering.
- Delegate-backed configuration cannot change parameter count, order, CLR types, or output CLR
  type.
- Delegate-backed configuration can override supported API-facing facets at the applicable
  precedence.
- Annotation and generic explicit contributions target one symbol with correct precedence.
- Repeated generic contributions configure one symbol.
- Name-only calls create independent symbols.
- Registration handles configure their durable declaration across sequential builds.
- Different delegate types with equivalent signatures remain distinct.
- Naming changes recompute without retaining obsolete query sources or parameter names.
- Duplicate final names produce compilation diagnostics without symbol merging.
- Assembly discovery is idempotent by configuration type and role.
- Repository-standard build, test, diff, encoding, and line-ending verification succeeds.
