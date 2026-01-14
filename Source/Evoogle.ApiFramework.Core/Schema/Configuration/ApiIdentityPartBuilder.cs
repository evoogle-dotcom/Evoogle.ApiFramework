// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Builds <see cref="ApiIdentityPart"/> instances that describe individual property parts of an identity.
/// </summary>
/// <param name="apiPropertyName">The API property name that is part of the identity.</param>
/// <param name="clrConfiguredIdType">Optional user configured CLR type for the identity part. If not provided, the type is inferred from the resolved property.</param>
public class ApiIdentityPartBuilder(string apiPropertyName, Type? clrConfiguredIdType = null) : ExtensionBuilder<ApiIdentityPartBuilder>
{
    #region Fields
    private readonly string _apiPropertyName = apiPropertyName;
    private readonly Type? _clrConfiguredIdType = clrConfiguredIdType;
    #endregion

    #region Methods
    /// <summary>
    ///     Builds the <see cref="ApiIdentityPart"/> configured by this builder.
    /// </summary>
    /// <returns>A new <see cref="ApiIdentityPart"/> instance.</returns>
    internal ApiIdentityPart Build()
    {
        var apiIdentityPart = new ApiIdentityPart(_apiPropertyName, _clrConfiguredIdType);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiIdentityPart.Extensions = extensions;
        }

        return apiIdentityPart;
    }
    #endregion
}
