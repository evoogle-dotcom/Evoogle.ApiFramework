// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores mutable API naming configuration for an enum value.
/// </summary>
internal sealed class ApiEnumValueState(string apiName, ApiConfigurationSource apiNameSource)
{
    #region Properties
    internal string ApiName { get; set; } = apiName;

    internal ApiConfigurationSource ApiNameSource { get; set; } = apiNameSource;
    #endregion
}
