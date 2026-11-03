// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Linq.Expressions;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Relationships;
using Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed partial class ApiSchemaBuilder
{
    #region Navigation Methods
    /// <summary>Explicitly attaches an inverse member to an existing relationship declaration.</summary>
    /// <typeparam name="TSource">The CLR type exposing an existing navigation.</typeparam>
    /// <typeparam name="TInverse">The CLR type exposing the inverse navigation.</typeparam>
    /// <param name="existingNavigation">The existing direct navigation selector.</param>
    /// <param name="inverseNavigation">The inverse direct navigation selector.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder AssociateInverseNavigation<TSource, TInverse>
    (
        Expression<Func<TSource, object?>> existingNavigation,
        Expression<Func<TInverse, object?>> inverseNavigation
    )
    {
        _context.AssociateInverseNavigation(ApiNavigationLocator.FromExpression(existingNavigation),
            ApiNavigationLocator.FromExpression(inverseNavigation));
        return this;
    }

    internal TBuilder? AddMemberRelationship<TBuilder>
    (
        string apiName,
        Type clrTypeA,
        Type clrTypeB,
        ApiNavigationLocator? navigationA,
        ApiNavigationLocator? navigationB,
        Func<string, TBuilder> factory,
        Action<TBuilder>? configure
    ) where TBuilder : ApiRelationshipBuilder
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        var bindings = new List<ApiRelationshipNavigationBinding>();
        if (navigationA is not null)
        {
            bindings.Add(new(navigationA, 0));
        }
        if (navigationB is not null)
        {
            bindings.Add(new(navigationB, 1));
        }
        var builder = _context.ResolveRelationship(apiName, bindings, factory);
        if (builder is null)
        {
            return null;
        }
        builder.ApplyConfiguration(ApiConfigurationSource.Convention, () =>
        {
            switch (builder)
            {
                case ApiRelationshipOneToOneBuilder oneToOne:
                    if (builder.EndsState.PrincipalEndA is null)
                    {
                        oneToOne.From(clrTypeA);
                    }
                    if (builder.EndsState.DependentEnd is null)
                    {
                        oneToOne.To(clrTypeB);
                    }
                    break;
                case ApiRelationshipOneToManyBuilder oneToMany:
                    if (builder.EndsState.PrincipalEndA is null)
                    {
                        oneToMany.From(clrTypeA);
                    }
                    if (builder.EndsState.DependentEnd is null)
                    {
                        oneToMany.To(clrTypeB);
                    }
                    break;
                case ApiRelationshipManyToManyBuilder manyToMany:
                    if (builder.EndsState.PrincipalEndA is null)
                    {
                        manyToMany.Between(clrTypeA);
                    }
                    if (builder.EndsState.PrincipalEndB is null)
                    {
                        manyToMany.And(clrTypeB);
                    }
                    break;
            }
        });
        if (configure is not null)
        {
            builder.ApplyConfiguration(_context.CurrentConfigurationSource, () => configure(builder));
        }
        return builder;
    }

    private ApiSchemaBuilder ConfigureMemberRelationship<TSource, TBuilder>
    (
        Expression<Func<TSource, object?>> navigation,
        Action<TBuilder> configure
    ) where TBuilder : ApiRelationshipBuilder
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = _context.FindRelationship(ApiNavigationLocator.FromExpression(navigation));
        if (builder is TBuilder typed)
        {
            typed.ApplyConfiguration(_context.CurrentConfigurationSource, () => configure(typed));
        }
        else if (builder is not null && _context.CurrentConfigurationSource >= builder.RegistrationSource)
        {
            _context.AddRelationshipIssue(ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict,
                builder.ConfiguredApiName, "The navigation belongs to another relationship kind", builder);
        }
        return this;
    }
    #endregion

    #region Relationship Configuration Methods
    private ApiSchemaBuilder ConfigureRelationshipCore<TBuilder>(object owner, object identity,
        Action<TBuilder> configure)
        where TBuilder : ApiRelationshipBuilder
    {
        ArgumentNullException.ThrowIfNull(configure);
        this.ValidateRegistrationOwner(owner);
        if (_context.GetRelationship(identity) is not TBuilder builder)
        {
            throw new ApiSchemaConfigurationException("The registration has an incompatible relationship kind.");
        }
        builder.ApplyConfiguration(_context.CurrentConfigurationSource, () => configure(builder));
        return this;
    }
    #endregion

    #region OneToOne Registration Methods
    /// <summary>Declares an independent one-to-one relationship and returns its durable authoring handle.</summary>
    /// <param name="apiName">The explicit relationship API name.</param>
    /// <param name="configure">The relationship configuration.</param>
    /// <returns>An opaque kind-safe registration owned by this builder.</returns>
    /// <exception cref="ApiSchemaConfigurationException">A schema build is active.</exception>
    public ApiRelationshipOneToOneRegistration DeclareOneToOneRelationship
    (
        string apiName,
        Action<ApiRelationshipOneToOneBuilder> configure
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        ArgumentNullException.ThrowIfNull(configure);
        this.EnsureDurableRegistration();
        var identity = new object();
        var builder = _context.DeclareRelationship(identity, apiName, static name => new ApiRelationshipOneToOneBuilder(name));
        builder.ApplyConfiguration(_context.CurrentConfigurationSource, () => configure(builder));
        return new(_registrationOwner, identity);
    }

    /// <summary>Configures a previously declared one-to-one relationship.</summary>
    /// <param name="registration">The registration created by this builder.</param>
    /// <param name="configure">The additional relationship configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder ConfigureRelationship
    (
        ApiRelationshipOneToOneRegistration registration,
        Action<ApiRelationshipOneToOneBuilder> configure
    )
    {
        ArgumentNullException.ThrowIfNull(registration);
        return this.ConfigureRelationshipCore(registration.Owner, registration.Identity, configure);
    }

    /// <summary>Declares or configures a one-to-one relationship through its navigation members.</summary>
    /// <typeparam name="TPrincipal">The CLR type of the first endpoint.</typeparam>
    /// <typeparam name="TDependent">The CLR type of the opposite endpoint.</typeparam>
    /// <param name="apiName">The explicit relationship API name.</param>
    /// <param name="principalNavigation">The first endpoint navigation, or null.</param>
    /// <param name="dependentNavigation">The opposite endpoint navigation, or null.</param>
    /// <param name="configure">Optional relationship configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder AddOneToOneRelationship<TPrincipal, TDependent>
    (
        string apiName,
        Expression<Func<TPrincipal, object?>>? principalNavigation,
        Expression<Func<TDependent, object?>>? dependentNavigation,
        Action<ApiRelationshipOneToOneBuilder>? configure = null
    )
    {
        if (principalNavigation is null && dependentNavigation is null)
        {
            throw new ArgumentException("Supply at least one navigation selector.", nameof(principalNavigation));
        }
        this.AddMemberRelationship
        (
            apiName,
            typeof(TPrincipal),
            typeof(TDependent),
            principalNavigation is null ? null : ApiNavigationLocator.FromExpression(principalNavigation),
            dependentNavigation is null ? null : ApiNavigationLocator.FromExpression(dependentNavigation),
            static name => new ApiRelationshipOneToOneBuilder(name),
            configure
        );
        return this;
    }

    /// <summary>Configures the one-to-one relationship located by an existing navigation member.</summary>
    /// <typeparam name="TSource">The CLR type exposing the navigation.</typeparam>
    /// <param name="navigation">The direct navigation selector.</param>
    /// <param name="configure">The additional relationship configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder ConfigureOneToOneRelationship<TSource>
    (
        Expression<Func<TSource, object?>> navigation,
        Action<ApiRelationshipOneToOneBuilder> configure
    )
        => this.ConfigureMemberRelationship(navigation, configure);
    #endregion

    #region OneToMany Registration Methods
    /// <summary>Declares an independent one-to-many relationship and returns its durable authoring handle.</summary>
    /// <param name="apiName">The explicit relationship API name.</param>
    /// <param name="configure">The relationship configuration.</param>
    /// <returns>An opaque kind-safe registration owned by this builder.</returns>
    /// <exception cref="ApiSchemaConfigurationException">A schema build is active.</exception>
    public ApiRelationshipOneToManyRegistration DeclareOneToManyRelationship
    (
        string apiName,
        Action<ApiRelationshipOneToManyBuilder> configure
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        ArgumentNullException.ThrowIfNull(configure);
        this.EnsureDurableRegistration();
        var identity = new object();
        var builder = _context.DeclareRelationship(identity, apiName, static name =>
            new ApiRelationshipOneToManyBuilder(name));
        builder.ApplyConfiguration(_context.CurrentConfigurationSource, () => configure(builder));
        return new(_registrationOwner, identity);
    }

    /// <summary>Configures a previously declared one-to-many relationship.</summary>
    /// <param name="registration">The registration created by this builder.</param>
    /// <param name="configure">The additional relationship configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder ConfigureRelationship
    (
        ApiRelationshipOneToManyRegistration registration,
        Action<ApiRelationshipOneToManyBuilder> configure
    )
    {
        ArgumentNullException.ThrowIfNull(registration);
        return this.ConfigureRelationshipCore(registration.Owner, registration.Identity, configure);
    }

    /// <summary>Declares or configures a one-to-many relationship through its navigation members.</summary>
    /// <typeparam name="TPrincipal">The CLR type of the first endpoint.</typeparam>
    /// <typeparam name="TDependent">The CLR type of the opposite endpoint.</typeparam>
    /// <param name="apiName">The explicit relationship API name.</param>
    /// <param name="principalNavigation">The first endpoint navigation, or null.</param>
    /// <param name="dependentNavigation">The opposite endpoint navigation, or null.</param>
    /// <param name="configure">Optional relationship configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder AddOneToManyRelationship<TPrincipal, TDependent>
    (
        string apiName,
        Expression<Func<TPrincipal, object?>>? principalNavigation,
        Expression<Func<TDependent, object?>>? dependentNavigation,
        Action<ApiRelationshipOneToManyBuilder>? configure = null
    )
    {
        if (principalNavigation is null && dependentNavigation is null)
        {
            throw new ArgumentException("Supply at least one navigation selector.", nameof(principalNavigation));
        }
        this.AddMemberRelationship
        (
            apiName,
            typeof(TPrincipal),
            typeof(TDependent),
            principalNavigation is null ? null : ApiNavigationLocator.FromExpression(principalNavigation),
            dependentNavigation is null ? null : ApiNavigationLocator.FromExpression(dependentNavigation),
            static name => new ApiRelationshipOneToManyBuilder(name),
            configure
        );
        return this;
    }

    /// <summary>Configures the one-to-many relationship located by an existing navigation member.</summary>
    /// <typeparam name="TSource">The CLR type exposing the navigation.</typeparam>
    /// <param name="navigation">The direct navigation selector.</param>
    /// <param name="configure">The additional relationship configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder ConfigureOneToManyRelationship<TSource>
    (
        Expression<Func<TSource, object?>> navigation,
        Action<ApiRelationshipOneToManyBuilder> configure
    )
        => this.ConfigureMemberRelationship(navigation, configure);
    #endregion

    #region ManyToMany Registration Methods
    /// <summary>Declares an independent many-to-many relationship and returns its durable authoring handle.</summary>
    /// <param name="apiName">The explicit relationship API name.</param>
    /// <param name="configure">The relationship configuration.</param>
    /// <returns>An opaque kind-safe registration owned by this builder.</returns>
    /// <exception cref="ApiSchemaConfigurationException">A schema build is active.</exception>
    public ApiRelationshipManyToManyRegistration DeclareManyToManyRelationship
    (
        string apiName,
        Action<ApiRelationshipManyToManyBuilder> configure
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        ArgumentNullException.ThrowIfNull(configure);
        this.EnsureDurableRegistration();
        var identity = new object();
        var builder = _context.DeclareRelationship(identity, apiName, static name =>
            new ApiRelationshipManyToManyBuilder(name));
        builder.ApplyConfiguration(_context.CurrentConfigurationSource, () => configure(builder));
        return new(_registrationOwner, identity);
    }

    /// <summary>Configures a previously declared many-to-many relationship.</summary>
    /// <param name="registration">The registration created by this builder.</param>
    /// <param name="configure">The additional relationship configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder ConfigureRelationship
    (
        ApiRelationshipManyToManyRegistration registration,
        Action<ApiRelationshipManyToManyBuilder> configure
    )
    {
        ArgumentNullException.ThrowIfNull(registration);
        return this.ConfigureRelationshipCore(registration.Owner, registration.Identity, configure);
    }

    /// <summary>Declares or configures a many-to-many relationship through its navigation members.</summary>
    /// <typeparam name="TPrincipal">The CLR type of the first endpoint.</typeparam>
    /// <typeparam name="TDependent">The CLR type of the opposite endpoint.</typeparam>
    /// <param name="apiName">The explicit relationship API name.</param>
    /// <param name="principalNavigation">The first endpoint navigation, or null.</param>
    /// <param name="dependentNavigation">The opposite endpoint navigation, or null.</param>
    /// <param name="configure">Optional relationship configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder AddManyToManyRelationship<TPrincipal, TDependent>
    (
        string apiName,
        Expression<Func<TPrincipal, object?>>? principalNavigation,
        Expression<Func<TDependent, object?>>? dependentNavigation,
        Action<ApiRelationshipManyToManyBuilder>? configure = null
    )
    {
        if (principalNavigation is null && dependentNavigation is null)
        {
            throw new ArgumentException("Supply at least one navigation selector.", nameof(principalNavigation));
        }
        this.AddMemberRelationship
        (
            apiName,
            typeof(TPrincipal),
            typeof(TDependent),
            principalNavigation is null ? null : ApiNavigationLocator.FromExpression(principalNavigation),
            dependentNavigation is null ? null : ApiNavigationLocator.FromExpression(dependentNavigation),
            static name => new ApiRelationshipManyToManyBuilder(name),
            configure
        );
        return this;
    }

    /// <summary>Configures the many-to-many relationship located by an existing navigation member.</summary>
    /// <typeparam name="TSource">The CLR type exposing the navigation.</typeparam>
    /// <param name="navigation">The direct navigation selector.</param>
    /// <param name="configure">The additional relationship configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder ConfigureManyToManyRelationship<TSource>
    (
        Expression<Func<TSource, object?>> navigation,
        Action<ApiRelationshipManyToManyBuilder> configure
    )
        => this.ConfigureMemberRelationship(navigation, configure);
    #endregion
}
