// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiRelationshipBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    private sealed class TestExtension
    {
        public bool Flag { get; set; }
    }

    public class BuildTest : XUnitTest
    {
        public string? ApiName { get; init; }
        public string? ApiPropertyName { get; init; }
        public bool? AddExtension { get; init; }

        private ApiRelationship? ApiRelationship { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"ApiName: {this.ApiName}");
            this.WriteLine($"ApiPropertyName: {this.ApiPropertyName}");
            this.WriteLine($"AddExtension: {this.AddExtension}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var builder = new ApiRelationshipBuilder(this.ApiName!, this.ApiPropertyName);
            if (this.AddExtension == true)
            {
                builder.AddExtension(new TestExtension { Flag = true });
            }
            var build = typeof(ApiRelationshipBuilder).GetMethod("Build", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            this.ApiRelationship = (ApiRelationship)build.Invoke(builder, null)!;
            this.WriteLine($"Extensions? {this.ApiRelationship.Extensions is not null}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ApiRelationship.Should().NotBeNull();
            this.ApiRelationship!.ApiName.Should().Be(this.ApiName);
            this.ApiRelationship.ApiPropertyName.Should().Be(this.ApiPropertyName);
            if (this.AddExtension == true)
            {
                this.ApiRelationship.Extensions.Should().NotBeNull();
                this.ApiRelationship.Extensions!.ContainsKey(typeof(TestExtension)).Should().BeTrue();
            }
        }
    }

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = "Builds relationship without extension",
            ApiName = "rel",
            ApiPropertyName = "prop"
        },
        new BuildTest
        {
            Name = "Builds relationship with extension",
            ApiName = "relExt",
            ApiPropertyName = null,
            AddExtension = true
        }
    ];

    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
}

