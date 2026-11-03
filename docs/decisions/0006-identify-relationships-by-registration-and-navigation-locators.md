# Identify Relationships by Registration and Navigation Locators

## Status

Accepted and implemented on 2026-11-01.

## Context and Problem Statement

[ADR 0003](0003-distinguish-contained-values-from-relationship-traversals.md) defines relationships
as first-class schema metadata and CLR navigation members as optional traversal bindings.
[ADR 0004](0004-separate-schema-authoring-from-build-sessions.md) separates durable authoring from
build-local symbols and prohibits mutable API names from serving as declaration identity.

A relationship has no single universal CLR type. The same endpoint types may participate in several
logical relationships, and one relationship may be represented by principal and dependent
navigations, foreign-key paths, an association object, or no CLR members at all. Those facts can
describe relationship structure, but they do not provide one safe universal key.

The framework needs deterministic relationship identity without marker types, global enums,
API-name merging, or structural guessing.

## Decision Drivers

- Support relationships with zero, one, or two CLR navigation members.
- Keep distinct relationships between the same CLR endpoint types separate.
- Allow multiple contributions to target one relationship through stable CLR members.
- Preserve repository-backed relationships without CLR navigation members.
- Avoid treating mutable foreign-key or traversal configuration as identity.
- Diagnose ambiguity instead of guessing or automatically merging symbols.
- Keep ordinary name-first `AddXXXRelationship` usage concise.
- Preserve `Convention < DataAnnotation < Explicit` configuration precedence.

## Considered Options

1. Continue keying relationship builders by API name.
2. Use endpoint types, foreign keys, or association types as structural keys.
3. Require marker types or user-maintained global enum identifiers.
4. Automatically merge provisional relationships that appear structurally compatible.
5. Give every declaration an opaque registration and optionally attach explicit navigation
   locators.

## Decision Outcome

Choose option 5. Every relationship declaration resolves to one build-session relationship symbol
through an opaque registration or explicitly supplied CLR navigation locators.

```text
Member-backed relationship:

    Customer.Orders ─┐
                     ├── RelationshipSymbol
    Order.Customer ──┘

Unbound relationship:

    RelationshipRegistration ──> RelationshipSymbol
```

One symbol may own multiple locators. One navigation locator may resolve to at most one symbol in a
schema.

## Locator Resolution

When a declaration or configuration contribution supplies navigation locators, the build session
resolves them as follows:

```text
No locator is claimed
    Create a new relationship symbol.

All claimed locators resolve to one symbol
    Configure that symbol.

Some locators are unclaimed and the rest resolve to one symbol
    Attach the unclaimed locators to that symbol.

Locators resolve to different existing symbols
    Report a declaration conflict; never merge the symbols.
```

For example:

```csharp
schemaBuilder.AddOneToManyRelationship
(
    "CustomerOrders",
    customer => customer.Orders,
    order => order.Customer,
    relationship => /* configure */
);
```

creates or resolves one symbol owning both `Customer.Orders` and `Order.Customer`. A later
member-targeted contribution through either locator configures the same symbol.

## Registration-Backed Relationships

A name-only fluent declaration has no natural CLR locator and always creates a new registration:

```csharp
schemaBuilder.AddOneToManyRelationship
(
    "CustomerOrders",
    relationship => relationship
        .From<Customer>()
        .To<Order>()
);
```

Conceptually, each `AddXXXRelationship` method calls its corresponding `DeclareXXXRelationship`
method and discards the returned handle. Advanced callers can retain a kind-safe handle:

```csharp
var registration = schemaBuilder.DeclareOneToManyRelationship
(
    "CustomerOrders",
    relationship => relationship
        .From<Customer>()
        .To<Order>()
);

schemaBuilder.ConfigureRelationship
(
    registration,
    relationship => relationship.WithDeleteBehavior(/* behavior */)
);
```

Two name-only calls create two relationship symbols even when their names and structures match.
Duplicate final names are reported during schema compilation.

