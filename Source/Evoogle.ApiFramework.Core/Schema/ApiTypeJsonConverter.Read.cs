// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Read Types
    private class ApiCollectionTypeReadData
    {
        #region Properties
        public ApiType? ApiItemType { get; set; }
        public ApiTypeModifiers? ApiItemTypeModifiers { get; set; }
        #endregion
    }

    private class ApiEnumTypeReadData
    {
        #region Properties
        public List<ApiEnumValueReadData>? ApiEnumValues { get; set; }
        #endregion
    }

    private class ApiEnumValueReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public string? ClrName { get; set; }
        public int? ClrOrdinal { get; set; }
        #endregion
    }

    private class ApiNamedTypeReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        #endregion
    }

    private class ApiObjectTypeReadData
    {
        #region Properties
        public List<ApiPropertyReadData>? ApiProperties { get; set; }
        #endregion
    }

    private class ApiPropertyReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiType? ApiType { get; set; }
        public ApiTypeModifiers? ApiTypeModifiers { get; set; }
        public string? ClrName { get; set; }
        #endregion
    }

    private class ApiTypeReadData
    {
        #region Properties
        public Type? ClrType { get; set; }
        public string? Kind { get; set; }
        #endregion
    }

    private class ExtensibleBaseReadData
    {
        #region Properties
        public Dictionary<string, object>? Extensions { get; set; }
        #endregion
    }

    private class ReadData
    {
        #region Properties
        public ApiCollectionTypeReadData? ApiCollectionType { get; set; }
        public ApiEnumTypeReadData? ApiEnumType { get; set; }
        public ApiNamedTypeReadData? ApiNamedType { get; set; }
        public ApiObjectTypeReadData? ApiObjectType { get; set; }
        public ApiTypeReadData? ApiType { get; set; }
        public ExtensibleBaseReadData? ExtensibleBase { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiEnumValue Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiEnumValuePropertyHandlers = new()
        {
            { propertyNames.ApiEnumValue.ApiName, HandleApiEnumValueApiName },
            { propertyNames.ApiEnumValue.ClrName, HandleApiEnumValueClrName },
            { propertyNames.ApiEnumValue.ClrOrdinal, HandleApiEnumValueClrOrdinal },
        };
        #endregion

        #region ApiProperty Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiPropertyPropertyHandlers = new()
        {
            { propertyNames.ApiProperty.ApiName, HandleApiPropertyApiName },
            { propertyNames.ApiProperty.ApiType, HandleApiPropertyApiType },
            { propertyNames.ApiProperty.ApiTypeModifiers, HandleApiPropertyApiTypeModifiers },
            { propertyNames.ApiProperty.ClrName, HandleApiPropertyClrName },
        };
        #endregion

        #region ApiType Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiTypePropertyHandlers = new()
        {
            // ApiCollectionType Property Handlers
            { propertyNames.ApiCollectionType.ApiItemType, HandleApiCollectionTypeApiItemType },
            { propertyNames.ApiCollectionType.ApiItemTypeModifiers, HandleApiCollectionTypeApiItemTypeModifiers },

            // ApiEnumType Property Handlers
            { propertyNames.ApiEnumType.ApiEnumValues, HandleApiEnumTypeApiEnumValues },

            // ApiNamedType Property Handlers
            { propertyNames.ApiNamedType.ApiName, HandleApiNamedTypeApiName },

            // ApiObjectType Property Handlers
            { propertyNames.ApiObjectType.ApiProperties, HandleApiObjectTypeApiProperties },

            // ApiType Property Handlers
            { propertyNames.ApiType.Kind, HandleApiTypeKind },
            { propertyNames.ApiType.ClrType, HandleApiTypeClrType },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, HandleExtensibleBaseExtensions },
        };
        #endregion

        #region ApiCollectionType Methods
        private static void HandleApiCollectionTypeApiItemType(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiCollectionType ??= new ApiCollectionTypeReadData();

            var options = context.Options;
            context.ReadData.ApiCollectionType.ApiItemType = JsonSerializer.Deserialize<ApiType>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiCollectionType.ApiItemType}.");
        }

        private static void HandleApiCollectionTypeApiItemTypeModifiers(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiCollectionType ??= new ApiCollectionTypeReadData();

            var options = context.Options;
            var typeToConvert = typeof(ApiTypeModifiers);
            context.ReadData.ApiCollectionType.ApiItemTypeModifiers = ApiTypeModifiersJsonConverter.Read(ref reader, typeToConvert, options);
        }
        #endregion

        #region ApiEnumType Methods
        private static void HandleApiEnumTypeApiEnumValues(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumType ??= new ApiEnumTypeReadData();

            ApiJsonConverterHelpers.ReadJsonArray<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => HandleApiEnumTypeApiEnumValuesArrayItem);
        }

        private static void HandleApiEnumTypeApiEnumValuesArrayItem(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumType!.ApiEnumValues ??= [];
            context.ReadData.ApiEnumType!.ApiEnumValues.Add(new ApiEnumValueReadData());

            ApiJsonConverterHelpers.ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => x.ReadHandlers.ApiEnumValuePropertyHandlers);
        }
        #endregion

        #region ApiEnumValue Methods
        private static void HandleApiEnumValueApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiEnumValue = context.ReadData.ApiEnumType!.ApiEnumValues!.Last();

            apiEnumValue.ApiName = reader.GetString();
        }

        private static void HandleApiEnumValueClrName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiEnumValue = context.ReadData.ApiEnumType!.ApiEnumValues!.Last();

            apiEnumValue.ClrName = reader.GetString();
        }

        private static void HandleApiEnumValueClrOrdinal(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiEnumValue = context.ReadData.ApiEnumType!.ApiEnumValues!.Last();

            apiEnumValue.ClrOrdinal = reader.GetInt32();
        }
        #endregion

        #region ApiNamedType Methods
        private static void HandleApiNamedTypeApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiNamedType ??= new ApiNamedTypeReadData();

            context.ReadData.ApiNamedType.ApiName = reader.GetString();
        }
        #endregion

        #region ApiObjectType Methods
        private static void HandleApiObjectTypeApiProperties(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiObjectType ??= new ApiObjectTypeReadData();

            ApiJsonConverterHelpers.ReadJsonArray<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => HandleApiObjectTypeApiPropertiesArrayItem);
        }

        private static void HandleApiObjectTypeApiPropertiesArrayItem(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiObjectType!.ApiProperties ??= [];
            context.ReadData.ApiObjectType!.ApiProperties.Add(new ApiPropertyReadData());

            ApiJsonConverterHelpers.ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => x.ReadHandlers.ApiPropertyPropertyHandlers);
        }
        #endregion

        #region ApiProperty Methods
        private static void HandleApiPropertyApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiProperty = context.ReadData.ApiObjectType!.ApiProperties!.Last();

            apiProperty.ApiName = reader.GetString();
        }

        private static void HandleApiPropertyApiType(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiProperty = context.ReadData.ApiObjectType!.ApiProperties!.Last();

            var options = context.Options;
            apiProperty.ApiType = JsonSerializer.Deserialize<ApiType>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiProperty.ApiType}.");
        }

        private static void HandleApiPropertyApiTypeModifiers(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiProperty = context.ReadData.ApiObjectType!.ApiProperties!.Last();

            var options = context.Options;
            var typeToConvert = typeof(ApiTypeModifiers);
            apiProperty.ApiTypeModifiers = ApiTypeModifiersJsonConverter.Read(ref reader, typeToConvert, options);
        }

        private static void HandleApiPropertyClrName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiProperty = context.ReadData.ApiObjectType!.ApiProperties!.Last();

            apiProperty.ClrName = reader.GetString();
        }
        #endregion

        #region ApiType Methods
        private static void HandleApiTypeKind(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiType ??= new ApiTypeReadData();

            context.ReadData.ApiType.Kind = reader.GetString();
        }

        private static void HandleApiTypeClrType(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiType ??= new ApiTypeReadData();

            context.ReadData.ApiType.ClrType = TypeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
        #endregion

        #region ExtensibleBase Methods
        private static void HandleExtensibleBaseExtensions(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ExtensibleBase ??= new ExtensibleBaseReadData();
            context.ReadData.ExtensibleBase.Extensions ??= [];

            // Validate the start of the JSON object.
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of an object.");

            // Read the JSON extension object properties.
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected object property name.");

                // Note: We didn't apply naming policy to extension type name as property name for round-trip compatibility.
                // Read the JSON property name (extension type name)
                var extensionTypeName = reader.GetString()!;
                var extensionType = Json.TypeJsonConverter.GetDeserializeType(extensionTypeName);

                context.Logger.LogTrace("Deserializing extension type: {ExtensionType}", extensionType.Name);

                // Move past the JSON property name (extension type name)
                reader.Read();

                // Read the extension object
                var options = context.Options;
                var extension = JsonSerializer.Deserialize(ref reader, extensionType, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ExtensibleBase.Extensions}.");

                context.Logger.LogDebug("Deserialized  extension type: {ExtensionType}", extensionType.Name);

                context.ReadData.ExtensibleBase.Extensions.Add(extensionTypeName, extension);
            }
        }
        #endregion
    }
    #endregion
}
