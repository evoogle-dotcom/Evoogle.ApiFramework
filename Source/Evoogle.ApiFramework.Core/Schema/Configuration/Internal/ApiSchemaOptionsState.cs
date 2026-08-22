// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores schema option values during configuration.
/// </summary>
internal sealed class ApiSchemaOptionsState
{
    #region Properties
    internal ApiKeyNullHandling ApiKeyNullHandling { get; set; } = ApiSchemaOptions.Default.ApiKeyNullHandling;
    #endregion
}
