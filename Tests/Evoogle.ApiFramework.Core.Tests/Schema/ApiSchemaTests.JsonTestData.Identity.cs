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
    private static ApiSchemaJsonTestCase[] IdentityJsonTestCases =>
    [
        // ApiSchema With Identity Schema (IdentityScalar)
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityScalar)})",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityScalar)})",
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
                        ApiName: nameof(IdentityScalar),
                        ClrType: typeof(IdentityScalar),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityScalar",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityScalar.Id)
                                    )
                                ]
                            ),
                            new ApiIdentityDef
                            (
                                ApiName: "AK_IdentityScalar",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityScalar.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityScalar.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityScalar.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityScalar.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityScalar.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            ),
            Json = @"
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityTwoScalarPartComposite)})",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityTwoScalarPartComposite)})",
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
                        ApiName: nameof(IdentityTwoScalarPartComposite),
                        ClrType: typeof(IdentityTwoScalarPartComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityTwoScalarPartComposite",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id1)
                                    ),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id2)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            ),
            Json = @"
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityThreeScalarPartComposite)})",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityThreeScalarPartComposite)})",
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
                        ApiName: nameof(IdentityThreeScalarPartComposite),
                        ClrType: typeof(IdentityThreeScalarPartComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityThreeScalarPartComposite",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id1)
                                    ),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id2)
                                    ),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id3)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id3),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id3),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            ),
            Json = @"
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} Without Explicit ApiIdentityName)",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} Without Explicit ApiIdentityName)",
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
                        ApiName: nameof(IdentityNested),
                        ClrType: typeof(IdentityNested),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedPart",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNested.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNested.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityNested.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNestedComposite),
                        ClrType: typeof(IdentityNestedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedComposite",
                                Parts:
                                [
                                    new ApiIdentityNestedPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.NestedPart)
                                    ),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.NestedPart),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.NestedPart),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            ),
            Json = @"
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} Without Explicit ApiIdentityName)",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} Without Explicit ApiIdentityName)",
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
                        ApiName: nameof(IdentityOwner),
                        ClrType: typeof(IdentityOwner),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwner",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwner.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityOwner.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Dependent),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Dependent),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedComposite),
                        ClrType: typeof(IdentityOwnedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedComposite",
                                Parts:
                                [
                                    new ApiIdentityOwnerPartDef(),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedDependent),
                        ClrType: typeof(IdentityOwnedDependent),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedDependent",
                                Parts:
                                [
                                    new ApiIdentityOwnerPartDef()
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedDependent.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedDependent.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            ),
            Json = @"
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Full Identity Schema",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Full Identity Schema",
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
                        ApiName: nameof(IdentityScalar),
                        ClrType: typeof(IdentityScalar),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityScalar",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityScalar.Id)
                                    )
                                ]
                            ),
                            new ApiIdentityDef
                            (
                                ApiName: "AK_IdentityScalar",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityScalar.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityScalar.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityScalar.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityScalar.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityScalar.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityTwoScalarPartComposite),
                        ClrType: typeof(IdentityTwoScalarPartComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityTwoScalarPartComposite",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id1)
                                    ),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id2)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityThreeScalarPartComposite),
                        ClrType: typeof(IdentityThreeScalarPartComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityThreeScalarPartComposite",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id1)
                                    ),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id2)
                                    ),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id3)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id3),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id3),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNested),
                        ClrType: typeof(IdentityNested),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedPart",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNested.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNested.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityNested.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNestedComposite),
                        ClrType: typeof(IdentityNestedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedComposite",
                                Parts:
                                [
                                    new ApiIdentityNestedPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.NestedPart)
                                    ),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.NestedPart),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.NestedPart),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwner),
                        ClrType: typeof(IdentityOwner),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwner",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwner.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityOwner.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Dependent),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Dependent),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedComposite),
                        ClrType: typeof(IdentityOwnedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedComposite",
                                Parts:
                                [
                                    new ApiIdentityOwnerPartDef(),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedDependent),
                        ClrType: typeof(IdentityOwnedDependent),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedDependent",
                                Parts:
                                [
                                    new ApiIdentityOwnerPartDef()
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedDependent.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedDependent.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            ),
            Json = @"
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} With Explicit ApiIdentityName)",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} With Explicit ApiIdentityName)",
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
                        ApiName: nameof(IdentityNested),
                        ClrType: typeof(IdentityNested),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedPart",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNested.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNested.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityNested.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNestedComposite),
                        ClrType: typeof(IdentityNestedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedComposite",
                                Parts:
                                [
                                    new ApiIdentityNestedPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.NestedPart),
                                        ApiIdentityName: "PK_IdentityNestedPart"
                                    ),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.NestedPart),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.NestedPart),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            ),
            Json = @"
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
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} With Explicit ApiIdentityName)",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} With Explicit ApiIdentityName)",
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
                        ApiName: nameof(IdentityOwner),
                        ClrType: typeof(IdentityOwner),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwner",
                                Parts:
                                [
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwner.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityOwner.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Dependent),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Dependent),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedComposite),
                        ClrType: typeof(IdentityOwnedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedComposite",
                                Parts:
                                [
                                    new ApiIdentityOwnerPartDef(ApiIdentityName: "PK_IdentityOwner"),
                                    new ApiIdentityScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedDependent),
                        ClrType: typeof(IdentityOwnedDependent),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedDependent",
                                Parts:
                                [
                                    new ApiIdentityOwnerPartDef(ApiIdentityName: "PK_IdentityOwner")
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedDependent.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedDependent.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            ),
            Json = @"
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
}
