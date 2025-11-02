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
///     Handles JSON serialization for <see cref="ApiEnumValue"/> instances, including support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiEnumValueJsonConverter(ILogger<ApiEnumValueJsonConverter>? logger) : JsonConverterBase<ApiEnumValue>(logger)
{
    #region Context Types
    /// <summary>
    ///     Represents the shared state required by read and write operations while converting <see cref="ApiEnumValue"/> objects.
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
    ///     Collects intermediate data while reading an <see cref="ApiEnumValue"/> from JSON.
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
    ///     Supplies contextual information while writing an <see cref="ApiEnumValue"/> to JSON.
    /// </summary>
    private class WriteContext(ILogger logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames), IWriteContext
    {
    }
    #endregion

    #region Property Types
    /// <summary>
    ///     Stores the resolved property names for enum value members under a given naming policy.
    /// </summary>
    private readonly record struct ApiEnumValuePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ClrName { get; init; }
        public required string ClrOrdinal { get; init; }
        #endregion
    }

    /// <summary>
    ///     Combines property name metadata used by the converter while reading or writing values.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiEnumValuePropertyNames ApiEnumValue { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used when deserializing individual enum value members.
    /// </summary>
    private class ApiEnumValueReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public string? ClrName { get; set; }
        public int? ClrOrdinal { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the fully parsed data required to construct an <see cref="ApiEnumValue"/>.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiEnumValueReadData ApiEnumValue { get; } = new();
        #endregion
    }

    /// <summary>
    ///     Maps JSON property names to handlers that populate <see cref="ReadData"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiEnumValue Fields
        public readonly Dictionary<string, JsonReaderHandler<ReadContext>> ApiEnumValuePropertyHandlers = new()
        {
            // ApiEnumValue Property Handlers
            { propertyNames.ApiEnumValue.ApiName, HandleApiEnumValueApiName },
            { propertyNames.ApiEnumValue.ClrName, HandleApiEnumValueClrName },
            { propertyNames.ApiEnumValue.ClrOrdinal, HandleApiEnumValueClrOrdinal },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, (ref Utf8JsonReader reader, ReadContext context) =>
                context.ReadData.Extensions = ReadJsonExtensionsObject(ref reader, context) },
        };
        #endregion

        #region ApiEnumValue Methods
        private static void HandleApiEnumValueApiName(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiEnumValue.ApiName = reader.GetString();
        }

        private static void HandleApiEnumValueClrName(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiEnumValue.ClrName = reader.GetString();
        }

        private static void HandleApiEnumValueClrOrdinal(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiEnumValue.ClrOrdinal = reader.GetInt32();
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
    public ApiEnumValueJsonConverter()
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

    protected override ApiEnumValue? CreateValue(IReadContext context)
    {
        var readContext = (ReadContext)context;

        var apiEnumValueReadData = readContext.ReadData.ApiEnumValue;
        var apiEnumValue = CreateApiEnumValue(apiEnumValueReadData);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiEnumValue, extensions);

        return apiEnumValue;
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
        ReadJsonObject(ref reader, readContext, (readContext) => readContext.ReadHandlers.ApiEnumValuePropertyHandlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiEnumValue value, IWriteContext context)
    {
        var writeContext = (WriteContext)context;
        WriteApiEnumValue(writer, value, writeContext);
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

        WriteExtensibleBaseExtensions(writer, context.PropertyNames.ExtensibleBase.Extensions, apiEnumValue, context);

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
