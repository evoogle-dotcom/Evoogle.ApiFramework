// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Converts <see cref="ApiTypeExpression"/> instances to and from JSON, including inline types and references.
/// </summary>
/// <param name="logger">The optional logger that receives diagnostics for serialization operations.</param>
public class ApiTypeExpressionJsonConverter(ILogger<ApiTypeExpressionJsonConverter>? logger) : JsonConverterBase<ApiTypeExpression>(logger)
{
    #region Context Types
    /// <summary>
    ///     Encapsulates the immutable state needed while reading or writing type expressions.
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
    ///     Holds intermediate values while deserializing an <see cref="ApiTypeExpression"/> from JSON.
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
    ///     Provides converter state while writing an <see cref="ApiTypeExpression"/> to JSON.
    /// </summary>
    private class WriteContext(ILogger logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames), IWriteContext
    {
    }
    #endregion

    #region Property Types
    /// <summary>
    ///     Caches the JSON property names used to represent a type expression for the active naming policy.
    /// </summary>
    private readonly record struct ApiTypeExpressionPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiInlineType { get; init; }
        public required string ClrType { get; init; }
        public required string Kind { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates all property names used by the converter for a given naming policy.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiTypeExpressionPropertyNames ApiTypeExpression { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage that holds parsed values prior to creating an <see cref="ApiTypeExpression"/>.
    /// </summary>
    private class ApiTypeExpressionReadData
    {
        #region Properties
        public ApiType? ApiInlineType { get; set; }
        public string? ApiName { get; set; }
        public Type? ClrType { get; set; }
        public string? Kind { get; set; }
        #endregion
    }

    /// <summary>
    ///     Captures the overall data read from JSON for a single type expression instance.
    /// </summary>
    private class ReadData
    {
        #region Properties
        public ApiTypeExpressionReadData? ApiTypeExpression { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides JSON property handlers that populate <see cref="ReadData"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiTypeExpression Fields
        public readonly Dictionary<string, JsonReaderHandler<ReadContext>> ApiTypeExpressionHandlers = new()
        {
            { propertyNames.ApiTypeExpression.ApiInlineType, HandleApiTypeExpressionApiInlineType },
            { propertyNames.ApiTypeExpression.ApiName, HandleApiTypeExpressionApiName },
            { propertyNames.ApiTypeExpression.ClrType, HandleApiTypeExpressionClrType },
            { propertyNames.ApiTypeExpression.Kind, HandleApiTypeExpressionKind },
        };
        #endregion

        #region ApiTypeExpression Methods
        private static void HandleApiTypeExpressionApiInlineType(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ApiInlineType = JsonSerializer.Deserialize<ApiType>(ref reader, context.Options);
        }

        private static void HandleApiTypeExpressionApiName(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ApiName = reader.GetString();
        }

        private static void HandleApiTypeExpressionClrType(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ClrType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }

        private static void HandleApiTypeExpressionKind(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.Kind = reader.GetString();
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeKind> _apiTypeKindJsonConverter = new();

    private static readonly TypeJsonConverter _typeJsonConverter = new();

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> _propertyNamesCache = new();

    // Cache read handlers per naming policy to avoid rebuilding on every call
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> _readHandlersCache = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiTypeExpressionJsonConverter()
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

    protected override ApiTypeExpression? CreateValue(IReadContext context)
    {
        var readContext = (ReadContext)context;
        var apiTypeExpression = CreateApiTypeExpression(readContext);

        return apiTypeExpression;
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
        ReadJsonObject(ref reader, readContext, (readContext) => readContext.ReadHandlers.ApiTypeExpressionHandlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiTypeExpression value, IWriteContext context)
    {
        var writeContext = (WriteContext)context;
        WriteApiTypeExpression(writer, value, writeContext);
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

        var kind = ApiJsonConverterHelpers.GetApiTypeKind(context.Logger, apiTypeExpressionReadData?.Kind);
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
