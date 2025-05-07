// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extension;
using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema;

public class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Types
    private delegate void PropertyHandler(ref Utf8JsonReader reader, ref DeserializationContext context);

    private class DeserializationContext
    (
        Type typeToConvert,
        JsonSerializerOptions options,
        PropertyNamePolicy propertyNamePolicy,
        Dictionary<string, PropertyHandler> propertyHandlers)
    {
        #region Immutable Properties
        public Type TypeToConvert { get; } = typeToConvert;
        public JsonSerializerOptions Options { get; } = options;
        public PropertyNamePolicy PropertyNamePolicy { get; } = propertyNamePolicy;
        public Dictionary<string, PropertyHandler> PropertyHandlers { get; } = propertyHandlers;
        #endregion

        #region Mutable Properties
        public List<ApiEnumValue>? ApiEnumValues { get; set; }
        public ApiType? ApiItemType { get; set; }
        public ApiTypeModifiers? ApiItemTypeModifiers { get; set; }
        public string? ApiName { get; set; }
        public Type? ClrType { get; set; }
        public Dictionary<string, object>? Extensions { get; set; }
        public string? Kind { get; set; }
        #endregion
    }

    private class PropertyNamePolicy(JsonSerializerOptions options)
    {
        #region Properties
        public string ApiEnumValues => NamingPolicy?.ConvertName(nameof(ApiEnumType.ApiEnumValues)) ?? nameof(ApiEnumType.ApiEnumValues);
        public string ApiName => NamingPolicy?.ConvertName(nameof(ApiNamedType.ApiName)) ?? nameof(ApiNamedType.ApiName);
        public string ApiItemType => NamingPolicy?.ConvertName(nameof(ApiCollectionType.ApiItemType)) ?? nameof(ApiCollectionType.ApiItemType);
        public string ApiItemTypeModifiers => NamingPolicy?.ConvertName(nameof(ApiCollectionType.ApiItemTypeModifiers)) ?? nameof(ApiCollectionType.ApiItemTypeModifiers);
        public string ClrType => NamingPolicy?.ConvertName(nameof(ApiType.ClrType)) ?? nameof(ApiType.ClrType);
        public string Extensions => NamingPolicy?.ConvertName(nameof(ExtensibleBase.Extensions)) ?? nameof(ExtensibleBase.Extensions);
        public string Kind => NamingPolicy?.ConvertName(nameof(ApiType.Kind)) ?? nameof(ApiType.Kind);

        private JsonNamingPolicy? NamingPolicy { get; } = options.PropertyNamingPolicy;
        #endregion

        #region Methods
        public string GetPropertyName(string name)
        {
            return NamingPolicy?.ConvertName(name) ?? nameof(name);
        }
        #endregion
    }

    private class SerializationContext(JsonSerializerOptions options)
    {
        #region Immutable Properties
        public JsonSerializerOptions Options { get; } = options;
        public PropertyNamePolicy PropertyNamePolicy { get; } = new PropertyNamePolicy(options);
        #endregion

        #region Mutable Properties
        #endregion
    }
    #endregion

    #region Properties
    private static TypeJsonConverter TypeJsonConverter { get; } = new TypeJsonConverter();
    private static EnumJsonConverter<ApiTypeModifiers> ApiTypeModifiersJsonConverter { get; } = new EnumJsonConverter<ApiTypeModifiers>();
    #endregion

    #region JsonConverter<T> Methods
    public override ApiType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var propertyNamePolicy = new PropertyNamePolicy(options);
        var propertyHandlers = CreatePropertyHandlers(propertyNamePolicy);
        var context = new DeserializationContext(typeToConvert, options, propertyNamePolicy, propertyHandlers);

        Deserialize(ref reader, ref context);

        var apiType = CreateApiType(context);
        return apiType;
    }

    public override void Write(Utf8JsonWriter writer, ApiType apiType, JsonSerializerOptions options)
    {
        var context = new SerializationContext(options);

        WriteApiTypeProlog(writer, apiType, ref context);
        WriteApiTypeBody(writer, apiType, ref context);
        WriteApiTypeEpilog(writer, apiType, ref context);
    }
    #endregion

    #region Factory Implementation Methods
    private static void AttachExtensions(DeserializationContext context, ApiType apiType)
    {
        var extensions = context.Extensions;
        if (extensions != null)
        {
            foreach (var (extensionTypeName, extension) in extensions)
            {
                var extensionType = TypeJsonConverter.GetDeserializeType(extensionTypeName);
                apiType.AttachExtension(extensionType, extension);
            }
        }
    }

    private static ApiCollectionType CreateApiCollectionType(DeserializationContext context, List<ValidationResult>? validationResults)
    {
        HandleValidationResults(validationResults);

        var apiItemType = context.ApiItemType!;
        var apiItemTypeModifiers = context.ApiItemTypeModifiers!.Value;
        var clrCollectionType = context.ClrType!;

        return new ApiCollectionType(apiItemType, apiItemTypeModifiers, clrCollectionType);
    }

    private static ApiEnumType CreateApiEnumType(DeserializationContext context, List<ValidationResult>? validationResults)
    {
        HandleValidationResults(validationResults);

        var apiName = context.ApiName!;
        var apiEnumValues = context.ApiEnumValues!;
        var clrType = context.ClrType!;

        return new ApiEnumType(apiName, apiEnumValues, clrType);
    }

    private static ApiScalarType CreateApiScalarType(DeserializationContext context, List<ValidationResult>? validationResults)
    {
        HandleValidationResults(validationResults);

        var apiName = context.ApiName!;
        var clrType = context.ClrType!;

        return new ApiScalarType(apiName, clrType);
    }

    private static void HandleValidationResults(List<ValidationResult>? validationResults)
    {
        if (validationResults == null)
            return;

        if (validationResults.Count == 0)
            return;

        // TODO: Need to think through a strategy on how to capture all validation errors and return it as an exception.
    }
    #endregion

    #region Read Implementation Methods
    private static ApiType? CreateApiType(DeserializationContext context)
    {
        // Validate all required properties are non-null.
        var validationResults = default(List<ValidationResult>);
        ValidateApiTypeProperties(context, validationResults);

        var kind = context.Kind;

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

    private static Dictionary<string, PropertyHandler> CreatePropertyHandlers(PropertyNamePolicy propertyNamePolicy)
    {
        // Initialize the dictionary with case-insensitive comparison
        var handlers = new Dictionary<string, PropertyHandler>(StringComparer.OrdinalIgnoreCase)
        {
            // ApiType properties
            {
                propertyNamePolicy.Kind,
                (ref Utf8JsonReader reader, ref DeserializationContext context) =>
                {
                    context.Kind = reader.GetString();
                }
            },
            {
                propertyNamePolicy.ClrType,
                (ref Utf8JsonReader reader, ref DeserializationContext context) =>
                {
                    context.ClrType = TypeJsonConverter.Read(ref reader, typeof(Type), context.Options);
                }
            },
            // ApiNamedType properties
            {
                propertyNamePolicy.ApiName,
                (ref Utf8JsonReader reader, ref DeserializationContext context) =>
                {
                    context.ApiName = reader.GetString();
                }
            },
            // ApiCollectionType properties
            {
                propertyNamePolicy.ApiItemType,
                (ref Utf8JsonReader reader, ref DeserializationContext context) =>
                {
                    context.ApiItemType = JsonSerializer.Deserialize<ApiType>(ref reader, context.Options)
                        ?? throw new JsonException($"Failed to deserialize {context.PropertyNamePolicy.ApiItemType}.");
                }
            },
            {
                propertyNamePolicy.ApiItemTypeModifiers,
                (ref Utf8JsonReader reader, ref DeserializationContext context) =>
                {
                    context.ApiItemTypeModifiers = ApiTypeModifiersJsonConverter.Read(ref reader, typeof(ApiTypeModifiers), context.Options);
                }
            },
            // ApiEnumType properties
            {
                propertyNamePolicy.ApiEnumValues,
                (ref Utf8JsonReader reader, ref DeserializationContext context) =>
                {
                    context.ApiEnumValues = JsonSerializer.Deserialize<List<ApiEnumValue>>(ref reader, context.Options)
                        ?? throw new JsonException($"Failed to deserialize {context.PropertyNamePolicy.ApiEnumValues}.");
                }
            },
            // ExtensibleBase properties
            {
                propertyNamePolicy.Extensions,
                (ref Utf8JsonReader reader, ref DeserializationContext context) =>
                {
                    context.Extensions = JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, context.Options)
                        ?? throw new JsonException($"Failed to deserialize {context.PropertyNamePolicy.Extensions}.");
                }
            }
        };

        return handlers;
    }

    private static void Deserialize(ref Utf8JsonReader reader, ref DeserializationContext context)
    {
        // Read the JSON object properties
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object.");

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name.");

            // Read the property name and advance to the property value.
            var propertyName = reader.GetString()!;
            reader.Read(); // Move to the value

            // Look up the handler for the property name
            if (context.PropertyHandlers.TryGetValue(propertyName, out var handler))
            {
                // Execute the handler to deserialize the property
                handler(ref reader, ref context);
            }
            else
            {
                // Skip unknown properties
                reader.Skip();
            }
        }
    }
    #endregion

    #region Validation Implementation Methods
    private static void ValidateApiCollectionTypeProperties
    (
        DeserializationContext context,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiCollectionTypeProperties
        (
            context.PropertyNamePolicy.ApiItemType, context.ApiItemType,
            context.PropertyNamePolicy.ApiItemTypeModifiers, context.ApiItemTypeModifiers,
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
        DeserializationContext context,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiEnumTypeProperties
        (
            context.PropertyNamePolicy.ApiEnumValues, context.ApiEnumValues,
            validationResults
        );
    }

    private static void ValidateApiEnumTypeProperties
    (
        string apiEnumValuesPropertyName, IEnumerable<ApiEnumValue>? apiEnumValues,
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
        }
    }

    private static void ValidateApiNamedTypeProperties
    (
        DeserializationContext context,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiNamedTypeProperties
        (
            context.PropertyNamePolicy.ApiName, context.ApiName,
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
        DeserializationContext context,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiTypeProperties
        (
            context.PropertyNamePolicy.Kind, context.Kind,
            context.PropertyNamePolicy.ClrType, context.ClrType,
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
    private static void WriteApiCollectionType(
        Utf8JsonWriter writer,
        ApiCollectionType apiCollectionType,
        ref SerializationContext context)
    {
        // Write concrete properties.
        WriteApiCollectionTypeApiItemType(writer, apiCollectionType, ref context);
        WriteApiCollectionTypeApiItemTypeModifiers(writer, apiCollectionType, ref context);
    }

    private static void WriteApiCollectionTypeApiItemType(
        Utf8JsonWriter writer,
        ApiCollectionType apiCollectionType,
        ref SerializationContext context)
    {
        var propertyName = context.PropertyNamePolicy.ApiItemType;
        writer.WritePropertyName(propertyName);

        var apiItemType = apiCollectionType.ApiItemType;
        var options = context.Options;
        JsonSerializer.Serialize(writer, apiItemType, options);
    }

    private static void WriteApiCollectionTypeApiItemTypeModifiers(
        Utf8JsonWriter writer,
        ApiCollectionType apiCollectionType,
        ref SerializationContext context)
    {
        var propertyName = context.PropertyNamePolicy.ApiItemTypeModifiers;
        writer.WritePropertyName(propertyName);

        var apiItemTypeModifiers = apiCollectionType.ApiItemTypeModifiers;
        var options = context.Options;

        ApiTypeModifiersJsonConverter.Write(writer, apiItemTypeModifiers, options);
    }

    // ApiEnumType
    private static void WriteApiEnumType(
        Utf8JsonWriter writer,
        ApiEnumType apiEnumType,
        ref SerializationContext context)
    {
        // Write concrete properties.
        WriteApiEnumTypeApiEnumValues(writer, apiEnumType, ref context);
    }

    private static void WriteApiEnumTypeApiEnumValues(
        Utf8JsonWriter writer,
        ApiEnumType apiEnumType,
        ref SerializationContext context)
    {
        var propertyName = context.PropertyNamePolicy.ApiEnumValues;
        writer.WritePropertyName(propertyName);

        writer.WriteStartArray();

        var values = apiEnumType.ApiEnumValues;
        foreach (var value in values)
        {
            var options = context.Options;
            JsonSerializer.Serialize(writer, value, options);
        }

        writer.WriteEndArray();
    }

    // ApiNamedType
    private static void WriteApiNamedType(
        Utf8JsonWriter writer,
        ApiNamedType apiNamedType,
        ref SerializationContext context)
    {
        // Write concrete properties.
        WriteApiNamedTypeApiName(writer, apiNamedType, ref context);
    }

    private static void WriteApiNamedTypeApiName(
        Utf8JsonWriter writer,
        ApiNamedType apiNamedType,
        ref SerializationContext context)
    {
        var propertyName = context.PropertyNamePolicy.ApiName;
        var value = apiNamedType.ApiName;
        writer.WriteString(propertyName, value);
    }

    // ApiObjectType
    private static void WriteApiObjectType(
        Utf8JsonWriter writer,
        ApiObjectType apiObjectType,
        ref SerializationContext context)
    {
        // Write concrete properties.
    }

    // ApiScalarType
    private static void WriteApiScalarType(
        Utf8JsonWriter writer,
        ApiScalarType apiScalarType,
        ref SerializationContext context)
    {
        // Write concrete properties.
        // -- Note: No concrete ApiScalarType properties to write => NOOP.
    }

    // ApiType
    private static void WriteApiTypeBody(
        Utf8JsonWriter writer,
        ApiType apiType,
        ref SerializationContext context)
    {
        // Write Derived ApiType JSON Properties
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

    private static void WriteApiTypeClrType(
        Utf8JsonWriter writer,
        ApiType apiType,
        ref SerializationContext context)
    {
        var propertyName = context.PropertyNamePolicy.ClrType;
        writer.WritePropertyName(propertyName);

        var clrType = apiType.ClrType;
        var options = context.Options;
        TypeJsonConverter.Write(writer, clrType, options);
    }

    private static void WriteApiTypeEpilog(
        Utf8JsonWriter writer,
        ApiType apiType,
        ref SerializationContext context)
    {
        // Write ApiType Epilog JSON Properties
        WriteApiTypeClrType(writer, apiType, ref context);
        WriteApiTypeExtensions(writer, apiType, ref context);

        // Write JSON End of Object
        writer.WriteEndObject();
    }

    private static void WriteApiTypeExtensions(
        Utf8JsonWriter writer,
        ApiType apiType,
        ref SerializationContext context)
    {
        var extensions = apiType.Extensions;
        if (extensions != null && extensions.Count > 0)
        {
            var extensionsPropertyName = context.PropertyNamePolicy.Extensions;
            writer.WritePropertyName(extensionsPropertyName);

            writer.WriteStartObject();
            foreach (var (extensionType, extension) in extensions)
            {
                var extensionTypeName = TypeJsonConverter.GetSerializeTypeName(extensionType);
                var extensionPropertyName = context.PropertyNamePolicy.GetPropertyName(extensionTypeName);

                writer.WritePropertyName(extensionPropertyName);

                var options = context.Options;
                JsonSerializer.Serialize(writer, extension, extensionType, options);
            }
            writer.WriteEndObject();
        }
    }

    private static void WriteApiTypeKind(
        Utf8JsonWriter writer,
        ApiType apiType,
        ref SerializationContext context)
    {
        var propertyName = context.PropertyNamePolicy.Kind;
        var value = apiType.Kind.ToString();

        writer.WriteString(propertyName, value);
    }

    private static void WriteApiTypeProlog(
        Utf8JsonWriter writer,
        ApiType apiType,
        ref SerializationContext context)
    {
        // Write JSON Start of Object
        writer.WriteStartObject();

        // Write ApiType Prolog JSON Properties
        WriteApiTypeKind(writer, apiType, ref context);
    }
    #endregion
}
