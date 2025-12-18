// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;

using static Evoogle.XUnit.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema;

public class ApiTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Fields
    private static readonly List<string> _excludeMembers = [$"{nameof(ApiSchemaElement.ApiPath)}"];
    #endregion

    #region Test Classes
    public class ApiTypeJsonDeserializeTest : JsonDeserializeTest<ApiType>
    {
        public ApiTypeJsonDeserializeTest()
        {
            this.ExcludeMembers = _excludeMembers;
        }
    }

    public class ApiTypeJsonRoundtripTest : JsonRoundtripTest<ApiType>
    {
        public ApiTypeJsonRoundtripTest()
        {
            this.ExcludeMembers = _excludeMembers;
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        // Null
        new ApiTypeJsonDeserializeTest
        {
            Name = "Null",
            Source = "null",
            Expected = null
        },

        // ApiScalarType
        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [Boolean]",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType(apiName: "Boolean", clrScalarType: typeof(bool))
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [Boolean] With Extension 1",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            Expected = new ApiScalarType(apiName: "Boolean", clrScalarType: typeof(bool)),
            ExtensionType1 = typeof(TestExtension1)
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [Boolean] With Extra Property",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Unexpected"": ""IgnoreMe""
            }",
            Expected = new ApiScalarType(apiName: "Boolean", clrScalarType: typeof(bool))
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [ID]",
            Source =  @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""ID"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType(apiName: "ID", clrScalarType: typeof(string))
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [Int]",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Int"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType(apiName: "Int", clrScalarType: typeof(int))
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [Float]",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Float"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType(apiName: "Float", clrScalarType: typeof(float))
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [String]",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType(apiName: "String", clrScalarType: typeof(string))
        },

        // ApiEnumType
        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}]",
            Source = @"
            {
                ""Kind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiEnumType
            (
                apiName: nameof(StopLight),
                apiEnumValues:
                [
                    new ApiEnumValue(apiName: "None", clrName: "None", clrOrdinal: 0),
                    new ApiEnumValue(apiName: "Green", clrName: "Green", clrOrdinal: 1),
                    new ApiEnumValue(apiName: "Yellow", clrName: "Yellow", clrOrdinal: 2),
                    new ApiEnumValue(apiName: "Red", clrName: "Red", clrOrdinal: 3)
                ],
                clrEnumType: typeof(StopLight)
            )
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}] With Extension 2",
            Source = @"
            {
                ""Kind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            Expected = new ApiEnumType
            (
                apiName: nameof(StopLight),
                apiEnumValues:
                [
                    new ApiEnumValue(apiName: "None", clrName: "None", clrOrdinal: 0),
                    new ApiEnumValue(apiName: "Green", clrName: "Green", clrOrdinal: 1),
                    new ApiEnumValue(apiName: "Yellow", clrName: "Yellow", clrOrdinal: 2),
                    new ApiEnumValue(apiName: "Red", clrName: "Red", clrOrdinal: 3)
                ],
                clrEnumType: typeof(StopLight)
            ),
            ExtensionType2 = typeof(TestExtension2)
        },

        // ApiCollectionType
        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}>]",
            Source = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Scalar"",
                    ""ApiName"": ""String""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }",
            Expected = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                apiItemTypeModifiers: ApiTypeModifiers.Required,
                clrCollectionType: typeof(List<string>)
            )
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}?>]",
            Source = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Scalar"",
                    ""ApiName"": ""String""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }",
            Expected = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                apiItemTypeModifiers: ApiTypeModifiers.None,
                clrCollectionType: typeof(List<string?>)
            )
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}>]",
            Source = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }",
            Expected = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                apiItemTypeModifiers: ApiTypeModifiers.Required,
                clrCollectionType: typeof(List<StopLight>)
            )
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}?>]",
            Source = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }",
            Expected = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                apiItemTypeModifiers: ApiTypeModifiers.None,
                clrCollectionType: typeof(List<StopLight>)
            )
        },

        // ApiObjectType With API Named Typed Expressions
        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships:[],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            )
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And With API Options",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships:[],
                apiIdentitySet: null,
                apiOptions: new ApiObjectTypeOptions
                {
                    ApiIdentityNullHandling = ApiIdentityNullHandling.ThrowException
                },
                clrObjectType: typeof(ScalarsOnly)
            )
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions With Extension 1 and Extension 2",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships: [],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            ),
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2)
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With API Named Typed Expressions",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""Company"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Name"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""Name""
                    },
                    {
                        ""ApiName"": ""Owner"",
                        ""ApiType"": {
                            ""Kind"": ""Object"",
                            ""ApiName"": ""Person""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees"",
                        ""ApiType"": {
                            ""ApiInlineType"": {
                                ""Kind"": ""Collection"",
                                ""ApiItemType"": {
                                    ""Kind"": ""Object"",
                                    ""ApiName"": ""Person""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                            }
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Employees""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiName"": ""OwnedBy"",
                        ""ApiPropertyName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiObjectType
            (
                apiName: nameof(Company),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(Company.Name),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Owner),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Employees),
                        apiTypeExpression: new ApiTypeExpression
                        (
                            apiInlineType: new ApiCollectionType
                            (
                                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                apiItemTypeModifiers: ApiTypeModifiers.Required,
                                clrCollectionType: typeof(List<Person>)
                            )
                        ),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Employees)
                    ),
                ],
                apiRelationships:
                [
                    new ApiRelationship(apiName: "OwnedBy", apiPropertyName: nameof(Company.Owner)),
                    new ApiRelationship(apiName: nameof(Company.Employees))
                ],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(Company)
            )
        },

        // ApiObjectType With CLR Typed Expressions
        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships:[],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            )
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions With Extension 1 and Extension 2",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships: [],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            ),
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2)
        },

        new ApiTypeJsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With CLR Typed Expressions",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""Company"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Name"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""Name""
                    },
                    {
                        ""ApiName"": ""Owner"",
                        ""ApiType"": {
                            ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees"",
                        ""ApiType"": {
                            ""ApiInlineType"": {
                                ""Kind"": ""Collection"",
                                ""ApiItemType"": {
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                            }
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Employees""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiName"": ""OwnedBy"",
                        ""ApiPropertyName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiObjectType
            (
                apiName: nameof(Company),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(Company.Name),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Owner),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Employees),
                        apiTypeExpression: new ApiTypeExpression
                        (
                            apiInlineType: new ApiCollectionType
                            (
                                apiItemTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                                apiItemTypeModifiers: ApiTypeModifiers.Required,
                                clrCollectionType: typeof(List<Person>)
                            )
                        ),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Employees)
                    ),
                ],
                apiRelationships:
                [
                    new ApiRelationship(apiName: "OwnedBy", apiPropertyName: nameof(Company.Owner)),
                    new ApiRelationship(apiName: nameof(Company.Employees))
                ],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(Company)
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        // Null
        new ApiTypeJsonRoundtripTest
        {
            Name = "Null",
            Expected = null
        },

        // ApiScalarType
        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [Boolean]",
            Expected = new ApiScalarType(apiName: "Boolean", clrScalarType: typeof(bool))
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [Boolean] With Extension 1",
            Expected = new ApiScalarType(apiName: "Boolean", clrScalarType: typeof(bool)),
            ExtensionType1 = typeof(TestExtension1)
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [ID]",
            Expected = new ApiScalarType(apiName: "ID", clrScalarType: typeof(string))
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [Int]",
            Expected = new ApiScalarType(apiName: "Int", clrScalarType: typeof(int))
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [Float]",
            Expected = new ApiScalarType(apiName: "Float", clrScalarType: typeof(float))
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [String]",
            Expected = new ApiScalarType(apiName: "String", clrScalarType: typeof(string))
        },

        // ApiEnumType
        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}]",
            Expected = new ApiEnumType
            (
                apiName: nameof(StopLight),
                apiEnumValues:
                [
                    new ApiEnumValue(apiName: "None", clrName: "None", clrOrdinal: 0),
                    new ApiEnumValue(apiName: "Green", clrName: "Green", clrOrdinal: 1),
                    new ApiEnumValue(apiName: "Yellow", clrName: "Yellow", clrOrdinal: 2),
                    new ApiEnumValue(apiName: "Red", clrName: "Red", clrOrdinal: 3)
                ],
                clrEnumType: typeof(StopLight)
            )
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}] With Extension 2",
            Expected = new ApiEnumType
            (
                apiName: nameof(StopLight),
                apiEnumValues:
                [
                    new ApiEnumValue(apiName: "None", clrName: "None", clrOrdinal: 0),
                    new ApiEnumValue(apiName: "Green", clrName: "Green", clrOrdinal: 1),
                    new ApiEnumValue(apiName: "Yellow", clrName: "Yellow", clrOrdinal: 2),
                    new ApiEnumValue(apiName: "Red", clrName: "Red", clrOrdinal: 3)
                ],
                clrEnumType: typeof(StopLight)
            ),
            ExtensionType2 = typeof(TestExtension2)
        },

        // ApiCollectionType
        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}>]",
            Expected = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                apiItemTypeModifiers: ApiTypeModifiers.Required,
                clrCollectionType: typeof(List<string>)
            )
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}?>]",
            Expected = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                apiItemTypeModifiers: ApiTypeModifiers.None,
                clrCollectionType: typeof(List<string?>)
            )
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}>]",
            Expected = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                apiItemTypeModifiers: ApiTypeModifiers.Required,
                clrCollectionType: typeof(List<StopLight>)
            )
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}?>]",
            Expected = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                apiItemTypeModifiers: ApiTypeModifiers.None,
                clrCollectionType: typeof(List<StopLight>)
            )
        },

        // ApiObjectType With API Named Typed Expressions
        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships:[],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            )
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And With API Options",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships:[],
                apiIdentitySet: null,
                apiOptions: new ApiObjectTypeOptions
                {
                    ApiIdentityNullHandling = ApiIdentityNullHandling.ThrowException
                },
                clrObjectType: typeof(ScalarsOnly)
            )
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions With Extension 1 and Extension 2",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships: [],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            ),
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2)
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With API Named Typed Expressions",
            Expected = new ApiObjectType
            (
                apiName: nameof(Company),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(Company.Name),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Owner),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Employees),
                        apiTypeExpression: new ApiTypeExpression
                        (
                            apiInlineType: new ApiCollectionType
                            (
                                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                apiItemTypeModifiers: ApiTypeModifiers.Required,
                                clrCollectionType: typeof(List<Person>)
                            )
                        ),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Employees)
                    ),
                ],
                apiRelationships:
                [
                    new ApiRelationship(apiName: "OwnedBy", apiPropertyName: nameof(Company.Owner)),
                    new ApiRelationship(apiName: nameof(Company.Employees))
                ],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(Company)
            )
        },

        // ApiObjectType With CLR Typed Expressions
        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships:[],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            )
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions With Extension 1 and Extension 2",
            Expected = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships: [],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            ),
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2)
        },

        new ApiTypeJsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With CLR Typed Expressions",
            Expected = new ApiObjectType
            (
                apiName: nameof(Company),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(Company.Name),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Owner),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Employees),
                        apiTypeExpression: new ApiTypeExpression
                        (
                            new ApiCollectionType
                            (
                                apiItemTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                                apiItemTypeModifiers: ApiTypeModifiers.Required,
                                clrCollectionType: typeof(List<Person>)
                            )
                        ),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Employees)
                    ),
                ],
                apiRelationships:
                [
                    new ApiRelationship(apiName: "OwnedBy", apiPropertyName: nameof(Company.Owner)),
                    new ApiRelationship(apiName: nameof(Company.Employees))
                ],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(Company)
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        // Null
        new JsonSerializeTest<ApiType>
        {
            Name = "Null",
            Source = null,
            Expected = "null"
        },

        // ApiScalarType
        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiScalarType)} [Boolean]",
            Source = new ApiScalarType(apiName: "Boolean", clrScalarType: typeof(bool)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiScalarType)} [Boolean] With Extension 1",
            Source = new ApiScalarType(apiName: "Boolean", clrScalarType: typeof(bool)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            ExtensionType1 = typeof(TestExtension1)
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiScalarType)} [ID]",
            Source = new ApiScalarType(apiName: "ID", clrScalarType: typeof(string)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""ID"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiScalarType)} [Int]",
            Source = new ApiScalarType(apiName: "Int", clrScalarType: typeof(int)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Int"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiScalarType)} [Float]",
            Source = new ApiScalarType(apiName: "Float", clrScalarType: typeof(float)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Float"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiScalarType)} [String]",
            Source = new ApiScalarType(apiName: "String", clrScalarType: typeof(string)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },

        // ApiEnumType
        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}]",
            Source = new ApiEnumType
            (
                apiName: nameof(StopLight),
                apiEnumValues:
                [
                    new ApiEnumValue(apiName: "None", clrName: "None", clrOrdinal: 0),
                    new ApiEnumValue(apiName: "Green", clrName: "Green", clrOrdinal: 1),
                    new ApiEnumValue(apiName: "Yellow", clrName: "Yellow", clrOrdinal: 2),
                    new ApiEnumValue(apiName: "Red", clrName: "Red", clrOrdinal: 3)
                ],
                clrEnumType: typeof(StopLight)
            ),
            Expected = @"
            {
                ""Kind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}] With Extension 2",
            Source = new ApiEnumType
            (
                apiName: nameof(StopLight),
                apiEnumValues:
                [
                    new ApiEnumValue(apiName: "None", clrName: "None", clrOrdinal: 0),
                    new ApiEnumValue(apiName: "Green", clrName: "Green", clrOrdinal: 1),
                    new ApiEnumValue(apiName: "Yellow", clrName: "Yellow", clrOrdinal: 2),
                    new ApiEnumValue(apiName: "Red", clrName: "Red", clrOrdinal: 3)
                ],
                clrEnumType: typeof(StopLight)
            ),
            Expected = @"
            {
                ""Kind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            ExtensionType2 = typeof(TestExtension2)
        },

        // ApiCollectionType
        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiCollectionType)} [List<String>]",
            Source = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                apiItemTypeModifiers: ApiTypeModifiers.Required,
                clrCollectionType: typeof(List<string>)
            ),
            Expected = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Scalar"",
                    ""ApiName"": ""String""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiCollectionType)} [List<String?>]",
            Source = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                apiItemTypeModifiers: ApiTypeModifiers.None,
                clrCollectionType: typeof(List<string?>)
            ),
            Expected = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Scalar"",
                    ""ApiName"": ""String""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}>]",
            Source = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                apiItemTypeModifiers: ApiTypeModifiers.Required,
                clrCollectionType: typeof(List<StopLight>)
            ),
            Expected = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}?>]",
            Source = new ApiCollectionType
            (
                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                apiItemTypeModifiers: ApiTypeModifiers.None,
                clrCollectionType: typeof(List<StopLight>)
            ),
            Expected = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }"
        },

        // ApiObjectType With API Named Typed Expressions
        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions",
            Source = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships:[],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And With API Options",
            Source = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships:[],
                apiIdentitySet: null,
                apiOptions: new ApiObjectTypeOptions
                {
                    ApiIdentityNullHandling = ApiIdentityNullHandling.ThrowException
                },
                clrObjectType: typeof(ScalarsOnly)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And With Extension 1 and Extension 2",
            Source = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Long"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "Boolean"),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships: [],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2)
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With API Named Typed Expressions",
            Source = new ApiObjectType
            (
                apiName: nameof(Company),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(Company.Name),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Owner),
                        apiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Employees),
                        apiTypeExpression: new ApiTypeExpression
                        (
                            apiInlineType: new ApiCollectionType
                            (
                                apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                apiItemTypeModifiers: ApiTypeModifiers.Required,
                                clrCollectionType: typeof(List<Person>)
                            )
                        ),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Employees)
                    ),
                ],
                apiRelationships:
                [
                    new ApiRelationship(apiName: "OwnedBy", apiPropertyName: nameof(Company.Owner)),
                    new ApiRelationship(apiName: nameof(Company.Employees))
                ],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(Company)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""Company"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Name"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""Name""
                    },
                    {
                        ""ApiName"": ""Owner"",
                        ""ApiType"": {
                            ""Kind"": ""Object"",
                            ""ApiName"": ""Person""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees"",
                        ""ApiType"": {
                            ""ApiInlineType"": {
                                ""Kind"": ""Collection"",
                                ""ApiItemType"": {
                                    ""Kind"": ""Object"",
                                    ""ApiName"": ""Person""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                            }
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Employees""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiName"": ""OwnedBy"",
                        ""ApiPropertyName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        // ApiObjectType With CLR Typed Expressions
        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions",
            Source = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships:[],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And With Extension 1 and Extension 2",
            Source = new ApiObjectType
            (
                apiName: nameof(ScalarsOnly),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.RequiredPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(ScalarsOnly.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalName),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalName)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalNumber),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(ScalarsOnly.OptionalPredicate),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(ScalarsOnly.OptionalPredicate)
                    ),
                ],
                apiRelationships: [],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(ScalarsOnly)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2)
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With CLR Typed Expressions",
            Source = new ApiObjectType
            (
                apiName: nameof(Company),
                apiProperties:
                [
                    new ApiProperty
                    (
                        apiName: nameof(Company.Name),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        apiTypeModifiers: ApiTypeModifiers.Required,
                        clrName: nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Owner),
                        apiTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        apiName: nameof(Company.Employees),
                        apiTypeExpression: new ApiTypeExpression
                        (
                            apiInlineType: new ApiCollectionType
                            (
                                apiItemTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                                apiItemTypeModifiers: ApiTypeModifiers.Required,
                                clrCollectionType: typeof(List<Person>)
                            )
                        ),
                        apiTypeModifiers: ApiTypeModifiers.None,
                        clrName: nameof(Company.Employees)
                    ),
                ],
                apiRelationships:
                [
                    new ApiRelationship(apiName: "OwnedBy", apiPropertyName: nameof(Company.Owner)),
                    new ApiRelationship(apiName: nameof(Company.Employees))
                ],
                apiIdentitySet: null,
                apiOptions: null,
                clrObjectType: typeof(Company)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""Company"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Name"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""Name""
                    },
                    {
                        ""ApiName"": ""Owner"",
                        ""ApiType"": {
                            ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees"",
                        ""ApiType"": {
                            ""ApiInlineType"": {
                                ""Kind"": ""Collection"",
                                ""ApiItemType"": {
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                            }
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Employees""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiName"": ""OwnedBy"",
                        ""ApiPropertyName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
            }"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeTheoryData))]
    public void JsonDeserialize(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtrip(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonSerializeTheoryData))]
    public void JsonSerialize(IXUnitTest test) => test.Execute(this);
    #endregion
}
