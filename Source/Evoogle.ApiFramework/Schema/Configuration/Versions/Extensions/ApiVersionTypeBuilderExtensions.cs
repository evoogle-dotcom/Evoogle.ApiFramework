// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Versions;

/// <summary>Convenience extension methods for <see cref="ApiVersionTypeBuilder"/>.</summary>
public static class ApiVersionTypeBuilderExtensions
{
    /// <summary>Adds version extension metadata keyed by its concrete type.</summary>
    /// <typeparam name="TExtension">The extension metadata type.</typeparam>
    /// <param name="builder">The version type builder.</param>
    /// <param name="extension">The extension metadata value.</param>
    /// <returns>The current builder.</returns>
    public static ApiVersionTypeBuilder AddVersionTypeExtension<TExtension>
    (
        this ApiVersionTypeBuilder builder,
        TExtension extension
    )
        where TExtension : class, IApiSchemaExtension
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddVersionTypeExtension(typeof(TExtension), extension);
    }
}
