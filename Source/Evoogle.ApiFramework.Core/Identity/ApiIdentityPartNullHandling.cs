// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies how null or unresolved identity parts are handled during identity value building and flattening operations.
/// </summary>
public enum ApiIdentityPartNullHandling
{
    /// <summary>
    ///     When a property value is null or an identity part cannot be resolved, use the default identity value representation and continue.
    ///
    ///     For scalar parts, this uses <see cref="ApiId.Empty"/>.
    ///     For unresolved object parts, structure skeletons are preserved so partial identity values can still be built and flattened.
    /// </summary>
    UseDefaultOnNull = 0,

    /// <summary>
    ///     When a property value is null or an identity part cannot be resolved,
    ///     throw an <see cref="Exceptions.ApiIdentityException"/> with diagnostic details about the identity, property, and object type.
    /// </summary>
    ThrowOnNull = 1
}
