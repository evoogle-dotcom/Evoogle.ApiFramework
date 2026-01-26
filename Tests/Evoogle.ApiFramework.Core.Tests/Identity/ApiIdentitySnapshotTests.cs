// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentitySnapshotTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Helper Methods
    private static ApiIdentityPartEntry ScalarEntry(string name, ApiId value) =>
        new(name, ApiIdentityPart.Scalar(value), Array.Empty<ApiIdentityPartEntry>());

    private static ApiIdentityPartEntry NestedEntry(string name, ApiIdentitySnapshot snapshot, IReadOnlyList<ApiIdentityPartEntry> nestedBlueprint) =>
        new(name, ApiIdentityPart.Nested(snapshot), nestedBlueprint);

    private static ApiIdentityPartEntry UnresolvedNestedEntry(string name, IReadOnlyList<ApiIdentityPartEntry> nestedBlueprint) =>
        new(name, ApiIdentityPart.UnresolvedNested(), nestedBlueprint);

    /// <summary>
    ///     Creates a sample nested order snapshot for testing navigation and flattening.
    /// </summary>
    private static ApiIdentitySnapshot CreateOrderSnapshot()
    {
        var countryBlueprint = new[]
        {
            ScalarEntry("Id", ApiId.FromInt32(1))
        };

        var countrySnapshot = ApiIdentitySnapshot.Composite("Country", countryBlueprint, "Order.Customer");

        var customerBlueprint = new[]
        {
            NestedEntry("Country", countrySnapshot, countryBlueprint),
            ScalarEntry("CustomerId", ApiId.FromInt32(42))
        };

        var customerSnapshot = ApiIdentitySnapshot.Composite("Customer", customerBlueprint, "Order");

        var orderBlueprint = new[]
        {
            NestedEntry("Customer", customerSnapshot, customerBlueprint),
            ScalarEntry("OrderNumber", ApiId.FromInt64(1001L))
        };

        return ApiIdentitySnapshot.Composite("Order", orderBlueprint);
    }

    /// <summary>
    ///     Creates a snapshot with unresolved (null) nested parts.
    /// </summary>
    private static ApiIdentitySnapshot CreateUnresolvedSnapshot()
    {
        var productBlueprint = new[]
        {
            ScalarEntry("ProductId", ApiId.FromInt32(99))
        };

        var product = ApiIdentitySnapshot.Composite("Product", productBlueprint, "Order");

        // Explicit unresolved nested snapshot for Customer.
        // Provide the nested blueprint so UseEmpty can flatten deterministically.
        var customerBlueprint = new[]
        {
            ScalarEntry("CustomerId", ApiId.Empty)
        };

        var orderBlueprint = new[]
        {
            UnresolvedNestedEntry("Customer", customerBlueprint),
            NestedEntry("Product", product, productBlueprint),
            ScalarEntry("OrderNumber", ApiId.FromInt64(1001L))
        };

        return ApiIdentitySnapshot.Composite("Order", orderBlueprint);
    }

    /// <summary>
    ///     Creates a deeply nested snapshot for complex navigation tests.
    /// </summary>
    private static ApiIdentitySnapshot CreateDeeplyNestedSnapshot()
    {
        var level2Blueprint = new[]
        {
            ScalarEntry("Level3", ApiId.FromString("deep")),
            ScalarEntry("Id", ApiId.FromInt32(100))
        };

        var level2 = ApiIdentitySnapshot.Composite("Level2", level2Blueprint, "Root.Level1");

        var level1Blueprint = new[]
        {
            NestedEntry("Level2", level2, level2Blueprint),
            ScalarEntry("Name", ApiId.FromString("test"))
        };

        var level1 = ApiIdentitySnapshot.Composite("Level1", level1Blueprint, "Root");

        var rootBlueprint = new[]
        {
            NestedEntry("Level1", level1, level1Blueprint),
            ScalarEntry("RootId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
        };

        return ApiIdentitySnapshot.Composite("Root", rootBlueprint);
    }
    #endregion
}
