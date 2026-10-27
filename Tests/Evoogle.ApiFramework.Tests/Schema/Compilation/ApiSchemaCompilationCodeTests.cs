// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.RegularExpressions;

using Evoogle.Json;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Compilation;

public class ApiSchemaCompilationCodeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class SerializationTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaCompilationCode Code { get; init; }
        public required string ExpectedValue { get; init; }
        #endregion

        #region Calculated Properties
        private string? ActualJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Compilation Code: {this.Code}");
            this.WriteLine($"Expected JSON:    \"{this.ExpectedValue}\"");
        }

        protected override void Act()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EnumJsonConverter<ApiSchemaCompilationCode>());

            this.ActualJson = JsonSerializer.Serialize(this.Code, options);

            this.WriteLine();
            this.WriteLine($"Actual JSON: {this.ActualJson}");
        }

        protected override void Assert() => this.ActualJson.Should().Be
        (
            $$"""
            "{{this.ExpectedValue}}"
            """
        );
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryData<IXUnitTest> SerializationTheoryData => new
    (
        Enum.GetValues<ApiSchemaCompilationCode>()
            .Select(code => (IXUnitTest)CreateSerializationTest(code, GetExpectedSerializedValue(code)))
    );
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(SerializationTheoryData))]
    public void Serialization(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static SerializationTest CreateSerializationTest
    (
        ApiSchemaCompilationCode code,
        string expectedValue
    ) => new()
    {
        Name = $"{code} Serializes As {expectedValue}",
        Code = code,
        ExpectedValue = expectedValue
    };

    private static string GetExpectedSerializedValue(ApiSchemaCompilationCode code)
    {
        var value = code.ToString();
        value = Regex.Replace(value, "([a-z0-9])([A-Z])", "$1_$2");
        value = Regex.Replace(value, "([A-Z])([A-Z][a-z])", "$1_$2");
        return value.ToUpperInvariant();
    }
    #endregion
}
