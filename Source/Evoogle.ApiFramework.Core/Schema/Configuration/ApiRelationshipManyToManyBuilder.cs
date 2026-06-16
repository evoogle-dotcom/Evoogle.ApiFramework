// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiRelationshipManyToMany"/> relationship.
/// </summary>
/// <remarks>
///     Call <see cref="Between{TPrincipal}(Action{ApiRelationshipPrincipalEndBuilder})"/>,
///     <see cref="And{TPrincipal}(Action{ApiRelationshipPrincipalEndBuilder})"/>,
///     and <see cref="WithAssociation{TAssociation}"/> (or their non-generic overloads) to define
///     both principal ends and the association object type.
///     Optionally call <see cref="WithDeleteBehavior"/> to override the default
///     (<see cref="ApiRelationshipManyToMany.DefaultDeleteBehavior"/>).
///     Subsequent calls to any configuration method for the same end replace the previous configuration.
/// </remarks>
/// <param name="apiName">The schema-unique API name of the relationship.</param>
public class ApiRelationshipManyToManyBuilder(string apiName)
    : ApiRelationshipBuilder(apiName, ApiRelationshipManyToMany.DefaultDeleteBehavior)
{
    #region Fields
    private ApiRelationshipPrincipalEndBuilder? _principalEndABuilder;
    private ApiRelationshipPrincipalEndBuilder? _principalEndBBuilder;
    private ApiRelationshipAssociationBuilder? _associationBuilder;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder AddRelationshipExtension(Type type, object value)
        => base.AddRelationshipExtension<ApiRelationshipManyToManyBuilder>(type, value);

    /// <summary>
    ///     Adds an extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="T">The extension value type.</typeparam>
    /// <param name="value">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder AddRelationshipExtension<T>(T value)
        where T : notnull
        => base.AddRelationshipExtension<ApiRelationshipManyToManyBuilder>(typeof(T), value);
    #endregion

    #region Non-Generic Between/And Methods
    /// <summary>
    ///     Configures principal end A of the relationship using the specified CLR type.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal end A object type.</param>
    /// <param name="configure">Optional callback to configure principal key type selection and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder Between(Type clrPrincipalType, Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        configure?.Invoke(builder);
        _principalEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures principal end A of the relationship using the specified CLR type,
    ///     and selects the named primary key type for the relationship.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal end A object type.</param>
    /// <param name="apiPrimaryKeyTypeName">The name of the primary key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder Between(Type clrPrincipalType, string apiPrimaryKeyTypeName)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        builder.WithPrimaryKey(apiPrimaryKeyTypeName);
        _principalEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures principal end B of the relationship using the specified CLR type.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal end B object type.</param>
    /// <param name="configure">Optional callback to configure principal key type selection and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder And(Type clrPrincipalType, Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        configure?.Invoke(builder);
        _principalEndBBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures principal end B of the relationship using the specified CLR type,
    ///     and selects the named primary key type for the relationship.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal end B object type.</param>
    /// <param name="apiPrimaryKeyTypeName">The name of the primary key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder And(Type clrPrincipalType, string apiPrimaryKeyTypeName)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        builder.WithPrimaryKey(apiPrimaryKeyTypeName);
        _principalEndBBuilder = builder;
        return this;
    }
    #endregion

    #region Non-Generic With Methods
    /// <summary>
    ///     Configures the association using the specified CLR type.
    /// </summary>
    /// <param name="clrAssociationType">The CLR type of the association <see cref="ApiObjectType"/>.</param>
    /// <param name="configure">Optional callback to add foreign key role key paths and extensions to the association.</param>
    /// <returns>The current builder instance.</returns>
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
        => base.WithDeleteBehavior<ApiRelationshipManyToManyBuilder>(apiDeleteBehavior);
    #endregion

    #region Generic Between/And Methods
    /// <summary>
    ///     Configures principal end A using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal end A object type.</typeparam>
    /// <param name="configure">Optional callback to configure the principal end.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder Between<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(typeof(TPrincipal));
        configure?.Invoke(builder);
        _principalEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures principal end A using the CLR type <typeparamref name="TPrincipal"/>,
    ///     and selects the named primary key type for the relationship.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal end A object type.</typeparam>
    /// <param name="apiPrimaryKeyTypeName">The name of the primary key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder Between<TPrincipal>(string apiPrimaryKeyTypeName)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(typeof(TPrincipal));
        builder.WithPrimaryKey(apiPrimaryKeyTypeName);
        _principalEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures principal end B using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal end B object type.</typeparam>
    /// <param name="configure">Optional callback to configure the principal end.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder And<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(typeof(TPrincipal));
        configure?.Invoke(builder);
        _principalEndBBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures principal end B using the CLR type <typeparamref name="TPrincipal"/>,
    ///     and selects the named primary key type for the relationship.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal end B object type.</typeparam>
    /// <param name="apiPrimaryKeyTypeName">The name of the primary key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder And<TPrincipal>(string apiPrimaryKeyTypeName)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(typeof(TPrincipal));
        builder.WithPrimaryKey(apiPrimaryKeyTypeName);
        _principalEndBBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures the association using the CLR type <typeparamref name="TAssociation"/>,
    ///     providing a strongly-typed builder for foreign key role key path configuration.
    /// </summary>
    /// <typeparam name="TAssociation">The CLR type of the association object.</typeparam>
    /// <param name="configure">Optional callback to add key paths and extensions using expression-based property selectors.</param>
    /// <returns>The current builder instance.</returns>
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
