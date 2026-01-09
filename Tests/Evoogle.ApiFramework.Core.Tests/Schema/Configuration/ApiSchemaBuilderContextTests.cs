// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiSchemaBuilderContextTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class GetOrAddTest : XUnitTest
    {
        #region User Supplied Properties
        public string MethodName { get; init; } = null!;
        public Type ClrType { get; init; } = null!;
        #endregion

        #region Calculated Properties
        private object? Builder1 { get; set; }
        private object? Builder2 { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"MethodName: {this.MethodName.SafeToString()}");
            this.WriteLine($"ClrType: {this.ClrType.SafeToName()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext();
            var method = typeof(ApiSchemaBuilderContext).GetMethod(this.MethodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

            this.Builder1 = method!.Invoke(context, [this.ClrType]);
            this.Builder2 = method!.Invoke(context, [this.ClrType]);
        }

        protected override void Assert()
        {
            this.Builder1.Should().NotBeNull();
            this.Builder2.Should().NotBeNull();

            ReferenceEquals(this.Builder1, this.Builder2).Should().BeTrue();
        }
        #endregion
    }
    #endregion

    public static TheoryDataRow<IXUnitTest>[] GetOrAddTheoryData =>
    [
        new GetOrAddTest
        {
            Name = "Scalar builder cached",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddScalarTypeBuilder),
            ClrType = typeof(int)
        },
        new GetOrAddTest
        {
            Name = "Enum builder cached",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddEnumTypeBuilder),
            ClrType = typeof(OrderStatus)
        },
        new GetOrAddTest
        {
            Name = "Object builder cached",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddObjectTypeBuilder),
            ClrType = typeof(Order)
        }
    ];

    [Theory]
    [MemberData(nameof(GetOrAddTheoryData))]
    public void GetOrAdd(IXUnitTest test) => test.Execute(this);
}

