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
