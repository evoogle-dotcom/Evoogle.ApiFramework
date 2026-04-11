// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiIdentity"/> on an object type
///     whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiIdentityBuilder"/> with expression-based overloads so property names
///     are extracted at compile time rather than supplied as raw strings.
/// </summary>
/// <typeparam name="T">The CLR type that owns this identity.</typeparam>
/// <param name="apiName">The API name of the identity.</param>
public sealed class ApiIdentityBuilder<T>(string apiName) : ApiIdentityBuilder(apiName)
{
    #region Builder Methods
    /// <inheritdoc cref="ApiIdentityBuilder.AddIdentityExtension(Type, object)"/>
    public new ApiIdentityBuilder<T> AddIdentityExtension(Type type, object value)
    {
        base.AddIdentityExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiIdentityBuilder.AddIdentityExtension{TExt}(TExt)"/>
    public new ApiIdentityBuilder<T> AddIdentityExtension<TExt>(TExt value) where TExt : notnull
        => this.AddIdentityExtension(typeof(TExt), value);

    /// <summary>
    ///     Adds a scalar identity part, deriving the CLR property name from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the scalar property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder<T> AddScalar<TResult>(
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiIdentityPartBuilder>? configure = null)
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddScalar(clrName, configure);
        return this;
    }

    /// <summary>
    ///     Adds a scalar identity part with an explicit CLR type hint, deriving the CLR property name
    ///     from <paramref name="clrProperty"/>.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the scalar property on <typeparamref name="T"/>.</param>
    /// <param name="clrScalarTypeHint">The CLR type to use when extracting the scalar value, overriding the property's inferred type.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder<T> AddScalar<TResult>(
        Expression<Func<T, TResult>> clrProperty,
        Type clrScalarTypeHint,
        Action<ApiIdentityPartBuilder>? configure = null)
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddScalar(clrName, clrScalarTypeHint, configure);
        return this;
    }

    /// <summary>
    ///     Adds a nested identity part, deriving the CLR property name from <paramref name="clrProperty"/>.
    ///     Uses the primary identity of the nested object type.
    /// </summary>
    /// <typeparam name="TResult">The property return type (must be an API object type).</typeparam>
    /// <param name="clrProperty">Expression selecting the nested object property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder<T> AddNested<TResult>(
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiIdentityPartBuilder>? configure = null)
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddNested(clrName, configure);
        return this;
    }

    /// <summary>
    ///     Adds a nested identity part, deriving the CLR property name from <paramref name="clrProperty"/>,
    ///     and uses a named identity on the nested object type.
    /// </summary>
    /// <typeparam name="TResult">The property return type (must be an API object type).</typeparam>
    /// <param name="clrProperty">Expression selecting the nested object property on <typeparamref name="T"/>.</param>
    /// <param name="apiIdentityName">The explicit name of the identity to use on the nested object type.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder<T> AddNested<TResult>(
        Expression<Func<T, TResult>> clrProperty,
        string apiIdentityName,
        Action<ApiIdentityPartBuilder>? configure = null)
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddNested(clrName, apiIdentityName, configure);
        return this;
    }

    /// <inheritdoc cref="ApiIdentityBuilder.AddOwner(Action{ApiIdentityPartBuilder}?)"/>
    public new ApiIdentityBuilder<T> AddOwner(Action<ApiIdentityPartBuilder>? configure = null)
    {
        base.AddOwner(configure);
        return this;
    }

    /// <inheritdoc cref="ApiIdentityBuilder.AddOwner(string, Action{ApiIdentityPartBuilder}?)"/>
    public new ApiIdentityBuilder<T> AddOwner(string apiIdentityName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        base.AddOwner(apiIdentityName, configure);
        return this;
    }
    #endregion
}
