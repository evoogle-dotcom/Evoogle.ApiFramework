// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines the severity levels for issues encountered during API schema compilation.
/// </summary>
public enum ApiSchemaCompilationSeverity
{
    #region Values
    /// <summary>
    ///     Informational message that does not prevent compilation.
    /// </summary>
    Info,

    /// <summary>
    ///     Warning about a potential issue that does not prevent compilation.
    /// </summary>
    Warning,

    /// <summary>
    ///     Error that prevents successful compilation.
    /// </summary>
    Error
    #endregion
}
