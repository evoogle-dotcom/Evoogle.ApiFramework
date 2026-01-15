// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiObjectTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private class GetIdentityTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required object? ClrInstance { get; init; }
        public required string? ApiIdentityName { get; init; }
        public required bool ShouldSucceed { get; init; }
        public required ApiId? ExpectedId { get; init; }
        public Type? ExpectedException { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private ApiId ActualId { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:         {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType:     {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiIdentityName:   {this.ApiIdentityName.SafeToString() ?? "<primary>"}");
            this.WriteLine($"ClrInstance:       {this.ClrInstance.SafeToString()}");
            this.WriteLine($"ShouldSucceed:     {this.ShouldSucceed}");
            this.WriteLine($"ExpectedId:        {this.ExpectedId.SafeToString()}");
            this.WriteLine($"ExpectedException: {this.ExpectedException?.Name.SafeToString() ?? "<none>"}");
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                this.ActualId = this.ApiObjectType!.GetIdentity(this.ClrInstance!, this.ApiIdentityName);
                this.WriteLine($"ActualId: {this.ActualId}");
            }
            catch (Exception ex)
            {
                this.ActualException = ex;
                this.WriteLine($"ActualException: {ex.GetType().Name} - {ex.Message}");
            }
            this.WriteLine();
        }

        protected override void Assert()
        {
            if (this.ShouldSucceed)
            {
                this.ActualException.Should().BeNull();
                this.ActualId.Should().NotBeNull();
                this.ActualId.Should().Be(this.ExpectedId!.Value);
            }
            else
            {
                this.ActualException.Should().NotBeNull();
                if (this.ExpectedException != null)
                {
                    this.ActualException.Should().BeOfType(this.ExpectedException);
                }
            }
        }
        #endregion
    }

    private class GetIdentityFromValuesTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required IReadOnlyDictionary<string, object?>? PropertyValues { get; init; }
        public required string? ApiIdentityName { get; init; }
        public required bool ShouldSucceed { get; init; }
        public required ApiId? ExpectedId { get; init; }
        public Type? ExpectedException { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private ApiId ActualId { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:        {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType:    {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiIdentityName:  {this.ApiIdentityName.SafeToString() ?? "<primary>"}");
            this.WriteLine($"PropertyValues:   {this.PropertyValues?.SafeToString()}");
            this.WriteLine($"ShouldSucceed:    {this.ShouldSucceed}");
            this.WriteLine($"ExpectedId:       {this.ExpectedId.SafeToString()}");
            this.WriteLine($"ExpectedException: {this.ExpectedException?.Name.SafeToString() ?? "<none>"}");
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                this.ActualId = this.ApiObjectType!.GetIdentity(this.PropertyValues!, this.ApiIdentityName);
                this.WriteLine($"ActualId: {this.ActualId}");
            }
            catch (Exception ex)
            {
                this.ActualException = ex;
                this.WriteLine($"ActualException: {ex.GetType().Name} - {ex.Message}");
            }
            this.WriteLine();
        }

        protected override void Assert()
        {
            if (this.ShouldSucceed)
            {
                this.ActualException.Should().BeNull();
                this.ActualId.Should().NotBeNull();
                this.ActualId.Should().Be(this.ExpectedId!.Value);
            }
            else
            {
                this.ActualException.Should().NotBeNull();
                if (this.ExpectedException != null)
                {
                    this.ActualException.Should().BeOfType(this.ExpectedException);
                }
            }
        }
        #endregion
    }

    private enum TryGetMethod
    {
        TryGetPropertyByApiName,
        TryGetPropertyByClrName,
        TryGetRelationshipByApiName
    }

    private class TryGetTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required TryGetMethod TryGetMethod { get; init; }
        public required string SearchKey { get; init; }
        public required bool ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private bool? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:      {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType:  {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"TryGetMethod:   {this.TryGetMethod.SafeToString()}");
            this.WriteLine($"SearchKey:      {this.SearchKey.SafeToString()}");
            this.WriteLine($"ExpectedResult: {this.ExpectedResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.TryGetMethod switch
            {
                TryGetMethod.TryGetPropertyByApiName => this.ApiObjectType!.TryGetPropertyByApiName(this.SearchKey!, out _),
                TryGetMethod.TryGetPropertyByClrName => this.ApiObjectType!.TryGetPropertyByClrName(this.SearchKey!, out _),
                TryGetMethod.TryGetRelationshipByApiName => this.ApiObjectType!.TryGetRelationshipByApiName(this.SearchKey!, out _),
                _ => throw new InvalidOperationException($"Unknown {nameof(this.TryGetMethod)}: {this.TryGetMethod}"),
            };

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

    private class TryGetIdentityTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required object? ClrInstance { get; init; }
        public required string? ApiIdentityName { get; init; }
        public required bool ExpectedResult { get; init; }
        public required ApiId? ExpectedId { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private bool ActualResult { get; set; }
        private ApiId ActualId { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:        {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType:    {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiIdentityName:  {this.ApiIdentityName.SafeToString() ?? "<primary>"}");
            this.WriteLine($"ClrInstance:      {this.ClrInstance.SafeToString()}");
            this.WriteLine($"ExpectedResult:   {this.ExpectedResult}");
            this.WriteLine($"ExpectedId:       {this.ExpectedId.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiObjectType!.TryGetIdentity(this.ClrInstance!, out var actualId, this.ApiIdentityName);
            this.ActualId = actualId;

            this.WriteLine($"ActualResult: {this.ActualResult}");
            this.WriteLine($"ActualId:     {this.ActualId}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().Be(this.ExpectedResult);
            if (this.ExpectedResult)
            {
                this.ActualId.Should().NotBeNull();
                this.ActualId.Should().Be(this.ExpectedId!.Value);
            }
            else
            {
                this.ActualId.Should().Be(ApiId.Empty);
            }
        }
        #endregion
    }

    private class TryGetIdentityFromValuesTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required IReadOnlyDictionary<string, object?>? PropertyValues { get; init; }
        public required string? ApiIdentityName { get; init; }
        public required bool ExpectedResult { get; init; }
        public required ApiId? ExpectedId { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private bool ActualResult { get; set; }
        private ApiId ActualId { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:        {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType:    {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiIdentityName:  {this.ApiIdentityName.SafeToString() ?? "<primary>"}");
            this.WriteLine($"PropertyValues:   {this.PropertyValues?.SafeToString()}");
            this.WriteLine($"ExpectedResult:   {this.ExpectedResult}");
            this.WriteLine($"ExpectedId:       {this.ExpectedId.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiObjectType!.TryGetIdentity(this.PropertyValues!, out var actualId, this.ApiIdentityName);
            this.ActualId = actualId;

            this.WriteLine($"ActualResult: {this.ActualResult}");
            this.WriteLine($"ActualId:     {this.ActualId}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().Be(this.ExpectedResult);
            if (this.ExpectedResult)
            {
                this.ActualId.Should().NotBeNull();
                this.ActualId.Should().Be(this.ExpectedId!.Value);
            }
            else
            {
                this.ActualId.Should().Be(ApiId.Empty);
            }
        }
        #endregion
    }

    private class TryGetIdentitiesTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required IEnumerable<object?>? Instances { get; init; }
        public required string? ApiIdentityName { get; init; }
        public required int ExpectedSuccessCount { get; init; }
        public required int ExpectedFailureCount { get; init; }
        public required int ExpectedSkippedCount { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private IReadOnlyList<ApiIdentityBuildResult>? ActualResults { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:              {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType:          {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiIdentityName:        {this.ApiIdentityName.SafeToString() ?? "<primary>"}");
            this.WriteLine($"Instances:              {this.Instances?.Count().SafeToString() ?? "<null>"} items");
            this.WriteLine($"ExpectedSuccessCount:   {this.ExpectedSuccessCount}");
            this.WriteLine($"ExpectedFailureCount:   {this.ExpectedFailureCount}");
            this.WriteLine($"ExpectedSkippedCount:   {this.ExpectedSkippedCount}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResults = this.ApiObjectType!.TryGetIdentities(this.Instances!, this.ApiIdentityName);

            var successCount = this.ActualResults.Count(r => r.Success);
            var failureCount = this.ActualResults.Count(r => !r.Success);
            var inputCount = this.Instances?.Count() ?? 0;
            var nullCount = this.Instances?.Count(i => i is null) ?? 0;
            var skippedCount = inputCount - this.ActualResults.Count - nullCount;

            this.WriteLine($"ActualResults:    {this.ActualResults.Count} items");
            this.WriteLine($"  Successes:      {successCount}");
            this.WriteLine($"  Failures:       {failureCount}");
            this.WriteLine($"  Skipped (null): {skippedCount}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResults.Should().NotBeNull();
            var successCount = this.ActualResults!.Count(r => r.Success);
            var failureCount = this.ActualResults!.Count(r => !r.Success);

            successCount.Should().Be(this.ExpectedSuccessCount, "success count should match");
            failureCount.Should().Be(this.ExpectedFailureCount, "failure count should match");

            // When method returns empty due to no identity or invalid identity name,
            // all instances are effectively skipped without being processed
            if (this.ActualResults.Count == 0 && this.ExpectedSkippedCount > 0)
            {
                // Early return scenario - instances weren't processed
                var inputCount = this.Instances?.Count() ?? 0;
                inputCount.Should().Be(this.ExpectedSkippedCount, "all instances should be effectively skipped");
                return;
            }

            // Normal processing - verify nulls were skipped
            var inputCount2 = this.Instances?.Count() ?? 0;
            var nullCount = this.Instances?.Count(i => i is null) ?? 0;
            var expectedResultCount = inputCount2 - nullCount;
            this.ActualResults.Count.Should().Be(expectedResultCount, "null instances should be skipped");

            // Verify all successful results have non-empty IDs
            foreach (var result in this.ActualResults.Where(r => r.Success))
            {
                result.Id.HasValue.Should().BeTrue("successful results should have valid IDs");
            }

            // Verify all failed results have empty IDs
            foreach (var result in this.ActualResults.Where(r => !r.Success))
            {
                result.Id.Should().Be(ApiId.Empty, "failed results should have empty IDs");
            }
        }
        #endregion
    }

    private class TryGetIdentityMapTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required IEnumerable<object?>? Instances { get; init; }
        public required string? ApiIdentityName { get; init; }
        public required bool ExpectedResult { get; init; }
        public required int ExpectedMapSize { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private bool ActualResult { get; set; }
        private IReadOnlyDictionary<object, ApiId>? ActualMap { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:         {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType:     {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiIdentityName:   {this.ApiIdentityName.SafeToString() ?? "<primary>"}");
            this.WriteLine($"Instances:         {this.Instances?.Count().SafeToString() ?? "<null>"} items");
            this.WriteLine($"ExpectedResult:    {this.ExpectedResult}");
            this.WriteLine($"ExpectedMapSize:   {this.ExpectedMapSize}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiObjectType!.TryGetIdentityMap(this.Instances!, out var map, this.ApiIdentityName);
            this.ActualMap = map;

            this.WriteLine($"ActualResult: {this.ActualResult}");
            this.WriteLine($"ActualMapSize: {this.ActualMap.Count}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().Be(this.ExpectedResult);
            this.ActualMap.Should().NotBeNull();
            this.ActualMap!.Count.Should().Be(this.ExpectedMapSize);

            if (this.ExpectedResult)
            {
                // All values should be non-empty
                foreach (var kvp in this.ActualMap)
                {
                    kvp.Value.HasValue.Should().BeTrue($"identity for instance should have value");
                }
            }
        }
        #endregion
    }
    #endregion

}