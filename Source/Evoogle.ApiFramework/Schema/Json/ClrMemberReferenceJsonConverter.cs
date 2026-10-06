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
///     Converts <see cref="ClrMemberReference"/> instances to and from JSON.
/// </summary>
/// <param name="logger">The optional serialization logger.</param>
public sealed class ClrMemberReferenceJsonConverter(ILogger<ClrMemberReferenceJsonConverter>? logger)
    : JsonConverterBase<ClrMemberReference>(logger)
{
    #region Property Types
    /// <summary>
    ///     Caches the JSON property names used to represent a CLR member reference for the active naming policy.
    /// </summary>
    private readonly record struct ClrMemberReferencePropertyNames
    {
        #region Immutable Properties
        public required string ClrKind { get; init; }
        public required string ClrName { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates all property names used by the converter for a given naming policy.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ClrMemberReferencePropertyNames ClrMemberReference { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ClrMemberReference = new ClrMemberReferencePropertyNames
                {
                    ClrKind = policy.ConvertName(nameof(Types.ClrMemberReference.ClrKind)),
                    ClrName = policy.ConvertName(nameof(Types.ClrMemberReference.ClrName)),
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage that holds parsed values prior to creating an <see cref="ClrMemberReference"/>.
    /// </summary>
    private class ClrMemberReferenceReadData
    {
        #region Properties
        public JsonEnumReadState<ClrMemberKind>? ClrKind { get; set; }
        public string? ClrName { get; set; }
        #endregion
    }

    /// <summary>
    ///     Captures the overall data read from JSON for a single CLR member reference instance.
    /// </summary>
    private class ReadState
    {
        #region Properties
        public ClrMemberReferenceReadData? ClrMemberReference { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides JSON property handlers that populate <see cref="ReadState"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ClrMemberReference Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            { propertyNames.ClrMemberReference.ClrKind, HandleClrMemberReferenceClrKind },
            { propertyNames.ClrMemberReference.ClrName, HandleClrMemberReferenceClrName },
        };
        #endregion

        #region ClrMemberReference Methods
        private static void HandleClrMemberReferenceClrKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ClrMemberReference ??= new ClrMemberReferenceReadData();

            var readData = context.ReadData.ClrMemberReference;
            readData.ClrKind ??= new JsonEnumReadState<ClrMemberKind>();
            readData.ClrKind.Read(ref reader, context.Options, _nullableClrMemberKindJsonConverter);
        }

        private static void HandleClrMemberReferenceClrName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ClrMemberReference ??= new ClrMemberReferenceReadData();

            context.ReadData.ClrMemberReference.ClrName = reader.GetString();
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
    public ClrMemberReferenceJsonConverter()
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
    protected override ClrMemberReference? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ClrMemberReference;
        var clrKindReadState = readState?.ClrKind;

        var clrKind = clrKindReadState?.Value;
        var clrName = readState?.ClrName;
        var hasInvalidClrKind = clrKind is null ||
            clrKindReadState?.IsInvalid == true ||
            clrKindReadState?.IsNull == true;

        var clrMemberReference = new ClrMemberReference(clrKind, clrName!, hasInvalidClrKind);
        return clrMemberReference;
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
    protected override void WriteCore(Utf8JsonWriter writer, ClrMemberReference clrMemberReference, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (clrMemberReference, writeContext), writeObject: static (writer, state) =>
        {
            var (clrMemberReference, writeContext) = state;
            WriteClrMemberReferenceClrKind(writer, clrMemberReference, writeContext);
            WriteClrMemberReferenceClrName(writer, clrMemberReference, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteClrMemberReferenceClrKind(Utf8JsonWriter writer, ClrMemberReference clrMemberReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ClrMemberReference.ClrKind, value: clrMemberReference.ClrKind, options: writeContext.Options, converter: _clrMemberKindJsonConverter);

    private static void WriteClrMemberReferenceClrName(Utf8JsonWriter writer, ClrMemberReference clrMemberReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ClrMemberReference.ClrName, value: clrMemberReference.ClrName, options: writeContext.Options);
    #endregion
}
