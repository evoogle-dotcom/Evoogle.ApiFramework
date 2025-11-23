// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a type reference in an API schema. This can either be:
///     - An API inline type, such as a collection (<see cref="ApiInlineType"/> set).
///     - An API named reference to an API type (<see cref="Kind"/> and <see cref="ApiName"/> set), or
///     - A CLR type reference to an API type (<see cref="ClrType"/> set), or
/// </summary>
[JsonConverter(typeof(ApiTypeExpressionJsonConverter))]
public sealed class ApiTypeExpression
{
    #region Fields
    private ApiType? _apiResolvedType = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the kind of the referenced API type (only used for API named references).
    /// </summary>    
    public ApiTypeKind? Kind { get; }

    /// <summary>
    ///     Gets the API name of the referenced type (only used for API named references).
    /// </summary>
    public string? ApiName { get; }

    /// <summary>
    ///     Gets the CLR type of the referenced type (only used for CLR references).
    /// </summary>
    public Type? ClrType { get; }

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
    ///     Gets a value indicating whether this is either an API named reference or a CLR type reference (i.e., <see cref="ApiInlineType"/> is null).
    /// </summary>
    public bool IsReference => this.IsApiNamedReference || this.IsClrTypeReference;

    /// <summary>
    ///     Gets a value indicating whether this is an API named reference (i.e., <see cref="ApiInlineType"/> is null).
    /// </summary>
    public bool IsApiNamedReference => this.ApiInlineType is null && this.Kind is not null && !string.IsNullOrWhiteSpace(this.ApiName);

    /// <summary>
    ///     Gets a value indicating whether this is a CLR reference (i.e., <see cref="ApiInlineType"/> is null).
    /// </summary>
    public bool IsClrTypeReference => this.ApiInlineType is null && this.ClrType is not null;

    /// <summary>
    ///    Gets a value indicating whether this expression has been resolved to an API type.
    /// </summary>
    public bool IsResolved => _apiResolvedType is not null;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes an inline type expression (e.g., a collection declared in-place).
    /// </summary>
    /// <param name="apiInlineType">The API type instance used directly by this expression.</param>
    public ApiTypeExpression(ApiType apiInlineType) => this.ApiInlineType = apiInlineType;

    /// <summary>
    ///     Initializes an API named reference to a declared API type within a schema.
    /// </summary>
    /// <param name="kind">The expected kind of the referenced API type.</param>
    /// <param name="apiName">The name of the API type to be resolved in the schema.</param>
    public ApiTypeExpression(ApiTypeKind kind, string apiName)
    {
        this.Kind = kind;
        this.ApiName = apiName;
    }

    /// <summary>
    ///     Initializes a CLR typed reference to a declared API type within a schema.
    /// </summary>
    /// <param name="clrType">The CLR type to be resolved in the schema.</param>
    public ApiTypeExpression(Type clrType) => this.ClrType = TypeReflection.IsNullableType(clrType) ? Nullable.GetUnderlyingType(clrType) : clrType;

    /// <summary>
    ///     Initializes either an API named reference or a CLR typed reference to a declared API type within a schema.
    /// </summary>
    /// <param name="kind">The expected kind of the referenced API type.</param>
    /// <param name="apiName">The name of the API type to be resolved in the schema.</param>
    /// <param name="clrType">The CLR type to be resolved in the schema.</param>
    public ApiTypeExpression(ApiTypeKind? kind, string? apiName, Type? clrType)
    {
        this.Kind = kind;
        this.ApiName = apiName;
        this.ClrType = clrType;
    }
    #endregion

    #region Methods
    public static ApiTypeExpression ClrRef<T>() => new(typeof(T));

    public static ApiTypeExpression HashSetOf<T>(ApiTypeModifiers apiItemTypeModifiers)
        => new(new ApiCollectionType(ClrRef<T>(), apiItemTypeModifiers, typeof(HashSet<T>)));

    public static ApiTypeExpression ListOf<T>(ApiTypeModifiers apiItemTypeModifiers)
        => new(new ApiCollectionType(ClrRef<T>(), apiItemTypeModifiers, typeof(List<T>)));

    internal void Initialize(ApiSchema apiSchema, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        _apiResolvedType = null;

        // Try and resolve API type with inlined API type first if applicable
        if (this.IsInline)
        {
            this.InitializeApiTypeByInline(apiSchema, ref results);
            return;
        }

        // Try and resolve API type with API named reference or CLR type reference if applicable
        if (this.IsApiNamedReference)
        {
            this.InitializeApiTypeByApiNamedReference(apiSchema, apiValidationPath, ref results);
            return;
        }
        else if (this.IsClrTypeReference)
        {
            this.InitializeApiTypeByClrTypeReference(apiSchema, apiValidationPath, ref results);
            return;
        }

        // Unable to resolve API type with either an API named reference or a CLR type reference.
        results ??= [];
        results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiType)} is unresolved because neither {nameof(this.ApiInlineType)}, nor a valid combination of {nameof(this.Kind)} and {nameof(this.ApiName)}, nor {nameof(this.ClrType)} is set.", [nameof(this.ApiType)]));
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
        var clrType = this.ClrType.SafeToString();
        return $"{nameof(ApiTypeExpression)} {{{nameof(this.Kind)}={kind}, {nameof(this.ApiName)}={apiName}, {nameof(this.ClrType)}={clrType}}}";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiTypeByApiNamedReference(ApiSchema apiSchema, string apiValidationPath, ref List<ValidationResult>? results)
    {
        var kind = this.Kind!;
        var apiName = this.ApiName!;

        switch (kind)
        {
            case ApiTypeKind.Scalar:
                _apiResolvedType = apiSchema.TryGetScalarTypeByApiName(apiName, out var apiScalarType) ? apiScalarType : null;
                break;

            case ApiTypeKind.Enum:
                _apiResolvedType = apiSchema.TryGetEnumTypeByApiName(apiName, out var apiEnumType) ? apiEnumType : null;
                break;

            case ApiTypeKind.Object:
                _apiResolvedType = apiSchema.TryGetObjectTypeByApiName(apiName, out var apiObjectType) ? apiObjectType : null;
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

        if (_apiResolvedType is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiType)} is unresolved for {nameof(this.Kind)}={this.Kind.SafeToString()} and {nameof(this.ApiName)}={this.ApiName.SafeToString()}.", [nameof(this.ApiType)]));
        }
    }

    private void InitializeApiTypeByClrTypeReference(ApiSchema apiSchema, string apiValidationPath, ref List<ValidationResult>? results)
    {
        var clrType = this.ClrType!;
        _apiResolvedType = apiSchema.TryGetTypeByClrType(clrType, out var apiType) ? apiType : null;

        if (_apiResolvedType is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiType)} is unresolved for {nameof(this.ClrType)}={this.ClrType.SafeToName()}.", [nameof(this.ApiType)]));
        }
    }

    private void InitializeApiTypeByInline(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        if (this.ApiInlineType is ApiCollectionType collection)
        {
            collection.Initialize(apiSchema, apiSchema.Context, ref results);
        }

        _apiResolvedType = this.ApiInlineType;
    }
    #endregion
}
