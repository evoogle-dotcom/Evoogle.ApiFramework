// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Identity.Json;

/// <summary>
///     Handles JSON serialization for <see cref="ApiIdentityPartValue"/> instances.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public sealed class ApiIdentityPartValueJsonConverter(ILogger<ApiIdentityPartValueJsonConverter>? logger)
    : JsonConverterBase<ApiIdentityPartValue>(logger)
{
    #region Property Types
    /// <summary>
    ///     JSON converter for serializing and deserializing <see cref="ApiIdentityPartValue"/> instances.
    /// </summary>
    private readonly record struct ApiIdentityPartValuePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiKind { get; init; }
        public required string ApiObjectValue { get; init; }
        public required string ApiStructure { get; init; }
        public required string ApiScalarValue { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdentityPartValuePropertyNames ApiIdentityPartValue { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentityPartValue = new ApiIdentityPartValuePropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Identity.ApiIdentityPartValue.ApiName)),
                    ApiKind = policy.ConvertName(nameof(Identity.ApiIdentityPartValue.ApiKind)),
                    ApiObjectValue = policy.ConvertName(nameof(ApiObjectIdentityPartValue.ApiObjectValue)),
                    ApiStructure = policy.ConvertName(nameof(ApiObjectIdentityPartValue.ApiStructure)),
                    ApiScalarValue = policy.ConvertName(nameof(ApiScalarIdentityPartValue.ApiScalarValue)),
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiIdentityPartValueReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiIdentityPartValueKind? ApiKind { get; set; }
        public ApiIdentityValue? ApiObjectValue { get; set; }
        public List<ApiIdentityPartValue>? ApiStructure { get; set; }
        public ApiId? ApiScalarValue { get; set; }
        #endregion
    }

    private class ReadData
    {
        #region Properties
        public ApiIdentityPartValueReadData? ApiIdentityPartValue { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> SnapshotPropertyHandlers = new()
        {
            { propertyNames.ApiIdentityPartValue.ApiName, HandleApiIdentityPartValueApiName },
            { propertyNames.ApiIdentityPartValue.ApiKind, HandleApiIdentityPartValueApiKind },
            { propertyNames.ApiIdentityPartValue.ApiObjectValue, HandleApiIdentityPartValueApiObjectValue },
            { propertyNames.ApiIdentityPartValue.ApiStructure, HandleApiIdentityPartValueApiStructure },
            { propertyNames.ApiIdentityPartValue.ApiScalarValue, HandleApiIdentityPartValueApiScalarValue }
        };
        #endregion

        #region Methods
        private static void HandleApiIdentityPartValueApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPartValue ??= new ApiIdentityPartValueReadData();

            context.ReadData.ApiIdentityPartValue.ApiName = reader.GetString();
        }

        private static void HandleApiIdentityPartValueApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPartValue ??= new ApiIdentityPartValueReadData();

            var options = context.Options;
            context.ReadData.ApiIdentityPartValue.ApiKind = _apiIdentityPartValueKindJsonConverter.Read(ref reader, typeof(ApiIdentityPartValueKind), options);
        }

        private static void HandleApiIdentityPartValueApiObjectValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPartValue ??= new ApiIdentityPartValueReadData();

            var options = context.Options;
            context.ReadData.ApiIdentityPartValue.ApiObjectValue = JsonSerializer.Deserialize<ApiIdentityValue>(ref reader, options);
        }

        private static void HandleApiIdentityPartValueApiStructure(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPartValue ??= new ApiIdentityPartValueReadData();
            context.ReadData.ApiIdentityPartValue.ApiStructure ??= [];

            ReadJsonArray(ref reader, context, (x) => HandleApiIdentityPartValueApiStructureArrayItem);
        }

        private static void HandleApiIdentityPartValueApiStructureArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var apiIdentityPartValue = JsonSerializer.Deserialize<ApiIdentityPartValue>(ref reader, context.Options);
            if (apiIdentityPartValue == null)
            {
                return;
            }

            context.ReadData.ApiIdentityPartValue!.ApiStructure!.Add(apiIdentityPartValue);
        }

        private static void HandleApiIdentityPartValueApiScalarValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPartValue ??= new ApiIdentityPartValueReadData();

            var options = context.Options;
            context.ReadData.ApiIdentityPartValue.ApiScalarValue = JsonSerializer.Deserialize<ApiId>(ref reader, options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiIdentityPartValueKind> _apiIdentityPartValueKindJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdentityPartValueJsonConverter()
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
    protected override ApiIdentityPartValue? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiIdentityPartValue;

        var apiName = readData?.ApiName;
        var apiKind = readData?.ApiKind;
        var apiObjectValue = readData?.ApiObjectValue;
        var apiStructure = readData?.ApiStructure;
        var apiScalarValue = readData?.ApiScalarValue.GetValueOrDefault() ?? default;

        var apiIdentityPartValue = default(ApiIdentityPartValue);
        if (apiKind != null)
        {
            switch (apiKind.Value)
            {
                case ApiIdentityPartValueKind.Scalar:
                    apiIdentityPartValue = new ApiScalarIdentityPartValue(apiName!, apiScalarValue);
                    break;

                case ApiIdentityPartValueKind.Object:
                    apiIdentityPartValue = new ApiObjectIdentityPartValue(apiName!, apiObjectValue, apiStructure?.ToArray());
                    break;

                default:
                    readContext.Logger.LogError("Unsupported {ApiKind} enumeration value: '{ApiKindValue}'", nameof(ApiIdentityPartValueKind), apiKind);
                    break;
            }
        }

        return apiIdentityPartValue;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.SnapshotPropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiIdentityPartValue value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentityPartValueApiName(writer, value, writeContext);
            WriteApiIdentityPartValueApiKind(writer, value, writeContext);
            WriteApiIdentityPartValueApiObjectValue(writer, value, writeContext);
            WriteApiIdentityPartValueApiStructure(writer, value, writeContext);
            WriteApiIdentityPartValueApiScalarValue(writer, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiIdentityPartValueApiName(Utf8JsonWriter writer, ApiIdentityPartValue apiIdentityPartValue, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityPartValue.ApiName;
        var value = apiIdentityPartValue.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdentityPartValueApiKind(Utf8JsonWriter writer, ApiIdentityPartValue apiIdentityPartValue, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityPartValue.ApiKind;
        var value = apiIdentityPartValue.ApiKind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _apiIdentityPartValueKindJsonConverter);
    }

    private static void WriteApiIdentityPartValueApiObjectValue(Utf8JsonWriter writer, ApiIdentityPartValue apiIdentityPartValue, DefaultWriteContext<PropertyNames> context)
    {
        var apiKind = apiIdentityPartValue.ApiKind;

        var value = default(ApiIdentityValue?);
        if (apiKind == ApiIdentityPartValueKind.Object)
        {
            var apiObjectIdentityPartValue = (ApiObjectIdentityPartValue)apiIdentityPartValue;
            value = apiObjectIdentityPartValue.ApiObjectValue;
        }

        var propertyName = context.PropertyNames.ApiIdentityPartValue.ApiObjectValue;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, value, options);
    }

    private static void WriteApiIdentityPartValueApiStructure(Utf8JsonWriter writer, ApiIdentityPartValue apiIdentityPartValue, DefaultWriteContext<PropertyNames> context)
    {
        var apiKind = apiIdentityPartValue.ApiKind;

        var value = default(ApiIdentityPartValue[]?);
        if (apiKind == ApiIdentityPartValueKind.Object)
        {
            var apiObjectIdentityPartValue = (ApiObjectIdentityPartValue)apiIdentityPartValue;
            value = apiObjectIdentityPartValue.ApiStructure;
        }

        var propertyName = context.PropertyNames.ApiIdentityPartValue.ApiStructure;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            value,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }

    private static void WriteApiIdentityPartValueApiScalarValue(Utf8JsonWriter writer, ApiIdentityPartValue apiIdentityPartValue, DefaultWriteContext<PropertyNames> context)
    {
        var apiKind = apiIdentityPartValue.ApiKind;

        var value = default(ApiId?);
        if (apiKind == ApiIdentityPartValueKind.Scalar)
        {
            var apiScalarIdentityPartValue = (ApiScalarIdentityPartValue)apiIdentityPartValue;
            value = apiScalarIdentityPartValue.ApiScalarValue;
        }

        var propertyName = context.PropertyNames.ApiIdentityPartValue.ApiScalarValue;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, value, options);
    }
    #endregion
}
