// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

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
        public required string ApiForeignKeyTypeA { get; init; }
        public required string ApiForeignKeyTypeB { get; init; }
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
                    ApiForeignKeyTypeA = policy.ConvertName(nameof(ApiRelationshipAssociation.ApiForeignKeyTypeA)),
                    ApiForeignKeyTypeB = policy.ConvertName(nameof(ApiRelationshipAssociation.ApiForeignKeyTypeB)),
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
        public ApiKeyType? ApiForeignKeyTypeA { get; set; }
        public ApiKeyType? ApiForeignKeyTypeB { get; set; }
    }

    private class ReadData : ExtensibleReadData
    {
        public ApiRelationshipElementReadData? ApiRelationshipElement { get; set; }
        public ApiRelationshipAssociationReadData? ApiRelationshipAssociation { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipElement.ClrObjectType, HandleClrObjectType },
            { propertyNames.ApiRelationshipAssociation.ApiForeignKeyTypeA, HandleApiForeignKeyTypeA },
            { propertyNames.ApiRelationshipAssociation.ApiForeignKeyTypeB, HandleApiForeignKeyTypeB },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };

        private static void HandleClrObjectType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipElement ??= new ApiRelationshipElementReadData();
            context.ReadData.ApiRelationshipElement.ClrObjectType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }

        private static void HandleApiForeignKeyTypeA(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipAssociation ??= new ApiRelationshipAssociationReadData();
            context.ReadData.ApiRelationshipAssociation.ApiForeignKeyTypeA = JsonSerializer.Deserialize<ApiKeyType>(ref reader, context.Options);
        }

        private static void HandleApiForeignKeyTypeB(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipAssociation ??= new ApiRelationshipAssociationReadData();
            context.ReadData.ApiRelationshipAssociation.ApiForeignKeyTypeB = JsonSerializer.Deserialize<ApiKeyType>(ref reader, context.Options);
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
        => CreateDefaultReadContext<PropertyNames, ReadData, ReadHandlers>
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
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;

        var clrObjectType = readContext.ReadData.ApiRelationshipElement?.ClrObjectType;
        var apiForeignKeyTypeA = readContext.ReadData.ApiRelationshipAssociation?.ApiForeignKeyTypeA;
        var apiForeignKeyTypeB = readContext.ReadData.ApiRelationshipAssociation?.ApiForeignKeyTypeB;

        var apiRelationshipAssociation = apiForeignKeyTypeA != null && apiForeignKeyTypeB != null
            ? new ApiRelationshipAssociation(clrObjectType!, apiForeignKeyTypeA, apiForeignKeyTypeB)
            : new ApiRelationshipAssociation(clrObjectType!);

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
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipAssociation value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteClrObjectType(writer, value, writeContext);
            WriteApiForeignKeyType(writer, value.HasKeyBinding ? value.ApiForeignKeyTypeA : null, writeContext.PropertyNames.ApiRelationshipAssociation.ApiForeignKeyTypeA, writeContext);
            WriteApiForeignKeyType(writer, value.HasKeyBinding ? value.ApiForeignKeyTypeB : null, writeContext.PropertyNames.ApiRelationshipAssociation.ApiForeignKeyTypeB, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteClrObjectType(Utf8JsonWriter writer, ApiRelationshipAssociation end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationshipElement.ClrObjectType, end.ClrObjectType, context.Options, _typeJsonConverter);

    private static void WriteApiForeignKeyType(Utf8JsonWriter writer, ApiKeyType? value, string propertyName, DefaultWriteContext<PropertyNames> context)
    {
        if (value is null)
        {
            return;
        }

        writer.WritePropertyName(propertyName);
        writer.TryWriteWithSerializer(value, context.Options);
    }
    #endregion
}
