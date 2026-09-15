// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.ApiFramework.Schema.Key;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Key;

/// <summary>
///     Strongly-typed fluent builder used to configure a single <see cref="ApiKeyPath"/> using
///     compile-time-safe lambda expressions for property selection.
/// </summary>
/// <typeparam name="TRoot">The root CLR type from which the key path navigation begins.</typeparam>
/// <remarks>
///     Supports paths inferred from the enclosing schema context and rooted at
///     <typeparamref name="TRoot"/>, along with explicitly rooted paths for another CLR type.
/// </remarks>
public sealed class ApiKeyPathBuilder<TRoot> : ApiKeyPathBuilder
{
    #region Constructors
    /// <summary>Creates a key-path builder with an explicit CLR root type.</summary>
    /// <param name="clrRootType">The explicit CLR root type.</param>
    /// <param name="segmentBuilders">The ordered key-path segment builders.</param>
    public ApiKeyPathBuilder(Type clrRootType, IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders)
        : base(clrRootType, segmentBuilders)
    {
    }

    private ApiKeyPathBuilder(IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders)
        : base(segmentBuilders)
    {
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a builder for a path rooted at <typeparamref name="TRoot"/> using a type-safe lambda expression.
    ///     The expression must consist only of chained member access (e.g. <c>x => x.Address.CityId</c>).
    /// </summary>
    /// <typeparam name="TScalar">The return type of the terminal scalar member.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar member, optionally through navigation members.</param>
    /// <returns>A new <see cref="ApiKeyPathBuilder{TRoot}"/> with <typeparamref name="TRoot"/> as the root CLR type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expression"/> is not a simple member access chain.</exception>
    public static ApiKeyPathBuilder<TRoot> For<TScalar>(Expression<Func<TRoot, TScalar>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var names = StaticReflection.GetMemberPath(expression);
        var segmentBuilders = names.Select(n => new ApiKeyPathSegmentBuilder(n));
        return new ApiKeyPathBuilder<TRoot>(segmentBuilders);
    }

    /// <summary>
    ///     Creates a builder for a path rooted at <typeparamref name="TPathRoot"/> using a type-safe lambda expression.
    ///     This overload allows callers to specify only the root type while the terminal value is boxed to
    ///     <see cref="object"/>.
    /// </summary>
    /// <typeparam name="TPathRoot">The CLR type from which the navigation begins.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar member on <typeparamref name="TPathRoot"/>, optionally through navigation members.</param>
    /// <returns>A new <see cref="ApiKeyPathBuilder{TRoot}"/> with <typeparamref name="TPathRoot"/> as the root CLR type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expression"/> is not a simple member access chain.</exception>
    public static ApiKeyPathBuilder<TRoot> For<TPathRoot>(Expression<Func<TPathRoot, object?>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var names = StaticReflection.GetMemberPath(expression);
        var segmentBuilders = names.Select(n => new ApiKeyPathSegmentBuilder(n));
        return new ApiKeyPathBuilder<TRoot>(typeof(TPathRoot), segmentBuilders);
    }
    #endregion
}
