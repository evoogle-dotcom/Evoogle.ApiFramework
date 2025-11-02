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
///     Provides System.Text.Json serialization support for <see cref="ApiRelationship"/> instances, including
///     extension payloads and schema-specific naming policies.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipJsonConverter(ILogger<ApiRelationshipJsonConverter>? logger) : JsonConverterBase<ApiRelationship>(logger)
{
    #region Context Types
    /// <summary>
    ///     Represents the common state that is required while reading or writing an <see cref="ApiRelationship"/>.
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
    ///     Captures state that is accumulated while deserializing an <see cref="ApiRelationship"/> from JSON.
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
    ///     Represents the contextual information required while serializing an <see cref="ApiRelationship"/> to JSON.
    /// </summary>
    private class WriteContext(ILogger logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames), IWriteContext
    {
    }
    #endregion

    #region Property Types
    /// <summary>
    ///     Provides cached JSON property names for <see cref="ApiRelationship"/> members under a specific naming policy.
    /// </summary>
    private readonly record struct ApiRelationshipPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiPropertyName { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates the property name sets used while reading or writing relationships and extension data.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiRelationshipPropertyNames ApiRelationship { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading the primitive relationship properties from JSON.
    /// </summary>
    private class ApiRelationshipReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public string? ApiPropertyName { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects all data encountered while deserializing a relationship, including extensions.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiRelationshipReadData ApiRelationship { get; } = new();
        #endregion
    }

    /// <summary>
    ///     Provides handlers that map JSON property names to strongly typed relationship data assignments.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiRelationship Fields
        public readonly Dictionary<string, JsonReaderHandler<ReadContext>> ApiRelationshipPropertyHandlers = new()
        {
            // ApiRelationship Property Handlers
            { propertyNames.ApiRelationship.ApiName, HandleApiRelationshipApiName },
            { propertyNames.ApiRelationship.ApiPropertyName, HandleApiRelationshipApiPropertyName },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, (ref Utf8JsonReader reader, ReadContext context) =>
                context.ReadData.Extensions = ReadJsonExtensionsObject(ref reader, context) },
        };
        #endregion

        #region ApiRelationship Methods
        private static void HandleApiRelationshipApiName(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiRelationship.ApiName = reader.GetString();
        }

        private static void HandleApiRelationshipApiPropertyName(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiRelationship.ApiPropertyName = reader.GetString();
        }
        #endregion

    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeModifiers> _apiTypeModifiersJsonConverter = new();

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> _propertyNamesCache = new();

    // Cache read handlers per naming policy to avoid rebuilding on every call
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> _readHandlersCache = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipJsonConverter()
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

    protected override ApiRelationship? CreateValue(IReadContext context)
    {
        var readContext = (ReadContext)context;

        var apiRelationshipReadData = readContext.ReadData.ApiRelationship;
        var apiRelationship = CreateApiRelationship(apiRelationshipReadData);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiRelationship, extensions);

        return apiRelationship;
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
        ReadJsonObject(ref reader, readContext, (readContext) => readContext.ReadHandlers.ApiRelationshipPropertyHandlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationship value, IWriteContext context)
    {
        var writeContext = (WriteContext)context;
        WriteApiRelationship(writer, value, writeContext);
    }
    #endregion

    #region Cache Implementation Methods
    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return _propertyNamesCache.GetOrAdd(policy, policy => new PropertyNames
        {
            ApiRelationship = new ApiRelationshipPropertyNames
            {
                ApiName = policy.ConvertName(nameof(ApiRelationship.ApiName)),
                ApiPropertyName = policy.ConvertName(nameof(ApiRelationship.ApiPropertyName))
            },
            ExtensibleBase = new ExtensibleBasePropertyNames
            {
                Extensions = policy.ConvertName(nameof(ExtensibleBase.Extensions))
            }
        });
    }

    private static ReadHandlers GetReadHandlers(JsonNamingPolicy policy, PropertyNames propertyNames) => _readHandlersCache.GetOrAdd(policy, policy => new ReadHandlers(propertyNames));
    #endregion

    #region Factory Implementation Methods
    private static ApiRelationship CreateApiRelationship(ApiRelationshipReadData apiRelationshipReadData)
    {
        var apiName = apiRelationshipReadData.ApiName;
        var apiPropertyName = apiRelationshipReadData.ApiPropertyName;

        return new ApiRelationship(apiName!, apiPropertyName);
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiRelationship(Utf8JsonWriter writer, ApiRelationship apiRelationship, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiRelationshipApiName(writer, apiRelationship, context);
        WriteApiRelationshipApiPropertyName(writer, apiRelationship, context);

        WriteExtensibleBaseExtensions(writer, context.PropertyNames.ExtensibleBase.Extensions, apiRelationship, context);

        writer.WriteEndObject();
    }

    private static void WriteApiRelationshipApiName(Utf8JsonWriter writer, ApiRelationship apiRelationship, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiRelationship.ApiName;
        var value = apiRelationship.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiRelationshipApiPropertyName(Utf8JsonWriter writer, ApiRelationship apiRelationship, WriteContext context)
    {
        var apiName = apiRelationship.ApiName;
        var apiPropertyName = apiRelationship.ApiPropertyName;
        if (apiName.Equals(apiPropertyName))
        {
            // If the API name and property name are the same, we do not need to write the property name.
            return;
        }

        var propertyName = context.PropertyNames.ApiRelationship.ApiPropertyName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, apiPropertyName, options);
    }
    #endregion
}
