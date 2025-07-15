// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
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
    ///     Gets the resolved <see cref="ApiType"/> this expression refers to, either inline or named reference.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if the expression has not been resolved yet.
    /// </exception>
    public ApiType ApiType => this.ThrowIfNotInitialized(_apiResolvedType);

    /// <summary>
    ///     Gets a value indicating whether this is an inline type (i.e., <see cref="ApiInlineType"/> is not null).
    /// </summary>
    public bool IsInline => this.ApiInlineType is not null;

    /// <summary>
    ///     Gets a value indicating whether this is a reference to a named type (i.e., <see cref="ApiInlineType"/> is null).
    /// </summary>
    public bool IsReference => this.ApiInlineType is null;

    /// <summary>
    ///    Gets a value indicating whether this expression has been resolved to an API type.
    /// </summary>
    public bool IsResolved => _apiResolvedType is not null;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a named reference to a declared API type within a schema.
    /// </summary>
    /// <param name="kind">The expected kind of the referenced API type.</param>
    /// <param name="apiName">The name of the API type to be resolved in the schema.</param>
    public ApiTypeExpression(ApiTypeKind kind, string apiName)
    {
        this.Kind = kind;
        this.ApiName = apiName;
    }

    /// <summary>
    ///     Initializes an inline type expression (e.g., a collection declared in-place).
    /// </summary>
    /// <param name="apiInlineType">The API type instance used directly by this expression.</param>
    public ApiTypeExpression(ApiType apiInlineType) => this.ApiInlineType = apiInlineType;
    #endregion

    #region Methods
    internal void Initialize(ApiSchema apiSchema, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        _apiResolvedType = null;

        // Try and resolve API type with inlined API type first if applicable
        this.InitializeApiTypeByInline(apiSchema, ref results);

        if (_apiResolvedType is not null)
            return;

        // Try and resolve API type with referenced kind and API name
        var tryToResolve = true;

        this.InitializeKind(apiSchema, apiValidationPath, ref tryToResolve, ref results);
        this.InitializeApiName(apiSchema, apiValidationPath, ref tryToResolve, ref results);
        this.InitializeApiTypeByReference(apiSchema, apiValidationPath, tryToResolve, ref results);
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

    #region Implementation Methods
    private void InitializeKind(ApiSchema _, string apiValidationPath, ref bool tryToResolve, ref List<ValidationResult>? results)
    {
        if (this.Kind is null)
        {
            tryToResolve = false;

            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.Kind)} cannot be null.", [nameof(this.Kind)]));
            return;
        }

        if (this.Kind == ApiTypeKind.Unknown)
        {
            tryToResolve = false;

            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.Kind)} cannot be equal to {ApiTypeKind.Unknown}.", [nameof(this.Kind)]));
        }
    }

    private void InitializeApiName(ApiSchema _, string apiValidationPath, ref bool tryToResolve, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            tryToResolve = false;

            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    private void InitializeApiTypeByInline(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        if (this.ApiInlineType is not null)
        {
            if (this.ApiInlineType is ApiCollectionType collection)
                collection.Initialize(apiSchema, ref results);

            _apiResolvedType = this.ApiInlineType;
        }
    }

    private void InitializeApiTypeByReference(ApiSchema apiSchema, string apiValidationPath, bool tryToResolve, ref List<ValidationResult>? results)
    {
        if (tryToResolve)
        {
            var kind = this.Kind!;
            var apiName = this.ApiName!;

            switch (kind)
            {
                case ApiTypeKind.Scalar:
                    _apiResolvedType = apiSchema.TryGetApiScalarType(apiName, out var apiScalarType) ? apiScalarType : null;
                    break;

                case ApiTypeKind.Enum:
                    _apiResolvedType = apiSchema.TryGetApiEnumType(apiName, out var apiEnumType) ? apiEnumType : null;
                    break;

                case ApiTypeKind.Object:
                    _apiResolvedType = apiSchema.TryGetApiObjectType(apiName, out var apiObjectType) ? apiObjectType : null;
                    break;

                case ApiTypeKind.Collection:
                    {
                        results ??= [];
                        results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.Kind)} is set to {nameof(ApiTypeKind.Collection)} which is invalid.", [nameof(this.Kind)]));
                        break;
                    }

                default:
                    break;
            }
        }

        if (_apiResolvedType is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiType)} is unresolved for {nameof(this.Kind)}={this.Kind.SafeToString()} and {nameof(this.ApiName)}={this.ApiName.SafeToString()}.", [nameof(this.ApiType)]));
        }
    }
    #endregion
}
