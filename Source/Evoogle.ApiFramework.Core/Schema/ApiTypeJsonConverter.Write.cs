// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extension;
using Evoogle.Json;

using static Evoogle.ApiFramework.Schema.Internal.ApiJsonConverterHelpers;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Write Implementation Methods
    // ApiCollectionType
    private static void WriteApiCollectionType
    (
        Utf8JsonWriter writer,
        ApiCollectionType apiCollectionType,
        in WriteContext context
    )
    {
        WriteApiCollectionTypeApiItemTypeExpression(writer, apiCollectionType, context);
        WriteApiCollectionTypeApiItemTypeModifiers(writer, apiCollectionType, context);
    }

    private static void WriteApiCollectionTypeApiItemTypeExpression
    (
        Utf8JsonWriter writer,
        ApiCollectionType apiCollectionType,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiCollectionType.ApiItemTypeExpression;
        writer.WritePropertyName(propertyName);

        var apiItemTypeExpression = apiCollectionType.ApiItemTypeExpression;
        WriteApiTypeExpression(writer, apiItemTypeExpression, context);
    }

    private static void WriteApiCollectionTypeApiItemTypeModifiers
    (
        Utf8JsonWriter writer,
        ApiCollectionType apiCollectionType,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiCollectionType.ApiItemTypeModifiers;
        var apiItemTypeModifiers = apiCollectionType.ApiItemTypeModifiers;
        var options = context.Options;

        writer.WritePropertyWithConverter(propertyName, apiItemTypeModifiers, options, ApiTypeModifiersJsonConverter);
    }

    // ApiEnumType
    private static void WriteApiEnumType
    (
        Utf8JsonWriter writer,
        ApiEnumType apiEnumType,
        in WriteContext context
    )
    {
        WriteApiEnumTypeApiEnumValues(writer, apiEnumType, context);
    }

    private static void WriteApiEnumTypeApiEnumValues
    (
        Utf8JsonWriter writer,
        ApiEnumType apiEnumType,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiEnumType.ApiEnumValues;
        writer.WritePropertyName(propertyName);

        writer.WriteStartArray();

        var apiEnumValues = apiEnumType.ApiEnumValues;
        foreach (var apiEnumValue in apiEnumValues)
        {
            WriteApiEnumValue(writer, apiEnumValue, context);
        }

        writer.WriteEndArray();
    }

    // ApiEnumValue
    private static void WriteApiEnumValue
    (
        Utf8JsonWriter writer,
        ApiEnumValue apiEnumValue,
        in WriteContext context
    )
    {
        writer.WriteStartObject();

        WriteApiEnumValueApiName(writer, apiEnumValue, context);
        WriteApiEnumValueClrName(writer, apiEnumValue, context);
        WriteApiEnumValueClrOrdinal(writer, apiEnumValue, context);

        writer.WriteEndObject();
    }

    private static void WriteApiEnumValueApiName
    (
        Utf8JsonWriter writer,
        ApiEnumValue apiEnumValue,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ApiName;
        var value = apiEnumValue.ApiName;
        var options = context.Options;

        writer.WritePropertyString(propertyName, value, options);
    }

    private static void WriteApiEnumValueClrName
    (
        Utf8JsonWriter writer,
        ApiEnumValue apiEnumValue,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrName;
        var value = apiEnumValue.ClrName;
        var options = context.Options;

        writer.WritePropertyString(propertyName, value, options);
    }

    private static void WriteApiEnumValueClrOrdinal
    (
        Utf8JsonWriter writer,
        ApiEnumValue apiEnumValue,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrOrdinal;
        var value = apiEnumValue.ClrOrdinal;
        var options = context.Options;

        writer.WritePropertyNumber(propertyName, value, options);
    }

    // ApiNamedType
    private static void WriteApiNamedType
    (
        Utf8JsonWriter writer,
        ApiNamedType apiNamedType,
        in WriteContext context
    )
    {
        WriteApiNamedTypeApiName(writer, apiNamedType, context);
    }

    private static void WriteApiNamedTypeApiName
    (
        Utf8JsonWriter writer,
        ApiNamedType apiNamedType,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiNamedType.ApiName;
        var value = apiNamedType.ApiName;
        var options = context.Options;

        writer.WritePropertyString(propertyName, value, options);
    }

    // ApiObjectType
    private static void WriteApiObjectType
    (
        Utf8JsonWriter writer,
        ApiObjectType apiObjectType,
        in WriteContext context
    )
    {
        WriteApiObjectTypeApiProperties(writer, apiObjectType, context);
    }

    private static void WriteApiObjectTypeApiProperties
    (
        Utf8JsonWriter writer,
        ApiObjectType apiObjectType,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiObjectType.ApiProperties;
        writer.WritePropertyName(propertyName);

        writer.WriteStartArray();

        var apiProperties = apiObjectType.ApiProperties;
        foreach (var apiProperty in apiProperties)
        {
            WriteApiProperty(writer, apiProperty, context);
        }

        writer.WriteEndArray();
    }

    // ApiProperty
    private static void WriteApiProperty
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        in WriteContext context
    )
    {
        writer.WriteStartObject();

        WriteApiPropertyApiName(writer, apiProperty, context);
        WriteApiPropertyApiTypeExpression(writer, apiProperty, context);
        WriteApiPropertyApiTypeModifiers(writer, apiProperty, context);
        WriteApiPropertyClrName(writer, apiProperty, context);

        writer.WriteEndObject();
    }

    private static void WriteApiPropertyApiName
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiName;
        var value = apiProperty.ApiName;
        var options = context.Options;

        writer.WritePropertyString(propertyName, value, options);
    }

    private static void WriteApiPropertyApiTypeExpression
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiTypeExpression;
        writer.WritePropertyName(propertyName);

        var apiTypeExpression = apiProperty.ApiTypeExpression;
        WriteApiTypeExpression(writer, apiTypeExpression, context);
    }

    private static void WriteApiPropertyApiTypeModifiers
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiTypeModifiers;
        var apiTypeModifiers = apiProperty.ApiTypeModifiers;
        var options = context.Options;

        writer.WritePropertyWithConverter(propertyName, apiTypeModifiers, options, ApiTypeModifiersJsonConverter);
    }

    private static void WriteApiPropertyClrName
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiProperty.ClrName;
        var value = apiProperty.ClrName;
        var options = context.Options;

        writer.WritePropertyString(propertyName, value, options);
    }

    // ApiScalarType
    private static void WriteApiScalarType
    (
        Utf8JsonWriter writer,
        ApiScalarType apiScalarType,
        in WriteContext context
    )
    {
        // Note: ApiScalarType has no serializable body fields — intentionally left empty.
    }

    // ApiType
    private static void WriteApiTypeBody
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        in WriteContext context
    )
    {
        var kind = apiType.Kind;
        switch (kind)
        {
            case ApiTypeKind.Collection:
                {
                    var apiCollectionType = (ApiCollectionType)apiType;
                    WriteApiCollectionType(writer, apiCollectionType, context);
                    break;
                }

            case ApiTypeKind.Enum:
                {
                    var apiEnumType = (ApiEnumType)apiType;
                    WriteApiNamedType(writer, apiEnumType, context);
                    WriteApiEnumType(writer, apiEnumType, context);
                    break;
                }

            case ApiTypeKind.Object:
                {
                    var apiObjectType = (ApiObjectType)apiType;
                    WriteApiNamedType(writer, apiObjectType, context);
                    WriteApiObjectType(writer, apiObjectType, context);
                    break;
                }

            case ApiTypeKind.Scalar:
                {
                    var apiScalarType = (ApiScalarType)apiType;
                    WriteApiNamedType(writer, apiScalarType, context);
                    WriteApiScalarType(writer, apiScalarType, context);
                    break;
                }

            default:
                {
                    throw new JsonException($"Unsupported Kind: {kind}");
                }
        }
    }

    private static void WriteApiTypeClrType
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiType.ClrType;
        var clrType = apiType.ClrType;
        var options = context.Options;

        writer.WritePropertyWithConverter(propertyName, clrType, options, TypeJsonConverter);
    }

    private static void WriteApiTypeEpilog
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        in WriteContext context
    )
    {
        WriteApiTypeClrType(writer, apiType, context);
        WriteExtensibleBaseExtensions(writer, apiType, context);

        writer.WriteEndObject();
    }

    private static void WriteApiTypeKind
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiType.Kind;
        var kind = apiType.Kind;
        var options = context.Options;

        writer.WritePropertyWithConverter(propertyName, kind, options, ApiTypeKindJsonConverter);
    }

    private static void WriteApiTypeProlog
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        in WriteContext context
    )
    {
        writer.WriteStartObject();

        WriteApiTypeKind(writer, apiType, context);
    }

    // ApiTypeExpression
    private static void WriteApiTypeExpression
    (
        Utf8JsonWriter writer,
        ApiTypeExpression apiTypeExpression,
        in WriteContext context
    )
    {
        writer.WriteStartObject();

        WriteApiTypeExpressionKind(writer, apiTypeExpression, context);
        WriteApiTypeExpressionApiName(writer, apiTypeExpression, context);
        WriteApiTypeExpressionApiInlineType(writer, apiTypeExpression, context);

        writer.WriteEndObject();
    }

    private static void WriteApiTypeExpressionKind
    (
        Utf8JsonWriter writer,
        ApiTypeExpression apiTypeExpression,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.Kind;
        var kind = apiTypeExpression.Kind;
        var options = context.Options;

        writer.WritePropertyWithConverter(propertyName, kind, options, ApiTypeKindJsonConverter);
    }

    private static void WriteApiTypeExpressionApiName
    (
        Utf8JsonWriter writer,
        ApiTypeExpression apiTypeExpression,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ApiName;
        var value = apiTypeExpression.ApiName;
        var options = context.Options;

        writer.WritePropertyString(propertyName, value, options);
    }

    private static void WriteApiTypeExpressionApiInlineType
    (
        Utf8JsonWriter writer,
        ApiTypeExpression apiTypeExpression,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ApiInlineType;
        var apiInlineType = apiTypeExpression.ApiInlineType;
        var options = context.Options;

        writer.WritePropertyWithSerializer(propertyName, apiInlineType, options);
    }

    // ExtensibleBase Methods
    private static void WriteExtensibleBaseExtensions
    (
        Utf8JsonWriter writer,
        ExtensibleBase extensibleBase,
        in WriteContext context
    )
    {
        var extensions = extensibleBase.Extensions;
        if (extensions != null)
        {
            var extensionsPropertyName = context.PropertyNames.ExtensibleBase.Extensions;
            writer.WritePropertyName(extensionsPropertyName);

            WriteExtensions(writer, extensions, context.Options, context.Logger);
        }
    }
    #endregion
}
