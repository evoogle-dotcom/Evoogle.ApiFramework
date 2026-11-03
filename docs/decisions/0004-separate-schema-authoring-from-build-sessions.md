# Separate Schema Authoring from Isolated Build Sessions

## Status

Accepted and implemented on 2026-11-01.

## Context and Problem Statement

`ApiSchemaBuilder` combines durable fluent authoring, assembly discovery, annotation reading,
conventions, inferred configuration, schema materialization, and compilation. Scalar, enum, and
object builders have stable CLR-type identities, while relationship and query-source builders have
historically been registered by API name.

API names are configurable schema output. A convention or explicit override may change an API name,
so a mutable API name cannot also be reliable builder identity. Repeated builds expose a related
lifecycle problem: annotations and conventions can mutate builder state retained from a previous
build. Query sources currently mitigate part of this problem with a specialized per-build snapshot,
but the schema-building workflow needs one consistent lifecycle.

The immutable `ApiSchema` compiler remains a sound publication boundary. The missing boundary is
between durable authoring state and the derived state used by one build.

## Decision Drivers

- Preserve immediate fluent-callback execution and exception timing.
- Make repeated builds deterministic after conventions, annotations, or authoring state change.
- Prevent failed builds from contaminating later builds.
- Keep `Convention < DataAnnotation < Explicit` configuration precedence.
- Separate declaration identity from mutable API naming.
- Allow declarations with and without natural CLR identities.
- Preserve the existing one-way schema compilation and freezing boundary.
- Keep symbol, locator, and build-session machinery internal to the runtime package.
- Avoid forcing marker types or user-maintained global identifiers on application developers.

## Considered Options

1. Continue mutating one durable builder graph and add targeted cleanup for each feature.
2. Make `ApiSchemaBuilder` one-shot and reject repeated builds.
3. Record and replay public configuration callbacks for every build.
4. Retain durable explicit authoring state and deep-copy it into an isolated build session.

## Decision Outcome

Choose option 4. `ApiSchemaBuilder` retains durable explicit authoring state. Every `BuildResult`
creates an isolated build session from that state, applies all derived configuration inside the
session, materializes one fresh schema graph, and discards the session after success or failure.

```text
Durable authoring state
          │
          ├── deep copy ──> Build session 1 ──> compile/freeze ──> schema 1
          │                                      │
          │                                      └── discard session
          │
          └── deep copy ──> Build session 2 ──> compile/freeze ──> schema 2
                                                 │
                                                 └── discard session
```

## Terminology

- **Authoring state** is durable explicit configuration retained by `ApiSchemaBuilder`.
- A **build session** is the isolated mutable working state for one `BuildResult` call.
- A **declaration** is one logical schema element being authored.
- A **registration** is durable opaque identity for a declaration without a natural CLR locator.
- A **registration handle** is optional public, builder-scoped access to a registration.
- A **locator** is stable CLR or discovery identity used to resolve a declaration.
- A **symbol** is the canonical build-session representation of one resolved declaration.
- A **contribution** is convention, annotation, discovery, or explicit configuration applied to a
  symbol.
- An **API name** is precedence-aware schema configuration resolved independently of declaration
  identity.

The terms describe build-time architecture. They do not add symbol or locator metadata to compiled
schema JSON.

## Authoring and Build Lifecycles

### Direct fluent callbacks execute immediately

Direct fluent callbacks continue to execute when their authoring method is called. Their resulting
explicit state is retained; the callback itself is not replayed during later builds.

```csharp
schemaBuilder.AddObject<Customer>(customer => customer.WithName("customer"));
```

This preserves observable exception timing and avoids replaying callbacks that capture mutable
values or perform other observable work.

### Build sessions copy durable state

Each build deeply copies durable declaration builders, nested builders, configuration-source
values, type-expression factories, ordering, and extension registrations into a fresh session.
Build-session mutations never flow back into durable authoring state.

Registration handles identify durable declarations. The same handle resolves to a fresh symbol in
each build session, so configuration added between builds appears in later builds without retaining
earlier derived state.

### Derived configuration is build-local

The following execute within each build session:

- Assembly type and configuration scanning.
- Annotation reading and annotation-derived declarations.
- Conventions and inference.
- Symbol and locator resolution.
- Configuration and discovery diagnostics.
- Final API-name resolution.
- Schema graph materialization.

Build success or failure always discards this state in `finally`-equivalent cleanup.

### Existing deferred callbacks keep their established timing

Callbacks already defined as build-time behavior, including assembly-discovered configuration and
schema-options construction, continue to execute at their existing lifecycle points. This decision
does not broadly move callback execution earlier or later.

## Declaration Resolution

Configuration contributions first resolve a symbol through a stable locator or registration. Only
after resolution does configuration precedence decide individual facet values. API-name equality is
never used to decide whether declarations are the same.

Natural locators remain preferred where available:

