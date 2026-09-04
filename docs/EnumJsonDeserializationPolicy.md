# Enum Handling Policy for JSON Deserialization

## Scope

This policy defines how enum-valued properties are handled while JSON materializes a frozen
`ApiSchema`. It applies to schema metadata, not to JSON request or response payloads.

Schema JSON materialization has two distinct validation boundaries:

1. The JSON reader validates document syntax and token structure.
2. Schema compilation validates a materialized schema graph and reports expected problems as
   `ApiSchemaCompilationIssue` values.

The enum's role determines which boundary reports a well-formed JSON value that is not valid for
that enum property. A reader must not use an enum's default value as an implicit invalid-value
sentinel.

## Policy

Malformed JSON syntax always remains a `JsonException`. This includes an incomplete document and
invalid escaping. A well-formed value with an invalid enum meaning is handled according to the
following table.

| Enum role | Examples | Omitted | JSON `null` | Unknown name or incompatible token |
| --- | --- | --- | --- | --- |
| Required schema metadata | `ApiProperty.ClrMemberKind` | Compilation issue | Compilation issue | Compilation issue |
| Defaulted schema setting | `ApiTypeModifiers`, `ApiRelationshipDeleteBehavior`, `ApiSchemaOptions.ApiKeyNullHandling` | Use the documented default | Compilation issue | Compilation issue |
| Nullable override | `ApiObjectTypeOptions.ApiKeyNullHandling` | Inherit | Inherit | Compilation issue |
| Conditional reference metadata | `ApiTypeExpression.ApiKind` | Allowed for inline and CLR references; otherwise compilation issue | Allowed for inline and CLR references; otherwise compilation issue | Compilation issue |
| Structural discriminator | `ApiType.ApiKind`, `ApiRelationship.ApiKind` | `JsonException` | `JsonException` | `JsonException` |

An incompatible token is well-formed JSON whose value cannot represent the enum, such as a number,
boolean, object, array, empty or whitespace string, or an undefined enum name. For flags enums,
it also includes a combination that the enum converter does not accept.

Only an omitted property may select a default or inheritance behavior. A present `null` must never
silently become a non-nullable enum's default value. Similarly, an unknown enum name must never
silently become a default or an inherited value.

## Rationale

Schema metadata and settings belong to a concrete schema element after JSON has been read. The
framework can therefore construct that element, compile the graph, and report the problem with the
element's fully qualified path and an appropriate compilation code. This keeps expected schema
validity failures together in `ApiSchemaCompilationException`.

Discriminators are different. `ApiType.ApiKind` and `ApiRelationship.ApiKind` select the concrete
type that the reader must construct. Without a valid discriminator, there is no concrete schema
element to compile or to own an issue. Those errors remain `JsonException` values at the JSON
boundary.

`ApiTypeExpression.ApiKind` does not select a concrete materialized type. It is optional metadata
for an API named reference, so an invalid non-null value is retained as materialization state and
reported during compilation. An omitted or null value remains valid when the expression instead
contains an inline type or CLR type reference.

## Converter and Materializer Responsibilities

Use `NullableEnumJsonConverter<TEnum>` configured with
`EnumJsonInvalidValuePolicy.Throw` where the policy requires a `JsonException`, especially for
structural discriminators. `EnumJsonConverter<TEnum>` retains its existing behavior for backwards
compatibility and must not be used to enforce structural-discriminator validation.

`NullableEnumJsonConverter<TEnum>` with
`EnumJsonInvalidValuePolicy.ReturnNull` is a parsing tool for materializers that can turn an
invalid enum into an compilation issue. Its `null` result alone does not distinguish a JSON
`null`, an unknown value, an incompatible token, and an omitted property. A materializer that
supports any of those distinctions must retain the property's presence and parsing state in
addition to the nullable enum value. `JsonConverterBase.ReadJsonObject` supports this narrowly:
the materializer must opt in only the JSON property names whose null values it needs to inspect.

`ApiProperty.ClrMemberKind` is the first use of this policy. It is required metadata whose only
concrete values are `Property` and `Field`. JSON materialization may hold a nullable backing value;
schema compilation reports `ApiPropertyInvalidClrMember` when that value is absent or invalid.
After successful compilation, `ClrMemberKind` is non-null and forms, together with `ClrName`,
the authoritative CLR member-binding identity.

Do not introduce an `Unknown` enum member or map a conversion failure to `default(TEnum)`. Both
make an invalid wire value indistinguishable from a potentially meaningful enum value.

## Adoption and Tests

This policy is the required standard for new or changed schema enum readers. Existing strict enum
readers may predate it; migrate them deliberately, preserving their property's omitted, `null`,
default, and inheritance semantics rather than applying a blanket nullable converter.

For every enum reader changed to follow this policy, cover a valid value, an omitted property, JSON
`null`, an unknown name, and an incompatible well-formed token. Assert both the failure channel
(`ApiSchemaCompilationException` or `JsonException`) and the externally visible default or
inheritance behavior where applicable.
