// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Key;

/// <summary>
///     Convenience extension methods for <see cref="ApiKeyDefinitionBuilder"/>.
/// </summary>
public static class ApiKeyDefinitionBuilderExtensions
{
    /// <summary>
    ///     Adds a key definition extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The key definition builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiKeyDefinitionBuilder AddKeyExtension<TExtension>(this ApiKeyDefinitionBuilder builder, TExtension extension)
        where TExtension : class, IApiSchemaExtension
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddKeyExtension(typeof(TExtension), extension);
    }
}
