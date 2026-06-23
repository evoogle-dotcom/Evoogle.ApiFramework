# Repository Guidelines

## Project Structure & Module Organization

This repository is a .NET 10 solution for `Evoogle.ApiFramework`.

- `Source/Evoogle.ApiFramework.Core/` contains the production library.
- `Tests/Evoogle.ApiFramework.Core.Tests/` contains xUnit v3 tests.
- `Evoogle.ApiFramework.sln` is the solution entry point.
- `Directory.Build.props` centralizes the target framework, nullable-reference-type settings, analyzers, and XML-documentation rules.
- The core project references sibling repository code under `../evoogle-core`; keep that repository checked out and available when building locally.

Keep production code and tests organized by the feature or API area they implement. When adding a new production type, place its tests in the corresponding test area.

## Build, Test, and Development Commands

Run commands from the repository root.

```powershell
dotnet build Source\Evoogle.ApiFramework.Core\Evoogle.ApiFramework.Core.csproj
dotnet build Tests\Evoogle.ApiFramework.Core.Tests\Evoogle.ApiFramework.Core.Tests.csproj
dotnet test Tests\Evoogle.ApiFramework.Core.Tests\Evoogle.ApiFramework.Core.Tests.csproj --no-build
dotnet test Tests\Evoogle.ApiFramework.Core.Tests\Evoogle.ApiFramework.Core.Tests.csproj --filter "FullyQualifiedName~ApiSchemaTests"
```

Use project-level build and test commands for focused work.

The full solution can include projects from the sibling `evoogle-core` repository and may fail when that repository is unavailable or when its build outputs are locked. Do not treat such failures as defects in this repository without first identifying the failing project and cause.

After changing production code:

1. Build the affected production project.
2. Build the affected test project.
3. Run the relevant focused tests.
4. Run the broader affected test project when the scope of the change warrants it.

Do not use `--no-build` unless the applicable project has already been built successfully after the latest code changes.

## Coding Style

Follow the repository `.editorconfig`.

The primary formatting conventions include:

- UTF-8 with BOM.
- CRLF line endings.
- 4-space indentation.
- A final newline.
- A target maximum line length of 100 characters.
- File-scoped namespaces.
- Opening braces on new lines.
- Braces for control-flow statements.
- `var` where required by the configured style rules.
- `this.` qualification for members where required by the configured style rules.

Nullable reference types are enabled. Do not suppress nullable warnings merely to make a build succeed. Correct the nullability model or document why suppression is necessary.

Public production APIs require XML documentation because CS1591 is treated as an error outside test projects.

Do not reformat or reorganize unrelated code as part of a focused change.

## Naming Standards

All new and modified identifiers must follow:

```text
docs/NamingStandards.md
```

Read that document before introducing or renaming variables, parameters, fields, properties, methods, types, or other identifiers.

Use the following order of precedence:

1. Compiler and language requirements.
2. Repository `.editorconfig` naming and formatting rules.
3. `docs/NamingStandards.md`.
4. The conventions established by the surrounding code when the preceding sources are silent.

The repository's basic mechanical naming conventions include:

- Public APIs, types, properties, methods, enum members, delegates, and events use `PascalCase`.
- Interfaces use `I` followed by `PascalCase`.
- Parameters and local variables use `camelCase`.
- Non-public instance fields use `_camelCase`.
- Boolean names use affirmative predicate terminology when practical, such as `is`, `has`, `can`, `should`, `supports`, `allows`, or `requires`.
- Collection names are plural unless the identifier represents a collection abstraction or concept whose established domain name is singular.
- Acronyms are treated as words unless `docs/NamingStandards.md` specifies otherwise.

Examples:

```csharp
ApiSchema schema;
ApiProperty property;
IReadOnlyList<ApiProperty> properties;

bool isRequired;
bool hasErrors;
bool supportsNullValues;

private readonly ApiSchema _schema;

public ApiObjectType DeclaringType { get; }
public IReadOnlyList<ApiProperty> Properties { get; }
```

Before choosing a name:

