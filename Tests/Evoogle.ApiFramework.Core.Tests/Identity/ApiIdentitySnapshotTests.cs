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
    /// <summary>
    ///     Creates a sample nested order snapshot for testing navigation and flattening.
    /// </summary>
    private static ApiIdentitySnapshot CreateOrderSnapshot()
    {
        // CountrySnapshot (nested 2 levels deep)
        var countrySnapshot = ApiIdentitySnapshot.Scalar("Country", ApiId.FromInt32(1), "Customer");

        // CustomerSnapshot (nested 1 level deep)
        var customerSnapshot = ApiIdentitySnapshot.Composite("Customer", new Dictionary<string, ApiIdentityPart>
        {
            ["Country"] = ApiIdentityPart.Nested(countrySnapshot),
            ["CustomerId"] = ApiIdentityPart.Scalar(ApiId.FromInt32(42))
        }, "Order");

        // OrderSnapshot (root)
        return ApiIdentitySnapshot.Composite("Order", new Dictionary<string, ApiIdentityPart>
        {
            ["Customer"] = ApiIdentityPart.Nested(customerSnapshot),
            ["OrderNumber"] = ApiIdentityPart.Scalar(ApiId.FromInt64(1001L))
        });
    }

    /// <summary>
    ///     Creates a snapshot with unresolved (null) nested parts.
    /// </summary>
    private static ApiIdentitySnapshot CreateUnresolvedSnapshot()
    {
        // Include a non-null ApiIdentitySnapshot to trigger the heuristic
        var product = ApiIdentitySnapshot.Scalar("Product", ApiId.FromInt32(99), "Order");

        return ApiIdentitySnapshot.Composite("Order", new Dictionary<string, ApiIdentityPart>
        {
            ["Customer"] = ApiIdentityPart.UnresolvedNested(),  // Explicit null nested snapshot
            ["Product"] = ApiIdentityPart.Nested(product),  // Non-null snapshot to enable null heuristic
            ["OrderNumber"] = ApiIdentityPart.Scalar(ApiId.FromInt64(1001L))
        });
    }

    /// <summary>
    ///     Creates a deeply nested snapshot for complex navigation tests.
    /// </summary>
    private static ApiIdentitySnapshot CreateDeeplyNestedSnapshot()
    {
        var level3 = ApiIdentitySnapshot.Scalar("Level3", ApiId.FromString("deep"), "Root.Level1.Level2");

        var level2 = ApiIdentitySnapshot.Composite("Level2", new Dictionary<string, ApiIdentityPart>
        {
            ["Level3"] = ApiIdentityPart.Nested(level3),
            ["Id"] = ApiIdentityPart.Scalar(ApiId.FromInt32(100))
        }, "Root.Level1");

        var level1 = ApiIdentitySnapshot.Composite("Level1", new Dictionary<string, ApiIdentityPart>
        {
            ["Level2"] = ApiIdentityPart.Nested(level2),
            ["Name"] = ApiIdentityPart.Scalar(ApiId.FromString("test"))
        }, "Root");

        return ApiIdentitySnapshot.Composite("Root", new Dictionary<string, ApiIdentityPart>
        {
            ["Level1"] = ApiIdentityPart.Nested(level1),
            ["RootId"] = ApiIdentityPart.Scalar(ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
        });
    }
    #endregion
}
