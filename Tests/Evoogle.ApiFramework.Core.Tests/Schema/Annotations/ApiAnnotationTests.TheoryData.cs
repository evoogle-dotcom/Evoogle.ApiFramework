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

namespace Evoogle.ApiFramework.Schema.Annotations;

public partial class ApiAnnotationTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        // ApiObjectTypeAttribute Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypeAttribute)} overrides API name",
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
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+PersonAnnotated, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiAnnotationTestsFactory.BuildWithApiObjectTypeAttributeOverridesApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiScalarTypeAttribute)} overrides API name",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+EmailValueAnnotated, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiScalarTypeAttributeOverridesApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiEnumTypeAttribute)} overrides API name",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OrderStatusAnnotated, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithApiEnumTypeAttributeOverridesApiName()
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OrderStatusValueAnnotated, Evoogle.ApiFramework.Core.Tests""
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OrderStatusValueAnnotated, Evoogle.ApiFramework.Core.Tests""
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OrderStatusValueAnnotated, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+PersonWithPropertyAnnotations, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""FieldKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+FieldAnnotationsType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Code"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+FieldAnnotationsType, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+PersonWithKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+PersonWithKeyAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiName"": ""ScalarKeyTypeAnnotation"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ScalarKeyTypeAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""AlternateKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ScalarKeyTypeAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Name"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ScalarKeyTypeAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiName"": ""CompositeKeyType"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""OrderLineKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+CompositeKeyType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""OrderId"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+CompositeKeyType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""LineItemNumber"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+CompositeKeyType, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiName"": ""ThreePartCompositeKeyType"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""ThreePartKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ThreePartCompositeKeyType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id1"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ThreePartCompositeKeyType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id2"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ThreePartCompositeKeyType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id3"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ThreePartCompositeKeyType, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""NestedPartKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+NestedKeyPartAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+NestedKeyPartAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""OwnerKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiName"": ""AnnotationPrimaryKeyType"",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+AnnotationPrimaryKeyType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Code"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+AnnotationPrimaryKeyType, Evoogle.ApiFramework.Core.Tests""
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
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+NestedKeyPartAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""NestedCompositeKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+NestedCompositeKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""NestedPart"" },
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+NestedCompositeKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Name"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+NestedCompositeKeyAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""NestedPartKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+NestedKeyPartAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+NestedKeyPartAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""OwnedCompositeKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    },
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnedCompositeKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""LineNumber"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnedCompositeKeyAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""OwnerKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""OwnedDependentKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnedDependentKeyAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""OwnerKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+OwnerKeyAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""DuplicatePathKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+DuplicatePathKeyAnnotation, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+DuplicatePathKeyAnnotation, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Customer, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Customer, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToMany"",
                        ""ApiName"": ""CustomerHasOrders"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Customer, Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyType"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""CustomerId"" }
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
            Name = $"Build with {nameof(ApiRelationshipTypeAttribute)} at type level",
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
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Invoice, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""OneToOne"",
                        ""ApiName"": ""InvoiceForOrder"",
                        ""ApiPrincipalEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Order, Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiDependentEnd"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Invoice, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyType"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Invoice, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""OrderId"" }
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
                ApiAnnotationTestsFactory.BuildWithApiRelationshipTypeAttributeAtTypeLevel()
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Product, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Product, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ProductTag, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Tag, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Tag, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""ProductHasTags"",
                        ""ApiPrincipalEndA"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Product, Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiPrincipalEndB"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Tag, Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiAssociation"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ProductTag, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyTypeA"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ProductTag, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""ProductId"" }
                                        ]
                                    }
                                ]
                            },
                            ""ApiForeignKeyTypeB"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ProductTag, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""TagId"" }
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
            Name = $"Build with {nameof(ApiManyToManyRelationshipTypeAttribute)} at type level",
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Category, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Category, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [
                            {
                                ""ApiName"": ""PrimaryKey"",
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Label, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""Id"" }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Label, Evoogle.ApiFramework.Core.Tests""
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
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ProductTagFromType, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiKind"": ""ManyToMany"",
                        ""ApiName"": ""ProductHasTagsFromType"",
                        ""ApiPrincipalEndA"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Category, Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiPrincipalEndB"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+Label, Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiAssociation"": {
                            ""ClrObjectType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ProductTagFromType, Evoogle.ApiFramework.Core.Tests"",
                            ""ApiForeignKeyTypeA"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ProductTagFromType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""ProductId"" }
                                        ]
                                    }
                                ]
                            },
                            ""ApiForeignKeyTypeB"": {
                                ""ApiKeyPaths"": [
                                    {
                                        ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests+ProductTagFromType, Evoogle.ApiFramework.Core.Tests"",
                                        ""ApiSegments"": [
                                            { ""ClrPropertyName"": ""TagId"" }
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
                ApiAnnotationTestsFactory.BuildWithApiManyToManyRelationshipTypeAttributeAtTypeLevel()
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildThrowsTheoryData =>
    [
        new BuildThrowsTest
        {
            Name = $"Build rejects a type-level {nameof(ApiKeyAttribute)} without {nameof(ApiKeyAttribute.ClrPath)}",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected = "*ClrPath*type level*",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithMissingTypeLevelKeyPathAnnotation()
        },

        new BuildThrowsTest
        {
            Name = $"Build rejects a malformed type-level {nameof(ApiKeyAttribute)} path",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected = "*Nested..Id*",
            ApiSchemaActualBuildExpression = static () =>
                ApiAnnotationTestsFactory.BuildWithMalformedTypeLevelKeyPathAnnotation()
        },

        new BuildThrowsTest
        {
            Name = $"Build rejects an unresolved type-level {nameof(ApiKeyAttribute)} path",
            ExceptionTypeExpected = typeof(ApiSchemaInitializationException),
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
            (nameof(ApiRelationshipTypeAttribute), null,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiRelationshipTypeAttribute),
                        null
                    )),
            (nameof(ApiRelationshipTypeAttribute), string.Empty,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiRelationshipTypeAttribute),
                        string.Empty
                    )),
            (nameof(ApiRelationshipTypeAttribute), " ",
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiRelationshipTypeAttribute),
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
            (nameof(ApiManyToManyRelationshipTypeAttribute), null,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiManyToManyRelationshipTypeAttribute),
                        null
                    )),
            (nameof(ApiManyToManyRelationshipTypeAttribute), string.Empty,
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiManyToManyRelationshipTypeAttribute),
                        string.Empty
                    )),
            (nameof(ApiManyToManyRelationshipTypeAttribute), " ",
                static () => ApiAnnotationTestsFactory
                    .BuildWithInvalidRequiredAnnotationApiName
                    (
                        nameof(ApiManyToManyRelationshipTypeAttribute),
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
