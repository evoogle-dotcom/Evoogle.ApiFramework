// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring the dependent end of an <see cref="ApiRelationship"/>
///     whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiRelationshipDependentEndBuilder"/> with a strongly-typed
///     <see cref="WithForeignKey"/> overload.
/// </summary>
/// <typeparam name="T">The CLR type of the dependent object.</typeparam>
public sealed class ApiRelationshipDependentEndBuilder<T>() : ApiRelationshipDependentEndBuilder(typeof(T))
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiRelationshipDependentEndBuilder.AddRelationshipDependentEndExtension(Type, object)"/>
    public new ApiRelationshipDependentEndBuilder<T> AddRelationshipDependentEndExtension(Type type, object extension)
    {
        base.AddRelationshipDependentEndExtension(type, extension);
        return this;
    }
    #endregion

    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> using a strongly-typed builder for
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<T> WithForeignKey(Action<ApiKeyTypeBuilder<T>>? configure = null)
    {
        var builder = new ApiKeyTypeBuilder<T>();
        configure?.Invoke(builder);
        base.SetForeignKeyTypeBuilderCore(builder);
        return this;
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with a single key path using a type-safe expression,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <typeparam name="TScalar">The return type of the terminal scalar property.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<T> WithForeignKey<TScalar>(Expression<Func<T, TScalar>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var builder = new ApiKeyTypeBuilder<T>();
        builder.AddPath(expression);
        base.SetForeignKeyTypeBuilderCore(builder);
        return this;
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with two key paths using type-safe expressions,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <typeparam name="TScalar1">The return type of the first terminal scalar property.</typeparam>
    /// <typeparam name="TScalar2">The return type of the second terminal scalar property.</typeparam>
    /// <param name="expression1">A lambda expression selecting the first scalar property, optionally through navigation properties.</param>
    /// <param name="expression2">A lambda expression selecting the second scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<T> WithForeignKey<TScalar1, TScalar2>
    (
        Expression<Func<T, TScalar1>> expression1,
        Expression<Func<T, TScalar2>> expression2
    )
    {
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);

        var builder = new ApiKeyTypeBuilder<T>();
        builder
            .AddPath(expression1)
            .AddPath(expression2);
        base.SetForeignKeyTypeBuilderCore(builder);
        return this;
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with three key paths using type-safe expressions,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <typeparam name="TScalar1">The return type of the first terminal scalar property.</typeparam>
    /// <typeparam name="TScalar2">The return type of the second terminal scalar property.</typeparam>
    /// <typeparam name="TScalar3">The return type of the third terminal scalar property.</typeparam>
    /// <param name="expression1">A lambda expression selecting the first scalar property, optionally through navigation properties.</param>
    /// <param name="expression2">A lambda expression selecting the second scalar property, optionally through navigation properties.</param>
    /// <param name="expression3">A lambda expression selecting the third scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<T> WithForeignKey<TScalar1, TScalar2, TScalar3>
    (
        Expression<Func<T, TScalar1>> expression1,
        Expression<Func<T, TScalar2>> expression2,
        Expression<Func<T, TScalar3>> expression3
    )
    {
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);
        ArgumentNullException.ThrowIfNull(expression3);

        var builder = new ApiKeyTypeBuilder<T>();
        builder
            .AddPath(expression1)
            .AddPath(expression2)
            .AddPath(expression3);
        base.SetForeignKeyTypeBuilderCore(builder);
        return this;
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with four key paths using type-safe expressions,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <typeparam name="TScalar1">The return type of the first terminal scalar property.</typeparam>
    /// <typeparam name="TScalar2">The return type of the second terminal scalar property.</typeparam>
    /// <typeparam name="TScalar3">The return type of the third terminal scalar property.</typeparam>
    /// <typeparam name="TScalar4">The return type of the fourth terminal scalar property.</typeparam>
    /// <param name="expression1">A lambda expression selecting the first scalar property, optionally through navigation properties.</param>
    /// <param name="expression2">A lambda expression selecting the second scalar property, optionally through navigation properties.</param>
    /// <param name="expression3">A lambda expression selecting the third scalar property, optionally through navigation properties.</param>
    /// <param name="expression4">A lambda expression selecting the fourth scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<T> WithForeignKey<TScalar1, TScalar2, TScalar3, TScalar4>
    (
        Expression<Func<T, TScalar1>> expression1,
        Expression<Func<T, TScalar2>> expression2,
        Expression<Func<T, TScalar3>> expression3,
        Expression<Func<T, TScalar4>> expression4
    )
    {
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);
        ArgumentNullException.ThrowIfNull(expression3);
        ArgumentNullException.ThrowIfNull(expression4);

        var builder = new ApiKeyTypeBuilder<T>();
        builder
            .AddPath(expression1)
            .AddPath(expression2)
            .AddPath(expression3)
            .AddPath(expression4);
        base.SetForeignKeyTypeBuilderCore(builder);
        return this;
    }

    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with a single key path rooted at
    ///     <typeparamref name="TRoot"/>, without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/>
    ///     callback.
    /// </summary>
    /// <typeparam name="TRoot">The CLR type from which the navigation begins.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar property on <typeparamref name="TRoot"/>, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<T> WithForeignKeyFrom<TRoot>(Expression<Func<TRoot, object?>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var builder = new ApiKeyTypeBuilder<T>();
        builder.AddPathFrom(expression);
        base.SetForeignKeyTypeBuilderCore(builder);
        return this;
    }
    #endregion
}
