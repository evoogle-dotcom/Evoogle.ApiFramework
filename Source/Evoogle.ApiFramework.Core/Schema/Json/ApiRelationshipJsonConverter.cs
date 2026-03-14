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
///     Provides System.Text.Json serialization support for <see cref="ApiRelationship"/> instances, including
///     extension payloads and schema-specific naming policies.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipJsonConverter(ILogger<ApiRelationshipJsonConverter>? logger) : JsonConverterBase<ApiRelationship>(logger)
{
    #region Property Types
    /// <summary>
    ///     Provides cached JSON property names for <see cref="ApiRelationship"/> members under a specific naming policy.
    /// </summary>
    private readonly record struct ApiRelationshipPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiPropertyName { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates the property name sets used while reading or writing relationships and extension data.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiRelationshipPropertyNames ApiRelationship { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationship = new ApiRelationshipPropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Schema.ApiRelationship.ApiName)),
                    ApiPropertyName = policy.ConvertName(nameof(Schema.ApiRelationship.ApiPropertyName))
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
    private class ApiRelationshipReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public string? ApiPropertyName { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects all data encountered while deserializing a relationship, including extensions.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiRelationshipReadData? ApiRelationship { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides handlers that map JSON property names to strongly typed relationship data assignments.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiRelationship Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiRelationship Property Handlers
            { propertyNames.ApiRelationship.ApiName, HandleApiRelationshipApiName },
            { propertyNames.ApiRelationship.ApiPropertyName, HandleApiRelationshipApiPropertyName },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiRelationship Methods
        private static void HandleApiRelationshipApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();

            context.ReadData.ApiRelationship.ApiName = reader.GetString();
        }

        private static void HandleApiRelationshipApiPropertyName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();

            context.ReadData.ApiRelationship.ApiPropertyName = reader.GetString();
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipJsonConverter()
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
    protected override ApiRelationship? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiRelationship;

        var apiName = readData?.ApiName;
        var apiPropertyName = readData?.ApiPropertyName;

        var apiRelationship = new ApiRelationship(apiName!, apiPropertyName);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiRelationship, extensions);

        return apiRelationship;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationship value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiRelationshipApiName(writer, value, writeContext);
            WriteApiRelationshipApiPropertyName(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiRelationshipApiName(Utf8JsonWriter writer, ApiRelationship apiRelationship, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationship.ApiName;
        var value = apiRelationship.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiRelationshipApiPropertyName(Utf8JsonWriter writer, ApiRelationship apiRelationship, DefaultWriteContext<PropertyNames> context)
    {
        var apiName = apiRelationship.ApiName;
        var apiPropertyName = apiRelationship.ApiPropertyName;
        if (apiName.Equals(apiPropertyName))
        {
            // If the API name and property name are the same, we do not need to write the property name.
            return;
        }

        var propertyName = context.PropertyNames.ApiRelationship.ApiPropertyName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, apiPropertyName, options);
    }
    #endregion
}
