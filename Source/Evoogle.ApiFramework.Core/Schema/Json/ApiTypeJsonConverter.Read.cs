// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using static Evoogle.ApiFramework.Schema.Json.Internal.ApiJsonConverterHelpers;
using Evoogle.ApiFramework.Schema.Json.Internal;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Partial implementation of <see cref="ApiTypeJsonConverter"/> that handles JSON reading.
/// </summary>
public partial class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Read Types
    private class ApiCollectionTypeReadData
    {
        #region Properties
        public ApiTypeExpression? ApiItemTypeExpression { get; set; }
        public ApiTypeModifiers? ApiItemTypeModifiers { get; set; }
        #endregion
    }

    private class ApiEnumTypeReadData
    {
        #region Properties
        public List<ApiEnumValue>? ApiEnumValues { get; set; }
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
        public List<ApiProperty>? ApiProperties { get; set; }
        public List<ApiRelationship>? ApiRelationships { get; set; }
        #endregion
    }

    private class ApiTypeReadData
    {
        #region Properties
        public Type? ClrType { get; set; }
        public string? Kind { get; set; }
        #endregion
    }

    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiCollectionTypeReadData? ApiCollectionType { get; set; }
        public ApiEnumTypeReadData? ApiEnumType { get; set; }
        public ApiNamedTypeReadData? ApiNamedType { get; set; }
        public ApiObjectTypeReadData? ApiObjectType { get; set; }
        public ApiTypeReadData? ApiType { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _apiTypeModifiersType = typeof(ApiTypeModifiers);
        #endregion

        #region ApiType Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiTypePropertyHandlers = new()
        {
            // ApiCollectionType Property Handlers
            { propertyNames.ApiCollectionType.ApiItemTypeExpression, HandleApiCollectionTypeApiItemTypeExpression },
            { propertyNames.ApiCollectionType.ApiItemTypeModifiers, HandleApiCollectionTypeApiItemTypeModifiers },

            // ApiEnumType Property Handlers
            { propertyNames.ApiEnumType.ApiEnumValues, HandleApiEnumTypeApiEnumValues },

            // ApiNamedType Property Handlers
            { propertyNames.ApiNamedType.ApiName, HandleApiNamedTypeApiName },

            // ApiObjectType Property Handlers
            { propertyNames.ApiObjectType.ApiProperties, HandleApiObjectTypeApiProperties },
            { propertyNames.ApiObjectType.ApiRelationships, HandleApiObjectTypeApiRelationships },

            // ApiType Property Handlers
            { propertyNames.ApiType.Kind, HandleApiTypeKind },
            { propertyNames.ApiType.ClrType, HandleApiTypeClrType },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, (ref Utf8JsonReader reader, ref ReadContext context) =>
                context.ReadData.Extensions = ReadExtensions(ref reader, context.Options, context.Logger) },
        };
        #endregion

        #region ApiCollectionType Methods
        private static void HandleApiCollectionTypeApiItemTypeExpression(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiCollectionType ??= new ApiCollectionTypeReadData();

            context.ReadData.ApiCollectionType.ApiItemTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, context.Options);
        }

        private static void HandleApiCollectionTypeApiItemTypeModifiers(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiCollectionType ??= new ApiCollectionTypeReadData();

            var options = context.Options;
            context.ReadData.ApiCollectionType.ApiItemTypeModifiers = _apiTypeModifiersJsonConverter.Read(ref reader, _apiTypeModifiersType, options);
        }
        #endregion

        #region ApiEnumType Methods
        private static void HandleApiEnumTypeApiEnumValues(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumType ??= new ApiEnumTypeReadData();
            context.ReadData.ApiEnumType.ApiEnumValues = [];

            ReadJsonArray<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => HandleApiEnumTypeApiEnumValuesArrayItem);
        }

        private static void HandleApiEnumTypeApiEnumValuesArrayItem(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiEnumValue = JsonSerializer.Deserialize<ApiEnumValue>(ref reader, context.Options);
            if (apiEnumValue == null)
            {
                return;
            }

            context.ReadData.ApiEnumType!.ApiEnumValues!.Add(apiEnumValue);
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
            context.ReadData.ApiObjectType.ApiProperties = [];

            ReadJsonArray<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => HandleApiObjectTypeApiPropertiesArrayItem);
        }

        private static void HandleApiObjectTypeApiPropertiesArrayItem(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiProperty = JsonSerializer.Deserialize<ApiProperty>(ref reader, context.Options);
            if (apiProperty == null)
            {
                return;
            }

            context.ReadData.ApiObjectType!.ApiProperties!.Add(apiProperty);
        }

        private static void HandleApiObjectTypeApiRelationships(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiObjectType ??= new ApiObjectTypeReadData();
            context.ReadData.ApiObjectType.ApiRelationships = [];

            ReadJsonArray<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => HandleApiObjectTypeApiRelationshipsArrayItem);
        }

        private static void HandleApiObjectTypeApiRelationshipsArrayItem(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var apiRelationship = JsonSerializer.Deserialize<ApiRelationship>(ref reader, context.Options);
            if (apiRelationship == null)
            {
                return;
            }

            context.ReadData.ApiObjectType!.ApiRelationships!.Add(apiRelationship);
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

            context.ReadData.ApiType.ClrType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
        #endregion

    }
    #endregion
}
