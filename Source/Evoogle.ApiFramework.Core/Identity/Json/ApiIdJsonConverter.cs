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
///     JSON converter for <see cref="ApiId"/> values that supports both scalar and composite
///     representations for read and write using System.Text.Json.
/// </summary>
/// <remarks>
///     This converter integrates with the Evoogle JSON infrastructure and writes/reads an object with
///     two properties: a <c>kind</c> enum and a <c>value</c> that is either a string (scalar) or an array
///     of parts (composite). Null values are written as JSON <c>null</c>.
/// </remarks>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiIdJsonConverter"/> class.
/// </remarks>
/// <param name="logger">The logger used for diagnostics (optional).</param>
public sealed class ApiIdJsonConverter(ILogger<ApiIdJsonConverter>? logger) : JsonConverterBase<ApiId>(logger)
{
    #region Property Types
    /// <summary>
    ///     Provides the JSON property names used when serializing and deserializing an <see cref="ApiId"/>.
    ///     Names are pre-computed using the configured <see cref="JsonNamingPolicy"/>.
    /// </summary>
    private readonly record struct ApiIdPropertyNames
    {
        #region Immutable Properties
        /// <summary>Gets the JSON property name used for the <see cref="ApiId.Kind"/> value.</summary>
        public required string Kind { get; init; }

        /// <summary>Gets the JSON property name used for the <see cref="ApiId.Value"/> value or the composite parts.</summary>
        public required string Value { get; init; }
        #endregion
    }

    /// <summary>
    ///     Provides the JSON property names used for a composite <see cref="ApiId"/> part.
    /// </summary>
    private readonly record struct ApiIdPartPropertyNames
    {
        #region Immutable Properties
        /// <summary>Gets the JSON property name used for the <see cref="ApiIdPart.Name"/> value.</summary>
        public required string Name { get; init; }

        /// <summary>Gets the JSON property name used for the part <see cref="ApiId.Kind"/> value.</summary>
        public required string Kind { get; init; }

        /// <summary>Gets the JSON property name used for the part <see cref="ApiId.Value"/> value.</summary>
        public required string Value { get; init; }
        #endregion
    }

    /// <summary>
    ///     Groups the property name collections used during serialization and deserialization for both the root <see cref="ApiId"/> and its composite parts.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        /// <summary>Gets the property names used for the root <see cref="ApiId"/> object.</summary>
        public required ApiIdPropertyNames ApiId { get; init; }

        /// <summary>Gets the property names used for an <see cref="ApiIdPart"/> entry in a composite <see cref="ApiId"/>.</summary>
        public required ApiIdPartPropertyNames ApiIdPart { get; init; }
        #endregion

