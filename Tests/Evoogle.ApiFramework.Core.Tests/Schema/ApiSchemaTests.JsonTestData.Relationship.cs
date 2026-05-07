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
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipUser_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipUser.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUser.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.UserName),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUser.UserName),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.Profile),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipUserProfile)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(RelationshipUser.Profile),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),

                    // UserRef
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipUserRef),
                        ClrType: typeof(RelationshipUserRef),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipUserRef_UserId",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipUserRef.UserId)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserRef.UserId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUserRef.UserId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ]
                    ),

                    // UserProfile
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipUserProfile),
                        ClrType: typeof(RelationshipUserProfile),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipUserProfile_UserId",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipUserProfile.UserId)
                                    )
                                ]
                            ),
                            new ApiIdentityDef
                            (
                                ApiName: "AK_RelationshipUserProfile_UserRef",
                                Parts:
                                [
                                    new ApiNestedPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipUserProfile.UserRef)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.UserId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUserProfile.UserId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.UserRef),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipUserRef)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUserProfile.UserRef),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.DisplayName),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUserProfile.DisplayName),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserProfile.User),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipUser)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUserProfile.User),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                ],
                ApiRelationships:
                [
                    // User_Profile_ScalarFK
                    new ApiOneToOneRelationshipDef
                    (
                        ApiName: "User_Profile_ScalarFK",
                        PrincipalEnd: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipUser)
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipUserProfile),
                            ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipUserProfile.UserId))]
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // User_Profile_NestedFK
                    new ApiOneToOneRelationshipDef
                    (
                        ApiName: "User_Profile_NestedFK",
                        PrincipalEnd: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipUser)
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipUserProfile),
                            ApiKeyPaths:
                            [
                                new ApiRelationshipNestedKeyPathDef
                                (
                                    ClrPropertyName: nameof(RelationshipUserProfile.UserRef),
                                    ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipUserRef.UserId))]
                                )
                            ]
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipUser_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            }
                        ],
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipUserProfile"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipUserProfile_UserId"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""UserId""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_RelationshipUserProfile_UserRef"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Nested"",
                                        ""ClrPropertyName"": ""UserRef""
                                    }
                                ]
                            }
                        ],
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipUserRef"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipUserRef_UserId"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""UserId""
                                    }
                                ]
                            }
                        ],
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUserRef,Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToOne"",
                        ""ApiName"": ""User_Profile_NestedFK"",
                        ""ApiDeleteBehavior"": ""Delete"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiKeyPaths"": [
                                {
                                    ""ApiKind"": ""Nested"",
                                    ""ClrPropertyName"": ""UserRef"",
                                    ""ApiKeyPaths"": [
                                        {
                                            ""ApiKind"": ""Scalar"",
                                            ""ClrPropertyName"": ""UserId""
                                        }
                                    ]
                                }
                            ]
                        }
                    },
                    {
                        ""ApiKind"": ""OneToOne"",
                        ""ApiName"": ""User_Profile_ScalarFK"",
                        ""ApiDeleteBehavior"": ""Delete"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiKeyPaths"": [
                                {
                                    ""ApiKind"": ""Scalar"",
                                    ""ClrPropertyName"": ""UserId""
                                }
                            ]
                        }
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
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipUser_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipUser.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUser.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.UserName),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUser.UserName),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUser.Posts),
                                ApiTypeExpression: new ApiTypeExpression
                                (
                                    apiInlineType: new ApiCollectionType
                                    (
                                        apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipPost)),
                                        apiItemTypeModifiers: ApiTypeModifiers.Required,
                                        clrCollectionType: typeof(List<RelationshipPost>)
                                    )
                                ),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUser.Posts),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ]
                    ),

                    // UserRef
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipUserRef),
                        ClrType: typeof(RelationshipUserRef),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipUserRef_UserId",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipUserRef.UserId)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipUserRef.UserId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipUserRef.UserId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ]
                    ),

                    // Post
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipPost),
                        ClrType: typeof(RelationshipPost),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipPost_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipPost.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.AuthorUserId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.AuthorUserId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.AuthorRef),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipUserRef)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.AuthorRef),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Title),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Title),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Comments),
                                ApiTypeExpression: new ApiTypeExpression
                                (
                                    apiInlineType: new ApiCollectionType
                                    (
                                        apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipComment)),
                                        apiItemTypeModifiers: ApiTypeModifiers.Required,
                                        clrCollectionType: typeof(List<RelationshipComment>)
                                    )
                                ),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Comments),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.User),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipUser)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.User),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),

                    // RelationshipPostRef
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipPostRef),
                        ClrType: typeof(RelationshipPostRef),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipPostRef_PostId",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipPostRef.PostId)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPostRef.PostId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPostRef.PostId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ]
                    ),

                    // Comment
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipComment),
                        ClrType: typeof(RelationshipComment),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipComment_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipComment.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipComment.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.PostId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipComment.PostId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.PostRef),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipPostRef)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipComment.PostRef),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.Body),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipComment.Body),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipComment.Post),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipPost)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipComment.Post),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ]
                    ),
                ],
                ApiRelationships:
                [
                    // User_Posts_ScalarFK
                    new ApiOneToManyRelationshipDef
                    (
                        ApiName: "User_Posts_ScalarFK",
                        PrincipalEnd: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipUser)
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipPost),
                            ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipPost.AuthorUserId))]
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // User_Posts_NestedFK
                    new ApiOneToManyRelationshipDef
                    (
                        ApiName: "User_Posts_NestedFK",
                        PrincipalEnd: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipUser)
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipPost),
                            ApiKeyPaths:
                            [
                                new ApiRelationshipNestedKeyPathDef
                                (
                                    ClrPropertyName: nameof(RelationshipPost.AuthorRef),
                                    ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipUserRef.UserId))]
                                )
                            ]
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // Post_Comments_ScalarFK
                    new ApiOneToManyRelationshipDef
                    (
                        ApiName: "Post_Comments_ScalarFK",
                        PrincipalEnd: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipPost)
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipComment),
                            ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipComment.PostId))]
                        ),
                        ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Delete
                    ),

                    // Post_Comments_NestedFK
                    new ApiOneToManyRelationshipDef
                    (
                        ApiName: "Post_Comments_NestedFK",
                        PrincipalEnd: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipPost)
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipComment),
                            ApiKeyPaths:
                            [
                                new ApiRelationshipNestedKeyPathDef
                                (
                                    ClrPropertyName: nameof(RelationshipComment.PostRef),
                                    ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipPostRef.PostId))]
                                )
                            ]
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipComment_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            }
                        ],
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipPost"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipPost_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            }
                        ],
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
                                ""ApiName"": ""AuthorRef"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""RelationshipUserRef""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""AuthorRef"",
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
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Object"",
                                            ""ApiName"": ""RelationshipComment""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Core.Tests]],System.Private.CoreLib""
                                    }
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipPostRef"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipPostRef_PostId"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""PostId""
                                    }
                                ]
                            }
                        ],
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPostRef,Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipUser"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipUser_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            }
                        ],
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
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Object"",
                                            ""ApiName"": ""RelationshipPost""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests]],System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Posts"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipUserRef"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipUserRef_UserId"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""UserId""
                                    }
                                ]
                            }
                        ],
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipUserRef,Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""Post_Comments_NestedFK"",
                        ""ApiDeleteBehavior"": ""Delete"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiKeyPaths"": [
                                {
                                    ""ApiKind"": ""Nested"",
                                    ""ClrPropertyName"": ""PostRef"",
                                    ""ApiKeyPaths"": [
                                        {
                                            ""ApiKind"": ""Scalar"",
                                            ""ClrPropertyName"": ""PostId""
                                        }
                                    ]
                                }
                            ]
                        }
                    },
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""Post_Comments_ScalarFK"",
                        ""ApiDeleteBehavior"": ""Delete"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipComment,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiKeyPaths"": [
                                {
                                    ""ApiKind"": ""Scalar"",
                                    ""ClrPropertyName"": ""PostId""
                                }
                            ]
                        }
                    },
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""User_Posts_NestedFK"",
                        ""ApiDeleteBehavior"": ""Delete"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiKeyPaths"": [
                                {
                                    ""ApiKind"": ""Nested"",
                                    ""ClrPropertyName"": ""AuthorRef"",
                                    ""ApiKeyPaths"": [
                                        {
                                            ""ApiKind"": ""Scalar"",
                                            ""ClrPropertyName"": ""UserId""
                                        }
                                    ]
                                }
                            ]
                        }
                    },
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""User_Posts_ScalarFK"",
                        ""ApiDeleteBehavior"": ""Delete"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiKeyPaths"": [
                                {
                                    ""ApiKind"": ""Scalar"",
                                    ""ClrPropertyName"": ""AuthorUserId""
                                }
                            ]
                        }
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
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipPost_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipPost.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Title),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Title),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPost.Tags),
                                ApiTypeExpression: new ApiTypeExpression
                                (
                                    apiInlineType: new ApiCollectionType
                                    (
                                        apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipTag)),
                                        apiItemTypeModifiers: ApiTypeModifiers.Required,
                                        clrCollectionType: typeof(List<RelationshipTag>)
                                    )
                                ),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.Tags),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),

                    // Tag
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipTag),
                        ClrType: typeof(RelationshipTag),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipTag_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipTag.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipTag.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipTag.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipTag.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipTag.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipTag.Posts),
                                ApiTypeExpression: new ApiTypeExpression
                                (
                                    apiInlineType: new ApiCollectionType
                                    (
                                        apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipPost)),
                                        apiItemTypeModifiers: ApiTypeModifiers.Required,
                                        clrCollectionType: typeof(List<RelationshipPost>)
                                    )
                                ),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipTag.Posts),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),

                    // PostTag
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(RelationshipPostTag),
                        ClrType: typeof(RelationshipPostTag),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_RelationshipPostTag_PostId_TagId",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipPostTag.PostId)
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(RelationshipPostTag.TagId)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPostTag.PostId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPostTag.PostId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(RelationshipPostTag.TagId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPostTag.TagId),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                ],
                ApiRelationships:
                [
                    // Post_Tags
                    new ApiManyToManyRelationshipDef
                    (
                        ApiName: "Post_Tags",
                        PrincipalEndA: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipPost)
                        ),
                        PrincipalEndB: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipTag)
                        ),
                        DependentEndA: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipPostTag),
                            ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipPostTag.PostId))]
                        ),
                        DependentEndB: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipPostTag),
                            ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipPostTag.TagId))]
                        ),
                        ClrAssociationObjectType: typeof(RelationshipPostTag)
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Relationship Schema for Many-To-Many Relationship And Scalar Foreign Key Paths"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipPost_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            }
                        ],
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
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Object"",
                                            ""ApiName"": ""RelationshipTag""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.RelationshipTag,Evoogle.ApiFramework.Core.Tests]],System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Tags"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipPostTag"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipPostTag_PostId_TagId"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""PostId""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""TagId""
                                    }
                                ]
                            }
                        ],
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipPostTag,Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelationshipTag"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_RelationshipTag_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            }
                        ],
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
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Object"",
                                            ""ApiName"": ""RelationshipPost""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests]],System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Posts"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.RelationshipTag,Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""Post_Tags"",
                        ""ApiPrincipalEndA"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiPrincipalEndB"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipTag,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiDependentEndA"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPostTag,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiKeyPaths"": [
                                {
                                    ""ApiKind"": ""Scalar"",
                                    ""ClrPropertyName"": ""PostId""
                                }
                            ]
                        },
                        ""ApiDependentEndB"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPostTag,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiKeyPaths"": [
                                {
                                    ""ApiKind"": ""Scalar"",
                                    ""ClrPropertyName"": ""TagId""
                                }
                            ]
                        },
                        ""ClrAssociationObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPostTag,Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }"
        }
    ];
    #endregion
}
