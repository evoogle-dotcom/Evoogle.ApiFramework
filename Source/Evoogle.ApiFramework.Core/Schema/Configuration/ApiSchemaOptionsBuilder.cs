// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiSchemaOptions"/>.
/// </summary>
public sealed class ApiSchemaOptionsBuilder()
{
    #region Fields
    private ApiKeyNullHandling _apiKeyNullHandling = ApiSchemaOptions.Default.ApiKeyNullHandling;
    #endregion

    #region With Methods
    /// <summary>
    ///     Configures key null handling to throw an <see cref="ApiKeyException"/> when a key path segment is null or cannot be resolved during key construction.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaOptionsBuilder ThrowOnNullKeyPart()
    {
        _apiKeyNullHandling = ApiKeyNullHandling.ThrowOnNull;
        return this;
    }

    /// <summary>
    ///     Configures key null handling to use the default key value behavior when a key path segment is null or cannot be resolved during key construction, rather than throwing.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaOptionsBuilder UseDefaultOnNullKeyPart()
    {
        _apiKeyNullHandling = ApiKeyNullHandling.UseDefaultOnNull;
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
            ApiKeyNullHandling = _apiKeyNullHandling
        };
    }
    #endregion
}
