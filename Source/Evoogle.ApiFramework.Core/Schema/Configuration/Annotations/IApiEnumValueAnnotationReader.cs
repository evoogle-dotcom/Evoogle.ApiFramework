// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>Reads annotations that configure API enum values.</summary>
public interface IApiEnumValueAnnotationReader : IApiAnnotationReader
{
    #region Methods
    /// <summary>Reads annotations for an enum member field.</summary>
    /// <param name="clrField">The CLR field defining the enum member.</param>
    /// <returns>The declarative enum-value annotation results.</returns>
    IReadOnlyList<ApiEnumValueAnnotationResult> ReadEnumValueAnnotations(FieldInfo clrField);
    #endregion
}
