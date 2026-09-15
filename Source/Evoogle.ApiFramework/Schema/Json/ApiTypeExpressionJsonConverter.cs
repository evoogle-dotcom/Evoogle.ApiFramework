// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Immutable;
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
        var propertyNames = reader.ReadObjectPropertyNames(JsonReaderNullPropertyHandling.Ignore);

        if (IsEmptyExpressionObject(propertyNames))
        {
            // If there are no properties, it is considered an invalid expression object.
            reader.Skip();
            return;
        }

        var readContext = (ReadContext)context;
        var options = readContext.Options;

        var clrTypePropertyName = readContext.PropertyNames.ApiTypeExpression.ClrType;
        var apiKindPropertyName = readContext.PropertyNames.ApiTypeExpression.ApiKind;
        var apiNamePropertyName = readContext.PropertyNames.ApiTypeExpression.ApiName;

        if (IsReferenceObject(propertyNames, clrTypePropertyName, apiKindPropertyName, apiNamePropertyName))
        {
            // Handle the reference case here
            var apiTypeReference = JsonSerializer.Deserialize<ApiTypeReference>(ref reader, options);
            readContext.ReadData.ApiTypeReference = apiTypeReference;
        }
        else if (IsInlineObject(propertyNames))
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
    private static bool IsEmptyExpressionObject(ImmutableArray<string> propertyNames)
        => propertyNames.Length == 0;

    private static bool IsInlineObject(ImmutableArray<string> propertyNames)
        => propertyNames.Length > 2;

    private static bool IsReferenceObject
    (
        ImmutableArray<string> propertyNames,
        string clrTypePropertyName,
        string apiKindPropertyName,
        string apiNamePropertyName
    )
    {
        // Handle the two most common cases: reference by CLR type, or by API kind and name.
        if (IsClrTypeReference(propertyNames, clrTypePropertyName))
        {
            return true;
        }

        if (IsApiKindAndApiNameReference(propertyNames, apiKindPropertyName, apiNamePropertyName))
        {
            return true;
        }

        // Handle the malformed reference case here
        if (IsMalformedReference(propertyNames, clrTypePropertyName, apiKindPropertyName, apiNamePropertyName))
        {
            return true;
        }

        return false;
    }

    private static bool IsApiKindAndApiNameReference(ImmutableArray<string> propertyNames, string apiKindPropertyName, string apiNamePropertyName)
        => propertyNames.Length == 2 && ((propertyNames[0] == apiKindPropertyName && propertyNames[1] == apiNamePropertyName) || (propertyNames[0] == apiNamePropertyName && propertyNames[1] == apiKindPropertyName));

    private static bool IsClrTypeReference(ImmutableArray<string> propertyNames, string clrTypePropertyName)
        => propertyNames.Length == 1 && propertyNames[0] == clrTypePropertyName;

    private static bool IsMalformedReference(ImmutableArray<string> propertyNames, string clrTypePropertyName, string apiKindPropertyName, string apiNamePropertyName)
    {
        if (propertyNames.Length is not (1 or 2))
        {
            return false;
        }

        return propertyNames.Contains(apiKindPropertyName)
            || propertyNames.Contains(apiNamePropertyName)
            || propertyNames.Contains(clrTypePropertyName);
    }
    #endregion
}
