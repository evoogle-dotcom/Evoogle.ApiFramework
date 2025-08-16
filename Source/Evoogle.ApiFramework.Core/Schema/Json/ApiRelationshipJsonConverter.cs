// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extension;
using Evoogle.Json;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

using static Evoogle.ApiFramework.Schema.Json.Internal.ApiJsonConverterHelpers;

namespace Evoogle.ApiFramework.Schema.Json;

public class ApiRelationshipJsonConverter(ILogger<ApiRelationshipJsonConverter>? logger) : JsonConverter<ApiRelationship>
{
    #region Context Types
    private abstract class Context(ILogger<ApiRelationshipJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames) : IHasLogger<ApiRelationshipJsonConverter>
    {
        #region Immutable Properties
        public ILogger<ApiRelationshipJsonConverter> Logger { get; } = new MultiplexingLogger<ApiRelationshipJsonConverter>(logger, MultiplexingLoggerMode.All);
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
        #endregion
    }

    private class ReadContext(ILogger<ApiRelationshipJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
        #region Immutable Properties
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        #endregion

        #region Mutable Properties
        public ReadData ReadData { get; } = new ReadData();
        #endregion
    }

    private class WriteContext(ILogger<ApiRelationshipJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
    }
    #endregion

    #region Property Types
    private readonly record struct ApiRelationshipPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiPropertyName { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiRelationshipPropertyNames ApiRelationship { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiRelationshipReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public string? ApiPropertyName { get; set; }
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
        public ApiRelationshipReadData ApiRelationship { get; } = new();
        public ExtensibleBaseReadData? ExtensibleBase { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _apiTypeModifiersType = typeof(ApiTypeModifiers);
        #endregion

        #region ApiRelationship Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiRelationshipPropertyHandlers = new()
        {
            // ApiRelationship Property Handlers
            { propertyNames.ApiRelationship.ApiName, HandleApiRelationshipApiName },
            { propertyNames.ApiRelationship.ApiPropertyName, HandleApiRelationshipApiPropertyName },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, HandleExtensibleBaseExtensions },
        };
        #endregion

        #region ApiRelationship Methods
        private static void HandleApiRelationshipApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiRelationship.ApiName = reader.GetString();
        }

        private static void HandleApiRelationshipApiPropertyName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiRelationship.ApiPropertyName = reader.GetString();
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
    private readonly ILogger<ApiRelationshipJsonConverter> _logger = new MultiplexingLogger<ApiRelationshipJsonConverter>(logger, MultiplexingLoggerMode.All);

    private static readonly EnumJsonConverter<ApiTypeModifiers> _apiTypeModifiersJsonConverter = new();

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> _propertyNamesCache = new();

    // Cache read handlers per naming policy to avoid rebuilding on every call
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> _readHandlersCache = new();
    #endregion

    #region Constructors
    /// <summary>
    ///     Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.
    /// </summary>
    public ApiRelationshipJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverter<T> Methods
    public override ApiRelationship? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var readHandlers = GetReadHandlers(propertyNamingPolicy, propertyNames);
        var context = new ReadContext(_logger, options, propertyNamingPolicy, propertyNames, readHandlers);

        context.Logger.LogTrace("Deserializing {ApiRelationship}", nameof(ApiRelationship));

        ReadJsonObject<ApiRelationshipJsonConverter, ReadContext>(ref reader, ref context, (context) => context.ReadHandlers.ApiRelationshipPropertyHandlers);

        var apiRelationship = CreateApiRelationship(context);

        context.Logger.LogDebug("Deserialized  {ApiRelationship}", apiRelationship);

        return apiRelationship;
    }

    public override void Write(Utf8JsonWriter writer, ApiRelationship apiRelationship, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var context = new WriteContext(_logger, options, propertyNamingPolicy, propertyNames);

        context.Logger.LogTrace("Serializing {ApiRelationship}", apiRelationship);

        WriteApiRelationship(writer, apiRelationship, context);

        context.Logger.LogDebug("Serialized  {ApiRelationship}", apiRelationship);
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
    private static ApiRelationship CreateApiRelationship(ReadContext context)
    {
        var apiRelationshipReadData = context.ReadData.ApiRelationship;
        var apiRelationship = CreateApiRelationship(apiRelationshipReadData);

        var extensions = context.ReadData.ExtensibleBase?.Extensions;
        AttachExtensions(apiRelationship, extensions);

        return apiRelationship;
    }

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

        WriteExtensibleBaseExtensions(writer, apiRelationship, context);

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
}
