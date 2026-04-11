// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides configuration for an <see cref="ApiRelationshipOneToManyBuilder"/>.
/// </summary>
public interface IApiRelationshipOneToManyConfiguration
{
    #region Methods
    /// <summary>
    ///     Applies configuration to the supplied <see cref="ApiRelationshipOneToManyBuilder"/>.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    void Configure(ApiRelationshipOneToManyBuilder builder);
    #endregion
}
