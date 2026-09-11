// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Version;

public class ApiVersionTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed record CustomVersion(string Value);

    private sealed class EqualityTest : XUnitTest
    {
        #region User Supplied Properties
        public required object LeftValue { get; init; }
        public required object RightValue { get; init; }
        public required bool ShouldEqual { get; init; }
        #endregion

        #region Calculated Properties
        private ApiVersion Left { get; set; }
        private ApiVersion Right { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.Left = ApiVersion.FromValue(this.LeftValue, this.LeftValue.GetType());
            this.Right = ApiVersion.FromValue(this.RightValue, this.RightValue.GetType());
        }

        protected override void Assert()
        {
            this.Left.Equals(this.Right).Should().Be(this.ShouldEqual);
            (this.Left == this.Right).Should().Be(this.ShouldEqual);
            (this.Left != this.Right).Should().Be(!this.ShouldEqual);

            if (this.ShouldEqual)
            {
                this.Left.GetHashCode().Should().Be(this.Right.GetHashCode());
            }
        }
        #endregion
    }

    private sealed class BinaryCopyTest : XUnitTest
    {
        #region Calculated Properties
        private ApiVersion Actual { get; set; }
        private ApiVersion Expected { get; set; }
        private byte[]? Input { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Input = [1, 2, 3];
            this.Expected = ApiVersion.FromValue(new byte[] { 1, 2, 3 }, typeof(byte[]));
        }

        protected override void Act()
        {
            this.Actual = ApiVersion.FromValue(this.Input!, typeof(byte[]));
            this.Input![0] = 9;

            var extracted = this.Actual.GetValue<byte[]>();
            extracted[1] = 9;
        }

        protected override void Assert()
        {
            this.Actual.Should().Be(this.Expected);
            this.Actual.GetValue<byte[]>().Should().Equal(1, 2, 3);
        }
        #endregion
    }

    private sealed class CustomReferenceTest : XUnitTest
    {
        #region Calculated Properties
        private ApiVersion Actual { get; set; }
        private ApiVersion Equivalent { get; set; }
        private CustomVersion? Source { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Source = new CustomVersion("v1");
        }

        protected override void Act()
        {
            this.Actual = ApiVersion.FromValue(this.Source!, typeof(CustomVersion));
            this.Equivalent = ApiVersion.FromValue
            (
                new CustomVersion("v1"),
                typeof(CustomVersion)
            );
        }

        protected override void Assert()
        {
            this.Actual.GetValue<CustomVersion>().Should().BeSameAs(this.Source);
            this.Actual.Should().Be(this.Equivalent);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] EqualityTheoryData =>
    [
        new EqualityTest
        {
            Name = "Equal Int32 Versions",
            LeftValue = 42,
            RightValue = 42,
            ShouldEqual = true
        },
        new EqualityTest
        {
            Name = "Exact CLR Representation Participates In Equality",
            LeftValue = 42,
            RightValue = 42L,
            ShouldEqual = false
        },
        new EqualityTest
        {
            Name = "String Equality Is Ordinal And Case Sensitive",
            LeftValue = "Version-A",
            RightValue = "version-a",
            ShouldEqual = false
        },
        new EqualityTest
        {
            Name = "Equal UInt64 Versions",
            LeftValue = ulong.MaxValue,
            RightValue = ulong.MaxValue,
            ShouldEqual = true
        },
        new EqualityTest
        {
            Name = "Equal Guid Versions",
            LeftValue = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            RightValue = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ShouldEqual = true
        },
        new EqualityTest
        {
            Name = "Equal Ulid Versions",
            LeftValue = Ulid.Parse("01ARZ3NDEKTSV4RRFFQ69G5FAV"),
            RightValue = Ulid.Parse("01ARZ3NDEKTSV4RRFFQ69G5FAV"),
            ShouldEqual = true
        },
        new EqualityTest
        {
            Name = "Equal DateTime Versions",
            LeftValue = new DateTime(638000000000000000, DateTimeKind.Utc),
            RightValue = new DateTime(638000000000000000, DateTimeKind.Utc),
            ShouldEqual = true
        },
        new EqualityTest
        {
            Name = "Equal DateTimeOffset Versions",
            LeftValue = new DateTimeOffset(638000000000000000, TimeSpan.FromHours(-5)),
            RightValue = new DateTimeOffset(638000000000000000, TimeSpan.FromHours(-5)),
            ShouldEqual = true
        },
        new EqualityTest
        {
            Name = "DateTime Kind Participates In Equality",
            LeftValue = new DateTime(638000000000000000, DateTimeKind.Utc),
            RightValue = new DateTime(638000000000000000, DateTimeKind.Unspecified),
            ShouldEqual = false
        },
        new EqualityTest
        {
            Name = "DateTimeOffset Offset Participates In Equality",
            LeftValue = new DateTimeOffset(2022, 11, 9, 1, 0, 0, TimeSpan.FromHours(-5)),
            RightValue = new DateTimeOffset(2022, 11, 9, 2, 0, 0, TimeSpan.FromHours(-4)),
            ShouldEqual = false
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] StorageTheoryData =>
    [
        new BinaryCopyTest
        {
            Name = "Binary Versions Are Defensively Copied"
        },
        new CustomReferenceTest
        {
            Name = "Custom Immutable Reference Versions Are Retained"
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(EqualityTheoryData))]
    public void Equality(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(StorageTheoryData))]
    public void Storage(IXUnitTest test) => test.Execute(this);
    #endregion
}
