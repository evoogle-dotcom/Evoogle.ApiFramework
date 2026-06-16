// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;

using Evoogle.ApiFramework.Key;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for <see cref="ApiKeyMaterializationContext"/>.
/// </summary>
public static class ApiKeyMaterializationContextExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Registers an already-materialized <see cref="ApiKey"/> value for the specified root CLR type and CLR property path.
    /// </summary>
    /// <typeparam name="T">The root CLR type of the key path.</typeparam>
    /// <param name="context">The materialization context to configure.</param>
    /// <param name="clrPath">The full dotted CLR property path from the root type to the scalar property.</param>
    /// <param name="value">The materialized key value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public static ApiKeyMaterializationContext WithKey<T>(this ApiKeyMaterializationContext context, string clrPath, ApiKey value)
        => context.WithKey(typeof(T), clrPath, value);

    /// <summary>Registers an <see cref="int"/> key value without boxing.</summary>
    /// <typeparam name="T">The root CLR type of the key path.</typeparam>
    /// <param name="context">The materialization context to configure.</param>
    /// <param name="clrPath">The full dotted CLR property path from the root type to the scalar property.</param>
    /// <param name="value">The scalar key value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public static ApiKeyMaterializationContext WithKey<T>(this ApiKeyMaterializationContext context, string clrPath, int value)
        => context.WithKey<T>(clrPath, ApiKey.FromInt32(value));

    /// <summary>Registers a <see cref="long"/> key value without boxing.</summary>
    /// <typeparam name="T">The root CLR type of the key path.</typeparam>
    /// <param name="context">The materialization context to configure.</param>
    /// <param name="clrPath">The full dotted CLR property path from the root type to the scalar property.</param>
    /// <param name="value">The scalar key value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public static ApiKeyMaterializationContext WithKey<T>(this ApiKeyMaterializationContext context, string clrPath, long value)
        => context.WithKey<T>(clrPath, ApiKey.FromInt64(value));

    /// <summary>Registers a <see cref="Guid"/> key value without boxing.</summary>
    /// <typeparam name="T">The root CLR type of the key path.</typeparam>
    /// <param name="context">The materialization context to configure.</param>
    /// <param name="clrPath">The full dotted CLR property path from the root type to the scalar property.</param>
    /// <param name="value">The scalar key value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public static ApiKeyMaterializationContext WithKey<T>(this ApiKeyMaterializationContext context, string clrPath, Guid value)
        => context.WithKey<T>(clrPath, ApiKey.FromGuid(value));

    /// <summary>Registers an <see cref="Ulid"/> key value without boxing.</summary>
    /// <typeparam name="T">The root CLR type of the key path.</typeparam>
    /// <param name="context">The materialization context to configure.</param>
    /// <param name="clrPath">The full dotted CLR property path from the root type to the scalar property.</param>
    /// <param name="value">The scalar key value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public static ApiKeyMaterializationContext WithKey<T>(this ApiKeyMaterializationContext context, string clrPath, Ulid value)
        => context.WithKey<T>(clrPath, ApiKey.FromUlid(value));

    /// <summary>Registers a <see cref="CultureInfo"/> key value.</summary>
    /// <typeparam name="T">The root CLR type of the key path.</typeparam>
    /// <param name="context">The materialization context to configure.</param>
    /// <param name="clrPath">The full dotted CLR property path from the root type to the scalar property.</param>
    /// <param name="value">The culture key value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public static ApiKeyMaterializationContext WithKey<T>(this ApiKeyMaterializationContext context, string clrPath, CultureInfo value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return context.WithKey<T>(clrPath, ApiKey.FromCulture(value));
    }

    /// <summary>
    ///     Registers raw text for the specified root CLR type and CLR property path.
    ///     The text is parsed according to schema metadata during value-based materialization.
    /// </summary>
    /// <typeparam name="T">The root CLR type of the key path.</typeparam>
    /// <param name="context">The materialization context to configure.</param>
    /// <param name="clrPath">The full dotted CLR property path from the root type to the scalar property.</param>
    /// <param name="value">The raw text value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public static ApiKeyMaterializationContext WithText<T>(this ApiKeyMaterializationContext context, string clrPath, string? value)
        => context.WithText(typeof(T), clrPath, value);
    #endregion
}
