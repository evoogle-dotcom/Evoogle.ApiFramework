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
            Name = "FromString",
            Factory = ApiIdFromFactory.FromString,
            Value = "alpha",
            ExpectedApiId = new ApiId(ApiIdKind.String, default, "alpha", "alpha")
        },
        new FromScalarTest
        {
            Name = "FromInt32",
            Factory = ApiIdFromFactory.FromInt32,
            Value = "42",
            ExpectedApiId = new ApiId(ApiIdKind.Int32, ApiIdValueUnion.FromInt32(42), null, "42")
        },
        new FromScalarTest
        {
            Name = "FromInt64",
            Factory = ApiIdFromFactory.FromInt64,
            Value = "8675309",
            ExpectedApiId = new ApiId(ApiIdKind.Int64, ApiIdValueUnion.FromInt64(8675309), null, "8675309")
        },
        new FromScalarTest
        {
            Name = "FromGuid",
            Factory = ApiIdFromFactory.FromGuid,
            Value = TestGuidString,
            ExpectedApiId = new ApiId(ApiIdKind.Guid, ApiIdValueUnion.FromGuid(TestGuid), null, TestGuidString)
        },
        new FromScalarTest
        {
            Name = "FromUlid",
            Factory = ApiIdFromFactory.FromUlid,
            Value = TestUlidString,
            ExpectedApiId = new ApiId(ApiIdKind.Ulid, ApiIdValueUnion.FromUlid(TestUlid), null, TestUlidString)
        },
        new FromScalarTest
        {
            Name = "FromCulture",
            Factory = ApiIdFromFactory.FromCulture,
            Value = "en-us",
            ExpectedApiId = new ApiId(ApiIdKind.Culture, default, new CultureInfo("en-us"), "en-us")
        }
    ];


    public static TheoryDataRow<IXUnitTest>[] TryParseTheoryData =>
    [
        new TryParseTest
        {
            Name = "String success",
            Kind = ApiIdKind.String,
            Text = "orders/123",
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.String, default, "orders/123", "orders/123")
        },
        new TryParseTest
        {
            Name = "String fails with null",
            Kind = ApiIdKind.String,
            Text = null,
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "String fails with empty string",
            Kind = ApiIdKind.String,
            Text = "",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "String fails with whitespace",
            Kind = ApiIdKind.String,
            Text = "  ",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "Int32 success",
            Kind = ApiIdKind.Int32,
            Text = "42",
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Int32, ApiIdValueUnion.FromInt32(42), null, "42")
        },
        new TryParseTest
        {
            Name = "Int32 fails with invalid text",
            Kind = ApiIdKind.Int32,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "Int64 success",
            Kind = ApiIdKind.Int64,
            Text = "42",
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Int64, ApiIdValueUnion.FromInt64(42), null, "42")
        },
        new TryParseTest
        {
            Name = "Int64 fails with invalid text",
            Kind = ApiIdKind.Int64,
            Text = "xyz",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "Guid success",
            Kind = ApiIdKind.Guid,
            Text = TestGuidString,
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Guid, ApiIdValueUnion.FromGuid(TestGuid), null, TestGuidString)
        },
        new TryParseTest
        {
            Name = "Guid fails with invalid text",
            Kind = ApiIdKind.Guid,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "Ulid success",
            Kind = ApiIdKind.Ulid,
            Text = TestUlidString,
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Ulid, ApiIdValueUnion.FromUlid(TestUlid), null, TestUlidString)
        },
        new TryParseTest
        {
            Name = "Ulid fails with invalid text",
            Kind = ApiIdKind.Ulid,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "Culture success",
            Kind = ApiIdKind.Culture,
            Text = "fr-FR",
            ExpectedResult = true,
            ExpectedApiId = new ApiId(ApiIdKind.Culture, default, new CultureInfo("fr-FR"), "fr-FR")
        },
        new TryParseTest
        {
            Name = "Culture fails with invalid text",
            Kind = ApiIdKind.Culture,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiId = null
        },
        new TryParseTest
        {
            Name = "Composite fails",
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
