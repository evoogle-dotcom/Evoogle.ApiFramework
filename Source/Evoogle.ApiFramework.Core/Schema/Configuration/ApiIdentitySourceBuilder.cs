// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Builds <see cref="ApiIdentitySource"/> instances that describe individual property sources of an identity.
/// </summary>
/// <param name="apiPropertyName">The API property name that is part of the identity.</param>
/// <param name="clrScalarType">Optional user-configured CLR type for scalar sources. If not provided, the type is inferred from the resolved property. Ignored for nested sources.</param>
/// <param name="apiNestedName">Optional name of the identity on the referenced object type to use for nested sources. If null, the primary identity is used.</param>
public class ApiIdentitySourceBuilder
(
    string apiPropertyName,
    Type? clrScalarType,
    string? apiNestedName
)
: ExtensionBuilder<ApiIdentitySourceBuilder>
{
    #region Fields
    private readonly string _apiPropertyName = apiPropertyName;
    private readonly string? _apiNestedName = apiNestedName;
    private readonly Type? _clrScalarType = clrScalarType;
    #endregion

    #region Methods
    /// <summary>
    ///     Builds the <see cref="ApiIdentitySource"/> configured by this builder.
    /// </summary>
    /// <returns>A new <see cref="ApiIdentitySource"/> instance.</returns>
    internal ApiIdentitySource Build()
    {
        var apiIdentitySource = new ApiIdentitySource(_apiPropertyName, _clrScalarType, _apiNestedName);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiIdentitySource.Extensions = extensions;
        }

        return apiIdentitySource;
    }
    #endregion
}
