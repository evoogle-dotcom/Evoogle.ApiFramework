// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Key.Internal;
using Evoogle.ApiFramework.Schema.Types;
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
        public required string ApiRootObjectTypeReference { get; init; }
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
                    ApiRootObjectTypeReference = policy.ConvertName(nameof(Key.ApiKeyPath.ApiRootObjectType)), // Mapping property name from ApiRootObjectTypeReference to ApiRootObjectType by design
                    ClrPath = policy.ConvertName(nameof(Key.ApiKeyPath.ClrPath)),
                    ApiSegments = policy.ConvertName(nameof(Key.ApiKeyPath.ApiSegments)),
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
        public ApiTypeReference? ApiRootObjectTypeReference { get; set; }
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
            { propertyNames.ApiKeyPath.ApiRootObjectTypeReference, HandleApiKeyPathApiRootObjectTypeReference },
            { propertyNames.ApiKeyPath.ClrPath, HandleApiKeyPathClrPath },
            { propertyNames.ApiKeyPath.ApiSegments, HandleApiKeyPathApiSegments },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() },
        };
        #endregion

        #region ApiKeyPath Methods
        private static void HandleApiKeyPathApiRootObjectTypeReference
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        )
        {
            context.ReadData.ApiKeyPath ??= new ApiKeyPathReadData();
            context.ReadData.ApiKeyPath.ApiRootObjectTypeReference = JsonSerializer.Deserialize<ApiTypeReference>(ref reader, context.Options);
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

        var apiRootObjectTypeReference = readState?.ApiRootObjectTypeReference;
        var apiSegments = readState?.ApiSegments ?? CreateApiSegments(readState?.ClrPath);

        var apiKeyPath = new ApiKeyPath(apiRootObjectTypeReference, apiSegments);

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
            WriteApiKeyPathApiRootObjectTypeReference(writer, value, writeContext);
            WriteApiKeyPathClrPathOrApiSegments(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Create Implementation Methods
    private static IEnumerable<ApiKeyPathSegment> CreateApiSegments(string? clrPath)
    {
        if (clrPath is null)
        {
            return [];
        }

        var parseResult = ApiKeyPathClrPathParser.Parse(clrPath);
        return [.. parseResult.ClrMemberNames.Select(static name => new ApiKeyPathSegment(name))];
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKeyPathApiRootObjectTypeReference
    (
        Utf8JsonWriter writer,
        ApiKeyPath apiKeyPath,
        DefaultWriteContext<PropertyNames> context
    )
    {
        // An inferred root does not require an explicit root object-type reference.
        if (apiKeyPath.IsInferredRoot)
        {
            return;
        }

        // If the root has not been resolved throw an exception.
        if (!apiKeyPath.IsRootResolved)
        {
            throw new JsonException("The root type has not been resolved and cannot be written.");
        }

        // Omit an explicit reference when it resolves to the root the context would infer.
        var resolvedClrRootType = apiKeyPath.ClrRootType;
        var inferredClrRootType = apiKeyPath.GetInferredClrRootType();
        if (inferredClrRootType == resolvedClrRootType)
        {
            return;
        }

        // A differing explicit root is written under the logical ApiRootObjectType JSON property.
        var propertyName = context.PropertyNames.ApiKeyPath.ApiRootObjectTypeReference;
        var apiRootObjectTypeReference = apiKeyPath.ApiRootObjectTypeReference;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, apiRootObjectTypeReference, options);
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
            static segment => segment.ExtensionCount > 0 || segment.ClrMemberName.Contains('.')
        );
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

    private static void WriteApiKeyPathClrPath(Utf8JsonWriter writer, ApiKeyPath apiKeyPath, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiKeyPath.ClrPath;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, apiKeyPath.ClrPath, options);
    }
    #endregion
}
