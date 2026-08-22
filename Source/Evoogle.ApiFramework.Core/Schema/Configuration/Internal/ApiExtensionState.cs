// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores extension metadata collected while a schema builder is being configured.
/// </summary>
internal sealed class ApiExtensionState
{
    #region Properties
    internal OrderedDictionary<Type, object> Extensions { get; } = [];
    #endregion
}
