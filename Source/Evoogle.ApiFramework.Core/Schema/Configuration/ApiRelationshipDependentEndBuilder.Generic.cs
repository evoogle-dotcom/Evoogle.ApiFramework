// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring the dependent end of an <see cref="ApiRelationship"/>
///     whose CLR type is <typeparamref name="TDependent"/>.
///     Extends <see cref="ApiRelationshipDependentEndBuilder"/> with a strongly-typed
///     <see cref="WithForeignKey"/> overload.
/// </summary>
/// <typeparam name="TDependent">The CLR type of the dependent object.</typeparam>
public sealed class ApiRelationshipDependentEndBuilder<TDependent>() : ApiRelationshipDependentEndBuilder(typeof(TDependent))
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiRelationshipDependentEndBuilder.AddRelationshipDependentEndExtension(Type, object)"/>
    public new ApiRelationshipDependentEndBuilder<TDependent> AddRelationshipDependentEndExtension(Type extensionType, object extension)
    {
        base.AddRelationshipDependentEndExtension(extensionType, extension);
        return this;
    }
    #endregion

    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyType"/> using a strongly-typed builder for
    ///     <typeparamref name="TDependent"/>.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder<TDependent> WithForeignKey(Action<ApiKeyTypeBuilder<TDependent>>? configure = null)
    {
        var builder = new ApiKeyTypeBuilder<TDependent>();
        configure?.Invoke(builder);
        base.SetForeignKeyTypeBuilderCore(builder);
        return this;
    }

    #endregion
}
