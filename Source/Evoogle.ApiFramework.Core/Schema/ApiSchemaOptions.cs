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
///     Holds schema-wide configuration options for all types within an <see cref="ApiSchema"/>.
/// </summary>
[JsonConverter(typeof(ApiSchemaOptionsJsonConverter))]
public record class ApiSchemaOptions
{
    #region Fields
    private static readonly ApiSchemaOptions _default = new();
    #endregion

    #region Properties
    /// <summary>Gets the default <see cref="ApiSchemaOptions"/> instance with all settings at their out-of-the-box defaults.</summary>
    public static ApiSchemaOptions Default => _default;

    /// <summary>
    ///     Gets the null-handling strategy applied when resolving keys across the entire schema.
    ///     Defaults to <see cref="ApiKeyNullHandling.UseDefaultOnNull"/>.
    /// </summary>
    public ApiKeyNullHandling ApiKeyNullHandling { get; init; } = ApiKeyNullHandling.UseDefaultOnNull;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiKeyNullHandling = this.ApiKeyNullHandling.SafeToString();

        return $"{nameof(ApiSchemaOptions)} {{{nameof(this.ApiKeyNullHandling)}={apiKeyNullHandling}}}";
    }
    #endregion
}
