// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.TestData;

/// <summary>
///     Test type with single scalar key part for key testing.
///     Has both primary and alternate key for Id and Name respectively.
/// </summary>
public class KeyOneScalarPart
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();

        return $"{nameof(KeyOneScalarPart)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}}}";
    }
}

/// <summary>
///     Test type with composite key (int + string) for key testing.
/// </summary>
public class KeyTwoScalarPartComposite
{
    public int Id1 { get; set; }
    public string? Id2 { get; set; }
    public string? Description { get; set; }

    public override string ToString()
    {
        var id1 = this.Id1.SafeToString();
        var id2 = this.Id2.SafeToString();
        var description = this.Description.SafeToString();

        return $"{nameof(KeyTwoScalarPartComposite)} {{{nameof(this.Id1)}={id1}, {nameof(this.Id2)}={id2}}}";
    }
}

/// <summary>
///     Test type with composite key (int + string + Guid) for key testing.
/// </summary>
public class KeyThreeScalarPartComposite
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

        return $"{nameof(KeyThreeScalarPartComposite)} {{{nameof(this.Id1)}={id1}, {nameof(this.Id2)}={id2}, {nameof(this.Id3)}={id3}}}";
    }
}

/// <summary>
///     Test type to be used as a nested key part for key testing.
/// </summary>
public class KeyNested
{
    public int Id { get; set; }
    public string? Description { get; set; }

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var description = this.Description.SafeToString();

        return $"{nameof(KeyNested)} {{{nameof(this.Id)}={id}}}";
    }
}

/// <summary>
///     Test type with composite key that includes a nested key part for key testing.
/// </summary>
public class KeyNestedComposite
{
    public KeyNested NestedPart { get; set; } = null!;
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        var nestedPart = this.NestedPart.SafeToString();
        var name = this.Name.SafeToString();

        return $"{nameof(KeyNestedComposite)} {{{nameof(this.NestedPart)}={nestedPart}, {nameof(this.Name)}={name}}}";
    }
}

/// <summary>
///     Test type to be used as an owner for an owned type with composite key for key testing.
/// </summary>
public class KeyOwner
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public List<KeyOwnedComposite> Dependents { get; set; } = [];
    public KeyOwnedDependent Dependent { get; set; } = null!;

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var description = this.Description.SafeToString();
        var dependents = $"[{this.Dependents.SafeToDelimitedString(',')}]";
        var dependent = this.Dependent.SafeToString();

        return $"{nameof(KeyOwner)} {{{nameof(this.Id)}={id}}}";
    }
}

/// <summary>
///     Test type with composite key that includes an owner key and scalar key composite for key testing.
/// </summary>
public class KeyOwnedComposite
{
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;

    public override string ToString()
    {
        var lineNumber = this.LineNumber.SafeToString();
        var description = this.Description.SafeToString();

        return $"{nameof(KeyOwnedComposite)} {{{nameof(this.LineNumber)}={lineNumber}}}";
    }
}

/// <summary>
///     Test type with a 1-to-1 relationship with the owner type for key testing.
/// </summary>
public class KeyOwnedDependent
{
    public string Description { get; set; } = string.Empty;

    public override string ToString()
    {
        var description = this.Description.SafeToString();

        return $"{nameof(KeyOwnedDependent)}";
    }
}
