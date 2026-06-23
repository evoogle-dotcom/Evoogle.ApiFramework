// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Extension methods for <see cref="ApiObjectTypeBuilder{TObject}"/>.
/// </summary>
public static class ApiObjectTypeBuilderGenericExtensions
{
    #region AddKey Methods
    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with a single key path using a type-safe expression.
    /// </summary>
    public static ApiObjectTypeBuilder<TObject> AddKey<TObject, TScalar>(this ApiObjectTypeBuilder<TObject> builder, string apiName, Expression<Func<TObject, TScalar>> expression)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(expression);

        return builder.AddKey(apiName, b => b.AddPath(expression));
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with two key paths using type-safe expressions.
    /// </summary>
    public static ApiObjectTypeBuilder<TObject> AddKey<TObject, TScalar1, TScalar2>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        Expression<Func<TObject, TScalar1>> expression1,
        Expression<Func<TObject, TScalar2>> expression2
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);

        return builder.AddKey(apiName, b => b
            .AddPath(expression1)
            .AddPath(expression2));
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with three key paths using type-safe expressions.
    /// </summary>
    public static ApiObjectTypeBuilder<TObject> AddKey<TObject, TScalar1, TScalar2, TScalar3>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        Expression<Func<TObject, TScalar1>> expression1,
        Expression<Func<TObject, TScalar2>> expression2,
        Expression<Func<TObject, TScalar3>> expression3
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);
        ArgumentNullException.ThrowIfNull(expression3);

        return builder.AddKey(apiName, b => b
            .AddPath(expression1)
            .AddPath(expression2)
            .AddPath(expression3));
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with four key paths using type-safe expressions.
    /// </summary>
    public static ApiObjectTypeBuilder<TObject> AddKey<TObject, TScalar1, TScalar2, TScalar3, TScalar4>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        Expression<Func<TObject, TScalar1>> expression1,
        Expression<Func<TObject, TScalar2>> expression2,
        Expression<Func<TObject, TScalar3>> expression3,
        Expression<Func<TObject, TScalar4>> expression4
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(expression1);
        ArgumentNullException.ThrowIfNull(expression2);
        ArgumentNullException.ThrowIfNull(expression3);
        ArgumentNullException.ThrowIfNull(expression4);

        return builder.AddKey(apiName, b => b
            .AddPath(expression1)
            .AddPath(expression2)
            .AddPath(expression3)
            .AddPath(expression4));
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with a single key path rooted at <typeparamref name="TRoot"/>.
    /// </summary>
    public static ApiObjectTypeBuilder AddKeyFrom<TRoot>(this ApiObjectTypeBuilder builder, string apiName, Expression<Func<TRoot, object?>> expression)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(expression);

        var clrPropertyNames = StaticReflection.GetMemberPath(expression);
        return builder.AddKey(apiName, b => b.AddPath(typeof(TRoot), clrPropertyNames));
    }

    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition with a single key path rooted at <typeparamref name="TPathRoot"/>.
    /// </summary>
    public static ApiObjectTypeBuilder<TObject> AddKeyFrom<TObject, TPathRoot>(this ApiObjectTypeBuilder<TObject> builder, string apiName, Expression<Func<TPathRoot, object?>> expression)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(expression);

        return builder.AddKey(apiName, b => b.AddPathFrom(expression));
    }
    #endregion

    #region AddProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition, deriving the CLR name from <paramref name="clrProperty"/>.
    /// </summary>
    public static ApiObjectTypeBuilder<TObject> AddProperty<TObject, TResult>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        Expression<Func<TObject, TResult>> clrProperty,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(clrProperty);

        var clrName = StaticReflection.GetMemberName(clrProperty);
        builder.AddProperty(clrName, clrName, configure);
        return builder;
    }

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition using a type-safe CLR property selector and explicit API name.
    /// </summary>
    public static ApiObjectTypeBuilder<TObject> AddProperty<TObject, TResult>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        Expression<Func<TObject, TResult>> clrProperty,
        string apiName,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(clrProperty);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        var clrName = StaticReflection.GetMemberName(clrProperty);
        builder.AddProperty(apiName, clrName, configure);
        return builder;
    }
    #endregion

    #region AddRequiredProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as required using a type-safe CLR property selector.
    /// </summary>
    public static ApiObjectTypeBuilder<TObject> AddRequiredProperty<TObject, TResult>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        Expression<Func<TObject, TResult>> clrProperty,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(clrProperty);

        var clrName = StaticReflection.GetMemberName(clrProperty);
        builder.AddProperty(clrName, clrName, b => { b.AsRequired(); configure?.Invoke(b); });
        return builder;
    }
    #endregion

    #region AddOptionalProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as optional using a type-safe CLR property selector.
    /// </summary>
    public static ApiObjectTypeBuilder<TObject> AddOptionalProperty<TObject, TResult>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        Expression<Func<TObject, TResult>> clrProperty,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(clrProperty);

        var clrName = StaticReflection.GetMemberName(clrProperty);
        builder.AddProperty(clrName, clrName, b => { b.AsOptional(); configure?.Invoke(b); });
        return builder;
    }
    #endregion
}
