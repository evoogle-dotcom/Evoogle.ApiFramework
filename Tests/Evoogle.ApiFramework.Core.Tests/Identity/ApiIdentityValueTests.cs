// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentityValueTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Data
    protected static ApiIdentityValue ScalarValue { get; } =
        ApiIdentityValue.Scalar("Id", ApiId.FromInt32(42));

    protected static ApiIdentityValue ScalarEmptyValue { get; } =
        ApiIdentityValue.Scalar("Id", ApiId.Empty);

    protected static ApiIdentityValue CompositeWithScalarParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiScalarIdentityPartValue("CustomerId", ApiId.FromInt32(42)),
            new ApiScalarIdentityPartValue("OrderNumber", ApiId.FromInt32(1001))
        ]);

    protected static ApiIdentityValue CompositeWithNestedParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiObjectIdentityPartValue("Customer",
                ApiIdentityValue.Composite(
                [
                    new ApiObjectIdentityPartValue("Country",
                        ApiIdentityValue.Composite(
                        [
                            new ApiScalarIdentityPartValue("Id", ApiId.FromInt32(1))
                        ])),
                    new ApiScalarIdentityPartValue("CustomerId", ApiId.FromInt32(42))
                ])),
            new ApiScalarIdentityPartValue("OrderNumber", ApiId.FromInt32(1001))
        ]);

    protected static ApiIdentityValue CompositeWithUnresolvedNestedParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiObjectIdentityPartValue("Customer", apiObjectValue: null, apiStructure:
            [
                new ApiObjectIdentityPartValue("Country", apiObjectValue: null, apiStructure:
                [
                    new ApiScalarIdentityPartValue("Id", ApiId.Empty)
                ]),
                new ApiScalarIdentityPartValue("CustomerId", ApiId.Empty)
            ]),
            new ApiScalarIdentityPartValue("OrderNumber", ApiId.Empty)
        ]);

    protected static ApiIdentityValue CompositeWithPartiallyUnresolvedNestedParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiObjectIdentityPartValue("Customer", apiObjectValue: null, apiStructure:
            [
                new ApiObjectIdentityPartValue("Country", apiObjectValue: null, apiStructure:
                [
                    new ApiScalarIdentityPartValue("Id", ApiId.Empty)
                ]),
                new ApiScalarIdentityPartValue("CustomerId", ApiId.Empty)
            ]),
            new ApiScalarIdentityPartValue("OrderNumber", ApiId.FromInt32(1001))
        ]);

    protected static ApiIdentityValue CompositeWithDeeplyNestedParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiObjectIdentityPartValue("Level1",
                ApiIdentityValue.Composite(
                [
                    new ApiObjectIdentityPartValue("Level2",
                        ApiIdentityValue.Composite(
                        [
                            new ApiObjectIdentityPartValue("Level3",
                                ApiIdentityValue.Composite(
                                [
                                    new ApiScalarIdentityPartValue("Level4", ApiId.FromString("DeepValue")),
                                    new ApiScalarIdentityPartValue("SequenceNumber", ApiId.FromInt32(42))
                                ]))
                        ]))
                ])),
            new ApiScalarIdentityPartValue("RootId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
        ]);
    #endregion
}
