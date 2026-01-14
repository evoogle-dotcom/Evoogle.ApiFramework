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
    ///     Adds an identity part (property) to this identity.
    /// </summary>
    /// <param name="apiPropertyName">The API property name that is part of the identity.</param>
    /// <param name="clrConfiguredIdType">Optional user configured CLR type for the identity part. If not provided, the type is inferred from the resolved property.</param>
    /// <param name="configure">Optional callback to configure the added identity part.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityBuilder AddPart(string apiPropertyName, Type? clrConfiguredIdType = null, Action<ApiIdentityPartBuilder>? configure = null)
    {
        var apiIdentityPartBuilder = new ApiIdentityPartBuilder(apiPropertyName, clrConfiguredIdType);

        configure?.Invoke(apiIdentityPartBuilder);

        _apiIdentityPartBuilders.Add(apiIdentityPartBuilder);

        return this;
    }
    #endregion

    #region Methods
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
