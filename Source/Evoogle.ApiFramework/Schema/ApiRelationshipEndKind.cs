// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines the kind of an API relationship end, indicating whether it is the principal or dependent end of the relationship.
/// </summary>
public enum ApiRelationshipEndKind
{
    #region Values
    /// <summary>
    ///     Indicates that the relationship end is the principal end, which typically represents the primary entity in the relationship.
    /// </summary>
    Principal,

    /// <summary>
    ///     Indicates that the relationship end is the dependent end, which typically represents the secondary entity in the relationship that depends on the principal end.
    /// </summary>
    Dependent
    #endregion
}
