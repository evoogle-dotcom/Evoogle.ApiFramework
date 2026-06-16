// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring the association of an <see cref="ApiRelationshipManyToMany"/>
///     whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiRelationshipAssociationBuilder"/> with strongly-typed
///     <see cref="WithForeignKeyA"/> and <see cref="WithForeignKeyB"/> overloads.
/// </summary>
/// <typeparam name="T">The CLR type of the association object.</typeparam>
public sealed class ApiRelationshipAssociationBuilder<T>()
    : ApiRelationshipAssociationBuilder(typeof(T))
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiRelationshipAssociationBuilder.AddRelationshipAssociationExtension(Type, object)"/>
    public new ApiRelationshipAssociationBuilder<T> AddRelationshipAssociationExtension(Type type, object extension)
    {
        base.AddRelationshipAssociationExtension(type, extension);
        return this;
    }
    #endregion

    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the A-side foreign key role's <see cref="ApiKeyType"/> using a strongly-typed builder for
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<T> WithForeignKeyA(Action<ApiKeyTypeBuilder<T>>? configure = null)
    {
        var builder = new ApiKeyTypeBuilder<T>();
        configure?.Invoke(builder);
        base.SetForeignKeyTypeBuilderACore(builder);
        return this;
    }

    /// <summary>
    ///     Sets the A-side foreign key role's <see cref="ApiKeyType"/> with a single key path using a type-safe expression,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <typeparam name="TScalar">The return type of the terminal scalar property.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<T> WithForeignKeyA<TScalar>(Expression<Func<T, TScalar>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var builder = new ApiKeyTypeBuilder<T>();
        builder.AddPath(expression);
        base.SetForeignKeyTypeBuilderACore(builder);
        return this;
    }

    /// <summary>
    ///     Sets the B-side foreign key role's <see cref="ApiKeyType"/> using a strongly-typed builder for
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<T> WithForeignKeyB(Action<ApiKeyTypeBuilder<T>>? configure = null)
    {
        var builder = new ApiKeyTypeBuilder<T>();
        configure?.Invoke(builder);
        base.SetForeignKeyTypeBuilderBCore(builder);
        return this;
    }

    /// <summary>
    ///     Sets the B-side foreign key role's <see cref="ApiKeyType"/> with a single key path using a type-safe expression,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <typeparam name="TScalar">The return type of the terminal scalar property.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<T> WithForeignKeyB<TScalar>(Expression<Func<T, TScalar>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var builder = new ApiKeyTypeBuilder<T>();
        builder.AddPath(expression);
        base.SetForeignKeyTypeBuilderBCore(builder);
        return this;
    }
    #endregion
}
