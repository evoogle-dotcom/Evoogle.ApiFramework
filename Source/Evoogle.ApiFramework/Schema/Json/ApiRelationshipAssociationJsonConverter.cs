// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Relationships;
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
    private readonly record struct ApiRelationshipElementPropertyNames
    {
        public required string ClrObjectType { get; init; }
    }

    private readonly record struct ApiRelationshipAssociationPropertyNames
    {
        public required string ApiForeignKeyA { get; init; }
        public required string ApiForeignKeyB { get; init; }
    }

    private readonly record struct PropertyNames
    {
        public required ApiRelationshipElementPropertyNames ApiRelationshipElement { get; init; }
        public required ApiRelationshipAssociationPropertyNames ApiRelationshipAssociation { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationshipElement = new ApiRelationshipElementPropertyNames
                {
                    ClrObjectType = policy.ConvertName(nameof(ApiRelationshipElement.ClrObjectType)),
                },
                ApiRelationshipAssociation = new ApiRelationshipAssociationPropertyNames
                {
                    ApiForeignKeyA = policy.ConvertName(nameof(ApiRelationshipAssociation.ApiForeignKeyA)),
                    ApiForeignKeyB = policy.ConvertName(nameof(ApiRelationshipAssociation.ApiForeignKeyB)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
    }
    #endregion

    #region Read Types
    private class ApiRelationshipElementReadData
    {
        public Type? ClrObjectType { get; set; }
    }

    private class ApiRelationshipAssociationReadData
    {
        public ApiKeyDefinition? ApiForeignKeyA { get; set; }
        public ApiKeyDefinition? ApiForeignKeyB { get; set; }
    }

    private class ReadState : ExtensibleReadData
    {
        public ApiRelationshipElementReadData? ApiRelationshipElement { get; set; }
        public ApiRelationshipAssociationReadData? ApiRelationshipAssociation { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipElement.ClrObjectType, HandleClrObjectType },
            { propertyNames.ApiRelationshipAssociation.ApiForeignKeyA, HandleApiForeignKeyA },
            { propertyNames.ApiRelationshipAssociation.ApiForeignKeyB, HandleApiForeignKeyB },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() },
        };

        private static void HandleClrObjectType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipElement ??= new ApiRelationshipElementReadData();
            context.ReadData.ApiRelationshipElement.ClrObjectType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }

        private static void HandleApiForeignKeyA(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipAssociation ??= new ApiRelationshipAssociationReadData();
            context.ReadData.ApiRelationshipAssociation.ApiForeignKeyA = JsonSerializer.Deserialize<ApiKeyDefinition>(ref reader, context.Options);
        }

        private static void HandleApiForeignKeyB(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipAssociation ??= new ApiRelationshipAssociationReadData();
            context.ReadData.ApiRelationshipAssociation.ApiForeignKeyB = JsonSerializer.Deserialize<ApiKeyDefinition>(ref reader, context.Options);
        }
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
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
    protected override ApiRelationshipAssociation? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;

        var clrObjectType = readContext.ReadData.ApiRelationshipElement?.ClrObjectType;
        var apiForeignKeyA = readContext.ReadData.ApiRelationshipAssociation?.ApiForeignKeyA;
        var apiForeignKeyB = readContext.ReadData.ApiRelationshipAssociation?.ApiForeignKeyB;

        var apiRelationshipAssociation = apiForeignKeyA != null && apiForeignKeyB != null
            ? new ApiRelationshipAssociation(clrObjectType!, apiForeignKeyA, apiForeignKeyB)
            : new ApiRelationshipAssociation(clrObjectType!);

        if (clrObjectType is not null)
        {
            foreach (var apiKeyPath in apiForeignKeyA?.ApiKeyPaths ?? [])
            {
                apiKeyPath.EnsureClrRootType(clrObjectType);
            }

            foreach (var apiKeyPath in apiForeignKeyB?.ApiKeyPaths ?? [])
            {
                apiKeyPath.EnsureClrRootType(clrObjectType);
            }
        }

        AttachExtensions(apiRelationshipAssociation, readContext.ReadData.Extensions);
        return apiRelationshipAssociation;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipAssociation value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteClrObjectType(writer, value, writeContext);
            WriteApiForeignKey(writer, value.HasForeignKeys ? value.ApiForeignKeyA : null, writeContext.PropertyNames.ApiRelationshipAssociation.ApiForeignKeyA, writeContext);
            WriteApiForeignKey(writer, value.HasForeignKeys ? value.ApiForeignKeyB : null, writeContext.PropertyNames.ApiRelationshipAssociation.ApiForeignKeyB, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteClrObjectType(Utf8JsonWriter writer, ApiRelationshipAssociation end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationshipElement.ClrObjectType, end.ClrObjectType, context.Options, _typeJsonConverter);

    private static void WriteApiForeignKey(Utf8JsonWriter writer, ApiKeyDefinition? value, string propertyName, DefaultWriteContext<PropertyNames> context)
    {
        writer.TryWritePropertyWithSerializer(propertyName, value, context.Options);
    }
    #endregion
}
