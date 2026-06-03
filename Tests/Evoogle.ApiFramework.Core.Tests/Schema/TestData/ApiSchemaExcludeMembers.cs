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
        // ApiCollectionType — cycle: ApiItemType → ApiObjectType → ApiProperties[].ApiType → ...
        new ExcludeMember(typeof(ApiCollectionType), nameof(ApiCollectionType.ApiItemType)),

        // ApiKeyPath — cycle: ApiRootObjectType → ApiObjectType → ApiKeyTypes[].ApiKeyPaths[].ApiRootObjectType → ...
        new ExcludeMember(typeof(ApiKeyPath), nameof(ApiKeyPath.ApiRootObjectType)),

        // ApiKeyPathSegment — cycle: ApiProperty → ApiType → ApiObjectType → ApiKeyTypes[].ApiKeyPaths[].ApiRootObjectType → ...
        new ExcludeMember(typeof(ApiKeyPathSegment), nameof(ApiKeyPathSegment.ApiProperty)),

        // ApiProperty — cycle: ApiType → ApiObjectType → ApiProperties[].ApiType → ...
        new ExcludeMember(typeof(ApiProperty), nameof(ApiProperty.ApiType)),

        // ApiObjectType — cycles: ApiProperties[].ApiType → ... / ApiKeyTypes[].ApiKeyPaths[].ApiProperty → ...
        new ExcludeMember(typeof(ApiObjectType), nameof(ApiObjectType.ApiRelationshipEnds)),
        new ExcludeMember(typeof(ApiObjectType), nameof(ApiObjectType.ApiRelationshipPrincipalEnds)),
        new ExcludeMember(typeof(ApiObjectType), nameof(ApiObjectType.ApiRelationshipDependentEnds)),

        // ApiRelationshipElement — object type resolved during initialization
        new ExcludeMember(typeof(ApiRelationshipElement), nameof(ApiRelationshipElement.ApiObjectType)),

        // ApiRelationshipEnd — cycle: end → relationship → end → ...
        new ExcludeMember(typeof(ApiRelationshipEnd), nameof(ApiRelationshipEnd.ApiRelationship)),

        // ApiRelationshipAssociation — cycle: assoc → relationship → assoc → ...
        new ExcludeMember(typeof(ApiRelationshipAssociation), nameof(ApiRelationshipAssociation.ApiRelationshipManyToMany)),

        // ApiRelationshipPrincipalEnd — key type resolved during initialization
        new ExcludeMember(typeof(ApiRelationshipPrincipalEnd), nameof(ApiRelationshipPrincipalEnd.ApiKeyType)),

        // ApiRelationshipDependentEnd — ApiForeignKeyType throws when IsNavigational (HasKeyBinding=false)
        new ExcludeMember(typeof(ApiRelationshipDependentEnd), nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)),

        // ApiSchema - cycle: ApiSchema → ApiSchemaContext → ApiSchema → ...
        new ExcludeMember(typeof(ApiSchema), nameof(ApiSchema.ApiSchemaContext)),
    ];

    public static readonly List<ExcludeMember> Standard =
    [
        // ApiCollectionType
        new ExcludeMember(typeof(ApiCollectionType), nameof(ApiCollectionType.ApiItemType)),

        // ApiKeyPath
        new ExcludeMember(typeof(ApiKeyPath), nameof(ApiKeyPath.ApiRootObjectType)),

        // ApiKeyPathSegment
        new ExcludeMember(typeof(ApiKeyPathSegment), nameof(ApiKeyPathSegment.ApiProperty)),

        // ApiProperty
        new ExcludeMember(typeof(ApiProperty), nameof(ApiProperty.ApiType)),

        // ApiSchema
        new ExcludeMember(typeof(ApiSchema), nameof(ApiSchema.ApiPath)),
        new ExcludeMember(typeof(ApiSchema), nameof(ApiSchema.ApiSchemaContext)),

        // ApiSchemaElement
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.ApiPath)),

        // ApiTypeExpression
        new ExcludeMember(typeof(ApiTypeExpression), nameof(ApiTypeExpression.ApiType)),
    ];

    /// <summary>
    ///     Exclusions for comparing pre-initialization <see cref="ApiRelationship"/> objects built directly via builders
    ///     without running the full schema initialization pass.
    /// </summary>
    public static readonly List<ExcludeMember> Relationship =
    [
        // ApiRelationshipElement — object type resolved during initialization
        new ExcludeMember(typeof(ApiRelationshipElement), nameof(ApiRelationshipElement.ApiObjectType)),

        // ApiRelationshipDependentEnd — ApiForeignKeyType throws when IsNavigational (HasKeyBinding=false)
        new ExcludeMember(typeof(ApiRelationshipDependentEnd), nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)),

        new ExcludeMember(typeof(ApiRelationshipElement), nameof(ApiRelationshipElement.ApiObjectType)),

        // ApiRelationshipEnd — cycle: end → relationship → end → ...
        new ExcludeMember(typeof(ApiRelationshipEnd), nameof(ApiRelationshipEnd.ApiRelationship)),

        // ApiRelationshipAssociation — cycle: assoc → relationship → assoc → ...
        new ExcludeMember(typeof(ApiRelationshipAssociation), nameof(ApiRelationshipAssociation.ApiRelationshipManyToMany)),

        // ApiRelationshipPrincipalEnd — key type resolved during initialization
        new ExcludeMember(typeof(ApiRelationshipPrincipalEnd), nameof(ApiRelationshipPrincipalEnd.ApiKeyType)),

        // Key path nodes — property and object type references resolved during initialization
        new ExcludeMember(typeof(ApiKeyPath), nameof(ApiKeyPath.ApiRootObjectType)),
        new ExcludeMember(typeof(ApiKeyPathSegment), nameof(ApiKeyPathSegment.ApiProperty)),

        // ApiSchemaElement
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.ApiPath)),
    ];
}
