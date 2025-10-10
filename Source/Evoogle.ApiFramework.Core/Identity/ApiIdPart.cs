// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a part of an API identifier, consisting of an optional name and a value.
/// </summary>
/// <param name="name">The optional name associated with the API identifier part.</param>
/// <param name="value">The value of the API identifier part.</param>
public readonly struct ApiIdPart(string? name, ApiId value)
{
    #region Properties
    /// <summary>Gets the optional name associated with the API identifier part.</summary>
    public string? Name { get; } = name;

    /// <summary>Gets the value of the API identifier part.</summary>
    public ApiId Value { get; } = value;

    /// <summary>Indicates whether the API identifier part has a name.</summary>
    public bool IsNamed => this.Name is not null;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString() => this.Name is null ? this.Value.SafeToString() : $"{this.Name}={this.Value.SafeToString()}";
    #endregion
}
