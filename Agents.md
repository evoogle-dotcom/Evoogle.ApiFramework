# Repository Guidelines

## Project Structure & Module Organization

This repository is a .NET 10 solution for `Evoogle.ApiFramework`.

- `Source/Evoogle.ApiFramework.Core/` contains the production library.
- `Tests/Evoogle.ApiFramework.Core.Tests/` contains xUnit v3 tests.
- `Evoogle.ApiFramework.sln` is the solution entry point.
- `Directory.Build.props` centralizes target framework, nullable, analyzers, and XML-doc rules.
- The core project references sibling repository code under `../evoogle-core`; keep that checkout available when building locally.

## Build, Test, and Development Commands

Run commands from the repository root:

```powershell
dotnet build Source\Evoogle.ApiFramework.Core\Evoogle.ApiFramework.Core.csproj
dotnet build Tests\Evoogle.ApiFramework.Core.Tests\Evoogle.ApiFramework.Core.Tests.csproj
dotnet test Tests\Evoogle.ApiFramework.Core.Tests\Evoogle.ApiFramework.Core.Tests.csproj --no-build
dotnet test Tests\Evoogle.ApiFramework.Core.Tests\Evoogle.ApiFramework.Core.Tests.csproj --filter "FullyQualifiedName~ApiSchemaTests"
```

Use the project-level build/test commands for focused work. The full solution can include sibling `evoogle-core` projects and may fail if those outputs are locked or unavailable.

## Coding Style & Naming Conventions

Follow `.editorconfig`: UTF-8 BOM, CRLF, 4-space indentation, final newline, and 100-character target line length. Use file-scoped namespaces, braces on new lines, `var` where required by style rules, and `this.` for members.

Public APIs, types, properties, methods, enum members, and events use PascalCase. Interfaces use `I` + PascalCase. Non-public instance fields use `_camelCase`. Nullable reference types are enabled. Public production APIs require XML documentation because CS1591 is treated as an error outside test projects.

## Testing Guidelines

Tests use xUnit v3 with `Evoogle.XUnit` helpers and FluentAssertions. Place tests under `Tests/Evoogle.ApiFramework.Core.Tests/`, matching the production area being exercised. Prefer descriptive test names such as `InitializeInfersOneToKeyBindingOnRelationship`. Add regression coverage for schema initialization, JSON conversion, and builder behavior when changing public schema types.

## Commit & Pull Request Guidelines

Recent commits use imperative, concise subjects, for example: `Rename relationship primary key terminology to principal key`. Keep commits focused on one logical change.

Pull requests should include a short problem statement, a summary of API or behavior changes, and the exact build/test commands run. Link related issues when available. Include screenshots only for user-visible UI changes; most changes here should provide test evidence instead.

## Agent-Specific Instructions

Do not edit generated `bin/` or `obj/` output. Avoid broad refactors when a localized schema or test change is enough. Before changing public API shape, search for JSON converters, builder APIs, test exclusions, and initialization issue expectations that may need coordinated updates.
