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
///     Handles JSON serialization for <see cref="ApiRelationshipAssociation"/> instances.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipAssociationJsonConverter(ILogger<ApiRelationshipAssociationJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipAssociation>(logger)
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
    ///     Stores the resolved JSON property names for the <see cref="ApiRelationshipAssociation"/> members under a
    ///     given naming policy.
    /// </summary>
    private readonly record struct ApiRelationshipAssociationPropertyNames
    {
        #region Immutable Properties
        public required string ApiForeignKeyA { get; init; }
        public required string ApiForeignKeyB { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiRelationshipElementPropertyNames ApiRelationshipElement { get; init; }
        public required ApiRelationshipAssociationPropertyNames ApiRelationshipAssociation { get; init; }
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
                ApiRelationshipAssociation = new ApiRelationshipAssociationPropertyNames
                {
                    ApiForeignKeyA = policy.ConvertName(nameof(Relationships.ApiRelationshipAssociation.ApiForeignKeyA)),
                    ApiForeignKeyB = policy.ConvertName(nameof(Relationships.ApiRelationshipAssociation.ApiForeignKeyB)),
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
    ///     Temporary storage used while reading relationship association members from JSON.
    /// </summary>
    private class ApiRelationshipAssociationReadData
    {
        #region Properties
        public ApiKeyDefinition? ApiForeignKeyA { get; set; }
        public ApiKeyDefinition? ApiForeignKeyB { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiRelationshipAssociation"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiRelationshipElementReadData? ApiRelationshipElement { get; set; }
        public ApiRelationshipAssociationReadData? ApiRelationshipAssociation { get; set; }
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

            // ApiRelationshipAssociation Property Handlers
            { propertyNames.ApiRelationshipAssociation.ApiForeignKeyA, HandleApiRelationshipAssociationApiForeignKeyA },
            { propertyNames.ApiRelationshipAssociation.ApiForeignKeyB, HandleApiRelationshipAssociationApiForeignKeyB },

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

        #region ApiRelationshipAssociation Methods
        private static void HandleApiRelationshipAssociationApiForeignKeyA(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipAssociation ??= new ApiRelationshipAssociationReadData();

            context.ReadData.ApiRelationshipAssociation.ApiForeignKeyA =
                JsonSerializer.Deserialize<ApiKeyDefinition>(ref reader, context.Options);
        }

        private static void HandleApiRelationshipAssociationApiForeignKeyB(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipAssociation ??= new ApiRelationshipAssociationReadData();

            context.ReadData.ApiRelationshipAssociation.ApiForeignKeyB =
                JsonSerializer.Deserialize<ApiKeyDefinition>(ref reader, context.Options);
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipAssociationJsonConverter()
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
    protected override ApiRelationshipAssociation? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;

        var apiObjectTypeReference = readContext.ReadData.ApiRelationshipElement?.ApiObjectTypeReference;
        var apiForeignKeyA = readContext.ReadData.ApiRelationshipAssociation?.ApiForeignKeyA;
        var apiForeignKeyB = readContext.ReadData.ApiRelationshipAssociation?.ApiForeignKeyB;

        var apiRelationshipAssociation = apiForeignKeyA != null && apiForeignKeyB != null
            ? new ApiRelationshipAssociation(apiObjectTypeReference!, apiForeignKeyA, apiForeignKeyB)
            : new ApiRelationshipAssociation(apiObjectTypeReference!);

        AttachExtensions(apiRelationshipAssociation, readContext.ReadData.Extensions);
        return apiRelationshipAssociation;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipAssociation apiRelationshipAssociation, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiRelationshipAssociation, writeContext), writeObject: static (writer, state) =>
        {
            var (apiRelationshipAssociation, writeContext) = state;
            WriteApiRelationshipElementApiObjectTypeReference(writer, apiRelationshipAssociation, writeContext);
            WriteApiRelationshipAssociationApiForeignKeyA(writer, apiRelationshipAssociation, writeContext);
            WriteApiRelationshipAssociationApiForeignKeyB(writer, apiRelationshipAssociation, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiRelationshipAssociation,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiRelationshipElementApiObjectTypeReference(Utf8JsonWriter writer, ApiRelationshipAssociation apiRelationshipAssociation, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipElement.ApiObjectTypeReference, obj: apiRelationshipAssociation.ApiObjectTypeReference, options: writeContext.Options);

    private static void WriteApiRelationshipAssociationApiForeignKeyA(Utf8JsonWriter writer, ApiRelationshipAssociation apiRelationshipAssociation, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipAssociation.ApiForeignKeyA, obj: apiRelationshipAssociation.HasForeignKeys ? apiRelationshipAssociation.ApiForeignKeyA : null, options: writeContext.Options);

    private static void WriteApiRelationshipAssociationApiForeignKeyB(Utf8JsonWriter writer, ApiRelationshipAssociation apiRelationshipAssociation, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipAssociation.ApiForeignKeyB, obj: apiRelationshipAssociation.HasForeignKeys ? apiRelationshipAssociation.ApiForeignKeyB : null, options: writeContext.Options);
    #endregion
}
