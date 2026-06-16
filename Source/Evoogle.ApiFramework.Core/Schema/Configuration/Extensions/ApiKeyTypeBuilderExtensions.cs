// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Convenience extension methods for <see cref="ApiKeyTypeBuilder"/>.
/// </summary>
public static class ApiKeyTypeBuilderExtensions
{
    /// <summary>
    ///     Adds a key type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The key type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiKeyTypeBuilder AddKeyTypeExtension<TExtension>(this ApiKeyTypeBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddKeyTypeExtension(typeof(TExtension), extension);
}
