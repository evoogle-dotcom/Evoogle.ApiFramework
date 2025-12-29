// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;
using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema;

public class ApiSchemaTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Fields
    private static readonly List<string> _excludeMembers =
    [
        $"{nameof(ApiSchemaContext)}",
        $"{nameof(ApiSchemaContext)}.{nameof(ApiSchemaContext.ApiSchema)}"
    ];
    #endregion

    #region Test Types
    private class InitializeThrowsTest : JsonConverterTestBase<ApiSchema>
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

    private class JsonDeserializeTest : JsonDeserializeTest<ApiSchema, ApiSchemaDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonDeserializeTest()
        {
            this.ExpectedFactoryExpression = (arg) => BuildTestApiSchema(arg);
            this.ExcludeMembers = _excludeMembers;
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiSchema, ApiSchemaDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonRoundtripTest()
        {
            this.ExpectedFactoryExpression = (arg) => BuildTestApiSchema(arg);
            this.ExcludeMembers = _excludeMembers;
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiSchema, ApiSchemaDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonSerializeTest()
        {
            this.SourceFactoryExpression = (arg) => BuildTestApiSchema(arg);
        }
        #endregion
    }

    private class TryGetByApiNameTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
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
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
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

    private class TryGetByClrTypeTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
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
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
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
    public static TheoryDataRow<IXUnitTest>[] InitializeThrowsTheoryData =>
    [
        // ApiEnumType throws if ApiName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiName)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiEnumType Throws If ApiName Is Invalid"",
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
                ""ApiName"": ""ApiEnumType Throws If ClrType Is Null"",
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
                    code: ApiInitializationCode.API_TYPE_NULL_CLR_TYPE,
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
                ""ApiName"": ""ApiEnumType Throws If ClrType Is Not a CLR Enum"",
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
                ""ApiName"": ""ApiEnumType Throws If ApiEnumValues Is Null"",
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
                ""ApiName"": ""ApiEnumType Throws If ApiEnumValues Is Empty"",
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
                ""ApiName"": ""ApiEnumType Throws If ApiEnumValues Has Duplicate ApiName Values"",
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
                ""ApiName"": ""ApiEnumType Throws If ApiEnumValues Has Duplicate ClrName Values"",
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
                ""ApiName"": ""ApiEnumType Throws If ApiEnumValues Has Duplicate ClrOrdinal Values"",
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
        new JsonDeserializeTest
        {
            Name = "Null",
            SourceJson = "null",
            ExpectedFactoryArgument = null
        },

        // ApiSchema With 0 ApiType
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}"
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType And ApiSchemaOptions"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                )
            )
        },

        // ApiSchema With 1 ApiScalarType
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiScalarType And ApiSchemaOptions
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And ApiSchemaOptions"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
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
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiScalarType And GraphQlExtension
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And GraphQlExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    }
                }
            }",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 2 ApiScalarType
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 2 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 1 ApiEnumType
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiEnumType And ProtobufExtension
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType And ProtobufExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                    ""Evoogle.ApiFramework.Schema.TestData.ProtobufExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Edition"": 2024
                    }
                }
            }",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
                    ExtensionTypes: [ typeof(ProtobufExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies)
                                ),
                            ]
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies)
                                ),
                            ]
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarTypes And 1 ApiEnumType And 2 ApiObjectTypes (Company And Person)
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 2 ApiObjectType (Company And Person)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Employees""
                            }
                        ],
                        ""ApiRelationships"": [
                            {
                                ""ApiName"": ""Company_Owner"",
                                ""ApiPropertyName"": ""Owner""
                            },
                            {
                                ""ApiName"": ""Company_Employees"",
                                ""ApiPropertyName"": ""Employees""
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
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Company)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Company)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Owner),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Owner)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Employees),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<Person>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Employees)
                                ),
                            ],
                            ApiRelationships:
                            [
                                new ApiRelationshipConfig(ApiName: "Company_Owner", ApiPropertyName: nameof(Company.Owner)),
                                new ApiRelationshipConfig(ApiName: "Company_Employees", ApiPropertyName: nameof(Company.Employees))
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies)
                                ),
                            ]
                        )
                    ),
                ]
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        // Null
        new JsonRoundtripTest
        {
            Name = "Null",
            ExpectedFactoryArgument = null
        },

        // ApiSchema With 0 ApiType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}"
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                )
            )
        },

        // ApiSchema With 1 ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiScalarType And ApiSchemaOptions
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiScalarType And GraphQlExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 2 ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 1 ApiEnumType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiEnumType And ProtobufExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
                    ExtensionTypes: [ typeof(ProtobufExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies)
                                ),
                            ]
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies)
                                ),
                            ]
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarTypes And 1 ApiEnumType And 2 ApiObjectTypes (Company And Person)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Company)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Company)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Owner),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Owner)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Employees),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<Person>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Employees)
                                ),
                            ],
                            ApiRelationships:
                            [
                                new ApiRelationshipConfig(ApiName: "Company_Owner", ApiPropertyName: nameof(Company.Owner)),
                                new ApiRelationshipConfig(ApiName: "Company_Employees", ApiPropertyName: nameof(Company.Employees))
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies)
                                ),
                            ]
                        )
                    ),
                ]
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        // Null
        new JsonSerializeTest
        {
            Name = "Null",
            SourceFactoryArgument = null,
            ExpectedJson = "null"
        },

        // ApiSchema With 0 ApiType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}"
                )
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                )
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 0 ApiType And ApiSchemaOptions"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }"
        },

        // ApiSchema With 1 ApiScalarType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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

        // ApiSchema With 1 ApiScalarType And ApiSchemaOptions
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And ApiSchemaOptions"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
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

        // ApiSchema With 1 ApiScalarType And GraphQlExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiScalarType And GraphQlExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    }
                }
            }"
        },

        // ApiSchema With 2 ApiScalarType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 2 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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

        // ApiSchema With 3 ApiScalarType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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

        // ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }"
        },

        // ApiSchema With 1 ApiEnumType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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

        // ApiSchema With 1 ApiEnumType And ProtobufExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
                    ExtensionTypes: [ typeof(ProtobufExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 1 ApiEnumType And ProtobufExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                    ""Evoogle.ApiFramework.Schema.TestData.ProtobufExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Edition"": 2024
                    }
                }
            }"
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies)
                                ),
                            ]
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies)
                                ),
                            ]
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }"
        },

        // ApiSchema With 3 ApiScalarTypes And 1 ApiEnumType And 2 ApiObjectTypes (Company And Person)
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)",
            SourceFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Company)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Company)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Owner),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Owner)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Employees),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<Person>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Employees)
                                ),
                            ],
                            ApiRelationships:
                            [
                                new ApiRelationshipConfig(ApiName: "Company_Owner", ApiPropertyName: nameof(Company.Owner)),
                                new ApiRelationshipConfig(ApiName: "Company_Employees", ApiPropertyName: nameof(Company.Employees))
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            Kind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender)
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies)
                                ),
                            ]
                        )
                    ),
                ]
            ),
            ExpectedJson = @"
            {
                ""ApiName"": ""ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 2 ApiObjectType (Company And Person)"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ReturnEmpty""
                },
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
                                        ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Employees""
                            }
                        ],
                        ""ApiRelationships"": [
                            {
                                ""ApiName"": ""Company_Owner"",
                                ""ApiPropertyName"": ""Owner""
                            },
                            {
                                ""ApiName"": ""Company_Employees"",
                                ""ApiPropertyName"": ""Employees""
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
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "UnknownType",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiEnumType)} with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "Gender",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiEnumType)} with case-insensitive search (uppercase)",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "GENDER",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiObjectType)} with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "ScalarsOnly",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiObjectType)} with case-insensitive search (lowercase)",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "scalarsonly",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiScalarType)} with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "Boolean",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiScalarType)} with case-insensitive search (uppercase)",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "BOOLEAN",
            ExpectedResult = false
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetByClrTypeTheoryData =>
    [
        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns false when {nameof(ApiType)} is not registered in schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = typeof(Order),
            ExpectedResult = false
        },

        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns true for registered {nameof(ApiEnumType)} CLR type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = typeof(Gender),
            ExpectedResult = true
        },

        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns true for registered {nameof(ApiObjectType)} CLR type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = typeof(ScalarsOnly),
            ExpectedResult = true
        },

        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns true for registered {nameof(ApiScalarType)} CLR type",
            ApiSchemaKind = ApiSchemaKind.Simple,
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
