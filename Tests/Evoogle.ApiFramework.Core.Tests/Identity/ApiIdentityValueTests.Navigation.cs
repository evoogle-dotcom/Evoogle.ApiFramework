// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentityValueTests
{
    #region Test Types
    private class TryNavigateTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiIdentityValue Value { get; init; }
        public required string DottedPath { get; init; }
        public bool ExpectedFound { get; init; }
        public string? ExpectedPartName { get; init; }
        public ApiIdentityPartValueKind? ExpectedPartKind { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentityPartValue? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"DottedPath:         {this.DottedPath.SafeToString()}");
            this.WriteLine($"Expected Found:     {this.ExpectedFound}");

            if (this.ExpectedPartName is not null)
            {
                this.WriteLine($"Expected PartName:  {this.ExpectedPartName}");
                this.WriteLine($"Expected PartKind:  {this.ExpectedPartKind.SafeToString()}");
            }
        }

        protected override void Act()
        {
            this.ActualResult = this.Value.TryNavigate(this.DottedPath);
            this.WriteLine();
            this.WriteLine($"Actual Result:    {(this.ActualResult is not null ? $"{this.ActualResult.ApiName} ({this.ActualResult.ApiKind})" : "null")}");
        }

        protected override void Assert()
        {
            if (this.ExpectedFound)
            {
                this.ActualResult.Should().NotBeNull();
                this.ActualResult!.ApiName.Should().Be(this.ExpectedPartName);
                this.ActualResult.ApiKind.Should().Be(this.ExpectedPartKind!.Value);
            }
            else
            {
                this.ActualResult.Should().BeNull();
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryNavigateTheoryData =>
    [
        // Direct scalar part
        new TryNavigateTest
        {
            Name = "Navigate to direct scalar part",
            Value = CompositeWithNestedParts,
            DottedPath = "OrderNumber",
            ExpectedFound = true,
            ExpectedPartName = "OrderNumber",
            ExpectedPartKind = ApiIdentityPartValueKind.Scalar
        },

        // Direct object part
        new TryNavigateTest
        {
            Name = "Navigate to direct object part",
            Value = CompositeWithNestedParts,
            DottedPath = "Customer",
            ExpectedFound = true,
            ExpectedPartName = "Customer",
            ExpectedPartKind = ApiIdentityPartValueKind.Object
        },

        // Nested scalar part via dotted path
        new TryNavigateTest
        {
            Name = "Navigate to nested scalar part via dotted path",
            Value = CompositeWithNestedParts,
            DottedPath = "Customer.CustomerId",
            ExpectedFound = true,
            ExpectedPartName = "CustomerId",
            ExpectedPartKind = ApiIdentityPartValueKind.Scalar
        },

        // Deeply nested scalar part
        new TryNavigateTest
        {
            Name = "Navigate to deeply nested scalar part",
            Value = CompositeWithNestedParts,
            DottedPath = "Customer.Country.Id",
            ExpectedFound = true,
            ExpectedPartName = "Id",
            ExpectedPartKind = ApiIdentityPartValueKind.Scalar
        },

        // Navigate into deeply nested structure
        new TryNavigateTest
        {
            Name = "Navigate four levels deep",
            Value = CompositeWithDeeplyNestedParts,
            DottedPath = "Level1.Level2.Level3.Level4",
            ExpectedFound = true,
            ExpectedPartName = "Level4",
            ExpectedPartKind = ApiIdentityPartValueKind.Scalar
        },

        // Not found — wrong name
        new TryNavigateTest
        {
            Name = "Navigate to nonexistent part returns null",
            Value = CompositeWithNestedParts,
            DottedPath = "NonExistent",
            ExpectedFound = false
        },

        // Not found — navigate past scalar
        new TryNavigateTest
        {
            Name = "Navigate past scalar returns null",
            Value = CompositeWithNestedParts,
            DottedPath = "OrderNumber.Something",
            ExpectedFound = false
        },

        // Navigate into unresolved object using structure skeleton
        new TryNavigateTest
        {
            Name = "Navigate into unresolved object via structure skeleton",
            Value = CompositeWithUnresolvedNestedParts,
            DottedPath = "Customer.CustomerId",
            ExpectedFound = true,
            ExpectedPartName = "CustomerId",
            ExpectedPartKind = ApiIdentityPartValueKind.Scalar
        },

        // Navigate into deeply unresolved structure
        new TryNavigateTest
        {
            Name = "Navigate deeply into unresolved structure skeleton",
            Value = CompositeWithUnresolvedNestedParts,
            DottedPath = "Customer.Country.Id",
            ExpectedFound = true,
            ExpectedPartName = "Id",
            ExpectedPartKind = ApiIdentityPartValueKind.Scalar
        },

        // Empty path
        new TryNavigateTest
        {
            Name = "Empty path returns null",
            Value = CompositeWithNestedParts,
            DottedPath = "",
            ExpectedFound = false
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TryNavigateTheoryData))]
    public void TryNavigate(IXUnitTest test) => test.Execute(this);
    #endregion
}
