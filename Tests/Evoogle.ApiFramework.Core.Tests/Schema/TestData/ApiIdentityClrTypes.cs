// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>
/// Test type with composite identity (int + string + Guid) for identity testing.
/// </summary>
public class ProductInventory
{
    public int WarehouseId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public Guid BatchId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// Test type with composite identity allowing nulls (ReturnEmpty) for identity testing.
/// </summary>
public class CompositeNullable
{
    public int Part1 { get; set; }
    public string? Part2 { get; set; }
}

/// <summary>
/// Test type with composite identity that throws on nulls (ThrowException) for identity testing.
/// </summary>
public class CompositeStrict
{
    public int Part1 { get; set; }
    public string? Part2 { get; set; }
}

/// <summary>
/// Test type with primary and alternate identities for identity testing.
/// </summary>
public class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
