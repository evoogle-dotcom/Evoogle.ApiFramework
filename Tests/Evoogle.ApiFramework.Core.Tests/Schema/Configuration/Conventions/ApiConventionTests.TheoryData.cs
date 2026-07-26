// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

public partial class ApiConventionTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        // Camel Case Naming Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} camel case naming expression inferred property names",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithCamelCaseNamingExpressionInferredPropertyNames()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} camel case naming required and optional expression property names",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithCamelCaseNamingRequiredAndOptionalExpressionPropertyNames()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} camel case naming preserves selector explicit API name",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithCamelCaseNamingPreservesSelectorExplicitApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} camel case naming preserves callback explicit API name",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithCamelCaseNamingPreservesCallbackExplicitApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} camel case naming preserves string based explicit API names",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithCamelCaseNamingPreservesStringBasedExplicitApiNames()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} camel case for enum type and values",
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
                        ""ApiName"": ""customEnum"",
                        ""ApiEnumValues"": [
                            {
                                ""ApiName"": ""active"",
                                ""ClrName"": ""Active"",
                                ""ClrOrdinal"": 0
                            }
                        ],
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+CustomEnum, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiObjectTypes"": [],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithCamelCaseNamingForEnumTypeAndValues()
        },

        // Property Discovery Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePropertyDiscoveryConvention)} discovers public instance properties",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithPropertyDiscoveryDiscoversPublicInstanceProperties()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePropertyDiscoveryConvention)} discovers public fields",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+TypeWithField, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithPropertyDiscoveryDiscoversPublicFields()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePropertyDiscoveryConvention)} does not duplicate explicitly added properties",
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
                        ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                    }
                ],
                ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () => ApiConventionTestsFactory.BuildWithPropertyDiscoveryDoesNotDuplicateExplicitlyAddedProperties()
        },

        // Built-In Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePropertyDiscoveryConvention)} and " +
                $"{nameof(ApiNamingCamelCaseConvention)}",
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithPropertyDiscoveryAndCamelCaseNaming()
        },

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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithPropertyNullabilityModifiers()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePrimaryKeyInferenceConvention)}",
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
                          ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests"",
                          ""ApiSegments"": [
                            {
                              ""ClrPropertyName"": ""Id""
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithPrimaryKeyInference()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiSchemaAssemblyScanConvention)}",
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+AssemblyScannedScalar, Evoogle.ApiFramework.Core.Tests""
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+AssemblyScannedEnum, Evoogle.ApiFramework.Core.Tests""
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+AssemblyScannedObject, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithAssemblyScanning()
        },

        // Naming Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} for scalar type",
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+CustomScalar, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiEnumTypes"": [],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithCamelCaseNamingForScalarType()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} for all enum values",
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithCamelCaseNamingForAllEnumValues()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} preserves typed " +
                "explicit enum value API name",
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory
                    .BuildWithCamelCaseNamingPreservesTypedExplicitEnumValueApiName()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiNamingCamelCaseConvention)} preserves string " +
                "explicit enum value API names",
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory
                    .BuildWithCamelCaseNamingPreservesStringExplicitEnumValueApiNames()
        },

        new BuildTest
        {
            Name = $"Build with composed {nameof(ApiNamingConvention)} instances",
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiObjectTypes"": [
                {
                  ""ApiKind"": ""Object"",
                  ""ApiName"": ""PersonWithIdApiModel"",
                  ""ApiProperties"": [],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithComposedNamingConventions()
        },

        new BuildTest
        {
            Name = "Build with explicit enum value convention API name",
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
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithExplicitEnumValueConventionName()
        },

        // Configuration and Default Convention Tests
        new BuildTest
        {
            Name = $"Build with {nameof(ApiPropertyNullabilityModifierConvention)} preserves " +
                "explicit modifier",
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
                      ""ApiName"": ""email"",
                      ""ApiType"": {
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Email"",
                      ""ClrMemberKind"": ""Property""
                    },
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
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithExplicitPropertyModifier()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePrimaryKeyInferenceConvention)} for " +
                "class name ID property",
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
                          ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+OrderItem, Evoogle.ApiFramework.Core.Tests"",
                          ""ApiSegments"": [
                            {
                              ""ClrPropertyName"": ""OrderItemId""
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+OrderItem, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithClassNamePrimaryKeyInference()
        },

        new BuildTest
        {
            Name = $"Build with {nameof(ApiObjectTypePrimaryKeyInferenceConvention)} preserves " +
                "existing primary key",
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
                          ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests"",
                          ""ApiSegments"": [
                            {
                              ""ClrPropertyName"": ""Id""
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithExistingPrimaryKey()
        },

        new BuildTest
        {
            Name = "Build with default conventions",
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
                  ""ApiKeyTypes"": [
                    {
                      ""ApiName"": ""PrimaryKey"",
                      ""ApiKeyPaths"": [
                        {
                          ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests"",
                          ""ApiSegments"": [
                            {
                              ""ClrPropertyName"": ""Id""
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithDefaultConventions()
        },

        new BuildTest
        {
            Name = "Build with explicit configuration takes precedence over conventions",
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
                  ""ApiName"": ""MyPerson"",
                  ""ApiProperties"": [
                    {
                      ""ApiName"": ""emailAddress"",
                      ""ApiType"": {
                        ""ClrType"": ""System.String, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Email"",
                      ""ClrMemberKind"": ""Property""
                    },
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
                    }
                  ],
                  ""ApiKeyTypes"": [
                    {
                      ""ApiName"": ""PrimaryKey"",
                      ""ApiKeyPaths"": [
                        {
                          ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests"",
                          ""ApiSegments"": [
                            {
                              ""ClrPropertyName"": ""Id""
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithExplicitConfiguration()
        },

        new BuildTest
        {
            Name = "Build with multiple types registered for convention processing",
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
                  ""ApiName"": ""decimal"",
                  ""ClrType"": ""System.Decimal, System.Private.CoreLib""
                },
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
                  ""ApiName"": ""orderWithPersonId"",
                  ""ApiProperties"": [
                    {
                      ""ApiName"": ""orderId"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""OrderId"",
                      ""ClrMemberKind"": ""Property""
                    },
                    {
                      ""ApiName"": ""personId"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Guid, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""PersonId"",
                      ""ClrMemberKind"": ""Property""
                    },
                    {
                      ""ApiName"": ""total"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Decimal, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Total"",
                      ""ClrMemberKind"": ""Property""
                    }
                  ],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+OrderWithPersonId, Evoogle.ApiFramework.Core.Tests""
                },
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
                  ""ApiKeyTypes"": [
                    {
                      ""ApiName"": ""PrimaryKey"",
                      ""ApiKeyPaths"": [
                        {
                          ""ClrRootType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests"",
                          ""ApiSegments"": [
                            {
                              ""ClrPropertyName"": ""Id""
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            ApiSchemaActualBuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithAddTypes()
        },

        // Convention Mutation and Ordering Tests
        new BuildTraceTest
        {
            Name = "Build with enum type convention added value receives enum value conventions",
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
                      ""ApiName"": ""OnHold"",
                      ""ClrName"": ""OnHold"",
                      ""ClrOrdinal"": 2
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            EventsExpected = [nameof(PipelineStatus.Active), nameof(PipelineStatus.OnHold)],
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithEnumTypeAddedValueTrace()
        },

        new BuildTraceTest
        {
            Name = "Build with enum value convention added value receives later enum value pass",
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
                      ""ApiName"": ""Queued"",
                      ""ClrName"": ""Queued"",
                      ""ClrOrdinal"": 3
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiObjectTypes"": [],
              ""ApiRelationships"": []
            }",
            EventsExpected = [nameof(PipelineStatus.Active), nameof(PipelineStatus.Queued)],
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithEnumValueAddedValueTrace()
        },

        new BuildTraceTest
        {
            Name = "Build with property convention added sibling receives later property pass",
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
                  ""ApiName"": ""PropertyConventionTarget"",
                  ""ApiProperties"": [
                    {
                      ""ApiName"": ""Initial"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Initial"",
                      ""ClrMemberKind"": ""Property""
                    },
                    {
                      ""ApiName"": ""Added"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Added"",
                      ""ClrMemberKind"": ""Property""
                    }
                  ],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PropertyConventionTarget, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            EventsExpected =
            [
                $"{nameof(PropertyConventionTarget)}.{nameof(PropertyConventionTarget.Initial)}",
                $"{nameof(PropertyConventionTarget)}.{nameof(PropertyConventionTarget.Added)}"
            ],
            BuildExpression = static () => ApiConventionTestsFactory.BuildWithSiblingPropertyTrace()
        },

        new BuildTraceTest
        {
            Name = "Build with property added to visited object receives later property pass",
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
                  ""ApiName"": ""PropertyConventionTarget"",
                  ""ApiProperties"": [
                    {
                      ""ApiName"": ""Initial"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Initial"",
                      ""ClrMemberKind"": ""Property""
                    },
                    {
                      ""ApiName"": ""Added"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Added"",
                      ""ClrMemberKind"": ""Property""
                    }
                  ],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PropertyConventionTarget, Evoogle.ApiFramework.Core.Tests""
                },
                {
                  ""ApiKind"": ""Object"",
                  ""ApiName"": ""PropertyConventionTrigger"",
                  ""ApiProperties"": [
                    {
                      ""ApiName"": ""Trigger"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Trigger"",
                      ""ClrMemberKind"": ""Property""
                    }
                  ],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PropertyConventionTrigger, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            EventsExpected =
            [
                $"{nameof(PropertyConventionTarget)}.{nameof(PropertyConventionTarget.Initial)}",
                $"{nameof(PropertyConventionTrigger)}.{nameof(PropertyConventionTrigger.Trigger)}",
                $"{nameof(PropertyConventionTarget)}.{nameof(PropertyConventionTarget.Added)}"
            ],
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithVisitedObjectPropertyTrace()
        },

        new BuildTraceTest
        {
            Name = "Build runs object convention before properties on convention registered object",
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
                  ""ApiName"": ""PropertyConventionRegistered"",
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
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PropertyConventionRegistered, Evoogle.ApiFramework.Core.Tests""
                },
                {
                  ""ApiKind"": ""Object"",
                  ""ApiName"": ""PropertyConventionTrigger"",
                  ""ApiProperties"": [
                    {
                      ""ApiName"": ""Trigger"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Trigger"",
                      ""ClrMemberKind"": ""Property""
                    }
                  ],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PropertyConventionTrigger, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            EventsExpected = ["ObjectType", "Property"],
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithPropertyRegisteredObjectTrace()
        },

        new BuildTraceTest
        {
            Name = "Build with property convention added enum value receives enum value " +
                "conventions",
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
                      ""ApiName"": ""Queued"",
                      ""ClrName"": ""Queued"",
                      ""ClrOrdinal"": 3
                    }
                  ],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PipelineStatus, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiObjectTypes"": [
                {
                  ""ApiKind"": ""Object"",
                  ""ApiName"": ""PropertyConventionTarget"",
                  ""ApiProperties"": [
                    {
                      ""ApiName"": ""Initial"",
                      ""ApiType"": {
                        ""ClrType"": ""System.Int32, System.Private.CoreLib""
                      },
                      ""ApiTypeModifiers"": ""Required"",
                      ""ClrName"": ""Initial"",
                      ""ClrMemberKind"": ""Property""
                    }
                  ],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PropertyConventionTarget, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            EventsExpected = [nameof(PipelineStatus.Active), nameof(PipelineStatus.Queued)],
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithPropertyAddedEnumValueTrace()
        },

        new BuildTraceTest
        {
            Name = "Build runs object conventions in phase and registration order",
            ApiSchemaExpectedJson = @"
            {
              ""ApiName"": ""Test"",
              ""ApiVersion"": ""0.1.0"",
              ""ApiOptions"": {
                ""ApiKeyNullHandling"": ""UseDefaultOnNull""
              },
              ""ApiScalarTypes"": [],
              ""ApiEnumTypes"": [],
              ""ApiObjectTypes"": [
                {
                  ""ApiKind"": ""Object"",
                  ""ApiName"": ""PersonWithId"",
                  ""ApiProperties"": [],
                  ""ApiKeyTypes"": [],
                  ""ClrType"": ""Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests+PersonWithId, Evoogle.ApiFramework.Core.Tests""
                }
              ],
              ""ApiRelationships"": []
            }",
            EventsExpected = ["Discovery1", "Discovery2", "Configuration1", "Configuration2"],
            BuildExpression = static () => ApiConventionTestsFactory.BuildWithObjectPhaseTrace()
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildThrowsTheoryData =>
    [
        new BuildThrowsTest
        {
            Name = "Build rejects convention phase that is invalid for target",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected =
                "*InvalidPropertyPhaseConvention*Discovery*IApiPropertyConvention*Configuration*",
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithInvalidPropertyConventionPhase()
        },

        new BuildThrowsTest
        {
            Name = "Build rejects structural registration from relationship convention",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected = "Relationship conventions cannot register schema " +
                "types, properties, or enum values.*",
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithRelationshipStructuralRegistration()
        },

        new BuildThrowsTest
        {
            Name = "Build rejects non-converging convention pipeline",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected = "The convention pipeline exceeded * iterations.*",
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithNonConvergingConventionPipeline()
        },

        new BuildThrowsTest
        {
            Name = "Build rejects non-converging property pipeline",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected =
                "The property convention pipeline exceeded * iterations.*",
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithNonConvergingPropertyPipeline()
        },

        new BuildThrowsTest
        {
            Name = "Build rejects non-converging enum value pipeline",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessagePatternExpected =
                "The enum-value convention pipeline exceeded * iterations.*",
            BuildExpression = static () =>
                ApiConventionTestsFactory.BuildWithNonConvergingEnumValuePipeline()
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] ConventionContractTheoryData =>
    [
        new EnumValueNamingContextTest
        {
            Name = "Enum value naming context provides target and CLR member metadata",
            SnapshotExpected = new
            (
                true,
                ApiNamingConventionTarget.EnumValue,
                typeof(PipelineStatus),
                nameof(PipelineStatus.Active),
                false,
                typeof(PipelineStatus),
                nameof(PipelineStatus.Active),
                typeof(PipelineStatus),
                true
            ),
            SnapshotExpression = static () =>
                ApiConventionTestsFactory.BuildEnumValueNamingContextSnapshot()
        },

        new EnumTargetValueTest
        {
            Name = $"{nameof(ApiNamingConventionTarget.ObjectType)} numeric value is preserved",
            Target = ApiNamingConventionTarget.ObjectType,
            ValueExpected = 0
        },

        new EnumTargetValueTest
        {
            Name = $"{nameof(ApiNamingConventionTarget.ScalarType)} numeric value is preserved",
            Target = ApiNamingConventionTarget.ScalarType,
            ValueExpected = 1
        },

        new EnumTargetValueTest
        {
            Name = $"{nameof(ApiNamingConventionTarget.EnumType)} numeric value is preserved",
            Target = ApiNamingConventionTarget.EnumType,
            ValueExpected = 2
        },

        new EnumTargetValueTest
        {
            Name = $"{nameof(ApiNamingConventionTarget.Property)} numeric value is preserved",
            Target = ApiNamingConventionTarget.Property,
            ValueExpected = 3
        },

        new EnumTargetValueTest
        {
            Name = $"{nameof(ApiNamingConventionTarget.EnumValue)} numeric value is appended",
            Target = ApiNamingConventionTarget.EnumValue,
            ValueExpected = 4
        },

        new ConventionSetTest
        {
            Name = $"{nameof(ApiConventionSetBuilder)} manages enum value conventions",
            SnapshotExpected = new(true, true, true, true),
            SnapshotExpression = static () => ApiConventionTestsFactory.BuildConventionSetSnapshot()
        },
    ];
    #endregion
}
