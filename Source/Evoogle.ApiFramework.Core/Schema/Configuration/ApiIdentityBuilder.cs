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
    private readonly List<ApiIdentitySourceBuilder> _apiIdentitySourceBuilders = [];
    #endregion

    #region Builder Methods
    public ApiIdentityBuilder AddSource(string apiPropertyName, Type? clrScalarType = null, string? apiNestedName = null, Action<ApiIdentitySourceBuilder>? configure = null)
    {
        var apiIdentitySourceBuilder = new ApiIdentitySourceBuilder(apiPropertyName, clrScalarType, apiNestedName);

        configure?.Invoke(apiIdentitySourceBuilder);

        _apiIdentitySourceBuilders.Add(apiIdentitySourceBuilder);

        return this;
    }

    public ApiIdentityBuilder AddScalar(string apiPropertyName, Action<ApiIdentitySourceBuilder>? configure = null)
    {
        this.AddSource(apiPropertyName, null, null, configure);
        return this;
    }

    public ApiIdentityBuilder AddScalar(string apiPropertyName, Type clrScalarType, Action<ApiIdentitySourceBuilder>? configure = null)
    {
        this.AddSource(apiPropertyName, clrScalarType, null, configure);
        return this;
    }

    public ApiIdentityBuilder AddNested(string apiPropertyName, Action<ApiIdentitySourceBuilder>? configure = null)
    {
        this.AddSource(apiPropertyName, null, null, configure);
        return this;
    }

    public ApiIdentityBuilder AddNested(string apiPropertyName, string apiNestedName, Action<ApiIdentitySourceBuilder>? configure = null)
    {
        this.AddSource(apiPropertyName, null, apiNestedName, configure);
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ApiIdentity"/> configured by this builder.
    /// </summary>
    /// <returns>A new <see cref="ApiIdentity"/> instance.</returns>
    internal ApiIdentity Build()
    {
        var apiIdentitySources = _apiIdentitySourceBuilders
            .Select(b => b.Build());

        var apiIdentity = new ApiIdentity(_apiName, apiIdentitySources);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiIdentity.Extensions = extensions;
        }

        return apiIdentity;
    }
    #endregion
}
