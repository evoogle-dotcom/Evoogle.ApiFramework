// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

internal sealed record ApiAssemblyTypeScanResult(
    IReadOnlyList<Type> Types,
    IReadOnlyList<ApiSchemaCompilationIssue> Issues);

internal static class ApiAssemblyTypeScanner
{
    internal static ApiAssemblyTypeScanResult Scan(
        Assembly assembly,
        Func<Type, bool>? filter = null)
    {
        return Scan(assembly, filter, static sourceAssembly => sourceAssembly.GetExportedTypes());
    }

    internal static ApiAssemblyTypeScanResult Scan(
        Assembly assembly,
        Func<Type, bool>? filter,
        Func<Assembly, Type[]> getExportedTypes)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(getExportedTypes);

        var issues = new List<ApiSchemaCompilationIssue>();
        Type[] exportedTypes;

        try
        {
            exportedTypes = getExportedTypes(assembly);
        }
        catch (ReflectionTypeLoadException exception)
        {
            exportedTypes = exception.Types.OfType<Type>().ToArray();
            issues.Add(CreateAssemblyIssue(
                assembly,
                exception,
                $"Assembly type discovery for '{GetAssemblyName(assembly)}' was incomplete.",
                "Resolve the assembly loader failures before building the API schema."));
        }
        catch (Exception exception)
        {
            exportedTypes = [];
            issues.Add(CreateAssemblyIssue(
                assembly,
                exception,
                $"Assembly type discovery for '{GetAssemblyName(assembly)}' failed.",
                "Resolve the assembly discovery failure before building the API schema."));
        }

        var types = new List<Type>(exportedTypes.Length);
        foreach (var type in exportedTypes)
        {
            if (type is null)
            {
                continue;
            }

            if (filter is null)
            {
                types.Add(type);
                continue;
            }

            try
            {
                if (filter(type))
                {
                    types.Add(type);
                }
            }
            catch (Exception exception)
            {
                issues.Add(CreateAssemblyIssue(
                    type.FullName ?? type.Name,
                    exception,
                    $"Assembly type discovery filter evaluation failed for CLR type " +
                        $"'{type.FullName ?? type.Name}'.",
                    "Correct the assembly discovery filter so it can evaluate " +
                        "every candidate type."));
            }
        }

        return new ApiAssemblyTypeScanResult(types, issues);
    }

    private static ApiSchemaCompilationIssue CreateAssemblyIssue(
        Assembly assembly,
        Exception exception,
        string description,
        string remediation)
    {
        return CreateAssemblyIssue(GetAssemblyName(assembly), exception, description, remediation);
    }

    private static ApiSchemaCompilationIssue CreateAssemblyIssue(
        string apiPath,
        Exception exception,
        string description,
        string remediation)
    {
        return new ApiSchemaCompilationIssue(
            apiPath,
            ApiSchemaCompilationSeverity.Error,
            ApiSchemaCompilationCode.ApiAssemblyDiscoveryFailed,
            description,
            remediation,
            exception: exception);
    }

    private static string GetAssemblyName(Assembly assembly)
    {
        return assembly.GetName().Name ?? assembly.FullName ?? assembly.ToString();
    }
}
