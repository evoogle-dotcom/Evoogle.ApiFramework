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
///     A JSON converter for <see cref="ApiType"/> that handles serialization and deserialization logic.
///     This converter supports reading and writing various derived types of <see cref="ApiType"/> including scalar, enum, object, and collection types, as well as handling custom property naming policies and extensions.
/// </summary>
/// <remarks>
///     Optional constructor with logger for use in DI contexts.
/// </remarks>
/// <param name="logger">The optional logger instance.</param>
public partial class ApiTypeJsonConverter(ILogger<ApiTypeJsonConverter>? logger) : JsonConverterBase<ApiType>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the JSON member names for <see cref="ApiCollectionType"/> properties under the active naming policy.
    /// </summary>
    private readonly record struct ApiCollectionTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiItemTypeExpression { get; init; }
        public required string ApiItemTypeModifiers { get; init; }
        #endregion
    }

    /// <summary>
    ///     Stores the JSON member names for <see cref="ApiEnumType"/> properties under the active naming policy.
    /// </summary>
    private readonly record struct ApiEnumTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiEnumValues { get; init; }
        #endregion
    }

    /// <summary>
    ///     Stores the JSON member names for <see cref="ApiNamedType"/> properties under the active naming policy.
    /// </summary>
    private readonly record struct ApiNamedTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        #endregion
    }

    /// <summary>
    ///     Stores the JSON member names for <see cref="ApiObjectType"/> properties under the active naming policy.
    /// </summary>
    private readonly record struct ApiObjectTypePropertyNames
    {
        #region Immutable Properties
        public required string ApiOptions { get; init; }
        public required string ApiProperties { get; init; }
        public required string ApiKeyTypes { get; init; }
        #endregion
    }

    /// <summary>
    ///     Stores the JSON member names for base <see cref="ApiType"/> properties under the active naming policy.
    /// </summary>
    private readonly record struct ApiTypePropertyNames
    {
        #region Immutable Properties
        public required string ClrType { get; init; }
        public required string ApiKind { get; init; }
        #endregion
    }

    /// <summary>
    ///     Aggregates all property name groups used by the converter for a given naming policy.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiCollectionTypePropertyNames ApiCollectionType { get; init; }
        public required ApiEnumTypePropertyNames ApiEnumType { get; init; }
        public required ApiNamedTypePropertyNames ApiNamedType { get; init; }
        public required ApiObjectTypePropertyNames ApiObjectType { get; init; }
        public required ApiTypePropertyNames ApiType { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiCollectionType = new ApiCollectionTypePropertyNames
                {
                    ApiItemTypeExpression = policy.ConvertName(nameof(Schema.ApiCollectionType.ApiItemType)), // Mapping property name from ApiItemTypeExpression to ApiItemType by design
                    ApiItemTypeModifiers = policy.ConvertName(nameof(Schema.ApiCollectionType.ApiItemTypeModifiers))
                },
                ApiEnumType = new ApiEnumTypePropertyNames
                {
                    ApiEnumValues = policy.ConvertName(nameof(Schema.ApiEnumType.ApiEnumValues))
                },
                ApiNamedType = new ApiNamedTypePropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Schema.ApiNamedType.ApiName))
                },
                ApiObjectType = new ApiObjectTypePropertyNames
                {
                    ApiOptions = policy.ConvertName(nameof(Schema.ApiObjectType.ApiOptions)),
                    ApiProperties = policy.ConvertName(nameof(Schema.ApiObjectType.ApiProperties)),
                    ApiKeyTypes = policy.ConvertName(nameof(Schema.ApiObjectType.ApiKeyTypes))
                },
                ApiType = new ApiTypePropertyNames
                {
                    ClrType = policy.ConvertName(nameof(Schema.ApiType.ClrType)),
                    ApiKind = policy.ConvertName(nameof(Schema.ApiType.ApiKind))
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeKind> _apiTypeKindJsonConverter = new();
    private static readonly EnumJsonConverter<ApiTypeModifiers> _apiTypeModifiersJsonConverter = new();

    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiTypeJsonConverter()
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
    protected override ApiType? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;

        var apiType = default(ApiType);

        var apiKind = readContext.ReadData.ApiType?.ApiKind;
        if (apiKind is not null)
        {
            switch (apiKind)
            {
                case ApiTypeKind.Collection:
                    apiType = CreateApiCollectionType(readContext);
                    break;

                case ApiTypeKind.Enum:
                    apiType = CreateApiEnumType(readContext);
                    break;

                case ApiTypeKind.Object:
                    apiType = CreateApiObjectType(readContext);
                    break;

                case ApiTypeKind.Scalar:
                    apiType = CreateApiScalarType(readContext);
                    break;

                default:
                    readContext.Logger.LogError("Unsupported {Kind} enumeration value: '{KindValue}'", nameof(ApiTypeKind), apiKind);
                    break;
            }
        }
        else
        {
            readContext.Logger.LogError("Missing {Kind} enumeration value", nameof(ApiType.ApiKind));
        }

        if (apiType is null)
        {
            return null;
        }

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiType, extensions);

        return apiType;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiType value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            // Prolog
            WriteApiTypeApiKind(writer, value, writeContext);

            // Body
            var apiKind = value.ApiKind;
            switch (apiKind)
            {
                case ApiTypeKind.Collection:
                    {
                        var apiCollectionType = (ApiCollectionType)value;
                        WriteApiCollectionType(writer, apiCollectionType, writeContext);
                        break;
                    }

                case ApiTypeKind.Enum:
                    {
                        var apiEnumType = (ApiEnumType)value;
                        WriteApiNamedType(writer, apiEnumType, writeContext);
                        WriteApiEnumType(writer, apiEnumType, writeContext);
                        break;
                    }

                case ApiTypeKind.Object:
                    {
                        var apiObjectType = (ApiObjectType)value;
                        WriteApiNamedType(writer, apiObjectType, writeContext);
                        WriteApiObjectType(writer, apiObjectType, writeContext);
                        break;
                    }

                case ApiTypeKind.Scalar:
                    {
                        var apiScalarType = (ApiScalarType)value;
                        WriteApiNamedType(writer, apiScalarType, writeContext);
                        WriteApiScalarType(writer, apiScalarType, writeContext);
                        break;
                    }

                default:
                    {
                        throw new JsonException($"Unsupported Kind: {apiKind}");
                    }
            }

            // Epilog
            WriteApiTypeClrType(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion
}
