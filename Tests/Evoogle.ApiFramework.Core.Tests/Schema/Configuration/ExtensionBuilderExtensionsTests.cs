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

public class ExtensionBuilderExtensionsTests(ITestOutputHelper output) : XUnitTests(output)
{
    private sealed class TestExtension
    {
        public int Value { get; set; }
    }

    public class GenericAddExtensionTest : XUnitTest
    {
        private ApiScalarType? ApiScalarType { get; set; }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext(null);
            var builder = new ApiScalarTypeBuilder(typeof(int), context).WithName("Int");
            builder.AddExtension(new TestExtension { Value = 1 });
            var build = typeof(ApiScalarTypeBuilder).GetMethod("Build", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            this.ApiScalarType = (ApiScalarType)build.Invoke(builder, null)!;
        }

        protected override void Assert()
        {
            this.ApiScalarType.Should().NotBeNull();
            this.ApiScalarType!.Extensions.Should().NotBeNull();
            var ext = this.ApiScalarType.Extensions![typeof(TestExtension)] as TestExtension;
            ext!.Value.Should().Be(1);
        }
    }

    public class NonGenericAddExtensionTest : XUnitTest
    {
        private ApiScalarType? ApiScalarType { get; set; }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext(null);
            var builder = new ApiScalarTypeBuilder(typeof(int), context).WithName("Int");
            builder.AddExtension(typeof(TestExtension), new TestExtension { Value = 2 });
            var build = typeof(ApiScalarTypeBuilder).GetMethod("Build", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            this.ApiScalarType = (ApiScalarType)build.Invoke(builder, null)!;
        }

        protected override void Assert()
        {
            this.ApiScalarType.Should().NotBeNull();
            this.ApiScalarType!.Extensions.Should().NotBeNull();
            var ext = this.ApiScalarType.Extensions![typeof(TestExtension)] as TestExtension;
            ext!.Value.Should().Be(2);
        }
    }

    public static TheoryDataRow<IXUnitTest>[] AddExtensionTheoryData =>
    [
        new GenericAddExtensionTest
        {
            Name = "Generic AddExtension adds extension"
        },
        new NonGenericAddExtensionTest
        {
            Name = "Non generic AddExtension adds extension"
        }
    ];

    [Theory]
    [MemberData(nameof(AddExtensionTheoryData))]
    public void AddExtension(IXUnitTest test) => test.Execute(this);
}

