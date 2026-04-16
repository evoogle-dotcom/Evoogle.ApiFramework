// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

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
    ///     Verifies that typed relationship end builder overloads produce a well-formed
    ///     <see cref="ApiRelationship"/> with the expected CLR type references on each end.
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
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildRelationshipTheoryData =>
    [
        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithPrincipalEnd<Customer> and WithDependentEnd<Order> with AddScalarPath(expr)",
            BuildExpected = static () =>
            {
                return new ApiRelationshipOneToOneBuilder("REL_Customer_Order_1to1")
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(b => b.AddScalarPath(o => o.Id))
                    .Build();
            },
            BuildActual = static () =>
            {
                return new ApiRelationshipOneToOneBuilder("REL_Customer_Order_1to1")
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(b => b.AddScalarPath(o => o.Id))
                    .Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToManyBuilder typed WithPrincipalEnd<Customer> and WithDependentEnd<Order> with AddScalarPath(expr)",
            BuildExpected = static () =>
            {
                return new ApiRelationshipOneToManyBuilder("REL_Customer_Order_1toN")
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(b => b.AddScalarPath(o => o.Id))
                    .Build();
            },
            BuildActual = static () =>
            {
                return new ApiRelationshipOneToManyBuilder("REL_Customer_Order_1toN")
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(b => b.AddScalarPath(o => o.Id))
                    .Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipManyToManyBuilder typed WithPrincipalEndA<Customer> and WithDependentEndA with AddScalarPath(expr)",
            BuildExpected = static () =>
            {
                return new ApiRelationshipManyToManyBuilder<OrderLine>("REL_Customer_Order_NtoN")
                    .WithPrincipalEndA<Customer>()
                    .WithPrincipalEndB<Order>()
                    .WithDependentEndA(b => b.AddScalarPath(ol => ol.OrderId))
                    .WithDependentEndB(b => b.AddScalarPath(ol => ol.OrderId))
                    .Build();
            },
            BuildActual = static () =>
            {
                return new ApiRelationshipManyToManyBuilder<OrderLine>("REL_Customer_Order_NtoN")
                    .WithPrincipalEndA<Customer>()
                    .WithPrincipalEndB<Order>()
                    .WithDependentEndA(b => b.AddScalarPath(ol => ol.OrderId))
                    .WithDependentEndB(b => b.AddScalarPath(ol => ol.OrderId))
                    .Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithDependentEnd<Order> with multiple scalar paths",
            BuildExpected = static () =>
            {
                return new ApiRelationshipOneToOneBuilder("REL_Customer_Order_Nested")
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(b => b
                        .AddScalarPath(o => o.Id)
                        .AddScalarPath(o => o.PlacedAt))
                    .Build();
            },
            BuildActual = static () =>
            {
                return new ApiRelationshipOneToOneBuilder("REL_Customer_Order_Nested")
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>(b => b
                        .AddScalarPath(o => o.Id)
                        .AddScalarPath(o => o.PlacedAt))
                    .Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithDependentEnd<CustomerProfile> AddNestedPath drill-down",
            BuildExpected = static () =>
            {
                return new ApiRelationshipOneToOneBuilder("REL_Customer_Profile_NestedPath")
                    .WithPrincipalEnd<Dummy.Customer>()
                    .WithDependentEnd<Dummy.CustomerProfile>(d => d
                        .AddNestedPath(cp => cp.CustomerRef, n => n
                            .AddScalarPath(r => r.CustomerId)))
                    .Build();
            },
            BuildActual = static () =>
            {
                return new ApiRelationshipOneToOneBuilder("REL_Customer_Profile_NestedPath")
                    .WithPrincipalEnd<Dummy.Customer>()
                    .WithDependentEnd<Dummy.CustomerProfile>(d => d
                        .AddNestedPath(cp => cp.CustomerRef, n => n
                            .AddScalarPath(r => r.CustomerId)))
                    .Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithPrincipalEnd<Customer>(configure) threads delete behavior through",
            BuildExpected = static () =>
            {
                return new ApiRelationshipOneToOneBuilder("REL_Customer_Order_PrincipalConfigure")
                    .WithPrincipalEnd<Customer>(p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                    .WithDependentEnd<Order>()
                    .Build();
            },
            BuildActual = static () =>
            {
                return new ApiRelationshipOneToOneBuilder("REL_Customer_Order_PrincipalConfigure")
                    .WithPrincipalEnd<Customer>(p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                    .WithDependentEnd<Order>()
                    .Build();
            }
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipManyToManyBuilder WithDependentEndA and WithDependentEndB resolve independent CLR types",
            BuildExpected = static () =>
            {
                return new ApiRelationshipManyToManyBuilder<OrderLine>("REL_Customer_Order_NtoN_Ends")
                    .WithPrincipalEndA<Customer>()
                    .WithPrincipalEndB<Order>()
                    .WithDependentEndA(b => b.AddScalarPath(ol => ol.OrderId))
                    .WithDependentEndB(b => b.AddScalarPath(ol => ol.LineNumber))
                    .Build();
            },
            BuildActual = static () =>
            {
                return new ApiRelationshipManyToManyBuilder<OrderLine>("REL_Customer_Order_NtoN_Ends")
                    .WithPrincipalEndA<Customer>()
                    .WithPrincipalEndB<Order>()
                    .WithDependentEndA(b => b.AddScalarPath(ol => ol.OrderId))
                    .WithDependentEndB(b => b.AddScalarPath(ol => ol.LineNumber))
                    .Build();
            }
        },
    ];
    #endregion

    #region Theory Tests
    [Theory]
    [MemberData(nameof(BuildRelationshipTheoryData))]
    public void BuildRelationship(IXUnitTest test) => test.Execute(this);
    #endregion
}

