// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides strongly-typed configuration for an <see cref="ApiRelationshipManyToMany"/> relationship
///     whose association CLR type is <typeparamref name="TAssociation"/>.
/// </summary>
/// <typeparam name="TAssociation">The CLR type of the association object type that mediates the relationship.</typeparam>
public interface IApiRelationshipManyToManyConfiguration<TAssociation>
{
    #region Methods
    /// <summary>
    ///     Applies configuration to the supplied <see cref="ApiRelationshipManyToManyBuilder{TAssociation}"/>.
    /// </summary>
    /// <param name="builder">The typed builder to configure.</param>
    void Configure(ApiRelationshipManyToManyBuilder<TAssociation> builder);
    #endregion
}
