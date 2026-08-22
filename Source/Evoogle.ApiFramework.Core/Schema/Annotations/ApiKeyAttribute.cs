// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Declares a CLR property, field, or CLR-rooted path as part of a named key type
///     on an object type.
///     Apply this attribute once per key path; use <see cref="Order"/> to sequence the paths
///     within a composite key.
///     When applied to a property or field without <see cref="ClrPath"/>, the decorated member
///     supplies the single path segment.
///     Processed by <see cref="Configuration.ApiAttributeAnnotationReader"/> at
///     <see cref="Configuration.Internal.ApiConfigurationSource.DataAnnotation"/> precedence.
/// </summary>
/// <remarks>
///     Initializes a new <see cref="ApiKeyAttribute"/> contributing to the specified key name.
///     <para>
///         <see cref="ClrPath"/> contains dot-delimited CLR member names relative to
///         <see cref="ClrRootType"/>. When <see cref="ClrRootType"/> is not supplied, the enclosing
///         object type is used.
///     </para>
/// </remarks>
/// <param name="apiName">The API name of the key type. Defaults to <c>PrimaryKey</c>.</param>
/// <param name="order">The zero-based position of this path within a composite key.</param>
[AttributeUsage
(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Property |
    AttributeTargets.Field,
    AllowMultiple = true,
    Inherited = true
)]
public sealed class ApiKeyAttribute(string apiName = "PrimaryKey", int order = 0)
    : ApiNamedElementAttribute(RequireApiName(apiName))
{
    #region Properties
    /// <summary>
    ///     Gets or initializes the API name of the key type this path contributes to.
    /// </summary>
    public new string ApiName
    {
        get => base.ApiName!;
        init => base.ApiName = RequireApiName(value);
    }

    /// <summary>
    ///     Gets or initializes the zero-based order of this path within a composite key.
    /// </summary>
    public int Order { get; init; } = order;

    /// <summary>
    ///     Gets or initializes the CLR type from which <see cref="ClrPath"/> begins.
    ///     When <see langword="null"/>, the enclosing object CLR type is used.
    /// </summary>
    public Type? ClrRootType { get; init; }

    /// <summary>
    ///     Gets or initializes the dot-delimited CLR member path relative to
    ///     <see cref="ClrRootType"/>. A type-level annotation must provide this value.
    /// </summary>
    public string? ClrPath { get; init; }
    #endregion
}
