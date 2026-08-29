// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Identifies the schema element kinds to which an API naming convention applies.
/// </summary>
[Flags]
public enum ApiNamingConventionTargets
{
    /// <summary>No schema element kinds.</summary>
    None = 0,

    /// <summary>API object type names.</summary>
    ObjectType = 1 << 0,

    /// <summary>API scalar type names.</summary>
    ScalarType = 1 << 1,

    /// <summary>API enumeration type names.</summary>
    EnumType = 1 << 2,

    /// <summary>API property names.</summary>
    Property = 1 << 3,

    /// <summary>API enumeration value names.</summary>
    EnumValue = 1 << 4,

    /// <summary>All supported schema element kinds.</summary>
    All = ObjectType | ScalarType | EnumType | Property | EnumValue,
}
