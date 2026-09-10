# Use an Operation-Oriented API Runtime

## Status

Accepted on 2026-09-09 for an alpha vertical proof of concept. The proof of concept will provide
evidence for a later architecture iteration rather than make every detail permanent.

## Context and Problem Statement

Evoogle.ApiFramework currently has a compiled, immutable schema describing API types, keys, and
relationships. The framework needs a provider-neutral runtime capable of expressing and executing
API interactions from both .NET clients and services, then projecting those interactions through
JSON:API, GraphQL, gRPC, and future providers.

The standard framework layer should carry most semantic complexity. Providers should translate
protocol representations, not redefine query, command, null, validation, materialization, or
repository behavior. Service execution must support heterogeneous data sources connected through
schema identity and relationships without assuming one shared `IQueryable<T>` graph.

## Decision Drivers

- Share POCO service models between .NET clients and services when desired.
- Keep API semantics independent of JSON:API, GraphQL, gRPC, and future protocols.
- Prefer a rich canonical model with capability negotiation over a lowest common denominator.
- Make the standard runtime substantial and third-party provider implementations comparatively
  small and intuitive.
- Support batched, independent operations in every logical request.
- Minimize repository network calls through batch-first identity and relationship retrieval.
- Traverse relationships across heterogeneous physical data sources.
- Preserve immutable compiled artifacts and bounded request-scoped mutable state.
- Reuse Evoogle.Core NTree for genuine one-to-many structural ownership trees.
- Define a bounded alpha proof of concept before expanding the architecture further.

## Considered Options

1. Make REST resources the framework's root abstraction.
2. Use only provider-specific request models with a small shared denominator.
3. Use `IQueryable<T>` as the universal service execution abstraction.
4. Use an operation-oriented canonical IR with schema-driven repository execution.

## Decision Outcome

Choose option 4. Evoogle.ApiFramework models API operations independently of interaction protocol.
Arbitrary service operations are the broad architecture, with structured data querying and state
change as the initial specializations. Resource-oriented APIs are provider projections rather than
the framework foundation.

The alpha operation model includes `ApiQuery` and `ApiCommand`. Arbitrary named invocation is
deferred.

## Top-Level Models

The framework has four top-level models:

```text
Service Model
    POCO types used by application clients and services
                │
                ▼
Schema Model
    Immutable metadata describing API-visible semantics
                │
                ▼
Operation Model
    Provider-neutral compiled request intent and result shape
                │
                ▼
Provider Model
    Protocol capability analysis, encoding, and decoding
```

### Service model

The service model contains CLR POCOs. Authoring APIs can use CLR expressions and POCO values, but
the compiled operation IR does not retain arbitrary CLR expression trees, captured closures, or
provider syntax.

### Schema model

The schema is the semantic authority for types, properties, keys, relationships, query sources,
command targets, and their policies.

```text
ApiSchema
├── ApiTypes
├── ApiRelationships
├── ApiQuerySources
└── ApiCommandTargets
```

Query sources and command targets are explicit compiled metadata. Conventions may generate common
sources and targets, but an `ApiObjectType` does not become queryable or mutable merely because it
exists.

### Operation model

`ApiOperation` is the immutable, normalized, schema-bound canonical IR produced by operation
compilation. Mutable authoring state is separate.

```text
ApiOperation
├── ApiQuery
└── ApiCommand
    ├── ApiCreateCommand
    ├── ApiUpdateCommand
    └── ApiDeleteCommand
```

### Provider model

Providers translate between canonical framework representations and external protocol
representations. They do not execute repositories or redefine standard semantics.

## Operation Compilation

Operation construction follows a publication lifecycle analogous to schema compilation:

```text
Mutable authoring input
    → bind to one frozen ApiSchema
    → normalize
    → validate
    → derive result shape and required features
    → publish immutable ApiOperation
```

Compiled operation nodes hold direct references to resolved schema metadata. The operation binds to
the exact schema snapshot, not only a schema name or version string.

The common operation anatomy is:

