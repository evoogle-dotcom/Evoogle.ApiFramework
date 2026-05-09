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
        public required string ApiKeyPathsA { get; init; }
        public required string ApiKeyPathsB { get; init; }
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
                    ApiKeyPathsA = policy.ConvertName(nameof(ApiRelationshipAssociation.ApiKeyPathsA)),
                    ApiKeyPathsB = policy.ConvertName(nameof(ApiRelationshipAssociation.ApiKeyPathsB)),
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
        public List<ApiRelationshipKeyPath>? ApiKeyPathsA { get; set; }
        public List<ApiRelationshipKeyPath>? ApiKeyPathsB { get; set; }
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
            { propertyNames.ApiRelationshipAssociation.ApiKeyPathsA, HandleApiKeyPathsA },
            { propertyNames.ApiRelationshipAssociation.ApiKeyPathsB, HandleApiKeyPathsB },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };

        private static void HandleClrObjectType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipElement ??= new ApiRelationshipElementReadData();
            context.ReadData.ApiRelationshipElement.ClrObjectType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }

        private static void HandleApiKeyPathsA(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
            => HandleApiKeyPaths(ref reader, context, paths => paths.ApiKeyPathsA ??= []);

        private static void HandleApiKeyPathsB(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
            => HandleApiKeyPaths(ref reader, context, paths => paths.ApiKeyPathsB ??= []);

        private static void HandleApiKeyPaths(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context, Func<ApiRelationshipAssociationReadData, List<ApiRelationshipKeyPath>> getPathsList)
        {
            context.ReadData.ApiRelationshipAssociation ??= new ApiRelationshipAssociationReadData();
            var pathsList = getPathsList(context.ReadData.ApiRelationshipAssociation);

            void itemHandler(ref Utf8JsonReader itemReader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> ctx) => HandleApiKeyPathItem(ref itemReader, ctx, pathsList);
            ReadJsonArray(ref reader, context, _ => itemHandler);
        }

        private static void HandleApiKeyPathItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context, List<ApiRelationshipKeyPath> pathsList)
        {
            var keyPath = JsonSerializer.Deserialize<ApiRelationshipKeyPath>(ref reader, context.Options);
            if (keyPath is null)
            {
                return;
            }

            pathsList.Add(keyPath);
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
        var apiKeyPathsA = readContext.ReadData.ApiRelationshipAssociation?.ApiKeyPathsA;
        var apiKeyPathsB = readContext.ReadData.ApiRelationshipAssociation?.ApiKeyPathsB;

        var apiRelationshipAssociation = new ApiRelationshipAssociation(clrObjectType!, apiKeyPathsA, apiKeyPathsB);

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
            WriteApiKeyPathsA(writer, value, writeContext);
            WriteApiKeyPathsB(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteClrObjectType(Utf8JsonWriter writer, ApiRelationshipAssociation end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationshipElement.ClrObjectType, end.ClrObjectType, context.Options, _typeJsonConverter);

    private static void WriteApiKeyPaths(Utf8JsonWriter writer, ApiRelationshipAssociation end, DefaultWriteContext<PropertyNames> context, string propertyName, IEnumerable<ApiRelationshipKeyPath>? value)
    {
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            value,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }

    private static void WriteApiKeyPathsA(Utf8JsonWriter writer, ApiRelationshipAssociation end, DefaultWriteContext<PropertyNames> context)
        => WriteApiKeyPaths(writer, end, context, context.PropertyNames.ApiRelationshipAssociation.ApiKeyPathsA, end.ApiKeyPathsA);

    private static void WriteApiKeyPathsB(Utf8JsonWriter writer, ApiRelationshipAssociation end, DefaultWriteContext<PropertyNames> context)
        => WriteApiKeyPaths(writer, end, context, context.PropertyNames.ApiRelationshipAssociation.ApiKeyPathsB, end.ApiKeyPathsB);
    #endregion
}
