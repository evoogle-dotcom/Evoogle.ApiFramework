// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a part of an API composite identifier, consisting of an optional name and a value.
/// </summary>
/// <param name="name">The optional name associated with the API identifier part.</param>
/// <param name="value">The value of the API identifier part.</param>
public readonly record struct ApiIdPart(string? Name, ApiId Value)
{
    #region Properties
    /// <summary>Indicates whether the API identifier part has a name.</summary>
    public bool IsNamed => string.IsNullOrWhiteSpace(this.Name) is false;
    #endregion

    #region Factory Methods
    /// <summary>Creates an unnamed API identifier part with the specified value.</summary>
    /// <param name="value">The value of the API identifier part.</param>
    /// <returns>A new <see cref="ApiIdPart"/> whose <see cref="Name"/> is null and whose <see cref="Value"/> equals <paramref name="value"/>.</returns>
    public static ApiIdPart Create(ApiId value) => new(null, value);

    /// <summary>Creates a named API identifier part with the specified name and value.</summary>
    /// <param name="name">The name associated with the API identifier part.</param>
    /// <param name="value">The value of the API identifier part.</param>
    /// <returns>A new <see cref="ApiIdPart"/> whose <see cref="Name"/> equals <paramref name="name"/> and whose <see cref="Value"/> equals <paramref name="value"/>.</returns>
    public static ApiIdPart Create(string name, ApiId value) => new(name, value);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString() => this.Name is null ? this.Value.SafeToString() : $"{this.Name}={this.Value.SafeToString()}";
    #endregion
}
