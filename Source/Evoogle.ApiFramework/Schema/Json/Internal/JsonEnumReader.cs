// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text;
using System.Text.Json;

using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema.Json.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal static class JsonEnumReader
{
    #region Types
    private static class ConverterCache<TEnum>
        where TEnum : struct, Enum
    {
        public static readonly NullableEnumJsonConverter<TEnum> Converter = new(EnumJsonInvalidValuePolicy.ReturnNull);
    }
    #endregion

    #region Methods
    public static JsonEnumReadResult<TEnum> Read<TEnum>(ref Utf8JsonReader reader, JsonSerializerOptions options)
        where TEnum : struct, Enum
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return new JsonEnumReadResult<TEnum>(JsonEnumReadStatus.Null, Value: null, RawText: null);
        }

        // The token text must be captured before the converter consumes the token.
        var rawText = reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => Encoding.UTF8.GetString(reader.ValueSpan),
            _ => $"<{reader.TokenType}>"
        };

        var value = ConverterCache<TEnum>.Converter.Read(ref reader, typeof(TEnum?), options);

        return value is null
            ? new JsonEnumReadResult<TEnum>(JsonEnumReadStatus.Invalid, Value: null, rawText)
            : new JsonEnumReadResult<TEnum>(JsonEnumReadStatus.Valid, value, rawText);
    }
    #endregion
}
