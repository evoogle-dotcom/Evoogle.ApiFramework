// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;

using Evoogle.ApiFramework.Identity.Internal;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] FromScalarTheoryData =>
    [
        new FromScalarTest
        {
            Name = "FromString creates ApiId with String kind",
            Factory = ApiIdFromFactory.FromString,
            Value = "alpha",
            ExpectedApiId = new ApiId(ApiIdKind.String, default, "alpha", "alpha")
        },
        new FromScalarTest
        {
            Name = "FromInt32 creates ApiId with Int32 kind",
            Factory = ApiIdFromFactory.FromInt32,
            Value = "42",
            ExpectedApiId = new ApiId(ApiIdKind.Int32, ApiIdValueUnion.FromInt32(42), null, "42")
        },
        new FromScalarTest
        {
            Name = "FromInt64 creates ApiId with Int64 kind",
            Factory = ApiIdFromFactory.FromInt64,
            Value = "8675309",
            ExpectedApiId = new ApiId(ApiIdKind.Int64, ApiIdValueUnion.FromInt64(8675309), null, "8675309")
        },
        new FromScalarTest
        {
            Name = "FromGuid creates ApiId with Guid kind",
            Factory = ApiIdFromFactory.FromGuid,
            Value = TestGuidString,
            ExpectedApiId = new ApiId(ApiIdKind.Guid, ApiIdValueUnion.FromGuid(TestGuid), null, TestGuidString)
        },
        new FromScalarTest
        {
            Name = "FromUlid creates ApiId with Ulid kind",
            Factory = ApiIdFromFactory.FromUlid,
            Value = TestUlidString,
            ExpectedApiId = new ApiId(ApiIdKind.Ulid, ApiIdValueUnion.FromUlid(TestUlid), null, TestUlidString)
        },
        new FromScalarTest
        {
            Name = "FromCulture creates ApiId with Culture kind",
            Factory = ApiIdFromFactory.FromCulture,
            Value = "en-us",
            ExpectedApiId = new ApiId(ApiIdKind.Culture, default, new CultureInfo("en-us"), "en-us")
        }
    ];


    public static TheoryDataRow<IXUnitTest>[] TryParseTheoryData =>
    [
        // String
        new TryParseTest
        {
            Name = "TryParse returns true for valid String text",
            Kind = ApiIdKind.String,
            Text = "orders/123",
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.String, default, "orders/123", "orders/123")
        },
        new TryParseTest
        {
            Name = "TryParse returns false for null String text",
            Kind = ApiIdKind.String,
            Text = null,
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "TryParse returns false for empty String text",
            Kind = ApiIdKind.String,
            Text = "",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "TryParse returns false for whitespace String text",
            Kind = ApiIdKind.String,
            Text = "  ",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        // Int32
        new TryParseTest
        {
            Name = "TryParse returns true for valid Int32 text",
            Kind = ApiIdKind.Int32,
            Text = "42",
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Int32, ApiIdValueUnion.FromInt32(42), null, "42")
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Int32 text",
            Kind = ApiIdKind.Int32,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        // Int64
        new TryParseTest
        {
            Name = "TryParse returns true for valid Int64 text",
            Kind = ApiIdKind.Int64,
            Text = "42",
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Int64, ApiIdValueUnion.FromInt64(42), null, "42")
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Int64 text",
            Kind = ApiIdKind.Int64,
            Text = "xyz",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        // Guid
        new TryParseTest
        {
            Name = "TryParse returns true for valid Guid text",
            Kind = ApiIdKind.Guid,
            Text = TestGuidString,
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Guid, ApiIdValueUnion.FromGuid(TestGuid), null, TestGuidString)
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Guid text",
            Kind = ApiIdKind.Guid,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        // Ulid
        new TryParseTest
        {
            Name = "TryParse returns true for valid Ulid text",
            Kind = ApiIdKind.Ulid,
            Text = TestUlidString,
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Ulid, ApiIdValueUnion.FromUlid(TestUlid), null, TestUlidString)
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Ulid text",
            Kind = ApiIdKind.Ulid,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        // Culture
        new TryParseTest
        {
            Name = "TryParse returns true for valid Culture text",
            Kind = ApiIdKind.Culture,
            Text = "fr-FR",
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Culture, default, new CultureInfo("fr-FR"), "fr-FR")
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Culture text",
            Kind = ApiIdKind.Culture,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        // Composite
        new TryParseTest
        {
            Name = "TryParse returns false for Composite kind",
            Kind = ApiIdKind.Composite,
            Text = "part",
            ExpectedResult = false,
            ExpectedApiId = null
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(FromScalarTheoryData))]
    public void FromScalar(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryParseTheoryData))]
    public void TryParse(IXUnitTest test) => test.Execute(this);
    #endregion
}
