// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiRelationshipOneToOne"/> relationship.
/// </summary>
/// <remarks>
///     Call <see cref="WithPrincipalEnd"/> and <see cref="WithDependentEnd"/> to configure the two ends.
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
    ///     Configures the principal end of the 1:1 relationship.
    /// </summary>
    /// <param name="apiObjectTypeName">The API name of the principal <see cref="ApiObjectType"/>.</param>
    /// <param name="configure">Optional callback to configure identity selection, delete behavior, and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToOneBuilder WithPrincipalEnd
    (
        string apiObjectTypeName,
        Action<ApiRelationshipPrincipalEndBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiObjectTypeName, nameof(apiObjectTypeName));

        var builder = new ApiRelationshipPrincipalEndBuilder(apiObjectTypeName);
        configure?.Invoke(builder);
        _principalEndBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures the dependent end of the 1:1 relationship.
    /// </summary>
    /// <param name="apiObjectTypeName">The API name of the dependent <see cref="ApiObjectType"/>.</param>
    /// <param name="configure">Optional callback to add FK key paths, set delete behavior, and attach extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToOneBuilder WithDependentEnd
    (
        string apiObjectTypeName,
        Action<ApiRelationshipDependentEndBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiObjectTypeName, nameof(apiObjectTypeName));

        var builder = new ApiRelationshipDependentEndBuilder(apiObjectTypeName);
        configure?.Invoke(builder);
        _dependentEndBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures the principal end using the API name registered for <typeparamref name="TPrincipal"/>.
    ///     Requires that <typeparamref name="TPrincipal"/> has already been registered via
    ///     <see cref="ApiSchemaBuilder.AddObject{T}(Action{ApiObjectTypeBuilder{T}})"/>.
    /// </summary>
    /// <typeparam name="TPrincipal">The CLR type of the principal object.</typeparam>
    /// <param name="configure">Optional callback to configure identity selection, delete behavior, and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToOneBuilder WithPrincipalEnd<TPrincipal>
    (
        Action<ApiRelationshipPrincipalEndBuilder>? configure = null
    )
    {
        var apiName = this.Context?.GetObjectTypeApiName(typeof(TPrincipal))
            ?? throw new ApiSchemaException(
                $"No schema builder context is available. Ensure the relationship is registered through {nameof(ApiSchemaBuilder)}.");

        return this.WithPrincipalEnd(apiName, configure);
    }

    /// <summary>
    ///     Configures the dependent end using the API name registered for <typeparamref name="TDependent"/>,
    ///     and provides a strongly-typed builder for FK key path configuration.
    ///     Requires that <typeparamref name="TDependent"/> has already been registered via
    ///     <see cref="ApiSchemaBuilder.AddObject{T}(Action{ApiObjectTypeBuilder{T}})"/>.
    /// </summary>
    /// <typeparam name="TDependent">The CLR type of the dependent object.</typeparam>
    /// <param name="configure">Optional callback to add FK key paths using expression-based property selectors.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToOneBuilder WithDependentEnd<TDependent>
    (
        Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null
    )
    {
        var apiName = this.Context?.GetObjectTypeApiName(typeof(TDependent))
            ?? throw new ApiSchemaException(
                $"No schema builder context is available. Ensure the relationship is registered through {nameof(ApiSchemaBuilder)}.");

        var builder = new ApiRelationshipDependentEndBuilder<TDependent>(apiName);
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
            apiDependentEnd,
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
}
