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
        #region Immutable Properties
        public required string Name { get; init; }
        public required string ApiScalarTypes { get; init; }
        public required string ApiEnumTypes { get; init; }
        public required string ApiObjectTypes { get; init; }
        public required string Extensions { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    #endregion

    #region Fields
    private readonly ILogger<ApiSchemaJsonConverter>? _logger;

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> PropertyNamesCache = new();

    private static readonly NullJsonNamingPolicy NullJsonNamingPolicy = new();
    #endregion

    #region Constructors
    /// <summary>
    ///     Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.
    /// </summary>
    public ApiSchemaJsonConverter()
        : this(null)
    {
    }

    /// <summary>
    ///     Optional constructor with logger for use in DI contexts.
    /// </summary>
    /// <param name="logger">The optional logger instance.</param>
    public ApiSchemaJsonConverter(ILogger<ApiSchemaJsonConverter>? logger)
    {
        _logger = new MultiplexingLogger<ApiSchemaJsonConverter>(logger, MultiplexingLoggerMode.All);
    }
    #endregion

    #region JsonConverter
    /// <summary>
    ///     Reads a JSON representation of an <see cref="ApiSchema"/> into a CLR object.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The target type (expected to be <see cref="ApiSchema"/>).</param>
    /// <param name="options">The serializer options in effect.</param>
    /// <returns>An <see cref="ApiSchema"/> instance.</returns>
    public override ApiSchema? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);

        var name = default(string);
        var scalarTypes = default(List<ApiScalarType>);
        var enumTypes = default(List<ApiEnumType>);
        var objectTypes = default(List<ApiObjectType>);
        var extensions = default(Dictionary<string, object>);

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

            if (propertyName == propertyNames.Name)
            {
                name = ReadName(ref reader, options, propertyName);
            }
            else if (propertyName == propertyNames.ApiScalarTypes)
            {
                scalarTypes = ReadApiScalarTypes(ref reader, options, propertyName);
            }
            else if (propertyName == propertyNames.ApiEnumTypes)
            {
                enumTypes = ReadApiEnumTypes(ref reader, options, propertyName);
            }
            else if (propertyName == propertyNames.ApiObjectTypes)
            {
                objectTypes = ReadApiObjectTypes(ref reader, options, propertyName);
            }
            else if (propertyName == propertyNames.Extensions)
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

    /// <summary>
    ///     Writes an <see cref="ApiSchema"/> instance into its JSON representation.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="apiSchema">The object to write.</param>
    /// <param name="options">The serializer options in effect.</param>
    public override void Write(Utf8JsonWriter writer, ApiSchema apiSchema, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);

        writer.WriteStartObject();

        writer.WriteString(propertyNames.Name, apiSchema.Name);

        writer.WritePropertyName(propertyNames.ApiScalarTypes);
        writer.WriteStartArray();
        foreach (var apiType in apiSchema.ApiScalarTypes)
        {
            JsonSerializer.Serialize<ApiType>(writer, apiType, options);
        }
        writer.WriteEndArray();

        writer.WritePropertyName(propertyNames.ApiEnumTypes);
        writer.WriteStartArray();
        foreach (var apiType in apiSchema.ApiEnumerationTypes)
        {
            JsonSerializer.Serialize<ApiType>(writer, apiType, options);
        }
        writer.WriteEndArray();

        writer.WritePropertyName(propertyNames.ApiObjectTypes);
        writer.WriteStartArray();
        foreach (var apiType in apiSchema.ApiObjectTypes)
        {
            JsonSerializer.Serialize<ApiType>(writer, apiType, options);
        }
        writer.WriteEndArray();

        if (apiSchema.Extensions != null)
        {
            writer.WritePropertyName(propertyNames.Extensions);
            writer.WriteStartObject();
            foreach (var (extensionType, extension) in apiSchema.Extensions)
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

    #region Cache Implementation Methods
    private static JsonNamingPolicy GetPropertyNamingPolicy(JsonSerializerOptions options)
    {
        var policy = options.PropertyNamingPolicy ?? NullJsonNamingPolicy;
        return policy;
    }

    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return PropertyNamesCache.GetOrAdd(policy, policy => new PropertyNames
        {
            Name = policy.ConvertName(nameof(ApiSchema.Name)),
            ApiScalarTypes = policy.ConvertName(nameof(ApiSchema.ApiScalarTypes)),
            ApiEnumTypes = policy.ConvertName(nameof(ApiSchema.ApiEnumerationTypes)),
            ApiObjectTypes = policy.ConvertName(nameof(ApiSchema.ApiObjectTypes)),
            Extensions = policy.ConvertName(nameof(ApiSchema.Extensions))
        });
    }
    #endregion

    #region Read Implementation Methods
    private static string ReadName(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
        var name = reader.GetString();
        return name ?? throw new JsonException("Expected a non-null name {propertyName}.");
    }

    private List<ApiScalarType> ReadApiScalarTypes(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
        var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options)
            ?? throw new JsonException($"Failed to deserialize {propertyName}.");

        var scalarTypes = apiTypes.Cast<ApiScalarType>().ToList();
        return scalarTypes;
    }

    private List<ApiEnumType> ReadApiEnumTypes(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
        var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options)
            ?? throw new JsonException($"Failed to deserialize {propertyName}.");

        var enumTypes = apiTypes.Cast<ApiEnumType>().ToList();
        return enumTypes;
    }

    private List<ApiObjectType> ReadApiObjectTypes(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
        var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options)
            ?? throw new JsonException($"Failed to deserialize {propertyName}.");

        var objectTypes = apiTypes.Cast<ApiObjectType>().ToList();
        return objectTypes;
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
