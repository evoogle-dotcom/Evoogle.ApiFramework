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
    private static readonly ApiClrMemberReference _apiAcmeFieldReference = new(ClrMemberKind.Field, "Acme");
    private static readonly ApiClrMemberReference _apiValueFieldReference = new(ClrMemberKind.Field, "Value");
    private static readonly ApiClrMemberReference _apiValuePropertyReference = new(ClrMemberKind.Property, "Value");
    private static readonly ApiClrMemberReference _apiVALUEPropertyReference = new(ClrMemberKind.Property, "VALUE");
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
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Field)}",
            SourceJson = @"
            {
                ""ClrName"": ""Value"",
                ""ClrKind"": ""Field""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Field, clrName: "Value")
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Field)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceJson = @"
            {
                ""clrName"": ""Value"",
                ""clrKind"": ""Field""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Field, clrName: "Value")
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Property)}",
            SourceJson = @"
            {
                ""ClrName"": ""Value"",
                ""ClrKind"": ""Property""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Property, clrName: "Value")
        },
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Property)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceJson = @"
            {
                ""clrName"": ""Value"",
                ""clrKind"": ""Property""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Property, clrName: "Value")
        },

        // Invalid JSON
        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with invalid {nameof(ApiClrMemberReference.ClrKind)} is preserved for schema validation",
            SourceJson = @"
            {
                ""ClrName"": ""Value"",
                ""ClrKind"": ""42""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrKind: null, clrName: "Value", hasInvalidClrKind: true)
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with missing {nameof(ApiClrMemberReference.ClrName)} is preserved for schema validation",
            SourceJson = @"
            {
                ""ClrKind"": ""Property""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Property, clrName: null, hasInvalidClrKind: false)
        },

        new JsonDeserializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with missing {nameof(ApiClrMemberReference.ClrKind)} is preserved for schema validation",
            SourceJson = @"
            {
                ""ClrName"": ""Value""
            }",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrKind: null, clrName: "Value", hasInvalidClrKind: true)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Field)}",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Field, clrName: "Value")
        },
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Property)}",
            ExpectedFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Property, clrName: "Value")
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Field)}",
            SourceFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Field, clrName: "Value"),
            ExpectedJson = @"
            {
                ""ClrKind"": ""Field"",
                ""ClrName"": ""Value""
            }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Field)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Field, clrName: "Value"),
            ExpectedJson = @"
            {
                ""clrKind"": ""Field"",
                ""clrName"": ""Value""
            }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Property)}",
            SourceFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Property, clrName: "Value"),
            ExpectedJson = @"
            {
                ""ClrKind"": ""Property"",
                ""ClrName"": ""Value""
            }"
        },
        new JsonSerializeTest
        {
            Name = $"{nameof(ApiClrMemberReference)} with {nameof(ApiClrMemberReference.ClrKind)}.{nameof(ClrMemberKind.Property)} and camel-case JSON",
            JsonSerializerOptions = CamelCaseOptions,
            SourceFactoryArgument = new ApiClrMemberReferenceDef(clrKind: ClrMemberKind.Property, clrName: "Value"),
            ExpectedJson = @"
            {
                ""clrKind"": ""Property"",
                ""clrName"": ""Value""
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
