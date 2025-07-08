// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for <see cref="ApiProperty"/> class.
/// </summary>
public static class ApiPropertyExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Determines whether the <see cref="ApiProperty"/> is marked as required.
    /// </summary>
    /// <param name="apiProperty">The <see cref="ApiProperty"/> instance.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="ApiProperty.ApiTypeModifiers"/> has the <see cref="ApiTypeModifiers.Required"/> flag set; otherwise, <c>false</c>.
    /// </returns>    
    public static bool IsRequired(this ApiProperty apiProperty) => apiProperty.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    /// <summary>
    ///     Determines whether the <see cref="ApiProperty"/> is marked as optional.
    /// </summary>
    /// <param name="apiProperty">The <see cref="ApiProperty"/> instance.</param>
    /// <returns>
    ///     <c>true</c> if the <see cref="ApiProperty.ApiTypeModifiers"/> does not have the <see cref="ApiTypeModifiers.Required"/> flag set; otherwise, <c>false</c>.
    /// </returns
    public static bool IsOptional(this ApiProperty apiProperty) => !apiProperty.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);
    #endregion
}