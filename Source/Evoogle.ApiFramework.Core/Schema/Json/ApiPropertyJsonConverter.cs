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
///     Serializes and deserializes <see cref="ApiProperty"/> instances, including extension payloads and type expressions.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiPropertyJsonConverter(ILogger<ApiPropertyJsonConverter>? logger) : JsonConverterBase<ApiProperty>(logger)
{
    #region Property Types
    /// <summary>
    ///     Stores the resolved JSON property names for an <see cref="ApiProperty"/> under a given naming policy.
    /// </summary>
    private readonly record struct ApiPropertyPropertyNames
    {
        #region Immutable Properties
        public required string ApiName { get; init; }
        public required string ApiTypeExpression { get; init; }
        public required string ApiTypeModifiers { get; init; }
        public required string ClrName { get; init; }
        public required string ClrMemberKind { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiPropertyPropertyNames ApiProperty { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiProperty = new ApiPropertyPropertyNames
                {
                    ApiName = policy.ConvertName(nameof(Schema.ApiProperty.ApiName)),
                    ApiTypeExpression = policy.ConvertName(nameof(Schema.ApiProperty.ApiType)), // Mapping property name from ApiTypeExpression to ApiType by design
                    ApiTypeModifiers = policy.ConvertName(nameof(Schema.ApiProperty.ApiTypeModifiers)),
                    ClrName = policy.ConvertName(nameof(Schema.ApiProperty.ClrName)),
                    ClrMemberKind = policy.ConvertName(nameof(Schema.ApiProperty.ClrMemberKind)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading property members from JSON.
    /// </summary>
    private class ApiPropertyReadData
    {
        #region Properties
        public string? ApiName { get; set; }
        public ApiTypeExpression? ApiTypeExpression { get; set; }
        public ApiTypeModifiers? ApiTypeModifiers { get; set; }
        public string? ClrName { get; set; }
        public ClrMemberKind? ClrMemberKind { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiProperty"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiPropertyReadData ApiProperty { get; } = new();
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Constants
        private static readonly Type _apiTypeModifiersType = typeof(ApiTypeModifiers);
        private static readonly Type _clrMemberKindType = typeof(ClrMemberKind);
        #endregion

        #region ApiProperty Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiProperty Property Handlers
            { propertyNames.ApiProperty.ApiName, HandleApiPropertyApiName },
            { propertyNames.ApiProperty.ApiTypeModifiers, HandleApiPropertyApiTypeModifiers },
            { propertyNames.ApiProperty.ApiTypeExpression, HandleApiPropertyApiTypeExpression },
            { propertyNames.ApiProperty.ClrName, HandleApiPropertyClrName },
            { propertyNames.ApiProperty.ClrMemberKind, HandleApiPropertyClrMemberKind },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiProperty Methods
        private static void HandleApiPropertyApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiProperty.ApiName = reader.GetString();
        }

        private static void HandleApiPropertyApiTypeModifiers(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var options = context.Options;
            context.ReadData.ApiProperty.ApiTypeModifiers = _apiTypeModifiersJsonConverter.Read(ref reader, _apiTypeModifiersType, options);
        }

        private static void HandleApiPropertyApiTypeExpression(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiProperty.ApiTypeExpression = JsonSerializer.Deserialize<ApiTypeExpression>(ref reader, context.Options);
        }

        private static void HandleApiPropertyClrName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiProperty.ClrName = reader.GetString();
        }

        private static void HandleApiPropertyClrMemberKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var options = context.Options;
            context.ReadData.ApiProperty.ClrMemberKind = _clrMemberKindJsonConverter.Read(ref reader, _clrMemberKindType, options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiTypeModifiers> _apiTypeModifiersJsonConverter = new();
    private static readonly EnumJsonConverter<ClrMemberKind> _clrMemberKindJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiPropertyJsonConverter()
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

    protected override ApiProperty? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiProperty;

        var apiName = readData.ApiName;
        var apiTypeExpression = readData.ApiTypeExpression;
        var apiTypeModifiers = readData.ApiTypeModifiers ?? ApiTypeModifiers.None;
        var clrName = readData.ClrName;
        var clrMemberKind = readData.ClrMemberKind ?? ClrMemberKind.Unknown;

        var apiProperty = new ApiProperty(apiName!, apiTypeExpression!, apiTypeModifiers, clrName!, clrMemberKind);

        var extensions = readContext.ReadData.Extensions;
        AttachExtensions(apiProperty, extensions);

        return apiProperty;
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiProperty value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiPropertyApiName(writer, value, writeContext);
            WriteApiPropertyApiTypeExpression(writer, value, writeContext);
            WriteApiPropertyApiTypeModifiers(writer, value, writeContext);
            WriteApiPropertyClrName(writer, value, writeContext);
            WriteApiPropertyClrMemberKind(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiPropertyApiName(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiName;
        var value = apiProperty.ApiName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiPropertyApiTypeExpression(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiTypeExpression;
        var apiTypeExpression = apiProperty.ApiTypeExpression;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, apiTypeExpression, options);
    }

    private static void WriteApiPropertyApiTypeModifiers(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ApiTypeModifiers;
        var apiTypeModifiers = apiProperty.ApiTypeModifiers;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, apiTypeModifiers, options, _apiTypeModifiersJsonConverter);
    }

    private static void WriteApiPropertyClrName(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ClrName;
        var value = apiProperty.ClrName;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiPropertyClrMemberKind(Utf8JsonWriter writer, ApiProperty apiProperty, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiProperty.ClrMemberKind;
        var clrMemberKind = apiProperty.ClrMemberKind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, clrMemberKind, options, _clrMemberKindJsonConverter);
    }
    #endregion
}
