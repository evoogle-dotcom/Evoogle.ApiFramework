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
                                ClrName: nameof(RelationshipUser.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.UserName),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUser.UserName),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.Profile),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipUserProfile), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(RelationshipUser.Profile),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipUser_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUser.Id))])
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
                                ClrName: nameof(RelationshipUserRef.UserId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipUserRef_UserId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUserRef.UserId))])
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
                                ClrName: nameof(RelationshipUserProfile.UserId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.UserRef),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipUserRef), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUserProfile.UserRef),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.DisplayName),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUserProfile.DisplayName),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.User),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipUser), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUserProfile.User),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipUserProfile_UserId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUserProfile.UserId))])
                                ]
                            ),
                            new ApiKeyDef
                            (
                                ApiName: "AK_RelationshipUserProfile_UserRef",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUserProfile.UserRef)), new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUserRef.UserId))])
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
                        PrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUser))
                        ),
                        DependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUserProfile)),
                            ApiForeignKey: new ApiKeyDef
                            (
                                ApiName: "FK_User_UserProfile_UserId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUserProfile)), ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUserProfile.UserId))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // User_Profile_NestedFK
                    new ApiRelationshipOneToOneDef
                    (
                        ApiName: "User_Profile_NestedFK",
                        PrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUser))
                        ),
                        DependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUserProfile)),
                            ApiForeignKey: new ApiKeyDef
                            (
                                ApiName: "FK_User_UserProfile_UserRef_UserId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUserProfile)), ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUserProfile.UserRef)), new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUserRef.UserId))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Relationship Schema for One-To-One Relationship And Scalar And Nested Foreign Key Paths"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Ulid"",
                        ""ClrType"": ""System.Ulid,Ulid""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipUser"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""UserName"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""UserName"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Profile"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""RelationshipUserProfile""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Profile"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipUser_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipUserProfile"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""UserId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""UserId"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""UserRef"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""RelationshipUserRef""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""UserRef"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""DisplayName"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""DisplayName"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""User"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""RelationshipUser""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""User"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipUserProfile_UserId"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""UserId""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_RelationshipUserProfile_UserRef"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""UserRef.UserId""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipUserRef"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""UserId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""UserId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipUserRef_UserId"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""UserId""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUserRef,Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToOne"",
                        ""ApiName"": ""User_Profile_NestedFK"",
                        ""ApiPrincipalEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests""
                            }
                        },
                        ""ApiDependentEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Tests""
                            },
                            ""ApiForeignKey"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""UserRef.UserId""
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    },
                    {
                        ""ApiKind"": ""OneToOne"",
                        ""ApiName"": ""User_Profile_ScalarFK"",
                        ""ApiPrincipalEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests""
                            }
                        },
                        ""ApiDependentEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Tests""
                            },
                            ""ApiForeignKey"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""UserId""
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    }
                ]
            }"
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
                                ClrName: nameof(RelationshipUser.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.UserName),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUser.UserName),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.Posts),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: new ApiCollectionTypeDef(ClrType: typeof(List<RelationshipPost>), ApiItemTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipPost), ClrType: null)), ApiItemTypeModifiers: ApiTypeModifiers.Required), ApiTypeReferenceDef: null),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUser.Posts),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipUser_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUser.Id))])
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
                                ClrName: nameof(RelationshipUserRef.UserId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipUserRef_UserId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUserRef.UserId))])
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
                                ClrName: nameof(RelationshipPost.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.AuthorUserId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.AuthorUserId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.AuthorUserRef),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipUserRef), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.AuthorUserRef),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Title),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Title),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Comments),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: new ApiCollectionTypeDef(ClrType: typeof(List<RelationshipComment>), ApiItemTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipComment), ClrType: null)), ApiItemTypeModifiers: ApiTypeModifiers.Required), ApiTypeReferenceDef: null),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Comments),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.User),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipUser), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.User),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipPost_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPost.Id))])
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
                                ClrName: nameof(RelationshipPostRef.PostId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipPostRef_PostId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPostRef.PostId))])
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
                                ClrName: nameof(RelationshipComment.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.PostId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipComment.PostId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.PostRef),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipPostRef), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipComment.PostRef),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.Body),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipComment.Body),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.Post),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipPost), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipComment.Post),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipComment_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipComment.Id))])
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
                        PrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUser))
                        ),
                        DependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost)),
                            ApiForeignKey: new ApiKeyDef
                            (
                                ApiName: "FK_User_Post_AuthorUserId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost)), ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPost.AuthorUserId))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // User_Posts_NestedFK
                    new ApiRelationshipOneToManyDef
                    (
                        ApiName: "User_Posts_NestedFK",
                        PrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipUser))
                        ),
                        DependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost)),
                            ApiForeignKey: new ApiKeyDef
                            (
                                ApiName: "FK_User_Post_AuthorUserRef_UserId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost)), ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPost.AuthorUserRef)), new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipUserRef.UserId))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // Post_Comments_ScalarFK
                    new ApiRelationshipOneToManyDef
                    (
                        ApiName: "Post_Comments_ScalarFK",
                        PrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost))
                        ),
                        DependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipComment)),
                            ApiForeignKey: new ApiKeyDef
                            (
                                ApiName: "FK_Post_Comment_PostId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipComment)), ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipComment.PostId))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // Post_Comments_NestedFK
                    new ApiRelationshipOneToManyDef
                    (
                        ApiName: "Post_Comments_NestedFK",
                        PrincipalEnd: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost))
                        ),
                        DependentEnd: new ApiRelationshipDependentEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipComment)),
                            ApiForeignKey: new ApiKeyDef
                            (
                                ApiName: "FK_Post_Comment_PostRef_PostId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipComment)), ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipComment.PostRef)), new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPostRef.PostId))])]
                            )
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Relationship Schema for One-To-Many Relationship And Scalar And Nested Foreign Key Paths"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String,System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Ulid"",
                        ""ClrType"": ""System.Ulid,Ulid""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipComment"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""PostId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""PostId"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""PostRef"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""RelationshipPostRef""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""PostRef"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Body"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Body"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Post"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""RelationshipPost""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Post"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipComment_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipPost"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""AuthorUserId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""AuthorUserId"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""AuthorUserRef"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""RelationshipUserRef""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""AuthorUserRef"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Title"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Title"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Comments"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Collection"",
                                    ""ApiItemType"": {
                                        ""ApiKind"": ""Object"",
                                        ""ApiName"": ""RelationshipComment""
                                    },
                                    ""ApiItemTypeModifiers"": ""Required"",
                                    ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Tests]],System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Comments"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""User"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""RelationshipUser""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""User"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipPost_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipPostRef"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""PostId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""PostId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipPostRef_PostId"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""PostId""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPostRef,Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipUser"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""UserName"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""UserName"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Posts"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Collection"",
                                    ""ApiItemType"": {
                                        ""ApiKind"": ""Object"",
                                        ""ApiName"": ""RelationshipPost""
                                    },
                                    ""ApiItemTypeModifiers"": ""Required"",
                                    ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests]],System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Posts"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipUser_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipUserRef"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""UserId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""UserId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipUserRef_UserId"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""UserId""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUserRef,Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""Post_Comments_NestedFK"",
                        ""ApiPrincipalEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests""
                            }
                        },
                        ""ApiDependentEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Tests""
                            },
                            ""ApiForeignKey"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""PostRef.PostId""
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    },
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""Post_Comments_ScalarFK"",
                        ""ApiPrincipalEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests""
                            }
                        },
                        ""ApiDependentEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Tests""
                            },
                            ""ApiForeignKey"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""PostId""
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    },
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""User_Posts_NestedFK"",
                        ""ApiPrincipalEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests""
                            }
                        },
                        ""ApiDependentEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests""
                            },
                            ""ApiForeignKey"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""AuthorUserRef.UserId""
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    },
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""User_Posts_ScalarFK"",
                        ""ApiPrincipalEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Tests""
                            }
                        },
                        ""ApiDependentEnd"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests""
                            },
                            ""ApiForeignKey"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""AuthorUserId""
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    }
                ]
            }"
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
                                ClrName: nameof(RelationshipPost.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Title),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Title),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Tags),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: new ApiCollectionTypeDef(ClrType: typeof(List<RelationshipTag>), ApiItemTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipTag), ClrType: null)), ApiItemTypeModifiers: ApiTypeModifiers.Required), ApiTypeReferenceDef: null),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Tags),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipPost_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPost.Id))])
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
                                ClrName: nameof(RelationshipTag.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipTag.Name),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipTag.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipTag.Posts),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: new ApiCollectionTypeDef(ClrType: typeof(List<RelationshipPost>), ApiItemTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(RelationshipPost), ClrType: null)), ApiItemTypeModifiers: ApiTypeModifiers.Required), ApiTypeReferenceDef: null),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipTag.Posts),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipTag_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipTag.Id))])
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
                                ClrName: nameof(RelationshipPostTag.PostId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPostTag.TagId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPostTag.TagId),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDef
                            (
                                ApiName: "PK_RelationshipPostTag_PostId_TagId",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPostTag.PostId))]),
                                    new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPostTag.TagId))])
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
                        PrincipalEndA: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPost))
                        ),
                        PrincipalEndB: new ApiRelationshipPrincipalEndDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipTag))
                        ),
                        Association: new ApiRelationshipAssociationDef
                        (
                            ApiObjectTypeReference: new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(RelationshipPostTag)),
                            ApiForeignKeyA: new ApiKeyDef
                            (
                                ApiName: "FK_Post_PostTag_PostId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPostTag.PostId))])]
                            ),
                            ApiForeignKeyB: new ApiKeyDef
                            (
                                ApiName: "FK_Tag_PostTag_TagId",
                                ApiKeyPaths: [new ApiKeyPathDef(ApiRootTypeReference: null, ApiKeyPathSegments: [new ApiKeyPathSegmentDef(ClrMemberName: nameof(RelationshipPostTag.TagId))])]
                            )
                        )
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Relationship Schema for Many-To-Many Relationship And Scalar Foreign Key Paths"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String,System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Ulid"",
                        ""ClrType"": ""System.Ulid,Ulid""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipPost"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Title"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Title"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Tags"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Collection"",
                                    ""ApiItemType"": {
                                        ""ApiKind"": ""Object"",
                                        ""ApiName"": ""RelationshipTag""
                                    },
                                    ""ApiItemTypeModifiers"": ""Required"",
                                    ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.RelationshipTag,Evoogle.ApiFramework.Tests]],System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Tags"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipPost_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipPostTag"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""PostId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""PostId"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""TagId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""TagId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipPostTag_PostId_TagId"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""PostId""
                                    },
                                    {
                                        ""ClrPath"": ""TagId""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPostTag,Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipTag"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Posts"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Collection"",
                                    ""ApiItemType"": {
                                        ""ApiKind"": ""Object"",
                                        ""ApiName"": ""RelationshipPost""
                                    },
                                    ""ApiItemTypeModifiers"": ""Required"",
                                    ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests]],System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Posts"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_RelationshipTag_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipTag,Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""Post_Tags"",
                        ""ApiPrincipalEndA"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Tests""
                            }
                        },
                        ""ApiPrincipalEndB"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipTag,Evoogle.ApiFramework.Tests""
                            }
                        },
                        ""ApiAssociation"": {
                            ""ApiObjectType"": {
                                ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPostTag,Evoogle.ApiFramework.Tests""
                            },
                            ""ApiForeignKeyA"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""PostId""
                                    }
                                ]
                            },
                            ""ApiForeignKeyB"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""TagId""
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    }
                ]
            }"
        }
    ];
    #endregion
}
