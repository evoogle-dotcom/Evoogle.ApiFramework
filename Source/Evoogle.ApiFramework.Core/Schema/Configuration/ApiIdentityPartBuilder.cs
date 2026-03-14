// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiIdentityPartBuilder
(
    ApiIdentityPartKind apiKind,
    string? apiPropertyName,
    string? apiIdentityName,
    Type? clrScalarTypeHint
)
: ExtensionBuilder<ApiIdentityPartBuilder>
{
    #region Fields
    private readonly ApiIdentityPartKind _apiKind = apiKind;
    private readonly string? _apiPropertyName = apiPropertyName;
    private readonly string? _apiIdentityName = apiIdentityName;
    private readonly Type? _clrScalarTypeHint = clrScalarTypeHint;
    #endregion

    #region Methods
    internal ApiIdentityPart Build()
    {
        ApiIdentityPart apiIdentityPart = _apiKind switch
        {
            ApiIdentityPartKind.Scalar => new ApiScalarIdentityPart(_apiPropertyName!, _clrScalarTypeHint),
            ApiIdentityPartKind.Nested => new ApiNestedIdentityPart(_apiPropertyName!, _apiIdentityName),
            ApiIdentityPartKind.Parent => new ApiParentIdentityPart(_apiIdentityName),
            _ => throw new ApiSchemaException($"Unsupported API identity part kind: {_apiKind}"),
        };

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiIdentityPart.Extensions = extensions;
        }

        return apiIdentityPart;
    }
    #endregion
}
