---
name: naming-review
description: Reviews and safely renames C# variables, parameters, fields, properties, methods, and types according to the repository naming standard.
---

# Naming review

Review C# identifiers using `docs/NamingStandards.md`.

## Scope

Unless explicitly asked for a repository-wide migration, inspect only:

- files changed in the current task
- declarations directly affected by the change
- references that must be renamed to keep the code compiling

Do not rename unrelated identifiers.

## Inputs

The invoking prompt may specify:

- `scope`: file, directory, project, solution, diff, or repository
- `mode`: `review-only`, `apply-safe`, or `apply-authorized`
- `output`: chat response or report file
- `risk-levels`: categories that may be modified

Defaults:

- `scope`: current diff
- `mode`: `review-only`
- `output`: chat response
- `risk-levels`: none

## Process

1. Read `AGENTS.md`.
2. Read `docs/NamingStandards.md`.
3. Read applicable `.editorconfig` files.
4. Inspect the current diff.
5. Classify every relevant identifier:
   - type
   - method
   - property
   - field
   - parameter
   - local variable
   - Boolean
   - collection
   - key or identifier
6. Identify naming violations.
7. Determine whether each rename affects:
   - public APIs
   - serialized data
   - JSON property names
   - database columns
   - reflection
   - dependency injection
   - configuration binding
   - tests
8. Safely rename private and local identifiers.
9. Do not rename public or persisted identifiers unless the task
   explicitly authorizes a breaking or migration-sensitive change.
10. Build the affected solution.
11. Run relevant tests.
12. Report:
    - identifiers renamed
    - violations intentionally retained
    - public API or persistence risks
