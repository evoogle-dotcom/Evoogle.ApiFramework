// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

public partial class ApiConventionTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class BuildTest : XUnitTest
    {
        #region Fields
        protected static readonly JsonSerializerOptions _defaultToJsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
        };
        #endregion

        #region User Supplied Properties
        public required string ApiSchemaExpectedJson { get; init; }

        [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchema>))]
        public required Expression<Func<ApiSchema>> ApiSchemaActualBuildExpression { get; init; }
        #endregion

        #region Calculated Properties
        protected ApiSchema? ApiSchemaExpected { get; set; }
        protected ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region Constructors
        public BuildTest()
        {
            this.Name = nameof(BuildTest);
            this.ExcludeMembers = ApiSchemaExcludeMembers.SchemaInitialized;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiSchemaExpected = JsonSerializer.Deserialize<ApiSchema>(this.ApiSchemaExpectedJson);

            this.WriteLine("ApiSchemaExpected:");
            this.WriteLine($"{this.ApiSchemaExpected.SafeToJson(_defaultToJsonOptions)}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var apiSchemaActualBuildLambda = this.ApiSchemaActualBuildExpression.Compile();
            this.ApiSchemaActual = apiSchemaActualBuildLambda();

            this.WriteLine("ApiSchemaActual:");
            this.WriteLine($"{this.ApiSchemaActual.SafeToJson(_defaultToJsonOptions)}");
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.AssertBeEquivalentTo(this.ApiSchemaActual, this.ApiSchemaExpected);
        }
        #endregion
    }
    #endregion

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
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}
