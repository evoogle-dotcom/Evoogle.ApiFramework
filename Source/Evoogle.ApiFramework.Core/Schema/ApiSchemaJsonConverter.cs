// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extension;
using Evoogle.Json;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

using static Evoogle.ApiFramework.Schema.Internal.ApiJsonConverterHelpers;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     JSON converter for <see cref="ApiSchema"/> which reads and writes schema objects.
///     Follows the same patterns used by <see cref="ApiTypeJsonConverter"/>.
/// </summary>
/// <remarks>
///     Optional constructor with logger for use in DI contexts.
/// </remarks>
/// <param name="logger">The optional logger instance.</param>
public class ApiSchemaJsonConverter(ILogger<ApiSchemaJsonConverter>? logger) : JsonConverter<ApiSchema>
{
    #region Context Types
    // Note: This class is not thread-safe and is intended for per-call use.
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
        public required string Version { get; init; }
        public required string ApiScalarTypes { get; init; }
        public required string ApiEnumTypes { get; init; }
        public required string ApiObjectTypes { get; init; }
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
        public string? Version { get; set; }
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
            { propertyNames.ApiSchema.Version, HandleApiSchemaVersion },
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

        private static void HandleApiSchemaVersion(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.Version = reader.GetString();
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
            context.ReadData.ExtensibleBase.Extensions = ReadExtensions(ref reader, context.Options, context.Logger);
        }
        #endregion
    }
    #endregion

    #region Fields
    private readonly ILogger<ApiSchemaJsonConverter> _logger = new MultiplexingLogger<ApiSchemaJsonConverter>(logger, MultiplexingLoggerMode.All);

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

        ReadJsonObject<ApiSchemaJsonConverter, ReadContext>(ref reader, ref context, c => c.ReadHandlers.PropertyHandlers);

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
    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return PropertyNamesCache.GetOrAdd(policy, policy => new PropertyNames
        {
            ApiSchema = new ApiSchemaPropertyNames
            {
                Name = policy.ConvertName(nameof(ApiSchema.Name)),
                Version = policy.ConvertName(nameof(ApiSchema.Version)),
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

    private static ReadHandlers GetReadHandlers(JsonNamingPolicy policy, PropertyNames propertyNames) => ReadHandlersCache.GetOrAdd(policy, policy => new ReadHandlers(propertyNames));
    #endregion

    #region Factory Implementation Methods
    private static ApiSchema CreateApiSchema(in ReadContext context)
    {
        // Validate the JSON that was read during deserialization.
        var validationResults = default(List<ValidationResult>);
        ValidateApiSchemaProperties(context, ref validationResults);

        // Throw if any JSON validation errors were found.
        // This ensures the JSON structure is valid before proceeding with ApiSchema creation.
        ThrowIfInvalid<ApiSchemaJsonConverter, ReadContext, JsonException>
        (
            context,
            nameof(ApiSchema),
            validationResults,
            message => new JsonException(message)
        );

        // Create the ApiSchema instance using the read data.
        var name = context.ReadData.ApiSchema!.Name!;
        var version = context.ReadData.ApiSchema!.Version;
        var apiScalarTypes = context.ReadData.ApiSchema!.ApiScalarTypes;
        var apiEnumTypes = context.ReadData.ApiSchema!.ApiEnumTypes;
        var apiObjectTypes = context.ReadData.ApiSchema!.ApiObjectTypes;

        var apiSchema = new ApiSchema(name, apiScalarTypes, apiEnumTypes, apiObjectTypes)
        {
            Version = version
        };

        // Resolve all ApiTypeExpression instances (named or inline)
        apiSchema.ResolveAllReferences(ref validationResults);

        // Resolve all ApiRelationship instances
        apiSchema.ResolveAllRelationships(ref validationResults);

        // Throw if any validation errors were found API schema creation.
        ThrowIfInvalid<ApiSchemaJsonConverter, ReadContext, ApiSchemaException>
        (
            context,
            nameof(ApiSchema),
            validationResults,
            message => new ApiSchemaException(message)
        );

        // Attach the extensions if present.
        var extensions = context.ReadData.ExtensibleBase?.Extensions;
        AttachExtensions(apiSchema, extensions);

        return apiSchema;
    }
    #endregion

    #region Write Implementation Methods
    // ApiSchema Methods
    private static void WriteApiSchemaBody(Utf8JsonWriter writer, ApiSchema apiSchema, WriteContext context)
    {
        WriteApiSchemaApiTypes(writer, context.PropertyNames.ApiSchema.ApiScalarTypes, apiSchema.ApiScalarTypes, context.Options);
        WriteApiSchemaApiTypes(writer, context.PropertyNames.ApiSchema.ApiEnumTypes, apiSchema.ApiEnumTypes, context.Options);
        WriteApiSchemaApiTypes(writer, context.PropertyNames.ApiSchema.ApiObjectTypes, apiSchema.ApiObjectTypes, context.Options);
    }

    private static void WriteApiSchemaApiTypes(Utf8JsonWriter writer, string propertyName, IEnumerable<ApiType> apiTypes, JsonSerializerOptions options)
    {
        writer.WritePropertyName(propertyName);

        writer.WriteStartArray();

        foreach (var apiType in apiTypes)
        {
            JsonSerializer.Serialize(writer, apiType, options);
        }

        writer.WriteEndArray();
    }

    private static void WriteApiSchemaEpilog(Utf8JsonWriter writer, ApiSchema apiSchema, WriteContext context)
    {
        WriteExtensibleBaseExtensions(writer, apiSchema, context);

        writer.WriteEndObject();
    }

    private static void WriteApiSchemaName(Utf8JsonWriter writer, ApiSchema apiSchema, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiSchema.Name;
        var value = apiSchema.Name;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, value, options);
    }

    private static void WriteApiSchemaProlog(Utf8JsonWriter writer, ApiSchema apiSchema, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiSchemaName(writer, apiSchema, context);
        WriteApiSchemaVersion(writer, apiSchema, context);
    }

    private static void WriteApiSchemaVersion(Utf8JsonWriter writer, ApiSchema apiSchema, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiSchema.Version;
        var value = apiSchema.Version;
        var options = context.Options;

        writer.WriteConditionalPropertyAsString(propertyName, value, options);
    }

    // ExtensibleBase Methods
    private static void WriteExtensibleBaseExtensions(Utf8JsonWriter writer, ExtensibleBase extensibleBase, WriteContext context)
    {
        var extensions = extensibleBase.Extensions;
        if (extensions != null)
        {
            var extensionsPropertyName = context.PropertyNames.ExtensibleBase.Extensions;
            writer.WritePropertyName(extensionsPropertyName);

            WriteExtensions(writer, extensions, context.Options, context.Logger);
        }
    }
    #endregion

    #region Validation Implementation Methods
    private static void ValidateApiSchemaProperties(in ReadContext context, ref List<ValidationResult>? results)
    {
        ValidateApiSchemaProperties
        (
            context.PropertyNames.ApiSchema.Name, context.ReadData.ApiSchema?.Name,
            ref results
        );
    }

    private static void ValidateApiSchemaProperties(string namePropertyName, string? name, ref List<ValidationResult>? results)
    {
        if (name == null)
        {
            AddMissingPropertyError(ref results, namePropertyName);
        }
        else if (string.IsNullOrWhiteSpace(name))
        {
            AddInvalidPropertyError(ref results, namePropertyName, $"{namePropertyName} cannot be empty or whitespace.");
        }
    }
    #endregion
}
