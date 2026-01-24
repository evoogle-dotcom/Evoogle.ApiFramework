// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies the kind of identity part.
/// </summary>
public enum ApiIdentityPartKind
{
    /// <summary>
    ///     A scalar identity value (ApiId).
    /// </summary>
    Scalar,

    /// <summary>
    ///     A nested identity snapshot (ApiIdentitySnapshot).
    /// </summary>
    Nested,

    /// <summary>
    ///     An unresolved nested identity snapshot (ApiIdentitySnapshot).
    /// </summary>
    UnresolvedNested
}
