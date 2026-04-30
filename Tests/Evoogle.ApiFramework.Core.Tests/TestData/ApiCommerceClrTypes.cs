// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.TestData;

#region Enums
public enum OrderStatus { Pending, Paid, Shipped, Cancelled, Returned }
public enum PaymentMethod { Card, Cash, Wire, Crypto }
public enum CountryCode { US, CA, GB }

[Flags]
public enum UserRole { None = 0, Reader = 1, Editor = 2, Admin = 4 }
#endregion

#region Value Object Types
public sealed record Money(decimal Amount, string Currency);
public sealed record Quantity(decimal Value, string Unit);
public sealed record EmailAddress(string Value);

public sealed class Address
{
    public string Line1 { get; init; } = string.Empty;
    public string? Line2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string Postal { get; init; } = string.Empty;
    public CountryCode Country { get; init; } = CountryCode.US;

    public override string ToString()
    {
        var line1 = this.Line1.SafeToString();
        var line2 = this.Line2.SafeToString();
        var city = this.City.SafeToString();
        var state = this.State.SafeToString();
        var postal = this.Postal.SafeToString();
        var country = this.Country.SafeToString();

        return $"{nameof(Address)} {{{nameof(this.Line1)}={line1}, {nameof(this.Line2)}={line2}, {nameof(this.City)}={city}, {nameof(this.State)}={state}, {nameof(this.Postal)}={postal}, {nameof(this.Country)}={country}}}";
    }
}
#endregion

#region Customer Object Types
public sealed class Customer
{
    public Ulid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public EmailAddress Email { get; init; } = new("<unset>");
    public Address PrimaryAddress { get; init; } = new();
    public List<Address> Addresses { get; init; } = [];
    public List<Order> Orders { get; init; } = [];

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();
        var email = this.Email.SafeToString();
        var primaryAddress = this.PrimaryAddress.SafeToString();
        var addresses = this.Addresses.SafeToString();
        var orders = this.Orders.SafeToString();

        return $"{nameof(Customer)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}, {nameof(this.Email)}={email}, {nameof(this.PrimaryAddress)}={primaryAddress}, {nameof(this.Addresses)}={addresses}, {nameof(this.Orders)}={orders}}}";
    }
}
#endregion

#region Product Object Types
public sealed class Category
{
    public Ulid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Category? Parent { get; init; }
    public List<Category> Children { get; init; } = [];

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();
        var parent = this.Parent.SafeToString();
        var children = this.Children.SafeToString();

        return $"{nameof(Category)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}, {nameof(this.Parent)}={parent}, {nameof(this.Children)}={children}}}";
    }
}

public sealed class Tag
{
    public Ulid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    // public List<ProductBase> Products { get; init; } = [];

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();
        // var products = this.Products.SafeToString();

        return $"{nameof(Tag)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}}}";
    }
}

