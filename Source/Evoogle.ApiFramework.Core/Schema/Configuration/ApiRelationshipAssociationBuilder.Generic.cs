// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring the association of an <see cref="ApiRelationshipManyToMany"/>
///     whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiRelationshipAssociationBuilder"/> with expression-based overloads for FK key paths.
/// </summary>
/// <typeparam name="T">The CLR type of the association object.</typeparam>
public sealed class ApiRelationshipAssociationBuilder<T>()
    : ApiRelationshipAssociationBuilder(typeof(T))
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiRelationshipAssociationBuilder.AddAssociationExtension(Type, object)"/>
    public new ApiRelationshipAssociationBuilder<T> AddAssociationExtension(Type type, object value)
    {
        base.AddAssociationExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiRelationshipAssociationBuilder.AddAssociationExtension{TExt}(TExt)"/>
    public new ApiRelationshipAssociationBuilder<T> AddAssociationExtension<TExt>(TExt value) where TExt : notnull
        => this.AddAssociationExtension(typeof(TExt), value);
    #endregion

    #region AddPath A Methods
    /// <summary>
    ///     Adds an A-side scalar FK key path, deriving the CLR property name from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the scalar FK property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to attach extensions to the path.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<T> AddScalarPathA<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiRelationshipKeyPathBuilder<T>>? configure = null
    )
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        var builder = new ApiRelationshipKeyPathBuilder<T>(ApiRelationshipKeyPathKind.Scalar, clrName);
        configure?.Invoke(builder);
        base.AddKeyPathBuilderACore(builder);
        return this;
    }

    /// <summary>
    ///     Adds an A-side nested FK key path, deriving the CLR property name from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The nested object property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the nested object property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Callback to add child paths within the nested object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<T> AddNestedPathA<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiRelationshipKeyPathBuilder<TResult>> configure
    )
    {
        ArgumentNullException.ThrowIfNull(configure);

        var clrName = StaticReflection.GetMemberName(clrProperty);
        var builder = new ApiRelationshipKeyPathBuilder<TResult>(ApiRelationshipKeyPathKind.Nested, clrName);
        configure(builder);
        base.AddKeyPathBuilderACore(builder);
        return this;
    }

    /// <inheritdoc cref="ApiRelationshipAssociationBuilder.AddOwnerPathA"/>
    public new ApiRelationshipAssociationBuilder<T> AddOwnerPathA(Action<ApiRelationshipKeyPathBuilder>? configure = null)
    {
        base.AddOwnerPathA(configure);
        return this;
    }
    #endregion

    #region AddPath B Methods
    /// <summary>
    ///     Adds a B-side scalar FK key path, deriving the CLR property name from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the scalar FK property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to attach extensions to the path.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<T> AddScalarPathB<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiRelationshipKeyPathBuilder<T>>? configure = null
    )
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        var builder = new ApiRelationshipKeyPathBuilder<T>(ApiRelationshipKeyPathKind.Scalar, clrName);
        configure?.Invoke(builder);
        base.AddKeyPathBuilderBCore(builder);
        return this;
    }

    /// <summary>
    ///     Adds a B-side nested FK key path, deriving the CLR property name from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The nested object property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the nested object property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Callback to add child paths within the nested object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<T> AddNestedPathB<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiRelationshipKeyPathBuilder<TResult>> configure
    )
    {
        ArgumentNullException.ThrowIfNull(configure);

        var clrName = StaticReflection.GetMemberName(clrProperty);
        var builder = new ApiRelationshipKeyPathBuilder<TResult>(ApiRelationshipKeyPathKind.Nested, clrName);
        configure(builder);
        base.AddKeyPathBuilderBCore(builder);
        return this;
    }

    /// <inheritdoc cref="ApiRelationshipAssociationBuilder.AddOwnerPathB"/>
    public new ApiRelationshipAssociationBuilder<T> AddOwnerPathB(Action<ApiRelationshipKeyPathBuilder>? configure = null)
    {
        base.AddOwnerPathB(configure);
        return this;
    }
    #endregion
}
