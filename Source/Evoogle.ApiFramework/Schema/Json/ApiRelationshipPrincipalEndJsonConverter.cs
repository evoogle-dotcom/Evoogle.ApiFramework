// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Handles JSON serialization for <see cref="ApiRelationshipPrincipalEnd"/> instances.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipPrincipalEndJsonConverter(ILogger<ApiRelationshipPrincipalEndJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipPrincipalEnd>(logger)
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
    ///     Stores the resolved JSON property names for the <see cref="ApiRelationshipPrincipalEnd"/> members under a
    ///     given naming policy.
    /// </summary>
    private readonly record struct ApiRelationshipPrincipalEndPropertyNames
    {
        #region Immutable Properties
        public required string ApiPrincipalKeyName { get; init; }
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
        public required ApiRelationshipPrincipalEndPropertyNames ApiRelationshipPrincipalEnd { get; init; }
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
                ApiRelationshipPrincipalEnd = new ApiRelationshipPrincipalEndPropertyNames
                {
                    ApiPrincipalKeyName = policy.ConvertName(nameof(Relationships.ApiRelationshipPrincipalEnd.ApiPrincipalKeyName)),
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
    ///     Temporary storage used while reading principal end members from JSON.
    /// </summary>
    private class ApiRelationshipPrincipalEndReadData
    {
        #region Properties
        public string? ApiPrincipalKeyName { get; set; }
        public ApiRelationshipTraversal? ApiTraversal { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiRelationshipPrincipalEnd"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiRelationshipElementReadData? ApiRelationshipElement { get; set; }
        public ApiRelationshipPrincipalEndReadData? ApiRelationshipPrincipalEnd { get; set; }
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

            // ApiRelationshipPrincipalEnd Property Handlers
            { propertyNames.ApiRelationshipPrincipalEnd.ApiPrincipalKeyName, HandleApiRelationshipPrincipalEndApiPrincipalKeyName },
            { propertyNames.ApiRelationshipPrincipalEnd.ApiTraversal, HandleApiRelationshipPrincipalEndApiTraversal },

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

        #region ApiRelationshipPrincipalEnd Methods
        private static void HandleApiRelationshipPrincipalEndApiPrincipalKeyName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipPrincipalEnd ??= new ApiRelationshipPrincipalEndReadData();

            context.ReadData.ApiRelationshipPrincipalEnd.ApiPrincipalKeyName = reader.GetString();
        }

        private static void HandleApiRelationshipPrincipalEndApiTraversal(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipPrincipalEnd ??= new ApiRelationshipPrincipalEndReadData();

            context.ReadData.ApiRelationshipPrincipalEnd.ApiTraversal =
                JsonSerializer.Deserialize<ApiRelationshipTraversal>(ref reader, context.Options);
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipPrincipalEndJsonConverter()
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
    protected override ApiRelationshipPrincipalEnd? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;

        var apiObjectTypeReference = readContext.ReadData.ApiRelationshipElement?.ApiObjectTypeReference;
        var apiPrincipalKeyName = readContext.ReadData.ApiRelationshipPrincipalEnd?.ApiPrincipalKeyName;

        var apiRelationshipPrincipalEnd = new ApiRelationshipPrincipalEnd
            (
                apiObjectTypeReference!,
                readContext.ReadData.ApiRelationshipPrincipalEnd?.ApiTraversal,
                apiPrincipalKeyName
            );

        AttachExtensions(apiRelationshipPrincipalEnd, readContext.ReadData.Extensions);
        return apiRelationshipPrincipalEnd;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd apiRelationshipPrincipalEnd, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiRelationshipPrincipalEnd, writeContext), writeObject: static (writer, state) =>
        {
            var (apiRelationshipPrincipalEnd, writeContext) = state;
            WriteApiRelationshipElementApiObjectTypeReference(writer, apiRelationshipPrincipalEnd, writeContext);
            WriteApiRelationshipPrincipalEndApiPrincipalKeyName(writer, apiRelationshipPrincipalEnd, writeContext);
            WriteApiRelationshipPrincipalEndApiTraversal(writer, apiRelationshipPrincipalEnd, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiRelationshipPrincipalEnd,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiRelationshipElementApiObjectTypeReference(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd apiRelationshipPrincipalEnd, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipElement.ApiObjectTypeReference, obj: apiRelationshipPrincipalEnd.ApiObjectTypeReference, options: writeContext.Options);

    private static void WriteApiRelationshipPrincipalEndApiPrincipalKeyName(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd apiRelationshipPrincipalEnd, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiRelationshipPrincipalEnd.ApiPrincipalKeyName, value: apiRelationshipPrincipalEnd.ApiPrincipalKeyName, options: writeContext.Options);

    private static void WriteApiRelationshipPrincipalEndApiTraversal(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd apiRelationshipPrincipalEnd, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipPrincipalEnd.ApiTraversal, obj: apiRelationshipPrincipalEnd.ApiTraversal, options: writeContext.Options);
    #endregion
}
