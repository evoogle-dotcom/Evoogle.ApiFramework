// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
    ///     Represents a nested customer reference used to demonstrate key paths stored inside a complex property.
    /// </summary>
    public class CustomerRef
    {
        /// <summary>Gets or sets the identifier of the referenced customer.</summary>
        public Guid CustomerId { get; set; }
    }

    /// <summary>
    ///     Represents a customer profile domain model — a 1:1 extension of <see cref="Customer"/>.
    ///     The key value back to <see cref="Customer"/> is stored inside the nested <see cref="CustomerRef"/> property.
    /// </summary>
    public class CustomerProfile
    {
        /// <summary>Gets or sets the nested customer reference that carries the key value.</summary>
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
                .AddScalarTypeExtension(new VisibleMetadata { Visible = true });
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
                .AddEnumTypeExtension(new VisibleMetadata { Visible = true });
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
                .AddObjectTypeExtension(new VisibleMetadata { Visible = true });
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
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                .WithPrincipalEnd<Customer>()
                .WithDependentEnd<Order>
                (
                    d => d.WithForeignKeyType("FK", b => b.AddKeyPath(typeof(Order), "CustomerId"))
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
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                .WithPrincipalEnd(typeof(Order))
                .WithDependentEnd(typeof(OrderItem),
                (
                    d => d.WithForeignKeyType("FK", b => b
                        .AddKeyPath(typeof(OrderItem), "OrderId")
                        .AddKeyPath(typeof(OrderItem), "LineItemNumber"))
                ));
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
    ///     Demonstrates nested <see cref="ApiKeyPath"/> values stored inside a complex property on the dependent type.
    /// </summary>
    public class CustomerHasProfileRelationshipConfiguration : IApiRelationshipOneToOneConfiguration
    {
        /// <inheritdoc />
        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
            builder
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                .WithPrincipalEnd<Customer>
                (
                    p => p
                        .AddRelationshipPrincipalEndExtension(new VisibleMetadata { Visible = true })
                )
                .WithDependentEnd<CustomerProfile>
                (
                    // The key value is not a direct scalar; it lives inside the nested CustomerRef property.
                    d => d.WithForeignKeyType("FK", b => b.AddKeyPath(typeof(CustomerProfile), "CustomerRef", "CustomerId"))
                );
        }
    }

    /// <summary>
    ///     Demonstrates how to configure a M:N relationship using the fluent schema builder APIs.
    ///     Models the "Products are tagged with Tags" relationship via the <see cref="ProductTag"/> association type.
    ///     Demonstrates <see cref="ApiRelationshipPrincipalEndBuilder.WithKeyTypeName"/> to select a
    ///     non-primary key type on the principal side, and extensions on principal ends.
    /// </summary>
    public class ProductTagRelationshipConfiguration : IApiRelationshipManyToManyConfiguration
    {
        /// <inheritdoc />
        public void Configure(ApiRelationshipManyToManyBuilder builder)
        {
            builder
                .WithPrincipalEndA<Product>
                (
                    p => p.AddRelationshipPrincipalEndExtension(new VisibleMetadata { Visible = true })
                )
                .WithPrincipalEndB<Tag>
                (
                    p => p.WithKeyTypeName("PrimaryKey")
                )
                .WithAssociation<ProductTag>
                (
                    a => a
                        .WithForeignKeyTypeA("FKA", b => b.AddKeyPath(typeof(ProductTag), "ProductId"))
                        .WithForeignKeyTypeB("FKB", b => b.AddKeyPath(typeof(ProductTag), "TagId"))
                );
        }
    }

    public class CustomerConfigurationGeneric : IApiObjectTypeConfiguration<Customer>
    {
        public void Configure(ApiObjectTypeBuilder<Customer> builder)
        {
            builder
                .WithName("Customer")
                .WithOptions(o => o.ThrowOnNullKeyPart())
                .AddRequiredProperty(c => c.Id)
                .AddRequiredProperty(c => c.Name)
                .AddOptionalProperty(c => c.Email)
                .AddRequiredProperty(c => c.Orders)
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(c => c.Id))
                .AddKeyType("AlternateKey", b => b
                    .AddKeyPath(c => c.Country.Code));
        }
    }

    public class CustomerHasOrdersConfigurationGeneric : IApiRelationshipOneToManyConfiguration
    {
        public void Configure(ApiRelationshipOneToManyBuilder builder)
        {
            builder
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                .WithPrincipalEnd<Customer>()
                .WithDependentEnd<Order>(d => d.WithForeignKeyType("FK", b => b.AddKeyPath(o => o.CustomerId)));
        }
    }

    public class CustomerHasProfileConfigurationGeneric : IApiRelationshipOneToOneConfiguration
    {
        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
            builder
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                .WithPrincipalEnd<Customer>()
                .WithDependentEnd<CustomerProfile>(d => d
                    .WithForeignKeyType("FK", b => b.AddKeyPath(cp => cp.CustomerRef.CustomerId)));
        }
    }

    public class ProductTagConfigurationGeneric : IApiRelationshipManyToManyConfiguration
    {
        public void Configure(ApiRelationshipManyToManyBuilder builder)
        {
            builder
                .WithPrincipalEndA<Product>(p => p.WithKeyTypeName("PrimaryKey"))
                .WithPrincipalEndB<Tag>(p => p.WithKeyTypeName("PrimaryKey"))
                .WithAssociation<ProductTag>
                (
                    a => a
                        .WithForeignKeyTypeA("FKA", b => b.AddKeyPath(p => p.ProductId))
                        .WithForeignKeyTypeB("FKB", b => b.AddKeyPath(p => p.TagId))
                );
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
                .UseDefaultOnNullKeyPart()
                .ThrowOnNullKeyPart())
            .AddScalar(typeof(EmailAddress), x => x
                .WithName("EmailAddress"))
                .AddSchemaExtension(new VisibleMetadata { Visible = true })
            .AddObject(typeof(Country), x => x
                .WithName("Country")
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(typeof(Country), "Code")))
            .AddObject(typeof(Customer), x => x
                .WithName("Customer")
                .WithOptions(o => o
                    .ThrowOnNullKeyPart())
                .AddProperty("Id", "Id", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Name", "Name", p => p.WithModifiers(m => m.Required()).AddPropertyExtension(new VisibleMetadata { Visible = true }))
                .AddProperty("Email", "Email", p => p.WithModifiers(m => m.Optional()))
                .AddProperty("Orders", "Orders", p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(typeof(Customer), "Id"))
                .AddKeyType("AlternateKey", b => b
                    .AddKeyPath(typeof(Customer), "Country", "Code")))
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
                .AddProperty("OrderId", "OrderId", p => p.WithModifiers(m => m.Required()))
                .AddProperty("LineItemNumber", "LineItemNumber", p => p.WithModifiers(m => m.Required()))
                .AddProperty("ProductName", "ProductName", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Quantity", "Quantity", p => p.WithModifiers(m => m.Required()))
                .AddProperty("UnitPrice", "UnitPrice", p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(typeof(OrderItem), ["OrderId"], p => p.AddKeyPathExtension(new VisibleMetadata { Visible = true }))
                    .AddKeyPath(typeof(OrderItem), "LineItemNumber"))
                .AddKeyType("AlternateKey", b => b
                    .AddKeyPath(typeof(Order), ["Id"], p => p.AddKeyPathExtension(new VisibleMetadata { Visible = true }))
                    .AddKeyPath(typeof(OrderItem), "LineItemNumber")))
            .AddEnum(typeof(OrderStatus), x => x
                .AddEnumTypeExtension(new VisibleMetadata { Visible = true })
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
                .AddProperty("Biography", "Biography", p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(typeof(CustomerProfile), "CustomerRef", "CustomerId")))
            .AddObject(typeof(Product), x => x
                .WithName("Product")
                .AddProperty("Id", "Id", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Name", "Name", p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(typeof(Product), "Id")))
            .AddObject(typeof(Tag), x => x
                .WithName("Tag")
                .AddProperty("Id", "Id", p => p.WithModifiers(m => m.Required()))
                .AddProperty("Name", "Name", p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(typeof(Tag), "Id")))
            .AddObject(typeof(ProductTag), x => x
                .WithName("ProductTag")
                .AddProperty("ProductId", "ProductId", p => p.WithModifiers(m => m.Required()))
                .AddProperty("TagId", "TagId", p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(typeof(ProductTag), "ProductId")
                    .AddKeyPath(typeof(ProductTag), "TagId")))
            .AddOneToManyRelationship("CustomerHasOrders", new CustomerHasOrdersRelationshipConfiguration())
            .AddOneToManyRelationship("OrderHasOrderItems", new OrderHasOrderItemsRelationshipConfiguration())
            .AddOneToManyRelationship
            (
                "CustomerOrders",
                r => r
                    .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                    .WithPrincipalEnd<Customer>()
                    .WithDependentEnd<Order>
                    (
                        d => d.WithForeignKeyType("FK", b => b.AddKeyPath(typeof(Order), "CustomerId"))
                    )
            )
            // 1:M — inline lambda; demonstrates WithForeignKeyType for a composite key role.
            .AddOneToManyRelationship
            (
                "OrderOrderItemsViaOwner",
                r => r
                    .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                    .WithPrincipalEnd<Order>()
                    .WithDependentEnd<OrderItem>
                    (
                        d => d.WithForeignKeyType("FK", b => b
                            .AddKeyPath(typeof(OrderItem), "OrderId")
                            .AddKeyPath(typeof(OrderItem), "LineItemNumber"))
                    )
            )
            // 1:1 — configuration class style; demonstrates AddNestedPath on the dependent end.
            .AddOneToOneRelationship("CustomerHasProfile", new CustomerHasProfileRelationshipConfiguration())
            // 1:1 — inline lambda style.
            .AddOneToOneRelationship
            (
                "CustomerHasProfileInline",
                r => r
                    .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                    .WithPrincipalEnd<Customer>
                    (
                        p => p
                            .WithKeyTypeName("PrimaryKey")
                    )
                    .WithDependentEnd<CustomerProfile>
                    (
                        d => d.WithForeignKeyType("FK", b => b.AddKeyPath(typeof(CustomerProfile), "CustomerRef", "CustomerId"))
                    )
            )
            // M:N — configuration class style; demonstrates WithKeyTypeName and extensions on principal ends.
            .AddManyToManyRelationship("ProductHasTags", new ProductTagRelationshipConfiguration())
            // M:N — inline lambda style; demonstrates all four end methods and relationship-level extensions.
            .AddManyToManyRelationship
            (
                "ProductHasTagsInline",
                r => r
                    .WithPrincipalEndA<Product>
                    (
                        p => p
                            .WithKeyTypeName("PrimaryKey")
                            .AddRelationshipPrincipalEndExtension(new VisibleMetadata { Visible = true })
                    )
                    .WithPrincipalEndB<Tag>
                    (
                        p => p
                            .WithKeyTypeName("PrimaryKey")
                    )
                    .WithAssociation<ProductTag>
                    (
                        a => a
                            .WithForeignKeyTypeA("FKA", b => b.AddKeyPath(typeof(ProductTag), "ProductId"))
                            .WithForeignKeyTypeB("FKB", b => b.AddKeyPath(typeof(ProductTag), "TagId"))
                            .AddRelationshipAssociationExtension(new VisibleMetadata { Visible = true })
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
                .AddProperty(c => c.Code)
                .AddKeyType("PrimaryKey", b => b.AddKeyPath(c => c.Code)))
            .AddObject(new CustomerConfigurationGeneric())
            .AddObject<Order>(x => x
                .WithName("Order")
                .AddProperty(o => o.Id, p => p.WithModifiers(m => m.Required()))
                .AddProperty(o => o.Status, p => p.WithModifiers(m => m.Required()))
                .AddProperty(o => o.Total, p => p.WithModifiers(m => m.Required()))
                .AddProperty(o => o.CustomerId, p => p.WithModifiers(m => m.Optional()))
                .AddProperty(o => o.Customer, p => p.WithModifiers(m => m.Optional()))
                .AddKeyType("PrimaryKey", b => b.AddKeyPath(o => o.Id)))
            .AddObject<OrderItem>(x => x
                .WithName("OrderItem")
                .AddProperty(oi => oi.OrderId, p => p.WithModifiers(m => m.Required()))
                .AddProperty(oi => oi.LineItemNumber, p => p.WithModifiers(m => m.Required()))
                .AddProperty(oi => oi.ProductName, p => p.WithModifiers(m => m.Required()))
                .AddProperty(oi => oi.Quantity, p => p.WithModifiers(m => m.Required()))
                .AddProperty(oi => oi.UnitPrice, p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(oi => oi.OrderId)
                    .AddKeyPath(oi => oi.LineItemNumber))
                .AddKeyType("AlternateKey", b => b
                    .AddKeyPath<Order, Guid>(o => o.Id)
                    .AddKeyPath(oi => oi.LineItemNumber)))
            .AddObject<CustomerProfile>(x => x
                .WithName("CustomerProfile")
                .AddProperty(cp => cp.CustomerRef, p => p.WithModifiers(m => m.Required()))
                .AddProperty(cp => cp.Biography, p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b.AddKeyPath(cp => cp.CustomerRef.CustomerId)))
            .AddObject<Product>(x => x
                .WithName("Product")
                .AddProperty(p => p.Id, cfg => cfg.WithModifiers(m => m.Required()))
                .AddProperty(p => p.Name, cfg => cfg.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b.AddKeyPath(p => p.Id)))
            .AddObject<Tag>(x => x
                .WithName("Tag")
                .AddProperty(t => t.Id, p => p.WithModifiers(m => m.Required()))
                .AddProperty(t => t.Name, p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b.AddKeyPath(t => t.Id)))
            .AddObject<ProductTag>(x => x
                .WithName("ProductTag")
                .AddProperty(pt => pt.ProductId, p => p.WithModifiers(m => m.Required()))
                .AddProperty(pt => pt.TagId, p => p.WithModifiers(m => m.Required()))
                .AddKeyType("PrimaryKey", b => b
                    .AddKeyPath(pt => pt.ProductId)
                    .AddKeyPath(pt => pt.TagId)))
            .AddOneToManyRelationship("CustomerHasOrders", new CustomerHasOrdersConfigurationGeneric())
            .AddOneToManyRelationship("OrderHasOrderItems", r => r
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                .WithPrincipalEnd<Order>()
                .WithDependentEnd<OrderItem>(d => d
                    .WithForeignKeyType("FK", b => b
                        .AddKeyPath(oi => oi.OrderId)
                        .AddKeyPath(oi => oi.LineItemNumber))))
            .AddOneToOneRelationship("CustomerHasProfile", new CustomerHasProfileConfigurationGeneric())
            .AddOneToOneRelationship("CustomerHasProfileInline", r => r
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
                .WithPrincipalEnd<Customer>(p => p
                    .WithKeyTypeName("PrimaryKey"))
                .WithDependentEnd<CustomerProfile>(d => d
                    .WithForeignKeyType("FK", b => b.AddKeyPath(cp => cp.CustomerRef.CustomerId))))
            .AddManyToManyRelationship("ProductHasTags", new ProductTagConfigurationGeneric())
            .AddManyToManyRelationship("ProductHasTagsInline", r => r
                .WithPrincipalEndA<Product>(p => p
                    .WithKeyTypeName("PrimaryKey"))
                .WithPrincipalEndB<Tag>(p => p
                    .WithKeyTypeName("PrimaryKey"))
                .WithAssociation<ProductTag>(a => a
                    .WithForeignKeyTypeA("FKA", b => b.AddKeyPath(p => p.ProductId))
                    .WithForeignKeyTypeB("FKB", b => b.AddKeyPath(p => p.TagId))))
            .Build();
    }
}
