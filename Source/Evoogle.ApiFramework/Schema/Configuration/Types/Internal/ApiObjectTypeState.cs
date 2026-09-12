// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Key;
using Evoogle.ApiFramework.Schema.Configuration.Version;

namespace Evoogle.ApiFramework.Schema.Configuration.Types.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiObjectTypeState
{
    #region Properties
    internal List<ApiKeyDefinitionBuilder> KeyBuilders { get; } = [];

    internal List<ApiPropertyBuilder> PropertyBuilders { get; } = [];

    internal Action<ApiObjectTypeOptionsBuilder>? OptionsConfiguration { get; set; }

    internal ApiVersionDefinitionBuilder? VersionBuilder { get; set; }

    internal ApiConfigurationSource? VersionConfigurationSource { get; set; }
    #endregion
}
