// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Keys;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiSchemaOptionsState
{
    #region Properties
    internal ApiKeyNullHandling ApiKeyNullHandling { get; set; } = ApiSchemaOptions.Default.ApiKeyNullHandling;
    #endregion
}
