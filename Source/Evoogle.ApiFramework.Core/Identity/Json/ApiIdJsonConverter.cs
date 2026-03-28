// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Identity.Json;

/// <summary>
///     JSON converter for <see cref="ApiId"/> values that supports both scalar and composite
///     representations for read and write using System.Text.Json.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiIdJsonConverter"/> class.
/// </remarks>
/// <param name="logger">The logger used for diagnostics (optional).</param>
public sealed class ApiIdJsonConverter(ILogger<ApiIdJsonConverter>? logger) : JsonConverterBase<ApiId>(logger)
{
    #region Property Types
    private readonly record struct ApiIdPropertyNames
    {
        #region Immutable Properties
        public required string ApiKind { get; init; }
        public required string ApiParts { get; init; }
        public required string ClrValue { get; init; }
        #endregion
    }

    private readonly record struct ApiIdPartPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiKind { get; init; }
        public required string ClrValue { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdPropertyNames ApiId { get; init; }
        public required ApiIdPartPropertyNames ApiIdPart { get; init; }
        #endregion

        #region Factory Methods
        /// <summary>
        ///     Creates a <see cref="PropertyNames"/> instance using the provided <paramref name="policy"/> to convert CLR member names to JSON property names.
        /// </summary>
        /// <param name="policy">The naming policy to apply when generating property names.</param>
        /// <returns>A new <see cref="PropertyNames"/> populated with converted names.</returns>
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiId = new ApiIdPropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(ApiId.ApiKind)),
                    ApiParts = policy.ConvertName(nameof(ApiId.ApiParts)),
                    ClrValue = policy.ConvertName(nameof(ApiId.ClrValue))
                },
                ApiIdPart = new ApiIdPartPropertyNames
                {
                    ApiName = policy.ConvertName(nameof(ApiIdPart.ApiName)),
                    ApiKind = policy.ConvertName(nameof(ApiIdPart.ApiKind)),
                    ClrValue = policy.ConvertName(nameof(ApiIdPart.ClrValue))
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiIdReadData
    {
        #region Properties
        public ApiIdKind? ApiKind { get; set; }
        public List<ApiIdPartReadData?>? ApiParts { get; set; }

        public long? ClrValueAsInt64 { get; set; }
        public string? ClrValueAsString { get; set; }
        #endregion
    }

    private class ApiIdPartReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiIdKind? ApiKind { get; set; }

        public long? ClrValueAsInt64 { get; set; }
        public string? ClrValueAsString { get; set; }
        #endregion
    }

    private class ReadData
    {
        #region Properties
        public ApiIdReadData? ApiId { get; set; }
        public ApiIdPartReadData? ApiIdPart { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiId Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> ApiIdPropertyHandlers = new()
        {
            // ApiId Property Handlers
            { propertyNames.ApiId.ApiKind, HandleApiIdApiKind },
            { propertyNames.ApiId.ApiParts, HandleApiIdApiParts },
            { propertyNames.ApiId.ClrValue, HandleApiIdClrValue },
        };
        #endregion

        #region ApiIdPart Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> ApiIdPartPropertyHandlers = new()
        {
            // ApiIdPart Property Handlers
            { propertyNames.ApiIdPart.ApiName, HandleApiIdPartApiName },
            { propertyNames.ApiIdPart.ApiKind, HandleApiIdPartApiKind },
            { propertyNames.ApiIdPart.ClrValue, HandleApiIdPartClrValue },
        };
        #endregion

        #region ApiId Methods
        private static void HandleApiIdApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiId ??= new ApiIdReadData();

            var options = context.Options;
            context.ReadData.ApiId.ApiKind = _apiIdKindJsonConverter.Read(ref reader, typeof(ApiIdKind), options);
        }

        private static void HandleApiIdApiParts(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiId ??= new ApiIdReadData();
            context.ReadData.ApiId.ApiParts ??= [];

            // The context argument is intentionally discarded:
            // The same fixed handler is used for every element regardless of context; the handler itself receives the context at invocation time.
            ReadJsonArray(ref reader, context, static _ => HandleApiIdApiPartsArrayItem);
        }

        private static void HandleApiIdApiPartsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdPart = null;
            ReadJsonObject(ref reader, context, context.ReadHandlers.ApiIdPartPropertyHandlers);
            var apiIdPartReadData = context.ReadData.ApiIdPart;

            context.ReadData.ApiId!.ApiParts!.Add(apiIdPartReadData);
        }

        private static void HandleApiIdClrValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiId ??= new ApiIdReadData();
            var propertyName = context.PropertyNames.ApiId.ClrValue;

            var (clrValueAsInt64, clrValueAsString) = ReadClrValue(ref reader, propertyName);

            context.ReadData.ApiId.ClrValueAsInt64 = clrValueAsInt64;
            context.ReadData.ApiId.ClrValueAsString = clrValueAsString;
        }
        #endregion

        #region ApiIdPart Methods
        private static void HandleApiIdPartApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdPart ??= new ApiIdPartReadData();

            context.ReadData.ApiIdPart.ApiName = reader.GetString();
        }

        private static void HandleApiIdPartApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdPart ??= new ApiIdPartReadData();

            var options = context.Options;
            context.ReadData.ApiIdPart.ApiKind = _apiIdKindJsonConverter.Read(ref reader, typeof(ApiIdKind), options);
        }

        private static void HandleApiIdPartClrValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdPart ??= new ApiIdPartReadData();
            var propertyName = context.PropertyNames.ApiIdPart.ClrValue;

            var (clrValueAsInt64, clrValueAsString) = ReadClrValue(ref reader, propertyName);

            context.ReadData.ApiIdPart.ClrValueAsInt64 = clrValueAsInt64;
            context.ReadData.ApiIdPart.ClrValueAsString = clrValueAsString;
        }
        #endregion

        #region Helper Methods
        private static (long? ClrValueAsInt64, string? ClrValueAsString) ReadClrValue(ref Utf8JsonReader reader, string propertyName)
        {
            var jsonTokenType = reader.TokenType;

            if (jsonTokenType == JsonTokenType.Number)
            {
                if (!reader.TryGetInt64(out var clrValueAsInt64))
                {
                    throw new JsonException($"Numeric value for {propertyName} could not be read as integer.");
                }

                return (clrValueAsInt64, null);
            }

            if (jsonTokenType == JsonTokenType.String)
            {
                var clrValueAsString = reader.GetString();
                return (null, clrValueAsString);
            }

            throw new JsonException($"Unexpected JSON token type for {propertyName}: {jsonTokenType}. Expected number or string.");
        }
        #endregion
    }
    #endregion

    #region Fields
    /// <summary>Cached enum converter used to read and write <see cref="ApiIdKind"/> values with current options.</summary>
    private static readonly EnumJsonConverter<ApiIdKind> _apiIdKindJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    /// <inheritdoc/>
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultReadContext<PropertyNames, ReadData, ReadHandlers>
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create,
                buildReadHandlers: names => new ReadHandlers(names)
            );

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create
            );

    /// <inheritdoc/>
    protected override ApiId CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var propertyNames = readContext.PropertyNames;
        var readData = readContext.ReadData;

        // Determine ApiId kind
        var nullableApiKind = readData.ApiId?.ApiKind;
        if (nullableApiKind is null)
        {
            var propertyName = propertyNames.ApiId.ApiKind;
            throw new JsonException($"Missing required property: {propertyName}.");
        }
        var apiKind = nullableApiKind.Value;

        return apiKind switch
        {
            ApiIdKind.Empty => CreateEmpty(),
            ApiIdKind.Composite => CreateComposite(propertyNames, readData),
            _ => CreateScalar(apiKind, propertyNames, readData)
        };
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.ApiIdPropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiId apiId, IWriteContext context)
    {
        if (!apiId.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        var writeContext = (DefaultWriteContext<PropertyNames>)context;
        WriteJsonObject(writer, () =>
        {
            WriteApiIdApiKind(writer, apiId, writeContext);

            if (apiId.IsComposite)
            {
                WriteApiIdApiParts(writer, apiId, writeContext);
            }
            else
            {
                WriteApiIdClrValue(writer, apiId, writeContext);
            }
        });
    }
    #endregion

    #region Create Implementation Methods
    private static ApiId CreateEmpty()
    {
        return ApiId.Empty;
    }

    private static ApiId CreateComposite(PropertyNames propertyNames, ReadData readData)
    {
        var nullableOrEmptyApiParts = readData.ApiId?.ApiParts;
        if (nullableOrEmptyApiParts is null || nullableOrEmptyApiParts.Count == 0)
        {
            var propertyName = propertyNames.ApiId.ApiParts;
            throw new JsonException($"Composite {nameof(ApiId)} requires non-empty array property: {propertyName}.");
        }

        var apiIdPartsCount = nullableOrEmptyApiParts.Count;
        var apiIdParts = new List<ApiIdPart>(apiIdPartsCount);
        for (var index = 0; index < apiIdPartsCount; index++)
        {
            var apiPart = nullableOrEmptyApiParts[index];
            if (apiPart is null)
            {
                var propertyName = propertyNames.ApiId.ApiParts;
                throw new JsonException($"Null composite {nameof(ApiId)} part at index {index} in property: {propertyName}.");
            }

            // Get part name (not required)
            var apiName = apiPart.ApiName;

            // Get and validate part kind (required)
            var nullableApiKind = apiPart.ApiKind;
            if (nullableApiKind is null)
            {
                var propertyName = propertyNames.ApiIdPart.ApiKind;
                throw new JsonException($"Missing required property: {propertyName} for composite part at index {index}.");
            }
            var apiKind = nullableApiKind.Value;

            // Get and validate part value (required)
            var clrValuePropertyName = propertyNames.ApiIdPart.ClrValue;
            var clrValueAsInt64 = apiPart.ClrValueAsInt64;
            var clrValueAsString = apiPart.ClrValueAsString;

            var apiId = CreateScalarCore
            (
                apiKind,
                clrValuePropertyName,
                clrValueAsInt64,
                clrValueAsString
            );

            // Create and add composite part to collection
            var apiIdPart = new ApiIdPart(apiName, apiId);
            apiIdParts.Add(apiIdPart);
        }

        // Create and return composite ApiId
        return ApiId.Composite(apiIdParts);
    }

    private static ApiId CreateScalar(ApiIdKind apiKind, PropertyNames propertyNames, ReadData readData)
    {
        var clrValuePropertyName = propertyNames.ApiId.ClrValue;
        var clrValueAsInt64 = readData.ApiId?.ClrValueAsInt64;
        var clrValueAsString = readData.ApiId?.ClrValueAsString;

        var apiId = CreateScalarCore
        (
            apiKind,
            clrValuePropertyName,
            clrValueAsInt64,
            clrValueAsString
        );
        return apiId;
    }

    private static ApiId CreateScalarCore
    (
        ApiIdKind apiKind,
        string clrValuePropertyName,
        long? clrValueAsInt64,
        string? clrValueAsString)
    {
        switch (apiKind)
        {
            case ApiIdKind.Empty:
            case ApiIdKind.Composite:
                throw new JsonException($"ApiIdKind.{apiKind} is not valid as a scalar value in property: {clrValuePropertyName}.");

            case ApiIdKind.Int32:
            case ApiIdKind.Int64:
                {
                    var nullableInt64 = clrValueAsInt64 ?? throw new JsonException($"Missing required numeric value for {apiKind} in property: {clrValuePropertyName}.");
                    var int64 = nullableInt64;

                    if (apiKind == ApiIdKind.Int64)
                    {
                        // Create and return Int64 ApiId
                        var apiId = ApiId.FromInt64(int64);
                        return apiId;
                    }
                    else
                    {
                        // For Int32 kind, we still validate the range and throw if it's out of bounds, even if it was provided as Int64.
                        if (int64 < int.MinValue || int64 > int.MaxValue)
                        {
                            throw new JsonException($"Numeric value for {apiKind} in property {clrValuePropertyName} is out of range for Int32: {int64}.");
                        }
                        var int32 = (int)int64;

                        // Create and return Int32 ApiId
                        var apiId = ApiId.FromInt32(int32);
                        return apiId;
                    }
                }

            default:
                {
                    var text = clrValueAsString;
                    if (!ApiId.TryParse(apiKind, text, out var apiId))
                    {
                        throw new JsonException($"Value '{text}' is not a valid {apiKind} for property: {clrValuePropertyName}.");
                    }

                    return apiId;
                }
        }
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiIdApiKind(Utf8JsonWriter writer, ApiId apiId, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiId.ApiKind;
        var value = apiId.ApiKind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _apiIdKindJsonConverter);
    }

    private static void WriteApiIdApiParts(Utf8JsonWriter writer, ApiId apiId, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiId.ApiParts;
        var apiParts = apiId.PartsAsSpan;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiParts,
            options,
            collection => WriteJsonArray
            (
                writer,
                collection,
                item =>
                {
                    WriteJsonObject(writer, () =>
                    {
                        WriteApiIdPartApiName(writer, item, context);
                        WriteApiIdPartApiKind(writer, item, context);
                        WriteApiIdPartClrValue(writer, item, context);
                    });
                }
            )
        );
    }

    private static void WriteApiIdClrValue(Utf8JsonWriter writer, ApiId apiId, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiId.ClrValue;
        var options = context.Options;

        WriteClrValueCore(writer, propertyName, apiId, options);
    }

    private static void WriteApiIdPartApiKind(Utf8JsonWriter writer, ApiIdPart apiIdPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdPart.ApiKind;
        var value = apiIdPart.ApiValue.ApiKind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _apiIdKindJsonConverter);
    }

    private static void WriteApiIdPartApiName(Utf8JsonWriter writer, ApiIdPart apiIdPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdPart.ApiName;
        var value = apiIdPart.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdPartClrValue(Utf8JsonWriter writer, ApiIdPart apiIdPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdPart.ClrValue;
        var options = context.Options;
        var apiId = apiIdPart.ApiValue;

        WriteClrValueCore(writer, propertyName, apiId, options);
    }

    private static void WriteClrValueCore(Utf8JsonWriter writer, string propertyName, ApiId apiId, JsonSerializerOptions options)
    {
        // Fast-path numeric kinds: write as JSON number instead of string.
        switch (apiId.ApiKind)
        {
            case ApiIdKind.Int32:
                {
                    var i32 = apiId.AsInt32OrThrow();
                    writer.TryWritePropertyAsNumber(propertyName, i32, options);
                    return;
                }
            case ApiIdKind.Int64:
                {
                    var i64 = apiId.AsInt64OrThrow();
                    writer.TryWritePropertyAsNumber(propertyName, i64, options);
                    return;
                }
        }

        // All other kinds (or fallback): write as string using existing extension for consistency
        writer.TryWritePropertyAsString(propertyName, apiId.ToString(), options);
    }
    #endregion
}
