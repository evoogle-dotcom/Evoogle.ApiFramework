// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentitySnapshotTests
{
    #region Test Types
    private class IndexerTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public string PathOrName { get; init; } = null!;
        public bool ExpectException { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        public string? ExpectedPath { get; init; }
        public string? ExpectedName { get; init; }
        public bool? ExpectedIsScalar { get; init; }
        public ApiId? ExpectedScalarValue { get; init; }
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
                this.ActualSnapshot = this.Snapshot[this.PathOrName];
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

            if (this.ExpectedPath != null)
            {
                this.ActualSnapshot!.Path.Should().Be(this.ExpectedPath);
            }

            if (this.ExpectedName != null)
            {
                this.ActualSnapshot!.Name.Should().Be(this.ExpectedName);
            }

            if (this.ExpectedIsScalar.HasValue)
            {
                this.ActualSnapshot!.IsScalar.Should().Be(this.ExpectedIsScalar.Value);
            }

            if (this.ExpectedScalarValue.HasValue)
            {
                this.ActualSnapshot!.ScalarValue.Should().Be(this.ExpectedScalarValue.Value);
            }
        }
        #endregion
    }

    private class TryGetPartTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public string PartName { get; init; } = null!;
        public bool ExpectedResult { get; init; }
        public string? ExpectedName { get; init; }
        public ApiId? ExpectedScalarValue { get; init; }
        #endregion

        #region Calculated Properties
        private bool ActualResult { get; set; }
        private ApiIdentitySnapshot? ActualSnapshot { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            ApiIdentitySnapshot? temp;
            this.ActualResult = this.Snapshot.TryGetPart(this.PartName, out temp);
            this.ActualSnapshot = temp;
        }

        protected override void Assert()
        {
            this.ActualResult.Should().Be(this.ExpectedResult);

            if (this.ExpectedResult)
            {
                this.ActualSnapshot.Should().NotBeNull();

                if (this.ExpectedName != null)
                {
                    this.ActualSnapshot!.Name.Should().Be(this.ExpectedName);
                }

                if (this.ExpectedScalarValue.HasValue)
                {
                    this.ActualSnapshot!.ScalarValue.Should().Be(this.ExpectedScalarValue.Value);
                }
            }
            else
            {
                this.ActualSnapshot.Should().BeNull();
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] IndexerTheoryData =>
    [
        new IndexerTest
        {
            Name = "Indexer access to direct scalar part",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "OrderNumber",
            ExpectedName = "OrderNumber",
            ExpectedPath = "Order.OrderNumber",
            ExpectedIsScalar = true,
            ExpectedScalarValue = ApiId.FromInt64(1001L)
        },
        new IndexerTest
        {
            Name = "Indexer access to direct composite part",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "Customer",
            ExpectedName = "Customer",
            ExpectedPath = "Order.Customer",
            ExpectedIsScalar = false
        },
        new IndexerTest
        {
            Name = "Indexer with dot notation navigates nested path",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "Customer.CustomerId",
            ExpectedName = "CustomerId",
            ExpectedPath = "Order.Customer.CustomerId",
            ExpectedIsScalar = true,
            ExpectedScalarValue = ApiId.FromInt32(42)
        },
        new IndexerTest
        {
            Name = "Indexer with deep dot notation navigates multiple levels",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "Customer.Country.Id",
            ExpectedName = "Id",
            ExpectedPath = "Order.Customer.Country.Id",
            ExpectedIsScalar = true,
            ExpectedScalarValue = ApiId.FromInt32(1)
        },
        new IndexerTest
        {
            Name = "Indexer wraps scalar ApiId in snapshot for consistent API",
            Snapshot = ApiIdentitySnapshot.Composite(
                "Product",
                [
                    ScalarEntry("ProductId", ApiId.FromInt32(99)),
                    ScalarEntry("Name", ApiId.FromString("Widget"))
                ]
            ),
            PathOrName = "ProductId",
            ExpectedName = "ProductId",
            ExpectedPath = "Product.ProductId",
            ExpectedIsScalar = true,
            ExpectedScalarValue = ApiId.FromInt32(99)
        },
        new IndexerTest
        {
            Name = "Indexer throws KeyNotFoundException for non-existent part",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "NonExistent",
            ExpectException = true,
            ExpectedExceptionType = typeof(KeyNotFoundException)
        },
        new IndexerTest
        {
            Name = "Indexer throws KeyNotFoundException for invalid nested path",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "Customer.Invalid.Path",
            ExpectException = true,
            ExpectedExceptionType = typeof(KeyNotFoundException)
        },
        new IndexerTest
        {
            Name = "Indexer throws ApiIdentityException for null nested part",
            Snapshot = CreateUnresolvedSnapshot(),
            PathOrName = "Customer",
            ExpectException = true,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
        new IndexerTest
        {
            Name = "Indexer throws ArgumentException for null or whitespace path",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "",
            ExpectException = true,
            ExpectedExceptionType = typeof(ArgumentException)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetPartTheoryData =>
    [
        new TryGetPartTest
        {
            Name = "TryGetPart returns true for existing nested snapshot",
            Snapshot = CreateOrderSnapshot(),
            PartName = "Customer",
            ExpectedResult = true,
            ExpectedName = "Customer"
        },
        new TryGetPartTest
        {
            Name = "TryGetPart returns true for existing scalar",
            Snapshot = CreateOrderSnapshot(),
            PartName = "OrderNumber",
            ExpectedResult = true,
            ExpectedName = "OrderNumber",
            ExpectedScalarValue = ApiId.FromInt64(1001L)
        },
        new TryGetPartTest
        {
            Name = "TryGetPart returns false for non-existent part",
            Snapshot = CreateOrderSnapshot(),
            PartName = "NonExistent",
            ExpectedResult = false
        },
        new TryGetPartTest
        {
            Name = "TryGetPart returns false for null nested snapshot",
            Snapshot = CreateUnresolvedSnapshot(),
            PartName = "Customer",
            ExpectedResult = false
        },
        new TryGetPartTest
        {
            Name = "TryGetPart wraps scalar ApiId in snapshot",
            Snapshot = ApiIdentitySnapshot.Composite(
                "Invoice",
                [
                    ScalarEntry("InvoiceId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))),
                    ScalarEntry("Amount", ApiId.FromInt32(100))
                ]
            ),
            PartName = "InvoiceId",
            ExpectedResult = true,
            ExpectedName = "InvoiceId",
            ExpectedScalarValue = ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(IndexerTheoryData))]
    public void Indexer(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetPartTheoryData))]
    public void TryGetPart(IXUnitTest test) => test.Execute(this);
    #endregion
}
