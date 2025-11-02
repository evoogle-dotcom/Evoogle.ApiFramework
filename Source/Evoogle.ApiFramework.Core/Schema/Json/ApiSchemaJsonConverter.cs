// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Text.Json;

using Evoogle.Extension;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     JSON converter for <see cref="ApiSchema"/> which reads and writes schema objects.
///     Follows the same patterns used by <see cref="ApiTypeJsonConverter"/>.
/// </summary>
/// <remarks>
///     Optional constructor with logger for use in DI contexts.
/// </remarks>
/// <param name="logger">The optional logger instance.</param>
public class ApiSchemaJsonConverter(ILogger<ApiSchemaJsonConverter>? logger) : JsonConverterBase<ApiSchema>(logger)
{
    #region Context Types
    /// <summary>
    ///     Base context class for reading or writing <see cref="ApiSchema"/> JSON data.
    /// </summary>
    private abstract class Context(ILogger logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames) : IContext
    {
        #region Immutable Properties
        public ILogger Logger { get; } = logger;
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
        #endregion
    }

    /// <summary>
    ///     Context class used during JSON deserialization of <see cref="ApiSchema"/>.
    ///     Holds intermediate values and read handlers.
    /// </summary>
    private class ReadContext(ILogger logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(logger, options, propertyNamingPolicy, propertyNames), IReadContext
    {
        #region Immutable Properties
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        #endregion

        #region Mutable Properties
        public ReadData ReadData { get; } = new ReadData();
        #endregion
    }

    /// <summary>
    ///     Context class used during JSON serialization of <see cref="ApiSchema"/>.
    /// </summary>
    private class WriteContext(ILogger logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames), IWriteContext
    {
    }
    #endregion

    #region Property Types
    /// <summary>
    ///     Represents the JSON property names associated with an <see cref="ApiSchema"/> for a given naming policy.
    /// </summary>
    private readonly record struct ApiSchemaPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiVersion { get; init; }
        public required string ApiScalarTypes { get; init; }
        public required string ApiEnumTypes { get; init; }
        public required string ApiObjectTypes { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates all property name metadata used during schema serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiSchemaPropertyNames ApiSchema { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while parsing an <see cref="ApiSchema"/> from JSON.
    /// </summary>
    private class ApiSchemaReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public string? ApiVersion { get; set; }
        public List<ApiScalarType>? ApiScalarTypes { get; set; }
        public List<ApiEnumType>? ApiEnumTypes { get; set; }
        public List<ApiObjectType>? ApiObjectTypes { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the complete data set required to instantiate an <see cref="ApiSchema"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiSchemaReadData? ApiSchema { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides property handlers that map JSON members to <see cref="ReadData"/> assignments.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiSchema Fields
        public readonly Dictionary<string, JsonReaderHandler<ReadContext>> PropertyHandlers = new()
        {
            // ApiSchema Property Handlers
            { propertyNames.ApiSchema.ApiName, HandleApiSchemaApiName },
            { propertyNames.ApiSchema.ApiVersion, HandleApiSchemaApiVersion },
            { propertyNames.ApiSchema.ApiScalarTypes, HandleApiSchemaApiScalarTypes },
            { propertyNames.ApiSchema.ApiEnumTypes, HandleApiSchemaApiEnumTypes },
            { propertyNames.ApiSchema.ApiObjectTypes, HandleApiObjectTypes },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, (ref Utf8JsonReader reader, ReadContext context) =>
                context.ReadData.Extensions = ReadJsonExtensionsObject(ref reader, context) },
        };
        #endregion

        #region ApiSchema Methods
        private static void HandleApiSchemaApiName(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.ApiName = reader.GetString();
        }

        private static void HandleApiSchemaApiVersion(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.ApiVersion = reader.GetString();
        }

        private static void HandleApiSchemaApiScalarTypes(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiSchema.ApiScalarTypes}.");
            var apiScalarTypes = apiTypes.Cast<ApiScalarType>().ToList();

            context.ReadData.ApiSchema.ApiScalarTypes = apiScalarTypes;
        }

        private static void HandleApiSchemaApiEnumTypes(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiSchema.ApiEnumTypes}.");
            var apiEnumTypes = apiTypes.Cast<ApiEnumType>().ToList();

            context.ReadData.ApiSchema.ApiEnumTypes = apiEnumTypes;
        }

        private static void HandleApiObjectTypes(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var apiTypes = JsonSerializer.Deserialize<List<ApiType>>(ref reader, options) ?? throw new JsonException($"Failed to deserialize {context.PropertyNames.ApiSchema.ApiObjectTypes}.");
            var apiObjectTypes = apiTypes.Cast<ApiObjectType>().ToList();

            context.ReadData.ApiSchema.ApiObjectTypes = apiObjectTypes;
        }
        #endregion

    }
    #endregion

    #region Fields
    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> _propertyNamesCache = new();

    // Cache read handlers per naming policy to avoid rebuilding on every call
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> _readHandlersCache = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiSchemaJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = options.GetPropertyNamingPolicy();
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var readHandlers = GetReadHandlers(propertyNamingPolicy, propertyNames);
        var context = new ReadContext(logger, options, propertyNamingPolicy, propertyNames, readHandlers);

        return context;
    }

