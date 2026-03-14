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
    private class TryNavigateTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public string Path { get; init; } = null!;

        public required ApiIdentityNavigationResult ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentityNavigationResult? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Snapshot: {this.Snapshot.SafeToString()}");
            this.WriteLine($"Path:     {this.Path.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected Result: {this.ExpectedResult.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualResult = this.Snapshot.TryNavigate(this.Path);
            this.WriteLine($"Actual Result:   {this.ActualResult.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualResult.Should().NotBeNull();
            this.ActualResult.Should().BeEquivalentTo(this.ExpectedResult);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryNavigateTheoryData =>
    [
        new TryNavigateTest
        {
            Name = $"Snapshot: {ScalarSnapshot.ToDebuggerDisplay()} Path:Id",
            Snapshot = ScalarSnapshot,
            Path = "Id",
            ExpectedResult = ApiIdentityNavigationResult.ScalarNavigationAttempt("Id", ApiIdentitySnapshot.RootPath)
        },

        new TryNavigateTest
        {
            Name = $"Snapshot: {CompositeSnapshotEmpty.ToDebuggerDisplay()} Path:Id",
            Snapshot = CompositeSnapshotEmpty,
            Path = "Id",
            ExpectedResult = ApiIdentityNavigationResult.NoNestedParts("Id", ApiIdentitySnapshot.RootPath)
        },

        new TryNavigateTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedScalarPart.ToDebuggerDisplay()} Path:Id",
            Snapshot = CompositeSnapshotWithResolvedScalarPart,
            Path = "Id",
            ExpectedResult = ApiIdentityNavigationResult.Success(ApiIdentitySnapshot.Scalar("Id", ApiId.FromInt32(42)), "Id")
        },

        new TryNavigateTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedScalarPart.ToDebuggerDisplay()} Path:KeyDoesNotExist",
            Snapshot = CompositeSnapshotWithResolvedScalarPart,
            Path = "KeyDoesNotExist",
            ExpectedResult = ApiIdentityNavigationResult.PartNotFound("KeyDoesNotExist", "KeyDoesNotExist", ApiIdentitySnapshot.RootPath)
        },

        new TryNavigateTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithUnresolvedScalarPart.ToDebuggerDisplay()} Path:Id",
            Snapshot = CompositeSnapshotWithUnresolvedScalarPart,
            Path = "Id",
            ExpectedResult = ApiIdentityNavigationResult.UnresolvedWithoutStructure("Id", "Id", ApiIdentitySnapshot.RootPath)
        },

        new TryNavigateTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedNestedIdentityParts.ToDebuggerDisplay()} Path:Customer",
            Snapshot = CompositeSnapshotWithResolvedNestedIdentityParts,
            Path = "Customer",
            ExpectedResult = ApiIdentityNavigationResult.Success
            (
                ApiIdentitySnapshot.Composite
                (
                    "Customer",
                    new ApiIdentityPart
                    (
                        Name: "Country",
                        Snapshot: ApiIdentitySnapshot.Composite
                        (
                            new ApiIdentityPart
                            (
                                Name: "Id",
                                Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(1))
                            )
                        )
                    ),
                    new ApiIdentityPart
                    (
                        Name: "CustomerId",
                        Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(42))
                    )
                ),
                "Customer"
            )
        },

        new TryNavigateTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedNestedIdentityParts.ToDebuggerDisplay()} Path:Customer.Country",
            Snapshot = CompositeSnapshotWithResolvedNestedIdentityParts,
            Path = "Customer.Country",
            ExpectedResult = ApiIdentityNavigationResult.Success
            (
                ApiIdentitySnapshot.Composite
                (
                    "Customer.Country",
                    new ApiIdentityPart
                    (
                        Name: "Id",
                        Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(1))
                    )
                ),
                "Customer.Country"
            )
        },

        new TryNavigateTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithUnresolvedNestedIdentityParts.ToDebuggerDisplay()} Path:Customer",
            Snapshot = CompositeSnapshotWithUnresolvedNestedIdentityParts,
            Path = "Customer",
            ExpectedResult = ApiIdentityNavigationResult.Success
            (
                ApiIdentitySnapshot.Composite
                (
                    "Customer",
                    new ApiIdentityPart
                    (
                        Name: "Country",
                        Snapshot: null,
                        Structure:
                        [
                            new ApiIdentityPart
                            (
                                Name: "Id",
                                Snapshot: null
                            )
                        ]
                    ),
                    new ApiIdentityPart
                    (
                        Name: "CustomerId",
                        Snapshot: null
                    )
                ),
                "Customer"
            )
        },

        new TryNavigateTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithUnresolvedNestedIdentityParts.ToDebuggerDisplay()} Path:Customer.Country",
            Snapshot = CompositeSnapshotWithUnresolvedNestedIdentityParts,
            Path = "Customer.Country",
            ExpectedResult = ApiIdentityNavigationResult.SuccessWithSynthetic
            (
                ApiIdentitySnapshot.Composite
                (
                    "Customer.Country",
                    new ApiIdentityPart
                    (
                        Name: "Id",
                        Snapshot: null
                    )
                ),
                "Customer.Country"
            )
        },

        new TryNavigateTest
        {
            Name = $"Snapshot: {CompositeSnapshotWithResolvedDeeplyNestedIdentityParts.ToDebuggerDisplay()} Path:Level1.Level2.Level3",
            Snapshot = CompositeSnapshotWithResolvedDeeplyNestedIdentityParts,
            Path = "Level1.Level2.Level3",
            ExpectedResult = ApiIdentityNavigationResult.Success
            (
                ApiIdentitySnapshot.Composite
                (
                    "Level1.Level2.Level3",
                    new ApiIdentityPart
                    (
                        Name: "Level4",
                        Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromString("DeepValue"))
                    ),
                    new ApiIdentityPart
                    (
                        Name: "SequenceNumber",
                        Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(42))
                    )
                ),
                "Level1.Level2.Level3"
            )
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TryNavigateTheoryData))]
    public void TryNavigate(IXUnitTest test) => test.Execute(this);
    #endregion
}
#endif
