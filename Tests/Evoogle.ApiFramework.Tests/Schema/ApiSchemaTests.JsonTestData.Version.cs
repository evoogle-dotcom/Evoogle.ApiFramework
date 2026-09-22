// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Version;
using Evoogle.ApiFramework.TestData;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Test Data
    private static ApiSchemaJsonTestCase[] VersionJsonTestCases =>
    [
        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Property Backed {nameof(ApiVersionDefinition)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Property Backed {nameof(ApiVersionDefinition)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
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
                            )
                        ],
                        ApiVersion: new ApiVersionDefinitionDef(new ApiPropertyReferenceDef(ApiName: null, ClrName: nameof(Person.Id)))
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Property Backed ApiVersionDefinition"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
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
                            }
                        ],
                        ""ApiKeys"": [],
                        ""ApiVersion"": {
                            ""ApiPropertyReference"": { ""ClrName"": ""Id"" }
                        },
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        },

        new ApiSchemaJsonTestCase
        {
            Name = $"{nameof(ApiSchema)} With Repository Backed {nameof(ApiVersionDefinition)}",
            FactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Repository Backed {nameof(ApiVersionDefinition)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(Empty),
                        ClrType: typeof(Empty),
                        ApiVersion: new ApiVersionDefinitionDef(typeof(int))
                    )
                ]
            ),
            Json = @"
            {
                ""ApiName"": ""ApiSchema With Repository Backed ApiVersionDefinition"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Empty"",
                        ""ApiProperties"": [],
                        ""ApiKeys"": [],
                        ""ApiVersion"": {
                            ""ClrType"": ""System.Int32, System.Private.CoreLib""
                        },
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Empty, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }"
        }
    ];
    #endregion
}
