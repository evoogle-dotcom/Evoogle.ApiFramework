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
///     Handles JSON serialization for <see cref="ApiIdentitySource"/> instances, including support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiIdentitySourceJsonConverter(ILogger<ApiIdentitySourceJsonConverter>? logger) : JsonConverterBase<ApiIdentitySource>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved property names for enum value members under a given naming policy.
    /// </summary>
    private readonly record struct ApiIdentitySourcePropertyNames
    {
        #region Immutable Properties
        public required string ApiKind { get; init; }
        public required string ApiPropertyName { get; init; }
        public required string ApiNestedName { get; init; }
        public required string ClrScalarType { get; init; }
        #endregion
    }

    /// <summary>
    ///     Combines property name metadata used by the converter while reading or writing values.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdentitySourcePropertyNames ApiIdentitySource { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentitySource = new ApiIdentitySourcePropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(Schema.ApiIdentitySource.ApiKind)),
                    ApiPropertyName = policy.ConvertName(nameof(Schema.ApiIdentitySource.ApiPropertyName)),
                    ApiNestedName = policy.ConvertName(nameof(Schema.ApiIdentitySource.ApiNestedName)),
                    ClrScalarType = policy.ConvertName(nameof(Schema.ApiIdentitySource.ClrScalarType)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used when deserializing individual identity source members.
    /// </summary>
    private class ApiIdentitySourceReadData
    {
        #region Properties
        public ApiIdentitySourceKind? ApiKind { get; set; }
        public string? ApiPropertyName { get; set; }
        public string? ApiNestedName { get; set; }
        public Type? ClrScalarType { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the fully parsed data required to construct an <see cref="ApiIdentitySource"/>.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiIdentitySourceReadData? ApiIdentitySource { get; set; }
        #endregion
    }

    /// <summary>
    ///     Maps JSON property names to handlers that populate <see cref="ReadData"/> during deserialization.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiIdentitySource Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiIdentitySource Property Handlers
            { propertyNames.ApiIdentitySource.ApiKind, HandleApiIdentitySourceApiKind },
            { propertyNames.ApiIdentitySource.ApiPropertyName, HandleApiIdentitySourceApiPropertyName },
            { propertyNames.ApiIdentitySource.ApiNestedName, HandleApiIdentitySourceApiNestedName },
            { propertyNames.ApiIdentitySource.ClrScalarType, HandleApiIdentitySourceClrScalarType },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiIdentitySource Methods
        private static void HandleApiIdentitySourceApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentitySource ??= new ApiIdentitySourceReadData();

            var options = context.Options;
            context.ReadData.ApiIdentitySource.ApiKind = _apiIdentitySourceKindJsonConverter.Read(ref reader, typeof(ApiIdentitySourceKind), options);
        }

        private static void HandleApiIdentitySourceApiPropertyName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentitySource ??= new ApiIdentitySourceReadData();

            context.ReadData.ApiIdentitySource.ApiPropertyName = reader.GetString();
        }

        private static void HandleApiIdentitySourceApiNestedName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentitySource ??= new ApiIdentitySourceReadData();

            context.ReadData.ApiIdentitySource.ApiNestedName = reader.GetString();
        }

        private static void HandleApiIdentitySourceClrScalarType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentitySource ??= new ApiIdentitySourceReadData();

            context.ReadData.ApiIdentitySource.ClrScalarType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiIdentitySourceKind> _apiIdentitySourceKindJsonConverter = new();

    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdentitySourceJsonConverter()
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

    protected override ApiIdentitySource? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiIdentitySource;

        var apiKind = readData?.ApiKind;
        var apiPropertyName = readData?.ApiPropertyName;
        var apiNestedName = readData?.ApiNestedName;
        var clrScalarType = readData?.ClrScalarType;

        var apiIdentitySource = default(ApiIdentitySource);
        if (apiKind is not null)
        {
            switch (apiKind.Value)
            {
                case ApiIdentitySourceKind.Scalar:
                    apiIdentitySource = ApiIdentitySource.Scalar(apiPropertyName!, clrScalarType);
                    break;

                case ApiIdentitySourceKind.Nested:
                    apiIdentitySource = ApiIdentitySource.Nested(apiPropertyName!, apiNestedName);
                    break;

                default:
                    readContext.Logger.LogError("Unsupported {Kind} enumeration value: '{KindValue}'", nameof(ApiIdentitySourceKind), apiKind);
                    break;
            }
        }

        apiIdentitySource ??= new ApiIdentitySource(apiPropertyName!, clrScalarType, apiNestedName);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiIdentitySource, extensions);

        return apiIdentitySource;
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiIdentitySource value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentitySourceApiKind(writer, value, writeContext);
            WriteApiIdentitySourceApiPropertyName(writer, value, writeContext);
            WriteApiIdentitySourceApiNestedName(writer, value, writeContext);
            WriteApiIdentitySourceClrScalarType(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiIdentitySourceApiKind(Utf8JsonWriter writer, ApiIdentitySource apiIdentitySource, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentitySource.ApiKind;
        var kind = apiIdentitySource.ApiKind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, kind, options, _apiIdentitySourceKindJsonConverter);
    }

    private static void WriteApiIdentitySourceApiPropertyName(Utf8JsonWriter writer, ApiIdentitySource apiIdentitySource, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentitySource.ApiPropertyName;
        var value = apiIdentitySource.ApiPropertyName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdentitySourceApiNestedName(Utf8JsonWriter writer, ApiIdentitySource apiIdentitySource, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentitySource.ApiNestedName;
        var value = apiIdentitySource.ApiNestedName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdentitySourceClrScalarType(Utf8JsonWriter writer, ApiIdentitySource apiIdentitySource, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentitySource.ClrScalarType;
        var value = apiIdentitySource.ClrScalarType;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _typeJsonConverter);
    }
    #endregion
}
