// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiObjectTypeOptions"/>.
/// </summary>
public sealed class ApiObjectTypeOptionsBuilder()
{
    #region Fields
    private ApiIdentityNullHandling? _apiIdentityNullHandling = ApiObjectTypeOptions.Default.ApiIdentityNullHandling;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Sets or clears the identity null handling strategy.
    /// </summary>
    /// <param name="handling">The optional null handling strategy to use.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeOptionsBuilder WithIdentityNullHandling(ApiIdentityNullHandling? handling)
    {
        _apiIdentityNullHandling = handling;
        return this;
    }

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
