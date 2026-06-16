// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Extension methods for <see cref="ApiRelationshipDependentEndBuilder{T}"/>.
/// </summary>
public static class ApiRelationshipDependentEndBuilderGenericExtensions
{
    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with a single key path using a type-safe expression.
    /// </summary>
    public static ApiRelationshipDependentEndBuilder<T> WithForeignKey<T, TScalar>
    (
        this ApiRelationshipDependentEndBuilder<T> builder,
        Expression<Func<T, TScalar>> expression
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(expression);

        return builder.WithForeignKey(b => b.AddPath(expression));
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with two key paths using type-safe expressions.
    /// </summary>
    public static ApiRelationshipDependentEndBuilder<T> WithForeignKey<T, TScalar1, TScalar2>
    (
        this ApiRelationshipDependentEndBuilder<T> builder,
        Expression<Func<T, TScalar1>> expression1,
        Expression<Func<T, TScalar2>> expression2
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);

        return builder.WithForeignKey(b => b
            .AddPath(expression1)
            .AddPath(expression2));
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with three key paths using type-safe expressions.
    /// </summary>
    public static ApiRelationshipDependentEndBuilder<T> WithForeignKey<T, TScalar1, TScalar2, TScalar3>
    (
        this ApiRelationshipDependentEndBuilder<T> builder,
        Expression<Func<T, TScalar1>> expression1,
        Expression<Func<T, TScalar2>> expression2,
        Expression<Func<T, TScalar3>> expression3
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);
        ArgumentNullException.ThrowIfNull(expression3);

        return builder.WithForeignKey(b => b
            .AddPath(expression1)
            .AddPath(expression2)
            .AddPath(expression3));
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with four key paths using type-safe expressions.
    /// </summary>
    public static ApiRelationshipDependentEndBuilder<T> WithForeignKey<T, TScalar1, TScalar2, TScalar3, TScalar4>
    (
        this ApiRelationshipDependentEndBuilder<T> builder,
        Expression<Func<T, TScalar1>> expression1,
        Expression<Func<T, TScalar2>> expression2,
        Expression<Func<T, TScalar3>> expression3,
        Expression<Func<T, TScalar4>> expression4
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);
        ArgumentNullException.ThrowIfNull(expression3);
        ArgumentNullException.ThrowIfNull(expression4);

        return builder.WithForeignKey(b => b
            .AddPath(expression1)
            .AddPath(expression2)
            .AddPath(expression3)
            .AddPath(expression4));
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with a single key path rooted at <typeparamref name="TRoot"/>.
    /// </summary>
    public static ApiRelationshipDependentEndBuilder WithForeignKeyFrom<TRoot>
    (
        this ApiRelationshipDependentEndBuilder builder,
        Expression<Func<TRoot, object?>> expression
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(expression);

        var clrPropertyNames = StaticReflection.GetMemberPath(expression);
        return builder.WithForeignKey(b => b.AddPath(typeof(TRoot), clrPropertyNames));
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with a single key path rooted at <typeparamref name="TRoot"/>.
    /// </summary>
    public static ApiRelationshipDependentEndBuilder<T> WithForeignKeyFrom<T, TRoot>
    (
        this ApiRelationshipDependentEndBuilder<T> builder,
        Expression<Func<TRoot, object?>> expression
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(expression);

        return builder.WithForeignKey(b => b.AddPathFrom(expression));
    }
    #endregion
}
