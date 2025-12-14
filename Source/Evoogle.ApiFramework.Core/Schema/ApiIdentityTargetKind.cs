// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies the target type kind for identity coercion.
/// </summary>
public enum ApiIdentityTargetKind
{
    /// <summary>Coerce to a string value.</summary>
    String,

    /// <summary>Coerce to a 32-bit integer value.</summary>
    Int32,

    /// <summary>Coerce to a 64-bit integer value.</summary>
    Int64,

    /// <summary>Coerce to a GUID value.</summary>
    Guid,

    /// <summary>Coerce to a ULID value.</summary>
    Ulid,

    /// <summary>Coerce to a culture identifier value.</summary>
    Culture
}
