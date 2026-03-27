// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a single component within a composite <see cref="ApiId"/>.
///     Each part consists of an optional name (for named composites) and an <see cref="ApiId"/> value.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiIdPart"/> is used to build composite identifiers through <see cref="ApiId.Composite(ApiIdPart[])"/>.
///         Parts can be either named (e.g., "CustomerId=42") or unnamed/positional (e.g., "42").
///     </para>
///     <para>
///         Invariants enforced by <see cref="ApiId"/>:
///         <list type="bullet">
///             <item>All parts in a composite must be either named or unnamed (no mixing).</item>
///             <item>Part values must be scalar (no nested composites).</item>
///             <item>Named parts must have unique names within the same composite.</item>
///         </list>
///     </para>
/// </remarks>
/// <param name="ApiName">The optional name for this part. Non-null/non-whitespace for named composites, null for ordered composites.</param>
/// <param name="ApiValue">The <see cref="ApiId"/> value for this part. Must be a scalar (non-composite) identifier.</param>
public readonly record struct ApiIdPart(string? ApiName, ApiId ApiValue)
{
    #region Properties
    /// <summary>
    ///     Gets whether this part is named (has a non-null, non-whitespace <see cref="ApiName"/>).
    /// </summary>
    /// <value>
    ///     <see langword="true"/> if <see cref="ApiName"/> is non-null and contains at least one non-whitespace character;
    ///     otherwise <see langword="false"/>.
    /// </value>
    public bool IsNamed => !string.IsNullOrWhiteSpace(this.ApiName);
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates an unnamed (positional) identifier part.
    /// </summary>
    /// <param name="value">The scalar identifier value for this part.</param>
    /// <returns>
    ///     A new <see cref="ApiIdPart"/> with <see cref="ApiName"/> set to <see langword="null"/> and
    ///     <see cref="ApiValue"/> set to <paramref name="value"/>.
    /// </returns>
    /// <remarks>
    ///     Use this factory method when building ordered/positional composite identifiers where
    ///     part order matters and names are not needed.
    /// </remarks>
    public static ApiIdPart Create(ApiId value) => new(null, value);

    /// <summary>
    ///     Creates a named identifier part with the specified name and value.
    /// </summary>
    /// <param name="name">
    ///     The name for this part. Can be <see langword="null"/> for unnamed parts,
    ///     but typically should be non-null/non-whitespace for named composites.
    /// </param>
    /// <param name="value">The scalar identifier value for this part.</param>
    /// <returns>
    ///     A new <see cref="ApiIdPart"/> with <see cref="ApiName"/> set to <paramref name="name"/> and
    ///     <see cref="ApiValue"/> set to <paramref name="value"/>.
    /// </returns>
    /// <remarks>
    ///     Use this factory method when building named composite identifiers where each part
    ///     has semantic meaning indicated by its name (e.g., "CustomerId", "OrderNumber").
    /// </remarks>
    public static ApiIdPart Create(string? name, ApiId value) => new(name, value);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString() => this.ApiName is null ? this.ApiValue.SafeToString() : $"{this.ApiName}={this.ApiValue.SafeToString()}";
    #endregion
}
