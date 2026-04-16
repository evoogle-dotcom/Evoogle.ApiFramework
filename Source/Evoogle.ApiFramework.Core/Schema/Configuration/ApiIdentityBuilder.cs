// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Builds <see cref="ApiIdentity"/> instances that describe unique identities for an <see cref="ApiObjectType"/>.
/// </summary>
/// <remarks>
///     The first identity added to an <see cref="ApiObjectType"/> is the primary identity by convention unless explicitly specified otherwise.
/// </remarks>
/// <param name="apiName">The API name of the identity.</param>
public class ApiIdentityBuilder(string apiName) : ExtensionBuilder<ApiIdentityBuilder>
{
    #region Fields
    private readonly string _apiName = apiName;
    private readonly List<ApiIdentityPartBuilder> _apiIdentityPartBuilders = [];
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddIdentityExtension(Type type, object value)
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
    public ApiIdentityBuilder AddIdentityExtension<T>(T value) where T : notnull
        => this.AddIdentityExtension(typeof(T), value);
    #endregion

    #region AddPart Methods
    /// <summary>
    ///     Adds a part of the specified kind to this identity definition.
    /// </summary>
    /// <param name="apiKind">The kind of identity part to add.</param>
    /// <param name="clrPropertyName">The CLR property name for scalar/object parts; <see langword="null"/> for owner parts.</param>
    /// <param name="apiIdentityName">Optional explicit identity name for object/owner parts.</param>
    /// <param name="clrScalarTypeHint">Optional CLR type hint for scalar parts.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    protected ApiIdentityBuilder AddPart
    (
        ApiIdentityPartKind apiKind,
        string? clrPropertyName,
        string? apiIdentityName,
        Type? clrScalarTypeHint,
        Action<ApiIdentityPartBuilder>? configure = null)
    {
        var apiIdentityPartBuilder = new ApiIdentityPartBuilder(apiKind, clrPropertyName, apiIdentityName, clrScalarTypeHint);

        configure?.Invoke(apiIdentityPartBuilder);

        _apiIdentityPartBuilders.Add(apiIdentityPartBuilder);

        return this;
    }

    /// <summary>Adds a scalar identity part sourced from the property named <paramref name="clrPropertyName"/>.</summary>
    /// <param name="clrPropertyName">The CLR property name of the scalar property.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddScalarPart(string clrPropertyName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Scalar, clrPropertyName, null, null, configure);
        return this;
    }

    /// <summary>Adds a scalar identity part sourced from the property named <paramref name="clrPropertyName"/> with an explicit CLR type hint.</summary>
    /// <param name="clrPropertyName">The CLR property name of the scalar property.</param>
    /// <param name="clrScalarTypeHint">The CLR type to use when extracting the scalar value, overriding the property's inferred type.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddScalarPart(string clrPropertyName, Type clrScalarTypeHint, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Scalar, clrPropertyName, null, clrScalarTypeHint, configure);
        return this;
    }

    /// <summary>Adds a nested identity part sourced from the primary identity of the object property named <paramref name="clrPropertyName"/>.</summary>
    /// <param name="clrPropertyName">The CLR property name of the nested object.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddNestedPart(string clrPropertyName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Nested, clrPropertyName, null, null, configure);
        return this;
    }

    /// <summary>Adds a nested identity part sourced from a named identity of the object property named <paramref name="clrPropertyName"/>.</summary>
    /// <param name="clrPropertyName">The CLR property name of the nested object.</param>
    /// <param name="apiIdentityName">The explicit name of the identity to use on the nested object type.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddNestedPart(string clrPropertyName, string apiIdentityName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Nested, clrPropertyName, apiIdentityName, null, configure);
        return this;
    }

    /// <summary>Adds an owner identity part sourced from the primary identity of the owning object.</summary>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddOwnerPart(Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Owner, null, null, null, configure);
        return this;
    }

    /// <summary>Adds an owner identity part sourced from a named identity of the owning object.</summary>
    /// <param name="apiIdentityName">The explicit name of the identity to use on the owner object type.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddOwnerPart(string apiIdentityName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Owner, null, apiIdentityName, null, configure);
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiIdentity"/> configured by this builder.
    /// </summary>
    /// <returns>A new <see cref="ApiIdentity"/> instance.</returns>
    internal ApiIdentity Build()
    {
        var apiIdentityParts = _apiIdentityPartBuilders
            .Select(b => b.Build());

        var apiIdentity = new ApiIdentity(_apiName, apiIdentityParts);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiIdentity.Extensions = extensions;
        }

        return apiIdentity;
    }
    #endregion
}
