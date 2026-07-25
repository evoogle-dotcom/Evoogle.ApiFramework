// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Declares a CLR property or field as part of a named key type on its enclosing object type.
///     Apply this attribute once per key path; use <see cref="Order"/> to sequence the paths
///     within a composite key.
///     Processed by <see cref="Configuration.ApiAttributeAnnotationReader"/> at
///     <see cref="Configuration.Internal.ApiConfigurationSource.DataAnnotation"/> precedence.
/// </summary>
/// <remarks>
///     Initializes a new <see cref="ApiKeyAttribute"/> contributing to the specified key name.
/// </remarks>
/// <param name="keyName">The API name of the key type. Defaults to <c>PrimaryKey</c>.</param>
/// <param name="order">The zero-based position of this path within a composite key.</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public sealed class ApiKeyAttribute(string keyName = "PrimaryKey", int order = 0) : Attribute
{
    #region Properties
    /// <summary>Gets the API name of the key type this path contributes to.</summary>
    public string KeyName { get; } = keyName;

    /// <summary>Gets the zero-based order of this path within a composite key.</summary>
    public int Order { get; } = order;
    #endregion
}
