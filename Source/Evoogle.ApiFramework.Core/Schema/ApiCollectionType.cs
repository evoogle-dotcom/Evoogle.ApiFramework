// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

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
public sealed class ApiCollectionType(ApiTypeExpression apiItemTypeExpression, ApiTypeModifiers apiItemTypeModifiers, Type clrCollectionType) : ApiType()
{
    #region ApiType Properties
    /// <inheritdoc />
    public override ApiTypeKind Kind => ApiTypeKind.Collection;

    /// <inheritdoc />
    public override Type ClrType { get; } = clrCollectionType;

    /// <inheritdoc/>
    protected override string ApiTypeName => nameof(ApiCollectionType);
    #endregion

    #region ApiCollectionType Properties
    /// <summary>
    ///     Gets the resolved API item type (named or inline) associated with this collection.
    /// </summary>
    public ApiType ApiItemType => this.ApiItemTypeExpression.ApiType;

    /// <summary>Gets the modifiers applied to the item type within the collection (e.g., Required).</summary>
    public ApiTypeModifiers ApiItemTypeModifiers { get; } = apiItemTypeModifiers;

    internal ApiTypeExpression ApiItemTypeExpression { get; } = apiItemTypeExpression;
    #endregion

    #region ApiType Methods
    /// <inheritdoc />
    protected override string GetValidationPath() => $"{this.ApiTypeName.SafeToString()}";

    internal override void Initialize(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);

        this.InitializeApiItemTypeExpression(apiSchema, ref results);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiItemTypeExpression = this.ApiItemTypeExpression.SafeToString();
        var apiItemTypeModifiers = this.ApiItemTypeModifiers.SafeToString();
        var clrType = this.ClrType.SafeToName();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiCollectionType)} {{{nameof(this.ApiItemTypeExpression)}={apiItemTypeExpression}, {nameof(this.ApiItemTypeModifiers)}={apiItemTypeModifiers}, {nameof(this.ExtensionCount)}={extensionCount}}} [{clrType}]";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiItemTypeExpression(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        if (this.ApiItemTypeExpression is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{this.GetValidationPath()}.{nameof(this.ApiItemTypeExpression)} cannot be null.", [nameof(this.ApiItemTypeExpression)]));
            return;
        }

        var apiChildValidationPath = $"{this.GetValidationPath()}.{nameof(this.ApiItemTypeExpression)}";
        this.ApiItemTypeExpression.Initialize(apiSchema, apiChildValidationPath, ref results);
    }
    #endregion
}
