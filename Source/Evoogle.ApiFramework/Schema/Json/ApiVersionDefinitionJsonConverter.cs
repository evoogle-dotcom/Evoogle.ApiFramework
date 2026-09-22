// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Version;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>Handles JSON serialization for <see cref="ApiVersionDefinition"/> instances.</summary>
/// <param name="logger">The optional logger used to emit JSON diagnostics.</param>
public class ApiVersionDefinitionJsonConverter(ILogger<ApiVersionDefinitionJsonConverter>? logger)
    : JsonConverterBase<ApiVersionDefinition>(logger)
{
    #region Property Types
    private readonly record struct PropertyNames
    {
        public required string ApiPropertyReference { get; init; }
        public required string ClrType { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiPropertyReference = policy.ConvertName
                (
                    nameof(ApiVersionDefinition.ApiPropertyReference)
                ),
                ClrType = policy.ConvertName(nameof(ApiVersionDefinition.ClrType)),
                ExtensibleBase = GetExtensiblePropertyNames(policy)
            };
    }
    #endregion

    #region Read Types
    private sealed class ReadState : ExtensibleReadData
    {
        public bool HasApiPropertyReference { get; set; }
        public bool HasClrType { get; set; }
        public ApiPropertyReference? ApiPropertyReference { get; set; }
        public Type? ClrType { get; set; }
    }

    private sealed class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            { propertyNames.ApiPropertyReference, HandleApiPropertyReference },
            { propertyNames.ClrType, HandleClrType },
            {
                propertyNames.ExtensibleBase.Extensions,
                CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>()
            }
        };

        private static void HandleApiPropertyReference
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        )
        {
            context.ReadData.HasApiPropertyReference = true;
            context.ReadData.ApiPropertyReference =
                JsonSerializer.Deserialize<ApiPropertyReference>(ref reader, context.Options);
        }

        private static void HandleClrType
        (
            ref Utf8JsonReader reader,
            DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context
        )
        {
            context.ReadData.HasClrType = true;
            context.ReadData.ClrType = _typeJsonConverter.Read
            (
                ref reader,
                typeof(Type),
                context.Options
            );
        }
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use by the JSON converter attribute.</summary>
    public ApiVersionDefinitionJsonConverter()
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
    protected override ApiVersionDefinition? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readData = readContext.ReadData;
        if (readData.HasClrType == readData.HasApiPropertyReference)
        {
            throw new JsonException($"An {nameof(ApiVersionDefinition)} must contain exactly one "
                + $"of {nameof(ApiVersionDefinition.ClrType)} or "
                + $"{nameof(ApiVersionDefinition.ApiPropertyReference)}.");
        }

        if (readData.HasApiPropertyReference && readData.ApiPropertyReference is null)
        {
            throw new JsonException
            (
                $"{nameof(ApiVersionDefinition.ApiPropertyReference)} must not be null."
            );
        }

        var value = readData.HasApiPropertyReference
            ? new ApiVersionDefinition(readData.ApiPropertyReference!)
            : new ApiVersionDefinition(readData.ClrType!);

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
        ApiVersionDefinition apiVersionDefinition,
        IWriteContext context
    )
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;
        writer.WriteJsonObject(state: (apiVersionDefinition, writeContext), writeObject: static (writer, state) =>
        {
            var (apiVersionDefinition, writeContext) = state;
            WriteApiPropertyReference(writer, apiVersionDefinition, writeContext);
            WriteClrType(writer, apiVersionDefinition, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiVersionDefinition,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiPropertyReference
    (
        Utf8JsonWriter writer,
        ApiVersionDefinition apiVersionDefinition,
        DefaultWriteContext<PropertyNames> writeContext
    ) => writer.TryWritePropertyWithSerializer
    (
        propertyName: writeContext.PropertyNames.ApiPropertyReference,
        obj: apiVersionDefinition.ApiPropertyReference,
        options: writeContext.Options
    );

    private static void WriteClrType(Utf8JsonWriter writer, ApiVersionDefinition apiVersionDefinition, DefaultWriteContext<PropertyNames> writeContext)
    {
        if (!apiVersionDefinition.IsPropertyBacked)
        {
            writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ClrType, type: apiVersionDefinition.ClrRepositoryType, options: writeContext.Options, converter: _typeJsonConverter);
        }
    }
    #endregion
}
