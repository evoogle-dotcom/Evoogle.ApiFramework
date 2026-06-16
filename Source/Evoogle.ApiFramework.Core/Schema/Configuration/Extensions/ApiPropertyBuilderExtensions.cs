// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Convenience extension methods for <see cref="ApiPropertyBuilder"/>.
/// </summary>
public static class ApiPropertyBuilderExtensions
{
    /// <summary>
    ///     Adds a property extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The property builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiPropertyBuilder AddPropertyExtension<TExtension>(this ApiPropertyBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddPropertyExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Marks this property as required, overriding any nullability-inferred modifier.
    /// </summary>
    /// <param name="builder">The property builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiPropertyBuilder AsRequired(this ApiPropertyBuilder builder)
        => builder.WithModifiers(m => m.Required());

    /// <summary>
    ///     Marks this property as optional, overriding any nullability-inferred modifier.
    /// </summary>
    /// <param name="builder">The property builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiPropertyBuilder AsOptional(this ApiPropertyBuilder builder)
        => builder.WithModifiers(m => m.Optional());
}
