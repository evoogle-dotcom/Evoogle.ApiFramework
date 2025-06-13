// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extension;
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
    #region Context Types
    private abstract class Context(ILogger<ApiSchemaJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames) : IHasLogger<ApiSchemaJsonConverter>
    {
        #region Immutable Properties
        public ILogger<ApiSchemaJsonConverter> Logger { get; } = logger;
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
        #endregion
    }

    private class ReadContext(ILogger<ApiSchemaJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
        #region Immutable Properties
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        #endregion

        #region Mutable Properties
        // Scratchpad for temporarily holding parsed values before type instantiation
        public ReadData ReadData { get; } = new ReadData();
        #endregion
    }

    private class WriteContext(ILogger<ApiSchemaJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
    }
    #endregion

    #region Property Types
    private readonly record struct ApiSchemaPropertyNames
    {
        #region Immutable Properties
        public required string Name { get; init; }
        public required string ApiScalarTypes { get; init; }
        public required string ApiEnumTypes { get; init; }
        public required string ApiObjectTypes { get; init; }
        #endregion
    }

    private readonly record struct ExtensibleBasePropertyNames
    {
        #region Immutable Properties
        public required string Extensions { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiSchemaPropertyNames ApiSchema { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiSchemaReadData
    {
        #region Properties
        public string? Name { get; set; }
        public List<ApiScalarType>? ApiScalarTypes { get; set; }
        public List<ApiEnumType>? ApiEnumTypes { get; set; }
        public List<ApiObjectType>? ApiObjectTypes { get; set; }
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
        public ApiSchemaReadData? ApiSchema { get; set; }
        public ExtensibleBaseReadData? ExtensibleBase { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiSchema Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> PropertyHandlers = new()
        {
            // ApiSchema Property Handlers
            { propertyNames.ApiSchema.Name, HandleApiSchemaName },
            { propertyNames.ApiSchema.ApiScalarTypes, HandleApiSchemaApiScalarTypes },
            { propertyNames.ApiSchema.ApiEnumTypes, HandleApiSchemaApiEnumTypes },
            { propertyNames.ApiSchema.ApiObjectTypes, HandleApiObjectTypes },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, HandleExtensibleBaseExtensions },
        };
        #endregion

        #region ApiSchema Methods
        private static void HandleApiSchemaName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.Name = reader.GetString();
        }

        private static void HandleApiSchemaApiScalarTypes(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiSchema.ApiScalarTypes}.");
            var apiScalarTypes = apiTypes.Cast<ApiScalarType>().ToList();

            context.ReadData.ApiSchema.ApiScalarTypes = apiScalarTypes;
        }

        private static void HandleApiSchemaApiEnumTypes(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiSchema.ApiEnumTypes}.");
            var apiEnumTypes = apiTypes.Cast<ApiEnumType>().ToList();

            context.ReadData.ApiSchema.ApiEnumTypes = apiEnumTypes;
        }

        private static void HandleApiObjectTypes(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiSchema.ApiObjectTypes}.");
            var apiObjectTypes = apiTypes.Cast<ApiObjectType>().ToList();

            context.ReadData.ApiSchema.ApiObjectTypes = apiObjectTypes;
        }
        #endregion

        #region ExtensibleBase Methods
        private static void HandleExtensibleBaseExtensions(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ExtensibleBase ??= new ExtensibleBaseReadData();
            context.ReadData.ExtensibleBase.Extensions ??= [];

            // Validate the start of the JSON object.
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of an object.");

            // Read the JSON extension object properties.
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected object property name.");

                // Note: We didn't apply naming policy to extension type name as property name for round-trip compatibility.
                // Read the JSON property name (extension type name)
                var extensionTypeName = reader.GetString()!;
                var extensionType = Json.TypeJsonConverter.GetDeserializeType(extensionTypeName);

                context.Logger.LogTrace("Deserializing extension type: {ExtensionType}", extensionType.Name);

                // Move past the JSON property name (extension type name)
                reader.Read();

                // Read the extension object
                var options = context.Options;
                var extension = JsonSerializer.Deserialize(ref reader, extensionType, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ExtensibleBase.Extensions}.");

                context.Logger.LogDebug("Deserialized  extension type: {ExtensionType}", extensionType.Name);

                context.ReadData.ExtensibleBase.Extensions.Add(extensionTypeName, extension);
            }
        }
        #endregion
    }
    #endregion

    #region Fields
    private readonly ILogger<ApiSchemaJsonConverter> _logger;

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> PropertyNamesCache = new();

    // Cache read handlers per naming policy to avoid rebuilding on every call
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> ReadHandlersCache = new();
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

    #region JsonConverter<T> Methods
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
        var readHandlers = GetReadHandlers(propertyNamingPolicy, propertyNames);
        var context = new ReadContext(_logger, options, propertyNamingPolicy, propertyNames, readHandlers);

        context.Logger.LogTrace("Deserializing {ApiSchema}", nameof(ApiSchema));

        ApiJsonConverterHelpers.ReadJsonObject<ApiSchemaJsonConverter, ReadContext>(ref reader, ref context, c => c.ReadHandlers.PropertyHandlers);

        var apiSchema = CreateApiSchema(context);

        context.Logger.LogTrace("Deserialized  {ApiSchema}", apiSchema);

        return apiSchema;
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

        context.Logger.LogTrace("Serializing {ApiSchema}", apiSchema);

        WriteApiSchemaProlog(writer, apiSchema, context);
        WriteApiSchemaBody(writer, apiSchema, context);
        WriteApiSchemaEpilog(writer, apiSchema, context);

        context.Logger.LogTrace("Serialized  {ApiSchema}", apiSchema);
    }
    #endregion

    #region Cache Implementation Methods
    private static JsonNamingPolicy GetPropertyNamingPolicy(JsonSerializerOptions options)
    {
        return ApiJsonConverterHelpers.GetPropertyNamingPolicy(options);
    }

    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return PropertyNamesCache.GetOrAdd(policy, policy => new PropertyNames
        {
            ApiSchema = new ApiSchemaPropertyNames
            {
                Name = policy.ConvertName(nameof(ApiSchema.Name)),
                ApiScalarTypes = policy.ConvertName(nameof(ApiSchema.ApiScalarTypes)),
                ApiEnumTypes = policy.ConvertName(nameof(ApiSchema.ApiEnumTypes)),
                ApiObjectTypes = policy.ConvertName(nameof(ApiSchema.ApiObjectTypes))
            },
            ExtensibleBase = new ExtensibleBasePropertyNames
            {
                Extensions = policy.ConvertName(nameof(ExtensibleBase.Extensions))
            }
        });
    }

    private static ReadHandlers GetReadHandlers(JsonNamingPolicy policy, PropertyNames propertyNames)
    {
        return ReadHandlersCache.GetOrAdd(policy, policy => new ReadHandlers(propertyNames));
    }
    #endregion

    #region Factory Implementation Methods
    private static ApiSchema CreateApiSchema(in ReadContext context)
    {
        var name = context.ReadData.ApiSchema!.Name!;

        var apiTypes = new List<ApiType>();

        if (context.ReadData.ApiSchema.ApiScalarTypes != null)
        {
            apiTypes.AddRange(context.ReadData.ApiSchema.ApiScalarTypes);
        }

        if (context.ReadData.ApiSchema.ApiEnumTypes != null)
        {
            apiTypes.AddRange(context.ReadData.ApiSchema.ApiEnumTypes);
        }

        if (context.ReadData.ApiSchema.ApiObjectTypes != null)
        {
            apiTypes.AddRange(context.ReadData.ApiSchema.ApiObjectTypes);
        }

        var apiSchema = new ApiSchema(name, apiTypes);

        AttachExtensions(context, apiSchema);

        return apiSchema;
    }

    private static void AttachExtensions(in ReadContext context, ExtensibleBase extensibleBase)
    {
        var extensions = context.ReadData.ExtensibleBase?.Extensions;
        if (extensions != null)
        {
            foreach (var (extensionTypeName, extension) in extensions)
            {
                var extensionType = TypeJsonConverter.GetDeserializeType(extensionTypeName);
                extensibleBase.AttachExtension(extensionType, extension);
            }
        }
    }
    #endregion

    #region Write Implementation Methods
    // ApiSchema Methods
    private static void WriteApiSchemaBody
    (
        Utf8JsonWriter writer,
        ApiSchema apiSchema,
        in WriteContext context
    )
    {
        WriteApiSchemaApiScalarTypes(writer, apiSchema, context);
        WriteApiSchemaApiEnumTypes(writer, apiSchema, context);
        WriteApiSchemaApiObjectTypes(writer, apiSchema, context);
    }

    private static void WriteApiSchemaApiEnumTypes
    (
        Utf8JsonWriter writer,
        ApiSchema apiSchema,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiEnumTypes;
        var values = apiSchema.ApiEnumTypes;
        var options = context.Options;

        writer.WritePropertyName(propertyName);

        writer.WriteStartArray();

        foreach (var value in values)
        {
            JsonSerializer.Serialize<ApiType>(writer, value, options);
        }

        writer.WriteEndArray();
    }

    private static void WriteApiSchemaEpilog
    (
        Utf8JsonWriter writer,
        ApiSchema apiSchema,
        in WriteContext context
    )
    {
        WriteExtensibleBaseExtensions(writer, apiSchema, context);

        writer.WriteEndObject();
    }

    private static void WriteApiSchemaApiObjectTypes
    (
        Utf8JsonWriter writer,
        ApiSchema apiSchema,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiObjectTypes;
        var values = apiSchema.ApiObjectTypes;
        var options = context.Options;

        writer.WritePropertyName(propertyName);

        writer.WriteStartArray();

        foreach (var value in values)
        {
            JsonSerializer.Serialize<ApiType>(writer, value, options);
        }

        writer.WriteEndArray();
    }

    private static void WriteApiSchemaApiScalarTypes
    (
        Utf8JsonWriter writer,
        ApiSchema apiSchema,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiScalarTypes;
        var values = apiSchema.ApiScalarTypes;
        var options = context.Options;

        writer.WritePropertyName(propertyName);

        writer.WriteStartArray();

        foreach (var value in values)
        {
            JsonSerializer.Serialize<ApiType>(writer, value, options);
        }

        writer.WriteEndArray();
    }

    private static void WriteApiSchemaName
    (
        Utf8JsonWriter writer,
        ApiSchema apiSchema,
        in WriteContext context
    )
    {
        var propertyName = context.PropertyNames.ApiSchema.Name;
        var value = apiSchema.Name;
        var options = context.Options;

        writer.WritePropertyString(propertyName, value, options);
    }

    private static void WriteApiSchemaProlog
    (
        Utf8JsonWriter writer,
        ApiSchema apiSchema,
        in WriteContext context
    )
    {
        writer.WriteStartObject();

        WriteApiSchemaName(writer, apiSchema, context);
    }

    // ExtensibleBase Methods
    private static void WriteExtensibleBaseExtensions
    (
        Utf8JsonWriter writer,
        ExtensibleBase extensibleBase,
        in WriteContext context
    )
    {
        var extensions = extensibleBase.Extensions;
        if (extensions != null)
        {
            var extensionsPropertyName = context.PropertyNames.ExtensibleBase.Extensions;
            writer.WritePropertyName(extensionsPropertyName);

            writer.WriteStartObject();
            foreach (var (extensionType, extension) in extensions)
            {
                // Note: Don't apply naming policy to extension type name as property name for round-trip compatibility.
                var extensionTypeName = TypeJsonConverter.GetSerializeTypeName(extensionType);

                context.Logger.LogTrace("Serializing extension type: {ExtensionType}", extensionType.Name);

                writer.WritePropertyName(extensionTypeName);

                var options = context.Options;
                JsonSerializer.Serialize(writer, extension, extensionType, options);

                context.Logger.LogDebug("Serialized  extension type: {ExtensionType}", extensionType.Name);
            }
            writer.WriteEndObject();
        }
    }
    #endregion
}
