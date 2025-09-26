// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiTypeModifiersBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    public class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public bool? UseRequired { get; init; }
        public bool? UseOptional { get; init; }
        public ApiTypeModifiers Expected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiTypeModifiers Actual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"UseRequired: {this.UseRequired}");
            this.WriteLine($"UseOptional: {this.UseOptional}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.Expected}");
        }

        protected override void Act()
        {
            var builder = new ApiTypeModifiersBuilder();
            if (this.UseRequired == true)
            {
                builder.Required();
            }
            if (this.UseOptional == true)
            {
                builder.Optional();
            }
            this.Actual = builder.Build();
            this.WriteLine($"Actual:   {this.Actual}");
        }

        protected override void Assert()
        {
            this.Actual.Should().Be(this.Expected);
        }
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
            Name = "UseRequired sets flag",
            UseRequired = true,
            Expected = ApiTypeModifiers.Required
        },
        new BuildTest
        {
            Name = "UseOptional clears flag",
            UseRequired = true,
            UseOptional = true,
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

