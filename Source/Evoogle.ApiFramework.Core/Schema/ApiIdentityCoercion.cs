// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines coercion rules for converting an API property value to an identity component.
/// </summary>
/// <remarks>
///     Coercion allows identity parts to be converted to specific types (e.g., string, GUID)
///     or transformed using a custom converter function.
/// </remarks>
/// <param name="targetKind">The target type kind for coercion, or <c>null</c> to use default coercion.</param>
/// <param name="converter">An optional custom converter function to transform the property value to an <see cref="ApiId"/>.</param>
public sealed class ApiIdentityCoercion(ApiIdentityTargetKind? targetKind = null, Func<object?, ApiId>? converter = null)
{
    /// <summary>
    ///     Gets the target type kind for coercion.
    /// </summary>
    public ApiIdentityTargetKind? TargetKind { get; } = targetKind;

    /// <summary>
    ///     Gets the custom converter function for transforming property values to <see cref="ApiId"/>.
    /// </summary>
    public Func<object?, ApiId>? Converter { get; } = converter;
}
