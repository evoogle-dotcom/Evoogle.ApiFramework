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
///     Handles JSON serialization for <see cref="ApiKeyPathSegment"/> instances, including support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiKeyPathSegmentJsonConverter(ILogger<ApiKeyPathSegmentJsonConverter>? logger) : JsonConverterBase<ApiKeyPathSegment>(logger)
{
    #region Property Types
    private readonly record struct ApiKeyPathSegmentPropertyNames
    {
        #region Immutable Properties
        public required string ClrPropertyName { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiKeyPathSegmentPropertyNames ApiKeyPathSegment { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiKeyPathSegment = new ApiKeyPathSegmentPropertyNames
                {
                    ClrPropertyName = policy.ConvertName(nameof(ApiKeyPathSegment.ClrPropertyName))
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiKeyPathSegmentReadData
    {
        #region Properties
        public string? ClrPropertyName { get; set; }
        #endregion
    }

    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiKeyPathSegmentReadData? ApiKeyPathSegment { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiKeyPathSegment Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiKeyPathSegment Property Handlers
            { propertyNames.ApiKeyPathSegment.ClrPropertyName, HandleApiKeyPathSegmentClrPropertyName },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiKeyPathSegment Methods
        private static void HandleApiKeyPathSegmentClrPropertyName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPathSegment ??= new ApiKeyPathSegmentReadData();

            context.ReadData.ApiKeyPathSegment.ClrPropertyName = reader.GetString();
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiKeyPathSegmentJsonConverter()
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
    protected override ApiKeyPathSegment? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiKeyPathSegment;

        var clrPropertyName = readData?.ClrPropertyName;

        var apiKeyPathSegment = new ApiKeyPathSegment(clrPropertyName!);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiKeyPathSegment, extensions);

        return apiKeyPathSegment;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiKeyPathSegment value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiKeyPathSegmentClrPropertyName(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKeyPathSegmentClrPropertyName(Utf8JsonWriter writer, ApiKeyPathSegment apiKeyPathSegment, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyPathSegment.ClrPropertyName;
        var value = apiKeyPathSegment.ClrPropertyName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }
    #endregion
}
