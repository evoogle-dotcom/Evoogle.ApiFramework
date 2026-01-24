// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.ApiFramework.Exceptions;
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
        public bool ExpectException { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        public ApiId? ExpectedApiId { get; init; }
        public bool ExpectedIsComposite { get; init; }
        public bool? ExpectedIsNamedComposite { get; init; }
        public int ExpectedPartCount { get; init; }
        public Expression<Func<ApiId, bool>>? CustomAssertionExpression { get; init; }
        #endregion

        #region Calculated Properties
        private Func<ApiId, bool>? CustomAssertion { get; set; }
        private ApiId ActualApiId { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            if (this.CustomAssertionExpression != null)
            {
                this.CustomAssertion = this.CustomAssertionExpression.Compile();
            }
        }

        protected override void Act()
        {
            try
            {
                this.ActualApiId = this.Snapshot.ToApiId(this.UseNamedParts);
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
            this.ActualApiId.IsComposite.Should().Be(this.ExpectedIsComposite);

            if (this.ExpectedIsComposite)
            {
                if (this.ExpectedIsNamedComposite.HasValue)
                {
                    this.ActualApiId.IsNamedComposite.Should().Be(this.ExpectedIsNamedComposite.Value);
                }

                this.ActualApiId.PartCount.Should().Be(this.ExpectedPartCount);
            }

            if (this.ExpectedApiId.HasValue)
            {
                this.ActualApiId.Should().Be(this.ExpectedApiId.Value);
            }

            if (this.CustomAssertion != null)
            {
                this.CustomAssertion(this.ActualApiId).Should().BeTrue();
            }
        }
        #endregion
    }

    private class CachingTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public bool UseNamedParts { get; init; }
        #endregion

        #region Calculated Properties
        private ApiId FirstCall { get; set; }
        private ApiId SecondCall { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            this.FirstCall = this.Snapshot.ToApiId(this.UseNamedParts);
            this.SecondCall = this.Snapshot.ToApiId(this.UseNamedParts);
        }

        protected override void Assert()
        {
            // Both calls should return equal values (cached)
            this.FirstCall.Should().Be(this.SecondCall);
        }
        #endregion
    }

    private class DualCachingTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        #endregion

        #region Calculated Properties
        private ApiId NamedFirst { get; set; }
        private ApiId UnnamedFirst { get; set; }
        private ApiId NamedSecond { get; set; }
        private ApiId UnnamedSecond { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            this.NamedFirst = this.Snapshot.ToApiId(useNamedParts: true);
            this.UnnamedFirst = this.Snapshot.ToApiId(useNamedParts: false);
            this.NamedSecond = this.Snapshot.ToApiId(useNamedParts: true);
            this.UnnamedSecond = this.Snapshot.ToApiId(useNamedParts: false);
        }

        protected override void Assert()
        {
            // Named caching
            this.NamedFirst.Should().Be(this.NamedSecond);
            this.NamedFirst.IsNamedComposite.Should().BeTrue();

            // Unnamed caching
            this.UnnamedFirst.Should().Be(this.UnnamedSecond);
            this.UnnamedFirst.IsOrderedComposite.Should().BeTrue();

            // Named and unnamed should be different
            this.NamedFirst.Should().NotBe(this.UnnamedFirst);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ToApiIdTheoryData =>
    [
        new ToApiIdTest
        {
            Name = "Scalar snapshot flattens to scalar ApiId",
            Snapshot = ApiIdentitySnapshot.Scalar("ProductId", 42),
            UseNamedParts = true,
            ExpectedApiId = ApiId.FromInt32(42),
            ExpectedIsComposite = false,
            ExpectedPartCount = 0
        },
        new ToApiIdTest
        {
            Name = "Empty snapshot flattens to ApiId.Empty",
            Snapshot = ApiIdentitySnapshot.Empty("Empty"),
            UseNamedParts = true,
            ExpectedApiId = ApiId.Empty,
            ExpectedIsComposite = false,
            ExpectedPartCount = 0
        },
        new ToApiIdTest
        {
            Name = "Simple composite flattens with named parts",
            Snapshot = ApiIdentitySnapshot.Composite("Order", new Dictionary<string, ApiIdentityPart>
            {
                ["CustomerId"] = ApiIdentityPart.Scalar(ApiId.FromInt32(42)),
                ["OrderNumber"] = ApiIdentityPart.Scalar(ApiId.FromInt64(1001L))
            }),
            UseNamedParts = true,
            ExpectedIsComposite = true,
            ExpectedIsNamedComposite = true,
            ExpectedPartCount = 2
        },
        new ToApiIdTest
        {
            Name = "Simple composite flattens with unnamed parts",
            Snapshot = ApiIdentitySnapshot.Composite("Order", new Dictionary<string, ApiIdentityPart>
            {
                ["CustomerId"] = ApiIdentityPart.Scalar(ApiId.FromInt32(42)),
                ["OrderNumber"] = ApiIdentityPart.Scalar(ApiId.FromInt64(1001L))
            }),
            UseNamedParts = false,
            ExpectedIsComposite = true,
            ExpectedIsNamedComposite = false,
            ExpectedPartCount = 2
        },
        new ToApiIdTest
        {
            Name = "Nested composite flattens all levels with named parts",
            Snapshot = CreateOrderSnapshot(),
            UseNamedParts = true,
            ExpectedIsComposite = true,
            ExpectedIsNamedComposite = true,
            ExpectedPartCount = 3 // Customer.Country, Customer.CustomerId, OrderNumber
        },
        new ToApiIdTest
        {
            Name = "Nested composite flattens all levels with unnamed parts",
            Snapshot = CreateOrderSnapshot(),
            UseNamedParts = false,
            ExpectedIsComposite = true,
            ExpectedIsNamedComposite = false,
            ExpectedPartCount = 3
        },
        new ToApiIdTest
        {
            Name = "Deeply nested composite flattens correctly",
            Snapshot = CreateDeeplyNestedSnapshot(),
            UseNamedParts = true,
            ExpectedIsComposite = true,
            ExpectedIsNamedComposite = true,
            ExpectedPartCount = 4 // RootId, Level1.Name, Level1.Level2.Id, Level1.Level2.Level3
        },
        new ToApiIdTest
        {
            Name = "Single unnamed part returns as scalar ApiId",
            Snapshot = ApiIdentitySnapshot.Composite("Product", new Dictionary<string, ApiIdentityPart>
            {
                ["ProductId"] = ApiIdentityPart.Scalar(ApiId.FromInt32(99))
            }),
            UseNamedParts = false,
            ExpectedApiId = ApiId.FromInt32(99),
            ExpectedIsComposite = false,
            ExpectedPartCount = 0
        },
        new ToApiIdTest
        {
            Name = "ToApiId throws exception for unresolved nested part",
            Snapshot = CreateUnresolvedSnapshot(),
            UseNamedParts = true,
            ExpectException = true,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
        new ToApiIdTest
        {
            Name = "Mixed scalar types flatten correctly with named parts",
            Snapshot = ApiIdentitySnapshot.Composite("Invoice", new Dictionary<string, ApiIdentityPart>
            {
                ["InvoiceId"] = ApiIdentityPart.Scalar(ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))),
                ["InvoiceNumber"] = ApiIdentityPart.Scalar(ApiId.FromString("INV-2024-001")),
                ["Amount"] = ApiIdentityPart.Scalar(ApiId.FromInt32(100))
            }),
            UseNamedParts = true,
            ExpectedIsComposite = true,
            ExpectedIsNamedComposite = true,
            ExpectedPartCount = 3
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] CachingTheoryData =>
    [
        new CachingTest
        {
            Name = "Named parts result is cached",
            Snapshot = CreateOrderSnapshot(),
            UseNamedParts = true
        },
        new CachingTest
        {
            Name = "Unnamed parts result is cached",
            Snapshot = CreateOrderSnapshot(),
            UseNamedParts = false
        },
        new CachingTest
        {
            Name = "Scalar snapshot caching works",
            Snapshot = ApiIdentitySnapshot.Scalar("Id", 42),
            UseNamedParts = true
        },
        new CachingTest
        {
            Name = "Empty snapshot caching works",
            Snapshot = ApiIdentitySnapshot.Empty("Empty"),
            UseNamedParts = true
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] DualCachingTheoryData =>
    [
        new DualCachingTest
        {
            Name = "Named and unnamed caches are independent",
            Snapshot = CreateOrderSnapshot()
        },
        new DualCachingTest
        {
            Name = "Simple composite has independent caches",
            Snapshot = ApiIdentitySnapshot.Composite("Product", new Dictionary<string, ApiIdentityPart>
            {
                ["Id"] = ApiIdentityPart.Scalar(ApiId.FromInt32(42)),
                ["Name"] = ApiIdentityPart.Scalar(ApiId.FromString("Widget"))
            })
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ToApiIdTheoryData))]
    public void ToApiId(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(CachingTheoryData))]
    public void Caching(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(DualCachingTheoryData))]
    public void DualCaching(IXUnitTest test) => test.Execute(this);
    #endregion
}
