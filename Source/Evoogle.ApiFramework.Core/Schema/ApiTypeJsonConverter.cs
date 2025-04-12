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
    #region Fields
    private const string ExtensionsPropertyName = nameof(ExtensibleBase.Extensions);

    private const string KindPropertyName = nameof(ApiType.Kind);
    private const string ClrTypePropertyName = nameof(ApiType.ClrType);

    private const string ApiNamePropertyName = nameof(ApiNamedType.ApiName);
    #endregion

    #region Properties
    private static TypeJsonConverter TypeJsonConverter { get; } = new TypeJsonConverter();
    #endregion

    #region JsonConverter<T> Methods
    public override ApiType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object.");

        var kind = default(string?);
        var clrType = default(Type?);
        var apiName = default(string?);
        var extensions = default(Dictionary<string, object>?);

        // Get the naming policy from options (defaults to null, meaning PascalCase)
        var namingPolicy = options.PropertyNamingPolicy;

        // Convert property names to the expected case based on the naming policy
        var kindKey = namingPolicy?.ConvertName(KindPropertyName) ?? KindPropertyName;
        var apiNameKey = namingPolicy?.ConvertName(ApiNamePropertyName) ?? ApiNamePropertyName;
        var clrTypeKey = namingPolicy?.ConvertName(ClrTypePropertyName) ?? ClrTypePropertyName;
        var extensionsKey = namingPolicy?.ConvertName(ExtensionsPropertyName) ?? ExtensionsPropertyName;

        // Read the JSON object properties
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
            if (string.Equals(propertyName, kindKey, StringComparison.OrdinalIgnoreCase))
                kind = reader.GetString();

            // .. ClrType [ApiType]
            else if (string.Equals(propertyName, clrTypeKey, StringComparison.OrdinalIgnoreCase))
                clrType = TypeJsonConverter.Read(ref reader, typeof(Type), options);

            // .. ApiName [ApiNamedType]
            else if (string.Equals(propertyName, apiNameKey, StringComparison.OrdinalIgnoreCase))
                apiName = reader.GetString();

            // .. Extensions [ExtensibleBase]
            else if (string.Equals(propertyName, extensionsKey, StringComparison.OrdinalIgnoreCase))
            {
                // Deserialize Extensions as a dictionary of type names to values
                extensions = JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options) ?? throw new JsonException("Failed to deserialize Extensions.");
            }

            // .. Skip unknown properties
            else
                reader.Skip();
        }

        // Validate all required properties are non-null.
        var validationResults = default(List<ValidationResult>);

        if (ValidateApiTypeProperties(
            kindKey, kind,
            clrTypeKey, clrType,
            validationResults) == false)
        {
            throw new JsonException();
        }

        // Create the appropriate derived ApiType based on the guidance of the Kind enumeration value.
        var apiType = default(ApiType);
        switch (kind)
        {
            case nameof(ApiTypeKind.Scalar):
                {
                    if (ValidateApiNamedTypeProperties(
                        apiNameKey,
                        apiName,
                        validationResults) == false)
                    {
                        throw new JsonException();
                    }

                    apiType = CreateApiScalarType(apiName, clrType);
                }
                break;

            default:
                {
                    throw new JsonException($"Unsupported Kind: {kind}");
                }
        }

        // Populate Extensions if present
        if (extensions != null)
        {
            foreach (var (extensionTypeName, extension) in extensions)
            {
                var extensionType = TypeJsonConverter.DeserializeTypeName(extensionTypeName);
                apiType.AttachExtension(extensionType, extension);
            }
        }

        return apiType;
    }

    public override void Write(Utf8JsonWriter writer, ApiType apiType, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // Get the naming policy from options (defaults to null, meaning PascalCase)
        var namingPolicy = options.PropertyNamingPolicy;

        // Write ApiType and derived ApiType properties

        // .. Kind [ApiType]
        var kindKey = namingPolicy?.ConvertName(KindPropertyName) ?? KindPropertyName;
        writer.WriteString(kindKey, apiType.Kind.ToString());

        // .. ApiName [ApiNamedType]
        var apiNamedType = apiType as ApiNamedType;
        if (apiNamedType != null)
        {
            var apiNameKey = namingPolicy?.ConvertName(ApiNamePropertyName) ?? ApiNamePropertyName;
            writer.WriteString(apiNameKey, apiNamedType.ApiName);
        }

        // .. ClrType [ApiType]
        var clrTypeKey = namingPolicy?.ConvertName(ClrTypePropertyName) ?? ClrTypePropertyName;
        writer.WritePropertyName(clrTypeKey);
        TypeJsonConverter.Write(writer, apiType.ClrType, options);

        // .. Extensions [ExtensibleBase]
        var extensions = apiType.Extensions;
        if (extensions != null && extensions.Count > 0)
        {
            var extensionsKey = namingPolicy?.ConvertName(ExtensionsPropertyName) ?? ExtensionsPropertyName;
            writer.WritePropertyName(extensionsKey);
            writer.WriteStartObject();
            foreach (var (extensionType, extension) in extensions)
            {
                var extensionTypeName = TypeJsonConverter.SerializeTypeName(extensionType);
                var extensionKey = namingPolicy?.ConvertName(extensionTypeName) ?? extensionTypeName;

                writer.WritePropertyName(extensionKey);
                JsonSerializer.Serialize(writer, extension, extensionType, options);
            }
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
    #endregion

    #region Implementation Methods
    private static ApiScalarType CreateApiScalarType(string apiName, Type clrType)
    {
        return new ApiScalarType(apiName, clrType);
    }

    private static bool ValidateApiTypeProperties(
        string kindKey, [NotNullWhen(true)] string? kind,
        string clrTypeKey, [NotNullWhen(true)] Type? clrType,
        List<ValidationResult>? validationResultCollection)
    {
        var result = true;

        if (kind == null)
        {
            result = false;

            validationResultCollection ??= [];
            validationResultCollection.Add(new ValidationResult($"Missing required property: {kindKey}"));
        }

        if (clrType == null)
        {
            result = false;

            validationResultCollection ??= [];
            validationResultCollection.Add(new ValidationResult($"Missing required property: {clrTypeKey}"));
        }

        return result;
    }

    private static bool ValidateApiNamedTypeProperties(
        string apiNameKey, [NotNullWhen(true)]string? apiName,
        List<ValidationResult>? validationResultCollection)
    {
        var result = true;

        if (apiName == null)
        {
            result = false;

            validationResultCollection ??= [];
            validationResultCollection.Add(new ValidationResult($"Missing required property: {apiNameKey}"));
        }

        return result;
    }
    #endregion
}