```text
ApiOperation
├── ApiSchema
├── parameter definitions
├── derived required features
├── frozen extensions
└── operation-specific structure
```

Required features are derived from normalized structure. Providers and execution bindings advertise
capabilities. Authored declarations cannot understate the features an operation uses.

## Parameters and Canonical Data

The model distinguishes:

```text
Parameter definition
    Declares a reusable operation input.

Parameter expression
    Refers directly to an owned parameter definition.

Argument
    Supplies data for one request-scoped operation execution.

Literal
    Embeds immutable data in compiled operation structure.
```

Missing data and explicit null are distinct. Presence, nullability, and default behavior are
separate concerns.

Actual canonical data uses a schema-aware hierarchy:

```text
ApiData
├── ApiNullData
├── ApiScalarData
├── ApiEnumData
├── ApiObjectData
└── ApiCollectionData
```

Scalar leaves retain schema-normalized CLR values. Objects bind fields to resolved schema
properties where applicable. Providers receive normalized data rather than unchecked `object`
values.

## Canonical Null Semantics

Canonical equality and inequality use two-valued, null-safe value semantics:

```text
null == null      → true
null == non-null  → false
null != null      → false
null != non-null  → true
```

The broader expression system may produce `Boolean?` when an expression explicitly has nullable
Boolean semantics. A query filter includes only `true`; `false` and null do not match.

Providers and execution backends preserve canonical semantics through native translation, standard
lowering, controlled local evaluation, or capability rejection. Backend-specific three-valued
semantics do not redefine the canonical operation.

## Query Model

An `ApiQuery` begins from one schema-declared `ApiQuerySource` and produces a general result shape.
One alpha query has this logical pipeline:

```text
Source
    → Filter
    → Order
    → Offset pagination
    → Project
    → Result shape
```

A query source is a named, parameterized producer of a schema-typed value. It declares its output
type and allowed composition. Multiple sources may produce the same object type. Sources may be
repository-backed or custom, and they are not required to produce objects.

The alpha expression vocabulary contains:

- Range references.
- Resolved property access.
- Parameters and literals.
- `And`, `Or`, and `Not`.
- Equality and ordered comparison.
- `IsNull` and `IsNotNull`.

Captured application values are parameterized by default. Arbitrary CLR method calls do not survive
operation compilation.

Ordering is an ordered sequence of typed terms with explicit direction and resolved null placement.
The alpha uses deterministic ordinal string ordering. Offset and limit are the only pagination
model. Cursor pagination and cross-request continuation state are deferred.

Nested relationship inclusion is supported, but nested filtering, ordering, and pagination are
deferred.

## Projection and Result Shapes

Projection describes how output is produced. The compiler derives an immutable result shape from
the projection; callers do not author both independently.

```text
ApiResultShape
├── ApiScalarResultShape
├── ApiEnumResultShape
├── ApiObjectResultShape
└── ApiCollectionResultShape
```

Scalar and enum shapes require their corresponding schema types. An object shape may retain
`ApiObjectType` provenance or describe an operation-local structural object. A collection shape
contains an item shape.

`Shape` describes expected structure. `Result` means an execution outcome. `Data` means actual
canonical content.

Partial schema-object projections preserve selected-versus-absent fields. Canonical data can always
represent the result even when a partial POCO cannot be constructed safely.

## Command Model

An `ApiCommandTarget` is a named mutation surface over exactly one `ApiObjectType`. One object type
may have multiple targets with different policies.

```text
ApiCommandTarget
├── optional create contract
├── optional update contract
└── optional delete contract
```

The presence of a contract enables that command kind. Contracts define accepted properties,
identity, concurrency, and result policies rather than attaching mutability directly to the object
type.

Create and update use one uniform property-assignment representation:

```text
ApiPropertyAssignment
├── resolved ApiProperty
└── ApiExpression
```

For create, assignments establish proposed initial state and absent properties use defaults or
generation. For update, assignments describe a sparse patch and absent properties remain unchanged.

The alpha assignment expressions are parameters and literals. Current-state calculations such as
increment expressions are deferred.

