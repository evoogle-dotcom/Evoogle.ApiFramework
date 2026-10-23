// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework;

/// <summary>
///     Overrides the API name or nullability for a CLR property or field.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ApiPropertyAttribute : ApiNamedElementAttribute
{
    #region Properties
    /// <summary>
    ///     Gets the the API name for the property.
    ///     When <c>null</c>, the name is derived by the active naming convention.
    /// </summary>
    public new string? ApiName
    {
        get => base.ApiName;
        init => base.ApiName = value;
    }

    /// <summary>
    ///     When <c>true</c>, forces the property to be non-nullable regardless of CLR nullability.
    ///     Takes precedence over <see cref="IsNullable"/>.
    ///     When both <see cref="IsNonNullable"/> and <see cref="IsNullable"/> are <c>false</c>,
    ///     nullability is left to convention inference.
    /// </summary>
    public bool IsNonNullable { get; init; }

    /// <summary>
    ///     When <c>true</c>, forces the property to be nullable regardless of CLR nullability.
    ///     Ignored when <see cref="IsNonNullable"/> is also <c>true</c>.
    ///     When both <see cref="IsNonNullable"/> and <see cref="IsNullable"/> are <c>false</c>,
    ///     nullability is left to convention inference.
    /// </summary>
    public bool IsNullable { get; init; }
    #endregion
}
