// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

internal sealed class BuildObservableEnumValueAnnotationReader : IApiEnumValueAnnotationReader
{
    #region IApiEnumValueAnnotationReader Methods
    public IReadOnlyList<ApiEnumValueAnnotationResult> ReadEnumValueAnnotations
    (
        System.Reflection.FieldInfo clrField
    )
    {
        return [new($"reader_{clrField.Name}")];
    }
    #endregion
}
