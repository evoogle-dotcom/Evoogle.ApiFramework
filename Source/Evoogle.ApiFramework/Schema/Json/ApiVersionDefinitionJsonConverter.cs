// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.ApiFramework.Schema.Version;
using Evoogle.Json;
using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>Handles JSON serialization for <see cref="ApiVersionDefinition"/> instances.</summary>
/// <param name="logger">The optional logger used to emit JSON diagnostics.</param>
public class ApiVersionDefinitionJsonConverter(ILogger<ApiVersionDefinitionJsonConverter>? logger)
    : JsonConverterBase<ApiVersionDefinition>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiVersionDefinition"/> under a given naming policy.
    /// </summary>
    private readonly record struct ApiVersionDefinitionPropertyNames
    {
        #region Immutable Properties
        public required string ApiPropertyReference { get; init; }
        public required string ClrType { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiVersionDefinitionPropertyNames ApiVersionDefinition { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiVersionDefinition = new ApiVersionDefinitionPropertyNames
                {
                    ApiPropertyReference = policy.ConvertName(nameof(Schema.Version.ApiVersionDefinition.ApiProperty)), // Mapping property name from ApiPropertyReference to ApiProperty by design
                    ClrType = policy.ConvertName(nameof(Schema.Version.ApiVersionDefinition.ClrType)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading version definition members from JSON.
    /// </summary>
    private class ApiVersionDefinitionReadData
    {
        #region Properties
        public bool HasApiPropertyReference { get; set; }
        public bool HasClrType { get; set; }
        public ApiPropertyReference? ApiPropertyReference { get; set; }
        public Type? ClrType { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiVersionDefinition"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiVersionDefinitionReadData? ApiVersionDefinition { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiVersionDefinition Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiVersionDefinition Property Handlers
            { propertyNames.ApiVersionDefinition.ApiPropertyReference, HandleApiVersionDefinitionApiPropertyReference },
            { propertyNames.ApiVersionDefinition.ClrType, HandleApiVersionDefinitionClrType },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiVersionDefinition Methods
        private static void HandleApiVersionDefinitionApiPropertyReference(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiVersionDefinition ??= new ApiVersionDefinitionReadData();

            var apiVersionDefinitionReadData = context.ReadData.ApiVersionDefinition;
            apiVersionDefinitionReadData.HasApiPropertyReference = true;
            apiVersionDefinitionReadData.ApiPropertyReference = JsonSerializer.Deserialize<ApiPropertyReference>(ref reader, context.Options);
        }

        private static void HandleApiVersionDefinitionClrType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiVersionDefinition ??= new ApiVersionDefinitionReadData();

            var apiVersionDefinitionReadData = context.ReadData.ApiVersionDefinition;
            apiVersionDefinitionReadData.HasClrType = true;
            apiVersionDefinitionReadData.ClrType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiVersionDefinitionJsonConverter()
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
    protected override ApiVersionDefinition? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var apiVersionDefinitionReadData = readContext.ReadData.ApiVersionDefinition ?? new ApiVersionDefinitionReadData();
        if (apiVersionDefinitionReadData.HasClrType == apiVersionDefinitionReadData.HasApiPropertyReference)
        {
            throw new JsonException($"An {nameof(ApiVersionDefinition)} must contain exactly one "
                + $"of {nameof(ApiVersionDefinition.ClrType)} or "
                + $"{nameof(ApiVersionDefinition.ApiPropertyReference)}.");
        }

        if (apiVersionDefinitionReadData.HasApiPropertyReference && apiVersionDefinitionReadData.ApiPropertyReference is null)
        {
            throw new JsonException
            (
                $"{nameof(ApiVersionDefinition.ApiPropertyReference)} must not be null."
            );
        }

        var apiVersionDefinition = apiVersionDefinitionReadData.HasApiPropertyReference
            ? new ApiVersionDefinition(apiVersionDefinitionReadData.ApiPropertyReference!)
            : new ApiVersionDefinition(apiVersionDefinitionReadData.ClrType!);

        AttachExtensions(apiVersionDefinition, readContext.ReadData.Extensions);
        return apiVersionDefinition;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject
        (
            ref reader,
            readContext,
            handlers
        );
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiVersionDefinition apiVersionDefinition, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiVersionDefinition, writeContext), writeObject: static (writer, state) =>
        {
            var (apiVersionDefinition, writeContext) = state;
            WriteApiVersionDefinitionApiPropertyReference(writer, apiVersionDefinition, writeContext);
            WriteApiVersionDefinitionClrType(writer, apiVersionDefinition, writeContext);

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
    private static void WriteApiVersionDefinitionApiPropertyReference(Utf8JsonWriter writer, ApiVersionDefinition apiVersionDefinition, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiVersionDefinition.ApiPropertyReference, obj: apiVersionDefinition.ApiPropertyReference, options: writeContext.Options);

    private static void WriteApiVersionDefinitionClrType(Utf8JsonWriter writer, ApiVersionDefinition apiVersionDefinition, DefaultWriteContext<PropertyNames> writeContext)
    {
        if (!apiVersionDefinition.IsPropertyBacked)
        {
            writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiVersionDefinition.ClrType, type: apiVersionDefinition.ClrRepositoryType, options: writeContext.Options, converter: _typeJsonConverter);
        }
    }
    #endregion
}
