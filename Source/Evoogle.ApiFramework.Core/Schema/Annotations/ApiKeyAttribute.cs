// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Declares a CLR property, field, or CLR-rooted path as part of a named key type on an object type.
///     Apply this attribute once per key path; use <see cref="Order"/> to sequence the paths within a composite key.
///     When applied to a property or field without <see cref="ClrPath"/>, the decorated member supplies the single path segment.
/// </summary>
[AttributeUsage
(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Property |
    AttributeTargets.Field,
    AllowMultiple = true,
    Inherited = true
)]
public sealed class ApiKeyAttribute : ApiNamedElementAttribute
{
    #region Constructors
    /// <summary>
    ///     Creates a new <see cref="ApiKeyAttribute"/> with the default <c>PrimaryKey</c> API name.
    /// </summary>
    public ApiKeyAttribute()
    {
        this.ApiName = "PrimaryKey";
    }
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the the API name of the key type this path contributes to.
    /// </summary>
    public new string ApiName
    {
        get => base.ApiName!;
        init => base.ApiName = RequireApiName(value);
    }

    /// <summary>
    ///     Gets the the zero-based order of this path within a composite key.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    ///     Gets the the CLR type from which <see cref="ClrPath"/> begins.
    ///     When <see langword="null"/>, the enclosing object CLR type is used.
    /// </summary>
    public Type? ClrRootType { get; init; }

    /// <summary>
    ///     Gets the the dot-delimited CLR member path relative to <see cref="ClrRootType"/>.
    ///     A type-level annotation must provide this value.
    /// </summary>
    public string? ClrPath { get; init; }
    #endregion
}
