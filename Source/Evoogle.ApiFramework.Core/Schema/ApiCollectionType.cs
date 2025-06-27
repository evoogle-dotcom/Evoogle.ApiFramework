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
///     Represents metadata of an API collection type, which describes a collection of items of a specified API type.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiCollectionType"/> class.
/// </remarks>
/// <param name="apiItemTypeExpression">The API type expression of the items in the collection.</param>
/// <param name="apiItemTypeModifiers">Modifiers applied to the item type (e.g., Required).</param>
/// <param name="clrCollectionType">The CLR type representing the collection type (e.g., List&lt;T&gt;).</param>
/// <exception cref="ArgumentNullException">Thrown if <paramref name="apiItemTypeExpression"/> is null.</exception>
public sealed class ApiCollectionType(ApiTypeExpression apiItemTypeExpression, ApiTypeModifiers apiItemTypeModifiers, Type clrCollectionType) : ApiType(clrCollectionType)
{
    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind Kind => ApiTypeKind.Collection;
    #endregion

    #region ApiObject Properties
    /// <summary>
    ///     Gets the API type expression to the API item type of this collection.
    ///     May point to a named type or inline type (e.g., collection).
    /// </summary>
    public ApiTypeExpression ApiItemTypeExpression { get; } = apiItemTypeExpression ?? throw new ArgumentNullException(nameof(apiItemTypeExpression));

    /// <summary>Gets the modifiers applied to the item type within the collection (e.g., Required).</summary>
    public ApiTypeModifiers ApiItemTypeModifiers { get; } = apiItemTypeModifiers;

    /// <summary>
    ///     Gets the resolved API item type (named or inline) associated with this collection.
    /// </summary>
    public ApiType ApiItemType => this.ApiItemTypeExpression.ApiResolvedType ?? throw new ApiSchemaException($"{nameof(this.ApiItemTypeExpression)} has not been resolved yet.");
    #endregion

    #region ApiCollectionType Methods
    /// <summary>
    ///     Resolves the API named type (or inline type) from the provided schema.
    /// </summary>
    /// <param name="apiSchema">The API schema to resolve the type from.</param>
    public void Resolve(ApiSchema apiSchema, ref List<ValidationResult>? results) => this.ApiItemTypeExpression.Resolve(apiSchema, ref results);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiItemTypeExpression = this.ApiItemTypeExpression.SafeToString();
        var apiItemTypeModifiers = this.ApiItemTypeModifiers.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiCollectionType)} {{{nameof(this.ApiItemTypeExpression)}={apiItemTypeExpression}, {nameof(this.ApiItemTypeModifiers)}={apiItemTypeModifiers}}} [{clrType}]";
    }
    #endregion
}
