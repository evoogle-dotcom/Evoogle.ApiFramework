// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiObjectTypeTests
{
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
                new ApiIdPart("WarehouseId", ApiId.FromInt32(123)),
                new ApiIdPart("ProductCode", ApiId.FromString("PROD-456")),
                new ApiIdPart("BatchId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
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
                new ApiIdPart("WarehouseId", ApiId.FromInt32(123)),
                new ApiIdPart("ProductCode", ApiId.FromString("PROD-456")),
                new ApiIdPart("BatchId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
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

    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(GetIdentityTheoryData))]
    public void GetIdentity(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(GetIdentityFromValuesTheoryData))]
    public void GetIdentityFromValues(IXUnitTest test) => test.Execute(this);
    #endregion
}
