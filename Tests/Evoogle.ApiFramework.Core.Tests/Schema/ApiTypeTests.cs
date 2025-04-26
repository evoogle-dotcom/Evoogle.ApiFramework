// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
using System.Text.Json;

using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    public class JsonDeserializeTest : XUnitTest
    {
        #region Calculated Properties
        public ApiType? ActualApiType { get; set; }
        #endregion

        #region User Supplied Properties
        public string? SourceJson { get; set; }
        public ApiType? ExpectedApiType { get; set; }
        #endregion

        protected override void Arrange()
        {
            this.WriteLine($"Source      JSON: {this.SourceJson.SafeToString().RemoveWhitespace()}");
            this.WriteLine($"Expected ApiType: {this.ExpectedApiType.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualApiType = JsonSerializer.Deserialize<ApiType>(this.SourceJson!);
            this.WriteLine($"Actual   ApiType: {this.ActualApiType.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualApiType.Should().BeEquivalentTo(ExpectedApiType);
        }
    }

    public class JsonSerializeTest : XUnitTest
    {
        #region Calculated Properties
        public string? ActualJson { get; set; }
        #endregion

        #region User Supplied Properties
        public ApiType? SourceApiType { get; set; }
        public string? ExpectedJson { get; set; }
        #endregion

        protected override void Arrange()
        {
            this.WriteLine($"Source ApiType: {this.SourceApiType.SafeToString()}");
            this.WriteLine($"Expected  JSON: {this.ExpectedJson.SafeToString().RemoveWhitespace()}");
        }

        protected override void Act()
        {
            this.ActualJson = JsonSerializer.Serialize(this.SourceApiType);
            this.WriteLine($"Actual    JSON: {this.ActualJson.SafeToString().RemoveWhitespace()}");
        }

        protected override void Assert()
        {
            var actualJsonMinusWhitespace = this.ActualJson.RemoveWhitespace();
            var expectedJsonMinusWhitespace = this.ExpectedJson.RemoveWhitespace();

            actualJsonMinusWhitespace.Should().Be(expectedJsonMinusWhitespace);
        }
    }
    #endregion

    #region Theory Data
    public enum StopLight
    {
        None,
        Green,
        Yellow,
        Red
    }

    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        // Null
        new JsonDeserializeTest
        {
            Name = "Null",
            SourceJson = "null",
            ExpectedApiType = null
        },

        // ApiScalarType
        new JsonDeserializeTest
        {
            Name = "ApiScalarType [Boolean]",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("Boolean", typeof(bool))
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [ID]",
            SourceJson =  @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""ID"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("ID", typeof(string))
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [Int]",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Int"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("Int", typeof(int))
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [Float]",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Float"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("Float", typeof(float))
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [String]",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("String", typeof(string))
        },

        // ApiEnumType
        new JsonDeserializeTest
        {
            Name = "ApiEnumType [StopLight]",
            SourceJson = @"
            {
                ""Kind"": ""Enum"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedApiType = new ApiEnumType
            (
                "StopLight",
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        // Null
        new JsonSerializeTest
        {
            Name = "Null",
            SourceApiType = null,
            ExpectedJson = "null"
        },

        // ApiScalarType
        new JsonSerializeTest
        {
            Name = "ApiScalarType [Boolean]",
            SourceApiType = new ApiScalarType("Boolean", typeof(bool)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [ID]",
            SourceApiType = new ApiScalarType("ID", typeof(string)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""ID"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [Int]",
            SourceApiType = new ApiScalarType("Int", typeof(int)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Int"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [Float]",
            SourceApiType = new ApiScalarType("Float", typeof(float)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Float"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [String]",
            SourceApiType = new ApiScalarType("String", typeof(string)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },

        // ApiEnumType
        new JsonSerializeTest
        {
            Name = "ApiEnumType [StopLight]",
            SourceApiType = new ApiEnumType
            (
                "StopLight",
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            ),
            ExpectedJson = @"
            {
                ""Kind"": ""Enum"",
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
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
            }"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeTheoryData))]
    public void JsonDeserialize(IXUnitTest test)
    {
        test.Execute(this);
    }

    [Theory]
    [MemberData(nameof(JsonSerializeTheoryData))]
    public void JsonSerialize(IXUnitTest test)
    {
        test.Execute(this);
    }
    #endregion
}