## Navigation Locators

A CLR member becomes a declaration locator only when explicitly supplied through:

- A member-based relationship declaration.
- A member annotation.
- A member-targeted configuration operation.
- An explicit inverse-locator association.

Relationship configuration supplied later through endpoint, traversal, foreign-key, or association
builders does not silently re-key the declaration.

An unclaimed inverse member may be attached to an already resolved symbol through an explicit
operation. If it already resolves to another symbol, the operation reports a declaration conflict.

### Navigation members are authoritative CLR structure

A navigation member supplied as a declaration locator also establishes an immutable CLR traversal
binding. Its declaring CLR type, target CLR type, and to-one or to-many cardinality constrain the
relationship end and kind.

For example, `Customer.Orders` as `ICollection<Order>` establishes a to-many traversal from
`Customer` to `Order`, while `Order.Customer` establishes a to-one traversal from `Order` to
`Customer`. A contribution cannot later reinterpret `Customer.Orders` as a to-one traversal or
change its target to another CLR type.

The CLR member name is the convention candidate for the traversal API name. Naming conventions and
explicit traversal naming may change that API-facing name without changing the CLR locator or
binding.

A relationship that should not bind a CLR navigation member uses a registration-backed declaration
instead of a member-located declaration.

## Annotations

A member annotation naturally locates its relationship declaration by declaring CLR type and
member identity. Explicit member-targeted configuration can therefore contribute to the same symbol
without knowing its API name.

Separate annotations on different members are independent declarations unless one declaration
explicitly associates the members as inverse locators. Equal API names do not merge them.

A type-level relationship-definition annotation has no CLR navigation locator. Each occurrence is
a registration-backed declaration. Assembly annotation scanning recreates it within each isolated
build session; it is not retained in durable authoring state.

Traversal naming participates through `IApiRelationshipTraversalConvention` and the convention pipeline's
dedicated traversal schedule. Traversal conventions run at convention precedence after relationship inference
and navigation binding, using `ApiRelationshipTraversalBuilder` and CLR/relationship context. Navigation
validation does not perform naming or inspect another schema element's convention collection.

### Explicit inverse annotation configuration

Member-level `ApiRelationshipAttribute` and `ApiManyToManyRelationshipAttribute` expose optional
`InverseNavigation` properties. The value is one public instance property or field name on the
opposite endpoint. It is resolved and validated during annotation discovery; blank, nested,
unresolved, or structurally incompatible references produce configuration diagnostics.

Existing principal-end annotation placement remains unchanged for one-to-one and one-to-many
relationships. Fluent `AssociateInverseNavigation` provides explicit later attachment. Member-targeted
configuration methods require an already declared relationship.

## Values That Are Not Identity Locators

The following remain configurable relationship structure and never implicitly establish declaration
identity:

- Relationship API name.
- Principal and dependent CLR endpoint types.
- Principal-key selection.
- Foreign-key paths.
- Association CLR type and association foreign keys.
- Delete behavior.
- Traversal API names.
- CLR traversal bindings supplied only as later builder configuration.

Endpoint and key compatibility may validate a symbol after resolution. It does not merge symbols.

## Relationship Kind and Configuration Precedence

Relationship kind determines builder shape and is structural. Contributions targeting one locator
must agree on kind. A lower-precedence incompatible contribution is ignored. An equal- or
higher-precedence attempt to change an established kind reports a configuration conflict rather
than replacing or merging symbols.

After the symbol and kind are resolved, configurable facets retain:

```text
Convention < DataAnnotation < Explicit
```

Later equal-precedence configuration retains the framework's established overwrite behavior where
the facet permits replacement.

## API Naming

Relationship API name becomes precedence-aware configuration rather than constructor identity.
Relationship builders expose explicit name configuration such as `WithName`. The name supplied to
a name-first `Add` or `Declare` method is explicit initial name configuration.

Independent symbols resolving to the same final API name produce the existing duplicate-name
compilation diagnostic. They are never merged by name.

