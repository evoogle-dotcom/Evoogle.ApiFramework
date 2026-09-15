// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Represents exactly one inline API type definition or one reference to a declared API type.
/// </summary>
[JsonConverter(typeof(ApiTypeExpressionJsonConverter))]
public sealed class ApiTypeExpression
{
    #region Fields
    private ApiType? _apiResolvedType;
    #endregion

    #region Properties
    /// <summary>Gets the resolved API type after schema compilation.</summary>
    /// <exception cref="ApiSchemaException">Thrown before the expression has resolved.</exception>
    public ApiType ApiType => this.RequireValue(_apiResolvedType);

    internal ApiType? ApiInlineType { get; }
    internal ApiTypeReference? ApiTypeReference { get; }
    #endregion

    #region Computed Properties
    /// <summary>Gets a value indicating whether this expression contains an inline definition.</summary>
    public bool IsInline => this.ApiInlineType is not null && this.ApiTypeReference is null;

    /// <summary>Gets a value indicating whether this expression contains a reference.</summary>
    public bool IsReference => this.ApiInlineType is null && this.ApiTypeReference is not null;

    /// <summary>Gets a value indicating whether this expression has resolved to an API type.</summary>
    public bool IsResolved => _apiResolvedType is not null;
    #endregion

    #region Constructors
    /// <summary>Creates an expression that owns an inline API type definition.</summary>
    /// <param name="apiInlineType">The inline API type definition.</param>
    public ApiTypeExpression(ApiType apiInlineType)
        : this(apiInlineType, null)
    {
    }

    /// <summary>Creates an expression containing a reference to a declared API type.</summary>
    /// <param name="apiTypeReference">The declared API type reference.</param>
    public ApiTypeExpression(ApiTypeReference apiTypeReference)
        : this(null, apiTypeReference)
    {
    }

    internal ApiTypeExpression(ApiType? apiInlineType, ApiTypeReference? apiTypeReference)
    {
        this.ApiInlineType = apiInlineType;
        this.ApiTypeReference = apiTypeReference;
    }
    #endregion

    #region Resolution Methods
    internal void ResolveForCollection(ApiSchemaCompilationContext context) => this.Resolve
    (
        context,
        ApiSchemaCompilationCode.ApiCollectionTypeUnresolvedItemType,
        nameof(ApiCollectionType.ApiItemType)
    );

    internal void ResolveForProperty(ApiSchemaCompilationContext context) => this.Resolve
    (
        context,
        ApiSchemaCompilationCode.ApiPropertyUnresolvedType,
        nameof(ApiProperty.ApiType)
    );

    private void Resolve
    (
        ApiSchemaCompilationContext context,
        ApiSchemaCompilationCode parentUnresolvedCode,
        string parentUnresolvedName
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!this.IsInline && !this.IsReference)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiTypeExpressionInvalidForm;
            var description = $"A type expression must contain exactly one of {nameof(this.ApiInlineType)} or {nameof(this.ApiTypeReference)}";
            var remediation = $"Specify exactly one of {nameof(this.ApiInlineType)} or {nameof(this.ApiTypeReference)}";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        if (this.IsInline)
        {
            _apiResolvedType = this.ApiInlineType;
            this.ApiInlineType!.Compile(context);
            return;
        }

        _apiResolvedType = this.ApiTypeReference!.Resolve
        (
            context,
            parentUnresolvedCode,
            parentUnresolvedName
        );
    }
    #endregion

    #region Factory Methods
    /// <summary>Creates an expression for a hash-set collection of CLR items.</summary>
    /// <typeparam name="TClr">The CLR item type.</typeparam>
    /// <param name="apiItemTypeModifiers">Modifiers applied to each item.</param>
    /// <returns>A new inline collection expression.</returns>
    public static ApiTypeExpression HashSetOf<TClr>(ApiTypeModifiers apiItemTypeModifiers) => new
    (
        new ApiCollectionType
        (
            new ApiTypeExpression(ApiTypeReference.ClrRef<TClr>()),
            apiItemTypeModifiers,
            typeof(HashSet<TClr>)
        )
    );

    /// <summary>Creates an expression for a list collection of CLR items.</summary>
    /// <typeparam name="TClr">The CLR item type.</typeparam>
    /// <param name="apiItemTypeModifiers">Modifiers applied to each item.</param>
    /// <returns>A new inline collection expression.</returns>
    public static ApiTypeExpression ListOf<TClr>(ApiTypeModifiers apiItemTypeModifiers) => new
    (
        new ApiCollectionType
        (
            new ApiTypeExpression(ApiTypeReference.ClrRef<TClr>()),
            apiItemTypeModifiers,
            typeof(List<TClr>)
        )
    );
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        if (this.IsInline)
        {
            var apiInlineType = this.ApiInlineType.SafeToString();
            return $"{nameof(ApiTypeExpression)} {{{nameof(this.ApiInlineType)}={apiInlineType}}}";
        }

        if (this.IsReference)
        {
            var apiTypeReference = this.ApiTypeReference.SafeToString();
            return $"{nameof(ApiTypeExpression)} {{{nameof(this.ApiTypeReference)}={apiTypeReference}}}";
        }

        return $"{nameof(ApiTypeExpression)}";
    }
    #endregion
}
