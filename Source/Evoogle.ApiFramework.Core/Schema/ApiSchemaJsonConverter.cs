// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Json;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     JSON converter for <see cref="ApiSchema"/> which reads and writes schema objects.
///     Follows the same patterns used by <see cref="ApiTypeJsonConverter"/>.
/// </summary>
public class ApiSchemaJsonConverter : JsonConverter<ApiSchema>
{
    #region Types
    private readonly record struct PropertyNames
    {
        public required string Name { get; init; }
        public required string ApiScalarTypes { get; init; }
        public required string ApiEnumTypes { get; init; }
        public required string ApiObjectTypes { get; init; }
        public required string Extensions { get; init; }
    }
    #endregion

    #region Fields
    private readonly ILogger<ApiSchemaJsonConverter>? _logger;
    private static readonly TypeJsonConverter TypeJsonConverter = new();
    private static readonly NullJsonNamingPolicy NullJsonNamingPolicy = new();

    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> PropertyNamesCache = new();
    #endregion

    #region Constructors
    /// <summary>
    ///     Parameterless constructor for attribute usage.
    /// </summary>
    public ApiSchemaJsonConverter()
        : this(null)
    {
    }

    /// <summary>
    ///     Constructor allowing injection of a logger.
    /// </summary>
    public ApiSchemaJsonConverter(ILogger<ApiSchemaJsonConverter>? logger)
    {
        _logger = new MultiplexingLogger<ApiSchemaJsonConverter>(logger, MultiplexingLoggerMode.All);
    }
    #endregion

    #region JsonConverter
    /// <inheritdoc />
    public override ApiSchema? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var policy = options.PropertyNamingPolicy ?? NullJsonNamingPolicy;
        var names = GetPropertyNames(policy);

        string? name = null;
        List<ApiScalarType>? scalarTypes = null;
        List<ApiEnumType>? enumTypes = null;
        List<ApiObjectType>? objectTypes = null;
        Dictionary<string, object>? extensions = null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of an object.");

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected object property name.");

            var propertyName = reader.GetString()!;
            reader.Read();

            if (propertyName == names.Name)
            {
                name = reader.GetString();
            }
            else if (propertyName == names.ApiScalarTypes)
            {
                scalarTypes = JsonSerializer.Deserialize<List<ApiScalarType>>(ref reader, options)
                    ?? throw new JsonException($"Failed to deserialize {names.ApiScalarTypes}.");
            }
            else if (propertyName == names.ApiEnumTypes)
            {
                enumTypes = JsonSerializer.Deserialize<List<ApiEnumType>>(ref reader, options)
                    ?? throw new JsonException($"Failed to deserialize {names.ApiEnumTypes}.");
            }
            else if (propertyName == names.ApiObjectTypes)
            {
                objectTypes = JsonSerializer.Deserialize<List<ApiObjectType>>(ref reader, options)
                    ?? throw new JsonException($"Failed to deserialize {names.ApiObjectTypes}.");
            }
            else if (propertyName == names.Extensions)
            {
                extensions = ReadExtensions(ref reader, options);
            }
            else
            {
                _logger?.LogWarning("Skipping unknown JSON property: '{Property}'", propertyName);
                reader.Skip();
            }
        }

        name ??= string.Empty;

        var apiTypes = new List<ApiType>();
        if (scalarTypes != null) apiTypes.AddRange(scalarTypes);
        if (enumTypes != null) apiTypes.AddRange(enumTypes);
        if (objectTypes != null) apiTypes.AddRange(objectTypes);

        var schema = new ApiSchema(name, apiTypes);

        if (extensions != null)
        {
            foreach (var (typeName, extension) in extensions)
            {
                var extensionType = TypeJsonConverter.GetDeserializeType(typeName);
                schema.AttachExtension(extensionType, extension);
            }
        }

        return schema;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ApiSchema value, JsonSerializerOptions options)
    {
        var policy = options.PropertyNamingPolicy ?? NullJsonNamingPolicy;
        var names = GetPropertyNames(policy);

        writer.WriteStartObject();

        writer.WriteString(names.Name, value.Name);

        writer.WritePropertyName(names.ApiScalarTypes);
        writer.WriteStartArray();
        foreach (var apiType in value.ApiScalarTypes)
        {
            JsonSerializer.Serialize(writer, apiType, options);
        }
        writer.WriteEndArray();

        writer.WritePropertyName(names.ApiEnumTypes);
        writer.WriteStartArray();
        foreach (var apiType in value.ApiEnumerationTypes)
        {
            JsonSerializer.Serialize(writer, apiType, options);
        }
        writer.WriteEndArray();

        writer.WritePropertyName(names.ApiObjectTypes);
        writer.WriteStartArray();
        foreach (var apiType in value.ApiObjectTypes)
        {
            JsonSerializer.Serialize(writer, apiType, options);
        }
        writer.WriteEndArray();

        if (value.Extensions != null)
        {
            writer.WritePropertyName(names.Extensions);
            writer.WriteStartObject();
            foreach (var (extensionType, extension) in value.Extensions)
            {
                var name = TypeJsonConverter.GetSerializeTypeName(extensionType);
                writer.WritePropertyName(name);
                JsonSerializer.Serialize(writer, extension, extensionType, options);
            }
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
    #endregion

    #region Implementation
    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return PropertyNamesCache.GetOrAdd(policy, p => new PropertyNames
        {
            Name = p.ConvertName(nameof(ApiSchema.Name)),
            ApiScalarTypes = p.ConvertName(nameof(ApiSchema.ApiScalarTypes)),
            ApiEnumTypes = p.ConvertName(nameof(ApiSchema.ApiEnumerationTypes)),
            ApiObjectTypes = p.ConvertName(nameof(ApiSchema.ApiObjectTypes)),
            Extensions = p.ConvertName(nameof(ApiSchema.Extensions))
        });
    }

    private static Dictionary<string, object> ReadExtensions(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of an object.");

        var result = new Dictionary<string, object>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected object property name.");

            var typeName = reader.GetString()!;
            var extensionType = TypeJsonConverter.GetDeserializeType(typeName);
            reader.Read();
            var extension = JsonSerializer.Deserialize(ref reader, extensionType, options)
                ?? throw new JsonException($"Failed to deserialize extension '{typeName}'.");
            result.Add(typeName, extension);
        }

        return result;
    }
    #endregion
}
