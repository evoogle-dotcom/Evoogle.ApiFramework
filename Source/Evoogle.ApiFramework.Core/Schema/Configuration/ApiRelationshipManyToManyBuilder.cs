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
///     A many-to-many relationship requires an explicit association <see cref="ApiObjectType"/> that holds the
///     foreign-key columns for both sides.  Both dependent ends always use
///     <see cref="ApiRelationshipDeleteBehavior.Cascade"/>, regardless of any delete behavior set on their
///     individual builders.
/// </remarks>
/// <param name="apiName">The schema-unique API name of the relationship.</param>
public class ApiRelationshipManyToManyBuilder(string apiName) : ApiRelationshipBuilder(apiName)
{
    #region Fields
    protected Type? _apiAssociationType;
    protected ApiRelationshipPrincipalEndBuilder? _principalEndABuilder;
    protected ApiRelationshipPrincipalEndBuilder? _principalEndBBuilder;
    protected ApiRelationshipDependentEndBuilder? _dependentEndABuilder;
    protected ApiRelationshipDependentEndBuilder? _dependentEndBBuilder;
    #endregion

    #region With Methods
    /// <summary>
    ///     Sets the CLR type of the association <see cref="ApiObjectType"/> that bridges both sides of the relationship.
    /// </summary>
    /// <param name="clrType">The CLR type of the association object.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithAssociationType(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        _apiAssociationType = clrType;
        return this;
    }

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

    /// <summary>
    ///     Configures dependent end A using the CLR type <typeparamref name="TDependent"/>. The typed builder allows
    ///     expression-based FK path configuration.
    ///     The effective delete behavior is always <see cref="ApiRelationshipDeleteBehavior.Cascade"/>.
    /// </summary>
    /// <typeparam name="TDependent">The CLR type of the dependent (association) object type.</typeparam>
    /// <param name="configure">Optional callback to add FK key paths and attach extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithDependentEndA<TDependent>(Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null)
    {
        var builder = new ApiRelationshipDependentEndBuilder<TDependent>();
        configure?.Invoke(builder);
        _dependentEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures dependent end B using the CLR type <typeparamref name="TDependent"/>. The typed builder allows
    ///     expression-based FK path configuration.
    ///     The effective delete behavior is always <see cref="ApiRelationshipDeleteBehavior.Cascade"/>.
    /// </summary>
    /// <typeparam name="TDependent">The CLR type of the dependent (association) object type.</typeparam>
    /// <param name="configure">Optional callback to add FK key paths and attach extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithDependentEndB<TDependent>(Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null)
    {
        var builder = new ApiRelationshipDependentEndBuilder<TDependent>();
        configure?.Invoke(builder);
        _dependentEndBBuilder = builder;
        return this;
    }
    #endregion

    #region Build Methods
    /// <inheritdoc/>
    internal override ApiRelationship Build()
    {
        var apiPrincipalEndA = _principalEndABuilder?.Build()!;
        var apiPrincipalEndB = _principalEndBBuilder?.Build()!;

        // Dependent ends always cascade — the forced delete behavior is injected here.
        var apiDependentEndA = BuildForcedCascadeEnd(_dependentEndABuilder);
        var apiDependentEndB = BuildForcedCascadeEnd(_dependentEndBBuilder);

        var relationship = new ApiRelationshipManyToMany
        (
            this.ApiName,
            apiPrincipalEndA,
            apiPrincipalEndB,
            apiDependentEndA!,
            apiDependentEndB!,
            _apiAssociationType!
        );

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            relationship.Extensions = extensions;
        }

        return relationship;
    }

    private static ApiRelationshipDependentEnd? BuildForcedCascadeEnd(ApiRelationshipDependentEndBuilder? builder)
    {
        if (builder is null)
        {
            return null;
        }

        // Build the end first, then promote the delete behavior to Cascade regardless of the
        // developer-configured value — M:N dependent ends always cascade.
        var end = builder.BuildWithForcedDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade);
        return end;
    }
    #endregion
}
