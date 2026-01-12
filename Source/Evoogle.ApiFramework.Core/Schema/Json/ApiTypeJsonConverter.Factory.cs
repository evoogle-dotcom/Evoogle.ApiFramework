// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Partial implementation of <see cref="ApiTypeJsonConverter"/> containing factory helpers.
/// </summary>
public partial class ApiTypeJsonConverter : JsonConverterBase<ApiType>
{
    #region ApiCollectionType Factory Methods
    private static ApiCollectionType CreateApiCollectionType(DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
    {
        var apiItemTypeExpression = context.ReadData.ApiCollectionType?.ApiItemTypeExpression;
        var apiItemTypeModifiers = context.ReadData.ApiCollectionType?.ApiItemTypeModifiers ?? ApiTypeModifiers.None;
        var clrCollectionType = context.ReadData.ApiType?.ClrType;

        var apiCollectionType = new ApiCollectionType
        (
            apiItemTypeExpression!,
            apiItemTypeModifiers,
            clrCollectionType!
        );

        return apiCollectionType;
    }
    #endregion

    #region ApiEnumType Factory Methods
    private static ApiEnumType CreateApiEnumType(DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
    {
        var apiName = context.ReadData.ApiNamedType?.ApiName;
        var apiEnumValues = context.ReadData.ApiEnumType?.ApiEnumValues;
        var clrType = context.ReadData.ApiType?.ClrType;

        var apiEnumType = new ApiEnumType
        (
            apiName!,
            apiEnumValues!,
            clrType!
        );

        return apiEnumType;
    }
    #endregion

    #region ApiObjectType Factory Methods
    private static ApiObjectType CreateApiObjectType(DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
    {
        var apiName = context.ReadData.ApiNamedType?.ApiName;
        var apiIdentities = context.ReadData.ApiObjectType?.ApiIdentities;
        var apiProperties = context.ReadData.ApiObjectType?.ApiProperties;
        var apiRelationships = context.ReadData.ApiObjectType?.ApiRelationships;
        var apiOptions = context.ReadData.ApiObjectType?.ApiOptions;
        var clrType = context.ReadData.ApiType?.ClrType;

        var apiObjectType = new ApiObjectType
        (
            apiName!,
            apiOptions,
            apiIdentities,
            apiProperties,
            apiRelationships,
            clrType!
        );

        return apiObjectType;
    }
    #endregion

    #region ApiScalarType Factory Methods
    private static ApiScalarType CreateApiScalarType(DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
    {
        var apiName = context.ReadData.ApiNamedType?.ApiName;
        var clrType = context.ReadData.ApiType?.ClrType;

        var apiScalarType = new ApiScalarType
        (
            apiName!,
            clrType!
        );

        return apiScalarType;
    }
    #endregion
}
