// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;
using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiTypeTests
{
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
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.StopLight, Evoogle.ApiFramework.Core.Tests""
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.StopLight, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
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
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.StopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
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
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.Nullable\u00601[[Evoogle.ApiFramework.TestData.StopLight,Evoogle.ApiFramework.Core.Tests]],System.Private.CoreLib]],System.Private.CoreLib""
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
                ""ApiIdentities"": [],
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
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
                ""ApiIdentities"": [],
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
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
                ""ApiIdentities"": [],
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
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
                ""ApiIdentities"": [
                    {
                        ""ApiName"": ""PK_Company_Id"",
                        ""ApiIdentityParts"": [
                            {
                                ""ApiPropertyName"": ""Id""
                            }
                        ]
                    },
                    {
                        ""ApiName"": ""AK_Company_Name"",
                        ""ApiIdentityParts"": [
                            {
                                ""ApiPropertyName"": ""Name""
                            }
                        ]
                    }
                ],
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Id"",
                        ""ApiType"": {
                            ""ApiKind"": ""Scalar"",
                            ""ApiName"": ""Ulid""
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
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.Company, Evoogle.ApiFramework.Core.Tests""
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
                    ApiIdentities:
                    [
                        new ApiIdentityConfig
                        (
                            ApiName: "PK_Company_Id",
                            ApiIdentityParts:
                            [
                                new ApiIdentityPartConfig
                                (
                                    ApiPropertyName: nameof(Company.Id)
                                )
                            ]
                        ),
                        new ApiIdentityConfig
                        (
                            ApiName: "AK_Company_Name",
                            ApiIdentityParts:
                            [
                                new ApiIdentityPartConfig
                                (
                                    ApiPropertyName: nameof(Company.Name)
                                )
                            ]
                        )
                    ],
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Id),
                            ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(Company.Id),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
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
                ""ApiIdentities"": [],
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
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
                ""ApiIdentities"": [],
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests""
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
                ""ApiIdentities"": [],
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.ScalarsOnly, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, Evoogle.ApiFramework.Core.Tests"": {
                        ""Count"": 42
                    },
                    ""Evoogle.ApiFramework.TestData.JsonApiExtension, Evoogle.ApiFramework.Core.Tests"": {
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
                ""ApiIdentities"": [
                    {
                        ""ApiName"": ""PK_Company_Id"",
                        ""ApiIdentityParts"": [
                            {
                                ""ApiPropertyName"": ""Id"",
					            ""ClrConfiguredIdType"": ""System.String,System.Private.CoreLib""
                            }
                        ]
                    },
                    {
                        ""ApiName"": ""AK_Company_Name"",
                        ""ApiIdentityParts"": [
                            {
                                ""ApiPropertyName"": ""Name""
                            }
                        ]
                    }
                ],
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""Id"",
                        ""ApiType"": {
                            ""ClrType"": ""System.Ulid,Ulid""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""Id"",
                        ""ClrMemberKind"": ""Property""
                    },
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
                            ""ClrType"": ""Evoogle.ApiFramework.TestData.Person,Evoogle.ApiFramework.Core.Tests""
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
                                    ""ClrType"": ""Evoogle.ApiFramework.TestData.Person,Evoogle.ApiFramework.Core.Tests""
                                },
                                ""ApiItemTypeModifiers"": ""Required"",
                                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.TestData.Person, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
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
                ""ClrType"": ""Evoogle.ApiFramework.TestData.Company, Evoogle.ApiFramework.Core.Tests""
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
                    ApiIdentities:
                    [
                        new ApiIdentityConfig
                        (
                            ApiName: "PK_Company_Id",
                            ApiIdentityParts:
                            [
                                new ApiIdentityPartConfig
                                (
                                    ApiPropertyName: nameof(Company.Id),
                                    ClrConfiguredIdType: typeof(string)
                                )
                            ]
                        ),
                        new ApiIdentityConfig
                        (
                            ApiName: "AK_Company_Name",
                            ApiIdentityParts:
                            [
                                new ApiIdentityPartConfig
                                (
                                    ApiPropertyName: nameof(Company.Name)
                                )
                            ]
                        )
                    ],
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Id),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(Ulid)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(Company.Id),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
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

    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeTheoryData))]
    public void JsonDeserialize(IXUnitTest test) => test.Execute(this);
    #endregion
}
