// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>Converts ApiQuerySource schema declarations to and from JSON.</summary>
/// <param name="logger">The optional serialization logger.</param>
public class ApiQuerySourceJsonConverter(ILogger<ApiQuerySourceJsonConverter>? logger)
    : JsonConverterBase<ApiQuerySource>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiQuerySource"/> under a given naming policy.
    /// </summary>
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

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
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
            ExtensibleBase = GetExtensiblePropertyNames(policy),
        };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading query source members from JSON.
    /// </summary>
    private class ApiQuerySourceReadData
    {
        #region Properties
        public string? ApiName { get; set; } = null;
        public List<ApiQuerySourceParameter>? ApiInputParameters { get; set; } = null;
        public ApiTypeExpression? ApiOutputTypeExpression { get; set; } = null;
        public JsonEnumReadResult<ApiTypeModifiers> ApiOutputTypeModifiers { get; set; }
        public JsonEnumReadResult<ApiQueryComposition> ApiOutputComposition { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiQuerySource"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiQuerySourceReadData? ApiQuerySource { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiQuerySource Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiQuerySource Property Handlers
            { propertyNames.ApiQuerySource.ApiName, HandleApiQuerySourceApiName },
            { propertyNames.ApiQuerySource.ApiInputParameters, HandleApiQuerySourceApiInputParameters },
            { propertyNames.ApiQuerySource.ApiOutputTypeExpression, HandleApiQuerySourceApiOutputTypeExpression },
            { propertyNames.ApiQuerySource.ApiOutputTypeModifiers,             HandleApiQuerySourceApiOutputTypeModifiers },
                        { propertyNames.ApiQuerySource.ApiOutputComposition, HandleApiQuerySourceApiOutputComposition },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiQuerySource Methods
        private static void HandleApiQuerySourceApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            context.ReadData.ApiQuerySource.ApiName = reader.GetString();
        }

        private static void HandleApiQuerySourceApiInputParameters(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiQuerySource.ApiInputParameters;
            var apiInputParameters = DeserializeListOf<ApiQuerySourceParameter, ApiQuerySourceParameter>(ref reader, options, propertyName);

            context.ReadData.ApiQuerySource.ApiInputParameters = apiInputParameters;
        }

        private static void HandleApiQuerySourceApiOutputTypeExpression(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            var options = context.Options;
            context.ReadData.ApiQuerySource.ApiOutputTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, options);
        }

        private static void HandleApiQuerySourceApiOutputTypeModifiers(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            context.ReadData.ApiQuerySource.ApiOutputTypeModifiers =
                JsonEnumReader.Read<ApiTypeModifiers>(ref reader, context.Options);
        }

        private static void HandleApiQuerySourceApiOutputComposition(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySource ??= new ApiQuerySourceReadData();

            context.ReadData.ApiQuerySource.ApiOutputComposition =
                JsonEnumReader.Read<ApiQueryComposition>(ref reader, context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeModifiers> _apiTypeModifiersJsonConverter = new();

    private static readonly EnumJsonConverter<ApiQueryComposition> _apiQueryCompositionJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiQuerySourceJsonConverter()
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
    protected override ApiQuerySource? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var apiQuerySourceReadData = readContext.ReadData.ApiQuerySource;

        var apiName = apiQuerySourceReadData?.ApiName;
        var apiInputParameters = apiQuerySourceReadData?.ApiInputParameters;
        var apiOutputTypeExpression = apiQuerySourceReadData?.ApiOutputTypeExpression;

        var apiOutputTypeModifiersReadResult = apiQuerySourceReadData?.ApiOutputTypeModifiers ?? default;
        var apiOutputTypeModifiers = apiOutputTypeModifiersReadResult.ValueOr(ApiSchemaDefaults.NullableTypeModifiers);

        var apiOutputCompositionReadResult = apiQuerySourceReadData?.ApiOutputComposition ?? default;
        var apiOutputComposition = apiOutputCompositionReadResult.ValueOr(ApiSchemaDefaults.QuerySourceOutputComposition);

        var apiQuerySource = new ApiQuerySource
        (
            apiName!,
            apiInputParameters,
            apiOutputTypeExpression!,
            apiOutputTypeModifiers,
            apiOutputComposition
        );

        apiQuerySource.RecordJsonEnumFinding(nameof(ApiQuerySource.ApiOutputTypeModifiers), apiOutputTypeModifiersReadResult);
        apiQuerySource.RecordJsonEnumFinding(nameof(ApiQuerySource.ApiOutputComposition), apiOutputCompositionReadResult);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiQuerySource, extensions);

        return apiQuerySource;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
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
        => writer.TryWritePropertyWithConverter
        (
            propertyName: writeContext.PropertyNames.ApiQuerySource.ApiOutputTypeModifiers,
            value: apiQuerySource.ApiOutputTypeModifiers,
            options: writeContext.Options,
            converter: _apiTypeModifiersJsonConverter,
            defaultValue: ApiSchemaDefaults.NullableTypeModifiers
        );

    private static void WriteApiQuerySourceApiOutputComposition(Utf8JsonWriter writer, ApiQuerySource apiQuerySource, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter
        (
            propertyName: writeContext.PropertyNames.ApiQuerySource.ApiOutputComposition,
            value: apiQuerySource.ApiOutputComposition,
            options: writeContext.Options,
            converter: _apiQueryCompositionJsonConverter,
            defaultValue: ApiSchemaDefaults.QuerySourceOutputComposition
        );
    #endregion
}
