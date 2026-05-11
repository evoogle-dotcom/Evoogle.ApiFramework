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
///     Handles JSON serialization for <see cref="ApiRelationshipKeyPath"/> instances, including
///     polymorphic dispatch across <see cref="ApiRelationshipScalarKeyPath"/>,
///     <see cref="ApiRelationshipNestedKeyPath"/>, and <see cref="ApiRelationshipOwnerKeyPath"/>.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipKeyPathJsonConverter(ILogger<ApiRelationshipKeyPathJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipKeyPath>(logger)
{
    #region Property Types
    private readonly record struct ApiRelationshipKeyPathPropertyNames
    {
        #region Immutable Properties
        public required string ApiKind { get; init; }
        public required string ApiKeyPaths { get; init; }
        public required string ClrPropertyName { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiRelationshipKeyPathPropertyNames ApiRelationshipKeyPath { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationshipKeyPath = new ApiRelationshipKeyPathPropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(ApiRelationshipKeyPath.ApiKind)),
                    ApiKeyPaths = policy.ConvertName(nameof(ApiRelationshipNestedKeyPath.ApiKeyPaths)),
                    ClrPropertyName = policy.ConvertName(nameof(ApiRelationshipNestedKeyPath.ClrPropertyName)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiRelationshipKeyPathReadData
    {
        #region Properties
        public ApiRelationshipKeyPathKind? ApiKind { get; set; }
        public List<ApiRelationshipKeyPath>? ApiKeyPaths { get; set; }
        public string? ClrPropertyName { get; set; }
        #endregion
    }

    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiRelationshipKeyPathReadData? ApiRelationshipKeyPath { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipKeyPath.ApiKind, HandleApiKind },
            { propertyNames.ApiRelationshipKeyPath.ApiKeyPaths, HandleApiKeyPaths },
            { propertyNames.ApiRelationshipKeyPath.ClrPropertyName, HandleClrPropertyName },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region Handler Methods
        private static void HandleApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipKeyPath ??= new ApiRelationshipKeyPathReadData();
            context.ReadData.ApiRelationshipKeyPath.ApiKind = _apiKindJsonConverter.Read(ref reader, typeof(ApiRelationshipKeyPathKind), context.Options);
        }

        private static void HandleApiKeyPaths(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipKeyPath ??= new ApiRelationshipKeyPathReadData();
            context.ReadData.ApiRelationshipKeyPath.ApiKeyPaths ??= [];

            ReadJsonArray(ref reader, context, _ => HandleApiKeyPathsArrayItem);
        }

        private static void HandleApiKeyPathsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            // Recursive: each child element uses the same converter via the [JsonConverter] attribute.
            var childPath = JsonSerializer.Deserialize<ApiRelationshipKeyPath>(ref reader, context.Options);
            if (childPath is null)
            {
                return;
            }

            context.ReadData.ApiRelationshipKeyPath!.ApiKeyPaths!.Add(childPath);
        }

        private static void HandleClrPropertyName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipKeyPath ??= new ApiRelationshipKeyPathReadData();
            context.ReadData.ApiRelationshipKeyPath.ClrPropertyName = reader.GetString();
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiRelationshipKeyPathKind> _apiKindJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipKeyPathJsonConverter()
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
    protected override ApiRelationshipKeyPath? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiRelationshipKeyPath;

        if (readData?.ApiKind is null)
        {
            return null;
        }

        var apiKindValue = readData.ApiKind.Value;
        ApiRelationshipKeyPath? path = apiKindValue switch
        {
            ApiRelationshipKeyPathKind.Scalar => new ApiRelationshipScalarKeyPath(readData.ClrPropertyName!),

            ApiRelationshipKeyPathKind.Nested => new ApiRelationshipNestedKeyPath(readData.ClrPropertyName!, readData.ApiKeyPaths!),

            ApiRelationshipKeyPathKind.Owner => readData.ApiKeyPaths?.Count > 0
                ? new ApiRelationshipOwnerKeyPath(readData.ApiKeyPaths)
                : new ApiRelationshipOwnerKeyPath(),

            _ => null
        };

        if (path is null)
        {
            readContext.Logger.LogError("Unsupported {ApiKind} enumeration value: '{ApiKindValue}'", nameof(ApiRelationshipKeyPathKind), apiKindValue);
            return null;
        }

        AttachExtensions(path, readContext.ReadData.Extensions);
        return path;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipKeyPath value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiKind(writer, value, writeContext);

            switch (value)
            {
                case ApiRelationshipScalarKeyPath apiRelationshipScalarKeyPath:
                    WriteClrPropertyName(writer, apiRelationshipScalarKeyPath, writeContext);
                    break;

                case ApiRelationshipNestedKeyPath apiRelationshipNestedKeyPath:
                    WriteClrPropertyName(writer, apiRelationshipNestedKeyPath, writeContext);
                    WriteApiKeyPaths(writer, apiRelationshipNestedKeyPath, writeContext);
                    break;

                case ApiRelationshipOwnerKeyPath apiRelationshipOwnerKeyPath:
                    WriteApiKeyPaths(writer, apiRelationshipOwnerKeyPath, writeContext);
                    break;
            }

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKind(Utf8JsonWriter writer, ApiRelationshipKeyPath path, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipKeyPath.ApiKind;
        writer.TryWritePropertyWithConverter(propertyName, path.ApiKind, context.Options, _apiKindJsonConverter);
    }

    private static void WriteClrPropertyName(Utf8JsonWriter writer, ApiRelationshipScalarKeyPath path, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipKeyPath.ClrPropertyName;
        var value = path.ClrPropertyName;

        writer.TryWritePropertyAsString(propertyName, value, context.Options);
    }

    private static void WriteClrPropertyName(Utf8JsonWriter writer, ApiRelationshipNestedKeyPath path, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipKeyPath.ClrPropertyName;
        var value = path.ClrPropertyName;

        writer.TryWritePropertyAsString(propertyName, value, context.Options);
    }

    private static void WriteApiKeyPaths(Utf8JsonWriter writer, ApiRelationshipNestedKeyPath path, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipKeyPath.ApiKeyPaths;
        var value = path.ApiKeyPaths;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            value,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }

    private static void WriteApiKeyPaths(Utf8JsonWriter writer, ApiRelationshipOwnerKeyPath path, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipKeyPath.ApiKeyPaths;
        var value = path.ApiKeyPaths;
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
