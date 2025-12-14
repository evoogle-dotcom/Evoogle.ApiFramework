// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.XUnit.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema;

public class ApiSchemaTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    public class InitializeThrowsTest : JsonConverterTestBase<ApiSchema>
    {
        #region User Supplied Properties
        public required string Source { get; init; }
        public required string ExpectedExceptionMessage { get; init; }
        public required List<ApiInitializationIssue> ExpectedIssues { get; init; }
        #endregion

        #region Calculated Properties
        private bool? ActualExceptionThrown { get; set; }
        private string? ActualExceptionMessage { get; set; }
        private List<ApiInitializationIssue>? ActualIssues { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Source:   {this.Source.SafeToString().RemoveWhitespace()}");
            this.WriteLine();

            this.WriteLine($"Expected Exception Message: {this.ExpectedExceptionMessage.SafeToString()}");
            this.WriteLine();
            foreach (var expectedIssue in this.ExpectedIssues)
            {
                this.WriteLine($"Expected Issue: {expectedIssue.SafeToString()}");
            }
            this.WriteLine();
        }
        #endregion

        protected override void Act()
        {
            try
            {
                JsonSerializer.Deserialize<ApiSchema>(this.Source, this.JsonSerializerOptions);
            }
            catch (ApiSchemaInitializationException ex)
            {
                this.ActualExceptionThrown = true;
                this.ActualExceptionMessage = ex.Message;
                this.ActualIssues = [.. ex.Issues];
            }

            this.WriteLine($"Actual Exception Thrown:  {this.ActualExceptionThrown.SafeToString()}");
            this.WriteLine($"Actual Exception Message: {this.ActualExceptionMessage.SafeToString()}");
            this.WriteLine();
            if (this.ActualIssues is not null)
            {
                foreach (var actualIssue in this.ActualIssues)
                {
                    this.WriteLine($"Actual Issue: {actualIssue.SafeToString()}");
                }
            }
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualExceptionThrown.Should().BeTrue();
            this.ActualExceptionMessage.Should().Be(this.ExpectedExceptionMessage);
            this.ActualIssues.Should().NotBeNull();
            this.ActualIssues.Should().BeEquivalentTo(this.ExpectedIssues);
        }
    }

    public class TryGetByApiNameTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiTestSchemaKind ApiSchemaKind { get; init; }
        public required string SearchKey { get; init; }
        public required bool ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private bool? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = ApiTestSchemaFactory.BuildTestSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            this.WriteLine($"ApiSchema:      {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"SearchKey:      {this.SearchKey.SafeToString()}");
            this.WriteLine($"ExpectedResult: {this.ExpectedResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiSchema!.TryGetTypeByApiName(this.SearchKey, out _);

            this.WriteLine($"ActualResult: {this.ActualResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().NotBeNull();
            this.ActualResult.Should().Be(this.ExpectedResult);
        }
        #endregion
    }

    public class TryGetByClrTypeTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiTestSchemaKind ApiSchemaKind { get; init; }
        public required Type SearchKey { get; init; }
        public required bool ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private bool? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = ApiTestSchemaFactory.BuildTestSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            this.WriteLine($"ApiSchema:      {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"SearchKey:      {this.SearchKey.SafeToString()}");
            this.WriteLine($"ExpectedResult: {this.ExpectedResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiSchema!.TryGetTypeByClrType(this.SearchKey, out _);

            this.WriteLine($"ActualResult: {this.ActualResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().NotBeNull();
            this.ActualResult.Should().Be(this.ExpectedResult);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static readonly ApiScalarType TestApiScalarTypeBoolean = new(nameof(Boolean), typeof(bool));
    public static readonly ApiScalarType TestApiScalarTypeInt32 = new(nameof(Int32), typeof(int));
    public static readonly ApiScalarType TestApiScalarTypeString = new(nameof(String), typeof(string));
    public static readonly ApiScalarType TestApiScalarTypeUInt32 = new(nameof(UInt32), typeof(uint));

    public static readonly ApiTypeExpression TestApiScalarTypeBooleanReference = new(ApiTypeKind.Scalar, nameof(Boolean));
    public static readonly ApiTypeExpression TestApiScalarTypeInt32Reference = new(ApiTypeKind.Scalar, nameof(Int32));
    public static readonly ApiTypeExpression TestApiScalarTypeStringReference = new(ApiTypeKind.Scalar, nameof(String));
    public static readonly ApiTypeExpression TestApiScalarTypeUInt32Reference = new(ApiTypeKind.Scalar, nameof(UInt32));

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

    public static readonly ApiTypeExpression TestApiEnumTypeGenderReference = new(ApiTypeKind.Enum, nameof(Gender));

    public static readonly ApiTypeExpression TestApiCollectionTypeListOfString = new
    (
        new ApiCollectionType(TestApiScalarTypeStringReference, ApiTypeModifiers.None, typeof(List<string>))
    );

    public static readonly ApiObjectType TestApiObjectTypePerson = new
    (
        nameof(Person),
        [
            new ApiProperty(nameof(Person.Name), TestApiScalarTypeStringReference, ApiTypeModifiers.Required, nameof(Person.Name)),
            new ApiProperty(nameof(Person.Age), TestApiScalarTypeInt32Reference, ApiTypeModifiers.None, nameof(Person.Age)),
            new ApiProperty(nameof(Person.Gender), TestApiEnumTypeGenderReference, ApiTypeModifiers.None, nameof(Person.Gender)),
            new ApiProperty(nameof(Person.Hobbies), new ApiTypeExpression(new ApiCollectionType(TestApiScalarTypeStringReference, ApiTypeModifiers.Required, typeof(List<string>))), ApiTypeModifiers.None, nameof(Person.Hobbies))
        ],
        [],
        typeof(Person)
    );

    public static readonly ApiTypeExpression TestApiObjectTypePersonReference = new(ApiTypeKind.Object, nameof(Person));

    public static readonly ApiObjectType TestApiObjectTypeCompany = new
    (
        nameof(Company),
        [
            new ApiProperty(nameof(Company.Name), TestApiScalarTypeStringReference, ApiTypeModifiers.Required, nameof(Company.Name)),
            new ApiProperty(nameof(Company.Owner), TestApiObjectTypePersonReference, ApiTypeModifiers.None, nameof(Company.Owner)),
            new ApiProperty(nameof(Company.Employees), new ApiTypeExpression(new ApiCollectionType(TestApiObjectTypePersonReference, ApiTypeModifiers.Required, typeof(List<Person>))), ApiTypeModifiers.None, nameof(Company.Employees))
        ],
        [
            new ApiRelationship(nameof(Company.Owner)),
            new ApiRelationship(nameof(Company.Employees))
        ],
        typeof(Company)
    );

    public static readonly ApiTypeExpression TestApiObjectTypeCompanyReference = new(ApiTypeKind.Object, nameof(Company));

    public class TestUnknownType
    {
    }

    public static TheoryDataRow<IXUnitTest>[] InitializeThrowsTheoryData =>
    [
        // ApiEnumType throws if ApiName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiName)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
                        ""ApiName"": """",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiEnumType)}.{nameof(ApiEnumType.ApiName)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_NAMED_TYPE_INVALID_API_NAME,
                    description: $"{nameof(ApiEnumType.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiEnumType.ApiName)} value"
                ),
            ]
        },

        // ApiEnumType throws if ClrType is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ClrType)} Is Null",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
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
                        ]
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumType.ClrType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_NAMED_TYPE_NULL_CLR_TYPE,
                    description: $"{nameof(ApiEnumType.ClrType)} must not be null",
                    remediation: $"Specify a valid {nameof(ApiEnumType.ClrType)}"
                ),
            ]
        },

        // ApiEnumType throws if ClrType is not a CLR Enum
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ClrType)} Is Not a CLR Enum",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
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
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumType.ClrType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_TYPE_INVALID_CLR_TYPE,
                    description: $"{nameof(ApiEnumType.ClrType)} 'String' must be a CLR Enum",
                    remediation: $"Set {nameof(ApiEnumType.ClrType)} to a CLR Enum type"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Is Null",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumType.ApiEnumValues)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_TYPE_NULL_OR_EMPTY_VALUES,
                    description: $"{nameof(ApiEnumType.ApiEnumValues)} must not be null or empty",
                    remediation: $"Define at least one {nameof(ApiEnumValue)}"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues is emtpy
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Is Empty",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""Kind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumType.ApiEnumValues)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_TYPE_NULL_OR_EMPTY_VALUES,
                    description: $"{nameof(ApiEnumType.ApiEnumValues)} must not be null or empty",
                    remediation: $"Define at least one {nameof(ApiEnumValue)}"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues has duplicate ApiName values
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Has Duplicate {nameof(ApiEnumValue.ApiName)} Values",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
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
                                ""ApiName"": ""Female"",
                                ""ClrName"": ""Male"",
                                ""ClrOrdinal"": 1
                            },
                            {
                                ""ApiName"": ""Female"",
                                ""ClrName"": ""Female"",
                                ""ClrOrdinal"": 2
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumType.ApiEnumValues)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_TYPE_DUPLICATE_VALUE_API_NAME,
                    description: $"Duplicate {nameof(ApiEnumValue)}.{nameof(ApiEnumValue.ApiName)} values: 'Female'",
                    remediation: $"Verify that each {nameof(ApiEnumValue)} has a unique {nameof(ApiEnumValue.ApiName)} value"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues has duplicate ClrName values
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Has Duplicate {nameof(ApiEnumValue.ClrName)} Values",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
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
                                ""ClrName"": ""Female"",
                                ""ClrOrdinal"": 1
                            },
                            {
                                ""ApiName"": ""Female"",
                                ""ClrName"": ""Female"",
                                ""ClrOrdinal"": 2
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumType.ApiEnumValues)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_TYPE_DUPLICATE_VALUE_CLR_NAME,
                    description: $"Duplicate {nameof(ApiEnumValue)}.{nameof(ApiEnumValue.ClrName)} values: 'Female'",
                    remediation: $"Verify that each {nameof(ApiEnumValue)} has a unique {nameof(ApiEnumValue.ClrName)} value"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues has duplicate ClrOrdinal values
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Has Duplicate {nameof(ApiEnumValue.ClrOrdinal)} Values",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
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
                            },
                            {
                                ""ApiName"": ""Alien"",
                                ""ClrName"": ""Alien"",
                                ""ClrOrdinal"": 2
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumType.ApiEnumValues)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_TYPE_DUPLICATE_VALUE_CLR_ORDINAL,
                    description: $"Duplicate {nameof(ApiEnumValue)}.{nameof(ApiEnumValue.ClrOrdinal)} values: '2'",
                    remediation: $"Verify that each {nameof(ApiEnumValue)} has a unique {nameof(ApiEnumValue.ClrOrdinal)} value"
                ),
            ]
        },
    ];

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
            Name = $"{nameof(ApiSchema)} With No ApiTypes",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With No ApiTypes"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            Expected = ApiSchema.Create("ApiSchema With No ApiTypes", []),
        },

        // ApiSchema With 1 ApiScalarType
        new JsonDeserializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiScalarType",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType"",
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
            Expected = ApiSchema.Create("ApiSchema With 1 ApiScalarType", [TestApiScalarTypeBoolean]),
        },

        // ApiSchema With 1 ApiScalarType And Extension 1
        new JsonDeserializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiScalarType And Extension 1",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And Extension 1"",
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
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            Expected = ApiSchema.Create("ApiSchema With 1 ApiScalarType And Extension 1", [TestApiScalarTypeBoolean]),
            ExtensionType1 = typeof(TestExtension1)
        },

        // ApiSchema With 2 ApiScalarTypes
        new JsonDeserializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 2 ApiScalarTypes",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 2 ApiScalarTypes"",
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
            Expected = ApiSchema.Create("ApiSchema With 2 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32]),
        },

        // ApiSchema With 3 ApiScalarTypes
        new JsonDeserializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes"",
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
            Expected = ApiSchema.Create("ApiSchema With 3 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
        },

        // ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2
        new JsonDeserializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes And Extension 1 And Extension 2",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2"",
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
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""TestExtension2""
                    }
                }
            }",
            Expected = ApiSchema.Create("ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2),
        },

        // ApiSchema With 1 ApiEnumType
        new JsonDeserializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiEnumType",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            Expected = ApiSchema.Create("ApiSchema With 1 ApiEnumType", [TestApiEnumTypeGender]),
        },

        // ApiSchema With 1 ApiEnumType And Extension 1
        new JsonDeserializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiEnumType And Extension 1",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType And Extension 1"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            Expected = ApiSchema.Create("ApiSchema With 1 ApiEnumType And Extension 1", [TestApiEnumTypeGender]),
            ExtensionType1 = typeof(TestExtension1),
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)
        new JsonDeserializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
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
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
                                    ""ApiName"": ""Gender""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            },
                            {
                                ""ApiName"": ""Hobbies"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""Kind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""Kind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Hobbies""
                            }
                        ],
                        ""ApiRelationships"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            Expected = ApiSchema.Create
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)",
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
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
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
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
                                    ""ApiName"": ""Gender""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            },
                            {
                                ""ApiName"": ""Hobbies"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""Kind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""Kind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Hobbies""
                            }
                        ],
                        ""ApiRelationships"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
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
            Expected = ApiSchema.Create
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson
                ]
            ),
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2),
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company)
        new JsonDeserializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company)",
            Source = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company)"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
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
                                ""ApiName"": ""Owner""
                            },
                            {
                                ""ApiName"": ""Employees""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""Kind"": ""Object"",
                        ""ApiName"": ""Person"",
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
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
                                    ""ApiName"": ""Gender""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            },
                            {
                                ""ApiName"": ""Hobbies"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""Kind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""Kind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Hobbies""
                            }
                        ],
                        ""ApiRelationships"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            Expected = ApiSchema.Create
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company)",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypeCompany,
                    TestApiObjectTypePerson
                ]
            )
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
            Name = $"{nameof(ApiSchema)} With No ApiTypes",
            Expected = ApiSchema.Create("ApiSchema With No ApiTypes", []),
        },

        // ApiSchema With 1 ApiScalarType
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiScalarType",
            Expected = ApiSchema.Create("ApiSchema With 1 ApiScalarType", [TestApiScalarTypeBoolean]),
        },

        // ApiSchema With 1 ApiScalarType And Extension 1
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiScalarType And Extension 1",
            Expected = ApiSchema.Create("ApiSchema With 1 ApiScalarType And Extension 1", [TestApiScalarTypeBoolean]),
            ExtensionType1 = typeof(TestExtension1)
        },

        // ApiSchema With 2 ApiScalarTypes
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 2 ApiScalarTypes",
            Expected = ApiSchema.Create("ApiSchema With 2 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32]),
        },

        // ApiSchema With 3 ApiScalarTypes
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes",
            Expected = ApiSchema.Create("ApiSchema With 3 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
        },

        // ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes And Extension 1 And Extension 2",
            Expected = ApiSchema.Create("ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2),
        },

        // ApiSchema With 1 ApiEnumType
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiEnumType",
            Expected = ApiSchema.Create("ApiSchema With 1 ApiEnumType", [TestApiEnumTypeGender]),
        },

        // ApiSchema With 1 ApiEnumType And Extension 1
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiEnumType And Extension 1",
            Expected = ApiSchema.Create("ApiSchema With 1 ApiEnumType And Extension 1", [TestApiEnumTypeGender]),
            ExtensionType1 = typeof(TestExtension1),
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)",
            Expected = ApiSchema.Create
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson
                ]
            ),
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2",
            Expected = ApiSchema.Create
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson
                ]
            ),
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2),
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company) And Extension 1 And Extension 2
        new JsonRoundtripTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company) And Extension 1 And Extension 2",
            Expected = ApiSchema.Create
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company) And Extension 1 And Extension 2",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson,
                    TestApiObjectTypeCompany
                ]
            ),
            ExtensionType1 = typeof(TestExtension1),
            ExtensionType2 = typeof(TestExtension2),
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
            Name = $"{nameof(ApiSchema)} With No ApiTypes",
            Source = ApiSchema.Create("ApiSchema With No ApiTypes", []),
            Expected = @"
            {
                ""ApiName"": ""ApiSchema With No ApiTypes"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType
        new JsonSerializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiScalarType",
            Source = ApiSchema.Create("ApiSchema With 1 ApiScalarType", [TestApiScalarTypeBoolean]),
            Expected = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType"",
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
            Name = $"{nameof(ApiSchema)} With 1 ApiScalarType And Extension 1",
            Source = ApiSchema.Create("ApiSchema With 1 ApiScalarType And Extension 1", [TestApiScalarTypeBoolean]),
            Expected = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And Extension 1"",
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
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            ExtensionType1 = typeof(TestExtension1)
        },

        // ApiSchema With 2 ApiScalarTypes
        new JsonSerializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 2 ApiScalarTypes",
            Source = ApiSchema.Create("ApiSchema With 2 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32]),
            Expected = @"
            {
                ""ApiName"": ""ApiSchema With 2 ApiScalarTypes"",
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
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes",
            Source = ApiSchema.Create("ApiSchema With 3 ApiScalarTypes", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
            Expected = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes"",
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
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes And Extension 1 And Extension 2",
            Source = ApiSchema.Create("ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2", [TestApiScalarTypeBoolean, TestApiScalarTypeInt32, TestApiScalarTypeString]),
            Expected = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes And Extension 1 And Extension 2"",
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

        // ApiSchema With 1 ApiEnumType
        new JsonSerializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiEnumType",
            Source = ApiSchema.Create("ApiSchema With 1 ApiEnumType", [TestApiEnumTypeGender]),
            Expected = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiEnumType And Extension 1
        new JsonSerializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 1 ApiEnumType And Extension 1",
            Source = ApiSchema.Create("ApiSchema With 1 ApiEnumType And Extension 1", [TestApiEnumTypeGender]),
            Expected = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType And Extension 1"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.TestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""TestExtension1""
                    }
                }
            }",
            ExtensionType1 = typeof(TestExtension1)
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)
        new JsonSerializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)",
            Source = ApiSchema.Create
            (

                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)",
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
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person)"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
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
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
                                    ""ApiName"": ""Gender""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            },
                            {
                                ""ApiName"": ""Hobbies"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""Kind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""Kind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Hobbies""
                            }
                        ],
                        ""ApiRelationships"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }"
        },

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2
        new JsonSerializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2",
            Source = ApiSchema.Create
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2",
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
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 1 ApiObjectType (Person) And Extension 1 And Extension 2"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
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
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
                                    ""ApiName"": ""Gender""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            },
                            {
                                ""ApiName"": ""Hobbies"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""Kind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""Kind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Hobbies""
                            }
                        ],
                        ""ApiRelationships"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
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

        // ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company)
        new JsonSerializeTest<ApiSchema>
        {
            Name = $"{nameof(ApiSchema)} With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company)",
            Source = ApiSchema.Create
            (
                "ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company)",
                [
                    TestApiScalarTypeBoolean,
                    TestApiScalarTypeInt32,
                    TestApiScalarTypeString,
                    TestApiEnumTypeGender,
                    TestApiObjectTypePerson,
                    TestApiObjectTypeCompany
                ]
            ),
            Expected = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarTypes and 1 ApiEnumType and 2 ApiObjectTypes (Person and Company)"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [
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
                                ""ApiName"": ""Owner""
                            },
                            {
                                ""ApiName"": ""Employees""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""Kind"": ""Object"",
                        ""ApiName"": ""Person"",
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
                                ""ApiName"": ""Age"",
                                ""ApiType"": {
                                    ""Kind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Age""
                            },
                            {
                                ""ApiName"": ""Gender"",
                                ""ApiType"": {
                                    ""Kind"": ""Enum"",
                                    ""ApiName"": ""Gender""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Gender""
                            },
                            {
                                ""ApiName"": ""Hobbies"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""Kind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""Kind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String,System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Hobbies""
                            }
                        ],
                        ""ApiRelationships"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }"
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetByApiNameTheoryData =>
    [
        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false when {nameof(ApiNamedType)} does not exist in schema",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = "UnknownType",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiEnumType)} with exact case match",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = "Gender",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiEnumType)} with case-insensitive search (uppercase)",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = "GENDER",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiObjectType)} with exact case match",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = "ScalarsOnly",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiObjectType)} with case-insensitive search (lowercase)",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = "scalarsonly",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiScalarType)} with exact case match",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = "Boolean",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiScalarType)} with case-insensitive search (uppercase)",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = "BOOLEAN",
            ExpectedResult = false
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetByClrTypeTheoryData =>
    [
        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns false when {nameof(ApiType)} is not registered in schema",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = typeof(TestUnknownType),
            ExpectedResult = false
        },

        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns true for registered {nameof(ApiEnumType)} CLR type",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = typeof(Gender),
            ExpectedResult = true
        },

        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns true for registered {nameof(ApiObjectType)} CLR type",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = typeof(ScalarsOnly),
            ExpectedResult = true
        },

        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns true for registered {nameof(ApiScalarType)} CLR type",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            SearchKey = typeof(bool),
            ExpectedResult = true
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(InitializeThrowsTheoryData))]
    public void InitializeThrows(IXUnitTest test) => test.Execute(this);

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
