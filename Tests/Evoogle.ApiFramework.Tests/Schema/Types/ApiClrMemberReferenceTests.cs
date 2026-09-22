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

public class ApiClrMemberReferenceTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private class EqualityTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiClrMemberReference Lhs { get; init; }
        public required ApiClrMemberReference Rhs { get; init; }
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

    private class JsonDeserializeTest : JsonDeserializeTest<ApiClrMemberReference, ApiClrMemberReferenceDef>
    {
        #region JsonDeserializeTest<T, TFactoryArg> Methods
        protected override ApiClrMemberReference? CreateExpected(ApiClrMemberReferenceDef? descriptor) => BuildApiClrMemberReference(descriptor);
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiClrMemberReference, ApiClrMemberReferenceDef>
    {
        #region JsonRoundtripTest<T, TFactoryArg> Methods
        protected override ApiClrMemberReference? CreateExpected(ApiClrMemberReferenceDef? descriptor) => BuildApiClrMemberReference(descriptor);
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiClrMemberReference, ApiClrMemberReferenceDef>
    {
        #region JsonSerializeTest<T, TFactoryArg> Methods
        protected override ApiClrMemberReference? CreateSource(ApiClrMemberReferenceDef? descriptor) => BuildApiClrMemberReference(descriptor);
        #endregion
    }
    #endregion

    #region Test Fields
    private static readonly ApiClrMemberReference _apiAcmeFieldReference = new("Acme", ClrMemberKind.Field);
    private static readonly ApiClrMemberReference _apiValueFieldReference = new("Value", ClrMemberKind.Field);
    private static readonly ApiClrMemberReference _apiValuePropertyReference = new("Value", ClrMemberKind.Property);
    private static readonly ApiClrMemberReference _apiVALUEPropertyReference = new("VALUE", ClrMemberKind.Property);
    #endregion

    #region Test Properties
    private static JsonSerializerOptions CamelCaseOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] EqualityTheoryData =>
    [
        new EqualityTest
        {
            Name = $"{_apiValueFieldReference} == {_apiValueFieldReference}",
            Lhs = _apiValueFieldReference,
            Rhs = _apiValueFieldReference,
            ExpectedEquality = true,
        },

        new EqualityTest
        {
            Name = $"{_apiValueFieldReference} != {_apiValuePropertyReference}",
            Lhs = _apiValueFieldReference,
            Rhs = _apiValuePropertyReference,
            ExpectedEquality = false,
        },

        new EqualityTest
        {
            Name = $"{_apiValuePropertyReference} == {_apiValuePropertyReference}",
            Lhs = _apiValuePropertyReference,
            Rhs = _apiValuePropertyReference,
            ExpectedEquality = true,
        },

        new EqualityTest
        {
            Name = $"{_apiValuePropertyReference} != {_apiValueFieldReference}",
            Lhs = _apiValuePropertyReference,
            Rhs = _apiValueFieldReference,
            ExpectedEquality = false,
        },

        new EqualityTest
        {
            Name = $"{_apiAcmeFieldReference} != {_apiValueFieldReference}",
            Lhs = _apiAcmeFieldReference,
            Rhs = _apiValueFieldReference,
            ExpectedEquality = false,
        },

        new EqualityTest
        {
            Name = $"{_apiValuePropertyReference} != {_apiVALUEPropertyReference}",
            Lhs = _apiValuePropertyReference,
            Rhs = _apiVALUEPropertyReference,
            ExpectedEquality = false,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Field)}",
            SourceJson = @"
            {
                ""ClrMemberName"": ""Value"",
                ""ClrMemberKind"": ""Field""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Field)
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Field)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceJson = @"
            {
                ""clrMemberName"": ""Value"",
                ""clrMemberKind"": ""Field""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Field)
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Property)}",
            SourceJson = @"
            {
                ""ClrMemberName"": ""Value"",
                ""ClrMemberKind"": ""Property""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Property)
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Property)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceJson = @"
            {
                ""clrMemberName"": ""Value"",
                ""clrMemberKind"": ""Property""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Property)
        },

        // Invalid JSON
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with invalid {nameof(ApiClrMemberReference.ClrMemberKind)} is preserved for schema validation",
            SourceJson = @"
            {
                ""ClrMemberName"": ""Value"",
                ""ClrMemberKind"": ""42""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: null, hasInvalidClrMemberKind: true)
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with missing {nameof(ApiClrMemberReference.ClrMemberName)} is preserved for schema validation",
            SourceJson = @"
            {
                ""ClrMemberKind"": ""Property""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: null, clrMemberKind: ClrMemberKind.Property, hasInvalidClrMemberKind: false)
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with missing {nameof(ApiClrMemberReference.ClrMemberKind)} is preserved for schema validation",
            SourceJson = @"
            {
                ""ClrMemberName"": ""Value""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: null, hasInvalidClrMemberKind: true)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Field)}",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Field)
        },
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Property)}",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Property)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Field)}",
            SourceFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Field),
            ExpectedJson = @"
            {
                ""ClrMemberName"": ""Value"",
                ""ClrMemberKind"": ""Field""
            }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Field)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Field),
            ExpectedJson = @"
            {
                ""clrMemberName"": ""Value"",
                ""clrMemberKind"": ""Field""
            }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Property)}",
            SourceFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Property),
            ExpectedJson = @"
            {
                ""ClrMemberName"": ""Value"",
                ""ClrMemberKind"": ""Property""
            }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrMemberKind)}.{nameof(ClrMemberKind.Property)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceFactoryArgument = new ApiClrMemberReferenceDef(clrMemberName: "Value", clrMemberKind: ClrMemberKind.Property),
            ExpectedJson = @"
            {
                ""clrMemberName"": ""Value"",
                ""clrMemberKind"": ""Property""
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
