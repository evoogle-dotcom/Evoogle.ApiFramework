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
    private readonly Internal.ApiRelationshipEndsState _endsState = new();
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder AddRelationshipExtension(Type extensionType, object extension)
        => base.AddRelationshipExtension<ApiRelationshipManyToManyBuilder>(extensionType, extension);
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
        ArgumentNullException.ThrowIfNull(clrPrincipalType);

        var source = this.CurrentConfigurationSource;
        if
        (
            _endsState.PrincipalEndA != null &&
            _endsState.PrincipalEndA.ClrObjectType == clrPrincipalType &&
            _endsState.PrincipalEndASource != null &&
            source < _endsState.PrincipalEndASource.Value
        )
        {
            if (configure != null)
            {
                _endsState.PrincipalEndA.ApplyConfiguration
                (
                    source,
                    () => configure(_endsState.PrincipalEndA)
                );
            }

            return this;
        }

        if (_endsState.PrincipalEndASource != null && source < _endsState.PrincipalEndASource.Value)
        {
            return this;
        }

        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        if (configure != null)
        {
            builder.ApplyConfiguration(source, () => configure(builder));
        }

        _endsState.PrincipalEndA = builder;
        _endsState.PrincipalEndASource = source;
        return this;
    }

    /// <summary>
    ///     Configures principal end A of the relationship using the specified CLR type,
    ///     and selects the named principal key type for the relationship.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal end A object type.</param>
    /// <param name="apiPrincipalKeyTypeName">The name of the principal key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder Between(Type clrPrincipalType, string apiPrincipalKeyTypeName)
    {
        ArgumentNullException.ThrowIfNull(clrPrincipalType);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPrincipalKeyTypeName, nameof(apiPrincipalKeyTypeName));

        return this.Between
        (
            clrPrincipalType,
            builder => builder.WithPrincipalKey(apiPrincipalKeyTypeName)
        );
    }

    /// <summary>
    ///     Configures principal end B of the relationship using the specified CLR type.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal end B object type.</param>
    /// <param name="configure">Optional callback to configure principal key type selection and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder And(Type clrPrincipalType, Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(clrPrincipalType);

        var source = this.CurrentConfigurationSource;
        if
        (
            _endsState.PrincipalEndB != null &&
            _endsState.PrincipalEndB.ClrObjectType == clrPrincipalType &&
            _endsState.PrincipalEndBSource != null &&
            source < _endsState.PrincipalEndBSource.Value
        )
        {
            if (configure != null)
            {
                _endsState.PrincipalEndB.ApplyConfiguration
                (
                    source,
                    () => configure(_endsState.PrincipalEndB)
                );
            }

            return this;
        }

        if (_endsState.PrincipalEndBSource != null && source < _endsState.PrincipalEndBSource.Value)
        {
            return this;
        }

        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        if (configure != null)
        {
            builder.ApplyConfiguration(source, () => configure(builder));
        }

        _endsState.PrincipalEndB = builder;
        _endsState.PrincipalEndBSource = source;
        return this;
    }

    /// <summary>
    ///     Configures principal end B of the relationship using the specified CLR type,
    ///     and selects the named principal key type for the relationship.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal end B object type.</param>
    /// <param name="apiPrincipalKeyTypeName">The name of the principal key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder And(Type clrPrincipalType, string apiPrincipalKeyTypeName)
    {
        ArgumentNullException.ThrowIfNull(clrPrincipalType);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPrincipalKeyTypeName, nameof(apiPrincipalKeyTypeName));

        return this.And
        (
            clrPrincipalType,
            builder => builder.WithPrincipalKey(apiPrincipalKeyTypeName)
        );
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
        ArgumentNullException.ThrowIfNull(clrAssociationType);

        var source = this.CurrentConfigurationSource;
        if (_endsState.Association != null && _endsState.Association.ClrObjectType == clrAssociationType)
        {
            if (configure != null)
            {
                _endsState.Association.ApplyConfiguration(source, () => configure(_endsState.Association));
            }

            if (_endsState.AssociationSource == null || source >= _endsState.AssociationSource.Value)
            {
                _endsState.AssociationSource = source;
            }

            return this;
        }

        if
        (
            _endsState.Association != null &&
            _endsState.Association.ClrObjectType == clrAssociationType &&
            _endsState.AssociationSource != null &&
            source < _endsState.AssociationSource.Value
        )
        {
            if (configure != null)
            {
                _endsState.Association.ApplyConfiguration
                (
                    source,
                    () => configure(_endsState.Association)
                );
            }

            return this;
        }

        if (_endsState.AssociationSource != null && source < _endsState.AssociationSource.Value)
        {
            return this;
        }

        var builder = Internal.ApiBuilderFactory.CreateClosedGeneric<ApiRelationshipAssociationBuilder>
        (
            typeof(ApiRelationshipAssociationBuilder<>),
            clrAssociationType
        );
        if (configure != null)
        {
            builder.ApplyConfiguration(source, () => configure(builder));
        }

        _endsState.Association = builder;
        _endsState.AssociationSource = source;
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
        var apiPrincipalEndA = _endsState.PrincipalEndA?.Build()!;
        var apiPrincipalEndB = _endsState.PrincipalEndB?.Build()!;
        var apiAssociation = _endsState.Association?.Build()!;
        var apiDeleteBehavior = this.DeleteBehavior;

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

        var source = this.CurrentConfigurationSource;
        if
        (
            _endsState.Association != null &&
            _endsState.Association.ClrObjectType == builder.ClrObjectType &&
            _endsState.AssociationSource != null &&
            source < _endsState.AssociationSource.Value
        )
        {
            _endsState.Association.MergeConfigurationFrom(builder);
            return this;
        }

        if (_endsState.AssociationSource == null || source >= _endsState.AssociationSource.Value)
        {
            _endsState.Association = builder;
            _endsState.AssociationSource = source;
        }

        return this;
    }

    /// <summary>Runs a fluent callback at the supplied configuration-source precedence.</summary>
    internal void ApplyConfiguration
    (
        Internal.ApiConfigurationSource source,
        Action<ApiRelationshipManyToManyBuilder> configure
    )
    {
        base.ApplyConfiguration(source, () => configure(this));
    }
    #endregion
}
