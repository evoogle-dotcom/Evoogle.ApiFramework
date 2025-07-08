// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extension;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public static class ApiJsonConverterUnitTests
{
    #region Test Classes
    public class ClassWithScalars(string requiredName, long requiredNumber, bool requiredPredicate)
    {
        public string RequiredName { get; set; } = requiredName;
        public long RequiredNumber { get; set; } = requiredNumber;
        public bool RequiredPredicate { get; set; } = requiredPredicate;

        public string? OptionalName { get; set; }
        public long? OptionalNumber { get; set; }
        public bool? OptionalPredicate { get; set; }
    }

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int? Age { get; set; }
        public Gender? Gender { get; set; }
        public List<string>? Hobbies { get; set; }
    }

    public class Company
    {
        public string Name { get; set; } = string.Empty;
        public Person? Owner { get; set; }
        public List<Person>? Employees { get; set; }
    }

    public enum StopLight
    {
        None,
        Green,
        Yellow,
        Red
    }

    public enum Gender
    {
        Unspecified,
        Male,
        Female
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

    public class JsonDeserializeTest<T> : XUnitTest
        where T : IExtensible
    {
        #region Default Properties
        private static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        #endregion

        #region User Supplied Properties
        public string? Source { get; init; }
        public T? Expected { get; init; }
        public bool? AddTestExtension1 { get; init; } = false;
        public bool? AddTestExtension2 { get; init; } = false;
        public JsonSerializerOptions? JsonSerializerOptions { get; init; } = DefaultJsonSerializerOptions;
        #endregion

        #region Calculated Properties
        private T? Actual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            if (this?.AddTestExtension1 == true)
            {
                // Attach test extension 1 if requested.
                this.Expected?.AttachExtension(new TestExtension1());
            }

            if (this?.AddTestExtension2 == true)
            {
                // Attach test extension 2 if requested.
                this.Expected?.AttachExtension(new TestExtension2());
            }

            this.WriteLine($"Source: {this.Source.SafeToString().RemoveWhitespace()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.Expected.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.Actual = JsonSerializer.Deserialize<T>(this.Source!, this.JsonSerializerOptions);
            this.WriteLine($"Actual:   {this.Actual.SafeToString()}");
        }

        protected override void Assert() => this.Actual.Should().BeEquivalentTo(this.Expected);
        #endregion
    }

    public class JsonRoundtripTest<T> : XUnitTest
        where T : IExtensible
    {
        #region Default Properties
        private static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        #endregion

        #region User Supplied Properties
        public T? Expected { get; init; }
        public bool? AddTestExtension1 { get; init; } = false;
        public bool? AddTestExtension2 { get; init; } = false;
        public JsonSerializerOptions? JsonSerializerOptions { get; init; } = DefaultJsonSerializerOptions;
        #endregion

        #region Calculated Properties
        private T? Actual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            if (this?.AddTestExtension1 == true)
            {
                // Attach test extension 1 if requested.
                this.Expected?.AttachExtension(new TestExtension1());
            }

            if (this?.AddTestExtension2 == true)
            {
                // Attach test extension 2 if requested.
                this.Expected?.AttachExtension(new TestExtension2());
            }

            this.WriteLine($"Expected: {this.Expected.SafeToString()}");
        }

        protected override void Act()
        {
            var json = JsonSerializer.Serialize(this.Expected, this.JsonSerializerOptions);
            this.Actual = JsonSerializer.Deserialize<T>(json, this.JsonSerializerOptions);
            this.WriteLine($"Actual:   {this.Actual.SafeToString()}");
        }

        protected override void Assert() => this.Actual.Should().BeEquivalentTo(this.Expected);
        #endregion
    }

    public class JsonSerializeTest<T> : XUnitTest
        where T : IExtensible
    {
        #region Default Properties
        private static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        #endregion

        #region User Supplied Properties
        public T? Source { get; init; }
        public string? Expected { get; init; }
        public bool? AddTestExtension1 { get; init; } = false;
        public bool? AddTestExtension2 { get; init; } = false;
        public JsonSerializerOptions? JsonSerializerOptions { get; init; } = DefaultJsonSerializerOptions;
        #endregion

        #region Calculated Properties
        private string? ActualJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            if (this?.AddTestExtension1 == true)
            {
                // Attach test extension 1 if requested.
                this.Source?.AttachExtension(new TestExtension1());
            }

            if (this?.AddTestExtension2 == true)
            {
                // Attach test extension 2 if requested.
                this.Source?.AttachExtension(new TestExtension2());
            }

            this.WriteLine($"Source: {this.Source.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.Expected.SafeToString().RemoveWhitespace()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualJson = JsonSerializer.Serialize(this.Source, DefaultJsonSerializerOptions);
            this.WriteLine($"Actual:   {this.ActualJson.SafeToString().RemoveWhitespace()}");
        }

        protected override void Assert()
        {
            var actualJsonMinusWhitespace = this.ActualJson.RemoveWhitespace();
            var expectedJsonMinusWhitespace = this.Expected.RemoveWhitespace();

            actualJsonMinusWhitespace.Should().Be(expectedJsonMinusWhitespace);
        }
        #endregion
    }
    #endregion
}
