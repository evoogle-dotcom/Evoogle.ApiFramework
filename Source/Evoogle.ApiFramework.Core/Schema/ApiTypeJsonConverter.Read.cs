// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using static Evoogle.ApiFramework.Schema.Internal.ApiJsonConverterHelpers;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Read Types
    private class ApiCollectionTypeReadData
    {
        #region Properties
        public ApiTypeExpressionReadData? ApiItemTypeExpression { get; set; }
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
        public List<ApiRelationshipReadData>? ApiRelationships { get; set; }
        #endregion
    }

    private class ApiPropertyReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiTypeExpressionReadData? ApiTypeExpression { get; set; }
        public ApiTypeModifiers? ApiTypeModifiers { get; set; }
        public string? ClrName { get; set; }
        #endregion
    }

    private class ApiPropertyExpressionReadData
    {
        #region Properties
        public ApiPropertyReadData? ApiInlineProperty { get; set; }
        public string? ApiName { get; set; }
        #endregion
    }

    private class ApiRelationshipReadData
    {
        #region Properties
        public ApiPropertyExpressionReadData? ApiPropertyExpression { get; set; }
        #endregion
    }

    private class ApiTypeReadData
    {
        #region Properties
        public Type? ClrType { get; set; }
        public string? Kind { get; set; }
        #endregion
    }

    private class ApiTypeExpressionReadData
    {
        #region Properties
        public ApiType? ApiInlineType { get; set; }
        public string? ApiName { get; set; }
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
        public ApiEnumValueReadData? ApiEnumValue { get; set; }
        public ApiNamedTypeReadData? ApiNamedType { get; set; }
        public ApiObjectTypeReadData? ApiObjectType { get; set; }
        public ApiPropertyReadData? ApiProperty { get; set; }
        public ApiPropertyExpressionReadData? ApiPropertyExpression { get; set; }
        public ApiRelationshipReadData? ApiRelationship { get; set; }
        public ApiTypeReadData? ApiType { get; set; }
        public ApiTypeExpressionReadData? ApiTypeExpression { get; set; }
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
            { propertyNames.ApiProperty.ApiTypeModifiers, HandleApiPropertyApiTypeModifiers },
            { propertyNames.ApiProperty.ApiTypeExpression, HandleApiPropertyApiTypeExpression },
            { propertyNames.ApiProperty.ClrName, HandleApiPropertyClrName },
        };
        #endregion

        #region ApiPropertyExpression Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiPropertyExpressionHandlers = new()
        {
            { propertyNames.ApiPropertyExpression.ApiInlineProperty, HandleApiPropertyExpressionApiInlineProperty },
            { propertyNames.ApiPropertyExpression.ApiName, HandleApiPropertyExpressionApiName },
        };
        #endregion

        #region ApiRelationship Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiRelationshipPropertyHandlers = new()
        {
            { propertyNames.ApiRelationship.ApiPropertyExpression, HandleApiRelationshipApiPropertyExpression },
        };
        #endregion

        #region ApiTypeExpression Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiTypeExpressionHandlers = new()
        {
            { propertyNames.ApiTypeExpression.ApiInlineType, HandleApiTypeExpressionApiInlineType },
            { propertyNames.ApiTypeExpression.ApiName, HandleApiTypeExpressionApiName },
            { propertyNames.ApiTypeExpression.Kind, HandleApiTypeExpressionKind },
        };
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
            { propertyNames.ExtensibleBase.Extensions, HandleExtensibleBaseExtensions },
        };
        #endregion

        #region ApiCollectionType Methods
        private static void HandleApiCollectionTypeApiItemTypeExpression(ref Utf8JsonReader reader, ref ReadContext context)
        {
            ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => x.ReadHandlers.ApiTypeExpressionHandlers);

            context.ReadData.ApiCollectionType ??= new ApiCollectionTypeReadData();
            context.ReadData.ApiCollectionType.ApiItemTypeExpression = context.ReadData.ApiTypeExpression;

            // Clear the ApiTypeExpression to avoid confusion in the next read operation
            context.ReadData.ApiTypeExpression = null;
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
            context.ReadData.ApiEnumType.ApiEnumValues = [];

            ReadJsonArray<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => HandleApiEnumTypeApiEnumValuesArrayItem);
        }

        private static void HandleApiEnumTypeApiEnumValuesArrayItem(ref Utf8JsonReader reader, ref ReadContext context)
        {
            ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => x.ReadHandlers.ApiEnumValuePropertyHandlers);
            if (context.ReadData.ApiEnumValue == null)
                return;

            context.ReadData.ApiEnumType!.ApiEnumValues!.Add(context.ReadData.ApiEnumValue);

            // Clear the ApiEnumValue to avoid confusion in the next read operation
            context.ReadData.ApiEnumValue = null;
        }
        #endregion

        #region ApiEnumValue Methods
        private static void HandleApiEnumValueApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumValue ??= new ApiEnumValueReadData();

            context.ReadData.ApiEnumValue.ApiName = reader.GetString();
        }

        private static void HandleApiEnumValueClrName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumValue ??= new ApiEnumValueReadData();

            context.ReadData.ApiEnumValue.ClrName = reader.GetString();
        }

        private static void HandleApiEnumValueClrOrdinal(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumValue ??= new ApiEnumValueReadData();

            context.ReadData.ApiEnumValue.ClrOrdinal = reader.GetInt32();
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
            ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => x.ReadHandlers.ApiPropertyPropertyHandlers);
            if (context.ReadData.ApiProperty == null)
                return;

            context.ReadData.ApiObjectType!.ApiProperties!.Add(context.ReadData.ApiProperty);

            // Clear the ApiProperty to avoid confusion in the next read operation
            context.ReadData.ApiProperty = null;
        }

        private static void HandleApiObjectTypeApiRelationships(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiObjectType ??= new ApiObjectTypeReadData();
            context.ReadData.ApiObjectType.ApiRelationships = [];

            ReadJsonArray<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => HandleApiObjectTypeApiRelationshipsArrayItem);
        }

        private static void HandleApiObjectTypeApiRelationshipsArrayItem(ref Utf8JsonReader reader, ref ReadContext context)
        {
            ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => x.ReadHandlers.ApiRelationshipPropertyHandlers);
            if (context.ReadData.ApiRelationship == null)
                return;

            context.ReadData.ApiObjectType!.ApiRelationships!.Add(context.ReadData.ApiRelationship);

            // Clear the ApiRelationship to avoid confusion in the next read operation
            context.ReadData.ApiRelationship = null;
        }
        #endregion

        #region ApiProperty Methods
        private static void HandleApiPropertyApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiProperty ??= new ApiPropertyReadData();

            context.ReadData.ApiProperty.ApiName = reader.GetString();
        }

        private static void HandleApiPropertyApiTypeModifiers(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiProperty ??= new ApiPropertyReadData();

            var options = context.Options;
            var typeToConvert = typeof(ApiTypeModifiers);
            context.ReadData.ApiProperty.ApiTypeModifiers = ApiTypeModifiersJsonConverter.Read(ref reader, typeToConvert, options);
        }

        private static void HandleApiPropertyApiTypeExpression(ref Utf8JsonReader reader, ref ReadContext context)
        {
            ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => x.ReadHandlers.ApiTypeExpressionHandlers);

            context.ReadData.ApiProperty ??= new ApiPropertyReadData();
            context.ReadData.ApiProperty.ApiTypeExpression = context.ReadData.ApiTypeExpression;

            // Clear the ApiTypeExpression to avoid confusion in the next read operation
            context.ReadData.ApiTypeExpression = null;
        }

        private static void HandleApiPropertyClrName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiProperty ??= new ApiPropertyReadData();

            context.ReadData.ApiProperty.ClrName = reader.GetString();
        }
        #endregion

        #region ApiPropertyExpression Fields
        private static void HandleApiPropertyExpressionApiInlineProperty(ref Utf8JsonReader reader, ref ReadContext context)
        {
            ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => x.ReadHandlers.ApiPropertyPropertyHandlers);
            if (context.ReadData.ApiProperty == null)
                return;

            context.ReadData.ApiPropertyExpression ??= new ApiPropertyExpressionReadData();
            context.ReadData.ApiPropertyExpression.ApiInlineProperty = context.ReadData.ApiProperty;

            // Clear the ApiProperty to avoid confusion in the next read operation
            context.ReadData.ApiProperty = null;
        }

        private static void HandleApiPropertyExpressionApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiPropertyExpression ??= new ApiPropertyExpressionReadData();

            context.ReadData.ApiPropertyExpression.ApiName = reader.GetString();
        }
        #endregion

        #region ApiRelationship Methods
        private static void HandleApiRelationshipApiPropertyExpression(ref Utf8JsonReader reader, ref ReadContext context)
        {
            ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (x) => x.ReadHandlers.ApiPropertyExpressionHandlers);

            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            context.ReadData.ApiRelationship.ApiPropertyExpression = context.ReadData.ApiPropertyExpression;

            // Clear the ApiPropertyExpression to avoid confusion in the next read operation
            context.ReadData.ApiPropertyExpression = null;
        }
        #endregion

        #region ApiTypeExpression Fields
        private static void HandleApiTypeExpressionApiInlineType(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ApiInlineType = JsonSerializer.Deserialize<ApiType>(ref reader, context.Options);
        }

        private static void HandleApiTypeExpressionApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ApiName = reader.GetString();
        }

        private static void HandleApiTypeExpressionKind(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.Kind = reader.GetString();
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

            context.ReadData.ExtensibleBase.Extensions = ReadExtensions(ref reader, context.Options, context.Logger);
        }
        #endregion
    }
    #endregion
}
