// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Provides context for building an <see cref="ApiIdentityValue"/> from a CLR object instance
///     using <see cref="ApiIdentity.BuildValue(ApiIdentityValueBuildContext)"/>.
/// </summary>
public sealed record ApiIdentityValueBuildContext
{
    /// <summary>
    ///     Gets the CLR object instance from which identity property values will be extracted.
    /// </summary>
    public required object ClrInstance { get; init; }

    /// <summary>
    ///     Gets the optional CLR owner object instance, used to resolve owner identity parts.
    ///     <see langword="null"/> when no owner relationship exists or the owner is not available.
    /// </summary>
    public object? ClrOwnerInstance { get; init; }

    /// <summary>
    ///     Gets the behavior when a property value is null or a nested object is not available.
    ///     Defaults to <see cref="ApiIdentityNullHandling.ReturnEmpty"/>.
    /// </summary>
    public ApiIdentityNullHandling NullHandling { get; init; } = ApiIdentityNullHandling.ReturnEmpty;
}