    protected override ApiSchema? CreateValue(IReadContext context)
    {
        var readContext = (ReadContext)context;

        // Create the ApiSchema instance using the read data.
        var apiName = readContext.ReadData.ApiSchema?.ApiName;
        var apiVersion = readContext.ReadData.ApiSchema?.ApiVersion;
        var apiScalarTypes = readContext.ReadData.ApiSchema?.ApiScalarTypes;
        var apiEnumTypes = readContext.ReadData.ApiSchema?.ApiEnumTypes;
        var apiObjectTypes = readContext.ReadData.ApiSchema?.ApiObjectTypes;

        var apiSchema = new ApiSchema
        (
            apiName!,
            apiScalarTypes,
            apiEnumTypes,
            apiObjectTypes
        )
        {
            ApiVersion = apiVersion
        };

        // Attach the extensions if present.
        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiSchema, extensions);

        // Initialize the ApiSchema instance.
        var result = apiSchema.Initialize();
        result.ThrowIfInvalid();

        return apiSchema;
    }

    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = options.GetPropertyNamingPolicy();
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var context = new WriteContext(logger, options, propertyNamingPolicy, propertyNames);

        return context;
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (ReadContext)context;
        ReadJsonObject(ref reader, readContext, (readContext) => readContext.ReadHandlers.PropertyHandlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiSchema value, IWriteContext context)
    {
        var writeContext = (WriteContext)context;
        WriteApiSchemaProlog(writer, value, writeContext);
        WriteApiSchemaBody(writer, value, writeContext);
        WriteApiSchemaEpilog(writer, value, writeContext);
    }
    #endregion

    #region Cache Implementation Methods
    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return _propertyNamesCache.GetOrAdd(policy, policy => new PropertyNames
        {
            ApiSchema = new ApiSchemaPropertyNames
            {
                ApiName = policy.ConvertName(nameof(ApiSchema.ApiName)),
                ApiVersion = policy.ConvertName(nameof(ApiSchema.ApiVersion)),
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

    private static ReadHandlers GetReadHandlers(JsonNamingPolicy policy, PropertyNames propertyNames) => _readHandlersCache.GetOrAdd(policy, policy => new ReadHandlers(propertyNames));
    #endregion

    #region Write Implementation Methods
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
        WriteExtensibleBaseExtensions(writer, context.PropertyNames.ExtensibleBase.Extensions, apiSchema, context);

        writer.WriteEndObject();
    }

    private static void WriteApiSchemaApiName(Utf8JsonWriter writer, ApiSchema apiSchema, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiName;
        var value = apiSchema.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiSchemaProlog(Utf8JsonWriter writer, ApiSchema apiSchema, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiSchemaApiName(writer, apiSchema, context);
        WriteApiSchemaApiVersion(writer, apiSchema, context);
    }

    private static void WriteApiSchemaApiVersion(Utf8JsonWriter writer, ApiSchema apiSchema, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiVersion;
        var value = apiSchema.ApiVersion;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }
    #endregion
}
