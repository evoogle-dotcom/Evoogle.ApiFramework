// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Converts <see cref="ApiPropertyReference"/> instances to and from JSON.
/// </summary>
/// <param name="logger">The optional serialization logger.</param>
public sealed class ApiPropertyReferenceJsonConverter(ILogger<ApiPropertyReferenceJsonConverter>? logger)
    : JsonConverterBase<ApiPropertyReference>(logger)
{
    #region Property Types
    /// <summary>
    ///     Caches the JSON property names used to represent a property reference for the active naming policy.
    /// </summary>
    private readonly record struct ApiPropertyReferencePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ClrName { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates all property names used by the converter for a given naming policy.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiPropertyReferencePropertyNames ApiPropertyReference { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiPropertyReference = new ApiPropertyReferencePropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Types.ApiPropertyReference.ApiName)),
                    ClrName = policy.ConvertName(nameof(Types.ApiPropertyReference.ClrName)),
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage that holds parsed values prior to creating an <see cref="ApiPropertyReference"/>.
    /// </summary>
    private class ApiPropertyReferenceReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public string? ClrName { get; set; }
        #endregion
    }

    /// <summary>
    ///     Captures the overall data read from JSON for a single type reference instance.
    /// </summary>
    private class ReadState
    {
        #region Properties
        public ApiPropertyReferenceReadData? ApiPropertyReference { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides JSON property handlers that populate <see cref="ReadState"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiPropertyReference Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            { propertyNames.ApiPropertyReference.ApiName, HandleApiPropertyReferenceApiName },
            { propertyNames.ApiPropertyReference.ClrName, HandleApiPropertyReferenceClrName },
        };
        #endregion

        #region ApiPropertyReference Methods
        private static void HandleApiPropertyReferenceApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiPropertyReference ??= new ApiPropertyReferenceReadData();

            context.ReadData.ApiPropertyReference.ApiName = reader.GetString();
        }

        private static void HandleApiPropertyReferenceClrName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiPropertyReference ??= new ApiPropertyReferenceReadData();

            context.ReadData.ApiPropertyReference.ClrName = reader.GetString();
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiPropertyReferenceJsonConverter()
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
    protected override ApiPropertyReference? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ApiPropertyReference;

        var apiName = readState?.ApiName;
        var clrName = readState?.ClrName;

        var apiPropertyReference = new ApiPropertyReference(apiName, clrName);
        return apiPropertyReference;
    }

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create
            );

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
    protected override void WriteCore(Utf8JsonWriter writer, ApiPropertyReference apiPropertyReference, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiPropertyReference, writeContext), writeObject: static (writer, state) =>
        {
            var (apiPropertyReference, writeContext) = state;
            WriteApiPropertyReferenceApiName(writer, apiPropertyReference, writeContext);
            WriteApiPropertyReferenceClrName(writer, apiPropertyReference, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiPropertyReferenceApiName(Utf8JsonWriter writer, ApiPropertyReference apiPropertyReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiPropertyReference.ApiName, value: apiPropertyReference.ApiName, options: writeContext.Options);

    private static void WriteApiPropertyReferenceClrName(Utf8JsonWriter writer, ApiPropertyReference apiPropertyReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiPropertyReference.ClrName, value: apiPropertyReference.ClrName, options: writeContext.Options);
    #endregion
}
