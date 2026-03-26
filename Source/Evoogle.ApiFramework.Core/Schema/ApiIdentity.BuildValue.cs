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

        var parts = new ApiIdentityPartValue[this.ApiIdentityParts.Length];
        for (var i = 0; i < this.ApiIdentityParts.Length; i++)
        {
            parts[i] = BuildPartValue(this.ApiIdentityParts[i], context.ClrInstance, context.ClrOwnerInstance, context.NullHandling);
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

        var parts = new ApiIdentityPartValue[this.ApiIdentityParts.Length];
        for (var i = 0; i < this.ApiIdentityParts.Length; i++)
        {
            parts[i] = BuildPartValueFromValues(this.ApiIdentityParts[i], context.Values, context.OwnerValues, context.NullHandling);
        }

        return ApiIdentityValue.Composite(parts);
    }
    #endregion

    #region BuildValue Implementation Methods
    private static ApiIdentityPartValue BuildPartValue(
        ApiIdentityPart schemaPart,
        object clrInstance,
        object? clrOwnerInstance,
        ApiIdentityNullHandling nullHandling)
    {
        return schemaPart switch
        {
            ApiScalarIdentityPart scalarPart => BuildScalarPartValue(scalarPart, clrInstance, nullHandling),
            ApiNestedIdentityPart nestedPart => BuildNestedPartValue(nestedPart, clrInstance, clrOwnerInstance, nullHandling),
            ApiOwnerIdentityPart ownerPart => BuildOwnerPartValue(ownerPart, clrOwnerInstance, nullHandling),
            _ => throw new ApiIdentityException($"Unsupported identity part type: {schemaPart.GetType().Name}")
        };
    }

    private static ApiScalarIdentityPartValue BuildScalarPartValue(
        ApiScalarIdentityPart schemaPart,
        object clrInstance,
        ApiIdentityNullHandling nullHandling)
    {
        var partName = schemaPart.ApiPropertyName;

        if (!schemaPart.ApiProperty.TryGetValue(clrInstance, out var rawValue, schemaPart.ClrScalarType) || rawValue is null)
        {
            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Identity property '{partName}' returned null on {clrInstance.GetType().Name}. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null values.");
            }

            return new ApiScalarIdentityPartValue(partName, ApiId.Empty);
        }

        var apiId = ApiId.FromObject(rawValue, schemaPart.ClrScalarType);
        return new ApiScalarIdentityPartValue(partName, apiId);
    }

    private static ApiObjectIdentityPartValue BuildNestedPartValue(
        ApiNestedIdentityPart schemaPart,
        object clrInstance,
        object? clrOwnerInstance,
        ApiIdentityNullHandling nullHandling)
    {
        var partName = schemaPart.ApiPropertyName;

        if (!schemaPart.ApiProperty.TryGetValue(clrInstance, out var nestedObj) || nestedObj is null)
        {
            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Nested identity property '{partName}' returned null on {clrInstance.GetType().Name}. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null values.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiIdentity);
            return new ApiObjectIdentityPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        var nestedContext = new ApiIdentityValueBuildContext
        {
            ClrInstance = nestedObj,
            ClrOwnerInstance = clrInstance,
            NullHandling = nullHandling
        };
        var nestedValue = schemaPart.ApiIdentity.BuildValue(nestedContext);
        return new ApiObjectIdentityPartValue(partName, nestedValue);
    }

    private static ApiObjectIdentityPartValue BuildOwnerPartValue(
        ApiOwnerIdentityPart schemaPart,
        object? clrOwnerInstance,
        ApiIdentityNullHandling nullHandling)
    {
        var partName = schemaPart.ApiOwnerType.ApiName;

        if (clrOwnerInstance is null)
        {
            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Owner instance is null for owner identity part '{partName}'. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null owner.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiOwnerIdentity);
            return new ApiObjectIdentityPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        var ownerContext = new ApiIdentityValueBuildContext
        {
            ClrInstance = clrOwnerInstance,
            ClrOwnerInstance = null,
            NullHandling = nullHandling
        };
        var ownerValue = schemaPart.ApiOwnerIdentity.BuildValue(ownerContext);
        return new ApiObjectIdentityPartValue(partName, ownerValue);
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
            ApiScalarIdentityPart scalarPart
                => new ApiScalarIdentityPartValue(scalarPart.ApiPropertyName, ApiId.Empty),

            ApiNestedIdentityPart nestedPart
                => new ApiObjectIdentityPartValue(nestedPart.ApiPropertyName, apiObjectValue: null, apiStructure: BuildStructureSkeleton(nestedPart.ApiIdentity)),

            ApiOwnerIdentityPart ownerPart
                => new ApiObjectIdentityPartValue(ownerPart.ApiOwnerType.ApiName, apiObjectValue: null, apiStructure: BuildStructureSkeleton(ownerPart.ApiOwnerIdentity)),

            _ => throw new ApiIdentityException($"Unsupported identity part type in skeleton: {schemaPart.GetType().Name}")
        };
    }

    private static ApiIdentityPartValue BuildPartValueFromValues(
        ApiIdentityPart schemaPart,
        IReadOnlyDictionary<string, object?> values,
        IReadOnlyDictionary<string, object?>? ownerValues,
        ApiIdentityNullHandling nullHandling)
    {
        return schemaPart switch
        {
            ApiScalarIdentityPart scalarPart => BuildScalarPartValueFromValues(scalarPart, values, nullHandling),
            ApiNestedIdentityPart nestedPart => BuildNestedPartValueFromValues(nestedPart, values, nullHandling),
            ApiOwnerIdentityPart ownerPart => BuildOwnerPartValueFromValues(ownerPart, ownerValues, nullHandling),
            _ => throw new ApiIdentityException($"Unsupported identity part type: {schemaPart.GetType().Name}")
        };
    }

    private static ApiScalarIdentityPartValue BuildScalarPartValueFromValues(
        ApiScalarIdentityPart schemaPart,
        IReadOnlyDictionary<string, object?> values,
        ApiIdentityNullHandling nullHandling)
    {
        var partName = schemaPart.ApiPropertyName;

        if (!values.TryGetValue(partName, out var rawValue) || rawValue is null)
        {
            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Identity value for '{partName}' is missing or null in the values dictionary. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow missing values.");
            }

            return new ApiScalarIdentityPartValue(partName, ApiId.Empty);
        }

        var apiId = ApiId.FromObject(rawValue, schemaPart.ClrScalarType);
        return new ApiScalarIdentityPartValue(partName, apiId);
    }

    private static ApiObjectIdentityPartValue BuildNestedPartValueFromValues(
        ApiNestedIdentityPart schemaPart,
        IReadOnlyDictionary<string, object?> values,
        ApiIdentityNullHandling nullHandling)
    {
        var partName = schemaPart.ApiPropertyName;

        if (!values.TryGetValue(partName, out var nestedValue) || nestedValue is null)
        {
            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Nested identity value for '{partName}' is missing or null in the values dictionary. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow missing values.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiIdentity);
            return new ApiObjectIdentityPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        if (nestedValue is IReadOnlyDictionary<string, object?> nestedDict)
        {
            var nestedContext = new ApiIdentityValueBuildFromValuesContext
            {
                Values = nestedDict,
                OwnerValues = null,
                NullHandling = nullHandling
            };
            var nestedIdentityValue = schemaPart.ApiIdentity.BuildValue(nestedContext);
            return new ApiObjectIdentityPartValue(partName, nestedIdentityValue);
        }

        // Nested value is an object instance — fall through to instance-based building
        var instanceContext = new ApiIdentityValueBuildContext
        {
            ClrInstance = nestedValue,
            ClrOwnerInstance = null,
            NullHandling = nullHandling
        };
        var nestedInstanceValue = schemaPart.ApiIdentity.BuildValue(instanceContext);
        return new ApiObjectIdentityPartValue(partName, nestedInstanceValue);
    }

    private static ApiObjectIdentityPartValue BuildOwnerPartValueFromValues(
        ApiOwnerIdentityPart schemaPart,
        IReadOnlyDictionary<string, object?>? ownerValues,
        ApiIdentityNullHandling nullHandling)
    {
        var partName = schemaPart.ApiOwnerType.ApiName;

        if (ownerValues is null)
        {
            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Owner values are null for owner identity part '{partName}'. " +
                    $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow null owner.");
            }

            var skeleton = BuildStructureSkeleton(schemaPart.ApiOwnerIdentity);
            return new ApiObjectIdentityPartValue(partName, apiObjectValue: null, apiStructure: skeleton);
        }

        var ownerContext = new ApiIdentityValueBuildFromValuesContext
        {
            Values = ownerValues,
            OwnerValues = null,
            NullHandling = nullHandling
        };
        var ownerValue = schemaPart.ApiOwnerIdentity.BuildValue(ownerContext);
        return new ApiObjectIdentityPartValue(partName, ownerValue);
    }
    #endregion
}