1. Determine the identifier's semantic role.
2. Prefer established domain terminology already used in the repository.
3. Choose a name that communicates meaning rather than implementation details.
4. Avoid vague terms such as `data`, `info`, `item`, `object`, `value`, `result`, `manager`, `helper`, or `utility` unless the surrounding context makes the meaning precise.
5. Use consistent terminology for the same concept throughout production code, tests, XML documentation, and error messages.

When modifying existing code, correct noncompliant identifiers that are directly involved in the change when the rename is safe and remains within scope.

Do not rename unrelated identifiers merely because they violate the current naming standard.

## Rename Safety

Before renaming an identifier, determine whether it affects:

- A public or protected API.
- JSON or other serialized property names.
- Database columns, stored data, or migrations.
- Reflection or dynamically resolved member names.
- Dependency-injection registration or resolution.
- Configuration binding.
- Source generators.
- XML documentation links.
- Tests or snapshots.
- Downstream projects, including the sibling `evoogle-core` repository.

Private fields, parameters, and local variables may generally be renamed when all references are updated and the affected project builds.

Do not apply potentially breaking public API, serialization, configuration, or persistence renames unless the task explicitly requires them. When such a rename appears beneficial but is outside the requested scope, report it as a recommendation instead of applying it.

A C# symbol rename must use symbol-aware refactoring where available. Do not use unrestricted text replacement for identifier renames.

## Testing Guidelines

Tests use xUnit v3 with `Evoogle.XUnit` helpers and FluentAssertions.

Place tests under:

```text
Tests/Evoogle.ApiFramework.Core.Tests/
```

Organize tests to match the production feature or API area being exercised.

Prefer descriptive test names that state the behavior under test, for example:

```text
InitializeInfersOneToKeyBindingOnRelationship
```

Test names should use repository terminology consistently. When a production concept is renamed, update the corresponding test names, assertions, XML documentation, and diagnostic messages when they describe the same concept.

Add regression coverage when changing:

- Schema initialization.
- JSON conversion.
- Builder behavior.
- Relationship and key inference.
- Validation behavior.
- Public schema types.
- Equality or identity semantics.

Tests should verify externally observable behavior rather than duplicating the implementation.

## Commit & Pull Request Guidelines

Use imperative, concise commit subjects, for example:

```text
Rename relationship primary key terminology to principal key
```

Keep each commit focused on one logical change.

Pull requests should include:

- A concise problem statement.
- A summary of API or behavioral changes.
- Any naming or terminology changes.
- Compatibility or migration implications.
- The exact build and test commands run.
- Links to related issues when available.

Include screenshots only for user-visible UI changes. Most changes in this repository should provide build and test evidence instead.

## Agent Workflow

Before making changes:

1. Read this `AGENTS.md`.
2. Read all applicable `.editorconfig` files.
3. Read `docs/NamingStandards.md` when the task adds, modifies, reviews, or renames identifiers.
4. Inspect the relevant production code and tests.
5. Search for existing repository terminology and patterns before introducing new terminology.
6. Identify possible public API, serialization, persistence, or downstream compatibility effects.

While making changes:

- Keep changes focused on the requested behavior.
- Prefer established repository abstractions and terminology.
- Coordinate related updates across production code, tests, XML documentation, converters, builders, diagnostics, and initialization logic.
- Do not silently introduce terminology that conflicts with an established domain distinction.
- Do not edit generated `bin/` or `obj/` output.
- Do not make broad refactors when a localized change is sufficient.

Before completing the task:

1. Review the changed identifiers against `docs/NamingStandards.md`.
2. Review formatting against `.editorconfig`.
3. Build the affected production project.
4. Build the affected test project.
5. Run the relevant tests.
6. Check the diff for unrelated formatting or renaming changes.
7. Report any standard that could not be followed and explain why.

## Public API Changes

Before changing public API shape or terminology, search for coordinated impacts in:

- JSON converters.
- Builders and fluent APIs.
- Interfaces and implementations.
- XML documentation.
- Equality and hashing behavior.
- Test exclusions.
- Initialization issue expectations.
- Validation messages.
- Reflection-based behavior.
- The sibling `evoogle-core` repository.

Preserve source and serialized compatibility unless the task explicitly authorizes a breaking change.

When a public rename is required, update all affected usages and clearly report the compatibility implications.
