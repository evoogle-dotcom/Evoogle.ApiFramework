// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Key;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies how optional <see cref="ApiKeyPart.ApiName"/> values are created during key materialization.
/// </summary>
public enum ApiKeyPartNameBuilder
{
    /// <summary>
    ///     Creates unnamed/positional key parts.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Creates names from only the dotted CLR property path.
    /// </summary>
    ClrPathOnly = 1,

    /// <summary>
    ///     Creates names from the CLR root type and dotted CLR property path.
    /// </summary>
    ClrRootAndPath = 2
}
