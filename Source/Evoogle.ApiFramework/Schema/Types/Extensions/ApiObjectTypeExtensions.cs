// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Extension methods for <see cref="ApiObjectType"/> class.
/// </summary>
public static class ApiObjectTypeExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Gets an <see cref="ApiNamedKeyDefinition"/> by its API name.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="apiName">The API name of the key to retrieve.</param>
    /// <returns>The <see cref="ApiNamedKeyDefinition"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no key with the specified API name exists in the object type.
    ///     The exception message includes a list of all available key API names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the key by its API name.
    ///     Use <see cref="ApiObjectType.TryGetKeyByApiName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiNamedKeyDefinition GetKeyByApiName
    (
        this ApiObjectType apiObjectType,
        string apiName
    )
    {
        if (apiObjectType.TryGetKeyByApiName(apiName, out var apiKey))
        {
            return apiKey;
        }

        var availableKeysByApiName = string.Join(',', apiObjectType.ApiKeyApiNames.OrderBy(k => k));
        var errorMessage =
            $"{nameof(ApiNamedKeyDefinition)} with name '{apiName.SafeToString()}' not found in " +
            $"{apiObjectType.SafeToString()}. Available {nameof(ApiNamedKeyDefinition)} names are: " +
            $"{availableKeysByApiName}.";
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
            return apiProperty;
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
            return apiProperty;
        }

        var availablePropertiesByClrName = string.Join(',', apiObjectType.ApiProperties.OrderBy(p => p.ClrName).Select(p => p.ClrName));
        var errorMessage = $"{nameof(ApiProperty)} with {nameof(ApiProperty.ClrName)} '{clrName.SafeToString()}' not found in {apiObjectType.SafeToString()}. " +
            $"Available {nameof(ApiProperty)} by {nameof(ApiProperty.ClrName)} are: {availablePropertiesByClrName}.";
        throw new ApiSchemaException(errorMessage);
    }
    #endregion
}
