// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework;

/// <summary>
///     Declares the version metadata for an API object type.
/// </summary>
/// <remarks>
///     Apply the attribute to a property or field for a property-backed version. Apply it to a
///     class or struct and provide <see cref="ClrType"/> for a repository-backed version.
///     This attribute designates version metadata only; it does not register scalar types, add API
///     properties, or make properties required.
/// </remarks>
[AttributeUsage
(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Property |
    AttributeTargets.Field,
    AllowMultiple = false,
    Inherited = true
)]
public sealed class ApiVersionAttribute : Attribute
{
    #region Properties
    /// <summary>
    ///     Gets the exact CLR type of a repository-supplied version value. A type-level annotation
    ///     must provide this value. A member-level annotation must omit it because the member and
    ///     its scalar API property determine the version definition.
    /// </summary>
    public Type? ClrType { get; init; }
    #endregion
}
