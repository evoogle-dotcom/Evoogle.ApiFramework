// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Fields
    private static readonly List<string> _excludeMembers =
    [
        $"{nameof(ApiSchemaContext)}",
        $"{nameof(ApiSchemaContext)}.{nameof(ApiSchemaContext.ApiSchema)}"
    ];
    #endregion

    #region Test Types
    public ref struct TypesWithRefStructMembers
    {
        // This will trigger API_PROPERTY_INVALID_CLR_MEMBER
        public Span<byte> SpanField;

        // This will trigger API_PROPERTY_INVALID_CLR_MEMBER
        public Span<byte> SpanProperty { get; set; }
    }

    public unsafe class TypesWithPointerMembers
    {
        // This will trigger API_PROPERTY_INVALID_FIELD_GETTER/SETTER
        public byte* PointerField;

        // This will trigger API_PROPERTY_INVALID_PROPERTY_GETTER/SETTER
        public byte* PointerProperty { get; set; }
    }

    public class CircularIdentityNodeType
    {
        public CircularIdentityNodeType? Self { get; set; }
    }

    public class CircularIdentityAlphaType
    {
        public CircularIdentityBetaType? Beta { get; set; }
    }

    public class CircularIdentityBetaType
    {
        public CircularIdentityAlphaType? Alpha { get; set; }
    }

    public class DuplicatePropertyApiNameType
    {
        public string Name { get; set; } = string.Empty;
        public string NameAlt { get; set; } = string.Empty;
    }

    public class DuplicatePropertyClrNameType
    {
        public string Name { get; set; } = string.Empty;
    }

    public class DuplicateIdentityApiNameType
    {
        public int Id { get; set; }
        public int Code { get; set; }
    }

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
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] InitializeThrowsTheoryData =>
    [
        //
        // ApiEnumType Initialization Tests
        //

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
                        ""ApiKind"": ""Enum"",
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
                    path: $"{nameof(ApiEnumType)}",
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
                        ""ApiKind"": ""Enum"",
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
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
                        ""ApiKind"": ""Enum"",
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
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
                        ""ApiKind"": ""Enum"",
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
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
                        ""ApiKind"": ""Enum"",
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
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
                        ""ApiKind"": ""Enum"",
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
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
                        ""ApiKind"": ""Enum"",
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
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
                        ""ApiKind"": ""Enum"",
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_TYPE_DUPLICATE_VALUE_CLR_ORDINAL,
                    description: $"Duplicate {nameof(ApiEnumValue)}.{nameof(ApiEnumValue.ClrOrdinal)} values: '2'",
                    remediation: $"Verify that each {nameof(ApiEnumValue)} has a unique {nameof(ApiEnumValue.ClrOrdinal)} value"
                ),
            ]
        },

        //
        // ApiEnumValue Initialization Tests
        //

        // ApiEnumValue throws if ApiName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumValue)} Throws If {nameof(ApiEnumValue.ApiName)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiEnumValue Throws If ApiName Is Invalid"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""Unspecified"",
                                ""ClrName"": ""Unspecified"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": """",
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumValue)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_VALUE_INVALID_API_NAME,
                    description: $"{nameof(ApiEnumValue.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiEnumValue.ApiName)} value"
                ),
            ]
        },

        // ApiEnumValue throws if ClrName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumValue)} Throws If {nameof(ApiEnumValue.ClrName)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiEnumValue Throws If ClrName Is Invalid"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""Unspecified"",
                                ""ClrName"": ""Unspecified"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""Male"",
                                ""ClrName"": """",
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumValue)}[\"{nameof(Gender.Male)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_VALUE_INVALID_CLR_NAME,
                    description: $"{nameof(ApiEnumValue.ClrName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiEnumValue.ClrName)} value"
                ),
            ]
        },

        //
        // ApiScalarType Initialization Tests
        //

        // ApiScalarType throws if ApiName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiScalarType)} Throws If {nameof(ApiScalarType.ApiName)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiScalarType Throws If ApiName Is Invalid"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": """",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiScalarType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_NAMED_TYPE_INVALID_API_NAME,
                    description: $"{nameof(ApiScalarType.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiScalarType.ApiName)} value"
                ),
            ]
        },

        // ApiScalarType throws if ClrType is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiScalarType)} Throws If {nameof(ApiScalarType.ClrType)} Is Null",
            Source = @"
            {
                ""ApiName"": ""ApiScalarType Throws If ClrType Is Null"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiScalarType)}[\"String\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_TYPE_NULL_CLR_TYPE,
                    description: $"{nameof(ApiScalarType.ClrType)} must not be null",
                    remediation: $"Specify a valid {nameof(ApiScalarType.ClrType)}"
                ),
            ]
        },

        //
        // ApiProperty Initialization Tests
        //

        // ApiProperty throws if ApiName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ApiName)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiProperty Throws If ApiName Is Invalid"",
                ""ApiScalarTypes"": [
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
                        ""ApiName"": ""ScalarsOnly"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": """",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""RequiredName"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_INVALID_API_NAME,
                    description: $"{nameof(ApiProperty.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiProperty.ApiName)} value"
                ),
            ]
        },

        // ApiProperty throws if ClrName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ClrName)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiProperty Throws If ClrName Is Invalid"",
                ""ApiScalarTypes"": [
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
                        ""ApiName"": ""ScalarsOnly"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""RequiredName"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": """",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_INVALID_CLR_NAME,
                    description: $"{nameof(ApiProperty.ClrName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiProperty.ClrName)} value"
                ),
            ]
        },

        // ApiProperty throws if CLR member is missing
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If CLR Member Is Missing",
            Source = @"
            {
                ""ApiName"": ""ApiProperty Throws If CLR Member Is Missing"",
                ""ApiScalarTypes"": [
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
                        ""ApiName"": ""ScalarsOnly"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NonExistent"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NonExistentProperty"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"NonExistent\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_MISSING_CLR_MEMBER,
                    description: $"CLR member 'NonExistentProperty' was not found on CLR type '{nameof(ScalarsOnly)}'",
                    remediation: $"Add a public CLR property or field named 'NonExistentProperty' to CLR type '{nameof(ScalarsOnly)}'"
                ),
            ]
        },

        // ApiProperty throws if Type is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ApiType)} Type Is Null",
            Source = @"
            {
                ""ApiName"": ""ApiProperty Throws If Type Is Null"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""ScalarsOnly"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""RequiredName"",
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""RequiredName"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_NULL_TYPE,
                    description: $"{nameof(ApiProperty.ApiType)} must not be null",
                    remediation: $"Specify a valid {nameof(ApiProperty.ApiType)}"
                ),
            ]
        },

        // ApiProperty throws if Type is an invalid CLR member
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If Type Is An Invalid CLR Member",
            Source = @"
            {
                ""ApiName"": ""ApiProperty Throws If Type Is An Invalid CLR Member"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Byte"",
                        ""ClrType"": ""System.Byte, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""TypesWithRefStructMembers"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""SpanField"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Scalar"",
                                            ""ApiName"": ""Byte""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Span\u00601[[System.Byte, System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""SpanField"",
                                ""ClrMemberKind"": ""Field""
                            },
                            {
                                ""ApiName"": ""SpanProperty"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Scalar"",
                                            ""ApiName"": ""Byte""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Span\u00601[[System.Byte, System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""SpanProperty"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests\u002BTypesWithRefStructMembers, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithRefStructMembers)}\"].{nameof(ApiProperty)}[\"SpanField\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_INVALID_CLR_MEMBER,
                    description: $"CLR member '{nameof(TypesWithRefStructMembers.SpanField)}' has type '{typeof(Span<byte>).SafeToName()}' which is a ref struct. Ref structs cannot be boxed to object and are not supported for API properties.",
                    remediation: $"Change the type of CLR member '{nameof(TypesWithRefStructMembers.SpanField)}' to a non-ref struct type."
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithRefStructMembers)}\"].{nameof(ApiProperty)}[\"SpanProperty\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_INVALID_CLR_MEMBER,
                    description: $"CLR member '{nameof(TypesWithRefStructMembers.SpanProperty)}' has type '{typeof(Span<byte>).SafeToName()}' which is a ref struct. Ref structs cannot be boxed to object and are not supported for API properties.",
                    remediation: $"Change the type of CLR member '{nameof(TypesWithRefStructMembers.SpanProperty)}' to a non-ref struct type."
                ),
            ]
        },

        // ApiProperty throws if Type is unresolved
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ApiType)} Is Unresolved Because Api Named Reference Type Does Not Exist",
            Source = @"
            {
                ""ApiName"": ""ApiProperty Throws If Type Is Unresolved Because Api Named Reference Type Does Not Exist"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""ScalarsOnly"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""RequiredName"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""RequiredName"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_UNRESOLVED_TYPE,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved for {nameof(ApiTypeExpression.ApiKind)}='Scalar' and {nameof(ApiTypeExpression.ApiName)}='String'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ApiKind)}='Scalar' and {nameof(ApiTypeExpression.ApiName)}='String'"
                ),
            ]
        },

        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ApiType)} Is Unresolved Because CLR Reference Type Does Not Exist",
            Source = @"
            {
                ""ApiName"": ""ApiProperty Throws If Type Is Unresolved Because CLR Reference Type Does Not Exist"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""ScalarsOnly"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""RequiredName"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""RequiredName"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_UNRESOLVED_TYPE,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'"
                ),
            ]
        },

        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ApiType)} Is Unresolved Because Type Reference Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiProperty Throws If Type Is Unresolved Because Type Reference Is Invalid"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""ScalarsOnly"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""RequiredName"",
                                ""ApiType"": {},
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""RequiredName"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_UNRESOLVED_TYPE,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved because none of the following are set: {nameof(ApiTypeExpression.ApiInlineType)}, a valid combination of {nameof(ApiTypeExpression.ApiKind)} and {nameof(ApiTypeExpression.ApiName)}, or {nameof(ApiTypeExpression.ClrType)}",
                    remediation: $"Specify either {nameof(ApiTypeExpression.ApiInlineType)}, a valid combination of {nameof(ApiTypeExpression.ApiKind)} and {nameof(ApiTypeExpression.ApiName)}, or {nameof(ApiTypeExpression.ClrType)}"
                ),
            ]
        },

        // ApiProperty throws if unable to get or set field/property value
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If Unable To Get Or Set Field/Property Value",
            Source = @"
            {
                ""ApiName"": ""ApiProperty Throws If Unable To Get Or Set Field/Property Value"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Byte"",
                        ""ClrType"": ""System.Byte, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""TypesWithPointerMembers"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""PointerField"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Byte""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""PointerField"",
                                ""ClrMemberKind"": ""Field""
                            },
                            {
                                ""ApiName"": ""PointerProperty"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Byte""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""PointerProperty"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests\u002BTypesWithPointerMembers, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=4, Errors=4, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithPointerMembers)}\"].{nameof(ApiProperty)}[\"PointerField\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_INVALID_FIELD_GETTER,
                    description: $"Failed to compile field getter for '{nameof(TypesWithPointerMembers.PointerField)}': No coercion operator is defined between types 'System.Byte*' and 'System.Object'",
                    remediation: $"Verify that field '{nameof(TypesWithPointerMembers.PointerField)}' is readable and can be used in expression trees"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithPointerMembers)}\"].{nameof(ApiProperty)}[\"PointerField\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_INVALID_FIELD_SETTER,
                    description: $"Failed to compile field setter for '{nameof(TypesWithPointerMembers.PointerField)}': Type must not be a pointer type (Parameter 'type')",
                    remediation: $"Verify that field '{nameof(TypesWithPointerMembers.PointerField)}' is writable and can be used in expression trees"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithPointerMembers)}\"].{nameof(ApiProperty)}[\"PointerProperty\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_INVALID_PROPERTY_GETTER,
                    description: $"Failed to compile property getter for '{nameof(TypesWithPointerMembers.PointerProperty)}': No coercion operator is defined between types 'System.Byte*' and 'System.Object'",
                    remediation: $"Verify that property '{nameof(TypesWithPointerMembers.PointerProperty)}' is readable and can be used in expression trees"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithPointerMembers)}\"].{nameof(ApiProperty)}[\"PointerProperty\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_INVALID_PROPERTY_SETTER,
                    description: $"Failed to compile property setter for '{nameof(TypesWithPointerMembers.PointerProperty)}': Type must not be a pointer type (Parameter 'type')",
                    remediation: $"Verify that property '{nameof(TypesWithPointerMembers.PointerProperty)}' is writable and can be used in expression trees"
                ),
            ]
        },

        //
        // ApiObjectType Initialization Tests
        //

        // ApiObjectType throws if ApiProperties is null or empty
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiObjectType)} Throws If ApiProperties Is Null Or Empty",
            Source = @"
            {
                ""ApiName"": ""ApiObjectType Throws If ApiProperties Is Null Or Empty"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Empty"",
                        ""ApiProperties"": [],
                        ""ClrType"": null
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=1, Warnings=1.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"Empty\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_TYPE_NULL_CLR_TYPE,
                    description: $"{nameof(ApiObjectType.ClrType)} must not be null",
                    remediation: "Specify a valid ClrType"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"Empty\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_OBJECT_TYPE_NULL_OR_EMPTY_PROPERTIES,
                    description: $"{nameof(ApiObjectType.ApiProperties)} is null or empty",
                    remediation: null
                ),
            ]
        },

        // ApiObjectType throws if ApiProperties has duplicate ApiName
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiObjectType)} Throws If ApiProperties Has Duplicate ApiName",
            Source = @"
            {
                ""ApiName"": ""ApiObjectType Throws If ApiProperties Has Duplicate ApiName"",
                ""ApiScalarTypes"": [
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
                        ""ApiName"": ""TestObject"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ClrName"": ""NameAlt""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicatePropertyApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_PROPERTY_API_NAME,
                    description: $"Duplicate {nameof(ApiProperty)}.{nameof(ApiProperty.ApiName)} values: 'Name'",
                    remediation: $"Verify that each {nameof(ApiProperty)} has a unique {nameof(ApiProperty.ApiName)} value"
                ),
            ]
        },

        // ApiObjectType throws if ApiProperties has duplicate ClrName
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiObjectType)} Throws If ApiProperties Has Duplicate ClrName",
            Source = @"
            {
                ""ApiName"": ""ApiObjectType Throws If ApiProperties Has Duplicate ClrName"",
                ""ApiScalarTypes"": [
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
                        ""ApiName"": ""TestObject"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""NameAlias"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ClrName"": ""Name""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicatePropertyClrNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_PROPERTY_CLR_NAME,
                    description: $"Duplicate {nameof(ApiProperty)}.{nameof(ApiProperty.ClrName)} values: 'Name'",
                    remediation: $"Verify that each {nameof(ApiProperty)} has a unique {nameof(ApiProperty.ClrName)} value"
                ),
            ]
        },

        // ApiObjectType throws if ApiIdentities has duplicate ApiName
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiObjectType)} Throws If ApiIdentities Has Duplicate ApiName",
            Source = @"
            {
                ""ApiName"": ""ApiObjectType Throws If ApiIdentities Has Duplicate ApiName"",
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
                        ""ApiName"": ""TestObject"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""Code""
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
                                ""ClrName"": ""Id""
                            },
                            {
                                ""ApiName"": ""Code"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ClrName"": ""Code""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests\u002BDuplicateIdentityApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_IDENTITY_API_NAME,
                    description: $"Duplicate {nameof(ApiIdentity)}.{nameof(ApiIdentity.ApiName)} values: 'Primary'",
                    remediation: $"Verify that each {nameof(ApiIdentity)} has a unique {nameof(ApiIdentity.ApiName)} value"
                ),
            ]
        },

        // ApiObjectType throws if ApiIdentities has duplicate ApiPropertyName sets
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiObjectType)} Throws If ApiIdentities Has Duplicate ApiPropertyName Sets",
            Source = @"
            {
                ""ApiName"": ""ApiObjectType Throws If ApiIdentities Has Duplicate ApiPropertyName Sets"",
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
                        ""ApiName"": ""TestObject"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""ById"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""Id""
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
                                ""ClrName"": ""Id""
                            }
                        ],
                        ""ClrType"": null
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=1, Warnings=1.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_TYPE_NULL_CLR_TYPE,
                    remediation: "Specify a valid ClrType",
                    description: $"{nameof(ApiObjectType.ClrType)} must not be null"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_OBJECT_TYPE_AMBIGUOUS_IDENTITIES,
                    description: "Identities 'ById' and 'Primary' use the same property set [Id], which may cause ambiguity",
                    remediation: "Consider using different property combinations for each identity, or remove one of the duplicate identities"
                ),
            ]
        },

        //
        // ApiIdentity Initialization Tests
        //

        // ApiIdentity throws if ApiName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentity)} Throws If {nameof(ApiIdentity.ApiName)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiIdentity Throws If ApiName Is Invalid"",
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
                        ""ApiName"": ""TestObject"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": """",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""WarehouseId""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""WarehouseId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""WarehouseId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ProductInventory, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiIdentity)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_INVALID_API_NAME,
                    description: $"{nameof(ApiIdentity.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiIdentity.ApiName)} value"
                ),
            ]
        },

        // ApiIdentity throws if ApiIdentityParts is empty
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentity)} Throws If {nameof(ApiIdentity.ApiIdentityParts)} Is Empty",
            Source = @"
            {
                ""ApiName"": ""ApiIdentity Throws If ApiIdentityParts Is Empty"",
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
                        ""ApiName"": ""TestObject"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": []
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""WarehouseId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""WarehouseId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ProductInventory, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiIdentity)}[\"Primary\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_NULL_OR_EMPTY_PARTS,
                    description: $"{nameof(ApiIdentity.ApiIdentityParts)} must not be null or empty",
                    remediation: $"Specify at least one {nameof(ApiIdentityPart)}"
                ),
            ]
        },

        // ApiIdentity throws if ApiIdentityParts has duplicate ApiPropertyName values
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentity)} Throws If {nameof(ApiIdentity.ApiIdentityParts)} Has Duplicate {nameof(ApiIdentityPart.ApiPropertyName)} Values",
            Source = @"
            {
                ""ApiName"": ""ApiIdentity Throws If ApiIdentityParts Has Duplicate ApiPropertyName Values"",
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
                        ""ApiName"": ""TestObject"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""WarehouseId""
                                    },
                                    {
                                        ""ApiPropertyName"": ""WarehouseId""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""WarehouseId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""WarehouseId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ProductInventory, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiIdentity)}[\"Primary\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_DUPLICATE_PART_API_PROPERTY_NAME,
                    description: $"Duplicate {nameof(ApiIdentityPart)}.{nameof(ApiIdentityPart.ApiPropertyName)} values: 'WarehouseId'",
                    remediation: $"Verify that each {nameof(ApiIdentityPart)} has a unique {nameof(ApiIdentityPart.ApiPropertyName)} value"
                ),
            ]
        },

        //
        // ApiIdentityPart Initialization Tests
        //

        // ApiIdentityPart throws if ApiPropertyName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityPart)} Throws If {nameof(ApiIdentityPart.ApiPropertyName)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiIdentityPart Throws If ApiPropertyName Is Invalid"",
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
                        ""ApiName"": ""TestObject"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": """"
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""WarehouseId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""WarehouseId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ProductInventory, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiIdentity)}[\"Primary\"].{nameof(ApiIdentityPart)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_INVALID_API_PROPERTY_NAME,
                    description: $"{nameof(ApiIdentityPart.ApiPropertyName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiIdentityPart.ApiPropertyName)} value"
                ),
            ]
        },

        // ApiIdentityPart throws if ApiPropertyName is unresolved
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityPart)} Throws If {nameof(ApiIdentityPart.ApiPropertyName)} Is Unresolved",
            Source = @"
            {
                ""ApiName"": ""ApiIdentityPart Throws If ApiPropertyName Is Unresolved"",
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
                        ""ApiName"": ""TestObject"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""DoesNotExist""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""WarehouseId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""WarehouseId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ProductInventory, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiIdentity)}[\"Primary\"].{nameof(ApiIdentityPart)}[\"DoesNotExist\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_PROPERTY,
                    description: $"{nameof(ApiIdentityPart.ApiProperty)} could not be resolved for {nameof(ApiIdentityPart.ApiPropertyName)}='DoesNotExist'",
                    remediation: $"Verify that {nameof(ApiIdentityPart.ApiPropertyName)} refers to a valid property on the parent {nameof(ApiObjectType)}"
                ),
            ]
        },

        // ApiIdentityPart throws if ClrConfiguredIdType is not ApiId compatible
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityPart)} Throws If {nameof(ApiIdentityPart.ClrConfiguredIdType)} Is Invalid",
            Source = @"
            {
                ""ApiName"": ""ApiIdentityPart Throws If ClrConfiguredIdType Is Invalid"",
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
                        ""ApiName"": ""TestObject"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""WarehouseId"",
                                        ""ClrConfiguredIdType"": ""System.Net.Sockets.Socket, System.Net.Sockets""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""WarehouseId"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""Int32""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""WarehouseId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ProductInventory, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiIdentity)}[\"Primary\"].{nameof(ApiIdentityPart)}[\"WarehouseId\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_INVALID_SCALAR_TYPE,
                    description: $"Scalar type 'Socket' is not compatible with {nameof(ApiId)}",
                    remediation: $"Use one of the supported scalar types: {string.Join(",", ApiId.GetCompatibleScalarTypes().Select(t => t.SafeToName()).OrderBy(n => n, StringComparer.Ordinal))}"
                ),
            ]
        },

        // ApiIdentityPart warns if identity part requires string parsing for a non-string scalar type
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityPart)} Warns If {nameof(String)} Requires Coercion",
            Source = @"
            {
                ""ApiName"": ""ApiIdentityPart Warns If String Requires Coercion"",
                ""ApiScalarTypes"": [
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
                        ""ApiName"": ""TestObject"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": """",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""ProductCode"",
                                        ""ClrConfiguredIdType"": ""System.Guid, System.Private.CoreLib""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""ProductCode"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""ProductCode"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ProductInventory, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=1, Warnings=1.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiIdentity)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_INVALID_API_NAME,
                    description: $"{nameof(ApiIdentity.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiIdentity.ApiName)} value"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiIdentity)}.{nameof(ApiIdentityPart)}[\"ProductCode\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_IDENTITY_PART_PERFORMANCE_CONCERN,
                    description: $"Identity part 'ProductCode' requires type coercion between {nameof(String)} and Guid, which may impact performance",
                    remediation: "Consider using Guid as the property type directly, or be aware of the parsing overhead"
                ),
            ]
        },

        // ApiIdentityPart throws if identity part references a type that creates a circular dependency
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityPart)} Throws If Identity Reference Is Circular",
            Source = @"
            {
                ""ApiName"": ""ApiIdentityPart Throws If Identity Reference Is Circular"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Node"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""Self""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Self"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""Node""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Self"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+CircularIdentityNodeType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"Node\"].{nameof(ApiIdentity)}[\"Primary\"].{nameof(ApiIdentityPart)}[\"Self\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_CIRCULAR_REFERENCE,
                    description: "Circular identity reference detected: property 'Self' references type 'Node' which has an identity that depends on the current type",
                    remediation: "Remove the circular dependency by restructuring the identity definitions or using a non-identity property"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"Node\"].{nameof(ApiIdentity)}[\"Primary\"].{nameof(ApiIdentityPart)}[\"Self\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_PROPERTY,
                    description: $"{nameof(ApiIdentityPart.ApiProperty)} could not be resolved for {nameof(ApiIdentityPart.ApiPropertyName)}='Self'",
                    remediation: $"Verify that {nameof(ApiIdentityPart.ApiPropertyName)} refers to a valid property on the parent {nameof(ApiObjectType)}"
                ),
            ]
        },

        // ApiIdentityPart throws if two object types reference each other in their identities
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityPart)} Throws If Two Identity References Are Circular",
            Source = @"
            {
                ""ApiName"": ""ApiIdentityPart Throws If Two Identity References Are Circular"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Alpha"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""Beta""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Beta"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""Beta""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Beta"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+CircularIdentityAlphaType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Beta"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiPropertyName"": ""Alpha""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Alpha"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""Alpha""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Alpha"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+CircularIdentityBetaType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"Beta\"].{nameof(ApiIdentity)}[\"Primary\"].{nameof(ApiIdentityPart)}[\"Alpha\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_CIRCULAR_REFERENCE,
                    description: "Circular identity reference detected: property 'Alpha' references type 'Alpha' which has an identity that depends on the current type",
                    remediation: "Remove the circular dependency by restructuring the identity definitions or using a non-identity property"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"Beta\"].{nameof(ApiIdentity)}[\"Primary\"].{nameof(ApiIdentityPart)}[\"Alpha\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_PROPERTY,
                    description: $"{nameof(ApiIdentityPart.ApiProperty)} could not be resolved for {nameof(ApiIdentityPart.ApiPropertyName)}='Alpha'",
                    remediation: $"Verify that {nameof(ApiIdentityPart.ApiPropertyName)} refers to a valid property on the parent {nameof(ApiObjectType)}"
                ),
            ]
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(InitializeThrowsTheoryData))]
    public void InitializeThrows(IXUnitTest test) => test.Execute(this);
    #endregion
}
