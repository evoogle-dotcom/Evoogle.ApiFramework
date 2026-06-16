// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Extension methods for <see cref="ApiRelationshipAssociationBuilder{T}"/>.
/// </summary>
public static class ApiRelationshipAssociationBuilderGenericExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Adds an association extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The association builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipAssociationBuilder AddRelationshipAssociationExtension<TExtension>(this ApiRelationshipAssociationBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddRelationshipAssociationExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds an association extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TAssociation">The CLR association type represented by the builder.</typeparam>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The association builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipAssociationBuilder<TAssociation> AddRelationshipAssociationExtension<TAssociation, TExtension>
    (
        this ApiRelationshipAssociationBuilder<TAssociation> builder,
        TExtension extension
    )
        where TExtension : class
        => builder.AddRelationshipAssociationExtension(typeof(TExtension), extension);
    #endregion

    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the A-side foreign key role's <see cref="ApiKeyType"/> with a single key path using a type-safe expression.
    /// </summary>
    public static ApiRelationshipAssociationBuilder<T> WithForeignKeyA<T, TScalar>
    (
        this ApiRelationshipAssociationBuilder<T> builder,
        Expression<Func<T, TScalar>> expression
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(expression);

        return builder.WithForeignKeyA(b => b.AddPath(expression));
    }

    /// <summary>
    ///     Sets the B-side foreign key role's <see cref="ApiKeyType"/> with a single key path using a type-safe expression.
    /// </summary>
    public static ApiRelationshipAssociationBuilder<T> WithForeignKeyB<T, TScalar>
    (
        this ApiRelationshipAssociationBuilder<T> builder,
        Expression<Func<T, TScalar>> expression
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(expression);

        return builder.WithForeignKeyB(b => b.AddPath(expression));
    }
    #endregion
}
