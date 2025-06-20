// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents either a reference to a named API type (kind and name will be non-null) or an inline API type (such as a collection).
/// </summary>
public sealed class ApiTypeExpression
{
    #region Fields
    private ApiType? _apiResolvedType = null;
    #endregion

    #region Properties
    public ApiTypeKind? Kind { get; }

    public string? ApiName { get; }

    public ApiType? ApiInlineType { get; }

    public ApiType ApiResolvedType => _apiResolvedType ?? throw new ApiSchemaException($"The API type expression '{this}' has not been resolved. Call Resolve(apiSchema) before accessing this property.");

    public bool IsInline => this.ApiInlineType is not null;

    public bool IsReference => this.ApiInlineType is null;
    #endregion

    #region Constructors
    public ApiTypeExpression(ApiTypeKind kind, string apiName)
    {
        ArgumentNullException.ThrowIfNull(apiName);

        this.Kind = kind;
        this.ApiName = apiName;
    }

    public ApiTypeExpression(ApiType apiInlineType)
    {
        ArgumentNullException.ThrowIfNull(apiInlineType);

        this.ApiInlineType = apiInlineType;
    }
    #endregion

    #region Methods
    public void Resolve(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);

        // If this is an inline type, we can resolve it directly.
        // Inline types are typically collections or other complex types defined directly in the API schema.
        if (this.IsInline)
        {
            if (this.ApiInlineType is ApiCollectionType apiCollectionType)
                apiCollectionType.Resolve(apiSchema, ref results); // recursive

            _apiResolvedType = this.ApiInlineType;
            return;
        }

        // This is a named API type, we need to resolve it from the schema.
        // If Kind or ApiName is null, we cannot resolve it.
        if (this.Kind is null || this.ApiName is null)
        {
            var message = $"Cannot resolve API type expression '{this}' because it is missing required properties.";
            results ??= [];
            results.Add(new ValidationResult(message, [nameof(ApiResolvedType)]));
            return;
        }

        // Resolve the named API type based on its kind.
        // This will look up the type in the API schema based on its kind and name.
        var kind = this.Kind.Value;
        var apiName = this.ApiName!;
        _apiResolvedType = kind switch
        {
            ApiTypeKind.Scalar => apiSchema.TryGetApiScalarType(apiName, out var apiScalarType)
                ? apiScalarType
                : null,

            ApiTypeKind.Enum => apiSchema.TryGetApiEnumType(apiName, out var apiEnumType)
                ? apiEnumType
                : null,

            ApiTypeKind.Object => apiSchema.TryGetApiObjectType(apiName, out var apiObjectType)
                ? apiObjectType
                : null,

            ApiTypeKind.Collection => throw new ApiSchemaException("Named API references to API collection types are not supported. Use inline definition."),

            _ => null
        };

        // If we couldn't resolve the API type, we need to add a validation result.
        // This will help identify issues in the API schema.
        if (_apiResolvedType is null)
        {
            var message = $"Failed to resolve API type '{this.Kind}:{apiName}' from API schema.";

            results ??= [];
            results.Add(new ValidationResult(message, [nameof(ApiResolvedType)]));
        }
    }
    #endregion

    #region Object Methods
    public override string ToString()
    {
        return this.IsInline
            ? $"Inline({this.ApiInlineType.SafeToString()})"
            : $"{this.Kind.SafeToString()}:{this.ApiName.SafeToString()}";
    }
    #endregion
}
