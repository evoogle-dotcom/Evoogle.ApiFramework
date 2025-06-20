// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using static Evoogle.ApiFramework.Schema.Internal.ApiJsonConverterHelpers;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region ApiCollectionType Validation Methods
    private static void ValidateApiCollectionTypeProperties(in ReadContext context, ref List<ValidationResult>? results)
    {
        ValidateApiCollectionTypeProperties
        (
            context,
            context.PropertyNames.ApiCollectionType.ApiItemTypeExpression, context.ReadData.ApiCollectionType?.ApiItemTypeExpression,
            context.PropertyNames.ApiCollectionType.ApiItemTypeModifiers, context.ReadData.ApiCollectionType?.ApiItemTypeModifiers,
            ref results
        );
    }

    private static void ValidateApiCollectionTypeProperties
    (
        in ReadContext context,
        string apiItemTypeExpressionPropertyName, ApiTypeExpressionReadData? apiItemTypeExpression,
        string apiItemTypeModifiersPropertyName, ApiTypeModifiers? apiItemTypeModifiers,
        ref List<ValidationResult>? results
    )
    {
        if (apiItemTypeExpression == null)
        {
            AddMissingRequiredPropertyError(ref results, apiItemTypeExpressionPropertyName);
        }
        else
        {
            ValidateApiTypeExpression(context, apiItemTypeExpression, ref results);
        }

        if (apiItemTypeModifiers == null)
        {
            AddMissingRequiredPropertyError(ref results, apiItemTypeModifiersPropertyName);
        }
    }
    #endregion

    #region ApiEnumType Validation Methods
    private static void ValidateApiEnumTypeProperties(in ReadContext context, ref List<ValidationResult>? results)
    {
        ValidateApiEnumTypeProperties
        (
            context,
            context.PropertyNames.ApiEnumType.ApiEnumValues, context.ReadData.ApiEnumType?.ApiEnumValues,
            ref results
        );
    }

    private static void ValidateApiEnumTypeProperties
    (
        in ReadContext context,
        string apiEnumValuesPropertyName, IReadOnlyList<ApiEnumValueReadData>? apiEnumValues,
        ref List<ValidationResult>? results
    )
    {
        if (apiEnumValues == null)
        {
            AddMissingRequiredPropertyError(ref results, apiEnumValuesPropertyName);
        }
        else
        {
            if (apiEnumValues.Any() == false)
            {
                AddEmptyRequiredCollectionPropertyError(ref results, apiEnumValuesPropertyName);
            }
            else
            {
                for (var i = 0; i < apiEnumValues.Count; ++i)
                {
                    var enumValue = apiEnumValues[i];
                    ValidateApiEnumValueProperties(
                        $"{apiEnumValuesPropertyName}[{i}].{context.PropertyNames.ApiEnumValue.ApiName}", enumValue.ApiName,
                        $"{apiEnumValuesPropertyName}[{i}].{context.PropertyNames.ApiEnumValue.ClrName}", enumValue.ClrName,
                        $"{apiEnumValuesPropertyName}[{i}].{context.PropertyNames.ApiEnumValue.ClrOrdinal}", enumValue.ClrOrdinal,
                        ref results
                    );
                }
            }
        }
    }

    private static void ValidateApiEnumValueProperties
    (
        string apiNamePropertyName, string? apiName,
        string clrNamePropertyName, string? clrName,
        string clrOrdinalPropertyName, long? clrOrdinal,
        ref List<ValidationResult>? results
    )
    {
        if (apiName == null)
        {
            AddMissingRequiredPropertyError(ref results, apiNamePropertyName);
        }

        if (clrName == null)
        {
            AddMissingRequiredPropertyError(ref results, clrNamePropertyName);
        }

        if (clrOrdinal == null)
        {
            AddMissingRequiredPropertyError(ref results, clrOrdinalPropertyName);
        }
    }
    #endregion

    #region ApiNamedType Validation Methods
    private static void ValidateApiNamedTypeProperties(in ReadContext context, ref List<ValidationResult>? results)
    {
        ValidateApiNamedTypeProperties
        (
            context.PropertyNames.ApiNamedType.ApiName, context.ReadData.ApiNamedType?.ApiName,
            ref results
        );
    }

    private static void ValidateApiNamedTypeProperties
    (
        string apiNamePropertyName, string? apiName,
        ref List<ValidationResult>? results
    )
    {
        if (apiName == null)
        {
            AddMissingRequiredPropertyError(ref results, apiNamePropertyName);
        }
    }
    #endregion

    #region ApiObjectType Validation Methods
    private static void ValidateApiObjectTypeProperties(in ReadContext context, ref List<ValidationResult>? results)
    {
        ValidateApiObjectTypeProperties
        (
            context,
            context.PropertyNames.ApiObjectType.ApiProperties, context.ReadData.ApiObjectType?.ApiProperties,
            ref results
        );
    }

    private static void ValidateApiObjectTypeProperties
    (
        in ReadContext context,
        string apiPropertiesPropertyName, IReadOnlyList<ApiPropertyReadData>? apiProperties,
        ref List<ValidationResult>? results
    )
    {
        if (apiProperties == null)
        {
            AddMissingRequiredPropertyError(ref results, apiPropertiesPropertyName);
        }
        else
        {
            if (apiProperties.Any() == false)
            {
                AddEmptyRequiredCollectionPropertyError(ref results, apiPropertiesPropertyName);
            }
            else
            {
                for (var i = 0; i < apiProperties.Count; ++i)
                {
                    var apiProperty = apiProperties[i];
                    ValidateApiPropertyProperties(
                        context,
                        $"{apiPropertiesPropertyName}[{i}].{context.PropertyNames.ApiProperty.ApiName}", apiProperty.ApiName,
                        $"{apiPropertiesPropertyName}[{i}].{context.PropertyNames.ApiProperty.ApiTypeExpression}", apiProperty.ApiTypeExpression,
                        $"{apiPropertiesPropertyName}[{i}].{context.PropertyNames.ApiProperty.ApiTypeModifiers}", apiProperty.ApiTypeModifiers,
                        $"{apiPropertiesPropertyName}[{i}].{context.PropertyNames.ApiProperty.ClrName}", apiProperty.ClrName,
                        ref results
                    );
                }
            }
        }
    }

    private static void ValidateApiPropertyProperties
    (
        in ReadContext context,
        string apiNamePropertyName, string? apiName,
        string apiTypeExpressionPropertyName, ApiTypeExpressionReadData? apiTypeExpression,
        string apiTypeModifiersPropertyName, ApiTypeModifiers? apiTypeModifiers,
        string clrNamePropertyName, string? clrName,
        ref List<ValidationResult>? results
    )
    {
        if (apiName == null)
        {
            AddMissingRequiredPropertyError(ref results, apiNamePropertyName);
        }

        if (apiTypeExpression == null)
        {
            AddMissingRequiredPropertyError(ref results, apiTypeExpressionPropertyName);
        }
        else
        {
            ValidateApiTypeExpression(context, apiTypeExpression, ref results);
        }

        if (apiTypeModifiers == null)
        {
            AddMissingRequiredPropertyError(ref results, apiTypeModifiersPropertyName);
        }

        if (clrName == null)
        {
            AddMissingRequiredPropertyError(ref results, clrNamePropertyName);
        }
    }
    #endregion

    #region ApiType Validation Methods
    private static void ValidateApiTypeProperties(in ReadContext context, ref List<ValidationResult>? results)
    {
        ValidateApiTypeProperties
        (
            context.PropertyNames.ApiType.Kind, context.ReadData.ApiType?.Kind,
            context.PropertyNames.ApiType.ClrType, context.ReadData.ApiType?.ClrType,
            ref results
        );
    }

    private static void ValidateApiTypeProperties
    (
        string kindPropertyName, string? kind,
        string clrTypePropertyName, Type? clrType,
        ref List<ValidationResult>? results
    )
    {
        if (kind == null)
        {
            AddMissingRequiredPropertyError(ref results, kindPropertyName);
        }
        else if (Enum.TryParse<ApiTypeKind>(kind, out var apiTypeKind) == false)
        {
            AddValidationError(ref results, $"Invalid ApiTypeKind value: {kind}", kindPropertyName);
        }

        if (clrType == null)
        {
            AddMissingRequiredPropertyError(ref results, clrTypePropertyName);
        }
    }
    #endregion

    #region ApiTypeExpression Validation Methods
    private static void ValidateApiTypeExpression
    (
        in ReadContext context,
        ApiTypeExpressionReadData apiTypeExpression,
        ref List<ValidationResult>? results
    )
    {
        var apiInlineType = apiTypeExpression.ApiInlineType;
        if (apiInlineType == null)
        {
            ValidateApiTypeExpressionReferenceProperties
            (
                context.PropertyNames.ApiTypeExpression.Kind, apiTypeExpression.Kind,
                context.PropertyNames.ApiTypeExpression.ApiName, apiTypeExpression.ApiName,
                ref results
            );
        }
    }

    private static void ValidateApiTypeExpressionReferenceProperties
    (
        string kindPropertyName, string? kind,
        string apiNamePropertyName, string? apiName,
        ref List<ValidationResult>? results
    )
    {
        if (kind == null)
        {
            AddMissingRequiredPropertyError(ref results, kindPropertyName);
        }
        else if (Enum.TryParse<ApiTypeKind>(kind, out var apiTypeKind) == false)
        {
            AddValidationError(ref results, $"Invalid ApiTypeKind value: {kind}", kindPropertyName);
        }

        if (apiName == null)
        {
            AddMissingRequiredPropertyError(ref results, apiNamePropertyName);
        }
    }
    #endregion
}
