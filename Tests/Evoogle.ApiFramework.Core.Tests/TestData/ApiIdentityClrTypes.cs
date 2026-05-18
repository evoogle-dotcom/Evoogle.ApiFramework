// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.TestData;

/// <summary>
///     Test type with single scalar identity for identity testing.
///     Has both primary and alternate identity for Id and Name respectively.
/// </summary>
public class IdentityScalar
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();

        return $"{nameof(IdentityScalar)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}}}";
    }
}

/// <summary>
///     Test type with composite identity (int + string) for identity testing.
/// </summary>
public class IdentityTwoScalarPartComposite
{
    public int Id1 { get; set; }
    public string? Id2 { get; set; }
    public string? Description { get; set; }

    public override string ToString()
    {
        var id1 = this.Id1.SafeToString();
        var id2 = this.Id2.SafeToString();
        var description = this.Description.SafeToString();

        return $"{nameof(IdentityTwoScalarPartComposite)} {{{nameof(this.Id1)}={id1}, {nameof(this.Id2)}={id2}}}";
    }
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

    public override string ToString()
    {
        var id1 = this.Id1.SafeToString();
        var id2 = this.Id2.SafeToString();
        var id3 = this.Id3.SafeToString();
        var description = this.Description.SafeToString();

        return $"{nameof(IdentityThreeScalarPartComposite)} {{{nameof(this.Id1)}={id1}, {nameof(this.Id2)}={id2}, {nameof(this.Id3)}={id3}}}";
    }
}

/// <summary>
///     Test type to be used as a nested part for identity testing.
/// </summary>
public class IdentityNested
{
    public int Id { get; set; }
    public string? Description { get; set; }

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var description = this.Description.SafeToString();

        return $"{nameof(IdentityNested)} {{{nameof(this.Id)}={id}}}";
    }
}

/// <summary>
///     Test type with composite identity that includes a nested part for identity testing.
/// </summary>
public class IdentityNestedComposite
{
    public IdentityNested NestedPart { get; set; } = null!;
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        var nestedPart = this.NestedPart.SafeToString();
        var name = this.Name.SafeToString();

        return $"{nameof(IdentityNestedComposite)} {{{nameof(this.NestedPart)}={nestedPart}, {nameof(this.Name)}={name}}}";
    }
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

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var description = this.Description.SafeToString();
        var dependents = $"[{this.Dependents.SafeToDelimitedString(',')}]";
        var dependent = this.Dependent.SafeToString();

        return $"{nameof(IdentityOwner)} {{{nameof(this.Id)}={id}}}";
    }
}

/// <summary>
///     Test type with composite identity that includes an owner identity and scalar identity composite for identity testing.
/// </summary>
public class IdentityOwnedComposite
{
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;

    public override string ToString()
    {
        var lineNumber = this.LineNumber.SafeToString();
        var description = this.Description.SafeToString();

        return $"{nameof(IdentityOwnedComposite)} {{{nameof(this.LineNumber)}={lineNumber}}}";
    }
}

/// <summary>
///     Test type with a 1-to-1 relationship with the owner type for identity testing.
/// </summary>
public class IdentityOwnedDependent
{
    public string Description { get; set; } = string.Empty;

    public override string ToString()
    {
        var description = this.Description.SafeToString();

        return $"{nameof(IdentityOwnedDependent)}";
    }
}
