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

    public ApiIdentityBuilder AddScalar(string apiPropertyName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Scalar, apiPropertyName, null, null, configure);
        return this;
    }

    public ApiIdentityBuilder AddScalar(string apiPropertyName, Type clrScalarTypeHint, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Scalar, apiPropertyName, null, clrScalarTypeHint, configure);
        return this;
    }

    public ApiIdentityBuilder AddNested(string apiPropertyName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Nested, apiPropertyName, null, null, configure);
        return this;
    }

    public ApiIdentityBuilder AddNested(string apiPropertyName, string apiIdentityName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Nested, apiPropertyName, apiIdentityName, null, configure);
        return this;
    }

    public ApiIdentityBuilder AddParent(Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Parent, null, null, null, configure);
        return this;
    }

    public ApiIdentityBuilder AddParent(string apiIdentityName, Action<ApiIdentityPartBuilder>? configure = null)
    {
        this.AddPart(ApiIdentityPartKind.Parent, null, apiIdentityName, null, configure);
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
