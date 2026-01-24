// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentitySnapshotTests
{
    #region Test Types
    private class ConstructionTest : XUnitTest
    {
        #region User Supplied Properties
        public string? ConstructorName { get; init; }
        public IReadOnlyDictionary<string, ApiIdentityPart>? Parts { get; init; }
        public string? ParentPath { get; init; }
        public bool ExpectException { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        public int ExpectedPartCount { get; init; }
        public bool ExpectedIsScalar { get; init; }
        public bool ExpectedIsComposite { get; init; }
        public string? ExpectedPath { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentitySnapshot? ActualSnapshot { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            try
            {
                this.ActualSnapshot = new ApiIdentitySnapshot(this.ConstructorName!, this.Parts!, this.ParentPath);
            }
            catch (Exception ex)
            {
                this.ActualException = ex;
            }
        }

        protected override void Assert()
        {
            if (this.ExpectException)
            {
                this.ActualException.Should().NotBeNull();
                this.ActualException.Should().BeOfType(this.ExpectedExceptionType!);
                return;
            }

            this.ActualException.Should().BeNull();
            this.ActualSnapshot.Should().NotBeNull();
            this.ActualSnapshot!.Name.Should().Be(this.ConstructorName);
            this.ActualSnapshot.Path.Should().Be(this.ExpectedPath ?? this.ConstructorName);
            this.ActualSnapshot.PartCount.Should().Be(this.ExpectedPartCount);
            this.ActualSnapshot.IsScalar.Should().Be(this.ExpectedIsScalar);
            this.ActualSnapshot.IsComposite.Should().Be(this.ExpectedIsComposite);
        }
        #endregion
    }

    private class FactoryMethodTest : XUnitTest
    {
        #region User Supplied Properties
        public required Expression<Func<ApiIdentitySnapshot>> FactoryExpression { get; init; }
        public string ExpectedName { get; init; } = null!;
        public bool ExpectedIsScalar { get; init; }
        public bool ExpectedIsComposite { get; init; }
        public int ExpectedPartCount { get; init; }
        public ApiId? ExpectedScalarValue { get; init; }
        public string? ExpectedPath { get; init; }
        #endregion

        #region Calculated Properties
        private Func<ApiIdentitySnapshot> Factory { get; set; } = null!;
        private ApiIdentitySnapshot? ActualSnapshot { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Factory = this.FactoryExpression.Compile();
        }

        protected override void Act()
        {
            this.ActualSnapshot = this.Factory();
        }

        protected override void Assert()
        {
            this.ActualSnapshot.Should().NotBeNull();
            this.ActualSnapshot!.Name.Should().Be(this.ExpectedName);
            this.ActualSnapshot.IsScalar.Should().Be(this.ExpectedIsScalar);
            this.ActualSnapshot.IsComposite.Should().Be(this.ExpectedIsComposite);
            this.ActualSnapshot.PartCount.Should().Be(this.ExpectedPartCount);

            if (this.ExpectedPath != null)
            {
                this.ActualSnapshot.Path.Should().Be(this.ExpectedPath);
            }

            if (this.ExpectedScalarValue.HasValue && this.ExpectedIsScalar)
            {
                this.ActualSnapshot.ScalarValue.Should().Be(this.ExpectedScalarValue.Value);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ConstructionTheoryData =>
    [
        new ConstructionTest
        {
            Name = "Constructor with null name throws ArgumentNullException",
            ConstructorName = null,
            Parts = new Dictionary<string, ApiIdentityPart>(),
            ExpectException = true,
            ExpectedExceptionType = typeof(ArgumentNullException)
        },
        new ConstructionTest
        {
            Name = "Constructor with empty name throws ArgumentException",
            ConstructorName = "",
            Parts = new Dictionary<string, ApiIdentityPart>(),
            ExpectException = true,
            ExpectedExceptionType = typeof(ArgumentException)
        },
        new ConstructionTest
        {
            Name = "Constructor with whitespace name throws ArgumentException",
            ConstructorName = "  ",
            Parts = new Dictionary<string, ApiIdentityPart>(),
            ExpectException = true,
            ExpectedExceptionType = typeof(ArgumentException)
        },
        new ConstructionTest
        {
            Name = "Constructor with null parts throws ArgumentNullException",
            ConstructorName = "Product",
            Parts = null,
            ExpectException = true,
            ExpectedExceptionType = typeof(ArgumentNullException)
        },
        new ConstructionTest
        {
            Name = "Constructor with empty parts creates empty snapshot",
            ConstructorName = "Empty",
            Parts = new Dictionary<string, ApiIdentityPart>(),
            ExpectedPartCount = 0,
            ExpectedIsScalar = false,
            ExpectedIsComposite = false,
            ExpectedPath = "Empty"
        },
        new ConstructionTest
        {
            Name = "Constructor with single scalar ApiId creates scalar snapshot",
            ConstructorName = "Product",
            Parts = new Dictionary<string, ApiIdentityPart> { ["Value"] = ApiIdentityPart.Scalar(ApiId.FromInt32(42)) },
            ExpectedPartCount = 1,
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedPath = "Product"
        },
        new ConstructionTest
        {
            Name = "Constructor with multiple scalars creates composite",
            ConstructorName = "Customer",
            Parts = new Dictionary<string, ApiIdentityPart>
            {
                ["CustomerId"] = ApiIdentityPart.Scalar(ApiId.FromInt32(100)),
                ["CountryId"] = ApiIdentityPart.Scalar(ApiId.FromInt32(1))
            },
            ExpectedPartCount = 2,
            ExpectedIsScalar = false,
            ExpectedIsComposite = true,
            ExpectedPath = "Customer"
        },
        new ConstructionTest
        {
            Name = "Constructor with nested snapshot creates composite",
            ConstructorName = "Order",
            Parts = new Dictionary<string, ApiIdentityPart>
            {
                ["Customer"] = ApiIdentityPart.Nested(ApiIdentitySnapshot.Scalar("Customer", ApiId.FromInt32(42))),
                ["OrderNumber"] = ApiIdentityPart.Scalar(ApiId.FromInt64(1001L))
            },
            ExpectedPartCount = 2,
            ExpectedIsScalar = false,
            ExpectedIsComposite = true,
            ExpectedPath = "Order"
        },
        new ConstructionTest
        {
            Name = "Constructor with parentPath creates correct path",
            ConstructorName = "CustomerId",
            Parts = new Dictionary<string, ApiIdentityPart> { ["Value"] = ApiIdentityPart.Scalar(ApiId.FromInt32(42)) },
            ParentPath = "Order.Customer",
            ExpectedPartCount = 1,
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedPath = "Order.Customer.CustomerId"
        },
        new ConstructionTest
        {
            Name = "Constructor converts object to ApiId",
            ConstructorName = "Product",
            Parts = new Dictionary<string, ApiIdentityPart> { ["ProductId"] = ApiIdentityPart.Scalar(ApiId.FromInt32(42)) },
            ExpectedPartCount = 1,
            ExpectedIsScalar = false,
            ExpectedIsComposite = false,
            ExpectedPath = "Product"
        },
        new ConstructionTest
        {
            Name = "Constructor accepts null nested snapshot",
            ConstructorName = "Order",
            Parts = new Dictionary<string, ApiIdentityPart>
            {
                ["Customer"] = ApiIdentityPart.UnresolvedNested(),  // Explicit null nested snapshot
                ["Product"] = ApiIdentityPart.Nested(ApiIdentitySnapshot.Scalar("Product", ApiId.FromInt32(99), "Order")),  // Non-null to enable heuristic
                ["OrderNumber"] = ApiIdentityPart.Scalar(ApiId.FromInt64(1001L))
            },
            ExpectedPartCount = 3,  // Customer (null) + Product + OrderNumber
            ExpectedIsScalar = false,
            ExpectedIsComposite = true,
            ExpectedPath = "Order"
        },
    ];

    private static Dictionary<string, ApiIdentityPart> OrderParts { get; } = new()
    {
        ["CustomerId"] = ApiIdentityPart.Scalar(ApiId.FromInt32(42)),
        ["OrderNumber"] = ApiIdentityPart.Scalar(ApiId.FromInt64(1001L))
    };

    private static Dictionary<string, ApiIdentityPart> CustomerParts { get; } = new()
    {
        ["CustomerId"] = ApiIdentityPart.Scalar(ApiId.FromInt32(42))
    };

    public static TheoryDataRow<IXUnitTest>[] FactoryMethodTheoryData =>
    [
        new FactoryMethodTest
        {
            Name = "Scalar factory from ApiId creates scalar snapshot",
            FactoryExpression = () => ApiIdentitySnapshot.Scalar("ProductId", ApiId.FromInt32(42), null),
            ExpectedName = "ProductId",
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedScalarValue = ApiId.FromInt32(42),
            ExpectedPath = "ProductId"
        },
        new FactoryMethodTest
        {
            Name = "Scalar factory from int converts to ApiId",
            FactoryExpression = () => ApiIdentitySnapshot.Scalar("Count", 100, null),
            ExpectedName = "Count",
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedScalarValue = ApiId.FromInt32(100),
            ExpectedPath = "Count"
        },
        new FactoryMethodTest
        {
            Name = "Scalar factory from long converts to ApiId",
            FactoryExpression = () => ApiIdentitySnapshot.Scalar("OrderNumber", 1001L, null),
            ExpectedName = "OrderNumber",
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedScalarValue = ApiId.FromInt64(1001L),
            ExpectedPath = "OrderNumber"
        },
        new FactoryMethodTest
        {
            Name = "Scalar factory from string converts to ApiId",
            FactoryExpression = () => ApiIdentitySnapshot.Scalar("Name", "Test", null),
            ExpectedName = "Name",
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedScalarValue = ApiId.FromString("Test"),
            ExpectedPath = "Name"
        },
        new FactoryMethodTest
        {
            Name = "Scalar factory from Guid converts to ApiId",
            FactoryExpression = () => ApiIdentitySnapshot.Scalar("Id", Guid.Parse("12345678-1234-1234-1234-123456789abc"), null),
            ExpectedName = "Id",
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedScalarValue = ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")),
            ExpectedPath = "Id"
        },
        new FactoryMethodTest
        {
            Name = "Scalar factory with parentPath creates correct path",
            FactoryExpression = () => ApiIdentitySnapshot.Scalar("CustomerId", 42, "Order.Customer"),
            ExpectedName = "CustomerId",
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedPartCount = 1,
            ExpectedScalarValue = ApiId.FromInt32(42),
            ExpectedPath = "Order.Customer.CustomerId"
        },
        new FactoryMethodTest
        {
            Name = "Composite factory creates composite snapshot",
            FactoryExpression = () => ApiIdentitySnapshot.Composite
            (
                "Order",
                new Dictionary<string, ApiIdentityPart>(OrderParts),
                null
            ),
            ExpectedName = "Order",
            ExpectedIsScalar = false,
            ExpectedIsComposite = true,
            ExpectedPartCount = 2,
            ExpectedPath = "Order"
        },
        new FactoryMethodTest
        {
            Name = "Composite factory with parentPath creates correct path",
            FactoryExpression = () => ApiIdentitySnapshot.Composite
            (
                "Customer",
                new Dictionary<string, ApiIdentityPart>(CustomerParts),
                "Order"),
            ExpectedName = "Customer",
            ExpectedIsScalar = false,
            ExpectedIsComposite = false, // Only 1 part, so not composite
            ExpectedPartCount = 1,
            ExpectedPath = "Order.Customer"
        },
        new FactoryMethodTest
        {
            Name = "Empty factory creates empty snapshot",
            FactoryExpression = () => ApiIdentitySnapshot.Empty("Empty", null),
            ExpectedName = "Empty",
            ExpectedIsScalar = false,
            ExpectedIsComposite = false,
            ExpectedPartCount = 0,
            ExpectedPath = "Empty"
        },
        new FactoryMethodTest
        {
            Name = "Empty factory with parentPath creates correct path",
            FactoryExpression = () => ApiIdentitySnapshot.Empty("Empty", "Root"),
            ExpectedName = "Empty",
            ExpectedIsScalar = false,
            ExpectedIsComposite = false,
            ExpectedPartCount = 0,
            ExpectedPath = "Root.Empty"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ConstructionTheoryData))]
    public void Construction(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(FactoryMethodTheoryData))]
    public void FactoryMethod(IXUnitTest test) => test.Execute(this);
    #endregion
}
