// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Handles JSON serialization for <see cref="ApiRelationshipDependentEnd"/> instances.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipDependentEndJsonConverter(ILogger<ApiRelationshipDependentEndJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipDependentEnd>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for the <see cref="ApiRelationshipElement"/> members under a given
    ///     naming policy.
    /// </summary>
    private readonly record struct ApiRelationshipElementPropertyNames
    {
        #region Immutable Properties
        public required string ApiObjectTypeReference { get; init; }
        #endregion
    }

    /// <summary>
    ///     Stores the resolved JSON property names for the <see cref="ApiRelationshipDependentEnd"/> members under a
    ///     given naming policy.
    /// </summary>
    private readonly record struct ApiRelationshipDependentEndPropertyNames
    {
        #region Immutable Properties
        public required string ApiForeignKey { get; init; }
        public required string ApiTraversal { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiRelationshipElementPropertyNames ApiRelationshipElement { get; init; }
        public required ApiRelationshipDependentEndPropertyNames ApiRelationshipDependentEnd { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationshipElement = new ApiRelationshipElementPropertyNames
                {
                    ApiObjectTypeReference = policy.ConvertName(nameof(Relationships.ApiRelationshipElement.ApiObjectType)), // Mapping property name from ApiObjectTypeReference to ApiObjectType by design
                },
                ApiRelationshipDependentEnd = new ApiRelationshipDependentEndPropertyNames
                {
                    ApiForeignKey = policy.ConvertName(nameof(Relationships.ApiRelationshipDependentEnd.ApiForeignKey)),
                    ApiTraversal = policy.ConvertName(nameof(ApiRelationshipEnd.ApiTraversal)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading relationship element members from JSON.
    /// </summary>
    private class ApiRelationshipElementReadData
    {
        #region Properties
        public ApiTypeReference? ApiObjectTypeReference { get; set; }
        #endregion
    }

    /// <summary>
    ///     Temporary storage used while reading dependent end members from JSON.
    /// </summary>
    private class ApiRelationshipDependentEndReadData
    {
        #region Properties
        public ApiKeyDefinition? ApiForeignKey { get; set; }
        public ApiRelationshipTraversal? ApiTraversal { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiRelationshipDependentEnd"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiRelationshipElementReadData? ApiRelationshipElement { get; set; }
        public ApiRelationshipDependentEndReadData? ApiRelationshipDependentEnd { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiRelationshipElement Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiRelationshipElement Property Handlers
            { propertyNames.ApiRelationshipElement.ApiObjectTypeReference, HandleApiRelationshipElementApiObjectTypeReference },

            // ApiRelationshipDependentEnd Property Handlers
            { propertyNames.ApiRelationshipDependentEnd.ApiForeignKey, HandleApiRelationshipDependentEndApiForeignKey },
            { propertyNames.ApiRelationshipDependentEnd.ApiTraversal, HandleApiRelationshipDependentEndApiTraversal },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiRelationshipElement Methods
        private static void HandleApiRelationshipElementApiObjectTypeReference(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipElement ??= new ApiRelationshipElementReadData();

            context.ReadData.ApiRelationshipElement.ApiObjectTypeReference =
                JsonSerializer.Deserialize<ApiTypeReference>(ref reader, context.Options);
        }
        #endregion

        #region ApiRelationshipDependentEnd Methods
        private static void HandleApiRelationshipDependentEndApiForeignKey(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipDependentEnd ??= new ApiRelationshipDependentEndReadData();

            context.ReadData.ApiRelationshipDependentEnd.ApiForeignKey =
                JsonSerializer.Deserialize<ApiKeyDefinition>(ref reader, context.Options);
        }

        private static void HandleApiRelationshipDependentEndApiTraversal(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipDependentEnd ??= new ApiRelationshipDependentEndReadData();

            context.ReadData.ApiRelationshipDependentEnd.ApiTraversal =
                JsonSerializer.Deserialize<ApiRelationshipTraversal>(ref reader, context.Options);
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipDependentEndJsonConverter()
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
    protected override ApiRelationshipDependentEnd? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;

        var apiObjectTypeReference = readContext.ReadData.ApiRelationshipElement?.ApiObjectTypeReference;
        var apiForeignKey = readContext.ReadData.ApiRelationshipDependentEnd?.ApiForeignKey;

        var apiTraversal = readContext.ReadData.ApiRelationshipDependentEnd?.ApiTraversal;
        var apiRelationshipDependentEnd = new ApiRelationshipDependentEnd
        (
            apiObjectTypeReference!, apiTraversal, apiForeignKey
        );

        AttachExtensions(apiRelationshipDependentEnd, readContext.ReadData.Extensions);
        return apiRelationshipDependentEnd;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipDependentEnd apiRelationshipDependentEnd, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiRelationshipDependentEnd, writeContext), writeObject: static (writer, state) =>
        {
            var (apiRelationshipDependentEnd, writeContext) = state;
            WriteApiRelationshipElementApiObjectTypeReference(writer, apiRelationshipDependentEnd, writeContext);
            WriteApiRelationshipDependentEndApiForeignKey(writer, apiRelationshipDependentEnd, writeContext);
            WriteApiRelationshipDependentEndApiTraversal(writer, apiRelationshipDependentEnd, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiRelationshipDependentEnd,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiRelationshipElementApiObjectTypeReference(Utf8JsonWriter writer, ApiRelationshipDependentEnd apiRelationshipDependentEnd, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipElement.ApiObjectTypeReference, obj: apiRelationshipDependentEnd.ApiObjectTypeReference, options: writeContext.Options);

    private static void WriteApiRelationshipDependentEndApiForeignKey(Utf8JsonWriter writer, ApiRelationshipDependentEnd apiRelationshipDependentEnd, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipDependentEnd.ApiForeignKey, obj: apiRelationshipDependentEnd.HasForeignKey ? apiRelationshipDependentEnd.ApiForeignKey : null, options: writeContext.Options);

    private static void WriteApiRelationshipDependentEndApiTraversal(Utf8JsonWriter writer, ApiRelationshipDependentEnd apiRelationshipDependentEnd, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipDependentEnd.ApiTraversal, obj: apiRelationshipDependentEnd.ApiTraversal, options: writeContext.Options);
    #endregion
}
