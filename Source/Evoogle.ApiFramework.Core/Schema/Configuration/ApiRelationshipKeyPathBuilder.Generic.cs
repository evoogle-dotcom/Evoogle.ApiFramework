// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring a single <see cref="ApiRelationshipKeyPath"/> node
///     on an object whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiRelationshipKeyPathBuilder"/> with expression-based overloads.
/// </summary>
/// <typeparam name="T">The CLR type of the object at this level of the key path tree.</typeparam>
/// <param name="apiKind">The kind of key path to build.</param>
/// <param name="clrPropertyName">
///     The CLR property name for <see cref="ApiRelationshipKeyPathKind.Scalar"/> and
///     <see cref="ApiRelationshipKeyPathKind.Nested"/> kinds; <see langword="null"/> for
///     <see cref="ApiRelationshipKeyPathKind.Owner"/>.
/// </param>
public sealed class ApiRelationshipKeyPathBuilder<T>(ApiRelationshipKeyPathKind apiKind, string? clrPropertyName)
    : ApiRelationshipKeyPathBuilder(apiKind, clrPropertyName)
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiRelationshipKeyPathBuilder.AddKeyPathExtension(Type, object)"/>
    public new ApiRelationshipKeyPathBuilder<T> AddKeyPathExtension(Type type, object value)
    {
        base.AddKeyPathExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiRelationshipKeyPathBuilder.AddKeyPathExtension{TExt}(TExt)"/>
    public new ApiRelationshipKeyPathBuilder<T> AddKeyPathExtension<TExt>(TExt value) where TExt : notnull
        => this.AddKeyPathExtension(typeof(TExt), value);
    #endregion

    #region AddPath Methods
    /// <summary>
    ///     Adds a scalar child key path, deriving the CLR property name from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the scalar FK property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to attach extensions to the scalar path.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipKeyPathBuilder<T> AddScalarPath<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiRelationshipKeyPathBuilder<T>>? configure = null
    )
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        var builder = new ApiRelationshipKeyPathBuilder<T>(ApiRelationshipKeyPathKind.Scalar, clrName);
        configure?.Invoke(builder);
        base.AddChildBuilderCore(builder);
        return this;
    }

    /// <summary>
    ///     Adds a nested child key path, deriving the CLR property name from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The nested object property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the nested object property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Callback to add child paths within the nested object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipKeyPathBuilder<T> AddNestedPath<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiRelationshipKeyPathBuilder<TResult>> configure
    )
    {
        ArgumentNullException.ThrowIfNull(configure);

        var clrName = StaticReflection.GetMemberName(clrProperty);
        var builder = new ApiRelationshipKeyPathBuilder<TResult>(ApiRelationshipKeyPathKind.Nested, clrName);
        configure(builder);
        base.AddChildBuilderCore(builder);
        return this;
    }
    #endregion
}
