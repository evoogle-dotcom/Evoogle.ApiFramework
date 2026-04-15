// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiRelationshipManyToMany"/> relationship
///     whose association CLR type is <typeparamref name="TAssociation"/>.
///     Extends <see cref="ApiRelationshipManyToManyBuilder"/> so that <see cref="WithDependentEndA"/> and
///     <see cref="WithDependentEndB"/> require no explicit type argument — the association type is already
///     fixed at construction time.
/// </summary>
/// <typeparam name="TAssociation">The CLR type of the association object type that mediates the relationship.</typeparam>
public sealed class ApiRelationshipManyToManyBuilder<TAssociation>
    : ApiRelationshipManyToManyBuilder
{
    #region Constructors
    /// <summary>
    ///     Initializes a new typed many-to-many builder, fixing the association CLR type to
    ///     <typeparamref name="TAssociation"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    public ApiRelationshipManyToManyBuilder(string apiName) : base(apiName)
    {
        _apiAssociationType = typeof(TAssociation);
    }
    #endregion

    #region With Methods
    /// <summary>
    ///     No-op; the association CLR type is already fixed to <typeparamref name="TAssociation"/> by the constructor.
    ///     Shadowed to preserve the typed return when called in a fluent chain on a typed builder instance.
    /// </summary>
    public new ApiRelationshipManyToManyBuilder<TAssociation> WithAssociationType<T>()
        => this;

    /// <inheritdoc cref="ApiRelationshipManyToManyBuilder.WithPrincipalEndA{TPrincipal}"/>
    public new ApiRelationshipManyToManyBuilder<TAssociation> WithPrincipalEndA<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        base.WithPrincipalEndA<TPrincipal>(configure);
        return this;
    }

    /// <inheritdoc cref="ApiRelationshipManyToManyBuilder.WithPrincipalEndB{TPrincipal}"/>
    public new ApiRelationshipManyToManyBuilder<TAssociation> WithPrincipalEndB<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        base.WithPrincipalEndB<TPrincipal>(configure);
        return this;
    }

    /// <summary>
    ///     Configures dependent end A targeting the association type <typeparamref name="TAssociation"/>.
    ///     The effective delete behavior is always <see cref="ApiRelationshipDeleteBehavior.Cascade"/>.
    /// </summary>
    /// <param name="configure">Optional callback to add FK key paths and attach extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder<TAssociation> WithDependentEndA(Action<ApiRelationshipDependentEndBuilder<TAssociation>>? configure = null)
    {
        var builder = new ApiRelationshipDependentEndBuilder<TAssociation>();
        configure?.Invoke(builder);
        _dependentEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures dependent end B targeting the association type <typeparamref name="TAssociation"/>.
    ///     The effective delete behavior is always <see cref="ApiRelationshipDeleteBehavior.Cascade"/>.
    /// </summary>
    /// <param name="configure">Optional callback to add FK key paths and attach extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder<TAssociation> WithDependentEndB(Action<ApiRelationshipDependentEndBuilder<TAssociation>>? configure = null)
    {
        var builder = new ApiRelationshipDependentEndBuilder<TAssociation>();
        configure?.Invoke(builder);
        _dependentEndBBuilder = builder;
        return this;
    }
    #endregion
}
