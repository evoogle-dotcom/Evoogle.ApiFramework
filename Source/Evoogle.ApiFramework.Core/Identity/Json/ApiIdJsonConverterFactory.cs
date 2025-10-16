// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Evoogle.ApiFramework.Identity.Json;

public sealed class ApiIdJsonConverterFactory(ApiIdJsonShape shape = ApiIdJsonShape.Auto) : JsonConverterFactory
{
    private readonly ApiIdJsonShape _shape = shape;

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(ApiId);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => _shape switch
        {
            ApiIdJsonShape.Scalar => new ScalarApiIdConverter(),
            ApiIdJsonShape.Object => new ObjectApiIdConverter(),
            _ => new ObjectApiIdConverter(),
        };

    private sealed class ObjectApiIdConverter : JsonConverter<ApiId>
    {
        private static readonly ApiIdJsonConverter _inner = new();

        public override ApiId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => _inner.Read(ref reader, typeToConvert, options);

        public override void Write(Utf8JsonWriter writer, ApiId value, JsonSerializerOptions options)
            => _inner.Write(writer, value, options);
    }

    private sealed class ScalarApiIdConverter : JsonConverter<ApiId>
    {
        public override ApiId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var text = reader.GetString();
                return ApiId.TryParse(text, out var auto) ? auto : ApiId.Empty;
            }

            var proxy = new ApiIdJsonConverter();
            return proxy.Read(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, ApiId value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }

            if (value.IsComposite)
            {
                var proxy = new ApiIdJsonConverter();
                proxy.Write(writer, value, options);
                return;
            }

            var s = value.ToString();
            if (s is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(s);
        }
    }
}
