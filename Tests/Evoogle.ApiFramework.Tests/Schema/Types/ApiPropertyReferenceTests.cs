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

public class ApiPropertyReferenceTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private class EqualityTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiPropertyReference Lhs { get; init; }
        public required ApiPropertyReference Rhs { get; init; }
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

    private class JsonDeserializeTest : JsonDeserializeTest<ApiPropertyReference, ApiPropertyReferenceDef>
    {
        #region JsonDeserializeTest<T, TFactoryArg> Methods
        protected override ApiPropertyReference? CreateExpected(ApiPropertyReferenceDef? descriptor) => BuildApiPropertyReference(descriptor);
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiPropertyReference, ApiPropertyReferenceDef>
    {
        #region JsonRoundtripTest<T, TFactoryArg> Methods
        protected override ApiPropertyReference? CreateExpected(ApiPropertyReferenceDef? descriptor) => BuildApiPropertyReference(descriptor);
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiPropertyReference, ApiPropertyReferenceDef>
    {
        #region JsonSerializeTest<T, TFactoryArg> Methods
        protected override ApiPropertyReference? CreateSource(ApiPropertyReferenceDef? descriptor) => BuildApiPropertyReference(descriptor);
        #endregion
    }
    #endregion

    #region Test Fields
    private static readonly ApiPropertyReference _apiAcmeReference = ApiPropertyReference.ApiRef("Acme");
    private static readonly ApiPropertyReference _apiValueReference = ApiPropertyReference.ApiRef("Value");
    private static readonly ApiPropertyReference _apiVALUEReference = ApiPropertyReference.ApiRef("VALUE");
    private static readonly ApiPropertyReference _clrValueReference = ApiPropertyReference.ClrRef("Value");
    private static readonly ApiPropertyReference _clrVALUEReference = ApiPropertyReference.ClrRef("VALUE");

    private static readonly ApiPropertyReference _nullNameReferenceLhs = new(null, null);
    private static readonly ApiPropertyReference _nullNameReferenceRhs = new(null, null);
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
            Name = $"{_apiValueReference} == {_apiValueReference}",
            Lhs = _apiValueReference,
            Rhs = _apiValueReference,
            ExpectedEquality = true,
        },
        new EqualityTest
        {
            Name = $"{_clrValueReference} == {_clrValueReference}",
            Lhs = _clrValueReference,
            Rhs = _clrValueReference,
            ExpectedEquality = true,
        },
        new EqualityTest
        {
            Name = $"{_apiValueReference} != {_clrValueReference}",
            Lhs = _apiValueReference,
            Rhs = _clrValueReference,
            ExpectedEquality = false,
        },
        new EqualityTest
        {
            Name = $"{_apiAcmeReference} != {_apiValueReference}",
            Lhs = _apiAcmeReference,
            Rhs = _apiValueReference,
            ExpectedEquality = false,
        },
        new EqualityTest
        {
            Name = $"{_apiValueReference} != {_apiVALUEReference}",
            Lhs = _apiValueReference,
            Rhs = _apiVALUEReference,
            ExpectedEquality = false,
        },
        new EqualityTest
        {
            Name = $"{_clrValueReference} != {_clrVALUEReference}",
            Lhs = _clrValueReference,
            Rhs = _clrVALUEReference,
            ExpectedEquality = false,
        },
        new EqualityTest
        {
            Name = $"{_nullNameReferenceLhs} == {_nullNameReferenceRhs}",
            Lhs = _nullNameReferenceLhs,
            Rhs = _nullNameReferenceRhs,
            ExpectedEquality = true,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ApiName)}",
            SourceJson = @"{ ""ApiName"": ""Value"" }",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef("Value",  null)
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ApiName)} and null {nameof(ApiPropertyReference.ClrName)}",
            SourceJson = @"{ ""ApiName"": ""Value"", ""ClrName"": null }",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef("Value",  null)
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ClrName)}",
            SourceJson = @"{ ""ClrName"": ""Value"" }",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef(null, "Value")
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ClrName)} and null {nameof(ApiPropertyReference.ApiName)}",
            SourceJson = @"{ ""ApiName"": null, ""ClrName"": ""Value"" }",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef(null, "Value")
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ApiName)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceJson = @"{ ""apiName"": ""Value"" }",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef("Value",  null)
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ClrName)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceJson = @"{ ""clrName"": ""Value"" }",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef(null, "Value")
        },

        // Invalid JSON
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ApiName)} and {nameof(ApiPropertyReference.ClrName)} having null values are preserved for schema validation",
            SourceJson = @"{}",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef(null, null)
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ApiName)} and {nameof(ApiPropertyReference.ClrName)} having non-null values are preserved for schema validation",
            SourceJson = @"{ ""ApiName"": ""ApiValue"", ""ClrName"": ""ClrValue"" }",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef("ApiValue", "ClrValue")
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ApiName)}",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef("Value",  null)
        },
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ClrName)}",
            ExpectedFactoryArgument = new ApiPropertyReferenceDef(null, "Value")
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ApiName)}",
            SourceFactoryArgument = new ApiPropertyReferenceDef("Value",  null),
            ExpectedJson = @"{ ""ApiName"": ""Value"" }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ClrName)}",
            SourceFactoryArgument = new ApiPropertyReferenceDef(null, "Value"),
            ExpectedJson = @"{ ""ClrName"": ""Value"" }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ApiName)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceFactoryArgument = new ApiPropertyReferenceDef("Value",  null),
            ExpectedJson = @"{ ""apiName"": ""Value"" }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ClrName)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceFactoryArgument = new ApiPropertyReferenceDef(null, "Value"),
            ExpectedJson = @"{ ""clrName"": ""Value"" }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ApiName)} and explicit null {nameof(ApiPropertyReference.ClrName)}",
            JsonSerializerOptions = WriteNullPropertyOptions,
            SourceFactoryArgument = new ApiPropertyReferenceDef("Value",  null),
            ExpectedJson = @"{ ""ApiName"": ""Value"", ""ClrName"": null }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiPropertyReference)} with {nameof(ApiPropertyReference.ClrName)} and explicit null {nameof(ApiPropertyReference.ApiName)}",
            JsonSerializerOptions = WriteNullPropertyOptions,
            SourceFactoryArgument = new ApiPropertyReferenceDef(null, "Value"),
            ExpectedJson = @"{ ""ApiName"": null, ""ClrName"": ""Value"" }"
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
