// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public static class Dummy
{
    public record struct MailAddress(string Address)
    {
        public static implicit operator MailAddress(string address) => new(address);
        public static implicit operator string(MailAddress mailAddress) => mailAddress.Address;
    }

    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = [];
    }

    public class Order
    {
        public Guid OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled
    }

    public class MailAddressConfiguration : IApiScalarTypeConfiguration
    {
        public void Configure(ApiScalarTypeBuilder builder)
        {
            builder
                .WithApiName("Email");
        }
    }

    public class OrderStatusConfiguration : IApiEnumTypeConfiguration
    {
        public void Configure(ApiEnumTypeBuilder builder)
        {
            builder
                .WithApiName("OrderStatus")
                .AddValue("Pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1)
                .AddValue("Delivered", "Delivered", 2)
                .AddValue("Cancelled", "Cancelled", 3);
        }
    }

    public class OrderConfiguration : IApiObjectTypeConfiguration
    {
        public void Configure(ApiObjectTypeBuilder builder)
        {
            builder
                .WithApiName("Order")
                .AddProperty("id", "Id", typeof(Guid))
                .AddProperty("status", "Status", typeof(OrderStatus))
                .AddProperty("total", "Total", typeof(decimal));
        }
    }

    public static void DummyMethod()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("CustomerOrdersAPI")
            .WithVersion("v1")
            .AddScalar(typeof(MailAddress), x => x
                .WithApiName("Email"))
            .AddObject(typeof(Customer), x => x
                .WithApiName("Customer")
                .AddProperty("email", "Email", typeof(MailAddress), m => m.Required())
                .AddRelationship("orders"))
            .AddObject(typeof(Order), x => x
                .WithApiName("Order")
                .AddProperty("id", "Id", typeof(Guid))
                .AddProperty("status", "Status", typeof(OrderStatus))
                .AddProperty("total", "Total", typeof(decimal)))
            .AddEnum(typeof(OrderStatus), x => x
                .WithApiName("OrderStatus")
                .AddValue("Pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1)
                .AddValue("Delivered", "Delivered", 2)
                .AddValue("Cancelled", "Cancelled", 3))
            .AddScalar(typeof(MailAddress), new MailAddressConfiguration())
            .AddEnum(typeof(OrderStatus), new OrderStatusConfiguration())
            .AddObject(typeof(Order), new OrderConfiguration())
            .Build();
    }
}
