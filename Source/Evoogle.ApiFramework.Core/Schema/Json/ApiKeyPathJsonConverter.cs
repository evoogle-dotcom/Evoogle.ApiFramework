// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Handles JSON serialization for <see cref="ApiKeyPath"/> instances, including support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiKeyPathJsonConverter(ILogger<ApiKeyPathJsonConverter>? logger) : JsonConverterBase<ApiKeyPath>(logger)
{
    #region Property Types
    private readonly record struct ApiKeyPathPropertyNames
    {
        #region Immutable Properties
        public required string ClrRootType { get; init; }
        public required string ClrPath { get; init; }
        public required string ApiSegments { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiKeyPathPropertyNames ApiKeyPath { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiKeyPath = new ApiKeyPathPropertyNames
                {
                    ClrRootType = policy.ConvertName(nameof(ApiKeyPath.ClrRootType)),
                    ClrPath = policy.ConvertName(nameof(ApiKeyPath.ClrPath)),
                    ApiSegments = policy.ConvertName(nameof(ApiKeyPath.ApiSegments)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiKeyPathReadData
    {
        #region Properties
        public Type? ClrRootType { get; set; }
        public string? ClrPath { get; set; }
        public List<ApiKeyPathSegment>? ApiSegments { get; set; }
        #endregion
    }

    private class ReadState : ExtensibleReadData
    {
        #region Properties
        public ApiKeyPathReadData? ApiKeyPath { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiKeyPath Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiKeyPath Property Handlers
            { propertyNames.ApiKeyPath.ClrRootType, HandleApiKeyPathClrRootType },
            { propertyNames.ApiKeyPath.ClrPath, HandleApiKeyPathClrPath },
            { propertyNames.ApiKeyPath.ApiSegments, HandleApiKeyPathApiSegments },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() },
        };
        #endregion

        #region ApiKeyPath Methods
        private static void HandleApiKeyPathClrRootType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPath ??= new ApiKeyPathReadData();

            var options = context.Options;
            context.ReadData.ApiKeyPath.ClrRootType = _typeJsonConverter.Read(ref reader, typeof(Type), options);
        }

        private static void HandleApiKeyPathApiSegments(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPath ??= new ApiKeyPathReadData();
            context.ReadData.ApiKeyPath.ApiSegments ??= [];

            ReadJsonArray(ref reader, context, _ => HandleApiKeyPathApiSegmentsArrayItem);
        }

        private static void HandleApiKeyPathClrPath(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiKeyPath ??= new ApiKeyPathReadData();
            context.ReadData.ApiKeyPath.ClrPath = reader.GetString();
        }

        private static void HandleApiKeyPathApiSegmentsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            // Recursive: each child element uses the same converter via the [JsonConverter] attribute.
            var segment = JsonSerializer.Deserialize<ApiKeyPathSegment>(ref reader, context.Options);
            if (segment is null)
            {
                return;
            }

            context.ReadData.ApiKeyPath!.ApiSegments!.Add(segment);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiKeyPathJsonConverter()
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
        => CreateDefaultWriteContext
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create
            );

    /// <inheritdoc/>
    protected override ApiKeyPath? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData.ApiKeyPath;

        var clrRootType = readState?.ClrRootType;
        var apiSegments = readState?.ApiSegments ?? CreateApiSegments(readState?.ClrPath);

        var apiKeyPath = new ApiKeyPath(clrRootType, apiSegments);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiKeyPath, extensions);

        return apiKeyPath;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiKeyPath value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiKeyPathClrRootType(writer, value, writeContext);
            WriteApiKeyPathClrPathOrApiSegments(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKeyPathClrRootType(Utf8JsonWriter writer, ApiKeyPath apiKeyPath, DefaultWriteContext<PropertyNames> context)
    {
        var type = apiKeyPath.ClrRootType;

        if (apiKeyPath.GetOwningDefaultClrRootType() == type)
        {
            return; // Inferred from the owning ApiObjectType or ApiRelationshipDependentEnd during compilation.
        }

        var propertyName = context.PropertyNames.ApiKeyPath.ClrRootType;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, type, options, _typeJsonConverter);
    }

    private static IEnumerable<ApiKeyPathSegment> CreateApiSegments(string? clrPath)
    {
        if (clrPath is null)
        {
            return [];
        }

        var parseResult = ApiKeyPathClrPathParser.Parse(clrPath);
        return [.. parseResult.ClrPropertyNames.Select(static name => new ApiKeyPathSegment(name))];
    }

    private static void WriteApiKeyPathClrPathOrApiSegments(Utf8JsonWriter writer, ApiKeyPath apiKeyPath, DefaultWriteContext<PropertyNames> context)
    {
        if (ShouldWriteApiSegments(apiKeyPath))
        {
            WriteApiKeyPathApiSegments(writer, apiKeyPath, context);
            return;
        }

        WriteApiKeyPathClrPath(writer, apiKeyPath, context);
    }

    private static bool ShouldWriteApiSegments(ApiKeyPath apiKeyPath)
    {
        return apiKeyPath.ApiSegments.IsEmpty || apiKeyPath.ApiSegments.Any
        (
            static segment => segment.ExtensionCount > 0 || segment.ClrPropertyName.Contains('.')
        );
    }

    private static void WriteApiKeyPathClrPath(Utf8JsonWriter writer, ApiKeyPath apiKeyPath, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyPath.ClrPath;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, apiKeyPath.ClrPath, options);
    }

    private static void WriteApiKeyPathApiSegments(Utf8JsonWriter writer, ApiKeyPath apiKeyPath, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyPath.ApiSegments;
        var apiSegments = apiKeyPath.ApiSegments;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiSegments,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }
    #endregion
}
