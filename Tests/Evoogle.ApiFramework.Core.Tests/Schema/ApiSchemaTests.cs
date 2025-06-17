// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests;

namespace Evoogle.ApiFramework.Schema;

public class ApiSchemaTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class TryGetByApiNameTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiSchema? ApiSchema { get; set; }
        public string? ApiName { get; set; }
        public ApiType? ExpectedApiType { get; set; }
        #endregion

        #region Calculated Properties
        private bool ExpectedResult { get; set; }
        private bool ActualResult { get; set; }
        private ApiType? ActualApiType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName: {this.ApiName.SafeToString()}");
            this.WriteLine();

            this.ExpectedResult = this.ExpectedApiType != null;

            this.WriteLine($"Expected Result:  {this.ExpectedResult.SafeToString()}");
            this.WriteLine($"Expected ApiType: {this.ExpectedApiType.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiSchema!.TryGetApiType(this.ApiName!, out var apiType);
            this.ActualApiType = apiType;

            this.WriteLine($"Actual Result:    {this.ActualResult.SafeToString()}");
            this.WriteLine($"Actual ApiType:   {this.ActualApiType.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualResult.Should().Be(this.ExpectedResult);
            this.ActualApiType.Should().BeEquivalentTo(this.ExpectedApiType);
        }
        #endregion
    }

    private class TryGetByClrTypeTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiSchema? ApiSchema { get; set; }
        public Type? ClrType { get; set; }
        public ApiType? ExpectedApiType { get; set; }
        #endregion

        #region Calculated Properties
        private bool ExpectedResult { get; set; }
        private bool ActualResult { get; set; }
        private ApiType? ActualApiType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var clrTypeName = this.ClrType!.Name;
            this.WriteLine($"ClrType: {clrTypeName.SafeToString()}");
            this.WriteLine();

            this.ExpectedResult = this.ExpectedApiType != null;

            this.WriteLine($"Expected Result:  {this.ExpectedResult.SafeToString()}");
            this.WriteLine($"Expected ApiType: {this.ExpectedApiType.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiSchema!.TryGetApiType(this.ClrType!, out var apiType);
            this.ActualApiType = apiType;

            this.WriteLine($"Actual   Result:  {this.ActualResult.SafeToString()}");
            this.WriteLine($"Actual   ApiType: {this.ActualApiType.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualResult.Should().Be(this.ExpectedResult);
            this.ActualApiType.Should().BeEquivalentTo(this.ExpectedApiType);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static readonly ApiScalarType TestApiScalarTypeBoolean = new(nameof(Boolean), typeof(bool));
    public static readonly ApiScalarType TestApiScalarTypeInt32 = new(nameof(Int32), typeof(int));
    public static readonly ApiScalarType TestApiScalarTypeString = new(nameof(String), typeof(string));
    public static readonly ApiScalarType TestApiScalarTypeUInt32 = new(nameof(UInt32), typeof(uint));

    public static readonly ApiEnumType TestApiEnumTypeGender = new
    (
        nameof(Gender),
        [
            new ApiEnumValue(nameof(Gender.Unspecified), nameof(Gender.Unspecified), (int)Gender.Unspecified),
            new ApiEnumValue(nameof(Gender.Male), nameof(Gender.Male), (int)Gender.Male),
            new ApiEnumValue(nameof(Gender.Female), nameof(Gender.Female), (int)Gender.Female)
        ],
        typeof(Gender)
    );

    public static readonly ApiObjectType TestApiObjectTypePerson = new
    (
        nameof(Person),
        [
            new ApiProperty(nameof(Person.Name), TestApiScalarTypeString, ApiTypeModifiers.Required, nameof(Person.Name)),
            new ApiProperty(nameof(Person.Age), TestApiScalarTypeInt32, ApiTypeModifiers.None, nameof(Person.Age)),
            new ApiProperty(nameof(Person.Gender), TestApiEnumTypeGender, ApiTypeModifiers.None, nameof(Person.Gender))
        ],
        typeof(Person)
    );

    public static readonly ApiSchema TestApiSchema = new
    (
        nameof(TestApiSchema),
        [
            TestApiScalarTypeBoolean,
            TestApiScalarTypeInt32,
            TestApiScalarTypeString,
            TestApiEnumTypeGender,
            TestApiObjectTypePerson
        ]
    );

    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        // Null
        new JsonDeserializeTest<ApiType>
        {
            Name = "Null",
            Source = "null",
            Expected = null
        },

        // ApiSchema With No ApiTypes
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With No ApiTypes",
            Source = @"
            {
                ""Name"": ""ApiSchema With No ApiTypes"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            Expected = new ApiSchema("ApiSchema With No ApiTypes", []),
        },

        // ApiSchema With 1 ApiScalarType
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiScalarType",
            Source = @"
            {
                ""Name"": ""ApiSchema With 1 ApiScalarType"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            Expected = new ApiSchema("ApiSchema With 1 ApiScalarType", [TestApiScalarTypeBoolean]),
        },

        // ApiSchema With 1 ApiScalarType And Extension 1
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiScalarType And Extension 1",
            Source = @"
            {
                ""Name"": ""ApiSchema With 1 ApiScalarType And Extension 1"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            Expected = new ApiSchema("ApiSchema With 1 ApiScalarType And Extension 1", [TestApiScalarTypeBoolean]),
            AddTestExtension1 = true
        },

        // ApiSchema With 2 ApiScalarTypes
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With 2 ApiScalarTypes",
            Source = @"
            {
                ""Name"": ""ApiSchema With 2 ApiScalarTypes"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            Expected = new ApiSchema("ApiSchema With 2 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32]),
        },

        // ApiSchema With 3 ApiScalarTypes
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes",
            Source = @"
            {
                ""Name"": ""ApiSchema With 3 ApiScalarTypes"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            Expected = new ApiSchema("ApiSchema With 3 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
        },

        // ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2",
            Source = @"
            {
                ""Name"": ""ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
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
            Expected = new ApiSchema("ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
            AddTestExtension1 = true,
            AddTestExtension2 = true,
        },

        // ApiSchema With 1 ApiEnumType
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiEnumType",
            Source = @"
            {
                ""Name"": ""ApiSchema With 1 ApiEnumType"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            Expected = new ApiSchema("ApiSchema With 1 ApiEnumType", [TestApiEnumTypeGender]),
        },

        // ApiSchema With 1 ApiEnumType And Extension 1
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiEnumType And Extension 1",
            Source = @"
            {
                ""Name"": ""ApiSchema With 1 ApiEnumType And Extension 1"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            Expected = new ApiSchema("ApiSchema With 1 ApiEnumType And Extension 1", [TestApiEnumTypeGender]),
            AddTestExtension1 = true,
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType",
            Source = @"
            {
                ""Name"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""Kind"": ""Object"",
                        ""ApiName"": ""Person"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""String"",
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32"",
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
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
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            Expected = new ApiSchema
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson
                ]
            ),
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2
        new JsonDeserializeTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2",
            Source = @"
            {
                ""Name"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""Kind"": ""Object"",
                        ""ApiName"": ""Person"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""String"",
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32"",
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
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
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
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
            Expected = new ApiSchema
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson
                ]
            ),
            AddTestExtension1 = true,
            AddTestExtension2 = true,
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

        // ApiSchema With No ApiTypes
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With No ApiTypes",
            Expected = new ApiSchema("ApiSchema With No ApiTypes", []),
        },

        // ApiSchema With 1 ApiScalarType
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiScalarType",
            Expected = new ApiSchema("ApiSchema With 1 ApiScalarType", [TestApiScalarTypeBoolean]),
        },

        // ApiSchema With 1 ApiScalarType And Extension 1
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiScalarType And Extension 1",
            Expected = new ApiSchema("ApiSchema With 1 ApiScalarType And Extension 1", [TestApiScalarTypeBoolean]),
            AddTestExtension1 = true
        },

        // ApiSchema With 2 ApiScalarTypes
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With 2 ApiScalarTypes",
            Expected = new ApiSchema("ApiSchema With 2 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32]),
        },

        // ApiSchema With 3 ApiScalarTypes
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes",
            Expected = new ApiSchema("ApiSchema With 3 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
        },

        // ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2",
            Expected = new ApiSchema("ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
            AddTestExtension1 = true,
            AddTestExtension2 = true,
        },

        // ApiSchema With 1 ApiEnumType
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiEnumType",
            Expected = new ApiSchema("ApiSchema With 1 ApiEnumType", [TestApiEnumTypeGender]),
        },

        // ApiSchema With 1 ApiEnumType And Extension 1
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiEnumType And Extension 1",
            Expected = new ApiSchema("ApiSchema With 1 ApiEnumType And Extension 1", [TestApiEnumTypeGender]),
            AddTestExtension1 = true,
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType",
            Expected = new ApiSchema
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson
                ]
            ),
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2
        new JsonRoundtripTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2",
            Expected = new ApiSchema
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson
                ]
            ),
            AddTestExtension1 = true,
            AddTestExtension2 = true,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        // Null
        new JsonSerializeTest<ApiSchema>
        {
            Name = "Null",
            Source = null,
            Expected = "null"
        },

        // ApiSchema With No ApiTypes
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With No ApiTypes",
            Source = new ApiSchema("ApiSchema With No ApiTypes", []),
            Expected = @"
            {
                ""Name"": ""ApiSchema With No ApiTypes"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiScalarType",
            Source = new ApiSchema("ApiSchema With 1 ApiScalarType", [TestApiScalarTypeBoolean]),
            Expected = @"
            {
                ""Name"": ""ApiSchema With 1 ApiScalarType"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType And Extension 1
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiScalarType And Extension 1",
            Source = new ApiSchema("ApiSchema With 1 ApiScalarType And Extension 1", [TestApiScalarTypeBoolean]),
            Expected = @"
            {
                ""Name"": ""ApiSchema With 1 ApiScalarType And Extension 1"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            AddTestExtension1 = true
        },

        // ApiSchema With 2 ApiScalarTypes
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With 2 ApiScalarTypes",
            Source = new ApiSchema("ApiSchema With 2 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32]),
            Expected = @"
            {
                ""Name"": ""ApiSchema With 2 ApiScalarTypes"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 3 ApiScalarTypes
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes",
            Source = new ApiSchema("ApiSchema With 3 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
            Expected = @"
            {
                ""Name"": ""ApiSchema With 3 ApiScalarTypes"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2",
            Source = new ApiSchema("ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
            Expected = @"
            {
                ""Name"": ""ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
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

        // ApiSchema With 1 ApiEnumType
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiEnumType",
            Source = new ApiSchema("ApiSchema With 1 ApiEnumType", [TestApiEnumTypeGender]),
            Expected = @"
            {
                ""Name"": ""ApiSchema With 1 ApiEnumType"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiEnumType And Extension 1
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With 1 ApiEnumType And Extension 1",
            Source = new ApiSchema("ApiSchema With 1 ApiEnumType And Extension 1", [TestApiEnumTypeGender]),
            Expected = @"
            {
                ""Name"": ""ApiSchema With 1 ApiEnumType And Extension 1"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            AddTestExtension1 = true
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType",
            Source = new ApiSchema
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson
                ]
            ),
            Expected = @"
            {
                ""Name"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""Kind"": ""Object"",
                        ""ApiName"": ""Person"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""String"",
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32"",
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
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
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }"
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2
        new JsonSerializeTest<ApiSchema>
        {
            Name = "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2",
            Source = new ApiSchema
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson
                ]
            ),
            Expected = @"
            {
                ""Name"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType And Extension 1 And Extension 2"",
                ""ApiScalarTypes"": [
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Boolean"",
                        ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
                    {
                        ""Kind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
                    {
                        ""Kind"": ""Object"",
                        ""ApiName"": ""Person"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""String"",
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32"",
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
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
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BGender, Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiJsonConverterUnitTests\u002BPerson, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
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
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetByApiNameTheoryData =>
    [
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiScalarTypeBoolean} by ApiName, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ApiName = nameof(Boolean),
            ExpectedApiType = TestApiScalarTypeBoolean,
        },
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiScalarTypeInt32} by ApiName, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ApiName = nameof(Int32),
            ExpectedApiType = TestApiScalarTypeInt32,
        },
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiScalarTypeString} by ApiName, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ApiName = nameof(String),
            ExpectedApiType = TestApiScalarTypeString,
        },
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiEnumTypeGender} by ApiName, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ApiName = nameof(Gender),
            ExpectedApiType = TestApiEnumTypeGender,
        },
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiObjectTypePerson} by ApiName, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ApiName = nameof(Person),
            ExpectedApiType = TestApiObjectTypePerson
        },
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiScalarTypeUInt32} by ApiName, Expect ApiType Does Not Exist",
            ApiSchema = TestApiSchema,
            ApiName = nameof(UInt32),
            ExpectedApiType = null,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetByClrTypeTheoryData =>
    [
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiScalarTypeBoolean} by ClrType, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ClrType = typeof(bool),
            ExpectedApiType = TestApiScalarTypeBoolean,
        },
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiScalarTypeInt32} by ClrType, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ClrType = typeof(int),
            ExpectedApiType = TestApiScalarTypeInt32,
        },
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiScalarTypeString} by ClrType, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ClrType = typeof(string),
            ExpectedApiType = TestApiScalarTypeString,
        },
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiEnumTypeGender} by ClrType, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ClrType = typeof(Gender),
            ExpectedApiType = TestApiEnumTypeGender,
        },
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiObjectTypePerson} by ClrType, Expect ApiType Exists",
            ApiSchema = TestApiSchema,
            ClrType = typeof(Person),
            ExpectedApiType = TestApiObjectTypePerson
        },
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiScalarTypeUInt32} by ClrType, Expect ApiType Does Not Exist",
            ApiSchema = TestApiSchema,
            ClrType = typeof(uint),
            ExpectedApiType = null,
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

    [Theory]
    [MemberData(nameof(TryGetByApiNameTheoryData))]
    public void TryGetByApiName(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetByClrTypeTheoryData))]
    public void TryGetByClrType(IXUnitTest test) => test.Execute(this);
    #endregion
}

