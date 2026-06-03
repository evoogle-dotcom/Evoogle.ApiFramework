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
///     Handles JSON serialization for <see cref="ApiRelationshipPrincipalEnd"/> instances.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipPrincipalEndJsonConverter(ILogger<ApiRelationshipPrincipalEndJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipPrincipalEnd>(logger)
{
    #region Property Types
    private readonly record struct ApiRelationshipElementPropertyNames
    {
        public required string ClrObjectType { get; init; }
    }

    private readonly record struct ApiRelationshipPrincipalEndPropertyNames
    {
        public required string ApiKeyTypeName { get; init; }
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
                    ClrObjectType = policy.ConvertName(nameof(ApiRelationshipElement.ClrObjectType)),
                },
                ApiRelationshipPrincipalEnd = new ApiRelationshipPrincipalEndPropertyNames
                {
                    ApiKeyTypeName = policy.ConvertName(nameof(ApiRelationshipPrincipalEnd.ApiKeyTypeName)),
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

    private class ApiRelationshipPrincipalEndReadData
    {
        public string? ApiKeyTypeName { get; set; }
    }

    private class ReadData : ExtensibleReadData
    {
        public ApiRelationshipElementReadData? ApiRelationshipElement { get; set; }
        public ApiRelationshipPrincipalEndReadData? ApiRelationshipPrincipalEnd { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipElement.ClrObjectType, HandleClrObjectType },
            { propertyNames.ApiRelationshipPrincipalEnd.ApiKeyTypeName, HandleApiKeyTypeName },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };

        private static void HandleClrObjectType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipElement ??= new ApiRelationshipElementReadData();
            context.ReadData.ApiRelationshipElement.ClrObjectType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }

        private static void HandleApiKeyTypeName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipPrincipalEnd ??= new ApiRelationshipPrincipalEndReadData();
            context.ReadData.ApiRelationshipPrincipalEnd.ApiKeyTypeName = reader.GetString();
        }
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
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
        => CreateDefaultWriteContext(logger, options, buildPropertyNames: PropertyNames.Create);

    /// <inheritdoc/>
    protected override ApiRelationshipPrincipalEnd? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;

        var clrObjectType = readContext.ReadData.ApiRelationshipElement?.ClrObjectType;
        var apiKeyTypeName = readContext.ReadData.ApiRelationshipPrincipalEnd?.ApiKeyTypeName;

        var end = new ApiRelationshipPrincipalEnd
            (
                clrObjectType!,
                apiKeyTypeName
            );

        AttachExtensions(end, readContext.ReadData.Extensions);
        return end;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteClrObjectType(writer, value, writeContext);
            WriteApiKeyTypeName(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteClrObjectType(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationshipElement.ClrObjectType, end.ClrObjectType, context.Options, _typeJsonConverter);

    private static void WriteApiKeyTypeName(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd end, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipPrincipalEnd.ApiKeyTypeName;
        var value = end.ApiKeyTypeName;

        writer.TryWritePropertyAsString(propertyName, value, context.Options);
    }
    #endregion
}
