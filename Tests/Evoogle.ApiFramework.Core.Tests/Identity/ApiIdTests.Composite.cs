// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] CompositeTheoryData =>
    [
        // Named
        new CompositeTest
        {
            Name = "Named Composite from null parts",
            Factory = ApiIdCompositeFactory.Named,
            PartCollection = null
        },
        new CompositeTest
        {
            Name = "Named Composite from empty parts",
            Factory = ApiIdCompositeFactory.Named,
            PartCollection = []
        },
        new CompositeTest
        {
            Name = "Named Composite from Int32 and Int32 ApiIds",
            Factory = ApiIdCompositeFactory.Named,
            PartCollection =
            [
                ApiIdPart.Create("id-1", ApiId.FromInt32(42)),
                ApiIdPart.Create("id-2", ApiId.FromInt32(24)),
            ]
        },
        new CompositeTest
        {
            Name = "Named Composite from String and Culture ApiIds",
            Factory = ApiIdCompositeFactory.Named,
            PartCollection =
            [
                ApiIdPart.Create("id", ApiId.FromString("42")),
                ApiIdPart.Create("locale", ApiId.FromCulture("en-us")),
            ]
        },
        new CompositeTest
        {
            Name = "Named Composite from String and Int64 and Culture ApiIds",
            Factory = ApiIdCompositeFactory.Named,
            PartCollection =
            [
                ApiIdPart.Create("id-string", ApiId.FromString("42")),
                ApiIdPart.Create("id-int64", ApiId.FromInt64(42)),
                ApiIdPart.Create("locale", ApiId.FromCulture("en-us")),
            ]
        },

        // Ordered
        new CompositeTest
        {
            Name = "Ordered Composite from null parts",
            Factory = ApiIdCompositeFactory.Ordered,
            IdCollection = null
        },
        new CompositeTest
        {
            Name = "Ordered Composite from empty parts",
            Factory = ApiIdCompositeFactory.Ordered,
            IdCollection = []
        },
        new CompositeTest
        {
            Name = "Ordered Composite from Int32 and Int32 ApiIds",
            Factory = ApiIdCompositeFactory.Ordered,
            IdCollection =
            [
                ApiId.FromInt32(42),
                ApiId.FromInt32(24),
            ]
        },
        new CompositeTest
        {
            Name = "Ordered Composite from String and Culture ApiIds",
            Factory = ApiIdCompositeFactory.Ordered,
            IdCollection =
            [
                ApiId.FromString("42"),
                ApiId.FromCulture("en-us"),
            ]
        },
        new CompositeTest
        {
            Name = "Ordered Composite from String and Int64 and Culture ApiIds",
            Factory = ApiIdCompositeFactory.Ordered,
            IdCollection =
            [
                ApiId.FromString("42"),
                ApiId.FromInt64(42),
                ApiId.FromCulture("en-us"),
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
