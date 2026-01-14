// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides sample domain types and schema configurations that are used in documentation and tests.
/// </summary>
public static class Dummy
{
    /// <summary>
    ///     Represents a lightweight value object that wraps a raw email address string.
    /// </summary>
    /// <param name="Email">The canonical email address value.</param>
    public record struct EmailAddress(string Email)
    {
        /// <summary>Implicitly converts a <see cref="string"/> to an <see cref="EmailAddress"/> value object.</summary>
        public static implicit operator EmailAddress(string address) => new(address);

        /// <summary>Implicitly converts an <see cref="EmailAddress"/> value object to its underlying string.</summary>
        public static implicit operator string(EmailAddress emailAddress) => emailAddress.Email;
    }

    /// <summary>
    ///     Represents a customer domain model that is used by the sample schema builder extensions.
    /// </summary>
    public class Customer
    {
        /// <summary>Gets or sets the unique customer identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the customer's display name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional customer email address.</summary>
        public EmailAddress? Email { get; set; }

        /// <summary>Gets or sets the collection of orders that belong to the customer.</summary>
        public List<Order> Orders { get; set; } = [];
    }

    /// <summary>
    ///     Represents an order domain model that is used by the sample schema builder extensions.
    /// </summary>
    public class Order
    {
        /// <summary>Gets or sets the unique order identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the current order status.</summary>
        public OrderStatus Status { get; set; }

        /// <summary>Gets or sets the order total.</summary>
        public decimal Total { get; set; }

        /// <summary>Gets or sets the optional identifier of the customer that owns the order.</summary>
        public Guid? CustomerId { get; set; }

        /// <summary>Gets or sets the optional navigation property to the owning customer.</summary>
        public Customer? Customer { get; set; }
    }

    public class OrderItem
    {
        public Guid OrderId { get; set; }
        public long LineItemNumber { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    ///     Defines the states that an order can be in for the sample schema configuration.
    /// </summary>
    public enum OrderStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled
    }

    /// <summary>
    ///     Demonstrates how to configure a scalar type using the fluent schema builder APIs.
    /// </summary>
    public class EmailAddressConfiguration : IApiScalarTypeConfiguration
    {
        /// <inheritdoc />
        public void Configure(ApiScalarTypeBuilder builder)
        {
            builder
                .WithName("Email")
                .AddExtension(new VisibleMetadata { Visible = true });
        }
    }

    /// <summary>
    ///     Demonstrates how to configure an enum type using the fluent schema builder APIs.
    /// </summary>
    public class OrderStatusConfiguration : IApiEnumTypeConfiguration
    {
        /// <inheritdoc />
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

    /// <summary>
    ///     Demonstrates how to configure an object type using the fluent schema builder APIs.
    /// </summary>
    public class OrderConfiguration : IApiObjectTypeConfiguration
    {
        /// <inheritdoc />
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

    /// <summary>
    ///     Represents a simple extension payload that controls visibility in the sample schema configuration.
    /// </summary>
    public class VisibleMetadata
    {
        /// <summary>Gets or sets a value indicating whether the target artifact should be visible.</summary>
        public bool Visible { get; set; }
    }

    /// <summary>
    ///     Builds a fully configured sample <see cref="ApiSchema"/> demonstrating the fluent builder API surface area.
    /// </summary>
    public static void DummyMethod()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("CustomerOrdersAPI")
            .WithVersion("v1")
            .WithDefaultOptions()
            .WithOptions(o => o
                .WithIdentityNullHandling(ApiIdentityNullHandling.ReturnEmpty))
            .AddScalar(typeof(EmailAddress), x => x
                .WithName("EmailAddress"))
                .AddExtension(new VisibleMetadata { Visible = true })
            .AddObject(typeof(Customer), x => x
                .WithName("Customer")
                .WithOptions(o => o
                    .WithIdentityNullHandling(ApiIdentityNullHandling.ThrowException))
                .AddIdentity("PrimaryKey", identity => identity
                    .AddPart("Id"))
                .AddProperty("Id", "Id", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Name", "Name", p => p.WithModifiers(m => m.Required()).AddExtension(new VisibleMetadata { Visible = true }))
                .AddProperty("Email", "Email", p => p.WithModifiers(m => m.Optional()))
                .AddProperty("Orders", "Orders", p => p.WithModifiers(m => m.Required()))
                .AddRelationship("Orders"))
                .AddExtension(new VisibleMetadata { Visible = true })
            .AddObject(typeof(Order), x => x
                .WithName("Order")
                .WithDefaultOptions()
                .AddProperty("Id", "Id", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Status", "Status", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Total", "Total", p => p.WithModifiers(m => m.Required()))
                .AddProperty("CustomerId", "CustomerId", p => p.WithModifiers(m => m.Optional()))
                .AddProperty("Customer", "Customer", p => p.WithModifiers(m => m.Optional()))
                .AddRelationship("Customer"))
            .AddObject(typeof(OrderItem), x => x
                .WithName("OrderItem")
                .WithDefaultOptions()
                .AddIdentity("PrimaryKey", identity => identity
                    .AddPart("OrderId")
                    .AddPart("LineItemNumber"))
                .AddProperty("OrderId", "OrderId", p => p.WithModifiers(m => m.Required()))
                .AddProperty("LineItemNumber", "LineItemNumber", p => p.WithModifiers(m => m.Required()))
                .AddProperty("ProductName", "ProductName", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Quantity", "Quantity", p => p.WithModifiers(m => m.Required()))
                .AddProperty("UnitPrice", "UnitPrice", p => p.WithModifiers(m => m.Required())))
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
