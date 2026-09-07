// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiConfigurationSourceScope
{
    #region Fields
    private ApiConfigurationSource? _currentSource;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the active configuration source, or explicit precedence outside a scope.
    /// </summary>
    internal ApiConfigurationSource CurrentSource =>
        _currentSource ?? ApiConfigurationSource.Explicit;
    #endregion

    #region Methods
    /// <summary>Executes the callback at the supplied configuration-source precedence.</summary>
    internal void Apply(ApiConfigurationSource source, Action configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var previousSource = _currentSource;
        _currentSource = source;

        try
        {
            configure();
        }
        finally
        {
            _currentSource = previousSource;
        }
    }
    #endregion
}
