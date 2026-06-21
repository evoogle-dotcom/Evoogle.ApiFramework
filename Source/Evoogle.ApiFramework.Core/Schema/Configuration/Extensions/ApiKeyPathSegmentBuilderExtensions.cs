// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Convenience extension methods for <see cref="ApiKeyPathSegmentBuilder"/>.
/// </summary>
public static class ApiKeyPathSegmentBuilderExtensions
{
    /// <summary>
    ///     Adds a key path segment extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The key path segment builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiKeyPathSegmentBuilder AddKeyPathSegmentExtension<TExtension>(this ApiKeyPathSegmentBuilder builder, TExtension extension)
        where TExtension : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddKeyPathSegmentExtension(typeof(TExtension), extension);
    }
}
