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
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ApiRequiresArgument: false
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
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ApiRequiresArgument: true
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
                                ApiItemTypeModifiers: ApiTypeModifiers.Required
                            ),
                            ApiTypeReferenceDef: null
                        ),
                        ApiOutputTypeModifiers: ApiTypeModifiers.Required,
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
                                "ApiTypeModifiers": "None",
                                "ApiRequiresArgument": false
                            },
                            {
                                "ApiName": "limit",
                                "ApiType": {
                                    "ApiKind": "Scalar",
                                    "ApiName": "Int32"
                                },
                                "ApiTypeModifiers": "Required",
                                "ApiRequiresArgument": true
                            }
                        ],
                        "ApiOutputType": {
                            "ApiKind": "Collection",
                            "ApiItemType": {
                                "ApiKind": "Scalar",
                                "ApiName": "String"
                            },
                            "ApiItemTypeModifiers": "Required",
                            "ClrType": "System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib"
                        },
                        "ApiOutputTypeModifiers": "Required",
                        "ApiOutputComposition": "Projection, Filtering, Ordering, OffsetPagination"
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
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ApiRequiresArgument: true
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
                        ApiOutputTypeModifiers: ApiTypeModifiers.None,
                        ApiOutputComposition: ApiQueryComposition.Projection
                    )
                ]
            ),
            Json = """
            {
                "ApiName": "QuerySourceDefaults",
                "ApiScalarTypes": [
                    {
                        "ApiKind": "Scalar",
                        "ApiName": "String",
                        "ClrType": "System.String, System.Private.CoreLib"
                    }
                ],
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
