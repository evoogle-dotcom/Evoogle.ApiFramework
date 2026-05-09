// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiRelationshipManyToManyBuilder(string apiName) : ApiRelationshipBuilder(apiName)
{
    #region Fields
    protected ApiRelationshipPrincipalEndBuilder? _principalEndABuilder;
    protected ApiRelationshipPrincipalEndBuilder? _principalEndBBuilder;
    protected ApiRelationshipAssociationBuilder? _associationBuilder;
    protected ApiRelationshipDeleteBehavior _apiDeleteBehavior = ApiRelationshipManyToMany.DefaultDeleteBehavior;
    #endregion

    #region Non-Generic With Methods
    public ApiRelationshipManyToManyBuilder WithPrincipalEndA(Type clrPrincipalType, Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        configure?.Invoke(builder);
        _principalEndABuilder = builder;
        return this;
    }

    public ApiRelationshipManyToManyBuilder WithPrincipalEndB(Type clrPrincipalType, Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        configure?.Invoke(builder);
        _principalEndBBuilder = builder;
        return this;
    }

    public ApiRelationshipManyToManyBuilder WithAssociation(Type clrAssociationType, Action<ApiRelationshipAssociationBuilder>? configure = null)
    {
        var builder = new ApiRelationshipAssociationBuilder(clrAssociationType);
        configure?.Invoke(builder);
        _associationBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Sets the delete behavior for the relationship.
    /// </summary>
    /// <param name="apiDeleteBehavior">The desired delete behavior.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithDeleteBehavior(ApiRelationshipDeleteBehavior apiDeleteBehavior)
    {
        _apiDeleteBehavior = apiDeleteBehavior;
        return this;
    }
    #endregion

    #region Generic With Methods
    /// <summary>
    ///     Configures principal end A using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal end A object type.</typeparam>
    /// <param name="configure">Optional callback to configure the principal end.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithPrincipalEndA<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(typeof(TPrincipal));
        configure?.Invoke(builder);
        _principalEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures principal end B using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal end B object type.</typeparam>
    /// <param name="configure">Optional callback to configure the principal end.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithPrincipalEndB<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(typeof(TPrincipal));
        configure?.Invoke(builder);
        _principalEndBBuilder = builder;
        return this;
    }

    public ApiRelationshipManyToManyBuilder WithAssociation<TAssociation>(Action<ApiRelationshipAssociationBuilder<TAssociation>>? configure = null)
    {
        var builder = new ApiRelationshipAssociationBuilder<TAssociation>();
        configure?.Invoke(builder);
        _associationBuilder = builder;
        return this;
    }
    #endregion

    #region Build Methods
    /// <inheritdoc/>
    internal override ApiRelationship Build()
    {
        var apiName = this.ApiName;
        var apiPrincipalEndA = _principalEndABuilder?.Build()!;
        var apiPrincipalEndB = _principalEndBBuilder?.Build()!;
        var apiAssociation = _associationBuilder?.Build()!;
        var apiDeleteBehavior = _apiDeleteBehavior;

        var relationship = new ApiRelationshipManyToMany
        (
            apiName,
            apiPrincipalEndA,
            apiPrincipalEndB,
            apiAssociation,
            apiDeleteBehavior
        );

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            relationship.Extensions = extensions;
        }

        return relationship;
    }
    #endregion
}
