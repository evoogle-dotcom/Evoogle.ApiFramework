// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema;
using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.XUnit;
using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiScalarTypeBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    private sealed class TestExtension
    {
        public bool Flag { get; set; }
    }

    public class BuildTest : XUnitTest
    {
        public bool? AddExtension { get; init; }

        private ApiScalarType? ApiScalarType { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"AddExtension: {this.AddExtension}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext(null);
            var builder = new ApiScalarTypeBuilder(typeof(string), context).WithName("String");
            if (this.AddExtension == true)
            {
                builder.AddExtension(new TestExtension { Flag = true });
            }
            var build = typeof(ApiScalarTypeBuilder).GetMethod("Build", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            this.ApiScalarType = (ApiScalarType)build.Invoke(builder, null)!;
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ApiScalarType.Should().NotBeNull();
            this.ApiScalarType!.ApiName.Should().Be("String");
            if (this.AddExtension == true)
            {
                this.ApiScalarType.Extensions.Should().NotBeNull();
                this.ApiScalarType.Extensions!.ContainsKey(typeof(TestExtension)).Should().BeTrue();
            }
        }
    }

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = "Builds scalar type without extension",
            AddExtension = false
        },
        new BuildTest
        {
            Name = "Builds scalar type with extension",
            AddExtension = true
        }
    ];

    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
}

