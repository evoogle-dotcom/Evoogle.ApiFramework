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
    ///     <c>true</c> if the <see cref="ApiCollectionType.ApiItemNullability"/> is <see cref="ApiNullability.NonNullable"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsItemNonNullable(this ApiCollectionType apiCollectionType)
        => apiCollectionType.ApiItemNullability == ApiNullability.NonNullable;

    /// <summary>
    ///     Determines whether the items in the <see cref="ApiCollectionType"/> are nullable.
    /// </summary>
    /// <param name="apiCollectionType">The <see cref="ApiCollectionType"/> instance.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="ApiCollectionType.ApiItemNullability"/> is <see cref="ApiNullability.Nullable"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsItemNullable(this ApiCollectionType apiCollectionType)
        => apiCollectionType.ApiItemNullability == ApiNullability.Nullable;
    #endregion
}
