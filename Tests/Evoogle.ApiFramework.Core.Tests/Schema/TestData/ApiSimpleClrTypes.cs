// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.TestData;

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

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();
        var age = this.Age.SafeToString();
        var gender = this.Gender.SafeToString();
        var hobbies = this.Hobbies.SafeToString();

        return $"{nameof(Person)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}, {nameof(this.Age)}={age}, {nameof(this.Gender)}={gender}, {nameof(this.Hobbies)}={hobbies}}}";
    }
}

public sealed class Company
{
    public string Name { get; set; } = string.Empty;
    public Person? Owner { get; set; }
    public List<Person>? Employees { get; set; }

    public override string ToString()
    {
        var name = this.Name.SafeToString();
        var owner = this.Owner.SafeToString();
        var employees = this.Employees.SafeToString();

        return $"{nameof(Company)} {{{nameof(this.Name)}={name}, {nameof(this.Owner)}={owner}, {nameof(this.Employees)}={employees}}}";
    }
}
#endregion
