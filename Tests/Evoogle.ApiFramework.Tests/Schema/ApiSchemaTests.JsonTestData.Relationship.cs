// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.TestData;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Test Data
    private static ApiSchemaJsonTestCase[] RelationshipJsonTestCases =>
    [
        // ApiSchema With Relationship Schema for One-To-One Relationship
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Relationship Schema for One-To-One Relationship And Scalar And Nested Foreign Key Paths",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Relationship Schema for One-To-One Relationship And Scalar And Nested Foreign Key Paths",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Ulid),
                        ClrType: typeof(Ulid)
                    ),

                    // User
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipUser),
                        ClrType: typeof(RelationshipUser),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUser.Id)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.UserName),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUser.UserName)
                                )
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipUser_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUser.Id)))])
                                ]
                            )
                        ]
                    ),

                    // UserRef
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipUserRef),
                        ClrType: typeof(RelationshipUserRef),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserRef.UserId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUserRef.UserId)
                                )
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipUserRef_UserId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUserRef.UserId)))])
                                ]
                            )
                        ]
                    ),

                    // UserProfile
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipUserProfile),
                        ClrType: typeof(RelationshipUserProfile),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.UserId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUserProfile.UserId)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.UserRef),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipUserRef), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUserProfile.UserRef)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.DisplayName),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUserProfile.DisplayName)
                                )
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipUserProfile_UserId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUserProfile.UserId)))])
                                ]
                            ),
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "AK_RelationshipUserProfile_UserRef",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUserProfile.UserRef))), new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUserRef.UserId)))])
                                ]
                            )
                        ]
                    ),
                ],
                ApiRelationships:
                [
                    // User_Profile_ScalarFK
                    new ApiRelationshipOneToOneDef
                    (
                        ApiName: "User_Profile_ScalarFK",
                        ApiPrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUser)),
                            ApiTraversal: new ApiRelationshipTraversalDef
                            (
                                ApiName: nameof(RelationshipUser.Profile),
                                ClrNavigationMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUser.Profile)
                                )
                            )
                        ),
                        ApiDependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUserProfile)),
                            ApiTraversal: new ApiRelationshipTraversalDef
                            (
                                ApiName: nameof(RelationshipUserProfile.User),
                                ClrNavigationMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUserProfile.User)
                                )
                            ),
                            ApiForeignKey: new ApiKeyDefinitionDef
                            (
                                ApiName: "FK_User_UserProfile_UserId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUserProfile)), ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUserProfile.UserId)))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // User_Profile_NestedFK
                    new ApiRelationshipOneToOneDef
                    (
                        ApiName: "User_Profile_NestedFK",
                        ApiPrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUser))
                        ),
                        ApiDependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUserProfile)),
                            ApiForeignKey: new ApiKeyDefinitionDef
                            (
                                ApiName: "FK_User_UserProfile_UserRef_UserId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUserProfile)), ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUserProfile.UserRef))), new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUserRef.UserId)))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    )
                ]
            ),
            Json = """
            {
                "ApiName": "ApiSchema With Relationship Schema for One-To-One Relationship And Scalar And Nested Foreign Key Paths",
                "ApiVersion": "0.1.0",
                "ApiOptions": {
                    "ApiKeyNullHandling": "UseDefaultOnNull"
                },
                "ApiScalarTypes": [
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "String",
                        "ClrType": "System.String, System.Private.CoreLib"
                    },
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "Ulid",
                        "ClrType": "System.Ulid,Ulid"
                    }
                ],
                "ApiEnumTypes": [],
                "ApiObjectTypes": [
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipUser",
                        "ApiProperties": [
                            {
                                "ApiName": "Id",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Id"
                                }
                            },
                            {
                                "ApiName": "UserName",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "String"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "UserName"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipUser_Id",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "Id"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests"
                    },
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipUserProfile",
                        "ApiProperties": [
                            {
                                "ApiName": "UserId",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "UserId"
                                }
                            },
                            {
                                "ApiName": "UserRef",
                                "ApiType": {
                                    "ApiKind": "Object",
                                    "ApiName": "RelationshipUserRef"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "UserRef"
                                }
                            },
                            {
                                "ApiName": "DisplayName",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "String"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "DisplayName"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipUserProfile_UserId",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "UserId"
                                    }
                                ]
                            },
                            {
                                "ApiName": "AK_RelationshipUserProfile_UserRef",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "UserRef.UserId"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Tests"
                    },
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipUserRef",
                        "ApiProperties": [
                            {
                                "ApiName": "UserId",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "UserId"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipUserRef_UserId",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "UserId"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUserRef,Evoogle.ApiFramework.Tests"
                    }
                ],
                "ApiRelationships": [
                    {
                        "ApiKind": "OneToOne",
                        "ApiName": "User_Profile_NestedFK",
                        "ApiPrincipalEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests"
                            }
                        },
                        "ApiDependentEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Tests"
                            },
                            "ApiForeignKey": {
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "UserRef.UserId"
                                    }
                                ]
                            }
                        },
                        "ApiDeleteBehavior": "Delete"
                    },
                    {
                        "ApiKind": "OneToOne",
                        "ApiName": "User_Profile_ScalarFK",
                        "ApiPrincipalEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests"
                            },
                            "ApiTraversal": {
                                "ApiName": "Profile",
                                "ClrNavigationMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Profile"
                                }
                            }
                        },
                        "ApiDependentEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Tests"
                            },
                            "ApiForeignKey": {
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "UserId"
                                    }
                                ]
                            },
                            "ApiTraversal": {
                                "ApiName": "User",
                                "ClrNavigationMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "User"
                                }
                            }
                        },
                        "ApiDeleteBehavior": "Delete"
                    }
                ]
            }
            """
        },

        // ApiSchema With Relationship Schema for One-To-Many Relationship
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Relationship Schema for One-To-Many Relationship And Scalar And Nested Foreign Key Paths",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Relationship Schema for One-To-Many Relationship And Scalar And Nested Foreign Key Paths",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Ulid),
                        ClrType: typeof(Ulid)
                    ),

                    // User
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipUser),
                        ClrType: typeof(RelationshipUser),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUser.Id)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.UserName),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUser.UserName)
                                )
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipUser_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUser.Id)))])
                                ]
                            )
                        ]
                    ),

                    // UserRef
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipUserRef),
                        ClrType: typeof(RelationshipUserRef),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserRef.UserId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUserRef.UserId)
                                )
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipUserRef_UserId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUserRef.UserId)))])
                                ]
                            )
                        ]
                    ),

                    // Post
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipPost),
                        ClrType: typeof(RelationshipPost),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPost.Id)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.AuthorUserId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPost.AuthorUserId)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.AuthorUserRef),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipUserRef), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPost.AuthorUserRef)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Title),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPost.Title)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Comments),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: new ApiCollectionTypeDef(ClrType: typeof(List<RelationshipComment>), ApiItemTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipComment), ClrType: null)), ApiItemTypeModifiers: ApiTypeModifiers.Required), ApiTypeReferenceDef: null),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPost.Comments)
                                )
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipPost_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPost.Id)))])
                                ]
                            )
                        ]
                    ),

                    // RelationshipPostRef
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipPostRef),
                        ClrType: typeof(RelationshipPostRef),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPostRef.PostId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPostRef.PostId)
                                )
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipPostRef_PostId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPostRef.PostId)))])
                                ]
                            )
                        ]
                    ),

                    // Comment
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipComment),
                        ClrType: typeof(RelationshipComment),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipComment.Id)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.PostId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipComment.PostId)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.PostRef),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipPostRef), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipComment.PostRef)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.Body),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipComment.Body)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.Post),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipPost), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipComment.Post)
                                )
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipComment_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipComment.Id)))])
                                ]
                            )
                        ]
                    ),
                ],
                ApiRelationships:
                [
                    // User_Posts_ScalarFK
                    new ApiRelationshipOneToManyDef
                    (
                        ApiName: "User_Posts_ScalarFK",
                        ApiPrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUser)),
                            ApiTraversal: new ApiRelationshipTraversalDef
                            (
                                ApiName: nameof(RelationshipUser.Posts),
                                ClrNavigationMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipUser.Posts)
                                )
                            )
                        ),
                        ApiDependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost)),
                            ApiTraversal: new ApiRelationshipTraversalDef
                            (
                                ApiName: nameof(RelationshipPost.User),
                                ClrNavigationMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPost.User)
                                )
                            ),
                            ApiForeignKey: new ApiKeyDefinitionDef
                            (
                                ApiName: "FK_User_Post_AuthorUserId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost)), ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPost.AuthorUserId)))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // User_Posts_NestedFK
                    new ApiRelationshipOneToManyDef
                    (
                        ApiName: "User_Posts_NestedFK",
                        ApiPrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUser))
                        ),
                        ApiDependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost)),
                            ApiForeignKey: new ApiKeyDefinitionDef
                            (
                                ApiName: "FK_User_Post_AuthorUserRef_UserId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost)), ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPost.AuthorUserRef))), new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipUserRef.UserId)))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // Post_Comments_ScalarFK
                    new ApiRelationshipOneToManyDef
                    (
                        ApiName: "Post_Comments_ScalarFK",
                        ApiPrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost))
                        ),
                        ApiDependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipComment)),
                            ApiForeignKey: new ApiKeyDefinitionDef
                            (
                                ApiName: "FK_Post_Comment_PostId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipComment)), ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipComment.PostId)))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // Post_Comments_NestedFK
                    new ApiRelationshipOneToManyDef
                    (
                        ApiName: "Post_Comments_NestedFK",
                        ApiPrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost))
                        ),
                        ApiDependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipComment)),
                            ApiForeignKey: new ApiKeyDefinitionDef
                            (
                                ApiName: "FK_Post_Comment_PostRef_PostId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipComment)), ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipComment.PostRef))), new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPostRef.PostId)))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    )
                ]
            ),
            Json = """
            {
                "ApiName": "ApiSchema With Relationship Schema for One-To-Many Relationship And Scalar And Nested Foreign Key Paths",
                "ApiVersion": "0.1.0",
                "ApiOptions": {
                    "ApiKeyNullHandling": "UseDefaultOnNull"
                },
                "ApiScalarTypes": [
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "String",
                        "ClrType": "System.String,System.Private.CoreLib"
                    },
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "Ulid",
                        "ClrType": "System.Ulid,Ulid"
                    }
                ],
                "ApiEnumTypes": [],
                "ApiObjectTypes": [
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipComment",
                        "ApiProperties": [
                            {
                                "ApiName": "Id",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Id"
                                }
                            },
                            {
                                "ApiName": "PostId",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "PostId"
                                }
                            },
                            {
                                "ApiName": "PostRef",
                                "ApiType": {
                                    "ApiKind": "Object",
                                    "ApiName": "RelationshipPostRef"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "PostRef"
                                }
                            },
                            {
                                "ApiName": "Body",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "String"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Body"
                                }
                            },
                            {
                                "ApiName": "Post",
                                "ApiType": {
                                    "ApiKind": "Object",
                                    "ApiName": "RelationshipPost"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Post"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipComment_Id",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "Id"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Tests"
                    },
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipPost",
                        "ApiProperties": [
                            {
                                "ApiName": "Id",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Id"
                                }
                            },
                            {
                                "ApiName": "AuthorUserId",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "AuthorUserId"
                                }
                            },
                            {
                                "ApiName": "AuthorUserRef",
                                "ApiType": {
                                    "ApiKind": "Object",
                                    "ApiName": "RelationshipUserRef"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "AuthorUserRef"
                                }
                            },
                            {
                                "ApiName": "Title",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "String"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Title"
                                }
                            },
                            {
                                "ApiName": "Comments",
                                "ApiType": {
                                    "ApiKind": "Collection",
                                    "ApiItemType": {
                                        "ApiKind": "Object",
                                        "ApiName": "RelationshipComment"
                                    },
                                    "ApiItemTypeModifiers": "Required",
                                    "ClrType": "System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Tests]],System.Private.CoreLib"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Comments"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipPost_Id",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "Id"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests"
                    },
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipPostRef",
                        "ApiProperties": [
                            {
                                "ApiName": "PostId",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "PostId"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipPostRef_PostId",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "PostId"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPostRef,Evoogle.ApiFramework.Tests"
                    },
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipUser",
                        "ApiProperties": [
                            {
                                "ApiName": "Id",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Id"
                                }
                            },
                            {
                                "ApiName": "UserName",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "String"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "UserName"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipUser_Id",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "Id"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests"
                    },
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipUserRef",
                        "ApiProperties": [
                            {
                                "ApiName": "UserId",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "UserId"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipUserRef_UserId",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "UserId"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUserRef,Evoogle.ApiFramework.Tests"
                    }
                ],
                "ApiRelationships": [
                    {
                        "ApiKind": "OneToMany",
                        "ApiName": "Post_Comments_NestedFK",
                        "ApiPrincipalEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests"
                            }
                        },
                        "ApiDependentEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Tests"
                            },
                            "ApiForeignKey": {
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "PostRef.PostId"
                                    }
                                ]
                            }
                        },
                        "ApiDeleteBehavior": "Delete"
                    },
                    {
                        "ApiKind": "OneToMany",
                        "ApiName": "Post_Comments_ScalarFK",
                        "ApiPrincipalEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests"
                            }
                        },
                        "ApiDependentEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Tests"
                            },
                            "ApiForeignKey": {
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "PostId"
                                    }
                                ]
                            }
                        },
                        "ApiDeleteBehavior": "Delete"
                    },
                    {
                        "ApiKind": "OneToMany",
                        "ApiName": "User_Posts_NestedFK",
                        "ApiPrincipalEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests"
                            }
                        },
                        "ApiDependentEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests"
                            },
                            "ApiForeignKey": {
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "AuthorUserRef.UserId"
                                    }
                                ]
                            }
                        },
                        "ApiDeleteBehavior": "Delete"
                    },
                    {
                        "ApiKind": "OneToMany",
                        "ApiName": "User_Posts_ScalarFK",
                        "ApiPrincipalEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests"
                            },
                            "ApiTraversal": {
                                "ApiName": "Posts",
                                "ClrNavigationMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Posts"
                                }
                            }
                        },
                        "ApiDependentEnd": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests"
                            },
                            "ApiForeignKey": {
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "AuthorUserId"
                                    }
                                ]
                            },
                            "ApiTraversal": {
                                "ApiName": "User",
                                "ClrNavigationMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "User"
                                }
                            }
                        },
                        "ApiDeleteBehavior": "Delete"
                    }
                ]
            }
            """
        },

        // ApiSchema With Relationship Schema for Many-To-Many Relationship
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Relationship Schema for Many-To-Many Relationship And Scalar Foreign Key Paths",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Relationship Schema for Many-To-Many Relationship And Scalar Foreign Key Paths",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Ulid),
                        ClrType: typeof(Ulid)
                    ),

                    // Post
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipPost),
                        ClrType: typeof(RelationshipPost),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPost.Id)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Title),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPost.Title)
                                )
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipPost_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPost.Id)))])
                                ]
                            )
                        ]
                    ),

                    // Tag
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipTag),
                        ClrType: typeof(RelationshipTag),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipTag.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipTag.Id)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipTag.Name),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipTag.Name)
                                )
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipTag_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipTag.Id)))])
                                ]
                            )
                        ]
                    ),

                    // PostTag
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipPostTag),
                        ClrType: typeof(RelationshipPostTag),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPostTag.PostId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPostTag.PostId)
                                )
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPostTag.TagId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrValueMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPostTag.TagId)
                                )
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_RelationshipPostTag_PostId_TagId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPostTag.PostId)))]),
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPostTag.TagId)))])
                                ]
                            )
                        ]
                    ),
                ],
                ApiRelationships:
                [
                    // Post_Tags
                    new ApiRelationshipManyToManyDef
                    (
                        ApiName: "Post_Tags",
                        ApiPrincipalEndA: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost)),
                            ApiTraversal: new ApiRelationshipTraversalDef
                            (
                                ApiName: nameof(RelationshipPost.Tags),
                                ClrNavigationMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipPost.Tags)
                                )
                            )
                        ),
                        ApiPrincipalEndB: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipTag)),
                            ApiTraversal: new ApiRelationshipTraversalDef
                            (
                                ApiName: nameof(RelationshipTag.Posts),
                                ClrNavigationMember: new ClrMemberReferenceDef
                                (
                                    ClrMemberKind.Property,
                                    nameof(RelationshipTag.Posts)
                                )
                            )
                        ),
                        ApiAssociation: new ApiRelationshipAssociationDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPostTag)),
                            ApiForeignKeyA: new ApiKeyDefinitionDef
                            (
                                ApiName: "FK_Post_PostTag_PostId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPostTag.PostId)))])]
                            ),
                            ApiForeignKeyB: new ApiKeyDefinitionDef
                            (
                                ApiName: "FK_Tag_PostTag_TagId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(RelationshipPostTag.TagId)))])]
                            )
                        )
                    )
                ]
            ),
            Json = """
            {
                "ApiName": "ApiSchema With Relationship Schema for Many-To-Many Relationship And Scalar Foreign Key Paths",
                "ApiVersion": "0.1.0",
                "ApiOptions": {
                    "ApiKeyNullHandling": "UseDefaultOnNull"
                },
                "ApiScalarTypes": [
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "String",
                        "ClrType": "System.String,System.Private.CoreLib"
                    },
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "Ulid",
                        "ClrType": "System.Ulid,Ulid"
                    }
                ],
                "ApiEnumTypes": [],
                "ApiObjectTypes": [
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipPost",
                        "ApiProperties": [
                            {
                                "ApiName": "Id",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Id"
                                }
                            },
                            {
                                "ApiName": "Title",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "String"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Title"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipPost_Id",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "Id"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests"
                    },
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipPostTag",
                        "ApiProperties": [
                            {
                                "ApiName": "PostId",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "PostId"
                                }
                            },
                            {
                                "ApiName": "TagId",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "TagId"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipPostTag_PostId_TagId",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "PostId"
                                    },
                                    {
                                        "ClrPath": "TagId"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPostTag,Evoogle.ApiFramework.Tests"
                    },
                    {
                        "ApiKind": "Object",
                        "ApiName": "RelationshipTag",
                        "ApiProperties": [
                            {
                                "ApiName": "Id",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Ulid"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Id"
                                }
                            },
                            {
                                "ApiName": "Name",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "String"
                                },
                                "ApiTypeModifiers": "Required",
                                "ClrValueMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Name"
                                }
                            }
                        ],
                        "ApiKeys": [
                            {
                                "ApiName": "PK_RelationshipTag_Id",
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "Id"
                                    }
                                ]
                            }
                        ],
                        "ClrType": "Evoogle.ApiFramework.TestData.RelationshipTag,Evoogle.ApiFramework.Tests"
                    }
                ],
                "ApiRelationships": [
                    {
                        "ApiKind": "ManyToMany",
                        "ApiName": "Post_Tags",
                        "ApiPrincipalEndA": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests"
                            },
                            "ApiTraversal": {
                                "ApiName": "Tags",
                                "ClrNavigationMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Tags"
                                }
                            }
                        },
                        "ApiPrincipalEndB": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipTag,Evoogle.ApiFramework.Tests"
                            },
                            "ApiTraversal": {
                                "ApiName": "Posts",
                                "ClrNavigationMember": {
                                    "ClrKind": "Property",
                                    "ClrName": "Posts"
                                }
                            }
                        },
                        "ApiAssociation": {
                            "ApiObjectType": {
                                "ClrType": "Evoogle.ApiFramework.TestData.RelationshipPostTag,Evoogle.ApiFramework.Tests"
                            },
                            "ApiForeignKeyA": {
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "PostId"
                                    }
                                ]
                            },
                            "ApiForeignKeyB": {
                                "ApiKeyPaths": [
                                    {
                                        "ClrPath": "TagId"
                                    }
                                ]
                            }
                        },
                        "ApiDeleteBehavior": "Delete"
                    }
                ]
            }
            """
        }
    ];
    #endregion
}
