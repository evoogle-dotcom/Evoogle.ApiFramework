// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Versions;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>Handles JSON serialization for <see cref="ApiVersionType"/> instances.</summary>
/// <param name="logger">The optional logger used to emit JSON diagnostics.</param>
public class ApiVersionTypeJsonConverter(ILogger<ApiVersionTypeJsonConverter>? logger)
    : JsonConverterBase<ApiVersionType>(logger)
{
    #region Property Types
    private readonly record struct PropertyNames
    {
        public required string ClrMemberName { get; init; }
        public required string ClrType { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ClrMemberName = policy.ConvertName(nameof(ApiVersionType.ClrMemberName)),
                ClrType = policy.ConvertName(nameof(ApiVersionType.ClrType)),
                ExtensibleBase = GetExtensiblePropertyNames(policy)
            };
    }
    #endregion

    #region Read Types
    private sealed class ReadState : ExtensibleReadData
    {
        public string? ClrMemberName { get; set; }
        public Type? ClrType { get; set; }
    }

    private sealed class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary
        <
            string,
            JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>
        > PropertyHandlers = new()
        {
            { propertyNames.ClrMemberName, HandleClrMemberName },
            { propertyNames.ClrType, HandleClrType },
            {
                propertyNames.ExtensibleBase.Extensions,
                CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>()
            }
        };

        private static void HandleClrMemberName
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        ) => context.ReadData.ClrMemberName = reader.GetString();

        private static void HandleClrType
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        ) => context.ReadData.ClrType = _typeJsonConverter.Read
        (
            ref reader,
            typeof(Type),
            context.Options
        );
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use by the JSON converter attribute.</summary>
    public ApiVersionTypeJsonConverter()
        : this(null)
    { }
    #endregion

    #region JsonConverterBase Methods
    /// <inheritdoc/>
    protected override IReadContext CreateReadContext
    (
        ILogger logger,
        JsonSerializerOptions options
    )
        => CreateDefaultReadContext<PropertyNames, ReadState, ReadHandlers>
        (
            logger,
            options,
            buildPropertyNames: PropertyNames.Create,
            buildReadHandlers: names => new ReadHandlers(names)
        );

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext
    (
        ILogger logger,
        JsonSerializerOptions options
    ) => CreateDefaultWriteContext(logger, options, buildPropertyNames: PropertyNames.Create);

    /// <inheritdoc/>
    protected override ApiVersionType? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var value = new ApiVersionType
        (
            readContext.ReadData.ClrType!,
            readContext.ReadData.ClrMemberName
        );

        AttachExtensions(value, readContext.ReadData.Extensions);
        return value;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore
    (
        Utf8JsonWriter writer,
        ApiVersionType value,
        IWriteContext context
    )
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;
        WriteJsonObject(writer, () =>
        {
            writer.TryWritePropertyWithConverter
            (
                writeContext.PropertyNames.ClrType,
                value.ClrType,
                writeContext.Options,
                _typeJsonConverter
            );
            writer.TryWritePropertyAsString
            (
                writeContext.PropertyNames.ClrMemberName,
                value.ClrMemberName,
                writeContext.Options
            );
            WriteExtensibleBaseExtensions
            (
                writer,
                writeContext.PropertyNames.ExtensibleBase.Extensions,
                value,
                writeContext
            );
        });
    }
    #endregion
}
