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
///     Serializes and deserializes <see cref="ApiProperty"/> instances, including extension payloads and type expressions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiPropertyJsonConverter(ILogger<ApiPropertyJsonConverter>? logger) : JsonConverterBase<ApiProperty>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiProperty"/> under a given naming policy.
    /// </summary>
    private readonly record struct ApiPropertyPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiTypeExpression { get; init; }
        public required string ApiTypeModifiers { get; init; }
        public required string ClrName { get; init; }
        public required string ClrMemberKind { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiPropertyPropertyNames ApiProperty { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiProperty = new ApiPropertyPropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Types.ApiProperty.ApiName)),
                    ApiTypeExpression = policy.ConvertName(nameof(Types.ApiProperty.ApiType)), // Mapping property name from ApiTypeExpression to ApiType by design
                    ApiTypeModifiers = policy.ConvertName(nameof(Types.ApiProperty.ApiTypeModifiers)),
                    ClrName = policy.ConvertName(nameof(Types.ApiProperty.ClrName)),
                    ClrMemberKind = policy.ConvertName(nameof(Types.ApiProperty.ClrMemberKind)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading property members from JSON.
    /// </summary>
    private class ApiPropertyReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiTypeExpression? ApiTypeExpression { get; set; }
        public JsonEnumReadState<ApiTypeModifiers>? ApiTypeModifiers { get; set; }
        public string? ClrName { get; set; }
        public ClrMemberKind? ClrMemberKind { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiProperty"/> during deserialization.
    /// </summary>
    private class ReadState : ExtensibleReadData
    {
        #region Properties
        public ApiPropertyReadData? ApiProperty { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadState"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _clrMemberKindType = typeof(ClrMemberKind?);
        #endregion

        #region ApiProperty Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiProperty Property Handlers
            { propertyNames.ApiProperty.ApiName, HandleApiPropertyApiName },
            { propertyNames.ApiProperty.ApiTypeModifiers, HandleApiPropertyApiTypeModifiers, true },
            { propertyNames.ApiProperty.ApiTypeExpression, HandleApiPropertyApiTypeExpression },
            { propertyNames.ApiProperty.ClrName, HandleApiPropertyClrName },
            { propertyNames.ApiProperty.ClrMemberKind, HandleApiPropertyClrMemberKind, true },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() },
        };
        #endregion

        #region ApiProperty Methods
        private static void HandleApiPropertyApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiProperty ??= new ApiPropertyReadData();

            context.ReadData.ApiProperty.ApiName = reader.GetString();
        }

        private static void HandleApiPropertyApiTypeModifiers(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiProperty ??= new ApiPropertyReadData();

            var readState = context.ReadData.ApiProperty;
            readState.ApiTypeModifiers ??= new JsonEnumReadState<ApiTypeModifiers>();
            readState.ApiTypeModifiers.Read(ref reader, context.Options, _nullableApiTypeModifiersJsonConverter);
        }

        private static void HandleApiPropertyApiTypeExpression(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiProperty ??= new ApiPropertyReadData();

            var options = context.Options;
            context.ReadData.ApiProperty.ApiTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, options);
        }

        private static void HandleApiPropertyClrName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiProperty ??= new ApiPropertyReadData();

            context.ReadData.ApiProperty.ClrName = reader.GetString();
        }

        private static void HandleApiPropertyClrMemberKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiProperty ??= new ApiPropertyReadData();

            var options = context.Options;
            context.ReadData.ApiProperty.ClrMemberKind = _nullableClrMemberKindJsonConverter.Read
            (
                ref reader,
                _clrMemberKindType,
                options
            );
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeModifiers> _apiTypeModifiersJsonConverter = new();
    private static readonly NullableEnumJsonConverter<ApiTypeModifiers> _nullableApiTypeModifiersJsonConverter =
        new(EnumJsonInvalidValuePolicy.ReturnNull);
    private static readonly EnumJsonConverter<ClrMemberKind> _clrMemberKindJsonConverter = new();
    private static readonly NullableEnumJsonConverter<ClrMemberKind> _nullableClrMemberKindJsonConverter =
        new(EnumJsonInvalidValuePolicy.ReturnNull);
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiPropertyJsonConverter()
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
    protected override ApiProperty? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ApiProperty;

        var apiName = readState?.ApiName;
        var apiTypeExpression = readState?.ApiTypeExpression;
        var apiTypeModifiersReadState = readState?.ApiTypeModifiers;
        var apiTypeModifiers = apiTypeModifiersReadState?.Value ?? ApiTypeModifiers.None;
        var clrName = readState?.ClrName;
        var clrMemberKind = readState?.ClrMemberKind;

        var apiProperty = new ApiProperty(apiName!, apiTypeExpression!, apiTypeModifiers, clrName!, clrMemberKind);

        if (apiTypeModifiersReadState?.IsInvalid == true || apiTypeModifiersReadState?.IsNull == true)
        {
            apiProperty.MarkInvalidApiTypeModifiers();
        }

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiProperty, extensions);

        return apiProperty;
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
    protected override void WriteCore(Utf8JsonWriter writer, ApiProperty apiProperty, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiProperty, writeContext), writeObject: static (writer, state) =>
        {
            var (apiProperty, writeContext) = state;
            WriteApiPropertyApiName(writer, apiProperty, writeContext);
            WriteApiPropertyApiTypeExpression(writer, apiProperty, writeContext);
            WriteApiPropertyApiTypeModifiers(writer, apiProperty, writeContext);
            WriteApiPropertyClrName(writer, apiProperty, writeContext);
            WriteApiPropertyClrMemberKind(writer, apiProperty, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiProperty,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiPropertyApiName(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiProperty.ApiName, value: apiProperty.ApiName, options: writeContext.Options);

    private static void WriteApiPropertyApiTypeExpression(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiProperty.ApiTypeExpression, obj: apiProperty.ApiTypeExpression, options: writeContext.Options);

    private static void WriteApiPropertyApiTypeModifiers(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiProperty.ApiTypeModifiers, value: apiProperty.ApiTypeModifiers, options: writeContext.Options, converter: _apiTypeModifiersJsonConverter);

    private static void WriteApiPropertyClrName(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiProperty.ClrName, value: apiProperty.ClrName, options: writeContext.Options);

    private static void WriteApiPropertyClrMemberKind(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiProperty.ClrMemberKind, value: apiProperty.ClrMemberKind, options: writeContext.Options, converter: _clrMemberKindJsonConverter);
    #endregion
}
