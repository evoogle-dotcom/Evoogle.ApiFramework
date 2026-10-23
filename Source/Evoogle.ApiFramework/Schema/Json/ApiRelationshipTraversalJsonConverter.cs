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

/// <summary>Converts relationship traversals to and from schema JSON.</summary>
/// <param name="logger">The optional logger used during JSON operations.</param>
public class ApiRelationshipTraversalJsonConverter(ILogger<ApiRelationshipTraversalJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipTraversal>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiRelationshipTraversal"/> under a given naming policy.
    /// </summary>
    private readonly record struct ApiRelationshipTraversalPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ClrNavigationMember { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiRelationshipTraversalPropertyNames ApiRelationshipTraversal { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy) => new()
        {
            ApiRelationshipTraversal = new ApiRelationshipTraversalPropertyNames
            {
                ApiName = policy.ConvertName(nameof(Relationships.ApiRelationshipTraversal.ApiName)),
                ClrNavigationMember = policy.ConvertName(nameof(Relationships.ApiRelationshipTraversal.ClrNavigationMember)),
            },
            ExtensibleBase = GetExtensiblePropertyNames(policy),
        };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading traversal members from JSON.
    /// </summary>
    private class ApiRelationshipTraversalReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ClrMemberReference? ClrNavigationMember { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiRelationshipTraversal"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiRelationshipTraversalReadData? ApiRelationshipTraversal { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiRelationshipTraversal Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiRelationshipTraversal Property Handlers
            { propertyNames.ApiRelationshipTraversal.ApiName, HandleApiRelationshipTraversalApiName },
            { propertyNames.ApiRelationshipTraversal.ClrNavigationMember, HandleApiRelationshipTraversalClrNavigationMember },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiRelationshipTraversal Methods
        private static void HandleApiRelationshipTraversalApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipTraversal ??= new ApiRelationshipTraversalReadData();

            context.ReadData.ApiRelationshipTraversal.ApiName = reader.GetString();
        }

        private static void HandleApiRelationshipTraversalClrNavigationMember(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipTraversal ??= new ApiRelationshipTraversalReadData();

            context.ReadData.ApiRelationshipTraversal.ClrNavigationMember =
                JsonSerializer.Deserialize<ClrMemberReference>(ref reader, context.Options);
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipTraversalJsonConverter()
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
    protected override ApiRelationshipTraversal? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var apiRelationshipTraversalReadData = readContext.ReadData.ApiRelationshipTraversal;

        var apiName = apiRelationshipTraversalReadData?.ApiName;
        var clrNavigationMember = apiRelationshipTraversalReadData?.ClrNavigationMember;

        var apiRelationshipTraversal = new ApiRelationshipTraversal(apiName!, clrNavigationMember);
        AttachExtensions(apiRelationshipTraversal, readContext.ReadData.Extensions);

        return apiRelationshipTraversal;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipTraversal apiRelationshipTraversal, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiRelationshipTraversal, writeContext), writeObject: static (writer, state) =>
        {
            var (apiRelationshipTraversal, writeContext) = state;
            WriteApiRelationshipTraversalApiName(writer, apiRelationshipTraversal, writeContext);
            WriteApiRelationshipTraversalClrNavigationMember(writer, apiRelationshipTraversal, writeContext);
            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiRelationshipTraversal,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiRelationshipTraversalApiName(Utf8JsonWriter writer, ApiRelationshipTraversal apiRelationshipTraversal, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiRelationshipTraversal.ApiName, value: apiRelationshipTraversal.ApiName, options: writeContext.Options);

    private static void WriteApiRelationshipTraversalClrNavigationMember(Utf8JsonWriter writer, ApiRelationshipTraversal apiRelationshipTraversal, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipTraversal.ClrNavigationMember, obj: apiRelationshipTraversal.ClrNavigationMember, options: writeContext.Options);
    #endregion
}
