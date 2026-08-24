// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

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
                PrincipalEnd: PrincipalEnd(typeof(Category), "PK_Category"),
                DependentEnd: DependentEnd
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
                PrincipalEnd: PrincipalEnd(typeof(Customer)),
                DependentEnd: DependentEnd(typeof(CustomerProfile))
            )
        },

        new KeyBindingTest
        {
            Name = "Resolves one-to-one scalar foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToOneDef
            (
                ApiName: "REL_User_UserProfile_1to1ViaScalar",
                PrincipalEnd: PrincipalEnd(typeof(RelationshipUser)),
                DependentEnd: DependentEnd
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
                PrincipalEnd: PrincipalEnd(typeof(RelationshipUser)),
                DependentEnd: DependentEnd
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
                PrincipalEnd: PrincipalEnd(typeof(RelationshipUser)),
                DependentEnd: DependentEnd
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
                PrincipalEnd: PrincipalEnd(typeof(RelationshipUser)),
                DependentEnd: DependentEnd
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
                PrincipalEnd: PrincipalEnd(typeof(RelationshipPost)),
                DependentEnd: DependentEnd
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
                PrincipalEnd: PrincipalEnd(typeof(RelationshipPost)),
                DependentEnd: DependentEnd
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
                PrincipalEndA: PrincipalEnd(typeof(RelationshipPost)),
                PrincipalEndB: PrincipalEnd(typeof(RelationshipTag)),
                Association: Association
                (
                    typeof(RelationshipPostTag),
                    ForeignKey
                    (
                        KeyPath(typeof(RelationshipPostTag), nameof(RelationshipPostTag.PostId))
                    ),
                    ForeignKey
                    (
                        KeyPath(typeof(RelationshipPostTag), nameof(RelationshipPostTag.TagId))
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
                PrincipalEnd: PrincipalEnd(typeof(RelationshipCatalogItem)),
                DependentEnd: DependentEnd
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
                PrincipalEnd: PrincipalEnd(typeof(RelationshipCatalogItem)),
                DependentEnd: DependentEnd
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
            Name = "Resolves one-to-many owner-rooted foreign key binding",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ExpectedApiRelationshipDef = new ApiRelationshipOneToManyDef
            (
                ApiName: "REL_Order_OwnedLine_1toN_ViaOwnerKeyPath",
                PrincipalEnd: PrincipalEnd(typeof(RelationshipOrder)),
                DependentEnd: DependentEnd
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
                PrincipalEnd: PrincipalEnd(typeof(RelationshipOrgUnit)),
                DependentEnd: DependentEnd
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
        string? apiPrincipalKeyTypeName = null
    )
        => new(clrObjectType, apiPrincipalKeyTypeName);

    private static ApiRelationshipDependentEndDef DependentEnd
    (
        Type clrObjectType,
        ApiKeyTypeDef? apiForeignKeyType = null
    )
        => new(clrObjectType, apiForeignKeyType);

    private static ApiRelationshipAssociationDef Association
    (
        Type clrObjectType,
        ApiKeyTypeDef? apiForeignKeyTypeA = null,
        ApiKeyTypeDef? apiForeignKeyTypeB = null
    )
        => new(clrObjectType, apiForeignKeyTypeA, apiForeignKeyTypeB);

    private static ApiKeyTypeDef ForeignKey(params ApiKeyPathDef[] apiKeyPaths)
        => new(null!, [.. apiKeyPaths]);

    private static ApiKeyPathDef KeyPath(Type clrRootType, params string[] clrPropertyNames)
        => new
        (
            clrRootType,
            [
                .. clrPropertyNames.Select
                (
                    static clrPropertyName => new ApiKeyPathSegmentDef(clrPropertyName)
                )
            ]
        );
    #endregion
}
