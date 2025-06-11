// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        #region Context Types
    private abstract class Context(ILogger<ApiSchemaJsonConverter>? logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames) : IJsonConverterLogger
    {
        public ILogger? Logger { get; } = logger;
        ILogger? IJsonConverterLogger.Logger => Logger;
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
    }

    private class ReadContext(ILogger<ApiSchemaJsonConverter>? logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        public string? Name { get; set; }
        public List<ApiScalarType>? ScalarTypes { get; set; }
        public List<ApiEnumType>? EnumTypes { get; set; }
        public List<ApiObjectType>? ObjectTypes { get; set; }
        public Dictionary<string, object>? Extensions { get; set; }
    }

    private class WriteContext(ILogger<ApiSchemaJsonConverter>? logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonConverterHelpers.JsonReadHandler<ReadContext>> PropertyHandlers = new()
        {
            { propertyNames.Name, HandleName },
            { propertyNames.ApiScalarTypes, HandleApiScalarTypes },
            { propertyNames.ApiEnumTypes, HandleApiEnumTypes },
            { propertyNames.ApiObjectTypes, HandleApiObjectTypes },
            { propertyNames.Extensions, HandleExtensions },
        };
    }
    private static void HandleName(ref Utf8JsonReader reader, ref ReadContext context)
    {
        context.Name = ReadName(ref reader, context.Options, context.PropertyNames.Name);
    }
    private static void HandleApiScalarTypes(ref Utf8JsonReader reader, ref ReadContext context)
    {
        context.ScalarTypes = ReadApiScalarTypes(ref reader, context.Options, context.PropertyNames.ApiScalarTypes);
    }

    private static void HandleApiEnumTypes(ref Utf8JsonReader reader, ref ReadContext context)
    {
        context.EnumTypes = ReadApiEnumTypes(ref reader, context.Options, context.PropertyNames.ApiEnumTypes);
    }

    private static void HandleApiObjectTypes(ref Utf8JsonReader reader, ref ReadContext context)
    {
        context.ObjectTypes = ReadApiObjectTypes(ref reader, context.Options, context.PropertyNames.ApiObjectTypes);
    }

    private static void HandleExtensions(ref Utf8JsonReader reader, ref ReadContext context)
    {
        context.Extensions = ReadExtensions(ref reader, context.Options);
    }
    #endregion
    #region Read Types

    #endregion

    #region Fields
    private readonly ILogger<ApiSchemaJsonConverter>? _logger;

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> PropertyNamesCache = new();
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
        var handlers = new ReadHandlers(propertyNames);
        var context = new ReadContext(_logger, options, propertyNamingPolicy, propertyNames, handlers);

        JsonConverterHelpers.ReadJsonObject(ref reader, ref context, c => c.ReadHandlers.PropertyHandlers);

        var name = context.Name ?? string.Empty;

        var apiTypes = new List<ApiType>();
        if (context.ScalarTypes != null) apiTypes.AddRange(context.ScalarTypes);
        if (context.EnumTypes != null) apiTypes.AddRange(context.EnumTypes);
        if (context.ObjectTypes != null) apiTypes.AddRange(context.ObjectTypes);

        var schema = new ApiSchema(name, apiTypes);

        if (context.Extensions != null)
        {
            foreach (var (typeName, extension) in context.Extensions)
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
        var context = new WriteContext(_logger, options, propertyNamingPolicy, propertyNames);

        writer.WriteStartObject();

        writer.WriteString(context.PropertyNames.Name, apiSchema.Name);

        writer.WritePropertyName(context.PropertyNames.ApiScalarTypes);
        writer.WriteStartArray();
        foreach (var apiType in apiSchema.ApiScalarTypes)
        {
            JsonSerializer.Serialize<ApiType>(writer, apiType, context.Options);
        }
        writer.WriteEndArray();

        writer.WritePropertyName(context.PropertyNames.ApiEnumTypes);
        writer.WriteStartArray();
        foreach (var apiType in apiSchema.ApiEnumerationTypes)
        {
            JsonSerializer.Serialize<ApiType>(writer, apiType, context.Options);
        }
        writer.WriteEndArray();

        writer.WritePropertyName(context.PropertyNames.ApiObjectTypes);
        writer.WriteStartArray();
        foreach (var apiType in apiSchema.ApiObjectTypes)
        {
            JsonSerializer.Serialize<ApiType>(writer, apiType, context.Options);
        }
        writer.WriteEndArray();

        if (apiSchema.Extensions != null)
        {
            writer.WritePropertyName(context.PropertyNames.Extensions);
            writer.WriteStartObject();
            foreach (var (extensionType, extension) in apiSchema.Extensions)
            {
                var name = TypeJsonConverter.GetSerializeTypeName(extensionType);
                writer.WritePropertyName(name);
                JsonSerializer.Serialize(writer, extension, extensionType, context.Options);
            }
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
    #endregion

    #region Cache Implementation Methods
    private static JsonNamingPolicy GetPropertyNamingPolicy(JsonSerializerOptions options)
    {
        return JsonConverterHelpers.GetPropertyNamingPolicy(options);
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

    private static List<ApiScalarType> ReadApiScalarTypes(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
        var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options)
            ?? throw new JsonException($"Failed to deserialize {propertyName}.");

        var scalarTypes = apiTypes.Cast<ApiScalarType>().ToList();
        return scalarTypes;
    }

    private static List<ApiEnumType> ReadApiEnumTypes(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
        var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options)
            ?? throw new JsonException($"Failed to deserialize {propertyName}.");

        var enumTypes = apiTypes.Cast<ApiEnumType>().ToList();
        return enumTypes;
    }

    private static List<ApiObjectType> ReadApiObjectTypes(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
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
