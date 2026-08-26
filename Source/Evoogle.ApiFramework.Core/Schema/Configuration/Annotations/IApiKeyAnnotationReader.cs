// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>Reads complete named-key declarations for an object CLR type.</summary>
public interface IApiKeyAnnotationReader : IApiAnnotationReader
{
    #region Methods
    /// <summary>Reads all key-path contributions declared on an object type and its members.</summary>
    /// <param name="clrType">The object CLR type whose complete key declarations are read.</param>
    /// <returns>The declarative key annotation results.</returns>
    IReadOnlyList<ApiKeyAnnotationResult> ReadKeyAnnotations(Type clrType);
    #endregion
}
