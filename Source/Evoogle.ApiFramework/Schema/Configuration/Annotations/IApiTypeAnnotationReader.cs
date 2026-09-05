// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>
///     Reads annotations that configure named API types.
/// </summary>
public interface IApiTypeAnnotationReader : IApiAnnotationReader
{
    #region Methods
    /// <summary>Reads annotations for an API object type.</summary>
    /// <param name="clrType">The CLR object type being configured.</param>
    /// <returns>The declarative type annotation results.</returns>
    IReadOnlyList<ApiTypeAnnotationResult> ReadObjectTypeAnnotations(Type clrType);

    /// <summary>Reads annotations for an API scalar type.</summary>
    /// <param name="clrType">The CLR scalar type being configured.</param>
    /// <returns>The declarative type annotation results.</returns>
    IReadOnlyList<ApiTypeAnnotationResult> ReadScalarTypeAnnotations(Type clrType);

    /// <summary>Reads annotations for an API enum type.</summary>
    /// <param name="clrType">The CLR enum type being configured.</param>
    /// <returns>The declarative type annotation results.</returns>
    IReadOnlyList<ApiTypeAnnotationResult> ReadEnumTypeAnnotations(Type clrType);
    #endregion
}
