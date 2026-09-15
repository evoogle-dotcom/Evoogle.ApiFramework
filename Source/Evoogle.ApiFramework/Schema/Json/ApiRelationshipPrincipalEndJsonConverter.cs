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
    private readonly record struct ApiRelationshipElementPropertyNames
    {
        public required string ApiObjectTypeReference { get; init; }
    }

    private readonly record struct ApiRelationshipPrincipalEndPropertyNames
    {
        public required string ApiPrincipalKeyName { get; init; }
    }

    private readonly record struct PropertyNames
    {
        public required ApiRelationshipElementPropertyNames ApiRelationshipElement { get; init; }
        public required ApiRelationshipPrincipalEndPropertyNames ApiRelationshipPrincipalEnd { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

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

    private class ApiRelationshipPrincipalEndReadData
    {
        public string? ApiPrincipalKeyName { get; set; }
    }

    private class ReadState : ExtensibleReadData
    {
        public ApiRelationshipElementReadData? ApiRelationshipElement { get; set; }
        public ApiRelationshipPrincipalEndReadData? ApiRelationshipPrincipalEnd { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipElement.ApiObjectTypeReference, HandleApiObjectTypeReference },
            { propertyNames.ApiRelationshipPrincipalEnd.ApiPrincipalKeyName, HandleApiPrincipalKeyName },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() },
        };

        private static void HandleApiObjectTypeReference(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipElement ??= new ApiRelationshipElementReadData();
            context.ReadData.ApiRelationshipElement.ApiObjectTypeReference =
                JsonSerializer.Deserialize<ApiTypeReference>(ref reader, context.Options);
        }

        private static void HandleApiPrincipalKeyName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipPrincipalEnd ??= new ApiRelationshipPrincipalEndReadData();
            context.ReadData.ApiRelationshipPrincipalEnd.ApiPrincipalKeyName = reader.GetString();
        }
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
    protected override ApiRelationshipPrincipalEnd? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;

        var apiObjectTypeReference = readContext.ReadData.ApiRelationshipElement?.ApiObjectTypeReference;
        var apiPrincipalKeyName = readContext.ReadData.ApiRelationshipPrincipalEnd?.ApiPrincipalKeyName;

        var end = new ApiRelationshipPrincipalEnd
            (
                apiObjectTypeReference!,
                apiPrincipalKeyName
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
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiObjectTypeReference(writer, value, writeContext);
            WriteApiPrincipalKeyName(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiObjectTypeReference(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithSerializer
        (
            context.PropertyNames.ApiRelationshipElement.ApiObjectTypeReference,
            end.ApiObjectTypeReference,
            context.Options
        );

    private static void WriteApiPrincipalKeyName(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd end, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipPrincipalEnd.ApiPrincipalKeyName;
        var value = end.ApiPrincipalKeyName;

        writer.TryWritePropertyAsString(propertyName, value, context.Options);
    }
    #endregion
}
