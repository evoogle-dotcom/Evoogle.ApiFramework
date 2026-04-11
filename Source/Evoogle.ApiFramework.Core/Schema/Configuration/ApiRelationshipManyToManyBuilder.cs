// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

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
    private string? _apiAssociationTypeName;
    private ApiRelationshipPrincipalEndBuilder? _principalEndABuilder;
    private ApiRelationshipPrincipalEndBuilder? _principalEndBBuilder;
    private ApiRelationshipDependentEndBuilder? _dependentEndABuilder;
    private ApiRelationshipDependentEndBuilder? _dependentEndBBuilder;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Sets the API name of the association <see cref="ApiObjectType"/> that bridges both sides of the relationship.
    /// </summary>
    /// <param name="apiAssociationTypeName">The API name of the pre-registered association object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithAssociationTypeName(string apiAssociationTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiAssociationTypeName, nameof(apiAssociationTypeName));

        _apiAssociationTypeName = apiAssociationTypeName;
        return this;
    }

    /// <summary>
    ///     Configures principal end A — the first participating object type.
    /// </summary>
    /// <param name="apiObjectTypeName">The API name of principal end A's <see cref="ApiObjectType"/>.</param>
    /// <param name="configure">Optional callback to configure identity selection, delete behavior, and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithPrincipalEndA
    (
        string apiObjectTypeName,
        Action<ApiRelationshipPrincipalEndBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiObjectTypeName, nameof(apiObjectTypeName));

        var builder = new ApiRelationshipPrincipalEndBuilder(apiObjectTypeName);
        configure?.Invoke(builder);
        _principalEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures principal end B — the second participating object type.
    /// </summary>
    /// <param name="apiObjectTypeName">The API name of principal end B's <see cref="ApiObjectType"/>.</param>
    /// <param name="configure">Optional callback to configure identity selection, delete behavior, and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithPrincipalEndB
    (
        string apiObjectTypeName,
        Action<ApiRelationshipPrincipalEndBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiObjectTypeName, nameof(apiObjectTypeName));

        var builder = new ApiRelationshipPrincipalEndBuilder(apiObjectTypeName);
        configure?.Invoke(builder);
        _principalEndBBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures dependent end A, whose FK paths live on the association object type and point back
    ///     to principal end A.
    ///     The effective delete behavior is always <see cref="ApiRelationshipDeleteBehavior.Cascade"/>.
    /// </summary>
    /// <param name="apiObjectTypeName">The API name of the association <see cref="ApiObjectType"/>.</param>
    /// <param name="configure">Optional callback to add FK key paths and attach extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithDependentEndA
    (
        string apiObjectTypeName,
        Action<ApiRelationshipDependentEndBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiObjectTypeName, nameof(apiObjectTypeName));

        var builder = new ApiRelationshipDependentEndBuilder(apiObjectTypeName);
        configure?.Invoke(builder);
        _dependentEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures dependent end B, whose FK paths live on the association object type and point back
    ///     to principal end B.
    ///     The effective delete behavior is always <see cref="ApiRelationshipDeleteBehavior.Cascade"/>.
    /// </summary>
    /// <param name="apiObjectTypeName">The API name of the association <see cref="ApiObjectType"/>.</param>
    /// <param name="configure">Optional callback to add FK key paths and attach extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithDependentEndB
    (
        string apiObjectTypeName,
        Action<ApiRelationshipDependentEndBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiObjectTypeName, nameof(apiObjectTypeName));

        var builder = new ApiRelationshipDependentEndBuilder(apiObjectTypeName);
        configure?.Invoke(builder);
        _dependentEndBBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures principal end A using the CLR type <typeparamref name="TPrincipal"/> to resolve the API name
    ///     from the schema context.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal end A object type.</typeparam>
    /// <param name="configure">Optional callback to configure the principal end.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithPrincipalEndA<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var apiName = this.Context?.GetObjectTypeApiName(typeof(TPrincipal))
            ?? throw new ApiSchemaException($"Cannot resolve API name for type '{typeof(TPrincipal).Name}': no schema context is available. Use {nameof(WithPrincipalEndA)}(string, ...) or register the relationship through a schema builder.");
        return this.WithPrincipalEndA(apiName, configure);
    }

    /// <summary>
    ///     Configures principal end B using the CLR type <typeparamref name="TPrincipal"/> to resolve the API name
    ///     from the schema context.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal end B object type.</typeparam>
    /// <param name="configure">Optional callback to configure the principal end.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithPrincipalEndB<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var apiName = this.Context?.GetObjectTypeApiName(typeof(TPrincipal))
            ?? throw new ApiSchemaException($"Cannot resolve API name for type '{typeof(TPrincipal).Name}': no schema context is available. Use {nameof(WithPrincipalEndB)}(string, ...) or register the relationship through a schema builder.");
        return this.WithPrincipalEndB(apiName, configure);
    }

    /// <summary>
    ///     Configures dependent end A using the CLR type <typeparamref name="TDependent"/> to resolve the API name
    ///     from the schema context. The typed builder allows expression-based FK path configuration.
    ///     The effective delete behavior is always <see cref="ApiRelationshipDeleteBehavior.Cascade"/>.
    /// </summary>
    /// <typeparam name="TDependent">The CLR type of the dependent (association) object type.</typeparam>
    /// <param name="configure">Optional callback to add FK key paths and attach extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithDependentEndA<TDependent>(Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null)
    {
        var apiName = this.Context?.GetObjectTypeApiName(typeof(TDependent))
            ?? throw new ApiSchemaException($"Cannot resolve API name for type '{typeof(TDependent).Name}': no schema context is available. Use {nameof(WithDependentEndA)}(string, ...) or register the relationship through a schema builder.");
        var builder = new ApiRelationshipDependentEndBuilder<TDependent>(apiName);
        configure?.Invoke(builder);
        _dependentEndABuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures dependent end B using the CLR type <typeparamref name="TDependent"/> to resolve the API name
    ///     from the schema context. The typed builder allows expression-based FK path configuration.
    ///     The effective delete behavior is always <see cref="ApiRelationshipDeleteBehavior.Cascade"/>.
    /// </summary>
    /// <typeparam name="TDependent">The CLR type of the dependent (association) object type.</typeparam>
    /// <param name="configure">Optional callback to add FK key paths and attach extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipManyToManyBuilder WithDependentEndB<TDependent>(Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null)
    {
        var apiName = this.Context?.GetObjectTypeApiName(typeof(TDependent))
            ?? throw new ApiSchemaException($"Cannot resolve API name for type '{typeof(TDependent).Name}': no schema context is available. Use {nameof(WithDependentEndB)}(string, ...) or register the relationship through a schema builder.");
        var builder = new ApiRelationshipDependentEndBuilder<TDependent>(apiName);
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
            _apiAssociationTypeName!,
            this.ApiDisplayName,
            this.ApiDescription
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
