// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Relationship convenience extension methods for <see cref="ApiSchemaBuilder"/>.
/// </summary>
public static class ApiSchemaBuilderRelationshipExtensions
{
    #region AddOneToOneRelationship Methods
    /// <summary>
    ///     Adds a one-to-one relationship with <typeparamref name="TPrincipal"/> as the principal type and <typeparamref name="TDependent"/> as the dependent type.
    /// </summary>
    public static ApiSchemaBuilder AddOneToOneRelationship<TPrincipal, TDependent>
    (
        this ApiSchemaBuilder builder,
        string apiName,
        Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        return builder.AddOneToOneRelationship(apiName, r => r
            .From<TPrincipal>()
            .To(configure));
    }

    /// <summary>
    ///     Adds a one-to-one relationship with a single scalar foreign key property on <typeparamref name="TDependent"/>.
    /// </summary>
    public static ApiSchemaBuilder AddOneToOneRelationship<TPrincipal, TDependent>
    (
        this ApiSchemaBuilder builder,
        string apiName,
        Expression<Func<TDependent, object>> fk
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(fk);

        return builder.AddOneToOneRelationship<TPrincipal, TDependent>(apiName, d => d.WithForeignKey(fk));
    }
    #endregion

    #region AddOneToManyRelationship Methods
    /// <summary>
    ///     Adds a one-to-many relationship with <typeparamref name="TPrincipal"/> as the principal type and <typeparamref name="TDependent"/> as the dependent type.
    /// </summary>
    public static ApiSchemaBuilder AddOneToManyRelationship<TPrincipal, TDependent>
    (
        this ApiSchemaBuilder builder,
        string apiName,
        Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        return builder.AddOneToManyRelationship(apiName, r => r
            .From<TPrincipal>()
            .To(configure));
    }

    /// <summary>
    ///     Adds a one-to-many relationship with a single scalar foreign key property on <typeparamref name="TDependent"/>.
    /// </summary>
    public static ApiSchemaBuilder AddOneToManyRelationship<TPrincipal, TDependent>
    (
        this ApiSchemaBuilder builder,
        string apiName,
        Expression<Func<TDependent, object>> fk
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(fk);

        return builder.AddOneToManyRelationship<TPrincipal, TDependent>(apiName, d => d.WithForeignKey(fk));
    }
    #endregion

    #region AddManyToManyRelationship Methods
    /// <summary>
    ///     Adds a many-to-many relationship with a single scalar key property for each association foreign key role.
    /// </summary>
    public static ApiSchemaBuilder AddManyToManyRelationship<TPrincipalA, TPrincipalB, TAssociation>
    (
        this ApiSchemaBuilder builder,
        string apiName,
        Expression<Func<TAssociation, object>> fkA,
        Expression<Func<TAssociation, object>> fkB
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(fkA);
        ArgumentNullException.ThrowIfNull(fkB);

        return builder.AddManyToManyRelationship(apiName, r => r
            .Between<TPrincipalA>()
            .And<TPrincipalB>()
            .WithAssociation<TAssociation>(a => a
                .WithForeignKeyA(fkA)
                .WithForeignKeyB(fkB)));
    }
    #endregion
}
