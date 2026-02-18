// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Identity.Json;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a named part of a composite identity snapshot.
/// </summary>
/// <param name="Name">
///     The name of this identity part (property name for nested objects).
/// </param>
/// <param name="Snapshot">
///     The nested identity snapshot, or null if the part is unresolved.
/// </param>
/// <param name="Structure">
///     The structural definition of this part's children.
///     Used to emit correct ApiId structure when Snapshot is null and UnresolvedPartBehavior is UseEmpty.
///     Null for scalar leaf parts.
/// </param>
[JsonConverter(typeof(ApiIdentityPartJsonConverter))]
public readonly record struct ApiIdentityPart
(
    string Name,
    ApiIdentitySnapshot? Snapshot,
    IReadOnlyList<ApiIdentityPart>? Structure = null
)
{
    #region Properties
    /// <summary>
    ///     Gets whether this part is resolved (has a non-null snapshot).
    /// </summary>
    public bool IsResolved => this.Snapshot is not null;

    /// <summary>
    ///     Gets whether this part is unresolved (has a null snapshot).
    /// </summary>
    public bool IsUnresolved => this.Snapshot is null;

    /// <summary>
    ///     Gets whether this part has structural definition information.
    /// </summary>
    public bool HasStructure => this.Structure is not null && this.Structure.Count > 0;
    #endregion
}
