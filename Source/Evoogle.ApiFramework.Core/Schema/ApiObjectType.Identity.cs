// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema;

public sealed partial class ApiObjectType
{
    #region Public Identity Methods

    /// <summary>
    ///     Attempts to build an identity from a CLR instance without throwing exceptions.
    /// </summary>
    /// <param name="clrInstance">The CLR instance to build the identity from.</param>
    /// <param name="id">When this method returns, contains the built identity if successful; otherwise, <see cref="ApiId.Empty"/>.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns><c>true</c> if the identity was built successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     This method never throws exceptions and returns <c>false</c> on any failure.
    ///     Use <see cref="BuildIdentity(object, string?)"/> if you need exception details.
    /// </remarks>
    public bool TryBuildIdentity(object clrInstance, out ApiId id, string? apiIdentityName = null)
    {
        id = default;

        try
        {
            id = this.BuildIdentity(clrInstance, apiIdentityName);
            return id.HasValue;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Attempts to build an identity from a dictionary of property values without throwing exceptions.
    /// </summary>
    /// <param name="values">The dictionary of property names to values.</param>
    /// <param name="id">When this method returns, contains the built identity if successful; otherwise, <see cref="ApiId.Empty"/>.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns><c>true</c> if the identity was built successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     This method never throws exceptions and returns <c>false</c> on any failure.
    ///     Use <see cref="BuildIdentity(IReadOnlyDictionary{string, object?}, string?)"/> if you need exception details.
    /// </remarks>
    public bool TryBuildIdentity(IReadOnlyDictionary<string, object?> values, out ApiId id, string? apiIdentityName = null)
    {
        id = default;

        try
        {
            id = this.BuildIdentity(values, apiIdentityName);
            return id.HasValue;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Attempts to build the primary identity from a CLR instance without throwing exceptions.
    /// </summary>
    /// <param name="clrInstance">The CLR instance to build the identity from.</param>
    /// <param name="id">When this method returns, contains the built identity if successful; otherwise, <see cref="ApiId.Empty"/>.</param>
    /// <returns><c>true</c> if the identity was built successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     This is a convenience method equivalent to calling <see cref="TryBuildIdentity(object, out ApiId, string?)"/> 
    ///     with <c>apiIdentityName</c> set to <c>null</c>.
    /// </remarks>
    public bool TryBuildPrimaryIdentity(object clrInstance, out ApiId id)
        => this.TryBuildIdentity(clrInstance, out id, apiIdentityName: null);

    /// <summary>
    ///     Builds an identity from a CLR instance.
    /// </summary>
    /// <param name="clrInstance">The CLR instance to build the identity from.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>The built identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrInstance"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no identity configured or the specified identity name is not found.</exception>
    /// <exception cref="ApiIdentityException">Thrown when type coercion fails or null handling requires throwing.</exception>
    public ApiId BuildIdentity(object clrInstance, string? apiIdentityName = null)
    {
        ArgumentNullException.ThrowIfNull(clrInstance);

        if (!this.HasIdentity)
        {
            throw new InvalidOperationException($"ApiObjectType '{this.ApiName}' has no identity configured.");
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
            throw new InvalidOperationException($"ApiObjectType '{this.ApiName}' does not have {identityRef}.");
        }

        return this.BuildIdentityFromInstance(identity, clrInstance);
    }

    /// <summary>
    ///     Builds an identity from a dictionary of property values.
    /// </summary>
    /// <param name="values">The dictionary of property names to values.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>The built identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no identity configured or the specified identity name is not found.</exception>
    /// <exception cref="ApiIdentityException">Thrown when type coercion fails, required properties are missing, or null handling requires throwing.</exception>
    public ApiId BuildIdentity(IReadOnlyDictionary<string, object?> values, string? apiIdentityName = null)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (!this.HasIdentity)
        {
            throw new InvalidOperationException($"ApiObjectType '{this.ApiName}' has no identity configured.");
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
            throw new InvalidOperationException($"ApiObjectType '{this.ApiName}' does not have {identityRef}.");
        }

        return this.BuildIdentityFromValues(identity, values);
    }

    /// <summary>
    ///     Builds the primary identity from a CLR instance.
    /// </summary>
    /// <param name="clrInstance">The CLR instance to build the identity from.</param>
    /// <returns>The built primary identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrInstance"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no primary identity configured.</exception>
    /// <exception cref="ApiIdentityException">Thrown when type coercion fails or null handling requires throwing.</exception>
    /// <remarks>
    ///     This is a convenience method equivalent to calling <see cref="BuildIdentity(object, string?)"/> 
    ///     with <c>apiIdentityName</c> set to <c>null</c>.
    /// </remarks>
    public ApiId BuildPrimaryIdentity(object clrInstance)
        => this.BuildIdentity(clrInstance, apiIdentityName: null);

    #endregion

    #region Implementation Methods
    private ApiId BuildIdentityFromInstance(ApiIdentity identity, object clrInstance)
    {
        var parts = new List<ApiIdPart>(identity.ApiIdentityParts.Length);

        foreach (var part in identity.ApiIdentityParts)
        {
            // Get the property value using compiled accessors
            if (!part.ApiProperty.TryGetValue(clrInstance, out var rawValue))
            {
                throw new ApiIdentityException(
                    $"Failed to read property '{part.ApiProperty.ApiName}' from type '{clrInstance.GetType().Name}' for identity '{identity.ApiName}'.");
            }

            // Materialize the ApiId using TypeCoercion and pre-resolved target type
            var partId = this.MaterializeApiIdFromProperty(part, rawValue, identity, clrInstance, identity.ApiSchemaContext);

            // Always create named parts (serialization layer decides ordered vs named formatting)
            parts.Add(ApiIdPart.Create(part.ApiProperty.ApiName, partId));
        }

        return FinalizeComposite(parts);
    }

    private ApiId BuildIdentityFromValues(ApiIdentity identity, IReadOnlyDictionary<string, object?> values)
    {
        var parts = new List<ApiIdPart>(identity.ApiIdentityParts.Length);

        foreach (var part in identity.ApiIdentityParts)
        {
            if (!values.TryGetValue(part.ApiPropertyName, out var rawValue))
            {
                throw new ApiIdentityException(
                    $"Property '{part.ApiPropertyName}' not found in values dictionary for identity '{identity.ApiName}'.");
            }

            // Materialize the ApiId using TypeCoercion and pre-resolved target type
            // Note: We don't have the actual CLR instance type, so error messages will reference the dictionary
            var partId = this.MaterializeApiIdFromPropertyValue(part, rawValue, identity, identity.ApiSchemaContext);

            // Always create named parts (serialization layer decides ordered vs named formatting)
            parts.Add(ApiIdPart.Create(part.ApiPropertyName, partId));
        }

        return FinalizeComposite(parts);
    }

    private ApiId MaterializeApiIdFromProperty(ApiIdentityPart part, object? rawValue, ApiIdentity identity, object clrInstance, ApiSchemaContext schemaContext)
    {
        // Use the pre-resolved target type from the identity part
        var targetType = part.ResolvedTargetType;

        // Use the schema's TypeCoercion to convert the raw value to the target type
        object? coercedValue;
        try
        {
            coercedValue = schemaContext.TypeCoercion.Coerce(rawValue, targetType, schemaContext.TypeCoercionContext);
        }
        catch (Exception ex)
        {
            throw new ApiIdentityException(
                $"Failed to coerce property '{part.ApiProperty.ApiName}' value to type '{targetType.Name}' for identity '{identity.ApiName}' on type '{clrInstance.GetType().Name}'.",
                ex);
        }

        // Handle null values according to the configured null handling
        if (coercedValue is null)
        {
            var nullHandling = this.GetIdentityNullHandling();
            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Property '{part.ApiProperty.ApiName}' has a null value for identity '{identity.ApiName}' on type '{clrInstance.GetType().Name}'. Null values are not allowed with {nameof(ApiIdentityNullHandling.ThrowException)} configured.");
            }

            return ApiId.Empty;
        }

        // Convert the typed value to ApiId
        return ConvertToApiId(coercedValue, targetType, part.ApiProperty.ApiName, identity.ApiName, clrInstance.GetType().Name);
    }

    private ApiId MaterializeApiIdFromPropertyValue(ApiIdentityPart part, object? rawValue, ApiIdentity identity, ApiSchemaContext schemaContext)
    {
        // Use the pre-resolved target type from the identity part
        var targetType = part.ResolvedTargetType;

        // Use the schema's TypeCoercion to convert the raw value to the target type
        object? coercedValue;
        try
        {
            coercedValue = schemaContext.TypeCoercion.Coerce(rawValue, targetType, schemaContext.TypeCoercionContext);
        }
        catch (Exception ex)
        {
            throw new ApiIdentityException(
                $"Failed to coerce property '{part.ApiPropertyName}' value to type '{targetType.Name}' for identity '{identity.ApiName}' from values dictionary.",
                ex);
        }

        // Handle null values according to the configured null handling
        if (coercedValue is null)
        {
            var nullHandling = this.GetIdentityNullHandling();
            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Property '{part.ApiPropertyName}' has a null value for identity '{identity.ApiName}' from values dictionary. Null values are not allowed with {nameof(ApiIdentityNullHandling.ThrowException)} configured.");
            }

            return ApiId.Empty;
        }

        // Convert the typed value to ApiId
        return ConvertToApiId(coercedValue, targetType, part.ApiPropertyName, identity.ApiName, "values dictionary");
    }

    private static ApiId ConvertToApiId(object value, Type targetType, string propertyName, string identityName, string contextDescription)
    {
        // Handle ApiId passthrough
        if (value is ApiId apiId)
        {
            return apiId;
        }

        // Convert based on the target type
        try
        {
            if (targetType == typeof(int))
            {
                return ApiId.FromInt32((int)value);
            }

            if (targetType == typeof(long))
            {
                return ApiId.FromInt64((long)value);
            }

            if (targetType == typeof(Guid))
            {
                return ApiId.FromGuid((Guid)value);
            }

            if (targetType == typeof(Ulid))
            {
                return ApiId.FromUlid((Ulid)value);
            }

            if (targetType == typeof(CultureInfo))
            {
                return ApiId.FromCulture((CultureInfo)value);
            }

            if (targetType == typeof(string))
            {
                return ApiId.FromString((string)value);
            }

            // Fallback: convert to string
            return ApiId.FromString(value.ToString() ?? string.Empty);
        }
        catch (Exception ex)
        {
            throw new ApiIdentityException(
                $"Failed to convert property '{propertyName}' value of type '{value.GetType().Name}' to ApiId for identity '{identityName}' on {contextDescription}.",
                ex);
        }
    }

    private static ApiId FinalizeComposite(List<ApiIdPart> parts)
        => parts.Count switch
        {
            0 => ApiId.Empty,
            1 => parts[0].Value,
            _ => ApiId.Composite(parts)
        };

    private ApiIdentityNullHandling GetIdentityNullHandling()
        => this.ApiOptions?.ApiIdentityNullHandling ?? this.ApiSchemaContext.ApiSchemaOptions.ApiIdentityNullHandling;

    private ApiIdentity? ResolveIdentityForBuild(string? apiIdentityName)
    {
        if (!string.IsNullOrWhiteSpace(apiIdentityName))
        {
            return this.TryGetIdentityByApiName(apiIdentityName, out var id) ? id : null;
        }

        return this.ApiIdentitySet!.ApiPrimaryIdentity;
    }
    #endregion
}