```text
Scalar, enum, or object type       CLR Type
Delegate-backed query source       Delegate Type
Member-backed relationship         CLR navigation member
Unanchored fluent declaration      Opaque registration
```

Registration handles are a fallback for unanchored declarations, not a mandatory authoring model
for every schema element.

## Convenience and Advanced Authoring

Ordinary `AddXXX` methods remain the primary fluent surface. When a declaration lacks a natural CLR
locator, the corresponding advanced `DeclareXXX` method returns an opaque registration handle.
Conceptually, `AddXXX` calls `DeclareXXX` and discards the handle.

```csharp
var registration = schemaBuilder.DeclareQuerySource("customers", source => /* configure */);
schemaBuilder.ConfigureQuerySource(registration, source => /* configure further */);
```

Registration handles are immutable, non-serializable, and owned by the `ApiSchemaBuilder` that
created them. Using a handle with another builder fails immediately with a configuration exception.
Relationship handles must retain relationship-kind safety. Public `Declare` methods issue durable
handles only during authoring; calling them during an active build is a configuration error.
Discovery and conventions use `Add` methods for session-local declarations and may configure copies
of existing durable registrations.

## Discovery Idempotency

Assembly-discovered declarations are idempotent within a build by configuration CLR type and
configuration role. The role is part of the locator because one configuration class can implement
multiple supported configuration interfaces.

Directly supplied non-generic configuration instances remain independent registrations. Multiple
instances of one configuration type may intentionally carry different constructor state. Generic
configuration interfaces may instead target a stable CLR locator defined by their contract.

## Conflict Categories

The build workflow distinguishes:

- A **declaration conflict**, where one stable locator is claimed with incompatible declaration
  structure.
- A **configuration conflict**, where contributions to one symbol specify incompatible settings at
  the applicable precedence.
- A **compiled-name conflict**, where independent valid symbols resolve to the same final API name.

Compiled-name conflicts are diagnosed during final schema lookup construction. They never merge
symbols.

## Compilation and Publication

The build session materializes one fresh `ApiSchema` graph and passes it to the existing compiler.
The compiler remains the one-way publication boundary that validates topology and semantics,
creates frozen extension snapshots, freezes owned elements, and publishes only a valid schema.

No mutable authoring builder or build-session symbol is retained by a compiled schema.

## Concurrency

`ApiSchemaBuilder` remains mutable and does not support concurrent authoring or concurrent builds.
A second build started while one build session is active fails immediately. Independently compiled
schemas remain immutable and safe for concurrent reads under their existing contracts.

## Compatibility

- Compiled schema types and runtime lookup behavior remain unchanged.
- JSON property names, defaults, and serialized contracts remain unchanged.
- API names retain ordinal, case-sensitive identity in compiled schemas.
- Builder and configuration contracts may require coordinated source and binary changes.
- Name-based relationship and query-source builder merging is intentionally replaced by the
  declaration models defined in ADR 0005 and ADR 0006.
- Implementation requires a separately reviewed and approved plan.

## Consequences

### Positive

- Every build starts from one deterministic durable baseline.
- Failed or obsolete discovery state cannot leak into later builds.
- Query sources no longer require a feature-specific build snapshot.
- Declaration identity, configuration precedence, and compiled API identity become separate
  concerns.
- The compiler publication boundary remains unchanged.

### Negative

- All durable builder state requires correct deep-copy support.
- The build workflow gains explicit session, symbol, locator, and registration concepts.
- Existing name-based configuration behavior changes for relationships and query sources.
- Build-session isolation requires broad repeated-build regression coverage.

### Neutral and Follow-up Work

- [ADR 0005](0005-identify-query-sources-by-delegate-or-registration.md) defines query-source
  declaration identity.
- [ADR 0006](0006-identify-relationships-by-registration-and-navigation-locators.md) defines
  relationship declaration identity and locator resolution.
- Exact internal type names and file organization remain implementation-level decisions.

## Confirmation

Implemented and covered by the [build-session regression suite](../../Tests/Evoogle.ApiFramework.Tests/Schema/Configuration/ApiSchemaBuildSessionTests.cs).
Final validation passed 1,664 runtime tests and 117 annotation tests with no skips. Production and test
builds completed with zero warnings and errors; diff, UTF-8 BOM, CRLF, and final-newline checks passed.

The decision is confirmed when tests demonstrate:

- Convention and annotation changes between builds recompute all derived state.
- Explicit authoring state survives repeated successful and failed builds.
- Failed discovery, materialization, or compilation does not contaminate a later build.
- Registration handles remain valid across sequential builds and are rejected by other builders.
- Repeated assembly configuration discovery is idempotent by configuration type and role.
- Fluent callbacks retain their established execution and exception timing.
- Materialization creates one fresh graph per build and compilation freezes only valid graphs.
- Production and test builds, focused and full tests, diff checks, UTF-8 BOM, CRLF, and final-newline
  verification satisfy repository requirements.
