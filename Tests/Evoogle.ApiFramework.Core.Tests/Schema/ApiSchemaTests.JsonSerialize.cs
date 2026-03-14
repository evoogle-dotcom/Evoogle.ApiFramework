// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        new JsonSerializeTest
        {
            Name = "Null",
            SourceFactoryArgument = null,
            ExpectedJson = "null"
        },

        // ApiSchema With 0 ApiType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}"
                )
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                )
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType And ApiSchemaOptions"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType And ApiSchemaOptions
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And ApiSchemaOptions"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType And GraphQlExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And GraphQlExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    }
                }
            }"
        },

        // ApiSchema With 2 ApiScalarType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 2 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 3 ApiScalarType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }"
        },

        // ApiSchema With 1 ApiEnumType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""Unspecified"",
                                ""ClrName"": ""Unspecified"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""Male"",
                                ""ClrName"": ""Male"",
                                ""ClrOrdinal"": 1
                            },
                            {
                                ""ApiName"": ""Female"",
                                ""ClrName"": ""Female"",
                                ""ClrOrdinal"": 2
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiEnumType And ProtobufExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
                    ExtensionTypes: [ typeof(ProtobufExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType And ProtobufExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""Unspecified"",
                                ""ClrName"": ""Unspecified"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""Male"",
                                ""ClrName"": ""Male"",
                                ""ClrOrdinal"": 1
                            },
                            {
                                ""ApiName"": ""Female"",
                                ""ClrName"": ""Female"",
                                ""ClrOrdinal"": 2
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.ProtobufExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Edition"": 2024
                    }
                }
            }"
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Ulid)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Ulid)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_Person_Id",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Id),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.CompanyId),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.CompanyId),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                            ]
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Ulid"",
                        ""ClrType"": ""System.Ulid, Ulid""
                    }
                ],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""Unspecified"",
                                ""ClrName"": ""Unspecified"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""Male"",
                                ""ClrName"": ""Male"",
                                ""ClrOrdinal"": 1
                            },
                            {
                                ""ApiName"": ""Female"",
                                ""ClrName"": ""Female"",
                                ""ClrOrdinal"": 2
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Person"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_Person_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Person_Name"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiPropertyName"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
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
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Enum"",
                                    ""ApiName"": ""Gender""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Hobbies"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Hobbies"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""CompanyId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""CompanyId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiRelationships"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }"
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Ulid)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Ulid)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_Person_Id",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Id),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.CompanyId),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.CompanyId),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                            ]
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Ulid"",
                        ""ClrType"": ""System.Ulid, Ulid""
                    }
                ],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""Unspecified"",
                                ""ClrName"": ""Unspecified"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""Male"",
                                ""ClrName"": ""Male"",
                                ""ClrOrdinal"": 1
                            },
                            {
                                ""ApiName"": ""Female"",
                                ""ClrName"": ""Female"",
                                ""ClrOrdinal"": 2
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Person"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_Person_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Person_Name"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiPropertyName"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
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
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Enum"",
                                    ""ApiName"": ""Gender""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Hobbies"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Hobbies"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""CompanyId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""CompanyId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiRelationships"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }"
        },

        // ApiSchema With 3 ApiScalarTypes And 1 ApiEnumType And 2 ApiObjectTypes (Company And Person)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Ulid)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Ulid)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Company)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Company)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_Company_Id",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Company.Id),
                                            ClrScalarTypeHint: typeof(string)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Company_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Company.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Id),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Owner),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Owner),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Employees),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<Person>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Employees),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                            ],
                            ApiRelationships:
                            [
                                new ApiRelationshipConfig(ApiName: "Company_Owner", ApiPropertyName: nameof(Company.Owner)),
                                new ApiRelationshipConfig(ApiName: "Company_Employees", ApiPropertyName: nameof(Company.Employees))
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_Person_Id",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Id),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.CompanyId),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.CompanyId),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                            ]
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 2 ApiObjectType (Company And Person)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Ulid"",
                        ""ClrType"": ""System.Ulid, Ulid""
                    }
                ],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""Unspecified"",
                                ""ClrName"": ""Unspecified"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""Male"",
                                ""ClrName"": ""Male"",
                                ""ClrOrdinal"": 1
                            },
                            {
                                ""ApiName"": ""Female"",
                                ""ClrName"": ""Female"",
                                ""ClrOrdinal"": 2
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Company"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_Company_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiPropertyName"": ""Id"",
					                    ""ClrScalarTypeHint"": ""System.String,System.Private.CoreLib""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Company_Name"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiPropertyName"": ""Name""
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
                                ""ApiName"": ""Owner"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""Person""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Owner"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Employees"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Object"",
                                            ""ApiName"": ""Person""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.Person,Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Employees"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiRelationships"": [
                            {
                                ""ApiName"": ""Company_Owner"",
                                ""ApiPropertyName"": ""Owner""
                            },
                            {
                                ""ApiName"": ""Company_Employees"",
                                ""ApiPropertyName"": ""Employees""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Company, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Person"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_Person_Id"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Person_Name"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiPropertyName"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
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
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Enum"",
                                    ""ApiName"": ""Gender""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Hobbies"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Hobbies"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""CompanyId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Ulid""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""CompanyId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiRelationships"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonSerializeTheoryData))]
    public void JsonSerialize(IXUnitTest test) => test.Execute(this);
    #endregion
}