Update and delete identify one object through a resolved named key and `ApiKey`. Identity mutation
is prohibited initially. Create identity may be supplied as an allowed property assignment or
generated by the service.

Update and delete may carry standard Boolean preconditions. Concurrency is a classified
precondition whose atomic repository handling uses the first-class version feature defined by
[ADR 0001](0001-model-object-version-as-first-class-schema-metadata.md).

Each command is semantically atomic. Operations within one request remain independently atomic and
do not share a request-level transaction.

Create and update may project resulting state. Delete initially returns outcome metadata without a
pre-deletion result projection.

## Request and Response Model

An `ApiRequest` is a logical batch containing one or more independent operation executions. An
operation execution binds a reusable compiled operation to a request-local correlation identity and
argument set. Its exact public type name remains provisional.

```text
ApiRequest
├── one exact ApiSchema
└── operation executions [1..N]
    ├── correlation identity
    ├── ApiOperation
    └── resolved arguments
```

All operations bind to the same exact schema snapshot. Initial batches have no dependencies,
workflow references, shared transaction, or semantic execution order.

Mutable request authoring passes through an all-or-nothing preparation phase. Every operation and
argument is validated before any command executes. Once preparation succeeds, every independent
operation is attempted and produces a correlated result when its outcome is known.

The response separates request processing status from aggregate operation success:

```text
ApiResponse
├── RequestStatus
│   ├── Completed
│   ├── Rejected
│   ├── Failed
│   └── Indeterminate
├── OperationResultSummary
│   ├── Succeeded
│   ├── PartiallySucceeded
│   └── Unsuccessful
└── correlated ApiOperationResults
```

The aggregate summary is derived from immutable operation results. A completed request may be
partially successful or unsuccessful.

## Runtime and Context Model

The runtime contains long-lived, thread-safe infrastructure. A context coordinates one bounded
request/response exchange and is not reused concurrently.

```text
ApiRuntime
├── ApiClientRuntime
└── ApiServiceRuntime

ApiContext
├── ApiClientContext
└── ApiServiceContext
```

`ApiRuntime` owns the schema, compilers, standard value and expression services, feature analysis,
and configured bindings. Role-specific runtimes add providers, transports, and service execution
bindings.

`ApiContext` contains request-scoped lifecycle state, diagnostics, cancellation, prepared request,
and completed response. Client and service contexts expose complementary workflows over shared
canonical models rather than one concrete context exposing invalid role combinations.

Required lifecycle transformations are fixed pipeline stages. Filters wrap named boundaries for
cross-cutting concerns. Provider-native and stage-specific mutable artifacts live on the context,
not in immutable canonical operations or requests.

## Provider Model

The provider interfaces use complementary, self-documenting transformations:

```text
IApiProvider
├── IApiClientProvider
│   ├── EncodeRequest
│   └── DecodeResponse
└── IApiServiceProvider
    ├── DecodeRequest
    └── EncodeResponse
```

`Encode` transforms canonical framework representations into provider representations. `Decode`
transforms provider representations into canonical authoring, data, or result representations.

Client request encoding may create one or more physical transmissions. Native protocol batching is
an optimization capability, not a requirement for canonical request batching. Correlation maps and
provider-native state belong to encoding/decoding artifacts rather than `ApiOperation`.

Providers own protocol syntax, scalar codecs, protocol error mapping, and declared capabilities.
They do not own canonical compilation, repository execution, relationship assembly, POCO
materialization, or null semantics.

Transport remains conceptually separate from provider representation. Provider transmission types
may remain opaque so the abstraction does not assume HTTP.

## Service Repository Model

The service uses a schema-driven data-loader architecture.

```text
ApiServiceRuntime
├── IApiRepository per executable ApiObjectType
└── IApiRelationshipRepository per executable relationship traversal
```

One concrete repository may implement both interfaces when objects and relationships share a data
source. `IQueryable<T>` is an optional repository implementation strategy, not the canonical service
execution abstraction.

