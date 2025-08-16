// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiObjectTypeBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public class Parent
    {
        public string Name { get; set; } = string.Empty;
        public List<Parent> Children { get; set; } = [];
    }

    private sealed class TestExtension
    {
        public bool Flag { get; set; }
    }
    #endregion

    public class BuildTest : XUnitTest
    {
        private ApiObjectType? ApiObjectType { get; set; }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext(null);
            var builder = new ApiObjectTypeBuilder(typeof(Parent), context)
                .WithName("Parent")
                .AddProperty("name", nameof(ApiObjectTypeBuilderTests.Parent.Name), m => m.Required(), p => p.AddExtension(new TestExtension { Flag = true }))
                .AddRelationship("children", nameof(ApiObjectTypeBuilderTests.Parent.Children), r => r.AddExtension(new TestExtension { Flag = true }))
                .AddExtension(new TestExtension { Flag = true });
            var build = typeof(ApiObjectTypeBuilder).GetMethod("Build", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            this.ApiObjectType = (ApiObjectType)build.Invoke(builder, null)!;
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ApiObjectType.Should().NotBeNull();
            this.ApiObjectType!.ApiName.Should().Be("Parent");
            this.ApiObjectType.ApiProperties.Should().HaveCount(1);
            var prop = this.ApiObjectType.ApiProperties[0];
            prop.ApiName.Should().Be("name");
            prop.ApiTypeModifiers.Should().Be(ApiTypeModifiers.Required);
            prop.Extensions.Should().NotBeNull();
            prop.Extensions!.ContainsKey(typeof(TestExtension)).Should().BeTrue();
            this.ApiObjectType.ApiRelationships.Should().HaveCount(1);
            var rel = this.ApiObjectType.ApiRelationships[0];
            rel.ApiName.Should().Be("children");
            rel.ApiPropertyName.Should().Be(nameof(ApiObjectTypeBuilderTests.Parent.Children));
            rel.Extensions.Should().NotBeNull();
            rel.Extensions!.ContainsKey(typeof(TestExtension)).Should().BeTrue();
            this.ApiObjectType.Extensions.Should().NotBeNull();
            this.ApiObjectType.Extensions!.ContainsKey(typeof(TestExtension)).Should().BeTrue();
        }
    }

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = "Builds object type with property, relationship and extensions"
        }
    ];

    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
}

