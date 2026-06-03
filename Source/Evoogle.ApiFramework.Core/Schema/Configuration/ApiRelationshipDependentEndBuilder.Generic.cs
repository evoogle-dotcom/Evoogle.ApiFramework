// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring the dependent end of an <see cref="ApiRelationship"/>
///     whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiRelationshipDependentEndBuilder"/> with a strongly-typed
///     <see cref="WithForeignKeyType"/> overload.
/// </summary>
/// <typeparam name="T">The CLR type of the dependent object.</typeparam>
public sealed class ApiRelationshipDependentEndBuilder<T>() : ApiRelationshipDependentEndBuilder(typeof(T))
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiRelationshipDependentEndBuilder.AddRelationshipDependentEndExtension(Type, object)"/>
    public new ApiRelationshipDependentEndBuilder<T> AddRelationshipDependentEndExtension(Type type, object value)
    {
        base.AddRelationshipDependentEndExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiRelationshipDependentEndBuilder.AddRelationshipDependentEndExtension{TExt}(TExt)"/>
    public new ApiRelationshipDependentEndBuilder<T> AddRelationshipDependentEndExtension<TExt>(TExt value) where TExt : notnull
        => this.AddRelationshipDependentEndExtension(typeof(TExt), value);
    #endregion

    #region WithForeignKeyType Methods
    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> with the given <paramref name="apiName"/>, using a strongly-typed builder for
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <param name="apiName">The API name of the key type used for the foreign key role.</param>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<T> WithForeignKeyType(string apiName, Action<ApiKeyTypeBuilder<T>>? configure = null)
    {
        var builder = new ApiKeyTypeBuilder<T>(apiName);
        configure?.Invoke(builder);
        base.SetForeignKeyTypeBuilderCore(builder);
        return this;
    }
    #endregion
}
