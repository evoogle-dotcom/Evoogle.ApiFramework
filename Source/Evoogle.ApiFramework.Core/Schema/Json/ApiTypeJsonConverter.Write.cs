// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extension;
using Evoogle.Json;

using static Evoogle.ApiFramework.Schema.Json.Internal.ApiJsonConverterHelpers;
using Evoogle.ApiFramework.Schema.Json.Internal;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Partial implementation of <see cref="ApiTypeJsonConverter"/> focused on writing JSON.
/// </summary>
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

        writer.TryWritePropertyWithSerializer(propertyName, apiItemTypeExpression, options);
    }

    private static void WriteApiCollectionTypeApiItemTypeModifiers(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiCollectionType.ApiItemTypeModifiers;
        var apiItemTypeModifiers = apiCollectionType.ApiItemTypeModifiers;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, apiItemTypeModifiers, options, _apiTypeModifiersJsonConverter);
    }

    // ApiEnumType
    private static void WriteApiEnumType(Utf8JsonWriter writer, ApiEnumType apiEnumType, WriteContext context) => WriteApiEnumTypeApiEnumValues(writer, apiEnumType, context);

    private static void WriteApiEnumTypeApiEnumValues(Utf8JsonWriter writer, ApiEnumType apiEnumType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiEnumType.ApiEnumValues;
        var apiEnumValues = apiEnumType.ApiEnumValues;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiEnumValues,
            options,
            (x) =>
            {
                writer.WriteStartArray();

                foreach (var y in x)
                {
                    writer.TryWriteWithSerializer(y, options);
                }

                writer.WriteEndArray();
            }
        );
    }

    // ApiNamedType
    private static void WriteApiNamedType(Utf8JsonWriter writer, ApiNamedType apiNamedType, WriteContext context) => WriteApiNamedTypeApiName(writer, apiNamedType, context);

    private static void WriteApiNamedTypeApiName(Utf8JsonWriter writer, ApiNamedType apiNamedType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiNamedType.ApiName;
        var value = apiNamedType.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
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

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiProperties,
            options,
            (x) =>
            {
                writer.WriteStartArray();

                foreach (var y in x)
                {
                    writer.TryWriteWithSerializer(y, options);
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

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiRelationships,
            options,
            (x) =>
            {
                writer.WriteStartArray();

                foreach (var y in x)
                {
                    writer.TryWriteWithSerializer(y, options);
                }

                writer.WriteEndArray();
            }
        );
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

        writer.TryWritePropertyWithConverter(propertyName, clrType, options, _typeJsonConverter);
    }

    private static void WriteApiTypeEpilog(Utf8JsonWriter writer, ApiType apiType, WriteContext context)
    {
        WriteApiTypeClrType(writer, apiType, context);
        WriteExtensibleBaseExtensions(writer, apiType, context.PropertyNames.ExtensibleBase.Extensions, context.Options, context.Logger);

        writer.WriteEndObject();
    }

    private static void WriteApiTypeKind(Utf8JsonWriter writer, ApiType apiType, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiType.Kind;
        var kind = apiType.Kind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, kind, options, _apiTypeKindJsonConverter);
    }

    private static void WriteApiTypeProlog(Utf8JsonWriter writer, ApiType apiType, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiTypeKind(writer, apiType, context);
    }

    #endregion
}
