// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Microsoft.Extensions.Logging;

using static Evoogle.ApiFramework.Schema.Json.Internal.ApiJsonConverterHelpers;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Partial implementation of <see cref="ApiTypeJsonConverter"/> containing factory helpers.
/// </summary>
public partial class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Factory Methods
    private static ApiType CreateApiType(ReadContext context)
    {
        var apiType = default(ApiType);

        var kindAsString = context.ReadData.ApiType?.Kind;

        var kind = GetApiTypeKind(context, kindAsString);
        if (kind is not null)
        {
            switch (kind)
            {
                case ApiTypeKind.Collection:
                    apiType = CreateApiCollectionType(context);
                    break;

                case ApiTypeKind.Enum:
                    apiType = CreateApiEnumType(context);
                    break;

                case ApiTypeKind.Object:
                    apiType = CreateApiObjectType(context);
                    break;

                case ApiTypeKind.Scalar:
                    apiType = CreateApiScalarType(context);
                    break;

                default:
                    context.Logger.LogError("Unsupported {Kind} enumeration value: '{KindValue}'", nameof(ApiTypeKind), kind);
                    break;
            }
        }

        apiType ??= new ApiUnknownType();

        var extensions = context.ReadData.ExtensibleBase?.Extensions;
        AttachExtensions(apiType, extensions);

        return apiType;
    }
    #endregion

    #region ApiCollectionType Factory Methods
    private static ApiCollectionType CreateApiCollectionType(ReadContext context)
    {
        var apiItemTypeExpression = CreateApiTypeExpression(context, context.ReadData.ApiCollectionType?.ApiItemTypeExpression);
        var apiItemTypeModifiers = context.ReadData.ApiCollectionType?.ApiItemTypeModifiers ?? ApiTypeModifiers.None;
        var clrCollectionType = context.ReadData.ApiType?.ClrType;

        return new ApiCollectionType(apiItemTypeExpression, apiItemTypeModifiers, clrCollectionType!);
    }
    #endregion

    #region ApiEnumType Factory Methods
    private static ApiEnumType CreateApiEnumType(ReadContext context)
    {
        var apiName = context.ReadData.ApiNamedType?.ApiName;
        var apiEnumValues = CreateApiEnumValues(context);
        var clrType = context.ReadData.ApiType?.ClrType;

        return new ApiEnumType(apiName!, apiEnumValues, clrType!);
    }

    private static ApiEnumValue[] CreateApiEnumValues(ReadContext context)
    {
        var apiEnumValues = context.ReadData.ApiEnumType?.ApiEnumValues?.Select(CreateApiEnumValue).ToArray() ?? [];
        return apiEnumValues;
    }

    private static ApiEnumValue CreateApiEnumValue(ApiEnumValueReadData apiEnumValueReadData)
    {
        var apiName = apiEnumValueReadData.ApiName;
        var clrName = apiEnumValueReadData.ClrName;
        var clrOrdinal = apiEnumValueReadData.ClrOrdinal.GetValueOrDefault();

        return new ApiEnumValue(apiName!, clrName!, clrOrdinal);
    }
    #endregion

    #region ApiObjectType Factory Methods
    private static ApiObjectType CreateApiObjectType(ReadContext context)
    {
        var apiName = context.ReadData.ApiNamedType?.ApiName;
        var apiProperties = CreateApiProperties(context);
        var apiRelationships = CreateApiRelationships(context);
        var clrType = context.ReadData.ApiType?.ClrType;

        return new ApiObjectType(apiName!, apiProperties, apiRelationships, clrType!);
    }

    private static ApiProperty[] CreateApiProperties(ReadContext context)
    {
        var apiProperties = context.ReadData.ApiObjectType?.ApiProperties?.Select(x => CreateApiProperty(context, x)).ToArray() ?? [];
        return apiProperties;
    }

    private static ApiProperty CreateApiProperty(ReadContext context, ApiPropertyReadData apiPropertyReadData)
    {
        var apiName = apiPropertyReadData.ApiName;
        var apiTypeExpression = CreateApiTypeExpression(context, apiPropertyReadData.ApiTypeExpression);
        var apiTypeModifiers = apiPropertyReadData.ApiTypeModifiers ?? ApiTypeModifiers.None;
        var clrName = apiPropertyReadData.ClrName;

        var apiProperty = new ApiProperty(apiName!, apiTypeExpression, apiTypeModifiers, clrName!);
        return apiProperty;
    }

    private static ApiRelationship[] CreateApiRelationships(ReadContext context)
    {
        var apiRelationships = context.ReadData.ApiObjectType?.ApiRelationships?.Select(CreateApiRelationship).ToArray() ?? [];
        return apiRelationships;
    }

    private static ApiRelationship CreateApiRelationship(ApiRelationshipReadData apiRelationshipReadData)
    {
        var apiName = apiRelationshipReadData.ApiName;
        var apiPropertyName = apiRelationshipReadData.ApiPropertyName;

        return new ApiRelationship(apiName!, apiPropertyName);
    }
    #endregion

    #region ApiScalarType Factory Methods
    private static ApiScalarType CreateApiScalarType(ReadContext context)
    {
        var apiName = context.ReadData.ApiNamedType?.ApiName;
        var clrType = context.ReadData.ApiType?.ClrType;

        return new ApiScalarType(apiName!, clrType!);
    }
    #endregion

    #region ApiTypeExpression Factory Methods
    private static ApiTypeExpression CreateApiTypeExpression(ReadContext context, ApiTypeExpressionReadData? apiTypeExpressionReadData)
    {
        var apiInlineType = apiTypeExpressionReadData?.ApiInlineType;
        if (apiInlineType is not null)
        {
            return new ApiTypeExpression(apiInlineType);
        }

        var kind = GetApiTypeKind(context, apiTypeExpressionReadData?.Kind);
        var apiName = apiTypeExpressionReadData?.ApiName;
        var clrType = apiTypeExpressionReadData?.ClrType;

        return new ApiTypeExpression(kind, apiName, clrType);
    }
    #endregion

    #region Utility Methods
    private static ApiTypeKind? GetApiTypeKind(ReadContext context, string? kindAsString)
    {
        if (kindAsString is null)
        {
            return null;
        }

        if (Enum.TryParse<ApiTypeKind>(kindAsString, out var kind) == false)
        {
            context.Logger.LogError("Unable to parse {Kind} enumeration string: '{KindAsString}'", nameof(ApiTypeKind), kindAsString);
            return null;
        }

        return kind;
    }
    #endregion
}
