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
        // ApiSchema - cycle: ApiSchema → ApiSchemaContext → ApiSchema → ...
        new ExcludeMember(typeof(ApiSchema), nameof(ApiSchema.ApiSchemaContext)),

        // ApiCollectionType — cycle: ApiItemType → ApiObjectType → ApiProperties[].ApiType → ...
        new ExcludeMember(typeof(ApiCollectionType), nameof(ApiCollectionType.ApiItemType)),

        // ApiProperty — cycle: ApiType → ApiObjectType → ApiProperties[].ApiType → ...
        new ExcludeMember(typeof(ApiProperty), nameof(ApiProperty.ApiType)),

        // ApiRelationship — cycle: ApiProperty → ApiProperty.ApiType → ApiObjectType → ...
        new ExcludeMember(typeof(ApiRelationship), nameof(ApiRelationship.ApiProperty)),

        // ApiPropertyIdentityPart — cycle: ApiProperty → ApiType → ApiObjectType → ApiIdentities[].ApiIdentityParts[].ApiProperty → ...
        new ExcludeMember(typeof(ApiPropertyIdentityPart), nameof(ApiPropertyIdentityPart.ApiProperty)),

        // ApiOwnerIdentityPart — cycle: ApiOwnerType → ApiObjectType → ... / ApiOwnerIdentity → ApiIdentity → ...
        new ExcludeMember(typeof(ApiOwnerIdentityPart), nameof(ApiOwnerIdentityPart.ApiOwnerType)),
        new ExcludeMember(typeof(ApiOwnerIdentityPart), nameof(ApiOwnerIdentityPart.ApiOwnerIdentity)),

        // ApiNestedIdentityPart — cycle: ApiIdentity → ApiObjectType → ApiIdentities[].ApiIdentityParts[].ApiProperty → ...
        new ExcludeMember(typeof(ApiNestedIdentityPart), nameof(ApiNestedIdentityPart.ApiIdentity)),
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
        new ExcludeMember(typeof(ApiOwnerIdentityPart), nameof(ApiOwnerIdentityPart.ApiOwnerIdentity)),
        new ExcludeMember(typeof(ApiOwnerIdentityPart), nameof(ApiOwnerIdentityPart.ApiOwnerType)),
        new ExcludeMember(typeof(ApiScalarIdentityPart), nameof(ApiScalarIdentityPart.ClrScalarType)),

        // ApiProperty
        new ExcludeMember(typeof(ApiProperty), nameof(ApiProperty.ApiType)),

        // ApiRelationship
        new ExcludeMember(typeof(ApiRelationship), nameof(ApiRelationship.ApiCardinality)),
        new ExcludeMember(typeof(ApiRelationship), nameof(ApiRelationship.ApiProperty)),
    ];
}
