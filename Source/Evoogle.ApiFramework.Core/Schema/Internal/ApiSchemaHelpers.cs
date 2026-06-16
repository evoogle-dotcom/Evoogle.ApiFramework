// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Runtime.CompilerServices;
using System.Text;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiSchemaHelpers
{
    #region Validation Methods
    public static string BuildPath(string? basePath, string segment, string? segmentName)
    {
        var hasBasePath = !string.IsNullOrWhiteSpace(basePath);
        var hasSegmentName = !string.IsNullOrWhiteSpace(segmentName);

        if (!hasBasePath)
        {
            return !hasSegmentName
                ? segment
                : $"{segment}[\"{segmentName}\"]";
        }

        if (!hasSegmentName)
        {
            return $"{basePath}.{segment}";
        }

        var capacity = (hasBasePath ? basePath!.Length + 1 : 0)
            + segment.Length
            + (hasSegmentName ? segmentName!.Length + 4 : 0);

        var sb = new StringBuilder(capacity);

        if (hasBasePath)
        {
            sb.Append(basePath);
            sb.Append('.');
        }

        sb.Append(segment);

        if (hasSegmentName)
        {
            sb.Append("[\"");
            sb.Append(segmentName);
            sb.Append("\"]");
        }

        return sb.ToString();
    }

    public static void InitializeLookupDictionary<TPart, TPartKey>
    (
        IEnumerable<TPart?> parts,
        Func<TPart, TPartKey?> partKeySelector,
        Func<TPartKey, bool>? partKeyFilter,
        string partKeyPropertyName,
        string path,
        ApiInitializationCode duplicatePartCode,
        ApiInitializationContext context,
        out Dictionary<TPartKey, TPart>? lookupDictionary
    )
        where TPart : class
        where TPartKey : notnull
    {
        partKeyFilter ??= static _ => true;

        var keyedParts = parts
            .OfType<TPart>()
            .Select(part => (Part: part, Key: partKeySelector(part)))
            .Where(x => x.Key is not null && partKeyFilter(x.Key));

        lookupDictionary = keyedParts
            .GroupBy(x => x.Key!)
            .Where(g => g.Count() == 1)
            .ToDictionary(g => g.Key, g => g.Single().Part);

        ValidateUnique(
            parts: parts,
            partKeySelector: partKeySelector,
            partKeyFilter: partKeyFilter,
            partKeyPropertyName: partKeyPropertyName,
            path: path,
            duplicatePartCode: duplicatePartCode,
            context: context);
    }

    public static bool IsNameInvalid(string? name)
    {
        return string.IsNullOrWhiteSpace(name);
    }

    public static bool IsNameValid(string? name)
    {
        return !IsNameInvalid(name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNotInitialized<T>(this object obj, T? value)
        where T : class
    {
        if (value is not null)
        {
            return value;
        }

        var objectType = obj.GetType();
        throw new ApiSchemaException($"{objectType.SafeToName()} has not been initialized. Initialize the {nameof(ApiSchema)} before accessing.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNotInitialized<T>(this object obj, T? value)
        where T : struct
    {
        if (value.HasValue)
        {
            return value.Value;
        }

        var objectType = obj.GetType();
        throw new ApiSchemaException($"{objectType.SafeToName()} has not been initialized. Initialize the {nameof(ApiSchema)} before accessing.");
    }

    public static void ValidateUnique<TPart, TPartKey>
    (
        IEnumerable<TPart?> parts,
        Func<TPart, TPartKey?> partKeySelector,
        Func<TPartKey, bool>? partKeyFilter,
        string partKeyPropertyName,
        string path,
        ApiInitializationCode duplicatePartCode,
        ApiInitializationContext context
    )
        where TPart : class
        where TPartKey : notnull
    {
        partKeyFilter ??= static _ => true;

        var duplicates = parts
            .OfType<TPart>()
            .Select(part => (Part: part, Key: partKeySelector(part)))
            .Where(x => x.Key is not null && partKeyFilter(x.Key))
            .GroupBy(x => x.Key!)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count == 0)
        {
            return;
        }

        var duplicatesString = duplicates.SafeToDelimitedString(',');
        var partTypeName = typeof(TPart).SafeToName();

        var severity = ApiInitializationSeverity.Error;
        var description = $"Duplicate {partTypeName}.{partKeyPropertyName} values: '{duplicatesString}'";
        var remediation = $"Verify that each {partTypeName} has a unique {partKeyPropertyName} value";

        context.AddIssue(path, severity, duplicatePartCode, description, remediation);
    }

    public static int? CountKeyLeaves(ApiKeyType keyType)
    {
        // Each ApiKeyPath in a key type corresponds to exactly one scalar leaf.
        return keyType.ApiKeyPaths.Length;
    }

    public static bool AreKeyTypesCompatible(ApiKeyType principalKeyType, ApiKeyType foreignKeyType)
        => TryAreKeyTypesCompatible(principalKeyType, foreignKeyType, out var compatible) && compatible;

    public static bool TryAreKeyTypesCompatible(ApiKeyType principalKeyType, ApiKeyType foreignKeyType, out bool compatible)
    {
        compatible = false;

        if (!TryGetKeyLeafTypes(principalKeyType, out var principalLeafTypes) ||
            !TryGetKeyLeafTypes(foreignKeyType, out var foreignLeafTypes))
        {
            return false;
        }

        if (principalLeafTypes.Length != foreignLeafTypes.Length)
        {
            return true;
        }

        for (var i = 0; i < principalLeafTypes.Length; i++)
        {
            if (principalLeafTypes[i] != foreignLeafTypes[i])
            {
                return true;
            }
        }

        compatible = true;
        return true;
    }

    public static string DescribeKeyLeafTypes(ApiKeyType keyType)
    {
        if (!TryGetKeyLeafTypes(keyType, out var leafTypes))
        {
            return "(unresolved)";
        }

        return string.Join(", ", leafTypes.Select(static t => t.SafeToName()));
    }

    private static bool TryGetKeyLeafTypes(ApiKeyType keyType, out Type[] leafTypes)
    {
        var paths = keyType.ApiKeyPaths;
        leafTypes = new Type[paths.Length];

        for (var i = 0; i < paths.Length; i++)
        {
            var scalarSegment = paths[i].ApiSegments.Length > 0 ? paths[i].ApiScalarSegment : null;
            if (scalarSegment?.IsPropertyResolved != true || !scalarSegment.ApiProperty.IsResolved)
            {
                leafTypes = [];
                return false;
            }

            leafTypes[i] = scalarSegment.ApiProperty.ApiType.ClrType;
        }

        return true;
    }
    #endregion
}
