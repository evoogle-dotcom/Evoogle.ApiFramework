// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Query;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Test Data
    private static ApiSchemaJsonTestCase[] QuerySourceJsonTestCases =>
    [
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With {nameof(ApiQuerySource)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With {nameof(ApiQuerySource)}",
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
                    )
                ],
                ApiQuerySources:
                [
                    new ApiQuerySourceDef
                    (
                        ApiName: "SearchNames",
                        ApiInputParameters:
                        [
                            new ApiQuerySourceParameterDef
                            (
                                ApiName: "searchTerm",
                                ApiTypeExpression: new ApiTypeExpressionDef
                                (
                                    ApiTypeDef: null,
                                    ApiTypeReferenceDef: new ApiTypeReferenceDef
                                    (
                                        ApiKind: ApiTypeKind.Scalar,
                                        ApiName: nameof(String),
                                        ClrType: null
                                    )
                                ),
                                ApiNullability: ApiNullability.Nullable,
                                ApiPresence: ApiPresence.Optional
                            ),
                            new ApiQuerySourceParameterDef
                            (
                                ApiName: "limit",
                                ApiTypeExpression: new ApiTypeExpressionDef
                                (
                                    ApiTypeDef: null,
                                    ApiTypeReferenceDef: new ApiTypeReferenceDef
                                    (
                                        ApiKind: ApiTypeKind.Scalar,
                                        ApiName: nameof(Int32),
                                        ClrType: null
                                    )
                                ),
                                ApiNullability: ApiNullability.NonNullable,
                                ApiPresence: ApiPresence.Required
                            )
                        ],
                        ApiOutputTypeExpression: new ApiTypeExpressionDef
                        (
                            ApiTypeDef: new ApiCollectionTypeDef
                            (
                                ClrType: typeof(List<string>),
                                ApiItemTypeExpression: new ApiTypeExpressionDef
                                (
                                    ApiTypeDef: null,
                                    ApiTypeReferenceDef: new ApiTypeReferenceDef
                                    (
                                        ApiKind: ApiTypeKind.Scalar,
                                        ApiName: nameof(String),
                                        ClrType: null
                                    )
                                ),
                                ApiItemNullability: ApiNullability.NonNullable
                            ),
                            ApiTypeReferenceDef: null
                        ),
                        ApiOutputNullability: ApiNullability.NonNullable,
                        ApiOutputComposition:
                            ApiQueryComposition.Projection
                            | ApiQueryComposition.Filtering
                            | ApiQueryComposition.Ordering
                            | ApiQueryComposition.OffsetPagination
                    )
                ]
            ),
            Json = """
            {
                "ApiName": "ApiSchema With ApiQuerySource",
                "ApiVersion": "0.1.0",
                "ApiOptions": {
                    "ApiKeyNullHandling": "UseEmptyOnNull"
                },
                "ApiScalarTypes": [
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "Int32",
                        "ClrType": "System.Int32, System.Private.CoreLib"
                    },
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "String",
                        "ClrType": "System.String, System.Private.CoreLib"
                    }
                ],
                "ApiEnumTypes": [],
                "ApiObjectTypes": [],
                "ApiRelationships": [],
                "ApiQuerySources": [
                    {
                        "ApiName": "SearchNames",
                        "ApiInputParameters": [
                            {
                                "ApiName": "searchTerm",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "String"
                                },
                                "ApiNullability": "Nullable",
                                "ApiPresence": "Optional"
                            },
                            {
                                "ApiName": "limit",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Int32"
                                },
                                "ApiNullability": "NonNullable",
                                "ApiPresence": "Required"
                            }
                        ],
                        "ApiOutputType": {
                            "ApiKind": "Collection",
                            "ApiItemType": {
                                "ApiKind": "Scalar",
                                "ApiName": "String"
                            },
                            "ApiItemNullability": "NonNullable",
                            "ClrType": "System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib"
                        },
                        "ApiOutputNullability": "NonNullable",
                        "ApiOutputComposition": "Projection, Filtering, Ordering, OffsetPagination"
                    }
                ]
            }
            """
        },
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With scalar {nameof(ApiQuerySource)} output and no parameters",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: "ScalarQuerySource",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    )
                ],
                ApiQuerySources:
                [
                    new ApiQuerySourceDef
                    (
                        ApiName: "CurrentName",
                        ApiInputParameters: [],
                        ApiOutputTypeExpression: new ApiTypeExpressionDef
                        (
                            ApiTypeDef: null,
                            ApiTypeReferenceDef: new ApiTypeReferenceDef
                            (
                                ApiKind: ApiTypeKind.Scalar,
                                ApiName: nameof(String),
                                ClrType: null
                            )
                        ),
                        ApiOutputNullability: ApiNullability.Nullable,
                        ApiOutputComposition: ApiQueryComposition.Projection
                    )
                ]
            ),
            Json = """
            {
                "ApiName": "ScalarQuerySource",
                "ApiVersion": "0.1.0",
                "ApiOptions": {
                    "ApiKeyNullHandling": "UseEmptyOnNull"
                },
                "ApiScalarTypes": [
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "String",
                        "ClrType": "System.String, System.Private.CoreLib"
                    }
                ],
                "ApiEnumTypes": [],
                "ApiObjectTypes": [],
                "ApiRelationships": [],
                "ApiQuerySources": [
                    {
                        "ApiName": "CurrentName",
                        "ApiInputParameters": [],
                        "ApiOutputType": {
                            "ApiKind": "Scalar",
                            "ApiName": "String"
                        },
                        "ApiOutputNullability": "Nullable",
                        "ApiOutputComposition": "Projection"
                    }
                ]
            }
            """
        },
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With collection {nameof(ApiQuerySourceParameter)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: "CollectionQuerySourceParameter",
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
                    )
                ],
                ApiQuerySources:
                [
                    new ApiQuerySourceDef
                    (
                        ApiName: "NamesByIds",
                        ApiInputParameters:
                        [
                            new ApiQuerySourceParameterDef
                            (
                                ApiName: "ids",
                                ApiTypeExpression: new ApiTypeExpressionDef
                                (
                                    ApiTypeDef: new ApiCollectionTypeDef
                                    (
                                        ClrType: typeof(int[]),
                                        ApiItemTypeExpression: new ApiTypeExpressionDef
                                        (
                                            ApiTypeDef: null,
                                            ApiTypeReferenceDef: new ApiTypeReferenceDef
                                            (
                                                ApiKind: ApiTypeKind.Scalar,
                                                ApiName: nameof(Int32),
                                                ClrType: null
                                            )
                                        ),
                                        ApiItemNullability: ApiNullability.NonNullable
                                    ),
                                    ApiTypeReferenceDef: null
                                ),
                                ApiNullability: ApiNullability.NonNullable,
                                ApiPresence: ApiPresence.Required
                            )
                        ],
                        ApiOutputTypeExpression: new ApiTypeExpressionDef
                        (
                            ApiTypeDef: null,
                            ApiTypeReferenceDef: new ApiTypeReferenceDef
                            (
                                ApiKind: ApiTypeKind.Scalar,
                                ApiName: nameof(String),
                                ClrType: null
                            )
                        ),
                        ApiOutputNullability: ApiNullability.Nullable,
                        ApiOutputComposition: ApiQueryComposition.Projection
                    )
                ]
            ),
            Json = """
            {
                "ApiName": "CollectionQuerySourceParameter",
                "ApiVersion": "0.1.0",
                "ApiOptions": {
                    "ApiKeyNullHandling": "UseEmptyOnNull"
                },
                "ApiScalarTypes": [
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "Int32",
                        "ClrType": "System.Int32, System.Private.CoreLib"
                    },
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "String",
                        "ClrType": "System.String, System.Private.CoreLib"
                    }
                ],
                "ApiEnumTypes": [],
                "ApiObjectTypes": [],
                "ApiRelationships": [],
                "ApiQuerySources": [
                    {
                        "ApiName": "NamesByIds",
                        "ApiInputParameters": [
                            {
                                "ApiName": "ids",
                                "ApiType": {
                                    "ApiKind": "Collection",
                                    "ApiItemType": {
                                        "ApiKind": "Scalar",
                                        "ApiName": "Int32"
                                    },
                                    "ApiItemNullability": "NonNullable",
                                    "ClrType": "System.Int32[], System.Private.CoreLib"
                                },
                                "ApiNullability": "NonNullable",
                                "ApiPresence": "Required"
                            }
                        ],
                        "ApiOutputType": {
                            "ApiKind": "Scalar",
                            "ApiName": "String"
                        },
                        "ApiOutputNullability": "Nullable",
                        "ApiOutputComposition": "Projection"
                    }
                ]
            }
            """
        }
    ];

    private static ApiSchemaJsonTestCase[] QuerySourceDefaultJsonTestCases =>
    [
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With omitted {nameof(ApiQuerySource)} defaults",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: "QuerySourceDefaults",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    )
                ],
                ApiQuerySources:
                [
                    new ApiQuerySourceDef
                    (
                        ApiName: "Echo",
                        ApiInputParameters:
                        [
                            new ApiQuerySourceParameterDef
                            (
                                ApiName: "text",
                                ApiTypeExpression: new ApiTypeExpressionDef
                                (
                                    ApiTypeDef: null,
                                    ApiTypeReferenceDef: new ApiTypeReferenceDef
                                    (
                                        ApiKind: ApiTypeKind.Scalar,
                                        ApiName: nameof(String),
                                        ClrType: null
                                    )
                                ),
                                ApiNullability: ApiNullability.NonNullable,
                                ApiPresence: ApiPresence.Required
                            )
                        ],
                        ApiOutputTypeExpression: new ApiTypeExpressionDef
                        (
                            ApiTypeDef: null,
                            ApiTypeReferenceDef: new ApiTypeReferenceDef
                            (
                                ApiKind: ApiTypeKind.Scalar,
                                ApiName: nameof(String),
                                ClrType: null
                            )
                        ),
                        ApiOutputNullability: ApiNullability.Nullable,
                        ApiOutputComposition: ApiQueryComposition.Projection
                    )
                ]
            ),
            Json = """
            {
                "ApiName": "QuerySourceDefaults",
                "ApiOptions": {},
                "ApiScalarTypes": [
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "String",
                        "ClrType": "System.String, System.Private.CoreLib"
                    }
                ],
                "ApiEnumTypes": [],
                "ApiObjectTypes": [],
                "ApiRelationships": [],
                "ApiQuerySources": [
                    {
                        "ApiName": "Echo",
                        "ApiInputParameters": [
                            {
                                "ApiName": "text",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "String"
                                }
                            }
                        ],
                        "ApiOutputType": {
                            "ApiKind": "Scalar",
                            "ApiName": "String"
                        }
                    }
                ]
            }
            """
        }
    ];
    #endregion
}
