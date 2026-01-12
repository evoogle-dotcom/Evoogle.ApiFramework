// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Handles JSON serialization for <see cref="ApiEnumValue"/> instances, including support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiEnumValueJsonConverter(ILogger<ApiEnumValueJsonConverter>? logger) : JsonConverterBase<ApiEnumValue>(logger)
{
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

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiEnumValue = new ApiEnumValuePropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Schema.ApiEnumValue.ApiName)),
                    ClrName = policy.ConvertName(nameof(Schema.ApiEnumValue.ClrName)),
                    ClrOrdinal = policy.ConvertName(nameof(Schema.ApiEnumValue.ClrOrdinal)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
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
        public ApiEnumValueReadData? ApiEnumValue { get; set; }
        #endregion
    }

    /// <summary>
    ///     Maps JSON property names to handlers that populate <see cref="ReadData"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiEnumValue Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiEnumValue Property Handlers
            { propertyNames.ApiEnumValue.ApiName, HandleApiEnumValueApiName },
            { propertyNames.ApiEnumValue.ClrName, HandleApiEnumValueClrName },
            { propertyNames.ApiEnumValue.ClrOrdinal, HandleApiEnumValueClrOrdinal },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiEnumValue Methods
        private static void HandleApiEnumValueApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiEnumValue ??= new ApiEnumValueReadData();

            context.ReadData.ApiEnumValue.ApiName = reader.GetString();
        }

        private static void HandleApiEnumValueClrName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiEnumValue ??= new ApiEnumValueReadData();

            context.ReadData.ApiEnumValue.ClrName = reader.GetString();
        }

        private static void HandleApiEnumValueClrOrdinal(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiEnumValue ??= new ApiEnumValueReadData();

            context.ReadData.ApiEnumValue.ClrOrdinal = reader.GetInt32();
        }
        #endregion
    }
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

    protected override ApiEnumValue? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiEnumValue;

        var apiName = readData?.ApiName;
        var clrName = readData?.ClrName;
        var clrOrdinal = readData?.ClrOrdinal.GetValueOrDefault() ?? 0;

        var apiEnumValue = new ApiEnumValue(apiName!, clrName!, clrOrdinal);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiEnumValue, extensions);

        return apiEnumValue;
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiEnumValue value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiEnumValueApiName(writer, value, writeContext);
            WriteApiEnumValueClrName(writer, value, writeContext);
            WriteApiEnumValueClrOrdinal(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiEnumValueApiName(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ApiName;
        var value = apiEnumValue.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiEnumValueClrName(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrName;
        var value = apiEnumValue.ClrName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiEnumValueClrOrdinal(Utf8JsonWriter writer, ApiEnumValue apiEnumValue, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiEnumValue.ClrOrdinal;
        var value = apiEnumValue.ClrOrdinal;
        var options = context.Options;

        writer.TryWritePropertyAsNumber(propertyName, value, options);
    }
    #endregion
}
