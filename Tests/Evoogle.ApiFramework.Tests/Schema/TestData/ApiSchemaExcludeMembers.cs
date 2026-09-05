// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.TestData;

public static class ApiSchemaExcludeMembers
{
    private static readonly ExcludeMember[] _schemaElementRuntimeMembers =
    [
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.ApiSchemaContext)),
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.Root)),
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.Parent)),
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.FirstChild)),
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.LastChild)),
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.NextSibling)),
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.PreviousSibling)),
    ];

    public static readonly List<ExcludeMember> SchemaInitialized =
    [
        .. _schemaElementRuntimeMembers,

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

        // ApiRelationshipElement — object type resolved during compilation
        new ExcludeMember(typeof(ApiRelationshipElement), nameof(ApiRelationshipElement.ApiObjectType)),

        // ApiRelationshipEnd — cycle: end → relationship → end → ...
        new ExcludeMember(typeof(ApiRelationshipEnd), nameof(ApiRelationshipEnd.ApiRelationship)),

        // ApiRelationshipAssociation — cycle: assoc → relationship → assoc → ...
        new ExcludeMember(typeof(ApiRelationshipAssociation), nameof(ApiRelationshipAssociation.ApiRelationshipManyToMany)),

        // ApiRelationshipOneTo — ApiKeyBinding throws when IsNavigational (HasKeyBinding=false)
        new ExcludeMember(typeof(ApiRelationshipOneTo), nameof(ApiRelationshipOneTo.ApiKeyBinding)),

        // ApiRelationshipManyToMany — ApiKeyBindingA/B throw when IsNavigational (HasKeyBindings=false)
        new ExcludeMember(typeof(ApiRelationshipManyToMany), nameof(ApiRelationshipManyToMany.ApiKeyBindingA)),
        new ExcludeMember(typeof(ApiRelationshipManyToMany), nameof(ApiRelationshipManyToMany.ApiKeyBindingB)),

        // ApiRelationshipDependentEnd — ApiForeignKeyType throws when HasForeignKey=false
        new ExcludeMember(typeof(ApiRelationshipDependentEnd), nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)),

    ];

    public static readonly List<ExcludeMember> Standard =
    [
        .. _schemaElementRuntimeMembers,

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
        // ApiSchemaElement
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.ApiPath)),

        // ApiTypeExpression
        new ExcludeMember(typeof(ApiTypeExpression), nameof(ApiTypeExpression.ApiType)),
    ];

    /// <summary>
    ///     Exclusions for comparing pre-compilation <see cref="ApiRelationship"/> objects built directly via builders
    ///     without running the full schema compilation pass.
    /// </summary>
    public static readonly List<ExcludeMember> Relationship =
    [
        .. _schemaElementRuntimeMembers,

        // ApiRelationshipElement — object type resolved during compilation
        new ExcludeMember(typeof(ApiRelationshipElement), nameof(ApiRelationshipElement.ApiObjectType)),

        // ApiRelationshipDependentEnd — ApiForeignKeyType throws when HasForeignKey=false
        new ExcludeMember(typeof(ApiRelationshipDependentEnd), nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)),

        new ExcludeMember(typeof(ApiRelationshipElement), nameof(ApiRelationshipElement.ApiObjectType)),

        // ApiRelationshipEnd — cycle: end → relationship → end → ...
        new ExcludeMember(typeof(ApiRelationshipEnd), nameof(ApiRelationshipEnd.ApiRelationship)),

        // ApiRelationshipAssociation — cycle: assoc → relationship → assoc → ...
        new ExcludeMember(typeof(ApiRelationshipAssociation), nameof(ApiRelationshipAssociation.ApiRelationshipManyToMany)),

        // ApiRelationshipOneTo — ApiKeyBinding throws when IsNavigational (HasKeyBinding=false)
        new ExcludeMember(typeof(ApiRelationshipOneTo), nameof(ApiRelationshipOneTo.ApiKeyBinding)),

        // ApiRelationshipManyToMany — ApiKeyBindingA/B throw when IsNavigational (HasKeyBindings=false)
        new ExcludeMember(typeof(ApiRelationshipManyToMany), nameof(ApiRelationshipManyToMany.ApiKeyBindingA)),
        new ExcludeMember(typeof(ApiRelationshipManyToMany), nameof(ApiRelationshipManyToMany.ApiKeyBindingB)),

        // Key path nodes — property and object type references resolved during compilation
        new ExcludeMember(typeof(ApiKeyPath), nameof(ApiKeyPath.ApiRootObjectType)),
        new ExcludeMember(typeof(ApiKeyPathSegment), nameof(ApiKeyPathSegment.ApiProperty)),

        // ApiSchemaElement
        new ExcludeMember(typeof(ApiSchemaElement), nameof(ApiSchemaElement.ApiPath)),
    ];
}
