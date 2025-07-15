// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNotInitialized<T>(this object obj, T? value)
    {
        if (value is not null)
            return value;

        var typeName = obj.GetType().Name;
        throw new ApiSchemaException($"{typeName} has not been initialized. Initialize the schema before accessing.");
    }

    public static bool ValidateUnique<TPart, TPartKey>(IEnumerable<TPart> parts, Func<TPart, TPartKey> partKeySelector, string validationPath, string partKeyPropertyName, ref List<ValidationResult>? results)
    {
        var duplicates = parts
            .GroupBy(partKeySelector)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count == 0)
            return false;

        var duplicatesString = duplicates.SafeToDelimitedString(',');
        var partTypeName = typeof(TPart).Name;
        var message = $"{validationPath}.{partTypeName} unable to initialize because duplicate {partKeyPropertyName} values detected: {duplicatesString}";

        results ??= [];
        results.Add(new ValidationResult(message));
        return true;
    }
    #endregion
}
