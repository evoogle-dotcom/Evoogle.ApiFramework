// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Partial implementation of <see cref="ApiTypeJsonConverter"/> focused on writing JSON.
/// </summary>
public partial class ApiTypeJsonConverter : JsonConverterBase<ApiType>
{
    #region Write Implementation Methods
    // ApiCollectionType
    private static void WriteApiCollectionType(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, DefaultWriteContext<PropertyNames> context)
    {
        WriteApiCollectionTypeApiItemTypeExpression(writer, apiCollectionType, context);
        WriteApiCollectionTypeApiItemTypeModifiers(writer, apiCollectionType, context);
    }

    private static void WriteApiCollectionTypeApiItemTypeExpression(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiCollectionType.ApiItemTypeExpression;
        var apiItemTypeExpression = apiCollectionType.ApiItemTypeExpression;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, apiItemTypeExpression, options);
    }

    private static void WriteApiCollectionTypeApiItemTypeModifiers(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiCollectionType.ApiItemTypeModifiers;
        var apiItemTypeModifiers = apiCollectionType.ApiItemTypeModifiers;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, apiItemTypeModifiers, options, _apiTypeModifiersJsonConverter);
    }

    // ApiEnumType
    private static void WriteApiEnumType(Utf8JsonWriter writer, ApiEnumType apiEnumType, DefaultWriteContext<PropertyNames> context) => WriteApiEnumTypeApiEnumValues(writer, apiEnumType, context);

    private static void WriteApiEnumTypeApiEnumValues(Utf8JsonWriter writer, ApiEnumType apiEnumType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiEnumType.ApiEnumValues;
        var apiEnumValues = apiEnumType.ApiEnumValues;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiEnumValues,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }

    // ApiNamedType
    private static void WriteApiNamedType(Utf8JsonWriter writer, ApiNamedType apiNamedType, DefaultWriteContext<PropertyNames> context) => WriteApiNamedTypeApiName(writer, apiNamedType, context);

    private static void WriteApiNamedTypeApiName(Utf8JsonWriter writer, ApiNamedType apiNamedType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiNamedType.ApiName;
        var value = apiNamedType.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    // ApiObjectType
    private static void WriteApiObjectType(Utf8JsonWriter writer, ApiObjectType apiObjectType, DefaultWriteContext<PropertyNames> context)
    {
        WriteApiObjectTypeApiProperties(writer, apiObjectType, context);
        WriteApiObjectTypeApiRelationships(writer, apiObjectType, context);
    }

    private static void WriteApiObjectTypeApiProperties(Utf8JsonWriter writer, ApiObjectType apiObjectType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiObjectType.ApiProperties;
        var apiProperties = apiObjectType.ApiProperties;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiProperties,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }

    private static void WriteApiObjectTypeApiRelationships(Utf8JsonWriter writer, ApiObjectType apiObjectType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiObjectType.ApiRelationships;
        var apiRelationships = apiObjectType.ApiRelationships;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiRelationships,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }

    // ApiScalarType
    private static void WriteApiScalarType(Utf8JsonWriter writer, ApiScalarType apiScalarType, DefaultWriteContext<PropertyNames> context)
    {
        // Note: ApiScalarType has no serializable body fields — intentionally left empty.
    }

    // ApiType
    private static void WriteApiTypeClrType(Utf8JsonWriter writer, ApiType apiType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiType.ClrType;
        var clrType = apiType.ClrType;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, clrType, options, _typeJsonConverter);
    }

    private static void WriteApiTypeKind(Utf8JsonWriter writer, ApiType apiType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiType.Kind;
        var kind = apiType.Kind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, kind, options, _apiTypeKindJsonConverter);
    }
    #endregion
}
