// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Partial implementation of <see cref="ApiTypeJsonConverter"/> focused on writing JSON.
/// </summary>
public partial class ApiTypeJsonConverter : JsonConverterBase<ApiType>
{
    #region Write Implementation Methods
    // ApiCollectionType
    private static void WriteApiCollectionType(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, DefaultWriteContext<PropertyNames> writeContext)
    {
        WriteApiCollectionTypeApiItemTypeExpression(writer, apiCollectionType, writeContext);
        WriteApiCollectionTypeApiItemTypeModifiers(writer, apiCollectionType, writeContext);
    }

    private static void WriteApiCollectionTypeApiItemTypeExpression(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiCollectionType.ApiItemTypeExpression, obj: apiCollectionType.ApiItemTypeExpression, options: writeContext.Options);

    private static void WriteApiCollectionTypeApiItemTypeModifiers(Utf8JsonWriter writer, ApiCollectionType apiCollectionType, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiCollectionType.ApiItemTypeModifiers, value: apiCollectionType.ApiItemTypeModifiers, options: writeContext.Options, converter: _apiTypeModifiersJsonConverter);

    // ApiEnumType
    private static void WriteApiEnumType(Utf8JsonWriter writer, ApiEnumType apiEnumType, DefaultWriteContext<PropertyNames> writeContext)
        => WriteApiEnumTypeApiEnumValues(writer, apiEnumType, writeContext);

    private static void WriteApiEnumTypeApiEnumValues(Utf8JsonWriter writer, ApiEnumType apiEnumType, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ApiEnumType.ApiEnumValues;
        var apiEnumValues = apiEnumType.ApiEnumValues;
        var options = writeContext.Options;

        writer.TryWritePropertyAsArray
        (
            propertyName: propertyName,
            collection: apiEnumValues,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
                writer.TryWriteWithSerializer(obj: item, options: state.Options)
        );
    }

    // ApiNamedType
    private static void WriteApiNamedType(Utf8JsonWriter writer, ApiNamedType apiNamedType, DefaultWriteContext<PropertyNames> writeContext)
        => WriteApiNamedTypeApiName(writer, apiNamedType, writeContext);

    private static void WriteApiNamedTypeApiName(Utf8JsonWriter writer, ApiNamedType apiNamedType, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiNamedType.ApiName, value: apiNamedType.ApiName, options: writeContext.Options);

    // ApiObjectType
    private static void WriteApiObjectType(Utf8JsonWriter writer, ApiObjectType apiObjectType, DefaultWriteContext<PropertyNames> writeContext)
    {
        WriteApiObjectTypeApiOptions(writer, apiObjectType, writeContext);
        WriteApiObjectTypeApiProperties(writer, apiObjectType, writeContext);
        WriteApiObjectTypeApiKeys(writer, apiObjectType, writeContext);
        WriteApiObjectTypeApiVersion(writer, apiObjectType, writeContext);
    }

    private static void WriteApiObjectTypeApiKeys(Utf8JsonWriter writer, ApiObjectType apiObjectType, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ApiObjectType.ApiKeys;
        var apiKeys = apiObjectType.ApiKeys;
        var options = writeContext.Options;

        writer.TryWritePropertyAsArray
        (
            propertyName: propertyName,
            collection: apiKeys,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
                writer.TryWriteWithSerializer(obj: item, options: state.Options)
        );
    }

    private static void WriteApiObjectTypeApiOptions(Utf8JsonWriter writer, ApiObjectType apiObjectType, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiObjectType.ApiOptions, obj: apiObjectType.ApiOptions, options: writeContext.Options);

    private static void WriteApiObjectTypeApiVersion
    (
        Utf8JsonWriter writer,
        ApiObjectType apiObjectType,
        DefaultWriteContext<PropertyNames> writeContext
    )
    {
        writer.TryWritePropertyWithSerializer
        (
            writeContext.PropertyNames.ApiObjectType.ApiVersion,
            apiObjectType.ApiVersion,
            writeContext.Options
        );
    }

    private static void WriteApiObjectTypeApiProperties(Utf8JsonWriter writer, ApiObjectType apiObjectType, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ApiObjectType.ApiProperties;
        var apiProperties = apiObjectType.ApiProperties;
        var options = writeContext.Options;

        writer.TryWritePropertyAsArray
        (
            propertyName: propertyName,
            collection: apiProperties,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
                writer.TryWriteWithSerializer(obj: item, options: state.Options)
        );
    }

    // ApiScalarType
    private static void WriteApiScalarType(Utf8JsonWriter writer, ApiScalarType apiScalarType, DefaultWriteContext<PropertyNames> writeContext)
    {
        // Note: ApiScalarType has no serializable body fields — intentionally left empty.
    }

    // ApiType
    private static void WriteApiTypeApiKind(Utf8JsonWriter writer, ApiType apiType, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiType.ApiKind, value: apiType.ApiKind, options: writeContext.Options, converter: _apiTypeKindJsonConverter);

    private static void WriteApiTypeClrType(Utf8JsonWriter writer, ApiType apiType, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiType.ClrType, type: apiType.ClrType, options: writeContext.Options, converter: _typeJsonConverter);
    #endregion
}
