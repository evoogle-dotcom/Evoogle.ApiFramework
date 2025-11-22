// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
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
}

public struct Point
{
    public int X; // field
    public int Y { get; set; } // property
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
}

public sealed class Person
{
    public string Name { get; set; } = string.Empty;
    public int? Age { get; set; }
    public Gender? Gender { get; set; }
    public List<string>? Hobbies { get; set; }
}

public sealed class Company
{
    public string Name { get; set; } = string.Empty;
    public Person? Owner { get; set; }
    public List<Person>? Employees { get; set; }
}
#endregion
