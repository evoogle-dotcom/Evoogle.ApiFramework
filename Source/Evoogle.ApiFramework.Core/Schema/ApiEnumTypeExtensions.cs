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
    ///     Gets an <see cref="ApiEnumValue"/> by its API name.
    /// </summary>
    /// <param name="apiEnumType">The API enum type to search.</param>
    /// <param name="apiName">The API name of the enum value to retrieve.</param>
    /// <returns>The <see cref="ApiEnumValue"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no enum value with the specified API name exists in the enum type.
    ///     The exception message includes a list of all valid enum value API names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the enum value by its API name.
    ///     Use <see cref="ApiEnumType.TryGetValueByApiName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiEnumValue GetValueByApiName(this ApiEnumType apiEnumType, string apiName)
    {
        if (apiEnumType.TryGetValueByApiName(apiName, out var apiEnumValue))
        {
            return apiEnumValue!;
        }

        var validValuesByApiName = string.Join(',', apiEnumType.ApiEnumValues.OrderBy(v => v.ApiName).Select(v => v.ApiName));
        var errorMessage =
            $"{nameof(ApiEnumValue)} with {nameof(ApiEnumValue.ApiName)} '{apiName.SafeToString()}' not found in {apiEnumType.SafeToString()}. " +
            $"Valid {nameof(ApiEnumValue)} by {nameof(ApiEnumValue.ApiName)} are: {validValuesByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiEnumValue"/> by its CLR name.
    /// </summary>
    /// <param name="apiEnumType">The API enum type to search.</param>
    /// <param name="clrName">The CLR name of the enum value to retrieve.</param>
    /// <returns>The <see cref="ApiEnumValue"/> with the specified CLR name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no enum value with the specified CLR name exists in the enum type.
    ///     The exception message includes a list of all valid enum value CLR names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the enum value by its CLR name,
    ///     which corresponds to the actual enumeration member name in the CLR enum type.
    ///     Use <see cref="ApiEnumType.TryGetValueByClrName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiEnumValue GetValueByClrName(this ApiEnumType apiEnumType, string clrName)
    {
        if (apiEnumType.TryGetValueByClrName(clrName, out var apiEnumValue))
        {
            return apiEnumValue!;
        }

        var validValuesByClrName = string.Join(',', apiEnumType.ApiEnumValues.OrderBy(v => v.ClrName).Select(v => v.ClrName));
        var errorMessage =
            $"{nameof(ApiEnumValue)} with {nameof(ApiEnumValue.ClrName)} '{clrName}' not found in {apiEnumType.SafeToString()}. " +
            $"Valid {nameof(ApiEnumValue)} by {nameof(ApiEnumValue.ClrName)} are: {validValuesByClrName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiEnumValue"/> by its CLR ordinal value.
    /// </summary>
    /// <param name="apiEnumType">The API enum type to search.</param>
    /// <param name="clrOrdinal">The CLR ordinal (numeric value) of the enum value to retrieve.</param>
    /// <returns>The <see cref="ApiEnumValue"/> with the specified CLR ordinal.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no enum value with the specified CLR ordinal exists in the enum type.
    ///     The exception message includes a list of all valid enum value CLR ordinals.
    /// </exception>
    /// <remarks>
    ///     The CLR ordinal corresponds to the underlying integer value of the enumeration member.
    ///     Use <see cref="ApiEnumType.TryGetValueByClrOrdinal"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiEnumValue GetValueByClrOrdinal(this ApiEnumType apiEnumType, int clrOrdinal)
    {
        if (apiEnumType.TryGetValueByClrOrdinal(clrOrdinal, out var apiEnumValue))
        {
            return apiEnumValue!;
        }

        var validValuesByClrOrdinal = string.Join(',', apiEnumType.ApiEnumValues.OrderBy(v => v.ClrOrdinal).Select(v => v.ClrOrdinal));
        var errorMessage =
            $"{nameof(ApiEnumValue)} with {nameof(ApiEnumValue.ClrOrdinal)} '{clrOrdinal}' not found in {apiEnumType.SafeToString()}. " +
            $"Valid {nameof(ApiEnumValue)} by {nameof(ApiEnumValue.ClrOrdinal)} are: {validValuesByClrOrdinal}.";
        throw new ApiSchemaException(errorMessage);
    }
    #endregion
}
