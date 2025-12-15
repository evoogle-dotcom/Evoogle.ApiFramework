// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

public record class ApiObjectTypeOptions
{
    #region Fields
    private static readonly ApiSchemaOptions _default = new();
    #endregion

    #region Properties
    public static ApiSchemaOptions Default => _default;

    public ApiIdentityNullHandling? ApiIdentityNullHandling { get; set; }
    #endregion

    #region Methods
    internal ApiIdentityNullHandling GetIdentityNullHandling(ApiObjectType parent)
    {
        return this.ApiIdentityNullHandling ?? parent.ApiSchemaContext.ApiSchema.ApiSchemaOptions.ApiIdentityNullHandling;
    }
    #endregion
}
