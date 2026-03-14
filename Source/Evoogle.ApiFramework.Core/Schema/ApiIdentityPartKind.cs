// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies how an <see cref="ApiIdentityPart"/> sources its value within an <see cref="ApiIdentity"/> definition.
/// </summary>
public enum ApiIdentityPartKind
{
    /// <summary>The part sources its value directly from a scalar property on the declaring object type.</summary>
    Scalar,

    /// <summary>The part sources its value from the identity of a nested object property.</summary>
    Nested,

    /// <summary>The part sources its value from the identity of the parent object in a parent-child relationship.</summary>
    Parent
}
