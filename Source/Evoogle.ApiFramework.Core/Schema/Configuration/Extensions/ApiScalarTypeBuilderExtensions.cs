// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Convenience extension methods for <see cref="ApiScalarTypeBuilder"/>.
/// </summary>
public static class ApiScalarTypeBuilderExtensions
{
    /// <summary>
    ///     Adds a scalar type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The scalar type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiScalarTypeBuilder AddScalarTypeExtension<TExtension>(this ApiScalarTypeBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddScalarTypeExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds a scalar type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TScalar">The CLR scalar type represented by the builder.</typeparam>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The scalar type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiScalarTypeBuilder<TScalar> AddScalarTypeExtension<TScalar, TExtension>(this ApiScalarTypeBuilder<TScalar> builder, TExtension extension)
        where TExtension : class
        => builder.AddScalarTypeExtension(typeof(TExtension), extension);
}
