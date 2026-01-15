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
    public static TheoryDataRow<IXUnitTest>[] ComparisonTheoryData =>
    [
        // Scalars
        new ComparisonTest
        {
            Name = $"{ApiId.Empty.ToDebuggerDisplay()} CompareTo {ApiId.Empty.ToDebuggerDisplay()} should be 0",
            Left = ApiId.Empty,
            Right = ApiId.Empty,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{ApiId.Empty.ToDebuggerDisplay()} CompareTo {TestStringAlphaApiId.ToDebuggerDisplay()} should be -1",
            Left = ApiId.Empty,
            Right = TestStringAlphaApiId,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestStringAlphaApiId.ToDebuggerDisplay()} CompareTo {TestStringAlphaApiId.ToDebuggerDisplay()} should be 0",
            Left = TestStringAlphaApiId,
            Right = TestStringAlphaApiId,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{TestStringAlphaApiId.ToDebuggerDisplay()} CompareTo {TestStringALPHAApiId.ToDebuggerDisplay()} should be +1",
            Left = TestStringAlphaApiId,
            Right = TestStringALPHAApiId,
            ExpectedSign = +1
        },
        new ComparisonTest
        {
            Name = $"{TestStringAlphaApiId.ToDebuggerDisplay()} CompareTo {TestStringBetaApiId.ToDebuggerDisplay()} should be -1",
            Left = TestStringAlphaApiId,
            Right = TestStringBetaApiId,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestInt42ApiId.ToDebuggerDisplay()} CompareTo {TestInt42ApiId.ToDebuggerDisplay()} should be 0",
            Left = TestInt42ApiId,
            Right = TestInt42ApiId,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{TestInt42ApiId.ToDebuggerDisplay()} CompareTo {TestInt24ApiId.ToDebuggerDisplay()} should be +1",
            Left = TestInt42ApiId,
            Right = TestInt24ApiId,
            ExpectedSign = +1
        },
        new ComparisonTest
        {
            Name = $"{TestInt24ApiId.ToDebuggerDisplay()} CompareTo {TestInt42ApiId.ToDebuggerDisplay()} should be -1",
            Left = TestInt24ApiId,
            Right = TestInt42ApiId,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestInt42ApiId.ToDebuggerDisplay()} CompareTo {TestLong42ApiId.ToDebuggerDisplay()} should be -1 based on kind ordinal comparison",
            Left = TestInt42ApiId,
            Right = TestLong42ApiId,
            ExpectedSign = -1
        },

        // Ordered Composites (unnamed parts)
        new ComparisonTest
        {
            Name = $"{TestCompositeInt24AndInt24ApiId.ToDebuggerDisplay()} CompareTo {TestCompositeInt24AndInt24ApiId.ToDebuggerDisplay()} should be 0",
            Left = TestCompositeInt24AndInt24ApiId,
            Right = TestCompositeInt24AndInt24ApiId,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeInt24AndInt24ApiId.ToDebuggerDisplay()} CompareTo {TestCompositeInt24AndInt42ApiId.ToDebuggerDisplay()} should be -1 based on part values",
            Left = TestCompositeInt24AndInt24ApiId,
            Right = TestCompositeInt24AndInt42ApiId,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeInt24AndInt42ApiId.ToDebuggerDisplay()} CompareTo {TestCompositeInt24AndInt24ApiId.ToDebuggerDisplay()} should be +1 based on part values",
            Left = TestCompositeInt24AndInt42ApiId,
            Right = TestCompositeInt24AndInt24ApiId,
            ExpectedSign = +1
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeInt24AndInt42ApiId.ToDebuggerDisplay()} CompareTo {TestCompositeInt24AndInt42AndInt48ApiId.ToDebuggerDisplay()} should be -1 based on part count",
            Left = TestCompositeInt24AndInt42ApiId,
            Right = TestCompositeInt24AndInt42AndInt48ApiId,
            ExpectedSign = -1
        },

        // Named Composites (named parts)
        new ComparisonTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt42ApiId.ToDebuggerDisplay()} CompareTo {TestCompositeAlphaInt24AndBetaInt42ApiId.ToDebuggerDisplay()} should be 0",
            Left = TestCompositeAlphaInt24AndBetaInt42ApiId,
            Right = TestCompositeAlphaInt24AndBetaInt42ApiId,
            ExpectedSign = 0
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt24ApiId.ToDebuggerDisplay()} CompareTo {TestCompositeAlphaInt24AndBetaInt42ApiId.ToDebuggerDisplay()} should be -1 based on part values",
            Left = TestCompositeAlphaInt24AndBetaInt24ApiId,
            Right = TestCompositeAlphaInt24AndBetaInt42ApiId,
            ExpectedSign = -1
        },
        new ComparisonTest
        {
            Name = $"{TestCompositeAlphaInt24AndZetaInt42ApiId.ToDebuggerDisplay()} CompareTo {TestCompositeAlphaInt24AndBetaInt42ApiId.ToDebuggerDisplay()} should be +1 based on part names",
            Left = TestCompositeAlphaInt24AndZetaInt42ApiId,
            Right = TestCompositeAlphaInt24AndBetaInt42ApiId,
            ExpectedSign = +1
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] EqualityTheoryData =>
    [
        // Scalars
        new EqualityTest
        {
            Name = $"{ApiId.Empty.ToDebuggerDisplay()} Equals {ApiId.Empty.ToDebuggerDisplay()} should be true",
            Left = ApiId.Empty,
            Right = ApiId.Empty,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{ApiId.Empty.ToDebuggerDisplay()} Equals {TestStringAlphaApiId.ToDebuggerDisplay()} should be false",
            Left = ApiId.Empty,
            Right = TestStringAlphaApiId,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestStringAlphaApiId.ToDebuggerDisplay()} Equals {TestStringAlphaApiId.ToDebuggerDisplay()} should be true",
            Left = TestStringAlphaApiId,
            Right = TestStringAlphaApiId,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestStringALPHAApiId.ToDebuggerDisplay()} Equals {TestStringAlphaApiId.ToDebuggerDisplay()} should be false",
            Left = TestStringALPHAApiId,
            Right = TestStringAlphaApiId,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestInt42ApiId.ToDebuggerDisplay()} Equals {TestInt42ApiId.ToDebuggerDisplay()} should be true",
            Left = TestInt42ApiId,
            Right = TestInt42ApiId,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestInt42ApiId.ToDebuggerDisplay()} Equals {TestInt24ApiId.ToDebuggerDisplay()} should be false",
            Left = TestInt42ApiId,
            Right = TestInt24ApiId,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestCultureEnUsApiId.ToDebuggerDisplay()} Equals {TestCultureEnUsApiId.ToDebuggerDisplay()} should be true",
            Left = TestCultureEnUsApiId,
            Right = TestCultureEnUsApiId,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestCultureEnUsApiId.ToDebuggerDisplay()} Equals {TestCultureFrFrApiId.ToDebuggerDisplay()} should be false",
            Left = TestCultureEnUsApiId,
            Right = TestCultureFrFrApiId,
            ExpectedEqual = false
        },

        // Ordered Composites (unnamed parts)
        new EqualityTest
        {
            Name = $"{TestCompositeInt24AndInt42ApiId.ToDebuggerDisplay()} Equals {TestCompositeInt24AndInt42ApiId.ToDebuggerDisplay()} should be true",
            Left = TestCompositeInt24AndInt42ApiId,
            Right = TestCompositeInt24AndInt42ApiId,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestCompositeInt24AndInt24ApiId.ToDebuggerDisplay()} Equals {TestCompositeInt24AndInt42ApiId.ToDebuggerDisplay()} should be false based on part values",
            Left = TestCompositeInt24AndInt24ApiId,
            Right = TestCompositeInt24AndInt42ApiId,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestCompositeInt24AndInt42ApiId.ToDebuggerDisplay()} Equals {TestCompositeInt24AndInt42AndInt48ApiId.ToDebuggerDisplay()} should be false based on part count",
            Left = TestCompositeInt24AndInt42ApiId,
            Right = TestCompositeInt24AndInt42AndInt48ApiId,
            ExpectedEqual = false
        },

        // Named Composites (named parts)
        new EqualityTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt42ApiId.ToDebuggerDisplay()} Equals {TestCompositeAlphaInt24AndBetaInt42ApiId.ToDebuggerDisplay()} should be true",
            Left = TestCompositeAlphaInt24AndBetaInt42ApiId,
            Right = TestCompositeAlphaInt24AndBetaInt42ApiId,
            ExpectedEqual = true
        },
        new EqualityTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt42ApiId.ToDebuggerDisplay()} Equals {TestCompositeAlphaInt24AndZetaInt42ApiId.ToDebuggerDisplay()} should be false based on part names",
            Left = TestCompositeAlphaInt24AndBetaInt42ApiId,
            Right = TestCompositeAlphaInt24AndZetaInt42ApiId,
            ExpectedEqual = false
        },
        new EqualityTest
        {
            Name = $"{TestCompositeAlphaInt24AndBetaInt24ApiId.ToDebuggerDisplay()} Equals {TestCompositeAlphaInt24AndZetaInt42ApiId.ToDebuggerDisplay()} should be false based on part values",
            Left = TestCompositeAlphaInt24AndBetaInt24ApiId,
            Right = TestCompositeAlphaInt24AndZetaInt42ApiId,
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
