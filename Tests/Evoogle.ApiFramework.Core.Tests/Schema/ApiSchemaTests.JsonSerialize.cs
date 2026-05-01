// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
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
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}"
                )
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsParams
                    (
                        ApiIdentityPartNullHandling: ApiIdentityPartNullHandling.ThrowOnNull
                    )
                )
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType And ApiSchemaOptions"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""ThrowOnNull""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType And ApiSchemaOptions
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsParams
                    (
                        ApiIdentityPartNullHandling: ApiIdentityPartNullHandling.ThrowOnNull
                    )
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
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
                    ""ApiIdentityPartNullHandling"": ""ThrowOnNull""
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
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType And GraphQlExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                ""ApiRelationships"": [],
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
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 3 ApiScalarType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                ""ApiRelationships"": [],
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
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 1 ApiEnumType And ProtobufExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
                    ExtensionTypes: [ typeof(ProtobufExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                ""ApiRelationships"": [],
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
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Ulid)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Ulid)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_Person_Id",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityParams
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.CompanyId),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Person_Name"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Name""
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Ulid)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Ulid)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_Person_Id",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityParams
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.CompanyId),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Person_Name"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Name""
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [],
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
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Ulid)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Ulid)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Company)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Company)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_Company_Id",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Company.Id),
                                            ClrScalarTypeHint: typeof(string)
                                        )
                                    ]
                                ),
                                new ApiIdentityParams
                                (
                                    ApiName: "AK_Company_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Company.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Company.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Company.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Company.Owner),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(Person)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Owner),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Company.Employees),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(Person)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<Person>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Employees),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_Person_Id",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityParams
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(Person.CompanyId),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
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
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                                        ""ClrPropertyName"": ""Id"",
					                    ""ClrScalarTypeHint"": ""System.String,System.Private.CoreLib""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Company_Name"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Name""
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
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Person_Name"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Name""
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Identity Schema (IdentityScalar)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityScalar)})",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityScalar)})"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityScalar)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityScalar)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityScalar",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityScalar.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityParams
                                (
                                    ApiName: "AK_IdentityScalar",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityScalar.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityScalar.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityScalar.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityScalar.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityScalar.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With Identity Schema (IdentityScalar)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityScalar"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityScalar"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_IdentityScalar"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Name""
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
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityScalar, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Identity Schema (IdentityTwoScalarPartComposite)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityTwoScalarPartComposite)})",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityTwoScalarPartComposite)})"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityTwoScalarPartComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityTwoScalarPartComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityTwoScalarPartComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id1)
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id2)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Id1),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Id1),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Id2),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Id2),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With Identity Schema (IdentityTwoScalarPartComposite)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityTwoScalarPartComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityTwoScalarPartComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id1""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id2""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id1"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id1"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Id2"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Id2"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityTwoScalarPartComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Identity Schema (IdentityThreeScalarPartComposite)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityThreeScalarPartComposite)})",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityThreeScalarPartComposite)})"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Guid)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Guid)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityThreeScalarPartComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityThreeScalarPartComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityThreeScalarPartComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id1)
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id2)
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id3)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id1),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id1),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id2),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id2),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id3),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id3),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With Identity Schema (IdentityThreeScalarPartComposite)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
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
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityThreeScalarPartComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityThreeScalarPartComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id1""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id2""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id3""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id1"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id1"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Id2"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Id2"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Id3"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Guid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id3"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Identity Schema (IdentityNested And IdentityNestedComposite Without Explicit ApiIdentityName)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} Without Explicit ApiIdentityName)",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} Without Explicit ApiIdentityName)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNested)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityNested)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityNestedPart",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNested.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNested.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNested.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNested.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityNested.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNestedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityNestedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityNestedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Nested,
                                            ApiPropertyName: nameof(IdentityNestedComposite.NestedPart)
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNestedComposite.NestedPart),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.NestedPart),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNestedComposite.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With Identity Schema (IdentityNested And IdentityNestedComposite Without Explicit ApiIdentityName)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityNested"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityNestedPart"",
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
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNested, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityNestedComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityNestedComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Nested"",
                                        ""ClrPropertyName"": ""NestedPart""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NestedPart"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""IdentityNested""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NestedPart"",
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
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNestedComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Identity Schema (IdentityOwner, IdentityOwnedComposite, And IdentityOwnedDependent Without Explicit ApiIdentityName)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} Without Explicit ApiIdentityName)",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} Without Explicit ApiIdentityName)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwner)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityOwner)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityOwner",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwner.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityOwner.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Dependents),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedComposite)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<IdentityOwnedComposite>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependents),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Dependent),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependent),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityOwnedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityOwnedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwnedComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedDependent)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityOwnedDependent)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityOwnedDependent",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwnedDependent.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedDependent.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With Identity Schema (IdentityOwner, IdentityOwnedComposite, And IdentityOwnedDependent Without Explicit ApiIdentityName)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityOwnedComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityOwnedComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Owner""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""LineNumber""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""LineNumber"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""LineNumber"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityOwnedComposite, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityOwnedDependent"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityOwnedDependent"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Owner""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityOwnedDependent, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityOwner"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityOwner"",
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
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Dependents"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Object"",
                                            ""ApiName"": ""IdentityOwnedComposite""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.IdentityOwnedComposite,Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Dependents"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Dependent"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""IdentityOwnedDependent""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Dependent"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityOwner, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Full Identity Schema
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With Full Identity Schema",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With Full Identity Schema"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Guid)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Guid)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityScalar)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityScalar)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityScalar",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityScalar.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityParams
                                (
                                    ApiName: "AK_IdentityScalar",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityScalar.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityScalar.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityScalar.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityScalar.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityScalar.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityTwoScalarPartComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityTwoScalarPartComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityTwoScalarPartComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id1)
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id2)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Id1),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Id1),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Id2),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Id2),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityThreeScalarPartComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityThreeScalarPartComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityThreeScalarPartComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id1)
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id2)
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id3)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id1),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id1),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id2),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id2),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id3),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id3),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNested)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityNested)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityNestedPart",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNested.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNested.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNested.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNested.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityNested.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNestedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityNestedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityNestedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Nested,
                                            ApiPropertyName: nameof(IdentityNestedComposite.NestedPart)
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNestedComposite.NestedPart),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.NestedPart),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNestedComposite.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwner)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityOwner)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityOwner",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwner.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityOwner.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Dependents),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedComposite)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<IdentityOwnedComposite>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependents),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Dependent),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependent),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityOwnedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityOwnedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwnedComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedDependent)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityOwnedDependent)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityOwnedDependent",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwnedDependent.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedDependent.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With Full Identity Schema"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
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
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityNested"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityNestedPart"",
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
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNested, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityNestedComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityNestedComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Nested"",
                                        ""ClrPropertyName"": ""NestedPart""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NestedPart"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""IdentityNested""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NestedPart"",
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
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNestedComposite, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityOwnedComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityOwnedComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Owner""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""LineNumber""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""LineNumber"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""LineNumber"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityOwnedComposite, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityOwnedDependent"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityOwnedDependent"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Owner""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityOwnedDependent, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityOwner"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityOwner"",
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
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Dependents"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Object"",
                                            ""ApiName"": ""IdentityOwnedComposite""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.IdentityOwnedComposite,Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Dependents"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Dependent"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""IdentityOwnedDependent""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Dependent"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityOwner, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityScalar"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityScalar"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_IdentityScalar"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Name""
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
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityScalar, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityThreeScalarPartComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityThreeScalarPartComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id1""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id2""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id3""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id1"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id1"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Id2"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Id2"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Id3"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Guid""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id3"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityTwoScalarPartComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityTwoScalarPartComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id1""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id2""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id1"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id1"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Id2"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Id2"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityTwoScalarPartComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Identity Schema (IdentityNested And IdentityNestedComposite With Explicit ApiIdentityName)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} With Explicit ApiIdentityName)",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} With Explicit ApiIdentityName)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNested)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityNested)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityNestedPart",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNested.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNested.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNested.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNested.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityNested.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNestedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityNestedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityNestedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Nested,
                                            ApiPropertyName: nameof(IdentityNestedComposite.NestedPart),
                                            ApiIdentityName: "PK_IdentityNestedPart"
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNestedComposite.NestedPart),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.NestedPart),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityNestedComposite.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With Identity Schema (IdentityNested And IdentityNestedComposite With Explicit ApiIdentityName)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityNested"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityNestedPart"",
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
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNested, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityNestedComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityNestedComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Nested"",
                                        ""ClrPropertyName"": ""NestedPart"",
                                        ""ApiIdentityName"": ""PK_IdentityNestedPart""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NestedPart"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""IdentityNested""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NestedPart"",
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
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNestedComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Identity Schema (IdentityOwner, IdentityOwnedComposite, And IdentityOwnedDependent With Explicit ApiIdentityName)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} With Explicit ApiIdentityName)",
            SourceFactoryArgument = new ApiSchemaSpec
            (
                ApiSchema: new ApiSchemaParams
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} With Explicit ApiIdentityName)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwner)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityOwner)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityOwner",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwner.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityOwner.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Dependents),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedComposite)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<IdentityOwnedComposite>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependents),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwner.Dependent),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependent),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityOwnedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityOwnedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner,
                                            ApiIdentityName: "PK_IdentityOwner"
                                        ),
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwnedComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeSpec
                    (
                        ApiType: new ApiTypeParams
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedDependent)
                        ),
                        ApiNamedType: new ApiNamedTypeParams
                        (
                            ApiName: nameof(IdentityOwnedDependent)
                        ),
                        ApiObjectType: new ApiObjectTypeParams
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityParams
                                (
                                    ApiName: "PK_IdentityOwnedDependent",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartParams
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner,
                                            ApiIdentityName: "PK_IdentityOwner"
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyParams
                                (
                                    ApiName: nameof(IdentityOwnedDependent.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedDependent.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With Identity Schema (IdentityOwner, IdentityOwnedComposite, And IdentityOwnedDependent With Explicit ApiIdentityName)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityPartNullHandling"": ""UseDefaultOnNull""
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityOwnedComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityOwnedComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Owner"",
                                        ""ApiIdentityName"": ""PK_IdentityOwner""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""LineNumber""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""LineNumber"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""LineNumber"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityOwnedComposite, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityOwnedDependent"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityOwnedDependent"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Owner"",
                                        ""ApiIdentityName"": ""PK_IdentityOwner""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityOwnedDependent, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityOwner"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityOwner"",
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
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Dependents"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Object"",
                                            ""ApiName"": ""IdentityOwnedComposite""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.IdentityOwnedComposite,Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Dependents"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Dependent"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""IdentityOwnedDependent""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Dependent"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityOwner, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
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
