// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Key.Json;

/// <summary>
///     JSON converter for <see cref="ApiKey"/> values that supports both scalar and composite
///     representations for read and write using System.Text.Json.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiKeyJsonConverter"/> class.
/// </remarks>
/// <param name="logger">The logger used for diagnostics (optional).</param>
public sealed class ApiKeyJsonConverter(ILogger<ApiKeyJsonConverter>? logger) : JsonConverterBase<ApiKey>(logger)
{
    #region Property Types
    private readonly record struct ApiKeyPropertyNames
    {
        #region Immutable Properties
        public required string ApiKind { get; init; }
        public required string ApiParts { get; init; }
        public required string ClrValue { get; init; }
        #endregion
    }

    private readonly record struct ApiKeyPartPropertyNames
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
        public required ApiKeyPropertyNames ApiKey { get; init; }
        public required ApiKeyPartPropertyNames ApiKeyPart { get; init; }
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
                ApiKey = new ApiKeyPropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(ApiKey.ApiKind)),
                    ApiParts = policy.ConvertName(nameof(ApiKey.ApiParts)),
                    ClrValue = policy.ConvertName(nameof(ApiKey.ClrValue))
                },
                ApiKeyPart = new ApiKeyPartPropertyNames
                {
                    ApiName = policy.ConvertName(nameof(ApiKeyPart.ApiName)),
                    ApiKind = policy.ConvertName(nameof(ApiKeyPart.ApiKind)),
                    ClrValue = policy.ConvertName(nameof(ApiKeyPart.ClrValue))
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiKeyReadData
    {
        #region Properties
        public ApiKeyKind? ApiKind { get; set; }
        public List<ApiKeyPartReadData?>? ApiParts { get; set; }

        public long? ClrValueAsInt64 { get; set; }
        public string? ClrValueAsString { get; set; }
        #endregion
    }

    private class ApiKeyPartReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiKeyKind? ApiKind { get; set; }

        public long? ClrValueAsInt64 { get; set; }
        public string? ClrValueAsString { get; set; }
        #endregion
    }

    private class ReadState
    {
        #region Properties
        public ApiKeyReadData? ApiKey { get; set; }
        public ApiKeyPartReadData? ApiKeyPart { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiKey Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>> ApiKeyPropertyHandlers = new()
        {
            // ApiKey Property Handlers
            { propertyNames.ApiKey.ApiKind, HandleApiKeyApiKind },
            { propertyNames.ApiKey.ApiParts, HandleApiKeyApiParts },
            { propertyNames.ApiKey.ClrValue, HandleApiKeyClrValue },
        };
        #endregion

        #region ApiKeyPart Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>> ApiKeyPartPropertyHandlers = new()
        {
            // ApiKeyPart Property Handlers
            { propertyNames.ApiKeyPart.ApiName, HandleApiKeyPartApiName },
            { propertyNames.ApiKeyPart.ApiKind, HandleApiKeyPartApiKind },
            { propertyNames.ApiKeyPart.ClrValue, HandleApiKeyPartClrValue },
        };
        #endregion

        #region ApiKey Methods
        private static void HandleApiKeyApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKey ??= new ApiKeyReadData();

            var options = context.Options;
            context.ReadData.ApiKey.ApiKind = _apiKeyKindJsonConverter.Read(ref reader, typeof(ApiKeyKind), options);
        }

        private static void HandleApiKeyApiParts(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKey ??= new ApiKeyReadData();
            context.ReadData.ApiKey.ApiParts ??= [];

            // The context argument is intentionally discarded:
            // The same fixed handler is used for every element regardless of context; the handler itself receives the context at invocation time.
            ReadJsonArray(ref reader, context, static _ => HandleApiKeyApiPartsArrayItem);
        }

        private static void HandleApiKeyApiPartsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPart = null;
            ReadJsonObject(ref reader, context, context.ReadHandlers.ApiKeyPartPropertyHandlers);
            var apiKeyPartReadData = context.ReadData.ApiKeyPart;

            context.ReadData.ApiKey!.ApiParts!.Add(apiKeyPartReadData);
        }

        private static void HandleApiKeyClrValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKey ??= new ApiKeyReadData();
            var propertyName = context.PropertyNames.ApiKey.ClrValue;

            var (clrValueAsInt64, clrValueAsString) = ReadClrValue(ref reader, propertyName);

            context.ReadData.ApiKey.ClrValueAsInt64 = clrValueAsInt64;
            context.ReadData.ApiKey.ClrValueAsString = clrValueAsString;
        }
        #endregion

        #region ApiKeyPart Methods
        private static void HandleApiKeyPartApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPart ??= new ApiKeyPartReadData();

            context.ReadData.ApiKeyPart.ApiName = reader.GetString();
        }

        private static void HandleApiKeyPartApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPart ??= new ApiKeyPartReadData();

            var options = context.Options;
            context.ReadData.ApiKeyPart.ApiKind = _apiKeyKindJsonConverter.Read(ref reader, typeof(ApiKeyKind), options);
        }

        private static void HandleApiKeyPartClrValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPart ??= new ApiKeyPartReadData();
            var propertyName = context.PropertyNames.ApiKeyPart.ClrValue;

            var (clrValueAsInt64, clrValueAsString) = ReadClrValue(ref reader, propertyName);

            context.ReadData.ApiKeyPart.ClrValueAsInt64 = clrValueAsInt64;
            context.ReadData.ApiKeyPart.ClrValueAsString = clrValueAsString;
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
    private static readonly EnumJsonConverter<ApiKeyKind> _apiKeyKindJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiKeyJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    /// <inheritdoc/>
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultReadContext<PropertyNames, ReadState, ReadHandlers>
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
    protected override ApiKey CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var propertyNames = readContext.PropertyNames;
        var readState = readContext.ReadData;

        // Determine ApiKey kind
        var nullableApiKind = readState.ApiKey?.ApiKind;
        if (nullableApiKind is null)
        {
            var propertyName = propertyNames.ApiKey.ApiKind;
            throw new JsonException($"Missing required property: {propertyName}.");
        }
        var apiKind = nullableApiKind.Value;

        return apiKind switch
        {
            ApiKeyKind.Empty => CreateEmpty(),
            ApiKeyKind.Composite => CreateComposite(propertyNames, readState),
            _ => CreateScalar(apiKind, propertyNames, readState)
        };
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.ApiKeyPropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiKey apiKey, IWriteContext context)
    {
        if (!apiKey.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        var writeContext = (DefaultWriteContext<PropertyNames>)context;
        WriteJsonObject(writer, () =>
        {
            WriteApiKeyApiKind(writer, apiKey, writeContext);

            if (apiKey.IsComposite)
            {
                WriteApiKeyApiParts(writer, apiKey, writeContext);
            }
            else
            {
                WriteApiKeyClrValue(writer, apiKey, writeContext);
            }
        });
    }
    #endregion

    #region Create Implementation Methods
    private static ApiKey CreateEmpty()
    {
        return ApiKey.Empty;
    }

    private static ApiKey CreateComposite(PropertyNames propertyNames, ReadState readState)
    {
        var nullableOrEmptyApiParts = readState.ApiKey?.ApiParts;
        if (nullableOrEmptyApiParts is null || nullableOrEmptyApiParts.Count == 0)
        {
            var propertyName = propertyNames.ApiKey.ApiParts;
            throw new JsonException($"Composite {nameof(ApiKey)} requires non-empty array property: {propertyName}.");
        }

        var apiKeyPartsCount = nullableOrEmptyApiParts.Count;
        var apiKeyParts = new List<ApiKeyPart>(apiKeyPartsCount);
        for (var index = 0; index < apiKeyPartsCount; index++)
        {
            var apiPart = nullableOrEmptyApiParts[index];
            if (apiPart is null)
            {
                var propertyName = propertyNames.ApiKey.ApiParts;
                throw new JsonException($"Null composite {nameof(ApiKey)} part at index {index} in property: {propertyName}.");
            }

            // Get part name (not required)
            var apiName = apiPart.ApiName;

            // Get and validate part kind (required)
            var nullableApiKind = apiPart.ApiKind;
            if (nullableApiKind is null)
            {
                var propertyName = propertyNames.ApiKeyPart.ApiKind;
                throw new JsonException($"Missing required property: {propertyName} for composite part at index {index}.");
            }
            var apiKind = nullableApiKind.Value;

            // Get and validate part value (required)
            var clrValuePropertyName = propertyNames.ApiKeyPart.ClrValue;
            var clrValueAsInt64 = apiPart.ClrValueAsInt64;
            var clrValueAsString = apiPart.ClrValueAsString;

            var apiKey = CreateScalarCore
            (
                apiKind,
                clrValuePropertyName,
                clrValueAsInt64,
                clrValueAsString
            );

            // Create and add composite part to collection
            var apiKeyPart = new ApiKeyPart(apiName, apiKey);
            apiKeyParts.Add(apiKeyPart);
        }

        // Create and return composite ApiKey
        return ApiKey.Composite(apiKeyParts);
    }

    private static ApiKey CreateScalar(ApiKeyKind apiKind, PropertyNames propertyNames, ReadState readState)
    {
        var clrValuePropertyName = propertyNames.ApiKey.ClrValue;
        var clrValueAsInt64 = readState.ApiKey?.ClrValueAsInt64;
        var clrValueAsString = readState.ApiKey?.ClrValueAsString;

        var apiKey = CreateScalarCore
        (
            apiKind,
            clrValuePropertyName,
            clrValueAsInt64,
            clrValueAsString
        );
        return apiKey;
    }

    private static ApiKey CreateScalarCore
    (
        ApiKeyKind apiKind,
        string clrValuePropertyName,
        long? clrValueAsInt64,
        string? clrValueAsString)
    {
        switch (apiKind)
        {
            case ApiKeyKind.Empty:
            case ApiKeyKind.Composite:
                throw new JsonException($"ApiKeyKind.{apiKind} is not valid as a scalar value in property: {clrValuePropertyName}.");

            case ApiKeyKind.Int32:
            case ApiKeyKind.Int64:
                {
                    var nullableInt64 = clrValueAsInt64 ?? throw new JsonException($"Missing required numeric value for {apiKind} in property: {clrValuePropertyName}.");
                    var int64 = nullableInt64;

                    if (apiKind == ApiKeyKind.Int64)
                    {
                        // Create and return Int64 ApiKey
                        var apiKey = ApiKey.FromInt64(int64);
                        return apiKey;
                    }
                    else
                    {
                        // For Int32 kind, we still validate the range and throw if it's out of bounds, even if it was provided as Int64.
                        if (int64 < int.MinValue || int64 > int.MaxValue)
                        {
                            throw new JsonException($"Numeric value for {apiKind} in property {clrValuePropertyName} is out of range for Int32: {int64}.");
                        }
                        var int32 = (int)int64;

                        // Create and return Int32 ApiKey
                        var apiKey = ApiKey.FromInt32(int32);
                        return apiKey;
                    }
                }

            default:
                {
                    var text = clrValueAsString;
                    if (!ApiKey.TryParse(apiKind, text, out var apiKey))
                    {
                        throw new JsonException($"Value '{text}' is not a valid {apiKind} for property: {clrValuePropertyName}.");
                    }

                    return apiKey;
                }
        }
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKeyApiKind(Utf8JsonWriter writer, ApiKey apiKey, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKey.ApiKind;
        var value = apiKey.ApiKind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _apiKeyKindJsonConverter);
    }

    private static void WriteApiKeyApiParts(Utf8JsonWriter writer, ApiKey apiKey, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKey.ApiParts;
        var apiParts = apiKey.PartsAsSpan;
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
                        WriteApiKeyPartApiName(writer, item, context);
                        WriteApiKeyPartApiKind(writer, item, context);
                        WriteApiKeyPartClrValue(writer, item, context);
                    });
                }
            )
        );
    }

    private static void WriteApiKeyClrValue(Utf8JsonWriter writer, ApiKey apiKey, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKey.ClrValue;
        var options = context.Options;

        WriteClrValueCore(writer, propertyName, apiKey, options);
    }

    private static void WriteApiKeyPartApiKind(Utf8JsonWriter writer, ApiKeyPart apiKeyPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyPart.ApiKind;
        var value = apiKeyPart.ApiValue.ApiKind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _apiKeyKindJsonConverter);
    }

    private static void WriteApiKeyPartApiName(Utf8JsonWriter writer, ApiKeyPart apiKeyPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyPart.ApiName;
        var value = apiKeyPart.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiKeyPartClrValue(Utf8JsonWriter writer, ApiKeyPart apiKeyPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyPart.ClrValue;
        var options = context.Options;
        var apiKey = apiKeyPart.ApiValue;

        WriteClrValueCore(writer, propertyName, apiKey, options);
    }

    private static void WriteClrValueCore(Utf8JsonWriter writer, string propertyName, ApiKey apiKey, JsonSerializerOptions options)
    {
        // Fast-path numeric kinds: write as JSON number instead of string.
        switch (apiKey.ApiKind)
        {
            case ApiKeyKind.Int32:
                {
                    var i32 = apiKey.AsInt32OrThrow();
                    writer.TryWritePropertyAsNumber(propertyName, i32, options);
                    return;
                }
            case ApiKeyKind.Int64:
                {
                    var i64 = apiKey.AsInt64OrThrow();
                    writer.TryWritePropertyAsNumber(propertyName, i64, options);
                    return;
                }
        }

        // All other kinds (or fallback): write as string using existing extension for consistency
        writer.TryWritePropertyAsString(propertyName, apiKey.ToString(), options);
    }
    #endregion
}
