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
///     Handles JSON serialization for <see cref="ApiNamedKeyDefinition"/> instances, including support
///     for extensions.
/// </summary>
/// <param name="logger">
///     The optional logger used to emit diagnostics during JSON operations.
/// </param>
public class ApiNamedKeyDefinitionJsonConverter(ILogger<ApiNamedKeyDefinitionJsonConverter>? logger)
    : JsonConverterBase<ApiNamedKeyDefinition>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiNamedKeyDefinition"/> under a given naming policy.
    /// </summary>
    private readonly record struct ApiNamedKeyDefinitionPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiKeyDefinitionJsonConverterCore.PropertyNames ApiKeyDefinition { get; init; }
        public required ApiNamedKeyDefinitionPropertyNames ApiNamedKeyDefinition { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiKeyDefinition = ApiKeyDefinitionJsonConverterCore.PropertyNames.Create(policy),
                ApiNamedKeyDefinition = new ApiNamedKeyDefinitionPropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Key.ApiNamedKeyDefinition.ApiName))
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading named key members from JSON.
    /// </summary>
    private class ApiNamedKeyDefinitionReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiNamedKeyDefinition"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiKeyDefinitionJsonConverterCore.ReadData? ApiKeyDefinition { get; set; }
        public ApiNamedKeyDefinitionReadData? ApiNamedKeyDefinition { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiNamedKeyDefinition Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiNamedKeyDefinition Property Handlers
            { propertyNames.ApiNamedKeyDefinition.ApiName, HandleApiNamedKeyDefinitionApiName },

            // ApiKeyDefinition Property Handlers
            { propertyNames.ApiKeyDefinition.ApiKeyPaths, HandleApiKeyDefinitionApiKeyPaths },

            // ExtensibleBase Property Handlers
            {
                propertyNames.ExtensibleBase.Extensions,
                CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>()
            },
        };
        #endregion

        #region ApiNamedKeyDefinition Methods
        private static void HandleApiNamedKeyDefinitionApiName
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context
        )
        {
            context.ReadData.ApiNamedKeyDefinition ??= new ApiNamedKeyDefinitionReadData();
            context.ReadData.ApiNamedKeyDefinition.ApiName = reader.GetString();
        }

        private static void HandleApiKeyDefinitionApiKeyPaths
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context
        )
        {
            context.ReadData.ApiKeyDefinition ??= new ApiKeyDefinitionJsonConverterCore.ReadData();
            context.ReadData.ApiKeyDefinition.ApiKeyPaths ??= [];

            ReadJsonArray(ref reader, context, HandleApiKeyDefinitionApiKeyPathsArrayItem);
        }

        private static void HandleApiKeyDefinitionApiKeyPathsArrayItem
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context
        )
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
    /// <summary>
    ///     Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.
    /// </summary>
    public ApiNamedKeyDefinitionJsonConverter()
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
    protected override ApiNamedKeyDefinition? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var apiNamedKeyDefinitionReadData = readContext.ReadData.ApiNamedKeyDefinition;

        var apiName = apiNamedKeyDefinitionReadData?.ApiName;
        var apiKeyPaths = readContext.ReadData.ApiKeyDefinition?.ApiKeyPaths;

        var apiNamedKeyDefinition = new ApiNamedKeyDefinition(apiName!, apiKeyPaths!);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiNamedKeyDefinition, extensions);

        return apiNamedKeyDefinition;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiNamedKeyDefinition apiKeyDefinition, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiKeyDefinition, writeContext), writeObject: static (writer, state) =>
        {
            var (apiKeyDefinition, writeContext) = state;
            WriteApiNamedKeyDefinitionApiName(writer, apiKeyDefinition, writeContext);
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
    private static void WriteApiNamedKeyDefinitionApiName(Utf8JsonWriter writer, ApiNamedKeyDefinition apiNamedKeyDefinition, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiNamedKeyDefinition.ApiName, value: apiNamedKeyDefinition.ApiName, options: writeContext.Options);

    private static void WriteApiKeyDefinitionApiKeyPaths(Utf8JsonWriter writer, ApiNamedKeyDefinition apiNamedKeyDefinition, DefaultWriteContext<PropertyNames> writeContext)
    {
        writer.TryWritePropertyAsArray
        (
            propertyName: writeContext.PropertyNames.ApiKeyDefinition.ApiKeyPaths,
            collection: apiNamedKeyDefinition.ApiKeyPaths,
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
