// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Key;

/// <summary>
///     Represents a single component within a composite <see cref="ApiKey"/>.
///     Each part consists of an optional name (for named composites) and an <see cref="ApiKey"/> value.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiKeyPart"/> is used to build composite keys through <see cref="ApiKey.Composite(ApiKeyPart[])"/>.
///         Parts can be either named (e.g., "CustomerId=42") or unnamed/positional (e.g., "42").
///     </para>
///     <para>
///         Invariants enforced by <see cref="ApiKey"/>:
///         <list type="bullet">
///             <item>All parts in a composite must be either named or unnamed (no mixing).</item>
///             <item>Part values must be scalar (no nested composites).</item>
///             <item>Named parts must have unique names within the same composite.</item>
///         </list>
///     </para>
/// </remarks>
/// <param name="ApiName">The optional name for this part. Non-null/non-whitespace for named composites, null for ordered composites.</param>
/// <param name="ApiValue">The <see cref="ApiKey"/> value for this part. Must be a scalar (non-composite) key.</param>
public readonly record struct ApiKeyPart(string? ApiName, ApiKey ApiValue)
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
    ///     Creates an unnamed (positional) key part.
    /// </summary>
    /// <param name="apiValue">The scalar key value for this part.</param>
    /// <returns>
    ///     A new <see cref="ApiKeyPart"/> with <see cref="ApiName"/> set to <see langword="null"/> and
    ///     <see cref="ApiValue"/> set to <paramref name="apiValue"/>.
    /// </returns>
    /// <remarks>
    ///     Use this factory method when building ordered/positional composite keys where
    ///     part order matters and names are not needed.
    /// </remarks>
    public static ApiKeyPart Create(ApiKey apiValue) => new(null, apiValue);

    /// <summary>
    ///     Creates a named key part with the specified API name and value.
    /// </summary>
    /// <param name="apiName">
    ///     The API name for this part. Can be <see langword="null"/> for unnamed parts,
    ///     but typically should be non-null/non-whitespace for named composites.
    /// </param>
    /// <param name="apiValue">The scalar key value for this part.</param>
    /// <returns>
    ///     A new <see cref="ApiKeyPart"/> with <see cref="ApiName"/> set to <paramref name="apiName"/> and
    ///     <see cref="ApiValue"/> set to <paramref name="apiValue"/>.
    /// </returns>
    /// <remarks>
    ///     Use this factory method when building named composite keys where each part
    ///     has semantic meaning indicated by its name (e.g., "CustomerId", "OrderNumber").
    /// </remarks>
    public static ApiKeyPart Create(string? apiName, ApiKey apiValue) => new(apiName, apiValue);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString() => this.ApiName is null ? this.ApiValue.SafeToString() : $"{this.ApiName}={this.ApiValue.SafeToString()}";
    #endregion
}
