// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;


/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiObjectType"/> whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiObjectTypeBuilder"/> with expression-based overloads so CLR member names are
///     extracted at compile time rather than supplied as raw strings.
/// </summary>
/// <typeparam name="T">The CLR type represented by the API object type.</typeparam>
/// <param name="context">The shared builder context.</param>
public sealed class ApiObjectTypeBuilder<T>(ApiSchemaBuilderContext context)
    : ApiObjectTypeBuilder(typeof(T), context)
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiObjectTypeBuilder.AddObjectTypeExtension(Type, object)"/>
    public new ApiObjectTypeBuilder<T> AddObjectTypeExtension(Type type, object extension)
    {
        base.AddObjectTypeExtension(type, extension);
        return this;
    }
    #endregion

    #region AddKey Methods
    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition using a strongly-typed <see cref="ApiKeyTypeBuilder{T}"/>
    ///     callback that supports expression-based property selection.
    /// </summary>
    /// <remarks>
    ///     Key-bound relationship principal ends infer the best compatible key from the corresponding foreign key
    ///     when no key name is supplied; call
    ///     <see cref="ApiRelationshipPrincipalEndBuilder.WithPrimaryKey"/> on the principal end builder to
    ///     select a named key explicitly.
    ///     To configure with string-based property names instead, use the non-generic
    ///     <see cref="ApiObjectTypeBuilder.AddKey"/> overload on the base class.
    /// </remarks>
    /// <param name="apiName">The API name of the key type.</param>
    /// <param name="configure">Optional callback to configure the key type using a typed builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddKey(string apiName, Action<ApiKeyTypeBuilder<T>>? configure = null)
    {
        var apiKeyTypeBuilder = new ApiKeyTypeBuilder<T>(apiName);
        configure?.Invoke(apiKeyTypeBuilder);
        base.AddKeyTypeBuilderCore(apiKeyTypeBuilder);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with a single key path using a type-safe expression,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <remarks>
    ///     Key-bound relationship principal ends infer the best compatible key from the corresponding foreign key
    ///     when no key name is supplied; call
    ///     <see cref="ApiRelationshipPrincipalEndBuilder.WithPrimaryKey"/> on the principal end builder to
    ///     select a named key explicitly.
    /// </remarks>
    /// <typeparam name="TScalar">The return type of the terminal scalar property.</typeparam>
    /// <param name="apiName">The API name of the key type.</param>
    /// <param name="expression">A lambda expression selecting the scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddKey<TScalar>(string apiName, Expression<Func<T, TScalar>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var apiKeyTypeBuilder = new ApiKeyTypeBuilder<T>(apiName);
        apiKeyTypeBuilder.AddPath(expression);
        base.AddKeyTypeBuilderCore(apiKeyTypeBuilder);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with two key paths using type-safe expressions,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <typeparam name="TScalar1">The return type of the first terminal scalar property.</typeparam>
    /// <typeparam name="TScalar2">The return type of the second terminal scalar property.</typeparam>
    /// <param name="apiName">The API name of the key type.</param>
    /// <param name="expression1">A lambda expression selecting the first scalar property, optionally through navigation properties.</param>
    /// <param name="expression2">A lambda expression selecting the second scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddKey<TScalar1, TScalar2>
    (
        string apiName,
        Expression<Func<T, TScalar1>> expression1,
        Expression<Func<T, TScalar2>> expression2
    )
    {
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);

        var apiKeyTypeBuilder = new ApiKeyTypeBuilder<T>(apiName);
        apiKeyTypeBuilder
            .AddPath(expression1)
            .AddPath(expression2);
        base.AddKeyTypeBuilderCore(apiKeyTypeBuilder);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with three key paths using type-safe expressions,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <typeparam name="TScalar1">The return type of the first terminal scalar property.</typeparam>
    /// <typeparam name="TScalar2">The return type of the second terminal scalar property.</typeparam>
    /// <typeparam name="TScalar3">The return type of the third terminal scalar property.</typeparam>
    /// <param name="apiName">The API name of the key type.</param>
    /// <param name="expression1">A lambda expression selecting the first scalar property, optionally through navigation properties.</param>
    /// <param name="expression2">A lambda expression selecting the second scalar property, optionally through navigation properties.</param>
    /// <param name="expression3">A lambda expression selecting the third scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddKey<TScalar1, TScalar2, TScalar3>
    (
        string apiName,
        Expression<Func<T, TScalar1>> expression1,
        Expression<Func<T, TScalar2>> expression2,
        Expression<Func<T, TScalar3>> expression3
    )
    {
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);
        ArgumentNullException.ThrowIfNull(expression3);

        var apiKeyTypeBuilder = new ApiKeyTypeBuilder<T>(apiName);
        apiKeyTypeBuilder
            .AddPath(expression1)
            .AddPath(expression2)
            .AddPath(expression3);
        base.AddKeyTypeBuilderCore(apiKeyTypeBuilder);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with four key paths using type-safe expressions,
    ///     without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/> callback.
    /// </summary>
    /// <typeparam name="TScalar1">The return type of the first terminal scalar property.</typeparam>
    /// <typeparam name="TScalar2">The return type of the second terminal scalar property.</typeparam>
    /// <typeparam name="TScalar3">The return type of the third terminal scalar property.</typeparam>
    /// <typeparam name="TScalar4">The return type of the fourth terminal scalar property.</typeparam>
    /// <param name="apiName">The API name of the key type.</param>
    /// <param name="expression1">A lambda expression selecting the first scalar property, optionally through navigation properties.</param>
    /// <param name="expression2">A lambda expression selecting the second scalar property, optionally through navigation properties.</param>
    /// <param name="expression3">A lambda expression selecting the third scalar property, optionally through navigation properties.</param>
    /// <param name="expression4">A lambda expression selecting the fourth scalar property, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddKey<TScalar1, TScalar2, TScalar3, TScalar4>
    (
        string apiName,
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

        var apiKeyTypeBuilder = new ApiKeyTypeBuilder<T>(apiName);
        apiKeyTypeBuilder
            .AddPath(expression1)
            .AddPath(expression2)
            .AddPath(expression3)
            .AddPath(expression4);
        base.AddKeyTypeBuilderCore(apiKeyTypeBuilder);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with a single key path rooted at
    ///     <typeparamref name="TRoot"/>, without requiring an explicit <see cref="ApiKeyTypeBuilder{T}"/>
    ///     callback.
    /// </summary>
    /// <remarks>
    ///     Use this overload when the key for <typeparamref name="T"/> is modeled through an owning or otherwise
    ///     related CLR type, such as an owned dependent whose identity is rooted at its owner.
    /// </remarks>
    /// <typeparam name="TRoot">The CLR type from which the navigation begins.</typeparam>
    /// <param name="apiName">The API name of the key type.</param>
    /// <param name="expression">A lambda expression selecting the scalar property on <typeparamref name="TRoot"/>, optionally through navigation properties.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddKeyFrom<TRoot>(string apiName, Expression<Func<TRoot, object?>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var apiKeyTypeBuilder = new ApiKeyTypeBuilder<T>(apiName);
        apiKeyTypeBuilder.AddPathFrom(expression);
        base.AddKeyTypeBuilderCore(apiKeyTypeBuilder);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddKey(string, Action{ApiKeyTypeBuilder}?)"/>
    public new ApiObjectTypeBuilder<T> AddKey(string apiName, Action<ApiKeyTypeBuilder>? configure = null)
    {
        base.AddKey(apiName, configure);
        return this;
    }
    #endregion

    #region AddProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition, deriving the CLR name from <paramref name="clrProperty"/>
    ///     and using it as the API name as well.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddProperty<TResult>(Expression<Func<T, TResult>> clrProperty, Action<ApiPropertyBuilder>? configure = null)
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddProperty(clrName, clrName, configure);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition, deriving the CLR name from <paramref name="clrProperty"/>
    ///     and using the explicitly supplied <paramref name="apiName"/> for the API surface.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the property on <typeparamref name="T"/>.</param>
    /// <param name="apiName">The API property name to expose.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddProperty<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        string apiName,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddProperty(apiName, clrName, configure);
        return this;
    }
    #endregion

    #region AddRequiredProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as required, deriving the CLR name from
    ///     <paramref name="clrProperty"/> and using it as the API name as well.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddRequiredProperty<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddProperty(clrName, clrName, b => { b.AsRequired(); configure?.Invoke(b); });
        return this;
    }
    #endregion

    #region AddOptionalProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as optional, deriving the CLR name from
    ///     <paramref name="clrProperty"/> and using it as the API name as well.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddOptionalProperty<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddProperty(clrName, clrName, b => { b.AsOptional(); configure?.Invoke(b); });
        return this;
    }
    #endregion

    #region With Methods
    /// <inheritdoc cref="ApiNamedTypeBuilder{TBuilder}.WithName"/>
    public new ApiObjectTypeBuilder<T> WithName(string apiName)
    {
        base.WithName(apiName);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.WithOptions"/>
    public new ApiObjectTypeBuilder<T> WithOptions(Action<ApiObjectTypeOptionsBuilder> configure)
    {
        base.WithOptions(configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.WithDefaultOptions"/>
    public new ApiObjectTypeBuilder<T> WithDefaultOptions()
    {
        base.WithDefaultOptions();
        return this;
    }
    #endregion
}
