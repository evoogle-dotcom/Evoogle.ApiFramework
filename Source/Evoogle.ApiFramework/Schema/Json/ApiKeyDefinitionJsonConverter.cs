// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Handles JSON serialization for anonymous <see cref="ApiKeyDefinition"/> instances, including
///     support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiKeyDefinitionJsonConverter(ILogger<ApiKeyDefinitionJsonConverter>? logger) : JsonConverterBase<ApiKeyDefinition>(logger)
{
    #region Property Types
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiKeyDefinitionJsonConverterCore.PropertyNames ApiKeyDefinition { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiKeyDefinition = ApiKeyDefinitionJsonConverterCore.PropertyNames.Create(policy),
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ReadState : ExtensibleReadData
    {
        #region Properties
        public ApiKeyDefinitionJsonConverterCore.ReadData? ApiKeyDefinition { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiKeyDefinition Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiKeyDefinition Property Handlers
            { propertyNames.ApiKeyDefinition.ApiKeyPaths, HandleApiKeyDefinitionApiKeyPaths },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() },
        };
        #endregion

        #region ApiKeyDefinition Methods
        private static void HandleApiKeyDefinitionApiKeyPaths(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKeyDefinition ??= new ApiKeyDefinitionJsonConverterCore.ReadData();
            context.ReadData.ApiKeyDefinition.ApiKeyPaths ??= [];

            ReadJsonArray(ref reader, context, (x) => HandleApiKeyDefinitionApiKeyPathsArrayItem);
        }

        private static void HandleApiKeyDefinitionApiKeyPathsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            ApiKeyDefinitionJsonConverterCore.ReadApiKeyPath
            (
                ref reader,
                context.Options,
                context.ReadData.ApiKeyDefinition!.ApiKeyPaths!
            );
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiKeyDefinitionJsonConverter()
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
    protected override ApiKeyDefinition? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ApiKeyDefinition;

        var apiKeyPaths = readState?.ApiKeyPaths;

        var apiKeyDefinition = new ApiKeyDefinition(apiKeyPaths!);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiKeyDefinition, extensions);

        return apiKeyDefinition;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiKeyDefinition value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiKeyDefinitionApiKeyPaths(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKeyDefinitionApiKeyPaths(Utf8JsonWriter writer, ApiKeyDefinition apiKeyDefinition, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyDefinition.ApiKeyPaths;
        var options = context.Options;

        ApiKeyDefinitionJsonConverterCore.WriteApiKeyPaths
        (
            writer,
            apiKeyDefinition,
            propertyName,
            options,
            WriteJsonArray
        );
    }
    #endregion
}
