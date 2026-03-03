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
///     Provides System.Text.Json serialization support for <see cref="ApiIdentity"/> instances, including
///     extension payloads and schema-specific naming policies.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiIdentityJsonConverter(ILogger<ApiIdentityJsonConverter>? logger) : JsonConverterBase<ApiIdentity>(logger)
{
    #region Property Types
    /// <summary>
    ///     Provides cached JSON property names for <see cref="ApiIdentity"/> members under a specific naming policy.
    /// </summary>
    private readonly record struct ApiIdentityPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiIdentitySources { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates the property name sets used while reading or writing relationships and extension data.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdentityPropertyNames ApiIdentity { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentity = new ApiIdentityPropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Schema.ApiIdentity.ApiName)),
                    ApiIdentitySources = policy.ConvertName(nameof(Schema.ApiIdentity.ApiIdentitySources))
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading the primitive relationship properties from JSON.
    /// </summary>
    private class ApiIdentityReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public List<ApiIdentitySource>? ApiIdentitySources { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects all data encountered while deserializing a relationship, including extensions.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiIdentityReadData? ApiIdentity { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides handlers that map JSON property names to strongly typed relationship data assignments.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiIdentity Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiIdentity Property Handlers
            { propertyNames.ApiIdentity.ApiName, HandleApiIdentityApiName },
            { propertyNames.ApiIdentity.ApiIdentitySources, HandleApiIdentityApiIdentitySources },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiIdentity Methods
        private static void HandleApiIdentityApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentity ??= new ApiIdentityReadData();

            context.ReadData.ApiIdentity.ApiName = reader.GetString();
        }

        private static void HandleApiIdentityApiIdentitySources(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentity ??= new ApiIdentityReadData();
            context.ReadData.ApiIdentity.ApiIdentitySources ??= new List<ApiIdentitySource>();

            ReadJsonArray(ref reader, context, (x) => HandleApiIdentityApiIdentitySourcesArrayItem);
        }

        private static void HandleApiIdentityApiIdentitySourcesArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var apiIdentitySource = JsonSerializer.Deserialize<ApiIdentitySource>(ref reader, context.Options);
            if (apiIdentitySource == null)
            {
                return;
            }

            context.ReadData.ApiIdentity!.ApiIdentitySources!.Add(apiIdentitySource);
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdentityJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultReadContext<PropertyNames, ReadData, ReadHandlers>
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create,
                buildReadHandlers: names => new ReadHandlers(names)
            );

    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create
            );

    protected override ApiIdentity? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiIdentity;

        var apiName = readData?.ApiName;
        var apiIdentitySources = readData?.ApiIdentitySources;

        var apiIdentity = new ApiIdentity(apiName!, apiIdentitySources!);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiIdentity, extensions);

        return apiIdentity;
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiIdentity value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentityApiName(writer, value, writeContext);
            WriteApiIdentityApiIdentitySources(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiIdentityApiName(Utf8JsonWriter writer, ApiIdentity apiIdentity, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentity.ApiName;
        var value = apiIdentity.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdentityApiIdentitySources(Utf8JsonWriter writer, ApiIdentity apiIdentity, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentity.ApiIdentitySources;
        var apiIdentitySources = apiIdentity.ApiIdentitySources;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiIdentitySources,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }
    #endregion
}
