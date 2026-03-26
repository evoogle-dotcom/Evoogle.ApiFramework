// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies how null or unresolved identity parts should be handled during identity value
///     building and flattening operations.
/// </summary>
public enum ApiIdentityNullHandling
{
    /// <summary>
    ///     When a property value is null or a part is unresolved, use <see cref="ApiId.Empty"/> as a
    ///     placeholder and continue.
    ///
    ///     Structure skeletons are preserved for unresolved object parts, allowing partial identity values
    ///     to be built and flattened.
    /// </summary>
    ReturnEmpty = 0,

    /// <summary>
    ///     When a property value is null or a part is unresolved, throw an <see cref="Exceptions.ApiIdentityException"/>
    ///     with diagnostic details about the identity, property, and object type.
    /// </summary>
    ThrowException = 1
}
