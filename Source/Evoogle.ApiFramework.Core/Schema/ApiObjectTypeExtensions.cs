// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for <see cref="ApiObjectType"/> class.
/// </summary>
public static class ApiObjectTypeExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Builds an identity from a CLR instance.
    /// </summary>
    /// <param name="apiObjectType">The API object type to build the identity for.</param>
    /// <param name="clrInstance">The CLR instance to build the identity from.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>The built identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrInstance"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no identity configured or the specified identity name is not found.</exception>
    /// <exception cref="ApiIdentityException">Thrown when type coercion fails or null handling requires throwing.</exception>
    public static ApiId BuildIdentity(this ApiObjectType apiObjectType, object clrInstance, string? apiIdentityName = null)
    {
        ArgumentNullException.ThrowIfNull(clrInstance);

        if (!apiObjectType.TryBuildIdentity(clrInstance, out var id, apiIdentityName))
        {
            // Provide detailed diagnostics for the throwing version
            if (!apiObjectType.HasIdentity)
            {
                throw new InvalidOperationException($"ApiObjectType '{apiObjectType.ApiName}' has no identity configured.");
            }

            var identity = apiObjectType.ResolveIdentityForBuild(apiIdentityName);
            if (identity is null)
            {
                var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
                throw new InvalidOperationException($"ApiObjectType '{apiObjectType.ApiName}' does not have {identityRef}.");
            }

            // If we got here, something else failed during building
            throw new ApiIdentityException($"Failed to build identity for '{apiObjectType.ApiName}'.");
        }

        return id;
    }

    /// <summary>
    ///     Builds an identity from a dictionary of property values.
    /// </summary>
    /// <param name="apiObjectType">The API object type to build the identity for.</param>
    /// <param name="values">The dictionary of property names to values.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>The built identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no identity configured or the specified identity name is not found.</exception>
    /// <exception cref="ApiIdentityException">Thrown when type coercion fails, required properties are missing, or null handling requires throwing.</exception>
    public static ApiId BuildIdentity(this ApiObjectType apiObjectType, IReadOnlyDictionary<string, object?> values, string? apiIdentityName = null)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (!apiObjectType.TryBuildIdentity(values, out var id, apiIdentityName))
        {
            if (!apiObjectType.HasIdentity)
            {
                throw new InvalidOperationException($"ApiObjectType '{apiObjectType.ApiName}' has no identity configured.");
            }

            var identity = apiObjectType.ResolveIdentityForBuild(apiIdentityName);
            if (identity is null)
            {
                var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
                throw new InvalidOperationException($"ApiObjectType '{apiObjectType.ApiName}' does not have {identityRef}.");
            }

            throw new ApiIdentityException($"Failed to build identity for '{apiObjectType.ApiName}' from values dictionary.");
        }

        return id;
    }

    /// <summary>
    ///     Builds the primary identity from a CLR instance.
    /// </summary>
    /// <param name="apiObjectType">The API object type to build the identity for.</param>
    /// <param name="clrInstance">The CLR instance to build the identity from.</param>
    /// <returns>The built primary identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrInstance"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no primary identity configured.</exception>
    /// <exception cref="ApiIdentityException">Thrown when type coercion fails or null handling requires throwing.</exception>
    /// <remarks>
    ///     This is a convenience method equivalent to calling <see cref="BuildIdentity(object, string?)"/> 
    ///     with <c>apiIdentityName</c> set to <c>null</c>.
    /// </remarks>
    public static ApiId BuildPrimaryIdentity(this ApiObjectType apiObjectType, object clrInstance)
        => apiObjectType.BuildIdentity(clrInstance, apiIdentityName: null);

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

    /// <summary>
    ///     Gets an <see cref="ApiRelationship"/> by its API name.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="apiName">The API name of the relationship to retrieve.</param>
    /// <returns>The <see cref="ApiRelationship"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no relationship with the specified API name exists in the object type.
    ///     The exception message includes a list of all available relationship API names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the relationship by its API name.
    ///     Use <see cref="ApiObjectType.TryGetRelationshipByApiName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiRelationship GetRelationshipByApiName(this ApiObjectType apiObjectType, string apiName)
    {
        if (apiObjectType.TryGetRelationshipByApiName(apiName, out var apiRelationship))
        {
            return apiRelationship!;
        }

        var availableRelationshipsByApiName = string.Join(',', apiObjectType.ApiRelationships.OrderBy(r => r.ApiName).Select(r => r.ApiName));
        var errorMessage = $"{nameof(ApiRelationship)} with {nameof(ApiRelationship.ApiName)} '{apiName.SafeToString()}' not found in {apiObjectType.SafeToString()}. " +
            $"Available {nameof(ApiRelationship)} by {nameof(ApiRelationship.ApiName)} are: {availableRelationshipsByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Attempts to build the primary identity from a CLR instance without throwing exceptions.
    /// </summary>
    /// <param name="apiObjectType">The API object type to build the identity for.</param>
    /// <param name="clrInstance">The CLR instance to build the identity from.</param>
    /// <param name="id">When this method returns, contains the built identity if successful; otherwise, <see cref="ApiId.Empty"/>.</param>
    /// <returns><c>true</c> if the identity was built successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     This is a convenience method equivalent to calling <see cref="TryBuildIdentity(object, out ApiId, string?)"/> 
    ///     with <c>apiIdentityName</c> set to <c>null</c>.
    /// </remarks>
    public static bool TryBuildPrimaryIdentity(this ApiObjectType apiObjectType, object clrInstance, out ApiId id)
        => apiObjectType.TryBuildIdentity(clrInstance, out id, apiIdentityName: null);
    #endregion
}
