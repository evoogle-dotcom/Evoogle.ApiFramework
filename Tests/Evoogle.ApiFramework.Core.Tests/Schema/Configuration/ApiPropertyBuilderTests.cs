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

public class ApiPropertyBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    private class Sample
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestExtension
    {
        public bool Flag { get; set; }
    }

    public class BuildTest : XUnitTest
    {
        private ApiProperty? ApiProperty { get; set; }

        protected override void Act()
        {
            var builder = new ApiPropertyBuilder("name", nameof(Sample.Name));
            builder.AddExtension(new TestExtension { Flag = true });
            var build = typeof(ApiPropertyBuilder).GetMethod("Build", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            this.ApiProperty = (ApiProperty)build.Invoke(builder, [typeof(Sample)])!;
        }

        protected override void Assert()
        {
            this.ApiProperty.Should().NotBeNull();
            this.ApiProperty!.ApiName.Should().Be("name");
            this.ApiProperty.ClrName.Should().Be(nameof(Sample.Name));
            this.ApiProperty.ApiTypeModifiers.Should().Be(ApiTypeModifiers.Required);
            this.ApiProperty.Extensions.Should().NotBeNull();
            this.ApiProperty.Extensions!.ContainsKey(typeof(TestExtension)).Should().BeTrue();
        }
    }

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = "Builds property for existing CLR property"
        }
    ];

    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
}

