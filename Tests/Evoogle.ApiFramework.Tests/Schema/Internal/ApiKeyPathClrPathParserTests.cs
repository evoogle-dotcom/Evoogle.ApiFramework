// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Internal;

public class ApiKeyPathClrPathParserTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class ParseTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ClrPath { get; init; }

        public required string[] ExpectedClrPropertyNames { get; init; }

        public required bool ExpectedIsValid { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyPathClrPathParser.ParseResult? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.ActualResult = ApiKeyPathClrPathParser.Parse(this.ClrPath);
        }

        protected override void Assert()
        {
            this.ActualResult.Should().NotBeNull();
            this.ActualResult!.ClrPropertyNames.Should().Equal(this.ExpectedClrPropertyNames);
            this.ActualResult.IsValid.Should().Be(this.ExpectedIsValid);
            this.ActualResult.ValidationMessage.Should().Be(this.ExpectedIsValid ? null :
                "CLR paths must contain one or more non-empty dot-delimited property names.");
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ParseTheoryData =>
    [
        new ParseTest
        {
            Name = "Parses nested CLR path",
            ClrPath = "NestedPart.Id",
            ExpectedClrPropertyNames = ["NestedPart", "Id"],
            ExpectedIsValid = true
        },
        new ParseTest
        {
            Name = "Normalizes CLR path whitespace",
            ClrPath = "  NestedPart . Id  ",
            ExpectedClrPropertyNames = ["NestedPart", "Id"],
            ExpectedIsValid = true
        },
        new ParseTest
        {
            Name = "Preserves empty CLR path segment for compilation validation",
            ClrPath = "NestedPart..Id",
            ExpectedClrPropertyNames = ["NestedPart", "", "Id"],
            ExpectedIsValid = false
        },
        new ParseTest
        {
            Name = "Marks whitespace-only CLR path invalid",
            ClrPath = "   ",
            ExpectedClrPropertyNames = [""],
            ExpectedIsValid = false
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ParseTheoryData))]
    public void Parse(IXUnitTest test) => test.Execute(this);
    #endregion
}
