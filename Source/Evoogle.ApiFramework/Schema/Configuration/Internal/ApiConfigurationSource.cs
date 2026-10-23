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
/// <remarks>
///     Builder states store non-nullable sources initialized to <see cref="Convention"/>, unless a constructor
///     supplies the source with an initial value. Optional configuration is represented by the value itself.
///     Accept a setting when its incoming source is greater than or equal to the stored source, updating both
///     the value and source together. Registrations retain the highest source supplied on creation or reuse.
///     Callback scopes default to <see cref="Explicit"/> because they describe the operation currently executing.
/// </remarks>
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
