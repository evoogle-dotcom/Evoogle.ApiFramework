// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides helper methods for adding strongly typed extensions to
///     <see cref="ExtensionBuilder{TBuilder}"/> instances.
/// </summary>
public static class ExtensionBuilderExtensions
{
    #region Methods
    /// <summary>
    ///     Adds an extension value keyed by its type and returns the original builder for chaining.
    /// </summary>
    /// <typeparam name="TBuilder">The concrete builder type.</typeparam>
    /// <typeparam name="T">The extension value type.</typeparam>
    /// <param name="builder">The builder to attach the extension to.</param>
    /// <param name="value">The extension value.</param>
    /// <returns>The same <typeparamref name="TBuilder"/> instance to support fluent calls.</returns>
    public static TBuilder AddExtension<TBuilder, T>(this ExtensionBuilder<TBuilder> builder, T value)
        where TBuilder : ExtensionBuilder<TBuilder>
        where T : notnull => builder.AddExtension(typeof(T), value);
    #endregion
}

