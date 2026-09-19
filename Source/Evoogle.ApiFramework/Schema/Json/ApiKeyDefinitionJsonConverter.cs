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
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
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

            ReadJsonArray(ref reader, context, HandleApiKeyDefinitionApiKeyPathsArrayItem);
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
    protected override void WriteCore(Utf8JsonWriter writer, ApiKeyDefinition apiKeyDefinition, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiKeyDefinition, writeContext), writeObject: static (writer, state) =>
        {
            var (apiKeyDefinition, writeContext) = state;
            WriteApiKeyDefinitionApiKeyPaths(writer, apiKeyDefinition, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiKeyDefinition,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKeyDefinitionApiKeyPaths(Utf8JsonWriter writer, ApiKeyDefinition apiKeyDefinition, DefaultWriteContext<PropertyNames> writeContext)
    {
        writer.TryWritePropertyAsArray
        (
            propertyName: writeContext.PropertyNames.ApiKeyDefinition.ApiKeyPaths,
            collection: apiKeyDefinition.ApiKeyPaths,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
            {
                var writeContext = state;

                writer.TryWriteWithSerializer(obj: item, options: writeContext.Options);
            }
        );
    }
    #endregion
}
