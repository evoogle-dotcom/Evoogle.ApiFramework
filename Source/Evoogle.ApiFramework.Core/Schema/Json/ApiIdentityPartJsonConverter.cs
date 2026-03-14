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
///     Handles JSON serialization for <see cref="ApiIdentityPart"/> instances, including support for extensions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiIdentityPartJsonConverter(ILogger<ApiIdentityPartJsonConverter>? logger) : JsonConverterBase<ApiIdentityPart>(logger)
{
    #region Property Types
    private readonly record struct ApiIdentityPartPropertyNames
    {
        #region Immutable Properties
        public required string ApiKind { get; init; }
        public required string ApiPropertyName { get; init; }
        public required string ApiIdentityName { get; init; }
        public required string ClrScalarTypeHint { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdentityPartPropertyNames ApiIdentityPart { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentityPart = new ApiIdentityPartPropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(ApiIdentityPart.ApiKind)),
                    ApiPropertyName = policy.ConvertName(nameof(ApiIdentityPart.ApiPropertyName)),
                    ApiIdentityName = policy.ConvertName(nameof(ApiIdentityPart.ApiIdentityName)),
                    ClrScalarTypeHint = policy.ConvertName(nameof(ApiIdentityPart.ClrScalarTypeHint)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiIdentityPartReadData
    {
        #region Properties
        public ApiIdentityPartKind? ApiKind { get; set; }
        public string? ApiPropertyName { get; set; }
        public string? ApiIdentityName { get; set; }
        public Type? ClrScalarTypeHint { get; set; }
        #endregion
    }

    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiIdentityPartReadData? ApiIdentityPart { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiIdentityPart Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiIdentityPart Property Handlers
            { propertyNames.ApiIdentityPart.ApiKind, HandleApiIdentityPartApiKind },
            { propertyNames.ApiIdentityPart.ApiPropertyName, HandleApiIdentityPartApiPropertyName },
            { propertyNames.ApiIdentityPart.ApiIdentityName, HandleApiIdentityPartApiIdentityName },
            { propertyNames.ApiIdentityPart.ClrScalarTypeHint, HandleApiIdentityPartClrScalarTypeHint },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiIdentityPart Methods
        private static void HandleApiIdentityPartApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPart ??= new ApiIdentityPartReadData();

            var options = context.Options;
            context.ReadData.ApiIdentityPart.ApiKind = _apiIdentityPartKindJsonConverter.Read(ref reader, typeof(ApiIdentityPartKind), options);
        }

        private static void HandleApiIdentityPartApiPropertyName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPart ??= new ApiIdentityPartReadData();

            context.ReadData.ApiIdentityPart.ApiPropertyName = reader.GetString();
        }

        private static void HandleApiIdentityPartApiIdentityName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPart ??= new ApiIdentityPartReadData();

            context.ReadData.ApiIdentityPart.ApiIdentityName = reader.GetString();
        }

        private static void HandleApiIdentityPartClrScalarTypeHint(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPart ??= new ApiIdentityPartReadData();

            context.ReadData.ApiIdentityPart.ClrScalarTypeHint = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiIdentityPartKind> _apiIdentityPartKindJsonConverter = new();

    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdentityPartJsonConverter()
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
    protected override ApiIdentityPart? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiIdentityPart;

        var apiKind = readData?.ApiKind;
        var apiPropertyName = readData?.ApiPropertyName;
        var apiIdentityName = readData?.ApiIdentityName;
        var clrScalarTypeHint = readData?.ClrScalarTypeHint;

        var apiIdentityPart = default(ApiIdentityPart);
        if (apiKind is not null)
        {
            switch (apiKind.Value)
            {
                case ApiIdentityPartKind.Scalar:
                    apiIdentityPart = new ApiScalarIdentityPart(apiPropertyName!, clrScalarTypeHint);
                    break;

                case ApiIdentityPartKind.Nested:
                    apiIdentityPart = new ApiNestedIdentityPart(apiPropertyName!, apiIdentityName);
                    break;

                case ApiIdentityPartKind.Parent:
                    apiIdentityPart = new ApiParentIdentityPart(apiIdentityName);
                    break;

                default:
                    readContext.Logger.LogError("Unsupported {Kind} enumeration value: '{KindValue}'", nameof(ApiIdentityPartKind), apiKind);
                    break;
            }
        }

        if (apiIdentityPart is null)
        {
            return null;
        }

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiIdentityPart, extensions);

        return apiIdentityPart;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiIdentityPart value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentityPartApiKind(writer, value, writeContext);
            WriteApiIdentityPartApiPropertyName(writer, value, writeContext);
            WriteApiIdentityPartApiIdentityName(writer, value, writeContext);
            WriteApiIdentityPartClrScalarTypeHint(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiIdentityPartApiKind(Utf8JsonWriter writer, ApiIdentityPart apiIdentityPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityPart.ApiKind;
        var kind = apiIdentityPart.ApiKind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, kind, options, _apiIdentityPartKindJsonConverter);
    }

    private static void WriteApiIdentityPartApiPropertyName(Utf8JsonWriter writer, ApiIdentityPart apiIdentityPart, DefaultWriteContext<PropertyNames> context)
    {
        var apiKind = apiIdentityPart.ApiKind;

        var value = default(string);
        if (apiKind == ApiIdentityPartKind.Scalar || apiKind == ApiIdentityPartKind.Nested)
        {
            var apiPropertyIdentityPart = (ApiPropertyIdentityPart)apiIdentityPart;
            value = apiPropertyIdentityPart.ApiPropertyName;
        }

        var propertyName = context.PropertyNames.ApiIdentityPart.ApiPropertyName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdentityPartApiIdentityName(Utf8JsonWriter writer, ApiIdentityPart apiIdentityPart, DefaultWriteContext<PropertyNames> context)
    {
        var apiKind = apiIdentityPart.ApiKind;

        var value = default(string);
        if (apiKind == ApiIdentityPartKind.Nested)
        {
            var apiNestedIdentityPart = (ApiNestedIdentityPart)apiIdentityPart;
            value = apiNestedIdentityPart.ApiIdentityName;
        }
        else if (apiKind == ApiIdentityPartKind.Parent)
        {
            var apiParentIdentityPart = (ApiParentIdentityPart)apiIdentityPart;
            value = apiParentIdentityPart.ApiIdentityName;
        }

        var propertyName = context.PropertyNames.ApiIdentityPart.ApiIdentityName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdentityPartClrScalarTypeHint(Utf8JsonWriter writer, ApiIdentityPart apiIdentityPart, DefaultWriteContext<PropertyNames> context)
    {
        var apiKind = apiIdentityPart.ApiKind;

        var value = default(Type);
        if (apiKind == ApiIdentityPartKind.Scalar)
        {
            var apiScalarIdentityPart = (ApiScalarIdentityPart)apiIdentityPart;
            value = apiScalarIdentityPart.ClrScalarTypeHint;
        }

        var propertyName = context.PropertyNames.ApiIdentityPart.ClrScalarTypeHint;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _typeJsonConverter);
    }
    #endregion
}
