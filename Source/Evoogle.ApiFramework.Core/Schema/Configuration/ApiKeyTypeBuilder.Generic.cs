// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiKeyType"/> whose paths are rooted at
///     <typeparamref name="T"/>. Extends <see cref="ApiKeyTypeBuilder"/> with expression-based overloads so
///     CLR property names are extracted at compile time rather than supplied as raw strings.
/// </summary>
/// <typeparam name="T">The default root CLR type for key paths added via expression overloads.</typeparam>
public sealed class ApiKeyTypeBuilder<T>() : ApiKeyTypeBuilder()
{
    #region AddPath Methods
    /// <summary>
    ///     Adds a key path rooted at <typeparamref name="T"/> using a type-safe lambda expression.
    ///     The expression must consist only of chained member access (e.g. <c>x => x.Address.CityId</c>).
    /// </summary>
    /// <typeparam name="TScalar">The return type of the terminal scalar property.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar property, optionally through navigation properties.</param>
    /// <param name="configure">Optional callback to attach extensions to the key path builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expression"/> is not a simple member access chain.</exception>
    public ApiKeyTypeBuilder<T> AddPath<TScalar>
    (
        Expression<Func<T, TScalar>> expression,
        Action<ApiKeyPathBuilder<T>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(expression);

        var builder = ApiKeyPathBuilder<T>.For(expression);
        configure?.Invoke(builder);
        base.AddKeyPathBuilderCore(builder);
        return this;
    }

    /// <summary>
    ///     Adds a key path rooted at <typeparamref name="TRoot"/> using a type-safe lambda expression.
    ///     This overload allows callers to specify only the root type while the terminal value is boxed to
    ///     <see cref="object"/>.
    /// </summary>
    /// <typeparam name="TRoot">The CLR type from which the navigation begins.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar property on <typeparamref name="TRoot"/>, optionally through navigation properties.</param>
    /// <param name="configure">Optional callback to attach extensions to the key path builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expression"/> is not a simple member access chain.</exception>
    public ApiKeyTypeBuilder<T> AddPathFrom<TRoot>
    (
        Expression<Func<TRoot, object?>> expression,
        Action<ApiKeyPathBuilder<T>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(expression);

        var builder = ApiKeyPathBuilder<T>.For(expression);
        configure?.Invoke(builder);
        base.AddKeyPathBuilderCore(builder);
        return this;
    }
    #endregion
}
