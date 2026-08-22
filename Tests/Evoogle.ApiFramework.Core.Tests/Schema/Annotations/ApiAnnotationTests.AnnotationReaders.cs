// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Annotations;

internal sealed class BuildObservableEnumValueAnnotationReader : IApiAnnotationReader
{
    #region IApiAnnotationReader Methods
    public void ApplyObjectTypeAnnotations(Type clrType, ApiObjectTypeBuilder builder)
    {
    }

    public void ApplyScalarTypeAnnotations(Type clrType, ApiScalarTypeBuilder builder)
    {
    }

    public void ApplyEnumTypeAnnotations(Type clrType, ApiEnumTypeBuilder builder)
    {
    }

    public void ApplyEnumValueAnnotations
    (
        FieldInfo clrField,
        ApiEnumTypeBuilder enumTypeBuilder,
        ApiEnumValueBuilder enumValueBuilder
    )
    {
        enumValueBuilder.WithName($"reader_{enumValueBuilder.ClrName}");
    }

    public void ApplyPropertyAnnotations
    (
        MemberInfo clrMember,
        ClrMemberKind clrMemberKind,
        MemberNullableInfo clrNullabilityInfo,
        ApiObjectTypeBuilder objectTypeBuilder,
        ApiPropertyBuilder propertyBuilder
    )
    {
    }

    public IReadOnlyList<(string ApiName, Action<ApiRelationshipOneToManyBuilder> Configure)>
        ReadOneToManyRelationships(Type clrType) => [];

    public IReadOnlyList<(string ApiName, Action<ApiRelationshipOneToOneBuilder> Configure)>
        ReadOneToOneRelationships(Type clrType) => [];

    public IReadOnlyList<(string ApiName, Action<ApiRelationshipManyToManyBuilder> Configure)>
        ReadManyToManyRelationships(Type clrType) => [];
    #endregion
}
