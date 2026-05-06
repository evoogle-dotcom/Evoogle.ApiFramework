// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring the dependent end of an <see cref="ApiRelationship"/>
///     whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiRelationshipDependentEndBuilder"/> with expression-based overloads for FK key paths.
/// </summary>
/// <typeparam name="T">The CLR type of the dependent object.</typeparam>
public sealed class ApiRelationshipDependentEndBuilder<T>()
    : ApiRelationshipDependentEndBuilder(typeof(T))
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiRelationshipDependentEndBuilder.AddDependentEndExtension(Type, object)"/>
    public new ApiRelationshipDependentEndBuilder<T> AddDependentEndExtension(Type type, object value)
    {
        base.AddDependentEndExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiRelationshipDependentEndBuilder.AddDependentEndExtension{TExt}(TExt)"/>
    public new ApiRelationshipDependentEndBuilder<T> AddDependentEndExtension<TExt>(TExt value) where TExt : notnull
        => this.AddDependentEndExtension(typeof(TExt), value);
    #endregion

    #region AddPath Methods
    /// <summary>
    ///     Adds a scalar FK key path, deriving the CLR property name from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the scalar FK property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to attach extensions to the path.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<T> AddScalarPath<TResult>(
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiRelationshipKeyPathBuilder<T>>? configure = null)
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        var builder = new ApiRelationshipKeyPathBuilder<T>(ApiRelationshipKeyPathKind.Scalar, clrName);
        configure?.Invoke(builder);
        base.AddKeyPathBuilderCore(builder);
        return this;
    }

    /// <summary>
    ///     Adds a nested FK key path, deriving the CLR property name from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The nested object property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the nested object property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Callback to add child paths within the nested object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<T> AddNestedPath<TResult>(
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiRelationshipKeyPathBuilder<TResult>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var clrName = StaticReflection.GetMemberName(clrProperty);
        var builder = new ApiRelationshipKeyPathBuilder<TResult>(ApiRelationshipKeyPathKind.Nested, clrName);
        configure(builder);
        base.AddKeyPathBuilderCore(builder);
        return this;
    }

    /// <inheritdoc cref="ApiRelationshipDependentEndBuilder.AddOwnerPath"/>
    public new ApiRelationshipDependentEndBuilder<T> AddOwnerPath(
        Action<ApiRelationshipKeyPathBuilder>? configure = null)
    {
        base.AddOwnerPath(configure);
        return this;
    }
    #endregion

}