        #region Factory Methods
        /// <summary>
        ///     Creates a <see cref="PropertyNames"/> instance using the provided <paramref name="policy"/> to convert CLR member names to JSON property names.
        /// </summary>
        /// <param name="policy">The naming policy to apply when generating property names.</param>
        /// <returns>A new <see cref="PropertyNames"/> populated with converted names.</returns>
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiId = new ApiIdPropertyNames
                {
                    Kind = policy.ConvertName(nameof(ApiId.Kind)),
                    Value = policy.ConvertName(nameof(ApiId.Value))
                },
                ApiIdPart = new ApiIdPartPropertyNames
                {
                    Name = policy.ConvertName(nameof(ApiIdPart.Name)),
                    Kind = policy.ConvertName(nameof(ApiIdPart.Kind)),
                    Value = policy.ConvertName(nameof(ApiIdPart.Value))
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Accumulates the state required to construct an <see cref="ApiId"/> during deserialization.
    /// </summary>
    private class ApiIdReadData
    {
        #region Properties
        /// <summary>Gets or sets the parsed <see cref="ApiIdKind"/> (required).</summary>
        public ApiIdKind? Kind { get; set; }

        /// <summary>Gets or sets the scalar value for non-composite ids.</summary>
        public string? ScalarValue { get; set; }

        /// <summary>Gets or sets the current index when iterating composite parts.</summary>
        public long CompositePartsIndex { get; set; }

        /// <summary>Gets or sets the collection of composite parts as they are read.</summary>
        public List<ApiIdPartReadData?>? CompositeParts { get; set; }
        #endregion
    }

    /// <summary>
    ///     Accumulates the state for a single <see cref="ApiIdPart"/> during composite deserialization.
    /// </summary>
    private class ApiIdPartReadData
    {
        #region Properties
        /// <summary>Gets or sets the optional part name.</summary>
        public string? Name { get; set; }

        /// <summary>Gets or sets the part <see cref="ApiIdKind"/> (required).</summary>
        public ApiIdKind? Kind { get; set; }

        /// <summary>Gets or sets the raw scalar string value for the part (required).</summary>
        public string? ScalarValue { get; set; }
        #endregion
    }

    /// <summary>
    ///     Root container for all read-time state while materializing an <see cref="ApiId"/>.
    /// </summary>
    private class ReadData
    {
        #region Properties
        /// <summary>Gets or sets the current <see cref="ApiIdReadData"/>.</summary>
        public ApiIdReadData? ApiId { get; set; }

        /// <summary>Gets or sets the current <see cref="ApiIdPartReadData"/> being populated.</summary>
        public ApiIdPartReadData? ApiIdPart { get; set; }
        #endregion
    }

    /// <summary>
    ///     Provides handler tables and callback methods for reading JSON tokens into <see cref="ReadData"/>.
    /// </summary>
    /// <param name="propertyNames">The property name dictionary for the current naming policy.</param>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiId Fields
        /// <summary>Gets the map of JSON property names to handlers for the root <see cref="ApiId"/> object.</summary>
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> ApiIdPropertyHandlers = new()
        {
            // ApiId Property Handlers
            { propertyNames.ApiId.Kind, HandleApiIdKind },
            { propertyNames.ApiId.Value, HandleApiIdValue },
        };
        #endregion

        #region ApiIdPart Fields
        /// <summary>Gets the map of JSON property names to handlers for an <see cref="ApiIdPart"/> object.</summary>
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> ApiIdPartPropertyHandlers = new()
        {
            // ApiIdPart Property Handlers
            { propertyNames.ApiIdPart.Name, HandleApiIdPartName },
            { propertyNames.ApiIdPart.Kind, HandleApiIdPartKind },
            { propertyNames.ApiIdPart.Value, HandleApiIdPartValue },
        };
        #endregion

        #region ApiIdPart Methods
        /// <summary>
        ///     Reads the <c>Name</c> property of an <see cref="ApiIdPart"/>.
        /// </summary>
        /// <param name="reader">The JSON reader positioned at the value token.</param>
        /// <param name="context">The read context to populate.</param>
        private static void HandleApiIdPartName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdPart ??= new ApiIdPartReadData();

            context.ReadData.ApiIdPart.Name = reader.GetString();
        }

        /// <summary>
        ///     Reads the <c>Kind</c> property of an <see cref="ApiIdPart"/>.
        /// </summary>
        /// <param name="reader">The JSON reader positioned at the value token.</param>
        /// <param name="context">The read context to populate.</param>
        private static void HandleApiIdPartKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdPart ??= new ApiIdPartReadData();

