// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

public sealed class ApiIdCompositeBuilder
{
    #region Fields
    private List<ApiIdPart>? _parts;
    #endregion

    #region Builder Methods
    public ApiIdCompositeBuilder Add(ApiId value)
    {
        _parts ??= [];
        _parts.Add(new ApiIdPart(null, value));
        return this;
    }

    public ApiIdCompositeBuilder Add(string name, ApiId value)
    {
        _parts ??= [];
        _parts.Add(new ApiIdPart(name, value));
        return this;
    }

    public ApiId Build()
    {
        return ApiId.Composite(_parts?.ToArray());
    }
    #endregion
}
