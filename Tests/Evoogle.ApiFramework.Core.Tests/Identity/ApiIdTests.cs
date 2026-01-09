// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Evoogle.ApiFramework.Identity.Internal;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Identity;

public class ApiIdTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum ApiIdFromFactory
    {
        FromString,
        FromInt32,
        FromInt64,
        FromGuid,
        FromUlid,
        FromCulture
    }

    private enum ApiIdCompositeFactory
    {
        Named,
        Ordered
    }

    private record ApiIdDescriptor
    (
        ApiIdScalarConfig? ScalarConfig = null,
        ApiIdCompositePartConfig[]? CompositePartsConfig = null
    );

    private record ApiIdScalarConfig
    (
        ApiIdKind Kind,

        string? CultureValue = null,
        Guid? GuidValue = null,
        int? Int32Value = null,
        long? Int64Value = null,
        string? StringValue = null,
        Ulid? UlidValue = null
    );

    private record ApiIdCompositePartConfig
    (
        ApiIdScalarConfig ScalarConfig,
        string? Name = null
    );

    private class ComparisonTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiId Left { get; init; }
        public ApiId Right { get; init; }
        public int ExpectedSign { get; init; }
        #endregion

        #region Calculated Properties
        private int ActualLeftToRightCompareToMethod { get; set; }
        private int ActualRightToLeftCompareToMethod { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Left:  {this.Left.SafeToString()}");
            this.WriteLine($"Right: {this.Right.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected Sign: {this.ExpectedSign}");
        }

        protected override void Act()
        {
            this.ActualLeftToRightCompareToMethod = this.Left.CompareTo(this.Right);
            this.ActualRightToLeftCompareToMethod = this.Right.CompareTo(this.Left);

            this.WriteLine($"Actual LeftToRight CompareTo Method: {this.ActualLeftToRightCompareToMethod}");
            this.WriteLine($"Actual RightToLeft CompareTo Method: {this.ActualRightToLeftCompareToMethod}");
        }

        protected override void Assert()
        {
            this.ActualLeftToRightCompareToMethod.Should().Be(this.ExpectedSign, "CompareTo should return the expected ordering");
            this.ActualRightToLeftCompareToMethod.Should().Be(-this.ExpectedSign, "CompareTo should be antisymmetric");
        }
        #endregion
    }

    private class CompositeTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdCompositeFactory Factory { get; init; }
        public ApiId[]? IdCollection { get; init; } = null;
        public ApiIdPart[]? PartCollection { get; init; } = null;
        #endregion

        #region Calculated Properties
        private ApiId ExpectedApiId { get; set; }
        private ApiId ActualApiId { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Factory:        {this.Factory}");
            this.WriteLine($"IdCollection:   [{this.IdCollection.SafeToDelimitedString(',')}]");
            this.WriteLine($"PartCollection: [{this.PartCollection.SafeToDelimitedString(',')}]");
            this.WriteLine();

            this.ExpectedApiId = this.CreateExpectedApiId();

            this.WriteLine($"Expected ApiId: {this.ExpectedApiId.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualApiId = this.CreateActualApiId();

            this.WriteLine($"Actual ApiId:   {this.ActualApiId.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualApiId.Should().BeEquivalentTo(this.ExpectedApiId);
        }
        #endregion

        #region Factory Methods
        private ApiId CreateActualApiId()
            => this.Factory switch
            {
                ApiIdCompositeFactory.Named => ApiId.Composite(this.PartCollection!),
                ApiIdCompositeFactory.Ordered => ApiId.Composite(this.IdCollection!),
                _ => throw new InvalidOperationException($"Unsupported factory {this.Factory}.")
            };

        private ApiId CreateExpectedApiId()
        {
            var partCollection = default(ApiIdPart[]?);
            switch (this.Factory)
            {
                case ApiIdCompositeFactory.Named:
                    {
                        if (this.PartCollection is null || this.PartCollection.Length == 0)
                        {
                            return ApiId.Empty;
                        }

                        partCollection = this.PartCollection!;
                        break;
                    }

                case ApiIdCompositeFactory.Ordered:
                    {
                        if (this.IdCollection is null || this.IdCollection.Length == 0)
                        {
                            return ApiId.Empty;
                        }

                        partCollection = [.. this.IdCollection.Select(id => new ApiIdPart(null, id))];
                        break;
                    }
            }

            if (partCollection is not null)
            {
                var apiId = new ApiId
                (
                    ApiIdKind.Composite,
                    default,
                    partCollection,
                    ApiId.ToDebugString(partCollection)
                );
                return apiId;
            }

            throw new InvalidOperationException($"Unsupported factory {this.Factory}.");
        }
        #endregion
    }

    private class EqualityTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiId Left { get; init; }
        public ApiId Right { get; init; }
        public bool ExpectedEqual { get; init; }
        #endregion

        #region Calculated Properties
        private bool ActualLeftToRightEqualsMethod { get; set; }
        private bool ActualRightToLeftEqualsMethod { get; set; }
        private bool ActualLeftToRightEqualsOperator { get; set; }
        private bool ActualLeftToRightNotEqualsOperator { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Left:  {this.Left.SafeToString()}");
            this.WriteLine($"Right: {this.Right.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected Equal: {this.ExpectedEqual}");
        }

        protected override void Act()
        {
            this.ActualLeftToRightEqualsMethod = this.Left.Equals(this.Right);
            this.ActualRightToLeftEqualsMethod = this.Right.Equals(this.Left);
            this.ActualLeftToRightEqualsOperator = this.Left == this.Right;
            this.ActualLeftToRightNotEqualsOperator = this.Left != this.Right;

            this.WriteLine($"Actual LeftToRight Equals Method:      {this.ActualLeftToRightEqualsMethod}");
            this.WriteLine($"Actual RightToLeft Equals Method:      {this.ActualRightToLeftEqualsMethod}");
            this.WriteLine($"Actual LeftToRight Equals Operator:    {this.ActualLeftToRightEqualsOperator}");
            this.WriteLine($"Actual LeftToRight NotEquals Operator: {this.ActualLeftToRightNotEqualsOperator}");
        }

        protected override void Assert()
        {
            this.ActualLeftToRightEqualsMethod.Should().Be(this.ExpectedEqual, "Equals should match expectation");
            this.ActualRightToLeftEqualsMethod.Should().Be(this.ExpectedEqual, "Equals should be symmetric");
            this.ActualLeftToRightEqualsOperator.Should().Be(this.ExpectedEqual, "operator == should match expectation");
            this.ActualLeftToRightNotEqualsOperator.Should().Be(!this.ExpectedEqual, "operator != should match expectation");

            if (this.ExpectedEqual)
            {
                this.Left.GetHashCode().Should().Be(this.Right.GetHashCode(), "equal values must produce identical hashes");
            }
        }
        #endregion
    }

    private class FromScalarTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdFromFactory Factory { get; init; }
        public string Value { get; init; } = null!;
        public ApiId ExpectedApiId { get; init; }
        #endregion

        #region Calculated Properties
        private ApiId ActualApiId { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Factory: {this.Factory}");
            this.WriteLine($"Value:   {this.Value.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected ApiId: {this.ExpectedApiId.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualApiId = this.CreateApiId();

            this.WriteLine($"Actual ApiId:   {this.ActualApiId.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualApiId.Should().BeEquivalentTo(this.ExpectedApiId);
        }
        #endregion

        #region Factory Methods
        private ApiId CreateApiId()
            => this.Factory switch
            {
                ApiIdFromFactory.FromString => ApiId.FromString(this.Value),
                ApiIdFromFactory.FromInt32 => ApiId.FromInt32(int.Parse(this.Value, CultureInfo.InvariantCulture)),
                ApiIdFromFactory.FromInt64 => ApiId.FromInt64(long.Parse(this.Value, CultureInfo.InvariantCulture)),
                ApiIdFromFactory.FromGuid => ApiId.FromGuid(Guid.Parse(this.Value)),
                ApiIdFromFactory.FromUlid => ApiId.FromUlid(Ulid.Parse(this.Value)),
                ApiIdFromFactory.FromCulture => ApiId.FromCulture(this.Value),
                _ => throw new InvalidOperationException($"Unsupported factory {this.Factory}.")
            };
        #endregion
    }

    private class JsonDeserializeTest : JsonDeserializeTest<ApiId, ApiIdDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonDeserializeTest()
        {
            this.ExpectedFactoryExpression = (arg) => BuildApiId(arg);
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiId, ApiIdDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonRoundtripTest()
        {
            this.ExpectedFactoryExpression = (arg) => BuildApiId(arg);
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiId, ApiIdDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonSerializeTest()
        {
            this.SourceFactoryExpression = (arg) => BuildApiId(arg);
        }
        #endregion
    }

    private class TryParseTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdKind Kind { get; init; }
        public string? Text { get; init; }
        public bool ExpectedResult { get; init; }
        public ApiId? ExpectedApiId { get; init; }
        #endregion

        #region Calculated Properties
        private bool ActualResult { get; set; }
        private ApiId? ActualApiId { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Kind: {this.Kind}");
            this.WriteLine($"Text: {this.Text.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected Result: {this.ExpectedResult}");
            this.WriteLine($"Expected ApiId:  {this.ExpectedApiId.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = ApiId.TryParse(this.Kind, this.Text, out var value);
            this.ActualApiId = value;
            this.WriteLine($"Actual Result: {this.ActualResult}");
            this.WriteLine($"Actual Value:  {this.ActualApiId.SafeToString()}");
        }

        protected override void Assert()
        {
            if (this.ExpectedResult)
            {
                this.ActualResult.Should().BeTrue();
                this.ActualApiId.Should().NotBeNull();
                this.ActualApiId.Should().BeEquivalentTo(this.ExpectedApiId);
            }
            else
            {
                this.ActualResult.Should().BeFalse();
                this.ActualApiId.Should().Be(ApiId.Empty);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    private static ApiId TestStringAlphaApiId { get; } = ApiId.FromString("alpha");
    private static ApiId TestStringALPHAApiId { get; } = ApiId.FromString("ALPHA");

    private static ApiId TestStringBetaApiId { get; } = ApiId.FromString("beta");

    private static ApiId TestInt24ApiId { get; } = ApiId.FromInt32(24);
    private static ApiId TestInt42ApiId { get; } = ApiId.FromInt32(42);

    private static ApiId TestLong24ApiId { get; } = ApiId.FromInt64(24);
    private static ApiId TestLong42ApiId { get; } = ApiId.FromInt64(42);

    private static ApiId TestCultureEnUsApiId { get; } = ApiId.FromCulture("en-us");
    private static ApiId TestCultureFrFrApiId { get; } = ApiId.FromCulture("fr-fr");

    private static ApiId TestCompositeInt24AndInt24ApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create(ApiId.FromInt32(24)),
        ApiIdPart.Create(ApiId.FromInt32(24))
    );

    private static ApiId TestCompositeInt24AndInt42ApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create(ApiId.FromInt32(24)),
        ApiIdPart.Create(ApiId.FromInt32(42))
    );

    private static ApiId TestCompositeInt24AndInt42AndInt48ApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create(ApiId.FromInt32(24)),
        ApiIdPart.Create(ApiId.FromInt32(42)),
        ApiIdPart.Create(ApiId.FromInt32(48))
    );

    private static ApiId TestCompositeAlphaInt24AndBetaInt24ApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create("alpha", ApiId.FromInt32(24)),
        ApiIdPart.Create("beta", ApiId.FromInt32(24))
    );

    private static ApiId TestCompositeAlphaInt24AndBetaInt42ApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create("alpha", ApiId.FromInt32(24)),
        ApiIdPart.Create("beta", ApiId.FromInt32(42))
    );

    private static ApiId TestCompositeAlphaInt24AndZetaInt42ApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create("alpha", ApiId.FromInt32(24)),
        ApiIdPart.Create("zeta", ApiId.FromInt32(42))
    );

    public const string TestGuidString = "86d5d1a9-ec14-4730-8d9a-41812e5a117a";
    public static Guid TestGuid { get; } = Guid.Parse(TestGuidString);
    private static ApiId TestGuidApiId { get; } = ApiId.FromGuid(TestGuid);

    public const string TestUlidString = "46TQ8TKV0M8WR8V6J1G4Q5M4BT";
    public static Ulid TestUlid { get; } = Ulid.Parse(TestUlidString);
    private static ApiId TestUlidApiId { get; } = ApiId.FromUlid(TestUlid);

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

    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        // Empty
        new JsonDeserializeTest
        {
            Name = "None",
            SourceJson = "null",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.None
                )
            )
        },

        // Scalars

        // .. String
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:alpha",
            SourceJson = @"{""Kind"":""String"",""Value"":""alpha""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "alpha"
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:ALPHA",
            SourceJson = @"{""Kind"":""String"",""Value"":""ALPHA""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "ALPHA"
                )
            )
        },

        // .. Int32
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int32)}:42",
            SourceJson = @"{""Kind"":""Int32"",""Value"":42}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int32,
                    Int32Value: 42
                )
            )
        },

        // .. Int64
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int64)}:24",
            SourceJson = @"{""Kind"":""Int64"",""Value"":24}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int64,
                    Int64Value: 24
                )
            )
        },

        // .. Guid
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Guid)}:{TestGuid}",
            SourceJson = @"{""Kind"":""Guid"",""Value"":""" + TestGuid + @"""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Guid,
                    GuidValue: TestGuid
                )
            )
        },

        // .. Ulid
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Ulid)}:{TestUlid}",
            SourceJson = @"{""Kind"":""Ulid"",""Value"":""" + TestUlid + @"""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Ulid,
                    UlidValue: TestUlid
                )
            )
        },

        // .. Culture
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:en-US",
            SourceJson = @"{""Kind"":""Culture"",""Value"":""en-US""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "en-US"
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:fr-FR",
            SourceJson = @"{""Kind"":""Culture"",""Value"":""fr-FR""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "fr-FR"
                )
            )
        },

        // Composites

        // .. Ordered (unnamed parts)
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:24|24",
            SourceJson = @"{""Kind"":""Composite"",""Value"":[{""Kind"":""Int32"",""Value"":24},{""Kind"":""Int32"",""Value"":24}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },

        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:24|42",
            SourceJson = @"{""Kind"":""Composite"",""Value"":[{""Kind"":""Int32"",""Value"":24},{""Kind"":""Int32"",""Value"":42}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },

        // .. Named (named parts)
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=24",
            SourceJson = @"{""Kind"":""Composite"",""Value"":[{""Name"":""alpha"",""Kind"":""Int32"",""Value"":24},{""Name"":""beta"",""Kind"":""Int32"",""Value"":24}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=42",
            SourceJson = @"{""Kind"":""Composite"",""Value"":[{""Name"":""alpha"",""Kind"":""Int32"",""Value"":24},{""Name"":""beta"",""Kind"":""Int32"",""Value"":42}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:alpha=24|zeta=42",
            SourceJson = @"{""Kind"":""Composite"",""Value"":[{""Name"":""alpha"",""Kind"":""Int32"",""Value"":24},{""Name"":""zeta"",""Kind"":""Int32"",""Value"":42}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "zeta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        // Empty
        new JsonRoundtripTest
        {
            Name = "None",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.None
                )
            )
        },

        // Scalars

        // .. String
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:alpha",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "alpha"
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:ALPHA",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "ALPHA"
                )
            )
        },

        // .. Int32
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int32)}:42",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int32,
                    Int32Value: 42
                )
            )
        },

        // .. Int64
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int64)}:24",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int64,
                    Int64Value: 24
                )
            )
        },

        // .. Guid
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Guid)}:{TestGuid}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Guid,
                    GuidValue: TestGuid
                )
            )
        },

        // .. Ulid
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Ulid)}:{TestUlid}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Ulid,
                    UlidValue: TestUlid
                )
            )
        },

        // .. Culture
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:en-US",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "en-US"
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:fr-FR",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "fr-FR"
                )
            )
        },

        // Composites

        // .. Ordered (unnamed parts)
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:24|24",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:24|42",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },

        // .. Named (named parts)
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:alpha=24|beta=24",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:alpha=24|beta=42",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:alpha=24|zeta=42",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "zeta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        // Empty
        new JsonSerializeTest
        {
            Name = "None",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.None
                )
            ),
            ExpectedJson = "null"
        },

        // Scalars

        // .. String
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:alpha",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "alpha"
                )
            ),
            ExpectedJson = @"{""Kind"":""String"",""Value"":""alpha""}"
        },

        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:ALPHA",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "ALPHA"
                )
            ),
            ExpectedJson = @"{""Kind"":""String"",""Value"":""ALPHA""}"
        },

        // .. Int32
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int32)}:42",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int32,
                    Int32Value: 42
                )
            ),
            ExpectedJson = @"{""Kind"":""Int32"",""Value"":42}"
        },

        // .. Int64
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int64)}:24",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int64,
                    Int64Value: 24
                )
            ),
            ExpectedJson = @"{""Kind"":""Int64"",""Value"":24}"
        },

        // .. Guid
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Guid)}:{TestGuid}",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Guid,
                    GuidValue: TestGuid
                )
            ),
            ExpectedJson = @"{""Kind"":""Guid"",""Value"":""" + TestGuid + @"""}"
        },

        // .. Ulid
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Ulid)}:{TestUlid}",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Ulid,
                    UlidValue: TestUlid
                )
            ),
            ExpectedJson = @"{""Kind"":""Ulid"",""Value"":""" + TestUlid + @"""}"
        },

        // .. Culture
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:en-US",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "en-US"
                )
            ),
            ExpectedJson = @"{""Kind"":""Culture"",""Value"":""en-US""}"
        },

        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:fr-FR",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "fr-FR"
                )
            ),
            ExpectedJson = @"{""Kind"":""Culture"",""Value"":""fr-FR""}"
        },

        // Composites

        // .. Ordered (unnamed parts)
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:24|24",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""Kind"":""Composite"",""Value"":[{""Kind"":""Int32"",""Value"":24},{""Kind"":""Int32"",""Value"":24}]}"
        },

        new JsonSerializeTest
        {
            Name = $"Composite: Composite:24|42",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""Kind"":""Composite"",""Value"":[{""Kind"":""Int32"",""Value"":24},{""Kind"":""Int32"",""Value"":42}]}"
        },

        // .. Named (named parts)
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=24",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""Kind"":""Composite"",""Value"":[{""Name"":""alpha"",""Kind"":""Int32"",""Value"":24},{""Name"":""beta"",""Kind"":""Int32"",""Value"":24}]}"
        },
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=42",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""Kind"":""Composite"",""Value"":[{""Name"":""alpha"",""Kind"":""Int32"",""Value"":24},{""Name"":""beta"",""Kind"":""Int32"",""Value"":42}]}"
        },
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:alpha=24|zeta=42",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "zeta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""Kind"":""Composite"",""Value"":[{""Name"":""alpha"",""Kind"":""Int32"",""Value"":24},{""Name"":""zeta"",""Kind"":""Int32"",""Value"":42}]}"
        },
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

    #region Theory Methods
    [Theory]
    [MemberData(nameof(ComparisonTheoryData))]
    public void Comparison(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(CompositeTheoryData))]
    public void Composite(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(EqualityTheoryData))]
    public void Equality(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(FromScalarTheoryData))]
    public void FromScalar(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonDeserializeTheoryData))]
    public void JsonDeserialize(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtrip(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonSerializeTheoryData))]
    public void JsonSerialize(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryParseTheoryData))]
    public void TryParse(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Helper Methods
    private static ApiId BuildApiId(ApiIdDescriptor? descriptor)
    {
        if (descriptor != null && descriptor.ScalarConfig != null)
        {
            var scalarConfig = descriptor.ScalarConfig;
            return scalarConfig.Kind switch
            {
                ApiIdKind.None => ApiId.Empty,
                ApiIdKind.Culture when scalarConfig.CultureValue != null => ApiId.FromCulture(scalarConfig.CultureValue),
                ApiIdKind.Guid when scalarConfig.GuidValue != null => ApiId.FromGuid(scalarConfig.GuidValue.Value),
                ApiIdKind.Int32 when scalarConfig.Int32Value != null => ApiId.FromInt32(scalarConfig.Int32Value.Value),
                ApiIdKind.Int64 when scalarConfig.Int64Value != null => ApiId.FromInt64(scalarConfig.Int64Value.Value),
                ApiIdKind.String when scalarConfig.StringValue != null => ApiId.FromString(scalarConfig.StringValue),
                ApiIdKind.Ulid when scalarConfig.UlidValue != null => ApiId.FromUlid(scalarConfig.UlidValue.Value),
                _ => throw new InvalidOperationException($"Invalid {nameof(ApiIdDescriptor)}: {nameof(ApiIdDescriptor.ScalarConfig)} is missing required value.")
            };
        }
        else if (descriptor != null && descriptor.CompositePartsConfig != null && descriptor.CompositePartsConfig.Length > 0)
        {
            var parts = descriptor.CompositePartsConfig.Select(partConfig =>
            {
                var partApiId = BuildApiId(new ApiIdDescriptor(partConfig.ScalarConfig, null));
                return ApiIdPart.Create(partConfig.Name, partApiId);
            }).ToArray();

            return ApiId.Composite(parts);
        }

        return ApiId.Empty;
    }
    #endregion
}
