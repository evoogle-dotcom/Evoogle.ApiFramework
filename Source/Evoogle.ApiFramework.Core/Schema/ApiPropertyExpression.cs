// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiPropertyExpression
{
    #region Fields
    private ApiProperty? _apiResolvedProperty = null;
    #endregion

    #region Properties
    public string? ApiName { get; }

    public ApiProperty? ApiInlineProperty { get; }

    public ApiProperty ApiResolvedProperty => _apiResolvedProperty ?? throw new ApiSchemaException($"The API property expression '{this}' has not been resolved. Call Resolve(apiObjectType) before accessing this property.");

    public bool IsInline => this.ApiInlineProperty is not null;

    public bool IsReference => this.ApiInlineProperty is null;
    #endregion

    #region Constructors
    public ApiPropertyExpression(string apiName)
    {
        ArgumentNullException.ThrowIfNull(apiName);

        this.ApiName = apiName;
    }

    public ApiPropertyExpression(ApiProperty apiInlineProperty)
    {
        ArgumentNullException.ThrowIfNull(apiInlineProperty);

        this.ApiInlineProperty = apiInlineProperty;
    }
    #endregion

    #region Methods
    public void Resolve(ApiObjectType apiObjectType, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiObjectType);

        // If this is an inline property, we can resolve it directly.
        if (this.IsInline)
        {
            _apiResolvedProperty = this.ApiInlineProperty;
            return;
        }

        // This is a named API property, we need to resolve it from the parent API object type.
        // If ApiName is null, we cannot resolve it.
        if (this.ApiName is null)
        {
            var message = $"Cannot resolve API property expression '{this}' because it is missing required properties.";

            results ??= [];
            results.Add(new ValidationResult(message, [nameof(this.ApiResolvedProperty)]));
            return;
        }

        // Resolve the named API property by API name.
        var apiName = this.ApiName!;
        if (!apiObjectType.TryGetPropertyByApiName(apiName, out var apiProperty))
        {
            var message = $"Failed to resolve API property '{apiName}' from {apiObjectType.SafeToString()}.";

            results ??= [];
            results.Add(new ValidationResult(message, [nameof(this.ApiResolvedProperty)]));
            return;
        }

        // If we found the property, we can resolve it.
        _apiResolvedProperty = apiProperty;
    }
    #endregion

    #region Object Methods
    public override string ToString()
    {
        return this.IsInline
            ? $"Inline({this.ApiInlineProperty.SafeToString()})"
            : $"{this.ApiName.SafeToString()}";
    }
    #endregion
}
