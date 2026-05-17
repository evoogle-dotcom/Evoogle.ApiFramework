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
/// <param name="apiDefaultDeleteBehavior">The default delete behavior for the relationship.</param>
public abstract class ApiRelationshipBuilder(string apiName, ApiRelationshipDeleteBehavior apiDefaultDeleteBehavior)
    : ExtensionBuilder<ApiRelationshipBuilder>
{
    #region Fields
    /// <summary>
    ///     The delete behavior for the relationship, initialized to the default value for the relationship kind.
    ///     Updated if the user calls <see cref="WithDeleteBehavior"/> on the concrete builder.
    ///     Used when building the final <see cref="ApiRelationship"/> instance.
    /// </summary>
    protected ApiRelationshipDeleteBehavior _apiDeleteBehavior = apiDefaultDeleteBehavior;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    protected TBuilder AddRelationshipExtension<TBuilder>(Type type, object value)
        where TBuilder : ApiRelationshipBuilder
    {
        base.AddExtension(type, value);
        return (TBuilder)this;
    }

    /// <summary>
    ///     Adds an extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the current builder.</typeparam>
    /// <typeparam name="T">The extension value type.</typeparam>
    /// <param name="value">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    protected TBuilder AddRelationshipExtension<TBuilder, T>(T value)
        where TBuilder : ApiRelationshipBuilder
        where T : notnull
        => this.AddRelationshipExtension<TBuilder>(typeof(T), value);
    #endregion

    #region With Methods
    /// <summary>
    ///     Sets the delete behavior for the relationship.
    /// </summary>
    /// <param name="apiDeleteBehavior">The desired delete behavior.</param>
    /// <returns>The current builder instance.</returns>
    protected TBuilder WithDeleteBehavior<TBuilder>(ApiRelationshipDeleteBehavior apiDeleteBehavior)
        where TBuilder : ApiRelationshipBuilder
    {
        _apiDeleteBehavior = apiDeleteBehavior;
        return (TBuilder)this;
    }
    #endregion

    #region Build Methods
    internal abstract ApiRelationship Build();
    #endregion

    #region Protected Helpers
    /// <summary>Gets the configured API name.</summary>
    protected string ApiName => apiName;
    #endregion
}
