// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;

using Evoogle.ApiFramework.Key.Internal;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Key;

public partial class ApiKeyTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] FromScalarTheoryData =>
    [
        new FromScalarTest
        {
            Name = "FromString creates ApiKey with String kind",
            Factory = ApiKeyFromFactory.FromString,
            Value = "alpha",
            ExpectedApiKey = new ApiKey(ApiKeyKind.String, default, "alpha")
        },
        new FromScalarTest
        {
            Name = "FromInt32 creates ApiKey with Int32 kind",
            Factory = ApiKeyFromFactory.FromInt32,
            Value = "42",
            ExpectedApiKey = new ApiKey(ApiKeyKind.Int32, ApiKeyValueUnion.FromInt32(42), null)
        },
        new FromScalarTest
        {
            Name = "FromInt64 creates ApiKey with Int64 kind",
            Factory = ApiKeyFromFactory.FromInt64,
            Value = "8675309",
            ExpectedApiKey = new ApiKey(ApiKeyKind.Int64, ApiKeyValueUnion.FromInt64(8675309), null)
        },
        new FromScalarTest
        {
            Name = "FromGuid creates ApiKey with Guid kind",
            Factory = ApiKeyFromFactory.FromGuid,
            Value = TestGuidString,
            ExpectedApiKey = new ApiKey(ApiKeyKind.Guid, ApiKeyValueUnion.FromGuid(TestGuid), null)
        },
        new FromScalarTest
        {
            Name = "FromUlid creates ApiKey with Ulid kind",
            Factory = ApiKeyFromFactory.FromUlid,
            Value = TestUlidString,
            ExpectedApiKey = new ApiKey(ApiKeyKind.Ulid, ApiKeyValueUnion.FromUlid(TestUlid), null)
        },
        new FromScalarTest
        {
            Name = "FromCulture creates ApiKey with Culture kind",
            Factory = ApiKeyFromFactory.FromCulture,
            Value = "en-us",
            ExpectedApiKey = new ApiKey(ApiKeyKind.Culture, default, new CultureInfo("en-us"))
        }
    ];


    public static TheoryDataRow<IXUnitTest>[] TryParseTheoryData =>
    [
        // String
        new TryParseTest
        {
            Name = "TryParse returns true for valid String text",
            Kind = ApiKeyKind.String,
            Text = "orders/123",
            ExpectedResult = true,
            ExpectedApiKey = new ApiKey(ApiKeyKind.String, default, "orders/123")
        },
        new TryParseTest
        {
            Name = "TryParse returns false for null String text",
            Kind = ApiKeyKind.String,
            Text = null,
            ExpectedResult = false,
            ExpectedApiKey = null
        },
        new TryParseTest
        {
            Name = "TryParse returns false for empty String text",
            Kind = ApiKeyKind.String,
            Text = "",
            ExpectedResult = false,
            ExpectedApiKey = null
        },
        new TryParseTest
        {
            Name = "TryParse returns false for whitespace String text",
            Kind = ApiKeyKind.String,
            Text = "  ",
            ExpectedResult = false,
            ExpectedApiKey = null
        },
        // Int32
        new TryParseTest
        {
            Name = "TryParse returns true for valid Int32 text",
            Kind = ApiKeyKind.Int32,
            Text = "42",
            ExpectedResult = true,
            ExpectedApiKey = new ApiKey(ApiKeyKind.Int32, ApiKeyValueUnion.FromInt32(42), null)
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Int32 text",
            Kind = ApiKeyKind.Int32,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiKey = null
        },
        // Int64
        new TryParseTest
        {
            Name = "TryParse returns true for valid Int64 text",
            Kind = ApiKeyKind.Int64,
            Text = "42",
            ExpectedResult = true,
            ExpectedApiKey = new ApiKey(ApiKeyKind.Int64, ApiKeyValueUnion.FromInt64(42), null)
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Int64 text",
            Kind = ApiKeyKind.Int64,
            Text = "xyz",
            ExpectedResult = false,
            ExpectedApiKey = null
        },
        // Guid
        new TryParseTest
        {
            Name = "TryParse returns true for valid Guid text",
            Kind = ApiKeyKind.Guid,
            Text = TestGuidString,
            ExpectedResult = true,
            ExpectedApiKey = new ApiKey(ApiKeyKind.Guid, ApiKeyValueUnion.FromGuid(TestGuid), null)
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Guid text",
            Kind = ApiKeyKind.Guid,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiKey = null
        },
        // Ulid
        new TryParseTest
        {
            Name = "TryParse returns true for valid Ulid text",
            Kind = ApiKeyKind.Ulid,
            Text = TestUlidString,
            ExpectedResult = true,
            ExpectedApiKey = new ApiKey(ApiKeyKind.Ulid, ApiKeyValueUnion.FromUlid(TestUlid), null)
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Ulid text",
            Kind = ApiKeyKind.Ulid,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiKey = null
        },
        // Culture
        new TryParseTest
        {
            Name = "TryParse returns true for valid Culture text",
            Kind = ApiKeyKind.Culture,
            Text = "fr-FR",
            ExpectedResult = true,
            ExpectedApiKey = new ApiKey(ApiKeyKind.Culture, default, new CultureInfo("fr-FR"))
        },
        new TryParseTest
        {
            Name = "TryParse returns false for invalid Culture text",
            Kind = ApiKeyKind.Culture,
            Text = "abc",
            ExpectedResult = false,
            ExpectedApiKey = null
        },
        // Composite
        new TryParseTest
        {
            Name = "TryParse returns false for Composite kind",
            Kind = ApiKeyKind.Composite,
            Text = "part",
            ExpectedResult = false,
            ExpectedApiKey = null
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
