// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.TestData;

/// <summary>
///     Test type with composite identity (int + string + Guid) for identity testing.
/// </summary>
public class ProductInventory
{
    public int WarehouseId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public Guid BatchId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
///     Test type with composite identity allowing nulls (ReturnEmpty) for identity testing.
/// </summary>
public class CompositeNullable
{
    public int Part1 { get; set; }
    public string? Part2 { get; set; }
}

/// <summary>
///     Test type with composite identity that throws on nulls (ThrowException) for identity testing.
/// </summary>
public class CompositeStrict
{
    public int Part1 { get; set; }
    public string? Part2 { get; set; }
}

/// <summary>
///     Test type with primary and alternate identities for identity testing.
/// </summary>
public class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

/// <summary>
///     Test type with simple scalar identity for snapshot testing.
/// </summary>
public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

/// <summary>
///     Test type with nested composite identity for snapshot navigation testing.
/// </summary>
public class OrderSnapshot
{
    public CustomerSnapshot? Customer { get; set; }
    public long OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
}

/// <summary>
///     Test type with nested identity for snapshot testing.
/// </summary>
public class CustomerSnapshot
{
    public int CustomerId { get; set; }
    public CountrySnapshot? Country { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
///     Test type for nested navigation in snapshot testing.
/// </summary>
public class CountrySnapshot
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
}

/// <summary>
///     Test type with mixed scalar types for snapshot flattening tests.
/// </summary>
public class Invoice
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
}
