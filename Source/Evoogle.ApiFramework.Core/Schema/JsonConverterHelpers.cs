using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema;

internal static class JsonConverterHelpers
{
    internal delegate void JsonReadHandler<TContext>(ref Utf8JsonReader reader, ref TContext context);

    internal static JsonNamingPolicy GetPropertyNamingPolicy(JsonSerializerOptions options)
    {
        return options.PropertyNamingPolicy ?? new NullJsonNamingPolicy();
    }

    internal static void ReadJsonArray<TContext>(
        ref Utf8JsonReader reader,
        ref TContext context,
        Func<TContext, JsonReadHandler<TContext>> arrayElementHandlerAccessor)
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

    internal static void ReadJsonObject<TContext>(
        ref Utf8JsonReader reader,
        ref TContext context,
        Func<TContext, Dictionary<string, JsonReadHandler<TContext>>> propertyHandlersAccessor)
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
            else if (context is IJsonConverterLogger loggerContext)
            {
                loggerContext.Logger?.LogWarning("Skipping unknown JSON property: '{Skipped}'", propertyName);
                reader.Skip();
            }
            else
            {
                reader.Skip();
            }
        }
    }
}

internal interface IJsonConverterLogger
{
    Microsoft.Extensions.Logging.ILogger? Logger { get; }
}
