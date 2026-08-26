// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores key name and path configuration for a key type builder.
/// </summary>
internal sealed class ApiKeyTypeState
{
    #region Properties
    internal string? ApiName { get; set; }

    internal List<ApiKeyPathBuilder> KeyPathBuilders { get; } = [];

    internal ApiConfigurationSource RegistrationSource { get; set; } = ApiConfigurationSource.Convention;
    #endregion
}
