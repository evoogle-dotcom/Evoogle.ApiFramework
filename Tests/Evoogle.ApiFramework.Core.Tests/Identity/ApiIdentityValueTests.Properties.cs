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
    private class PropertiesTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiIdentityValue Value { get; init; }
        public required bool ExpectedIsScalarValue { get; init; }
        public bool ExpectedIsObjectValue { get; init; }
        public required bool ExpectedIsComposite { get; init; }
        public required int ExpectedPartCount { get; init; }
        public required bool ExpectedIsFullyResolved { get; init; }
        public ApiId? ExpectedScalarValue { get; init; }
        #endregion

        #region Calculated Properties
        private bool ActualIsScalarValue { get; set; }
        private bool ActualIsObjectValue { get; set; }
        private bool ActualIsComposite { get; set; }
        private int ActualPartCount { get; set; }
        private bool ActualIsFullyResolved { get; set; }
        private ApiId? ActualScalarValue { get; set; }
        private bool ScalarAccessThrew { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Value:                   {this.Value.SafeToString()}");
            this.WriteLine($"Expected IsScalarValue:  {this.ExpectedIsScalarValue}");
            this.WriteLine($"Expected IsObjectValue:  {this.ExpectedIsObjectValue}");
            this.WriteLine($"Expected IsComposite:    {this.ExpectedIsComposite}");
            this.WriteLine($"Expected PartCount:      {this.ExpectedPartCount}");
            this.WriteLine($"Expected FullyResolved:  {this.ExpectedIsFullyResolved}");
        }

        protected override void Act()
        {
            this.ActualIsScalarValue = this.Value.IsScalarValue;
            this.ActualIsObjectValue = this.Value.IsObjectValue;
            this.ActualIsComposite = this.Value.IsComposite;
            this.ActualPartCount = this.Value.ApiParts.Length;
            this.ActualIsFullyResolved = this.Value.IsFullyResolved;

            if (this.ExpectedScalarValue is not null)
            {
                try
                {
                    this.ActualScalarValue = this.Value.ApiScalarValue;
                }
                catch (ApiIdentityException)
                {
                    this.ScalarAccessThrew = true;
                }
            }

            this.WriteLine();
            this.WriteLine($"Actual IsScalarValue:  {this.ActualIsScalarValue}");
            this.WriteLine($"Actual IsObjectValue:  {this.ActualIsObjectValue}");
            this.WriteLine($"Actual IsComposite:    {this.ActualIsComposite}");
            this.WriteLine($"Actual PartCount:      {this.ActualPartCount}");
            this.WriteLine($"Actual FullyResolved:  {this.ActualIsFullyResolved}");
        }

        protected override void Assert()
        {
            this.ActualIsScalarValue.Should().Be(this.ExpectedIsScalarValue);
            this.ActualIsObjectValue.Should().Be(this.ExpectedIsObjectValue);
            this.ActualIsComposite.Should().Be(this.ExpectedIsComposite);
            this.ActualPartCount.Should().Be(this.ExpectedPartCount);
            this.ActualIsFullyResolved.Should().Be(this.ExpectedIsFullyResolved);

            if (this.ExpectedScalarValue is not null)
            {
                this.ScalarAccessThrew.Should().BeFalse();
                this.ActualScalarValue.Should().Be(this.ExpectedScalarValue);
            }
        }
        #endregion
    }

    private class ApiScalarValueTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiIdentityValue Value { get; init; }
        public ApiId? ExpectedScalarValue { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiId? ActualScalarValue { get; set; }
        private Type? ActualExceptionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Value: {this.Value.SafeToString()}");
            this.WriteLine();

            if (this.ExpectedScalarValue is not null)
            {
                this.WriteLine($"Expected ScalarValue: {this.ExpectedScalarValue.SafeToString()}");
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
                this.ActualScalarValue = this.Value.ApiScalarValue;
                this.WriteLine($"Actual ScalarValue: {this.ActualScalarValue.SafeToString()}");
            }
            catch (Exception ex)
            {
                this.ActualExceptionType = ex.GetType();
                this.WriteLine($"Actual Exception: {this.ActualExceptionType.SafeToName()}");
            }
        }

        protected override void Assert()
        {
            if (this.ExpectedScalarValue is not null)
            {
                this.ActualScalarValue.Should().NotBeNull();
                this.ActualScalarValue.Should().Be(this.ExpectedScalarValue);
            }
            else if (this.ExpectedExceptionType is not null)
            {
                this.ActualExceptionType.Should().NotBeNull();
                this.ActualExceptionType.Should().Be(this.ExpectedExceptionType);
            }
        }
        #endregion
    }

    private class ApiObjectValueTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiIdentityValue Value { get; init; }
        public ApiIdentityValue? ExpectedObjectValue { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentityValue? ActualObjectValue { get; set; }
        private Type? ActualExceptionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Value: {this.Value.SafeToString()}");
            this.WriteLine();

            if (this.ExpectedObjectValue is not null)
            {
                this.WriteLine($"Expected ObjectValue: {this.ExpectedObjectValue.SafeToString()}");
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
                this.ActualObjectValue = this.Value.ApiObjectValue;
                this.WriteLine($"Actual ObjectValue: {this.ActualObjectValue.SafeToString()}");
            }
            catch (Exception ex)
            {
                this.ActualExceptionType = ex.GetType();
                this.WriteLine($"Actual Exception: {this.ActualExceptionType.SafeToName()}");
            }
        }

        protected override void Assert()
        {
            if (this.ExpectedObjectValue is not null)
            {
                this.ActualObjectValue.Should().NotBeNull();
                this.ActualObjectValue.Should().BeEquivalentTo(this.ExpectedObjectValue, options => options
                    .Excluding(ctx => ctx.Path.EndsWith(nameof(ApiIdentityValue.ApiScalarValue), StringComparison.Ordinal))
                    .Excluding(ctx => ctx.Path.EndsWith(nameof(ApiIdentityValue.ApiObjectValue), StringComparison.Ordinal)));
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
    public static TheoryDataRow<IXUnitTest>[] PropertiesTheoryData =>
    [
        new PropertiesTest
        {
            Name = "Scalar identity with int value",
            Value = ScalarIntegerPart,
            ExpectedIsScalarValue = true,
            ExpectedIsObjectValue = false,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedIsFullyResolved = true,
            ExpectedScalarValue = ApiId.FromInt32(42)
        },
        new PropertiesTest
        {
            Name = "Scalar identity with empty value",
            Value = ScalarEmptyPart,
            ExpectedIsScalarValue = true,
            ExpectedIsObjectValue = false,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedIsFullyResolved = true,
            ExpectedScalarValue = ApiId.Empty
        },
        new PropertiesTest
        {
            Name = "Object single part with resolved value",
            Value = ObjectSinglePart,
            ExpectedIsScalarValue = false,
            ExpectedIsObjectValue = true,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedIsFullyResolved = true
        },
        new PropertiesTest
        {
            Name = "Object single part with unresolved value is not fully resolved",
            Value = ObjectSinglePartUnresolved,
            ExpectedIsScalarValue = false,
            ExpectedIsObjectValue = true,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedIsFullyResolved = false
        },
        new PropertiesTest
        {
            Name = "Composite with scalar parts",
            Value = CompositeWithScalarParts,
            ExpectedIsScalarValue = false,
            ExpectedIsObjectValue = false,
            ExpectedIsComposite = true,
            ExpectedPartCount = 2,
            ExpectedIsFullyResolved = true
        },
        new PropertiesTest
        {
            Name = "Composite with resolved object parts",
            Value = CompositeWithObjectParts,
            ExpectedIsScalarValue = false,
            ExpectedIsObjectValue = false,
            ExpectedIsComposite = true,
            ExpectedPartCount = 2,
            ExpectedIsFullyResolved = true
        },
        new PropertiesTest
        {
            Name = "Composite with unresolved object parts is not fully resolved",
            Value = CompositeWithUnresolvedObjectParts,
            ExpectedIsScalarValue = false,
            ExpectedIsObjectValue = false,
            ExpectedIsComposite = true,
            ExpectedPartCount = 2,
            ExpectedIsFullyResolved = false
        },
        new PropertiesTest
        {
            Name = "Composite with partially unresolved object parts is not fully resolved",
            Value = CompositeWithPartiallyUnresolvedObjectParts,
            ExpectedIsScalarValue = false,
            ExpectedIsObjectValue = false,
            ExpectedIsComposite = true,
            ExpectedPartCount = 2,
            ExpectedIsFullyResolved = false
        },
        new PropertiesTest
        {
            Name = "Composite with deep object parts",
            Value = CompositeWithDeepObjectParts,
            ExpectedIsScalarValue = false,
            ExpectedIsObjectValue = false,
            ExpectedIsComposite = true,
            ExpectedPartCount = 2,
            ExpectedIsFullyResolved = true
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] ApiScalarValueTheoryData =>
    [
        new ApiScalarValueTest
        {
            Name = "Scalar identity with int value returns scalar value",
            Value = ScalarIntegerPart,
            ExpectedScalarValue = ApiId.FromInt32(42)
        },
        new ApiScalarValueTest
        {
            Name = "Scalar identity with empty value returns empty scalar value",
            Value = ScalarEmptyPart,
            ExpectedScalarValue = ApiId.Empty
        },
        new ApiScalarValueTest
        {
            Name = "Composite identity throws ApiIdentityException",
            Value = CompositeWithScalarParts,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] ApiObjectValueTheoryData =>
    [
        new ApiObjectValueTest
        {
            Name = "Object single part with resolved value returns nested identity",
            Value = ObjectSinglePart,
            ExpectedObjectValue = ApiIdentityValue.Scalar("Id", ApiId.FromInt32(42))
        },
        new ApiObjectValueTest
        {
            Name = "Object single part with unresolved value throws ApiIdentityException",
            Value = ObjectSinglePartUnresolved,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
        new ApiObjectValueTest
        {
            Name = "Scalar identity throws ApiIdentityException",
            Value = ScalarIntegerPart,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
        new ApiObjectValueTest
        {
            Name = "Composite identity throws ApiIdentityException",
            Value = CompositeWithScalarParts,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(PropertiesTheoryData))]
    public void Properties(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ApiScalarValueTheoryData))]
    public void ApiScalarValue(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ApiObjectValueTheoryData))]
    public void ApiObjectValue(IXUnitTest test) => test.Execute(this);
    #endregion
}
