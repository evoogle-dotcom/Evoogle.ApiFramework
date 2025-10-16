// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Globalization;

using Evoogle.ApiFramework.Identity.Internal;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Identity;

public class ApiIdTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    public enum ApiIdFromFactory
    {
        FromString,
        FromInt32,
        FromInt64,
        FromGuid,
        FromUlid,
        FromCulture
    }

    public enum ApiIdCompositeFactory
    {
        Named,
        Ordered
    }

    public class CompositeTest : XUnitTest
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
                    ApiId.CompositeString(partCollection)
                );
                return apiId;
            }

            throw new InvalidOperationException($"Unsupported factory {this.Factory}.");
        }
        #endregion
    }

    public class FromScalarTest : XUnitTest
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

    public class TryParseTest : XUnitTest
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
    private static Guid TestGuid { get; } = Guid.NewGuid();
    private static string TestGuidString { get; } = TestGuid.ToString();

    private static Ulid TestUlid { get; } = Ulid.NewUlid();
    private static string TestUlidString { get; } = TestUlid.ToString();

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
                ApiIdPart.CreateNamed("id-1", ApiId.FromInt32(42)),
                ApiIdPart.CreateNamed("id-2", ApiId.FromInt32(24)),
            ]
        },
        new CompositeTest
        {
            Name = "Named Composite from String and Culture ApiIds",
            Factory = ApiIdCompositeFactory.Named,
            PartCollection =
            [
                ApiIdPart.CreateNamed("id", ApiId.FromString("42")),
                ApiIdPart.CreateNamed("locale", ApiId.FromCulture("en-us")),
            ]
        },
        new CompositeTest
        {
            Name = "Named Composite from String and Int64 and Culture ApiIds",
            Factory = ApiIdCompositeFactory.Named,
            PartCollection =
            [
                ApiIdPart.CreateNamed("id-string", ApiId.FromString("42")),
                ApiIdPart.CreateNamed("id-int64", ApiId.FromInt64(42)),
                ApiIdPart.CreateNamed("locale", ApiId.FromCulture("en-us")),
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

    public static IEnumerable<object[]> EqualityCases()
    {
        yield return new object[] { ApiId.Empty, ApiId.Empty, true };
        yield return new object[] { ApiId.FromString("alpha"), ApiId.FromInt32(1), false };
        yield return new object[] { ApiId.FromString("Alpha"), ApiId.FromString("alpha"), false };
        yield return new object[] { ApiId.FromCulture("en-US"), ApiId.FromCulture("EN-us"), true };
        yield return new object[] { ApiId.FromInt32(42), ApiId.FromInt32(42), true };
        yield return new object[]
        {
            ApiId.Composite(ApiIdPart.CreateNamed("id", ApiId.FromInt32(1))),
            ApiId.Composite(ApiIdPart.CreateNamed("id", ApiId.FromInt32(1))),
            true
        };
        yield return new object[]
        {
            ApiId.Composite(ApiIdPart.CreateNamed("id", ApiId.FromInt32(1))),
            ApiId.Composite(ApiIdPart.CreateNamed("other", ApiId.FromInt32(1))),
            false
        };
    }

    public static IEnumerable<object[]> ComparisonCases()
    {
        yield return new object[] { ApiId.Empty, ApiId.Empty, 0 };
        yield return new object[] { ApiId.Empty, ApiId.FromString("alpha"), -1 };
        yield return new object[] { ApiId.FromString("alpha"), ApiId.FromString("beta"), -1 };
        yield return new object[] { ApiId.FromInt32(10), ApiId.FromInt32(2), 1 };
        yield return new object[] { ApiId.FromCulture("en-US"), ApiId.FromCulture("EN-us"), 0 };
        yield return new object[]
        {
            ApiId.Composite(ApiId.FromInt32(1), ApiId.FromInt32(2)),
            ApiId.Composite(ApiId.FromInt32(1), ApiId.FromInt32(3)),
            -1
        };
    }
    #endregion

    #region Theory Methods
    [Theory]
    [MemberData(nameof(CompositeTheoryData))]
    public void Composite(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(FromScalarTheoryData))]
    public void FromScalar(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryParseTheoryData))]
    public void TryParse(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(EqualityCases))]
    public void Equality(ApiId left, ApiId right, bool expectedEqual)
    {
        left.Equals(right).Should().Be(expectedEqual, "Equals should match expectation");
        right.Equals(left).Should().Be(expectedEqual, "Equals should be symmetric");
        (left == right).Should().Be(expectedEqual, "operator == should match expectation");
        (left != right).Should().Be(!expectedEqual, "operator != should match expectation");

        if (expectedEqual)
        {
            left.GetHashCode().Should().Be(right.GetHashCode(), "equal values must produce identical hashes");
        }
    }

    [Theory]
    [MemberData(nameof(ComparisonCases))]
    public void Comparison(ApiId left, ApiId right, int expectedSign)
    {
        var actualSign = Math.Sign(left.CompareTo(right));
        actualSign.Should().Be(expectedSign, "CompareTo should return the expected ordering");

        var reverseSign = Math.Sign(right.CompareTo(left));
        reverseSign.Should().Be(-actualSign, "CompareTo should be antisymmetric");
    }
    #endregion
}
