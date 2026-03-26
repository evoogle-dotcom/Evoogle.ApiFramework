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

public partial class ApiIdentityValueTests
{
    #region Test Types
    private class ToApiIdTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiIdentityValue Value { get; init; }
        public bool UseNamedParts { get; init; }
        public ApiIdentityNullHandling NullHandling { get; init; }
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
            this.WriteLine($"Value:         {this.Value.SafeToString()}");
            this.WriteLine($"UseNamedParts: {this.UseNamedParts.SafeToString()}");
            this.WriteLine($"NullHandling:  {this.NullHandling.SafeToString()}");
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
                this.ActualApiId = this.Value.ToApiId(this.UseNamedParts, this.NullHandling);
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
        // Scalar values
        new ToApiIdTest
        {
            Name = "Scalar identity flattens to scalar ApiId",
            Value = ScalarIntegerPart,
            ExpectedApiId = ApiId.FromInt32(42)
        },
        new ToApiIdTest
        {
            Name = "Scalar empty identity flattens to empty ApiId",
            Value = ScalarEmptyPart,
            ExpectedApiId = ApiId.Empty
        },

        // Composite with scalar parts
        new ToApiIdTest
        {
            Name = "Composite with scalar parts flattens to named composite",
            Value = CompositeWithScalarParts,
            UseNamedParts = true,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create("CustomerId", ApiId.FromInt32(42)),
                ApiIdPart.Create("OrderNumber", ApiId.FromInt32(1001))
            )
        },
        new ToApiIdTest
        {
            Name = "Composite with scalar parts flattens to unnamed composite",
            Value = CompositeWithScalarParts,
            UseNamedParts = false,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create(ApiId.FromInt32(42)),
                ApiIdPart.Create(ApiId.FromInt32(1001))
            )
        },

        // Composite with resolved object parts
        new ToApiIdTest
        {
            Name = "Composite with resolved object parts flattens to named composite with dotted names",
            Value = CompositeWithObjectParts,
            UseNamedParts = true,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create("Customer.Country.Id", ApiId.FromInt32(1)),
                ApiIdPart.Create("Customer.CustomerId", ApiId.FromInt32(42)),
                ApiIdPart.Create("OrderNumber", ApiId.FromInt32(1001))
            )
        },
        new ToApiIdTest
        {
            Name = "Composite with resolved object parts flattens to unnamed composite",
            Value = CompositeWithObjectParts,
            UseNamedParts = false,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create(ApiId.FromInt32(1)),
                ApiIdPart.Create(ApiId.FromInt32(42)),
                ApiIdPart.Create(ApiId.FromInt32(1001))
            )
        },

        // Composite with unresolved object parts — ThrowException
        new ToApiIdTest
        {
            Name = "Composite with unresolved object parts throws when ThrowException",
            Value = CompositeWithUnresolvedObjectParts,
            NullHandling = ApiIdentityNullHandling.ThrowException,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        // Composite with unresolved object parts — ReturnEmpty
        new ToApiIdTest
        {
            Name = "Composite with unresolved object parts flattens to named composite with empty slots",
            Value = CompositeWithUnresolvedObjectParts,
            UseNamedParts = true,
            NullHandling = ApiIdentityNullHandling.ReturnEmpty,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create("Customer.Country.Id", ApiId.Empty),
                ApiIdPart.Create("Customer.CustomerId", ApiId.Empty),
                ApiIdPart.Create("OrderNumber", ApiId.Empty)
            )
        },
        new ToApiIdTest
        {
            Name = "Composite with unresolved object parts flattens to unnamed composite with empty slots",
            Value = CompositeWithUnresolvedObjectParts,
            UseNamedParts = false,
            NullHandling = ApiIdentityNullHandling.ReturnEmpty,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create(ApiId.Empty),
                ApiIdPart.Create(ApiId.Empty),
                ApiIdPart.Create(ApiId.Empty)
            )
        },

        // Composite with partially unresolved object parts — ThrowException
        new ToApiIdTest
        {
            Name = "Composite with partially unresolved object parts throws when ThrowException",
            Value = CompositeWithPartiallyUnresolvedObjectParts,
            NullHandling = ApiIdentityNullHandling.ThrowException,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        // Composite with partially unresolved object parts — ReturnEmpty
        new ToApiIdTest
        {
            Name = "Composite with partially unresolved object parts flattens to named composite",
            Value = CompositeWithPartiallyUnresolvedObjectParts,
            UseNamedParts = true,
            NullHandling = ApiIdentityNullHandling.ReturnEmpty,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create("Customer.Country.Id", ApiId.Empty),
                ApiIdPart.Create("Customer.CustomerId", ApiId.Empty),
                ApiIdPart.Create("OrderNumber", ApiId.FromInt32(1001))
            )
        },
        new ToApiIdTest
        {
            Name = "Composite with partially unresolved object parts flattens to unnamed composite",
            Value = CompositeWithPartiallyUnresolvedObjectParts,
            UseNamedParts = false,
            NullHandling = ApiIdentityNullHandling.ReturnEmpty,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create(ApiId.Empty),
                ApiIdPart.Create(ApiId.Empty),
                ApiIdPart.Create(ApiId.FromInt32(1001))
            )
        },

        // Deep object parts
        new ToApiIdTest
        {
            Name = "Composite with deep object parts flattens to named composite with deep dotted names",
            Value = CompositeWithDeepObjectParts,
            UseNamedParts = true,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create("Level1.Level2.Level3.Level4", ApiId.FromString("DeepValue")),
                ApiIdPart.Create("Level1.Level2.Level3.SequenceNumber", ApiId.FromInt32(42)),
                ApiIdPart.Create("RootId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
            )
        },
        new ToApiIdTest
        {
            Name = "Composite with deep object parts flattens to unnamed composite",
            Value = CompositeWithDeepObjectParts,
            UseNamedParts = false,
            ExpectedApiId = ApiId.Composite
            (
                ApiIdPart.Create(ApiId.FromString("DeepValue")),
                ApiIdPart.Create(ApiId.FromInt32(42)),
                ApiIdPart.Create(ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
            )
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ToApiIdTheoryData))]
    public void ToApiId(IXUnitTest test) => test.Execute(this);
    #endregion
}
