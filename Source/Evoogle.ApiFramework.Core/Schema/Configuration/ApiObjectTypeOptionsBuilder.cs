// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiObjectTypeOptions"/>.
/// </summary>
public sealed class ApiObjectTypeOptionsBuilder()
{
    #region Fields
    private ApiIdentityPartNullHandling? _apiIdentityPartNullHandling = null;
    #endregion

    #region With Methods
    /// <summary>
    ///     Configures key-part null handling to throw an <see cref="ApiIdentityException"/> when a key part is null or cannot be resolved during identity construction.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeOptionsBuilder ThrowOnNullKeyPart()
    {
        _apiIdentityPartNullHandling = ApiIdentityPartNullHandling.ThrowOnNull;
        return this;
    }

    /// <summary>
    ///     Configures key-part null handling to use the default identity value behavior when a key part is null or cannot be resolved during identity construction, rather than throwing.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeOptionsBuilder UseDefaultOnNullKeyPart()
    {
        _apiIdentityPartNullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull;
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
            ApiIdentityPartNullHandling = _apiIdentityPartNullHandling
        };
    }
    #endregion
}
