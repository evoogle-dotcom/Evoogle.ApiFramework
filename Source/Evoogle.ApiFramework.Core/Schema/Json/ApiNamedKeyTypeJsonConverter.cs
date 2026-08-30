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
///     Handles JSON serialization for <see cref="ApiNamedKeyType"/> instances, including support
///     for extensions.
/// </summary>
/// <param name="logger">
///     The optional logger used to emit diagnostics during JSON operations.
/// </param>
public class ApiNamedKeyTypeJsonConverter(ILogger<ApiNamedKeyTypeJsonConverter>? logger)
    : JsonConverterBase<ApiNamedKeyType>(logger)
{
    #region Property Types
    private readonly record struct ApiNamedKeyTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiKeyTypeJsonConverterCore.PropertyNames ApiKeyType { get; init; }
        public required ApiNamedKeyTypePropertyNames ApiNamedKeyType { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiKeyType = ApiKeyTypeJsonConverterCore.PropertyNames.Create(policy),
                ApiNamedKeyType = new ApiNamedKeyTypePropertyNames
                {
                    ApiName = policy.ConvertName(nameof(ApiNamedKeyType.ApiName))
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiNamedKeyTypeReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        #endregion
    }

    private class ReadState : ExtensibleReadData
    {
        #region Properties
        public ApiKeyTypeJsonConverterCore.ReadData? ApiKeyType { get; set; }
        public ApiNamedKeyTypeReadData? ApiNamedKeyType { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiNamedKeyType Fields
        public readonly Dictionary
        <
            string,
            JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>
        > PropertyHandlers = new()
        {
            // ApiNamedKeyType Property Handlers
            { propertyNames.ApiNamedKeyType.ApiName, HandleApiNamedKeyTypeApiName },

            // ApiKeyType Property Handlers
            { propertyNames.ApiKeyType.ApiKeyPaths, HandleApiKeyTypeApiKeyPaths },

            // ExtensibleBase Property Handlers
            {
                propertyNames.ExtensibleBase.Extensions,
                CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>()
            },
        };
        #endregion

        #region ApiNamedKeyType Methods
        private static void HandleApiNamedKeyTypeApiName
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        )
        {
            context.ReadData.ApiNamedKeyType ??= new ApiNamedKeyTypeReadData();
            context.ReadData.ApiNamedKeyType.ApiName = reader.GetString();
        }

        private static void HandleApiKeyTypeApiKeyPaths
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        )
        {
            context.ReadData.ApiKeyType ??= new ApiKeyTypeJsonConverterCore.ReadData();
            context.ReadData.ApiKeyType.ApiKeyPaths ??= [];

            ReadJsonArray(ref reader, context, (x) => HandleApiKeyTypeApiKeyPathsArrayItem);
        }

        private static void HandleApiKeyTypeApiKeyPathsArrayItem
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        )
        {
            ApiKeyTypeJsonConverterCore.ReadApiKeyPath
            (
                ref reader,
                context.Options,
                context.ReadData.ApiKeyType!.ApiKeyPaths!
            );
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>
    ///     Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.
    /// </summary>
    public ApiNamedKeyTypeJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase Methods
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
    protected override IWriteContext CreateWriteContext
    (
        ILogger logger,
        JsonSerializerOptions options
    )
        => CreateDefaultWriteContext
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create
            );

    /// <inheritdoc/>
    protected override ApiNamedKeyType? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ApiNamedKeyType;

        var apiName = readState?.ApiName;
        var apiKeyPaths = readContext.ReadData.ApiKeyType?.ApiKeyPaths;

        var apiNamedKeyType = new ApiNamedKeyType(apiName!, apiKeyPaths!);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiNamedKeyType, extensions);

        return apiNamedKeyType;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore
    (
        Utf8JsonWriter writer,
        ApiNamedKeyType value,
        IWriteContext context
    )
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiName(writer, value, writeContext);
            WriteApiKeyPaths(writer, value, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                writeContext.PropertyNames.ExtensibleBase.Extensions,
                value,
                writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiName
    (
        Utf8JsonWriter writer,
        ApiNamedKeyType apiNamedKeyType,
        DefaultWriteContext<PropertyNames> context
    )
    {
        var propertyName = context.PropertyNames.ApiNamedKeyType.ApiName;
        var value = apiNamedKeyType.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiKeyPaths
    (
        Utf8JsonWriter writer,
        ApiNamedKeyType apiNamedKeyType,
        DefaultWriteContext<PropertyNames> context
    )
    {
        var propertyName = context.PropertyNames.ApiKeyType.ApiKeyPaths;
        var options = context.Options;

        ApiKeyTypeJsonConverterCore.WriteApiKeyPaths
        (
            writer,
            apiNamedKeyType,
            propertyName,
            options,
            WriteJsonArray
        );
    }
    #endregion
}
