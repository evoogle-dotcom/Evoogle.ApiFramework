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
///     Handles JSON serialization for <see cref="ApiRelationshipDependentEnd"/> instances.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipDependentEndJsonConverter(ILogger<ApiRelationshipDependentEndJsonConverter>? logger)
    : JsonConverterBase<ApiRelationshipDependentEnd>(logger)
{
    #region Property Types
    private readonly record struct ApiRelationshipElementPropertyNames
    {
        public required string ClrObjectType { get; init; }
    }

    private readonly record struct ApiRelationshipDependentEndPropertyNames
    {
        public required string ApiForeignKeyType { get; init; }
    }

    private readonly record struct PropertyNames
    {
        public required ApiRelationshipElementPropertyNames ApiRelationshipElement { get; init; }
        public required ApiRelationshipDependentEndPropertyNames ApiRelationshipDependentEnd { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationshipElement = new ApiRelationshipElementPropertyNames
                {
                    ClrObjectType = policy.ConvertName(nameof(ApiRelationshipElement.ClrObjectType)),
                },
                ApiRelationshipDependentEnd = new ApiRelationshipDependentEndPropertyNames
                {
                    ApiForeignKeyType = policy.ConvertName(nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
    }
    #endregion

    #region Read Types
    private class ApiRelationshipElementReadData
    {
        public Type? ClrObjectType { get; set; }
    }

    private class ApiRelationshipDependentEndReadData
    {
        public ApiKeyType? ApiForeignKeyType { get; set; }
    }

    private class ReadState : ExtensibleReadData
    {
        public ApiRelationshipElementReadData? ApiRelationshipElement { get; set; }
        public ApiRelationshipDependentEndReadData? ApiRelationshipDependentEnd { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationshipElement.ClrObjectType, HandleClrObjectType },
            { propertyNames.ApiRelationshipDependentEnd.ApiForeignKeyType, HandleApiForeignKeyType },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() },
        };

        private static void HandleClrObjectType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipElement ??= new ApiRelationshipElementReadData();
            context.ReadData.ApiRelationshipElement.ClrObjectType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }

        private static void HandleApiForeignKeyType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipDependentEnd ??= new ApiRelationshipDependentEndReadData();
            context.ReadData.ApiRelationshipDependentEnd.ApiForeignKeyType = JsonSerializer.Deserialize<ApiKeyType>(ref reader, context.Options);
        }
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipDependentEndJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    /// <inheritdoc/>
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultReadContext<PropertyNames, ReadState, ReadHandlers>
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
    protected override ApiRelationshipDependentEnd? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;

        var clrObjectType = readContext.ReadData.ApiRelationshipElement?.ClrObjectType;
        var apiForeignKeyType = readContext.ReadData.ApiRelationshipDependentEnd?.ApiForeignKeyType;

        var end = apiForeignKeyType != null
            ? new ApiRelationshipDependentEnd(clrObjectType!, apiForeignKeyType)
            : new ApiRelationshipDependentEnd(clrObjectType!);

        AttachExtensions(end, readContext.ReadData.Extensions);
        return end;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationshipDependentEnd value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteClrObjectType(writer, value, writeContext);
            WriteApiForeignKeyType(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteClrObjectType(Utf8JsonWriter writer, ApiRelationshipDependentEnd end, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationshipElement.ClrObjectType, end.ClrObjectType, context.Options, _typeJsonConverter);

    private static void WriteApiForeignKeyType(Utf8JsonWriter writer, ApiRelationshipDependentEnd end, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipDependentEnd.ApiForeignKeyType;
        var value = end.HasForeignKey ? end.ApiForeignKeyType : null;

        writer.WritePropertyName(propertyName);
        writer.TryWriteWithSerializer(value, context.Options);
    }
    #endregion
}
