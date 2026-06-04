// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder used to configure a single <see cref="ApiKeyPath"/> using
///     compile-time-safe lambda expressions for property selection.
/// </summary>
/// <typeparam name="T">The root CLR type from which the key path navigation begins.</typeparam>
/// <remarks>
///     Initializes an <see cref="ApiKeyPathBuilder{T}"/> with the specified root CLR type and pre-configured
///     segment builders.
/// </remarks>
/// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
/// <param name="segmentBuilders">
///     Ordered <see cref="ApiKeyPathSegmentBuilder"/> instances from the root type to the terminal scalar property.
///     Must contain at least one builder.
/// </param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> or <paramref name="segmentBuilders"/> is <c>null</c>.</exception>
/// <exception cref="ArgumentException">Thrown when <paramref name="segmentBuilders"/> contains no elements.</exception>
public sealed class ApiKeyPathBuilder<T>(Type clrRootType, IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders) : ApiKeyPathBuilder(clrRootType, segmentBuilders)
{
    #region Factory Methods
    /// <summary>
    ///     Creates a builder for a path rooted at <typeparamref name="T"/> using a type-safe lambda expression.
    ///     The expression must consist only of chained member access (e.g. <c>x => x.Address.CityId</c>).
    /// </summary>
    /// <typeparam name="TScalar">The return type of the terminal scalar property.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar property, optionally through navigation properties.</param>
    /// <returns>A new <see cref="ApiKeyPathBuilder{T}"/> with <typeparamref name="T"/> as the root CLR type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expression"/> is not a simple member access chain.</exception>
    public static ApiKeyPathBuilder<T> For<TScalar>(Expression<Func<T, TScalar>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var names = StaticReflection.GetMemberPath(expression);
        var segmentBuilders = names.Select(n => new ApiKeyPathSegmentBuilder(n));
        return new ApiKeyPathBuilder<T>(typeof(T), segmentBuilders);
    }

    /// <summary>
    ///     Creates a builder for a path rooted at <typeparamref name="TRoot"/> using a type-safe lambda expression.
    ///     This overload allows callers to specify only the root type while the terminal value is boxed to
    ///     <see cref="object"/>.
    /// </summary>
    /// <typeparam name="TRoot">The CLR type from which the navigation begins.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar property on <typeparamref name="TRoot"/>, optionally through navigation properties.</param>
    /// <returns>A new <see cref="ApiKeyPathBuilder{T}"/> with <typeparamref name="TRoot"/> as the root CLR type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expression"/> is not a simple member access chain.</exception>
    public static ApiKeyPathBuilder<T> For<TRoot>(Expression<Func<TRoot, object?>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var names = StaticReflection.GetMemberPath(expression);
        var segmentBuilders = names.Select(n => new ApiKeyPathSegmentBuilder(n));
        return new ApiKeyPathBuilder<T>(typeof(TRoot), segmentBuilders);
    }
    #endregion
}
