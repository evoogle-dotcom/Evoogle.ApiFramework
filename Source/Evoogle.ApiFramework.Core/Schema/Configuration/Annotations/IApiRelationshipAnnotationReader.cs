// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>Reads declarative relationship annotations.</summary>
public interface IApiRelationshipAnnotationReader : IApiAnnotationReader
{
    #region Methods
    /// <summary>Reads one-to-many relationship declarations for a CLR type.</summary>
    /// <param name="clrType">The CLR type being inspected.</param>
    /// <returns>The declarative one-to-many relationship results.</returns>
    IReadOnlyList<ApiOneToManyRelationshipAnnotationResult> ReadOneToManyRelationships(Type clrType);

    /// <summary>Reads one-to-one relationship declarations for a CLR type.</summary>
    /// <param name="clrType">The CLR type being inspected.</param>
    /// <returns>The declarative one-to-one relationship results.</returns>
    IReadOnlyList<ApiOneToOneRelationshipAnnotationResult> ReadOneToOneRelationships(Type clrType);

    /// <summary>Reads many-to-many relationship declarations for a CLR type.</summary>
    /// <param name="clrType">The CLR type being inspected.</param>
    /// <returns>The declarative many-to-many relationship results.</returns>
    IReadOnlyList<ApiManyToManyRelationshipAnnotationResult> ReadManyToManyRelationships(Type clrType);
    #endregion
}
