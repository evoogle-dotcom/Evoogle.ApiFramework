// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiSchemaOptions"/>.
/// </summary>
public sealed class ApiSchemaOptionsBuilder()
{
    #region Fields
    private ApiIdentityPartNullHandling _apiIdentityPartNullHandling = ApiSchemaOptions.Default.ApiIdentityPartNullHandling;
    #endregion

    #region With Methods
    /// <summary>
    ///     Configures key-part null handling to throw an <see cref="ApiIdentityException"/> when a key part is null or cannot be resolved during identity construction.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaOptionsBuilder ThrowOnNullKeyPart()
    {
        _apiIdentityPartNullHandling = ApiIdentityPartNullHandling.ThrowOnNull;
        return this;
    }

    /// <summary>
    ///     Configures key-part null handling to use the default identity value behavior when a key part is null or cannot be resolved during identity construction, rather than throwing.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaOptionsBuilder UseDefaultOnNullKeyPart()
    {
        _apiIdentityPartNullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull;
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
            ApiIdentityPartNullHandling = _apiIdentityPartNullHandling
        };
    }
    #endregion
}
