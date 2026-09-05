// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Identifies the source precedence reported by an API schema build trace.
/// </summary>
public enum ApiSchemaBuildConfigurationSource
{
    /// <summary>Reports a default or convention-provided value.</summary>
    Convention = 0,

    /// <summary>Reports a value provided by a data annotation.</summary>
    DataAnnotation = 1,

    /// <summary>Reports a value supplied through explicit builder configuration.</summary>
    Explicit = 2,
}
