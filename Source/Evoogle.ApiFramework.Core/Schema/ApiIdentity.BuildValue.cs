// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema;

public sealed partial class ApiIdentity
{
    #region BuildValue Methods
    /// <summary>
    ///     Builds an <see cref="ApiIdentityValue"/> by extracting property values from a CLR object instance.
    /// </summary>
    /// <param name="context">The build context containing the CLR instance and configuration.</param>
    /// <returns>
    ///     An <see cref="ApiIdentityValue"/> representing the resolved identity of the given CLR instance.
    /// </returns>
    /// <exception cref="ApiIdentityException">
    ///     Thrown when <see cref="ApiIdentityValueBuildContext.NullHandling"/> is
    ///     <see cref="ApiIdentityNullHandling.ThrowException"/> and a required property value is <see langword="null"/>.
    /// </exception>
    public ApiIdentityValue BuildValue(ApiIdentityValueBuildContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Fast path: single-scalar identity avoids pre-allocating the intermediate parts array.
        if (this.IsScalarDefinition)
        {
            var singleScalarPart = (ApiIdentityScalarPart)this.ApiIdentityParts[0];
            return ApiIdentityValue.Composite([BuildScalarPartValue(singleScalarPart, context)]);
        }

        var parts = new ApiIdentityPartValue[this.ApiIdentityParts.Length];
        for (var i = 0; i < this.ApiIdentityParts.Length; i++)
        {
            parts[i] = BuildPartValue(this.ApiIdentityParts[i], context);
        }

        return ApiIdentityValue.Composite(parts);
    }

    /// <summary>
    ///     Builds an <see cref="ApiIdentityValue"/> from pre-extracted property name/value pairs.
    /// </summary>
    /// <param name="context">The build context containing the values dictionary and configuration.</param>
    /// <returns>
    ///     An <see cref="ApiIdentityValue"/> representing the resolved identity from the given values.
    /// </returns>
    /// <exception cref="ApiIdentityException">
    ///     Thrown when <see cref="ApiIdentityValueBuildFromValuesContext.NullHandling"/> is
    ///     <see cref="ApiIdentityNullHandling.ThrowException"/> and a required value is missing or <see langword="null"/>.
    /// </exception>
    public ApiIdentityValue BuildValue(ApiIdentityValueBuildFromValuesContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Fast path: single-scalar identity avoids pre-allocating the intermediate parts array.
        if (this.IsScalarDefinition)
        {
            var singleScalarPart = (ApiIdentityScalarPart)this.ApiIdentityParts[0];
            return ApiIdentityValue.Composite([BuildScalarPartValueFromValues(singleScalarPart, context)]);
        }

        var parts = new ApiIdentityPartValue[this.ApiIdentityParts.Length];
        for (var i = 0; i < this.ApiIdentityParts.Length; i++)
        {
            parts[i] = BuildPartValueFromValues(this.ApiIdentityParts[i], context);
        }

        return ApiIdentityValue.Composite(parts);
    }
    #endregion

    #region BuildValue Implementation Methods
    private static ApiIdentityPartValue BuildPartValue(ApiIdentityPart schemaPart, ApiIdentityValueBuildContext context)
    {
        return schemaPart switch
        {
            ApiIdentityScalarPart scalarPart => BuildScalarPartValue(scalarPart, context),
            ApiIdentityNestedPart nestedPart => BuildNestedPartValue(nestedPart, context),
            ApiIdentityOwnerPart ownerPart => BuildOwnerPartValue(ownerPart, context),
            _ => throw new ApiIdentityException($"Unsupported identity part type: {schemaPart.GetType().Name}")
        };
    }

    private static ApiIdentityScalarPartValue BuildScalarPartValue(ApiIdentityScalarPart schemaPart, ApiIdentityValueBuildContext context)
    {
        var partName = schemaPart.ClrPropertyName;

        if (!schemaPart.ApiProperty.TryGetValue(context.ClrInstance, out var rawValue, schemaPart.ClrScalarType))
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Identity property '{partName}' could not be read from {context.ClrInstance.GetType().Name}. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to suppress this error.");
            }

