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

/// <summary>Converts ApiQuerySourceParameter schema declarations to and from JSON.</summary>
/// <param name="logger">The optional serialization logger.</param>
public class ApiQuerySourceParameterJsonConverter(ILogger<ApiQuerySourceParameterJsonConverter>? logger)
    : JsonConverterBase<ApiQuerySourceParameter>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiQuerySourceParameter"/> under a given naming policy.
    /// </summary>
    private readonly record struct ApiQuerySourceParameterPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiTypeExpression { get; init; }
        public required string ApiTypeModifiers { get; init; }
        public required string ApiRequiresArgument { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
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
                ApiRequiresArgument = policy.ConvertName(nameof(Query.ApiQuerySourceParameter.ApiRequiresArgument)),
            },
            ExtensibleBase = GetExtensiblePropertyNames(policy),
        };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading query source parameter members from JSON.
    /// </summary>
    private class ApiQuerySourceParameterReadData
    {
        #region Properties
        public string? ApiName { get; set; } = null;
        public ApiTypeExpression? ApiTypeExpression { get; set; } = null;
        public JsonEnumReadState<ApiTypeModifiers>? ApiTypeModifiers { get; set; }
        public bool? ApiRequiresArgument { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiQuerySourceParameter"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiQuerySourceParameterReadData? ApiQuerySourceParameter { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiQuerySourceParameter Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiQuerySourceParameter Property Handlers
            { propertyNames.ApiQuerySourceParameter.ApiName, HandleApiQuerySourceParameterApiName },
            { propertyNames.ApiQuerySourceParameter.ApiTypeExpression, HandleApiQuerySourceParameterApiTypeExpression },
            { propertyNames.ApiQuerySourceParameter.ApiTypeModifiers, HandleApiQuerySourceParameterApiTypeModifiers },
            { propertyNames.ApiQuerySourceParameter.ApiRequiresArgument, HandleApiQuerySourceParameterApiRequiresArgument },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiQuerySourceParameter Methods
        private static void HandleApiQuerySourceParameterApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            context.ReadData.ApiQuerySourceParameter.ApiName = reader.GetString();
        }

        private static void HandleApiQuerySourceParameterApiTypeExpression(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            var options = context.Options;
            context.ReadData.ApiQuerySourceParameter.ApiTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, options);
        }

        private static void HandleApiQuerySourceParameterApiTypeModifiers(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            var options = context.Options;
            context.ReadData.ApiQuerySourceParameter.ApiTypeModifiers ??= new JsonEnumReadState<ApiTypeModifiers>();
            context.ReadData.ApiQuerySourceParameter.ApiTypeModifiers.Read(ref reader, options, _nullableApiTypeModifiersJsonConverter);
        }

        private static void HandleApiQuerySourceParameterApiRequiresArgument(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            context.ReadData.ApiQuerySourceParameter.ApiRequiresArgument = reader.GetBoolean();
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeModifiers> _apiTypeModifiersJsonConverter = new();
    private static readonly NullableEnumJsonConverter<ApiTypeModifiers> _nullableApiTypeModifiersJsonConverter =
        new(EnumJsonInvalidValuePolicy.ReturnNull);
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiQuerySourceParameterJsonConverter()
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
    protected override ApiQuerySourceParameter? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var apiQuerySourceParameterReadData = readContext.ReadData.ApiQuerySourceParameter;

        var apiName = apiQuerySourceParameterReadData?.ApiName;
        var apiTypeExpression = apiQuerySourceParameterReadData?.ApiTypeExpression;
        var apiTypeModifiersReadState = apiQuerySourceParameterReadData?.ApiTypeModifiers;
        var apiTypeModifiers = apiTypeModifiersReadState?.Value
            ?? ApiSchemaDefaults.NonNullableTypeModifiers;
        var apiRequiresArgument = apiQuerySourceParameterReadData?.ApiRequiresArgument
            ?? ApiSchemaDefaults.QuerySourceParameterRequiresArgument;

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
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
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
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiQuerySourceParameter.ApiTypeModifiers, value: apiQuerySourceParameter.ApiTypeModifiers, options: writeContext.Options, converter: _apiTypeModifiersJsonConverter);

    private static void WriteApiQuerySourceParameterApiRequiresArgument(Utf8JsonWriter writer, ApiQuerySourceParameter apiQuerySourceParameter, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsBoolean(propertyName: writeContext.PropertyNames.ApiQuerySourceParameter.ApiRequiresArgument, value: apiQuerySourceParameter.ApiRequiresArgument, options: writeContext.Options);
    #endregion
}
