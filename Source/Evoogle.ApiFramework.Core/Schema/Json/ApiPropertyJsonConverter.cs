// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Text.Json;

using Evoogle.Extension;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Serializes and deserializes <see cref="ApiProperty"/> instances, including extension payloads and type expressions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiPropertyJsonConverter(ILogger<ApiPropertyJsonConverter>? logger) : JsonConverterBase<ApiProperty>(logger)
{
    #region Context Types
    /// <summary>
    ///     Represents the immutable state shared by read and write operations for <see cref="ApiProperty"/> conversion.
    /// </summary>
    private abstract class Context(ILogger logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames) : IContext
    {
        #region Immutable Properties
        public ILogger Logger { get; } = logger;
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
        #endregion
    }

    /// <summary>
    ///     Captures transient state while reading a property from JSON.
    /// </summary>
    private class ReadContext(ILogger logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(logger, options, propertyNamingPolicy, propertyNames), IReadContext
    {
        #region Immutable Properties
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        #endregion

        #region Mutable Properties
        public ReadData ReadData { get; } = new ReadData();
        #endregion
    }

    /// <summary>
    ///     Provides contextual information required while writing a property to JSON.
    /// </summary>
    private class WriteContext(ILogger logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames), IWriteContext
    {
    }
    #endregion

    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiProperty"/> under a given naming policy.
    /// </summary>
    private readonly record struct ApiPropertyPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiTypeExpression { get; init; }
        public required string ApiTypeModifiers { get; init; }
        public required string ClrName { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiPropertyPropertyNames ApiProperty { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading property members from JSON.
    /// </summary>
    private class ApiPropertyReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiTypeExpression? ApiTypeExpression { get; set; }
        public ApiTypeModifiers? ApiTypeModifiers { get; set; }
        public string? ClrName { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiProperty"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiPropertyReadData ApiProperty { get; } = new();
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _apiTypeModifiersType = typeof(ApiTypeModifiers);
        #endregion

        #region ApiProperty Fields
        public readonly Dictionary<string, JsonReaderHandler<ReadContext>> ApiPropertyPropertyHandlers = new()
        {
            // ApiProperty Property Handlers
            { propertyNames.ApiProperty.ApiName, HandleApiPropertyApiName },
            { propertyNames.ApiProperty.ApiTypeModifiers, HandleApiPropertyApiTypeModifiers },
            { propertyNames.ApiProperty.ApiTypeExpression, HandleApiPropertyApiTypeExpression },
            { propertyNames.ApiProperty.ClrName, HandleApiPropertyClrName },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, (ref Utf8JsonReader reader, ReadContext context) =>
                context.ReadData.Extensions = ReadJsonExtensionsObject(ref reader, context) },
        };
        #endregion

        #region ApiProperty Methods
        private static void HandleApiPropertyApiName(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiProperty.ApiName = reader.GetString();
        }

        private static void HandleApiPropertyApiTypeModifiers(ref Utf8JsonReader reader, ReadContext context)
        {
            var options = context.Options;
            context.ReadData.ApiProperty.ApiTypeModifiers = _apiTypeModifiersJsonConverter.Read(ref reader, _apiTypeModifiersType, options);
        }

        private static void HandleApiPropertyApiTypeExpression(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiProperty.ApiTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, context.Options);
        }

        private static void HandleApiPropertyClrName(ref Utf8JsonReader reader, ReadContext context)
        {
            context.ReadData.ApiProperty.ClrName = reader.GetString();
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeModifiers> _apiTypeModifiersJsonConverter = new();

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> _propertyNamesCache = new();

    // Cache read handlers per naming policy to avoid rebuilding on every call
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> _readHandlersCache = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiPropertyJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = options.GetPropertyNamingPolicy();
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var readHandlers = GetReadHandlers(propertyNamingPolicy, propertyNames);
        var context = new ReadContext(logger, options, propertyNamingPolicy, propertyNames, readHandlers);

        return context;
    }

    protected override ApiProperty? CreateValue(IReadContext context)
    {
        var readContext = (ReadContext)context;

        var apiPropertyReadData = readContext.ReadData.ApiProperty;
        var apiProperty = CreateApiProperty(apiPropertyReadData);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiProperty, extensions);

        return apiProperty;
    }

    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = options.GetPropertyNamingPolicy();
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var context = new WriteContext(logger, options, propertyNamingPolicy, propertyNames);

        return context;
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (ReadContext)context;
        ReadJsonObject(ref reader, readContext, (readContext) => readContext.ReadHandlers.ApiPropertyPropertyHandlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiProperty value, IWriteContext context)
    {
        var writeContext = (WriteContext)context;
        WriteApiProperty(writer, value, writeContext);
    }
    #endregion

    #region Cache Implementation Methods
    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return _propertyNamesCache.GetOrAdd(policy, policy => new PropertyNames
        {
            ApiProperty = new ApiPropertyPropertyNames
            {
                ApiName = policy.ConvertName(nameof(ApiProperty.ApiName)),
                ApiTypeExpression = policy.ConvertName(nameof(ApiProperty.ApiType)), // Mapping property name from ApiTypeExpression to ApiType by design
                ApiTypeModifiers = policy.ConvertName(nameof(ApiProperty.ApiTypeModifiers)),
                ClrName = policy.ConvertName(nameof(ApiProperty.ClrName))
            },
            ExtensibleBase = new ExtensibleBasePropertyNames
            {
                Extensions = policy.ConvertName(nameof(ExtensibleBase.Extensions))
            }
        });
    }

    private static ReadHandlers GetReadHandlers(JsonNamingPolicy policy, PropertyNames propertyNames) => _readHandlersCache.GetOrAdd(policy, policy => new ReadHandlers(propertyNames));
    #endregion

    #region Factory Implementation Methods
    private static ApiProperty CreateApiProperty(ApiPropertyReadData apiPropertyReadData)
    {
        var apiName = apiPropertyReadData.ApiName;
        var apiTypeExpression = apiPropertyReadData.ApiTypeExpression;
        var apiTypeModifiers = apiPropertyReadData.ApiTypeModifiers ?? ApiTypeModifiers.None;
        var clrName = apiPropertyReadData.ClrName;

        var apiProperty = new ApiProperty(apiName!, apiTypeExpression!, apiTypeModifiers, clrName!);
        return apiProperty;
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiProperty(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        writer.WriteStartObject();

        WriteApiPropertyApiName(writer, apiProperty, context);
        WriteApiPropertyApiTypeExpression(writer, apiProperty, context);
        WriteApiPropertyApiTypeModifiers(writer, apiProperty, context);
        WriteApiPropertyClrName(writer, apiProperty, context);

        WriteExtensibleBaseExtensions(writer, context.PropertyNames.ExtensibleBase.Extensions, apiProperty, context);

        writer.WriteEndObject();
    }

    private static void WriteApiPropertyApiName(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiName;
        var value = apiProperty.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiPropertyApiTypeExpression(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiTypeExpression;
        var apiTypeExpression = apiProperty.ApiTypeExpression;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, apiTypeExpression, options);
    }

    private static void WriteApiPropertyApiTypeModifiers(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiTypeModifiers;
        var apiTypeModifiers = apiProperty.ApiTypeModifiers;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, apiTypeModifiers, options, _apiTypeModifiersJsonConverter);
    }

    private static void WriteApiPropertyClrName(Utf8JsonWriter writer, ApiProperty apiProperty, WriteContext context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ClrName;
        var value = apiProperty.ClrName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }
    #endregion
}
