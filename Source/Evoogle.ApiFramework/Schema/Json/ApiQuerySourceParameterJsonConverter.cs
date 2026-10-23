// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>Converts ApiQuerySourceParameter schema declarations to and from JSON.</summary>
/// <param name="logger">The optional serialization logger.</param>
public sealed class ApiQuerySourceParameterJsonConverter(ILogger<ApiQuerySourceParameterJsonConverter>? logger)
    : JsonConverterBase<ApiQuerySourceParameter>(logger)
{
    #region Property Types
    private readonly record struct ApiQuerySourceParameterPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiTypeExpression { get; init; }
        public required string ApiTypeModifiers { get; init; }
        public required string ApiRequiresArgument { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiQuerySourceParameterPropertyNames ApiQuerySourceParameter { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy) => new()
        {
            ApiQuerySourceParameter = new ApiQuerySourceParameterPropertyNames
            {
                ApiName = policy.ConvertName(nameof(Query.ApiQuerySourceParameter.ApiName)),
                ApiTypeExpression = policy.ConvertName(nameof(Query.ApiQuerySourceParameter.ApiType)), // Mapping property name from ApiTypeExpression to ApiType by design
                ApiTypeModifiers = policy.ConvertName(nameof(Query.ApiQuerySourceParameter.ApiTypeModifiers)),
                ApiRequiresArgument = policy.ConvertName(nameof(Query.ApiQuerySourceParameter.ApiRequiresArgument))
            },
            ExtensibleBase = GetExtensiblePropertyNames(policy)
        };
        #endregion
    }
    #endregion

    #region Read Types
    private sealed class ApiQuerySourceParameterReadData
    {
        #region Properties
        public string? ApiName { get; set; } = null;
        public ApiTypeExpression? ApiTypeExpression { get; set; } = null;
        public JsonEnumReadState<ApiTypeModifiers>? ApiTypeModifiers { get; set; }
        public bool? ApiRequiresArgument { get; set; }
        #endregion
    }

    private sealed class ReadState : ExtensibleReadData
    {
        #region Properties
        public ApiQuerySourceParameterReadData? ApiQuerySourceParameter { get; set; }
        #endregion
    }

    private sealed class ReadHandlers(PropertyNames names)
    {
        #region Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiQuerySourceParameter Property Handlers
            { names.ApiQuerySourceParameter.ApiName, HandleApiName },
            { names.ApiQuerySourceParameter.ApiTypeExpression, HandleApiTypeExpression },
            { names.ApiQuerySourceParameter.ApiTypeModifiers, HandleApiTypeModifiers },
            { names.ApiQuerySourceParameter.ApiRequiresArgument, HandleApiRequiresArgument },

            // ExtensibleBase Property Handlers
            { names.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() }
        };
        #endregion

        #region Methods
        private static void HandleApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            context.ReadData.ApiQuerySourceParameter.ApiName = reader.GetString();
        }

        private static void HandleApiTypeExpression(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            var options = context.Options;
            context.ReadData.ApiQuerySourceParameter.ApiTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, options);
        }

        private static void HandleApiTypeModifiers(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            var options = context.Options;
            context.ReadData.ApiQuerySourceParameter.ApiTypeModifiers ??= new JsonEnumReadState<ApiTypeModifiers>();
            context.ReadData.ApiQuerySourceParameter.ApiTypeModifiers.Read(ref reader, options, _modifiersReader);
        }

        private static void HandleApiRequiresArgument(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            context.ReadData.ApiQuerySourceParameter.ApiRequiresArgument = reader.GetBoolean();
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeModifiers> _modifiersWriter = new();
    private static readonly NullableEnumJsonConverter<ApiTypeModifiers> _modifiersReader = new(EnumJsonInvalidValuePolicy.ReturnNull);
    #endregion

    #region Constructors
    /// <summary>Creates a converter for attribute-based serialization.</summary>
    public ApiQuerySourceParameterJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    /// <inheritdoc/>
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultReadContext<PropertyNames, ReadState, ReadHandlers>(logger, options, PropertyNames.Create, names => new ReadHandlers(names));

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext(logger, options, PropertyNames.Create);

    /// <inheritdoc/>
    protected override ApiQuerySourceParameter? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ApiQuerySourceParameter;

        var apiName = readState?.ApiName;
        var apiTypeExpression = readState?.ApiTypeExpression;
        var apiTypeModifiersReadState = readState?.ApiTypeModifiers;
        var apiTypeModifiers = apiTypeModifiersReadState?.Value ?? ApiTypeModifiers.None;
        var apiRequiresArgument = readState?.ApiRequiresArgument ?? false;

        var apiQuerySourceParameter = new ApiQuerySourceParameter
        (
            apiName!,
            apiTypeExpression!,
            apiTypeModifiers,
            apiRequiresArgument
        );

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiQuerySourceParameter, extensions);

        return apiQuerySourceParameter;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiQuerySourceParameter apiQuerySourceParameter, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiQuerySourceParameter, writeContext), writeObject: static (writer, state) =>
        {
            var (apiQuerySourceParameter, writeContext) = state;
            WriteApiQuerySourceParameterApiName(writer, apiQuerySourceParameter, writeContext);
            WriteApiQuerySourceParameterApiTypeExpression(writer, apiQuerySourceParameter, writeContext);
            WriteApiQuerySourceParameterApiTypeModifiers(writer, apiQuerySourceParameter, writeContext);
            WriteApiQuerySourceParameterApiRequiresArgument(writer, apiQuerySourceParameter, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiQuerySourceParameter,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiQuerySourceParameterApiName(Utf8JsonWriter writer, ApiQuerySourceParameter apiQuerySourceParameter, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiQuerySourceParameter.ApiName, value: apiQuerySourceParameter.ApiName, options: writeContext.Options);

    private static void WriteApiQuerySourceParameterApiTypeExpression(Utf8JsonWriter writer, ApiQuerySourceParameter apiQuerySourceParameter, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiQuerySourceParameter.ApiTypeExpression, obj: apiQuerySourceParameter.ApiTypeExpression, options: writeContext.Options);

    private static void WriteApiQuerySourceParameterApiTypeModifiers(Utf8JsonWriter writer, ApiQuerySourceParameter apiQuerySourceParameter, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiQuerySourceParameter.ApiTypeModifiers, value: apiQuerySourceParameter.ApiTypeModifiers, options: writeContext.Options, converter: _modifiersWriter);

    private static void WriteApiQuerySourceParameterApiRequiresArgument(Utf8JsonWriter writer, ApiQuerySourceParameter apiQuerySourceParameter, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsBoolean(propertyName: writeContext.PropertyNames.ApiQuerySourceParameter.ApiRequiresArgument, value: apiQuerySourceParameter.ApiRequiresArgument, options: writeContext.Options);
    #endregion
}
