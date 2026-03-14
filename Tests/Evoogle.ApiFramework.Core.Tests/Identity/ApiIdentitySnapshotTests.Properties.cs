// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

#if false
public partial class ApiIdentitySnapshotTests
{
    #region Test Types
    private class GetUnresolvedPartsTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public string[] Expected { get; init; } = null!;
        #endregion

        #region Calculated Properties
        private string[]? Actual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Snapshot: {this.Snapshot.SafeToString()}");
            this.WriteLine($"Expected: {this.Expected.SafeToDelimitedString(',')}");
        }

        protected override void Act()
        {
            this.Actual = this.Snapshot.GetUnresolvedParts();

            this.WriteLine($"Actual:   {this.Actual.SafeToDelimitedString(',')}");
        }

        protected override void Assert()
        {
            this.Actual.Should().BeEquivalentTo(this.Expected);
        }
        #endregion
    }

    private class TryGetScalarValueTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public string Path { get; init; } = null!;
        public bool ExpectedResult { get; init; }
        public ApiId? ExpectedScalarValue { get; init; }
        #endregion

        #region Calculated Properties
        private bool? ActualResult { get; set; }
        private ApiId? ActualScalarValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Snapshot: {this.Snapshot.SafeToString()}");
            this.WriteLine($"Path:     {this.Path.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedResult:      {this.ExpectedResult.SafeToString()}");
            this.WriteLine($"ExpectedScalarValue: {this.ExpectedScalarValue.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var (success, _) = this.Snapshot.TryGetScalarValue(this.Path, out var scalarValue);

            this.ActualResult = success;
            this.ActualScalarValue = scalarValue;
            this.WriteLine($"Actual Result:      {this.ActualResult.SafeToString()}");
            this.WriteLine($"Actual ScalarValue: {this.ActualScalarValue.SafeToString()}");
        }

        protected override void Assert()
        {
            if (this.ExpectedResult)
            {
                this.ActualResult.Should().BeTrue();
                this.ActualScalarValue.Should().BeEquivalentTo(this.ExpectedScalarValue);
            }
            else
            {
                this.ActualResult.Should().BeFalse();
                this.ActualScalarValue.Should().BeEquivalentTo(ApiId.Empty);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] GetUnresolvedPartsTheoryData =>
    [
        new GetUnresolvedPartsTest
        {
            Name = $"Snapshot: {ScalarSnapshot.ToDebuggerDisplay()}",
            Snapshot = ScalarSnapshot,
            Expected = []
        },

        new GetUnresolvedPartsTest
        {
            Name = $"Snapshot: {CompositeSnapshotEmpty.ToDebuggerDisplay()}",
            Snapshot = CompositeSnapshotEmpty,
            Expected = []
        },

        new GetUnresolvedPartsTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedScalarPart.ToDebuggerDisplay()}",
            Snapshot = CompositeSnapshotWithResolvedScalarPart,
            Expected = []
        },

        new GetUnresolvedPartsTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithUnresolvedScalarPart.ToDebuggerDisplay()}",
            Snapshot = CompositeSnapshotWithUnresolvedScalarPart,
            Expected = ["Id"]
        },

        new GetUnresolvedPartsTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedNestedIdentityParts.ToDebuggerDisplay()}",
            Snapshot = CompositeSnapshotWithResolvedNestedIdentityParts,
            Expected = []
        },

        new GetUnresolvedPartsTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithUnresolvedNestedIdentityParts.ToDebuggerDisplay()}",
            Snapshot = CompositeSnapshotWithUnresolvedNestedIdentityParts,
            Expected = ["Customer.Country.Id", "Customer.CustomerId", "OrderNumber"]
        },

        new GetUnresolvedPartsTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithPartialUnresolvedNestedIdentityParts.ToDebuggerDisplay()}",
            Snapshot = CompositeSnapshotWithPartialUnresolvedNestedIdentityParts,
            Expected = ["Customer.Country.Id"]
        },

        new GetUnresolvedPartsTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedDeeplyNestedIdentityParts.ToDebuggerDisplay()}",
            Snapshot = CompositeSnapshotWithResolvedDeeplyNestedIdentityParts,
            Expected = []
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetScalarValueTheoryData =>
    [
        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {ScalarSnapshot.ToDebuggerDisplay()} Path:Id",
            Snapshot = ScalarSnapshot,
            Path = "Id",
            ExpectedResult = false
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotEmpty.ToDebuggerDisplay()} Path:Id",
            Snapshot = CompositeSnapshotEmpty,
            Path = "Id",
            ExpectedResult = false
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedScalarPart.ToDebuggerDisplay()} Path:Id",
            Snapshot = CompositeSnapshotWithResolvedScalarPart,
            Path = "Id",
            ExpectedResult = true,
            ExpectedScalarValue = ApiId.FromInt32(42)
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedScalarPart.ToDebuggerDisplay()} Path:KeyDoesNotExist",
            Snapshot = CompositeSnapshotWithResolvedScalarPart,
            Path = "KeyDoesNotExist",
            ExpectedResult = false
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithUnresolvedScalarPart.ToDebuggerDisplay()} Path:Id",
            Snapshot = CompositeSnapshotWithUnresolvedScalarPart,
            Path = "Id",
            ExpectedResult = false
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedNestedIdentityParts.ToDebuggerDisplay()} Path:Customer",
            Snapshot = CompositeSnapshotWithResolvedNestedIdentityParts,
            Path = "Customer",
            ExpectedResult = false
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedNestedIdentityParts.ToDebuggerDisplay()} Path:Customer.Country.Id",
            Snapshot = CompositeSnapshotWithResolvedNestedIdentityParts,
            Path = "Customer.Country.Id",
            ExpectedResult = true,
            ExpectedScalarValue = ApiId.FromInt32(1)
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithUnresolvedNestedIdentityParts.ToDebuggerDisplay()} Path:Customer",
            Snapshot = CompositeSnapshotWithUnresolvedNestedIdentityParts,
            Path = "Customer",
            ExpectedResult = false
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithUnresolvedNestedIdentityParts.ToDebuggerDisplay()} Path:Customer.Country.Id",
            Snapshot = CompositeSnapshotWithUnresolvedNestedIdentityParts,
            Path = "Customer.Country.Id",
            ExpectedResult = false
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedDeeplyNestedIdentityParts.ToDebuggerDisplay()} Path:Level1.Level2.Level3",
            Snapshot = CompositeSnapshotWithResolvedDeeplyNestedIdentityParts,
            Path = "Level1.Level2.Level3",
            ExpectedResult = false
        },

        new TryGetScalarValueTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedDeeplyNestedIdentityParts.ToDebuggerDisplay()} Path:Level1.Level2.Level3.Level4",
            Snapshot = CompositeSnapshotWithResolvedDeeplyNestedIdentityParts,
            Path = "Level1.Level2.Level3.Level4",
            ExpectedResult = true,
            ExpectedScalarValue = ApiId.FromString("DeepValue")
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(GetUnresolvedPartsTheoryData))]
    public void GetUnresolvedParts(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetScalarValueTheoryData))]
    public void TryGetScalarValue(IXUnitTest test) => test.Execute(this);
    #endregion
}
#endif
