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
    public static TheoryDataRow<IXUnitTest>[] ComparisonTheoryData =>
    [
        // Scalars
        new ComparisonTest
        {
            Name = $"{ApiKey.Empty.ToDebuggerDisplay()} CompareTo {ApiKey.Empty.ToDebuggerDisplay()} should be 0",
            Left = ApiKey.Empty,
            Right = ApiKey.Empty,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{ApiKey.Empty.ToDebuggerDisplay()} CompareTo {TestStringAlphaApiKey.ToDebuggerDisplay()} should be -1",
            Left = ApiKey.Empty,
            Right = TestStringAlphaApiKey,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestStringAlphaApiKey.ToDebuggerDisplay()} CompareTo {TestStringAlphaApiKey.ToDebuggerDisplay()} should be 0",
            Left = TestStringAlphaApiKey,
            Right = TestStringAlphaApiKey,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{TestStringAlphaApiKey.ToDebuggerDisplay()} CompareTo {TestStringALPHAApiKey.ToDebuggerDisplay()} should be +1",
            Left = TestStringAlphaApiKey,
            Right = TestStringALPHAApiKey,
            ExpectedSign = +1
        },
        new ComparisonTest
        {
            Name = $"{TestStringAlphaApiKey.ToDebuggerDisplay()} CompareTo {TestStringBetaApiKey.ToDebuggerDisplay()} should be -1",
            Left = TestStringAlphaApiKey,
            Right = TestStringBetaApiKey,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestInt42ApiKey.ToDebuggerDisplay()} CompareTo {TestInt42ApiKey.ToDebuggerDisplay()} should be 0",
            Left = TestInt42ApiKey,
            Right = TestInt42ApiKey,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{TestInt42ApiKey.ToDebuggerDisplay()} CompareTo {TestInt24ApiKey.ToDebuggerDisplay()} should be +1",
            Left = TestInt42ApiKey,
            Right = TestInt24ApiKey,
            ExpectedSign = +1
        },
        new ComparisonTest
        {
            Name = $"{TestInt24ApiKey.ToDebuggerDisplay()} CompareTo {TestInt42ApiKey.ToDebuggerDisplay()} should be -1",
            Left = TestInt24ApiKey,
            Right = TestInt42ApiKey,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestInt42ApiKey.ToDebuggerDisplay()} CompareTo {TestLong42ApiKey.ToDebuggerDisplay()} should be -1 based on kind ordinal comparison",
            Left = TestInt42ApiKey,
            Right = TestLong42ApiKey,
            ExpectedSign = -1
        },

        // Ordered Composites (unnamed parts)
        new ComparisonTest
        {
            Name = $"{TestCompositeInt24AndInt24ApiKey.ToDebuggerDisplay()} CompareTo {TestCompositeInt24AndInt24ApiKey.ToDebuggerDisplay()} should be 0",
            Left = TestCompositeInt24AndInt24ApiKey,
            Right = TestCompositeInt24AndInt24ApiKey,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeInt24AndInt24ApiKey.ToDebuggerDisplay()} CompareTo {TestCompositeInt24AndInt42ApiKey.ToDebuggerDisplay()} should be -1 based on part values",
            Left = TestCompositeInt24AndInt24ApiKey,
            Right = TestCompositeInt24AndInt42ApiKey,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeInt24AndInt42ApiKey.ToDebuggerDisplay()} CompareTo {TestCompositeInt24AndInt24ApiKey.ToDebuggerDisplay()} should be +1 based on part values",
            Left = TestCompositeInt24AndInt42ApiKey,
            Right = TestCompositeInt24AndInt24ApiKey,
            ExpectedSign = +1
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeInt24AndInt42ApiKey.ToDebuggerDisplay()} CompareTo {TestCompositeInt24AndInt42AndInt48ApiKey.ToDebuggerDisplay()} should be -1 based on part count",
            Left = TestCompositeInt24AndInt42ApiKey,
            Right = TestCompositeInt24AndInt42AndInt48ApiKey,
            ExpectedSign = -1
        },

        // Named Composites (named parts)
        new ComparisonTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt42ApiKey.ToDebuggerDisplay()} CompareTo {TestCompositeAlphaInt24AndBetaInt42ApiKey.ToDebuggerDisplay()} should be 0",
            Left = TestCompositeAlphaInt24AndBetaInt42ApiKey,
            Right = TestCompositeAlphaInt24AndBetaInt42ApiKey,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt24ApiKey.ToDebuggerDisplay()} CompareTo {TestCompositeAlphaInt24AndBetaInt42ApiKey.ToDebuggerDisplay()} should be -1 based on part values",
            Left = TestCompositeAlphaInt24AndBetaInt24ApiKey,
            Right = TestCompositeAlphaInt24AndBetaInt42ApiKey,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeAlphaInt24AndZetaInt42ApiKey.ToDebuggerDisplay()} CompareTo {TestCompositeAlphaInt24AndBetaInt42ApiKey.ToDebuggerDisplay()} should be +1 based on part names",
            Left = TestCompositeAlphaInt24AndZetaInt42ApiKey,
            Right = TestCompositeAlphaInt24AndBetaInt42ApiKey,
            ExpectedSign = +1
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] EqualityTheoryData =>
    [
        // Scalars
        new EqualityTest
        {
            Name = $"{ApiKey.Empty.ToDebuggerDisplay()} Equals {ApiKey.Empty.ToDebuggerDisplay()} should be true",
            Left = ApiKey.Empty,
            Right = ApiKey.Empty,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{ApiKey.Empty.ToDebuggerDisplay()} Equals {TestStringAlphaApiKey.ToDebuggerDisplay()} should be false",
            Left = ApiKey.Empty,
            Right = TestStringAlphaApiKey,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestStringAlphaApiKey.ToDebuggerDisplay()} Equals {TestStringAlphaApiKey.ToDebuggerDisplay()} should be true",
            Left = TestStringAlphaApiKey,
            Right = TestStringAlphaApiKey,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestStringALPHAApiKey.ToDebuggerDisplay()} Equals {TestStringAlphaApiKey.ToDebuggerDisplay()} should be false",
            Left = TestStringALPHAApiKey,
            Right = TestStringAlphaApiKey,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestInt42ApiKey.ToDebuggerDisplay()} Equals {TestInt42ApiKey.ToDebuggerDisplay()} should be true",
            Left = TestInt42ApiKey,
            Right = TestInt42ApiKey,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestInt42ApiKey.ToDebuggerDisplay()} Equals {TestInt24ApiKey.ToDebuggerDisplay()} should be false",
            Left = TestInt42ApiKey,
            Right = TestInt24ApiKey,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestCultureEnUsApiKey.ToDebuggerDisplay()} Equals {TestCultureEnUsApiKey.ToDebuggerDisplay()} should be true",
            Left = TestCultureEnUsApiKey,
            Right = TestCultureEnUsApiKey,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestCultureEnUsApiKey.ToDebuggerDisplay()} Equals {TestCultureFrFrApiKey.ToDebuggerDisplay()} should be false",
            Left = TestCultureEnUsApiKey,
            Right = TestCultureFrFrApiKey,
            ExpectedEqual = false
        },

        // Ordered Composites (unnamed parts)
        new EqualityTest
        {
            Name = $"{TestCompositeInt24AndInt42ApiKey.ToDebuggerDisplay()} Equals {TestCompositeInt24AndInt42ApiKey.ToDebuggerDisplay()} should be true",
            Left = TestCompositeInt24AndInt42ApiKey,
            Right = TestCompositeInt24AndInt42ApiKey,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestCompositeInt24AndInt24ApiKey.ToDebuggerDisplay()} Equals {TestCompositeInt24AndInt42ApiKey.ToDebuggerDisplay()} should be false based on part values",
            Left = TestCompositeInt24AndInt24ApiKey,
            Right = TestCompositeInt24AndInt42ApiKey,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestCompositeInt24AndInt42ApiKey.ToDebuggerDisplay()} Equals {TestCompositeInt24AndInt42AndInt48ApiKey.ToDebuggerDisplay()} should be false based on part count",
            Left = TestCompositeInt24AndInt42ApiKey,
            Right = TestCompositeInt24AndInt42AndInt48ApiKey,
            ExpectedEqual = false
        },

        // Named Composites (named parts)
        new EqualityTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt42ApiKey.ToDebuggerDisplay()} Equals {TestCompositeAlphaInt24AndBetaInt42ApiKey.ToDebuggerDisplay()} should be true",
            Left = TestCompositeAlphaInt24AndBetaInt42ApiKey,
            Right = TestCompositeAlphaInt24AndBetaInt42ApiKey,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt42ApiKey.ToDebuggerDisplay()} Equals {TestCompositeAlphaInt24AndZetaInt42ApiKey.ToDebuggerDisplay()} should be false based on part names",
            Left = TestCompositeAlphaInt24AndBetaInt42ApiKey,
            Right = TestCompositeAlphaInt24AndZetaInt42ApiKey,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt24ApiKey.ToDebuggerDisplay()} Equals {TestCompositeAlphaInt24AndZetaInt42ApiKey.ToDebuggerDisplay()} should be false based on part values",
            Left = TestCompositeAlphaInt24AndBetaInt24ApiKey,
            Right = TestCompositeAlphaInt24AndZetaInt42ApiKey,
            ExpectedEqual = false
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ComparisonTheoryData))]
    public void Comparison(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(EqualityTheoryData))]
    public void Equality(IXUnitTest test) => test.Execute(this);
    #endregion
}
