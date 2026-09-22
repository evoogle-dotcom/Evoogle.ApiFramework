// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema.Relationships;

public partial class ApiRelationshipTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] KeyBindingTheoryData =>
    [
        new KeyBindingTest
        {
            Name = "Resolves explicit principal key binding",
            ApiSchemaKind = ApiSchemaKind.Commerce,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_Category_PhysicalProduct_1toN",
                ApiPrincipalEnd: PrincipalEnd(typeof(Category), "PK_Category"),
                ApiDependentEnd: DependentEnd
                (
                    typeof(PhysicalProduct),
                    ForeignKey(KeyPath(typeof(PhysicalProduct), nameof(PhysicalProduct.CategoryId)))
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Leaves navigational one-to-one relationship without key binding",
            ApiSchemaKind = ApiSchemaKind.Commerce,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToOneDef
            (
                ApiName: "REL_Customer_Profile_1to1",
                ApiPrincipalEnd: PrincipalEnd(typeof(Customer)),
                ApiDependentEnd: DependentEnd(typeof(CustomerProfile))
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves one-to-one scalar foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToOneDef
            (
                ApiName: "REL_User_UserProfile_1to1ViaScalar",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipUser)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipUserProfile),
                    ForeignKey
                    (
                        KeyPath
                        (
                            typeof(RelationshipUserProfile),
                            nameof(RelationshipUserProfile.UserId)
                        )
                    )
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves one-to-one nested foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToOneDef
            (
                ApiName: "REL_User_UserProfile_1to1ViaNested",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipUser)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipUserProfile),
                    ForeignKey
                    (
                        KeyPath
                        (
                            typeof(RelationshipUserProfile),
                            nameof(RelationshipUserProfile.UserRef),
                            nameof(RelationshipUserRef.UserId)
                        )
                    )
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves one-to-many scalar foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_User_Post_1toN_ViaScalar",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipUser)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipPost),
                    ForeignKey
                    (
                        KeyPath(typeof(RelationshipPost), nameof(RelationshipPost.AuthorUserId))
                    )
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves one-to-many nested foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_User_Post_1toN_ViaNested",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipUser)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipPost),
                    ForeignKey
                    (
                        KeyPath
                        (
                            typeof(RelationshipPost),
                            nameof(RelationshipPost.AuthorUserRef),
                            nameof(RelationshipUserRef.UserId)
                        )
                    )
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves post-to-comment scalar foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_Post_Comment_1toN_ViaScalar",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipPost)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipComment),
                    ForeignKey
                    (
                        KeyPath(typeof(RelationshipComment), nameof(RelationshipComment.PostId))
                    )
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves post-to-comment nested foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_Post_Comment_1toN_ViaNested",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipPost)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipComment),
                    ForeignKey
                    (
                        KeyPath
                        (
                            typeof(RelationshipComment),
                            nameof(RelationshipComment.PostRef),
                            nameof(RelationshipPostRef.PostId)
                        )
                    )
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves many-to-many scalar foreign key bindings",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipManyToManyDef
            (
                ApiName: "REL_Post_Tag_NtoN_ViaPostTag",
                ApiPrincipalEndA: PrincipalEnd(typeof(RelationshipPost)),
                ApiPrincipalEndB: PrincipalEnd(typeof(RelationshipTag)),
                ApiAssociation: Association
                (
                    typeof(RelationshipPostTag),
                    ForeignKey
                    (
                        InferredKeyPath(nameof(RelationshipPostTag.PostId))
                    ),
                    ForeignKey
                    (
                        InferredKeyPath(nameof(RelationshipPostTag.TagId))
                    )
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves one-to-many scalar composite foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_CatalogItem_OrderLine_1toN_ViaScalarComposite",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipCatalogItem)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipOrderLine),
                    ForeignKey
                    (
                        KeyPath
                        (
                            typeof(RelationshipOrderLine),
                            nameof(RelationshipOrderLine.ProductSku)
                        ),
                        KeyPath
                        (
                            typeof(RelationshipOrderLine),
                            nameof(RelationshipOrderLine.ProductRevision)
                        )
                    )
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves one-to-many nested composite foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_CatalogItem_OrderLine_1toN_ViaNestedComposite",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipCatalogItem)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipOrderLine),
                    ForeignKey
                    (
                        KeyPath
                        (
                            typeof(RelationshipOrderLine),
                            nameof(RelationshipOrderLine.ProductKey),
                            nameof(RelationshipCatalogKey.Sku)
                        ),
                        KeyPath
                        (
                            typeof(RelationshipOrderLine),
                            nameof(RelationshipOrderLine.ProductKey),
                            nameof(RelationshipCatalogKey.Revision)
                        )
                    )
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves one-to-many explicitly rooted foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_Order_OwnedLine_1toN_ViaOwnerKeyPath",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipOrder)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipOwnedLine),
                    ForeignKey(KeyPath(typeof(RelationshipOrder), nameof(RelationshipOrder.Id)))
                )
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves self-referential one-to-many foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_OrgUnit_OrgUnit_1toN",
                ApiPrincipalEnd: PrincipalEnd(typeof(RelationshipOrgUnit)),
                ApiDependentEnd: DependentEnd
                (
                    typeof(RelationshipOrgUnit),
                    ForeignKey
                    (
                        KeyPath(typeof(RelationshipOrgUnit), nameof(RelationshipOrgUnit.ParentId))
                    )
                )
            )
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(KeyBindingTheoryData))]
    public void KeyBinding(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static ApiRelationshipPrincipalEndDef PrincipalEnd
    (
        Type clrObjectType,
        string? apiPrincipalKeyName = null
    )
        => new
        (
            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: clrObjectType),
            ApiPrincipalKeyName: apiPrincipalKeyName
        );

    private static ApiRelationshipDependentEndDef DependentEnd
    (
        Type clrObjectType,
        ApiKeyDefinitionDef? apiForeignKey = null
    )
        => new
        (
            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: clrObjectType),
            ApiForeignKey: apiForeignKey
        );

    private static ApiRelationshipAssociationDef Association
    (
        Type clrObjectType,
        ApiKeyDefinitionDef? apiForeignKeyA = null,
        ApiKeyDefinitionDef? apiForeignKeyB = null
    )
        => new
        (
            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: clrObjectType),
            ApiForeignKeyA: apiForeignKeyA,
            ApiForeignKeyB: apiForeignKeyB
        );

    private static ApiKeyDefinitionDef ForeignKey(params ApiKeyPathDef[] apiKeyPaths)
        => new(ApiName: null!, ApiKeyPaths: [.. apiKeyPaths]);

    private static ApiKeyPathDef KeyPath(Type clrRootType, params string[] clrMemberNames)
        => new
        (
            ApiRootObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: clrRootType),
            ApiSegments:
            [
                .. clrMemberNames.Select
                (
                    static clrMemberName => new ApiKeyPathSegmentDef(new ApiPropertyReferenceDef(ApiName: null, ClrName: clrMemberName))
                )
            ]
        );

    private static ApiKeyPathDef InferredKeyPath(params string[] clrMemberNames)
        => new
        (
            null,
            [
                .. clrMemberNames.Select
                (
                    static clrMemberName => new ApiKeyPathSegmentDef(new ApiPropertyReferenceDef(ApiName: null, ClrName: clrMemberName))
                )
            ]
        );
    #endregion
}
