// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Verifies that registering a relationship via <see cref="ApiObjectTypeBuilder{T}"/>
///     produces an identical <see cref="ApiRelationship"/> to registering the same relationship
///     directly via <see cref="ApiSchemaBuilder"/>, and that the two entry points share the
///     same builder instance (idempotency).
/// </summary>
public class ApiObjectTypeBuilderRelationshipTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    /// <summary>
    ///     Compares an <see cref="ApiRelationship"/> built via the schema-level entry point
    ///     against the same relationship built via the object-type-level entry point.
    /// </summary>
    private class EquivalenceTest : XUnitTest
    {
        #region User Supplied Properties
        public required Func<ApiRelationship> BuildExpected { get; init; }
        public required Func<ApiRelationship> BuildActual { get; init; }
        #endregion

        #region Calculated Properties
        private ApiRelationship? Expected { get; set; }
        private ApiRelationship? Actual { get; set; }
        #endregion

        #region Constructors
        // [SetsRequiredMembers]
        public EquivalenceTest()
        {
            this.Name = nameof(EquivalenceTest);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Relationship;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Expected = this.BuildExpected();
            this.WriteLine($"Expected: {this.Expected.SafeToString()}");
        }

        protected override void Act()
        {
            this.Actual = this.BuildActual();
            this.WriteLine($"Actual:   {this.Actual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.Actual.Should().NotBeNull();
            this.AssertBeEquivalentTo(this.Actual, this.Expected);
        }
        #endregion
    }

    /// <summary>
    ///     Verifies that registering the same relationship name from both the schema-level entry point
    ///     and the object-type-level entry point produces exactly one relationship in the schema
    ///     (i.e., <see cref="ApiSchemaBuilderContext.GetOrAddXxxRelationshipBuilder"/> is truly idempotent
    ///     across both registration paths).
    /// </summary>
    private class IdempotencyTest : XUnitTest
    {
        #region User Supplied Properties
        public required Action<ApiSchemaBuilderContext> ConfigureViaFirstPath { get; init; }
        public required Action<ApiSchemaBuilderContext> ConfigureViaSecondPath { get; init; }
        #endregion

        #region Calculated Properties
        private int? FirstPathRelationshipCount { get; set; }
        private int? SecondPathRelationshipCount { get; set; }
        #endregion

        #region Constructors
        // [SetsRequiredMembers]
        public IdempotencyTest()
        {
            this.Name = nameof(IdempotencyTest);
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine("Verifying that registering the same relationship name from both entry points yields exactly one ApiRelationship.");
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext();

            // Register via first path
            this.ConfigureViaFirstPath(context);
            this.FirstPathRelationshipCount = context.ApiRelationshipBuilders.Count();

            // Register the same name via second path — should reuse the existing builder
            this.ConfigureViaSecondPath(context);
            this.SecondPathRelationshipCount = context.ApiRelationshipBuilders.Count();
        }

        protected override void Assert()
        {
            this.FirstPathRelationshipCount.Should().Be(1, "one relationship was registered via the first entry point");
            this.SecondPathRelationshipCount.Should().Be(1, "the second entry point should reuse the same builder, not add a second one");
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] EquivalenceTheoryData =>
    [
        new EquivalenceTest
        {
            Name = "1:1 — object-level registration produces identical ApiRelationship to schema-level registration",
            BuildExpected = static () =>
            {
                // Schema-level (canonical entry point): build directly via the relationship builder
                return new ApiRelationshipOneToOneBuilder("REL_Customer_Order_1to1")
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(d => d.AddScalarPath(o => o.Id))
                    .Build();
            },
            BuildActual = static () =>
            {
                // Object-level (Option C entry point): register via ApiObjectTypeBuilder<T>, retrieve from context
                var context = new ApiSchemaBuilderContext();
                var objectTypeBuilder = context.GetOrAddObjectTypeBuilder<Customer>();
                objectTypeBuilder.AddOneToOneRelationship("REL_Customer_Order_1to1", r => r
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(d => d.AddScalarPath(o => o.Id)));
                return context.ApiRelationshipBuilders.Single().Build();
            }
        },

        new EquivalenceTest
        {
            Name = "1:M — object-level registration on the principal produces identical ApiRelationship to schema-level registration",
            BuildExpected = static () =>
            {
                return new ApiRelationshipOneToManyBuilder("REL_Order_OrderLine_1toN")
                    .WithPrincipalEnd<Order>()
                    .WithDependentEnd<OrderLine>(d => d.AddScalarPath(ol => ol.OrderId))
                    .Build();
            },
            BuildActual = static () =>
            {
                var context = new ApiSchemaBuilderContext();
                var objectTypeBuilder = context.GetOrAddObjectTypeBuilder<Order>();
                objectTypeBuilder.AddOneToManyRelationship("REL_Order_OrderLine_1toN", r => r
                    .WithPrincipalEnd<Order>()
                    .WithDependentEnd<OrderLine>(d => d.AddScalarPath(ol => ol.OrderId)));
                return context.ApiRelationshipBuilders.Single().Build();
            }
        },

        new EquivalenceTest
        {
            Name = "1:M — object-level registration on the dependent produces identical ApiRelationship to schema-level registration",
            BuildExpected = static () =>
            {
                return new ApiRelationshipOneToManyBuilder("REL_Order_OrderLine_1toN")
                    .WithPrincipalEnd<Order>()
                    .WithDependentEnd<OrderLine>(d => d.AddScalarPath(ol => ol.OrderId))
                    .Build();
            },
            BuildActual = static () =>
            {
                var context = new ApiSchemaBuilderContext();
                var objectTypeBuilder = context.GetOrAddObjectTypeBuilder<OrderLine>();
                objectTypeBuilder.AddOneToManyRelationship("REL_Order_OrderLine_1toN", r => r
                    .WithPrincipalEnd<Order>()
                    .WithDependentEnd<OrderLine>(d => d.AddScalarPath(ol => ol.OrderId)));
                return context.ApiRelationshipBuilders.Single().Build();
            }
        },

        new EquivalenceTest
        {
            Name = "M:M — object-level registration produces identical ApiRelationship to schema-level registration",
            BuildExpected = static () =>
            {
                return new ApiRelationshipManyToManyBuilder<OrderLine>("REL_Customer_Order_NtoN")
                    .WithPrincipalEndA<Customer>()
                    .WithPrincipalEndB<Order>()
                    .WithDependentEndA(d => d.AddScalarPath(ol => ol.OrderId))
                    .WithDependentEndB(d => d.AddScalarPath(ol => ol.LineNumber))
                    .Build();
            },
            BuildActual = static () =>
            {
                var context = new ApiSchemaBuilderContext();
                var objectTypeBuilder = context.GetOrAddObjectTypeBuilder<Customer>();
                objectTypeBuilder.AddManyToManyRelationship<OrderLine>("REL_Customer_Order_NtoN", r => r
                    .WithPrincipalEndA<Customer>()
                    .WithPrincipalEndB<Order>()
                    .WithDependentEndA(d => d.AddScalarPath(ol => ol.OrderId))
                    .WithDependentEndB(d => d.AddScalarPath(ol => ol.LineNumber)));
                return context.ApiRelationshipBuilders.Single().Build();
            }
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] IdempotencyTheoryData =>
    [
        new IdempotencyTest
        {
            Name = "1:M — schema-level registration then object-level registration with same name yields exactly one ApiRelationship",
            ConfigureViaFirstPath = static context =>
            {
                // Schema-level path: get the relationship builder directly from the context
                var builder = context.GetOrAddOneToManyRelationshipBuilder("REL_Order_OrderLine_1toN");
                builder
                    .WithPrincipalEnd<Order>()
                    .WithDependentEnd<OrderLine>(d => d.AddScalarPath(ol => ol.OrderId));
            },
            ConfigureViaSecondPath = static context =>
            {
                // Object-level path: register via ApiObjectTypeBuilder<T> with the same name
                var objectTypeBuilder = context.GetOrAddObjectTypeBuilder<Order>();
                objectTypeBuilder.AddOneToManyRelationship("REL_Order_OrderLine_1toN", r => r
                    .WithPrincipalEnd<Order>()
                    .WithDependentEnd<OrderLine>(d => d.AddScalarPath(ol => ol.OrderId)));
            }
        },

        new IdempotencyTest
        {
            Name = "1:1 — object-level registration then schema-level registration with same name yields exactly one ApiRelationship",
            ConfigureViaFirstPath = static context =>
            {
                // Object-level path first
                var objectTypeBuilder = context.GetOrAddObjectTypeBuilder<Customer>();
                objectTypeBuilder.AddOneToOneRelationship("REL_Customer_Order_1to1", r => r
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(d => d.AddScalarPath(o => o.Id)));
            },
            ConfigureViaSecondPath = static context =>
            {
                // Schema-level path second: should reuse the existing builder
                var builder = context.GetOrAddOneToOneRelationshipBuilder("REL_Customer_Order_1to1");
                builder
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(d => d.AddScalarPath(o => o.Id));
            }
        },
    ];
    #endregion

    #region Theory Tests
    [Theory]
    [MemberData(nameof(EquivalenceTheoryData))]
    public void Equivalence(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(IdempotencyTheoryData))]
    public void Idempotency(IXUnitTest test) => test.Execute(this);
    #endregion
}
