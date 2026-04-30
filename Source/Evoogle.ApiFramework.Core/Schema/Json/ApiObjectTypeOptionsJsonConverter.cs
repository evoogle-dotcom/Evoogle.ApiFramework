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
///     JSON converter for serializing and deserializing <see cref="ApiObjectTypeOptions"/> instances.
/// </summary>
public class ApiObjectTypeOptionsJsonConverter(ILogger<ApiObjectTypeOptionsJsonConverter>? logger) : JsonConverterBase<ApiObjectTypeOptions>(logger)
{
    #region Property Types
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required string ApiIdentityPartNullHandling { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentityPartNullHandling = policy.ConvertName(nameof(ApiObjectTypeOptions.ApiIdentityPartNullHandling)),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ReadData
    {
        #region Properties
        public ApiIdentityPartNullHandling? ApiIdentityPartNullHandling { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _apiIdentityPartNullHandlingType = typeof(ApiIdentityPartNullHandling);
        #endregion

        #region Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiIdentityPartNullHandling, HandleApiIdentityPartNullHandling },
        };
        #endregion

        #region Methods
        private static void HandleApiIdentityPartNullHandling(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var options = context.Options;
            context.ReadData.ApiIdentityPartNullHandling = _apiIdentityPartNullHandlingJsonConverter.Read(ref reader, _apiIdentityPartNullHandlingType, options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiIdentityPartNullHandling> _apiIdentityPartNullHandlingJsonConverter = new();
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
        var readData = readContext.ReadData;

        var apiIdentityPartNullHandling = readData.ApiIdentityPartNullHandling;

        var apiObjectTypeOptions = new ApiObjectTypeOptions()
        {
            ApiIdentityPartNullHandling = apiIdentityPartNullHandling,
        };

        return apiObjectTypeOptions;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiObjectTypeOptions value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentityPartNullHandling(writer, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiIdentityPartNullHandling(Utf8JsonWriter writer, ApiObjectTypeOptions apiObjectTypeOptions, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityPartNullHandling;
        var apiIdentityPartNullHandling = apiObjectTypeOptions.ApiIdentityPartNullHandling;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, apiIdentityPartNullHandling, options, _apiIdentityPartNullHandlingJsonConverter);
    }
    #endregion
}
