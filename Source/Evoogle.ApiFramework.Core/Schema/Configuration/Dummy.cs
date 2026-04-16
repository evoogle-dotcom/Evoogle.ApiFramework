// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

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
    ///     Represents a country domain model that is used by the sample schema builder extensions.
    /// </summary>
    public class Country
    {
        /// <summary>Gets or sets the ISO country code.</summary>
        public string Code { get; set; } = "us";
    }

    /// <summary>
    ///     Represents a customer domain model that is used by the sample schema builder extensions.
    /// </summary>
    public class Customer
    {
        /// <summary>Gets or sets the unique customer identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the customer's country.</summary>
        public Country Country { get; set; } = new Country();

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

    /// <summary>
    ///     Represents an order line-item domain model used by the sample schema builder extensions.
    /// </summary>
    public class OrderItem
    {
        /// <summary>Gets or sets the identifier of the order that owns this line item.</summary>
        public Guid OrderId { get; set; }

        /// <summary>Gets or sets the sequential line-item number within its parent order.</summary>
        public long LineItemNumber { get; set; }

        /// <summary>Gets or sets the display name of the product on this line item.</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>Gets or sets the quantity of units ordered for this line item.</summary>
        public int Quantity { get; set; }

        /// <summary>Gets or sets the per-unit price for this line item.</summary>
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    ///     Represents a nested customer reference — used to demonstrate FK key paths stored inside a complex property.
    /// </summary>
    public class CustomerRef
    {
        /// <summary>Gets or sets the identifier of the referenced customer.</summary>
        public Guid CustomerId { get; set; }
    }

    /// <summary>
    ///     Represents a customer profile domain model — a 1:1 extension of <see cref="Customer"/>.
    ///     The FK back to <see cref="Customer"/> is stored inside the nested <see cref="CustomerRef"/> property.
    /// </summary>
    public class CustomerProfile
    {
        /// <summary>Gets or sets the nested customer reference that carries the FK.</summary>
        public CustomerRef CustomerRef { get; set; } = new CustomerRef();

        /// <summary>Gets or sets the customer biography text.</summary>
        public string Biography { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Represents a product domain model used by the many-to-many sample schema.
    /// </summary>
    public class Product
    {
        /// <summary>Gets or sets the unique product identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the product display name.</summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Represents a tag domain model used by the many-to-many sample schema.
    /// </summary>
    public class Tag
    {
        /// <summary>Gets or sets the unique tag identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the tag label.</summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Represents the association type that mediates the many-to-many relationship between
    ///     <see cref="Product"/> and <see cref="Tag"/>.
    /// </summary>
    public class ProductTag
    {
        /// <summary>Gets or sets the ID of the associated product.</summary>
        public Guid ProductId { get; set; }

        /// <summary>Gets or sets the ID of the associated tag.</summary>
        public Guid TagId { get; set; }
    }

    /// <summary>
    ///     Defines the states that an order can be in for the sample schema configuration.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>The order has been placed but not yet processed or shipped.</summary>
        Pending,

        /// <summary>The order has been dispatched and is in transit.</summary>
        Shipped,

        /// <summary>The order has been received by the customer.</summary>
        Delivered,

        /// <summary>The order was cancelled before or after shipment.</summary>
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
                .AddScalarExtension(new VisibleMetadata { Visible = true });
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
                .AddEnumExtension(new VisibleMetadata { Visible = true });
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
                .AddObjectExtension(new VisibleMetadata { Visible = true });
        }
    }

    /// <summary>
    ///     Demonstrates how to configure a 1:M relationship using the fluent schema builder APIs.
    ///     Models the "a Customer has zero-or-more Orders" relationship.
    /// </summary>
    public class CustomerHasOrdersRelationshipConfiguration : IApiRelationshipOneToManyConfiguration
    {
        /// <inheritdoc />
        public void Configure(ApiRelationshipOneToManyBuilder builder)
        {
            builder
                .WithPrincipalEnd<Customer>
                (
                    p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                )
                .WithDependentEnd<Order>
                (
                    d => d.AddScalarPath("CustomerId")
                );
        }
    }

    /// <summary>
    ///     Demonstrates how to configure a 1:M relationship using the fluent schema builder APIs.
    ///     Models the "an Order has one-or-more OrderItems" relationship.
    /// </summary>
    public class OrderHasOrderItemsRelationshipConfiguration : IApiRelationshipOneToManyConfiguration
    {
        /// <inheritdoc />
        public void Configure(ApiRelationshipOneToManyBuilder builder)
        {
            builder
                .WithPrincipalEnd<Order>
                (
                    p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                )
                .WithDependentEnd<OrderItem>
                (
                    d => d
                        .AddScalarPath("OrderId")
                        .AddScalarPath("LineItemNumber")
                );
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
    ///     Demonstrates how to configure a 1:1 relationship using the fluent schema builder APIs.
    ///     Models the "a Customer has at most one CustomerProfile" relationship.
    ///     Demonstrates <see cref="ApiRelationshipDependentEndBuilder.AddNestedPath"/> for FK values
    ///     that are stored inside a complex/nested property on the dependent type.
    /// </summary>
    public class CustomerHasProfileRelationshipConfiguration : IApiRelationshipOneToOneConfiguration
    {
        /// <inheritdoc />
        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
            builder
                .WithPrincipalEnd<Customer>
                (
                    p => p
                        .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                        .AddPrincipalEndExtension(new VisibleMetadata { Visible = true })
                )
                .WithDependentEnd<CustomerProfile>
                (
                    // FK is NOT a direct scalar — it lives inside the nested CustomerRef property.
                    d => d.AddNestedPath("CustomerRef", n => n.AddScalarPath("CustomerId"))
                );
        }
    }

    /// <summary>
    ///     Demonstrates how to configure a M:N relationship using the fluent schema builder APIs.
    ///     Models the "Products are tagged with Tags" relationship via the <see cref="ProductTag"/> association type.
    ///     Demonstrates <see cref="ApiRelationshipPrincipalEndBuilder.WithIdentityName"/> to select a
    ///     non-primary identity on the principal side, and extensions on principal ends.
    /// </summary>
    public class ProductTagRelationshipConfiguration : IApiRelationshipManyToManyConfiguration<ProductTag>
    {
        /// <inheritdoc />
        public void Configure(ApiRelationshipManyToManyBuilder<ProductTag> builder)
        {
            builder
                .WithPrincipalEndA<Product>
                (
                    p => p
                        .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                        .AddPrincipalEndExtension(new VisibleMetadata { Visible = true })
                )
                .WithPrincipalEndB<Tag>
                (
                    p => p
                        .WithIdentityName("PrimaryKey")
                        .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                )
                .WithDependentEndA
                (
                    d => d.AddScalarPath("ProductId")
                )
                .WithDependentEndB
                (
                    d => d.AddScalarPath("TagId")
                );
        }
    }

    public class CustomerConfigurationGeneric : IApiObjectTypeConfiguration<Customer>
    {
        public void Configure(ApiObjectTypeBuilder<Customer> builder)
        {
            builder
                .WithName("Customer")
                .WithOptions(o => o.ThrowOnNull())
                .AddIdentity("PrimaryKey", b => b
                    .AddScalarPart(c => c.Id))
                .AddIdentity("AlternateKey", b => b
                    .AddNestedPart(c => c.Country)
                    .AddScalarPart(c => c.Name))
                .AddProperty(c => c.Id, p => p.WithModifiers(m => m.Required()))
                .AddProperty(c => c.Name, p => p.WithModifiers(m => m.Required()))
                .AddProperty(c => c.Email, p => p.WithModifiers(m => m.Optional()))
                .AddProperty(c => c.Orders, p => p.WithModifiers(m => m.Required()));
        }
    }

    public class CustomerHasOrdersConfigurationGeneric : IApiRelationshipOneToManyConfiguration
    {
        public void Configure(ApiRelationshipOneToManyBuilder builder)
        {
            builder
                .WithPrincipalEnd<Customer>(p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                .WithDependentEnd<Order>(d => d.AddScalarPath(o => o.CustomerId));
        }
    }

    public class CustomerHasProfileConfigurationGeneric : IApiRelationshipOneToOneConfiguration
    {
        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
            builder
                .WithPrincipalEnd<Customer>(p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                .WithDependentEnd<CustomerProfile>(d => d
                    .AddNestedPath(cp => cp.CustomerRef, n => n
                        .AddScalarPath(r => r.CustomerId)));
        }
    }

    public class ProductTagConfigurationGeneric : IApiRelationshipManyToManyConfiguration<ProductTag>
    {
        public void Configure(ApiRelationshipManyToManyBuilder<ProductTag> builder)
        {
            builder
                .WithPrincipalEndA<Product>(p => p
                    .WithIdentityName("PrimaryKey")
                    .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                .WithPrincipalEndB<Tag>(p => p
                    .WithIdentityName("PrimaryKey")
                    .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                .WithDependentEndA(d => d.AddScalarPath(pt => pt.ProductId))
                .WithDependentEndB(d => d.AddScalarPath(pt => pt.TagId));
        }
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
                .ReturnEmptyOnNull())
            .AddScalar(typeof(EmailAddress), x => x
                .WithName("EmailAddress"))
                .AddSchemaExtension(new VisibleMetadata { Visible = true })
            .AddObject(typeof(Country), x => x
                .WithName("Country")
                .AddIdentity("PrimaryKey", identity => identity
                    .AddScalarPart("Code")))
            .AddObject(typeof(Customer), x => x
                .WithName("Customer")
                .WithOptions(o => o
                    .ThrowOnNull())
                .AddIdentity("PrimaryKey", identity => identity
                    .AddScalarPart("Id"))
                .AddIdentity("AlternateKey", identity => identity
                    .AddNestedPart("Country")
                    .AddScalarPart("Name"))
                .AddProperty("Id", "Id", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Name", "Name", p => p.WithModifiers(m => m.Required()).AddPropertyExtension(new VisibleMetadata { Visible = true }))
                .AddProperty("Email", "Email", p => p.WithModifiers(m => m.Optional()))
                .AddProperty("Orders", "Orders", p => p.WithModifiers(m => m.Required())))
                .AddSchemaExtension(new VisibleMetadata { Visible = true })
            .AddObject(typeof(Order), x => x
                .WithName("Order")
                .WithDefaultOptions()
                .AddProperty("Id", "Id", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Status", "Status", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Total", "Total", p => p.WithModifiers(m => m.Required()))
                .AddProperty("CustomerId", "CustomerId", p => p.WithModifiers(m => m.Optional()))
                .AddProperty("Customer", "Customer", p => p.WithModifiers(m => m.Optional())))
            .AddObject(typeof(OrderItem), x => x
                .WithName("OrderItem")
                .WithDefaultOptions()
                .AddIdentity("PrimaryKey", identity => identity
                    .AddOwnerPart()
                    .AddScalarPart("OrderId", x => x.AddIdentityPartExtension(new VisibleMetadata { Visible = true }))
                    .AddScalarPart("LineItemNumber", typeof(long)))
                .AddProperty("OrderId", "OrderId", p => p.WithModifiers(m => m.Required()))
                .AddProperty("LineItemNumber", "LineItemNumber", p => p.WithModifiers(m => m.Required()))
                .AddProperty("ProductName", "ProductName", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Quantity", "Quantity", p => p.WithModifiers(m => m.Required()))
                .AddProperty("UnitPrice", "UnitPrice", p => p.WithModifiers(m => m.Required())))
            .AddEnum(typeof(OrderStatus), x => x
                .AddEnumExtension(new VisibleMetadata { Visible = true })
                .WithName("OrderStatus")
                .AddValue("Pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1)
                .AddValue("Delivered", "Delivered", 2)
                .AddValue("Cancelled", "Cancelled", 3))
            .AddScalar(typeof(EmailAddress), new EmailAddressConfiguration())
            .AddEnum(typeof(OrderStatus), new OrderStatusConfiguration())
            .AddObject(typeof(Order), new OrderConfiguration())
            .AddObject(typeof(CustomerProfile), x => x
                .WithName("CustomerProfile")
                .AddIdentity("PrimaryKey", identity => identity
                    .AddNestedPart("CustomerRef")
                    .AddScalarPart("CustomerId"))
                .AddProperty("Biography", "Biography", p => p.WithModifiers(m => m.Required())))
            .AddObject(typeof(Product), x => x
                .WithName("Product")
                .AddIdentity("PrimaryKey", identity => identity
                    .AddScalarPart("Id"))
                .AddProperty("Id", "Id", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Name", "Name", p => p.WithModifiers(m => m.Required())))
            .AddObject(typeof(Tag), x => x
                .WithName("Tag")
                .AddIdentity("PrimaryKey", identity => identity
                    .AddScalarPart("Id"))
                .AddProperty("Id", "Id", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Name", "Name", p => p.WithModifiers(m => m.Required())))
            .AddObject(typeof(ProductTag), x => x
                .WithName("ProductTag")
                .AddIdentity("PrimaryKey", identity => identity
                    .AddScalarPart("ProductId")
                    .AddScalarPart("TagId"))
                .AddProperty("ProductId", "ProductId", p => p.WithModifiers(m => m.Required()))
                .AddProperty("TagId", "TagId", p => p.WithModifiers(m => m.Required())))
            .AddOneToManyRelationship("CustomerHasOrders", new CustomerHasOrdersRelationshipConfiguration())
            .AddOneToManyRelationship("OrderHasOrderItems", new OrderHasOrderItemsRelationshipConfiguration())
            .AddOneToManyRelationship
            (
                "CustomerOrders",
                r => r
                    .WithPrincipalEnd<Customer>
                    (
                        p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                    )
                    .WithDependentEnd<Order>
                    (
                        d => d.AddScalarPath("CustomerId")
                    )
            )
            // 1:M — inline lambda; demonstrates AddOwnerPath for a dependent type whose FK
            //        is expressed through the owner reference in its composite identity.
            .AddOneToManyRelationship
            (
                "OrderOrderItemsViaOwner",
                r => r
                    .WithPrincipalEnd<Order>
                    (
                        p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                    )
                    .WithDependentEnd<OrderItem>
                    (
                        d => d.AddOwnerPath()
                    )
            )
            // 1:1 — configuration class style; demonstrates AddNestedPath on the dependent end.
            .AddOneToOneRelationship("CustomerHasProfile", new CustomerHasProfileRelationshipConfiguration())
            // 1:1 — inline lambda style.
            .AddOneToOneRelationship
            (
                "CustomerHasProfileInline",
                r => r
                    .WithPrincipalEnd<Customer>
                    (
                        p => p
                            .WithIdentityName("PrimaryKey")
                            .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                    )
                    .WithDependentEnd<CustomerProfile>
                    (
                        d => d.AddNestedPath("CustomerRef", n => n.AddScalarPath("CustomerId"))
                    )
            )
            // M:N — configuration class style; demonstrates WithIdentityName and extensions on principal ends.
            .AddManyToManyRelationship<ProductTag>("ProductHasTags", new ProductTagRelationshipConfiguration())
            // M:N — inline lambda style; demonstrates all four end methods and relationship-level extensions.
            .AddManyToManyRelationship<ProductTag>
            (
                "ProductHasTagsInline",
                r => r
                    .WithPrincipalEndA<Product>
                    (
                        p => p
                            .WithIdentityName("PrimaryKey")
                            .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                            .AddPrincipalEndExtension(new VisibleMetadata { Visible = true })
                    )
                    .WithPrincipalEndB<Tag>
                    (
                        p => p
                            .WithIdentityName("PrimaryKey")
                            .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade)
                    )
                    .WithDependentEndA
                    (
                        d => d
                            .AddScalarPath("ProductId")
                            .AddDependentEndExtension(new VisibleMetadata { Visible = true })
                    )
                    .WithDependentEndB
                    (
                        d => d.AddScalarPath("TagId")
                    )
                    .AddRelationshipExtension(new VisibleMetadata { Visible = true })
            )
            .AddSchemaExtension(new VisibleMetadata { Visible = true })
            .Build();
    }

    /// <summary>
    ///     Builds the same schema as <see cref="DummyMethod"/> using the generic, expression-based fluent builder APIs.
    /// </summary>
    public static void DummyMethodGeneric()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("CustomerOrdersAPI")
            .WithVersion("v1")
            .AddScalar(typeof(EmailAddress), x => x.WithName("EmailAddress"))
            .AddScalar(typeof(EmailAddress))
            .AddEnum(typeof(OrderStatus), x => x
                .WithName("OrderStatus")
                .AddValue("Pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1)
                .AddValue("Delivered", "Delivered", 2)
                .AddValue("Cancelled", "Cancelled", 3))
            .AddObject<Country>(x => x
                .WithName("Country")
                .AddIdentity("PrimaryKey", b => b.AddScalarPart(c => c.Code))
                .AddProperty(c => c.Code))
            .AddObject(new CustomerConfigurationGeneric())
            .AddObject<Order>(x => x
                .WithName("Order")
                .AddIdentity("PrimaryKey", b => b.AddScalarPart(o => o.Id))
                .AddProperty(o => o.Id, p => p.WithModifiers(m => m.Required()))
                .AddProperty(o => o.Status, p => p.WithModifiers(m => m.Required()))
                .AddProperty(o => o.Total, p => p.WithModifiers(m => m.Required()))
                .AddProperty(o => o.CustomerId, p => p.WithModifiers(m => m.Optional()))
                .AddProperty(o => o.Customer, p => p.WithModifiers(m => m.Optional())))
            .AddObject<OrderItem>(x => x
                .WithName("OrderItem")
                .AddIdentity("PrimaryKey", b => b
                    .AddOwnerPart()
                    .AddScalarPart(oi => oi.OrderId)
                    .AddScalarPart(oi => oi.LineItemNumber, typeof(long)))
                .AddProperty(oi => oi.OrderId, p => p.WithModifiers(m => m.Required()))
                .AddProperty(oi => oi.LineItemNumber, p => p.WithModifiers(m => m.Required()))
                .AddProperty(oi => oi.ProductName, p => p.WithModifiers(m => m.Required()))
                .AddProperty(oi => oi.Quantity, p => p.WithModifiers(m => m.Required()))
                .AddProperty(oi => oi.UnitPrice, p => p.WithModifiers(m => m.Required())))
            .AddObject<CustomerProfile>(x => x
                .WithName("CustomerProfile")
                .AddIdentity("PrimaryKey", b => b.AddNestedPart(cp => cp.CustomerRef))
                .AddProperty(cp => cp.CustomerRef, p => p.WithModifiers(m => m.Required()))
                .AddProperty(cp => cp.Biography, p => p.WithModifiers(m => m.Required())))
            .AddObject<Product>(x => x
                .WithName("Product")
                .AddIdentity("PrimaryKey", b => b.AddScalarPart(p => p.Id))
                .AddProperty(p => p.Id, cfg => cfg.WithModifiers(m => m.Required()))
                .AddProperty(p => p.Name, cfg => cfg.WithModifiers(m => m.Required())))
            .AddObject<Tag>(x => x
                .WithName("Tag")
                .AddIdentity("PrimaryKey", b => b.AddScalarPart(t => t.Id))
                .AddProperty(t => t.Id, p => p.WithModifiers(m => m.Required()))
                .AddProperty(t => t.Name, p => p.WithModifiers(m => m.Required())))
            .AddObject<ProductTag>(x => x
                .WithName("ProductTag")
                .AddIdentity("PrimaryKey", b => b
                    .AddScalarPart(pt => pt.ProductId)
                    .AddScalarPart(pt => pt.TagId))
                .AddProperty(pt => pt.ProductId, p => p.WithModifiers(m => m.Required()))
                .AddProperty(pt => pt.TagId, p => p.WithModifiers(m => m.Required())))
            .AddOneToManyRelationship("CustomerHasOrders", new CustomerHasOrdersConfigurationGeneric())
            .AddOneToManyRelationship("OrderHasOrderItems", r => r
                .WithPrincipalEnd<Order>(p => p.WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                .WithDependentEnd<OrderItem>(d => d
                    .AddScalarPath(oi => oi.OrderId)
                    .AddScalarPath(oi => oi.LineItemNumber)))
            .AddOneToOneRelationship("CustomerHasProfile", new CustomerHasProfileConfigurationGeneric())
            .AddOneToOneRelationship("CustomerHasProfileInline", r => r
                .WithPrincipalEnd<Customer>(p => p
                    .WithIdentityName("PrimaryKey")
                    .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                .WithDependentEnd<CustomerProfile>(d => d
                    .AddNestedPath(cp => cp.CustomerRef, n => n
                        .AddScalarPath(r => r.CustomerId))))
            .AddManyToManyRelationship<ProductTag>("ProductHasTags", new ProductTagConfigurationGeneric())
            .AddManyToManyRelationship<ProductTag>("ProductHasTagsInline", r => r
                .WithPrincipalEndA<Product>(p => p
                    .WithIdentityName("PrimaryKey")
                    .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                .WithPrincipalEndB<Tag>(p => p
                    .WithIdentityName("PrimaryKey")
                    .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Cascade))
                .WithDependentEndA(d => d.AddScalarPath(pt => pt.ProductId))
                .WithDependentEndB(d => d.AddScalarPath(pt => pt.TagId)))
            .Build();
    }
}
