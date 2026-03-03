// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies how an <see cref="ApiIdentitySource"/> resolves its identity value.
/// </summary>
public enum ApiIdentitySourceKind
{
    /// <summary>
    ///     The source references a <see cref="ApiScalarType"/> and resolves directly to a scalar value from the referenced property.
    /// </summary>
    Scalar = 0,

    /// <summary>
    ///     The source references a nested <see cref="ApiObjectType"/> and delegates to one of that type's <see cref="ApiIdentity"/> definitions.
    /// </summary>
    Nested = 1
}
