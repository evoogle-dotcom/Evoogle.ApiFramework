// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.XUnit;
using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiTypeModifiersBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    public class BuildTest : XUnitTest
    {
        public bool? UseRequired { get; init; }
        public bool? UseOptional { get; init; }
        public ApiTypeModifiers? Expected { get; init; }

        private ApiTypeModifiers? Actual { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"UseRequired: {this.UseRequired}");
            this.WriteLine($"UseOptional: {this.UseOptional}");
            this.WriteLine($"Expected: {this.Expected}");
            this.WriteLine();
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
            var build = typeof(ApiTypeModifiersBuilder).GetMethod("Build", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            this.Actual = (ApiTypeModifiers)build.Invoke(builder, null)!;
            this.WriteLine($"Actual: {this.Actual}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.Actual.Should().Be(this.Expected);
        }
    }

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = "Default is None",
            Expected = ApiTypeModifiers.None
        },
        new BuildTest
        {
            Name = "Required sets flag",
            UseRequired = true,
            Expected = ApiTypeModifiers.Required
        },
        new BuildTest
        {
            Name = "Optional clears flag",
            UseRequired = true,
            UseOptional = true,
            Expected = ApiTypeModifiers.None
        }
    ];

    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
}

