// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Key;

namespace Evoogle.ApiFramework.Schema.Key;

/// <summary>
///     Specifies how <see langword="null"/> property values are handled when walking an <see cref="ApiKeyPath"/>
///     during <see cref="ApiKeyDefinition.MaterializeKey"/> materialization.
/// </summary>
/// <remarks>
///     This policy applies at every step of the path walk: both intermediate navigation members
///     (those leading to the terminal member) and the terminal scalar member itself.
/// </remarks>
public enum ApiKeyNullHandling
{
    /// <summary>
    ///     When any member in the path — whether an intermediate navigation member or the terminal
    ///     scalar member — is <see langword="null"/>, emit <see cref="ApiKey.Empty"/> for that path and continue.
    /// </summary>
    UseDefaultOnNull = 0,

    /// <summary>
    ///     When any member in the path — whether an intermediate navigation member or the terminal
    ///     scalar member — is <see langword="null"/>, throw an <see cref="ApiKeyException"/> with
    ///     diagnostic details about the path and the offending property.
    /// </summary>
    ThrowOnNull = 1
}
