// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
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
    #region Property Types
    /// <summary>
    ///     Caches the JSON property names used to represent a type expression for the active naming policy.
    /// </summary>
    private readonly record struct ApiTypeExpressionPropertyNames
    {
        #region Immutable Properties
        public required string ApiInlineType { get; init; }
        public required string ApiKind { get; init; }
        public required string ApiName { get; init; }
        public required string ClrType { get; init; }
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

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiTypeExpression = new ApiTypeExpressionPropertyNames
                {
                    ApiInlineType = policy.ConvertName(nameof(Schema.ApiTypeExpression.ApiInlineType)),
                    ApiKind = policy.ConvertName(nameof(Schema.ApiTypeExpression.ApiKind)),
                    ApiName = policy.ConvertName(nameof(Schema.ApiTypeExpression.ApiName)),
                    ClrType = policy.ConvertName(nameof(Schema.ApiTypeExpression.ClrType)),
                }
            };
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
        public ApiTypeKind? ApiKind { get; set; }
        public string? ApiName { get; set; }
        public Type? ClrType { get; set; }
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
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiTypeExpression.ApiInlineType, HandleApiTypeExpressionApiInlineType },
            { propertyNames.ApiTypeExpression.ApiKind, HandleApiTypeExpressionApiKind },
            { propertyNames.ApiTypeExpression.ApiName, HandleApiTypeExpressionApiName },
            { propertyNames.ApiTypeExpression.ClrType, HandleApiTypeExpressionClrType },
        };
        #endregion

        #region ApiTypeExpression Methods
        private static void HandleApiTypeExpressionApiInlineType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ApiInlineType = JsonSerializer.Deserialize<ApiType>(ref reader, context.Options);
        }

        private static void HandleApiTypeExpressionApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            var options = context.Options;
            context.ReadData.ApiTypeExpression.ApiKind = _apiTypeKindJsonConverter.Read(ref reader, typeof(ApiTypeKind), options);
        }

        private static void HandleApiTypeExpressionApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ApiName = reader.GetString();
        }

        private static void HandleApiTypeExpressionClrType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiTypeExpression ??= new ApiTypeExpressionReadData();

            context.ReadData.ApiTypeExpression.ClrType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeKind> _apiTypeKindJsonConverter = new();

    private static readonly TypeJsonConverter _typeJsonConverter = new();
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
        => CreateDefaultReadContext<PropertyNames, ReadData, ReadHandlers>
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create,
                buildReadHandlers: names => new ReadHandlers(names)
            );

    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create
            );

    protected override ApiTypeExpression? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiTypeExpression;

        var apiInlineType = readData?.ApiInlineType;
        if (apiInlineType is not null)
        {
            return new ApiTypeExpression(apiInlineType);
        }

        var apiKind = readData?.ApiKind;
        var apiName = readData?.ApiName;
        var clrType = readData?.ClrType;

        return new ApiTypeExpression(apiKind, apiName, clrType);
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiTypeExpression value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiTypeExpressionApiKind(writer, value, writeContext);
            WriteApiTypeExpressionApiName(writer, value, writeContext);
            WriteApiTypeExpressionApiInlineType(writer, value, writeContext);
            WriteApiTypeExpressionClrType(writer, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiTypeExpressionApiKind(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ApiKind;
        var kind = apiTypeExpression.ApiKind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, kind, options, _apiTypeKindJsonConverter);
    }

    private static void WriteApiTypeExpressionApiName(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ApiName;
        var value = apiTypeExpression.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiTypeExpressionApiInlineType(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ApiInlineType;
        var apiInlineType = apiTypeExpression.ApiInlineType;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, apiInlineType, options);
    }

    private static void WriteApiTypeExpressionClrType(Utf8JsonWriter writer, ApiTypeExpression apiTypeExpression, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiTypeExpression.ClrType;
        var clrType = apiTypeExpression.ClrType;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, clrType, options, _typeJsonConverter);
    }
    #endregion
}
