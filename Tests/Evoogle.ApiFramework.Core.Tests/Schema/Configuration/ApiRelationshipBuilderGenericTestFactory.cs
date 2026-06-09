// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using Evoogle.ApiFramework.TestData;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Static factory supplying named <c>BuildExpected_*</c> / <c>BuildActual_*</c> methods used by
///     <see cref="ApiRelationshipBuilderGenericTests"/> theory data rows.
///     <para>
///         The <see cref="DynamicLinqTypeAttribute"/> lets Dynamic LINQ resolve this class by name when the
///         <see cref="ApiRelationshipBuilderGenericTests"/> serializer reconstructs
///         <c>Expression&lt;Func&lt;T&gt;&gt;</c> properties from their JSON body strings.
///     </para>
/// </summary>
[DynamicLinqType]
public static class ApiRelationshipBuilderGenericTestFactory
{
    #region BuildRelationship — BuildExpected methods
    public static ApiRelationship BuildExpected_OneToOne_DependentEnd_ScalarPath()
        => new ApiRelationshipOneToOneBuilder("REL_Customer_Order_1to1")
            .From<Customer>()
            .To<Order>(b => b.WithForeignKey(fk => fk.AddPath(o => o.Id)))
            .Build();

    public static ApiRelationship BuildExpected_OneToMany_DependentEnd_ScalarPath()
        => new ApiRelationshipOneToManyBuilder("REL_Customer_Order_1toN")
            .From<Customer>()
            .To<Order>(b => b.WithForeignKey(fk => fk.AddPath(o => o.Id)))
            .Build();

    public static ApiRelationship BuildExpected_ManyToMany_DependentEnds_ScalarPaths()
        => new ApiRelationshipManyToManyBuilder("REL_Customer_Order_NtoN")
            .Between<Customer>()
            .And<Order>()
            .WithAssociation<OrderLine>(a => a
                .WithForeignKeyA(fk => fk.AddPath(ol => ol.OrderId))
                .WithForeignKeyB(fk => fk.AddPath(ol => ol.OrderId)))
            .Build();

    public static ApiRelationship BuildExpected_OneToOne_DependentEnd_MultipleScalarPaths()
        => new ApiRelationshipOneToOneBuilder("REL_Customer_Order_Nested")
            .From<Customer>()
            .To<Order>(b => b.WithForeignKey(fk => fk
                .AddPath(o => o.Id)
                .AddPath(o => o.PlacedAt)))
            .Build();

    public static ApiRelationship BuildExpected_OneToOne_NestedPath()
        => new ApiRelationshipOneToOneBuilder("REL_Customer_Profile_NestedPath")
            .From<Dummy.Customer>()
            .To<Dummy.CustomerProfile>(d => d
                .WithForeignKey(fk => fk.AddPath(cp => cp.CustomerRef.CustomerId)))
            .Build();

    public static ApiRelationship BuildExpected_OneToOne_PrincipalConfigure_DeleteBehavior()
        => new ApiRelationshipOneToOneBuilder("REL_Customer_Order_PrincipalConfigure")
            .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
            .From<Customer>()
            .To<Order>()
            .Build();

    public static ApiRelationship BuildExpected_ManyToMany_IndependentEndTypes()
        => new ApiRelationshipManyToManyBuilder("REL_Customer_Order_NtoN_Ends")
            .Between<Customer>()
            .And<Order>()
            .WithAssociation<OrderLine>(a => a
                .WithForeignKeyA(fk => fk.AddPath(ol => ol.OrderId))
                .WithForeignKeyB(fk => fk.AddPath(ol => ol.OrderId)))
            .Build();
    #endregion

    #region BuildRelationship — BuildActual methods
    public static ApiRelationship BuildActual_OneToOne_DependentEnd_ScalarPath()
        => new ApiRelationshipOneToOneBuilder("REL_Customer_Order_1to1")
            .From<Customer>()
            .To<Order>(b => b.WithForeignKey(fk => fk.AddPath(o => o.Id)))
            .Build();

    public static ApiRelationship BuildActual_OneToMany_DependentEnd_ScalarPath()
        => new ApiRelationshipOneToManyBuilder("REL_Customer_Order_1toN")
            .From<Customer>()
            .To<Order>(b => b.WithForeignKey(fk => fk.AddPath(o => o.Id)))
            .Build();

    public static ApiRelationship BuildActual_ManyToMany_DependentEnds_ScalarPaths()
        => new ApiRelationshipManyToManyBuilder("REL_Customer_Order_NtoN")
            .Between<Customer>()
            .And<Order>()
            .WithAssociation<OrderLine>(a => a
                .WithForeignKeyA(fk => fk.AddPath(ol => ol.OrderId))
                .WithForeignKeyB(fk => fk.AddPath(ol => ol.OrderId)))
            .Build();

    public static ApiRelationship BuildActual_OneToOne_DependentEnd_MultipleScalarPaths()
        => new ApiRelationshipOneToOneBuilder("REL_Customer_Order_Nested")
            .From<Customer>()
            .To<Order>(b => b.WithForeignKey(o => o.Id, o => o.PlacedAt))
            .Build();

    public static ApiRelationship BuildActual_OneToOne_NestedPath()
        => new ApiRelationshipOneToOneBuilder("REL_Customer_Profile_NestedPath")
            .From<Dummy.Customer>()
            .To<Dummy.CustomerProfile>(d => d
                .WithForeignKey(fk => fk.AddPath(cp => cp.CustomerRef.CustomerId)))
            .Build();

    public static ApiRelationship BuildActual_OneToOne_PrincipalConfigure_DeleteBehavior()
        => new ApiRelationshipOneToOneBuilder("REL_Customer_Order_PrincipalConfigure")
            .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
            .From<Customer>()
            .To<Order>()
            .Build();

    public static ApiRelationship BuildActual_ManyToMany_IndependentEndTypes()
        => new ApiRelationshipManyToManyBuilder("REL_Customer_Order_NtoN_Ends")
            .Between<Customer>()
            .And<Order>()
            .WithAssociation<OrderLine>(a => a
                .WithForeignKeyA(fk => fk.AddPath(ol => ol.OrderId))
                .WithForeignKeyB(fk => fk.AddPath(ol => ol.OrderId)))
            .Build();
    #endregion
}
