// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     JSON converter for <see cref="ApiSchema"/> which reads and writes schema objects.
///     Follows the same patterns used by <see cref="ApiTypeJsonConverter"/>.
/// </summary>
/// <remarks>
///     Optional constructor with logger for use in DI contexts.
/// </remarks>
/// <param name="logger">The optional logger instance.</param>
public class ApiSchemaJsonConverter(ILogger<ApiSchemaJsonConverter>? logger) : JsonConverterBase<ApiSchema>(logger)
{
    #region Property Types
    /// <summary>
    ///     Represents the JSON property names associated with an <see cref="ApiSchema"/> for a given naming policy.
    /// </summary>
    private readonly record struct ApiSchemaPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiVersion { get; init; }
        public required string ApiOptions { get; init; }
        public required string ApiScalarTypes { get; init; }
        public required string ApiEnumTypes { get; init; }
        public required string ApiObjectTypes { get; init; }
        public required string ApiRelationships { get; init; }
        public required string ApiQuerySources { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates all property name metadata used during schema serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiSchemaPropertyNames ApiSchema { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiSchema = new ApiSchemaPropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Schema.ApiSchema.ApiName)),
                    ApiVersion = policy.ConvertName(nameof(Schema.ApiSchema.ApiVersion)),
                    ApiOptions = policy.ConvertName(nameof(Schema.ApiSchema.ApiOptions)),
                    ApiScalarTypes = policy.ConvertName(nameof(Schema.ApiSchema.ApiScalarTypes)),
                    ApiEnumTypes = policy.ConvertName(nameof(Schema.ApiSchema.ApiEnumTypes)),
                    ApiObjectTypes = policy.ConvertName(nameof(Schema.ApiSchema.ApiObjectTypes)),
                    ApiRelationships = policy.ConvertName(nameof(Schema.ApiSchema.ApiRelationships)),
                    ApiQuerySources = policy.ConvertName(nameof(Schema.ApiSchema.ApiQuerySources)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while parsing an <see cref="ApiSchema"/> from JSON.
    /// </summary>
    private class ApiSchemaReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public string? ApiVersion { get; set; }
        public ApiSchemaOptions? ApiOptions { get; set; }
        public List<ApiScalarType>? ApiScalarTypes { get; set; }
        public List<ApiEnumType>? ApiEnumTypes { get; set; }
        public List<ApiObjectType>? ApiObjectTypes { get; set; }
        public List<ApiRelationship>? ApiRelationships { get; set; }
        public List<ApiQuerySource>? ApiQuerySources { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the complete data set required to instantiate an <see cref="ApiSchema"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiSchemaReadData? ApiSchema { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides property handlers that map JSON members to <see cref="ReadData"/> assignments.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiSchema Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiSchema Property Handlers
            { propertyNames.ApiSchema.ApiName, HandleApiSchemaApiName },
            { propertyNames.ApiSchema.ApiVersion, HandleApiSchemaApiVersion },
            { propertyNames.ApiSchema.ApiOptions, HandleApiSchemaApiOptions },
            { propertyNames.ApiSchema.ApiScalarTypes, HandleApiSchemaApiScalarTypes },
            { propertyNames.ApiSchema.ApiEnumTypes, HandleApiSchemaApiEnumTypes },
            { propertyNames.ApiSchema.ApiObjectTypes, HandleApiSchemaApiObjectTypes },
            { propertyNames.ApiSchema.ApiRelationships, HandleApiSchemaApiRelationships },
            { propertyNames.ApiSchema.ApiQuerySources, HandleApiSchemaApiQuerySources },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiSchema Methods
        private static void HandleApiSchemaApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.ApiName = reader.GetString();
        }

        private static void HandleApiSchemaApiVersion(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.ApiVersion = reader.GetString();
        }

        private static void HandleApiSchemaApiOptions(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.ApiOptions = JsonSerializer.Deserialize<ApiSchemaOptions>(ref reader, context.Options);
        }

        private static void HandleApiSchemaApiScalarTypes(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiSchema.ApiScalarTypes;
            var apiScalarTypes = DeserializeListOf<ApiType, ApiScalarType>(ref reader, options, propertyName);

            context.ReadData.ApiSchema.ApiScalarTypes = apiScalarTypes;
        }

        private static void HandleApiSchemaApiEnumTypes(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiSchema.ApiEnumTypes;
            var apiEnumTypes = DeserializeListOf<ApiType, ApiEnumType>(ref reader, options, propertyName);

            context.ReadData.ApiSchema.ApiEnumTypes = apiEnumTypes;
        }

        private static void HandleApiSchemaApiObjectTypes(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiSchema.ApiObjectTypes;
            var apiObjectTypes = DeserializeListOf<ApiType, ApiObjectType>(ref reader, options, propertyName);

            context.ReadData.ApiSchema.ApiObjectTypes = apiObjectTypes;
        }

        private static void HandleApiSchemaApiRelationships(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiSchema.ApiRelationships;
            var apiRelationships = DeserializeListOf<ApiRelationship>(ref reader, options, propertyName);

            context.ReadData.ApiSchema.ApiRelationships = apiRelationships;
        }

        private static void HandleApiSchemaApiQuerySources(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiSchema.ApiQuerySources;
            var apiQuerySources = DeserializeListOf<ApiQuerySource>(ref reader, options, propertyName);

            context.ReadData.ApiSchema.ApiQuerySources = apiQuerySources;
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiSchemaJsonConverter()
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
    protected override ApiSchema? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;

        // Create the ApiSchema instance using the read data.
        var apiName = readContext.ReadData.ApiSchema?.ApiName;
        var apiVersion = readContext.ReadData.ApiSchema?.ApiVersion;
        var apiOptions = readContext.ReadData.ApiSchema?.ApiOptions;
        var apiScalarTypes = readContext.ReadData.ApiSchema?.ApiScalarTypes;
        var apiEnumTypes = readContext.ReadData.ApiSchema?.ApiEnumTypes;
        var apiObjectTypes = readContext.ReadData.ApiSchema?.ApiObjectTypes;
        var apiRelationships = readContext.ReadData.ApiSchema?.ApiRelationships;
        var apiQuerySources = readContext.ReadData.ApiSchema?.ApiQuerySources;

        var apiSchema = new ApiSchema
        (
            apiName!,
            apiVersion,
            apiOptions,
            apiScalarTypes,
            apiEnumTypes,
            apiObjectTypes,
            apiRelationships,
            apiQuerySources
        );

        // Attach the extensions if present.
        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiSchema, extensions);

        // Compile and freeze the API schema instance before publishing it.
        var result = ApiSchemaCompiler.Compile(apiSchema);
        result.ThrowIfInvalid();

        return result.Schema;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiSchema apiSchema, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiSchema, writeContext), writeObject: static (writer, state) =>
        {
            var (apiSchema, writeContext) = state;

            // Prolog
            WriteApiSchemaApiName(writer, apiSchema, writeContext);
            WriteApiSchemaApiVersion(writer, apiSchema, writeContext);
            WriteApiSchemaApiOptions(writer, apiSchema, writeContext);

            // Body
            WriteApiSchemaApiScalarTypes(writer, apiSchema, writeContext);
            WriteApiSchemaApiEnumTypes(writer, apiSchema, writeContext);
            WriteApiSchemaApiObjectTypes(writer, apiSchema, writeContext);
            WriteApiSchemaApiRelationships(writer, apiSchema, writeContext);
            WriteApiSchemaApiQuerySources(writer, apiSchema, writeContext);

            // Epilog
            WriteExtensions(writer, apiSchema, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiSchemaApiName(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiSchema.ApiName, value: apiSchema.ApiName, options: writeContext.Options);

    private static void WriteApiSchemaApiVersion(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiSchema.ApiVersion, value: apiSchema.ApiVersion, options: writeContext.Options);

    private static void WriteApiSchemaApiOptions(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiSchema.ApiOptions, obj: apiSchema.ApiOptions, options: writeContext.Options);

    private static void WriteApiSchemaApiScalarTypes(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ApiSchema.ApiScalarTypes;
        var apiScalarTypes = apiSchema.ApiScalarTypes.Cast<ApiType>();
        var options = writeContext.Options;

        writer.TryWritePropertyAsArray
        (
            propertyName: propertyName,
            collection: apiScalarTypes,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
                writer.TryWriteWithSerializer(obj: item, options: state.Options)
        );
    }

    private static void WriteApiSchemaApiEnumTypes(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ApiSchema.ApiEnumTypes;
        var apiEnumTypes = apiSchema.ApiEnumTypes.Cast<ApiType>();
        var options = writeContext.Options;

        writer.TryWritePropertyAsArray
        (
            propertyName: propertyName,
            collection: apiEnumTypes,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
                writer.TryWriteWithSerializer(obj: item, options: state.Options)
        );
    }

    private static void WriteApiSchemaApiObjectTypes(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ApiSchema.ApiObjectTypes;
        var apiObjectTypes = apiSchema.ApiObjectTypes.Cast<ApiType>();
        var options = writeContext.Options;

        writer.TryWritePropertyAsArray
        (
            propertyName: propertyName,
            collection: apiObjectTypes,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
                writer.TryWriteWithSerializer(obj: item, options: state.Options)
        );
    }

    private static void WriteApiSchemaApiRelationships(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ApiSchema.ApiRelationships;
        var apiRelationships = apiSchema.ApiRelationships;
        var options = writeContext.Options;

        writer.TryWritePropertyAsArray
        (
            propertyName: propertyName,
            collection: apiRelationships,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
                writer.TryWriteWithSerializer(obj: item, options: state.Options)
        );
    }

    private static void WriteApiSchemaApiQuerySources(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ApiSchema.ApiQuerySources;
        var apiQuerySources = apiSchema.ApiQuerySources;
        var options = writeContext.Options;

        writer.TryWritePropertyAsArray
        (
            propertyName: propertyName,
            collection: apiQuerySources,
            state: writeContext,
            options: writeContext.Options,
            writeItem: static (writer, item, state) =>
                writer.TryWriteWithSerializer(obj: item, options: state.Options)
        );
    }

    private static void WriteExtensions(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> writeContext)
    {
        var propertyName = writeContext.PropertyNames.ExtensibleBase.Extensions;

        WriteExtensibleBaseExtensions(writer, propertyName, apiSchema, writeContext);
    }
    #endregion
}
