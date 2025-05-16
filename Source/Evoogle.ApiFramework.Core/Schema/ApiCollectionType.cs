// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API collection type, which describes a collection of items of a specified API type.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiCollectionType"/> class.
/// </remarks>
/// <param name="apiItemType">The API type of the items in the collection.</param>
/// <param name="apiItemTypeModifiers">Modifiers applied to the item type (e.g., Required).</param>
/// <param name="clrCollectionType">The CLR type representing the collection type (e.g., List&lt;T&gt;).</param>
/// <exception cref="ArgumentNullException">Thrown if <paramref name="apiItemType"/> is null.</exception>
public sealed class ApiCollectionType(ApiType apiItemType, ApiTypeModifiers apiItemTypeModifiers, Type clrCollectionType) : ApiType(clrCollectionType)
{
    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind Kind => ApiTypeKind.Collection;
    #endregion

    #region ApiObject Properties
    /// <summary>Gets the API type of the items contained within the collection.</summary>
    public ApiType ApiItemType { get; } = apiItemType ?? throw new ArgumentNullException(nameof(apiItemType));

    /// <summary>Gets the modifiers applied to the item type within the collection (e.g., Required).</summary>
    public ApiTypeModifiers ApiItemTypeModifiers { get; } = apiItemTypeModifiers;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiItemType = this.ApiItemType.SafeToString();
        var apiItemTypeModifiers = this.ApiItemTypeModifiers.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiCollectionType)} {{{nameof(ApiItemType)}={apiItemType}, {nameof(ApiItemTypeModifiers)}={apiItemTypeModifiers}}} [{clrType}]";
    }
    #endregion
}
