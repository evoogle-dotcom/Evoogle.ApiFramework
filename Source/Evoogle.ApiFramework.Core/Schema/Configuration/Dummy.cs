// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides sample domain models and schema configuration used for demonstration and tests.
/// </summary>
public static class Dummy
{
    /// <summary>
    ///     Represents an email address value object.
    /// </summary>
    /// <param name="Email">The email string.</param>
    public record struct EmailAddress(string Email)
    {
        /// <summary>
        ///     Implicitly converts a string to an <see cref="EmailAddress"/>.
        /// </summary>
        /// <param name="address">The email address.</param>
        public static implicit operator EmailAddress(string address) => new(address);

        /// <summary>
        ///     Converts the <see cref="EmailAddress"/> to its string representation.
        /// </summary>
        /// <param name="emailAddress">The email address value.</param>
        public static implicit operator string(EmailAddress emailAddress) => emailAddress.Email;
    }

    /// <summary>
    ///     Sample customer entity used in configuration examples.
    /// </summary>
    public class Customer
    {
        /// <summary>Gets or sets the unique customer identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the customer's display name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the customer's email address.</summary>
        public EmailAddress? Email { get; set; }

        /// <summary>Gets or sets the orders associated with the customer.</summary>
        public List<Order> Orders { get; set; } = [];
    }

    /// <summary>
    ///     Sample order entity used in configuration examples.
    /// </summary>
    public class Order
    {
        /// <summary>Gets or sets the unique order identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the current status of the order.</summary>
        public OrderStatus Status { get; set; }

        /// <summary>Gets or sets the total monetary value of the order.</summary>
        public decimal Total { get; set; }

        /// <summary>Gets or sets the identifier of the related customer.</summary>
        public Guid? CustomerId { get; set; }

        /// <summary>Gets or sets the related customer.</summary>
        public Customer? Customer { get; set; }
    }

    /// <summary>
    ///     Defines the lifecycle states for an <see cref="Order"/>.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>The order has been created but not yet processed.</summary>
        Pending,
        /// <summary>The order has shipped.</summary>
        Shipped,
        /// <summary>The order has been delivered.</summary>
        Delivered,
        /// <summary>The order was cancelled.</summary>
        Cancelled
    }

    /// <summary>
    ///     Example configuration for the <see cref="EmailAddress"/> scalar type.
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
    ///     Example configuration for the <see cref="OrderStatus"/> enumeration.
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
    ///     Example configuration for the <see cref="Order"/> object type.
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
    ///     Extension metadata used to indicate whether a schema element should be visible.
    /// </summary>
    public class VisibleMetadata
    {
        /// <summary>Gets or sets a value indicating whether the element is visible.</summary>
        public bool Visible { get; set; }
    }

    /// <summary>
    ///     Demonstrates building an <see cref="ApiSchema"/> using the provided configuration types.
    /// </summary>
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
