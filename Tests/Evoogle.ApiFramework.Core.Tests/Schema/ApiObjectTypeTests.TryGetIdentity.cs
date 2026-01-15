// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiObjectTypeTests
{
    #region Theory Data
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
                new ApiIdPart("WarehouseId", ApiId.FromInt32(123)),
                new ApiIdPart("ProductCode", ApiId.FromString("PROD-456")),
                new ApiIdPart("BatchId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
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
                new ApiIdPart("WarehouseId", ApiId.FromInt32(123)),
                new ApiIdPart("ProductCode", ApiId.FromString("PROD-456")),
                new ApiIdPart("BatchId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
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
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TryGetIdentityTheoryData))]
    public void TryGetIdentity(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetIdentityFromValuesTheoryData))]
    public void TryGetIdentityFromValues(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetIdentitiesTheoryData))]
    public void TryGetIdentities(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetIdentityMapTheoryData))]
    public void TryGetIdentityMap(IXUnitTest test) => test.Execute(this);
    #endregion
}