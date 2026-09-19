// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Converts <see cref="ApiTypeExpression"/> instances to and from JSON.
/// </summary>
/// <param name="logger">The optional logger instance.</param>
public sealed class ApiTypeExpressionJsonConverter(ILogger<ApiTypeExpressionJsonConverter>? logger)
    : JsonConverterBase<ApiTypeExpression>(logger)
{
    private enum ExpressionShape
    {
        Empty,
        Reference,
        Inline,
        Invalid
    }

    private enum PropertyKind
    {
        Other,
        ApiKind,
        ApiName,
        ClrType
    }

    #region Property Types
    private readonly record struct ApiTypeExpressionPropertyNames
    {
        #region Immutable Properties
        public required string ApiKind { get; init; }
        public required string ApiName { get; init; }
        public required string ClrType { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiTypeExpressionPropertyNames ApiTypeExpression { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiTypeExpression = new ApiTypeExpressionPropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(ApiTypeReference.ApiKind)),
                    ApiName = policy.ConvertName(nameof(ApiTypeReference.ApiName)),
                    ClrType = policy.ConvertName(nameof(ApiTypeReference.ClrType)),
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    private sealed class ReadState
    {
        public ApiType? ApiInlineType { get; set; }
        public ApiTypeReference? ApiTypeReference { get; set; }
    }

    private sealed class ReadContext
    (
        ILogger logger,
        JsonSerializerOptions options,
        JsonNamingPolicy propertyNamingPolicy,
        PropertyNames propertyNames
    ) : DefaultContext<PropertyNames>(logger, options, propertyNamingPolicy, propertyNames), IReadContext
    {
        #region Properties
        public ReadState ReadData { get; } = new();
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiTypeExpressionJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase Methods
    /// <inheritdoc/>
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
    {
        var policy = options.GetPropertyNamingPolicy();
        var names = GetPropertyNames(options, PropertyNames.Create);
        return new ReadContext(logger, options, policy, names);
    }

    /// <inheritdoc/>
    protected override ApiTypeExpression CreateValue(IReadContext context)
    {
        var readContext = (ReadContext)context;
        var apiInlineType = readContext.ReadData.ApiInlineType;
        var apiTypeReference = readContext.ReadData.ApiTypeReference;

        var apiTypeExpression = new ApiTypeExpression(apiInlineType, apiTypeReference);
        return apiTypeExpression;
    }

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext(logger, options, buildPropertyNames: PropertyNames.Create);

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (ReadContext)context;
        var propertyNames = readContext.PropertyNames.ApiTypeExpression;
        var shape = GetExpressionShape
        (
            ref reader,
            propertyNames.ApiKind,
            propertyNames.ApiName,
            propertyNames.ClrType
        );

        if (shape == ExpressionShape.Empty)
        {
            // If there are no properties, it is considered an invalid expression object.
            reader.Skip();
            return;
        }

        var options = readContext.Options;
        if (shape == ExpressionShape.Reference)
        {
            // Handle the reference case here
            var apiTypeReference = JsonSerializer.Deserialize<ApiTypeReference>(ref reader, options);
            readContext.ReadData.ApiTypeReference = apiTypeReference;
        }
        else if (shape == ExpressionShape.Inline)
        {
            // Handle the inline case here
            var apiInlineType = JsonSerializer.Deserialize<ApiType>(ref reader, options);
            readContext.ReadData.ApiInlineType = apiInlineType;
        }

        // If the object is neither a reference nor an inline object, it is considered an invalid expression object.
        reader.Skip();
    }

    /// <inheritdoc/>
    protected override void WriteCore
    (
        Utf8JsonWriter writer,
        ApiTypeExpression value,
        IWriteContext context
    )
    {
        var options = context.Options;
        if (value.IsReference)
        {
            // Handle the reference case here
            var apiTypeReference = value.ApiTypeReference;
            writer.TryWriteWithSerializer(apiTypeReference, options);
            return;
        }
        else if (value.IsInline)
        {
            // Handle the inline case here
            var apiInlineType = value.ApiInlineType;
            writer.TryWriteWithSerializer(apiInlineType, options);
            return;
        }
        else
        {
            // Handle the invalid state here
            throw new JsonException("Invalid ApiTypeExpression state: neither inline nor reference.");
        }
    }
    #endregion

    #region Helper Methods
    private static ExpressionShape GetExpressionShape
    (
        ref Utf8JsonReader reader,
        string apiKindPropertyName,
        string apiNamePropertyName,
        string clrTypePropertyName
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of JSON object.");
        }

        var lookahead = reader;
        var objectDepth = lookahead.CurrentDepth;
        var propertyCount = 0;
        var firstPropertyKind = PropertyKind.Other;
        var secondPropertyKind = PropertyKind.Other;

        while (true)
        {
            if (!lookahead.Read())
            {
                throw new JsonException("Unexpected end of JSON.");
            }

            if (lookahead.TokenType == JsonTokenType.EndObject && lookahead.CurrentDepth == objectDepth)
            {
                break;
            }

            if (lookahead.TokenType != JsonTokenType.PropertyName
                || lookahead.CurrentDepth != objectDepth + 1)
            {
                throw new JsonException("Expected a JSON property name.");
            }

            var propertyKind = GetPropertyKind
            (
                ref lookahead,
                apiKindPropertyName,
                apiNamePropertyName,
                clrTypePropertyName
            );

            if (!lookahead.Read())
            {
                throw new JsonException("Unexpected end of JSON.");
            }

            if (lookahead.TokenType == JsonTokenType.Null)
            {
                continue;
            }

            propertyCount++;
            if (propertyCount == 1)
            {
                firstPropertyKind = propertyKind;
            }
            else if (propertyCount == 2)
            {
                secondPropertyKind = propertyKind;
            }

            lookahead.Skip();
        }

        if (propertyCount == 0)
        {
            return ExpressionShape.Empty;
        }

        if (propertyCount > 2)
        {
            return ExpressionShape.Inline;
        }

        if (propertyCount == 1 && firstPropertyKind == PropertyKind.ClrType)
        {
            return ExpressionShape.Reference;
        }

        if (propertyCount == 2
            && ((firstPropertyKind == PropertyKind.ApiKind && secondPropertyKind == PropertyKind.ApiName)
                || (firstPropertyKind == PropertyKind.ApiName && secondPropertyKind == PropertyKind.ApiKind)))
        {
            return ExpressionShape.Reference;
        }

        if (firstPropertyKind != PropertyKind.Other || secondPropertyKind != PropertyKind.Other)
        {
            return ExpressionShape.Reference;
        }

        return ExpressionShape.Invalid;
    }

    private static PropertyKind GetPropertyKind
    (
        ref Utf8JsonReader reader,
        string apiKindPropertyName,
        string apiNamePropertyName,
        string clrTypePropertyName
    )
    {
        if (reader.ValueTextEquals(apiKindPropertyName))
        {
            return PropertyKind.ApiKind;
        }

        if (reader.ValueTextEquals(apiNamePropertyName))
        {
            return PropertyKind.ApiName;
        }

        if (reader.ValueTextEquals(clrTypePropertyName))
        {
            return PropertyKind.ClrType;
        }

        return PropertyKind.Other;
    }
    #endregion
}
