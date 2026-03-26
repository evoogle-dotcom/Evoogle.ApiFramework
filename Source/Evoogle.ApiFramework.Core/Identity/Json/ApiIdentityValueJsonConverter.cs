// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Identity.Json;

/// <summary>
///     Handles JSON serialization for <see cref="ApiIdentityValue"/> instances.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public sealed class ApiIdentityValueJsonConverter(ILogger<ApiIdentityValueJsonConverter>? logger)
    : JsonConverterBase<ApiIdentityValue>(logger)
{
    #region Property Types
    private readonly record struct ApiIdentityValuePropertyNames
    {
        #region Immutable Properties
        public required string ApiParts { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdentityValuePropertyNames ApiIdentityValue { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentityValue = new ApiIdentityValuePropertyNames
                {
                    ApiParts = policy.ConvertName(nameof(Identity.ApiIdentityValue.ApiParts)),
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiIdentityValueReadData
    {
        #region Properties
        public List<ApiIdentityPartValue>? ApiParts { get; set; }
        #endregion
    }

    private class ReadData
    {
        #region Properties
        public ApiIdentityValueReadData? ApiIdentityValue { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiIdentityValue Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> ApiIdentityValuePropertyHandlers = new()
        {
            { propertyNames.ApiIdentityValue.ApiParts, HandleApiIdentityValueApiParts },
        };
        #endregion

        #region ApiIdentityValue Methods
        private static void HandleApiIdentityValueApiParts(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityValue ??= new ApiIdentityValueReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiIdentityValue.ApiParts;
            var apiParts = DeserializeListOf<ApiIdentityPartValue>(ref reader, options, propertyName);

            context.ReadData.ApiIdentityValue.ApiParts = apiParts;
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdentityValueJsonConverter()
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
    protected override ApiIdentityValue? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiIdentityValue;

        var apiParts = readData?.ApiParts;

        return new ApiIdentityValue(apiParts);
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.ApiIdentityValuePropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiIdentityValue value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentityValueApiParts(writer, value, writeContext);
        });
    }
    #endregion

    #region Write Methods
    private static void WriteApiIdentityValueApiParts(Utf8JsonWriter writer, ApiIdentityValue apiIdentityValue, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityValue.ApiParts;
        var value = apiIdentityValue.ApiParts;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            value,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }
    #endregion
}
