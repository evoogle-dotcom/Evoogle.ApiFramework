// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Unit tests for the typed relationship end builder overloads:
///     <see cref="ApiRelationshipOneToOneBuilder.WithPrincipalEnd{TPrincipal}"/>,
///     <see cref="ApiRelationshipOneToOneBuilder.WithDependentEnd{TDependent}"/>,
///     <see cref="ApiRelationshipOneToManyBuilder.WithDependentEnd{TDependent}"/>,
///     <see cref="ApiRelationshipManyToManyBuilder.WithDependentEndA{TDependent}"/>, and related overloads.
/// </summary>
public class ApiRelationshipBuilderGenericTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    /// <summary>
    ///     Verifies that typed relationship end builder overloads produce the same
    ///     <see cref="ApiRelationship"/> as the equivalent string-based overloads.
    /// </summary>
    private class BuildRelationshipTest : XUnitTest
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
        public BuildRelationshipTest()
        {
            this.Name = nameof(BuildRelationshipTest);
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
    ///     Verifies that a typed end builder method throws the expected exception.
    /// </summary>
    private class ThrowsTest : XUnitTest
    {
        #region User Supplied Properties
        public required Action ThrowingAction { get; init; }
        public required string ExpectedMessageContains { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? ActualException { get; set; }
        #endregion

        #region Constructors
        public ThrowsTest()
        {
            this.Name = nameof(ThrowsTest);
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Expected exception containing: {this.ExpectedMessageContains.SafeToString()}");
        }

        protected override void Act()
        {
            try
            {
                this.ThrowingAction();
            }
            catch (Exception ex)
            {
                this.ActualException = ex;
                this.WriteLine($"Actual exception: {ex.GetType().Name}: {ex.Message}");
            }
        }

        protected override void Assert()
        {
            this.ActualException.Should().NotBeNull();
            this.ActualException.Should().BeOfType<ApiSchemaException>();
            this.ActualException!.Message.Should().Contain(this.ExpectedMessageContains);
        }
        #endregion
    }

    /// <summary>Creates a context with <c>Dummy.Customer</c> → <c>"Customer"</c> and <c>Dummy.CustomerProfile</c> → <c>"CustomerProfile"</c> registered.</summary>
    private static ApiSchemaBuilderContext MakeContextWithDummyCustomerAndProfile()
    {
        var ctx = new ApiSchemaBuilderContext();
        ctx.GetOrAddObjectTypeBuilder<Dummy.Customer>().WithName("Customer");
        ctx.GetOrAddObjectTypeBuilder<Dummy.CustomerProfile>().WithName("CustomerProfile");
        return ctx;
    }

    /// <summary>Creates a context with <c>Customer</c> → <c>"Customer"</c> and <c>Order</c> → <c>"Order"</c> registered.</summary>
    private static ApiSchemaBuilderContext MakeContextWithCustomerAndOrder()
    {
        var ctx = new ApiSchemaBuilderContext();
        ctx.GetOrAddObjectTypeBuilder<Customer>().WithName("Customer");
        ctx.GetOrAddObjectTypeBuilder<Order>().WithName("Order");
        return ctx;
    }

    /// <summary>Creates a context with <c>Customer</c>, <c>Order</c>, and <c>OrderLine</c> registered.</summary>
    private static ApiSchemaBuilderContext MakeContextWithCommerce()
    {
        var ctx = new ApiSchemaBuilderContext();
        ctx.GetOrAddObjectTypeBuilder<Customer>().WithName("Customer");
        ctx.GetOrAddObjectTypeBuilder<Order>().WithName("Order");
        ctx.GetOrAddObjectTypeBuilder<OrderLine>().WithName("OrderLine");
        return ctx;
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildRelationshipTheoryData =>
    [
        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithPrincipalEnd<Customer> and WithDependentEnd<Order> with AddScalarPath(expr)",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                var rel = ctx.GetOrAddOneToOneRelationshipBuilder("REL_Customer_Order_1to1");
                rel.WithPrincipalEnd("Customer")
                   .WithDependentEnd("Order", b => b.AddScalarPath("Id"));
                return rel.Build();
            },
            BuildActual = static () =>
            {
                var ctx = MakeContextWithCustomerAndOrder();
                var rel = ctx.GetOrAddOneToOneRelationshipBuilder("REL_Customer_Order_1to1");
                rel.WithPrincipalEnd<Customer>()
                   .WithDependentEnd<Order>(b => b.AddScalarPath(o => o.Id));
                return rel.Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToManyBuilder typed WithPrincipalEnd<Customer> and WithDependentEnd<Order> with AddScalarPath(expr)",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                var rel = ctx.GetOrAddOneToManyRelationshipBuilder("REL_Customer_Order_1toN");
                rel.WithPrincipalEnd("Customer")
                   .WithDependentEnd("Order", b => b.AddScalarPath("Id"));
                return rel.Build();
            },
            BuildActual = static () =>
            {
                var ctx = MakeContextWithCustomerAndOrder();
                var rel = ctx.GetOrAddOneToManyRelationshipBuilder("REL_Customer_Order_1toN");
                rel.WithPrincipalEnd<Customer>()
                   .WithDependentEnd<Order>(b => b.AddScalarPath(o => o.Id));
                return rel.Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipManyToManyBuilder typed WithPrincipalEndA<Customer> and WithDependentEndA<OrderLine> with AddScalarPath(expr)",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                var rel = ctx.GetOrAddManyToManyRelationshipBuilder("REL_Customer_Order_NtoN");
                rel.WithAssociationTypeName("OrderLine")
                   .WithPrincipalEndA("Customer")
                   .WithPrincipalEndB("Order")
                   .WithDependentEndA("OrderLine", b => b.AddScalarPath("OrderId"))
                   .WithDependentEndB("OrderLine", b => b.AddScalarPath("OrderId"));
                return rel.Build();
            },
            BuildActual = static () =>
            {
                var ctx = MakeContextWithCommerce();
                var rel = ctx.GetOrAddManyToManyRelationshipBuilder("REL_Customer_Order_NtoN");
                rel.WithAssociationTypeName("OrderLine")
                   .WithPrincipalEndA<Customer>()
                   .WithPrincipalEndB<Order>()
                   .WithDependentEndA<OrderLine>(b => b.AddScalarPath(ol => ol.OrderId))
                   .WithDependentEndB<OrderLine>(b => b.AddScalarPath(ol => ol.OrderId));
                return rel.Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithDependentEnd<Order> with nested AddNestedPath(expr)",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                var rel = ctx.GetOrAddOneToOneRelationshipBuilder("REL_Customer_Order_Nested");
                rel.WithPrincipalEnd("Customer")
                   .WithDependentEnd("Order", b => b
                       .AddScalarPath("Id")
                       .AddScalarPath("PlacedAt"));
                return rel.Build();
            },
            BuildActual = static () =>
            {
                var ctx = MakeContextWithCustomerAndOrder();
                var rel = ctx.GetOrAddOneToOneRelationshipBuilder("REL_Customer_Order_Nested");
                rel.WithPrincipalEnd<Customer>()
                   .WithDependentEnd<Order>(b => b
                       .AddScalarPath(o => o.Id)
                       .AddScalarPath(o => o.PlacedAt));
                return rel.Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithDependentEnd<CustomerProfile> AddNestedPath drill-down",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                var rel = ctx.GetOrAddOneToOneRelationshipBuilder("REL_Customer_Profile_NestedPath");
                rel.WithPrincipalEnd("Customer")
                   .WithDependentEnd("CustomerProfile", d => d
                       .AddNestedPath("CustomerRef", n => n.AddScalarPath("CustomerId")));
                return rel.Build();
            },
            BuildActual = static () =>
            {
                var ctx = MakeContextWithDummyCustomerAndProfile();
                var rel = ctx.GetOrAddOneToOneRelationshipBuilder("REL_Customer_Profile_NestedPath");
                rel.WithPrincipalEnd<Dummy.Customer>()
                   .WithDependentEnd<Dummy.CustomerProfile>(d => d
                       .AddNestedPath(cp => cp.CustomerRef, n => n
                           .AddScalarPath(r => r.CustomerId)));
                return rel.Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithPrincipalEnd<Customer>(configure) threads delete behavior through",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                var rel = ctx.GetOrAddOneToOneRelationshipBuilder("REL_Customer_Order_PrincipalConfigure");
                rel.WithPrincipalEnd("Customer", p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                   .WithDependentEnd("Order");
                return rel.Build();
            },
            BuildActual = static () =>
            {
                var ctx = MakeContextWithCustomerAndOrder();
                var rel = ctx.GetOrAddOneToOneRelationshipBuilder("REL_Customer_Order_PrincipalConfigure");
                rel.WithPrincipalEnd<Customer>(p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                   .WithDependentEnd<Order>();
                return rel.Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipManyToManyBuilder WithDependentEndA<T> and WithDependentEndB<T> resolve independent object type names",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                var rel = ctx.GetOrAddManyToManyRelationshipBuilder("REL_Customer_Order_NtoN_Ends");
                rel.WithAssociationTypeName("OrderLine")
                   .WithPrincipalEndA("Customer")
                   .WithPrincipalEndB("Order")
                   .WithDependentEndA("OrderLine", b => b.AddScalarPath("OrderId"))
                   .WithDependentEndB("OrderLine", b => b.AddScalarPath("LineNumber"));
                return rel.Build();
            },
            BuildActual = static () =>
            {
                var ctx = MakeContextWithCommerce();
                var rel = ctx.GetOrAddManyToManyRelationshipBuilder("REL_Customer_Order_NtoN_Ends");
                rel.WithAssociationTypeName("OrderLine")
                   .WithPrincipalEndA<Customer>()
                   .WithPrincipalEndB<Order>()
                   .WithDependentEndA<OrderLine>(b => b.AddScalarPath(ol => ol.OrderId))
                   .WithDependentEndB<OrderLine>(b => b.AddScalarPath(ol => ol.LineNumber));
                return rel.Build();
            }
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] ThrowsTheoryData =>
    [
        new ThrowsTest
        {
            Name = "ApiRelationshipOneToOneBuilder WithDependentEnd<T> without context throws ApiSchemaException",
            ThrowingAction = static () =>
            {
                // Create builder directly — no context injection
                var rel = new ApiRelationshipOneToOneBuilder("REL_NoContext");
                rel.WithDependentEnd<Order>();
            },
            ExpectedMessageContains = "No schema builder context is available"
        },

        new ThrowsTest
        {
            Name = "ApiRelationshipOneToOneBuilder WithDependentEnd<T> with unregistered type throws ApiSchemaException",
            ThrowingAction = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                // Only Customer registered — Order is not.
                ctx.GetOrAddObjectTypeBuilder<Customer>().WithName("Customer");
                var rel = ctx.GetOrAddOneToOneRelationshipBuilder("REL_UnregisteredDependent");
                rel.WithDependentEnd<Order>();
            },
            ExpectedMessageContains = nameof(Order)
        },

        new ThrowsTest
        {
            Name = "ApiRelationshipManyToManyBuilder WithDependentEndA<T> without context throws ApiSchemaException",
            ThrowingAction = static () =>
            {
                var rel = new ApiRelationshipManyToManyBuilder("REL_NoContext_MtoN");
                rel.WithDependentEndA<OrderLine>();
            },
            ExpectedMessageContains = "no schema context is available"
        },

        new ThrowsTest
        {
            Name = "ApiRelationshipOneToManyBuilder WithPrincipalEnd<T> without context throws ApiSchemaException",
            ThrowingAction = static () =>
            {
                var rel = new ApiRelationshipOneToManyBuilder("REL_NoContext_1toN");
                rel.WithPrincipalEnd<Customer>();
            },
            ExpectedMessageContains = "No schema builder context is available"
        },

        new ThrowsTest
        {
            Name = "ApiRelationshipManyToManyBuilder WithPrincipalEndA<T> without context throws ApiSchemaException",
            ThrowingAction = static () =>
            {
                var rel = new ApiRelationshipManyToManyBuilder("REL_NoContext_PrincipalA");
                rel.WithPrincipalEndA<Customer>();
            },
            ExpectedMessageContains = "no schema context is available"
        },
    ];
    #endregion

    #region Theory Tests
    [Theory]
    [MemberData(nameof(BuildRelationshipTheoryData))]
    public void BuildRelationship(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ThrowsTheoryData))]
    public void Throws(IXUnitTest test) => test.Execute(this);
    #endregion
}
