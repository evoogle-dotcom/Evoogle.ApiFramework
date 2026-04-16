// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiSchemaOptions"/>.
/// </summary>
public sealed class ApiSchemaOptionsBuilder()
{
    #region Fields
    private ApiIdentityNullHandling _apiIdentityNullHandling = ApiSchemaOptions.Default.ApiIdentityNullHandling;
    #endregion

    #region With Methods
    /// <summary>
    ///     Configures identity null handling to return <see cref="ApiId.Empty"/> when a property value
    ///     is null or a part is unresolved.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaOptionsBuilder ReturnEmptyOnNull()
    {
        _apiIdentityNullHandling = ApiIdentityNullHandling.ReturnEmpty;
        return this;
    }

    /// <summary>
    ///     Configures identity null handling to throw an <see cref="Exceptions.ApiIdentityException"/>
    ///     when a property value is null or a part is unresolved.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaOptionsBuilder ThrowOnNull()
    {
        _apiIdentityNullHandling = ApiIdentityNullHandling.ThrowException;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the configured <see cref="ApiSchemaOptions"/> instance.
    /// </summary>
    /// <returns>The configured options.</returns>
    internal ApiSchemaOptions Build()
    {
        return new ApiSchemaOptions
        {
            ApiIdentityNullHandling = _apiIdentityNullHandling
        };
    }
    #endregion
}
