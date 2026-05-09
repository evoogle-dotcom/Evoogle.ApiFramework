// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiRelationshipOneToMany"/> relationship.
/// </summary>
/// <remarks>
///     Call <see cref="WithPrincipalEnd{TPrincipal}"/>, <see cref="WithDependentEnd{TDependent}"/>, and optionally
///     <see cref="WithDeleteBehavior"/> to configure the relationship.
///     At most one principal end and one dependent end are allowed; subsequent calls replace the previous
///     configuration for that end.
/// </remarks>
/// <param name="apiName">The schema-unique API name of the relationship.</param>
public sealed class ApiRelationshipOneToManyBuilder(string apiName) : ApiRelationshipBuilder(apiName)
{
    #region Fields
    private ApiRelationshipPrincipalEndBuilder? _principalEndBuilder;
    private ApiRelationshipDependentEndBuilder? _dependentEndBuilder;
    private ApiRelationshipDeleteBehavior _apiDeleteBehavior = ApiRelationshipOneToMany.DefaultDeleteBehavior;
    #endregion

    #region Non-Generic With Methods
    /// <summary>
    ///     Configures the principal end of the 1:M relationship using the specified CLR type.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal object.</param>
    /// <param name="configure">Optional callback to configure identity selection and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder WithPrincipalEnd(Type clrPrincipalType, Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        configure?.Invoke(builder);
        _principalEndBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Sets the delete behavior for the relationship.
    /// </summary>
    /// <param name="apiDeleteBehavior">The desired delete behavior.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder WithDeleteBehavior(ApiRelationshipDeleteBehavior apiDeleteBehavior)
    {
        _apiDeleteBehavior = apiDeleteBehavior;
        return this;
    }

    /// <summary>
    ///     Configures the dependent end of the 1:M relationship using the specified CLR type.
    /// </summary>
    /// <param name="clrDependentType">The CLR type of the dependent object.</param>
    /// <param name="configure">Optional callback to add FK key paths and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder WithDependentEnd(Type clrDependentType, Action<ApiRelationshipDependentEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipDependentEndBuilder(clrDependentType);
        configure?.Invoke(builder);
        _dependentEndBuilder = builder;
        return this;
    }
    #endregion

    #region With Methods
    /// <summary>
    ///     Configures the principal end of the 1:M relationship using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal object.</typeparam>
    /// <param name="configure">Optional callback to configure identity selection and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder WithPrincipalEnd<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(typeof(TPrincipal));
        configure?.Invoke(builder);
        _principalEndBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures the dependent end of the 1:M relationship using the CLR type <typeparamref name="TDependent"/>,
    ///     and provides a strongly-typed builder for FK key path configuration.
    /// </summary>
    /// <typeparam name="TDependent">The CLR type of the dependent object.</typeparam>
    /// <param name="configure">Optional callback to add FK key paths using expression-based property selectors.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder WithDependentEnd<TDependent>(Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null)
    {
        var builder = new ApiRelationshipDependentEndBuilder<TDependent>();
        configure?.Invoke(builder);
        _dependentEndBuilder = builder;
        return this;
    }
    #endregion

    #region Build Methods
    /// <inheritdoc/>
    internal override ApiRelationship Build()
    {
        var apiName = this.ApiName;
        var apiPrincipalEnd = _principalEndBuilder?.Build()!;
        var apiDependentEnd = _dependentEndBuilder?.Build()!;
        var apiDeleteBehavior = _apiDeleteBehavior;

        var relationship = new ApiRelationshipOneToMany
        (
            apiName,
            apiPrincipalEnd,
            apiDependentEnd,
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
