// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     JSON converter for serializing and deserializing <see cref="ApiSchemaOptions"/> instances.
/// </summary>
public class ApiSchemaOptionsJsonConverter(ILogger<ApiSchemaOptionsJsonConverter>? logger) : JsonConverterBase<ApiSchemaOptions>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiSchemaOptions"/> under a given naming policy.
    /// </summary>
    private readonly record struct ApiSchemaOptionsPropertyNames
    {
        #region Immutable Properties
        public required string ApiKeyNullHandling { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiSchemaOptionsPropertyNames ApiSchemaOptions { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiSchemaOptions = new ApiSchemaOptionsPropertyNames
                {
                    ApiKeyNullHandling = policy.ConvertName(nameof(Schema.ApiSchemaOptions.ApiKeyNullHandling)),
                },
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading options members from JSON.
    /// </summary>
    private class ApiSchemaOptionsReadData
    {
        #region Properties
        public JsonEnumReadResult<ApiKeyNullHandling> ApiKeyNullHandling { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiSchemaOptions"/> during deserialization.
    /// </summary>
    private class ReadData
    {
        #region Properties
        public ApiSchemaOptionsReadData? ApiSchemaOptions { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiSchemaOptions Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiSchemaOptions Property Handlers
            { propertyNames.ApiSchemaOptions.ApiKeyNullHandling, HandleApiSchemaOptionsApiKeyNullHandling, true },
        };
        #endregion

        #region ApiSchemaOptions Methods
        private static void HandleApiSchemaOptionsApiKeyNullHandling(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchemaOptions ??= new ApiSchemaOptionsReadData();

            context.ReadData.ApiSchemaOptions.ApiKeyNullHandling =
                JsonEnumReader.Read<ApiKeyNullHandling>(ref reader, context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiKeyNullHandling> _apiKeyNullHandlingJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiSchemaOptionsJsonConverter()
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
    protected override ApiSchemaOptions? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var apiSchemaOptionsReadData = readContext.ReadData.ApiSchemaOptions;

        var apiKeyNullHandlingReadResult = apiSchemaOptionsReadData?.ApiKeyNullHandling ?? default;
        var apiKeyNullHandling = apiKeyNullHandlingReadResult.ValueOr(ApiSchemaDefaults.KeyNullHandling);

        var apiKeyNullHandlingFinding = JsonEnumFinding.Create
        (
            nameof(ApiSchemaOptions.ApiKeyNullHandling),
            apiKeyNullHandlingReadResult
        );

        var apiSchemaOptions = new ApiSchemaOptions(apiKeyNullHandlingFinding)
        {
            ApiKeyNullHandling = apiKeyNullHandling,
        };

        return apiSchemaOptions;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject
        (
            ref reader,
            readContext,
            handlers
        );
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiSchemaOptions apiSchemaOptions, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiSchemaOptions, writeContext), writeObject: static (writer, state) =>
        {
            var (apiSchemaOptions, writeContext) = state;
            WriteApiSchemaOptionsApiKeyNullHandling(writer, apiSchemaOptions, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiSchemaOptionsApiKeyNullHandling(Utf8JsonWriter writer, ApiSchemaOptions apiSchemaOptions, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter
        (
            propertyName: writeContext.PropertyNames.ApiSchemaOptions.ApiKeyNullHandling,
            value: apiSchemaOptions.ApiKeyNullHandling,
            options: writeContext.Options,
            converter: _apiKeyNullHandlingJsonConverter,
            defaultValue: ApiSchemaDefaults.KeyNullHandling
        );
    #endregion
}
