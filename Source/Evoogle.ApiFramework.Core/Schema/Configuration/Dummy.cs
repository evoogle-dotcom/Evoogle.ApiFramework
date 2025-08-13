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
                .WithName("Email")
                .AddExtension(new VisibleMetadata { Visible = true });
        }
    }

    public class OrderStatusConfiguration : IApiEnumTypeConfiguration
    {
        public void Configure(ApiEnumTypeBuilder builder)
        {
            builder
                .WithName("OrderStatus")
                .AddValue("Pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1)
                .AddValue("Delivered", "Delivered", 2)
                .AddValue("Cancelled", "Cancelled", 3)
                .AddExtension(new VisibleMetadata { Visible = true });
        }
    }

    public class OrderConfiguration : IApiObjectTypeConfiguration
    {
        public void Configure(ApiObjectTypeBuilder builder)
        {
            builder
                .WithName("Order")
                .AddProperty("id", "Id")
                .AddProperty("status", "Status")
                .AddProperty("total", "Total")
                .AddExtension(new VisibleMetadata { Visible = true });
        }
    }

    public class VisibleMetadata
    {
        public bool Visible { get; set; }
    }

    public static void DummyMethod()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("CustomerOrdersAPI")
            .WithVersion("v1")
            .AddScalar(typeof(EmailAddress), x => x
                .WithName("EmailAddress"))
                .AddExtension(new VisibleMetadata { Visible = true })
            .AddObject(typeof(Customer), x => x
                .WithName("Customer")
                .AddProperty("Id", "Id", m => m.Required())
                .AddProperty("Name", "Name", m => m.Required(), p => p.AddExtension(new VisibleMetadata { Visible = true }))
                .AddProperty("Email", "Email", m => m.Optional())
                .AddProperty("Orders", "Orders", m => m.Required())
                .AddRelationship("Orders"))
                .AddExtension(new VisibleMetadata { Visible = true })
            .AddObject(typeof(Order), x => x
                .WithName("Order")
                .AddProperty("Id", "Id", m => m.Required())
                .AddProperty("Status", "Status", m => m.Required())
                .AddProperty("Total", "Total", m => m.Required())
                .AddProperty("CustomerId", "CustomerId", m => m.Optional())
                .AddProperty("Customer", "Customer", m => m.Optional())
                .AddRelationship("Customer"))
            .AddEnum(typeof(OrderStatus), x => x
                .AddExtension(new VisibleMetadata { Visible = true })
                .WithName("OrderStatus")
                .AddValue("Pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1)
                .AddValue("Delivered", "Delivered", 2)
                .AddValue("Cancelled", "Cancelled", 3))
            .AddScalar(typeof(EmailAddress), new EmailAddressConfiguration())
            .AddEnum(typeof(OrderStatus), new OrderStatusConfiguration())
            .AddObject(typeof(Order), new OrderConfiguration())
            .AddExtension(new VisibleMetadata { Visible = true })
            .Build();
    }
}
