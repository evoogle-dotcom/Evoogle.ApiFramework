// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Extension methods for <see cref="ApiProperty"/> class.
/// </summary>
public static class ApiPropertyExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Determines whether the <see cref="ApiProperty"/> is non-nullable.
    /// </summary>
    /// <param name="apiProperty">The <see cref="ApiProperty"/> instance.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="ApiProperty.ApiNullability"/> is <see cref="ApiNullability.NonNullable"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNonNullable(this ApiProperty apiProperty)
        => apiProperty.ApiNullability == ApiNullability.NonNullable;

    /// <summary>
    ///     Determines whether the <see cref="ApiProperty"/> is nullable.
    /// </summary>
    /// <param name="apiProperty">The <see cref="ApiProperty"/> instance.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="ApiProperty.ApiNullability"/> is <see cref="ApiNullability.Nullable"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullable(this ApiProperty apiProperty)
        => apiProperty.ApiNullability == ApiNullability.Nullable;
    #endregion
}
