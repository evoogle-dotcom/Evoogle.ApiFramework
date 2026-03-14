// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies whether an <see cref="ApiIdentityPartValue"/> holds a scalar primitive or a nested object identity.
/// </summary>
public enum ApiIdentityPartValueKind
{
    /// <summary>The part value is a scalar <see cref="ApiId"/> primitive (e.g., string, int, Guid).</summary>
    Scalar,

    /// <summary>The part value is a nested <see cref="ApiIdentityValue"/> representing a composite child identity.</summary>
    Object
}
