// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.TestData;

#region Enums
public enum Gender
{
    Unspecified,
    Male,
    Female
}

public enum StopLight
{
    None,
    Green,
    Yellow,
    Red
}
#endregion

#region Object Types
public sealed class Empty()
{
    public override string ToString()
    {
        return "Empty {}";
    }
}

public struct Point
{
    public long X; // field
    public long Y { get; set; } // property
    public string? Note { get; set; }

    public override string ToString()
    {
        var x = this.X.SafeToString();
        var y = this.Y.SafeToString();
        var note = this.Note.SafeToString();

        return $"{nameof(Point)} {{{nameof(this.X)}={x}, {nameof(this.Y)}={y}, {nameof(this.Note)}={note}}}";
    }
}

public sealed class ScalarsOnly
{
    public string RequiredName { get; set; }
    public long RequiredNumber { get; set; }
    public bool RequiredPredicate { get; set; }

    public string? OptionalName;
    public long? OptionalNumber;
    public bool? OptionalPredicate;

    public ScalarsOnly()
    {
        this.RequiredName = string.Empty;
        this.RequiredNumber = default;
        this.RequiredPredicate = default;
    }

    public ScalarsOnly(string requiredName, long requiredNumber, bool requiredPredicate)
    {
        this.RequiredName = requiredName;
        this.RequiredNumber = requiredNumber;
        this.RequiredPredicate = requiredPredicate;
    }

    public override string ToString()
    {
        var requiredName = this.RequiredName.SafeToString();
        var requiredNumber = this.RequiredNumber.SafeToString();
        var requiredPredicate = this.RequiredPredicate.SafeToString();
        var optionalName = this.OptionalName.SafeToString();
        var optionalNumber = this.OptionalNumber.SafeToString();
        var optionalPredicate = this.OptionalPredicate.SafeToString();

        return $"{nameof(ScalarsOnly)} {{{nameof(this.RequiredName)}={requiredName}, {nameof(this.RequiredNumber)}={requiredNumber}, {nameof(this.RequiredPredicate)}={requiredPredicate}, {nameof(this.OptionalName)}={optionalName}, {nameof(this.OptionalNumber)}={optionalNumber}, {nameof(this.OptionalPredicate)}={optionalPredicate}}}";
    }
}

public sealed class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Age { get; set; }
    public Gender? Gender { get; set; }
    public List<string>? Hobbies { get; set; }
    public Ulid? CompanyId { get; set; }

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();
        var age = this.Age.SafeToString();
        var gender = this.Gender.SafeToString();
        var hobbies = this.Hobbies.SafeToString();
        var companyId = this.CompanyId.SafeToString();

        return $"{nameof(Person)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}, {nameof(this.Age)}={age}, {nameof(this.Gender)}={gender}, {nameof(this.Hobbies)}={hobbies}, {nameof(this.CompanyId)}={companyId}}}";
    }
}

public sealed class Company
{
    public Ulid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Person? Owner { get; set; }
    public List<Person>? Employees { get; set; }

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();
        var owner = this.Owner.SafeToString();
        var employees = this.Employees.SafeToString();

        return $"{nameof(Company)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}, {nameof(this.Owner)}={owner}, {nameof(this.Employees)}={employees}}}";
    }
}

/// <summary>
///     CLR type used to test API_PROPERTY_REQUIRED_NULLABLE_MISMATCH and API_PROPERTY_OPTIONAL_NON_NULLABLE_MISMATCH
///     initialization warnings by deliberately pairing nullable/non-nullable CLR members with mismatched API modifiers.
/// </summary>
public sealed class NullabilityMismatch
{
    /// <summary>Nullable reference type: triggers API_PROPERTY_REQUIRED_NULLABLE_MISMATCH when declared Required.</summary>
    public string? NullableProp { get; set; }

    /// <summary>Non-nullable reference type: triggers API_PROPERTY_OPTIONAL_NON_NULLABLE_MISMATCH when declared Optional.</summary>
    public string NonNullableProp { get; set; } = string.Empty;
}

/// <summary>
///     CLR type used to test API_COLLECTION_ITEM_REQUIRED_NULLABLE_MISMATCH and
///     API_COLLECTION_ITEM_OPTIONAL_NON_NULLABLE_MISMATCH initialization warnings by deliberately
///     pairing nullable/non-nullable CLR collection element types with mismatched API item modifiers.
/// </summary>
public sealed class CollectionNullabilityMismatch
{
    /// <summary>Collection with nullable elements: triggers API_COLLECTION_ITEM_REQUIRED_NULLABLE_MISMATCH when item declared Required.</summary>
    public List<string?>? NullableItemsProp { get; set; }

    /// <summary>Collection with non-nullable elements: triggers API_COLLECTION_ITEM_OPTIONAL_NON_NULLABLE_MISMATCH when item declared Optional.</summary>
    public List<string> NonNullableItemsProp { get; set; } = [];
}
#endregion