            var options = context.Options;
            context.ReadData.ApiIdPart.Kind = _apiIdKindJsonConverter.Read(ref reader, typeof(ApiIdKind), options);
        }

        /// <summary>
        ///     Reads the <c>Value</c> property of an <see cref="ApiIdPart"/>.
        /// </summary>
        /// <param name="reader">The JSON reader positioned at the value token.</param>
        /// <param name="context">The read context to populate.</param>
        private static void HandleApiIdPartValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdPart ??= new ApiIdPartReadData();

            context.ReadData.ApiIdPart.ScalarValue = reader.GetString();
        }
        #endregion

        #region ApiId Methods
        /// <summary>
        ///     Reads the <c>Kind</c> property of an <see cref="ApiId"/>.
        /// </summary>
        /// <param name="reader">The JSON reader positioned at the value token.</param>
        /// <param name="context">The read context to populate.</param>
        private static void HandleApiIdKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiId ??= new ApiIdReadData();

            var options = context.Options;
            context.ReadData.ApiId.Kind = _apiIdKindJsonConverter.Read(ref reader, typeof(ApiIdKind), options);
        }

        /// <summary>
        ///     Reads the <c>Value</c> property of an <see cref="ApiId"/>, which may be a string (scalar) or an array (composite).
        ///     Populates <see cref="ApiIdReadData.ScalarValue"/> or <see cref="ApiIdReadData.CompositeParts"/> accordingly.
        /// </summary>
        /// <param name="reader">The JSON reader positioned at the value token.</param>
        /// <param name="context">The read context to populate.</param>
        /// <exception cref="JsonException">Thrown when the token type is not a string or array.</exception>
        private static void HandleApiIdValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiId ??= new ApiIdReadData();

            // If the value is a string we have a scalar ApiId
            // If the value is an array we have a composite ApiId
            if (reader.TokenType == JsonTokenType.String)
            {
                context.ReadData.ApiId.ScalarValue = reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                context.ReadData.ApiId.CompositeParts = [];

                ReadJsonArray(ref reader, context, (x) => HandleApiIdPartArrayItem);
            }
            else
            {
                var propertyName = context.PropertyNames.ApiId.Value;
                throw new JsonException($"Invalid token type for {propertyName} : {reader.TokenType}.");
            }
        }

        /// <summary>
        ///     Reads a single array item for the composite <c>value</c> and appends the populated <see cref="ApiIdPartReadData"/> to <see cref="ApiIdReadData.CompositeParts"/>.
        /// </summary>
        /// <param name="reader">The JSON reader positioned at the start of an object.</param>
        /// <param name="context">The read context to populate.</param>
        private static void HandleApiIdPartArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdPart = null;
            ReadJsonObject(ref reader, context, context.ReadHandlers.ApiIdPartPropertyHandlers);
            var apiIdPartReadData = context.ReadData.ApiIdPart;

            context.ReadData.ApiId!.CompositeParts!.Add(apiIdPartReadData);
        }
        #endregion
    }
    #endregion

    #region Fields
    /// <summary>Cached enum converter used to read and write <see cref="ApiIdKind"/> values with current options.</summary>
    private static readonly EnumJsonConverter<ApiIdKind> _apiIdKindJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdJsonConverter()
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
    protected override ApiId CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var propertyNames = readContext.PropertyNames;
        var readData = readContext.ReadData;

        // Determine ApiId kind
        var nullableKind = readData.ApiId?.Kind;
        if (nullableKind is null)
        {
            var kindPropertyName = propertyNames.ApiId.Kind;
            throw new JsonException($"Missing required property: {kindPropertyName}.");
        }
        var kind = nullableKind.Value;

        // Handle Empty ApiId
        if (kind is ApiIdKind.None)
        {
            // Return Empty ApiId
            return ApiId.Empty;
        }

        // Handle Scalar ApiId
        if (kind is not ApiIdKind.Composite)
        {
            var scalarValueAsString = readData.ApiId?.ScalarValue;
            if (!ApiId.TryParse(kind, scalarValueAsString, out var scalarApiId))
            {
                var valuePropertyName = propertyNames.ApiId.Value;
                throw new JsonException($"Value '{scalarValueAsString}' is not a valid {kind} for property: {valuePropertyName}.");
            }

            // Create and return scalar ApiId
            return scalarApiId;
        }

        // Handle Composite ApiId
        var compositePartsReadData = readData.ApiId?.CompositeParts;
        if (compositePartsReadData is null || compositePartsReadData.Count == 0)
        {
            var valuePropertyName = propertyNames.ApiId.Value;
            throw new JsonException($"Composite ApiId requires non-empty array property: {valuePropertyName}.");
        }

        var compositePartsCount = compositePartsReadData.Count;
        var compositeParts = new List<ApiIdPart>(compositePartsCount);
        for (var index = 0; index < compositePartsCount; index++)
        {
            var partReadData = compositePartsReadData[index];
            if (partReadData is null)
            {
                var valuePropertyName = propertyNames.ApiId.Value;
                throw new JsonException($"Null composite ApiId part at index {index} in property: {valuePropertyName}.");
            }

            // Get part name (not required)
            var partName = partReadData.Name;

            // Get and validate part kind (required)
            var partNullableKind = partReadData.Kind;
            if (partNullableKind is null)
            {
                var propertyName = propertyNames.ApiIdPart.Kind;
                throw new JsonException($"Missing required property: {propertyName} for composite part at index {index}.");
            }
            var partKind = partNullableKind.Value;

            // Get and validate part value (required)
            var partScalarValueAsString = partReadData.ScalarValue;
            if (!ApiId.TryParse(partKind, partScalarValueAsString, out var partApiId))
            {
                var propertyName = propertyNames.ApiIdPart.Value;
                throw new JsonException($"Value '{partScalarValueAsString}' is not a valid {partKind} for property: {propertyName} of composite part at index {index}.");
            }

            // Create and add composite part to collection
            var apiIdPart = new ApiIdPart(partName, partApiId);
            compositeParts.Add(apiIdPart);
        }

        // Create and return composite ApiId
        return ApiId.Composite(compositeParts);
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.ApiIdPropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiId apiId, IWriteContext context)
    {
        if (!apiId.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        var writeContext = (DefaultWriteContext<PropertyNames>)context;
        WriteJsonObject(writer, () =>
        {
            WriteApiIdKind(writer, apiId, writeContext);

            if (apiId.IsComposite)
            {
                WriteApiIdCompositeValue(writer, apiId, writeContext);
            }
            else
            {
                WriteApiIdScalarValue(writer, apiId, writeContext);
            }
        });
    }
    #endregion

    #region Write Implementation Methods
    /// <summary>
    ///     Writes the <c>Kind</c> property for an <see cref="ApiId"/> using the enum converter.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="apiId">The value being serialized.</param>
    /// <param name="context">The write context with naming/options.</param>
    private static void WriteApiIdKind(Utf8JsonWriter writer, ApiId apiId, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiId.Kind;
        var value = apiId.Kind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _apiIdKindJsonConverter);
    }

    /// <summary>
    ///     Writes the <c>Value</c> property for a composite <see cref="ApiId"/> as an array of objects, including each part's name, kind, and value.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="apiId">The composite identifier to serialize.</param>
    /// <param name="context">The write context with naming/options.</param>
    private static void WriteApiIdCompositeValue(Utf8JsonWriter writer, ApiId apiId, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiId.Value;
        var parts = apiId.Parts.ToArray();
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            parts,
            options,
            collection => WriteJsonArray
            (
                writer,
                collection,
                item =>
                {
                    WriteJsonObject
                    (
                        writer,
                        () =>
                        {
                            WriteApiIdPartName(writer, item, context);
                            WriteApiIdPartKind(writer, item, context);
                            WriteApiIdPartValue(writer, item, context);
                        }
                    );
                }
            )
        );
    }

    /// <summary>
    ///     Writes the <c>Kind</c> property for a single composite part.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="apiIdPart">The part being serialized.</param>
    /// <param name="context">The write context with naming/options.</param>
    private static void WriteApiIdPartKind(Utf8JsonWriter writer, ApiIdPart apiIdPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdPart.Kind;
        var value = apiIdPart.Value.Kind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _apiIdKindJsonConverter);
    }

    /// <summary>
    ///     Writes the <c>Name</c> property for a single composite part.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="apiIdPart">The part being serialized.</param>
    /// <param name="context">The write context with naming/options.</param>
    private static void WriteApiIdPartName(Utf8JsonWriter writer, ApiIdPart apiIdPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdPart.Name;
        var value = apiIdPart.Name;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    /// <summary>
    ///     Writes the <c>Value</c> property for a single composite part as a string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="apiIdPart">The part being serialized.</param>
    /// <param name="context">The write context with naming/options.</param>
    private static void WriteApiIdPartValue(Utf8JsonWriter writer, ApiIdPart apiIdPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdPart.Value;
        var value = apiIdPart.Value.ToString();
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    /// <summary>
    ///     Writes the <c>Value</c> property for a scalar <see cref="ApiId"/> as a string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="apiId">The scalar identifier to serialize.</param>
    /// <param name="context">The write context with naming/options.</param>
    private static void WriteApiIdScalarValue(Utf8JsonWriter writer, ApiId apiId, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiId.Value;
        var value = apiId.ToString();
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }
    #endregion
}
