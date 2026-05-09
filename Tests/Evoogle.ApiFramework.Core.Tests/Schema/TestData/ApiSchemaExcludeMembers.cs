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

        // ApiIdentityPropertyPart — cycle: ApiProperty → ApiType → ApiObjectType → ApiIdentities[].ApiIdentityParts[].ApiProperty → ...
        new ExcludeMember(typeof(ApiIdentityPropertyPart), nameof(ApiIdentityPropertyPart.ApiProperty)),

        // ApiIdentityOwnerPart — cycle: ApiOwnerType → ApiObjectType → ... / ApiOwnerIdentity → ApiIdentity → ...
        new ExcludeMember(typeof(ApiIdentityOwnerPart), nameof(ApiIdentityOwnerPart.ApiObjectType)),
        new ExcludeMember(typeof(ApiIdentityOwnerPart), nameof(ApiIdentityOwnerPart.ApiIdentity)),

        // ApiIdentityNestedPart — cycle: ApiIdentity → ApiObjectType → ApiIdentities[].ApiIdentityParts[].ApiProperty → ...
        new ExcludeMember(typeof(ApiIdentityNestedPart), nameof(ApiIdentityNestedPart.ApiIdentity)),

        // ApiObjectType — cycles: ApiProperties[].ApiType → ... / ApiIdentities[].ApiIdentityParts[].ApiProperty → ...
        new ExcludeMember(typeof(ApiObjectType), nameof(ApiObjectType.ApiRelationshipEnds)),
        new ExcludeMember(typeof(ApiObjectType), nameof(ApiObjectType.ApiRelationshipPrincipalEnds)),
        new ExcludeMember(typeof(ApiObjectType), nameof(ApiObjectType.ApiRelationshipDependentEnds)),

        // ApiRelationshipEnd — object type resolved during initialization
        new ExcludeMember(typeof(ApiRelationshipEnd), nameof(ApiRelationshipEnd.ApiObjectType)),

        // ApiRelationshipPrincipalEnd — identity resolved during initialization
        new ExcludeMember(typeof(ApiRelationshipPrincipalEnd), nameof(ApiRelationshipPrincipalEnd.ApiIdentity)),
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
        new ExcludeMember(typeof(ApiIdentityPropertyPart), nameof(ApiIdentityPropertyPart.ApiProperty)),
        new ExcludeMember(typeof(ApiIdentityOwnerPart), nameof(ApiIdentityOwnerPart.ApiIdentity)),
        new ExcludeMember(typeof(ApiIdentityOwnerPart), nameof(ApiIdentityOwnerPart.ApiObjectType)),
        new ExcludeMember(typeof(ApiIdentityScalarPart), nameof(ApiIdentityScalarPart.ClrScalarType)),
        new ExcludeMember(typeof(ApiIdentityNestedPart), nameof(ApiIdentityNestedPart.ApiIdentity)),

        // ApiProperty
        new ExcludeMember(typeof(ApiProperty), nameof(ApiProperty.ApiType)),
    ];

    /// <summary>
    ///     Exclusions for comparing pre-initialization <see cref="ApiRelationship"/> objects built directly via builders
    ///     without running the full schema initialization pass.
    /// </summary>
    public static readonly List<ExcludeMember> Relationship =
    [
        // ApiSchemaElement
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.ApiPath)),

        // ApiRelationshipElement — object type resolved during initialization
        new ExcludeMember(typeof(ApiRelationshipElement), nameof(ApiRelationshipElement.ApiObjectType)),

        // ApiRelationshipPrincipalEnd — identity resolved during initialization
        new ExcludeMember(typeof(ApiRelationshipPrincipalEnd), nameof(ApiRelationshipPrincipalEnd.ApiIdentity)),

        // Key path nodes — property and object type references resolved during initialization
        new ExcludeMember(typeof(ApiRelationshipScalarKeyPath), nameof(ApiRelationshipScalarKeyPath.ApiProperty)),
        new ExcludeMember(typeof(ApiRelationshipNestedKeyPath), nameof(ApiRelationshipNestedKeyPath.ApiProperty)),
        new ExcludeMember(typeof(ApiRelationshipNestedKeyPath), nameof(ApiRelationshipNestedKeyPath.ApiObjectType)),
        new ExcludeMember(typeof(ApiRelationshipOwnerKeyPath), nameof(ApiRelationshipOwnerKeyPath.ApiObjectType)),
    ];
}
