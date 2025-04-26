// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extension;
using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema;

public class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Types
    private class PropertyNamePolicy(JsonSerializerOptions options)
    {
        #region Properties
        public string ApiEnumValues => NamingPolicy?.ConvertName(nameof(ApiEnumType.ApiEnumValues)) ?? nameof(ApiEnumType.ApiEnumValues);
        public string ApiName => NamingPolicy?.ConvertName(nameof(ApiNamedType.ApiName)) ?? nameof(ApiNamedType.ApiName);
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

    private class ReadResult
    {
        #region Properties
        public List<ApiEnumValue>? ApiEnumValues { get; set; }
        public string? ApiName { get; set; }
        public Type? ClrType { get; set; }
        public Dictionary<string, object>? Extensions { get; set; }
        public string? Kind { get; set; }
        #endregion
    }
    #endregion

    #region Properties
    private static TypeJsonConverter TypeJsonConverter { get; } = new TypeJsonConverter();
    #endregion

    #region JsonConverter<T> Methods
    public override ApiType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var propertyNamePolicy = new PropertyNamePolicy(options);

        var readResult = Read(ref reader, typeToConvert, options, propertyNamePolicy);

        var apiType = Create(propertyNamePolicy, readResult);
        return apiType;
    }

    public override void Write(Utf8JsonWriter writer, ApiType apiType, JsonSerializerOptions options)
    {
        var propertyNamePolicy = new PropertyNamePolicy(options);

        writer.WriteStartObject();

        WriteApiTypeKind(writer, apiType, options, propertyNamePolicy);

        WriteApiNamedTypeApiName(writer, apiType, options, propertyNamePolicy);

        WriteApiEnumTypeApiEnumValues(writer, apiType, options, propertyNamePolicy);

        WriteApiTypeClrType(writer, apiType, options, propertyNamePolicy);
        WriteApiTypeExtensions(writer, apiType, options, propertyNamePolicy);

        writer.WriteEndObject();
    }
    #endregion

    #region Factory Implementation Methods
    private static void AttachExtensions(ReadResult readResult, ApiType apiType)
    {
        var extensions = readResult.Extensions;
        if (extensions != null)
        {
            foreach (var (extensionTypeName, extension) in extensions)
            {
                var extensionType = TypeJsonConverter.DeserializeTypeName(extensionTypeName);
                apiType.AttachExtension(extensionType, extension);
            }
        }
    }

    private static ApiEnumType CreateApiEnumType(ReadResult readResult, List<ValidationResult>? validationResults)
    {
        // TODO: Handle validation results now.

        var apiName = readResult.ApiName!;
        var apiEnumValues = readResult.ApiEnumValues!;
        var clrType = readResult.ClrType!;

        return new ApiEnumType(apiName, apiEnumValues, clrType);
    }

    private static ApiScalarType CreateApiScalarType(ReadResult readResult, List<ValidationResult>? validationResults)
    {
        // TODO: Handle validation results now.

        var apiName = readResult.ApiName!;
        var clrType = readResult.ClrType!;

        return new ApiScalarType(apiName, clrType);
    }
    #endregion

    #region Read Implementation Methods
    private static ApiType? Create(PropertyNamePolicy propertyNamePolicy, ReadResult readResult)
    {
        // Validate all required properties are non-null.
        var validationResults = default(List<ValidationResult>);
        ValidateApiTypeProperties(propertyNamePolicy, readResult, validationResults);

        var kind = readResult.Kind;

        // Create the appropriate derived ApiType based on the guidance of the Kind enumeration value.
        var apiType = default(ApiType);
        switch (kind)
        {
            case nameof(ApiTypeKind.Enum):
                {
                    ValidateApiNamedTypeProperties(propertyNamePolicy, readResult, validationResults);
                    ValidateApiEnumTypeProperties(propertyNamePolicy, readResult, validationResults);

                    apiType = CreateApiEnumType(readResult, validationResults);
                }
                break;

            case nameof(ApiTypeKind.Scalar):
                {
                    ValidateApiNamedTypeProperties(propertyNamePolicy, readResult, validationResults);
                    apiType = CreateApiScalarType(readResult, validationResults);
                }
                break;

            default:
                {
                    throw new JsonException($"Unsupported Kind: {kind}");
                }
        }

        AttachExtensions(readResult, apiType);

        return apiType;
    }

    private static ReadResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, PropertyNamePolicy propertyNamePolicy)
    {
        var readResult = new ReadResult();

        // Read the JSON object properties
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object.");

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name.");

            // Read ApiType and derived ApiType properties
            var propertyName = reader.GetString()!;
            reader.Read(); // Move to the value

            // .. Kind [ApiType]
            if (string.Equals(propertyName, propertyNamePolicy.Kind, StringComparison.OrdinalIgnoreCase))
                readResult.Kind = reader.GetString();

            // .. ClrType [ApiType]
            else if (string.Equals(propertyName, propertyNamePolicy.ClrType, StringComparison.OrdinalIgnoreCase))
                readResult.ClrType = TypeJsonConverter.Read(ref reader, typeof(Type), options);

            // .. ApiName [ApiNamedType]
            else if (string.Equals(propertyName, propertyNamePolicy.ApiName, StringComparison.OrdinalIgnoreCase))
                readResult.ApiName = reader.GetString();

            // .. ApiEnumValues [ApiEnumType]
            else if (string.Equals(propertyName, propertyNamePolicy.ApiEnumValues, StringComparison.OrdinalIgnoreCase))
                readResult.ApiEnumValues = JsonSerializer.Deserialize<List<ApiEnumValue>>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {propertyNamePolicy.ApiEnumValues}.");

            // .. Extensions [ExtensibleBase]
            else if (string.Equals(propertyName, propertyNamePolicy.Extensions, StringComparison.OrdinalIgnoreCase))
            {
                // Deserialize Extensions as a dictionary of type names to values
                readResult.Extensions = JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {propertyNamePolicy.Extensions}.");
            }

            // .. Skip unknown properties
            else
                reader.Skip();
        }

        return readResult;
    }
    #endregion

    #region Validation Implementation Methods
    private static void ValidateApiEnumTypeProperties
    (
        PropertyNamePolicy propertyNamePolicy,
        ReadResult readResult,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiEnumTypeProperties
        (
            propertyNamePolicy.ApiEnumValues, readResult.ApiEnumValues,
            validationResults
        );
    }

    private static void ValidateApiEnumTypeProperties
    (
        string apiEnumValuesPropertyName, [NotNullWhen(true)] IEnumerable<ApiEnumValue>? apiEnumValues,
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
        PropertyNamePolicy propertyNamePolicy,
        ReadResult readResult,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiNamedTypeProperties
        (
            propertyNamePolicy.ApiName, readResult.ApiName,
            validationResults
        );
    }

    private static void ValidateApiNamedTypeProperties
    (
        string apiNamePropertyName, [NotNullWhen(true)] string? apiName,
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
        PropertyNamePolicy propertyNamePolicy,
        ReadResult readResult,
        List<ValidationResult>? validationResults
    )
    {
        ValidateApiTypeProperties
        (
            propertyNamePolicy.Kind, readResult.Kind,
            propertyNamePolicy.ClrType, readResult.ClrType,
            validationResults
        );
    }

    private static void ValidateApiTypeProperties
    (
        string kindPropertyName, [NotNullWhen(true)] string? kind,
        string clrTypePropertyName, [NotNullWhen(true)] Type? clrType,
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
    private static void WriteApiEnumTypeApiEnumValues(
        Utf8JsonWriter writer,
        ApiType apiType,
        JsonSerializerOptions options,
        PropertyNamePolicy propertyNamePolicy)
    {
        var apiEnumType = apiType as ApiEnumType;
        if (apiEnumType != null)
        {
            var values = apiEnumType.ApiEnumValues;
            if (values != null && values.Any())
            {
                var propertyName = propertyNamePolicy.ApiEnumValues;
                writer.WritePropertyName(propertyName);

                writer.WriteStartArray();
                foreach (var value in values)
                {
                    JsonSerializer.Serialize(writer, value, typeof(ApiEnumValue), options);
                }
                writer.WriteEndArray();
            }
        }
    }

    private static void WriteApiNamedTypeApiName(
        Utf8JsonWriter writer,
        ApiType apiType,
        JsonSerializerOptions options,
        PropertyNamePolicy propertyNamePolicy)
    {
        var apiNamedType = apiType as ApiNamedType;
        if (apiNamedType != null)
        {
            var propertyName = propertyNamePolicy.ApiName;
            var value = apiNamedType.ApiName;
            writer.WriteString(propertyName, value);
        }
    }

    private static void WriteApiTypeClrType(
        Utf8JsonWriter writer,
        ApiType apiType,
        JsonSerializerOptions options,
        PropertyNamePolicy propertyNamePolicy)
    {
        var propertyName = propertyNamePolicy.ClrType;
        writer.WritePropertyName(propertyName);

        var type = apiType.ClrType;
        TypeJsonConverter.Write(writer, type, options);
    }

    private static void WriteApiTypeExtensions(
        Utf8JsonWriter writer,
        ApiType apiType,
        JsonSerializerOptions options,
        PropertyNamePolicy propertyNamePolicy)
    {
        var extensions = apiType.Extensions;
        if (extensions != null && extensions.Count > 0)
        {
            var extensionsPropertyName = propertyNamePolicy.Extensions;
            writer.WritePropertyName(extensionsPropertyName);

            writer.WriteStartObject();
            foreach (var (extensionType, extension) in extensions)
            {
                var extensionTypeName = TypeJsonConverter.SerializeTypeName(extensionType);
                var extensionPropertyName = propertyNamePolicy.GetPropertyName(extensionTypeName);

                writer.WritePropertyName(extensionPropertyName);

                JsonSerializer.Serialize(writer, extension, extensionType, options);
            }
            writer.WriteEndObject();
        }
    }

    private static void WriteApiTypeKind(
        Utf8JsonWriter writer,
        ApiType apiType,
        JsonSerializerOptions options,
        PropertyNamePolicy propertyNamePolicy)
    {
        var propertyName = propertyNamePolicy.Kind;
        var value = apiType.Kind.ToString();

        writer.WriteString(propertyName, value);
    }
    #endregion
}
