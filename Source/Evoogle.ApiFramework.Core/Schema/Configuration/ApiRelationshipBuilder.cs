// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiRelationshipBuilder(string apiName, string? apiPropertyName) : ExtensionBuilder<ApiRelationshipBuilder>
{
    #region Fields
    private readonly string _apiName = apiName;
    private readonly string? _apiPropertyName = apiPropertyName;
    #endregion

    #region Methods
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
