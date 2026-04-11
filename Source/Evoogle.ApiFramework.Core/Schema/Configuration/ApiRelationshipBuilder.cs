// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Abstract base class for fluent builders that configure an <see cref="ApiRelationship"/>.
/// </summary>
/// <remarks>
///     Use <see cref="ApiRelationshipOneToOneBuilder"/>, <see cref="ApiRelationshipOneToManyBuilder"/>,
///     or <see cref="ApiRelationshipManyToManyBuilder"/> to configure the specific relationship kind.
/// </remarks>
/// <param name="apiName">The schema-unique API name of the relationship.</param>
public abstract class ApiRelationshipBuilder(string apiName) : ExtensionBuilder<ApiRelationshipBuilder>
{
    #region Fields
    private string? _apiDisplayName;
    private string? _apiDescription;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Sets an optional human-readable display name for the relationship.
    /// </summary>
    /// <param name="apiDisplayName">The display name.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipBuilder WithDisplayName(string apiDisplayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiDisplayName, nameof(apiDisplayName));

        _apiDisplayName = apiDisplayName;
        return this;
    }

    /// <summary>
    ///     Sets an optional description for the relationship.
    /// </summary>
    /// <param name="apiDescription">The description text.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipBuilder WithDescription(string apiDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiDescription, nameof(apiDescription));

        _apiDescription = apiDescription;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiRelationship"/> configured by this builder.
    /// </summary>
    internal abstract ApiRelationship Build();
    #endregion

    #region Protected Helpers
    /// <summary>Gets the configured API name.</summary>
    protected string ApiName => apiName;

    /// <summary>Gets the configured display name.</summary>
    protected string? ApiDisplayName => _apiDisplayName;

    /// <summary>Gets the configured description.</summary>
    protected string? ApiDescription => _apiDescription;

    /// <summary>Gets or sets the shared schema builder context used for typed end resolution.</summary>
    internal ApiSchemaBuilderContext? Context { get; set; }
    #endregion
}

