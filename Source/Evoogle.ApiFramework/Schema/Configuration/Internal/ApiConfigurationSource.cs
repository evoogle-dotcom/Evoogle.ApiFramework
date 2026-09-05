// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal enum ApiConfigurationSource
{
    /// <summary>
    ///     Applied to a default or inferred value, or by a framework or user-authored convention.
    /// </summary>
    Convention = 0,

    /// <summary>Applied by a CLR attribute annotation reader.</summary>
    DataAnnotation = 1,

    /// <summary>
    ///     Applied to a value expressly supplied for a configurable facet through a fluent builder
    ///     call or configuration class.
    /// </summary>
    Explicit = 2,
}
