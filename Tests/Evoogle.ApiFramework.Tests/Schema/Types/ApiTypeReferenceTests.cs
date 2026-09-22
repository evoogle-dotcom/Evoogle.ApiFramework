// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;
using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema.Types;

public class ApiTypeReferenceTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private class EqualityTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiTypeReference Lhs { get; init; }
        public required ApiTypeReference Rhs { get; init; }
        public required bool ExpectedEquality { get; init; }
        #endregion

        #region Calculated Properties
        private bool ActualEquality { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"LHS: {this.Lhs.SafeToString()}");
            this.WriteLine($"RHS: {this.Rhs.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected Equality: {this.ExpectedEquality}");
        }

        protected override void Act()
        {
            this.ActualEquality = this.Lhs == this.Rhs;
            this.WriteLine($"Actual Equality:   {this.ActualEquality}");
        }

        protected override void Assert()
        {
            this.ActualEquality.Should().Be(this.ExpectedEquality);
            if (this.ExpectedEquality)
            {
                this.Lhs.GetHashCode().Should().Be(this.Rhs.GetHashCode());
            }
        }
        #endregion
    }

    private class JsonDeserializeTest : JsonDeserializeTest<ApiTypeReference, ApiTypeReferenceDef>
    {
        #region JsonDeserializeTest<T, TFactoryArg> Methods
        protected override ApiTypeReference? CreateExpected(ApiTypeReferenceDef? descriptor) => BuildApiTypeReference(descriptor);
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiTypeReference, ApiTypeReferenceDef>
    {
        #region JsonSerializeTest<T, TFactoryArg> Methods
        protected override ApiTypeReference? CreateSource(ApiTypeReferenceDef? descriptor) => BuildApiTypeReference(descriptor);
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiTypeReference, ApiTypeReferenceDef>
    {
        #region JsonRoundtripTest<T, TFactoryArg> Methods
        protected override ApiTypeReference? CreateExpected(ApiTypeReferenceDef? descriptor) => BuildApiTypeReference(descriptor);
        #endregion
    }
    #endregion

    #region Test Fields
    private static readonly ApiTypeReference _apiBoolTypeReference = new(apiKind: ApiTypeKind.Scalar, apiName: "Bool");
    private static readonly ApiTypeReference _apiBOOLTypeReference = new(apiKind: ApiTypeKind.Scalar, apiName: "BOOL");
    private static readonly ApiTypeReference _apiBooleanTypeReference = new(apiKind: ApiTypeKind.Scalar, apiName: "Boolean");
    private static readonly ApiTypeReference _clrBoolTypeReference = new(clrType: typeof(bool));

    private static readonly ApiTypeReference _apiIntTypeReference = new(apiKind: ApiTypeKind.Scalar, apiName: "Int");
    private static readonly ApiTypeReference _clrIntTypeReference = new(clrType: typeof(int));

    private static readonly ApiTypeReference _nullTypeReference = new(null, null, null, true);
    #endregion

    #region Test Properties
    private static JsonSerializerOptions CamelCaseOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static JsonSerializerOptions WriteNullPropertyOptions => new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] EqualityTheoryData =>
    [
        new EqualityTest
        {
            Name = $"{_apiBoolTypeReference} == {_apiBoolTypeReference}",
            Lhs = _apiBoolTypeReference,
            Rhs = _apiBoolTypeReference,
            ExpectedEquality = true,
        },
        new EqualityTest
        {
            Name = $"{_clrBoolTypeReference} == {_clrBoolTypeReference}",
            Lhs = _clrBoolTypeReference,
            Rhs = _clrBoolTypeReference,
            ExpectedEquality = true,
        },

        new EqualityTest
        {
            Name = $"{_apiBoolTypeReference} != {_apiBOOLTypeReference}",
            Lhs = _apiBoolTypeReference,
            Rhs = _apiBOOLTypeReference,
            ExpectedEquality = false,
        },
        new EqualityTest
        {
            Name = $"{_apiBoolTypeReference} != {_apiBooleanTypeReference}",
            Lhs = _apiBoolTypeReference,
            Rhs = _apiBooleanTypeReference,
            ExpectedEquality = false,
        },

        new EqualityTest
        {
            Name = $"{_apiBoolTypeReference} != {_clrBoolTypeReference}",
            Lhs = _apiBoolTypeReference,
            Rhs = _clrBoolTypeReference,
            ExpectedEquality = false,
        },
        new EqualityTest
        {
            Name = $"{_clrBoolTypeReference} != {_apiBoolTypeReference}",
            Lhs = _clrBoolTypeReference,
            Rhs = _apiBoolTypeReference,
            ExpectedEquality = false,
        },

        new EqualityTest
        {
            Name = $"{_apiBoolTypeReference} != {_apiIntTypeReference}",
            Lhs = _apiBoolTypeReference,
            Rhs = _apiIntTypeReference,
            ExpectedEquality = false,
        },
        new EqualityTest
        {
            Name = $"{_clrBoolTypeReference} != {_clrIntTypeReference}",
            Lhs = _clrBoolTypeReference,
            Rhs = _clrIntTypeReference,
            ExpectedEquality = false,
        },

        new EqualityTest
        {
            Name = $"{_nullTypeReference} == {_nullTypeReference}",
            Lhs = _nullTypeReference,
            Rhs = _nullTypeReference,
            ExpectedEquality = true,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)}",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null)
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ClrType)}",
            SourceJson = @"
            {
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool))
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)} and explicit null {nameof(ApiTypeReference.ClrType)}",
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
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ClrType)} and explicit null {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)}",
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
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)} with camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceJson = @"
            {
                ""apiKind"": ""Scalar"",
                ""apiName"": ""Boolean""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null),
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ClrType)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceJson = @"
            {
                ""clrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool)),
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)} in reverse order",
            SourceJson = @"
            {
                ""ApiName"": ""Boolean"",
                ""ApiKind"": ""Scalar""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null)
        },

        // Invalid JSON
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)} and {nameof(ApiTypeReference.ClrType)} having null values are preserved for schema validation",
            SourceJson = @"{}",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: null)
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)} and {nameof(ApiTypeReference.ClrType)} having non-null values are preserved for schema validation",
            SourceJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: typeof(bool))
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)}",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Object, ApiName: "Customer", ClrType: null)
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ClrType)}",
            ExpectedFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool))
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)}",
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null),
            ExpectedJson = @"
            {
                ""ApiKind"": ""Scalar"",
                ""ApiName"": ""Boolean""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ClrType)}",
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool)),
            ExpectedJson = @"
            {
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)} with camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: ApiTypeKind.Scalar, ApiName: "Boolean", ClrType: null),
            ExpectedJson = @"
            {
                ""apiKind"": ""Scalar"",
                ""apiName"": ""Boolean""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ClrType)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool)),
            ExpectedJson = @"
            {
                ""clrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)} and explicit null {nameof(ApiTypeReference.ClrType)}",
            JsonSerializerOptions = WriteNullPropertyOptions,
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
            Name = $"{nameof(ApiTypeReference)} with {nameof(ApiTypeReference.ClrType)} and explicit null {nameof(ApiTypeReference.ApiKind)} and {nameof(ApiTypeReference.ApiName)}",
            JsonSerializerOptions = WriteNullPropertyOptions,
            SourceFactoryArgument = new ApiTypeReferenceDef(ApiKind: null, ApiName: null, ClrType: typeof(bool)),
            ExpectedJson = @"
            {
                ""ApiKind"": null,
                ""ApiName"": null,
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(EqualityTheoryData))]
    public void Equality(IXUnitTest test) => test.Execute(this);

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
