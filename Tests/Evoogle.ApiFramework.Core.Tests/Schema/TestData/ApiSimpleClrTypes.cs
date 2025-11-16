// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

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

public sealed class ScalarsOnly
{
    public string RequiredName { get; set; }
    public long RequiredNumber { get; set; }
    public bool RequiredPredicate { get; set; }

    public string? OptionalName { get; set; }
    public long? OptionalNumber { get; set; }
    public bool? OptionalPredicate { get; set; }

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

public class TestExtension
{
    public bool Flag { get; set; } = true;
}

public class TestExtension1
{
    public string Description { get; set; } = nameof(TestExtension1);
}

public class TestExtension2
{
    public string Id { get; set; } = "2";
    public string Name { get; set; } = nameof(TestExtension2);
}
#endregion
