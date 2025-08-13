// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests;

namespace Evoogle.ApiFramework.Schema;

public class ApiTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        // Null
        new JsonDeserializeTest<ApiType>
        {
            Name = "Null",
            Source = "null",
            Expected = null
        },

        // ApiScalarType
        new JsonDeserializeTest<ApiType>
        {
            Name = "ApiScalarType [Boolean]",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType("Boolean", typeof(bool))
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = "ApiScalarType [Boolean] With Extension 1",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            Expected = new ApiScalarType("Boolean", typeof(bool)),
            AddTestExtension1 = true
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = "ApiScalarType [Boolean] With Extra Property",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Unexpected"": ""IgnoreMe""
            }",
            Expected = new ApiScalarType("Boolean", typeof(bool))
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = "ApiScalarType [ID]",
            Source =  @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""ID"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType("ID", typeof(string))
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = "ApiScalarType [Int]",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Int"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType("Int", typeof(int))
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = "ApiScalarType [Float]",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Float"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType("Float", typeof(float))
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = "ApiScalarType [String]",
            Source = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }",
            Expected = new ApiScalarType("String", typeof(string))
        },

        // ApiEnumType
        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiEnumType [{nameof(StopLight)}]",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            )
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiEnumType [{nameof(StopLight)}] With Extension 2",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            Expected = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            ),
            AddTestExtension2 = true
        },

        // ApiCollectionType
        new JsonDeserializeTest<ApiType>
        {
            Name = "ApiCollectionType [List<String>]",
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
            Expected = new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Scalar, "String"), ApiTypeModifiers.Required, typeof(List<string>))
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = "ApiCollectionType [List<String?>]",
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
            Expected = new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Scalar, "String"), ApiTypeModifiers.None, typeof(List<string?>))
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}>]",
            Source = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }",
            Expected = new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Enum, nameof(StopLight)), ApiTypeModifiers.Required, typeof(List<StopLight>))
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}?>]",
            Source = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }",
            Expected = new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Enum, nameof(StopLight)), ApiTypeModifiers.None, typeof(List<StopLight>))
        },

        // ApiObjectType With API Named Typed Expressions
        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With API Named Typed Expressions",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            )
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With API Named Typed Expressions With Extension 1 and Extension 2",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    },
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            Expected = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            ),
            AddTestExtension1 = true,
            AddTestExtension2 = true
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(Company)}] With API Named Typed Expressions",
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
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BCompany, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiObjectType
            (
                nameof(Company),
                [
                    new ApiProperty
                    (
                        nameof(Company.Name),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Owner),
                        new ApiTypeExpression(ApiTypeKind.Object, nameof(Person)),
                        ApiTypeModifiers.None,
                        nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Employees),
                        new ApiTypeExpression(new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Object, nameof(Person)), ApiTypeModifiers.Required, typeof(List<Person>))),
                        ApiTypeModifiers.None,
                        nameof(Company.Employees)
                    ),
                ],
                [
                    new ApiRelationship("OwnedBy", nameof(Company.Owner)),
                    new ApiRelationship(nameof(Company.Employees))
                ],
                typeof(Company)
            ),
        },

        // ApiObjectType With CLR Typed Expressions
        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With CLR Typed Expressions",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            )
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With CLR Typed Expressions With Extension 1 and Extension 2",
            Source = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    },
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            Expected = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            ),
            AddTestExtension1 = true,
            AddTestExtension2 = true
        },

        new JsonDeserializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(Company)}] With CLR Typed Expressions",
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
                            ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson,Evoogle.ApiFramework.Core.Tests""
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
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson,Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BCompany, Evoogle.ApiFramework.Core.Tests""
            }",
            Expected = new ApiObjectType
            (
                nameof(Company),
                [
                    new ApiProperty
                    (
                        nameof(Company.Name),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Owner),
                        new ApiTypeExpression(ApiTypeKind.Object, nameof(Person)),
                        ApiTypeModifiers.None,
                        nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Employees),
                        new ApiTypeExpression(new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Object, nameof(Person)), ApiTypeModifiers.Required, typeof(List<Person>))),
                        ApiTypeModifiers.None,
                        nameof(Company.Employees)
                    ),
                ],
                [
                    new ApiRelationship("OwnedBy", nameof(Company.Owner)),
                    new ApiRelationship(nameof(Company.Employees))
                ],
                typeof(Company)
            ),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        // Null
        new JsonRoundtripTest<ApiType>
        {
            Name = "Null",
            Expected = null
        },

        // ApiScalarType
        new JsonRoundtripTest<ApiType>
        {
            Name = "ApiScalarType [Boolean]",
            Expected = new ApiScalarType("Boolean", typeof(bool))
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = "ApiScalarType [Boolean] With Extension 1",
            Expected = new ApiScalarType("Boolean", typeof(bool)),
            AddTestExtension1 = true
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = "ApiScalarType [ID]",
            Expected = new ApiScalarType("ID", typeof(string))
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = "ApiScalarType [Int]",
            Expected = new ApiScalarType("Int", typeof(int))
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = "ApiScalarType [Float]",
            Expected = new ApiScalarType("Float", typeof(float))
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = "ApiScalarType [String]",
            Expected = new ApiScalarType("String", typeof(string))
        },

        // ApiEnumType
        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiEnumType [{nameof(StopLight)}]",
            Expected = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            )
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiEnumType [{nameof(StopLight)}] With Extension 2",
            Expected = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            ),
            AddTestExtension2 = true
        },

        // ApiCollectionType
        new JsonRoundtripTest<ApiType>
        {
            Name = "ApiCollectionType [List<String>]",
            Expected = new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Scalar,  "String"), ApiTypeModifiers.Required, typeof(List<string>))
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = "ApiCollectionType [List<String?>]",
            Expected = new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Scalar,  "String"), ApiTypeModifiers.None, typeof(List<string?>))
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}>]",
            Expected = new ApiCollectionType
            (
                new ApiTypeExpression(ApiTypeKind.Enum, nameof(StopLight)),
                ApiTypeModifiers.Required,
                typeof(List<StopLight>)
            )
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}?>]",
            Expected = new ApiCollectionType
            (
                new ApiTypeExpression(ApiTypeKind.Enum, nameof(StopLight)),
                ApiTypeModifiers.None,
                typeof(List<StopLight>)
            )
        },

        // ApiObjectType With API Named Typed Expressions
        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With API Named Typed Expressions",
            Expected = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            )
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With API Named Typed Expressions With Extension 1 and Extension 2",
            Expected = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            ),
            AddTestExtension1 = true,
            AddTestExtension2 = true
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(Company)}] With API Named Typed Expressions",
            Expected = new ApiObjectType
            (
                nameof(Company),
                [
                    new ApiProperty
                    (
                        nameof(Company.Name),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Owner),
                        new ApiTypeExpression(ApiTypeKind.Object, nameof(Person)),
                        ApiTypeModifiers.None,
                        nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Employees),
                        new ApiTypeExpression(new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Object, nameof(Person)), ApiTypeModifiers.Required, typeof(List<Person>))),
                        ApiTypeModifiers.None,
                        nameof(Company.Employees)
                    ),
                ],
                [
                    new ApiRelationship("OwnedBy", nameof(Company.Owner)),
                    new ApiRelationship(nameof(Company.Employees))
                ],
                typeof(Company)
            ),
        },

        // ApiObjectType With CLR Typed Expressions
        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With CLR Typed Expressions",
            Expected = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(typeof(long)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(typeof(bool)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            )
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With CLR Typed Expressions With Extension 1 and Extension 2",
            Expected = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(typeof(long)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(typeof(bool)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            ),
            AddTestExtension1 = true,
            AddTestExtension2 = true
        },

        new JsonRoundtripTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(Company)}] With CLR Typed Expressions",
            Expected = new ApiObjectType
            (
                nameof(Company),
                [
                    new ApiProperty
                    (
                        nameof(Company.Name),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Owner),
                        new ApiTypeExpression(typeof(Person)),
                        ApiTypeModifiers.None,
                        nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Employees),
                        new ApiTypeExpression(new ApiCollectionType(new ApiTypeExpression(typeof(Person)), ApiTypeModifiers.Required, typeof(List<Person>))),
                        ApiTypeModifiers.None,
                        nameof(Company.Employees)
                    ),
                ],
                [
                    new ApiRelationship("OwnedBy", nameof(Company.Owner)),
                    new ApiRelationship(nameof(Company.Employees))
                ],
                typeof(Company)
            ),
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
            Name = "ApiScalarType [Boolean]",
            Source = new ApiScalarType("Boolean", typeof(bool)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = "ApiScalarType [Boolean] With Extension 1",
            Source = new ApiScalarType("Boolean", typeof(bool)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            AddTestExtension1 = true
        },

        new JsonSerializeTest<ApiType>
        {
            Name = "ApiScalarType [ID]",
            Source = new ApiScalarType("ID", typeof(string)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""ID"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = "ApiScalarType [Int]",
            Source = new ApiScalarType("Int", typeof(int)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Int"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = "ApiScalarType [Float]",
            Source = new ApiScalarType("Float", typeof(float)),
            Expected = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Float"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = "ApiScalarType [String]",
            Source = new ApiScalarType("String", typeof(string)),
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
            Name = $"ApiEnumType [{nameof(StopLight)}]",
            Source = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"ApiEnumType [{nameof(StopLight)}] With Extension 2",
            Source = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            AddTestExtension2 = true
        },

        // ApiCollectionType
        new JsonSerializeTest<ApiType>
        {
            Name = "ApiCollectionType [List<String>]",
            Source = new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Scalar, "String"), ApiTypeModifiers.Required, typeof(List<string>)),
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
            Name = "ApiCollectionType [List<String?>]",
            Source = new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Scalar, "String"), ApiTypeModifiers.None, typeof(List<string?>)),
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
            Name = $"ApiCollectionType [List<{nameof(StopLight)}>]",
            Source = new ApiCollectionType
            (
                new ApiTypeExpression(ApiTypeKind.Enum, nameof(StopLight)),
                ApiTypeModifiers.Required,
                typeof(List<StopLight>)
            ),
            Expected = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}?>]",
            Source = new ApiCollectionType
            (
                new ApiTypeExpression(ApiTypeKind.Enum, nameof(StopLight)),
                ApiTypeModifiers.None,
                typeof(List<StopLight>)
            ),
            Expected = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }"
        },

        // ApiObjectType With API Named Typed Expressions
        new JsonSerializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With API Named Typed Expressions",
            Source = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With API Named Typed Expressions And With Extension 1 and Extension 2",
            Source = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Long"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "Boolean"),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    },
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            AddTestExtension1 = true,
            AddTestExtension2 = true
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(Company)}] With API Named Typed Expressions",
            Source = new ApiObjectType
            (
                nameof(Company),
                [
                    new ApiProperty
                    (
                        nameof(Company.Name),
                        new ApiTypeExpression(ApiTypeKind.Scalar, "String"),
                        ApiTypeModifiers.Required,
                        nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Owner),
                        new ApiTypeExpression(ApiTypeKind.Object, nameof(Person)),
                        ApiTypeModifiers.None,
                        nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Employees),
                        new ApiTypeExpression(new ApiCollectionType(new ApiTypeExpression(ApiTypeKind.Object, nameof(Person)), ApiTypeModifiers.Required, typeof(List<Person>))),
                        ApiTypeModifiers.None,
                        nameof(Company.Employees)
                    ),
                ],
                [
                    new ApiRelationship("OwnedBy", nameof(Company.Owner)),
                    new ApiRelationship(nameof(Company.Employees))
                ],
                typeof(Company)
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
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BCompany, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        // ApiObjectType With CLR Typed Expressions
        new JsonSerializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With CLR Typed Expressions",
            Source = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(typeof(long?)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(typeof(bool?)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With CLR Typed Expressions And With Extension 1 and Extension 2",
            Source = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiTypeExpression(typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiTypeExpression(typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiTypeExpression(typeof(long?)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiTypeExpression(typeof(bool?)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                [],
                typeof(ClassWithScalars)
            ),
            Expected = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    },
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            AddTestExtension1 = true,
            AddTestExtension2 = true
        },

        new JsonSerializeTest<ApiType>
        {
            Name = $"ApiObjectType [{nameof(Company)}] With CLR Typed Expressions",
            Source = new ApiObjectType
            (
                nameof(Company),
                [
                    new ApiProperty
                    (
                        nameof(Company.Name),
                        new ApiTypeExpression(typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(Company.Name)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Owner),
                        new ApiTypeExpression(typeof(Person)),
                        ApiTypeModifiers.None,
                        nameof(Company.Owner)
                    ),
                    new ApiProperty
                    (
                        nameof(Company.Employees),
                        new ApiTypeExpression(new ApiCollectionType(new ApiTypeExpression(typeof(Person)), ApiTypeModifiers.Required, typeof(List<Person>))),
                        ApiTypeModifiers.None,
                        nameof(Company.Employees)
                    ),
                ],
                [
                    new ApiRelationship("OwnedBy", nameof(Company.Owner)),
                    new ApiRelationship(nameof(Company.Employees))
                ],
                typeof(Company)
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
                            ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson,Evoogle.ApiFramework.Core.Tests""
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
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson,Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BCompany, Evoogle.ApiFramework.Core.Tests""
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
