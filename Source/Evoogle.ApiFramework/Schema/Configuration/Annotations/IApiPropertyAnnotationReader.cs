// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>Reads annotations that configure API properties.</summary>
public interface IApiPropertyAnnotationReader : IApiAnnotationReader
{
    #region Methods
    /// <summary>Reads annotations for a CLR property or field.</summary>
    /// <param name="clrMember">The CLR property or field being configured.</param>
    /// <param name="clrMemberKind">Whether the member is a property or field.</param>
    /// <param name="clrNullabilityInfo">The pre-computed CLR nullability information.</param>
    /// <returns>The declarative property annotation results.</returns>
    IReadOnlyList<ApiPropertyAnnotationResult> ReadPropertyAnnotations
    (
        MemberInfo clrMember,
        ClrMemberKind clrMemberKind,
        MemberNullableInfo clrNullabilityInfo
    );
    #endregion
}
