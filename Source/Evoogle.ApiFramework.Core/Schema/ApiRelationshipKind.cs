// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Identifies the structural kind of an <see cref="ApiRelationship"/>.
///     Used as the JSON discriminator for polymorphic relationship serialization.
/// </summary>
public enum ApiRelationshipKind
{
    #region Values
    /// <summary>
    ///     A one-to-one relationship: the principal value appears exactly once among principal objects
    ///     and the corresponding dependent value appears at most once among dependent objects.
    /// </summary>
    OneToOne,

    /// <summary>
    ///     A one-to-many relationship: the principal value appears exactly once among principal objects
    ///     and the corresponding dependent value may appear zero or more times among dependent objects.
    /// </summary>
    OneToMany,

    /// <summary>
    ///     A many-to-many relationship: modeled through an explicit association <see cref="ApiObjectType"/>
    ///     that holds key values referencing both sides.
    /// </summary>
    ManyToMany
    #endregion
}
