# Naming Standards

This document defines naming standards for `Evoogle.ApiFramework`. It builds on the
repository `.editorconfig`, the public README guidance, and the Google C# style guide.

## Baseline C# Naming

Follow the repository `.editorconfig` first. When a rule is not explicitly covered here,
use the Google C# naming baseline:

- Public APIs, types, namespaces, methods, properties, events, delegates, enum types, and
  enum members use `PascalCase`.
- Interfaces use `I` + `PascalCase`, such as `IApiObjectTypeConfiguration`.
- Local variables and parameters use `camelCase`.
- Non-public instance fields use `_camelCase`.
- File and directory names use `PascalCase` where practical, and a source file should
  usually match the main type it contains.
- Treat acronyms as words in identifiers. Use `ApiSchema`, `ApiName`, `ClrType`, and
  `MyRpc`, not `APISchema`, `APIName`, `CLRType`, or `MyRPC`.

In prose, use normal English acronyms such as "API" and "CLR" when referring to the
concept generally. In code identifiers, use `Api` and `Clr`.

## `Api` and `Clr` Prefixes

ApiFramework models two related naming spaces:

- `Api`: the framework/API schema abstraction.
- `Clr`: the .NET CLR/BCL backing representation.

Use these prefixes to mark the boundary between the framework/API-schema representation
and the CLR/BCL representation. They are not general domain prefixes and should not repeat
information already supplied by the containing type, parameter type, or local scope.

### Use `Api`

Use `Api` when a member names the API/schema-side representation of a concept that also
has, or can easily be confused with, a CLR/BCL-side representation:

```csharp
public string ApiName { get; }
public string ApiPath { get; }
public ApiType? ApiInlineType { get; }
public ApiKeyType? ApiPrincipalKeyType { get; }
```

Use `Api` for canonical framework terms that are intentionally named that way across the
model, especially when a shorter name would create a second spelling for the same concept:

```csharp
public string ApiName { get; }
public string ApiPath { get; }
public ApiTypeKind ApiKind { get; }
```

Do not add `Api` merely because the containing type or property type starts with `Api`.
Name the member by its semantic role. If the role is already clear in context, use the
shorter name.

```csharp
public IEnumerable<ApiInitializationIssue> Issues { get; }
public ApiKeyPartNameFormat PartNameFormat { get; init; }
public ApiKeyNullHandling NullHandling { get; init; }
```

### Use `Clr`

Use `Clr` when a member explicitly represents the CLR/BCL backing of an ApiFramework
concept.

```csharp
public Type ClrType { get; }
public string ClrName { get; }
public Type ClrObjectType { get; }
public string ClrPropertyName { get; }
```

Parameter names mirror the same distinction in `camelCase`:

```csharp
public ApiObjectTypeBuilder AddObject(Type clrType);
public ApiPropertyBuilder(string apiName, string clrName);
public ApiKeyPathBuilder AddPath(Type clrRootType, params string[] clrPropertyNames);
```

Use `Clr` only when the value really is the CLR-side representation. Do not use `Clr` for
general runtime values, parsed values, API names, or schema concepts.

### Use No Prefix

Do not add `Api` or `Clr` when no cross-level ambiguity exists. The enclosing type and the
member name should already make the meaning clear.

```csharp
public ApiInitializationSeverity Severity { get; }
public string Description { get; }
public bool IsValid { get; }
public bool HasErrors { get; }
public ApiKeyPartNameFormat PartNameFormat { get; init; }
public ApiKeyNullHandling NullHandling { get; init; }
```

Computed predicates and state flags should stay natural and self-describing:

```csharp
public bool IsComposite { get; }
public bool HasWarnings { get; }
public bool IsNavigational { get; }
```

## Paired Names

When both API-level and CLR-level names appear on the same type, name them as an explicit
pair:

```csharp
public string ApiName { get; }
public string ClrName { get; }
```

Use paired prefixes for methods and parameters as well:

```csharp
public ApiObjectTypeBuilder AddProperty(string apiName, string clrName);
public ApiEnumTypeBuilder AddValue(string apiName, string clrName, int clrOrdinal);
```

Avoid vague unpaired names in this situation:

```csharp
// Avoid when both API and CLR names are in play.
public string Name { get; }
public string PropertyName { get; }
```

## Domain Terminology

Use framework terminology consistently.

- Use `ApiName` for schema-visible names.
- Use `ApiPath` for schema element paths because path-like values can otherwise mean
  schema paths, CLR property paths, JSON paths, or file-system paths.
- Use `ClrName`, `ClrType`, `ClrObjectType`, and `ClrPropertyName` for CLR-side backing
  information.
- Use `PrincipalKey` for relationship principal-end key terminology, not `PrimaryKey`.
- Use `ForeignKey` for dependent/association-side key terminology.
- Use `Extension` for extension metadata values. Generic parameters should be named
  `TExtension`, and variables/parameters should be named `extension`.

Do not rename user-defined data to satisfy terminology preferences. For example,
`AddKey("PK_Customer", ...)`, `AddKey("PrimaryKey", ...)`, and JSON values supplied by a
consumer are data, not framework member names.

## JSON and Documentation Names

Serialized property names should normally match the public API property names, including
`Api` and `Clr` casing. Use a different JSON name only for an intentional compatibility or
format-design reason.

XML documentation should use the same member names and terminology as the API surface.
Do not document `ApiName` as just "name" when the distinction from `ClrName` matters.

## Quick Reference

| Situation | Standard |
| --- | --- |
| Public type, method, property, enum member | `PascalCase` |
| Interface | `I` + `PascalCase` |
| Local variable or parameter | `camelCase` |
| Non-public instance field | `_camelCase` |
| Acronym inside identifier | Treat as a word: `Api`, `Clr`, `Rpc` |
| API/schema side of an API-vs-CLR boundary | Prefix with `Api` |
| Canonical schema term such as `ApiName` or `ApiPath` | Prefix with `Api` |
| CLR/BCL backing concept | Prefix with `Clr` |
| No API/CLR ambiguity | No prefix |
| Relationship principal key terminology | `PrincipalKey` |
| Extension metadata generic parameter | `TExtension` |

## References

- [Google C# Style Guide](https://google.github.io/styleguide/csharp-style.html)
- [Repository README](../ReadMe.md)
