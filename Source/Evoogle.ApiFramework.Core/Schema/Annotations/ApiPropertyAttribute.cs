// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Overrides the API name or required/optional modifier for a CLR property or field.
///     Processed by <see cref="Configuration.ApiAttributeAnnotationReader"/> at
///     <see cref="Configuration.Internal.ApiConfigurationSource.DataAnnotation"/> precedence.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ApiPropertyAttribute : ApiNamedElementAttribute
{
    #region Properties
    /// <summary>
    ///     Gets or initializes the API name for the property.
    ///     When <c>null</c>, the name is derived by
    ///     the active naming convention.
    /// </summary>
    public new string? ApiName
    {
        get => base.ApiName;
        init => base.ApiName = value;
    }

    /// <summary>
    ///     When <c>true</c>, forces the property to be required regardless of CLR nullability.
    ///     Takes precedence over <see cref="IsOptional"/>.
    ///     When both <see cref="IsRequired"/> and <see cref="IsOptional"/> are <c>false</c>
    ///     the modifier is left to convention inference.
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    ///     When <c>true</c>, forces the property to be optional regardless of CLR nullability.
    ///     Ignored when <see cref="IsRequired"/> is also <c>true</c>.
    ///     When both <see cref="IsRequired"/> and <see cref="IsOptional"/> are <c>false</c>
    ///     the modifier is left to convention inference.
    /// </summary>
    public bool IsOptional { get; init; }
    #endregion
}
