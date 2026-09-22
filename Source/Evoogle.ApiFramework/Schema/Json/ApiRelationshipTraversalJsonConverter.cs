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
    private readonly record struct PropertyNames
    {
        public required string ApiName { get; init; }
        public required string ClrMemberReference { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy) => new()
        {
            ApiName = policy.ConvertName(nameof(ApiRelationshipTraversal.ApiName)),
            ClrMemberReference = policy.ConvertName
            (
                nameof(ApiRelationshipTraversal.ClrMemberReference)
            ),
            ExtensibleBase = GetExtensiblePropertyNames(policy)
        };
    }
    #endregion

    #region Read Types
    private sealed class ReadState : ExtensibleReadData
    {
        public string? ApiName { get; set; }
        public ApiClrMemberReference? ClrMemberReference { get; set; }
    }

    private sealed class ReadHandlers(PropertyNames names)
    {
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            { names.ApiName, HandleApiName },
            { names.ClrMemberReference, HandleClrMemberReference },
            {
                names.ExtensibleBase.Extensions,
                CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>()
            }
        };

        private static void HandleApiName
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        ) => context.ReadData.ApiName = reader.GetString();

        private static void HandleClrMemberReference
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        ) => context.ReadData.ClrMemberReference =
            JsonSerializer.Deserialize<ApiClrMemberReference>(ref reader, context.Options);
    }
    #endregion

    #region Constructors
    /// <summary>Creates a converter for use by the JSON converter attribute.</summary>
    public ApiRelationshipTraversalJsonConverter() : this(null)
    { }
    #endregion

    #region JsonConverterBase<T> Methods
    /// <inheritdoc/>
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultReadContext<PropertyNames, ReadState, ReadHandlers>
        (
            logger, options,
            buildPropertyNames: PropertyNames.Create,
            buildReadHandlers: names => new ReadHandlers(names)
        );

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext
    (
        ILogger logger,
        JsonSerializerOptions options
    )
        => CreateDefaultWriteContext(logger, options, buildPropertyNames: PropertyNames.Create);

    /// <inheritdoc/>
    protected override ApiRelationshipTraversal? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var state = readContext.ReadData;
        var traversal = new ApiRelationshipTraversal(state.ApiName!, state.ClrMemberReference);
        AttachExtensions(traversal, state.Extensions);
        return traversal;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore
    (
        Utf8JsonWriter writer,
        ApiRelationshipTraversal apiRelationshipTraversal,
        IWriteContext context
    )
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;
        writer.WriteJsonObject(state: (apiRelationshipTraversal, writeContext), writeObject: static (writer, state) =>
        {
            var (apiRelationshipTraversal, writeContext) = state;
            WriteApiName(writer, apiRelationshipTraversal, writeContext);
            WriteClrMemberReference(writer, apiRelationshipTraversal, writeContext);
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
    private static void WriteApiName(Utf8JsonWriter writer, ApiRelationshipTraversal apiRelationshipTraversal, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiName, value: apiRelationshipTraversal.ApiName, options: writeContext.Options);

    private static void WriteClrMemberReference
    (
        Utf8JsonWriter writer,
        ApiRelationshipTraversal apiRelationshipTraversal,
        DefaultWriteContext<PropertyNames> writeContext
    ) => writer.TryWritePropertyWithSerializer
    (
        propertyName: writeContext.PropertyNames.ClrMemberReference,
        obj: apiRelationshipTraversal.ClrMemberReference,
        options: writeContext.Options
    );
    #endregion
}
