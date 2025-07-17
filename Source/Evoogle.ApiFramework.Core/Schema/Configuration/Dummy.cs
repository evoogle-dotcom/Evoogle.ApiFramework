// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public static class Dummy
{
    public record struct EmailAddress(string Email)
    {
        public static implicit operator EmailAddress(string address) => new(address);
        public static implicit operator string(EmailAddress emailAddress) => emailAddress.Email;
    }

    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public EmailAddress? Email { get; set; }
        public List<Order> Orders { get; set; } = [];
    }

    public class Order
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled
    }

    public class EmailAddressConfiguration : IApiScalarTypeConfiguration
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
            .AddScalar(typeof(EmailAddress), x => x
                .WithApiName("EmailAddress"))
            .AddObject(typeof(Customer), x => x
                .WithApiName("Customer")
                .AddProperty("Id", "Id", typeof(Guid), m => m.Required())
                .AddProperty("Name", "Name", typeof(string), m => m.Required())
                .AddProperty("Email", "Email", typeof(EmailAddress), m => m.Nullable())
                .AddProperty("Orders", "Orders", typeof(List<Order>), m => m.Required())
                .AddRelationship("Orders"))
            .AddObject(typeof(Order), x => x
                .WithApiName("Order")
                .AddProperty("Id", "Id", typeof(Guid), m => m.Required())
                .AddProperty("Status", "Status", typeof(OrderStatus), m => m.Required())
                .AddProperty("Total", "Total", typeof(decimal), m => m.Required())
                .AddProperty("CustomerId", "CustomerId", typeof(Guid?), m => m.Nullable())
                .AddProperty("Customer", "Customer", typeof(Customer), m => m.Nullable())
                .AddRelationship("Customer")
                )
            .AddEnum(typeof(OrderStatus), x => x
                .WithApiName("OrderStatus")
                .AddValue("Pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1)
                .AddValue("Delivered", "Delivered", 2)
                .AddValue("Cancelled", "Cancelled", 3))
            .AddScalar(typeof(EmailAddress), new EmailAddressConfiguration())
            .AddEnum(typeof(OrderStatus), new OrderStatusConfiguration())
            .AddObject(typeof(Order), new OrderConfiguration())
            .Build();
    }
}
