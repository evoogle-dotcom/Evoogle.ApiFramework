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

public class ApiEnumTypeBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    public enum SampleEnum
    {
        First,
        Second
    }

    private sealed class TestExtension
    {
        public bool Flag { get; set; }
    }

    public class BuildTest : XUnitTest
    {
        public bool? AddExtension { get; init; }

        private ApiEnumType? ApiEnumType { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"AddExtension: {this.AddExtension}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext(null);
            var builder = new ApiEnumTypeBuilder(typeof(SampleEnum), context)
                .WithName("SampleEnum")
                .AddValue("first", nameof(SampleEnum.First), 0)
                .AddValue("second", nameof(SampleEnum.Second), 1);
            if (this.AddExtension == true)
            {
                builder.AddExtension(new TestExtension { Flag = true });
            }
            var build = typeof(ApiEnumTypeBuilder).GetMethod("Build", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            this.ApiEnumType = (ApiEnumType)build.Invoke(builder, null)!;
            this.WriteLine($"Values: {this.ApiEnumType.ApiEnumValues.Length}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ApiEnumType.Should().NotBeNull();
            this.ApiEnumType!.ApiName.Should().Be("SampleEnum");
            this.ApiEnumType.ApiEnumValues.Should().HaveCount(2);
            if (this.AddExtension == true)
            {
                this.ApiEnumType.Extensions.Should().NotBeNull();
                this.ApiEnumType.Extensions!.ContainsKey(typeof(TestExtension)).Should().BeTrue();
            }
        }
    }

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = "Builds enum type without extension",
            AddExtension = false
        },
        new BuildTest
        {
            Name = "Builds enum type with extension",
            AddExtension = true
        }
    ];

    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
}

