// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;
using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema;

public class ApiTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Fields
    private static readonly List<string> _excludeMembers =
    [
        $"{nameof(ApiSchemaElement.ApiPath)}"
    ];
    #endregion

    #region Test Types
    private class JsonDeserializeTest : JsonDeserializeTest<ApiType, ApiTypeDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonDeserializeTest()
        {
            this.ExpectedFactoryExpression = (arg) => BuildTestApiType(arg);
            this.ExcludeMembers = _excludeMembers;
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiType, ApiTypeDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonRoundtripTest()
        {
            this.ExpectedFactoryExpression = (arg) => BuildTestApiType(arg);
            this.ExcludeMembers = _excludeMembers;
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiType, ApiTypeDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonSerializeTest()
        {
            this.SourceFactoryExpression = (arg) => BuildTestApiType(arg);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        // Null
        new JsonDeserializeTest
        {
            Name = "Null",
            SourceJson = "null",
            ExpectedFactoryArgument = null
        },

        // ApiScalarType
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [Boolean]",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(bool)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Boolean)
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Boolean)}] With {nameof(GraphQlExtension)}",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    }
                }
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(bool),
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Boolean)
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Int32)}]",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Int32"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(int)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Int32)
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Single)}]",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Single"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(float)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Single)
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(String)}]",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(string)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(String)
                )
            )
        },

        // ApiEnumType
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}]",
            SourceJson = @"
            {
                ""ApiKind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Enum,
                    ClrType: typeof(StopLight)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(StopLight)
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}] With {nameof(JsonApiExtension)}",
            SourceJson = @"
            {
                ""ApiKind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Enum,
                    ClrType: typeof(StopLight),
                    ExtensionTypes: [ typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(StopLight)
                )
            )
        },

        // ApiCollectionType
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}>]",
            SourceJson = @"
            {
                ""ApiKind"": ""Collection"",
                ""ApiItemType"": {
                    ""ApiKind"": ""Scalar"",
                    ""ApiName"": ""String""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<string>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                    ApiItemTypeModifiers: ApiTypeModifiers.Required
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}?>]",
            SourceJson = @"
            {
                ""ApiKind"": ""Collection"",
                ""ApiItemType"": {
                    ""ApiKind"": ""Scalar"",
                    ""ApiName"": ""String""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<string?>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                    ApiItemTypeModifiers: ApiTypeModifiers.None
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}>]",
            SourceJson = @"
            {
                ""ApiKind"": ""Collection"",
                ""ApiItemType"": {
                    ""ApiKind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<StopLight>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                    ApiItemTypeModifiers: ApiTypeModifiers.Required
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}?>]",
            SourceJson = @"
            {
                ""ApiKind"": ""Collection"",
                ""ApiItemType"": {
                    ""ApiKind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.Nullable\u00601[[Evoogle.ApiFramework.Schema.TestData.StopLight,Evoogle.ApiFramework.Core.Tests]],System.Private.CoreLib]],System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<StopLight?>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                    ApiItemTypeModifiers: ApiTypeModifiers.None
                )
            )
        },

        // ApiObjectType With API Named Typed Expressions
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions",
            SourceJson = @"
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
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And API Options",
            SourceJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
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
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiOptions: new ApiObjectTypeOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    ),
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceJson = @"
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
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly),
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With API Named Typed Expressions",
            SourceJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""Company"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Name"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""Name"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""Owner"",
                        ""ApiType"": {
                            ""ApiKind"": ""Object"",
                            ""ApiName"": ""Person""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Owner"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""Employees"",
                        ""ApiType"": {
                            ""ApiInlineType"": {
                                ""ApiKind"": ""Collection"",
                                ""ApiItemType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""Person""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                            }
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Employees"",
                        ""ClrMemberKind"": ""Property""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiName"": ""OwnedBy"",
                        ""ApiPropertyName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
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
                            ClrName: nameof(Company.Name),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Owner),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(Company.Owner),
                            ClrMemberKind: ClrMemberKind.Property
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
                            ClrName: nameof(Company.Employees),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                    ],
                    ApiRelationships:
                    [
                        new ApiRelationshipConfig(ApiName: "OwnedBy", ApiPropertyName: nameof(Company.Owner)),
                        new ApiRelationshipConfig(ApiName: nameof(Company.Employees))
                    ]
                )
            )
        },

        // ApiObjectType With CLR Typed Expressions
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions",
            SourceJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And API Options",
            SourceJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiOptions: new ApiObjectTypeOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    ),
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.Schema.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly),
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With CLR Typed Expressions",
            SourceJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""Company"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Name"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""Name"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""Owner"",
                        ""ApiType"": {
                            ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Owner"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""Employees"",
                        ""ApiType"": {
                            ""ApiInlineType"": {
                                ""ApiKind"": ""Collection"",
                                ""ApiItemType"": {
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                            }
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Employees"",
                        ""ClrMemberKind"": ""Property""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiName"": ""OwnedBy"",
                        ""ApiPropertyName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
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
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(Company.Name),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Owner),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(Company.Owner),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Employees),
                            ApiTypeExpression: new ApiTypeExpression
                            (
                                apiInlineType: new ApiCollectionType
                                (
                                    apiItemTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                                    apiItemTypeModifiers: ApiTypeModifiers.Required,
                                    clrCollectionType: typeof(List<Person>)
                                )
                            ),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(Company.Employees),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                    ],
                    ApiRelationships:
                    [
                        new ApiRelationshipConfig(ApiName: "OwnedBy", ApiPropertyName: nameof(Company.Owner)),
                        new ApiRelationshipConfig(ApiName: nameof(Company.Employees))
                    ]
                )
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

        // ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [Boolean]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(bool)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Boolean)
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Boolean)}] With {nameof(GraphQlExtension)}",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(bool),
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Boolean)
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Int32)}]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(int)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Int32)
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Single)}]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(float)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Single)
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(String)}]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(string)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(String)
                )
            )
        },

        // ApiEnumType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Enum,
                    ClrType: typeof(StopLight)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(StopLight)
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}] With {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Enum,
                    ClrType: typeof(StopLight),
                    ExtensionTypes: [ typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(StopLight)
                )
            )
        },

        // ApiCollectionType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}>]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<string>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                    ApiItemTypeModifiers: ApiTypeModifiers.Required
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}?>]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<string?>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                    ApiItemTypeModifiers: ApiTypeModifiers.None
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}>]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<StopLight>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                    ApiItemTypeModifiers: ApiTypeModifiers.Required
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}?>]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<StopLight?>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                    ApiItemTypeModifiers: ApiTypeModifiers.None
                )
            )
        },

        // ApiObjectType With API Named Typed Expressions
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And API Options",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiOptions: new ApiObjectTypeOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    ),
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly),
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With API Named Typed Expressions",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
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
                            ClrName: nameof(Company.Name),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Owner),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(Company.Owner),
                            ClrMemberKind: ClrMemberKind.Property
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
                            ClrName: nameof(Company.Employees),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                    ],
                    ApiRelationships:
                    [
                        new ApiRelationshipConfig(ApiName: "OwnedBy", ApiPropertyName: nameof(Company.Owner)),
                        new ApiRelationshipConfig(ApiName: nameof(Company.Employees))
                    ]
                )
            )
        },

        // ApiObjectType With CLR Typed Expressions
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And API Options",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiOptions: new ApiObjectTypeOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    ),
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly),
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With CLR Typed Expressions",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
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
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(Company.Name),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Owner),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(Company.Owner),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Employees),
                            ApiTypeExpression: new ApiTypeExpression
                            (
                                apiInlineType: new ApiCollectionType
                                (
                                    apiItemTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                                    apiItemTypeModifiers: ApiTypeModifiers.Required,
                                    clrCollectionType: typeof(List<Person>)
                                )
                            ),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(Company.Employees),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                    ],
                    ApiRelationships:
                    [
                        new ApiRelationshipConfig(ApiName: "OwnedBy", ApiPropertyName: nameof(Company.Owner)),
                        new ApiRelationshipConfig(ApiName: nameof(Company.Employees), ApiPropertyName: nameof(Company.Employees))
                    ]
                )
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

        // ApiScalarType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Boolean)}]",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(bool)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Boolean)
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Boolean)}] With {nameof(GraphQlExtension)}",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(bool),
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Boolean)
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    }
                }
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Int32)}]",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(int)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Int32)
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Int32"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Single)}]",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(float)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Single)
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Single"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(String)}]",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(string)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(String)
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },

        // ApiEnumType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}]",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Enum,
                    ClrType: typeof(StopLight)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(StopLight)
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}] With {nameof(JsonApiExtension)}",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Enum,
                    ClrType: typeof(StopLight),
                    ExtensionTypes: [ typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(StopLight)
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Website"": ""http://jsonapi.org""
                    }
                }
            }"
        },

        // ApiCollectionType
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}>]",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<string>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                    ApiItemTypeModifiers: ApiTypeModifiers.Required
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Collection"",
                ""ApiItemType"": {
                    ""ApiKind"": ""Scalar"",
                    ""ApiName"": ""String""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}?>]",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<string?>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: "String"),
                    ApiItemTypeModifiers: ApiTypeModifiers.None
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Collection"",
                ""ApiItemType"": {
                    ""ApiKind"": ""Scalar"",
                    ""ApiName"": ""String""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}>]",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<StopLight>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                    ApiItemTypeModifiers: ApiTypeModifiers.Required
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Collection"",
                ""ApiItemType"": {
                    ""ApiKind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.StopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}?>]",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<StopLight?>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                    ApiItemTypeModifiers: ApiTypeModifiers.None
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Collection"",
                ""ApiItemType"": {
                    ""ApiKind"": ""Enum"",
                    ""ApiName"": ""StopLight""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.Nullable\u00601[[Evoogle.ApiFramework.Schema.TestData.StopLight,Evoogle.ApiFramework.Core.Tests]],System.Private.CoreLib]],System.Private.CoreLib""
            }"
        },

        // ApiObjectType With API Named Typed Expressions
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            ),
            ExpectedJson = @"
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
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And API Options",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiOptions: new ApiObjectTypeOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    ),
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
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
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly),
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            ),
            ExpectedJson = @"
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
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Int64""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Boolean""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
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

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With API Named Typed Expressions",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
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
                            ClrName: nameof(Company.Name),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Owner),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(Company.Owner),
                            ClrMemberKind: ClrMemberKind.Property
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
                            ClrName: nameof(Company.Employees),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                    ],
                    ApiRelationships:
                    [
                        new ApiRelationshipConfig(ApiName: "OwnedBy", ApiPropertyName: nameof(Company.Owner)),
                        new ApiRelationshipConfig(ApiName: nameof(Company.Employees))
                    ]
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""Company"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Name"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""String""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""Name"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""Owner"",
                        ""ApiType"": {
                            ""ApiKind"": ""Object"",
                            ""ApiName"": ""Person""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Owner"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""Employees"",
                        ""ApiType"": {
                            ""ApiInlineType"": {
                                ""ApiKind"": ""Collection"",
                                ""ApiItemType"": {
                                    ""ApiKind"": ""Object"",
                                    ""ApiName"": ""Person""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                            }
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Employees"",
                        ""ClrMemberKind"": ""Property""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiName"": ""OwnedBy"",
                        ""ApiPropertyName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        // ApiObjectType With CLR Typed Expressions
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And API Options",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiOptions: new ApiObjectTypeOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    ),
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiOptions"": {
                    ""ApiIdentityNullHandling"": ""ThrowException""
                },
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly),
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""ScalarsOnly"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Int64,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber"",
                        ""ClrMemberKind"": ""Field""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Boolean,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate"",
                        ""ClrMemberKind"": ""Field""
                    }
                ],
                ""ApiRelationships"": [],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
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

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With CLR Typed Expressions",
            SourceFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
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
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(Company.Name),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Owner),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(Company.Owner),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Employees),
                            ApiTypeExpression: new ApiTypeExpression
                            (
                                apiInlineType: new ApiCollectionType
                                (
                                    apiItemTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
                                    apiItemTypeModifiers: ApiTypeModifiers.Required,
                                    clrCollectionType: typeof(List<Person>)
                                )
                            ),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(Company.Employees),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                    ],
                    ApiRelationships:
                    [
                        new ApiRelationshipConfig(ApiName: "OwnedBy", ApiPropertyName: nameof(Company.Owner)),
                        new ApiRelationshipConfig(ApiName: nameof(Company.Employees))
                    ]
                )
            ),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Object"",
                ""ApiName"": ""Company"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Name"",
                        ""ApiType"": {
                            ""ClrType"": ""System.String,System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""Name"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""Owner"",
                        ""ApiType"": {
                            ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Owner"",
                        ""ClrMemberKind"": ""Property""
                    },
                    {
                        ""ApiName"": ""Employees"",
                        ""ApiType"": {
                            ""ApiInlineType"": {
                                ""ApiKind"": ""Collection"",
                                ""ApiItemType"": {
                                    ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Person,Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
                            }
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""Employees"",
                        ""ClrMemberKind"": ""Property""
                    }
                ],
                ""ApiRelationships"": [
                    {
                        ""ApiName"": ""OwnedBy"",
                        ""ApiPropertyName"": ""Owner""
                    },
                    {
                        ""ApiName"": ""Employees""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.TestData.Company, Evoogle.ApiFramework.Core.Tests""
            }"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeTheoryData))]
    public void JsonDeserialize(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtrip(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonSerializeTheoryData))]
    public void JsonSerialize(IXUnitTest test) => test.Execute(this);
    #endregion
}
