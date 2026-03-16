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
    public int Id1 { get; set; }
    public string? Id2 { get; set; }
    public string? Description { get; set; }
}

/// <summary>
///     Test type with composite identity (int + string + Guid) for identity testing.
/// </summary>
public class IdentityThreeScalarPartComposite
{
    public int Id1 { get; set; }
    public string? Id2 { get; set; }
    public Guid Id3 { get; set; }
    public string? Description { get; set; }
}

/// <summary>
///     Test type to be used as a nested part for identity testing.
/// </summary>
public class IdentityNested
{
    public int Id { get; set; }
    public string? Description { get; set; }
}

/// <summary>
///     Test type with composite identity that includes a nested part for identity testing.
/// </summary>
public class IdentityNestedComposite
{
    public IdentityNested NestedPart { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
}

/// <summary>
///     Test type to be used as an owner for an owned type with composite identity for identity testing.
/// </summary>
public class IdentityOwner
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public List<IdentityOwnedComposite> Dependents { get; set; } = [];
    public IdentityOwnedDependent Dependent { get; set; } = null!;
}

/// <summary>
///     Test type with composite identity that includes an owner identity and scalar identity composite for identity testing.
/// </summary>
public class IdentityOwnedComposite
{
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
///     Test type with a 1-to-1 relationship with the owner type for identity testing.
/// </summary>
public class IdentityOwnedDependent
{
    public string Description { get; set; } = string.Empty;
}
