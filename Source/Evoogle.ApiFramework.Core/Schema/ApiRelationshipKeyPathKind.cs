// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies the kind of an <see cref="ApiRelationshipKeyPath"/>, which determines how the key path is resolved to locate principal identity scalar values for relationship binding.
/// </summary>
public enum ApiRelationshipKeyPathKind
{
    #region Values
    /// <summary>
    ///     A key path that maps a single principal identity scalar leaf directly to a scalar property
    ///     on the dependent object type at the current navigation depth.
    /// </summary>
    Scalar,

    /// <summary>
    ///     A key path that navigates into a nested object property on the current object type
    ///     before resolving FK scalar values deeper in the object graph.
    /// </summary>
    Nested,

    /// <summary>
    ///     A key path that navigates to the owning object type of the dependent object to locate
    ///     FK scalar values for the principal identity. Resolved in a deferred schema-wide second pass.
    /// </summary>
    Owner
    #endregion
}
