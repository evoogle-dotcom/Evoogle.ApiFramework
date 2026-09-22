// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Converts <see cref="ApiClrMemberReference"/> instances to and from JSON.
/// </summary>
/// <param name="logger">The optional serialization logger.</param>
public sealed class ApiClrMemberReferenceJsonConverter(ILogger<ApiClrMemberReferenceJsonConverter>? logger)
    : JsonConverterBase<ApiClrMemberReference>(logger)
{
    #region Property Types
    /// <summary>
    ///     Caches the JSON property names used to represent a CLR member reference for the active naming policy.
    /// </summary>
    private readonly record struct ApiClrMemberReferencePropertyNames
    {
        #region Immutable Properties
        public required string ClrName { get; init; }
        public required string ClrKind { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates all property names used by the converter for a given naming policy.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiClrMemberReferencePropertyNames ApiClrMemberReference { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiClrMemberReference = new ApiClrMemberReferencePropertyNames
                {
                    ClrName = policy.ConvertName(nameof(Types.ApiClrMemberReference.ClrName)),
                    ClrKind = policy.ConvertName(nameof(Types.ApiClrMemberReference.ClrKind)),
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage that holds parsed values prior to creating an <see cref="ApiClrMemberReference"/>.
    /// </summary>
    private class ApiClrMemberReferenceReadData
    {
        #region Properties
        public string? ClrName { get; set; }
        public JsonEnumReadState<ClrMemberKind>? ClrKind { get; set; }
        #endregion
    }

    /// <summary>
    ///     Captures the overall data read from JSON for a single CLR member reference instance.
    /// </summary>
    private class ReadState
    {
        #region Properties
        public ApiClrMemberReferenceReadData? ApiClrMemberReference { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides JSON property handlers that populate <see cref="ReadState"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiClrMemberReference Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            { propertyNames.ApiClrMemberReference.ClrName, HandleApiClrMemberReferenceClrName },
            { propertyNames.ApiClrMemberReference.ClrKind, HandleApiClrMemberReferenceClrKind },
        };
        #endregion

        #region ApiClrMemberReference Methods
        private static void HandleApiClrMemberReferenceClrName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiClrMemberReference ??= new ApiClrMemberReferenceReadData();

            context.ReadData.ApiClrMemberReference.ClrName = reader.GetString();
        }

        private static void HandleApiClrMemberReferenceClrKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiClrMemberReference ??= new ApiClrMemberReferenceReadData();

            var readData = context.ReadData.ApiClrMemberReference;
            readData.ClrKind ??= new JsonEnumReadState<ClrMemberKind>();
            readData.ClrKind.Read(ref reader, context.Options, _nullableClrMemberKindJsonConverter);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ClrMemberKind> _clrMemberKindJsonConverter = new();
    private static readonly NullableEnumJsonConverter<ClrMemberKind> _nullableClrMemberKindJsonConverter = new(EnumJsonInvalidValuePolicy.ReturnNull);
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiClrMemberReferenceJsonConverter()
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
    protected override ApiClrMemberReference? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ApiClrMemberReference;
        var clrKindReadState = readState?.ClrKind;

        var clrName = readState?.ClrName;
        var clrKind = clrKindReadState?.Value;
        var hasInvalidClrKind = clrKindReadState?.IsInvalid == true;

        var apiClrMemberReference = new ApiClrMemberReference(clrName!, clrKind, hasInvalidClrKind);
        return apiClrMemberReference;
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
    protected override void WriteCore(Utf8JsonWriter writer, ApiClrMemberReference apiClrMemberReference, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiClrMemberReference, writeContext), writeObject: static (writer, state) =>
        {
            var (apiClrMemberReference, writeContext) = state;
            WriteApiClrMemberReferenceClrName(writer, apiClrMemberReference, writeContext);
            WriteApiClrMemberReferenceClrKind(writer, apiClrMemberReference, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiClrMemberReferenceClrName(Utf8JsonWriter writer, ApiClrMemberReference apiClrMemberReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiClrMemberReference.ClrName, value: apiClrMemberReference.ClrName, options: writeContext.Options);

    private static void WriteApiClrMemberReferenceClrKind(Utf8JsonWriter writer, ApiClrMemberReference apiClrMemberReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiClrMemberReference.ClrKind, value: apiClrMemberReference.ClrKind, options: writeContext.Options, converter: _clrMemberKindJsonConverter);
    #endregion
}
