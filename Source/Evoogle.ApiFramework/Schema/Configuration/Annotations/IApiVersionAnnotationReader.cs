// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>Reads object-version declarations for an object CLR type.</summary>
/// <remarks>
///     Readers run in registration order. A later valid reader contribution replaces an earlier
///     contribution at data-annotation precedence. Each result supplies either a CLR member name
///     for a property-backed version or a CLR type for a repository-backed version.
/// </remarks>
public interface IApiVersionAnnotationReader : IApiAnnotationReader
{
    #region Methods
    /// <summary>Reads all version declarations for an object CLR type.</summary>
    /// <param name="clrType">The object CLR type whose version declarations are read.</param>
    /// <returns>The declarative version annotation results.</returns>
    IReadOnlyList<ApiVersionAnnotationResult> ReadVersionAnnotations(Type clrType);
    #endregion
}
