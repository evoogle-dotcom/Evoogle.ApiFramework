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
using Evoogle.ApiFramework.Schema.Json.Internal;

namespace Evoogle.ApiFramework.Schema.Json;

public class ApiEnumValueJsonConverter(ILogger<ApiEnumValueJsonConverter>? logger) : JsonConverter<ApiEnumValue>
{
    #region Context Types
    private abstract class Context(ILogger<ApiEnumValueJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames) : IHasLogger<ApiEnumValueJsonConverter>
    {
        #region Immutable Properties
        public ILogger<ApiEnumValueJsonConverter> Logger { get; } = new MultiplexingLogger<ApiEnumValueJsonConverter>(logger, MultiplexingLoggerMode.All);
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
        #endregion
    }

    private class ReadContext(ILogger<ApiEnumValueJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
        #region Immutable Properties
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        #endregion

        #region Mutable Properties
        public ReadData ReadData { get; } = new ReadData();
        #endregion
    }

    private class WriteContext(ILogger<ApiEnumValueJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
    }
    #endregion

    #region Property Types
    private readonly record struct ApiEnumValuePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ClrName { get; init; }
        public required string ClrOrdinal { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiEnumValuePropertyNames ApiEnumValue { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiEnumValueReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public string? ClrName { get; set; }
        public int? ClrOrdinal { get; set; }
        #endregion
    }

    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiEnumValueReadData ApiEnumValue { get; } = new();
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _apiTypeModifiersType = typeof(ApiTypeModifiers);
        #endregion

        #region ApiEnumValue Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiEnumValuePropertyHandlers = new()
        {
            // ApiEnumValue Property Handlers
            { propertyNames.ApiEnumValue.ApiName, HandleApiEnumValueApiName },
            { propertyNames.ApiEnumValue.ClrName, HandleApiEnumValueClrName },
            { propertyNames.ApiEnumValue.ClrOrdinal, HandleApiEnumValueClrOrdinal },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, (ref Utf8JsonReader reader, ref ReadContext context) =>
                context.ReadData.Extensions = ReadExtensions(ref reader, context.Options, context.Logger) },
        };
        #endregion

        #region ApiEnumValue Methods
        private static void HandleApiEnumValueApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumValue.ApiName = reader.GetString();
        }

        private static void HandleApiEnumValueClrName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumValue.ClrName = reader.GetString();
        }

        private static void HandleApiEnumValueClrOrdinal(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiEnumValue.ClrOrdinal = reader.GetInt32();
        }
        #endregion

    }
    #endregion

    #region Fields
    private readonly ILogger<ApiEnumValueJsonConverter> _logger = new MultiplexingLogger<ApiEnumValueJsonConverter>(logger, MultiplexingLoggerMode.All);

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
    public ApiEnumValueJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverter<T> Methods
    public override ApiEnumValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var readHandlers = GetReadHandlers(propertyNamingPolicy, propertyNames);
        var context = new ReadContext(_logger, options, propertyNamingPolicy, propertyNames, readHandlers);

        context.Logger.LogTrace("Deserializing {ApiEnumValue}", nameof(ApiEnumValue));

        ReadJsonObject<ApiEnumValueJsonConverter, ReadContext>(ref reader, ref context, (context) => context.ReadHandlers.ApiEnumValuePropertyHandlers);

        var apiEnumValue = CreateApiEnumValue(context);

        context.Logger.LogDebug("Deserialized  {ApiEnumValue}", apiEnumValue);

        return apiEnumValue;
    }

    public override void Write(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var context = new WriteContext(_logger, options, propertyNamingPolicy, propertyNames);

        context.Logger.LogTrace("Serializing {ApiEnumValue}", apiEnumValue);

        WriteApiEnumValue(writer, apiEnumValue, context);

        context.Logger.LogDebug("Serialized  {ApiEnumValue}", apiEnumValue);
    }
    #endregion

    #region Cache Implementation Methods
    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return _propertyNamesCache.GetOrAdd(policy, policy => new PropertyNames
        {
            ApiEnumValue = new ApiEnumValuePropertyNames
            {
                ApiName = policy.ConvertName(nameof(ApiEnumValue.ApiName)),
                ClrName = policy.ConvertName(nameof(ApiEnumValue.ClrName)),
                ClrOrdinal = policy.ConvertName(nameof(ApiEnumValue.ClrOrdinal))
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
    private static ApiEnumValue CreateApiEnumValue(ReadContext context)
    {
        var apiEnumValueReadData = context.ReadData.ApiEnumValue;
        var apiEnumValue = CreateApiEnumValue(apiEnumValueReadData);

        var extensions = context.ReadData.Extensions;
        AttachExtensions(apiEnumValue, extensions);

        return apiEnumValue;
    }

    private static ApiEnumValue CreateApiEnumValue(ApiEnumValueReadData apiEnumValueReadData)
    {
        var apiName = apiEnumValueReadData.ApiName;
        var clrName = apiEnumValueReadData.ClrName;
        var clrOrdinal = apiEnumValueReadData.ClrOrdinal.GetValueOrDefault();

        return new ApiEnumValue(apiName!, clrName!, clrOrdinal);
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiEnumValue(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiEnumValueApiName(writer, apiEnumValue, context);
        WriteApiEnumValueClrName(writer, apiEnumValue, context);
        WriteApiEnumValueClrOrdinal(writer, apiEnumValue, context);

        WriteExtensibleBaseExtensions(writer, apiEnumValue, context.PropertyNames.ExtensibleBase.Extensions, context.Options, context.Logger);

        writer.WriteEndObject();
    }

    private static void WriteApiEnumValueApiName(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ApiName;
        var value = apiEnumValue.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiEnumValueClrName(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrName;
        var value = apiEnumValue.ClrName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiEnumValueClrOrdinal(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrOrdinal;
        var value = apiEnumValue.ClrOrdinal;
        var options = context.Options;

        writer.TryWritePropertyAsNumber(propertyName, value, options);
    }

    #endregion
}
