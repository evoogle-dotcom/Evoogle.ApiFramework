// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;
using Evoogle.XUnit;

using BuildTest = Evoogle.ApiFramework.Schema.TestData.ApiSchemaJsonExpressionBuildTest;
using BuildThrowsTest = Evoogle.ApiFramework.Schema.TestData.ApiSchemaExpressionBuildThrowsTest;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

public partial class ApiConventionTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        // EnumType - EnumValue Discovery Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiEnumTypeEnumValueDiscoveryConvention)} that discovers all enum values",
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
                  ""ApiName"": ""PipelineStatus"",
                  ""ApiEnumValues"": [
                    {
                      ""ApiName"": ""Active"",
                      ""ClrName"": ""Active"",
                      ""ClrOrdinal"": 0
                    },
                    {
                      ""ApiName"": ""InProgress"",
                      ""ClrName"": ""InProgress"",
                      ""ClrOrdinal"": 1
                    },
                    {
                      ""ApiName"": ""OnHold"",
                      ""ClrName"": ""OnHold"",
                      ""ClrOrdinal"": 2
                    },
                    {
                      ""ApiName"": ""Queued"",
                      ""ClrName"": ""Queued"",
                      ""ClrOrdinal"": 3
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithEnumTypeEnumValueDiscoveryThatDiscoversAllEnumValues()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiEnumTypeEnumValueDiscoveryConvention)} with explicit enum value override",
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
                  ""ApiName"": ""PipelineStatus"",
                  ""ApiEnumValues"": [
                    {
                      ""ApiName"": ""Enabled"",
                      ""ClrName"": ""Active"",
                      ""ClrOrdinal"": 0
                    },
                    {
                      ""ApiName"": ""InProgress"",
                      ""ClrName"": ""InProgress"",
                      ""ClrOrdinal"": 1
                    },
                    {
                      ""ApiName"": ""OnHold"",
                      ""ClrName"": ""OnHold"",
                      ""ClrOrdinal"": 2
                    },
                    {
                      ""ApiName"": ""Queued"",
                      ""ClrName"": ""Queued"",
                      ""ClrOrdinal"": 3
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithEnumTypeEnumValueDiscoveryWithExplicitEnumValueOverride()
        },

        // Naming Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingConvention)} for scalar type",
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
                  ""ApiName"": ""customScalar"",
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+CustomScalar, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiEnumTypes"": [],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithNamingConventionForScalarType()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingConvention)} for enum type and all enum values",
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
                  ""ApiName"": ""pipelineStatus"",
                  ""ApiEnumValues"": [
                    {
                      ""ApiName"": ""active"",
                      ""ClrName"": ""Active"",
                      ""ClrOrdinal"": 0
                    },
                    {
                      ""ApiName"": ""inProgress"",
                      ""ClrName"": ""InProgress"",
                      ""ClrOrdinal"": 1
                    },
                    {
                      ""ApiName"": ""onHold"",
                      ""ClrName"": ""OnHold"",
                      ""ClrOrdinal"": 2
                    },
                    {
                      ""ApiName"": ""queued"",
                      ""ClrName"": ""Queued"",
                      ""ClrOrdinal"": 3
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithNamingConventionForEnumTypeAndAllEnumValues()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingConvention)} for enum type and preserves typed explicit enum value API name",
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
                  ""ApiName"": ""pipelineStatus"",
                  ""ApiEnumValues"": [
                    {
                      ""ApiName"": ""Enabled"",
                      ""ClrName"": ""Active"",
                      ""ClrOrdinal"": 0
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory
                    .BuildWithNamingConventionForEnumTypeAndPreservesTypedExplicitEnumValueApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingConvention)} for enum type and preserves string explicit enum value API names",
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
                  ""ApiName"": ""pipelineStatus"",
                  ""ApiEnumValues"": [
                    {
                      ""ApiName"": ""Enabled"",
                      ""ClrName"": ""Active"",
                      ""ClrOrdinal"": 0
                    },
                    {
                      ""ApiName"": ""InProgress"",
                      ""ClrName"": ""InProgress"",
                      ""ClrOrdinal"": 1
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory
                    .BuildWithNamingConventionForEnumTypeAndPreservesStringExplicitEnumValueApiNames()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingConvention)} for object type and expression inferred property names",
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
                        ""ApiName"": ""guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""string"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""personWithId"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""email"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Email"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithNamingConventionForObjectTypeAndExpressionInferredPropertyNames()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingConvention)} for object type and preserves selector explicit API name",
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
                        ""ApiName"": ""guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""string"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""personWithId"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""EmailAddress"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Email"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithNamingConventionForObjectTypeAndPreservesSelectorExplicitApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingConvention)} for object type and preserves callback explicit API name",
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
                        ""ApiName"": ""guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""string"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""personWithId"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""EmailAddress"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Email"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithNamingConventionForObjectTypeAndPreservesCallbackExplicitApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingConvention)} for object type and preserves string based explicit API names",
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
                        ""ApiName"": ""guid"",
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                    },
                    {
                        ""ApiKind"": ""Scalar"",
                        ""ApiName"": ""string"",
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                    }
                ],
                ""ApiEnumTypes"": [],
                ""ApiObjectTypes"": [
                    {
                        ""ApiKind"": ""Object"",
                        ""ApiName"": ""Person"",
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
                                ""ApiName"": ""EmailAddress"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Email"",
                                ""ClrMemberKind"": ""Property""
                            }
                        ],
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithNamingConventionForObjectTypeAndPreservesStringBasedExplicitApiNames()
        },

        new BuildTest
        {
            Name = $"Build with custom {nameof(ApiNamingConvention)}s that appends 'Api' and 'Model' to API name",
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
                  ""ApiName"": ""PipelineStatusApiModel"",
                  ""ApiEnumValues"": [
                    {
                      ""ApiName"": ""ActiveApiModel"",
                      ""ClrName"": ""Active"",
                      ""ClrOrdinal"": 0
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiObjectTypes"": [
                {
                  ""ApiKind"": ""Object"",
                  ""ApiName"": ""PersonWithIdApiModel"",
                  ""ApiProperties"": [],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithCustomNamingConventionsThatAppendsApiAndModelToApiName()
        },

        new BuildTest
        {
            Name = $"Build with custom {nameof(ApiNamingConvention)}s that hard codes API enum value and appends 'Changed' to API name",
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
                  ""ApiName"": ""PipelineStatusChanged"",
                  ""ApiEnumValues"": [
                    {
                      ""ApiName"": ""LockedName"",
                      ""ClrName"": ""Active"",
                      ""ClrOrdinal"": 0
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithCustomNamingConventionsThatHardCodesApiEnumValueAndAppendsChangedToApiName()
        },

        // ObjectType - PrimaryKey Inference Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePrimaryKeyInferenceConvention)} that discovers primary key property 'Id'",
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
                  ""ApiName"": ""PersonWithId"",
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
                  ""ApiKeyTypes"": [
                    {
                      ""ApiName"": ""PrimaryKey"",
                      ""ApiKeyPaths"": [
                        {
                          ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests"",
                          ""ApiSegments"": [
                            {
                              ""ClrPropertyName"": ""Id""
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithObjectTypePrimaryKeyInferenceThatDiscoversId()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePrimaryKeyInferenceConvention)} that discovers primary key property 'Id' but does not overwrite an explicit primary key",
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
                  ""ApiName"": ""PersonWithId"",
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
                  ""ApiKeyTypes"": [
                    {
                      ""ApiName"": ""PrimaryKey"",
                      ""ApiKeyPaths"": [
                        {
                          ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests"",
                          ""ApiSegments"": [
                            {
                              ""ClrPropertyName"": ""Name""
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithObjectTypePrimaryKeyInferenceThatDiscoversIdButDoesNotOverwriteAnExplicitPrimaryKey()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePrimaryKeyInferenceConvention)} that discovers primary key property 'ClassNameId'",
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
                  ""ApiName"": ""OrderItem"",
                  ""ApiProperties"": [
                    {
                      ""ApiName"": ""OrderItemId"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""OrderItemId"",
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
                      ""ApiName"": ""PrimaryKey"",
                      ""ApiKeyPaths"": [
                        {
                          ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+OrderItem, Evoogle.ApiFramework.Tests"",
                          ""ApiSegments"": [
                            {
                              ""ClrPropertyName"": ""OrderItemId""
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+OrderItem, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithObjectTypePrimaryKeyInferenceThatDiscoversClassNameId()
        },

        // ObjectType - Property Configuration Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiPropertyNullabilityModifierConvention)}",
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
                  ""ApiName"": ""PersonWithId"",
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithObjectTypePropertyNullabilityModifiers()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiPropertyNullabilityModifierConvention)} and explicit property modifier",
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
                  ""ApiName"": ""PersonWithId"",
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
                      ""ApiName"": ""email"",
                      ""ApiType"": {
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Email"",
                      ""ClrMemberKind"": ""Property""
                    }
                  ],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithObjectTypePropertyNullabilityModifiersAndExplicitPropertyModifier()
        },

        // ObjectType - Property Discovery Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePropertyDiscoveryConvention)} that discovers public instance properties",
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
                        ""ApiName"": ""PersonWithId"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithObjectTypePropertyDiscoveryThatDiscoversPublicInstanceProperties()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePropertyDiscoveryConvention)} that discovers public instance fields",
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
                        ""ApiName"": ""TypeWithField"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""Id"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Field""
                            },
                            {
                                ""ApiName"": ""Name"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.String, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Name"",
                                ""ClrMemberKind"": ""Field""
                            },
                            {
                                ""ApiName"": ""Count"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Int32, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""None"",
                                ""ClrName"": ""Count"",
                                ""ClrMemberKind"": ""Field""
                            }
                        ],
                        ""ApiKeyTypes"": [],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+TypeWithField, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithObjectTypePropertyDiscoveryThatDiscoversPublicInstanceFields()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePropertyDiscoveryConvention)} that does not duplicate explicitly added properties",
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
                        ""ApiName"": ""PersonWithId"",
                        ""ApiProperties"": [
                            {
                                ""ApiName"": ""identifier"",
                                ""ApiType"": {
                                    ""ClrType"": ""System.Guid, System.Private.CoreLib""
                                },
                                ""ApiTypeModifiers"": ""Required"",
                                ""ClrName"": ""Id"",
                                ""ClrMemberKind"": ""Property""
                            },
                            {
                                ""ApiName"": ""displayName"",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithObjectTypePropertyDiscoveryThatDoesNotDuplicateExplicitlyAddedProperties()
        },

        // Schema - Type Discovery Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiSchemaAssemblyTypeInferenceConvention)}",
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
                  ""ApiName"": ""AssemblyScannedScalar"",
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+AssemblyScannedScalar, Evoogle.ApiFramework.Tests""
                },
                {
                  ""ApiKind"": ""Scalar"",
                  ""ApiName"": ""Guid"",
                  ""ClrType"": ""System.Guid, System.Private.CoreLib""
                }
              ],
              ""ApiEnumTypes"": [
                {
                  ""ApiKind"": ""Enum"",
                  ""ApiName"": ""AssemblyScannedEnum"",
                  ""ApiEnumValues"": [
                    {
                      ""ApiName"": ""Active"",
                      ""ClrName"": ""Active"",
                      ""ClrOrdinal"": 0
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+AssemblyScannedEnum, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiObjectTypes"": [
                {
                  ""ApiKind"": ""Object"",
                  ""ApiName"": ""AssemblyScannedObject"",
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
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+AssemblyScannedObject, Evoogle.ApiFramework.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithSchemaAssemblyTypeInference()
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildThrowsTheoryData =>
    [
        new BuildThrowsTest
        {
            Name = "Build rejects convention phase that is invalid for target",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected = "*InvalidPropertyPhaseConvention*Discovery*IApiPropertyConvention*Configuration*",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithInvalidPropertyConventionPhase()
        },

        new BuildThrowsTest
        {
            Name = "Build rejects structural registration from relationship convention",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected = "Relationship conventions cannot register schema types, properties, or enum values.*",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithRelationshipStructuralRegistration()
        },

        new BuildThrowsTest
        {
            Name = "Build rejects non-converging convention pipeline",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected = "The convention pipeline exceeded * iterations.*",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithNonConvergingConventionPipeline()
        },

        new BuildThrowsTest
        {
            Name = "Build rejects non-converging property pipeline",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected = "The property convention pipeline exceeded * iterations.*",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithNonConvergingPropertyPipeline()
        },

        new BuildThrowsTest
        {
            Name = "Build rejects non-converging enum value pipeline",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected = "The enum-value convention pipeline exceeded * iterations.*",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithNonConvergingEnumValuePipeline()
        },
    ];
    #endregion
}
