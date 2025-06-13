// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiJsonConverterHelpers
{
    #region Read Methods
    public static void ReadJsonArray<T, TContext>
    (
        ref Utf8JsonReader reader,
        ref TContext context,
        Func<TContext, ApiJsonReaderHandler<TContext>> arrayElementHandlerAccessor
    )
        where TContext : IHasLogger<T>
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of an array.");

        var handler = arrayElementHandlerAccessor(context);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            handler(ref reader, ref context);
        }
    }

    public static void ReadJsonObject<T, TContext>
    (
        ref Utf8JsonReader reader,
        ref TContext context,
        Func<TContext, Dictionary<string, ApiJsonReaderHandler<TContext>>> propertyHandlersAccessor
    )
        where TContext : IHasLogger<T>
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of an object.");

        var handlers = propertyHandlersAccessor(context);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected object property name.");

            var propertyName = reader.GetString()!;
            reader.Read();

            if (handlers.TryGetValue(propertyName, out var handler))
            {
                handler(ref reader, ref context);
            }
            else
            {
                context.Logger.LogWarning("Skipping unknown JSON property: '{Skipped}'", propertyName);
                reader.Skip();
            }
        }
    }
    #endregion

    #region Utility Methods
    public static JsonNamingPolicy GetPropertyNamingPolicy(JsonSerializerOptions options)
    {
        return options.PropertyNamingPolicy ?? new NullJsonNamingPolicy();
    }
    #endregion
}
