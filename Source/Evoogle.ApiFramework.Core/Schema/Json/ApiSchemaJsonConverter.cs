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
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiSchema Property Handlers
            { propertyNames.ApiSchema.ApiName, HandleApiName },
            { propertyNames.ApiSchema.ApiVersion, HandleApiVersion },
            { propertyNames.ApiSchema.ApiOptions, HandleApiOptions },
            { propertyNames.ApiSchema.ApiScalarTypes, HandleApiScalarTypes },
            { propertyNames.ApiSchema.ApiEnumTypes, HandleApiEnumTypes },
            { propertyNames.ApiSchema.ApiObjectTypes, HandleApiObjectTypes },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiSchema Methods
        private static void HandleApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.ApiName = reader.GetString();
        }

        private static void HandleApiVersion(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.ApiVersion = reader.GetString();
        }

        private static void HandleApiOptions(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            context.ReadData.ApiSchema.ApiOptions = JsonSerializer.Deserialize<ApiSchemaOptions>(ref reader, context.Options);
        }

        private static void HandleApiScalarTypes(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiSchema.ApiScalarTypes;
            var apiScalarTypes = DeserializeListOf<ApiType, ApiScalarType>(ref reader, options, propertyName);

            context.ReadData.ApiSchema.ApiScalarTypes = apiScalarTypes;
        }

        private static void HandleApiEnumTypes(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiSchema.ApiEnumTypes;
            var apiEnumTypes = DeserializeListOf<ApiType, ApiEnumType>(ref reader, options, propertyName);

            context.ReadData.ApiSchema.ApiEnumTypes = apiEnumTypes;
        }

        private static void HandleApiObjectTypes(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiSchema ??= new ApiSchemaReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiSchema.ApiObjectTypes;
            var apiObjectTypes = DeserializeListOf<ApiType, ApiObjectType>(ref reader, options, propertyName);

            context.ReadData.ApiSchema.ApiObjectTypes = apiObjectTypes;
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

        var apiSchema = new ApiSchema
        (
            apiName!,
            apiVersion,
            apiOptions,
            apiScalarTypes,
            apiEnumTypes,
            apiObjectTypes
        );

        // Attach the extensions if present.
        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiSchema, extensions);

        // Initialize the ApiSchema instance.
        var result = apiSchema.Initialize();
        result.ThrowIfInvalid();

        return apiSchema;
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiSchema value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            // Prolog
            WriteApiName(writer, value, writeContext);
            WriteApiVersion(writer, value, writeContext);
            WriteApiOptions(writer, value, writeContext);

            // Body
            WriteApiScalarTypes(writer, value, writeContext);
            WriteApiEnumTypes(writer, value, writeContext);
            WriteApiObjectTypes(writer, value, writeContext);

            // Epilog
            WriteExtensions(writer, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiName(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiName;
        var value = apiSchema.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiVersion(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiVersion;
        var value = apiSchema.ApiVersion;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiOptions(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiOptions;
        var apiSchemaOptions = apiSchema.ApiOptions;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, apiSchemaOptions, options);
    }

    private static void WriteApiScalarTypes(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiScalarTypes;
        var apiScalarTypes = apiSchema.ApiScalarTypes.Cast<ApiType>();
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiScalarTypes,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }

    private static void WriteApiEnumTypes(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiEnumTypes;
        var apiEnumTypes = apiSchema.ApiEnumTypes.Cast<ApiType>();
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiEnumTypes,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }

    private static void WriteApiObjectTypes(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiSchema.ApiObjectTypes;
        var apiObjectTypes = apiSchema.ApiObjectTypes.Cast<ApiType>();
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiObjectTypes,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }

    private static void WriteExtensions(Utf8JsonWriter writer, ApiSchema apiSchema, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ExtensibleBase.Extensions;

        WriteExtensibleBaseExtensions(writer, propertyName, apiSchema, context);
    }
    #endregion
}
