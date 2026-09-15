// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;
using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema.Types;

public class ApiTypeReferenceTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private class JsonDeserializeTest : JsonDeserializeTest<ApiTypeReference, ApiTypeReferenceDef>
    {
        #region JsonDeserializeTest<T, TFactoryArg> Methods
        protected override ApiTypeReference? CreateExpected(ApiTypeReferenceDef? descriptor)
        {
            return BuildApiTypeReference(descriptor);
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiTypeReference, ApiTypeReferenceDef>
    {
        #region JsonSerializeTest<T, TFactoryArg> Methods
        protected override ApiTypeReference? CreateSource(ApiTypeReferenceDef? descriptor)
        {
            return BuildApiTypeReference(descriptor);
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiTypeReference, ApiTypeReferenceDef>
    {
        #region JsonRoundtripTest<T, TFactoryArg> Methods
        protected override ApiTypeReference? CreateExpected(ApiTypeReferenceDef? descriptor)
        {
            return BuildApiTypeReference(descriptor);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        new JsonDeserializeTest
        {
            Name = "API-named type reference",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null)
        },

        new JsonDeserializeTest
        {
            Name = "API-named type reference with null ClrType",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": null
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null)
        },

        new JsonDeserializeTest
        {
            Name = "CLR-typed type reference",
            SourceJson = @"
            {
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool))
        },

        new JsonDeserializeTest
        {
            Name = "CLR-typed type reference with null ApiKind and ApiName",
            SourceJson = @"
            {
                ""ApiKind"": null,
                ""ApiName"": null,
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool))
        },

        new JsonDeserializeTest
        {
            Name = "API-named type reference with properties in reverse order",
            SourceJson = @"
            {
                ""ApiName"": ""Boolean"",
                ""ApiKind"": ""Scalar""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null)
        },

        new JsonDeserializeTest
        {
            Name = "API-named type reference with camel-case property names",
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            },
            SourceJson = @"
            {
                ""apiKind"": ""Scalar"",
                ""apiName"": ""Boolean"",
                ""clrType"": null
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null)
        },

        new JsonDeserializeTest
        {
            Name = "Invalid mixed reference is preserved for schema validation",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: typeof(bool))
        },

        new JsonDeserializeTest
        {
            Name = "All-null reference is preserved for schema validation",
            SourceJson = @"
            {
                ""ApiKind"": null,
                ""ApiName"": null,
                ""ClrType"": null
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: null)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        new JsonRoundtripTest
        {
            Name = "API-named type reference with explicit null ClrType roundtrips",
            JsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            },
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: "Customer", ClrType: null)
        },

        new JsonRoundtripTest
        {
            Name = "CLR-typed type reference with explicit null API properties roundtrips",
            JsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            },
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool))
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        new JsonSerializeTest
        {
            Name = "API-named type reference",
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean""
            }"
        },

        new JsonSerializeTest
        {
            Name = "CLR-typed type reference",
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool)),
            ExpectedJson = @"
            {
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "API-named type reference with explicit null ClrType",
            JsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            },
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": null
            }"
        },

        new JsonSerializeTest
        {
            Name = "CLR-typed type reference with explicit null API properties",
            JsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            },
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool)),
            ExpectedJson = @"
            {
                ""ApiKind"": null,
                ""ApiName"": null,
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "API-named type reference with camel-case property names",
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            },
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null),
            ExpectedJson = @"
            {
                ""apiKind"": ""Scalar"",
                ""apiName"": ""Boolean""
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
