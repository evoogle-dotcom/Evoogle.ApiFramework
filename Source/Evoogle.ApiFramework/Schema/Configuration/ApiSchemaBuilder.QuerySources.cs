// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration.Query;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed partial class ApiSchemaBuilder
{
    #region Query Source Registration Methods
    /// <summary>Adds or configures the source identified by a public closed CLR delegate.</summary>
    /// <typeparam name="TDelegate">The delegate defining the source signature.</typeparam>
    /// <param name="configure">Optional API-facing configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder AddQuerySource<TDelegate>(Action<ApiQuerySourceBuilder>? configure = null)
        where TDelegate : Delegate
        => this.AddQuerySource(typeof(TDelegate), configure);

    /// <summary>Adds or configures the source identified by a public closed CLR delegate.</summary>
    /// <param name="clrDelegateType">The delegate defining the source signature.</param>
    /// <param name="configure">Optional API-facing configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder AddQuerySource(Type clrDelegateType, Action<ApiQuerySourceBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(clrDelegateType);
        var builder = _context.GetOrAddDelegateQuerySourceBuilder(clrDelegateType);
        var source = _context.CurrentConfigurationSource;
        if (configure is not null)
        {
            builder.ApplyConfiguration(source, () => configure(builder));
        }
        return this;
    }

    /// <summary>Applies a reusable configuration to its delegate-backed source.</summary>
    /// <typeparam name="TDelegate">The delegate defining the source signature.</typeparam>
    /// <param name="configuration">The delegate source configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder AddQuerySource<TDelegate>(IApiQuerySourceConfiguration<TDelegate> configuration)
        where TDelegate : Delegate
    {
        ArgumentNullException.ThrowIfNull(configuration);
        return this.AddQuerySource<TDelegate>(configuration.Configure);
    }

    /// <summary>Declares an independent fluent source and returns its durable authoring handle.</summary>
    /// <param name="apiName">The initial source naming candidate.</param>
    /// <param name="configure">The source configuration.</param>
    /// <returns>An opaque registration owned by this schema builder.</returns>
    /// <exception cref="ApiSchemaConfigurationException">A schema build is active.</exception>
    public ApiQuerySourceRegistration DeclareQuerySource(string apiName, Action<ApiQuerySourceBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        ArgumentNullException.ThrowIfNull(configure);
        this.EnsureDurableRegistration();
        var identity = new object();
        var source = _context.CurrentConfigurationSource;
        var builder = _context.DeclareQuerySourceBuilder(identity, apiName, source);
        _context.ApplyConfiguration(source, () => builder.ApplyConfiguration(source, () => configure(builder)));
        return new ApiQuerySourceRegistration(_registrationOwner, identity);
    }

    /// <summary>Configures a previously declared source without using its current API name.</summary>
    /// <param name="registration">The registration created by this builder.</param>
    /// <param name="configure">The additional source configuration.</param>
    /// <returns>The current schema builder.</returns>
    public ApiSchemaBuilder ConfigureQuerySource
    (
        ApiQuerySourceRegistration registration,
        Action<ApiQuerySourceBuilder> configure
    )
    {
        ArgumentNullException.ThrowIfNull(registration);
        ArgumentNullException.ThrowIfNull(configure);
        this.ValidateRegistrationOwner(registration.Owner);
        var builder = _context.GetQuerySourceBuilder(registration.Identity);
        builder.ApplyConfiguration(_context.CurrentConfigurationSource, () => configure(builder));
        return this;
    }

    private void ValidateRegistrationOwner(object owner)
    {
        if (!ReferenceEquals(owner, _registrationOwner))
        {
            throw new ApiSchemaConfigurationException("The registration belongs to a different schema builder.");
        }
    }
    #endregion

    #region Registration Lifetime Methods
    private void EnsureDurableRegistration()
    {
        if (Volatile.Read(ref _buildGuard._isBuilding) != 0)
        {
            throw new ApiSchemaConfigurationException
            (
                "Declare methods create durable authoring registrations before a build. " +
                "Use Add methods for build-local declarations."
            );
        }
    }
    #endregion
}
