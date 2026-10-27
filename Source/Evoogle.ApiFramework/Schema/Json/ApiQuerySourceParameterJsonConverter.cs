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
        public required string ApiNullability { get; init; }
        public required string ApiPresence { get; init; }
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
                ApiNullability = policy.ConvertName(nameof(Query.ApiQuerySourceParameter.ApiNullability)),
                ApiPresence = policy.ConvertName(nameof(Query.ApiQuerySourceParameter.ApiPresence)),
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
        public JsonEnumReadResult<ApiNullability> ApiNullability { get; set; }
        public JsonEnumReadResult<ApiPresence> ApiPresence { get; set; }
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
            { propertyNames.ApiQuerySourceParameter.ApiNullability, HandleApiQuerySourceParameterApiNullability, true },
            { propertyNames.ApiQuerySourceParameter.ApiPresence, HandleApiQuerySourceParameterApiPresence, true },

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

        private static void HandleApiQuerySourceParameterApiNullability(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            context.ReadData.ApiQuerySourceParameter.ApiNullability =
                JsonEnumReader.Read<ApiNullability>(ref reader, context.Options);
        }

        private static void HandleApiQuerySourceParameterApiPresence(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiQuerySourceParameter ??= new ApiQuerySourceParameterReadData();

            context.ReadData.ApiQuerySourceParameter.ApiPresence =
                JsonEnumReader.Read<ApiPresence>(ref reader, context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiNullability> _apiNullabilityJsonConverter = new();
    private static readonly EnumJsonConverter<ApiPresence> _apiPresenceJsonConverter = new();
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
        var apiNullabilityReadResult = apiQuerySourceParameterReadData?.ApiNullability ?? default;
        var apiNullability = apiNullabilityReadResult.ValueOr(ApiSchemaDefaults.QuerySourceParameterNullability);
        var apiPresenceReadResult = apiQuerySourceParameterReadData?.ApiPresence ?? default;
        var apiPresence = apiPresenceReadResult.ValueOr(ApiSchemaDefaults.QuerySourceParameterPresence);

        var apiQuerySourceParameter = new ApiQuerySourceParameter
        (
            apiName!,
            apiTypeExpression!,
            apiNullability,
            apiPresence
        );

        apiQuerySourceParameter.RecordJsonEnumFinding(nameof(ApiQuerySourceParameter.ApiNullability), apiNullabilityReadResult);
        apiQuerySourceParameter.RecordJsonEnumFinding(nameof(ApiQuerySourceParameter.ApiPresence), apiPresenceReadResult);

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
            WriteApiQuerySourceParameterApiNullability(writer, apiQuerySourceParameter, writeContext);
            WriteApiQuerySourceParameterApiPresence(writer, apiQuerySourceParameter, writeContext);

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

    private static void WriteApiQuerySourceParameterApiNullability(Utf8JsonWriter writer, ApiQuerySourceParameter apiQuerySourceParameter, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter
        (
            propertyName: writeContext.PropertyNames.ApiQuerySourceParameter.ApiNullability,
            value: apiQuerySourceParameter.ApiNullability,
            options: writeContext.Options,
            converter: _apiNullabilityJsonConverter,
            defaultValue: ApiSchemaDefaults.QuerySourceParameterNullability
        );

    private static void WriteApiQuerySourceParameterApiPresence(Utf8JsonWriter writer, ApiQuerySourceParameter apiQuerySourceParameter, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter
        (
            propertyName: writeContext.PropertyNames.ApiQuerySourceParameter.ApiPresence,
            value: apiQuerySourceParameter.ApiPresence,
            options: writeContext.Options,
            converter: _apiPresenceJsonConverter,
            defaultValue: ApiSchemaDefaults.QuerySourceParameterPresence
        );
    #endregion
}
