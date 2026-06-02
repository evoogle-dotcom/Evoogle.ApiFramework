// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for <see cref="ApiObjectType"/> class.
/// </summary>
public static class ApiObjectTypeExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Gets an <see cref="ApiKeyType"/> by its API name.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="apiName">The API name of the identity to retrieve.</param>
    /// <returns>The <see cref="ApiKeyType"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no key type with the specified API name exists in the object type.
    ///     The exception message includes a list of all available key type API names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the key type by its API name.
    ///     Use <see cref="ApiObjectType.TryGetKeyTypeByApiName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiKeyType GetKeyTypeByApiName(this ApiObjectType apiObjectType, string apiName)
    {
        if (apiObjectType.TryGetKeyTypeByApiName(apiName, out var apiKeyType))
        {
            return apiKeyType;
        }

        var availableKeyTypesByApiName = string.Join(',', apiObjectType.ApiKeyTypes.OrderBy(i => i.ApiName).Select(i => i.ApiName));
        var errorMessage =
            $"{nameof(ApiKeyType)} with {nameof(ApiKeyType.ApiName)} '{apiName.SafeToString()}' not found in {apiObjectType.SafeToString()}. " +
            $"Available {nameof(ApiKeyType)} by {nameof(ApiKeyType.ApiName)} are: {availableKeyTypesByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiProperty"/> by its API name.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="apiName">The API name of the property to retrieve.</param>
    /// <returns>The <see cref="ApiProperty"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no property with the specified API name exists in the object type.
    ///     The exception message includes a list of all available property API names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the property by its API name.
    ///     Use <see cref="ApiObjectType.TryGetPropertyByApiName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiProperty GetPropertyByApiName(this ApiObjectType apiObjectType, string apiName)
    {
        if (apiObjectType.TryGetPropertyByApiName(apiName, out var apiProperty))
        {
            return apiProperty!;
        }

        var availablePropertiesByApiName = string.Join(',', apiObjectType.ApiProperties.OrderBy(p => p.ApiName).Select(p => p.ApiName));
        var errorMessage =
            $"{nameof(ApiProperty)} with {nameof(ApiProperty.ApiName)} '{apiName.SafeToString()}' not found in {apiObjectType.SafeToString()}. " +
            $"Available {nameof(ApiProperty)} by {nameof(ApiProperty.ApiName)} are: {availablePropertiesByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiProperty"/> by its CLR name.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="clrName">The CLR name of the property to retrieve.</param>
    /// <returns>The <see cref="ApiProperty"/> with the specified CLR name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no property with the specified CLR name exists in the object type.
    ///     The exception message includes a list of all available property CLR names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the property by its CLR name,
    ///     which corresponds to the actual field or property name in the CLR type.
    ///     Use <see cref="ApiObjectType.TryGetPropertyByClrName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiProperty GetPropertyByClrName(this ApiObjectType apiObjectType, string clrName)
    {
        if (apiObjectType.TryGetPropertyByClrName(clrName, out var apiProperty))
        {
            return apiProperty!;
        }

        var availablePropertiesByClrName = string.Join(',', apiObjectType.ApiProperties.OrderBy(p => p.ClrName).Select(p => p.ClrName));
        var errorMessage = $"{nameof(ApiProperty)} with {nameof(ApiProperty.ClrName)} '{clrName.SafeToString()}' not found in {apiObjectType.SafeToString()}. " +
            $"Available {nameof(ApiProperty)} by {nameof(ApiProperty.ClrName)} are: {availablePropertiesByClrName}.";
        throw new ApiSchemaException(errorMessage);
    }
    #endregion
}
