# Distinguish Contained Values from Relationship Traversals

## Status

Accepted on 2026-09-16 for the operation runtime alpha. The schema metadata, builders,
compilation, and JSON foundation were implemented on 2026-09-16. Service retrieval and client
materialization remain part of the operation runtime alpha. Reusable CLR-member and API-property
reference models were adopted on 2026-09-19.

## Context and Problem Statement

[ADR 0002](0002-use-an-operation-oriented-api-runtime.md) establishes schema-driven relationship
retrieval and nested query inclusion. It does not distinguish an object value contained in another
object from an independently identifiable object reached through a relationship. Both can have an
`ApiObjectType` shape and both can appear nested in a response.

A CLR member such as `Order.Customer` may also represent a related object. Its presence alone does
not say whether the member is loaded, whether a null value means no customer, or whether the member
is part of the order's own data. The schema must express these meanings independently of JSON:API,
GraphQL, gRPC, and CLR navigation conventions.

## Decision Drivers

- Reuse one named `ApiObjectType` in contained and independently identifiable roles.
- Keep first-class `ApiRelationship` metadata authoritative for related objects.
- Allow relationships to be traversed without CLR navigation members.
- Use repository-populated navigation members when their loaded state is known.
- Materialize selected relationships into client POCOs when a CLR binding exists.
- Preserve the distinction between an unselected relationship and a selected empty relationship.
- Keep provider representations separate from schema meaning.

## Decision Outcome

An object-typed `ApiProperty` represents a contained value. An `ApiRelationshipTraversal` represents
an API-visible direction through a first-class `ApiRelationship`. A traversal is not an
`ApiProperty`, and it may optionally bind to a CLR navigation member on its source type.

```text
Named ApiObjectType: Person

Order
├── ContactPerson: Person       contained ApiProperty
└── customer → Person           ApiRelationshipTraversal
                                optional CLR binding: Order.Customer

AnotherOrder
└── ContactPerson: Person       another contained use of the same named type
```

The role belongs to each use of `Person`; it is not determined solely by the `Person` type.
Whether a type expression is inline or refers to a named type is a separate choice. A reusable
contained type can be named and referenced from multiple properties.

## Schema Semantics

### Contained object values

A contained object is part of its enclosing object's data. Its `ApiProperty` may resolve to an
`ApiObjectType` directly or through a collection item type. The enclosing object's repository
supplies that value with the enclosing object; an optional property may be null. The runtime does
not infer a relationship traversal merely because the property has an object type.

Containment here describes the API value boundary. It does not by itself prescribe database
ownership or prohibit the same named object type from being used elsewhere as a relationship
target.

### Relationship traversals

An `ApiRelationshipTraversal` exposes one named direction from a source `ApiObjectType` to a target
`ApiObjectType` through an `ApiRelationship`. The relationship remains the authority for the
association. Each exposed direction has its own name and may have its own optional CLR binding;
principal and dependent key roles do not alone determine which directions are exposed.
Each relationship end owns at most one traversal. An end without a traversal does not expose that
direction; an object type can still expose traversals from multiple distinct relationships.

The traversal is the API field selected by a query. An optional CLR navigation member is a binding
to that traversal, not a second API field or an `ApiProperty`. A traversal can exist without any
corresponding CLR member. Where a CLR member exists, the schema configuration identifies the
binding explicitly rather than inferring it from matching names alone.

```text
With a CLR binding:                 Without a CLR binding:
Order --customer--> Person          Order --customer--> Person
  Order.Customer → traversal          Order POCO has no Customer member
```

This use of *navigation member* means a CLR member bound to a traversal. Existing schema usage of
*navigational relationship* to mean a relationship without a foreign-key binding is a separate
concept; it does not imply that a CLR member exists.

### Reference and binding model

Schema components distinguish a configured identity from the target resolved during compilation:

- `ApiClrMemberReference` identifies one public instance CLR property or field by the required pair
  `ClrName` and `ClrKind`. A traversal owns either one complete reference or no reference;
  absence means the traversal has no CLR navigation binding.
- `ApiPropertyReference` identifies a declared `ApiProperty` by exactly one of its API name or CLR
  name. The reference does not repeat `ClrMemberKind`; that is declaration metadata owned by the
  resolved `ApiProperty`.
- Internal one-shot bindings retain configured references and their resolved targets. Resolution
  occurs during schema compilation, and a failed resolution consumes the binding attempt just as a
  successful resolution does.

This follows the existing `ApiTypeReference` and `ApiTypeBinding` split. CLR-member references are
used when a consumer binds directly to POCO structure. API-property references are used when a
consumer refers to schema-declared property metadata. Consequently, relationship traversals use
CLR-member references, while key-path segments and property-backed version definitions use
API-property references.

Key paths may mix API-name and CLR-name property references. After successful compilation,
`ApiKeyPath.ClrPath` is derived from the resolved properties' CLR names, regardless of how each
segment was configured. Compact `ClrPath` JSON is used only when every segment was configured by
CLR name and no segment extension requires detailed output; API-name or mixed paths use detailed
`ApiSegments` JSON so reference identity survives a round trip.

## Service Retrieval

The runtime has two ways to fulfill a selected traversal:

1. Use a CLR navigation value supplied with the source object when the repository explicitly reports
   that the traversal is loaded. A loaded null to-one value means no target; a loaded empty to-many
   value means no targets. The runtime validates identities and incorporates supplied objects into
   the operation's identity map.
2. When the traversal is not loaded, obtain target identities from the relationship repository and
   retrieve missing target objects by key through their object repository, following ADR 0002's
   batch-first execution model.

An unpopulated or null CLR member without a loaded-state guarantee does not establish that the
relationship is empty. A non-null CLR member without that guarantee does not override the
relationship repository. The loading contract is per repository result or retrieval context, not
an assumption made from the member value. Both retrieval paths must produce the same canonical
relationship result.

The exact loaded-state representation, repository signatures, identity validation, and precedence
when multiple sources supply a target are implementation-level design for the alpha.

## Query Results and Client Materialization

An `ApiQuery` selects contained properties and relationship traversals as distinct projection
elements. The result shape and canonical data preserve whether a traversal was selected, whether a
selected traversal has no target, and which related objects were returned. A query does not decide
whether an object is contained or related; the schema supplies that meaning.

When the client requests a POCO result, ApiFramework materializes the returned object graph. If a
selected traversal has a CLR binding, client materialization assigns the returned related object or
objects to that member. Application code receives the populated POCO; it does not perform the
assignment.
An unselected traversal must not be interpreted as an empty relationship merely because its CLR
member has a default or null value.

Providers translate the selected canonical result into their protocol representations. JSON:API
may express related resources through relationship linkage and an `included` array; GraphQL and
gRPC may nest related objects in their result representations. These shapes do not change the
schema distinction between containment and relationship traversal.

## Consequences and Alpha Evidence

- Relationship traversals become first-class schema metadata, separate from `ApiProperties`.
- Object types can be reused across contained and related roles without duplicating definitions.
- Repositories can avoid relationship mapping and object fetches when they explicitly supply loaded
  navigation values, while relationships remain executable without CLR navigation members.
- Schema compilation validates traversal names, source and target types, directions, and optional
  CLR bindings. The loaded-state representation and runtime materialization APIs remain for the
  operation runtime alpha.
- The alpha should demonstrate one contained `Person` value, a loaded `customer` traversal, and
  the same traversal resolved through relationship mapping and object retrieval. It should verify
  equivalent canonical results and client POCO materialization for a selected traversal.

