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
///     Call <see cref="From(Type, Action{ApiRelationshipPrincipalEndBuilder}?)"/>,
///     <see cref="To(Type, Action{ApiRelationshipDependentEndBuilder}?)"/>, and optionally
///     <see cref="WithDeleteBehavior"/> to configure the relationship.
///     At most one principal end and one dependent end are allowed; subsequent calls replace the previous
///     configuration for that end.
/// </remarks>
/// <param name="apiName">The schema-unique API name of the relationship.</param>
public sealed class ApiRelationshipOneToManyBuilder(string apiName)
    : ApiRelationshipBuilder(apiName, ApiRelationshipOneToMany.DefaultDeleteBehavior)
{
    #region Fields
    private ApiRelationshipPrincipalEndBuilder? _principalEndBuilder;
    private ApiRelationshipDependentEndBuilder? _dependentEndBuilder;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder AddRelationshipExtension(Type type, object extension)
        => base.AddRelationshipExtension<ApiRelationshipOneToManyBuilder>(type, extension);
    #endregion

    #region Non-Generic From/To Methods
    /// <summary>
    ///     Configures the principal end of the 1:M relationship using the specified CLR type.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal object.</param>
    /// <param name="configure">Optional callback to configure principal key type selection and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder From(Type clrPrincipalType, Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        configure?.Invoke(builder);
        _principalEndBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures the principal end of the 1:M relationship using the specified CLR type,
    ///     and selects the named primary key type for the relationship.
    /// </summary>
    /// <param name="clrPrincipalType">The CLR type of the principal object.</param>
    /// <param name="apiPrimaryKeyTypeName">The name of the primary key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder From(Type clrPrincipalType, string apiPrimaryKeyTypeName)
    {
        var builder = new ApiRelationshipPrincipalEndBuilder(clrPrincipalType);
        builder.WithPrimaryKey(apiPrimaryKeyTypeName);
        _principalEndBuilder = builder;
        return this;
    }

    /// <summary>
    ///     Configures the dependent end of the 1:M relationship using the specified CLR type.
    /// </summary>
    /// <param name="clrDependentType">The CLR type of the dependent object.</param>
    /// <param name="configure">Optional callback to add foreign key role key paths and extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder To(Type clrDependentType, Action<ApiRelationshipDependentEndBuilder>? configure = null)
    {
        var builder = new ApiRelationshipDependentEndBuilder(clrDependentType);
        configure?.Invoke(builder);
        _dependentEndBuilder = builder;
        return this;
    }
    #endregion

    #region Non-Generic With Methods
    /// <summary>
    ///     Sets the delete behavior for the relationship.
    /// </summary>
    /// <param name="apiDeleteBehavior">The desired delete behavior.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipOneToManyBuilder WithDeleteBehavior(ApiRelationshipDeleteBehavior apiDeleteBehavior)
        => base.WithDeleteBehavior<ApiRelationshipOneToManyBuilder>(apiDeleteBehavior);
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

    #region Implementation Methods
    /// <summary>
    ///     Allows extension methods to set a pre-constructed dependent end builder.
    /// </summary>
    internal ApiRelationshipOneToManyBuilder SetDependentEndBuilderCore(ApiRelationshipDependentEndBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _dependentEndBuilder = builder;
        return this;
    }
    #endregion
}
