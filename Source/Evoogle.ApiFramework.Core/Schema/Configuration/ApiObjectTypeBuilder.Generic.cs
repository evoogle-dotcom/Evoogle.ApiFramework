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
    /// <inheritdoc cref="ApiObjectTypeBuilder.AddObjectExtension(Type, object)"/>
    public new ApiObjectTypeBuilder<T> AddObjectExtension(Type type, object value)
    {
        base.AddObjectExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddObjectExtension{TExt}(TExt)"/>
    public new ApiObjectTypeBuilder<T> AddObjectExtension<TExt>(TExt value) where TExt : notnull
        => this.AddObjectExtension(typeof(TExt), value);
    #endregion

    #region AddIdentity Methods
    /// <inheritdoc cref="ApiObjectTypeBuilder.AddIdentity"/>
    public new ApiObjectTypeBuilder<T> AddIdentity(string apiName, Action<ApiIdentityBuilder>? configure = null)
    {
        base.AddIdentity(apiName, configure);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiIdentity"/> definition using a strongly-typed <see cref="ApiIdentityBuilder{T}"/>
    ///     callback that supports expression-based property selection.
    /// </summary>
    /// <param name="apiName">The API name of the identity.</param>
    /// <param name="configure">Optional callback to configure the identity using a typed builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddIdentity(string apiName, Action<ApiIdentityBuilder<T>>? configure = null)
    {
        var apiIdentityBuilder = new ApiIdentityBuilder<T>(apiName);
        configure?.Invoke(apiIdentityBuilder);
        base.AddIdentityBuilderCore(apiIdentityBuilder);
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

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition, deriving the CLR name from <paramref name="clrProperty"/>
    ///     and using it as the API name as well, and explicitly overrides the CLR nullability-inferred
    ///     required/optional modifier.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the property on <typeparamref name="T"/>.</param>
    /// <param name="required"><see langword="true"/> to mark the property as required; <see langword="false"/> to mark it as optional.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddProperty<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        bool required,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddProperty(clrName, clrName, required, configure);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition, deriving the CLR name from <paramref name="clrProperty"/>
    ///     and using the explicitly supplied <paramref name="apiName"/> for the API surface, and explicitly
    ///     overrides the CLR nullability-inferred required/optional modifier.
    /// </summary>
    /// <typeparam name="TResult">The property return type.</typeparam>
    /// <param name="clrProperty">Expression selecting the property on <typeparamref name="T"/>.</param>
    /// <param name="apiName">The API property name to expose.</param>
    /// <param name="required"><see langword="true"/> to mark the property as required; <see langword="false"/> to mark it as optional.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddProperty<TResult>
    (
        Expression<Func<T, TResult>> clrProperty,
        string apiName,
        bool required,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        var clrName = StaticReflection.GetMemberName(clrProperty);
        base.AddProperty(apiName, clrName, required, configure);
        return this;
    }
    #endregion

    #region AddRelationship Methods
    /// <inheritdoc cref="ApiObjectTypeBuilder.AddOneToOneRelationship(string, Action{ApiRelationshipOneToOneBuilder})"/>
    public new ApiObjectTypeBuilder<T> AddOneToOneRelationship(string apiName, Action<ApiRelationshipOneToOneBuilder> configure)
    {
        base.AddOneToOneRelationship(apiName, configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddOneToOneRelationship(string, IApiRelationshipOneToOneConfiguration)"/>
    public new ApiObjectTypeBuilder<T> AddOneToOneRelationship(string apiName, IApiRelationshipOneToOneConfiguration configuration)
    {
        base.AddOneToOneRelationship(apiName, configuration);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddOneToManyRelationship(string, Action{ApiRelationshipOneToManyBuilder})"/>
    public new ApiObjectTypeBuilder<T> AddOneToManyRelationship(string apiName, Action<ApiRelationshipOneToManyBuilder> configure)
    {
        base.AddOneToManyRelationship(apiName, configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddOneToManyRelationship(string, IApiRelationshipOneToManyConfiguration)"/>
    public new ApiObjectTypeBuilder<T> AddOneToManyRelationship(string apiName, IApiRelationshipOneToManyConfiguration configuration)
    {
        base.AddOneToManyRelationship(apiName, configuration);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddManyToManyRelationship(string, Action{ApiRelationshipManyToManyBuilder})"/>
    public new ApiObjectTypeBuilder<T> AddManyToManyRelationship(string apiName, Action<ApiRelationshipManyToManyBuilder> configure)
    {
        base.AddManyToManyRelationship(apiName, configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddManyToManyRelationship(string, IApiRelationshipManyToManyConfiguration)"/>
    public new ApiObjectTypeBuilder<T> AddManyToManyRelationship(string apiName, IApiRelationshipManyToManyConfiguration configuration)
    {
        base.AddManyToManyRelationship(apiName, configuration);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddManyToManyRelationship{TAssociation}(string, Action{ApiRelationshipManyToManyBuilder{TAssociation}})"/>
    public new ApiObjectTypeBuilder<T> AddManyToManyRelationship<TAssociation>(string apiName, Action<ApiRelationshipManyToManyBuilder<TAssociation>> configure)
    {
        base.AddManyToManyRelationship(apiName, configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddManyToManyRelationship{TAssociation}(string, IApiRelationshipManyToManyConfiguration{TAssociation})"/>
    public new ApiObjectTypeBuilder<T> AddManyToManyRelationship<TAssociation>(string apiName, IApiRelationshipManyToManyConfiguration<TAssociation> configuration)
    {
        base.AddManyToManyRelationship(apiName, configuration);
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
