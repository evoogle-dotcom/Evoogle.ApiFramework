// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Builds <see cref="ApiRelationship"/> instances that describe object-to-object navigation.
/// </summary>
/// <param name="apiName">The API name of the relationship.</param>
/// <param name="apiPropertyName">The API property name this relationship points to, if any.</param>
public class ApiRelationshipBuilder(string apiName, string? apiPropertyName) : ExtensionBuilder<ApiRelationshipBuilder>
{
    #region Fields
    private readonly string _apiName = apiName;
    private readonly string? _apiPropertyName = apiPropertyName;
    #endregion

    #region Methods
    /// <summary>
    ///     Builds the <see cref="ApiRelationship"/> configured by this builder.
    /// </summary>
    /// <returns>A new <see cref="ApiRelationship"/> instance.</returns>
    internal ApiRelationship Build()
    {
        var apiPropertyName = _apiPropertyName;
        var apiName = _apiName;

        var apiRelationship = new ApiRelationship(apiName, apiPropertyName);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiRelationship.Extensions = extensions;
        }

        return apiRelationship;
    }
    #endregion
}
