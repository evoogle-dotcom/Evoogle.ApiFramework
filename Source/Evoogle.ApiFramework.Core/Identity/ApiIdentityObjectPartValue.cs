// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents an object identity part value that holds a nested <see cref="ApiIdentityValue"/>,
///     representing either a nested object identity or an owner object identity.
/// </summary>
/// <param name="apiName">The name of the identity part as declared in the schema.</param>
/// <param name="apiObjectValue">
///     The nested <see cref="ApiIdentityValue"/> when fully resolved, or <see langword="null"/> when the nested object
///     was not available at build time (unresolved).
///     When unresolved, <paramref name="apiStructure"/> preserves the expected shape for flattening operations.
/// </param>
/// <param name="apiStructure">
///     The structural skeleton of <see cref="ApiIdentityPartValue"/> entries representing the expected shape
///     of the nested identity.
///     Used when <paramref name="apiObjectValue"/> is <see langword="null"/> so that <see cref="ApiIdentityValue.ToApiId"/> can emit the correct number of <see cref="ApiId.Empty"/> slots.
/// </param>
public sealed class ApiIdentityObjectPartValue(string apiName, ApiIdentityValue? apiObjectValue, IEnumerable<ApiIdentityPartValue>? apiStructure = null) : ApiIdentityPartValue(apiName)
{
    #region ApiIdentityPartValue Properties
    /// <inheritdoc/>
    public override ApiIdentityPartValueKind ApiKind => ApiIdentityPartValueKind.Object;
    #endregion

    #region ApiIdentityObjectPartValue Properties
    /// <summary>
    ///     Gets the nested <see cref="ApiIdentityValue"/> when fully resolved, or <see langword="null"/> when unresolved.
    /// </summary>
    public ApiIdentityValue? ApiObjectValue { get; } = apiObjectValue;

    /// <summary>
    ///     Gets the structural skeleton for unresolved object parts, or <see langword="null"/> when the nested identity
    ///     resolves to a single scalar (no composite wrapper needed).
    /// </summary>
    public ApiIdentityPartValue[]? ApiStructure { get; } = apiStructure != null ? [.. apiStructure.Where(x => x is not null)] : null;
    #endregion

    #region ApiIdentityObjectPartValue Computed Properties
    /// <summary>Gets a value indicating whether the nested object identity was fully resolved at build time.</summary>
    public bool IsResolved => this.ApiObjectValue is not null;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiObjectValue = this.ApiObjectValue.SafeToString();
        var apiStructure = this.ApiStructure.SafeToDelimitedString(',');

        return $"{nameof(ApiIdentityObjectPartValue)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiObjectValue)}={apiObjectValue}, {nameof(this.ApiStructure)}={apiStructure}}}";
    }
    #endregion
}
