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
///     Call <see cref="Between(Type, Action{ApiRelationshipPrincipalEndBuilder}?)"/>,
///     <see cref="And(Type, Action{ApiRelationshipPrincipalEndBuilder}?)"/>,
///     and <see cref="WithAssociation(Type, Action{ApiRelationshipAssociationBuilder}?)"/> to define
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
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder AddRelationshipExtension(Type type, object extension)
        => base.AddRelationshipExtension<ApiRelationshipManyToManyBuilder>(type, extension);
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
    ///     and selects the named principal key type for the relationship.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal end A object type.</param>
    /// <param name="apiKeyTypeName">The name of the principal key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder Between(Type clrPrincipalType, string apiKeyTypeName)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        builder.WithPrincipalKey(apiKeyTypeName);
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
    ///     and selects the named principal key type for the relationship.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal end B object type.</param>
    /// <param name="apiKeyTypeName">The name of the principal key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder And(Type clrPrincipalType, string apiKeyTypeName)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        builder.WithPrincipalKey(apiKeyTypeName);
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

    #region Implementation Methods
    /// <summary>
    ///     Allows extension methods to set a pre-constructed association builder.
    /// </summary>
    internal ApiRelationshipManyToManyBuilder SetAssociationBuilderCore(ApiRelationshipAssociationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _associationBuilder = builder;
        return this;
    }
    #endregion
}