            return new ApiIdentityScalarPartValue(partName, ApiId.Empty);
        }

        if (rawValue is null)
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Identity property '{partName}' returned null on {context.ClrInstance.GetType().Name}. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null values.");
            }

            return new ApiIdentityScalarPartValue(partName, ApiId.Empty);
        }

        var apiId = ApiId.FromObject(rawValue, schemaPart.ClrScalarType);
        return new ApiIdentityScalarPartValue(partName, apiId);
    }

    private static ApiIdentityObjectPartValue BuildNestedPartValue(ApiIdentityNestedPart schemaPart, ApiIdentityValueBuildContext context)
    {
        var partName = schemaPart.ClrPropertyName;

        if (!schemaPart.ApiProperty.TryGetValue(context.ClrInstance, out var nestedObj))
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Nested identity property '{partName}' could not be read from {context.ClrInstance.GetType().Name}. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to suppress this error.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiIdentity);
            return new ApiIdentityObjectPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        if (nestedObj is null)
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Nested identity property '{partName}' returned null on {context.ClrInstance.GetType().Name}. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null values.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiIdentity);
            return new ApiIdentityObjectPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        var nestedContext = context with { ClrInstance = nestedObj, ClrOwnerInstance = context.ClrInstance };
        var nestedValue = schemaPart.ApiIdentity.BuildValue(nestedContext);
        return new ApiIdentityObjectPartValue(partName, nestedValue);
    }

    private static ApiIdentityObjectPartValue BuildOwnerPartValue(ApiIdentityOwnerPart schemaPart, ApiIdentityValueBuildContext context)
    {
        var partName = schemaPart.ApiOwnerType.ApiName;

        if (context.ClrOwnerInstance is null)
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Owner instance is null for owner identity part '{partName}'. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null owner.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiOwnerIdentity);
            return new ApiIdentityObjectPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        var ownerContext = context with { ClrInstance = context.ClrOwnerInstance, ClrOwnerInstance = null };
        var ownerValue = schemaPart.ApiOwnerIdentity.BuildValue(ownerContext);
        return new ApiIdentityObjectPartValue(partName, ownerValue);
    }

    private static ApiIdentityPartValue[] BuildStructureSkeleton(ApiIdentity schemaIdentity)
    {
        var parts = new ApiIdentityPartValue[schemaIdentity.ApiIdentityParts.Length];
        for (var i = 0; i < schemaIdentity.ApiIdentityParts.Length; i++)
        {
            parts[i] = BuildStructureSkeletonPart(schemaIdentity.ApiIdentityParts[i]);
        }

        return parts;
    }

    private static ApiIdentityPartValue BuildStructureSkeletonPart(ApiIdentityPart schemaPart)
    {
        return schemaPart switch
        {
            ApiIdentityScalarPart scalarPart
                => new ApiIdentityScalarPartValue(scalarPart.ClrPropertyName, ApiId.Empty),

            ApiIdentityNestedPart nestedPart
                => new ApiIdentityObjectPartValue(nestedPart.ClrPropertyName, apiObjectValue: null, apiStructure: BuildStructureSkeleton(nestedPart.ApiIdentity)),

            ApiIdentityOwnerPart ownerPart
                => new ApiIdentityObjectPartValue(ownerPart.ApiOwnerType.ApiName, apiObjectValue: null, apiStructure: BuildStructureSkeleton(ownerPart.ApiOwnerIdentity)),

            _ => throw new ApiIdentityException($"Unsupported identity part type in skeleton: {schemaPart.GetType().Name}")
        };
    }

    private static ApiIdentityPartValue BuildPartValueFromValues(ApiIdentityPart schemaPart, ApiIdentityValueBuildFromValuesContext context)
    {
        return schemaPart switch
        {
            ApiIdentityScalarPart scalarPart => BuildScalarPartValueFromValues(scalarPart, context),
            ApiIdentityNestedPart nestedPart => BuildNestedPartValueFromValues(nestedPart, context),
            ApiIdentityOwnerPart ownerPart => BuildOwnerPartValueFromValues(ownerPart, context),
            _ => throw new ApiIdentityException($"Unsupported identity part type: {schemaPart.GetType().Name}")
        };
    }

    private static ApiIdentityScalarPartValue BuildScalarPartValueFromValues(ApiIdentityScalarPart schemaPart, ApiIdentityValueBuildFromValuesContext context)
    {
        var partName = schemaPart.ClrPropertyName;

        if (!context.Values.TryGetValue(partName, out var rawValue))
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Identity value for '{partName}' is missing from the values dictionary. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow missing values.");
            }

            return new ApiIdentityScalarPartValue(partName, ApiId.Empty);
        }

        if (rawValue is null)
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Identity value for '{partName}' is null in the values dictionary. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null values.");
            }

            return new ApiIdentityScalarPartValue(partName, ApiId.Empty);
        }

        var apiId = ApiId.FromObject(rawValue, schemaPart.ClrScalarType);
        return new ApiIdentityScalarPartValue(partName, apiId);
    }

    private static ApiIdentityObjectPartValue BuildNestedPartValueFromValues(ApiIdentityNestedPart schemaPart, ApiIdentityValueBuildFromValuesContext context)
    {
        var partName = schemaPart.ClrPropertyName;

        if (!context.Values.TryGetValue(partName, out var nestedValue))
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Nested identity value for '{partName}' is missing from the values dictionary. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow missing values.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiIdentity);
            return new ApiIdentityObjectPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        if (nestedValue is null)
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Nested identity value for '{partName}' is null in the values dictionary. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null values.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiIdentity);
            return new ApiIdentityObjectPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        // The nested value can be provided in two forms:
        // 1. IReadOnlyDictionary<string, object?>: a pre-extracted key/value map — use the values-based path.
        // 2. A CLR object instance: extract identity property values via reflection using the instance-based path.
        //    If nestedValue is a primitive scalar rather than a structured object, TryGetValue calls on
        //    the backing ApiProperty will fail and the configured null-handling policy applies.
        if (nestedValue is IReadOnlyDictionary<string, object?> nestedDict)
        {
            var nestedContext = context with { Values = nestedDict, OwnerValues = null };
            var nestedIdentityValue = schemaPart.ApiIdentity.BuildValue(nestedContext);
            return new ApiIdentityObjectPartValue(partName, nestedIdentityValue);
        }

        var instanceContext = new ApiIdentityValueBuildContext
        {
            ClrInstance = nestedValue,
            ClrOwnerInstance = null,
            NullHandling = context.NullHandling
        };
        var nestedInstanceValue = schemaPart.ApiIdentity.BuildValue(instanceContext);
        return new ApiIdentityObjectPartValue(partName, nestedInstanceValue);
    }

    private static ApiIdentityObjectPartValue BuildOwnerPartValueFromValues(ApiIdentityOwnerPart schemaPart, ApiIdentityValueBuildFromValuesContext context)
    {
        var partName = schemaPart.ApiOwnerType.ApiName;

        if (context.OwnerValues is null)
        {
            if (context.NullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Owner values are null for owner identity part '{partName}'. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null owner.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiOwnerIdentity);
            return new ApiIdentityObjectPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        var ownerContext = context with { Values = context.OwnerValues, OwnerValues = null };
        var ownerValue = schemaPart.ApiOwnerIdentity.BuildValue(ownerContext);
        return new ApiIdentityObjectPartValue(partName, ownerValue);
    }
    #endregion
}
