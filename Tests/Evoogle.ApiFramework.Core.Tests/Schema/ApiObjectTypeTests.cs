// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public class ApiObjectTypeTests(ITestOutputHelper output) : XUnitTests(output)
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

            // FIX: Changed from this.ApiIdentityName to this.ApiObjectTypeName
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

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] GetIdentityTheoryData =>
    [
        // Simple single-part identity
        new GetIdentityTest
        {
            Name = "GetIdentity succeeds with simple single-part identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = new Person { Id = 42, Name = "John Doe" },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        // Composite identity with multiple types
        new GetIdentityTest
        {
            Name = "GetIdentity succeeds with composite identity (int + string + Guid)",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(ProductInventory),
            ClrInstance = new ProductInventory
            {
                WarehouseId = 123,
                ProductCode = "PROD-456",
                BatchId = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
                Quantity = 10
            },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.Composite(
                new ApiIdPart("BatchId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))),
                new ApiIdPart("ProductCode", ApiId.FromString("PROD-456")),
                new ApiIdPart("WarehouseId", ApiId.FromInt32(123))
            )
        },

        // Composite identity with null handling - ReturnEmpty
        new GetIdentityTest
        {
            Name = "GetIdentity succeeds with composite identity containing null part (ReturnEmpty)",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(CompositeNullable),
            ClrInstance = new CompositeNullable { Part1 = 100, Part2 = null },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.Composite(
                new ApiIdPart("Part1", ApiId.FromInt32(100)),
                new ApiIdPart("Part2", ApiId.Empty)
            )
        },

        // Composite identity with null handling - ThrowException
        new GetIdentityTest
        {
            Name = "GetIdentity throws with composite identity containing null part (ThrowException)",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(CompositeStrict),
            ClrInstance = new CompositeStrict { Part1 = 200, Part2 = null },
            ApiIdentityName = null,
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(ApiIdentityException)
        },

        // Alternate identity
        new GetIdentityTest
        {
            Name = "GetIdentity succeeds with alternate identity",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(User),
            ClrInstance = new User { UserId = 999, Email = "user@example.com", Username = "johndoe" },
            ApiIdentityName = "AK_User_Email",
            ShouldSucceed = true,
            ExpectedId = ApiId.FromString("user@example.com")
        },

        // Primary identity when object has alternates
        new GetIdentityTest
        {
            Name = "GetIdentity succeeds with primary identity when alternates exist",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(User),
            ClrInstance = new User { UserId = 999, Email = "user@example.com", Username = "johndoe" },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.FromInt32(999)
        },

        // Null instance
        new GetIdentityTest
        {
            Name = "GetIdentity throws ArgumentNullException with null instance",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = null,
            ApiIdentityName = null,
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(ArgumentNullException)
        },

        // Object type without identity
        new GetIdentityTest
        {
            Name = "GetIdentity throws InvalidOperationException when object type has no identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            // FIX: Added required properties to ScalarsOnly
            ClrInstance = new ScalarsOnly
            {
                RequiredName = "test",
                RequiredNumber = 1,
                RequiredPredicate = true
            },
            ApiIdentityName = null,
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(InvalidOperationException)
        },

        // Invalid identity name
        new GetIdentityTest
        {
            Name = "GetIdentity throws InvalidOperationException with invalid identity name",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = new Person { Id = 42, Name = "John Doe" },
            ApiIdentityName = "NonExistentIdentity",
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(InvalidOperationException)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] GetIdentityFromValuesTheoryData =>
    [
        // Simple identity from dictionary
        new GetIdentityFromValuesTest
        {
            Name = "GetIdentity from values succeeds with simple identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = 42 },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        // Composite identity from dictionary
        new GetIdentityFromValuesTest
        {
            Name = "GetIdentity from values succeeds with composite identity",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(ProductInventory),
            PropertyValues = new Dictionary<string, object?>
            {
                ["WarehouseId"] = 123,
                ["ProductCode"] = "PROD-456",
                ["BatchId"] = Guid.Parse("12345678-1234-1234-1234-123456789abc")
            },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.Composite(
                new ApiIdPart("BatchId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))),
                new ApiIdPart("ProductCode", ApiId.FromString("PROD-456")),
                new ApiIdPart("WarehouseId", ApiId.FromInt32(123))
            )
        },

        // Type coercion in dictionary
        new GetIdentityFromValuesTest
        {
            Name = "GetIdentity from values succeeds with type coercion (string to int)",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = "42" },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        // Missing property in dictionary
        new GetIdentityFromValuesTest
        {
            Name = "GetIdentity from values throws when property missing",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(ProductInventory),
            PropertyValues = new Dictionary<string, object?> { ["WarehouseId"] = 123 },
            ApiIdentityName = null,
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(ApiIdentityException)
        },

        // Null dictionary
        new GetIdentityFromValuesTest
        {
            Name = "GetIdentity from values throws with null dictionary",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = null,
            ApiIdentityName = null,
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(ArgumentNullException)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetTheoryData =>
    [
        // TryGetPropertyByApiName
        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByApiName)} returns true when {nameof(ApiProperty)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            SearchKey = "RequiredName",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByApiName)} returns true when {nameof(ApiProperty)} exists but with case-insensitive search",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            SearchKey = "requiredname",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByApiName)} returns false when {nameof(ApiProperty)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            SearchKey = "Unknown_Property",
            ExpectedResult = false
        },

        // TryGetPropertyByClrName
        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByClrName)} returns true when {nameof(ApiProperty)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            SearchKey = "OptionalName",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByClrName)} returns true when {nameof(ApiProperty)} exists but with case-insensitive search",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            SearchKey = "OPTIONALNAME",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByClrName)} returns false when {nameof(ApiProperty)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            SearchKey = "Unknown_Property",
            ExpectedResult = false
        },

        // TryGetRelationshipByApiName
        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetRelationshipByApiName)} returns true when {nameof(ApiRelationship)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            SearchKey = "Company_Owner",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetRelationshipByApiName)} returns true when {nameof(ApiRelationship)} exists but with case-insensitive search",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            SearchKey = "COMPANY_OWNER",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetRelationshipByApiName)} returns false when {nameof(ApiRelationship)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            SearchKey = "Unknown_Relationship",
            ExpectedResult = false
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetIdentityTheoryData =>
    [
        // Successful cases
        new TryGetIdentityTest
        {
            Name = "TryGetIdentity returns true with simple identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = new Person { Id = 42, Name = "John Doe" },
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        new TryGetIdentityTest
        {
            Name = "TryGetIdentity returns true with composite identity",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(ProductInventory),
            ClrInstance = new ProductInventory
            {
                WarehouseId = 123,
                ProductCode = "PROD-456",
                BatchId = Guid.Parse("12345678-1234-1234-1234-123456789abc")
            },
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedId = ApiId.Composite(
                new ApiIdPart("BatchId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))),
                new ApiIdPart("ProductCode", ApiId.FromString("PROD-456")),
                new ApiIdPart("WarehouseId", ApiId.FromInt32(123))
            )
        },

        // Failure cases - should return false without throwing
        new TryGetIdentityTest
        {
            Name = "TryGetIdentity returns false with null instance",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = null,
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryGetIdentityTest
        {
            Name = "TryGetIdentity returns false when object type has no identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            // FIX: Added required properties to ScalarsOnly
            ClrInstance = new ScalarsOnly
            {
                RequiredName = "test",
                RequiredNumber = 1,
                RequiredPredicate = true
            },
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryGetIdentityTest
        {
            Name = "TryGetIdentity returns false with invalid identity name",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = new Person { Id = 42, Name = "John Doe" },
            ApiIdentityName = "InvalidIdentity",
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetIdentityFromValuesTheoryData =>
    [
        // Successful cases
        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns true with simple identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = 42 },
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns true with composite identity",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(ProductInventory),
            PropertyValues = new Dictionary<string, object?>
            {
                ["WarehouseId"] = 123,
                ["ProductCode"] = "PROD-456",
                ["BatchId"] = Guid.Parse("12345678-1234-1234-1234-123456789abc")
            },
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedId = ApiId.Composite(
                new ApiIdPart("BatchId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))),
                new ApiIdPart("ProductCode", ApiId.FromString("PROD-456")),
                new ApiIdPart("WarehouseId", ApiId.FromInt32(123))
            )
        },

        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns true with type coercion",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = "42" },
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns true with alternate identity",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(User),
            PropertyValues = new Dictionary<string, object?> { ["Email"] = "user@example.com" },
            ApiIdentityName = "AK_User_Email",
            ExpectedResult = true,
            ExpectedId = ApiId.FromString("user@example.com")
        },

        // Failure cases - should return false without throwing
        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns false with null dictionary",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = null,
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns false when object type has no identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            PropertyValues = new Dictionary<string, object?> { ["RequiredName"] = "test" },
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns false with invalid identity name",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = 42 },
            ApiIdentityName = "InvalidIdentity",
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns false when property missing",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(ProductInventory),
            PropertyValues = new Dictionary<string, object?> { ["WarehouseId"] = 123 },
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns false with invalid property value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = "not-a-number" },
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryGetIdentityFromValuesTest
        {
            Name = "TryGetIdentity from values returns false with empty dictionary",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?>(),
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetIdentitiesTheoryData =>
    [
        // Success with multiple instances
        new TryGetIdentitiesTest
        {
            Name = "TryGetIdentities returns all successes with valid instances",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances =
            [
                new Person { Id = 1, Name = "Alice" },
                new Person { Id = 2, Name = "Bob" },
                new Person { Id = 3, Name = "Charlie" }
            ],
            ApiIdentityName = null,
            ExpectedSuccessCount = 3,
            ExpectedFailureCount = 0,
            ExpectedSkippedCount = 0
        },

        // Partial success with composite identities
        new TryGetIdentitiesTest
        {
            Name = "TryGetIdentities returns partial successes with nullable composite identities",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(CompositeNullable),
            Instances =
            [
                new CompositeNullable { Part1 = 100, Part2 = "A" },
                new CompositeNullable { Part1 = 200, Part2 = null }, // Has null part but ReturnEmpty handling
                new CompositeNullable { Part1 = 300, Part2 = "C" }
            ],
            ApiIdentityName = null,
            ExpectedSuccessCount = 3,
            ExpectedFailureCount = 0,
            ExpectedSkippedCount = 0
        },

        // Failures with strict null handling
        new TryGetIdentitiesTest
        {
            Name = "TryGetIdentities returns failures with strict null handling",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(CompositeStrict),
            Instances =
            [
                new CompositeStrict { Part1 = 100, Part2 = "A" },
                new CompositeStrict { Part1 = 200, Part2 = null }, // Should fail with ThrowException handling
                new CompositeStrict { Part1 = 300, Part2 = "C" }
            ],
            ApiIdentityName = null,
            ExpectedSuccessCount = 2,
            ExpectedFailureCount = 1,
            ExpectedSkippedCount = 0
        },

        // Null instances are skipped
        new TryGetIdentitiesTest
        {
            Name = "TryGetIdentities skips null instances",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances =
            [
                new Person { Id = 1, Name = "Alice" },
                null!,
                new Person { Id = 3, Name = "Charlie" },
                null!
            ],
            ApiIdentityName = null,
            ExpectedSuccessCount = 2,
            ExpectedFailureCount = 0,
            ExpectedSkippedCount = 2
        },

        // Empty collection
        new TryGetIdentitiesTest
        {
            Name = "TryGetIdentities returns empty results with empty collection",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances = [],
            ApiIdentityName = null,
            ExpectedSuccessCount = 0,
            ExpectedFailureCount = 0,
            ExpectedSkippedCount = 0
        },

        // Null collection
        new TryGetIdentitiesTest
        {
            Name = "TryGetIdentities returns empty results with null collection",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances = null,
            ApiIdentityName = null,
            ExpectedSuccessCount = 0,
            ExpectedFailureCount = 0,
            ExpectedSkippedCount = 0
        },

        // No identity configured
        new TryGetIdentitiesTest
        {
            Name = "TryGetIdentities returns empty results when object type has no identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            Instances =
            [
                new ScalarsOnly { RequiredName = "test1", RequiredNumber = 1, RequiredPredicate = true },
                new ScalarsOnly { RequiredName = "test2", RequiredNumber = 2, RequiredPredicate = false }
            ],
            ApiIdentityName = null,
            ExpectedSuccessCount = 0,
            ExpectedFailureCount = 0,
            ExpectedSkippedCount = 2 // All instances skipped because no identity configured
        },

        // Invalid identity name
        new TryGetIdentitiesTest
        {
            Name = "TryGetIdentities returns empty results with invalid identity name",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances =
            [
                new Person { Id = 1, Name = "Alice" },
                new Person { Id = 2, Name = "Bob" }
            ],
            ApiIdentityName = "InvalidIdentity",
            ExpectedSuccessCount = 0,
            ExpectedFailureCount = 0,
            ExpectedSkippedCount = 2 // All instances skipped because identity name is invalid
        },

        // Alternate identity
        new TryGetIdentitiesTest
        {
            Name = "TryGetIdentities succeeds with alternate identity",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(User),
            Instances =
            [
                new User { UserId = 1, Email = "alice@example.com", Username = "alice" },
                new User { UserId = 2, Email = "bob@example.com", Username = "bob" }
            ],
            ApiIdentityName = "AK_User_Email",
            ExpectedSuccessCount = 2,
            ExpectedFailureCount = 0,
            ExpectedSkippedCount = 0
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetIdentityMapTheoryData =>
    [
        // Success with multiple instances
        new TryGetIdentityMapTest
        {
            Name = "TryGetIdentityMap succeeds with valid instances",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances =
            [
                new Person { Id = 1, Name = "Alice" },
                new Person { Id = 2, Name = "Bob" },
                new Person { Id = 3, Name = "Charlie" }
            ],
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedMapSize = 3
        },

        // Success with composite identities
        new TryGetIdentityMapTest
        {
            Name = "TryGetIdentityMap succeeds with composite identities",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(ProductInventory),
            Instances =
            [
                new ProductInventory
                {
                    WarehouseId = 1,
                    ProductCode = "PROD-1",
                    BatchId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },
                new ProductInventory
                {
                    WarehouseId = 2,
                    ProductCode = "PROD-2",
                    BatchId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                }
            ],
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedMapSize = 2
        },

        // Fail-fast: fails on first invalid instance
        new TryGetIdentityMapTest
        {
            Name = "TryGetIdentityMap fails with any invalid instance (fail-fast)",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(CompositeStrict),
            Instances =
            [
                new CompositeStrict { Part1 = 100, Part2 = "A" },
                new CompositeStrict { Part1 = 200, Part2 = null }, // Should fail with ThrowException handling
                new CompositeStrict { Part1 = 300, Part2 = "C" }
            ],
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedMapSize = 0 // Partial results up to failure not returned in map
        },

        // Null instance in collection causes failure
        new TryGetIdentityMapTest
        {
            Name = "TryGetIdentityMap fails with null instance in collection",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances =
            [
                new Person { Id = 1, Name = "Alice" },
                null!,
                new Person { Id = 3, Name = "Charlie" }
            ],
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedMapSize = 0
        },

        // Empty collection succeeds
        new TryGetIdentityMapTest
        {
            Name = "TryGetIdentityMap succeeds with empty collection",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances = [],
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedMapSize = 0
        },

        // Null collection fails
        new TryGetIdentityMapTest
        {
            Name = "TryGetIdentityMap fails with null collection",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances = null,
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedMapSize = 0
        },

        // No identity configured fails
        new TryGetIdentityMapTest
        {
            Name = "TryGetIdentityMap fails when object type has no identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            Instances =
            [
                new ScalarsOnly { RequiredName = "test1", RequiredNumber = 1, RequiredPredicate = true },
                new ScalarsOnly { RequiredName = "test2", RequiredNumber = 2, RequiredPredicate = false }
            ],
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedMapSize = 0
        },

        // Invalid identity name fails
        new TryGetIdentityMapTest
        {
            Name = "TryGetIdentityMap fails with invalid identity name",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            Instances =
            [
                new Person { Id = 1, Name = "Alice" },
                new Person { Id = 2, Name = "Bob" }
            ],
            ApiIdentityName = "InvalidIdentity",
            ExpectedResult = false,
            ExpectedMapSize = 0
        },

        // Alternate identity succeeds
        new TryGetIdentityMapTest
        {
            Name = "TryGetIdentityMap succeeds with alternate identity",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(User),
            Instances =
            [
                new User { UserId = 1, Email = "alice@example.com", Username = "alice" },
                new User { UserId = 2, Email = "bob@example.com", Username = "bob" }
            ],
            ApiIdentityName = "AK_User_Email",
            ExpectedResult = true,
            ExpectedMapSize = 2
        },
    ];

    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(GetIdentityTheoryData))]
    public void GetIdentity(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(GetIdentityFromValuesTheoryData))]
    public void GetIdentityFromValues(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetTheoryData))]
    public void TryGet(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetIdentityMapTheoryData))]
    public void TryGetIdentityMap(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetIdentityTheoryData))]
    public void TryGetIdentity(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetIdentityFromValuesTheoryData))]
    public void TryGetIdentityFromValues(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetIdentitiesTheoryData))]
    public void TryGetIdentities(IXUnitTest test) => test.Execute(this);
    #endregion
}
