// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies the kinds of identifiers that can be used in the API framework.
/// </summary>
/// <remarks>
///     The <see cref="ApiIdKind"/> enum distinguishes scalar identifier representations (e.g., <see cref="int"/>, <see cref="Guid"/>, <see cref="string"/>)
///     from the <see cref="Composite"/> kind which models ordered or named collections of component identifiers.
/// </remarks>
public enum ApiIdKind : byte
{
    #region Values
    /// <summary>No value present (empty identifier).</summary>
    None,

    /// <summary>A composite identifier composed of one or more component <see cref="ApiId"/> values.</summary>
    Composite,

    /// <summary>A <see cref="System.Globalization.CultureInfo"/> identifier (by culture name).</summary>
    Culture,

    /// <summary>A <see cref="System.Guid"/> identifier.</summary>
    Guid,

    /// <summary>An <see cref="int"/> identifier.</summary>
    Int32,

    /// <summary>An <see cref="long"/> identifier.</summary>
    Int64,

    /// <summary>A <see cref="string"/> identifier.</summary>
    String,

    /// <summary>A <see cref="Ulid"/> identifier.</summary>
    Ulid,
    #endregion
}
