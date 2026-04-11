// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Unit tests for <see cref="ApiObjectTypeBuilder{T}"/>, <see cref="ApiIdentityBuilder{T}"/>, and
///     <see cref="IApiObjectTypeConfiguration{T}"/>.
/// </summary>
public class ApiObjectTypeBuilderGenericTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    /// <summary>
    ///     Verifies that expression-based <see cref="ApiObjectTypeBuilder{T}"/> overloads produce the same
    ///     <see cref="ApiObjectType"/> as the equivalent string-based non-generic overloads.
    /// </summary>
    private class BuildObjectTypeTest : XUnitTest
    {
        #region User Supplied Properties
        public required Func<ApiObjectType> BuildExpected { get; init; }
        public required Func<ApiObjectType> BuildActual { get; init; }
        #endregion

        #region Calculated Properties
        private ApiObjectType? Expected { get; set; }
        private ApiObjectType? Actual { get; set; }
        #endregion

        #region Constructors
        public BuildObjectTypeTest()
        {
            this.Name = nameof(BuildObjectTypeTest);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Expected = this.BuildExpected();
            this.WriteLine($"Expected: {this.Expected.SafeToString()}");
        }

        protected override void Act()
        {
            this.Actual = this.BuildActual();
            this.WriteLine($"Actual:   {this.Actual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.Actual.Should().NotBeNull();
            this.AssertBeEquivalentTo(this.Actual, this.Expected);
        }
        #endregion
    }

    /// <summary>
    ///     Verifies that expression-based <see cref="ApiIdentityBuilder{T}"/> overloads produce the same
    ///     <see cref="ApiIdentity"/> as the equivalent string-based non-generic overloads.
    /// </summary>
    private class BuildIdentityTest : XUnitTest
    {
        #region User Supplied Properties
        public required Func<ApiIdentity> BuildExpected { get; init; }
        public required Func<ApiIdentity> BuildActual { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentity? Expected { get; set; }
        private ApiIdentity? Actual { get; set; }
        #endregion

        #region Constructors
        public BuildIdentityTest()
        {
            this.Name = nameof(BuildIdentityTest);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Expected = this.BuildExpected();
            this.WriteLine($"Expected: {this.Expected.SafeToString()}");
        }

        protected override void Act()
        {
            this.Actual = this.BuildActual();
            this.WriteLine($"Actual:   {this.Actual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.Actual.Should().NotBeNull();
            this.AssertBeEquivalentTo(this.Actual, this.Expected);
        }
        #endregion
    }

    /// <summary>Local configuration class used by <see cref="IApiObjectTypeConfiguration{T}"/> tests.</summary>
    private sealed class CustomerConfiguration : IApiObjectTypeConfiguration<Customer>
    {
        public void Configure(ApiObjectTypeBuilder<Customer> builder)
        {
            builder
                .WithName("Customer")
                .AddProperty(c => c.Id)
                .AddProperty(c => c.Name)
                .AddIdentity("PK_Customer", (ApiIdentityBuilder<Customer> b) => b.AddScalar(c => c.Id, typeof(string)));
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildObjectTypeTheoryData =>
    [
        new BuildObjectTypeTest
        {
            Name = "ApiObjectTypeBuilder<Customer> AddProperty(expr) derives CLR name as API name",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder(typeof(Customer), ctx)
                    .WithName("Customer")
                    .AddProperty("Id", "Id")
                    .AddProperty("Name", "Name")
                    .Build();
            },
            BuildActual = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder<Customer>(ctx)
                    .WithName("Customer")
                    .AddProperty(c => c.Id)
                    .AddProperty(c => c.Name)
                    .Build();
            }
        },

        new BuildObjectTypeTest
        {
            Name = "ApiObjectTypeBuilder<Customer> AddProperty(expr, apiName) uses explicit API name with CLR name from expression",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder(typeof(Customer), ctx)
                    .WithName("Customer")
                    .AddProperty("customer.id", "Id")
                    .AddProperty("customer.name", "Name")
                    .Build();
            },
            BuildActual = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder<Customer>(ctx)
                    .WithName("Customer")
                    .AddProperty(c => c.Id, "customer.id")
                    .AddProperty(c => c.Name, "customer.name")
                    .Build();
            }
        },

        new BuildObjectTypeTest
        {
            Name = "ApiObjectTypeBuilder<Customer> AddIdentity with typed ApiIdentityBuilder<T> AddScalar(expr)",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder(typeof(Customer), ctx)
                    .WithName("Customer")
                    .AddProperty("Id", "Id")
                    .AddIdentity("PK_Customer", b => b.AddScalar("Id"))
                    .Build();
            },
            BuildActual = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder<Customer>(ctx)
                    .WithName("Customer")
                    .AddProperty(c => c.Id)
                    .AddIdentity("PK_Customer", (ApiIdentityBuilder<Customer> b) => b.AddScalar(c => c.Id))
                    .Build();
            }
        },

        new BuildObjectTypeTest
        {
            Name = "ApiObjectTypeBuilder<Customer> AddIdentity with typed ApiIdentityBuilder<T> AddScalar(expr, typeHint)",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder(typeof(Customer), ctx)
                    .WithName("Customer")
                    .AddProperty("Id", "Id")
                    .AddIdentity("PK_Customer", b => b.AddScalar("Id", typeof(string)))
                    .Build();
            },
            BuildActual = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder<Customer>(ctx)
                    .WithName("Customer")
                    .AddProperty(c => c.Id)
                    .AddIdentity("PK_Customer", (ApiIdentityBuilder<Customer> b) => b.AddScalar(c => c.Id, typeof(string)))
                    .Build();
            }
        },

        new BuildObjectTypeTest
        {
            Name = "IApiObjectTypeConfiguration<Customer> Configure routes to typed ApiObjectTypeBuilder<Customer>",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder(typeof(Customer), ctx)
                    .WithName("Customer")
                    .AddProperty("Id", "Id")
                    .AddProperty("Name", "Name")
                    .AddIdentity("PK_Customer", b => b.AddScalar("Id", typeof(string)))
                    .Build();
            },
            BuildActual = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                var builder = ctx.GetOrAddObjectTypeBuilder<Customer>();
                IApiObjectTypeConfiguration config = new CustomerConfiguration();
                config.Configure(builder);
                return builder.Build();
            }
        },

        new BuildObjectTypeTest
        {
            Name = "ApiObjectTypeBuilder<Customer> AddProperty(expr, configure) threads configure callback through",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder(typeof(Customer), ctx)
                    .WithName("Customer")
                    .AddProperty("Name", "Name", p => p.WithModifiers(m => m.Required()))
                    .Build();
            },
            BuildActual = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder<Customer>(ctx)
                    .WithName("Customer")
                    .AddProperty(c => c.Name, p => p.WithModifiers(m => m.Required()))
                    .Build();
            }
        },

        new BuildObjectTypeTest
        {
            Name = "ApiObjectTypeBuilder<Customer> WithOptions sets ApiIdentityNullHandling",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder(typeof(Customer), ctx)
                    .WithName("Customer")
                    .WithOptions(o => o.WithIdentityNullHandling(ApiIdentityNullHandling.ThrowException))
                    .Build();
            },
            BuildActual = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder<Customer>(ctx)
                    .WithName("Customer")
                    .WithOptions(o => o.WithIdentityNullHandling(ApiIdentityNullHandling.ThrowException))
                    .Build();
            }
        },

        new BuildObjectTypeTest
        {
            Name = "ApiObjectTypeBuilder<Customer> AddProperty(expr, apiName, configure) uses explicit API name and configure callback",
            BuildExpected = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder(typeof(Customer), ctx)
                    .WithName("Customer")
                    .AddProperty("customer.name", "Name", p => p.WithModifiers(m => m.Required()))
                    .Build();
            },
            BuildActual = static () =>
            {
                var ctx = new ApiSchemaBuilderContext();
                return new ApiObjectTypeBuilder<Customer>(ctx)
                    .WithName("Customer")
                    .AddProperty(c => c.Name, "customer.name", p => p.WithModifiers(m => m.Required()))
                    .Build();
            }
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildIdentityTheoryData =>
    [
        new BuildIdentityTest
        {
            Name = "ApiIdentityBuilder<Customer> AddScalar(expr) extracts CLR property name",
            BuildExpected = static () =>
                new ApiIdentityBuilder("PK_Customer")
                    .AddScalar("Id")
                    .Build(),
            BuildActual = static () =>
                new ApiIdentityBuilder<Customer>("PK_Customer")
                    .AddScalar(c => c.Id)
                    .Build()
        },

        new BuildIdentityTest
        {
            Name = "ApiIdentityBuilder<Customer> AddScalar(expr, typeHint) extracts CLR property name with type hint",
            BuildExpected = static () =>
                new ApiIdentityBuilder("PK_Customer")
                    .AddScalar("Id", typeof(string))
                    .Build(),
            BuildActual = static () =>
                new ApiIdentityBuilder<Customer>("PK_Customer")
                    .AddScalar(c => c.Id, typeof(string))
                    .Build()
        },

        new BuildIdentityTest
        {
            Name = "ApiIdentityBuilder<Customer> AddScalar(expr) with multiple scalar parts",
            BuildExpected = static () =>
                new ApiIdentityBuilder("AK_Customer_Name")
                    .AddScalar("Name")
                    .Build(),
            BuildActual = static () =>
                new ApiIdentityBuilder<Customer>("AK_Customer_Name")
                    .AddScalar(c => c.Name)
                    .Build()
        },

        new BuildIdentityTest
        {
            Name = "ApiIdentityBuilder<Customer> AddNested(expr) extracts CLR property name",
            BuildExpected = static () =>
                new ApiIdentityBuilder("PK_Customer_Address")
                    .AddNested("PrimaryAddress")
                    .Build(),
            BuildActual = static () =>
                new ApiIdentityBuilder<Customer>("PK_Customer_Address")
                    .AddNested(c => c.PrimaryAddress)
                    .Build()
        },

        new BuildIdentityTest
        {
            Name = "ApiIdentityBuilder<Customer> AddNested(expr, identityName) extracts CLR property name with explicit identity name",
            BuildExpected = static () =>
                new ApiIdentityBuilder("PK_Customer_Address_Named")
                    .AddNested("PrimaryAddress", "PK_Address")
                    .Build(),
            BuildActual = static () =>
                new ApiIdentityBuilder<Customer>("PK_Customer_Address_Named")
                    .AddNested(c => c.PrimaryAddress, "PK_Address")
                    .Build()
        },

        new BuildIdentityTest
        {
            Name = "ApiIdentityBuilder<Customer> composite AddScalar(expr) + AddScalar(expr) builds two-part identity",
            BuildExpected = static () =>
                new ApiIdentityBuilder("AK_Customer_Id_Name")
                    .AddScalar("Id")
                    .AddScalar("Name")
                    .Build(),
            BuildActual = static () =>
                new ApiIdentityBuilder<Customer>("AK_Customer_Id_Name")
                    .AddScalar(c => c.Id)
                    .AddScalar(c => c.Name)
                    .Build()
        },

        new BuildIdentityTest
        {
            Name = "ApiIdentityBuilder<Customer> mixed AddNested(expr) + AddScalar(expr) builds composite identity",
            BuildExpected = static () =>
                new ApiIdentityBuilder("AK_Customer_Address_Name")
                    .AddNested("PrimaryAddress")
                    .AddScalar("Name")
                    .Build(),
            BuildActual = static () =>
                new ApiIdentityBuilder<Customer>("AK_Customer_Address_Name")
                    .AddNested(c => c.PrimaryAddress)
                    .AddScalar(c => c.Name)
                    .Build()
        },
    ];
    #endregion

    #region Theory Tests
    [Theory]
    [MemberData(nameof(BuildObjectTypeTheoryData))]
    public void BuildObjectType(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildIdentityTheoryData))]
    public void BuildIdentity(IXUnitTest test) => test.Execute(this);
    #endregion
}
