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
    private static ApiSchemaJsonTestCase[] SimpleJsonTestCases =>
    [
        new ApiSchemaJsonTestCase
        {
            Name = "Null",
            FactoryArgument = null,
            Json = "null"
        },

        // ApiSchema With 0 ApiType
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}"
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }"
        },

        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
                ApiKeyNullHandling: ApiKeyNullHandling.ThrowOnNull
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType And ApiSchemaOptions"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""ThrowOnNull""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
                ApiKeyNullHandling: ApiKeyNullHandling.ThrowOnNull,
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And ApiSchemaOptions"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""ThrowOnNull""
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    )
                ],
                ExtensionTypes: [ typeof(GraphQlExtension) ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And GraphQlExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, Evoogle.ApiFramework.Tests"": {
                        ""Count"": 42
                    }
                }
            }"
        },

        // ApiSchema With 2 ApiScalarType
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 2 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                ],
                ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, Evoogle.ApiFramework.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.TestData.JsonApiExtension, Evoogle.ApiFramework.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }"
        },

        // ApiSchema With 1 ApiEnumType
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}",
                ApiNamedTypes:
                [
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 1 ApiEnumType And ProtobufExtension
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
                ApiNamedTypes:
                [
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    )
                ],
                ExtensionTypes: [ typeof(ProtobufExtension) ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType And ProtobufExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.ProtobufExtension, Evoogle.ApiFramework.Tests"": {
                        ""Edition"": 2024
                    }
                }
            }"
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
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
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(Person),
                        ClrType: typeof(Person),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Int32), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Name),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Age),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Int32), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Age),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Gender),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Enum, ApiName: nameof(Gender), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Gender),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Hobbies),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: new ApiCollectionTypeDef(ClrType: typeof(List<string>), ApiItemTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)), ApiItemTypeModifiers: ApiTypeModifiers.Required), ApiTypeReferenceDef: null),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Hobbies),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.CompanyId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.CompanyId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_Person_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(Person.Id)))])
                                ]
                            ),
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "AK_Person_Name",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(Person.Name)))])
                                ]
                            )
                        ]
                    ),
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Person"",
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
                                    ""ApiKind"": ""Collection"",
                                    ""ApiItemType"": {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiName"": ""String""
                                    },
                                    ""ApiItemTypeModifiers"": ""Required"",
                                    ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
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
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_Person_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Person_Name"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
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
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(Person),
                        ClrType: typeof(Person),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Int32), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Name),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Age),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Int32), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Age),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Gender),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Enum, ApiName: nameof(Gender), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Gender),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Hobbies),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: new ApiCollectionTypeDef(ClrType: typeof(List<string>), ApiItemTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)), ApiItemTypeModifiers: ApiTypeModifiers.Required), ApiTypeReferenceDef: null),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Hobbies),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.CompanyId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.CompanyId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_Person_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(Person.Id)))])
                                ]
                            ),
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "AK_Person_Name",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(Person.Name)))])
                                ]
                            )
                        ]
                    ),
                ],
                ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Person"",
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
                                    ""ApiKind"": ""Collection"",
                                    ""ApiItemType"": {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiName"": ""String""
                                    },
                                    ""ApiItemTypeModifiers"": ""Required"",
                                    ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
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
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_Person_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Person_Name"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, Evoogle.ApiFramework.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.TestData.JsonApiExtension, Evoogle.ApiFramework.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }"
        },

        // ApiSchema With 3 ApiScalarTypes And 1 ApiEnumType And 2 ApiObjectTypes (Company And Person)
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
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
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(Company),
                        ClrType: typeof(Company),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Company.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Company.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Company.Name),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Company.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Company.Owner),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(Person), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Company.Owner),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Company.Employees),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: new ApiCollectionTypeDef(ClrType: typeof(List<Person>), ApiItemTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: nameof(Person), ClrType: null)), ApiItemTypeModifiers: ApiTypeModifiers.Required), ApiTypeReferenceDef: null),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Company.Employees),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_Company_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(Company.Id)))])
                                ]
                            ),
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "AK_Company_Name",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(Company.Name)))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(Person),
                        ClrType: typeof(Person),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Id),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Int32), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Name),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Age),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Int32), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Age),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Gender),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Enum, ApiName: nameof(Gender), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Gender),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Hobbies),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: new ApiCollectionTypeDef(ClrType: typeof(List<string>), ApiItemTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(String), ClrType: null)), ApiItemTypeModifiers: ApiTypeModifiers.Required), ApiTypeReferenceDef: null),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Hobbies),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.CompanyId),
                                ApiTypeExpression: new ApiTypeExpressionDef(ApiTypeDef: null, ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: nameof(Ulid), ClrType: null)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.CompanyId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ],
                        ApiKeys:
                        [
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "PK_Person_Id",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(Person.Id)))])
                                ]
                            ),
                            new ApiKeyDefinitionDef
                            (
                                ApiName: "AK_Person_Name",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ApiRootObjectTypeReference: null, ApiSegments: [new ApiKeyPathSegmentDef(ApiPropertyReference: new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(Person.Name)))])
                                ]
                            )
                        ]
                    ),
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 2 ApiObjectType (Company And Person)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Company"",
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
                                    ""ApiKind"": ""Collection"",
                                    ""ApiItemType"": {
                                        ""ApiKind"": ""Object"",
                                        ""ApiName"": ""Person""
                                    },
                                    ""ApiItemTypeModifiers"": ""Required"",
                                    ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.Person,Evoogle.ApiFramework.Tests]], System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Employees"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_Company_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Company_Name"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Company, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Person"",
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
                                    ""ApiKind"": ""Collection"",
                                    ""ApiItemType"": {
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiName"": ""String""
                                    },
                                    ""ApiItemTypeModifiers"": ""Required"",
                                    ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
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
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PK_Person_Id"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_Person_Name"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrPath"": ""Name""
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },
    ];
    #endregion
}