`IApiRepository<TObject>` is the repository-author-facing POCO contract. Retrieval initially
returns complete CLR objects compatible with the registered `ApiObjectType`. The standard runtime
extracts identity and version, normalizes values, and assembles `ApiData`. Repositories do not need
to populate navigation properties.

Retrieval primitives are batch-first:

```text
Query root objects using one repository-local query
Retrieve objects by one ApiNamedKeyType and ApiKey batch
Retrieve relationship mappings from one identity batch to target identities
```

A single identity is the one-element case. Batch sizes and safe concurrency are repository
capabilities used by the standard runtime to minimize physical calls.

Relationship storage is independently bindable through `IApiRelationshipRepository`. Relationship
methods receive resolved relationship traversal metadata and return identity-only to-one or to-many
mappings.

```text
ApiRelationshipMapping
├── ApiRelationshipToOneMapping
│   ├── FromKey
│   └── optional ToKey
└── ApiRelationshipToManyMapping
    ├── FromKey
    └── ToKeys
```

`ToOne` provisionally permits zero or one target because required multiplicity is not yet
represented in the schema. `ToMany` permits an empty collection. A future multiplicity model uses
minimum and maximum counts; exceptional non-contiguous constraints such as zero-or-two belong to an
additional relationship constraint layer.

Repository patch input is a sparse evaluated write set:

```text
ApiRepositoryPatch
├── object identity
├── expected version
├── resolved property and ApiData pairs
└── atomic conditions
```

The standard runtime does not mutate a cached POCO before persistence. The repository applies only
listed properties and enforces version and other required conditions atomically.

## Data-Loader Execution

Each operation execution receives an isolated data-loader scope. Caches are not automatically
shared across independent operations because doing so would introduce implicit consistency and
ordering semantics.

The scope contains:

- An object identity map keyed by canonical named key and `ApiKey`.
- Alternate-key aliases to canonical identity.
- Known-missing object identities.
- Relationship mappings keyed by traversal and source identity.
- Known-empty relationship mappings.

Query execution is breadth-first:

```text
1. Execute the repository-local root query.
2. Materialize and validate root identities and versions.
3. Batch relationship mappings for all source identities at the next projection level.
4. Union and deduplicate target identities.
5. Satisfy cached identities and batch-retrieve missing target objects.
6. Repeat for each finite projection level.
7. Assemble canonical ApiData according to the projection and result shape.
```

The projection depth, not the potentially cyclic schema relationship graph, bounds traversal.
Repeated identities are retrieved once within one operation. Missing targets referenced by known
mappings fail the affected query initially rather than being silently omitted. Duplicate target
identities in one mapping are invalid initially.

Root repository ordering is preserved separately from the identity map. To-many relationship
results without semantic ordering are normalized deterministically by canonical target identity.

## Structural Tree Constraint

Framework-owned structures that form genuine one-to-many ownership trees reuse Evoogle.Core NTree
instead of introducing another tree abstraction. Likely uses include operation authoring,
expressions, projections, result shapes, canonical data, query expansion plans, and provider
lowering plans.

NTree is not used for general graphs. Schema references, parameter references, identity maps,
relationship mappings, repository bindings, provider correlations, shared references, and cyclic
POCO relationships remain explicit non-owning references or graph structures.

Structural containment and semantic references remain distinct. Compiled trees expose immutable
published structure even if mutable topology is used during authoring or compilation.

## Alpha Vertical Proof of Concept

The proof of concept uses `Customer`, `Order`, and `Product` POCOs with `CustomerOrders` and
`OrderProduct` relationships. The schema declares `Customers` and `CustomerById` query sources and
a `Customers` command target supporting create, update, and delete.

Repositories deliberately use heterogeneous implementation styles. One query performs:

```text
Customers
├── filter IsActive == true
├── order by Name, then Id
├── offset 0, limit 2
└── project
    ├── Id
    ├── Name
    └── Orders
        ├── Id
        ├── Total
        └── Product
            ├── Id
            └── Name
```

