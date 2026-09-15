// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.XUnit;

using BuildTest = Evoogle.ApiFramework.Schema.TestData.ApiSchemaJsonExpressionBuildTest;
using BuildThrowsTest = Evoogle.ApiFramework.Schema.TestData.ApiSchemaExpressionBuildThrowsTest;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

public partial class ApiAnnotationTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        // ApiObjectAttribute Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectAttribute)} overrides API name",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
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
                        ""ApiName"": ""RenamedPerson"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Email"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Email"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+PersonAnnotated, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiAnnotationTestsFactory.BuildWithApiObjectAttributeOverridesApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiScalarAttribute)} overrides API name",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""EmailValue"",
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+EmailValueAnnotated, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiScalarAttributeOverridesApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiEnumAttribute)} overrides API name",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""OrderState"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""Pending"",
                                ""ClrName"": ""Pending"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""Shipped"",
                                ""ClrName"": ""Shipped"",
                                ""ClrOrdinal"": 1
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OrderStatusAnnotated, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiEnumAttributeOverridesApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiEnumValueAttribute)} overrides enum value API name",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""OrderStatusValueAnnotated"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""awaiting_payment"",
                                ""ClrName"": ""Pending"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""Shipped"",
                                ""ClrName"": ""Shipped"",
                                ""ClrOrdinal"": 1
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OrderStatusValueAnnotated, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiEnumValueAttributeOverridesApiName()
        },

        new BuildTest
        {
            Name = $"Build with explicit enum value API name overriding {nameof(ApiEnumValueAttribute)}",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""OrderStatusValueAnnotated"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""explicit_pending"",
                                ""ClrName"": ""Pending"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""Shipped"",
                                ""ClrName"": ""Shipped"",
                                ""ClrOrdinal"": 1
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OrderStatusValueAnnotated, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory
                    .BuildWithExplicitEnumValueNameOverridesApiEnumValueAttribute()
        },

        new BuildTest
        {
            Name = "Build with a custom annotation reader receiving enum value annotations",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [],
                ""ApiEnumTypes"": [
                    {
                        ""ApiKind"": ""Enum"",
                        ""ApiName"": ""OrderStatusValueAnnotated"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""reader_Pending"",
                                ""ClrName"": ""Pending"",
                                ""ClrOrdinal"": 0
                            },
                            {
                                ""ApiName"": ""reader_Shipped"",
                                ""ClrName"": ""Shipped"",
                                ""ClrOrdinal"": 1
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OrderStatusValueAnnotated, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithCustomEnumValueAnnotationReader()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiPropertyAttribute)} configures name and modifiers",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
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
                        ""ApiName"": ""PersonWithPropertyAnnotations"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""display_name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""NonNullableButOptional"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""NonNullableButOptional"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""RequiredWins"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""RequiredWins"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+PersonWithPropertyAnnotations, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiPropertyAttributesConfigureNameAndModifiers()
        },

        new BuildTest
        {
            Name = "Build with field annotations configures name, key, and ignore",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
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
                        ""ApiName"": ""FieldAnnotationsType"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""field_code"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Code"",
                                ""ClrMemberKind"": ""Field""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""FieldKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Code"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+FieldAnnotationsType, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiFieldAttributesConfigureNameKeyAndIgnore()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} creates a primary key",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
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
                        ""ApiName"": ""PersonWithKeyAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+PersonWithKeyAnnotation, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributeCreatesPrimaryKey()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} creates primary and alternate scalar keys",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
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
                        ""ApiName"": ""ScalarKeyAnnotationObject"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AlternateKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Name"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ScalarKeyAnnotationObject, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributesCreatePrimaryAndAlternateScalarKeys()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} creates an ordered named composite key",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Int64"",
                        ""ClrType"": ""System.Int64, System.Private.CoreLib""
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
                        ""ApiName"": ""CompositeKeyObject"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""OrderId"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""OrderId"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""LineItemNumber"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int64, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""LineItemNumber"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""OrderLineKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""OrderId"" }
                                        ]
                                    },
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""LineItemNumber"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+CompositeKeyObject, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributesCreateOrderedNamedCompositeKey()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} creates a three-part scalar composite key",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                    },
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
                        ""ApiName"": ""ThreePartCompositeKeyObject"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id1"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id1"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Id2"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Id2"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Id3"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id3"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""ThreePartKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id1"" }
                                        ]
                                    },
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id2"" }
                                        ]
                                    },
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id3"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ThreePartCompositeKeyObject, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributesCreateThreePartScalarCompositeKey()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} creates a key on the nested key-part type",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
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
                        ""ApiName"": ""NestedKeyPartAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""NestedPartKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+NestedKeyPartAnnotation, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributeCreatesNestedTypePrimaryKey()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} creates a key on the owner type",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
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
                        ""ApiName"": ""OwnerKeyAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""OwnerKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributeCreatesOwnerTypePrimaryKey()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} before primary-key inference",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
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
                        ""ApiName"": ""AnnotationPrimaryKeyObject"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Code"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Code"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Code"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+AnnotationPrimaryKeyObject, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributeRunsBeforePrimaryKeyInference()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} creates a nested composite key",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
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
                        ""ApiName"": ""NestedCompositeKeyAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""NestedPart"",
                                ""ApiType"": {
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+NestedKeyPartAnnotation, Evoogle.ApiFramework.Tests""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""NestedPart"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""NestedCompositeKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""NestedPart"" },
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    },
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Name"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+NestedCompositeKeyAnnotation, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""NestedKeyPartAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""NestedPartKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+NestedKeyPartAnnotation, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributesCreateNestedCompositeKey()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} creates an owned composite key",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
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
                        ""ApiName"": ""OwnedCompositeKeyAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""LineNumber"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""LineNumber"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""OwnedCompositeKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiRootObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Tests"" },
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    },
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""LineNumber"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OwnedCompositeKeyAnnotation, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""OwnerKeyAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""OwnerKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributesCreateOwnedCompositeKey()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} creates an owned dependent key",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
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
                        ""ApiName"": ""OwnedDependentKeyAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""OwnedDependentKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiRootObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Tests"" },
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OwnedDependentKeyAnnotation, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""OwnerKeyAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Description"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Description"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""OwnerKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributeCreatesOwnedDependentKey()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiKeyAttribute)} suppresses duplicate paths",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
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
                        ""ApiName"": ""DuplicatePathKeyAnnotation"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""DuplicatePathKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+DuplicatePathKeyAnnotation, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiKeyAttributesCreateDuplicatePath()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiRelationshipAttribute)} on a navigation property",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Decimal"",
                        ""ClrType"": ""System.Decimal, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
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
                        ""ApiName"": ""Customer"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Customer, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Order"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""CustomerId"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""CustomerId"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Total"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Decimal, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Total"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""CustomerHasOrders"",
                        ""ApiPrincipalEnd"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Customer, Evoogle.ApiFramework.Tests"" }
                        },
                        ""ApiDependentEnd"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Tests"" },
                            ""ApiForeignKey"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiRootObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Tests"" },
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""CustomerId"" }
                                        ]
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    }
                ]
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiRelationshipAttributeOnNavigationProperty()
        },

        new BuildTest
        {
            Name =
                $"Build with {nameof(ApiRelationshipDefinitionAttribute)} at type level",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Decimal"",
                        ""ClrType"": ""System.Decimal, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Invoice"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""OrderId"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""OrderId"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""Amount"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Decimal, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Amount"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Invoice, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Order"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToOne"",
                        ""ApiName"": ""InvoiceForOrder"",
                        ""ApiPrincipalEnd"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Tests"" }
                        },
                        ""ApiDependentEnd"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Invoice, Evoogle.ApiFramework.Tests"" },
                            ""ApiForeignKey"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiRootObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Invoice, Evoogle.ApiFramework.Tests"" },
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""OrderId"" }
                                        ]
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""None""
                    }
                ]
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiRelationshipDefinitionAttributeAtTypeLevel()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiManyToManyRelationshipAttribute)} on a navigation property",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Product"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Product, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""ProductTag"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""ProductId"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""ProductId"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""TagId"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""TagId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ProductTag, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Tag"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Tag, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""ProductHasTags"",
                        ""ApiPrincipalEndA"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Product, Evoogle.ApiFramework.Tests"" }
                        },
                        ""ApiPrincipalEndB"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Tag, Evoogle.ApiFramework.Tests"" }
                        },
                        ""ApiAssociation"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ProductTag, Evoogle.ApiFramework.Tests"" },
                            ""ApiForeignKeyA"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiRootObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ProductTag, Evoogle.ApiFramework.Tests"" },
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""ProductId"" }
                                        ]
                                    }
                                ]
                            },
                            ""ApiForeignKeyB"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiRootObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ProductTag, Evoogle.ApiFramework.Tests"" },
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""TagId"" }
                                        ]
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    }
                ]
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiManyToManyRelationshipAttributeOnNavigationProperty()
        },

        new BuildTest
        {
            Name =
                $"Build with {nameof(ApiManyToManyRelationshipDefinitionAttribute)} at type level",
            ApiSchemaExpectedJson = @"
            {
                ""ApiName"": ""Test"",
                ""ApiVersion"": ""0.1.0"",
                ""ApiOptions"": {
                    ""ApiKeyNullHandling"": ""UseDefaultOnNull""
                },
                ""ApiScalarTypes"": [
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""Guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Category"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Category, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Label"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Label, Evoogle.ApiFramework.Tests""
                    },
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""ProductTagFromType"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""ProductId"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""ProductId"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""TagId"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""TagId"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeys"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ProductTagFromType, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""ProductHasTagsFromType"",
                        ""ApiPrincipalEndA"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Category, Evoogle.ApiFramework.Tests"" }
                        },
                        ""ApiPrincipalEndB"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+Label, Evoogle.ApiFramework.Tests"" }
                        },
                        ""ApiAssociation"": {
                            ""ApiObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ProductTagFromType, Evoogle.ApiFramework.Tests"" },
                            ""ApiForeignKeyA"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiRootObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ProductTagFromType, Evoogle.ApiFramework.Tests"" },
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""ProductId"" }
                                        ]
                                    }
                                ]
                            },
                            ""ApiForeignKeyB"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ApiRootObjectType"": { ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests+ProductTagFromType, Evoogle.ApiFramework.Tests"" },
                                        ""ApiSegments"": [
                                            { ""ClrMemberName"": ""TagId"" }
                                        ]
                                    }
                                ]
                            }
                        },
                        ""ApiDeleteBehavior"": ""Delete""
                    }
                ]
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory
                    .BuildWithApiManyToManyRelationshipDefinitionAttributeAtTypeLevel()
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildThrowsTheoryData =>
    [
        new BuildThrowsTest
        {
            Name = $"Build rejects a type-level {nameof(ApiKeyAttribute)} without {nameof(ApiKeyAttribute.ClrPath)}",
            ExceptionTypeExpected = typeof(ApiSchemaCompilationException),
            ExceptionMessagePatternExpected = "*ClrPath*type level*",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithMissingTypeLevelKeyPathAnnotation()
        },

        new BuildThrowsTest
        {
            Name = $"Build rejects a malformed type-level {nameof(ApiKeyAttribute)} path",
            ExceptionTypeExpected = typeof(ApiSchemaCompilationException),
            ExceptionMessagePatternExpected = "*Nested..Id*",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithMalformedTypeLevelKeyPathAnnotation()
        },

        new BuildThrowsTest
        {
            Name = $"Build rejects an unresolved type-level {nameof(ApiKeyAttribute)} path",
            ExceptionTypeExpected = typeof(ApiSchemaCompilationException),
            ExceptionMessagePatternExpected = "*ApiKeyPathSegmentUnresolvedApiProperty*",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithUnresolvedTypeLevelKeyPathAnnotation()
        },

        ..BuildInvalidRequiredAnnotationApiNameTheoryData()
    ];

    private static TheoryDataRow<IXUnitTest>[] BuildInvalidRequiredAnnotationApiNameTheoryData()
    {
        var invalidApiNameTests = new
        (
            string AnnotationType,
            string? ApiName,
            Expression<Func<ApiSchema>> BuildExpression
        )[]
        {
            (nameof(ApiKeyAttribute), null,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiKeyAttribute),
                        null
                    )),
            (nameof(ApiKeyAttribute), string.Empty,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiKeyAttribute),
                        string.Empty
                    )),
            (nameof(ApiKeyAttribute), " ",
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiKeyAttribute),
                        " "
                    )),
            (nameof(ApiRelationshipAttribute), null,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiRelationshipAttribute),
                        null
                    )),
            (nameof(ApiRelationshipAttribute), string.Empty,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiRelationshipAttribute),
                        string.Empty
                    )),
            (nameof(ApiRelationshipAttribute), " ",
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiRelationshipAttribute),
                        " "
                    )),
            (nameof(ApiRelationshipDefinitionAttribute), null,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiRelationshipDefinitionAttribute),
                        null
                    )),
            (nameof(ApiRelationshipDefinitionAttribute), string.Empty,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiRelationshipDefinitionAttribute),
                        string.Empty
                    )),
            (nameof(ApiRelationshipDefinitionAttribute), " ",
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiRelationshipDefinitionAttribute),
                        " "
                    )),
            (nameof(ApiManyToManyRelationshipAttribute), null,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiManyToManyRelationshipAttribute),
                        null
                    )),
            (nameof(ApiManyToManyRelationshipAttribute), string.Empty,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiManyToManyRelationshipAttribute),
                        string.Empty
                    )),
            (nameof(ApiManyToManyRelationshipAttribute), " ",
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiManyToManyRelationshipAttribute),
                        " "
                    )),
            (nameof(ApiManyToManyRelationshipDefinitionAttribute), null,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiManyToManyRelationshipDefinitionAttribute),
                        null
                    )),
            (nameof(ApiManyToManyRelationshipDefinitionAttribute), string.Empty,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiManyToManyRelationshipDefinitionAttribute),
                        string.Empty
                    )),
            (nameof(ApiManyToManyRelationshipDefinitionAttribute), " ",
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiManyToManyRelationshipDefinitionAttribute),
                        " "
                    )),
        };
        var tests = new List<TheoryDataRow<IXUnitTest>>();

        foreach (var (annotationType, apiName, buildExpression) in invalidApiNameTests)
        {
            tests.Add
            (
                new BuildThrowsTest
                {
                    Name = $"Build rejects invalid {annotationType} API name " +
                        $"'{apiName ?? "null"}'",
                    ExceptionTypeExpected = apiName switch
                    {
                        null => typeof(ArgumentNullException),
                        _ => typeof(ArgumentException)
                    },
                    ExceptionMessagePatternExpected = "*apiName*",
                    ApiSchemaActualBuildExpression = buildExpression
                }
            );
        }

        return [.. tests];
    }
    #endregion
}
