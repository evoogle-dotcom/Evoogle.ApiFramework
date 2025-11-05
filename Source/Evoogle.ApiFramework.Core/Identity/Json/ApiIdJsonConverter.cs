// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;
using System.Text.Json;

using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Identity.Json;

public sealed class ApiIdJsonConverter(ILogger<ApiIdJsonConverter>? logger) : JsonConverterBase<ApiId>(logger)
{
    #region Property Types
    private readonly record struct ApiIdPropertyNames
    {
        #region Immutable Properties
        public required string Kind { get; init; }
        public required string Value { get; init; }
        #endregion
    }

    private readonly record struct ApiIdPartPropertyNames
    {
        #region Immutable Properties
        public required string Name { get; init; }
        public required string Kind { get; init; }
        public required string Value { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdPropertyNames ApiId { get; init; }
        public required ApiIdPartPropertyNames ApiIdPart { get; init; }
        #endregion

        #region Factory Methods
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
    private class ApiIdReadData
    {
        #region Properties
        public string? Kind { get; set; }
        public string? ScalarValue { get; set; }
        public List<ApiIdPartReadData>? CompositeParts { get; set; }
        #endregion
    }

    private class ApiIdPartReadData
    {
        #region Properties
        public string? Name { get; set; }
        public string? Kind { get; set; }
        public string? ScalarValue { get; set; }
        #endregion
    }

    private class ReadData
    {
        #region Properties
        public ApiId? Foo { get; set; }

        public ApiIdReadData? ApiId { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiId Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            // ApiId Property Handlers
            { propertyNames.ApiId.Kind, HandleApiIdKind },
            { propertyNames.ApiId.Value, HandleApiIdValue },
        };
        #endregion

        #region ApiId Methods
        private static void HandleApiIdKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiId ??= new ApiIdReadData();

            context.ReadData.ApiId.Kind = reader.GetString();
        }

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
                var options = context.Options;
                var propertyName = context.PropertyNames.ApiId.Value;

                throw new NotImplementedException();
            }
            else
            {
                throw new JsonException("Invalid token type for ApiId value.");
            }
        }
        #endregion
    }
    #endregion

    #region Fields
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

    protected override ApiId CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData;
        return readData.Foo ?? throw new InvalidOperationException("ApiId value was not set during reading.");
    }

    private static ApiIdKind ParseKind(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return ApiIdKind.None;
        }

        s = s.Trim();
        if (Enum.TryParse<ApiIdKind>(s, true, out var k))
        {
            return k;
        }

        return s.ToLowerInvariant() switch
        {
            "str" or "string" => ApiIdKind.String,
            "i32" or "int" or "int32" => ApiIdKind.Int32,
            "i64" or "long" or "int64" => ApiIdKind.Int64,
            "guid" or "uuid" => ApiIdKind.Guid,
            "ulid" => ApiIdKind.Ulid,
            "culture" or "cultureinfo" or "locale" => ApiIdKind.Culture,
            "composite" or "cmp" => ApiIdKind.Composite,
            _ => ApiIdKind.None
        };
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData;
        var propertyNames = readContext.PropertyNames;
        var kindPropertyName = propertyNames.ApiId.Kind;
        var valuePropertyName = propertyNames.ApiId.Value;
        var options = readContext.Options;

        if (reader.TokenType == JsonTokenType.Null)
        {
            readData.Foo = ApiId.Empty;
            return;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var text = reader.GetString();
            readData.Foo = ApiId.TryParse(text, out var auto) ? auto : ApiId.Empty;
            return;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected object or string for ApiId.");
        }

        var kind = ApiIdKind.None;
        var valueText = default(string?);
        var parts = default(ApiIdPart[]?);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Invalid token in ApiId object.");
            }

            var propName = reader.GetString();
            reader.Read();

            if (string.Equals(propName, kindPropertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    kind = ParseKind(reader.GetString());
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    if (!reader.TryGetByte(out var b))
                    {
                        throw new JsonException("Invalid kind numeric value.");
                    }
                    kind = (ApiIdKind)b;
                }
                else
                {
                    throw new JsonException("Invalid kind value type.");
                }
            }
            else if (string.Equals(propName, valuePropertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (kind == ApiIdKind.Composite)
                {
                    if (reader.TokenType != JsonTokenType.StartArray)
                    {
                        throw new JsonException("Composite value must be an array of parts.");
                    }

                    var list = new List<ApiIdPart>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        if (reader.TokenType != JsonTokenType.StartObject)
                        {
                            throw new JsonException("Composite part must be an object.");
                        }

                        var name = default(string?);
                        var val = default(ApiId);
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                        {
                            if (reader.TokenType != JsonTokenType.PropertyName)
                            {
                                throw new JsonException("Invalid part token.");
                            }

                            var pn = reader.GetString();
                            reader.Read();
                            if (string.Equals(pn, "name", StringComparison.OrdinalIgnoreCase))
                            {
                                name = reader.TokenType == JsonTokenType.String ? reader.GetString() : throw new JsonException("Part.name must be a string.");
                            }
                            else if (string.Equals(pn, "value", StringComparison.OrdinalIgnoreCase))
                            {
                                val = this.Read(ref reader, typeof(ApiId), options);
                                if (val.Kind == ApiIdKind.Composite)
                                {
                                    throw new JsonException("Nested composite parts are not allowed in ApiId.");
                                }
                            }
                            else
                            {
                                reader.Skip();
                            }
                        }
                        list.Add(new ApiIdPart(name, val));
                    }
                    parts = [.. list];
                }
                else
                {
                    valueText = reader.TokenType switch
                    {
                        JsonTokenType.String => reader.GetString(),
                        JsonTokenType.Number => reader.GetDouble().ToString(CultureInfo.InvariantCulture),
                        JsonTokenType.True => bool.TrueString,
                        JsonTokenType.False => bool.FalseString,
                        _ => throw new JsonException("Unsupported value token for ApiId.")
                    };
                }
            }
            else
            {
                reader.Skip();
            }
        }

        if (kind == ApiIdKind.None)
        {
            throw new JsonException("ApiId.kind is required.");
        }

        if (kind == ApiIdKind.Composite)
        {
            if (parts is null || parts.Length == 0)
            {
                throw new JsonException("Composite value array required.");
            }

            readData.Foo = ApiId.Composite(parts);
            return;
        }

        if (!ApiId.TryParse(kind, valueText, out var id))
        {
            throw new JsonException($"Value '{valueText}' is not a valid {kind}.");
        }

        readData.Foo = id;
        return;
    }

    protected void ReadCoreWorkingVersion(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.PropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

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
    private static void WriteApiIdKind(Utf8JsonWriter writer, ApiId apiId, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiId.Kind;
        var value = apiId.Kind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _apiIdKindJsonConverter);
    }

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

    private static void WriteApiIdPartKind(Utf8JsonWriter writer, ApiIdPart apiIdPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdPart.Kind;
        var value = apiIdPart.Value.Kind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, value, options, _apiIdKindJsonConverter);
    }

    private static void WriteApiIdPartName(Utf8JsonWriter writer, ApiIdPart apiIdPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdPart.Name;
        var value = apiIdPart.Name;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdPartValue(Utf8JsonWriter writer, ApiIdPart apiIdPart, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdPart.Value;
        var value = apiIdPart.Value.ToString();
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteApiIdScalarValue(Utf8JsonWriter writer, ApiId apiId, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiId.Value;
        var value = apiId.ToString();
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }
    #endregion
}





// === JSON converters ===
// public sealed class ApiIdJsonConverter : JsonConverter<ApiId>
// {
//     private const string _kindProp = "kind";
//     private const string _valueProp = "value";

//     public override ApiId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//     {
//         if (reader.TokenType == JsonTokenType.Null)
//         {
//             return ApiId.Empty;
//         }

//         if (reader.TokenType == JsonTokenType.String)
//         {
//             var text = reader.GetString();
//             return ApiId.TryParse(text, out var auto) ? auto : ApiId.Empty;
//         }

//         if (reader.TokenType != JsonTokenType.StartObject)
//         {
//             throw new JsonException("Expected object or string for ApiId.");
//         }

//         var kind = ApiIdKind.None;
//         var valueText = default(string?);
//         var parts = default(ApiIdPart[]?);

//         while (reader.Read())
//         {
//             if (reader.TokenType == JsonTokenType.EndObject)
//             {
//                 break;
//             }

//             if (reader.TokenType != JsonTokenType.PropertyName)
//             {
//                 throw new JsonException("Invalid token in ApiId object.");
//             }

//             var propName = reader.GetString();
//             reader.Read();

//             if (string.Equals(propName, _kindProp, StringComparison.OrdinalIgnoreCase))
//             {
//                 if (reader.TokenType == JsonTokenType.String)
//                 {
//                     kind = ParseKind(reader.GetString());
//                 }
//                 else if (reader.TokenType == JsonTokenType.Number)
//                 {
//                     if (!reader.TryGetByte(out var b))
//                     {
//                         throw new JsonException("Invalid kind numeric value.");
//                     }
//                     kind = (ApiIdKind)b;
//                 }
//                 else
//                 {
//                     throw new JsonException("Invalid kind value type.");
//                 }
//             }
//             else if (string.Equals(propName, _valueProp, StringComparison.OrdinalIgnoreCase))
//             {
//                 if (kind == ApiIdKind.Composite)
//                 {
//                     if (reader.TokenType != JsonTokenType.StartArray)
//                     {
//                         throw new JsonException("Composite value must be an array of parts.");
//                     }

//                     var list = new List<ApiIdPart>();
//                     while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
//                     {
//                         if (reader.TokenType != JsonTokenType.StartObject)
//                         {
//                             throw new JsonException("Composite part must be an object.");
//                         }

//                         var name = default(string?);
//                         var val = default(ApiId);
//                         while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
//                         {
//                             if (reader.TokenType != JsonTokenType.PropertyName)
//                             {
//                                 throw new JsonException("Invalid part token.");
//                             }

//                             var pn = reader.GetString();
//                             reader.Read();
//                             if (string.Equals(pn, "name", StringComparison.OrdinalIgnoreCase))
//                             {
//                                 name = reader.TokenType == JsonTokenType.String ? reader.GetString() : throw new JsonException("Part.name must be a string.");
//                             }
//                             else if (string.Equals(pn, "value", StringComparison.OrdinalIgnoreCase))
//                             {
//                                 val = this.Read(ref reader, typeof(ApiId), options);
//                                 if (val.Kind == ApiIdKind.Composite)
//                                 {
//                                     throw new JsonException("Nested composite parts are not allowed in ApiId.");
//                                 }
//                             }
//                             else
//                             {
//                                 reader.Skip();
//                             }
//                         }
//                         list.Add(new ApiIdPart(name, val));
//                     }
//                     parts = [.. list];
//                 }
//                 else
//                 {
//                     valueText = reader.TokenType switch
//                     {
//                         JsonTokenType.String => reader.GetString(),
//                         JsonTokenType.Number => reader.GetDouble().ToString(CultureInfo.InvariantCulture),
//                         JsonTokenType.True => bool.TrueString,
//                         JsonTokenType.False => bool.FalseString,
//                         _ => throw new JsonException("Unsupported value token for ApiId.")
//                     };
//                 }
//             }
//             else
//             {
//                 reader.Skip();
//             }
//         }

//         if (kind == ApiIdKind.None)
//         {
//             throw new JsonException("ApiId.kind is required.");
//         }

//         if (kind == ApiIdKind.Composite)
//         {
//             if (parts is null || parts.Length == 0)
//             {
//                 throw new JsonException("Composite value array required.");
//             }

//             return ApiId.Composite(parts);
//         }

//         if (!ApiId.TryParse(kind, valueText, out var id))
//         {
//             throw new JsonException($"Value '{valueText}' is not a valid {kind}.");
//         }

//         return id;
//     }

//     public override void Write(Utf8JsonWriter writer, ApiId value, JsonSerializerOptions options)
//     {
//         if (!value.HasValue)
//         {
//             writer.WriteNullValue();
//             return;
//         }

//         writer.WriteStartObject();
//         writer.WriteString(_kindProp, value.Kind.ToString().ToLowerInvariant());
//         if (value.IsComposite)
//         {
//             writer.WritePropertyName(_valueProp);
//             writer.WriteStartArray();
//             foreach (var p in value.Parts)
//             {
//                 writer.WriteStartObject();
//                 if (p.Name is not null)
//                 {
//                     writer.WriteString("name", p.Name);
//                 }

//                 writer.WritePropertyName("value");
//                 this.Write(writer, p.Value, options);
//                 writer.WriteEndObject();
//             }
//             writer.WriteEndArray();
//         }
//         else
//         {
//             writer.WriteString(_valueProp, value.ToString());
//         }
//         writer.WriteEndObject();
//     }

//     private static ApiIdKind ParseKind(string? s)
//     {
//         if (string.IsNullOrWhiteSpace(s))
//         {
//             return ApiIdKind.None;
//         }

//         s = s.Trim();
//         if (Enum.TryParse<ApiIdKind>(s, true, out var k))
//         {
//             return k;
//         }

//         return s.ToLowerInvariant() switch
//         {
//             "str" or "string" => ApiIdKind.String,
//             "i32" or "int" or "int32" => ApiIdKind.Int32,
//             "i64" or "long" or "int64" => ApiIdKind.Int64,
//             "guid" or "uuid" => ApiIdKind.Guid,
//             "ulid" => ApiIdKind.Ulid,
//             "culture" or "cultureinfo" or "locale" => ApiIdKind.Culture,
//             "composite" or "cmp" => ApiIdKind.Composite,
//             _ => ApiIdKind.None
//         };
//     }
// }