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

/// <summary>Converts ApiQuerySource schema declarations to and from JSON.</summary>
/// <param name="logger">The optional serialization logger.</param>
public sealed class ApiQuerySourceJsonConverter(ILogger<ApiQuerySourceJsonConverter>? logger)
    : JsonConverterBase<ApiQuerySource>(logger)
{
    #region Property Types
    private readonly record struct ApiQuerySourcePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiInputParameters { get; init; }
        public required string ApiOutputTypeExpression { get; init; }
        public required string ApiOutputTypeModifiers { get; init; }
        public required string ApiOutputComposition { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiQuerySourcePropertyNames ApiQuerySource { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy) => new()
        {
            ApiQuerySource = new ApiQuerySourcePropertyNames
            {
                ApiName = policy.ConvertName(nameof(Query.ApiQuerySource.ApiName)),
                ApiInputParameters = policy.ConvertName(nameof(Query.ApiQuerySource.ApiInputParameters)),
                ApiOutputTypeExpression = policy.ConvertName(nameof(Query.ApiQuerySource.ApiOutputType)), // Mapping property name from ApiOutputTypeExpression to ApiOutputType by design
                ApiOutputTypeModifiers = policy.ConvertName(nameof(Query.ApiQuerySource.ApiOutputTypeModifiers)),
                ApiOutputComposition = policy.ConvertName(nameof(Query.ApiQuerySource.ApiOutputComposition)),
            },
            ExtensibleBase = GetExtensiblePropertyNames(policy)
        };
        #endregion
    }
    #endregion

    #region Read Types
    private sealed class ApiQuerySourceReadData
    {
        #region Properties
        public string? ApiName { get; set; } = null;
        public List<ApiQuerySourceParameter>? ApiInputParameters { get; set; } = null;
        public ApiTypeExpression? ApiOutputTypeExpression { get; set; } = null;
        public JsonEnumReadState<ApiTypeModifiers>? ApiOutputTypeModifiers { get; set; }
        public JsonEnumReadState<ApiQueryComposition>? ApiOutputComposition { get; set; }
        #endregion
    }

    private sealed class ReadState : ExtensibleReadData
    {
        #region Properties
        public ApiQuerySourceReadData? ApiQuerySource { get; set; }
        #endregion
    }

    private sealed class ReadHandlers(PropertyNames names)
    {
        #region Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiQuerySourceParameter Property Handlers
            { names.ApiQuerySource.ApiName, HandleApiName },
            { names.ApiQuerySource.ApiInputParameters, HandleApiInputParameters },
            { names.ApiQuerySource.ApiOutputTypeExpression, HandleApiOutputTypeExpression },
            { names.ApiQuerySource.ApiOutputTypeModifiers, HandleApiOutputTypeModifiers },
            { names.ApiQuerySource.ApiOutputComposition, HandleApiOutputComposition },

            // ExtensibleBase Property Handlers
            { names.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() }
        };
        #endregion

        #region Methods
        private static void HandleApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            context.ReadData.ApiQuerySource.ApiName = reader.GetString();
        }

        private static void HandleApiInputParameters(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiQuerySource.ApiInputParameters;
            var apiInputParameters = DeserializeListOf<ApiQuerySourceParameter, ApiQuerySourceParameter>(ref reader, options, propertyName);

            context.ReadData.ApiQuerySource.ApiInputParameters = apiInputParameters;
        }

        private static void HandleApiOutputTypeExpression(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            var options = context.Options;
            context.ReadData.ApiQuerySource.ApiOutputTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, options);
        }

        private static void HandleApiOutputTypeModifiers(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            var options = context.Options;
            context.ReadData.ApiQuerySource.ApiOutputTypeModifiers ??= new JsonEnumReadState<ApiTypeModifiers>();
            context.ReadData.ApiQuerySource.ApiOutputTypeModifiers.Read(ref reader, options, _modifiersReader);
        }

        private static void HandleApiOutputComposition(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            var options = context.Options;
            context.ReadData.ApiQuerySource.ApiOutputComposition ??= new JsonEnumReadState<ApiQueryComposition>();
            context.ReadData.ApiQuerySource.ApiOutputComposition.Read(ref reader, options, _compositionReader);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeModifiers> _modifiersWriter = new();
    private static readonly NullableEnumJsonConverter<ApiTypeModifiers> _modifiersReader = new(EnumJsonInvalidValuePolicy.ReturnNull);

    private static readonly EnumJsonConverter<ApiQueryComposition> _compositionWriter = new();
    private static readonly NullableEnumJsonConverter<ApiQueryComposition> _compositionReader = new(EnumJsonInvalidValuePolicy.ReturnNull);
    #endregion

    #region Constructors
    /// <summary>Creates a converter for attribute-based serialization.</summary>
    public ApiQuerySourceJsonConverter() : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    /// <inheritdoc/>
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultReadContext<PropertyNames, ReadState, ReadHandlers>
            (logger, options, PropertyNames.Create, names => new ReadHandlers(names));

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext(logger, options, PropertyNames.Create);

    /// <inheritdoc/>
    protected override ApiQuerySource? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ApiQuerySource;

        var apiName = readState?.ApiName;
        var apiInputParameters = readState?.ApiInputParameters;
        var apiOutputTypeExpression = readState?.ApiOutputTypeExpression;

        var apiOutputTypeModifiersReadState = readState?.ApiOutputTypeModifiers;
        var apiOutputTypeModifiers = apiOutputTypeModifiersReadState?.Value ?? ApiTypeModifiers.None;

        var apiOutputCompositionReadState = readState?.ApiOutputComposition;
        var apiOutputComposition = apiOutputCompositionReadState?.Value ?? ApiQueryComposition.None;

        var apiQuerySource = new ApiQuerySource
        (
            apiName!,
            apiInputParameters,
            apiOutputTypeExpression!,
            apiOutputTypeModifiers,
            apiOutputComposition
        );

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiQuerySource, extensions);

        return apiQuerySource;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiQuerySource apiQuerySource, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiQuerySource, writeContext), writeObject: static (writer, state) =>
        {
            var (apiQuerySource, writeContext) = state;
            WriteApiQuerySourceApiName(writer, apiQuerySource, writeContext);
            WriteApiQuerySourceApiInputParameters(writer, apiQuerySource, writeContext);
            WriteApiQuerySourceApiOutputTypeExpression(writer, apiQuerySource, writeContext);
            WriteApiQuerySourceApiOutputTypeModifiers(writer, apiQuerySource, writeContext);
            WriteApiQuerySourceApiOutputComposition(writer, apiQuerySource, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiQuerySource,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiQuerySourceApiName(Utf8JsonWriter writer, ApiQuerySource apiQuerySource, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiQuerySource.ApiName, value: apiQuerySource.ApiName, options: writeContext.Options);

    private static void WriteApiQuerySourceApiInputParameters(Utf8JsonWriter writer, ApiQuerySource apiQuerySource, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ApiQuerySource.ApiInputParameters;
        var apiInputParameters = apiQuerySource.ApiInputParameters;
        var options = writeContext.Options;

        writer.TryWritePropertyAsArray
        (
            propertyName: propertyName,
            collection: apiInputParameters,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
                writer.TryWriteWithSerializer(obj: item, options: state.Options)
        );
    }

    private static void WriteApiQuerySourceApiOutputTypeExpression(Utf8JsonWriter writer, ApiQuerySource apiQuerySource, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiQuerySource.ApiOutputTypeExpression, obj: apiQuerySource.ApiOutputTypeExpression, options: writeContext.Options);

    private static void WriteApiQuerySourceApiOutputTypeModifiers(Utf8JsonWriter writer, ApiQuerySource apiQuerySource, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiQuerySource.ApiOutputTypeModifiers, value: apiQuerySource.ApiOutputTypeModifiers, options: writeContext.Options, converter: _modifiersWriter);

    private static void WriteApiQuerySourceApiOutputComposition(Utf8JsonWriter writer, ApiQuerySource apiQuerySource, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiQuerySource.ApiOutputComposition, value: apiQuerySource.ApiOutputComposition, options: writeContext.Options, converter: _compositionWriter);
    #endregion
}
