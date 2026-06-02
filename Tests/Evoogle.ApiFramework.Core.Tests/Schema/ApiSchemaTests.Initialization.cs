// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
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

    public class DuplicateKeyTypeApiNameType
    {
        public int Id { get; set; }
        public int Code { get; set; }
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
    }

    public class RelAssociationType
    {
        public int PrincipalAId { get; set; }
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
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiProperty)}[\"Items\"].{nameof(ApiCollectionType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_COLLECTION_TYPE_NULL_ITEM_TYPE,
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
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"].{nameof(ApiProperty)}[\"Items\"].{nameof(ApiCollectionType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_COLLECTION_TYPE_UNRESOLVED_ITEM_TYPE,
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
                    path: $"{nameof(ApiEnumType)}[\"{nameof(Gender)}\"].{nameof(ApiEnumValue)}[\"{nameof(Gender.Male)}\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_ENUM_VALUE_INVALID_CLR_NAME,
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
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_PROPERTY_CLR_NAME,
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
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""Primary"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Code""
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
                    path: $"{nameof(ApiObjectType)}[\"TestObject\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_KEY_TYPE_API_NAME,
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
                    path: $"{nameof(ApiScalarType)}[\"String\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_TYPE_NULL_CLR_TYPE,
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
                    path: $"{nameof(ApiSchema)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_INVALID_NAME,
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
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiScalarTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_API_NAME,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ApiName)} values: 'String'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ApiName)} value"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiScalarTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_SCALAR_TYPE_API_NAME,
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
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiScalarTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_CLR_TYPE,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ClrType)} values: '{typeof(string)}'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ClrType)} value"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiScalarTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_SCALAR_TYPE_CLR_TYPE,
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
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiEnumTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_API_NAME,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ApiName)} values: 'Gender'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ApiName)} value"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiEnumTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_ENUM_TYPE_API_NAME,
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
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiEnumTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_CLR_TYPE,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ClrType)} values: '{typeof(Gender)}'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ClrType)} value"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiEnumTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_ENUM_TYPE_CLR_TYPE,
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
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiObjectTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_API_NAME,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ApiName)} values: 'TestObject'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ApiName)} value"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiObjectTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_OBJECT_TYPE_API_NAME,
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
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiObjectTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_CLR_TYPE,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ClrType)} values: '{typeof(DuplicatePropertyApiNameType)}'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ClrType)} value"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiObjectTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_OBJECT_TYPE_CLR_TYPE,
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
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiNamedTypes Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_API_NAME,
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
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiNamedTypes Has Duplicate ClrType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_CLR_TYPE,
                    description: $"Duplicate {nameof(ApiNamedType)}.{nameof(ApiNamedType.ClrType)} values: '{typeof(Gender)}'",
                    remediation: $"Verify that each {nameof(ApiNamedType)} has a unique {nameof(ApiNamedType.ClrType)} value"
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
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                    path: $"{nameof(ApiRelationshipOneToMany)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_INVALID_API_NAME,
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
                    path: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_NULL_PRINCIPAL_END,
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
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                    path: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_NULL_DEPENDENT_END,
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
                    path: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"].{nameof(ApiRelationshipPrincipalEnd)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_ELEMENT_NULL_CLR_OBJECT_TYPE,
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
                    path: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"].{nameof(ApiRelationshipPrincipalEnd)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_ELEMENT_UNRESOLVED_OBJECT_TYPE,
                    description: $"No {nameof(ApiObjectType)} is registered for CLR type '{typeof(object).FullName}'",
                    remediation: $"Use one of the available object types: 'RelDependent' ({nameof(RelDependentType)})"
                ),
            ]
        },

        // ApiRelationshipPrincipalEnd throws if referenced ApiIdentityName cannot be resolved
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipPrincipalEnd)} Throws If {nameof(ApiRelationshipPrincipalEnd.ApiIdentityName)} Is Unresolved",
            SourceJson = @"
            {
                ""ApiName"": ""ApiRelationshipPrincipalEnd Throws If ApiIdentityName Is Unresolved"",
                ""ApiScalarTypes"": [
                    { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"", ""ClrType"": ""System.Int32, System.Private.CoreLib"" }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipal"",
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                            ""ApiIdentityName"": ""NonExistentIdentity""
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
                    path: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"].{nameof(ApiRelationshipPrincipalEnd)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_IDENTITY,
                    description: "Referenced identity 'NonExistentIdentity' could not be found on object type 'RelPrincipal'",
                    remediation: "Use one of the available identities: 'Id'"
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
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                    path: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_A,
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
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                    path: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_B,
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
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                    path: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_ASSOCIATION,
                    description: $"{nameof(ApiRelationshipManyToMany.ApiAssociation)} must not be null",
                    remediation: $"Provide a valid {nameof(ApiRelationshipAssociation)} for the association between the two principal ends"
                ),
            ]
        },

        // ApiRelationshipManyToMany throws if ApiAssociation.ApiForeignKeyTypeA.ApiKeyPaths count does not match principal end A identity key path count
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipManyToMany)} Throws If {nameof(ApiRelationshipAssociation.ApiForeignKeyTypeA)}.{nameof(ApiKeyType.ApiKeyPaths)} Count Does Not Match Principal End A Identity",
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
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipalB"",
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalBType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                                ""ApiName"": ""FK_Test_A"",
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalAId"" } ] },
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalBId"" } ] }
                                ]
                            },
                            ""ApiForeignKeyTypeB"": {
                                ""ApiName"": ""FK_Test_B"",
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
                    path: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_A_COUNT,
                    description: $"{nameof(ApiRelationshipManyToMany.ApiAssociation)}.{nameof(ApiRelationshipAssociation.ApiForeignKeyTypeA)}.{nameof(ApiKeyType.ApiKeyPaths)} has 2 key path(s) but principal end A identity 'Id' has 1 key path(s)",
                    remediation: $"Ensure {nameof(ApiRelationshipManyToMany.ApiAssociation)}.{nameof(ApiRelationshipAssociation.ApiForeignKeyTypeA)}.{nameof(ApiKeyType.ApiKeyPaths)} contains exactly 1 key path(s) to match principal end A's join-key identity"
                ),
            ]
        },

        // ApiRelationshipManyToMany throws if ApiAssociation.ApiForeignKeyTypeB.ApiKeyPaths count does not match principal end B identity key path count
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipManyToMany)} Throws If {nameof(ApiRelationshipAssociation.ApiForeignKeyTypeB)}.{nameof(ApiKeyType.ApiKeyPaths)} Count Does Not Match Principal End B Identity",
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
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""RelPrincipalB"",
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalBType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                                ""ApiName"": ""FK_Test_A"",
                                ""ApiKeyPaths"": [
                                    { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelAssociationType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""PrincipalAId"" } ] }
                                ]
                            },
                            ""ApiForeignKeyTypeB"": {
                                ""ApiName"": ""FK_Test_B"",
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
                    path: $"{nameof(ApiRelationshipManyToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_B_COUNT,
                    description: $"{nameof(ApiRelationshipManyToMany.ApiAssociation)}.{nameof(ApiRelationshipAssociation.ApiForeignKeyTypeB)}.{nameof(ApiKeyType.ApiKeyPaths)} has 2 key path(s) but principal end B identity 'Id' has 1 key path(s)",
                    remediation: $"Ensure {nameof(ApiRelationshipManyToMany.ApiAssociation)}.{nameof(ApiRelationshipAssociation.ApiForeignKeyTypeB)}.{nameof(ApiKeyType.ApiKeyPaths)} contains exactly 1 key path(s) to match principal end B's join-key identity"
                ),
            ]
        },

        // ApiRelationshipOneTo throws if ApiDependentEnd.ApiForeignKeyType.ApiKeyPaths count does not match principal identity key path count
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiRelationshipOneTo)} Throws If {nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)}.{nameof(ApiKeyType.ApiKeyPaths)} Count Does Not Match Principal Identity",
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
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                                ""ApiName"": ""FK_Test"",
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
                    path: $"{nameof(ApiRelationshipOneToMany)}[\"TestRel\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_ONE_TO_INVALID_DEPENDENT_KEY_PATHS_COUNT,
                    description: $"{nameof(ApiRelationshipOneTo.ApiDependentEnd)}.{nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)}.{nameof(ApiKeyType.ApiKeyPaths)} has 2 key path(s) but principal identity 'Id' has 1 key path(s)",
                    remediation: $"Ensure {nameof(ApiRelationshipOneTo.ApiDependentEnd)}.{nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)}.{nameof(ApiKeyType.ApiKeyPaths)} contains exactly 1 key path(s) to match the principal end's join-key identity"
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
                        ""ApiKeyTypes"": [
                            { ""ApiName"": ""Id"", ""ApiKeyPaths"": [ { ""ClrRootType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+RelPrincipalType, Evoogle.ApiFramework.Core.Tests"", ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ] } ] }
                        ],
                        ""ApiProperties"": [
                            { ""ApiName"": ""Id"", ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" }, ""ApiTypeModifiers"": ""Required"", ""ClrName"": ""Id"", ""ClrMemberKind"": ""Property"" }
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
                    path: $"{nameof(ApiSchema)}[\"ApiSchema Throws If ApiRelationships Has Duplicate ApiName\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_SCHEMA_DUPLICATE_RELATIONSHIP_API_NAME,
                    description: $"Duplicate {nameof(ApiRelationship)}.{nameof(ApiRelationship.ApiName)} values: 'DupRel'",
                    remediation: $"Verify that each {nameof(ApiRelationship)} has a unique {nameof(ApiRelationship.ApiName)} value"
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
                    path: $"{nameof(ApiObjectType)}[\"Empty\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_OBJECT_TYPE_NULL_OR_EMPTY_PROPERTIES,
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
                    path: $"{nameof(ApiObjectType)}[\"NullabilityMismatch\"].{nameof(ApiProperty)}[\"NullableProp\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_PROPERTY_REQUIRED_NULLABLE_MISMATCH,
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
                    path: $"{nameof(ApiObjectType)}[\"NullabilityMismatch\"].{nameof(ApiProperty)}[\"NonNullableProp\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_PROPERTY_OPTIONAL_NON_NULLABLE_MISMATCH,
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
                    path: $"{nameof(ApiObjectType)}[\"CollectionNullabilityMismatch\"].{nameof(ApiProperty)}[\"NullableItemsProp\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_COLLECTION_ITEM_REQUIRED_NULLABLE_MISMATCH,
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
                    path: $"{nameof(ApiObjectType)}[\"CollectionNullabilityMismatch\"].{nameof(ApiProperty)}[\"NonNullableItemsProp\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_COLLECTION_ITEM_OPTIONAL_NON_NULLABLE_MISMATCH,
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

    [Fact]
    public void InitializeDoesNotThrowWhenKeyTypeNavigatesThroughAlphabeticallyLaterObjectType()
    {
        // Regression test: ApiSchema.Initialize() previously used a single pass over object types,
        // initializing both structure and key types together. A key type on an alphabetically-early
        // object type (e.g. "Alpha") that navigates through an alphabetically-later object type
        // (e.g. "Zeta") would fail with ApiSchemaException because Zeta's property lookups were
        // not yet populated when Alpha's key was being initialized.
        //
        // The fix splits initialization into two sub-passes:
        //   Sub-pass A: InitializeStructure for ALL object types (populates property lookups).
        //   Sub-pass B: InitializeKeyTypes for ALL object types (navigates cross-type key paths).
        var intType = new ApiScalarType(nameof(Int32), typeof(int));
        var stringType = new ApiScalarType(nameof(String), typeof(string));

        var zetaObjectType = new ApiObjectType(
            apiName: nameof(ZetaKeyType),
            apiOptions: null,
            apiProperties:
            [
                new ApiProperty(nameof(ZetaKeyType.Id), ApiTypeExpression.ClrRef<int>(), ApiTypeModifiers.Required, nameof(ZetaKeyType.Id), ClrMemberKind.Property)
            ],
            apiKeyTypes:
            [
                new ApiKeyType("PK_ZetaKeyType",
                [
                    new ApiKeyPath(typeof(ZetaKeyType), [new ApiKeyPathSegment(nameof(ZetaKeyType.Id))])
                ])
            ],
            clrObjectType: typeof(ZetaKeyType));

        var alphaObjectType = new ApiObjectType(
            apiName: nameof(AlphaKeyType),
            apiOptions: null,
            apiProperties:
            [
                new ApiProperty(nameof(AlphaKeyType.ZetaRef), ApiTypeExpression.ClrRef<ZetaKeyType>(), ApiTypeModifiers.None, nameof(AlphaKeyType.ZetaRef), ClrMemberKind.Property)
            ],
            apiKeyTypes:
            [
                new ApiKeyType("PK_AlphaKeyType",
                [
                    new ApiKeyPath(typeof(AlphaKeyType),
                    [
                        new ApiKeyPathSegment(nameof(AlphaKeyType.ZetaRef)),
                        new ApiKeyPathSegment(nameof(ZetaKeyType.Id))
                    ])
                ])
            ],
            clrObjectType: typeof(AlphaKeyType));

        // AlphaKeyType sorts before ZetaKeyType alphabetically, so ApiSchema will order it first.
        // Its key path navigates through ZetaKeyType — requires ZetaKeyType's structure to be
        // initialized before AlphaKeyType's key types are resolved.
        // If this throws ApiSchemaException, the two-pass initialization fix has regressed.
        ApiSchema.Create
        (
            apiName: "KeyInitOrderRegressionSchema",
            apiVersion: null,
            apiOptions: null,
            apiScalarTypes: [intType, stringType],
            apiEnumTypes: [],
            apiObjectTypes: [alphaObjectType, zetaObjectType]
        );
    }
    #endregion
}
