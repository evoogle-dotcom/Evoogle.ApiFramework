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

    public class DuplicateIdentityApiNameType
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
        // ApiIdentity Initialization Tests
        //

        // ApiIdentity throws if ApiName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentity)} Throws If {nameof(ApiIdentity.ApiName)} Is Invalid",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentity Throws If ApiName Is Invalid"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""IdentityScalar"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": """",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
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
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityScalar, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityScalar\"].{nameof(ApiIdentity)}",
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
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentity Throws If ApiIdentityParts Is Empty"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""IdentityScalar"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityScalar"",
                                ""ApiIdentityParts"": []
                            }
                        ],
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
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityScalar, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityScalar\"].{nameof(ApiIdentity)}[\"PK_IdentityScalar\"]",
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
            Name = $"{nameof(ApiIdentity)} Throws If {nameof(ApiIdentity.ApiIdentityParts)} Has Duplicate {nameof(ApiIdentityPropertyPart.ClrPropertyName)} Values",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentity Throws If ApiIdentityParts Has Duplicate ApiPropertyName Values"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""IdentityScalar"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityScalar"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    },
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
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
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityScalar, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityScalar\"].{nameof(ApiIdentity)}[\"PK_IdentityScalar\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_DUPLICATE_PART_API_PROPERTY_NAME,
                    description: $"Duplicate {nameof(ApiIdentityPropertyPart)}.{nameof(ApiIdentityPropertyPart.ClrPropertyName)} values: 'Id'",
                    remediation: $"Verify that each {nameof(ApiIdentityPropertyPart)} has a unique {nameof(ApiIdentityPropertyPart.ClrPropertyName)} value"
                ),
            ]
        },

        // ApiIdentity throws if ApiIdentityParts contains multiple ApiIdentityOwnerPart instances
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentity)} Throws If {nameof(ApiIdentity.ApiIdentityParts)} Contains Multiple {nameof(ApiIdentityOwnerPart)} Instances",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentity Throws If ApiIdentityParts Contains Multiple ApiIdentityOwnerPart Instances"",
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
                        ""ApiName"": ""OwnerType"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_OwnerType"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Item"",
                                ""ApiType"": { ""ApiKind"": ""Object"", ""ApiName"": ""OwnedType"" },
                                ""ClrName"": ""Item"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+OwnerType, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""OwnedType"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_OwnedType"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Owner"" },
                                    { ""ApiKind"": ""Owner"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+OwnedType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"OwnedType\"].{nameof(ApiIdentity)}[\"PK_OwnedType\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_MULTIPLE_OWNER_PARTS,
                    description: $"An {nameof(ApiIdentity)} may contain at most one {nameof(ApiIdentityOwnerPart)}, but 2 were found",
                    remediation: $"Remove the extra {nameof(ApiIdentityOwnerPart)} instances, leaving exactly one"
                ),
            ]
        },

        //
        // ApiIdentityPart Initialization Tests
        //

        // ApiIdentityNestedPart throws if the identity property is not an ApiObjectType
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityNestedPart)} Throws If {nameof(ApiIdentityNestedPart.ClrPropertyName)} Is Not An {nameof(ApiObjectType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentityNestedPart Throws If ApiPropertyName Is Not An ApiObjectType"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""IdentityNested"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_InvalidNestedPart"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Nested"", ""ClrPropertyName"": ""Id"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNested, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityNested\"].{nameof(ApiIdentity)}[\"PK_InvalidNestedPart\"].{nameof(ApiIdentityNestedPart)}[\"Id\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_INVALID_API_PROPERTY_TYPE,
                    description: "Property 'Id' must be an object type for a nested identity part",
                    remediation: $"Use an object-typed property or switch to {nameof(ApiIdentityScalarPart)}"
                ),
            ]
        },

        // ApiIdentityNestedPart throws if the referenced identity name is unresolved
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityNestedPart)} Throws If {nameof(ApiIdentityNestedPart.ApiIdentityName)} Is Unresolved",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentityNestedPart Throws If ApiIdentityName Is Unresolved"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""IdentityNested"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityNested"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Scalar"", ""ClrPropertyName"": ""Id"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNested, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityNestedComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityNestedComposite"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Nested"",
                                        ""ClrPropertyName"": ""NestedPart"",
                                        ""ApiIdentityName"": ""NonExistentId""
                                    }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NestedPart"",
                                ""ApiType"": { ""ApiKind"": ""Object"", ""ApiName"": ""IdentityNested"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NestedPart"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNestedComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityNestedComposite\"].{nameof(ApiIdentity)}[\"PK_IdentityNestedComposite\"].{nameof(ApiIdentityNestedPart)}[\"NestedPart\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_NESTED_IDENTITY,
                    description: "Referenced identity 'NonExistentId' could not be found on object type 'IdentityNested'",
                    remediation: "Use one of the available identities: 'PK_IdentityNested'"
                ),
            ]
        },

        // ApiIdentityOwnerPart throws if no candidate owner ApiObjectType exists
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityOwnerPart)} Throws If No Candidate Owner {nameof(ApiObjectType)} Exists",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentityOwnerPart Throws If No Candidate Owner ApiObjectType Exists"",
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
                        ""ApiName"": ""OwnedType"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_OwnedType"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Owner"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+OwnedType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"OwnedType\"].{nameof(ApiIdentity)}[\"PK_OwnedType\"].{nameof(ApiIdentityOwnerPart)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_OWNER,
                    description: "No owner object type was found for 'OwnedType' \u2014 no other object type has a collection or direct object property of this type",
                    remediation: $"Ensure another {nameof(ApiObjectType)} has a collection or direct object property typed as 'OwnedType', or remove the {nameof(ApiIdentityOwnerPart)}"
                ),
            ]
        },

        // ApiIdentityOwnerPart throws if multiple candidate owner ApiObjectTypes exist
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityOwnerPart)} Throws If Multiple Candidate Owner {nameof(ApiObjectType)}s Exist",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentityOwnerPart Throws If Multiple Candidate Owner ApiObjectTypes Exist"",
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
                        ""ApiName"": ""OwnerTypeA"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_OwnerTypeA"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Scalar"", ""ClrPropertyName"": ""Id"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Item"",
                                ""ApiType"": { ""ApiKind"": ""Object"", ""ApiName"": ""OwnedType"" },
                                ""ClrName"": ""Item"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+OwnerTypeA, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""OwnerTypeB"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_OwnerTypeB"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Scalar"", ""ClrPropertyName"": ""Id"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Item"",
                                ""ApiType"": { ""ApiKind"": ""Object"", ""ApiName"": ""OwnedType"" },
                                ""ClrName"": ""Item"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+OwnerTypeB, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""OwnedType"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_OwnedType"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Owner"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+OwnedType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"OwnedType\"].{nameof(ApiIdentity)}[\"PK_OwnedType\"].{nameof(ApiIdentityOwnerPart)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_AMBIGUOUS_OWNER,
                    description: "Multiple candidate owner object types found for 'OwnedType': 'OwnerTypeA', 'OwnerTypeB'",
                    remediation: $"Disambiguate by ensuring only one {nameof(ApiObjectType)} holds a collection or direct object property of 'OwnedType'"
                ),
            ]
        },

        // ApiIdentityOwnerPart throws if a cyclic owner reference is detected
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityOwnerPart)} Throws If A Cyclic Owner Reference Is Detected",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentityOwnerPart Throws If A Cyclic Owner Reference Is Detected"",
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""CircularIdentityNodeType"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_NodeType"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Owner"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Self"",
                                ""ApiType"": { ""ApiKind"": ""Object"", ""ApiName"": ""CircularIdentityNodeType"" },
                                ""ClrName"": ""Self"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiSchemaTests+CircularIdentityNodeType, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"CircularIdentityNodeType\"].{nameof(ApiIdentity)}[\"PK_NodeType\"].{nameof(ApiIdentityOwnerPart)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_CYCLIC_OWNER,
                    description: "A cyclic owner identity reference was detected involving 'CircularIdentityNodeType'",
                    remediation: $"Remove the cyclic {nameof(ApiIdentityOwnerPart)} reference"
                ),
            ]
        },

        // ApiIdentityPropertyPart throws if ApiPropertyName is invalid
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityPropertyPart)} Throws If {nameof(ApiIdentityPropertyPart.ClrPropertyName)} Is Invalid",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentityPropertyPart Throws If ApiPropertyName Is Invalid"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""IdentityScalar"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityScalar"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": """"
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
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityScalar, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityScalar\"].{nameof(ApiIdentity)}[\"PK_IdentityScalar\"].{nameof(ApiIdentityScalarPart)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_INVALID_CLR_PROPERTY_NAME,
                    description: $"{nameof(ApiIdentityPropertyPart.ClrPropertyName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiIdentityPropertyPart.ClrPropertyName)} value"
                ),
            ]
        },

        // ApiIdentityPropertyPart throws if ApiPropertyName is unresolved
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityPropertyPart)} Throws If {nameof(ApiIdentityPropertyPart.ClrPropertyName)} Is Unresolved",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentityPropertyPart Throws If ApiPropertyName Is Unresolved"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""IdentityScalar"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityScalar"",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""DoesNotExist""
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
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityScalar, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityScalar\"].{nameof(ApiIdentity)}[\"PK_IdentityScalar\"].{nameof(ApiIdentityScalarPart)}[\"DoesNotExist\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_API_PROPERTY,
                    description: $"{nameof(ApiIdentityPropertyPart.ApiProperty)} could not be resolved for {nameof(ApiIdentityPropertyPart.ClrPropertyName)}='DoesNotExist'",
                    remediation: $"Verify that {nameof(ApiIdentityPropertyPart.ClrPropertyName)} refers to a valid property on {nameof(ApiObjectType)}[\"IdentityScalar\"]"
                ),
            ]
        },

        // ApiIdentityScalarPart warns if identity part requires string parsing for a non-string scalar type
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityScalarPart)} Warns If {nameof(String)} Requires Coercion",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentityScalarPart Warns If String Requires Coercion"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""IdentityScalar"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": """",
                                ""ApiIdentityParts"": [
                                    {
                                        ""ApiKind"": ""Scalar"",
                                        ""ClrPropertyName"": ""Id"",
                                        ""ClrScalarTypeHint"": ""System.String,System.Private.CoreLib""
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
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiName"": ""String""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityScalar, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=1, Warnings=1.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityScalar\"].{nameof(ApiIdentity)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_INVALID_API_NAME,
                    description: $"{nameof(ApiIdentity.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiIdentity.ApiName)} value"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityScalar\"].{nameof(ApiIdentity)}.{nameof(ApiIdentityScalarPart)}[\"Id\"]",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_IDENTITY_PART_PERFORMANCE_CONCERN,
                    description: $"Identity part 'Id' has CLR property type {nameof(Int32)} but resolves to scalar type {nameof(String)}, requiring string parse/format on every identity operation",
                    remediation: $"Align the 'Id' property type with the resolved scalar type {nameof(String)} to eliminate string coercion, or accept the overhead if the type mismatch is intentional"
                ),
            ]
        },

        // ApiIdentityScalarPart throws if the identity property is not an ApiScalarType
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiIdentityScalarPart)} Throws If {nameof(ApiIdentityScalarPart.ClrPropertyName)} Is Not An {nameof(ApiScalarType)}",
            SourceJson = @"
            {
                ""ApiName"": ""ApiIdentityScalarPart Throws If ApiPropertyName Is Not An ApiScalarType"",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int32"",
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""IdentityNested"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_IdentityNested"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Scalar"", ""ClrPropertyName"": ""Id"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""Int32"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNested, Evoogle.ApiFramework.Core.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""IdentityNestedComposite"",
                        ""ApiIdentities"": [
                            {
                                ""ApiName"": ""PK_InvalidScalarPart"",
                                ""ApiIdentityParts"": [
                                    { ""ApiKind"": ""Scalar"", ""ClrPropertyName"": ""NestedPart"" }
                                ]
                            }
                        ],
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NestedPart"",
                                ""ApiType"": { ""ApiKind"": ""Object"", ""ApiName"": ""IdentityNested"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NestedPart"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.TestData.IdentityNestedComposite, Evoogle.ApiFramework.Core.Tests""
                    }
                ]
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=1, Errors=1, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"IdentityNestedComposite\"].{nameof(ApiIdentity)}[\"PK_InvalidScalarPart\"].{nameof(ApiIdentityScalarPart)}[\"NestedPart\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_IDENTITY_PART_INVALID_API_PROPERTY_TYPE,
                    description: "Property 'NestedPart' must be a scalar type for a scalar identity part",
                    remediation: $"Use a scalar-typed property or switch to {nameof(ApiIdentityNestedPart)}"
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

        // ApiObjectType throws if ApiProperties is null or empty
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiObjectType)} Throws If ApiProperties Is Null Or Empty",
            SourceJson = @"
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
            SourceJson = @"
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

        // ApiSchema throws if ApiName is invalid (warning - uses null ClrType scalar type as companion error)
        new InitializeThrowsTest
        {
            Name = $"{nameof(ApiSchema)} Throws If {nameof(ApiSchema.ApiName)} Is Invalid",
            SourceJson = @"
            {
                ""ApiName"": """",
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""String""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": []
            }",
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=1, Warnings=1.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiSchema)}",
                    severity: ApiInitializationSeverity.Warning,
                    code: ApiInitializationCode.API_SCHEMA_INVALID_NAME,
                    description: $"{nameof(ApiSchema.ApiName)} must not be null, empty, or whitespace",
                    remediation: $"Specify a valid {nameof(ApiSchema.ApiName)} value"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiScalarType)}[\"String\"]",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_TYPE_NULL_CLR_TYPE,
                    description: $"{nameof(ApiScalarType.ClrType)} must not be null",
                    remediation: "Specify a valid ClrType"
                ),
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
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""NameAlt"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
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
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""NameAlt"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
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
                                ""ClrName"": ""Name""
                            },
                            {
                                ""ApiName"": ""NameAlt"",
                                ""ApiType"": { ""ApiKind"": ""Scalar"", ""ApiName"": ""String"" },
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
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(InitializeThrowsTheoryData))]
    public void InitializeThrows(IXUnitTest test) => test.Execute(this);
    #endregion
}
