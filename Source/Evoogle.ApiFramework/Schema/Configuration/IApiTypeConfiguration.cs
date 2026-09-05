// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Identifies an API schema type configuration by its CLR type.
/// </summary>
public interface IApiTypeConfiguration : IApiConfiguration
{
    #region Properties
    /// <summary>
    ///     Gets the CLR type represented by the configuration.
    /// </summary>
    Type ClrType { get; }
    #endregion
}
