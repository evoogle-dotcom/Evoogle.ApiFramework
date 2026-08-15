// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Identifies the schema element targeted by a build trace event.
/// </summary>
public enum ApiSchemaBuildTargetKind
{
    /// <summary>Reports the schema builder itself.</summary>
    Schema = 0,

    /// <summary>Reports a scalar type builder.</summary>
    ScalarType = 1,

    /// <summary>Reports an enumeration type builder.</summary>
    EnumType = 2,

    /// <summary>Reports an enumeration value builder.</summary>
    EnumValue = 3,

    /// <summary>Reports an object type builder.</summary>
    ObjectType = 4,

    /// <summary>Reports an object property builder.</summary>
    Property = 5,

    /// <summary>Reports a key type builder.</summary>
    KeyType = 6,

    /// <summary>Reports a relationship builder.</summary>
    Relationship = 7,
}
