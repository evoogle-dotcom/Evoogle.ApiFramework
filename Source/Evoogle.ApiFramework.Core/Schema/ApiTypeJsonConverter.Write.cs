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
    private static void WriteApiCollectionType(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, WriteContext context)
    {
        WriteApiCollectionTypeApiItemTypeExpression(writer, apiCollectionType, context);
        WriteApiCollectionTypeApiItemTypeModifiers(writer, apiCollectionType, context);
    }

    private static void WriteApiCollectionTypeApiItemTypeExpression(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiCollectionType.ApiItemTypeExpression;
        var apiItemTypeExpression = apiCollectionType.ApiItemTypeExpression;
        var options = context.Options;

        writer.WriteConditionalPropertyWithAction
        (
            propertyName,
            apiItemTypeExpression,
            options,
            (x) => WriteApiTypeExpression(writer, x, context)
        );
    }

    private static void WriteApiCollectionTypeApiItemTypeModifiers(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiCollectionType.ApiItemTypeModifiers;
        var apiItemTypeModifiers = apiCollectionType.ApiItemTypeModifiers;
        var options = context.Options;

        writer.WriteConditionalPropertyWithConverter(propertyName, apiItemTypeModifiers, options, _apiTypeModifiersJsonConverter);
    }

    // ApiEnumType
    private static void WriteApiEnumType(Utf8JsonWriter writer, ApiEnumType apiEnumType, WriteContext context) => WriteApiEnumTypeApiEnumValues(writer, apiEnumType, context);

    private static void WriteApiEnumTypeApiEnumValues(Utf8JsonWriter writer, ApiEnumType apiEnumType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiEnumType.ApiEnumValues;
        var apiEnumValues = apiEnumType.ApiEnumValues;
        var options = context.Options;

        writer.WriteConditionalPropertyWithAction
        (
            propertyName,
            apiEnumValues,
            options,
            (x) =>
            {
                writer.WriteStartArray();

                foreach (var y in x)
                {
                    writer.WriteConditionalReference(y, options, z => WriteApiEnumValue(writer, z, context));
                }

                writer.WriteEndArray();
            }
        );
    }

    // ApiEnumValue
    private static void WriteApiEnumValue(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiEnumValueApiName(writer, apiEnumValue, context);
        WriteApiEnumValueClrName(writer, apiEnumValue, context);
        WriteApiEnumValueClrOrdinal(writer, apiEnumValue, context);

        writer.WriteEndObject();
    }

    private static void WriteApiEnumValueApiName(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ApiName;
        var value = apiEnumValue.ApiName;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, value, options);
    }

    private static void WriteApiEnumValueClrName(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrName;
        var value = apiEnumValue.ClrName;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, value, options);
    }

    private static void WriteApiEnumValueClrOrdinal(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrOrdinal;
        var value = apiEnumValue.ClrOrdinal;
        var options = context.Options;

        writer.WriteConditionalPropertyAsNumber(propertyName, value, options);
    }

    // ApiNamedType
    private static void WriteApiNamedType(Utf8JsonWriter writer, ApiNamedType apiNamedType, WriteContext context) => WriteApiNamedTypeApiName(writer, apiNamedType, context);

    private static void WriteApiNamedTypeApiName(Utf8JsonWriter writer, ApiNamedType apiNamedType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiNamedType.ApiName;
        var value = apiNamedType.ApiName;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, value, options);
    }

    // ApiObjectType
    private static void WriteApiObjectType(Utf8JsonWriter writer, ApiObjectType apiObjectType, WriteContext context)
    {
        WriteApiObjectTypeApiProperties(writer, apiObjectType, context);
        WriteApiObjectTypeApiRelationships(writer, apiObjectType, context);
    }

    private static void WriteApiObjectTypeApiProperties(Utf8JsonWriter writer, ApiObjectType apiObjectType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiObjectType.ApiProperties;
        var apiProperties = apiObjectType.ApiProperties;
        var options = context.Options;

        writer.WriteConditionalPropertyWithAction
        (
            propertyName,
            apiProperties,
            options,
            (x) =>
            {
                writer.WriteStartArray();

                foreach (var y in x)
                {
                    writer.WriteConditionalReference(y, options, z => WriteApiProperty(writer, z, context));
                }

                writer.WriteEndArray();
            }
        );
    }

    private static void WriteApiObjectTypeApiRelationships(Utf8JsonWriter writer, ApiObjectType apiObjectType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiObjectType.ApiRelationships;
        var apiRelationships = apiObjectType.ApiRelationships;
        var options = context.Options;

        writer.WriteConditionalPropertyWithAction
        (
            propertyName,
            apiRelationships,
            options,
            (x) =>
            {
                writer.WriteStartArray();

                foreach (var y in x)
                {
                    writer.WriteConditionalReference(y, options, z => WriteApiRelationship(writer, z, context));
                }

                writer.WriteEndArray();
            }
        );
    }

    // ApiProperty
    private static void WriteApiProperty(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiPropertyApiName(writer, apiProperty, context);
        WriteApiPropertyApiTypeExpression(writer, apiProperty, context);
        WriteApiPropertyApiTypeModifiers(writer, apiProperty, context);
        WriteApiPropertyClrName(writer, apiProperty, context);

        writer.WriteEndObject();
    }

    private static void WriteApiPropertyApiName(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiName;
        var value = apiProperty.ApiName;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, value, options);
    }

    private static void WriteApiPropertyApiTypeExpression(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiTypeExpression;
        var apiTypeExpression = apiProperty.ApiTypeExpression;
        var options = context.Options;

        writer.WriteConditionalPropertyWithAction
        (
            propertyName,
            apiTypeExpression,
            options,
            (x) => WriteApiTypeExpression(writer, x, context)
        );
    }

    private static void WriteApiPropertyApiTypeModifiers(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiTypeModifiers;
        var apiTypeModifiers = apiProperty.ApiTypeModifiers;
        var options = context.Options;

        writer.WriteConditionalPropertyWithConverter(propertyName, apiTypeModifiers, options, _apiTypeModifiersJsonConverter);
    }

    private static void WriteApiPropertyClrName(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ClrName;
        var value = apiProperty.ClrName;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, value, options);
    }

    // ApiRelationship
    private static void WriteApiRelationship(Utf8JsonWriter writer, ApiRelationship apiRelationship, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiRelationshipApiName(writer, apiRelationship, context);
        WriteApiRelationshipApiPropertyName(writer, apiRelationship, context);

        writer.WriteEndObject();
    }

    private static void WriteApiRelationshipApiName(Utf8JsonWriter writer, ApiRelationship apiRelationship, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiRelationship.ApiName;
        var value = apiRelationship.ApiName;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, value, options);
    }

    private static void WriteApiRelationshipApiPropertyName(Utf8JsonWriter writer, ApiRelationship apiRelationship, WriteContext context)
    {
        var apiName = apiRelationship.ApiName;
        var apiPropertyName = apiRelationship.ApiPropertyName;
        if (apiName.Equals(apiPropertyName))
        {
            // If the API name and property name are the same, we do not need to write the property name.
            return;
        }

        var propertyName = context.PropertyNames.ApiRelationship.ApiPropertyName;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, apiPropertyName, options);
    }

    // ApiScalarType
    private static void WriteApiScalarType(Utf8JsonWriter writer, ApiScalarType apiScalarType, WriteContext context)
    {
        // Note: ApiScalarType has no serializable body fields — intentionally left empty.
    }

    // ApiType
    private static void WriteApiTypeBody(Utf8JsonWriter writer, ApiType apiType, WriteContext context)
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

    private static void WriteApiTypeClrType(Utf8JsonWriter writer, ApiType apiType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiType.ClrType;
        var clrType = apiType.ClrType;
        var options = context.Options;

        writer.WriteConditionalPropertyWithConverter(propertyName, clrType, options, _typeJsonConverter);
    }

    private static void WriteApiTypeEpilog(Utf8JsonWriter writer, ApiType apiType, WriteContext context)
    {
        WriteApiTypeClrType(writer, apiType, context);
        WriteExtensibleBaseExtensions(writer, apiType, context);

        writer.WriteEndObject();
    }

    private static void WriteApiTypeKind(Utf8JsonWriter writer, ApiType apiType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiType.Kind;
        var kind = apiType.Kind;
        var options = context.Options;

        writer.WriteConditionalPropertyWithConverter(propertyName, kind, options, _apiTypeKindJsonConverter);
    }

    private static void WriteApiTypeProlog(Utf8JsonWriter writer, ApiType apiType, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiTypeKind(writer, apiType, context);
    }

    // ApiTypeExpression
    private static void WriteApiTypeExpression(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiTypeExpressionKind(writer, apiTypeExpression, context);
        WriteApiTypeExpressionApiName(writer, apiTypeExpression, context);
        WriteApiTypeExpressionApiInlineType(writer, apiTypeExpression, context);

        writer.WriteEndObject();
    }

    private static void WriteApiTypeExpressionKind(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.Kind;
        var kind = apiTypeExpression.Kind;
        var options = context.Options;

        writer.WriteConditionalPropertyWithConverter(propertyName, kind, options, _apiTypeKindJsonConverter);
    }

    private static void WriteApiTypeExpressionApiName(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ApiName;
        var value = apiTypeExpression.ApiName;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, value, options);
    }

    private static void WriteApiTypeExpressionApiInlineType(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ApiInlineType;
        var apiInlineType = apiTypeExpression.ApiInlineType;
        var options = context.Options;

        writer.WriteConditionalPropertyWithSerializer(propertyName, apiInlineType, options);
    }

    // ExtensibleBase Methods
    private static void WriteExtensibleBaseExtensions(Utf8JsonWriter writer, ExtensibleBase extensibleBase, WriteContext context)
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
