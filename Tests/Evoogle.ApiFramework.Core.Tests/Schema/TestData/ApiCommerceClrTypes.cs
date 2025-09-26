// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.TestData;

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
}
#endregion

#region Product Object Types
public sealed class Category
{
    public Ulid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Category? Parent { get; init; }
    public List<Category> Children { get; init; } = [];
}

public sealed class Tag
{
    public Ulid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public List<ProductBase> Products { get; init; } = [];
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

public sealed class PhysicalProduct : ProductBase
{
    public decimal Weight { get; init; }
    public Quantity? Size { get; init; }
}

public sealed class DigitalProduct : ProductBase
{
    public Uri? DownloadUrl { get; init; }
    public long? Bytes { get; init; }
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
}

public sealed class OrderLine
{
    public Ulid Id { get; init; }
    public ProductBase Product { get; init; } = default!;
    public Quantity Qty { get; init; } = new(1, "ea");
    public Money UnitPrice { get; init; } = new(0, "USD");
    public Money LineTotal { get; init; } = new(0, "USD");
}

public sealed class Payment
{
    public Ulid Id { get; init; }
    public PaymentMethod Method { get; init; }
    public Money Amount { get; init; } = new(0, "USD");
    public DateTimeOffset? CapturedAt { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = [];
}
#endregion
