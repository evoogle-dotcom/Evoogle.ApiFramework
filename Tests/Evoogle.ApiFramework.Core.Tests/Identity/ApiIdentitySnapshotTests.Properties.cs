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
    private class PropertyTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public bool ExpectedIsScalar { get; init; }
        public bool ExpectedIsComposite { get; init; }
        public bool ExpectedIsFullyResolved { get; init; }
        public int ExpectedPartCount { get; init; }
        public string[]? ExpectedPartNames { get; init; }
        public string ExpectedPath { get; init; } = null!;
        public string ExpectedName { get; init; } = null!;
        public ApiId? ExpectedScalarValue { get; init; }
        public bool ExpectScalarValueException { get; init; }
        #endregion

        #region XUnitTest Methods
        protected override void Assert()
        {
            this.Snapshot.IsScalar.Should().Be(this.ExpectedIsScalar);
            this.Snapshot.IsComposite.Should().Be(this.ExpectedIsComposite);
            this.Snapshot.IsFullyResolved.Should().Be(this.ExpectedIsFullyResolved);
            this.Snapshot.PartCount.Should().Be(this.ExpectedPartCount);
            this.Snapshot.Path.Should().Be(this.ExpectedPath);
            this.Snapshot.Name.Should().Be(this.ExpectedName);

            if (this.ExpectedPartNames != null)
            {
                this.Snapshot.PartNames.Should().BeEquivalentTo(this.ExpectedPartNames);
            }

            if (this.ExpectedScalarValue.HasValue)
            {
                this.Snapshot.ScalarValue.Should().Be(this.ExpectedScalarValue.Value);
            }

            if (this.ExpectScalarValueException)
            {
                var act = () => this.Snapshot.ScalarValue;
                act.Should().Throw<ApiIdentityException>();
            }
        }
        #endregion
    }

    private class GetScalarValueTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public string PathOrName { get; init; } = null!;
        public Type TargetType { get; init; } = null!;
        public bool ExpectException { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        public object? ExpectedValue { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualValue { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            try
            {
                if (this.TargetType == typeof(int))
                {
                    this.ActualValue = this.Snapshot.GetScalarValue<int>(this.PathOrName);
                }
                else if (this.TargetType == typeof(long))
                {
                    this.ActualValue = this.Snapshot.GetScalarValue<long>(this.PathOrName);
                }
                else if (this.TargetType == typeof(string))
                {
                    this.ActualValue = this.Snapshot.GetScalarValue<string>(this.PathOrName);
                }
                else if (this.TargetType == typeof(Guid))
                {
                    this.ActualValue = this.Snapshot.GetScalarValue<Guid>(this.PathOrName);
                }
                else
                {
                    throw new NotSupportedException($"Type {this.TargetType} not supported in test");
                }
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
            this.ActualValue.Should().Be(this.ExpectedValue);
        }
        #endregion
    }

    private class GetScalarApiIdTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public string PathOrName { get; init; } = null!;
        public bool ExpectException { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        public ApiId? ExpectedApiId { get; init; }
        #endregion

        #region Calculated Properties
        private ApiId ActualApiId { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            try
            {
                this.ActualApiId = this.Snapshot.GetScalarApiId(this.PathOrName);
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

            if (this.ExpectedApiId.HasValue)
            {
                this.ActualApiId.Should().Be(this.ExpectedApiId.Value);
            }
        }
        #endregion
    }

    private class GetUnresolvedPartsTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdentitySnapshot Snapshot { get; init; } = null!;
        public string[] ExpectedUnresolvedPaths { get; init; } = [];
        #endregion

        #region Calculated Properties
        private string[] ActualUnresolvedPaths { get; set; } = [];
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            this.ActualUnresolvedPaths = this.Snapshot.GetUnresolvedParts().ToArray();
        }

        protected override void Assert()
        {
            this.ActualUnresolvedPaths.Should().BeEquivalentTo(this.ExpectedUnresolvedPaths);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] PropertyTheoryData =>
    [
        new PropertyTest
        {
            Name = "Scalar snapshot has correct properties",
            Snapshot = ApiIdentitySnapshot.Scalar("ProductId", 42),
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedIsFullyResolved = true,
            ExpectedPartCount = 1,
            ExpectedPath = "ProductId",
            ExpectedName = "ProductId",
            ExpectedPartNames = ["Value"],
            ExpectedScalarValue = ApiId.FromInt32(42)
        },
        new PropertyTest
        {
            Name = "Composite snapshot has correct properties",
            Snapshot = ApiIdentitySnapshot.Composite(
                "Order",
                [
                    ScalarEntry("CustomerId", ApiId.FromInt32(42)),
                    ScalarEntry("OrderNumber", ApiId.FromInt64(1001L))
                ]
            ),
            ExpectedIsScalar = false,
            ExpectedIsComposite = true,
            ExpectedIsFullyResolved = true,
            ExpectedPartCount = 2,
            ExpectedPath = "Order",
            ExpectedName = "Order",
            ExpectedPartNames = ["CustomerId", "OrderNumber"],
            ExpectScalarValueException = true
        },
        new PropertyTest
        {
            Name = "Nested snapshot is fully resolved",
            Snapshot = CreateOrderSnapshot(),
            ExpectedIsScalar = false,
            ExpectedIsComposite = true,
            ExpectedIsFullyResolved = true,
            ExpectedPartCount = 2,
            ExpectedPath = "Order",
            ExpectedName = "Order",
            ExpectedPartNames = ["Customer", "OrderNumber"]
        },
        new PropertyTest
        {
            Name = "Snapshot with null nested part is not fully resolved",
            Snapshot = CreateUnresolvedSnapshot(),
            ExpectedIsScalar = false,
            ExpectedIsComposite = true,  // 3 parts: null Customer + Product + OrderNumber
            ExpectedIsFullyResolved = false,
            ExpectedPartCount = 3,
            ExpectedPath = "Order",
            ExpectedName = "Order",
            ExpectedPartNames = ["Customer", "OrderNumber", "Product"]
        },
        new PropertyTest
        {
            Name = "Empty snapshot has correct properties",
            Snapshot = ApiIdentitySnapshot.Empty("Empty"),
            ExpectedIsScalar = false,
            ExpectedIsComposite = false,
            ExpectedIsFullyResolved = true,
            ExpectedPartCount = 0,
            ExpectedPath = "Empty",
            ExpectedName = "Empty",
            ExpectedPartNames = [],
            ExpectScalarValueException = true
        },
        new PropertyTest
        {
            Name = "Snapshot with parentPath has correct full path",
            Snapshot = ApiIdentitySnapshot.Scalar("CustomerId", 42, "Order.Customer"),
            ExpectedIsScalar = true,
            ExpectedIsComposite = false,
            ExpectedIsFullyResolved = true,
            ExpectedPartCount = 1,
            ExpectedPath = "Order.Customer.CustomerId",
            ExpectedName = "CustomerId",
            ExpectedScalarValue = ApiId.FromInt32(42)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] GetScalarValueTheoryData =>
    [
        new GetScalarValueTest
        {
            Name = "GetScalarValue<int> succeeds on scalar snapshot",
            Snapshot = ApiIdentitySnapshot.Scalar("Count", 42),
            PathOrName = "Value",
            TargetType = typeof(int),
            ExpectedValue = 42
        },
        new GetScalarValueTest
        {
            Name = "GetScalarValue<long> succeeds on scalar snapshot",
            Snapshot = ApiIdentitySnapshot.Scalar("OrderNumber", 1001L),
            PathOrName = "Value",
            TargetType = typeof(long),
            ExpectedValue = 1001L
        },
        new GetScalarValueTest
        {
            Name = "GetScalarValue<string> succeeds on scalar snapshot",
            Snapshot = ApiIdentitySnapshot.Scalar("Name", "Test"),
            PathOrName = "Value",
            TargetType = typeof(string),
            ExpectedValue = "Test"
        },
        new GetScalarValueTest
        {
            Name = "GetScalarValue<Guid> succeeds on scalar snapshot",
            Snapshot = ApiIdentitySnapshot.Scalar("Id", Guid.Parse("12345678-1234-1234-1234-123456789abc")),
            PathOrName = "Value",
            TargetType = typeof(Guid),
            ExpectedValue = Guid.Parse("12345678-1234-1234-1234-123456789abc")
        },
        new GetScalarValueTest
        {
            Name = "GetScalarValue with direct part name succeeds",
            Snapshot = ApiIdentitySnapshot.Composite(
                "Product",
                [
                    ScalarEntry("ProductId", ApiId.FromInt32(99)),
                    ScalarEntry("Name", ApiId.FromString("Widget"))
                ]
            ),
            PathOrName = "ProductId",
            TargetType = typeof(int),
            ExpectedValue = 99
        },
        new GetScalarValueTest
        {
            Name = "GetScalarValue with nested dot path succeeds",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "Customer.CustomerId",
            TargetType = typeof(int),
            ExpectedValue = 42
        },
        new GetScalarValueTest
        {
            Name = "GetScalarValue with deep nested path succeeds",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "Customer.Country.Id",
            TargetType = typeof(int),
            ExpectedValue = 1
        },
        new GetScalarValueTest
        {
            Name = "GetScalarValue on composite throws ApiIdentityException",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "Customer",
            TargetType = typeof(int),
            ExpectException = true,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
        new GetScalarValueTest
        {
            Name = "GetScalarValue with wrong type throws ApiIdentityException",
            Snapshot = ApiIdentitySnapshot.Scalar("Count", 42),
            PathOrName = "Value",
            TargetType = typeof(string),
            ExpectException = true,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] GetScalarApiIdTheoryData =>
    [
        new GetScalarApiIdTest
        {
            Name = "GetScalarApiId succeeds on scalar snapshot",
            Snapshot = ApiIdentitySnapshot.Scalar("ProductId", 42),
            PathOrName = "Value",
            ExpectedApiId = ApiId.FromInt32(42)
        },
        new GetScalarApiIdTest
        {
            Name = "GetScalarApiId with direct part name succeeds",
            Snapshot = ApiIdentitySnapshot.Composite(
                "Order",
                [
                    ScalarEntry("OrderNumber", ApiId.FromInt64(1001L)),
                    ScalarEntry("CustomerId", ApiId.FromInt32(42))
                ]
            ),
            PathOrName = "OrderNumber",
            ExpectedApiId = ApiId.FromInt64(1001L)
        },
        new GetScalarApiIdTest
        {
            Name = "GetScalarApiId with nested path succeeds",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "Customer.CustomerId",
            ExpectedApiId = ApiId.FromInt32(42)
        },
        new GetScalarApiIdTest
        {
            Name = "GetScalarApiId on composite throws ApiIdentityException",
            Snapshot = CreateOrderSnapshot(),
            PathOrName = "Customer",
            ExpectException = true,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] GetUnresolvedPartsTheoryData =>
    [
        new GetUnresolvedPartsTest
        {
            Name = "Fully resolved snapshot returns empty array",
            Snapshot = CreateOrderSnapshot(),
            ExpectedUnresolvedPaths = []
        },
        new GetUnresolvedPartsTest
        {
            Name = "Snapshot with one null part returns that path",
            Snapshot = CreateUnresolvedSnapshot(),
            ExpectedUnresolvedPaths = ["Order.Customer"]
        },
        new GetUnresolvedPartsTest
        {
            Name = "Snapshot with multiple unresolved parts returns all paths",
            Snapshot = ApiIdentitySnapshot.Composite(
                "Invoice",
                [
                    UnresolvedNestedEntry("Customer", [ ScalarEntry("CustomerId", ApiId.Empty) ]),
                    UnresolvedNestedEntry("Order", [ ScalarEntry("OrderNumber", ApiId.Empty) ]),
                    NestedEntry(
                        "Product",
                        ApiIdentitySnapshot.Scalar("Product", ApiId.FromInt32(99), "Invoice"),
                        Array.Empty<ApiIdentityPartEntry>()
                    ),
                    ScalarEntry("InvoiceNumber", ApiId.FromInt64(1001L))
                ]
            ),
            ExpectedUnresolvedPaths = ["Invoice.Customer", "Invoice.Order"]
        },
        new GetUnresolvedPartsTest
        {
            Name = "Nested snapshot with deep unresolved parts returns all paths",
            Snapshot =
                ApiIdentitySnapshot.Composite(
                    "Root",
                    [
                        NestedEntry(
                            "Level1",
                            ApiIdentitySnapshot.Composite(
                                "Level1",
                                [
                                    UnresolvedNestedEntry("Level2A", [ ScalarEntry("Value", ApiId.Empty) ]),
                                    NestedEntry(
                                        "Level2B",
                                        ApiIdentitySnapshot.Composite(
                                            "Level2B",
                                            [
                                                UnresolvedNestedEntry("Level3", [ ScalarEntry("Value", ApiId.Empty) ]),
                                                ScalarEntry("Anchor", ApiId.FromInt32(1)),
                                                ScalarEntry("Value", ApiId.FromInt32(42))
                                            ],
                                            "Root.Level1"
                                        ),
                                        [
                                            UnresolvedNestedEntry("Level3", [ ScalarEntry("Value", ApiId.Empty) ]),
                                            ScalarEntry("Anchor", ApiId.Empty),
                                            ScalarEntry("Value", ApiId.Empty)
                                        ]
                                    )
                                ],
                                "Root"
                            ),
                            [
                                UnresolvedNestedEntry("Level2A", [ ScalarEntry("Value", ApiId.Empty) ]),
                                UnresolvedNestedEntry(
                                    "Level2B",
                                    [
                                        UnresolvedNestedEntry("Level3", [ ScalarEntry("Value", ApiId.Empty) ]),
                                        ScalarEntry("Anchor", ApiId.Empty),
                                        ScalarEntry("Value", ApiId.Empty)
                                    ]
                                )
                            ]
                        ),
                        ScalarEntry("RootValue", ApiId.FromInt32(1))
                    ]
                ),
            ExpectedUnresolvedPaths = ["Root.Level1.Level2A", "Root.Level1.Level2B.Level3"]
        },
        new GetUnresolvedPartsTest
        {
            Name = "Empty snapshot returns empty array",
            Snapshot = ApiIdentitySnapshot.Empty("Empty"),
            ExpectedUnresolvedPaths = []
        },
        new GetUnresolvedPartsTest
        {
            Name = "Scalar snapshot returns empty array",
            Snapshot = ApiIdentitySnapshot.Scalar("Id", 42),
            ExpectedUnresolvedPaths = []
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(PropertyTheoryData))]
    public void Property(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(GetScalarValueTheoryData))]
    public void GetScalarValue(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(GetScalarApiIdTheoryData))]
    public void GetScalarApiId(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(GetUnresolvedPartsTheoryData))]
    public void GetUnresolvedParts(IXUnitTest test) => test.Execute(this);
    #endregion
}
