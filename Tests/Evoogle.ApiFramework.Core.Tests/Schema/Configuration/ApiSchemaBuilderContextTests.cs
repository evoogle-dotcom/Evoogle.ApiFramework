// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.TestData;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
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
            var method = typeof(ApiSchemaBuilderContext).GetMethod(this.MethodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, [typeof(Type)]);

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
            Name = "GetOrAddScalarTypeBuilder returns same instance for same CLR type",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddScalarTypeBuilder),
            ClrType = typeof(int)
        },
        new GetOrAddTest
        {
            Name = "GetOrAddEnumTypeBuilder returns same instance for same CLR type",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddEnumTypeBuilder),
            ClrType = typeof(OrderStatus)
        },
        new GetOrAddTest
        {
            Name = "GetOrAddObjectTypeBuilder returns same instance for same CLR type",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddObjectTypeBuilder),
            ClrType = typeof(Order)
        }
    ];

    [Theory]
    [MemberData(nameof(GetOrAddTheoryData))]
    public void GetOrAdd(IXUnitTest test) => test.Execute(this);

    [Fact]
    public void GetOrAddObjectTypeBuilderGenericAndNonGenericReturnSameBuilder()
    {
        var context = new ApiSchemaBuilderContext();
        var nonGenericBuilder = context.GetOrAddObjectTypeBuilder(typeof(Order));

        var genericBuilder = context.GetOrAddObjectTypeBuilder<Order>();

        ReferenceEquals(nonGenericBuilder, genericBuilder).Should().BeTrue();
    }

    [Fact]
    public void GetOrAddScalarTypeBuilderGenericAndNonGenericReturnSameBuilder()
    {
        var context = new ApiSchemaBuilderContext();
        var nonGenericBuilder = context.GetOrAddScalarTypeBuilder(typeof(int));

        var genericBuilder = context.GetOrAddScalarTypeBuilder<int>();

        ReferenceEquals(nonGenericBuilder, genericBuilder).Should().BeTrue();
    }

    [Fact]
    public void GetOrAddEnumTypeBuilderGenericAndNonGenericReturnSameBuilder()
    {
        var context = new ApiSchemaBuilderContext();
        var nonGenericBuilder = context.GetOrAddEnumTypeBuilder(typeof(OrderStatus));

        var genericBuilder = context.GetOrAddEnumTypeBuilder<OrderStatus>();

        ReferenceEquals(nonGenericBuilder, genericBuilder).Should().BeTrue();
    }

    [Fact]
    public void GetOrAddObjectTypeBuilderNonGenericReturnsExistingGenericBuilder()
    {
        var context = new ApiSchemaBuilderContext();
        var genericBuilder = context.GetOrAddObjectTypeBuilder<Order>();

        var nonGenericBuilder = context.GetOrAddObjectTypeBuilder(typeof(Order));

        ReferenceEquals(genericBuilder, nonGenericBuilder).Should().BeTrue();
    }

    [Fact]
    public void GetOrAddScalarTypeBuilderNonGenericReturnsExistingGenericBuilder()
    {
        var context = new ApiSchemaBuilderContext();
        var genericBuilder = context.GetOrAddScalarTypeBuilder<int>();

        var nonGenericBuilder = context.GetOrAddScalarTypeBuilder(typeof(int));

        ReferenceEquals(genericBuilder, nonGenericBuilder).Should().BeTrue();
    }

    [Fact]
    public void GetOrAddEnumTypeBuilderNonGenericReturnsExistingGenericBuilder()
    {
        var context = new ApiSchemaBuilderContext();
        var genericBuilder = context.GetOrAddEnumTypeBuilder<OrderStatus>();

        var nonGenericBuilder = context.GetOrAddEnumTypeBuilder(typeof(OrderStatus));

        ReferenceEquals(genericBuilder, nonGenericBuilder).Should().BeTrue();
    }

    [Fact]
    public void CreateClosedGenericBuilderThrowsSchemaConfigurationExceptionForInvalidType()
    {
        var act = () => ApiBuilderFactory.CreateClosedGeneric<ApiEnumTypeBuilder>
        (
            typeof(ApiEnumTypeBuilder<>),
            typeof(int),
            new ApiSchemaBuilderContext()
        );

        act.Should()
            .Throw<ApiSchemaConfigurationException>()
            .WithMessage("Unable to create ApiEnumTypeBuilder`1 for CLR type 'Int32'.");
    }

    [Fact]
    public void GetOrAddTypedRelationshipBuilderThrowsConfigurationExceptionWhenDifferentKindExists()
    {
        var context = new ApiSchemaBuilderContext();
        context.GetOrAddOneToOneRelationshipBuilder("REL_Test");

        var act = () => context.GetOrAddOneToManyRelationshipBuilder("REL_Test");

        act.Should().Throw<ApiSchemaConfigurationException>();
    }
}
