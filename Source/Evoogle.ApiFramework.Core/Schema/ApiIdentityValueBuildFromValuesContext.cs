// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Provides context for building an <see cref="ApiIdentityValue"/> from pre-extracted key/value pairs
///     using <see cref="ApiIdentity.BuildValue(ApiIdentityValueBuildFromValuesContext)"/>.
/// </summary>
public sealed record ApiIdentityValueBuildFromValuesContext
{
    /// <summary>
    ///     Gets the dictionary of property name to value mappings from which identity values will be read.
    /// </summary>
    public required IReadOnlyDictionary<string, object?> Values { get; init; }

    /// <summary>
    ///     Gets the optional dictionary of owner property name to value mappings, used to resolve owner identity parts.
    ///     <see langword="null"/> when no owner relationship exists or the owner values are not available.
    /// </summary>
    public IReadOnlyDictionary<string, object?>? OwnerValues { get; init; }

    /// <summary>
    ///     Gets the behavior when a value is null or a nested object is not available.
    ///     Defaults to <see cref="ApiIdentityNullHandling.ReturnEmpty"/>.
    /// </summary>
    public ApiIdentityNullHandling NullHandling { get; init; } = ApiIdentityNullHandling.ReturnEmpty;
}
