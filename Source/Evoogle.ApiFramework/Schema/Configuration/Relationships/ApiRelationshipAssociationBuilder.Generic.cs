// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Key;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Relationships;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships;

/// <summary>
///     Strongly-typed fluent builder for configuring the association of an <see cref="ApiRelationshipManyToMany"/>
///     whose CLR type is <typeparamref name="TAssociation"/>.
///     Extends <see cref="ApiRelationshipAssociationBuilder"/> with strongly-typed
///     <see cref="WithForeignKeyA"/> and <see cref="WithForeignKeyB"/> overloads.
/// </summary>
/// <typeparam name="TAssociation">The CLR type of the association object.</typeparam>
public sealed class ApiRelationshipAssociationBuilder<TAssociation>()
    : ApiRelationshipAssociationBuilder(typeof(TAssociation))
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiRelationshipAssociationBuilder.AddRelationshipAssociationExtension(Type, object)"/>
    public new ApiRelationshipAssociationBuilder<TAssociation> AddRelationshipAssociationExtension(Type extensionType, object extension)
    {
        base.AddRelationshipAssociationExtension(extensionType, extension);
        return this;
    }
    #endregion

    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the A-side foreign key role's <see cref="ApiKeyDefinition"/> using a strongly-typed builder for
    ///     <typeparamref name="TAssociation"/>.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<TAssociation> WithForeignKeyA(Action<ApiKeyDefinitionBuilder<TAssociation>>? configure = null)
    {
        base.WithForeignKeyA
        (
            configure == null
                ? null
                : builder => configure((ApiKeyDefinitionBuilder<TAssociation>)builder)
        );
        return this;
    }

    /// <summary>
    ///     Sets the B-side foreign key role's <see cref="ApiKeyDefinition"/> using a strongly-typed builder for
    ///     <typeparamref name="TAssociation"/>.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder<TAssociation> WithForeignKeyB(Action<ApiKeyDefinitionBuilder<TAssociation>>? configure = null)
    {
        base.WithForeignKeyB
        (
            configure == null
                ? null
                : builder => configure((ApiKeyDefinitionBuilder<TAssociation>)builder)
        );
        return this;
    }

    #endregion
}
