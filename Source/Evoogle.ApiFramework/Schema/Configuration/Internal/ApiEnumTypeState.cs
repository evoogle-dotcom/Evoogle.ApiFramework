// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores enum values collected while an API enum type is configured.
/// </summary>
internal sealed class ApiEnumTypeState
{
    #region Properties
    internal List<ApiEnumValueBuilder> Values { get; } = [];
    #endregion
}
