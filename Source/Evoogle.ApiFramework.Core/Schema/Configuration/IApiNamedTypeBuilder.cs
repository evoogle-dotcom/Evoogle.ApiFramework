// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Base interface for all API named type builders.
/// </summary>
public interface IApiNamedTypeBuilder
{
    #region Properties
    /// <summary>
    ///     Gets the configured API name.
    /// </summary>
    string ApiName { get; }

    /// <summary>
    ///     Gets the CLR type that the API type maps to.
    /// </summary>
    Type ClrType { get; }
    #endregion
}