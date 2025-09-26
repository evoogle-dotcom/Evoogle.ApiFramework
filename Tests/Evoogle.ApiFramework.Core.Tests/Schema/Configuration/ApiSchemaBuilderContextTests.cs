// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiSchemaBuilderContextTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public class Parent
    {
        public string Name { get; set; } = string.Empty;
        public List<Parent> Children { get; set; } = [];
    }
    #endregion

    public enum BuilderKind
    {
        Enum,
        Object,
        Scalar
    }

    public class GetOrAddTest : XUnitTest
    {
        public BuilderKind? Kind { get; init; }
        public Type? ClrType { get; init; }

        private object? Builder1 { get; set; }
        private object? Builder2 { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"Kind: {this.Kind}");
            this.WriteLine($"ClrType: {this.ClrType}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext();
            var methodName = this.Kind switch
            {
                BuilderKind.Enum => "GetOrAddEnumTypeBuilder",
                BuilderKind.Object => "GetOrAddObjectTypeBuilder",
                BuilderKind.Scalar => "GetOrAddScalarTypeBuilder",
                _ => throw new InvalidOperationException()
            };
            var method = typeof(ApiSchemaBuilderContext).GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            this.Builder1 = method!.Invoke(context, [this.ClrType!]);
            this.Builder2 = method.Invoke(context, [this.ClrType!]);
        }

        protected override void Assert()
        {
            ReferenceEquals(this.Builder1, this.Builder2).Should().BeTrue();
        }
    }

    public static TheoryDataRow<IXUnitTest>[] GetOrAddTheoryData =>
    [
        new GetOrAddTest
        {
            Name = "Scalar builder cached",
            Kind = BuilderKind.Scalar,
            ClrType = typeof(int)
        },
        new GetOrAddTest
        {
            Name = "Enum builder cached",
            Kind = BuilderKind.Enum,
            ClrType = typeof(OrderStatus)
        },
        new GetOrAddTest
        {
            Name = "Object builder cached",
            Kind = BuilderKind.Object,
            ClrType = typeof(Parent)
        }
    ];

    [Theory]
    [MemberData(nameof(GetOrAddTheoryData))]
    public void GetOrAdd(IXUnitTest test) => test.Execute(this);
}

