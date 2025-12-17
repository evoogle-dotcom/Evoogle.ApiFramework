// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public record class ApiObjectTypeOptions
{
    #region Fields
    private static readonly ApiObjectTypeOptions _default = new();
    #endregion

    #region Properties
    public static ApiObjectTypeOptions Default => _default;

    public ApiIdentityNullHandling? ApiIdentityNullHandling { get; set; }
    #endregion

    #region Methods
    internal ApiIdentityNullHandling GetIdentityNullHandling(ApiObjectType parent)
    {
        return this.ApiIdentityNullHandling ?? parent.ApiSchemaContext.ApiSchemaOptions.ApiIdentityNullHandling;
    }
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
