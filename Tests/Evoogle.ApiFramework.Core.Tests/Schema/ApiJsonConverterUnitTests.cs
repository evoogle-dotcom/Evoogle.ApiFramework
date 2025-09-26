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
        public Type? ExtensionType1 { get; init; }
        public Type? ExtensionType2 { get; init; }
        public JsonSerializerOptions? JsonSerializerOptions { get; init; } = DefaultJsonSerializerOptions;
        #endregion

        #region Calculated Properties
        private T? Actual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Source: {this.Source.SafeToString().RemoveWhitespace()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.Expected.SafeToString()}");
            this.WriteLine();

            if (this.ExtensionType1 != null)
            {
                // Attach test extension 1 if requested.
                var extension1 = Activator.CreateInstance(this.ExtensionType1);
                this.Expected?.AttachExtension(this.ExtensionType1, extension1!);
            }

            if (this.ExtensionType2 != null)
            {
                // Attach test extension 2 if requested.
                var extension2 = Activator.CreateInstance(this.ExtensionType2);
                this.Expected?.AttachExtension(this.ExtensionType2, extension2!);
            }
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
        public Type? AddTestExtension1 { get; init; }
        public Type? AddTestExtension2 { get; init; }
        public JsonSerializerOptions? JsonSerializerOptions { get; init; } = DefaultJsonSerializerOptions;
        #endregion

        #region Calculated Properties
        private T? Actual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Expected: {this.Expected.SafeToString()}");

            if (this.AddTestExtension1 != null)
            {
                // Attach test extension 1 if requested.
                var extension1 = Activator.CreateInstance(this.AddTestExtension1);
                this.Expected?.AttachExtension(this.AddTestExtension1, extension1!);
            }

            if (this.AddTestExtension2 != null)
            {
                // Attach test extension 2 if requested.
                var extension2 = Activator.CreateInstance(this.AddTestExtension2);
                this.Expected?.AttachExtension(this.AddTestExtension2, extension2!);
            }
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
        public Type? AddTestExtension1 { get; init; }
        public Type? AddTestExtension2 { get; init; }
        public JsonSerializerOptions? JsonSerializerOptions { get; init; } = DefaultJsonSerializerOptions;
        #endregion

        #region Calculated Properties
        private string? ActualJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Source: {this.Source.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.Expected.SafeToString().RemoveWhitespace()}");
            this.WriteLine();

            if (this.AddTestExtension1 != null)
            {
                // Attach test extension 1 if requested.
                var extension1 = Activator.CreateInstance(this.AddTestExtension1);
                this.Source?.AttachExtension(this.AddTestExtension1, extension1!);
            }

            if (this.AddTestExtension2 != null)
            {
                // Attach test extension 2 if requested.
                var extension2 = Activator.CreateInstance(this.AddTestExtension2);
                this.Source?.AttachExtension(this.AddTestExtension2, extension2!);
            }
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
