// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extension;
using Evoogle.Json;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     A JSON converter for <see cref="ApiType"/> that handles serialization and deserialization logic.
///     This converter supports reading and writing various derived types of <see cref="ApiType"/> including scalar, enum, object, and collection types, as well as handling custom property naming policies and extensions.
/// </summary>
public partial class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Context Types
    private abstract class Context(ILogger<ApiTypeJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames) : IHasLogger<ApiTypeJsonConverter>
    {
        #region Immutable Properties
        public ILogger<ApiTypeJsonConverter> Logger { get; } = logger;
        public JsonSerializerOptions Options { get; } = options;
        public JsonNamingPolicy PropertyNamingPolicy { get; } = propertyNamingPolicy;
        public PropertyNames PropertyNames { get; } = propertyNames;
        #endregion
    }

    private class ReadContext(ILogger<ApiTypeJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames, ReadHandlers readHandlers)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
        #region Immutable Properties
        public ReadHandlers ReadHandlers { get; } = readHandlers;
        #endregion

        #region Mutable Properties
        // Scratchpad for temporarily holding parsed values before type instantiation
        public ReadData ReadData { get; } = new ReadData();
        #endregion
    }

    private class WriteContext(ILogger<ApiTypeJsonConverter> logger, JsonSerializerOptions options, JsonNamingPolicy propertyNamingPolicy, PropertyNames propertyNames)
        : Context(logger, options, propertyNamingPolicy, propertyNames)
    {
    }
    #endregion

    #region Property Types
    private readonly record struct ApiCollectionTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiItemType { get; init; }
        public required string ApiItemTypeModifiers { get; init; }
        #endregion
    }

    private readonly record struct ApiEnumTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiEnumValues { get; init; }
        #endregion
    }

    private readonly record struct ApiEnumValuePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ClrName { get; init; }
        public required string ClrOrdinal { get; init; }
        #endregion
    }

    private readonly record struct ApiNamedTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        #endregion
    }
    private readonly record struct ApiObjectTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiProperties { get; init; }
        #endregion
    }

    private readonly record struct ApiPropertyPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiType { get; init; }
        public required string ApiTypeModifiers { get; init; }
        public required string ClrName { get; init; }
        #endregion
    }

    private readonly record struct ApiTypePropertyNames
    {
        #region Immutable Properties
        public required string ClrType { get; init; }
        public required string Kind { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiCollectionTypePropertyNames ApiCollectionType { get; init; }
        public required ApiEnumTypePropertyNames ApiEnumType { get; init; }
        public required ApiEnumValuePropertyNames ApiEnumValue { get; init; }
        public required ApiNamedTypePropertyNames ApiNamedType { get; init; }
        public required ApiObjectTypePropertyNames ApiObjectType { get; init; }
        public required ApiPropertyPropertyNames ApiProperty { get; init; }
        public required ApiTypePropertyNames ApiType { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion
    }
    #endregion

    #region Fields
    private readonly ILogger<ApiTypeJsonConverter> _logger;

    private static readonly EnumJsonConverter<ApiTypeKind> ApiTypeKindJsonConverter = new();
    private static readonly EnumJsonConverter<ApiTypeModifiers> ApiTypeModifiersJsonConverter = new();

    private static readonly TypeJsonConverter TypeJsonConverter = new();

    // Cache resolved property names per naming policy for performance and consistency
    private static readonly ConcurrentDictionary<JsonNamingPolicy, PropertyNames> PropertyNamesCache = new();

    // Cache read handlers per naming policy to avoid rebuilding on every call
    private static readonly ConcurrentDictionary<JsonNamingPolicy, ReadHandlers> ReadHandlersCache = new();
    #endregion

    #region Constructors
    /// <summary>
    ///     Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.
    /// </summary>
    public ApiTypeJsonConverter()
        : this(null)
    {
    }

    /// <summary>
    ///     Optional constructor with logger for use in DI contexts.
    /// </summary>
    /// <param name="logger">The optional logger instance.</param>
    public ApiTypeJsonConverter(ILogger<ApiTypeJsonConverter>? logger)
    {
        _logger = new MultiplexingLogger<ApiTypeJsonConverter>(logger, MultiplexingLoggerMode.All);
    }
    #endregion

    #region JsonConverter<T> Methods
    /// <summary>
    ///     Reads a JSON representation of an <see cref="ApiType"/> into a CLR object.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The target type (expected to be <see cref="ApiType"/>).</param>
    /// <param name="options">The serializer options in effect.</param>
    /// <returns>An <see cref="ApiType"/> instance.</returns>
    public override ApiType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var readHandlers = GetReadHandlers(propertyNamingPolicy, propertyNames);
        var context = new ReadContext(_logger, options, propertyNamingPolicy, propertyNames, readHandlers);

        context.Logger.LogTrace("Deserializing {ApiType}", nameof(ApiType));

        ApiJsonConverterHelpers.ReadJsonObject<ApiTypeJsonConverter, ReadContext>(ref reader, ref context, (context) => context.ReadHandlers.ApiTypePropertyHandlers);

        var kind = context.ReadData.ApiType?.Kind;
        var clrType = context.ReadData.ApiType?.ClrType;

        var apiType = CreateApiType(context);

        context.Logger.LogDebug("Deserialized  {ApiType}", apiType);

        return apiType;
    }

    /// <summary>
    ///     Writes an <see cref="ApiType"/> instance into its JSON representation.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="apiType">The object to write.</param>
    /// <param name="options">The serializer options in effect.</param>
    public override void Write(Utf8JsonWriter writer, ApiType apiType, JsonSerializerOptions options)
    {
        var propertyNamingPolicy = GetPropertyNamingPolicy(options);
        var propertyNames = GetPropertyNames(propertyNamingPolicy);
        var context = new WriteContext(_logger, options, propertyNamingPolicy, propertyNames);

        context.Logger.LogTrace("Serializing {ApiType}", apiType);

        WriteApiTypeProlog(writer, apiType, context);
        WriteApiTypeBody(writer, apiType, context);
        WriteApiTypeEpilog(writer, apiType, context);

        context.Logger.LogDebug("Serialized  {ApiType}", apiType);
    }
    #endregion

    #region Cache Implementation Methods
    private static JsonNamingPolicy GetPropertyNamingPolicy(JsonSerializerOptions options)
    {
        return ApiJsonConverterHelpers.GetPropertyNamingPolicy(options);
    }

    private static PropertyNames GetPropertyNames(JsonNamingPolicy policy)
    {
        return PropertyNamesCache.GetOrAdd(policy, policy => new PropertyNames
        {
            ApiCollectionType = new ApiCollectionTypePropertyNames
            {
                ApiItemType = policy.ConvertName(nameof(ApiCollectionType.ApiItemType)),
                ApiItemTypeModifiers = policy.ConvertName(nameof(ApiCollectionType.ApiItemTypeModifiers))
            },
            ApiEnumType = new ApiEnumTypePropertyNames
            {
                ApiEnumValues = policy.ConvertName(nameof(ApiEnumType.ApiEnumValues))
            },
            ApiEnumValue = new ApiEnumValuePropertyNames
            {
                ApiName = policy.ConvertName(nameof(ApiEnumValue.ApiName)),
                ClrName = policy.ConvertName(nameof(ApiEnumValue.ClrName)),
                ClrOrdinal = policy.ConvertName(nameof(ApiEnumValue.ClrOrdinal))
            },
            ApiNamedType = new ApiNamedTypePropertyNames
            {
                ApiName = policy.ConvertName(nameof(ApiNamedType.ApiName))
            },
            ApiObjectType = new ApiObjectTypePropertyNames
            {
                ApiProperties = policy.ConvertName(nameof(ApiObjectType.ApiProperties))
            },
            ApiProperty = new ApiPropertyPropertyNames
            {
                ApiName = policy.ConvertName(nameof(ApiProperty.ApiName)),
                ApiType = policy.ConvertName(nameof(ApiProperty.ApiType)),
                ApiTypeModifiers = policy.ConvertName(nameof(ApiProperty.ApiTypeModifiers)),
                ClrName = policy.ConvertName(nameof(ApiProperty.ClrName))
            },
            ApiType = new ApiTypePropertyNames
            {
                ClrType = policy.ConvertName(nameof(ApiType.ClrType)),
                Kind = policy.ConvertName(nameof(ApiType.Kind))
            },
            ExtensibleBase = new ExtensibleBasePropertyNames
            {
                Extensions = policy.ConvertName(nameof(ExtensibleBase.Extensions))
            }
        });
    }

    private static ReadHandlers GetReadHandlers(JsonNamingPolicy policy, PropertyNames propertyNames)
    {
        return ReadHandlersCache.GetOrAdd(policy, policy => new ReadHandlers(propertyNames));
    }
    #endregion
}
