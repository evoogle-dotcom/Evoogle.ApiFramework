// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores object properties, keys, and options collected during configuration.
/// </summary>
internal sealed class ApiObjectTypeState
{
    #region Properties
    internal List<ApiKeyTypeBuilder> KeyTypeBuilders { get; } = [];

    internal List<ApiPropertyBuilder> PropertyBuilders { get; } = [];

    internal Action<ApiObjectTypeOptionsBuilder>? OptionsConfiguration { get; set; }
    #endregion
}
