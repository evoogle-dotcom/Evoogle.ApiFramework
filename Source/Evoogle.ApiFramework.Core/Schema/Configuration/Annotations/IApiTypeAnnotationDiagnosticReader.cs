// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>
///     Reads diagnostics associated with type-level annotations.
/// </summary>
public interface IApiTypeAnnotationDiagnosticReader : IApiTypeAnnotationReader
{
    #region Methods
    /// <summary>Reads diagnostics for a CLR type's built-in or custom type markers.</summary>
    /// <param name="clrType">The CLR type being inspected.</param>
    /// <returns>The diagnostics associated with the CLR type.</returns>
    IReadOnlyList<ApiAnnotationReaderDiagnostic> ReadTypeAnnotationDiagnostics(Type clrType);
    #endregion
}
