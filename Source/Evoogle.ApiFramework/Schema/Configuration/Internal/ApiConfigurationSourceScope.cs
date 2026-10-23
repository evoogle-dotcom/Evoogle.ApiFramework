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
/// <remarks>
///     Tracks the source of the operation currently executing. Ordinary fluent calls use explicit precedence.
///     Scoped callbacks temporarily replace that source and restore it even when the callback throws.
///     Stored builder-state sources instead start at convention precedence and describe retained configuration.
/// </remarks>
internal sealed class ApiConfigurationSourceScope
{
    #region Fields
    private ApiConfigurationSource _currentSource = ApiConfigurationSource.Explicit;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the active operation's source, initialized to explicit precedence.
    /// </summary>
    internal ApiConfigurationSource CurrentSource => _currentSource;
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
