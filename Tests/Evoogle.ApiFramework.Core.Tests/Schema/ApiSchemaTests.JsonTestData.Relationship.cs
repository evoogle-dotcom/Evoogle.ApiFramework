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
            Name = $"{nameof(ApiSchema)} With Relationship Schema for One-To-One Relationship",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Relationship Schema for One-To-One Relationship",
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
                            ClrObjectType: typeof(RelationshipUser),
                            ApiDeleteBehavior: ApiRelationshipDeleteBehavior.None
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipUserProfile),
                            ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Cascade,
                            ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipUserProfile.UserId))]
                        )
                    ),

                    // User_Profile_NestedFK
                    new ApiOneToOneRelationshipDef
                    (
                        ApiName: "User_Profile_NestedFK",
                        PrincipalEnd: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipUser),
                            ApiDeleteBehavior: ApiRelationshipDeleteBehavior.None
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipUserProfile),
                            ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Cascade,
                            ApiKeyPaths:
                            [
                                new ApiRelationshipNestedKeyPathDef
                                (
                                    ClrPropertyName: nameof(RelationshipUserProfile.UserRef),
                                    ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipUserRef.UserId))]
                                )
                            ]
                        )
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Relationship Schema for One-To-One Relationship"",
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
                        ""ApiPrincipalEnd"": {
                            ""ApiKind"": ""Principal"",
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiDeleteBehavior"": ""None""
                        },
                        ""ApiDependentEnd"": {
                            ""ApiKind"": ""Dependent"",
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiDeleteBehavior"": ""Cascade"",
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
                        ""ApiPrincipalEnd"": {
                            ""ApiKind"": ""Principal"",
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiDeleteBehavior"": ""None""
                        },
                        ""ApiDependentEnd"": {
                            ""ApiKind"": ""Dependent"",
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUserProfile,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiDeleteBehavior"": ""Cascade"",
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
            Name = $"{nameof(ApiSchema)} With Relationship Schema for One-To-Many Relationship",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Relationship Schema for One-To-Many Relationship",
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
                                ApiName: nameof(RelationshipPost.User),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(RelationshipUser)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(RelationshipPost.User),
                                ClrMemberKind: ClrMemberKind.Property
                            )
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
                            ClrObjectType: typeof(RelationshipUser),
                            ApiDeleteBehavior: ApiRelationshipDeleteBehavior.None
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipPost),
                            ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Cascade,
                            ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipPost.AuthorUserId))]
                        )
                    ),

                    // User_Posts_NestedFK
                    new ApiOneToManyRelationshipDef
                    (
                        ApiName: "User_Posts_NestedFK",
                        PrincipalEnd: new ApiPrincipalEndDef
                        (
                            ClrObjectType: typeof(RelationshipUser),
                            ApiDeleteBehavior: ApiRelationshipDeleteBehavior.None
                        ),
                        DependentEnd: new ApiDependentEndDef
                        (
                            ClrObjectType: typeof(RelationshipPost),
                            ApiDeleteBehavior: ApiRelationshipDeleteBehavior.Cascade,
                            ApiKeyPaths:
                            [
                                new ApiRelationshipNestedKeyPathDef
                                (
                                    ClrPropertyName: nameof(RelationshipPost.AuthorRef),
                                    ApiKeyPaths: [new ApiRelationshipScalarKeyPathDef(ClrPropertyName: nameof(RelationshipUserRef.UserId))]
                                )
                            ]
                        )
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Relationship Schema for One-To-Many Relationship"",
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
                        ""ApiName"": ""User_Posts_NestedFK"",
                        ""ApiPrincipalEnd"": {
                            ""ApiKind"": ""Principal"",
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiDeleteBehavior"": ""None""
                        },
                        ""ApiDependentEnd"": {
                            ""ApiKind"": ""Dependent"",
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiDeleteBehavior"": ""Cascade"",
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
                        ""ApiPrincipalEnd"": {
                            ""ApiKind"": ""Principal"",
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipUser,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiDeleteBehavior"": ""None""
                        },
                        ""ApiDependentEnd"": {
                            ""ApiKind"": ""Dependent"",
                            ""ClrObjectType"": ""Evoogle.ApiFramework.TestData.RelationshipPost,Evoogle.ApiFramework.Core.Tests"",
                            ""ApiDeleteBehavior"": ""Cascade"",
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
    ];
    #endregion
}
