// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiRelationshipOneToOne"/> relationship.
/// </summary>
/// <remarks>
///     Call <see cref="WithPrincipalEnd{TPrincipal}"/> and <see cref="WithDependentEnd{TDependent}"/> to configure the two ends.
///     At most one principal end and one dependent end are allowed; subsequent calls replace the previous
///     configuration for that end.
/// </remarks>
/// <param name="apiName">The schema-unique API name of the relationship.</param>
public sealed class ApiRelationshipOneToOneBuilder(string apiName) : ApiRelationshipBuilder(apiName)
{
    #region Fields
    private ApiRelationshipPrincipalEndBuilder? _principalEndBuilder;
    private ApiRelationshipDependentEndBuilder? _dependentEndBuilder;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Configures the principal end of the 1:1 relationship using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal object.</typeparam>
    /// <param name="configure">Optional callback to configure identity selection, delete behavior, and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToOneBuilder WithPrincipalEnd<TPrincipal>
    (
        Action<ApiRelationshipPrincipalEndBuilder>? configure = null
    )
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(typeof(TPrincipal));
        configure?.Invoke(builder);
        _principalEndBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures the dependent end of the 1:1 relationship using the CLR type <typeparamref name="TDependent"/>,
    ///     and provides a strongly-typed builder for FK key path configuration.
    /// </summary>
    /// <typeparam name="TDependent">The CLR type of the dependent object.</typeparam>
    /// <param name="configure">Optional callback to add FK key paths using expression-based property selectors.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToOneBuilder WithDependentEnd<TDependent>
    (
        Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null
    )
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
        var apiPrincipalEnd = _principalEndBuilder?.Build()!;
        var apiDependentEnd = _dependentEndBuilder?.Build()!;

        var relationship = new ApiRelationshipOneToOne
        (
            this.ApiName,
            apiPrincipalEnd,
            apiDependentEnd
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
