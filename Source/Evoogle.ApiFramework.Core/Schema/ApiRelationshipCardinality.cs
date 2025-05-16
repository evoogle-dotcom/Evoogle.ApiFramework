// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the relationship cardinality from an API object type to a related API object type.
/// </summary>
public enum ApiRelationshipCardinality
{
    #region Values
    /// <summary>Represents a to-one relationship.</summary>
    ToOne,

    /// <summary>Represents a to-many relationship.</summary>
    ToMany
    #endregion
}
