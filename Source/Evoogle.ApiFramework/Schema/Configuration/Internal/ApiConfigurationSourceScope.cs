// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Provides a synchronous, nestable configuration-source scope for fluent configuration
///     callbacks whose public surface does not expose configuration provenance.
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
