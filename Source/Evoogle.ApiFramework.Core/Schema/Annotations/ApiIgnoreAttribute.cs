// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Excludes a CLR property or field from the API schema.
///     When present, the built-in property discovery convention skips the member and
///     <see cref="Configuration.ApiAttributeAnnotationReader"/> removes any property builder already
///     registered for it.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ApiIgnoreAttribute : Attribute
{
}
