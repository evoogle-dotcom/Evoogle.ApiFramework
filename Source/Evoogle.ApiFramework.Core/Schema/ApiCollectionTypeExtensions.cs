// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for <see cref="ApiCollectionType"/> class.
/// </summary>
public static class ApiCollectionTypeExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Determines whether the items in the <see cref="ApiCollectionType"/> are required.
    /// </summary>
    /// <param name="apiCollectionType">The <see cref="ApiCollectionType"/> instance.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="ApiCollectionType.ApiItemTypeModifiers"/> has the <see cref="ApiTypeModifiers.Required"/> flag set; otherwise, <c>false</c>.
    /// </returns>    
    public static bool IsItemRequired(this ApiCollectionType apiCollectionType) => apiCollectionType.ApiItemTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    /// <summary>
    ///     Determines whether the items in the <see cref="ApiCollectionType"/> are optional.
    /// </summary>
    /// <param name="apiCollectionType">The <see cref="ApiCollectionType"/> instance.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="ApiCollectionType.ApiItemTypeModifiers"/> does not have the <see cref="ApiTypeModifiers.Required"/> flag set; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsItemOptional(this ApiCollectionType apiCollectionType) => !apiCollectionType.ApiItemTypeModifiers.HasFlag(ApiTypeModifiers.Required);
    #endregion
}