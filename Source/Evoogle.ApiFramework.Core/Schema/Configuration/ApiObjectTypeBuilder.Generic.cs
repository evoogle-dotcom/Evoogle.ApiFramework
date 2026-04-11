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
    #region Builder Methods
    /// <inheritdoc cref="ApiObjectTypeBuilder.AddObjectExtension(Type, object)"/>
    public new ApiObjectTypeBuilder<T> AddObjectExtension(Type type, object value)
    {
        base.AddObjectExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddObjectExtension{TExt}(TExt)"/>
    public new ApiObjectTypeBuilder<T> AddObjectExtension<TExt>(TExt value) where TExt : notnull
        => this.AddObjectExtension(typeof(TExt), value);

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition, deriving the CLR name from <paramref name="clrProperty"/>
    ///     and using it as the API name as well.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the property on <typeparamref name="T"/>.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddProperty<TResult>(
        Expression<Func<T, TResult>> clrProperty,
        Action<ApiPropertyBuilder>? configure = null)
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
    public ApiObjectTypeBuilder<T> AddProperty<TResult>(
        Expression<Func<T, TResult>> clrProperty,
        string apiName,
        Action<ApiPropertyBuilder>? configure = null)
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddProperty(apiName, clrName, configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddIdentity"/>
    public new ApiObjectTypeBuilder<T> AddIdentity(string apiName, Action<ApiIdentityBuilder> configure)
    {
        base.AddIdentity(apiName, configure);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiIdentity"/> definition using a strongly-typed <see cref="ApiIdentityBuilder{T}"/>
    ///     callback that supports expression-based property selection.
    /// </summary>
    /// <param name="apiName">The API name of the identity.</param>
    /// <param name="configure">Callback to configure the identity using a typed builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddIdentity(string apiName, Action<ApiIdentityBuilder<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var apiIdentityBuilder = new ApiIdentityBuilder<T>(apiName);
        configure(apiIdentityBuilder);
        base.AddIdentityBuilderCore(apiIdentityBuilder);
        return this;
    }

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
