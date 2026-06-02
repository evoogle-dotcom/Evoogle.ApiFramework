// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Key;

public partial class ApiKeyTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] CompositeTheoryData =>
    [
        // Named
        new CompositeTest
        {
            Name = "Named Composite from null parts",
            Factory = ApiKeyCompositeFactory.Named,
            PartCollection = null
        },
        new CompositeTest
        {
            Name = "Named Composite from empty parts",
            Factory = ApiKeyCompositeFactory.Named,
            PartCollection = []
        },
        new CompositeTest
        {
            Name = "Named Composite from Int32 and Int32 ApiKeys",
            Factory = ApiKeyCompositeFactory.Named,
            PartCollection =
            [
                ApiKeyPart.Create("id-1", ApiKey.FromInt32(42)),
                ApiKeyPart.Create("id-2", ApiKey.FromInt32(24)),
            ]
        },
        new CompositeTest
        {
            Name = "Named Composite from String and Culture ApiKeys",
            Factory = ApiKeyCompositeFactory.Named,
            PartCollection =
            [
                ApiKeyPart.Create("id", ApiKey.FromString("42")),
                ApiKeyPart.Create("locale", ApiKey.FromCulture("en-us")),
            ]
        },
        new CompositeTest
        {
            Name = "Named Composite from String and Int64 and Culture ApiKeys",
            Factory = ApiKeyCompositeFactory.Named,
            PartCollection =
            [
                ApiKeyPart.Create("id-string", ApiKey.FromString("42")),
                ApiKeyPart.Create("id-int64", ApiKey.FromInt64(42)),
                ApiKeyPart.Create("locale", ApiKey.FromCulture("en-us")),
            ]
        },

        // Ordered
        new CompositeTest
        {
            Name = "Ordered Composite from null parts",
            Factory = ApiKeyCompositeFactory.Ordered,
            KeyCollection = null
        },
        new CompositeTest
        {
            Name = "Ordered Composite from empty parts",
            Factory = ApiKeyCompositeFactory.Ordered,
            KeyCollection = []
        },
        new CompositeTest
        {
            Name = "Ordered Composite from Int32 and Int32 ApiKeys",
            Factory = ApiKeyCompositeFactory.Ordered,
            KeyCollection =
            [
                ApiKey.FromInt32(42),
                ApiKey.FromInt32(24),
            ]
        },
        new CompositeTest
        {
            Name = "Ordered Composite from String and Culture ApiKeys",
            Factory = ApiKeyCompositeFactory.Ordered,
            KeyCollection =
            [
                ApiKey.FromString("42"),
                ApiKey.FromCulture("en-us"),
            ]
        },
        new CompositeTest
        {
            Name = "Ordered Composite from String and Int64 and Culture ApiKeys",
            Factory = ApiKeyCompositeFactory.Ordered,
            KeyCollection =
            [
                ApiKey.FromString("42"),
                ApiKey.FromInt64(42),
                ApiKey.FromCulture("en-us"),
            ]
        },
    ];

    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(CompositeTheoryData))]
    public void Composite(IXUnitTest test) => test.Execute(this);
    #endregion
}
