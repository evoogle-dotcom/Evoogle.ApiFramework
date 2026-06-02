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
///     Handles JSON serialization for <see cref="ApiKeyPath"/> instances, including support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiKeyPathJsonConverter(ILogger<ApiKeyPathJsonConverter>? logger) : JsonConverterBase<ApiKeyPath>(logger)
{
    #region Property Types
    private readonly record struct ApiKeyPathPropertyNames
    {
        #region Immutable Properties
        public required string ClrRootType { get; init; }
        public required string ApiSegments { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiKeyPathPropertyNames ApiKeyPath { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiKeyPath = new ApiKeyPathPropertyNames
                {
                    ClrRootType = policy.ConvertName(nameof(ApiKeyPath.ClrRootType)),
                    ApiSegments = policy.ConvertName(nameof(ApiKeyPath.ApiSegments)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiKeyPathReadData
    {
        #region Properties
        public Type? ClrRootType { get; set; }
        public List<ApiKeyPathSegment>? ApiSegments { get; set; }
        #endregion
    }

    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiKeyPathReadData? ApiKeyPath { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiKeyPath Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiKeyPath Property Handlers
            { propertyNames.ApiKeyPath.ClrRootType, HandleApiKeyPathClrRootType },
            { propertyNames.ApiKeyPath.ApiSegments, HandleApiKeyPathApiSegments },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiKeyPath Methods
        private static void HandleApiKeyPathClrRootType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPath ??= new ApiKeyPathReadData();

            var options = context.Options;
            context.ReadData.ApiKeyPath.ClrRootType = _typeJsonConverter.Read(ref reader, typeof(Type), options);
        }

        private static void HandleApiKeyPathApiSegments(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPath ??= new ApiKeyPathReadData();
            context.ReadData.ApiKeyPath.ApiSegments ??= [];

            ReadJsonArray(ref reader, context, _ => HandleApiKeyPathApiSegmentsArrayItem);
        }

        private static void HandleApiKeyPathApiSegmentsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            // Recursive: each child element uses the same converter via the [JsonConverter] attribute.
            var segment = JsonSerializer.Deserialize<ApiKeyPathSegment>(ref reader, context.Options);
            if (segment is null)
            {
                return;
            }

            context.ReadData.ApiKeyPath!.ApiSegments!.Add(segment);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiKeyPathJsonConverter()
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
    protected override ApiKeyPath? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiKeyPath;

        var clrRootType = readData?.ClrRootType;
        var apiSegments = readData?.ApiSegments;

        var apiKeyPath = new ApiKeyPath(clrRootType!, apiSegments!);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiKeyPath, extensions);

        return apiKeyPath;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiKeyPath value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiKeyPathClrRootType(writer, value, writeContext);
            WriteApiKeyPathApiSegments(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKeyPathClrRootType(Utf8JsonWriter writer, ApiKeyPath apiKeyPath, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyPath.ClrRootType;
        var type = apiKeyPath.ClrRootType;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, type, options, _typeJsonConverter);
    }

    private static void WriteApiKeyPathApiSegments(Utf8JsonWriter writer, ApiKeyPath apiKeyPath, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyPath.ApiSegments;
        var apiSegments = apiKeyPath.ApiSegments;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiSegments,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }
    #endregion
}
