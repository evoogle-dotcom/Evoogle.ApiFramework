// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure the association of an <see cref="ApiRelationshipManyToMany"/>.
/// </summary>
/// <remarks>
///     The association holds two FK key path trees — one per principal side.
///     Add A-side key paths with <see cref="AddScalarPathA"/>, <see cref="AddNestedPathA"/>, or <see cref="AddOwnerPathA"/>.
///     Add B-side key paths with <see cref="AddScalarPathB"/>, <see cref="AddNestedPathB"/>, or <see cref="AddOwnerPathB"/>.
///     When no paths are added for a side the relationship is treated as purely navigational for that side.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the association <see cref="ApiObjectType"/>.</param>
public class ApiRelationshipAssociationBuilder(Type clrObjectType) : ExtensionBuilder<ApiRelationshipAssociationBuilder>
{
    #region Fields
    private readonly List<ApiRelationshipKeyPathBuilder> _keyPathBuildersA = [];
    private readonly List<ApiRelationshipKeyPathBuilder> _keyPathBuildersB = [];
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddAssociationExtension(Type type, object value)
    {
        base.AddExtension(type, value);
        return this;
    }

    /// <summary>
    ///     Adds an extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="T">The extension value type.</typeparam>
    /// <param name="value">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddAssociationExtension<T>(T value) where T : notnull
        => this.AddAssociationExtension(typeof(T), value);
    #endregion

    #region AddPath A Methods
    /// <summary>
    ///     Adds an A-side scalar FK key path that maps principal A's identity's scalar leaf
    ///     directly to a property on the association object type.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name on the association type that holds the FK value.</param>
    /// <param name="configure">Optional callback to attach extensions to the path.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddScalarPathA(string clrPropertyName, Action<ApiRelationshipKeyPathBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName, nameof(clrPropertyName));

        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Scalar, clrPropertyName);
        configure?.Invoke(builder);
        _keyPathBuildersA.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds an A-side nested FK key path that navigates into an object-typed property on the association type
    ///     before resolving the inner scalar FK values.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name of the nested object on the association type to navigate into.</param>
    /// <param name="configure">Callback to add child paths within the nested object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddNestedPathA(string clrPropertyName, Action<ApiRelationshipKeyPathBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName, nameof(clrPropertyName));
        ArgumentNullException.ThrowIfNull(configure, nameof(configure));

        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Nested, clrPropertyName);
        configure(builder);
        _keyPathBuildersA.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds an A-side owner FK key path whose scalar values are resolved from the owning object type
    ///     in a deferred second pass, mirroring the <c>AddOwner</c> pattern from identity part builders.
    /// </summary>
    /// <param name="configure">Optional callback to add child paths within the owner object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddOwnerPathA(Action<ApiRelationshipKeyPathBuilder>? configure = null)
    {
        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Owner, null);
        configure?.Invoke(builder);
        _keyPathBuildersA.Add(builder);
        return this;
    }

    /// <summary>
    ///     Allows subclasses to add a pre-constructed A-side key path builder without bypassing internal list management.
    /// </summary>
    protected void AddKeyPathBuilderACore(ApiRelationshipKeyPathBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _keyPathBuildersA.Add(builder);
    }
    #endregion

    #region AddPath B Methods
    /// <summary>
    ///     Adds a B-side scalar FK key path that maps principal B's identity's scalar leaf
    ///     directly to a property on the association object type.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name on the association type that holds the FK value.</param>
    /// <param name="configure">Optional callback to attach extensions to the path.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddScalarPathB(string clrPropertyName, Action<ApiRelationshipKeyPathBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName, nameof(clrPropertyName));

        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Scalar, clrPropertyName);
        configure?.Invoke(builder);
        _keyPathBuildersB.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a B-side nested FK key path that navigates into an object-typed property on the association type
    ///     before resolving the inner scalar FK values.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name of the nested object on the association type to navigate into.</param>
    /// <param name="configure">Callback to add child paths within the nested object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddNestedPathB(string clrPropertyName, Action<ApiRelationshipKeyPathBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName, nameof(clrPropertyName));
        ArgumentNullException.ThrowIfNull(configure, nameof(configure));

        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Nested, clrPropertyName);
        configure(builder);
        _keyPathBuildersB.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a B-side owner FK key path whose scalar values are resolved from the owning object type
    ///     in a deferred second pass, mirroring the <c>AddOwner</c> pattern from identity part builders.
    /// </summary>
    /// <param name="configure">Optional callback to add child paths within the owner object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddOwnerPathB(Action<ApiRelationshipKeyPathBuilder>? configure = null)
    {
        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Owner, null);
        configure?.Invoke(builder);
        _keyPathBuildersB.Add(builder);
        return this;
    }

    /// <summary>
    ///     Allows subclasses to add a pre-constructed B-side key path builder without bypassing internal list management.
    /// </summary>
    protected void AddKeyPathBuilderBCore(ApiRelationshipKeyPathBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _keyPathBuildersB.Add(builder);
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiRelationshipAssociation"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipAssociation Build()
    {
        // Null means "purely navigational" for that side; non-null (even empty) means FK key paths were declared.
        var apiKeyPathsA = _keyPathBuildersA.Count > 0
            ? _keyPathBuildersA.Select(b => b.Build())
            : null;

        var apiKeyPathsB = _keyPathBuildersB.Count > 0
            ? _keyPathBuildersB.Select(b => b.Build())
            : null;

        var apiRelationshipAssociation = apiKeyPathsA != null && apiKeyPathsB != null
            ? new ApiRelationshipAssociation(clrObjectType, apiKeyPathsA, apiKeyPathsB)
            : new ApiRelationshipAssociation(clrObjectType);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiRelationshipAssociation.Extensions = extensions;
        }

        return apiRelationshipAssociation;
    }
    #endregion
}
