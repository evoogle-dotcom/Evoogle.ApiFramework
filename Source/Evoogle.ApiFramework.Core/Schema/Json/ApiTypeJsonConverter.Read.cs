// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Partial implementation of <see cref="ApiTypeJsonConverter"/> that handles JSON reading.
/// </summary>
public partial class ApiTypeJsonConverter : JsonConverterBase<ApiType>
{
    #region Read Types
    /// <summary>
    ///     Holds interim data for <see cref="ApiCollectionType"/> members encountered during deserialization.
    /// </summary>
    private class ApiCollectionTypeReadData
    {
        #region Properties
        public ApiTypeExpression? ApiItemTypeExpression { get; set; }
        public ApiTypeModifiers? ApiItemTypeModifiers { get; set; }
        #endregion
    }

    /// <summary>
    ///     Holds interim data for <see cref="ApiEnumType"/> members encountered during deserialization.
    /// </summary>
    private class ApiEnumTypeReadData
    {
        #region Properties
        public List<ApiEnumValue>? ApiEnumValues { get; set; }
        #endregion
    }

    /// <summary>
    ///     Holds interim data for <see cref="ApiNamedType"/> members encountered during deserialization.
    /// </summary>
    private class ApiNamedTypeReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        #endregion
    }

    /// <summary>
    ///     Holds interim data for <see cref="ApiObjectType"/> members encountered during deserialization.
    /// </summary>
    private class ApiObjectTypeReadData
    {
        #region Properties
        public ApiObjectTypeOptions? ApiOptions { get; set; }
        public List<ApiIdentity>? ApiIdentities { get; set; }
        public List<ApiProperty>? ApiProperties { get; set; }
        #endregion
    }

    /// <summary>
    ///     Holds interim data for common <see cref="ApiType"/> members encountered during deserialization.
    /// </summary>
    private class ApiTypeReadData
    {
        #region Properties
        public Type? ClrType { get; set; }
        public ApiTypeKind? ApiKind { get; set; }
        #endregion
    }

    /// <summary>
    ///     Aggregates all temporary data required to create an <see cref="ApiType"/> instance from JSON.
    /// </summary>
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

    /// <summary>
    ///     Provides JSON property handlers that map serialized data to <see cref="ReadData"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _apiTypeModifiersType = typeof(ApiTypeModifiers);
        #endregion

        #region ApiType Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiCollectionType Property Handlers
            { propertyNames.ApiCollectionType.ApiItemTypeExpression, HandleApiCollectionTypeApiItemTypeExpression },
            { propertyNames.ApiCollectionType.ApiItemTypeModifiers, HandleApiCollectionTypeApiItemTypeModifiers },

            // ApiEnumType Property Handlers
            { propertyNames.ApiEnumType.ApiEnumValues, HandleApiEnumTypeApiEnumValues },

            // ApiNamedType Property Handlers
            { propertyNames.ApiNamedType.ApiName, HandleApiNamedTypeApiName },

            // ApiObjectType Property Handlers
            { propertyNames.ApiObjectType.ApiOptions, HandleApiObjectTypeApiOptions },
            { propertyNames.ApiObjectType.ApiIdentities, HandleApiObjectTypeApiIdentities },
            { propertyNames.ApiObjectType.ApiProperties, HandleApiObjectTypeApiProperties },

            // ApiType Property Handlers
            { propertyNames.ApiType.ApiKind, HandleApiTypeApiKind },
            { propertyNames.ApiType.ClrType, HandleApiTypeClrType },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiCollectionType Methods
        private static void HandleApiCollectionTypeApiItemTypeExpression(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiCollectionType ??= new ApiCollectionTypeReadData();

            context.ReadData.ApiCollectionType.ApiItemTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, context.Options);
        }

        private static void HandleApiCollectionTypeApiItemTypeModifiers(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiCollectionType ??= new ApiCollectionTypeReadData();

            var options = context.Options;
            context.ReadData.ApiCollectionType.ApiItemTypeModifiers = _apiTypeModifiersJsonConverter.Read(ref reader, _apiTypeModifiersType, options);
        }
        #endregion

        #region ApiEnumType Methods
        private static void HandleApiEnumTypeApiEnumValues(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiEnumType ??= new ApiEnumTypeReadData();
            context.ReadData.ApiEnumType.ApiEnumValues ??= [];

            ReadJsonArray(ref reader, context, (x) => HandleApiEnumTypeApiEnumValuesArrayItem);
        }

        private static void HandleApiEnumTypeApiEnumValuesArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
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
        private static void HandleApiNamedTypeApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiNamedType ??= new ApiNamedTypeReadData();

            context.ReadData.ApiNamedType.ApiName = reader.GetString();
        }
        #endregion

        #region ApiObjectType Methods
        private static void HandleApiObjectTypeApiOptions(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiObjectType ??= new ApiObjectTypeReadData();

            context.ReadData.ApiObjectType.ApiOptions = JsonSerializer.Deserialize<ApiObjectTypeOptions>(ref reader, context.Options);
        }

        private static void HandleApiObjectTypeApiIdentities(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiObjectType ??= new ApiObjectTypeReadData();
            context.ReadData.ApiObjectType.ApiIdentities ??= [];

            ReadJsonArray(ref reader, context, (x) => HandleApiObjectTypeApiIdentitiesArrayItem);
        }

        private static void HandleApiObjectTypeApiIdentitiesArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var apiIdentity = JsonSerializer.Deserialize<ApiIdentity>(ref reader, context.Options);
            if (apiIdentity == null)
            {
                return;
            }

            context.ReadData.ApiObjectType!.ApiIdentities!.Add(apiIdentity);
        }

        private static void HandleApiObjectTypeApiProperties(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiObjectType ??= new ApiObjectTypeReadData();
            context.ReadData.ApiObjectType.ApiProperties ??= [];

            ReadJsonArray(ref reader, context, (x) => HandleApiObjectTypeApiPropertiesArrayItem);
        }

        private static void HandleApiObjectTypeApiPropertiesArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var apiProperty = JsonSerializer.Deserialize<ApiProperty>(ref reader, context.Options);
            if (apiProperty == null)
            {
                return;
            }

            context.ReadData.ApiObjectType!.ApiProperties!.Add(apiProperty);
        }

        #endregion

        #region ApiType Methods
        private static void HandleApiTypeApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiType ??= new ApiTypeReadData();

            var options = context.Options;
            context.ReadData.ApiType.ApiKind = _apiTypeKindJsonConverter.Read(ref reader, typeof(ApiTypeKind), options);
        }

        private static void HandleApiTypeClrType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiType ??= new ApiTypeReadData();

            context.ReadData.ApiType.ClrType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
        #endregion
    }
    #endregion
}
