// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Holds optional per-type configuration for an <see cref="ApiObjectType"/>, allowing options to be overridden on a type-by-type basis.
/// </summary>
[JsonConverter(typeof(ApiObjectTypeOptionsJsonConverter))]
public record class ApiObjectTypeOptions
{
    #region Properties
    /// <summary>
    ///     Gets the null-handling strategy applied when resolving keys on this object type.
    ///     When <see langword="null"/>, the strategy is inherited from the containing <see cref="ApiSchema"/>'s options.
    /// </summary>
    public ApiKeyNullHandling? ApiKeyNullHandling { get; init; }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiKeyNullHandling = this.ApiKeyNullHandling.SafeToString();

        return $"{nameof(ApiObjectTypeOptions)} {{{nameof(this.ApiKeyNullHandling)}={apiKeyNullHandling}}}";
    }
    #endregion
}
