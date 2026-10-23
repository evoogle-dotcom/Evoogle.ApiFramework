// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Extension methods for <see cref="ApiCollectionType"/> class.
/// </summary>
public static class ApiCollectionTypeExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Determines whether the items in the <see cref="ApiCollectionType"/> are non-nullable.
    /// </summary>
    /// <param name="apiCollectionType">The <see cref="ApiCollectionType"/> instance.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="ApiCollectionType.ApiItemTypeModifiers"/> has the <see cref="ApiTypeModifiers.Required"/> flag set; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsItemNonNullable(this ApiCollectionType apiCollectionType)
        => apiCollectionType.ApiItemTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    /// <summary>
    ///     Determines whether the items in the <see cref="ApiCollectionType"/> are nullable.
    /// </summary>
    /// <param name="apiCollectionType">The <see cref="ApiCollectionType"/> instance.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="ApiCollectionType.ApiItemTypeModifiers"/> does not have the <see cref="ApiTypeModifiers.Required"/> flag set; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsItemNullable(this ApiCollectionType apiCollectionType)
        => !apiCollectionType.ApiItemTypeModifiers.HasFlag(ApiTypeModifiers.Required);
    #endregion
}
