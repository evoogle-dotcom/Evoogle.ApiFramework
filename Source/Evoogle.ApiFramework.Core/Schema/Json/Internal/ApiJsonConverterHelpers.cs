// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using Evoogle.Extension;
using Evoogle.Extensions;
using Evoogle.Json;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiJsonConverterHelpers
{
    #region Types
    public delegate void ApiJsonReaderHandler<TContext>(ref Utf8JsonReader reader, ref TContext context);

    public readonly record struct ExtensibleBasePropertyNames
    {
        #region Properties
        public required string Extensions { get; init; }
        #endregion
    }
    #endregion

    #region Read Methods
    public static void ReadJsonArray<T, TContext>
    (
        ref Utf8JsonReader reader,
        ref TContext context,
        Func<TContext, ApiJsonReaderHandler<TContext>> arrayElementHandlerAccessor
    )
        where TContext : IHasLogger<T>
    {
        // Ensure we are at the start of an array.
        // If not, throw an exception to indicate that we expected an array.
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected start of an array.");
        }

        var index = -1;
        var handler = arrayElementHandlerAccessor(context);
        while (reader.Read())
        {
            // Check for end of the array.
            // If we reach the end, break out of the loop.
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            // Increment the index for each element read.
            // This helps in tracking the position of the element in the array.
            ++index;

            // Handle null array elements.
            // If we encounter a null element, log it and continue to the next element.
            if (reader.TokenType == JsonTokenType.Null)
            {
                // Log the skipped null element and continue to the next element.
                // This prevents null elements from causing issues in the deserialization process.
                context.Logger.LogTrace("Skipping null JSON array element at index {Index}", index);
                continue;
            }

            // Handle the current array element using the provided handler.
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
        // Ensure we are at the start of an object.
        // If not, throw an exception to indicate that we expected an object.
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of an object.");
        }

        var handlers = propertyHandlersAccessor(context);
        while (reader.Read())
        {
            // Check for end of the object.
            // If we reach the end, break out of the loop.
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            // If we are not at a property name, throw an exception.
            // This ensures we are reading a valid JSON object.
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected object property name.");
            }

            // Read the property name.
            var propertyName = reader.GetString()!;

            // Read the property value.
            var readSuccess = reader.Read();
            if (!readSuccess)
            {
                throw new JsonException($"Failed to read value for property '{propertyName}'.");
            }

            // Handle null property values.
            if (reader.TokenType == JsonTokenType.Null)
            {
                // Log the skipped null property and continue to the next property.
                // This prevents null properties from causing issues in the deserialization process.
                context.Logger.LogTrace("Skipping null JSON property: '{Skipped}'", propertyName);
                continue;
            }

            // Check if we have a handler for this property value.
            // If not, skip it and log a warning.
            if (handlers.TryGetValue(propertyName, out var handler))
            {
                // Handle the property value using the corresponding handler.
                handler(ref reader, ref context);
            }
            else
            {
                // Log a warning for the skipped property.
                // This helps in identifying properties that are not handled by the deserialization logic.
                context.Logger.LogWarning("Skipping unknown JSON property: '{Skipped}'", propertyName);
                reader.Skip();
            }
        }
    }
    #endregion

    #region Extension Methods
    public static Dictionary<string, object> ReadExtensions<T>
    (
        ref Utf8JsonReader reader,
        JsonSerializerOptions options,
        ILogger<T> logger
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of an object.");
        }

        var extensions = new Dictionary<string, object>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected object property name.");
            }

            var typeName = reader.GetString()!;
            var extensionType = TypeJsonConverter.GetDeserializeType(typeName);

            logger.LogTrace("Deserializing extension type: {ExtensionType}", extensionType.Name);

            reader.Read();

            var extension = JsonSerializer.Deserialize(ref reader, extensionType, options)
                ?? throw new JsonException($"Failed to deserialize {typeName}.");

            logger.LogDebug("Deserialized  extension type: {ExtensionType}", extensionType.Name);

            extensions.Add(typeName, extension);
        }

        return extensions;
    }

    public static void WriteExtensions<T>
    (
        Utf8JsonWriter writer,
        OrderedDictionary<Type, object> extensions,
        JsonSerializerOptions options,
        ILogger<T> logger
    )
    {
        writer.WriteStartObject();

        foreach (var (extensionType, extension) in extensions)
        {
            var typeName = TypeJsonConverter.GetSerializeTypeName(extensionType);

            logger.LogTrace("Serializing extension type: {ExtensionType}", extensionType.Name);

            writer.WritePropertyName(typeName);
            JsonSerializer.Serialize(writer, extension, extensionType, options);

            logger.LogDebug("Serialized  extension type: {ExtensionType}", extensionType.Name);
        }

        writer.WriteEndObject();
    }

    public static void AttachExtensions
    (
        ExtensibleBase extensibleBase,
        Dictionary<string, object>? extensions
    )
    {
        if (extensions == null)
        {
            return;
        }

        foreach ((var typeName, var extension) in extensions)
        {
            var extensionType = TypeJsonConverter.GetDeserializeType(typeName);
            extensibleBase.AttachExtension(extensionType, extension);
        }
    }
    #endregion

    #region Utility Methods
    public static JsonNamingPolicy GetPropertyNamingPolicy(JsonSerializerOptions options) => options.PropertyNamingPolicy ?? new NullJsonNamingPolicy();

    public static ApiTypeKind? GetApiTypeKind(ILogger logger, string? kindAsString)
    {
        if (kindAsString is null)
        {
            return null;
        }

        if (Enum.TryParse<ApiTypeKind>(kindAsString, out var kind) == false)
        {
            logger.LogError("Unable to parse {Kind} enumeration string: '{KindAsString}'", nameof(ApiTypeKind), kindAsString);
            return null;
        }

        return kind;
    }
    #endregion

    #region Validation Methods
    public static void AddValidationError(ref List<ValidationResult>? results, string message, string memberName)
    {
        results ??= [];
        results.Add(new ValidationResult(message, [memberName]));
    }

    public static void AddEmptyCollectionPropertyError(ref List<ValidationResult>? results, string propertyName) => AddValidationError(ref results, $"Empty collection property: {propertyName}.", propertyName);

    public static void AddInvalidPropertyError(ref List<ValidationResult>? results, string propertyName, string reason) => AddValidationError(ref results, $"Invalid property: {propertyName}. Reason: {reason}", propertyName);

    public static void AddMissingPropertyError(ref List<ValidationResult>? results, string propertyName) => AddValidationError(ref results, $"Missing property: {propertyName}.", propertyName);

    public static void ThrowIfInvalid<T, TReadContext, TException>
    (
        in TReadContext context,
        string typeName,
        IEnumerable<ValidationResult>? validationResults,
        Func<string, TException> exceptionFactory
    )
        where TReadContext : IHasLogger<T>
        where TException : Exception
    {
        if (validationResults == null)
        {
            return;
        }

        if (validationResults.Any() == false)
        {
            return;
        }

        // Create a delimited string of all the failed validation result error messages.
        var validationErrorMessage = validationResults.Where(x => x != ValidationResult.Success && !string.IsNullOrWhiteSpace(x.ErrorMessage)).SafeToDelimitedString('\n');
        if (string.IsNullOrWhiteSpace(validationErrorMessage))
        {
            return;
        }

        context.Logger.LogError("Validation failed for '{TypeName}': {Message}", typeName, validationErrorMessage);

        throw exceptionFactory(validationErrorMessage);
    }
    #endregion
}
