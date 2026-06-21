# Repository

This repository is home to the following **Evoogle** projects. These projects are maintained by [Evoogle](https://github.com/evoogle-dotcom) and licensed under the [MIT License](License.txt).

- [Evoogle.ApiFramework](#Evoogle.ApiFramework)

## Evoogle.ApiFramework

TBD: Need a high-level description.

- **Schema**. Library that represents an API schema where a schema defines a type system that describes what data can be queried or mutated from an API.

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

`Build()` initializes and validates the schema.

### Validation and Error Reporting

Schema validation is centralized in `ApiSchema.Initialize()`. `Build()` and JSON deserialization construct as much schema state as they can, run initialization, and collect all initialization issues before deciding whether the schema is usable. They do not stop at the first schema issue. If initialization records one or more errors, the framework throws one `ApiSchemaInitializationException` that packages the full `ApiInitializationResult`, including errors and warnings. Consumers can catch the exception and inspect `Issues`, `Errors`, or `Warnings` to produce a complete report.

This aggregation applies to schema validity problems such as duplicate API names, unresolved CLR types, invalid key paths, missing properties, invalid relationship definitions, and other whole-schema consistency issues. Malformed JSON or incompatible JSON token shapes may still be rejected by the JSON serializer before schema initialization runs.

Fluent builder methods are stricter because they are explicit authoring APIs. A builder method may fail fast with standard argument exceptions when the method call itself violates its parameter contract, such as passing a `null` callback, `null` configuration object, `null` `Type`, blank name, invalid expression, or invalid extension metadata. Those precondition failures are treated as programmer errors and are separate from schema initialization diagnostics.

## Naming Conventions

### Property Prefix Standard: `Api` and `Clr`

ApiFramework uses two explicit property prefixes to express which abstraction level a property belongs to. The motivation is disambiguation: many types in the framework have a direct correspondence between an ApiFramework-level concept and a CLR/BCL-level backing concept. Without prefixes, it is impossible to tell them apart at a glance.

The canonical example is `ApiType`, which exposes both `ApiKind` (the framework's type classification) and `ClrType` (the `System.Type` backing it). The prefix makes the boundary immediately clear.

#### When to use `Api`

Use the `Api` prefix when:
- the property's type is itself an `Api*` type, **or**
- the same concept has a competing CLR/BCL counterpart on the same class, **or**
- the concept is consistently expressed as `Api*` across the type system (prefer consistency over strict necessity in this case).

```csharp
public ApiTypeKind ApiKind { get; }       // Api prefix — type is an Api* type, and Kind exists at both levels
public ApiType?    ApiInlineType { get; } // Api prefix — type is an Api* type
public string?     ApiName { get; }       // Api prefix — string, but "name" is universally ApiName across the type system
```

The third case matters: even when a `string` property wouldn't strictly require disambiguation, if the same concept is expressed as `ApiName` (or `ApiVersion`, etc.) everywhere else in the codebase, use the prefix for consistency. A reader should never encounter the same concept under two different names just because one instance happened to live on a simpler type.

#### When to use `Clr`

Use the `Clr` prefix when the property explicitly represents the CLR-level backing of an ApiFramework-level concept on the same class.

```csharp
public Type   ClrType     { get; } // Clr prefix — the System.Type that backs the ApiType concept
public string ClrName     { get; } // Clr prefix — the CLR member name that backs the Api name
public Type?  ClrScalarTypeHint { get; } // Clr prefix — a CLR type hint backing an Api identity part
```

#### When to use no prefix

Do not prefix a property when no disambiguation is needed — that is, when the enclosing class is already unambiguously at one abstraction level and the property does not have a competing counterpart at the other level.

```csharp
// On ApiInitializationIssue — there is no CLR counterpart to Path, Description, or Severity
public string                  Path        { get; }
public ApiInitializationSeverity Severity  { get; }
public string                  Description { get; }

// Computed predicates are self-describing regardless of context
public bool IsValid { get; }
```

#### Summary

| Situation                                                       | Prefix                     |
| --------------------------------------------------------------- | -------------------------- |
| Property type is an `Api*` type                                 | `Api`                      |
| Same concept exists at both Api and CLR level on the same class | `Api` or `Clr` accordingly |
| Concept is universally `Api*`-prefixed across the type system   | `Api`                      |
| Property is the CLR backing of an Api concept                   | `Clr`                      |
| No ambiguity — concept only exists at one level                 | *(none)*                   |
| Computed predicate or derived value                             | *(none)*                   |
