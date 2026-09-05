// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Identifies the kind of structural registration represented by a trace event.
/// </summary>
public enum ApiSchemaBuildRegistrationKind
{
    /// <summary>Reports a type registration.</summary>
    Type = 0,

    /// <summary>Reports an object property registration.</summary>
    Property = 1,

    /// <summary>Reports an enum value registration.</summary>
    EnumValue = 2,

    /// <summary>Reports a key type registration.</summary>
    KeyType = 3,

    /// <summary>Reports a relationship registration.</summary>
    Relationship = 4,
}
