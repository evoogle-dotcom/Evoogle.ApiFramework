// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Identity;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     JSON converter for serializing and deserializing <see cref="ApiSchemaOptions"/> instances.
/// </summary>
public class ApiSchemaOptionsJsonConverter(ILogger<ApiSchemaOptionsJsonConverter>? logger) : JsonConverterBase<ApiSchemaOptions>(logger)
{
    #region Property Types
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required string ApiIdentityNullHandling { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentityNullHandling = policy.ConvertName(nameof(ApiSchemaOptions.ApiIdentityNullHandling)),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ReadData
    {
        #region Properties
        public ApiIdentityNullHandling? ApiIdentityNullHandling { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _apiIdentityNullHandlingType = typeof(ApiIdentityNullHandling);
        #endregion

        #region Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiIdentityNullHandling, HandleApiIdentityNullHandling },
        };
        #endregion

        #region Methods
        private static void HandleApiIdentityNullHandling(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var options = context.Options;
            context.ReadData.ApiIdentityNullHandling = _apiIdentityNullHandlingJsonConverter.Read(ref reader, _apiIdentityNullHandlingType, options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiIdentityNullHandling> _apiIdentityNullHandlingJsonConverter = new();
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
        var readData = readContext.ReadData;

        var apiIdentityNullHandling = readData.ApiIdentityNullHandling ?? ApiSchemaOptions.Default.ApiIdentityNullHandling;

        var apiSchemaOptions = new ApiSchemaOptions()
        {
            ApiIdentityNullHandling = apiIdentityNullHandling,
        };

        return apiSchemaOptions;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiSchemaOptions value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentityNullHandling(writer, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiIdentityNullHandling(Utf8JsonWriter writer, ApiSchemaOptions apiSchemaOptions, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityNullHandling;
        var apiIdentityNullHandling = apiSchemaOptions.ApiIdentityNullHandling;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, apiIdentityNullHandling, options, _apiIdentityNullHandlingJsonConverter);
    }
    #endregion
}
