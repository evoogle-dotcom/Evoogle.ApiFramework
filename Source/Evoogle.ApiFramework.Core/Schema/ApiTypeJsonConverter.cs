// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema;

public class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Property Types
    private class ApiTypePropertyNames(JsonNamingPolicy policy)
    {
        #region Fields
        public readonly string ClrType = policy.ConvertName(nameof(ApiType.ClrType));
        public readonly string Extensions = policy.ConvertName(nameof(ApiType.Extensions));
        public readonly string Kind = policy.ConvertName(nameof(ApiType.Kind));
        #endregion
    }

    private class ApiNamedTypePropertyNames(JsonNamingPolicy policy)
    {
        #region Fields
        public readonly string ApiName = policy.ConvertName(nameof(ApiNamedType.ApiName));
        #endregion
    }

    private class ApiCollectionTypePropertyNames(JsonNamingPolicy policy)
    {
        #region Fields
        public readonly string ApiItemType = policy.ConvertName(nameof(ApiCollectionType.ApiItemType));
        public readonly string ApiItemTypeModifiers = policy.ConvertName(nameof(ApiCollectionType.ApiItemTypeModifiers));
        #endregion
    }

    private class ApiEnumTypePropertyNames(JsonNamingPolicy policy)
    {
        #region Fields
        public readonly string ApiEnumValues = policy.ConvertName(nameof(ApiEnumType.ApiEnumValues));
        #endregion
    }

    private class ApiEnumValuePropertyNames(JsonNamingPolicy policy)
    {
        #region Fields
        public readonly string ApiName = policy.ConvertName(nameof(ApiEnumValue.ApiName));
        public readonly string ClrName = policy.ConvertName(nameof(ApiEnumValue.ClrName));
        public readonly string ClrOrdinal = policy.ConvertName(nameof(ApiEnumValue.ClrOrdinal));
        #endregion
    }

    private class ApiObjectTypePropertyNames(JsonNamingPolicy policy)
    {
        #region Fields
        public readonly string ApiProperties = policy.ConvertName(nameof(ApiObjectType.ApiProperties));
        #endregion
    }

    private class ApiPropertyPropertyNames(JsonNamingPolicy policy)
    {
        #region Fields
        public readonly string ApiName = policy.ConvertName(nameof(ApiProperty.ApiName));
        public readonly string ApiType = policy.ConvertName(nameof(ApiProperty.ApiType));
        public readonly string ApiTypeModifiers = policy.ConvertName(nameof(ApiProperty.ApiTypeModifiers));
        public readonly string ClrName = policy.ConvertName(nameof(ApiProperty.ClrName));
        #endregion
    }

    private class PropertyNames(JsonNamingPolicy policy)
    {
        #region Fields
        public readonly ApiTypePropertyNames ApiType = new(policy);
        public readonly ApiNamedTypePropertyNames ApiNamedType = new(policy);
        public readonly ApiCollectionTypePropertyNames ApiCollectionType = new(policy);
        public readonly ApiEnumTypePropertyNames ApiEnumType = new(policy);
        public readonly ApiEnumValuePropertyNames ApiEnumValue = new(policy);
        public readonly ApiObjectTypePropertyNames ApiObjectType = new(policy);
        public readonly ApiPropertyPropertyNames ApiProperty = new(policy);
        #endregion
    }
    #endregion

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
        public Dictionary<string, object>? Extensions { get; set; }
        public string? Kind { get; set; }
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
        #endregion
    }

    private delegate void ReadHandler(ref Utf8JsonReader reader, ref ReadContext context);

    private class ReadHandlers(JsonNamingPolicy policy)
    {
        #region Fields
        public readonly Dictionary<string, ReadHandler> ApiEnumValuePropertyHandlers = new()
        {
            {
                policy.ConvertName(nameof(ApiEnumValue.ApiName)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    var apiEnumValue = context.ReadData.ApiEnumType!.ApiEnumValues!.Last();
                    apiEnumValue.ApiName = reader.GetString();
                }
            },
            {
                policy.ConvertName(nameof(ApiEnumValue.ClrName)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    var apiEnumValue = context.ReadData.ApiEnumType!.ApiEnumValues!.Last();
                    apiEnumValue.ClrName = reader.GetString();
                }
            },
            {
                policy.ConvertName(nameof(ApiEnumValue.ClrOrdinal)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    var apiEnumValue = context.ReadData.ApiEnumType!.ApiEnumValues!.Last();
                    apiEnumValue.ClrOrdinal = reader.GetInt32();
                }
            },
        };

        public readonly Dictionary<string, ReadHandler> ApiPropertyPropertyHandlers = new()
        {
            {
                policy.ConvertName(nameof(ApiProperty.ApiName)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    var apiProperty = context.ReadData.ApiObjectType!.ApiProperties!.Last();
                    apiProperty.ApiName = reader.GetString();
                }
            },
            {
                policy.ConvertName(nameof(ApiProperty.ApiType)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    var apiProperty = context.ReadData.ApiObjectType!.ApiProperties!.Last();
                    apiProperty.ApiType = JsonSerializer.Deserialize<ApiType>(ref reader, context.Options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiCollectionType.ApiItemType}.");
                }
            },
            {
                policy.ConvertName(nameof(ApiProperty.ApiTypeModifiers)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    var apiProperty = context.ReadData.ApiObjectType!.ApiProperties!.Last();
                    apiProperty.ApiTypeModifiers = ApiTypeModifiersJsonConverter.Read(ref reader, typeof(ApiTypeModifiers), context.Options);
                }
            },
            {
                policy.ConvertName(nameof(ApiProperty.ClrName)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    var apiProperty = context.ReadData.ApiObjectType!.ApiProperties!.Last();
                    apiProperty.ClrName = reader.GetString();
                }
            },
        };

        public readonly Dictionary<string, ReadHandler> ApiTypePropertyHandlers = new()
        {
            // ApiType Property Handlers
            {
                policy.ConvertName(nameof(ApiType.Kind)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    context.ReadData.ApiType ??= new ApiTypeReadData();
                    context.ReadData.ApiType.Kind = reader.GetString();
                }
            },
            {
                policy.ConvertName(nameof(ApiType.ClrType)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    context.ReadData.ApiType ??= new ApiTypeReadData();
                    context.ReadData.ApiType.ClrType = TypeJsonConverter.Read(ref reader, typeof(Type), context.Options);
                }
            },
            {
                policy.ConvertName(nameof(ApiType.Extensions)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    context.ReadData.ApiType ??= new ApiTypeReadData();
                    context.ReadData.ApiType.Extensions = JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, context.Options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiType.Extensions}.");
                }
            },

            // ApiNamedType Property Handlers
            {
                policy.ConvertName(nameof(ApiNamedType.ApiName)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    context.ReadData.ApiNamedType ??= new ApiNamedTypeReadData();
                    context.ReadData.ApiNamedType.ApiName = reader.GetString();
                }
            },

            // ApiCollectionType Property Handlers
            {
                policy.ConvertName(nameof(ApiCollectionType.ApiItemType)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    context.ReadData.ApiCollectionType ??= new ApiCollectionTypeReadData();
                    context.ReadData.ApiCollectionType.ApiItemType = JsonSerializer.Deserialize<ApiType>(ref reader, context.Options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiCollectionType.ApiItemType}.");
                }
            },
            {
                policy.ConvertName(nameof(ApiCollectionType.ApiItemTypeModifiers)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    context.ReadData.ApiCollectionType ??= new ApiCollectionTypeReadData();
                    context.ReadData.ApiCollectionType.ApiItemTypeModifiers = ApiTypeModifiersJsonConverter.Read(ref reader, typeof(ApiTypeModifiers), context.Options);
                }
            },

            // ApiEnumType Property Handlers
            {
                policy.ConvertName(nameof(ApiEnumType.ApiEnumValues)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    context.ReadData.ApiEnumType ??= new ApiEnumTypeReadData();

                    ReadJsonArray(ref reader, ref context, (x) => ReadApiEnumValueArrayItem);
                }
            },

            // ApiObjectType Property Handlers
            {
                policy.ConvertName(nameof(ApiObjectType.ApiProperties)),
                (ref Utf8JsonReader reader, ref ReadContext context) =>
                {
                    context.ReadData.ApiObjectType ??= new ApiObjectTypeReadData();

                    ReadJsonArray(ref reader, ref context, (x) => ReadApiPropertyArrayItem);
                }
            },
        };
        #endregion

        #region Methods
        private static void ReadApiEnumValueArrayItem(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumType!.ApiEnumValues ??= [];
            context.ReadData.ApiEnumType!.ApiEnumValues.Add(new ApiEnumValueReadData());

            ReadJsonObject(ref reader, ref context, (x) => x.ReadHandlers.ApiEnumValuePropertyHandlers);
        }

        private static void ReadApiPropertyArrayItem(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiObjectType!.ApiProperties ??= [];
            context.ReadData.ApiObjectType!.ApiProperties.Add(new ApiPropertyReadData());

            ReadJsonObject(ref reader, ref context, (x) => x.ReadHandlers.ApiPropertyPropertyHandlers);
        }
        #endregion
    }
    #endregion

    #region Read/Write Types
    private abstract class Context(JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
    {
        #region Immutable Properties
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
        #endregion

        #region Methods
        public string GetPropertyName(string name)
        {
            return PropertyNamingPolicy.ConvertName(name);
        }
        #endregion
    }

    private class ReadContext(JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(options, propertyNamingPolicy, propertyNames)
    {
        #region Immutable Properties
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        #endregion

        #region Mutable Properties
        public ReadData ReadData { get; } = new ReadData();
        #endregion
    }

    private class WriteContext(JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(options, propertyNamingPolicy, propertyNames)
    {
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeModifiers> ApiTypeModifiersJsonConverter = new();
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> PropertyNamesCache = new();
    private static readonly NullJsonNamingPolicy NullJsonNamingPolicy = new();
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> ReadHandlersCache = new();
    private static readonly TypeJsonConverter TypeJsonConverter = new();
    #endregion

    #region JsonConverter<T> Methods
    public override ApiType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var readHandlers = GetReadHandlers(propertyNamingPolicy);
        var context = new ReadContext(options, propertyNamingPolicy, propertyNames, readHandlers);

        ReadJsonObject(ref reader, ref context, (context) => context.ReadHandlers.ApiTypePropertyHandlers);

        var apiType = CreateApiType(context);
        return apiType;
    }

    public override void Write(Utf8JsonWriter writer, ApiType apiType, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var context = new WriteContext(options, propertyNamingPolicy, propertyNames);

        WriteApiTypeProlog(writer, apiType, ref context);
        WriteApiTypeBody(writer, apiType, ref context);
        WriteApiTypeEpilog(writer, apiType, ref context);
    }
    #endregion

    #region Cache Implementation Methods
    private static JsonNamingPolicy GetPropertyNamingPolicy(JsonSerializerOptions options)
    {
        var policy = options.PropertyNamingPolicy ?? NullJsonNamingPolicy;
        return policy;
    }

    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return PropertyNamesCache.GetOrAdd(policy, policy => new PropertyNames(policy));
    }

    private static ReadHandlers GetReadHandlers(JsonNamingPolicy policy)
    {
        return ReadHandlersCache.GetOrAdd(policy, policy => new ReadHandlers(policy));
    }
    #endregion

    #region Factory Implementation Methods
    private static void AttachExtensions(ReadContext context, ApiType apiType)
    {
        var extensions = context.ReadData.ApiType?.Extensions;
        if (extensions != null)
        {
            foreach (var (extensionTypeName, extension) in extensions)
            {
                var extensionType = TypeJsonConverter.GetDeserializeType(extensionTypeName);
                apiType.AttachExtension(extensionType, extension);
            }
        }
    }

    private static ApiCollectionType CreateApiCollectionType(ReadContext context, List<ValidationResult>? validationResults)
    {
        HandleValidationResults(validationResults);

        var apiItemType = context.ReadData.ApiCollectionType!.ApiItemType!;
        var apiItemTypeModifiers = context.ReadData.ApiCollectionType!.ApiItemTypeModifiers!.Value;
        var clrCollectionType = context.ReadData.ApiType!.ClrType!;

        return new ApiCollectionType(apiItemType, apiItemTypeModifiers, clrCollectionType);
    }

    private static ApiEnumType CreateApiEnumType(ReadContext context, List<ValidationResult>? validationResults)
    {
        HandleValidationResults(validationResults);

        var apiName = context.ReadData.ApiNamedType!.ApiName!;
        var apiEnumValues = CreateApiEnumValues(context);
        var clrType = context.ReadData.ApiType!.ClrType!;

        return new ApiEnumType(apiName, apiEnumValues, clrType);
    }

    private static List<ApiEnumValue> CreateApiEnumValues(ReadContext context)
    {
        var apiEnumValues = context.ReadData.ApiEnumType!.ApiEnumValues!.Select(x =>
        {
            var apiName = x.ApiName!;
            var clrName = x.ClrName!;
            var clrOrdinal = x.ClrOrdinal!.Value;

            var apiEnumValue = new ApiEnumValue(apiName, clrName, clrOrdinal);
            return apiEnumValue;
        })
        .ToList();

        return apiEnumValues;
    }

    private static ApiObjectType CreateApiObjectType(ReadContext context, List<ValidationResult>? validationResults)
    {
        HandleValidationResults(validationResults);

        var apiName = context.ReadData.ApiNamedType!.ApiName!;
        var apiProperties = context.ReadData.ApiObjectType!.ApiProperties!.Select(x =>
        {
            var apiName = x.ApiName!;
            var apiType = x.ApiType!;
            var apiTypeModifiers = x.ApiTypeModifiers!.Value;
            var clrName = x.ClrName!;

            var apiProperty = new ApiProperty(apiName, apiType, apiTypeModifiers, clrName);
            return apiProperty;
        })
        .ToList();
        var clrType = context.ReadData.ApiType!.ClrType!;

        return new ApiObjectType(apiName, apiProperties, clrType);
    }

    private static ApiScalarType CreateApiScalarType(ReadContext context, List<ValidationResult>? validationResults)
    {
        HandleValidationResults(validationResults);

        var apiName = context.ReadData.ApiNamedType!.ApiName!;
        var clrType = context.ReadData.ApiType!.ClrType!;

        return new ApiScalarType(apiName, clrType);
    }

    private static void HandleValidationResults(IEnumerable<ValidationResult>? validationResults)
    {
        if (validationResults == null)
            return;

        if (validationResults.Any() == false)
            return;

        // Create a delimited string of all the failed validation result error messages.
        var message = validationResults.Where(x => x != ValidationResult.Success && !string.IsNullOrWhiteSpace(x.ErrorMessage)).SafeToDelimitedString('\n');
        if (string.IsNullOrWhiteSpace(message))
            return;

        throw new JsonException(message);
    }
    #endregion

    #region Read Implementation Methods
    private static ApiType? CreateApiType(ReadContext context)
    {
        // Validate all required properties are non-null.
        var validationResults = default(List<ValidationResult>);
        ValidateApiTypeProperties(context, validationResults);

        var kind = context.ReadData.ApiType!.Kind;

        // Create the appropriate derived ApiType based on the guidance of the Kind enumeration value.
        var apiType = default(ApiType);
        switch (kind)
        {
            case nameof(ApiTypeKind.Collection):
                {
                    ValidateApiCollectionTypeProperties(context, validationResults);

                    apiType = CreateApiCollectionType(context, validationResults);
                }
                break;

            case nameof(ApiTypeKind.Enum):
                {
                    ValidateApiNamedTypeProperties(context, validationResults);
                    ValidateApiEnumTypeProperties(context, validationResults);

                    apiType = CreateApiEnumType(context, validationResults);
                }
                break;

            case nameof(ApiTypeKind.Object):
                {
                    ValidateApiNamedTypeProperties(context, validationResults);
                    ValidateApiObjectTypeProperties(context, validationResults);

                    apiType = CreateApiObjectType(context, validationResults);
                }
                break;

            case nameof(ApiTypeKind.Scalar):
                {
                    ValidateApiNamedTypeProperties(context, validationResults);

                    apiType = CreateApiScalarType(context, validationResults);
                }
                break;

            default:
                {
                    throw new JsonException($"Unsupported Kind: {kind}");
                }
        }

        AttachExtensions(context, apiType);

        return apiType;
    }

    private static void ReadJsonArray
    (
        ref Utf8JsonReader reader,
        ref ReadContext context,
        Func<ReadContext, ReadHandler> arrayElementHandlerAccessor
    )
    {
        // Validate the start of the JSON array.
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of an array.");

        // Iterate through the JSON array elements.
        var handler = arrayElementHandlerAccessor(context);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            // Execute the JSON array element handler to read the JSON array element.
            handler(ref reader, ref context);
        }
    }

    private static void ReadJsonObject
    (
        ref Utf8JsonReader reader,
        ref ReadContext context,
        Func<ReadContext, Dictionary<string, ReadHandler>> propertyHandlersAccessor
    )
    {
        // Validate the start of the JSON object.
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of an object.");

        // Read the JSON object properties.
        var handlers = propertyHandlersAccessor(context);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected object property name.");

            // Read the JSON property name.
            var propertyName = reader.GetString()!;

            // Move past the JSON property name.
            reader.Read();

            // Look up the JSON property value handler based on the JSON property name.
            if (handlers.TryGetValue(propertyName, out var handler))
            {
                // Execute the JSON property value handler to read the JSON property value.
                handler(ref reader, ref context);
            }
            else
            {
                // Skip unknown JSON object properties.
                reader.Skip();
            }
        }
    }
    #endregion

    #region Validation Implementation Methods
    private static void ValidateApiCollectionTypeProperties
    (
        ReadContext context,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiCollectionTypeProperties
        (
            context.PropertyNames.ApiCollectionType.ApiItemType, context.ReadData.ApiCollectionType?.ApiItemType,
            context.PropertyNames.ApiCollectionType.ApiItemTypeModifiers, context.ReadData.ApiCollectionType?.ApiItemTypeModifiers,
            validationResults
        );
    }

    private static void ValidateApiCollectionTypeProperties
    (
        string apiItemTypePropertyName, ApiType? apiItemType,
        string apiItemTypeModifiersPropertyName, ApiTypeModifiers? apiItemTypeModifiers,
        List<ValidationResult>? validationResults
    )
    {
        if (apiItemType == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {apiItemTypePropertyName}", [apiItemTypePropertyName]));
        }

        if (apiItemTypeModifiers == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {apiItemTypeModifiersPropertyName}", [apiItemTypeModifiersPropertyName]));
        }
    }

    private static void ValidateApiEnumTypeProperties
    (
        ReadContext context,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiEnumTypeProperties
        (
            context,
            context.PropertyNames.ApiEnumType.ApiEnumValues, context.ReadData.ApiEnumType?.ApiEnumValues,
            validationResults
        );
    }

    private static void ValidateApiEnumTypeProperties
    (
        ReadContext context,
        string apiEnumValuesPropertyName, IEnumerable<ApiEnumValueReadData>? apiEnumValues,
        List<ValidationResult>? validationResults
    )
    {
        if (apiEnumValues == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {apiEnumValuesPropertyName}", [apiEnumValuesPropertyName]));
        }
        else
        {
            if (apiEnumValues.Any() == false)
            {
                validationResults ??= [];
                validationResults.Add(new ValidationResult($"Empty required property: {apiEnumValuesPropertyName}", [apiEnumValuesPropertyName]));
            }

            foreach (var apiEnumValue in apiEnumValues)
            {
                ValidateApiEnumValueProperties
                (
                    context.PropertyNames.ApiEnumValue.ApiName, apiEnumValue.ApiName,
                    context.PropertyNames.ApiEnumValue.ClrName, apiEnumValue.ClrName,
                    context.PropertyNames.ApiEnumValue.ClrOrdinal, apiEnumValue.ClrOrdinal,
                    validationResults
                );
            }
        }
    }

    private static void ValidateApiEnumValueProperties
    (
        string apiNamePropertyName, string? apiName,
        string clrNamePropertyName, string? clrName,
        string clrOrdinalPropertyName, long? clrOrdinal,
        List<ValidationResult>? validationResults
    )
    {
        if (apiName == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {apiNamePropertyName}", [apiNamePropertyName]));
        }

        if (clrName == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {clrNamePropertyName}", [clrNamePropertyName]));
        }

        if (clrOrdinal == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {clrOrdinalPropertyName}", [clrOrdinalPropertyName]));
        }
    }

    private static void ValidateApiObjectTypeProperties
    (
        ReadContext context,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiObjectTypeProperties
        (
            context,
            context.PropertyNames.ApiObjectType.ApiProperties, context.ReadData.ApiObjectType?.ApiProperties,
            validationResults
        );
    }

    private static void ValidateApiObjectTypeProperties
    (
        ReadContext context,
        string apiPropertiesPropertyName, IEnumerable<ApiPropertyReadData>? apiProperties,
        List<ValidationResult>? validationResults
    )
    {
        if (apiProperties == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {apiPropertiesPropertyName}", [apiPropertiesPropertyName]));
        }
        else
        {
            if (apiProperties.Any() == false)
            {
                validationResults ??= [];
                validationResults.Add(new ValidationResult($"Empty required property: {apiPropertiesPropertyName}", [apiPropertiesPropertyName]));
            }

            foreach (var apiProperty in apiProperties)
            {
                ValidateApiPropertyProperties
                (
                    context.PropertyNames.ApiProperty.ApiName, apiProperty.ApiName,
                    context.PropertyNames.ApiProperty.ApiType, apiProperty.ApiType,
                    context.PropertyNames.ApiProperty.ApiTypeModifiers, apiProperty.ApiTypeModifiers,
                    context.PropertyNames.ApiProperty.ClrName, apiProperty.ClrName,
                    validationResults
                );
            }
        }
    }

    private static void ValidateApiPropertyProperties
    (
        string apiNamePropertyName, string? apiName,
        string apiTypePropertyName, ApiType? apiType,
        string apiTypeModifiersPropertyName, ApiTypeModifiers? apiTypeModifiers,
        string clrNamePropertyName, string? clrName,
        List<ValidationResult>? validationResults
    )
    {
        if (apiName == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {apiNamePropertyName}", [apiNamePropertyName]));
        }

        if (apiType == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {apiTypePropertyName}", [apiTypePropertyName]));
        }

        if (apiTypeModifiers == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {apiTypeModifiersPropertyName}", [apiTypeModifiersPropertyName]));
        }

        if (clrName == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {clrNamePropertyName}", [clrNamePropertyName]));
        }
    }

    private static void ValidateApiNamedTypeProperties
    (
        ReadContext context,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiNamedTypeProperties
        (
            context.PropertyNames.ApiNamedType.ApiName, context.ReadData.ApiNamedType?.ApiName,
            validationResults
        );
    }

    private static void ValidateApiNamedTypeProperties
    (
        string apiNamePropertyName, string? apiName,
        List<ValidationResult>? validationResults
    )
    {
        if (apiName == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {apiNamePropertyName}", [apiNamePropertyName]));
        }
    }

    private static void ValidateApiTypeProperties
    (
        ReadContext context,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiTypeProperties
        (
            context.PropertyNames.ApiType.Kind, context.ReadData.ApiType?.Kind,
            context.PropertyNames.ApiType.ClrType, context.ReadData.ApiType?.ClrType,
            validationResults
        );
    }

    private static void ValidateApiTypeProperties
    (
        string kindPropertyName, string? kind,
        string clrTypePropertyName, Type? clrType,
        List<ValidationResult>? validationResults
    )
    {
        if (kind == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {kindPropertyName}", [kindPropertyName]));
        }

        if (clrType == null)
        {
            validationResults ??= [];
            validationResults.Add(new ValidationResult($"Missing required property: {clrTypePropertyName}", [clrTypePropertyName]));
        }
    }
    #endregion

    #region Write Implementation Methods
    // ApiCollectionType
    private static void WriteApiCollectionType
    (
        Utf8JsonWriter writer,
        ApiCollectionType apiCollectionType,
        ref WriteContext context
    )
    {
        WriteApiCollectionTypeApiItemType(writer, apiCollectionType, ref context);
        WriteApiCollectionTypeApiItemTypeModifiers(writer, apiCollectionType, ref context);
    }

    private static void WriteApiCollectionTypeApiItemType
    (
        Utf8JsonWriter writer,
        ApiCollectionType apiCollectionType,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiCollectionType.ApiItemType;
        writer.WritePropertyName(propertyName);

        var apiItemType = apiCollectionType.ApiItemType;
        var options = context.Options;
        JsonSerializer.Serialize(writer, apiItemType, options);
    }

    private static void WriteApiCollectionTypeApiItemTypeModifiers
    (
        Utf8JsonWriter writer,
        ApiCollectionType apiCollectionType,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiCollectionType.ApiItemTypeModifiers;
        writer.WritePropertyName(propertyName);

        var apiItemTypeModifiers = apiCollectionType.ApiItemTypeModifiers;
        var options = context.Options;

        ApiTypeModifiersJsonConverter.Write(writer, apiItemTypeModifiers, options);
    }

    // ApiEnumType
    private static void WriteApiEnumType
    (
        Utf8JsonWriter writer,
        ApiEnumType apiEnumType,
        ref WriteContext context
    )
    {
        WriteApiEnumTypeApiEnumValues(writer, apiEnumType, ref context);
    }

    private static void WriteApiEnumTypeApiEnumValues
    (
        Utf8JsonWriter writer,
        ApiEnumType apiEnumType,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiEnumType.ApiEnumValues;
        writer.WritePropertyName(propertyName);

        writer.WriteStartArray();

        var apiEnumValues = apiEnumType.ApiEnumValues;
        foreach (var apiEnumValue in apiEnumValues)
        {
            WriteApiEnumValue(writer, apiEnumValue, ref context);
        }

        writer.WriteEndArray();
    }

    // ApiEnumValue
    private static void WriteApiEnumValue
    (
        Utf8JsonWriter writer,
        ApiEnumValue apiEnumValue,
        ref WriteContext context
    )
    {
        writer.WriteStartObject();

        WriteApiEnumValueApiName(writer, apiEnumValue, ref context);
        WriteApiEnumValueClrName(writer, apiEnumValue, ref context);
        WriteApiEnumValueClrOrdinal(writer, apiEnumValue, ref context);

        writer.WriteEndObject();
    }

    private static void WriteApiEnumValueApiName
    (
        Utf8JsonWriter writer,
        ApiEnumValue apiEnumValue,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ApiName;
        var value = apiEnumValue.ApiName;
        writer.WriteString(propertyName, value);
    }

    private static void WriteApiEnumValueClrName
    (
        Utf8JsonWriter writer,
        ApiEnumValue apiEnumValue,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrName;
        var value = apiEnumValue.ClrName;
        writer.WriteString(propertyName, value);
    }

    private static void WriteApiEnumValueClrOrdinal
    (
        Utf8JsonWriter writer,
        ApiEnumValue apiEnumValue,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrOrdinal;
        var value = apiEnumValue.ClrOrdinal;
        writer.WriteNumber(propertyName, value);
    }

    // ApiNamedType
    private static void WriteApiNamedType
    (
        Utf8JsonWriter writer,
        ApiNamedType apiNamedType,
        ref WriteContext context
    )
    {
        WriteApiNamedTypeApiName(writer, apiNamedType, ref context);
    }

    private static void WriteApiNamedTypeApiName
    (
        Utf8JsonWriter writer,
        ApiNamedType apiNamedType,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiNamedType.ApiName;
        var value = apiNamedType.ApiName;
        writer.WriteString(propertyName, value);
    }

    // ApiObjectType
    private static void WriteApiObjectType
    (
        Utf8JsonWriter writer,
        ApiObjectType apiObjectType,
        ref WriteContext context
    )
    {
        WriteApiObjectTypeApiProperties(writer, apiObjectType, ref context);
    }

    private static void WriteApiObjectTypeApiProperties
    (
        Utf8JsonWriter writer,
        ApiObjectType apiObjectType,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiObjectType.ApiProperties;
        writer.WritePropertyName(propertyName);

        writer.WriteStartArray();

        var apiProperties = apiObjectType.ApiProperties;
        foreach (var apiProperty in apiProperties)
        {
            WriteApiProperty(writer, apiProperty, ref context);
        }

        writer.WriteEndArray();
    }

    // ApiProperty
    private static void WriteApiProperty
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        ref WriteContext context
    )
    {
        writer.WriteStartObject();

        WriteApiPropertyApiName(writer, apiProperty, ref context);
        WriteApiPropertyApiType(writer, apiProperty, ref context);
        WriteApiPropertyApiTypeModifiers(writer, apiProperty, ref context);
        WriteApiPropertyClrName(writer, apiProperty, ref context);

        writer.WriteEndObject();
    }

    private static void WriteApiPropertyApiName
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiName;
        var value = apiProperty.ApiName;
        writer.WriteString(propertyName, value);
    }

    private static void WriteApiPropertyApiType
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiType;
        writer.WritePropertyName(propertyName);

        var apiType = apiProperty.ApiType;
        var options = context.Options;
        JsonSerializer.Serialize(writer, apiType, options);
    }

    private static void WriteApiPropertyApiTypeModifiers
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiTypeModifiers;
        writer.WritePropertyName(propertyName);

        var apiTypeModifiers = apiProperty.ApiTypeModifiers;
        var options = context.Options;

        ApiTypeModifiersJsonConverter.Write(writer, apiTypeModifiers, options);
    }

    private static void WriteApiPropertyClrName
    (
        Utf8JsonWriter writer,
        ApiProperty apiProperty,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiProperty.ClrName;
        var value = apiProperty.ClrName;
        writer.WriteString(propertyName, value);
    }

    // ApiScalarType
    private static void WriteApiScalarType
    (
        Utf8JsonWriter writer,
        ApiScalarType apiScalarType,
        ref WriteContext context
    )
    {
        // Note: No concrete ApiScalarType properties to write => NOOP.
    }

    // ApiType
    private static void WriteApiTypeBody
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        ref WriteContext context
    )
    {
        var kind = apiType.Kind;
        switch (kind)
        {
            case ApiTypeKind.Collection:
                {
                    var apiCollectionType = (ApiCollectionType)apiType;
                    WriteApiCollectionType(writer, apiCollectionType, ref context);
                    break;
                }

            case ApiTypeKind.Enum:
                {
                    var apiEnumType = (ApiEnumType)apiType;
                    WriteApiNamedType(writer, apiEnumType, ref context);
                    WriteApiEnumType(writer, apiEnumType, ref context);
                    break;
                }

            case ApiTypeKind.Object:
                {
                    var apiObjectType = (ApiObjectType)apiType;
                    WriteApiNamedType(writer, apiObjectType, ref context);
                    WriteApiObjectType(writer, apiObjectType, ref context);
                    break;
                }

            case ApiTypeKind.Scalar:
                {
                    var apiScalarType = (ApiScalarType)apiType;
                    WriteApiNamedType(writer, apiScalarType, ref context);
                    WriteApiScalarType(writer, apiScalarType, ref context);
                    break;
                }
        }
    }

    private static void WriteApiTypeClrType
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiType.ClrType;
        writer.WritePropertyName(propertyName);

        var clrType = apiType.ClrType;
        var options = context.Options;
        TypeJsonConverter.Write(writer, clrType, options);
    }

    private static void WriteApiTypeEpilog
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        ref WriteContext context
    )
    {
        WriteApiTypeClrType(writer, apiType, ref context);
        WriteApiTypeExtensions(writer, apiType, ref context);

        writer.WriteEndObject();
    }

    private static void WriteApiTypeExtensions
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        ref WriteContext context
    )
    {
        var extensions = apiType.Extensions;
        if (extensions != null && !extensions.IsEmpty)
        {
            var extensionsPropertyName = context.PropertyNames.ApiType.Extensions;
            writer.WritePropertyName(extensionsPropertyName);

            writer.WriteStartObject();
            foreach (var (extensionType, extension) in extensions)
            {
                var extensionTypeName = TypeJsonConverter.GetSerializeTypeName(extensionType);
                var extensionPropertyName = context.GetPropertyName(extensionTypeName);

                writer.WritePropertyName(extensionPropertyName);

                var options = context.Options;
                JsonSerializer.Serialize(writer, extension, extensionType, options);
            }
            writer.WriteEndObject();
        }
    }

    private static void WriteApiTypeKind
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        ref WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiType.Kind;
        var value = apiType.Kind.ToString();

        writer.WriteString(propertyName, value);
    }

    private static void WriteApiTypeProlog
    (
        Utf8JsonWriter writer,
        ApiType apiType,
        ref WriteContext context
    )
    {
        writer.WriteStartObject();

        WriteApiTypeKind(writer, apiType, ref context);
    }
    #endregion
}