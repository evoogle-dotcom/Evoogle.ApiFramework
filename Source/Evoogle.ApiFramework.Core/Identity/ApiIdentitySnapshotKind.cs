// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies the kind of an identity snapshot value.
/// </summary>
public enum ApiIdentitySnapshotKind
{
    /// <summary>
    ///     A scalar identity value (leaf node containing an ApiId).
    /// </summary>
    Scalar,

    /// <summary>
    ///     A composite identity value (branch node containing nested parts).
    /// </summary>
    Composite
}
