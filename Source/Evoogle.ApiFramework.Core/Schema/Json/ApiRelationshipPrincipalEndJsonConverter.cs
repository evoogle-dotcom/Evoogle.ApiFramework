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
///     Handles JSON serialization for <see cref="ApiRelationshipPrincipalEnd"/> instances.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipPrincipalEndJsonConverter(ILogger<ApiRelationshipPrincipalEndJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipPrincipalEnd>(logger)
{
    #region Property Types
    private readonly record struct ApiRelationshipEndPropertyNames
    {
        public required string ClrObjectType { get; init; }
    }

    private readonly record struct ApiRelationshipPrincipalEndPropertyNames
    {
        public required string ApiIdentityName { get; init; }
    }

    private readonly record struct PropertyNames
    {
        public required ApiRelationshipEndPropertyNames ApiRelationshipEnd { get; init; }
        public required ApiRelationshipPrincipalEndPropertyNames ApiRelationshipPrincipalEnd { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationshipEnd = new ApiRelationshipEndPropertyNames
                {
                    ClrObjectType = policy.ConvertName(nameof(ApiRelationshipEnd.ClrObjectType)),
                },
                ApiRelationshipPrincipalEnd = new ApiRelationshipPrincipalEndPropertyNames
                {
                    ApiIdentityName = policy.ConvertName(nameof(ApiRelationshipPrincipalEnd.ApiIdentityName)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
    }
    #endregion

    #region Read Types
    private class ApiRelationshipEndReadData
    {
        public Type? ClrObjectType { get; set; }
    }

    private class ApiRelationshipPrincipalEndReadData
    {
        public string? ApiIdentityName { get; set; }
    }

    private class ReadData : ExtensibleReadData
    {
        public ApiRelationshipEndReadData? ApiRelationshipEnd { get; set; }
        public ApiRelationshipPrincipalEndReadData? ApiRelationshipPrincipalEnd { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipEnd.ClrObjectType, HandleClrObjectType },
            { propertyNames.ApiRelationshipPrincipalEnd.ApiIdentityName, HandleApiIdentityName },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };

        private static void HandleClrObjectType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipEnd ??= new ApiRelationshipEndReadData();
            context.ReadData.ApiRelationshipEnd.ClrObjectType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }

        private static void HandleApiIdentityName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipPrincipalEnd ??= new ApiRelationshipPrincipalEndReadData();
            context.ReadData.ApiRelationshipPrincipalEnd.ApiIdentityName = reader.GetString();
        }
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipPrincipalEndJsonConverter()
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
        => CreateDefaultWriteContext(logger, options, buildPropertyNames: PropertyNames.Create);

    /// <inheritdoc/>
    protected override ApiRelationshipPrincipalEnd? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;

        var clrObjectType = readContext.ReadData.ApiRelationshipEnd?.ClrObjectType;
        var apiIdentityName = readContext.ReadData.ApiRelationshipPrincipalEnd?.ApiIdentityName;

        var end = new ApiRelationshipPrincipalEnd
            (
                clrObjectType!,
                apiIdentityName
            );

        AttachExtensions(end, readContext.ReadData.Extensions);
        return end;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteClrObjectType(writer, value, writeContext);
            WriteApiIdentityName(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteClrObjectType(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationshipEnd.ClrObjectType, end.ClrObjectType, context.Options, _typeJsonConverter);

    private static void WriteApiIdentityName(Utf8JsonWriter writer, ApiRelationshipPrincipalEnd end, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipPrincipalEnd.ApiIdentityName;
        var value = end.ApiIdentityName;

        writer.TryWritePropertyAsString(propertyName, value, context.Options);
    }
    #endregion
}
