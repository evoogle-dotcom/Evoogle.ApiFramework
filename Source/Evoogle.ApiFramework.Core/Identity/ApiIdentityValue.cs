// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a structured snapshot of a resolved object identity, capturing the schema path and
///     all resolved part values at a single point in time.
/// </summary>
public class ApiIdentityValue
{
    #region Properties
    /// <summary>
    ///     Gets the schema path of the identity from which this value was resolved,
    ///     or <see langword="null"/> if the path is not available.
    /// </summary>
    public string? Path { get; }

    /// <summary>
    ///     Gets the ordered array of resolved part values that make up this identity snapshot,
    ///     or <see langword="null"/> if no parts were resolved.
    /// </summary>
    public ApiIdentityPartValue[]? Parts { get; }
    #endregion
}
