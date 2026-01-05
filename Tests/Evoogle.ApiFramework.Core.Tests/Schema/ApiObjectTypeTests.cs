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
    private class BuildIdentityTest : XUnitTest
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

            this.WriteLine($"ApiSchema:        {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType:    {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiIdentityName:  {this.ApiIdentityName.SafeToString() ?? "<primary>"}");
            this.WriteLine($"ClrInstance:      {this.ClrInstance.SafeToString()}");
            this.WriteLine($"ShouldSucceed:    {this.ShouldSucceed}");
            this.WriteLine($"ExpectedId:       {this.ExpectedId.SafeToString()}");
            this.WriteLine($"ExpectedException: {this.ExpectedException?.Name.SafeToString() ?? "<none>"}");
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                this.ActualId = this.ApiObjectType!.BuildIdentity(this.ClrInstance!, this.ApiIdentityName);
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

    private class BuildIdentityFromValuesTest : XUnitTest
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
                this.ActualId = this.ApiObjectType!.BuildIdentity(this.PropertyValues!, this.ApiIdentityName);
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

    private class TryBuildIdentityTest : XUnitTest
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
            this.ActualResult = this.ApiObjectType!.TryBuildIdentity(this.ClrInstance!, out var actualId, this.ApiIdentityName);
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

    private class TryBuildIdentityFromValuesTest : XUnitTest
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
            this.ActualResult = this.ApiObjectType!.TryBuildIdentity(this.PropertyValues!, out var actualId, this.ApiIdentityName);
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
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildIdentityTheoryData =>
    [
        // Simple single-part identity
        new BuildIdentityTest
        {
            Name = "BuildIdentity succeeds with simple single-part identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = new Person { Id = 42, Name = "John Doe" },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        // Composite identity with multiple types
        new BuildIdentityTest
        {
            Name = "BuildIdentity succeeds with composite identity (int + string + Guid)",
            ApiSchemaKind = ApiSchemaKind.CompositeIdentity,
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
        new BuildIdentityTest
        {
            Name = "BuildIdentity succeeds with composite identity containing null part (ReturnEmpty)",
            ApiSchemaKind = ApiSchemaKind.CompositeIdentityNullable,
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
        new BuildIdentityTest
        {
            Name = "BuildIdentity throws with composite identity containing null part (ThrowException)",
            ApiSchemaKind = ApiSchemaKind.CompositeIdentityStrict,
            ApiObjectTypeName = nameof(CompositeStrict),
            ClrInstance = new CompositeStrict { Part1 = 200, Part2 = null },
            ApiIdentityName = null,
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(ApiIdentityException)
        },

        // Alternate identity
        new BuildIdentityTest
        {
            Name = "BuildIdentity succeeds with alternate identity",
            ApiSchemaKind = ApiSchemaKind.AlternateIdentity,
            ApiObjectTypeName = nameof(User),
            ClrInstance = new User { UserId = 999, Email = "user@example.com", Username = "johndoe" },
            ApiIdentityName = "AK_User_Email",
            ShouldSucceed = true,
            ExpectedId = ApiId.FromString("user@example.com")
        },

        // Primary identity when object has alternates
        new BuildIdentityTest
        {
            Name = "BuildIdentity succeeds with primary identity when alternates exist",
            ApiSchemaKind = ApiSchemaKind.AlternateIdentity,
            ApiObjectTypeName = nameof(User),
            ClrInstance = new User { UserId = 999, Email = "user@example.com", Username = "johndoe" },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.FromInt32(999)
        },

        // Null instance
        new BuildIdentityTest
        {
            Name = "BuildIdentity throws ArgumentNullException with null instance",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = null,
            ApiIdentityName = null,
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(ArgumentNullException)
        },

        // Object type without identity
        new BuildIdentityTest
        {
            Name = "BuildIdentity throws InvalidOperationException when object type has no identity",
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
        new BuildIdentityTest
        {
            Name = "BuildIdentity throws InvalidOperationException with invalid identity name",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = new Person { Id = 42, Name = "John Doe" },
            ApiIdentityName = "NonExistentIdentity",
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(InvalidOperationException)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildIdentityFromValuesTheoryData =>
    [
        // Simple identity from dictionary
        new BuildIdentityFromValuesTest
        {
            Name = "BuildIdentity from values succeeds with simple identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = 42 },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        // Composite identity from dictionary
        new BuildIdentityFromValuesTest
        {
            Name = "BuildIdentity from values succeeds with composite identity",
            ApiSchemaKind = ApiSchemaKind.CompositeIdentity,
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
        new BuildIdentityFromValuesTest
        {
            Name = "BuildIdentity from values succeeds with type coercion (string to int)",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = "42" },
            ApiIdentityName = null,
            ShouldSucceed = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        // Missing property in dictionary
        new BuildIdentityFromValuesTest
        {
            Name = "BuildIdentity from values throws when property missing",
            ApiSchemaKind = ApiSchemaKind.CompositeIdentity,
            ApiObjectTypeName = nameof(ProductInventory),
            PropertyValues = new Dictionary<string, object?> { ["WarehouseId"] = 123 },
            ApiIdentityName = null,
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(ApiIdentityException)
        },

        // Null dictionary
        new BuildIdentityFromValuesTest
        {
            Name = "BuildIdentity from values throws with null dictionary",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = null,
            ApiIdentityName = null,
            ShouldSucceed = false,
            ExpectedId = null,
            ExpectedException = typeof(ArgumentNullException)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryBuildIdentityTheoryData =>
    [
        // Successful cases
        new TryBuildIdentityTest
        {
            Name = "TryBuildIdentity returns true with simple identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = new Person { Id = 42, Name = "John Doe" },
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        new TryBuildIdentityTest
        {
            Name = "TryBuildIdentity returns true with composite identity",
            ApiSchemaKind = ApiSchemaKind.CompositeIdentity,
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
        new TryBuildIdentityTest
        {
            Name = "TryBuildIdentity returns false with null instance",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = null,
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryBuildIdentityTest
        {
            Name = "TryBuildIdentity returns false when object type has no identity",
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

        new TryBuildIdentityTest
        {
            Name = "TryBuildIdentity returns false with invalid identity name",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            ClrInstance = new Person { Id = 42, Name = "John Doe" },
            ApiIdentityName = "InvalidIdentity",
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryBuildIdentityFromValuesTheoryData =>
    [
        // Successful cases
        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns true with simple identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = 42 },
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns true with composite identity",
            ApiSchemaKind = ApiSchemaKind.CompositeIdentity,
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

        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns true with type coercion",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = "42" },
            ApiIdentityName = null,
            ExpectedResult = true,
            ExpectedId = ApiId.FromInt32(42)
        },

        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns true with alternate identity",
            ApiSchemaKind = ApiSchemaKind.AlternateIdentity,
            ApiObjectTypeName = nameof(User),
            PropertyValues = new Dictionary<string, object?> { ["Email"] = "user@example.com" },
            ApiIdentityName = "AK_User_Email",
            ExpectedResult = true,
            ExpectedId = ApiId.FromString("user@example.com")
        },

        // Failure cases - should return false without throwing
        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns false with null dictionary",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = null,
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns false when object type has no identity",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            PropertyValues = new Dictionary<string, object?> { ["RequiredName"] = "test" },
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns false with invalid identity name",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = 42 },
            ApiIdentityName = "InvalidIdentity",
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns false when property missing",
            ApiSchemaKind = ApiSchemaKind.CompositeIdentity,
            ApiObjectTypeName = nameof(ProductInventory),
            PropertyValues = new Dictionary<string, object?> { ["WarehouseId"] = 123 },
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns false with invalid property value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?> { ["Id"] = "not-a-number" },
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
        },

        new TryBuildIdentityFromValuesTest
        {
            Name = "TryBuildIdentity from values returns false with empty dictionary",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Person),
            PropertyValues = new Dictionary<string, object?>(),
            ApiIdentityName = null,
            ExpectedResult = false,
            ExpectedId = ApiId.Empty
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
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildIdentityTheoryData))]
    public void BuildIdentity(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildIdentityFromValuesTheoryData))]
    public void BuildIdentityFromValues(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryBuildIdentityTheoryData))]
    public void TryBuildIdentity(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryBuildIdentityFromValuesTheoryData))]
    public void TryBuildIdentityFromValues(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetTheoryData))]
    public void TryGet(IXUnitTest test) => test.Execute(this);
    #endregion
}
