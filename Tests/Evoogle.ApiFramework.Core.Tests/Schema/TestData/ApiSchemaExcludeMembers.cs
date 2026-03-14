// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.TestData;

public static class ApiSchemaExcludeMembers
{
    public static readonly List<ExcludeMember> SchemaInitialized =
    [
        // ApiSchema
        new ExcludeMember(typeof(ApiSchema), nameof(ApiSchema.ApiSchemaContext)),
    ];

    public static readonly List<ExcludeMember> Standard =
    [
        // ApiSchema
        new ExcludeMember(typeof(ApiSchema), nameof(ApiSchema.ApiPath)),
        new ExcludeMember(typeof(ApiSchema), nameof(ApiSchema.ApiSchemaContext)),

        // ApiSchemaElement
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.ApiPath)),

        // ApiTypeExpression
        new ExcludeMember(typeof(ApiTypeExpression), nameof(ApiTypeExpression.ApiType)),

        // ApiCollectionType
        new ExcludeMember(typeof(ApiCollectionType), nameof(ApiCollectionType.ApiItemType)),

        // ApiIdentityPart
        new ExcludeMember(typeof(ApiPropertyIdentityPart), nameof(ApiPropertyIdentityPart.ApiProperty)),
        new ExcludeMember(typeof(ApiScalarIdentityPart), nameof(ApiScalarIdentityPart.ClrScalarType)),

        // ApiProperty
        new ExcludeMember(typeof(ApiProperty), nameof(ApiProperty.ApiType)),

        // ApiRelationship
        new ExcludeMember(typeof(ApiRelationship), nameof(ApiRelationship.ApiCardinality)),
        new ExcludeMember(typeof(ApiRelationship), nameof(ApiRelationship.ApiProperty)),
    ];
}
