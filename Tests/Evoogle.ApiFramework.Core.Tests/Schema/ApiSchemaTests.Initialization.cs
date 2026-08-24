// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Test Types
    public ref struct TypesWithRefStructMembers
    {
        // This will trigger ApiPropertyInvalidClrMember
        public Span<byte> SpanField;

        // This will trigger ApiPropertyInvalidClrMember
        public Span<byte> SpanProperty { get; set; }
    }

    public unsafe class TypesWithPointerMembers
    {
        // This will trigger ApiPropertyInvalidFieldGetter/SETTER
        public byte* PointerField;

        // This will trigger ApiPropertyInvalidPropertyGetter/SETTER
        public byte* PointerProperty { get; set; }
    }

    public class CircularKeyNodeType
    {
        public CircularKeyNodeType? Self { get; set; }
    }

    public class CircularKeyAlphaType
    {
        public CircularKeyBetaType? Beta { get; set; }
    }

    public class CircularKeyBetaType
    {
        public CircularKeyAlphaType? Alpha { get; set; }
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

    public class DuplicateKeyTypeApiNameType
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ExternalCode { get; set; } = string.Empty;
    }

    public class TypeWithListProperty
    {
        public List<string>? Items { get; set; }
    }

    public class OwnedType
    {
        public int Id { get; set; }
    }

    public class OwnerType
    {
        public int Id { get; set; }
        public OwnedType? Item { get; set; }
    }

    public class OwnerTypeA
    {
        public int Id { get; set; }
        public OwnedType? Item { get; set; }
    }

    public class OwnerTypeB
    {
        public int Id { get; set; }
        public OwnedType? Item { get; set; }
    }

    public class RelPrincipalType
    {
        public int Id { get; set; }
    }

    public class RelPrincipalBType
    {
        public int Id { get; set; }
    }

    public class RelDependentType
    {
        public int PrincipalId { get; set; }
        public int PrincipalId2 { get; set; }
        public string PrincipalCode { get; set; } = string.Empty;
    }

    public class RelAssociationType
    {
        public int PrincipalAId { get; set; }
        public string PrincipalACode { get; set; } = string.Empty;
        public int PrincipalBId { get; set; }
    }

    public class RelNestedType
    {
        public int Id { get; set; }
    }

    public class RelDependentWithObjectType
    {
        public int PrincipalId { get; set; }
        public RelNestedType? Nested { get; set; }
    }

    // CLR types for the key initialization order regression test.
    // AlphaKeyType sorts before ZetaKeyType alphabetically; its composite key navigates through ZetaKeyType.
    // Under the original single-pass initialization, AlphaKeyType's key would be initialized before
    // ZetaKeyType's property lookups were populated, causing an ApiSchemaException.
    public class ZetaKeyType
    {
        public int Id { get; set; }
    }

    public class AlphaKeyType
    {
        public ZetaKeyType? ZetaRef { get; set; }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] InitializeThrowsTheoryData =>
    [
        //
        // ApiCollectionType Initialization Tests
        //

        // ApiCollectionType throws if item type expression is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiCollectionType)} Throws If {nameof(ApiCollectionType.ApiItemType)} Is Null",
            SourceJson = @"
            {
                ""ApiName"": ""ApiCollectionType Throws If ApiItemType Is Null"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""TestObject"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Items"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ClrName"": ""Items""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+TypeWithListProperty, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiProperty)}[\"Items\"].{nameof(ApiCollectionType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiCollectionTypeNullItemType,
                    description: $"{nameof(ApiCollectionType.ApiItemType)} must not be null",
                    remediation: $"Specify a valid {nameof(ApiCollectionType.ApiItemType)}"
                ),
            ]
        },

        // ApiCollectionType throws if item type expression cannot be resolved
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiCollectionType)} Throws If {nameof(ApiCollectionType.ApiItemType)} Is Unresolved",
            SourceJson = @"
            {
                ""ApiName"": ""ApiCollectionType Throws If ApiItemType Is Unresolved"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""TestObject"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Items"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": {
                                            ""ApiKind"": ""Scalar"",
                                            ""ApiName"": ""String""
                                        },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ClrName"": ""Items""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+TypeWithListProperty, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiProperty)}[\"Items\"].{nameof(ApiCollectionType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiCollectionTypeUnresolvedItemType,
                    description: $"{nameof(ApiCollectionType.ApiItemType)} could not be resolved for {nameof(ApiTypeExpression.ApiKind)}='{ApiTypeKind.Scalar}' and {nameof(ApiTypeExpression.ApiName)}='String'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ApiKind)}='{ApiTypeKind.Scalar}' and {nameof(ApiTypeExpression.ApiName)}='String'"
                ),
            ]
        },

        //
        // ApiEnumType Initialization Tests
        //

        // ApiEnumType throws if ApiName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiName)} Is Invalid",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiEnumType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiNamedTypeInvalidApiName,
                    description: $"{nameof(ApiEnumType.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiEnumType.ApiName)} value"
                ),
            ]
        },

        // ApiEnumType throws if ClrType is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ClrType)} Is Null",
            SourceJson = @"
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
                    apiPath: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiTypeNullClrType,
                    description: $"{nameof(ApiEnumType.ClrType)} must not be null",
                    remediation: $"Specify a valid {nameof(ApiEnumType.ClrType)}"
                ),
            ]
        },

        // ApiEnumType throws if ClrType is not a CLR Enum
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ClrType)} Is Not a CLR Enum",
            SourceJson = @"
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
                    apiPath: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiEnumTypeInvalidClrType,
                    description: $"{nameof(ApiEnumType.ClrType)} 'String' must be a CLR Enum",
                    remediation: $"Set {nameof(ApiEnumType.ClrType)} to a CLR Enum type"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Is Null",
            SourceJson = @"
            {
                ""ApiName"": ""ApiEnumType Throws If ApiEnumValues Is Null"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiEnumTypeNullOrEmptyValues,
                    description: $"{nameof(ApiEnumType.ApiEnumValues)} must not be null or empty",
                    remediation: $"Define at least one {nameof(ApiEnumValue)}"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues is emtpy
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Is Empty",
            SourceJson = @"
            {
                ""ApiName"": ""ApiEnumType Throws If ApiEnumValues Is Empty"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiEnumTypeNullOrEmptyValues,
                    description: $"{nameof(ApiEnumType.ApiEnumValues)} must not be null or empty",
                    remediation: $"Define at least one {nameof(ApiEnumValue)}"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues has duplicate ApiName values
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Has Duplicate {nameof(ApiEnumValue.ApiName)} Values",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiEnumTypeDuplicateValueApiName,
                    description: $"Duplicate {nameof(ApiEnumValue)}.{nameof(ApiEnumValue.ApiName)} values: 'Female'",
                    remediation: $"Verify that each {nameof(ApiEnumValue)} has a unique {nameof(ApiEnumValue.ApiName)} value"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues has duplicate ClrName values
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Has Duplicate {nameof(ApiEnumValue.ClrName)} Values",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiEnumTypeDuplicateValueClrName,
                    description: $"Duplicate {nameof(ApiEnumValue)}.{nameof(ApiEnumValue.ClrName)} values: 'Female'",
                    remediation: $"Verify that each {nameof(ApiEnumValue)} has a unique {nameof(ApiEnumValue.ClrName)} value"
                ),
            ]
        },

        // ApiEnumType throws if ApiEnumValues has duplicate ClrOrdinal values
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumType)} Throws If {nameof(ApiEnumType.ApiEnumValues)} Has Duplicate {nameof(ApiEnumValue.ClrOrdinal)} Values",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiEnumTypeDuplicateValueClrOrdinal,
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
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumValue)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiEnumValueInvalidApiName,
                    description: $"{nameof(ApiEnumValue.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiEnumValue.ApiName)} value"
                ),
            ]
        },

        // ApiEnumValue throws if ClrName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiEnumValue)} Throws If {nameof(ApiEnumValue.ClrName)} Is Invalid",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumValue)}[\"{nameof(Gender.Male)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiEnumValueInvalidClrName,
                    description: $"{nameof(ApiEnumValue.ClrName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiEnumValue.ClrName)} value"
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
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyInvalidApiName,
                    description: $"{nameof(ApiProperty.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiProperty.ApiName)} value"
                ),
            ]
        },

        // ApiProperty throws if ClrName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ClrName)} Is Invalid",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyInvalidClrName,
                    description: $"{nameof(ApiProperty.ClrName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiProperty.ClrName)} value"
                ),
            ]
        },

        // ApiProperty throws if CLR member is missing
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If CLR Member Is Missing",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"NonExistent\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyMissingClrMember,
                    description: $"CLR member 'NonExistentProperty' was not found on CLR type '{nameof(ScalarsOnly)}'",
                    remediation: $"Add a public CLR property or field named 'NonExistentProperty' to CLR type '{nameof(ScalarsOnly)}'"
                ),
            ]
        },

        // ApiProperty throws if Type is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ApiType)} Type Is Null",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyNullType,
                    description: $"{nameof(ApiProperty.ApiType)} must not be null",
                    remediation: $"Specify a valid {nameof(ApiProperty.ApiType)}"
                ),
            ]
        },

        // ApiProperty throws if Type is an invalid CLR member
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If Type Is An Invalid CLR Member",
            SourceJson = @"
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
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithRefStructMembers)}\"].{nameof(ApiProperty)}[\"SpanField\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyInvalidClrMember,
                    description: $"CLR member '{nameof(TypesWithRefStructMembers.SpanField)}' has type '{typeof(Span<byte>).SafeToName()}' which is a ref struct. Ref structs cannot be boxed to object and are not supported for API properties.",
                    remediation: $"Change the type of CLR member '{nameof(TypesWithRefStructMembers.SpanField)}' to a non-ref struct type."
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithRefStructMembers)}\"].{nameof(ApiProperty)}[\"SpanProperty\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyInvalidClrMember,
                    description: $"CLR member '{nameof(TypesWithRefStructMembers.SpanProperty)}' has type '{typeof(Span<byte>).SafeToName()}' which is a ref struct. Ref structs cannot be boxed to object and are not supported for API properties.",
                    remediation: $"Change the type of CLR member '{nameof(TypesWithRefStructMembers.SpanProperty)}' to a non-ref struct type."
                ),
            ]
        },

        // ApiProperty throws if Type is unresolved
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ApiType)} Is Unresolved Because Api Named Reference Type Does Not Exist",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyUnresolvedType,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved for {nameof(ApiTypeExpression.ApiKind)}='Scalar' and {nameof(ApiTypeExpression.ApiName)}='String'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ApiKind)}='Scalar' and {nameof(ApiTypeExpression.ApiName)}='String'"
                ),
            ]
        },

        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ApiType)} Is Unresolved Because CLR Reference Type Does Not Exist",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyUnresolvedType,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'"
                ),
            ]
        },

        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If {nameof(ApiProperty.ApiType)} Is Unresolved Because Type Reference Is Invalid",
            SourceJson = @"
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
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"RequiredName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyUnresolvedType,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved because none of the following are set: {nameof(ApiTypeExpression.ApiInlineType)}, a valid combination of {nameof(ApiTypeExpression.ApiKind)} and {nameof(ApiTypeExpression.ApiName)}, or {nameof(ApiTypeExpression.ClrType)}",
                    remediation: $"Specify either {nameof(ApiTypeExpression.ApiInlineType)}, a valid combination of {nameof(ApiTypeExpression.ApiKind)} and {nameof(ApiTypeExpression.ApiName)}, or {nameof(ApiTypeExpression.ClrType)}"
                ),
            ]
        },

        // ApiProperty throws if unable to get or set field/property value
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiProperty)} Throws If Unable To Get Or Set Field/Property Value",
            SourceJson = @"
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
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithPointerMembers)}\"].{nameof(ApiProperty)}[\"PointerField\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyInvalidFieldGetter,
                    description: $"Failed to compile field getter for '{nameof(TypesWithPointerMembers.PointerField)}': No coercion operator is defined between types 'System.Byte*' and 'System.Object'",
                    remediation: $"Verify that field '{nameof(TypesWithPointerMembers.PointerField)}' is readable and can be used in expression trees"
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithPointerMembers)}\"].{nameof(ApiProperty)}[\"PointerField\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyInvalidFieldSetter,
                    description: $"Failed to compile field setter for '{nameof(TypesWithPointerMembers.PointerField)}': Type must not be a pointer type (Parameter 'type')",
                    remediation: $"Verify that field '{nameof(TypesWithPointerMembers.PointerField)}' is writable and can be used in expression trees"
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithPointerMembers)}\"].{nameof(ApiProperty)}[\"PointerProperty\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyInvalidPropertyGetter,
                    description: $"Failed to compile property getter for '{nameof(TypesWithPointerMembers.PointerProperty)}': No coercion operator is defined between types 'System.Byte*' and 'System.Object'",
                    remediation: $"Verify that property '{nameof(TypesWithPointerMembers.PointerProperty)}' is readable and can be used in expression trees"
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"{nameof(TypesWithPointerMembers)}\"].{nameof(ApiProperty)}[\"PointerProperty\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiPropertyInvalidPropertySetter,
                    description: $"Failed to compile property setter for '{nameof(TypesWithPointerMembers.PointerProperty)}': Type must not be a pointer type (Parameter 'type')",
                    remediation: $"Verify that property '{nameof(TypesWithPointerMembers.PointerProperty)}' is writable and can be used in expression trees"
                ),
            ]
        },

        //
        // ApiObjectType Initialization Tests
        //

        // ApiObjectType throws if ApiProperties has duplicate ApiName
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiObjectType)} Throws If ApiProperties Has Duplicate ApiName",
            SourceJson = @"
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
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
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
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiObjectTypeDuplicatePropertyApiName,
                    description: $"Duplicate {nameof(ApiProperty)}.{nameof(ApiProperty.ApiName)} values: 'Name'",
                    remediation: $"Verify that each {nameof(ApiProperty)} has a unique {nameof(ApiProperty.ApiName)} value"
                ),
            ]
        },

        // ApiObjectType throws if ApiProperties has duplicate ClrName
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiObjectType)} Throws If ApiProperties Has Duplicate ClrName",
            SourceJson = @"
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
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""NameAlias"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
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
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiObjectTypeDuplicatePropertyClrName,
                    description: $"Duplicate {nameof(ApiProperty)}.{nameof(ApiProperty.ClrName)} values: 'Name'",
                    remediation: $"Verify that each {nameof(ApiProperty)} has a unique {nameof(ApiProperty.ClrName)} value"
                ),
            ]
        },

        // ApiObjectType throws if ApiKeyTypes has duplicate ApiName
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiObjectType)} Throws If ApiKeyTypes Has Duplicate ApiName",
            SourceJson = @"
            {
                ""ApiName"": ""ApiObjectType Throws If ApiKeyTypes Has Duplicate ApiName"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Code"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiObjectTypeDuplicateKeyTypeApiName,
                    description: $"Duplicate {nameof(ApiKeyType)}.{nameof(ApiKeyType.ApiName)} values: 'Primary'",
                    remediation: $"Verify that each {nameof(ApiKeyType)} has a unique {nameof(ApiKeyType.ApiName)} value"
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
            SourceJson = @"
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
                    apiPath: $"{nameof(ApiScalarType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiNamedTypeInvalidApiName,
                    description: $"{nameof(ApiScalarType.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiScalarType.ApiName)} value"
                ),
            ]
        },

        // ApiScalarType throws if ClrType is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiScalarType)} Throws If {nameof(ApiScalarType.ClrType)} Is Null",
            SourceJson = @"
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
                    apiPath: $"{nameof(ApiScalarType)}[\"String\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiTypeNullClrType,
                    description: $"{nameof(ApiScalarType.ClrType)} must not be null",
                    remediation: $"Specify a valid {nameof(ApiScalarType.ClrType)}"
                ),
            ]
        },

        //
        // ApiSchema Initialization Tests
        //

        // ApiSchema throws if ApiName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiName)} Is Invalid",
            SourceJson = @"
            {
                ""ApiName"": """",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaInvalidName,
                    description: $"{nameof(ApiSchema.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiSchema.ApiName)} value"
                )
            ]
        },

        // ApiSchema throws if scalar types have duplicate ApiName
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiScalarTypes)} Has Duplicate {nameof(ApiScalarType.ApiName)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema Throws If ApiScalarTypes Has Duplicate ApiName"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiScalarTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateNamedTypeApiName,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ApiName)} values: 'String'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ApiName)} value"
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiScalarTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateScalarTypeApiName,
                    description: $"Duplicate {nameof(ApiScalarType)}.{nameof(ApiScalarType.ApiName)} values: 'String'",
                    remediation: $"Verify that each {nameof(ApiScalarType)} has a unique {nameof(ApiScalarType.ApiName)} value"
                ),
            ]
        },

        // ApiSchema throws if scalar types have duplicate ClrType
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiScalarTypes)} Has Duplicate {nameof(ApiScalarType.ClrType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema Throws If ApiScalarTypes Has Duplicate ClrType"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""StringAlias"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiScalarTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateNamedTypeClrType,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ClrType)} values: '{typeof(string)}'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ClrType)} value"
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiScalarTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateScalarTypeClrType,
                    description: $"Duplicate {nameof(ApiScalarType)}.{nameof(ApiScalarType.ClrType)} values: '{typeof(string)}'",
                    remediation: $"Verify that each {nameof(ApiScalarType)} has a unique {nameof(ApiScalarType.ClrType)} value"
                ),
            ]
        },

        // ApiSchema throws if enum types have duplicate ApiName
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiEnumTypes)} Has Duplicate {nameof(ApiEnumType.ApiName)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema Throws If ApiEnumTypes Has Duplicate ApiName"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            { ""ApiName"": ""Unspecified"", ""ClrName"": ""Unspecified"", ""ClrOrdinal"": 0 },
                            { ""ApiName"": ""Male"", ""ClrName"": ""Male"", ""ClrOrdinal"": 1 },
                            { ""ApiName"": ""Female"", ""ClrName"": ""Female"", ""ClrOrdinal"": 2 }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            { ""ApiName"": ""None"", ""ClrName"": ""None"", ""ClrOrdinal"": 0 },
                            { ""ApiName"": ""Green"", ""ClrName"": ""Green"", ""ClrOrdinal"": 1 },
                            { ""ApiName"": ""Yellow"", ""ClrName"": ""Yellow"", ""ClrOrdinal"": 2 },
                            { ""ApiName"": ""Red"", ""ClrName"": ""Red"", ""ClrOrdinal"": 3 }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.StopLight, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiEnumTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateNamedTypeApiName,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ApiName)} values: 'Gender'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ApiName)} value"
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiEnumTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateEnumTypeApiName,
                    description: $"Duplicate {nameof(ApiEnumType)}.{nameof(ApiEnumType.ApiName)} values: 'Gender'",
                    remediation: $"Verify that each {nameof(ApiEnumType)} has a unique {nameof(ApiEnumType.ApiName)} value"
                ),
            ]
        },

        // ApiSchema throws if enum types have duplicate ClrType
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiEnumTypes)} Has Duplicate {nameof(ApiEnumType.ClrType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema Throws If ApiEnumTypes Has Duplicate ClrType"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""Gender"",
                        ""ApiEnumValues"": [
                            { ""ApiName"": ""Unspecified"", ""ClrName"": ""Unspecified"", ""ClrOrdinal"": 0 },
                            { ""ApiName"": ""Male"", ""ClrName"": ""Male"", ""ClrOrdinal"": 1 },
                            { ""ApiName"": ""Female"", ""ClrName"": ""Female"", ""ClrOrdinal"": 2 }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""GenderAlias"",
                        ""ApiEnumValues"": [
                            { ""ApiName"": ""Unspecified"", ""ClrName"": ""Unspecified"", ""ClrOrdinal"": 0 },
                            { ""ApiName"": ""Male"", ""ClrName"": ""Male"", ""ClrOrdinal"": 1 },
                            { ""ApiName"": ""Female"", ""ClrName"": ""Female"", ""ClrOrdinal"": 2 }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiEnumTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateNamedTypeClrType,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ClrType)} values: '{typeof(Gender)}'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ClrType)} value"
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiEnumTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateEnumTypeClrType,
                    description: $"Duplicate {nameof(ApiEnumType)}.{nameof(ApiEnumType.ClrType)} values: '{typeof(Gender)}'",
                    remediation: $"Verify that each {nameof(ApiEnumType)} has a unique {nameof(ApiEnumType.ClrType)} value"
                ),
            ]
        },

        // ApiSchema throws if object types have duplicate ApiName
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiObjectTypes)} Has Duplicate {nameof(ApiObjectType.ApiName)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema Throws If ApiObjectTypes Has Duplicate ApiName"",
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
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""NameAlt"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NameAlt""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicatePropertyApiNameType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""TestObject"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicatePropertyClrNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiObjectTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateNamedTypeApiName,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ApiName)} values: 'TestObject'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ApiName)} value"
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiObjectTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateObjectTypeApiName,
                    description: $"Duplicate {nameof(ApiObjectType)}.{nameof(ApiObjectType.ApiName)} values: 'TestObject'",
                    remediation: $"Verify that each {nameof(ApiObjectType)} has a unique {nameof(ApiObjectType.ApiName)} value"
                ),
            ]
        },

        // ApiSchema throws if object types have duplicate ClrType
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiObjectTypes)} Has Duplicate {nameof(ApiObjectType.ClrType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema Throws If ApiObjectTypes Has Duplicate ClrType"",
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
                        ""ApiName"": ""TestObjectA"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""NameAlt"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NameAlt""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicatePropertyApiNameType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""TestObjectB"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""NameAlt"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NameAlt""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicatePropertyApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiObjectTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateNamedTypeClrType,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ClrType)} values: '{typeof(DuplicatePropertyApiNameType)}'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ClrType)} value"
                ),
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiObjectTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateObjectTypeClrType,
                    description: $"Duplicate {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} values: '{typeof(DuplicatePropertyApiNameType)}'",
                    remediation: $"Verify that each {nameof(ApiObjectType)} has a unique {nameof(ApiObjectType.ClrType)} value"
                ),
            ]
        },

        // ApiSchema throws if named types have duplicate ApiName (cross-type: scalar + enum with same API name)
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiNamedTypes)} Has Duplicate {nameof(ApiNamedType.ApiName)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema Throws If ApiNamedTypes Has Duplicate ApiName"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""MyType"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""MyType"",
                        ""ApiEnumValues"": [
                            { ""ApiName"": ""Unspecified"", ""ClrName"": ""Unspecified"", ""ClrOrdinal"": 0 },
                            { ""ApiName"": ""Male"", ""ClrName"": ""Male"", ""ClrOrdinal"": 1 },
                            { ""ApiName"": ""Female"", ""ClrName"": ""Female"", ""ClrOrdinal"": 2 }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiNamedTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateNamedTypeApiName,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ApiName)} values: 'MyType'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ApiName)} value"
                ),
            ]
        },

        // ApiSchema throws if named types have duplicate ClrType (cross-type: scalar + enum with same CLR type)
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiNamedTypes)} Has Duplicate {nameof(ApiNamedType.ClrType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema Throws If ApiNamedTypes Has Duplicate ClrType"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""GenderScalar"",
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""GenderEnum"",
                        ""ApiEnumValues"": [
                            { ""ApiName"": ""Unspecified"", ""ClrName"": ""Unspecified"", ""ClrOrdinal"": 0 },
                            { ""ApiName"": ""Male"", ""ClrName"": ""Male"", ""ClrOrdinal"": 1 },
                            { ""ApiName"": ""Female"", ""ClrName"": ""Female"", ""ClrOrdinal"": 2 }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.Gender, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiNamedTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateNamedTypeClrType,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ClrType)} values: '{typeof(Gender)}'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ClrType)} value"
                ),
            ]
        },

        //
        // ApiKeyType Initialization Tests
        //

        // ApiKeyType throws if ApiName is invalid and owned by ApiObjectType (null)
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiKeyType)} Throws If {nameof(ApiKeyType.ApiName)} Is Invalid And Owned By {nameof(ApiObjectType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiKeyType Throws If ApiName Is Invalid And Owned By ApiObjectType"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiKeyType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiKeyTypeInvalidApiName,
                    description: $"{nameof(ApiKeyType.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiKeyType.ApiName)} value"
                ),
            ]
        },

        // ApiKeyType throws if ApiKeyPaths is null or empty
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiKeyType)} Throws If {nameof(ApiKeyType.ApiKeyPaths)} Is Null Or Empty",
            SourceJson = @"
            {
                ""ApiName"": ""ApiKeyType Throws If ApiKeyPaths Is Null Or Empty"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": []
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiKeyType)}[\"PrimaryKey\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiKeyTypeNullOrEmptyPaths,
                    description: $"{nameof(ApiKeyType.ApiKeyPaths)} must not be null or empty",
                    remediation: $"Specify at least one {nameof(ApiKeyPath)}"
                ),
            ]
        },

        // ApiKeyPath throws if ApiSegments is empty
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiKeyPath)} Throws If {nameof(ApiKeyPath.ApiSegments)} Is Empty",
            SourceJson = @"
            {
                ""ApiName"": ""ApiKeyPath Throws If ApiSegments Is Empty"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": []
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiKeyType)}[\"PrimaryKey\"].{nameof(ApiKeyPath)}[\"{nameof(DuplicateKeyTypeApiNameType)}.\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiKeyPathEmptySegments,
                    description: $"{nameof(ApiKeyPath.ApiSegments)} must contain at least one property name",
                    remediation: $"Specify at least one CLR property name when creating an {nameof(ApiKeyPath)}"
                ),
            ]
        },

        // ApiKeyPath throws if ClrRootType is not registered as an ApiObjectType
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiKeyPath)} Throws If {nameof(ApiKeyPath.ClrRootType)} Is Unresolved",
            SourceJson = @"
            {
                ""ApiName"": ""ApiKeyPath Throws If ClrRootType Is Unresolved"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+TypeWithListProperty, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiKeyType)}[\"PrimaryKey\"].{nameof(ApiKeyPath)}[\"{nameof(TypeWithListProperty)}.Id\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiKeyPathUnresolvedRootType,
                    description: $"Root CLR type '{nameof(TypeWithListProperty)}' is not registered as an {nameof(ApiObjectType)} in the schema",
                    remediation: $"Add an {nameof(ApiObjectType)} for '{nameof(TypeWithListProperty)}' to the schema, or correct the root CLR type"
                ),
            ]
        },

        // ApiKeyPathSegment throws if ClrPropertyName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiKeyPathSegment)} Throws If {nameof(ApiKeyPathSegment.ClrPropertyName)} Is Invalid",
            SourceJson = @"
            {
                ""ApiName"": ""ApiKeyPathSegment Throws If ClrPropertyName Is Invalid"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": """" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiKeyType)}[\"PrimaryKey\"].{nameof(ApiKeyPath)}[\"{nameof(DuplicateKeyTypeApiNameType)}.\"].{nameof(ApiKeyPathSegment)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiKeyPathSegmentInvalidClrPropertyName,
                    description: $"{nameof(ApiKeyPathSegment.ClrPropertyName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiKeyPathSegment.ClrPropertyName)} value"
                ),
            ]
        },

        // ApiKeyPathSegment throws if ClrPropertyName cannot resolve to a property
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiKeyPathSegment)} Throws If {nameof(ApiKeyPathSegment.ClrPropertyName)} Is Unresolved",
            SourceJson = @"
            {
                ""ApiName"": ""ApiKeyPathSegment Throws If ClrPropertyName Is Unresolved"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""MissingId"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiKeyType)}[\"PrimaryKey\"].{nameof(ApiKeyPath)}[\"{nameof(DuplicateKeyTypeApiNameType)}.MissingId\"].{nameof(ApiKeyPathSegment)}[\"MissingId\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiKeyPathSegmentUnresolvedApiProperty,
                    description: $"Property with CLR name 'MissingId' could not be found on object type 'TestObject'",
                    remediation: $"Verify the CLR property name or add a property with CLR name 'MissingId' to 'TestObject'"
                ),
            ]
        },

        // ApiKeyPath throws if a navigation segment resolves to a non-object type
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiKeyPath)} Throws If Navigation Segment Resolves To Non Object Type",
            SourceJson = @"
            {
                ""ApiName"": ""ApiKeyPath Throws If Navigation Segment Resolves To Non Object Type"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" },
                                            { ""ClrPropertyName"": ""Code"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiKeyType)}[\"PrimaryKey\"].{nameof(ApiKeyPath)}[\"{nameof(DuplicateKeyTypeApiNameType)}.Id.Code\"].{nameof(ApiKeyPathSegment)}[\"Id\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiKeyPathNavigationSegmentInvalidType,
                    description: $"Navigation segment property 'Id' must resolve to an object type; found '{nameof(ApiScalarType)}'",
                    remediation: $"Change the navigation property to an object-typed property or restructure the path segments"
                ),
            ]
        },

        // ApiKeyPath throws if the scalar segment resolves to a non-scalar type
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiKeyPath)} Throws If Scalar Segment Resolves To Non Scalar Type",
            SourceJson = @"
            {
                ""ApiName"": ""ApiKeyPath Throws If Scalar Segment Resolves To Non Scalar Type"",
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
                        ""ApiName"": ""Owned"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+OwnedType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Owner"",
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
                                ""ApiName"": ""Item"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""Owned""
                                },
                                ""ClrName"": ""Item""
                            }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+OwnerType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Item"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+OwnerType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"Owner\"].{nameof(ApiKeyType)}[\"PrimaryKey\"].{nameof(ApiKeyPath)}[\"{nameof(OwnerType)}.Item\"].{nameof(ApiKeyPathSegment)}[\"Item\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiKeyPathScalarSegmentInvalidType,
                    description: $"Terminal segment property 'Item' must resolve to a scalar type; found '{nameof(ApiObjectType)}'",
                    remediation: $"Change the terminal property to a scalar-typed property or remove extra navigation segments"
                ),
            ]
        },

        //
        // ApiRelationship Initialization Tests
        //

        // ApiRelationship throws if ApiName is invalid (empty)
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationship)} Throws If {nameof(ApiRelationship.ApiName)} Is Invalid",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationship Throws If ApiName Is Invalid"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": """",
                        ""ApiPrincipalEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiDependentEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipInvalidApiName,
                    description: $"{nameof(ApiRelationship.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiRelationship.ApiName)} value"
                ),
            ]
        },

        // ApiRelationshipOneTo throws if ApiPrincipalEnd is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipOneTo)} Throws If {nameof(ApiRelationshipOneTo.ApiPrincipalEnd)} Is Null",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipOneTo Throws If ApiPrincipalEnd Is Null"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiDependentEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipNullPrincipalEnd,
                    description: $"{nameof(ApiRelationshipOneTo.ApiPrincipalEnd)} must not be null",
                    remediation: $"Provide a valid {nameof(ApiRelationshipPrincipalEnd)}"
                ),
            ]
        },

        // ApiRelationshipOneTo throws if ApiDependentEnd is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipOneTo)} Throws If {nameof(ApiRelationshipOneTo.ApiDependentEnd)} Is Null",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipOneTo Throws If ApiDependentEnd Is Null"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipNullDependentEnd,
                    description: $"{nameof(ApiRelationshipOneTo.ApiDependentEnd)} must not be null",
                    remediation: $"Provide a valid {nameof(ApiRelationshipDependentEnd)}"
                ),
            ]
        },

        // ApiRelationshipElement throws if ClrObjectType is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipElement)} Throws If {nameof(ApiRelationshipElement.ClrObjectType)} Is Null",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipElement Throws If ClrObjectType Is Null"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEnd"": { },
                        ""ApiDependentEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"].{nameof(ApiRelationshipPrincipalEnd)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipElementNullClrObjectType,
                    description: $"{nameof(ApiRelationshipElement.ClrObjectType)} must not be null",
                    remediation: $"Specify a valid {nameof(ApiRelationshipElement.ClrObjectType)} value"
                ),
            ]
        },

        // ApiRelationshipElement throws if ClrObjectType is not registered as an ApiObjectType
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipElement)} Throws If {nameof(ApiRelationshipElement.ClrObjectType)} Is Unresolved",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipElement Throws If ClrObjectType Is Unresolved"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEnd"": { ""ClrObjectType"": ""System.Object, System.Private.CoreLib"" },
                        ""ApiDependentEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"].{nameof(ApiRelationshipPrincipalEnd)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipElementUnresolvedObjectType,
                    description: $"No {nameof(ApiObjectType)} is registered for CLR type '{typeof(object).FullName}'",
                    remediation: $"Use one of the available object types: 'RelDependent' ({nameof(RelDependentType)})"
                ),
            ]
        },

        // ApiRelationshipPrincipalEnd throws if referenced ApiPrincipalKeyTypeName cannot be resolved
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipPrincipalEnd)} Throws If {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName)} Is Unresolved",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipPrincipalEnd Throws If ApiPrincipalKeyTypeName Is Unresolved"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiPrincipalKeyTypeName"": ""NonExistentKeyType""
                        },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyType"": {
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalId"" } ] }
                                ]
                            }
                        }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"].{nameof(ApiRelationshipPrincipalEnd)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipEndUnresolvedKeyType,
                    description: "Referenced principal key type 'NonExistentKeyType' could not be found on object type 'RelPrincipal'",
                    remediation: "Use one of the available key types: 'Id'"
                ),
            ]
        },

        // ApiRelationshipOneTo throws if principal key type is named on a navigational relationship
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipOneTo)} Throws If {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName)} Is Supplied Without Foreign Key",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipOneTo Throws If Principal Key Name Is Supplied Without Foreign Key"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiPrincipalKeyTypeName"": ""Id""
                        },
                        ""ApiDependentEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipEndPrincipalKeyWithoutForeignKey,
                    description: $"Cannot resolve {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName)} 'Id' because this relationship has no foreign key binding",
                    remediation: $"Declare {nameof(ApiRelationshipOneTo.ApiDependentEnd)}.{nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)} or remove {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName)}"
                ),
            ]
        },

        // ApiRelationshipManyToMany throws if ApiPrincipalEndA is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipManyToMany)} Throws If {nameof(ApiRelationshipManyToMany.ApiPrincipalEndA)} Is Null",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipManyToMany Throws If ApiPrincipalEndA Is Null"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelAssociation"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalAId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalAId"", ""ClrMemberKind"": ""Property"" },
                            { ""ApiName"": ""PrincipalBId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalBId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEndB"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiAssociation"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipManyToManyNullPrincipalEndA,
                    description: $"{nameof(ApiRelationshipManyToMany.ApiPrincipalEndA)} must not be null",
                    remediation: $"Provide a valid {nameof(ApiRelationshipPrincipalEnd)} for end A"
                ),
            ]
        },

        // ApiRelationshipManyToMany throws if ApiPrincipalEndB is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipManyToMany)} Throws If {nameof(ApiRelationshipManyToMany.ApiPrincipalEndB)} Is Null",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipManyToMany Throws If ApiPrincipalEndB Is Null"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelAssociation"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalAId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalAId"", ""ClrMemberKind"": ""Property"" },
                            { ""ApiName"": ""PrincipalBId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalBId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEndA"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiAssociation"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipManyToManyNullPrincipalEndB,
                    description: $"{nameof(ApiRelationshipManyToMany.ApiPrincipalEndB)} must not be null",
                    remediation: $"Provide a valid {nameof(ApiRelationshipPrincipalEnd)} for end B"
                ),
            ]
        },

        // ApiRelationshipManyToMany throws if ApiAssociation is null
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipManyToMany)} Throws If {nameof(ApiRelationshipManyToMany.ApiAssociation)} Is Null",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipManyToMany Throws If ApiAssociation Is Null"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEndA"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiPrincipalEndB"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipManyToManyNullAssociation,
                    description: $"{nameof(ApiRelationshipManyToMany.ApiAssociation)} must not be null",
                    remediation: $"Provide a valid {nameof(ApiRelationshipAssociation)} for the association between the two principal ends"
                ),
            ]
        },

        // ApiRelationshipManyToMany throws if ApiAssociation.ApiForeignKeyTypeA.ApiKeyPaths count does not match principal end A key type path count
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipManyToMany)} Throws If {nameof(ApiRelationshipAssociation.ApiForeignKeyTypeA)}.{nameof(ApiKeyType.ApiKeyPaths)} Count Does Not Match Principal End A Key Type",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipManyToMany Throws If ApiForeignKeyTypeA ApiKeyPaths Count Mismatch"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipalB"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalBType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalBType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelAssociation"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalAId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalAId"", ""ClrMemberKind"": ""Property"" },
                            { ""ApiName"": ""PrincipalBId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalBId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEndA"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiPrincipalEndB"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalBType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiAssociation"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyTypeA"": {
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalAId"" } ] },
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalBId"" } ] }
                                ]
                            },
                            ""ApiForeignKeyTypeB"": {
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalBId"" } ] }
                                ]
                            }
                        }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipManyToManyInvalidAssociationKeyPathsACount,
                    description: $"Cannot automatically determine the referenced principal key type for principal end A: {nameof(ApiRelationshipManyToMany.ApiAssociation)}.{nameof(ApiRelationshipAssociation.ApiForeignKeyTypeA)}.{nameof(ApiKeyType.ApiKeyPaths)} has 2 key path(s), but no key type on 'RelPrincipal' has 2 key path(s)",
                    remediation: $"Set {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName)} on principal end A explicitly or align the foreign key shape with one of these key types: 'Id'"
                ),
            ]
        },

        // ApiRelationshipManyToMany throws if ApiAssociation.ApiForeignKeyTypeB.ApiKeyPaths count does not match principal end B key type path count
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipManyToMany)} Throws If {nameof(ApiRelationshipAssociation.ApiForeignKeyTypeB)}.{nameof(ApiKeyType.ApiKeyPaths)} Count Does Not Match Principal End B Key Type",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipManyToMany Throws If ApiForeignKeyTypeB ApiKeyPaths Count Mismatch"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipalB"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalBType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalBType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelAssociation"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalAId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalAId"", ""ClrMemberKind"": ""Property"" },
                            { ""ApiName"": ""PrincipalBId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalBId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEndA"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiPrincipalEndB"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalBType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiAssociation"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyTypeA"": {
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalAId"" } ] }
                                ]
                            },
                            ""ApiForeignKeyTypeB"": {
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalAId"" } ] },
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalBId"" } ] }
                                ]
                            }
                        }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipManyToManyInvalidAssociationKeyPathsBCount,
                    description: $"Cannot automatically determine the referenced principal key type for principal end B: {nameof(ApiRelationshipManyToMany.ApiAssociation)}.{nameof(ApiRelationshipAssociation.ApiForeignKeyTypeB)}.{nameof(ApiKeyType.ApiKeyPaths)} has 2 key path(s), but no key type on 'RelPrincipalB' has 2 key path(s)",
                    remediation: $"Set {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName)} on principal end B explicitly or align the foreign key shape with one of these key types: 'Id'"
                ),
            ]
        },

        // ApiRelationshipOneTo throws if ApiDependentEnd.ApiForeignKeyType.ApiKeyPaths count does not match principal key type path count
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipOneTo)} Throws If {nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)}.{nameof(ApiKeyType.ApiKeyPaths)} Count Does Not Match Principal Key Type",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipOneTo Throws If Dependent ForeignKeyType ApiKeyPaths Count Mismatch"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId"", ""ClrMemberKind"": ""Property"" },
                            { ""ApiName"": ""PrincipalId2"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId2"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyType"": {
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalId"" } ] },
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalId2"" } ] }
                                ]
                            }
                        }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipOneToInvalidDependentKeyPathsCount,
                    description: $"Cannot automatically determine the referenced principal key type: {nameof(ApiRelationshipOneTo.ApiDependentEnd)}.{nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)}.{nameof(ApiKeyType.ApiKeyPaths)} has 2 key path(s), but no key type on 'RelPrincipal' has 2 key path(s)",
                    remediation: $"Set {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName)} explicitly or align the foreign key shape with one of these key types: 'Id'"
                ),
            ]
        },

        // ApiRelationshipOneTo throws if an explicitly selected principal key type is incompatible with the dependent foreign key type
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipOneTo)} Throws If Explicit Principal Key Type Is Incompatible With Foreign Key Type",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipOneTo Throws If Explicit Principal Key Type Is Incompatible"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" },
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"", ""ClrType"": ""System.String, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalCode"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalCode"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"", ""ApiPrincipalKeyTypeName"": ""PK_Id"" },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyType"": {
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalCode"" } ] }
                                ]
                            }
                        }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipIncompatiblePrincipalForeignKey,
                    description: $"{nameof(ApiRelationshipOneTo.ApiDependentEnd)}.{nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)} leaf type(s) [String] are not compatible with principal end principal key type 'PK_Id' leaf type(s) [Int32]",
                    remediation: $"Ensure {nameof(ApiRelationshipOneTo.ApiDependentEnd)}.{nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)} paths are ordered to match the principal end's principal key type and use compatible scalar types"
                ),
            ]
        },

        // ApiSchema throws if ApiRelationships contains entries with duplicate ApiName values
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiRelationships)} Has Duplicate {nameof(ApiRelationship.ApiName)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiSchema Throws If ApiRelationships Has Duplicate ApiName"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""DupRel"",
                        ""ApiPrincipalEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiDependentEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"" }
                    },
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""DupRel"",
                        ""ApiPrincipalEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiDependentEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"" }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiRelationships Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiSchemaDuplicateRelationshipApiName,
                    description: $"Duplicate {nameof(ApiRelationship)}.{nameof(ApiRelationship.ApiName)} values: 'DupRel'",
                    remediation: $"Verify that each {nameof(ApiRelationship)} has a unique {nameof(ApiRelationship.ApiName)} value"
                ),
            ]
        },

        // ApiRelationshipOneTo throws if the principal has multiple key types compatible with the foreign key (ambiguous)
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipOneTo)} Throws If Principal Key Type Is Ambiguous",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipOneTo Throws If Principal Key Type Is Ambiguous"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" },
                            { ""ApiName"": ""Code"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Code"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PK_Id"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ]
                            },
                            {
                                ""ApiName"": ""PK_Code"",
                                ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Code"" } ] } ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelDependent"",
                        ""ApiProperties"": [
                            { ""ApiName"": ""PrincipalId"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""PrincipalId"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""TestRel"",
                        ""ApiPrincipalEnd"": { ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+DuplicateKeyTypeApiNameType, Evoogle.ApiFramework.Core.Tests"" },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyType"": {
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelDependentType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalId"" } ] }
                                ]
                            }
                        }
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.ApiRelationshipAmbiguousPrincipalKey,
                    description: "Cannot automatically determine the referenced principal key type: 2 key types on 'RelPrincipal' are compatible with the foreign key type: 'PK_Id', 'PK_Code'",
                    remediation: $"Set {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName)} to specify the principal key type explicitly; available key types: 'PK_Id', 'PK_Code'"
                ),
            ]
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] InitializeWarnsTheoryData =>
    [
        // ApiObjectType warns if ApiProperties is null or empty
        new InitializeWarnsTest
        {
            Name = $"{nameof(ApiObjectType)} Warns If ApiProperties Is Null Or Empty",
            SourceJson = @"
            {
                ""ApiName"": ""ApiObjectType Warns If ApiProperties Is Null Or Empty"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Empty"",
                        ""ApiProperties"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedWarnings =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"Empty\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.ApiObjectTypeNullOrEmptyProperties,
                    description: $"{nameof(ApiObjectType.ApiProperties)} is null or empty",
                    remediation: $"Add at least one {nameof(ApiProperty)} to {nameof(ApiObjectType)}[\"Empty\"]"
                ),
            ]
        },

        // ApiProperty warns if Required property maps to a nullable CLR member
        new InitializeWarnsTest
        {
            Name = $"{nameof(ApiProperty)} Warns If Required Property Maps To Nullable CLR Member",
            SourceJson = @"
            {
                ""ApiName"": ""ApiProperty Warns If Required Property Maps To Nullable CLR Member"",
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
                        ""ApiName"": ""NullabilityMismatch"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NullableProp"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NullableProp"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.NullabilityMismatch, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedWarnings =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"NullabilityMismatch\"].{nameof(ApiProperty)}[\"NullableProp\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.ApiPropertyRequiredNullableMismatch,
                    description: "CLR member 'NullableProp' is nullable but property 'NullableProp' is declared Required",
                    remediation: "Change CLR member 'NullableProp' to a non-nullable type, or change property 'NullableProp' to Optional"
                ),
            ]
        },

        // ApiProperty warns if Optional property maps to a non-nullable CLR reference type member
        new InitializeWarnsTest
        {
            Name = $"{nameof(ApiProperty)} Warns If Optional Property Maps To Non-Nullable CLR Reference Type Member",
            SourceJson = @"
            {
                ""ApiName"": ""ApiProperty Warns If Optional Property Maps To Non-Nullable CLR Reference Type Member"",
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
                        ""ApiName"": ""NullabilityMismatch"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NonNullableProp"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ClrName"": ""NonNullableProp"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.NullabilityMismatch, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedWarnings =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"NullabilityMismatch\"].{nameof(ApiProperty)}[\"NonNullableProp\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.ApiPropertyOptionalNonNullableMismatch,
                    description: "CLR member 'NonNullableProp' is non-nullable but property 'NonNullableProp' is declared Optional",
                    remediation: "Change CLR member 'NonNullableProp' to a nullable reference type, or change property 'NonNullableProp' to Required"
                ),
            ]
        },

        // ApiProperty warns if Required item modifier maps to a nullable CLR collection element type
        new InitializeWarnsTest
        {
            Name = $"{nameof(ApiProperty)} Warns If Required Collection Item Maps To Nullable CLR Element Type",
            SourceJson = @"
            {
                ""ApiName"": ""ApiProperty Warns If Required Collection Item Maps To Nullable CLR Element Type"",
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
                        ""ApiName"": ""CollectionNullabilityMismatch"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NullableItemsProp"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                        ""ApiItemTypeModifiers"": ""Required"",
                                        ""ClrType"": ""System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ClrName"": ""NullableItemsProp"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.CollectionNullabilityMismatch, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedWarnings =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"CollectionNullabilityMismatch\"].{nameof(ApiProperty)}[\"NullableItemsProp\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.ApiCollectionItemRequiredNullableMismatch,
                    description: "CLR collection element in 'NullableItemsProp' is nullable but item is declared Required",
                    remediation: "Change the CLR element type in 'NullableItemsProp' to non-nullable, or change the item modifier to Optional"
                ),
            ]
        },

        // ApiProperty warns if Optional item modifier maps to a non-nullable CLR collection element reference type
        new InitializeWarnsTest
        {
            Name = $"{nameof(ApiProperty)} Warns If Optional Collection Item Maps To Non-Nullable CLR Element Reference Type",
            SourceJson = @"
            {
                ""ApiName"": ""ApiProperty Warns If Optional Collection Item Maps To Non-Nullable CLR Element Reference Type"",
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
                        ""ApiName"": ""CollectionNullabilityMismatch"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NonNullableItemsProp"",
                                ""ApiType"": {
                                    ""ApiInlineType"": {
                                        ""ApiKind"": ""Collection"",
                                        ""ApiItemType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                        ""ClrType"": ""System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
                                    }
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NonNullableItemsProp"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.CollectionNullabilityMismatch, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedWarnings =
            [
                new ApiInitializationIssue
                (
                    apiPath: $"{nameof(ApiObjectType)}[\"CollectionNullabilityMismatch\"].{nameof(ApiProperty)}[\"NonNullableItemsProp\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.ApiCollectionItemOptionalNonNullableMismatch,
                    description: "CLR collection element in 'NonNullableItemsProp' is non-nullable but item is declared Optional",
                    remediation: "Change the CLR element type in 'NonNullableItemsProp' to a nullable reference type, or change the item modifier to Required"
                ),
            ]
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(InitializeThrowsTheoryData))]
    public void InitializeThrows(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InitializeWarnsTheoryData))]
    public void InitializeWarns(IXUnitTest test) => test.Execute(this);

    #endregion
}
