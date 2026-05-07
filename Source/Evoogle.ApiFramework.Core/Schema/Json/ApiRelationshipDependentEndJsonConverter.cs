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
///     Handles JSON serialization for <see cref="ApiRelationshipDependentEnd"/> instances.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipDependentEndJsonConverter(ILogger<ApiRelationshipDependentEndJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipDependentEnd>(logger)
{
    #region Property Types
    private readonly record struct ApiRelationshipEndPropertyNames
    {
        public required string ClrObjectType { get; init; }
    }

    private readonly record struct ApiRelationshipDependentEndPropertyNames
    {
        public required string ApiKeyPaths { get; init; }
    }

    private readonly record struct PropertyNames
    {
        public required ApiRelationshipEndPropertyNames ApiRelationshipEnd { get; init; }
        public required ApiRelationshipDependentEndPropertyNames ApiRelationshipDependentEnd { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationshipEnd = new ApiRelationshipEndPropertyNames
                {
                    ClrObjectType = policy.ConvertName(nameof(ApiRelationshipEnd.ClrObjectType)),
                },
                ApiRelationshipDependentEnd = new ApiRelationshipDependentEndPropertyNames
                {
                    ApiKeyPaths = policy.ConvertName(nameof(ApiRelationshipDependentEnd.ApiKeyPaths)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
    }
    #endregion

    #region Read Types
    private class ApiRelationshipEndReadData
    {
        public Type? ClrObjectType { get; set; }
    }

    private class ApiRelationshipDependentEndReadData
    {
        public List<ApiRelationshipKeyPath>? ApiKeyPaths { get; set; }
    }

    private class ReadData : ExtensibleReadData
    {
        public ApiRelationshipEndReadData? ApiRelationshipEnd { get; set; }
        public ApiRelationshipDependentEndReadData? ApiRelationshipDependentEnd { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipEnd.ClrObjectType, HandleClrObjectType },
            { propertyNames.ApiRelationshipDependentEnd.ApiKeyPaths, HandleApiKeyPaths },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };

        private static void HandleClrObjectType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipEnd ??= new ApiRelationshipEndReadData();
            context.ReadData.ApiRelationshipEnd.ClrObjectType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }

        private static void HandleApiKeyPaths(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipDependentEnd ??= new ApiRelationshipDependentEndReadData();
            context.ReadData.ApiRelationshipDependentEnd.ApiKeyPaths ??= [];

            ReadJsonArray(ref reader, context, _ => HandleApiKeyPathsArrayItem);
        }

        private static void HandleApiKeyPathsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var keyPath = JsonSerializer.Deserialize<ApiRelationshipKeyPath>(ref reader, context.Options);
            if (keyPath is null)
            {
                return;
            }

            context.ReadData.ApiRelationshipDependentEnd!.ApiKeyPaths!.Add(keyPath);
        }
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
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
    protected override ApiRelationshipDependentEnd? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;

        var clrObjectType = readContext.ReadData.ApiRelationshipEnd?.ClrObjectType;
        var apiKeyPaths = readContext.ReadData.ApiRelationshipDependentEnd?.ApiKeyPaths;

        var end = new ApiRelationshipDependentEnd
            (
                clrObjectType!,
                apiKeyPaths
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
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipDependentEnd value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteClrObjectType(writer, value, writeContext);
            WriteApiKeyPaths(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteClrObjectType(Utf8JsonWriter writer, ApiRelationshipDependentEnd end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationshipEnd.ClrObjectType, end.ClrObjectType, context.Options, _typeJsonConverter);

    private static void WriteApiKeyPaths(Utf8JsonWriter writer, ApiRelationshipDependentEnd end, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipDependentEnd.ApiKeyPaths;
        var value = end.ApiKeyPaths;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            value,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }
    #endregion
}
