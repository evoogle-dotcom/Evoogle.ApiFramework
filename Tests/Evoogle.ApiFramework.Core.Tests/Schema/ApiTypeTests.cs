// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
using Evoogle.XUnit;

using FluentAssertions;

using System.Text.Json;

namespace Evoogle.ApiFramework.Schema;

public class ApiTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    public class JsonSerializeTest : XUnitTest
    {
        #region Calculated Properties
        public string? ActualJson { get; set; }
        #endregion

        #region User Supplied Properties
        public ApiType? Source { get; set; }
        public string? ExpectedJson { get; set; }
        #endregion

        protected override void Arrange()
        {
            this.WriteLine($"Source: {this.Source.SafeToString()}");
            this.WriteLine($"Expected JSON: {this.ExpectedJson.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualJson = JsonSerializer.Serialize(this.Source);
            this.WriteLine($"Actual   JSON: {this.ActualJson.SafeToString()}");
        }

        protected override void Assert()
        {
            var expectedJsonMinusWhitespace = this.ExpectedJson.RemoveWhitespace();
            var actualJsonMinusWhitespace = this.ActualJson.RemoveWhitespace();

            expectedJsonMinusWhitespace.Should().Be(actualJsonMinusWhitespace);
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        new JsonSerializeTest
        {
            Name = "Null",
            Source = null,
            ExpectedJson = "null"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [Boolean]",
            Source = new ApiScalarType("Boolean", typeof(bool)),
            ExpectedJson =@"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [ID]",
            Source = new ApiScalarType("ID", typeof(string)),
            ExpectedJson =@"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""ID"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [Int]",
            Source = new ApiScalarType("Int", typeof(int)),
            ExpectedJson =@"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Int"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [Float]",
            Source = new ApiScalarType("Float", typeof(float)),
            ExpectedJson =@"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Float"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [String]",
            Source = new ApiScalarType("String", typeof(string)),
            ExpectedJson =@"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonSerializeTheoryData))]
    public void JsonSerialize(IXUnitTest test)
    {
        test.Execute(this);
    }
    #endregion
}