// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.TestData;

/// <summary>
///     Test type with single scalar identity for identity testing.
///     Has both primary and alternate identity for Id and Name respectively.
/// </summary>
public class IdentityScalar
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
///     Test type with composite identity (int + string) for identity testing.
/// </summary>
public class IdentityTwoScalarPartComposite
{
    public int Part1 { get; set; }
    public string? Part2 { get; set; }
}

/// <summary>
///     Test type with composite identity (int + string + Guid) for identity testing.
/// </summary>
public class IdentityThreeScalarPartComposite
{
    public int Part1 { get; set; }
    public string? Part2 { get; set; }
    public Guid Part3 { get; set; }
}

/// <summary>
///     Test type to be used as a nested part for identity testing.
/// </summary>
public class IdentityNestedPart
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
}

/// <summary>
///     Test type with composite identity that includes a nested part for identity testing.
/// </summary>
public class IdentityNestedComposite
{
    public IdentityNestedPart NestedPart { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
}

/// <summary>
///     Test type to be used as a parent for a child type with composite identity for identity testing.
/// </summary>
public class IdentityParent
{
    public int Id { get; set; }
}

/// <summary>
///     Test type with composite identity that includes a parent identity and scalar identity composite for identity testing.
/// </summary>
public class IdentityChildComposite
{
    public int ChildId { get; set; }
    public string ChildName { get; set; } = string.Empty;
}
