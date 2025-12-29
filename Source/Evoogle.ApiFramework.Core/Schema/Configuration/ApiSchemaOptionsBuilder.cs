// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiSchemaOptions"/>.
/// </summary>
public sealed class ApiSchemaOptionsBuilder()
{
    #region Fields
    private ApiIdentityNullHandling _apiIdentityNullHandling = ApiSchemaOptions.Default.ApiIdentityNullHandling;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Sets the identity null handling strategy.
    /// </summary>
    /// <param name="handling">The null handling strategy to use.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaOptionsBuilder WithIdentityNullHandling(ApiIdentityNullHandling handling)
    {
        _apiIdentityNullHandling = handling;
        return this;
    }

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
