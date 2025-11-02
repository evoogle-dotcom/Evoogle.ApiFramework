// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Json;

using Microsoft.Extensions.Logging;

using static Evoogle.ApiFramework.Schema.Json.Internal.ApiJsonConverterHelpers;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Partial implementation of <see cref="ApiTypeJsonConverter"/> containing factory helpers.
/// </summary>
public partial class ApiTypeJsonConverter : JsonConverterBase<ApiType>
{
    #region JsonConverterBase<T> Methods
    protected override ApiType? CreateValue(IReadContext context)
    {
        var readContext = (ReadContext)context;

        var apiType = default(ApiType);

        var kindAsString = readContext.ReadData.ApiType?.Kind;

        var kind = GetApiTypeKind(readContext.Logger, kindAsString);
        if (kind is not null)
        {
            switch (kind)
            {
                case ApiTypeKind.Collection:
                    apiType = CreateApiCollectionType(readContext);
                    break;

                case ApiTypeKind.Enum:
                    apiType = CreateApiEnumType(readContext);
                    break;

                case ApiTypeKind.Object:
                    apiType = CreateApiObjectType(readContext);
                    break;

                case ApiTypeKind.Scalar:
                    apiType = CreateApiScalarType(readContext);
                    break;

                default:
                    readContext.Logger.LogError("Unsupported {Kind} enumeration value: '{KindValue}'", nameof(ApiTypeKind), kind);
                    break;
            }
        }

        apiType ??= new ApiUnknownType();

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiType, extensions);

        return apiType;
    }
    #endregion

    #region ApiCollectionType Factory Methods
    private static ApiCollectionType CreateApiCollectionType(ReadContext context)
    {
        var apiItemTypeExpression = context.ReadData.ApiCollectionType?.ApiItemTypeExpression;
        var apiItemTypeModifiers = context.ReadData.ApiCollectionType?.ApiItemTypeModifiers ?? ApiTypeModifiers.None;
        var clrCollectionType = context.ReadData.ApiType?.ClrType;

        return new ApiCollectionType(apiItemTypeExpression!, apiItemTypeModifiers, clrCollectionType!);
    }
    #endregion

    #region ApiEnumType Factory Methods
    private static ApiEnumType CreateApiEnumType(ReadContext context)
    {
        var apiName = context.ReadData.ApiNamedType?.ApiName;
        var apiEnumValues = context.ReadData.ApiEnumType?.ApiEnumValues; ;
        var clrType = context.ReadData.ApiType?.ClrType;

        return new ApiEnumType(apiName!, apiEnumValues!, clrType!);
    }
    #endregion

    #region ApiObjectType Factory Methods
    private static ApiObjectType CreateApiObjectType(ReadContext context)
    {
        var apiName = context.ReadData.ApiNamedType?.ApiName;
        var apiProperties = context.ReadData.ApiObjectType?.ApiProperties;
        var apiRelationships = context.ReadData.ApiObjectType?.ApiRelationships;
        var clrType = context.ReadData.ApiType?.ClrType;

        return new ApiObjectType(apiName!, apiProperties!, apiRelationships!, clrType!);
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
}
