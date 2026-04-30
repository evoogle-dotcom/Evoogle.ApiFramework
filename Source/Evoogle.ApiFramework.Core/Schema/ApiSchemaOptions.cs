// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.Json;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Holds schema-wide configuration options that control identity resolution and serialization behaviour
///     for all types within an <see cref="ApiSchema"/>.
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
    ///     Gets the null-handling strategy applied when resolving identities across the entire schema.
    ///     Defaults to <see cref="ApiIdentityPartNullHandling.UseDefaultOnNull"/>.
    /// </summary>
    public ApiIdentityPartNullHandling ApiIdentityPartNullHandling { get; init; } = ApiIdentityPartNullHandling.UseDefaultOnNull;
    #endregion
}
