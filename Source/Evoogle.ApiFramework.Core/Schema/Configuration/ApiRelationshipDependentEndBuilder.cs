// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure the dependent end of an <see cref="ApiRelationship"/>.
/// </summary>
/// <remarks>
///     The dependent end holds the FK key path tree.  Add key paths with
///     <see cref="AddScalarPath"/>, <see cref="AddNestedPath"/>, or <see cref="AddOwnerPath"/>.
///     When no paths are added the relationship is treated as purely navigational.
/// </remarks>
/// <param name="apiObjectTypeName">The API name of the dependent <see cref="ApiObjectType"/>.</param>
public class ApiRelationshipDependentEndBuilder
(
    string apiObjectTypeName
) : ExtensionBuilder<ApiRelationshipDependentEndBuilder>
{
    #region Fields
    private readonly List<ApiRelationshipKeyPathBuilder> _keyPathBuilders = [];
    private ApiRelationshipDeleteBehavior _apiDeleteBehavior = ApiRelationshipDeleteBehavior.None;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder AddDependentEndExtension(Type type, object value)
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
    public ApiRelationshipDependentEndBuilder AddDependentEndExtension<T>(T value) where T : notnull
        => this.AddDependentEndExtension(typeof(T), value);

    /// <summary>
    ///     Sets the delete behavior that governs what happens to the principal objects when a dependent object is deleted.
    /// </summary>
    /// <param name="apiDeleteBehavior">The desired delete behavior.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder WithDeleteBehavior(ApiRelationshipDeleteBehavior apiDeleteBehavior)
    {
        _apiDeleteBehavior = apiDeleteBehavior;
        return this;
    }

    /// <summary>
    ///     Adds a scalar FK key path that maps the principal identity's scalar leaf
    ///     directly to a property on the dependent object type.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name on the dependent type that holds the FK value.</param>
    /// <param name="configure">Optional callback to attach extensions to the path.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder AddScalarPath
    (
        string clrPropertyName,
        Action<ApiRelationshipKeyPathBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName, nameof(clrPropertyName));

        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Scalar, clrPropertyName);
        configure?.Invoke(builder);
        _keyPathBuilders.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a nested FK key path that navigates into an object-typed property on the dependent type
    ///     before resolving the inner scalar FK values.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name of the nested object on the dependent type to navigate into.</param>
    /// <param name="configure">Callback to add child paths within the nested object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder AddNestedPath
    (
        string clrPropertyName,
        Action<ApiRelationshipKeyPathBuilder> configure
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName, nameof(clrPropertyName));
        ArgumentNullException.ThrowIfNull(configure, nameof(configure));

        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Nested, clrPropertyName);
        configure(builder);
        _keyPathBuilders.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds an owner FK key path whose scalar values are resolved from the owning object type
    ///     in a deferred second pass, mirroring the <c>AddOwner</c> pattern from identity part builders.
    /// </summary>
    /// <param name="configure">Optional callback to add child paths within the owner object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder AddOwnerPath
    (
        Action<ApiRelationshipKeyPathBuilder>? configure = null
    )
    {
        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Owner, null);
        configure?.Invoke(builder);
        _keyPathBuilders.Add(builder);
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Allows subclasses to add a pre-constructed key path builder without bypassing internal list management.
    /// </summary>
    protected void AddKeyPathBuilderCore(ApiRelationshipKeyPathBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _keyPathBuilders.Add(builder);
    }

    /// <summary>
    ///     Builds the <see cref="ApiRelationshipDependentEnd"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipDependentEnd Build()
    {
        // Null means "purely navigational"; non-null (even empty) means FK key paths were declared.
        var apiKeyPaths = _keyPathBuilders.Count > 0
            ? _keyPathBuilders.Select(b => b.Build())
            : null;

        var end = new ApiRelationshipDependentEnd
        (
            apiObjectTypeName,
            apiKeyPaths,
            _apiDeleteBehavior
        );

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            end.Extensions = extensions;
        }

        return end;
    }

    /// <summary>
    ///     Builds the <see cref="ApiRelationshipDependentEnd"/> with a forced delete behavior that overrides
    ///     any developer-configured value.  Used by M:N builders to enforce cascading deletes.
    /// </summary>
    internal ApiRelationshipDependentEnd BuildWithForcedDeleteBehavior(ApiRelationshipDeleteBehavior forcedDeleteBehavior)
    {
        var apiKeyPaths = _keyPathBuilders.Count > 0
            ? _keyPathBuilders.Select(b => b.Build())
            : null;

        var end = new ApiRelationshipDependentEnd
        (
            apiObjectTypeName,
            apiKeyPaths,
            _apiDeleteBehavior,
            forcedDeleteBehavior
        );

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            end.Extensions = extensions;
        }

        return end;
    }
    #endregion
}
