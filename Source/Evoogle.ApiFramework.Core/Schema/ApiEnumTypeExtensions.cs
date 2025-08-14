// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for <see cref="ApiEnumType"/> class.
/// </summary>
public static class ApiEnumTypeExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Retrieves an API enum value by its API name or throws if not found.
    /// </summary>
    /// <param name="apiEnumType">The API enum type to search.</param>
    /// <param name="apiName">The API name of the property.</param>
    /// <returns>The <see cref="ApiEnumValue"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the property is not found.</exception>
    public static ApiEnumValue GetValueByApiName(this ApiEnumType apiEnumType, string apiName)
    {
        if (apiEnumType.TryGetValueByApiName(apiName, out var apiEnumValue))
        {
            return apiEnumValue!;
        }

        throw new ApiSchemaException($"{nameof(ApiEnumValue)} with {nameof(ApiEnumValue.ApiName)} '{apiName}' was not found in {apiEnumType.SafeToString()}.");
    }

    /// <summary>
    ///     Retrieves an API enum value by its CLR name or throws if not found.
    /// </summary>
    /// <param name="apiEnumType">The API enum type to search.</param>
    /// <param name="clrName">The CLR name of the property.</param>
    /// <returns>The <see cref="ApiEnumValue"/> with the specified CLR name.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the property is not found.</exception>
    public static ApiEnumValue GetValueByClrName(this ApiEnumType apiEnumType, string clrName)
    {
        if (apiEnumType.TryGetValueByClrName(clrName, out var apiEnumValue))
        {
            return apiEnumValue!;
        }

        throw new ApiSchemaException($"{nameof(ApiEnumValue)} with {nameof(ApiEnumValue.ClrName)} '{clrName}' was not found in {apiEnumType.SafeToString()}.");
    }

    /// <summary>
    ///     Retrieves an API enum value by its CLR ordinal or throws if not found.
    /// </summary>
    /// <param name="apiEnumType">The API enum type to search.</param>
    /// <param name="clrOrdinal">The CLR ordinal of the property.</param>
    /// <returns>The <see cref="ApiEnumValue"/> with the specified CLR ordinal.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the property is not found.</exception>
    public static ApiEnumValue GetValueByClrOrdinal(this ApiEnumType apiEnumType, int clrOrdinal)
    {
        if (apiEnumType.TryGetValueByClrOrdinal(clrOrdinal, out var apiEnumValue))
        {
            return apiEnumValue!;
        }

        throw new ApiSchemaException($"{nameof(ApiEnumValue)} with {nameof(ApiEnumValue.ClrOrdinal)} '{clrOrdinal}' was not found in {apiEnumType.SafeToString()}.");
    }
    #endregion
}
