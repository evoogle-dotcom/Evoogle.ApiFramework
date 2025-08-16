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

using static Evoogle.ApiFramework.Schema.Json.Internal.ApiJsonConverterHelpers;

namespace Evoogle.ApiFramework.Schema.Json;

public class ApiTypeExpressionJsonConverter(ILogger<ApiTypeExpressionJsonConverter>? logger) : JsonConverter<ApiTypeExpression>
{
    #region Context Types
    private abstract class Context(ILogger<ApiTypeExpressionJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames) : IHasLogger<ApiTypeExpressionJsonConverter>
    {
        #region Immutable Properties
        public ILogger<ApiTypeExpressionJsonConverter> Logger { get; } = new MultiplexingLogger<ApiTypeExpressionJsonConverter>(logger, MultiplexingLoggerMode.All);
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
        #endregion
    }

    private class ReadContext(ILogger<ApiTypeExpressionJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
        #region Immutable Properties
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        #endregion

        #region Mutable Properties
        public ReadData ReadData { get; } = new ReadData();
        #endregion
    }

    private class WriteContext(ILogger<ApiTypeExpressionJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
    }
    #endregion

    #region Property Types
    private readonly record struct ApiTypeExpressionPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiInlineType { get; init; }
        public required string ClrType { get; init; }
        public required string Kind { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiTypeExpressionPropertyNames ApiTypeExpression { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiTypeExpressionReadData
    {
        #region Properties
        public ApiType? ApiInlineType { get; set; }
        public string? ApiName { get; set; }
        public Type? ClrType { get; set; }
        public string? Kind { get; set; }
        #endregion
    }

    private class ReadData
    {
        #region Properties
        public ApiTypeExpressionReadData? ApiTypeExpression { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiTypeExpression Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiTypeExpressionHandlers = new()
        {
            { propertyNames.ApiTypeExpression.ApiInlineType, HandleApiTypeExpressionApiInlineType },
            { propertyNames.ApiTypeExpression.ApiName, HandleApiTypeExpressionApiName },
            { propertyNames.ApiTypeExpression.ClrType, HandleApiTypeExpressionClrType },
            { propertyNames.ApiTypeExpression.Kind, HandleApiTypeExpressionKind },
        };
        #endregion

        #region ApiTypeExpression Methods
        private static void HandleApiTypeExpressionApiInlineType(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ApiInlineType = JsonSerializer.Deserialize<ApiType>(ref reader, context.Options);
        }

        private static void HandleApiTypeExpressionApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ApiName = reader.GetString();
        }

        private static void HandleApiTypeExpressionClrType(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ClrType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }

        private static void HandleApiTypeExpressionKind(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.Kind = reader.GetString();
        }
        #endregion
    }
    #endregion

    #region Fields
    private readonly ILogger<ApiTypeExpressionJsonConverter> _logger = new MultiplexingLogger<ApiTypeExpressionJsonConverter>(logger, MultiplexingLoggerMode.All);

    private static readonly EnumJsonConverter<ApiTypeKind> _apiTypeKindJsonConverter = new();

    private static readonly TypeJsonConverter _typeJsonConverter = new();

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> _propertyNamesCache = new();

    // Cache read handlers per naming policy to avoid rebuilding on every call
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> _readHandlersCache = new();
    #endregion

    #region Constructors
    /// <summary>
    ///     Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.
    /// </summary>
    public ApiTypeExpressionJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverter<T> Methods
    public override ApiTypeExpression? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var readHandlers = GetReadHandlers(propertyNamingPolicy, propertyNames);
        var context = new ReadContext(_logger, options, propertyNamingPolicy, propertyNames, readHandlers);

        context.Logger.LogTrace("Deserializing {ApiTypeExpression}", nameof(ApiTypeExpression));

        ReadJsonObject<ApiTypeExpressionJsonConverter, ReadContext>(ref reader, ref context, (context) => context.ReadHandlers.ApiTypeExpressionHandlers);

        var apiTypeExpression = CreateApiTypeExpression(context);

        context.Logger.LogDebug("Deserialized  {ApiTypeExpression}", apiTypeExpression);

        return apiTypeExpression;
    }

    public override void Write(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var context = new WriteContext(_logger, options, propertyNamingPolicy, propertyNames);

        context.Logger.LogTrace("Serializing {ApiTypeExpression}", apiTypeExpression);

        WriteApiTypeExpression(writer, apiTypeExpression, context);

        context.Logger.LogDebug("Serialized  {ApiTypeExpression}", apiTypeExpression);
    }
    #endregion

    #region Cache Implementation Methods
    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return _propertyNamesCache.GetOrAdd(policy, policy => new PropertyNames
        {
            ApiTypeExpression = new ApiTypeExpressionPropertyNames
            {
                ApiName = policy.ConvertName(nameof(ApiTypeExpression.ApiName)),
                ApiInlineType = policy.ConvertName(nameof(ApiTypeExpression.ApiInlineType)),
                ClrType = policy.ConvertName(nameof(ApiTypeExpression.ClrType)),
                Kind = policy.ConvertName(nameof(ApiTypeExpression.Kind))
            }
        });
    }

    private static ReadHandlers GetReadHandlers(JsonNamingPolicy policy, PropertyNames propertyNames) => _readHandlersCache.GetOrAdd(policy, policy => new ReadHandlers(propertyNames));
    #endregion

    #region Factory Implementation Methods
    private static ApiTypeExpression CreateApiTypeExpression(ReadContext context)
    {
        var apiTypeExpressionReadData = context.ReadData.ApiTypeExpression;

        var apiInlineType = apiTypeExpressionReadData?.ApiInlineType;
        if (apiInlineType is not null)
        {
            return new ApiTypeExpression(apiInlineType);
        }

        var kind = GetApiTypeKind(context.Logger, apiTypeExpressionReadData?.Kind);
        var apiName = apiTypeExpressionReadData?.ApiName;
        var clrType = apiTypeExpressionReadData?.ClrType;

        return new ApiTypeExpression(kind, apiName, clrType);
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiTypeExpression(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiTypeExpressionKind(writer, apiTypeExpression, context);
        WriteApiTypeExpressionApiName(writer, apiTypeExpression, context);
        WriteApiTypeExpressionApiInlineType(writer, apiTypeExpression, context);
        WriteApiTypeExpressionClrType(writer, apiTypeExpression, context);

        writer.WriteEndObject();
    }

    private static void WriteApiTypeExpressionKind(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.Kind;
        var kind = apiTypeExpression.Kind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, kind, options, _apiTypeKindJsonConverter);
    }

    private static void WriteApiTypeExpressionApiName(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ApiName;
        var value = apiTypeExpression.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiTypeExpressionApiInlineType(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ApiInlineType;
        var apiInlineType = apiTypeExpression.ApiInlineType;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, apiInlineType, options);
    }

    private static void WriteApiTypeExpressionClrType(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ClrType;
        var clrType = apiTypeExpression.ClrType;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, clrType, options, _typeJsonConverter);
    }
    #endregion
}
