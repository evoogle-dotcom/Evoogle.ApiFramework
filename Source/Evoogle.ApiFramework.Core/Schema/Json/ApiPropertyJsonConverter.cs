// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extension;
using Evoogle.Json;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

using static Evoogle.ApiFramework.Schema.Json.Internal.ApiJsonConverterHelpers;

namespace Evoogle.ApiFramework.Schema.Json;

public class ApiPropertyJsonConverter(ILogger<ApiPropertyJsonConverter>? logger) : JsonConverter<ApiProperty>
{
    #region Context Types
    private abstract class Context(ILogger<ApiPropertyJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames) : IHasLogger<ApiPropertyJsonConverter>
    {
        #region Immutable Properties
        public ILogger<ApiPropertyJsonConverter> Logger { get; } = new MultiplexingLogger<ApiPropertyJsonConverter>(logger, MultiplexingLoggerMode.All);
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
        #endregion
    }

    private class ReadContext(ILogger<ApiPropertyJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
        #region Immutable Properties
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        #endregion

        #region Mutable Properties
        public ReadData ReadData { get; } = new ReadData();
        #endregion
    }

    private class WriteContext(ILogger<ApiPropertyJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
    }
    #endregion

    #region Property Types
    private readonly record struct ApiPropertyPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiTypeExpression { get; init; }
        public required string ApiTypeModifiers { get; init; }
        public required string ClrName { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiPropertyPropertyNames ApiProperty { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiPropertyReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiTypeExpression? ApiTypeExpression { get; set; }
        public ApiTypeModifiers? ApiTypeModifiers { get; set; }
        public string? ClrName { get; set; }
        #endregion
    }

    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiPropertyReadData ApiProperty { get; } = new();
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _apiTypeModifiersType = typeof(ApiTypeModifiers);
        #endregion

        #region ApiProperty Fields
        public readonly Dictionary<string, ApiJsonReaderHandler<ReadContext>> ApiPropertyPropertyHandlers = new()
        {
            // ApiProperty Property Handlers
            { propertyNames.ApiProperty.ApiName, HandleApiPropertyApiName },
            { propertyNames.ApiProperty.ApiTypeModifiers, HandleApiPropertyApiTypeModifiers },
            { propertyNames.ApiProperty.ApiTypeExpression, HandleApiPropertyApiTypeExpression },
            { propertyNames.ApiProperty.ClrName, HandleApiPropertyClrName },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, (ref Utf8JsonReader reader, ref ReadContext context) =>
                context.ReadData.Extensions = ReadExtensions(ref reader, context.Options, context.Logger) },
        };
        #endregion

        #region ApiProperty Methods
        private static void HandleApiPropertyApiName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiProperty.ApiName = reader.GetString();
        }

        private static void HandleApiPropertyApiTypeModifiers(ref Utf8JsonReader reader, ref ReadContext context)
        {
            var options = context.Options;
            context.ReadData.ApiProperty.ApiTypeModifiers = _apiTypeModifiersJsonConverter.Read(ref reader, _apiTypeModifiersType, options);
        }

        private static void HandleApiPropertyApiTypeExpression(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiProperty.ApiTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, context.Options);
        }

        private static void HandleApiPropertyClrName(ref Utf8JsonReader reader, ref ReadContext context)
        {
            context.ReadData.ApiProperty.ClrName = reader.GetString();
        }
        #endregion

    }
    #endregion

    #region Fields
    private readonly ILogger<ApiPropertyJsonConverter> _logger = new MultiplexingLogger<ApiPropertyJsonConverter>(logger, MultiplexingLoggerMode.All);

    private static readonly EnumJsonConverter<ApiTypeModifiers> _apiTypeModifiersJsonConverter = new();

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> _propertyNamesCache = new();

    // Cache read handlers per naming policy to avoid rebuilding on every call
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> _readHandlersCache = new();
    #endregion

    #region Constructors
    /// <summary>
    ///     Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.
    /// </summary>
    public ApiPropertyJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverter<T> Methods
    public override ApiProperty? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var readHandlers = GetReadHandlers(propertyNamingPolicy, propertyNames);
        var context = new ReadContext(_logger, options, propertyNamingPolicy, propertyNames, readHandlers);

        context.Logger.LogTrace("Deserializing {ApiProperty}", nameof(ApiProperty));

        ReadJsonObject<ApiPropertyJsonConverter, ReadContext>(ref reader, ref context, (context) => context.ReadHandlers.ApiPropertyPropertyHandlers);

        var apiProperty = CreateApiProperty(context);

        context.Logger.LogDebug("Deserialized  {ApiProperty}", apiProperty);

        return apiProperty;
    }

    public override void Write(Utf8JsonWriter writer, ApiProperty apiProperty, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var context = new WriteContext(_logger, options, propertyNamingPolicy, propertyNames);

        context.Logger.LogTrace("Serializing {ApiProperty}", apiProperty);

        WriteApiProperty(writer, apiProperty, context);

        context.Logger.LogDebug("Serialized  {ApiProperty}", apiProperty);
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
    private static ApiProperty CreateApiProperty(ReadContext context)
    {
        var apiPropertyReadData = context.ReadData.ApiProperty;
        var apiProperty = CreateApiProperty(apiPropertyReadData);

        var extensions = context.ReadData.Extensions;
        AttachExtensions(apiProperty, extensions);

        return apiProperty;
    }

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

        WriteExtensibleBaseExtensions(writer, apiProperty, context.PropertyNames.ExtensibleBase.Extensions, context.Options, context.Logger);
        
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
