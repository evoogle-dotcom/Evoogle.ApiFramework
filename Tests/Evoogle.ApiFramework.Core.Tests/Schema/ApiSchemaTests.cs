// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
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
        public required string SourceJson { get; init; }
        public required List<ApiInitializationIssue> ExpectedIssues { get; init; }
        #endregion

        #region Calculated Properties
        private List<ApiInitializationIssue>? ActualIssues { get; set; }
        #endregion

        #region Constructors
        public InitializeThrowsTest()
        {
            this.ExpectedExceptionType = typeof(ApiSchemaInitializationException);
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Source JSON:\n{this.SourceJson.SafeToString().RemoveWhitespace()}");
            this.WriteLine();

            this.WriteException("Expected", this.ExpectedExceptionType, this.ExpectedExceptionMessage);
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
                JsonSerializer.Deserialize<ApiSchema>(this.SourceJson, this.JsonSerializerOptions);
            }
            catch (ApiSchemaInitializationException ex)
            {
                this.CaptureException(ex);

                this.ActualIssues = [.. ex.Issues];
                this.WriteLine();
                foreach (var actualIssue in this.ActualIssues)
                {
                    this.WriteLine($"Actual Issue: {actualIssue.SafeToString()}");
                }
            }
            catch (Exception ex)
            {
                this.CaptureException(ex);
            }
        }

        protected override void Assert()
        {
            base.AssertException();

            this.ActualIssues.Should().NotBeNull();
            this.ActualIssues.Should().BeEquivalentTo(this.ExpectedIssues);
        }
    }

    private class InitializeWarnsTest : XUnitTest
    {
        #region User Supplied Properties
        public required string SourceJson { get; init; }
        public required List<ApiInitializationIssue> ExpectedWarnings { get; init; }
        #endregion

        #region Calculated Properties
        private List<ApiInitializationIssue>? ActualWarnings { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Source JSON:\n{this.SourceJson.SafeToString().RemoveWhitespace()}");
            this.WriteLine();

            foreach (var expectedWarning in this.ExpectedWarnings)
            {
                this.WriteLine($"Expected Warning: {expectedWarning.SafeToString()}");
            }
            this.WriteLine();
        }

        protected override void Act()
        {
            // Deserialize the schema — succeeds because warnings do not throw
            var schema = JsonSerializer.Deserialize<ApiSchema>(this.SourceJson);

            if (schema is not null)
            {
                // Re-initialize to capture the ApiInitializationResult with warnings
                // (the initial result is discarded by the JSON converter after ThrowIfInvalid)
                var result = schema.Initialize();
                this.ActualWarnings = result.Warnings is not null ? [.. result.Warnings] : [];
            }

            this.WriteLine();
            foreach (var actualWarning in this.ActualWarnings ?? [])
            {
                this.WriteLine($"Actual Warning: {actualWarning.SafeToString()}");
            }
        }

        protected override void Assert()
        {
            this.ActualWarnings.Should().NotBeNull();
            this.ActualWarnings.Should().BeEquivalentTo(this.ExpectedWarnings);
        }
        #endregion
    }

    private class JsonDeserializeTest : JsonDeserializeTest<ApiSchema, ApiSchemaSpec>
    {
        #region Constructors
        public JsonDeserializeTest()
        {
            this.ExcludeMembers = ApiSchemaExcludeMembers.SchemaInitialized;
        }
        #endregion

        #region JsonDeserializeTest<T, TFactoryArg> Methods
        protected override ApiSchema? CreateExpected(ApiSchemaSpec? descriptor)
        {
            return BuildTestApiSchema(descriptor);
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiSchema, ApiSchemaSpec>
    {
        #region Constructors
        public JsonRoundtripTest()
        {
            this.ExcludeMembers = ApiSchemaExcludeMembers.SchemaInitialized;
        }
        #endregion

        #region JsonRoundtripTest<T, TFactoryArg> Methods
        protected override ApiSchema? CreateExpected(ApiSchemaSpec? descriptor)
        {
            return BuildTestApiSchema(descriptor);
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiSchema, ApiSchemaSpec>
    {
        #region JsonSerializeTest<T, TFactoryArg> Methods
        protected override ApiSchema? CreateSource(ApiSchemaSpec? descriptor)
        {
            return BuildTestApiSchema(descriptor);
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
