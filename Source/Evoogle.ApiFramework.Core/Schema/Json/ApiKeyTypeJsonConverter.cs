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
///     Handles JSON serialization for <see cref="ApiKeyType"/> instances, including support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiKeyTypeJsonConverter(ILogger<ApiKeyTypeJsonConverter>? logger) : JsonConverterBase<ApiKeyType>(logger)
{
    #region Property Types
    private readonly record struct ApiKeyTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiKeyPaths { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiKeyTypePropertyNames ApiKeyType { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiKeyType = new ApiKeyTypePropertyNames
                {
                    ApiName = policy.ConvertName(nameof(ApiKeyType.ApiName)),
                    ApiKeyPaths = policy.ConvertName(nameof(ApiKeyType.ApiKeyPaths))
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiKeyTypeReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public List<ApiKeyPath>? ApiKeyPaths { get; set; }
        #endregion
    }

    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiKeyTypeReadData? ApiKeyType { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiKeyType Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiKeyType Property Handlers
            { propertyNames.ApiKeyType.ApiName, HandleApiKeyTypeApiName },
            { propertyNames.ApiKeyType.ApiKeyPaths, HandleApiKeyTypeApiKeyPaths },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiKeyType Methods
        private static void HandleApiKeyTypeApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiKeyType ??= new ApiKeyTypeReadData();

            context.ReadData.ApiKeyType.ApiName = reader.GetString();
        }

        private static void HandleApiKeyTypeApiKeyPaths(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiKeyType ??= new ApiKeyTypeReadData();
            context.ReadData.ApiKeyType.ApiKeyPaths ??= new List<ApiKeyPath>();

            ReadJsonArray(ref reader, context, (x) => HandleApiKeyTypeApiKeyPathsArrayItem);
        }

        private static void HandleApiKeyTypeApiKeyPathsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var apiKeyPath = JsonSerializer.Deserialize<ApiKeyPath>(ref reader, context.Options);
            if (apiKeyPath == null)
            {
                return;
            }

            context.ReadData.ApiKeyType!.ApiKeyPaths!.Add(apiKeyPath);
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiKeyTypeJsonConverter()
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
    protected override ApiKeyType? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiKeyType;

        var apiName = readData?.ApiName;
        var apiKeyPaths = readData?.ApiKeyPaths;

        var apiKeyType = new ApiKeyType(apiName!, apiKeyPaths!);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiKeyType, extensions);

        return apiKeyType;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiKeyType value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiKeyTypeApiName(writer, value, writeContext);
            WriteApiKeyTypeApiKeyPaths(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKeyTypeApiName(Utf8JsonWriter writer, ApiKeyType apiKeyType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyType.ApiName;
        var value = apiKeyType.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiKeyTypeApiKeyPaths(Utf8JsonWriter writer, ApiKeyType apiKeyType, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyType.ApiKeyPaths;
        var apiKeyPaths = apiKeyType.ApiKeyPaths;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiKeyPaths,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }
    #endregion
}
