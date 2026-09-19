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
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required string ApiKeyNullHandling { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiKeyNullHandling = policy.ConvertName(nameof(ApiObjectTypeOptions.ApiKeyNullHandling)),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ReadState
    {
        #region Properties
        public JsonEnumReadState<ApiKeyNullHandling>? ApiKeyNullHandling { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            { propertyNames.ApiKeyNullHandling, HandleApiKeyNullHandling, true },
        };
        #endregion

        #region Methods
        private static void HandleApiKeyNullHandling(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKeyNullHandling ??= new JsonEnumReadState<ApiKeyNullHandling>();
            context.ReadData.ApiKeyNullHandling.Read(ref reader, context.Options, _nullableApiKeyNullHandlingJsonConverter);
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
    protected override ApiObjectTypeOptions? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData;

        var apiKeyNullHandlingReadState = readState.ApiKeyNullHandling;
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
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
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
            WriteApiKeyNullHandling(writer, apiObjectTypeOptions, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKeyNullHandling(Utf8JsonWriter writer, ApiObjectTypeOptions apiObjectTypeOptions, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiKeyNullHandling, value: apiObjectTypeOptions.ApiKeyNullHandling, options: writeContext.Options, converter: _apiKeyNullHandlingJsonConverter);
    #endregion
}
