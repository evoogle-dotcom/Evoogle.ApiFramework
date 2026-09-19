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
    private readonly record struct ApiRelationshipElementPropertyNames
    {
        public required string ApiObjectTypeReference { get; init; }
    }

    private readonly record struct ApiRelationshipDependentEndPropertyNames
    {
        public required string ApiForeignKey { get; init; }
        public required string ApiTraversal { get; init; }
    }

    private readonly record struct PropertyNames
    {
        public required ApiRelationshipElementPropertyNames ApiRelationshipElement { get; init; }
        public required ApiRelationshipDependentEndPropertyNames ApiRelationshipDependentEnd { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

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
                    ApiTraversal = policy.ConvertName
                    (
                        nameof(Relationships.ApiRelationshipEnd.ApiTraversal)
                    ),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
    }
    #endregion

    #region Read Types
    private class ApiRelationshipElementReadData
    {
        public ApiTypeReference? ApiObjectTypeReference { get; set; }
    }

    private class ApiRelationshipDependentEndReadData
    {
        public ApiKeyDefinition? ApiForeignKey { get; set; }
        public ApiRelationshipTraversal? ApiTraversal { get; set; }
    }

    private class ReadState : ExtensibleReadData
    {
        public ApiRelationshipElementReadData? ApiRelationshipElement { get; set; }
        public ApiRelationshipDependentEndReadData? ApiRelationshipDependentEnd { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipElement.ApiObjectTypeReference, HandleApiObjectTypeReference },
            { propertyNames.ApiRelationshipDependentEnd.ApiForeignKey, HandleApiForeignKey },
            { propertyNames.ApiRelationshipDependentEnd.ApiTraversal, HandleApiTraversal },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() },
        };

        private static void HandleApiObjectTypeReference(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipElement ??= new ApiRelationshipElementReadData();
            context.ReadData.ApiRelationshipElement.ApiObjectTypeReference =
                JsonSerializer.Deserialize<ApiTypeReference>(ref reader, context.Options);
        }

        private static void HandleApiForeignKey(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipDependentEnd ??= new ApiRelationshipDependentEndReadData();
            context.ReadData.ApiRelationshipDependentEnd.ApiForeignKey = JsonSerializer.Deserialize<ApiKeyDefinition>(ref reader, context.Options);
        }

        private static void HandleApiTraversal
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        )
        {
            context.ReadData.ApiRelationshipDependentEnd ??=
                new ApiRelationshipDependentEndReadData();
            context.ReadData.ApiRelationshipDependentEnd.ApiTraversal =
                JsonSerializer.Deserialize<ApiRelationshipTraversal>(ref reader, context.Options);
        }
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
        => CreateDefaultReadContext<PropertyNames, ReadState, ReadHandlers>
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create,
                buildReadHandlers: names => new ReadHandlers(names)
            );

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext(logger, options, buildPropertyNames: PropertyNames.Create);

    /// <inheritdoc/>
    protected override ApiRelationshipDependentEnd? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;

        var apiObjectTypeReference = readContext.ReadData.ApiRelationshipElement?.ApiObjectTypeReference;
        var apiForeignKey = readContext.ReadData.ApiRelationshipDependentEnd?.ApiForeignKey;

        var apiTraversal = readContext.ReadData.ApiRelationshipDependentEnd?.ApiTraversal;
        var end = new ApiRelationshipDependentEnd
        (
            apiObjectTypeReference!, apiTraversal, apiForeignKey
        );

        AttachExtensions(end, readContext.ReadData.Extensions);
        return end;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipDependentEnd apiRelationshipDependentEnd, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiRelationshipDependentEnd, writeContext), writeObject: static (writer, state) =>
        {
            var (apiRelationshipDependentEnd, writeContext) = state;
            WriteApiObjectTypeReference(writer, apiRelationshipDependentEnd, writeContext);
            WriteApiForeignKey(writer, apiRelationshipDependentEnd, writeContext);
            WriteApiTraversal(writer, apiRelationshipDependentEnd, writeContext);

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
    private static void WriteApiObjectTypeReference(Utf8JsonWriter writer, ApiRelationshipDependentEnd end, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer
        (
            propertyName: writeContext.PropertyNames.ApiRelationshipElement.ApiObjectTypeReference,
            obj: end.ApiObjectTypeReference,
            options: writeContext.Options
        );

    private static void WriteApiForeignKey(Utf8JsonWriter writer, ApiRelationshipDependentEnd end, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer
        (
            propertyName: writeContext.PropertyNames.ApiRelationshipDependentEnd.ApiForeignKey,
            obj: end.HasForeignKey ? end.ApiForeignKey : null,
            options: writeContext.Options
        );

    private static void WriteApiTraversal(Utf8JsonWriter writer, ApiRelationshipDependentEnd end, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer
        (
            propertyName: writeContext.PropertyNames.ApiRelationshipDependentEnd.ApiTraversal,
            obj: end.ApiTraversal,
            options: writeContext.Options
        );
    #endregion
}
