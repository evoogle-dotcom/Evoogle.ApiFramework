// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Evoogle.ApiFramework.Schema.Json.Internal;

/// <summary>
///     Common read data contract for JSON converters that support extensions.
/// </summary>
internal class ExtensibleReadData
{
    /// <summary>
    ///     Gets or sets the extensions read from JSON.
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }
}

