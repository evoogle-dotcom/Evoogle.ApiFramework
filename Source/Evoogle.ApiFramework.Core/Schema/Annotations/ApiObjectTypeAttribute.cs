// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Marks a class or struct as an API object type and optionally overrides its API name.
///     Processed by <see cref="Configuration.ApiAttributeAnnotationReader"/> at
///     <see cref="Configuration.Internal.ApiConfigurationSource.DataAnnotation"/> precedence.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public sealed class ApiObjectTypeAttribute : Attribute
{
    /// <summary>Gets or sets the API name for the type. When <c>null</c> the CLR type name is used.</summary>
    public string? Name { get; set; }
}
