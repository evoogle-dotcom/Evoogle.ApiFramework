// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     JSON converter for serializing and deserializing <see cref="ApiObjectTypeOptions"/> instances.
/// </summary>
public class ApiObjectTypeOptionsJsonConverter(ILogger<ApiObjectTypeOptionsJsonConverter>? logger) : JsonConverterBase<ApiObjectTypeOptions>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiObjectTypeOptions"/> under a given naming policy.
    /// </summary>
    private readonly record struct ApiObjectTypeOptionsPropertyNames
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
        public required ApiObjectTypeOptionsPropertyNames ApiObjectTypeOptions { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiObjectTypeOptions = new ApiObjectTypeOptionsPropertyNames
                {
                    ApiKeyNullHandling = policy.ConvertName(nameof(Types.ApiObjectTypeOptions.ApiKeyNullHandling)),
                },
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading options members from JSON.
    /// </summary>
    private class ApiObjectTypeOptionsReadData
    {
        #region Properties
        public JsonEnumReadState<ApiKeyNullHandling>? ApiKeyNullHandling { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiObjectTypeOptions"/> during deserialization.
    /// </summary>
    private class ReadData
    {
        #region Properties
        public ApiObjectTypeOptionsReadData? ApiObjectTypeOptions { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiObjectTypeOptions Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiObjectTypeOptions Property Handlers
            { propertyNames.ApiObjectTypeOptions.ApiKeyNullHandling, HandleApiObjectTypeOptionsApiKeyNullHandling, true },
        };
        #endregion

        #region ApiObjectTypeOptions Methods
        private static void HandleApiObjectTypeOptionsApiKeyNullHandling(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiObjectTypeOptions ??= new ApiObjectTypeOptionsReadData();

            var apiObjectTypeOptionsReadData = context.ReadData.ApiObjectTypeOptions;
            apiObjectTypeOptionsReadData.ApiKeyNullHandling ??= new JsonEnumReadState<ApiKeyNullHandling>();
            apiObjectTypeOptionsReadData.ApiKeyNullHandling.Read(ref reader, context.Options, _nullableApiKeyNullHandlingJsonConverter);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiKeyNullHandling> _apiKeyNullHandlingJsonConverter = new();
    private static readonly NullableEnumJsonConverter<ApiKeyNullHandling> _nullableApiKeyNullHandlingJsonConverter =
        new(EnumJsonInvalidValuePolicy.ReturnNull);
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiObjectTypeOptionsJsonConverter()
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
    protected override ApiObjectTypeOptions? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var apiObjectTypeOptionsReadData = readContext.ReadData.ApiObjectTypeOptions;

        var apiKeyNullHandlingReadState = apiObjectTypeOptionsReadData?.ApiKeyNullHandling;
        var apiKeyNullHandling = apiKeyNullHandlingReadState?.Value;

        var apiObjectTypeOptions = new ApiObjectTypeOptions()
        {
            ApiKeyNullHandling = apiKeyNullHandling,
            HasInvalidApiKeyNullHandling = apiKeyNullHandlingReadState?.IsInvalid == true,
        };

        return apiObjectTypeOptions;
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
    protected override void WriteCore(Utf8JsonWriter writer, ApiObjectTypeOptions apiObjectTypeOptions, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiObjectTypeOptions, writeContext), writeObject: static (writer, state) =>
        {
            var (apiObjectTypeOptions, writeContext) = state;
            WriteApiObjectTypeOptionsApiKeyNullHandling(writer, apiObjectTypeOptions, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiObjectTypeOptionsApiKeyNullHandling(Utf8JsonWriter writer, ApiObjectTypeOptions apiObjectTypeOptions, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiObjectTypeOptions.ApiKeyNullHandling, value: apiObjectTypeOptions.ApiKeyNullHandling, options: writeContext.Options, converter: _apiKeyNullHandlingJsonConverter);
    #endregion
}
