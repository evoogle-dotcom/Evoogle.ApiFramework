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
    private static ApiSchemaJsonTestCase[] KeyJsonTestCases =>
    [
        // ApiSchema With Key Schema (KeyOneScalarPart)
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyOneScalarPart)})",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyOneScalarPart)})",
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
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyOneScalarPart),
                        ClrType: typeof(KeyOneScalarPart),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOneScalarPart.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOneScalarPart.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOneScalarPart.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOneScalarPart.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyOneScalarPart",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOneScalarPart), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOneScalarPart.Id))])
                                ]
                            ),
                            new ApiKeyTypeDef
                            (
                                ApiName: "AK_KeyOneScalarPart",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOneScalarPart), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOneScalarPart.Name))])
                                ]
                            )
                        ]
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Key Schema (KeyOneScalarPart)"",
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyOneScalarPart"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyOneScalarPart"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOneScalarPart, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_KeyOneScalarPart"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOneScalarPart, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Name"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyOneScalarPart, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Key Schema (KeyTwoScalarPartComposite)
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyTwoScalarPartComposite)})",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyTwoScalarPartComposite)})",
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
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyTwoScalarPartComposite),
                        ClrType: typeof(KeyTwoScalarPartComposite),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyTwoScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyTwoScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyTwoScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyTwoScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyTwoScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyTwoScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyTwoScalarPartComposite",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyTwoScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyTwoScalarPartComposite.Id1))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyTwoScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyTwoScalarPartComposite.Id2))])
                                ]
                            )
                        ]
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Key Schema (KeyTwoScalarPartComposite)"",
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyTwoScalarPartComposite"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyTwoScalarPartComposite"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyTwoScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id1"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyTwoScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id2"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyTwoScalarPartComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Key Schema (KeyThreeScalarPartComposite)
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyThreeScalarPartComposite)})",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyThreeScalarPartComposite)})",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Guid),
                        ClrType: typeof(Guid)
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
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyThreeScalarPartComposite),
                        ClrType: typeof(KeyThreeScalarPartComposite),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyThreeScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyThreeScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyThreeScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyThreeScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyThreeScalarPartComposite.Id3),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyThreeScalarPartComposite.Id3),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyThreeScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyThreeScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyThreeScalarPartComposite",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyThreeScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyThreeScalarPartComposite.Id1))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyThreeScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyThreeScalarPartComposite.Id2))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyThreeScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyThreeScalarPartComposite.Id3))])
                                ]
                            )
                        ]
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Key Schema (KeyThreeScalarPartComposite)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
                        ""ApiName"": ""KeyThreeScalarPartComposite"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyThreeScalarPartComposite"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id1"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id2"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id3"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Key Schema (KeyNested And KeyNestedComposite)
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyNested)} And {nameof(KeyNestedComposite)})",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyNested)} And {nameof(KeyNestedComposite)})",
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
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyNested),
                        ClrType: typeof(KeyNested),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyNested.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyNested.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyNested.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyNested.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyNestedPart",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyNested), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyNested.Id))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyNestedComposite),
                        ClrType: typeof(KeyNestedComposite),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyNestedComposite.NestedPart),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(KeyNested)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyNestedComposite.NestedPart),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyNestedComposite.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyNestedComposite.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyNestedComposite",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyNestedComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyNestedComposite.NestedPart)), new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyNested.Id))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyNestedComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyNestedComposite.Name))])
                                ]
                            )
                        ]
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Key Schema (KeyNested And KeyNestedComposite)"",
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyNested"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyNestedPart"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyNested, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyNested, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyNestedComposite"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NestedPart"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""KeyNested""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyNestedComposite"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyNestedComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""NestedPart"" },
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyNestedComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Name"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyNestedComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Key Schema (KeyOwner, KeyOwnedComposite, And KeyOwnedDependent)
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyOwner)}, {nameof(KeyOwnedComposite)}, And {nameof(KeyOwnedDependent)})",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Key Schema ({nameof(KeyOwner)}, {nameof(KeyOwnedComposite)}, And {nameof(KeyOwnedDependent)})",
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
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyOwner),
                        ClrType: typeof(KeyOwner),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwner.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwner.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwner.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyOwner.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwner.Dependents),
                                ApiTypeExpression: new ApiTypeExpression
                                (
                                    apiInlineType: new ApiCollectionType
                                    (
                                        apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(KeyOwnedComposite)),
                                        apiItemTypeModifiers: ApiTypeModifiers.Required,
                                        clrCollectionType: typeof(List<KeyOwnedComposite>)
                                    )
                                ),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwner.Dependents),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwner.Dependent),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(KeyOwnedDependent)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwner.Dependent),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyOwner",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOwner), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOwner.Id))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyOwnedComposite),
                        ClrType: typeof(KeyOwnedComposite),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwnedComposite.LineNumber),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwnedComposite.LineNumber),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwnedComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwnedComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyOwnedComposite",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOwner), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOwner.Id))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOwnedComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOwnedComposite.LineNumber))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyOwnedDependent),
                        ClrType: typeof(KeyOwnedDependent),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwnedDependent.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwnedDependent.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyOwnedDependent",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOwner), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOwner.Id))])
                                ]
                            )
                        ]
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Key Schema (KeyOwner, KeyOwnedComposite, And KeyOwnedDependent)"",
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
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyOwnedComposite"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyOwnedComposite"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOwner, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOwnedComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""LineNumber"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyOwnedComposite, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyOwnedDependent"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyOwnedDependent"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOwner, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyOwnedDependent, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyOwner"",
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
                                            ""ApiName"": ""KeyOwnedComposite""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.KeyOwnedComposite,Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
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
                                    ""ApiName"": ""KeyOwnedDependent""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Dependent"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyOwner"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOwner, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyOwner, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        // ApiSchema With Full Key Schema
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Full Key Schema",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Full Key Schema",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Guid),
                        ClrType: typeof(Guid)
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
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyOneScalarPart),
                        ClrType: typeof(KeyOneScalarPart),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOneScalarPart.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOneScalarPart.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOneScalarPart.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOneScalarPart.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyOneScalarPart",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOneScalarPart), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOneScalarPart.Id))])
                                ]
                            ),
                            new ApiKeyTypeDef
                            (
                                ApiName: "AK_KeyOneScalarPart",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOneScalarPart), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOneScalarPart.Name))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyTwoScalarPartComposite),
                        ClrType: typeof(KeyTwoScalarPartComposite),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyTwoScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyTwoScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyTwoScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyTwoScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyTwoScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyTwoScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyTwoScalarPartComposite",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyTwoScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyTwoScalarPartComposite.Id1))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyTwoScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyTwoScalarPartComposite.Id2))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyThreeScalarPartComposite),
                        ClrType: typeof(KeyThreeScalarPartComposite),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyThreeScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyThreeScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyThreeScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyThreeScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyThreeScalarPartComposite.Id3),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyThreeScalarPartComposite.Id3),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyThreeScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyThreeScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyThreeScalarPartComposite",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyThreeScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyThreeScalarPartComposite.Id1))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyThreeScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyThreeScalarPartComposite.Id2))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyThreeScalarPartComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyThreeScalarPartComposite.Id3))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyNested),
                        ClrType: typeof(KeyNested),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyNested.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyNested.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyNested.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyNested.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyNestedPart",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyNested), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyNested.Id))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyNestedComposite),
                        ClrType: typeof(KeyNestedComposite),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyNestedComposite.NestedPart),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(KeyNested)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyNestedComposite.NestedPart),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyNestedComposite.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyNestedComposite.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyNestedComposite",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyNestedComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyNestedComposite.NestedPart)), new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyNested.Id))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyNestedComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyNestedComposite.Name))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyOwner),
                        ClrType: typeof(KeyOwner),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwner.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwner.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwner.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(KeyOwner.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwner.Dependents),
                                ApiTypeExpression: new ApiTypeExpression
                                (
                                    apiInlineType: new ApiCollectionType
                                    (
                                        apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(KeyOwnedComposite)),
                                        apiItemTypeModifiers: ApiTypeModifiers.Required,
                                        clrCollectionType: typeof(List<KeyOwnedComposite>)
                                    )
                                ),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwner.Dependents),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwner.Dependent),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(KeyOwnedDependent)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwner.Dependent),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyOwner",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOwner), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOwner.Id))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyOwnedComposite),
                        ClrType: typeof(KeyOwnedComposite),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwnedComposite.LineNumber),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwnedComposite.LineNumber),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwnedComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwnedComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyOwnedComposite",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOwner), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOwner.Id))]),
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOwnedComposite), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOwnedComposite.LineNumber))])
                                ]
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(KeyOwnedDependent),
                        ClrType: typeof(KeyOwnedDependent),
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(KeyOwnedDependent.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(KeyOwnedDependent.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ],
                        ApiKeyTypes:
                        [
                            new ApiKeyTypeDef
                            (
                                ApiName: "PK_KeyOwnedDependent",
                                ApiKeyPaths:
                                [
                                    new ApiKeyPathDef(ClrRootType: typeof(KeyOwner), [new ApiKeyPathSegmentDef(ClrPropertyName: nameof(KeyOwner.Id))])
                                ]
                            )
                        ]
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Full Key Schema"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
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
                        ""ApiName"": ""KeyNested"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyNestedPart"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyNested, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyNested, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyNestedComposite"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NestedPart"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""KeyNested""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyNestedComposite"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyNestedComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""NestedPart"" },
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyNestedComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Name"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyNestedComposite, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyOneScalarPart"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyOneScalarPart"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOneScalarPart, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AK_KeyOneScalarPart"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOneScalarPart, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Name"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyOneScalarPart, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyOwnedComposite"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyOwnedComposite"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOwner, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOwnedComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""LineNumber"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyOwnedComposite, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyOwnedDependent"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyOwnedDependent"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOwner, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyOwnedDependent, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyOwner"",
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
                                            ""ApiName"": ""KeyOwnedComposite""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.KeyOwnedComposite,Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
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
                                    ""ApiName"": ""KeyOwnedDependent""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Dependent"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyOwner"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOwner, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyOwner, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyThreeScalarPartComposite"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyThreeScalarPartComposite"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id1"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id2"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id3"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyThreeScalarPartComposite, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""KeyTwoScalarPartComposite"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_KeyTwoScalarPartComposite"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyTwoScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id1"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyTwoScalarPartComposite, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id2"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.KeyTwoScalarPartComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },
    ];
    #endregion
}
