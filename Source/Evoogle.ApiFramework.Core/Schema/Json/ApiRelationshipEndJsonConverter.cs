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
///     Handles JSON serialization for <see cref="ApiRelationshipEnd"/> instances, including
///     polymorphic dispatch across <see cref="ApiRelationshipPrincipalEnd"/> and
///     <see cref="ApiRelationshipDependentEnd"/>.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipEndJsonConverter(ILogger<ApiRelationshipEndJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipEnd>(logger)
{
    #region Property Types
    private readonly record struct ApiRelationshipEndPropertyNames
    {
        public required string ApiKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required string ApiDeleteBehavior { get; init; }
        public required string ApiIdentityName { get; init; }   // Principal only
        public required string ApiKeyPaths { get; init; }       // Dependent only
    }

    private readonly record struct PropertyNames
    {
        public required ApiRelationshipEndPropertyNames ApiRelationshipEnd { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationshipEnd = new ApiRelationshipEndPropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(ApiRelationshipEnd.ApiKind)),
                    ApiObjectTypeName = policy.ConvertName(nameof(ApiRelationshipEnd.ApiObjectTypeName)),
                    ApiDeleteBehavior = policy.ConvertName(nameof(ApiRelationshipEnd.ApiDeleteBehavior)),
                    ApiIdentityName = policy.ConvertName(nameof(ApiRelationshipPrincipalEnd.ApiIdentityName)),
                    ApiKeyPaths = policy.ConvertName(nameof(ApiRelationshipDependentEnd.ApiKeyPaths)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
    }
    #endregion

    #region Read Types
    private class ApiRelationshipEndReadData
    {
        public ApiRelationshipEndKind? ApiKind { get; set; }
        public string? ApiObjectTypeName { get; set; }
        public ApiRelationshipDeleteBehavior? ApiDeleteBehavior { get; set; }
        public string? ApiIdentityName { get; set; }
        public List<ApiRelationshipKeyPath>? ApiKeyPaths { get; set; }
    }

    private class ReadData : ExtensibleReadData
    {
        public ApiRelationshipEndReadData? ApiRelationshipEnd { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipEnd.ApiKind, HandleApiKind },
            { propertyNames.ApiRelationshipEnd.ApiObjectTypeName, HandleApiObjectTypeName },
            { propertyNames.ApiRelationshipEnd.ApiDeleteBehavior, HandleApiDeleteBehavior },
            { propertyNames.ApiRelationshipEnd.ApiIdentityName, HandleApiIdentityName },
            { propertyNames.ApiRelationshipEnd.ApiKeyPaths, HandleApiKeyPaths },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };

        private static void HandleApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipEnd ??= new ApiRelationshipEndReadData();
            context.ReadData.ApiRelationshipEnd.ApiKind = _endKindConverter.Read(ref reader, typeof(ApiRelationshipEndKind), context.Options);
        }

        private static void HandleApiObjectTypeName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipEnd ??= new ApiRelationshipEndReadData();
            context.ReadData.ApiRelationshipEnd.ApiObjectTypeName = reader.GetString();
        }

        private static void HandleApiDeleteBehavior(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipEnd ??= new ApiRelationshipEndReadData();
            context.ReadData.ApiRelationshipEnd.ApiDeleteBehavior = _deleteBehaviorConverter.Read(ref reader, typeof(ApiRelationshipDeleteBehavior), context.Options);
        }

        private static void HandleApiIdentityName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipEnd ??= new ApiRelationshipEndReadData();
            context.ReadData.ApiRelationshipEnd.ApiIdentityName = reader.GetString();
        }

        private static void HandleApiKeyPaths(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipEnd ??= new ApiRelationshipEndReadData();
            context.ReadData.ApiRelationshipEnd.ApiKeyPaths ??= [];

            ReadJsonArray(ref reader, context, _ => HandleApiKeyPathsArrayItem);
        }

        private static void HandleApiKeyPathsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var keyPath = JsonSerializer.Deserialize<ApiRelationshipKeyPath>(ref reader, context.Options);
            if (keyPath is null)
            {
                return;
            }

            context.ReadData.ApiRelationshipEnd!.ApiKeyPaths!.Add(keyPath);
        }
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiRelationshipEndKind> _endKindConverter = new();
    private static readonly EnumJsonConverter<ApiRelationshipDeleteBehavior> _deleteBehaviorConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipEndJsonConverter()
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
    protected override ApiRelationshipEnd? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiRelationshipEnd;

        if (readData?.ApiKind is null || readData.ApiObjectTypeName is null)
        {
            return null;
        }

        var end = default(ApiRelationshipEnd);
        switch (readData.ApiKind.Value)
        {
            case ApiRelationshipEndKind.Principal:
                end = new ApiRelationshipPrincipalEnd(
                    readData.ApiObjectTypeName,
                    readData.ApiIdentityName,
                    readData.ApiDeleteBehavior ?? ApiRelationshipDeleteBehavior.None);
                break;

            case ApiRelationshipEndKind.Dependent:
                end = new ApiRelationshipDependentEnd(
                    readData.ApiObjectTypeName,
                    readData.ApiKeyPaths,
                    readData.ApiDeleteBehavior ?? ApiRelationshipDeleteBehavior.None);
                break;

            default:
                readContext.Logger.LogError("Unsupported {ApiKind} enumeration value: '{ApiKindValue}'",
                    nameof(ApiRelationshipEndKind), readData.ApiKind);
                break;
        }

        if (end is null)
        {
            return null;
        }

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
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipEnd value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiKind(writer, value, writeContext);
            WriteApiObjectTypeName(writer, value, writeContext);
            WriteApiDeleteBehavior(writer, value, writeContext);
            WriteApiIdentityName(writer, value, writeContext);
            WriteApiKeyPaths(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKind(Utf8JsonWriter writer, ApiRelationshipEnd end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationshipEnd.ApiKind, end.ApiKind, context.Options, _endKindConverter);

    private static void WriteApiObjectTypeName(Utf8JsonWriter writer, ApiRelationshipEnd end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyAsString(context.PropertyNames.ApiRelationshipEnd.ApiObjectTypeName, end.ApiObjectTypeName, context.Options);

    private static void WriteApiDeleteBehavior(Utf8JsonWriter writer, ApiRelationshipEnd end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationshipEnd.ApiDeleteBehavior, end.ApiDeleteBehavior, context.Options, _deleteBehaviorConverter);

    private static void WriteApiIdentityName(Utf8JsonWriter writer, ApiRelationshipEnd end, DefaultWriteContext<PropertyNames> context)
    {
        var value = end.ApiKind == ApiRelationshipEndKind.Principal
            ? ((ApiRelationshipPrincipalEnd)end).ApiIdentityName
            : null;

        writer.TryWritePropertyAsString(context.PropertyNames.ApiRelationshipEnd.ApiIdentityName, value, context.Options);
    }

    private static void WriteApiKeyPaths(Utf8JsonWriter writer, ApiRelationshipEnd end, DefaultWriteContext<PropertyNames> context)
    {
        var value = end.ApiKind == ApiRelationshipEndKind.Dependent
            ? ((ApiRelationshipDependentEnd)end).ApiKeyPaths
            : null;

        var propertyName = context.PropertyNames.ApiRelationshipEnd.ApiKeyPaths;
        var options = context.Options;

        writer.TryWritePropertyWithAction(
            propertyName,
            value,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options)));
    }
    #endregion
}
