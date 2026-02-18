// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentitySnapshotTests
{
    #region Test Types
    private class ToApiIdTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public bool UseNamedParts { get; init; }
        public ApiUnresolvedIdentityPartBehavior UnresolvedBehavior { get; init; }
        public ApiId? ExpectedApiId { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiId? ActualApiId { get; set; }
        private Type? ActualExceptionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Snapshot:           {this.Snapshot.SafeToString()}");
            this.WriteLine($"UseNamedParts:      {this.UseNamedParts.SafeToString()}");
            this.WriteLine($"UnresolvedBehavior: {this.UnresolvedBehavior.SafeToString()}");
            this.WriteLine();

            if (this.ExpectedApiId is not null)
            {
                this.WriteLine($"Expected ApiId: {this.ExpectedApiId.SafeToString()}");
            }
            else if (this.ExpectedExceptionType is not null)
            {
                this.WriteLine($"Expected Exception: {this.ExpectedExceptionType.SafeToName()}");
            }
        }

        protected override void Act()
        {
            try
            {
                this.ActualApiId = this.Snapshot.ToApiId(this.UseNamedParts, this.UnresolvedBehavior);
                this.WriteLine($"Actual ApiId:   {this.ActualApiId.SafeToString()}");
            }
            catch (Exception ex)
            {
                this.ActualExceptionType = ex.GetType();
                this.WriteLine($"Actual Exception:   {this.ActualExceptionType.SafeToName()}");
            }
        }

        protected override void Assert()
        {
            if (this.ExpectedApiId is not null)
            {
                this.ActualApiId.Should().NotBeNull();
                this.ActualApiId.Should().BeEquivalentTo(this.ExpectedApiId);
            }
            else if (this.ExpectedExceptionType is not null)
            {
                this.ActualExceptionType.Should().NotBeNull();
                this.ActualExceptionType.Should().Be(this.ExpectedExceptionType);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ToApiIdTheoryData =>
    [
        // Scalar snapshots
        new ToApiIdTest
        {
            Name = $"Scalar snapshot with empty value flattens to scalar with no value",
            Snapshot = ApiIdentitySnapshot.Scalar(ApiId.Empty),
            ExpectedApiId = ApiId.Empty
        },

        new ToApiIdTest
        {
            Name = $"Scalar snapshot with integer value flattens to scalar with value",
            Snapshot = ApiIdentitySnapshot.Scalar(ApiId.FromInt32(42)),
            ExpectedApiId = ApiId.FromInt32(42)
        },

        // Composite snapshots with empty parts
        new ToApiIdTest
        {
            Name = $"Composite snapshot with empty parts flattens to composite with empty parts",
            Snapshot = ApiIdentitySnapshot.Composite(),
            ExpectedApiId = ApiId.Empty
        },

        // Composite snapshots with scalar parts only
        new ToApiIdTest
        {
            Name = $"Composite snapshot with resolved scalar part flattens to named composite with scalar part",
            Snapshot = CompositeSnapshotWithResolvedScalarPart,
            UseNamedParts = true,
            ExpectedApiId = CompositeSnapshotWithResolvedScalarPartToNamedCompositeApiId
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with resolved scalar part flattens to unnamed composite with scalar part",
            Snapshot = CompositeSnapshotWithResolvedScalarPart,
            UseNamedParts = false,
            ExpectedApiId = CompositeSnapshotWithResolvedScalarPartToUnnamedCompositeApiId
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with unresolved scalar part throws exception",
            Snapshot = CompositeSnapshotWithUnresolvedScalarPart,
            UnresolvedBehavior = ApiUnresolvedIdentityPartBehavior.Throw,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with unresolved scalar part flattens to named composite with empty part",
            Snapshot = CompositeSnapshotWithUnresolvedScalarPart,
            UseNamedParts = true,
            UnresolvedBehavior = ApiUnresolvedIdentityPartBehavior.AllowUnresolved,
            ExpectedApiId = CompositeSnapshotWithUnresolvedScalarPartToNamedCompositeApiId
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with unresolved scalar part flattens to unnamed composite with empty part",
            Snapshot = CompositeSnapshotWithUnresolvedScalarPart,
            UseNamedParts = false,
            UnresolvedBehavior = ApiUnresolvedIdentityPartBehavior.AllowUnresolved,
            ExpectedApiId = CompositeSnapshotWithUnresolvedScalarPartToUnnamedCompositeApiId
        },

        // Composite snapshots with nested identity parts
        new ToApiIdTest
        {
            Name = $"Composite snapshot with resolved nested identity parts flattens to named composite with many parts",
            Snapshot = CompositeSnapshotWithResolvedNestedIdentityParts,
            UseNamedParts = true,
            ExpectedApiId = CompositeSnapshotWithResolvedNestedIdentityPartsToNamedCompositeApiId
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with resolved nested identity parts flattens to unnamed composite with many parts",
            Snapshot = CompositeSnapshotWithResolvedNestedIdentityParts,
            UseNamedParts = false,
            ExpectedApiId = CompositeSnapshotWithResolvedNestedIdentityPartsToUnnamedCompositeApiId
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with unresolved nested identity parts throws exception",
            Snapshot = CompositeSnapshotWithUnresolvedNestedIdentityParts,
            UnresolvedBehavior = ApiUnresolvedIdentityPartBehavior.Throw,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with unresolved nested identity parts flattens to named composite with many parts",
            Snapshot = CompositeSnapshotWithUnresolvedNestedIdentityParts,
            UseNamedParts = true,
            UnresolvedBehavior = ApiUnresolvedIdentityPartBehavior.AllowUnresolved,
            ExpectedApiId = CompositeSnapshotWithUnresolvedNestedIdentityPartsToNamedCompositeApiId
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with unresolved nested identity parts flattens to unnamed composite with many parts",
            Snapshot = CompositeSnapshotWithUnresolvedNestedIdentityParts,
            UseNamedParts = false,
            UnresolvedBehavior = ApiUnresolvedIdentityPartBehavior.AllowUnresolved,
            ExpectedApiId = CompositeSnapshotWithUnresolvedNestedIdentityPartsToUnnamedCompositeApiId
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with partial unresolved nested identity parts throws exception",
            Snapshot = CompositeSnapshotWithPartialUnresolvedNestedIdentityParts,
            UnresolvedBehavior = ApiUnresolvedIdentityPartBehavior.Throw,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with partial unresolved nested identity parts flattens to named composite with many parts",
            Snapshot = CompositeSnapshotWithPartialUnresolvedNestedIdentityParts,
            UseNamedParts = true,
            UnresolvedBehavior = ApiUnresolvedIdentityPartBehavior.AllowUnresolved,
            ExpectedApiId = CompositeSnapshotWithPartialUnresolvedNestedIdentityPartsToNamedCompositeApiId
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with partial unresolved nested identity parts flattens to unnamed composite with many parts",
            Snapshot = CompositeSnapshotWithPartialUnresolvedNestedIdentityParts,
            UseNamedParts = false,
            UnresolvedBehavior = ApiUnresolvedIdentityPartBehavior.AllowUnresolved,
            ExpectedApiId = CompositeSnapshotWithPartialUnresolvedNestedIdentityPartsToUnnamedCompositeApiId
        },

        // Composite snapshots with deeply nested identity parts
        new ToApiIdTest
        {
            Name = $"Composite snapshot with resolved deeply nested identity parts flattens to named composite with many parts",
            Snapshot = CompositeSnapshotWithResolvedDeeplyNestedIdentityParts,
            UseNamedParts = true,
            ExpectedApiId = CompositeSnapshotWithResolvedDeeplyNestedIdentityPartsToNamedCompositeApiId
        },

        new ToApiIdTest
        {
            Name = $"Composite snapshot with resolved deeply nested identity parts flattens to unnamed composite with many parts",
            Snapshot = CompositeSnapshotWithResolvedDeeplyNestedIdentityParts,
            UseNamedParts = false,
            ExpectedApiId = CompositeSnapshotWithResolvedDeeplyNestedIdentityPartsToUnnamedCompositeApiId
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ToApiIdTheoryData))]
    public void ToApiId(IXUnitTest test) => test.Execute(this);
    #endregion
}
