// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores mutable API name and type-modifier configuration for a property.
/// </summary>
internal sealed class ApiPropertyState(string apiName, ApiConfigurationSource apiNameSource)
{
    #region Properties
    internal string ApiName { get; set; } = apiName;

    internal ApiConfigurationSource ApiNameSource { get; set; } = apiNameSource;

    internal Action<ApiTypeModifiersBuilder>? Modifiers { get; set; }

    internal ApiConfigurationSource? ModifiersSource { get; set; }
    #endregion
}
