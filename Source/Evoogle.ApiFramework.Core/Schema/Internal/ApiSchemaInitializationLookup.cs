// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiSchemaInitializationLookup
{
    #region Utility Methods
    public static void InitializeLookupDictionary<TPart, TPartKey>
    (
        IEnumerable<TPart?> parts,
        Func<TPart, TPartKey?> partKeySelector,
        Func<TPartKey, bool>? partKeyFilter,
        string partKeyPropertyName,
        string apiPath,
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
            apiPath: apiPath,
            duplicatePartCode: duplicatePartCode,
            context: context);
    }

    public static void ValidateUnique<TPart, TPartKey>
    (
        IEnumerable<TPart?> parts,
        Func<TPart, TPartKey?> partKeySelector,
        Func<TPartKey, bool>? partKeyFilter,
        string partKeyPropertyName,
        string apiPath,
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

        context.AddIssue(apiPath, severity, duplicatePartCode, description, remediation);
    }
    #endregion
}
