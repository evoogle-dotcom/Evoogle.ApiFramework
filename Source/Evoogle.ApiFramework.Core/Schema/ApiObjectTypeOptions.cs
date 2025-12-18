// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

[JsonConverter(typeof(ApiObjectTypeOptionsJsonConverter))]
public record class ApiObjectTypeOptions
{
    #region Properties
    public ApiIdentityNullHandling? ApiIdentityNullHandling { get; init; }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiIdentityNullHandling = this.ApiIdentityNullHandling.SafeToString();

        return $"{nameof(ApiObjectTypeOptions)} {{{nameof(this.ApiIdentityNullHandling)}={apiIdentityNullHandling}}}";
    }
    #endregion
}
