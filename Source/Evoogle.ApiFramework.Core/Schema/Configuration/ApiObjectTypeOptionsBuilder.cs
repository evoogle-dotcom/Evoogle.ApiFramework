// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiObjectTypeOptions"/>.
/// </summary>
public sealed class ApiObjectTypeOptionsBuilder()
{
    #region Fields
    private ApiIdentityNullHandling? _apiIdentityNullHandling = null;
    #endregion

    #region With Methods
    /// <summary>
    ///     Configures identity null handling to return <see cref="ApiId.Empty"/> when a property value
    ///     is null or a part is unresolved.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeOptionsBuilder ReturnEmptyOnNull()
    {
        _apiIdentityNullHandling = ApiIdentityNullHandling.ReturnEmpty;
        return this;
    }

    /// <summary>
    ///     Configures identity null handling to throw an <see cref="Exceptions.ApiIdentityException"/>
    ///     when a property value is null or a part is unresolved.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeOptionsBuilder ThrowOnNull()
    {
        _apiIdentityNullHandling = ApiIdentityNullHandling.ThrowException;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the configured <see cref="ApiObjectTypeOptions"/> instance.
    /// </summary>
    /// <returns>The configured options.</returns>
    internal ApiObjectTypeOptions Build()
    {
        return new ApiObjectTypeOptions
        {
            ApiIdentityNullHandling = _apiIdentityNullHandling
        };
    }
    #endregion
}
