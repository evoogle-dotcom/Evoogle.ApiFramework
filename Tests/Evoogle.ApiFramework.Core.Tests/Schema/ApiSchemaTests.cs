// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;
using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private class InitializeThrowsTest : JsonConverterTestBase<ApiSchema>
    {
        #region User Supplied Properties
        public required string Part { get; init; }
        public required string ExpectedExceptionMessage { get; init; }
        public required List<ApiInitializationIssue> ExpectedIssues { get; init; }
        #endregion

        #region Calculated Properties
        private bool? ActualExceptionThrown { get; set; }
        private string? ActualExceptionMessage { get; set; }
        private List<ApiInitializationIssue>? ActualIssues { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Part:   {this.Part.SafeToString().RemoveWhitespace()}");
            this.WriteLine();

            this.WriteLine($"Expected Exception Message: {this.ExpectedExceptionMessage.SafeToString()}");
            this.WriteLine();
            foreach (var expectedIssue in this.ExpectedIssues)
            {
                this.WriteLine($"Expected Issue: {expectedIssue.SafeToString()}");
            }
            this.WriteLine();
        }
        #endregion

        protected override void Act()
        {
            try
            {
                JsonSerializer.Deserialize<ApiSchema>(this.Part, this.JsonSerializerOptions);
            }
            catch (ApiSchemaInitializationException ex)
            {
                this.ActualExceptionThrown = true;
                this.ActualExceptionMessage = ex.Message;
                this.ActualIssues = [.. ex.Issues];
            }

            this.WriteLine($"Actual Exception Thrown:  {this.ActualExceptionThrown.SafeToString()}");
            this.WriteLine($"Actual Exception Message: {this.ActualExceptionMessage.SafeToString()}");
            this.WriteLine();
            if (this.ActualIssues is not null)
            {
                foreach (var actualIssue in this.ActualIssues)
                {
                    this.WriteLine($"Actual Issue: {actualIssue.SafeToString()}");
                }
            }
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualExceptionThrown.Should().BeTrue();
            this.ActualExceptionMessage.Should().Be(this.ExpectedExceptionMessage);
            this.ActualIssues.Should().NotBeNull();
            this.ActualIssues.Should().BeEquivalentTo(this.ExpectedIssues);
        }
    }

    private class JsonDeserializeTest : JsonDeserializeTest<ApiSchema, ApiSchemaDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonDeserializeTest()
        {
            this.Name = nameof(JsonDeserializeTest);
            this.ExpectedFactoryExpression = (arg) => BuildTestApiSchema(arg);
            this.ExcludeMembers = ApiSchemaExcludeMembers.SchemaInitialized;
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiSchema, ApiSchemaDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonRoundtripTest()
        {
            this.Name = nameof(JsonRoundtripTest);
            this.ExpectedFactoryExpression = (arg) => BuildTestApiSchema(arg);
            this.ExcludeMembers = ApiSchemaExcludeMembers.SchemaInitialized;
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiSchema, ApiSchemaDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonSerializeTest()
        {
            this.Name = nameof(JsonSerializeTest);
            this.SourceFactoryExpression = (arg) => BuildTestApiSchema(arg);
        }
        #endregion
    }

    private class TryGetByApiNameTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string SearchKey { get; init; }
        public required bool ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private bool? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            this.WriteLine($"ApiSchema:      {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"SearchKey:      {this.SearchKey.SafeToString()}");
            this.WriteLine($"ExpectedResult: {this.ExpectedResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiSchema!.TryGetTypeByApiName(this.SearchKey, out _);

            this.WriteLine($"ActualResult: {this.ActualResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().NotBeNull();
            this.ActualResult.Should().Be(this.ExpectedResult);
        }
        #endregion
    }

    private class TryGetByClrTypeTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required Type SearchKey { get; init; }
        public required bool ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private bool? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            this.WriteLine($"ApiSchema:      {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"SearchKey:      {this.SearchKey.SafeToString()}");
            this.WriteLine($"ExpectedResult: {this.ExpectedResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiSchema!.TryGetTypeByClrType(this.SearchKey, out _);

            this.WriteLine($"ActualResult: {this.ActualResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().NotBeNull();
            this.ActualResult.Should().Be(this.ExpectedResult);
        }
        #endregion
    }
    #endregion
}
