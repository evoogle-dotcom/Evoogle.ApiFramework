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
///     Converts <see cref="ApiTypeReference"/> instances to and from JSON.
/// </summary>
/// <param name="logger">The optional serialization logger.</param>
public sealed class ApiTypeReferenceJsonConverter(ILogger<ApiTypeReferenceJsonConverter>? logger)
    : JsonConverterBase<ApiTypeReference>(logger)
{
    #region Property Types
    /// <summary>
    ///     Caches the JSON property names used to represent a type reference for the active naming policy.
    /// </summary>
    private readonly record struct ApiTypeReferencePropertyNames
    {
        #region Immutable Properties
        public required string ApiKind { get; init; }
        public required string ApiName { get; init; }
        public required string ClrType { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates all property names used by the converter for a given naming policy.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiTypeReferencePropertyNames ApiTypeReference { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiTypeReference = new ApiTypeReferencePropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(Types.ApiTypeReference.ApiKind)),
                    ApiName = policy.ConvertName(nameof(Types.ApiTypeReference.ApiName)),
                    ClrType = policy.ConvertName(nameof(Types.ApiTypeReference.ClrType)),
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage that holds parsed values prior to creating an <see cref="ApiTypeReference"/>.
    /// </summary>
    private class ApiTypeReferenceReadData
    {
        #region Properties
        public JsonEnumReadState<ApiTypeKind>? ApiKind { get; set; }
        public string? ApiName { get; set; }
        public Type? ClrType { get; set; }
        #endregion
    }

    /// <summary>
    ///     Captures the overall data read from JSON for a single type reference instance.
    /// </summary>
    private class ReadState
    {
        #region Properties
        public ApiTypeReferenceReadData? ApiTypeReference { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides JSON property handlers that populate <see cref="ReadState"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiTypeReference Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            { propertyNames.ApiTypeReference.ApiKind, HandleApiTypeReferenceApiKind, true },
            { propertyNames.ApiTypeReference.ApiName, HandleApiTypeReferenceApiName },
            { propertyNames.ApiTypeReference.ClrType, HandleApiTypeReferenceClrType },
        };
        #endregion

        #region ApiTypeReference Methods
        private static void HandleApiTypeReferenceApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiTypeReference ??= new ApiTypeReferenceReadData();

            var readData = context.ReadData.ApiTypeReference;
            readData.ApiKind ??= new JsonEnumReadState<ApiTypeKind>();
            readData.ApiKind.Read(ref reader, context.Options, _nullableApiTypeKindJsonConverter);
        }

        private static void HandleApiTypeReferenceApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiTypeReference ??= new ApiTypeReferenceReadData();

            context.ReadData.ApiTypeReference.ApiName = reader.GetString();
        }

        private static void HandleApiTypeReferenceClrType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiTypeReference ??= new ApiTypeReferenceReadData();

            context.ReadData.ApiTypeReference.ClrType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeKind> _apiTypeKindJsonConverter = new();
    private static readonly NullableEnumJsonConverter<ApiTypeKind> _nullableApiTypeKindJsonConverter =
        new(EnumJsonInvalidValuePolicy.ReturnNull);

    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiTypeReferenceJsonConverter()
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
    protected override ApiTypeReference? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ApiTypeReference;
        var apiKindReadState = readState?.ApiKind;

        var apiKind = apiKindReadState?.Value;
        var apiName = readState?.ApiName;
        var clrType = readState?.ClrType;
        var hasInvalidApiKind = apiKindReadState?.IsInvalid == true;

        var apiTypeReference = new ApiTypeReference(apiKind, apiName, clrType, hasInvalidApiKind);
        return apiTypeReference;
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
    protected override void WriteCore(Utf8JsonWriter writer, ApiTypeReference apiTypeReference, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiTypeReference, writeContext), writeObject: static (writer, state) =>
        {
            var (apiTypeReference, writeContext) = state;
            WriteApiTypeReferenceApiKind(writer, apiTypeReference, writeContext);
            WriteApiTypeReferenceApiName(writer, apiTypeReference, writeContext);
            WriteApiTypeReferenceClrType(writer, apiTypeReference, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiTypeReferenceApiKind(Utf8JsonWriter writer, ApiTypeReference apiTypeReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiTypeReference.ApiKind, value: apiTypeReference.ApiKind, options: writeContext.Options, converter: _apiTypeKindJsonConverter);

    private static void WriteApiTypeReferenceApiName(Utf8JsonWriter writer, ApiTypeReference apiTypeReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiTypeReference.ApiName, value: apiTypeReference.ApiName, options: writeContext.Options);

    private static void WriteApiTypeReferenceClrType(Utf8JsonWriter writer, ApiTypeReference apiTypeReference, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiTypeReference.ClrType, type: apiTypeReference.ClrType, options: writeContext.Options, converter: _typeJsonConverter);
    #endregion
}