Expected retrieval consists of one root query followed by one batched relationship-mapping call and
one batched identity retrieval per expansion level. Shared product identities are retrieved once.
Tests assert exact repository call counts and batch contents.

The same request contains independent create, update, and missing-target delete commands. Their
results demonstrate generated identity/version data, sparse patching with atomic expected-version
handling, `NotFound`, correlation, a completed request, and a derived `PartiallySucceeded` summary.

A second request executes the same compiled `CustomerById` query twice with different arguments to
confirm reusable operations and isolated execution scopes.

An internal reference provider implements all four provider methods and crosses a provider-owned
representation boundary. It must not pass original operation references directly between client and
service. After the reference-provider slice works, JSON:API, GraphQL, and gRPC spikes exercise the
same canonical scenario.

## Deferred Roadmap

The alpha explicitly excludes:

- `ApiInvocation` and arbitrary named behavior.
- Dependent operations, workflows, and request-level transactions.
- Cross-operation identity-map sharing.
- Cursor pagination and cross-request continuation state.
- Nested relationship filtering, ordering, and pagination.
- Cross-relationship root predicates and correlated subqueries.
- Aggregation, grouping, set operations, and streaming.
- Graph and bulk mutation within one command.
- Partial repository object retrieval and cache field coverage.
- Persistent client state tracking.
- Production authentication, authorization, retries, and idempotency policy.
- Complete JSON:API, GraphQL, or gRPC implementations.

Two additional interaction patterns are retained for future design:

```text
ApiDispatch
    One-way command dispatch without a synchronous semantic result.

ApiEvent
    Event publication and delivery to zero or more listeners.
```

Broadcast is delivery topology for an event, not a separate operation type. Long-lived subscription
management belongs to the runtime; each delivered event receives a bounded processing context.

## Consequences

### Positive

- Client, service, and provider implementations share one rich semantic model.
- Providers have four intuitive boundary methods and avoid business or storage semantics.
- Heterogeneous object and relationship stores participate in one API graph.
- Batch-first breadth traversal minimizes network calls and avoids N+1 retrieval.
- Immutable compilation boundaries make operation and request behavior inspectable and testable.
- Explicit capabilities expose unsupported features instead of silently changing semantics.
- The vertical slice provides concrete evidence before broader implementation.

### Negative

- The standard runtime requires substantial compilers, planners, validators, and data assembly.
- Efficient nested and cross-relationship operations require future federated planning.
- Protocol providers may need standard lowering or multiple transmissions to preserve semantics.
- Complete POCO retrieval can over-fetch until partial retrieval is designed.
- The number of schema and runtime abstractions is larger than a protocol-specific framework.

### Neutral and Follow-up Work

- Exact namespaces, signatures, builders, diagnostics, and public names require implementation-level
  design after this ADR.
- The alpha may reveal that provisional artifact names or boundaries should change.
- The version feature is implemented and vetted separately under ADR 0001 before version-dependent
  command scenarios are completed.
- Roadmap features require new ADRs or explicit amendments rather than being inferred from this
  decision.

## Confirmation

The decision is confirmed by an alpha vertical slice that demonstrates:

1. A frozen schema containing the required types, relationships, query sources, and command target.
2. Client authoring compiled into immutable schema-bound operations.
3. Independent batched operation executions with distinct arguments and correlations.
4. Provider-owned request and response representations crossing all four provider methods.
5. Service-side recompilation and all-or-nothing request preparation before command execution.
6. Repository-local filtering, ordering, and offset pagination.
7. Breadth-first batched identity and relationship retrieval with asserted call counts.
8. Canonical `ApiData` assembly conforming to derived `ApiResultShape` trees.
9. Create, sparse patch, atomic version conflict handling, delete, and command outcomes.
10. Correlated results with completed and partially succeeded aggregate response semantics.
11. Client-side response validation and POCO or structural result materialization.
12. NTree use for all applicable structural ownership trees and no forced graph representation.
13. Focused build, test, diff, UTF-8 BOM, and CRLF verification required by the repository.
