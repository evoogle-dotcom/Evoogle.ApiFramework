// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Evoogle.ApiFramework.Identity.Json;

// === JSON converters ===
public sealed class ApiIdJsonConverter : JsonConverter<ApiId>
{
    private const string _kindProp = "kind";
    private const string _valueProp = "value";

    public override ApiId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return ApiId.Empty;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var text = reader.GetString();
            return ApiId.TryParse(text, out var auto) ? auto : ApiId.Empty;
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

            if (string.Equals(propName, _kindProp, StringComparison.OrdinalIgnoreCase))
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
            else if (string.Equals(propName, _valueProp, StringComparison.OrdinalIgnoreCase))
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

            return ApiId.Composite(parts);
        }

        if (!ApiId.TryParse(kind, valueText, out var id))
        {
            throw new JsonException($"Value '{valueText}' is not a valid {kind}.");
        }

        return id;
    }

    public override void Write(Utf8JsonWriter writer, ApiId value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString(_kindProp, value.Kind.ToString().ToLowerInvariant());
        if (value.IsComposite)
        {
            writer.WritePropertyName(_valueProp);
            writer.WriteStartArray();
            foreach (var p in value.Parts)
            {
                writer.WriteStartObject();
                if (p.Name is not null)
                {
                    writer.WriteString("name", p.Name);
                }

                writer.WritePropertyName("value");
                this.Write(writer, p.Value, options);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
        else
        {
            writer.WriteString(_valueProp, value.ToString());
        }
        writer.WriteEndObject();
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
}