## Configuration Interfaces and Discovery

Assembly-discovered non-member relationship configurations declare registration-backed symbols and
are idempotent by configuration CLR type and relationship role within one build session. A class
implementing several relationship configuration roles has a distinct discovery locator per role.

Directly supplied non-generic configuration instances create independent registrations. A future
member-targeted configuration contract may instead identify its declaration through an explicit CLR
navigation locator.

## Validation and Diagnostics

The configuration and compilation workflow diagnoses at least:

- One navigation locator claimed by incompatible relationship kinds.
- Supplied locators that already resolve to different symbols.
- A relationship kind, endpoint, target type, or cardinality incompatible with an authoritative
  navigation locator.
- A registration handle used with another schema builder or incompatible relationship kind.
- An invalid or unresolved CLR navigation locator.
- Incompatible endpoint, foreign-key, association, or traversal configuration on one symbol.
- Duplicate final relationship API names across independent symbols.

Declaration conflicts, configuration conflicts, and final-name conflicts remain distinct diagnostic
categories with fully qualified schema paths where a compiled declaration path exists.

## Compatibility

- Existing name-first `AddOneToOneRelationship`, `AddOneToManyRelationship`, and
  `AddManyToManyRelationship` usage remains available.
- Repeated name-only calls no longer configure one builder; they create independent relationships.
- Member-based declaration and configuration overloads add stable targeting APIs.
- Advanced `DeclareXXXRelationship` methods add kind-safe opaque registration handles.
- Relationship builders gain precedence-aware explicit name configuration.
- Annotation-reader and configuration contracts may require coordinated source and binary changes.
- Compiled relationship metadata, traversal semantics from ADR 0003, JSON, and runtime API-name
  lookups remain unchanged.

## Consequences

### Positive

- Distinct relationships between identical endpoint types remain distinct.
- Member-backed contributions can converge without relying on API names.
- Repository-backed and purely navigational relationships remain supported.
- Ambiguous relationship evidence produces diagnostics rather than implicit symbol merging.
- Foreign-key and association configuration can evolve without invalidating registry identity.

### Negative

- Configuring an unbound relationship after declaration requires its registration handle.
- Existing same-name relationship reconfiguration changes behavior.
- Member-locator conflicts and inverse association require new diagnostics and tests.
- Public builders, annotations, and configuration interfaces require coordinated evolution.

### Neutral and Follow-up Work

- Exact expression overloads and kind-safe registration-handle type names remain implementation-level
  decisions.
- This decision does not infer relationships from every compatible CLR navigation member.
- Service relationship execution remains governed by ADR 0002 and traversal semantics by ADR 0003.

## Confirmation

Implemented and covered by the [relationship declaration regression suite](../../Tests/Evoogle.ApiFramework.Tests/Schema/Configuration/Relationships/ApiRelationshipDeclarationTests.cs).
Final validation passed 1,664 runtime tests and 117 annotation tests with no skips. Production and test
builds completed with zero warnings and errors; diff, UTF-8 BOM, CRLF, and final-newline checks passed.

The decision is confirmed when tests demonstrate:

- Name-only calls create independent registration-backed symbols.
- Member-located calls resolve and configure one symbol.
- One symbol can own principal and dependent navigation locators.
- Member locators authoritatively constrain CLR binding, target type, and cardinality while
  permitting API-facing traversal naming overrides.
- Unclaimed inverse locators can be attached to the resolved symbol.
- Locators resolving to different symbols produce conflicts without merging.
- Structurally similar relationships remain distinct.
- Foreign-key, endpoint, association, and API-name equality do not establish identity.
- Member annotations and explicit member-targeted configuration converge with correct precedence.
- Type-level annotations are build-local registration-backed declarations.
- Registration handles remain valid across sequential builds and reject other builders or kinds.
- Duplicate final names remain compilation diagnostics.
- Repeated-build, failed-build, discovery-idempotency, build, test, diff, encoding, and line-ending
  verification satisfies repository requirements.
