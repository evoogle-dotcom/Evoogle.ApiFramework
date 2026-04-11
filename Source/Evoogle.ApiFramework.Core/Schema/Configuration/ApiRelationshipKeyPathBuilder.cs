// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure a single <see cref="ApiRelationshipKeyPath"/> node and,
///     for <see cref="ApiRelationshipKeyPathKind.Nested"/> and <see cref="ApiRelationshipKeyPathKind.Owner"/> nodes,
///     its child paths.
/// </summary>
/// <remarks>
///     <para>
///         Scalar nodes terminate the path tree and carry no child builders.
///     </para>
///     <para>
///         Nested and owner nodes accept child paths through
///         <see cref="AddScalarPath"/>, <see cref="AddNestedPath"/>, and <see cref="AddOwnerPath"/>.
///     </para>
/// </remarks>
/// <param name="apiKind">The kind of key path to build.</param>
/// <param name="clrPropertyName">
///     The CLR property name for <see cref="ApiRelationshipKeyPathKind.Scalar"/> and
///     <see cref="ApiRelationshipKeyPathKind.Nested"/> kinds; <see langword="null"/> for
///     <see cref="ApiRelationshipKeyPathKind.Owner"/>.
/// </param>
public class ApiRelationshipKeyPathBuilder
(
    ApiRelationshipKeyPathKind apiKind,
    string? clrPropertyName
) : ExtensionBuilder<ApiRelationshipKeyPathBuilder>
{
    #region Fields
    private readonly List<ApiRelationshipKeyPathBuilder> _childBuilders = [];
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipKeyPathBuilder AddKeyPathExtension(Type type, object value)
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
    public ApiRelationshipKeyPathBuilder AddKeyPathExtension<T>(T value) where T : notnull
        => this.AddKeyPathExtension(typeof(T), value);

    /// <summary>
    ///     Adds a <see cref="ApiRelationshipScalarKeyPath"/> child to this node.
    ///     Valid only on <see cref="ApiRelationshipKeyPathKind.Nested"/> and
    ///     <see cref="ApiRelationshipKeyPathKind.Owner"/> parent nodes.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name of the scalar FK property on the current object type.</param>
    /// <param name="configure">Optional callback to attach extensions to the scalar path.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipKeyPathBuilder AddScalarPath
    (
        string clrPropertyName,
        Action<ApiRelationshipKeyPathBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName, nameof(clrPropertyName));

        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Scalar, clrPropertyName);
        configure?.Invoke(builder);
        _childBuilders.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a <see cref="ApiRelationshipNestedKeyPath"/> child to this node.
    ///     Valid only on <see cref="ApiRelationshipKeyPathKind.Nested"/> and
    ///     <see cref="ApiRelationshipKeyPathKind.Owner"/> parent nodes.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name of the nested object property to navigate into.</param>
    /// <param name="configure">Callback to add child paths within the nested object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipKeyPathBuilder AddNestedPath
    (
        string clrPropertyName,
        Action<ApiRelationshipKeyPathBuilder> configure
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName, nameof(clrPropertyName));
        ArgumentNullException.ThrowIfNull(configure, nameof(configure));

        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Nested, clrPropertyName);
        configure(builder);
        _childBuilders.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a <see cref="ApiRelationshipOwnerKeyPath"/> child to this node.
    ///     Valid only on <see cref="ApiRelationshipKeyPathKind.Nested"/> and
    ///     <see cref="ApiRelationshipKeyPathKind.Owner"/> parent nodes.
    /// </summary>
    /// <param name="configure">Optional callback to add child paths within the owner object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipKeyPathBuilder AddOwnerPath
    (
        Action<ApiRelationshipKeyPathBuilder>? configure = null
    )
    {
        var builder = new ApiRelationshipKeyPathBuilder(ApiRelationshipKeyPathKind.Owner, null);
        configure?.Invoke(builder);
        _childBuilders.Add(builder);
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Allows subclasses to add a pre-constructed child builder without bypassing internal list management.
    /// </summary>
    protected void AddChildBuilderCore(ApiRelationshipKeyPathBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _childBuilders.Add(builder);
    }

    /// <summary>
    ///     Builds the <see cref="ApiRelationshipKeyPath"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipKeyPath Build()
    {
        ApiRelationshipKeyPath path = apiKind switch
        {
            ApiRelationshipKeyPathKind.Scalar => new ApiRelationshipScalarKeyPath(clrPropertyName!),

            ApiRelationshipKeyPathKind.Nested => new ApiRelationshipNestedKeyPath
            (
                clrPropertyName!,
                _childBuilders.Select(b => b.Build())
            ),

            ApiRelationshipKeyPathKind.Owner => new ApiRelationshipOwnerKeyPath
            (
                _childBuilders.Count > 0 ? _childBuilders.Select(b => b.Build()) : null
            ),

            _ => throw new ApiSchemaException($"Unsupported API relationship key path kind: {apiKind}")
        };

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            path.Extensions = extensions;
        }

        return path;
    }
    #endregion
}
