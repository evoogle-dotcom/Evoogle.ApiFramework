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
        public required string ClrMemberName { get; init; }
        public required string ClrMemberKind { get; init; }
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
                    ClrMemberName = policy.ConvertName(nameof(Types.ApiClrMemberReference.ClrMemberName)),
                    ClrMemberKind = policy.ConvertName(nameof(Types.ApiClrMemberReference.ClrMemberKind)),
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
        public string? ClrMemberName { get; set; }
        public JsonEnumReadState<ClrMemberKind>? ClrMemberKind { get; set; }
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
            { propertyNames.ApiClrMemberReference.ClrMemberName, HandleApiClrMemberReferenceClrMemberName },
            { propertyNames.ApiClrMemberReference.ClrMemberKind, HandleApiClrMemberReferenceClrMemberKind },
        };
        #endregion

        #region ApiClrMemberReference Methods
        private static void HandleApiClrMemberReferenceClrMemberName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiClrMemberReference ??= new ApiClrMemberReferenceReadData();

            context.ReadData.ApiClrMemberReference.ClrMemberName = reader.GetString();
        }

        private static void HandleApiClrMemberReferenceClrMemberKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiClrMemberReference ??= new ApiClrMemberReferenceReadData();

            var readData = context.ReadData.ApiClrMemberReference;
            readData.ClrMemberKind ??= new JsonEnumReadState<ClrMemberKind>();
            readData.ClrMemberKind.Read(ref reader, context.Options, _nullableClrMemberKindJsonConverter);
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
        var clrMemberKindReadState = readState?.ClrMemberKind;

        var clrMemberName = readState?.ClrMemberName;
        var clrMemberKind = clrMemberKindReadState?.Value;
        var hasInvalidClrMemberKind = clrMemberKindReadState?.IsInvalid == true;

        var apiClrMemberReference = new ApiClrMemberReference(clrMemberName!, clrMemberKind, hasInvalidClrMemberKind);
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
            WriteApiClrMemberReferenceClrMemberName(writer, apiClrMemberReference, writeContext);
            WriteApiClrMemberReferenceClrMemberKind(writer, apiClrMemberReference, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiClrMemberReferenceClrMemberName(Utf8JsonWriter writer, ApiClrMemberReference apiClrMemberReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiClrMemberReference.ClrMemberName, value: apiClrMemberReference.ClrMemberName, options: writeContext.Options);

    private static void WriteApiClrMemberReferenceClrMemberKind(Utf8JsonWriter writer, ApiClrMemberReference apiClrMemberReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiClrMemberReference.ClrMemberKind, value: apiClrMemberReference.ClrMemberKind, options: writeContext.Options, converter: _clrMemberKindJsonConverter);
    #endregion
}
