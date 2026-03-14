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
        IEnumerable<TPart> parts,
        Func<TPart, TPartKey> partKeySelector,
        Func<TPartKey, bool>? partKeyFilter,
        string partKeyPropertyName,
        string path,
        ApiInitializationCode code,
        ApiInitializationContext context,
        out Dictionary<TPartKey, TPart>? lookupDictionary
    )
        where TPartKey : notnull
    {
        partKeyFilter ??= (_ => true);

        lookupDictionary = parts
            .Where(p => p is not null)
            .GroupBy(partKeySelector)
            .Where(g => partKeyFilter(g.Key))
            .Where(g => g.Count() == 1)
            .ToDictionary(g => g.Key, g => g.Single());

        ValidateUnique
        (
            parts: parts,
            partKeySelector: partKeySelector,
            partKeyFilter: partKeyFilter,
            partKeyPropertyName: partKeyPropertyName,
            path: path,
            code: code,
            context: context
        );
    }

    public static bool IsNameInvalid(string name)
    {
        return string.IsNullOrWhiteSpace(name);
    }

    public static bool IsNameValid(string name)
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
        IEnumerable<TPart> parts,
        Func<TPart, TPartKey> partKeySelector,
        Func<TPartKey, bool>? partKeyFilter,
        string partKeyPropertyName,
        string path,
        ApiInitializationCode code,
        ApiInitializationContext context
    )
        where TPartKey : notnull
    {
        partKeyFilter ??= (_ => true);

        var duplicates = parts
            .Where(p => p is not null)
            .GroupBy(partKeySelector)
            .Where(g => partKeyFilter(g.Key))
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

        context.AddIssue(path, severity, code, description, remediation);
    }
    #endregion
}
