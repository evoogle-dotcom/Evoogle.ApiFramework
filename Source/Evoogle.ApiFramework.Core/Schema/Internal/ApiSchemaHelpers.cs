// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;
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
    public static string BuildPath(string? apiParentPath, string apiChildPath, string? apiApiName)
    {
        var hasParent = !string.IsNullOrWhiteSpace(apiParentPath);
        var hasName = !string.IsNullOrWhiteSpace(apiApiName);

        if (!hasParent && !hasName)
        {
            return apiChildPath;
        }

        var capacity = (hasParent ? apiParentPath!.Length + 1 : 0)
            + apiChildPath.Length
            + (hasName ? apiApiName!.Length + 4 : 0);

        var sb = new StringBuilder(capacity);

        if (hasParent)
        {
            sb.Append(apiParentPath);
            sb.Append('.');
        }

        sb.Append(apiChildPath);

        if (hasName)
        {
            sb.Append("[\"");
            sb.Append(apiApiName);
            sb.Append("\"]");
        }

        return sb.ToString();
    }

    // /// <summary>
    // ///     Detects the target CLR type for converting a property value to an <see cref="Identity.ApiId"/> scalar.
    // /// </summary>
    // /// <param name="clrType">The CLR type of the property.</param>
    // /// <returns>
    // ///     The target CLR type for type coercion. Common types include <see cref="int"/>, <see cref="long"/>,
    // ///     <see cref="Guid"/>, <see cref="Ulid"/>, <see cref="CultureInfo"/>,
    // ///     or <see cref="string"/> as a fallback.
    // /// </returns>
    // public static Type DetectApiIdScalarClrType(Type clrType)
    // {
    //     ArgumentNullException.ThrowIfNull(clrType);

    //     // Map common CLR types to ApiId-compatible scalar types
    //     if (clrType == typeof(int))
    //     {
    //         return typeof(int);
    //     }

    //     if (clrType == typeof(long))
    //     {
    //         return typeof(long);
    //     }

    //     if (clrType == typeof(Guid))
    //     {
    //         return typeof(Guid);
    //     }

    //     if (clrType == typeof(Ulid))
    //     {
    //         return typeof(Ulid);
    //     }

    //     if (clrType == typeof(CultureInfo))
    //     {
    //         return typeof(CultureInfo);
    //     }

    //     // Default fallback to string for all other types
    //     return typeof(string);
    // }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNotInitialized<T>(this object obj, T? value)
    {
        if (value is not null)
        {
            return value;
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
