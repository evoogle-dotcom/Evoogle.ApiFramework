// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring the association of an <see cref="ApiRelationshipManyToMany"/>
///     whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiRelationshipAssociationBuilder"/> with strongly-typed
///     <see cref="WithForeignKeyTypeA"/> and <see cref="WithForeignKeyTypeB"/> overloads.
/// </summary>
/// <typeparam name="T">The CLR type of the association object.</typeparam>
public sealed class ApiRelationshipAssociationBuilder<T>()
    : ApiRelationshipAssociationBuilder(typeof(T))
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiRelationshipAssociationBuilder.AddRelationshipAssociationExtension(Type, object)"/>
    public new ApiRelationshipAssociationBuilder<T> AddRelationshipAssociationExtension(Type type, object value)
    {
        base.AddRelationshipAssociationExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiRelationshipAssociationBuilder.AddRelationshipAssociationExtension{TExt}(TExt)"/>
    public new ApiRelationshipAssociationBuilder<T> AddRelationshipAssociationExtension<TExt>(TExt value) where TExt : notnull
        => this.AddRelationshipAssociationExtension(typeof(TExt), value);
    #endregion

    #region WithForeignKeyType Methods
    /// <summary>
    ///     Sets the A-side FK key type with the given <paramref name="apiName"/>, using a strongly-typed builder for
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <param name="apiName">The API name of the A-side FK key type.</param>
    /// <param name="configure">Optional callback to configure key paths on the FK key type.</param>
    /// <returns>The current builder instance.</returns>
    public new ApiRelationshipAssociationBuilder<T> WithForeignKeyTypeA(string apiName, Action<ApiKeyTypeBuilder<T>>? configure = null)
    {
        var builder = new ApiKeyTypeBuilder<T>(apiName);
        configure?.Invoke(builder);
        base.SetForeignKeyTypeBuilderACore(builder);
        return this;
    }

    /// <summary>
    ///     Sets the B-side FK key type with the given <paramref name="apiName"/>, using a strongly-typed builder for
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <param name="apiName">The API name of the B-side FK key type.</param>
    /// <param name="configure">Optional callback to configure key paths on the FK key type.</param>
    /// <returns>The current builder instance.</returns>
    public new ApiRelationshipAssociationBuilder<T> WithForeignKeyTypeB(string apiName, Action<ApiKeyTypeBuilder<T>>? configure = null)
    {
        var builder = new ApiKeyTypeBuilder<T>(apiName);
        configure?.Invoke(builder);
        base.SetForeignKeyTypeBuilderBCore(builder);
        return this;
    }
    #endregion
}
