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
// On ApiInitializationIssue
public string ApiPath { get; }
public ApiInitializationSeverity Severity { get; }
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
