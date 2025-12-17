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
    public bool TryBuildIdentity(object clrInstance, out ApiId id, string? apiIdentityName = null)
    {
        id = default;
        if (clrInstance is null || !this.HasIdentity)
        {
            return false;
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            return false;
        }

        return this.BuildIdentityFromInstance(identity, clrInstance, out id);
    }

    public bool TryBuildIdentity(IReadOnlyDictionary<string, object?> values, out ApiId id, string? apiIdentityName = null)
    {
        id = default;
        if (values is null || !this.HasIdentity)
        {
            return false;
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            return false;
        }

        return this.BuildIdentityFromValues(identity, values, out id);
    }

    private ApiIdentity? ResolveIdentityForBuild(string? apiIdentityName)
    {
        if (!string.IsNullOrWhiteSpace(apiIdentityName))
        {
            return this.TryGetIdentityByApiName(apiIdentityName, out var id) ? id : null;
        }

        return this.ApiIdentitySet!.ApiPrimaryIdentity;
    }

    private static ApiId FinalizeComposite(List<ApiIdPart> parts)
        => parts.Count switch
        {
            0 => ApiId.Empty,
            1 => parts[0].Value,
            _ => ApiId.Composite(parts)
        };

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

        // Handle null values according to the identity's null handling configuration
        if (coercedValue is null)
        {
            var nullHandling = this.ApiOptions.GetIdentityNullHandling(this);
            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Property '{part.ApiProperty.ApiName}' has a null value for identity '{identity.ApiName}' on type '{clrInstance.GetType().Name}'. Null values are not allowed with {nameof(ApiIdentityNullHandling.ThrowException)} configured.");
            }

            return ApiId.Empty;
        }

        // Convert the typed value to ApiId
        return ConvertToApiId(coercedValue, targetType, part.ApiProperty, identity, clrInstance);
    }

    private static ApiId ConvertToApiId(object value, Type targetType, ApiProperty property, ApiIdentity identity, object clrInstance)
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
                $"Failed to convert property '{property.ApiName}' value of type '{value.GetType().Name}' to ApiId for identity '{identity.ApiName}' on type '{clrInstance.GetType().Name}'.",
                ex);
        }
    }

    private bool BuildIdentityFromInstance(ApiIdentity identity, object clrInstance, out ApiId id)
    {
        id = default;

        try
        {
            // Disallow mixed named/ordered parts (already validated during initialization)
            var anyOrdered = identity.ApiIdentityParts.Any(p => p.EmitAsOrdered);
            var anyNamed = identity.ApiIdentityParts.Any(p => !p.EmitAsOrdered);
            if (anyOrdered && anyNamed)
            {
                return false;
            }

            var parts = new List<ApiIdPart>(identity.ApiIdentityParts.Length);
            foreach (var part in identity.ApiIdentityParts)
            {
                // Get the property value using compiled accessors
                if (!part.ApiProperty.TryGetValue(clrInstance, out var rawValue))
                {
                    return false;
                }

                // Materialize the ApiId using TypeCoercion and pre-resolved target type
                var partId = this.MaterializeApiIdFromProperty(part, rawValue, identity, clrInstance, identity.ApiSchemaContext);

                // Only allow Empty if null handling is ReturnEmpty
                var nullHandling = this.ApiOptions.GetIdentityNullHandling(this);
                if (!partId.HasValue && nullHandling != ApiIdentityNullHandling.ReturnEmpty)
                {
                    return false;
                }

                parts.Add(part.EmitAsOrdered
                    ? ApiIdPart.Create(partId)
                    : ApiIdPart.Create(part.ApiProperty.ApiName, partId));
            }

            id = FinalizeComposite(parts);
            return id.HasValue;
        }
        catch (ApiIdentityException)
        {
            // Re-throw identity exceptions as-is
            throw;
        }
        catch (Exception)
        {
            // Swallow other exceptions and return false for try pattern
            return false;
        }
    }

    private bool BuildIdentityFromValues(ApiIdentity identity, IReadOnlyDictionary<string, object?> values, out ApiId id)
    {
        id = default;

        try
        {
            var anyOrdered = identity.ApiIdentityParts.Any(p => p.EmitAsOrdered);
            var anyNamed = identity.ApiIdentityParts.Any(p => !p.EmitAsOrdered);
            if (anyOrdered && anyNamed)
            {
                return false;
            }

            var parts = new List<ApiIdPart>(identity.ApiIdentityParts.Length);
            foreach (var part in identity.ApiIdentityParts)
            {
                if (!values.TryGetValue(part.ApiPropertyName, out var rawValue))
                {
                    return false;
                }

                // Materialize the ApiId using TypeCoercion and pre-resolved target type
                // Use a dummy object for error reporting since we don't have the actual instance
                var dummyInstance = new { DictionaryValues = true };
                var partId = this.MaterializeApiIdFromProperty(part, rawValue, identity, dummyInstance, identity.ApiSchemaContext);

                // Only allow Empty if null handling is ReturnEmpty
                var nullHandling = this.ApiOptions.GetIdentityNullHandling(this);
                if (!partId.HasValue && nullHandling != ApiIdentityNullHandling.ReturnEmpty)
                {
                    return false;
                }

                parts.Add(part.EmitAsOrdered
                    ? ApiIdPart.Create(partId)
                    : ApiIdPart.Create(part.ApiPropertyName, partId));
            }

            id = FinalizeComposite(parts);
            return id.HasValue;
        }
        catch (ApiIdentityException)
        {
            // Re-throw identity exceptions as-is
            throw;
        }
        catch (Exception)
        {
            // Swallow other exceptions and return false for try pattern
            return false;
        }
    }
}
