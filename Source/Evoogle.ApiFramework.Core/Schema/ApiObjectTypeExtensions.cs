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
    ///     Retrieves an API property by its API name or throws if not found.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="apiName">The API name of the property.</param>
    /// <returns>The <see cref="ApiProperty"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the property is not found.</exception>
    public static ApiProperty GetPropertyByApiName(this ApiObjectType apiObjectType, string apiName)
    {
        if (apiObjectType.TryGetPropertyByApiName(apiName, out var apiProperty))
            return apiProperty!;

        throw new ApiSchemaException($"{nameof(ApiProperty)} with {nameof(ApiProperty.ApiName)} '{apiName}' was not found in {apiObjectType.SafeToString()}.");
    }

    /// <summary>
    ///     Retrieves an API property by its CLR name or throws if not found.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="clrName">The CLR name of the property.</param>
    /// <returns>The <see cref="ApiProperty"/> with the specified CLR name.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the property is not found.</exception>
    public static ApiProperty GetPropertyByClrName(this ApiObjectType apiObjectType, string clrName)
    {
        if (apiObjectType.TryGetPropertyByClrName(clrName, out var apiProperty))
            return apiProperty!;

        throw new ApiSchemaException($"{nameof(ApiProperty)} with {nameof(ApiProperty.ClrName)} '{clrName}' was not found in {apiObjectType.SafeToString()}.");
    }

    /// <summary>
    ///     Retrieves an API relationship by its API name or throws if not found.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="apiName">The API name of the relationship.</param>
    /// <returns>The <see cref="ApiRelationship"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the relationship is not found.</exception>
    public static ApiRelationship GetRelationshipByApiName(this ApiObjectType apiObjectType, string apiName)
    {
        if (apiObjectType.TryGetRelationshipByApiName(apiName, out var apiRelationship))
            return apiRelationship!;

        throw new ApiSchemaException($"{nameof(ApiRelationship)} with {nameof(ApiRelationship.ApiName)} '{apiName}' was not found in {apiObjectType.SafeToString()}.");
    }
    #endregion
}
