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
///     Handles JSON serialization for <see cref="ApiIdentityPart"/> instances, including support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiIdentityPartJsonConverter(ILogger<ApiIdentityPartJsonConverter>? logger) : JsonConverterBase<ApiIdentityPart>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved property names for enum value members under a given naming policy.
    /// </summary>
    private readonly record struct ApiIdentityPartPropertyNames
    {
        #region Immutable Properties
        public required string ApiPropertyName { get; init; }
        public required string ClrConfiguredIdType { get; init; }
        #endregion
    }

    /// <summary>
    ///     Combines property name metadata used by the converter while reading or writing values.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdentityPartPropertyNames ApiIdentityPart { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentityPart = new ApiIdentityPartPropertyNames
                {
                    ApiPropertyName = policy.ConvertName(nameof(Schema.ApiIdentityPart.ApiPropertyName)),
                    ClrConfiguredIdType = policy.ConvertName(nameof(Schema.ApiIdentityPart.ClrConfiguredIdType)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used when deserializing individual identity part members.
    /// </summary>
    private class ApiIdentityPartReadData
    {
        #region Properties
        public string? ApiPropertyName { get; set; }
        public Type? ClrConfiguredIdType { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the fully parsed data required to construct an <see cref="ApiIdentityPart"/>.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiIdentityPartReadData? ApiIdentityPart { get; set; }
        #endregion
    }

    /// <summary>
    ///     Maps JSON property names to handlers that populate <see cref="ReadData"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiIdentityPart Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiIdentityPart Property Handlers
            { propertyNames.ApiIdentityPart.ApiPropertyName, HandleApiIdentityPartApiPropertyName },
            { propertyNames.ApiIdentityPart.ClrConfiguredIdType, HandleApiIdentityPartClrConfiguredIdType },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiIdentityPart Methods
        private static void HandleApiIdentityPartApiPropertyName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPart ??= new ApiIdentityPartReadData();

            context.ReadData.ApiIdentityPart.ApiPropertyName = reader.GetString();
        }

        private static void HandleApiIdentityPartClrConfiguredIdType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPart ??= new ApiIdentityPartReadData();

            context.ReadData.ApiIdentityPart.ClrConfiguredIdType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdentityPartJsonConverter()
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

    protected override ApiIdentityPart? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiIdentityPart;

        var apiPropertyName = readData?.ApiPropertyName;
        var clrConfiguredIdType = readData?.ClrConfiguredIdType;

        var apiIdentityPart = new ApiIdentityPart(apiPropertyName!, clrConfiguredIdType);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiIdentityPart, extensions);

        return apiIdentityPart;
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiIdentityPart value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentityPartApiPropertyName(writer, value, writeContext);
            WriteApiIdentityPartClrConfiguredIdType(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiIdentityPartApiPropertyName(Utf8JsonWriter writer, ApiIdentityPart apiIdentityPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityPart.ApiPropertyName;
        var value = apiIdentityPart.ApiPropertyName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdentityPartClrConfiguredIdType(Utf8JsonWriter writer, ApiIdentityPart apiIdentityPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityPart.ClrConfiguredIdType;
        var value = apiIdentityPart.ClrConfiguredIdType;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _typeJsonConverter);
    }
    #endregion
}
