// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
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
    /// <summary>
    ///     Validates that the specified key selector produces unique values for creating a new entity.
    ///     This is used to ensure that the entity can be created without conflicts.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <typeparam name="TPart">The type of the part within the entity.</typeparam>
    /// <typeparam name="TPartKey">The type of the key selected from each part.</typeparam>
    /// <param name="parts">The collection of entity parts to check.</param>
    /// <param name="partKeySelector">A function to extract the key to test for uniqueness.</param>
    /// <param name="partKeyPropertyName">The name of the property being validated (for error messages).</param>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when duplicate key values are found for the specified entity.
    /// </exception>
    public static void ValidateUnique<T, TPart, TPartKey>(IEnumerable<TPart> parts, Func<TPart, TPartKey> partKeySelector, string partKeyPropertyName)
    {
        var duplicates = parts
            .GroupBy(partKeySelector)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count == 0)
            return;

        var duplicateKeysString = duplicates.SafeToDelimitedString(',');
        var typeName = typeof(T).Name;
        var partTypeName = typeof(TPart).Name;
        var message = $"Unable to create {typeName} because duplicate {partTypeName}.{partKeyPropertyName} values detected: {duplicateKeysString}";
        throw new ApiSchemaException(message);
    }
    #endregion
}