public abstract class ProductBase
{
    public Ulid Id { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Money Price { get; init; } = new(0, "USD");
    public List<Tag>? Tags { get; init; }
    public Category? Category { get; init; }
}

// public sealed class PhysicalProduct : ProductBase
public sealed class PhysicalProduct
{
    // ProductBase properties
    // Note: We have to redefine the base class properties because framework does not support inheritance in CLR types yet.
    public Ulid Id { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Money Price { get; init; } = new(0, "USD");
    public List<Tag>? Tags { get; init; }
    public Category? Category { get; init; }

    // PhysicalProduct properties
    public decimal Weight { get; init; }
    public Quantity? Size { get; init; }

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var sku = this.Sku.SafeToString();
        var name = this.Name.SafeToString();
        var price = this.Price.SafeToString();
        var tags = this.Tags.SafeToString();
        var category = this.Category.SafeToString();
        var weight = this.Weight.SafeToString();
        var size = this.Size.SafeToString();

        return $"{nameof(PhysicalProduct)} {{{nameof(this.Id)}={id}, {nameof(this.Sku)}={sku}, {nameof(this.Name)}={name}, {nameof(this.Price)}={price}, {nameof(this.Tags)}={tags}, {nameof(this.Category)}={category}, {nameof(this.Weight)}={weight}, {nameof(this.Size)}={size}}}";
    }
}

// public sealed class DigitalProduct : ProductBase
public sealed class DigitalProduct
{
    // ProductBase properties
    // Note: We have to redefine the base class properties because framework does not support inheritance in CLR types yet.
    public Ulid Id { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Money Price { get; init; } = new(0, "USD");
    public List<Tag>? Tags { get; init; }
    public Category? Category { get; init; }

    // DigitalProduct properties
    public Uri? DownloadUrl { get; init; }
    public long? Bytes { get; init; }

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var sku = this.Sku.SafeToString();
        var name = this.Name.SafeToString();
        var price = this.Price.SafeToString();
        var tags = this.Tags.SafeToString();
        var category = this.Category.SafeToString();
        var downloadUrl = this.DownloadUrl.SafeToString();
        var bytes = this.Bytes.SafeToString();

        return $"{nameof(DigitalProduct)} {{{nameof(this.Id)}={id}, {nameof(this.Sku)}={sku}, {nameof(this.Name)}={name}, {nameof(this.Price)}={price}, {nameof(this.Tags)}={tags}, {nameof(this.Category)}={category}, {nameof(this.DownloadUrl)}={downloadUrl}, {nameof(this.Bytes)}={bytes}}}";
    }
}
#endregion

#region Order/Payment Object Types
public sealed class Order
{
    public Ulid Id { get; init; }
    public Customer Customer { get; init; } = default!;
    public DateTimeOffset PlacedAt { get; init; }
    public OrderStatus Status { get; init; } = OrderStatus.Pending;
    public List<OrderLine> Lines { get; init; } = [];
    public Payment? Payment { get; init; }
    public Money Total { get; init; } = new(0, "USD");

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var customer = this.Customer.SafeToString();
        var placedAt = this.PlacedAt.SafeToString();
        var status = this.Status.SafeToString();
        var lines = this.Lines.SafeToString();
        var payment = this.Payment.SafeToString();
        var total = this.Total.SafeToString();

        return $"{nameof(Order)} {{{nameof(this.Id)}={id}, {nameof(this.Customer)}={customer}, {nameof(this.PlacedAt)}={placedAt}, {nameof(this.Status)}={status}, {nameof(this.Lines)}={lines}, {nameof(this.Payment)}={payment}, {nameof(this.Total)}={total}}}";
    }
}

public sealed class OrderLine
{
    public Ulid OrderId { get; init; }
    public int LineNumber { get; init; }
    // public ProductBase Product { get; init; } = default!;
    public Quantity Qty { get; init; } = new(1, "ea");
    public Money UnitPrice { get; init; } = new(0, "USD");
    public Money LineTotal { get; init; } = new(0, "USD");

    public override string ToString()
    {
        var orderId = this.OrderId.SafeToString();
        var lineNumber = this.LineNumber.SafeToString();
        // var product = this.Product.SafeToString();
        var qty = this.Qty.SafeToString();
        var unitPrice = this.UnitPrice.SafeToString();
        var lineTotal = this.LineTotal.SafeToString();

        return $"{nameof(OrderLine)} {{{nameof(this.OrderId)}={orderId}, {nameof(this.LineNumber)}={lineNumber}, {nameof(this.Qty)}={qty}, {nameof(this.UnitPrice)}={unitPrice}, {nameof(this.LineTotal)}={lineTotal}}}";
    }
}

public sealed class Payment
{
    public Ulid Id { get; init; }
    public PaymentMethod Method { get; init; }
    public Money Amount { get; init; } = new(0, "USD");
    public DateTimeOffset? CapturedAt { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = [];

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var method = this.Method.SafeToString();
        var amount = this.Amount.SafeToString();
        var capturedAt = this.CapturedAt.SafeToString();
        var metadata = this.Metadata.SafeToString();

        return $"{nameof(Payment)} {{{nameof(this.Id)}={id}, {nameof(this.Method)}={method}, {nameof(this.Amount)}={amount}, {nameof(this.CapturedAt)}={capturedAt}, {nameof(this.Metadata)}={metadata}}}";
    }
}
#endregion
