// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides configuration for an <see cref="ApiScalarTypeBuilder"/>.
/// </summary>
public interface IApiScalarTypeConfiguration
{
    #region Methods
    /// <summary>
    ///     Applies configuration to the supplied <see cref="ApiScalarTypeBuilder"/>.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    void Configure(ApiScalarTypeBuilder builder);
    #endregion
}