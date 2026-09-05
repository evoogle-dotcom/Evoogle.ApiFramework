// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Key;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies how <see langword="null"/> property values are handled when walking an <see cref="ApiKeyPath"/>
///     during <see cref="ApiKeyType.MaterializeKey"/> materialization.
/// </summary>
/// <remarks>
///     This policy applies at every step of the path walk: both intermediate navigation properties
///     (those leading to the terminal property) and the terminal scalar property itself.
/// </remarks>
public enum ApiKeyNullHandling
{
    /// <summary>
    ///     When any property in the path — whether an intermediate navigation property or the terminal
    ///     scalar property — is <see langword="null"/>, emit <see cref="ApiKey.Empty"/> for that path and continue.
    /// </summary>
    UseDefaultOnNull = 0,

    /// <summary>
    ///     When any property in the path — whether an intermediate navigation property or the terminal
    ///     scalar property — is <see langword="null"/>, throw an <see cref="ApiKeyException"/> with
    ///     diagnostic details about the path and the offending property.
    /// </summary>
    ThrowOnNull = 1
}
