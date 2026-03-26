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

    #region Builder Methods
    /// <summary>
    ///     Adds a part of the specified kind to this identity definition.
    /// </summary>
    /// <param name="apiKind">The kind of identity part to add.</param>
    /// <param name="apiPropertyName">The API property name for scalar/object parts; <see langword="null"/> for owner parts.</param>
    /// <param name="apiIdentityName">Optional explicit identity name for object/owner parts.</param>
    /// <param name="clrScalarTypeHint">Optional CLR type hint for scalar parts.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddPart
    (
        ApiIdentityPartKind apiKind,
        string? apiPropertyName,
        string? apiIdentityName,
        Type? clrScalarTypeHint,
        Action<ApiIdentityPartBuilder>? configure = null)
    {
        var apiIdentityPartBuilder = new ApiIdentityPartBuilder(apiKind, apiPropertyName, apiIdentityName, clrScalarTypeHint);

        configure?.Invoke(apiIdentityPartBuilder);

        _apiIdentityPartBuilders.Add(apiIdentityPartBuilder);

        return this;
    }

    /// <summary>Adds a scalar identity part sourced from the property named <paramref name="apiPropertyName"/>.</summary>
    /// <param name="apiPropertyName">The API property name of the scalar property.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddScalar(string apiPropertyName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Scalar, apiPropertyName, null, null, configure);
        return this;
    }

    /// <summary>Adds a scalar identity part sourced from the property named <paramref name="apiPropertyName"/> with an explicit CLR type hint.</summary>
    /// <param name="apiPropertyName">The API property name of the scalar property.</param>
    /// <param name="clrScalarTypeHint">The CLR type to use when extracting the scalar value, overriding the property's inferred type.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddScalar(string apiPropertyName, Type clrScalarTypeHint, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Scalar, apiPropertyName, null, clrScalarTypeHint, configure);
        return this;
    }

    /// <summary>Adds a nested identity part sourced from the primary identity of the object property named <paramref name="apiPropertyName"/>.</summary>
    /// <param name="apiPropertyName">The API property name of the nested object.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddNested(string apiPropertyName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Nested, apiPropertyName, null, null, configure);
        return this;
    }

    /// <summary>Adds a nested identity part sourced from a named identity of the object property named <paramref name="apiPropertyName"/>.</summary>
    /// <param name="apiPropertyName">The API property name of the nested object.</param>
    /// <param name="apiIdentityName">The explicit name of the identity to use on the nested object type.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddNested(string apiPropertyName, string apiIdentityName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Nested, apiPropertyName, apiIdentityName, null, configure);
        return this;
    }

    /// <summary>Adds an owner identity part sourced from the primary identity of the owning object.</summary>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddOwner(Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Owner, null, null, null, configure);
        return this;
    }

    /// <summary>Adds an owner identity part sourced from a named identity of the owning object.</summary>
    /// <param name="apiIdentityName">The explicit name of the identity to use on the owner object type.</param>
    /// <param name="configure">Optional callback to further configure the part builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddOwner(string apiIdentityName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Owner, null, apiIdentityName, null, configure);
        return this;
    }

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
