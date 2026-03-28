// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;
using System.Text.Json;

using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Identity.Json;

/// <summary>
///     JSON converter for <see cref="ApiId"/> values that supports both scalar and composite
///     representations for read and write using System.Text.Json.
/// </summary>
/// <remarks>
///     This converter integrates with the Evoogle JSON infrastructure and writes/reads an object with
///     two properties: a <c>kind</c> enum and a <c>value</c> that is either a string (scalar) or an array
///     of parts (composite). Null values are written as JSON <c>null</c>.
/// </remarks>
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
        public string? ClrValue { get; set; }
        #endregion
    }

    private class ApiIdPartReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiIdKind? ApiKind { get; set; }
        public string? ClrValue { get; set; }
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
            var propertyName = context.PropertyNames.ApiIdPart.ClrValue;
            var apiKind = context.ReadData.ApiId.ApiKind;
            var apiKindKnown = apiKind is not null;

            if (apiKindKnown)
            {
                context.ReadData.ApiId.ClrValue = ReadClrValueKnownKind(ref reader, apiKind!.Value, propertyName);
                return;
            }

            context.ReadData.ApiId.ClrValue = ReadClrValueUnknownKind(ref reader, propertyName);
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
            var apiKind = context.ReadData.ApiIdPart.ApiKind;
            var apiKindKnown = apiKind is not null;

            if (apiKindKnown)
            {
                context.ReadData.ApiIdPart.ClrValue = ReadClrValueKnownKind(ref reader, apiKind!.Value, propertyName);
                return;
            }

            context.ReadData.ApiIdPart.ClrValue = ReadClrValueUnknownKind(ref reader, propertyName);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        ///     Reads a scalar JSON token and normalizes it to an invariant string when the ApiIdKind is known.
        ///     Enforces token expectations (e.g. forbids numeric for Guid/Ulid/Culture) and range checks for integers.
        /// </summary>
        private static string ReadClrValueKnownKind(ref Utf8JsonReader reader, ApiIdKind kind, string propertyName)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString()!;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                switch (kind)
                {
                    case ApiIdKind.Int32:
                        if (!reader.TryGetInt64(out var i64For32))
                        {
                            throw new JsonException($"Numeric value for {propertyName} could not be read as integer.");
                        }
                        if (i64For32 < int.MinValue || i64For32 > int.MaxValue)
                        {
                            throw new JsonException($"Numeric value for {propertyName} is out of range for Int32: {i64For32}.");
                        }
                        return ((int)i64For32).ToString(CultureInfo.InvariantCulture);

                    case ApiIdKind.Int64:
                        if (!reader.TryGetInt64(out var i64))
                        {
                            throw new JsonException($"Numeric value for {propertyName} could not be read as Int64.");
                        }
                        return i64.ToString(CultureInfo.InvariantCulture);

                    case ApiIdKind.String:
                        if (!reader.TryGetInt64(out var numberAsText))
                        {
                            throw new JsonException($"Numeric value for {propertyName} could not be read as integer for String kind.");
                        }
                        return numberAsText.ToString(CultureInfo.InvariantCulture);

                    default:
                        throw new JsonException($"Invalid numeric token for {propertyName} with kind {kind}. Expected string.");
                }
            }

            throw new JsonException($"Invalid token type for {propertyName} with kind {kind}: {reader.TokenType}.");
        }

        /// <summary>
        ///     Reads a scalar JSON token and normalizes it to an invariant string when the ApiIdKind is not yet known.
        ///     Accepts string or integer number tokens only.
        /// </summary>
        private static string ReadClrValueUnknownKind(ref Utf8JsonReader reader, string propertyName)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString()!;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (!reader.TryGetInt64(out var number))
                {
                    throw new JsonException($"Numeric value for {propertyName} is not an integer.");
                }
                return number.ToString(CultureInfo.InvariantCulture);
            }

            throw new JsonException($"Invalid token type for {propertyName} : {reader.TokenType}.");
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

    private static ApiId CreateScalar(ApiIdKind apiKind, PropertyNames propertyNames, ReadData readData)
    {
        var clrValueAsString = readData.ApiId?.ClrValue;
        if (!ApiId.TryParse(apiKind, clrValueAsString, out var apiId))
        {
            var propertyName = propertyNames.ApiId.ClrValue;
            throw new JsonException($"Value '{clrValueAsString}' is not a valid {apiKind} for property: {propertyName}.");
        }

        // Create and return scalar ApiId
        return apiId;
    }

    private static ApiId CreateComposite(PropertyNames propertyNames, ReadData readData)
    {
        var nullableOrEmptyApiParts = readData.ApiId?.ApiParts;
        if (nullableOrEmptyApiParts is null || nullableOrEmptyApiParts.Count == 0)
        {
            var propertyName = propertyNames.ApiId.ApiParts;
            throw new JsonException($"Composite ApiId requires non-empty array property: {propertyName}.");
        }

        var apiIdPartsCount = nullableOrEmptyApiParts.Count;
        var apiIdParts = new List<ApiIdPart>(apiIdPartsCount);
        for (var index = 0; index < apiIdPartsCount; index++)
        {
            var apiPart = nullableOrEmptyApiParts[index];
            if (apiPart is null)
            {
                var propertyName = propertyNames.ApiId.ApiParts;
                throw new JsonException($"Null composite ApiId part at index {index} in property: {propertyName}.");
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
            var clrValueAsString = apiPart.ClrValue;
            if (!ApiId.TryParse(apiKind, clrValueAsString, out var apiId))
            {
                var propertyName = propertyNames.ApiIdPart.ClrValue;
                throw new JsonException($"Value '{clrValueAsString}' is not a valid {apiKind} for property: {propertyName} of composite part at index {index}.");
            }

            // Create and add composite part to collection
            var apiIdPart = new ApiIdPart(apiName, apiId);
            apiIdParts.Add(apiIdPart);
        }

        // Create and return composite ApiId
        return ApiId.Composite(apiIdParts);
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
