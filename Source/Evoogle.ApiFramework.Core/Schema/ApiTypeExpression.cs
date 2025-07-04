// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a type reference in an API schema. This can either be:
///     - A named reference to an API type (<see cref="Kind"/> and <see cref="ApiName"/> set), or
///     - An inline type, such as a collection (<see cref="ApiInlineType"/> set).
/// </summary>
public sealed class ApiTypeExpression
{
    #region Fields
    private ApiType? _apiResolvedType = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the kind of the referenced API type (only used for named references).
    /// </summary>    
    public ApiTypeKind? Kind { get; }

    /// <summary>
    ///     Gets the API name of the referenced type (only used for named references).
    /// </summary>
    public string? ApiName { get; }

    /// <summary>
    ///     Gets the inline type definition, if any.
    ///     This is typically used for inline collection types.
    /// </summary>
    public ApiType? ApiInlineType { get; }

    /// <summary>
    ///     Gets the resolved <see cref="ApiType"/> this expression refers to, either inline or named.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if the expression has not been resolved yet using <see cref="Resolve"/>.
    /// </exception>
    public ApiType ApiResolvedType => _apiResolvedType ?? throw new ApiSchemaException($"{this} has not been resolved. Call '{nameof(Resolve)}' before accessing this property.");

    /// <summary>
    ///     Gets a value indicating whether this is an inline type (i.e., <see cref="ApiInlineType"/> is not null).
    /// </summary>
    public bool IsInline => this.ApiInlineType is not null;

    /// <summary>
    ///     Gets a value indicating whether this is a reference to a named type (i.e., <see cref="ApiInlineType"/> is null).
    /// </summary>
    public bool IsReference => this.ApiInlineType is null;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a named type reference.
    /// </summary>
    /// <param name="kind">The kind of the named API type.</param>
    /// <param name="apiName">The API name of the type.</param>    
    public ApiTypeExpression(ApiTypeKind kind, string apiName)
    {
        ArgumentNullException.ThrowIfNull(apiName);

        this.Kind = kind;
        this.ApiName = apiName;
    }

    /// <summary>
    ///     Initializes an inline type expression.
    /// </summary>
    /// <param name="apiInlineType">The inline API type.</param>
    public ApiTypeExpression(ApiType apiInlineType)
    {
        ArgumentNullException.ThrowIfNull(apiInlineType);

        this.ApiInlineType = apiInlineType;
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Resolves the API type expression to a concrete <see cref="ApiType"/> using the provided schema.
    ///     Adds a <see cref="ValidationResult"/> to <paramref name="results"/> if resolution fails.
    /// </summary>
    /// <param name="apiSchema">The schema to resolve from.</param>
    /// <param name="results">An optional list to which validation errors are appended.</param>    
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
            var message = $"Cannot resolve {this} because it is missing required properties.";
            results ??= [];
            results.Add(new ValidationResult(message, [nameof(this.ApiResolvedType)]));
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

            ApiTypeKind.Collection => throw new ApiSchemaException($"Cannot resolve {nameof(ApiType)} '{kind.SafeToString()}:{apiName.SafeToString()}' because it is an {nameof(ApiCollectionType)}. Use inline feature instead."),

            _ => null
        };

        // If we couldn't resolve the API type, we need to add a validation result.
        // This will help identify issues in the API schema.
        if (_apiResolvedType is null)
        {
            var message = $"Failed to resolve {nameof(ApiType)} '{kind.SafeToString()}:{apiName.SafeToString()}' from API schema.";

            results ??= [];
            results.Add(new ValidationResult(message, [nameof(this.ApiResolvedType)]));
        }
    }
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        if (this.IsInline)
        {
            var apiInlineType = this.ApiInlineType.SafeToString();
            return $"{nameof(ApiTypeExpression)} {{ApiInlineType={apiInlineType}}}";
        }

        var kind = this.Kind.SafeToString();
        var apiName = this.ApiName.SafeToString();
        return $"{nameof(ApiTypeExpression)} {{{nameof(this.Kind)}={kind}, {nameof(this.ApiName)}={apiName}}}";
    }
    #endregion
}
