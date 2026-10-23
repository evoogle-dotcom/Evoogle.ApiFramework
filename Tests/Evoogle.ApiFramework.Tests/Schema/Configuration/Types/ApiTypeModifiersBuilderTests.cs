// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Types;

public class ApiTypeModifiersBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    private class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public bool? UsesNonNullable { get; init; }
        public bool? UsesNullable { get; init; }
        public ApiTypeModifiers Expected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiTypeModifiers Actual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario: {this.Name}");
            this.WriteLine();
            this.WriteLine($"Uses Non-Nullable: {this.UsesNonNullable}");
            this.WriteLine($"Uses Nullable:     {this.UsesNullable}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.Expected}");
        }

        protected override void Act()
        {
            var builder = new ApiTypeModifiersBuilder();
            if (this.UsesNonNullable == true)
            {
                builder.AsNonNullable();
            }
            if (this.UsesNullable == true)
            {
                builder.AsNullable();
            }
            this.Actual = builder.Build();
            this.WriteLine();
            this.WriteLine($"Actual: {this.Actual}");
        }

        protected override void Assert() => this.Actual.Should().Be(this.Expected);
        #endregion
    }

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = "Default is None",
            Expected = ApiTypeModifiers.None
        },
        new BuildTest
        {
            Name = "AsNonNullable sets the Required flag",
            UsesNonNullable = true,
            Expected = ApiTypeModifiers.Required
        },
        new BuildTest
        {
            Name = "AsNullable clears the Required flag",
            UsesNonNullable = true,
            UsesNullable = true,
            Expected = ApiTypeModifiers.None
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}
