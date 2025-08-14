// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiSchemaBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public record struct EmailAddress(string Value);

    public enum OrderStatus
    {
        Pending,
        Shipped
    }

    public class Order
    {
        public EmailAddress Email { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class EmailAddressConfiguration : IApiScalarTypeConfiguration
    {
        public void Configure(ApiScalarTypeBuilder builder) => builder.WithName("Email");
    }

    public class OrderStatusConfiguration : IApiEnumTypeConfiguration
    {
        public void Configure(ApiEnumTypeBuilder builder)
        {
            builder
                .WithName("OrderStatus")
                .AddValue("Pending", nameof(OrderStatus.Pending), 0)
                .AddValue("Shipped", nameof(OrderStatus.Shipped), 1);
        }
    }

    public class OrderConfiguration : IApiObjectTypeConfiguration
    {
        public void Configure(ApiObjectTypeBuilder builder)
        {
            builder
                .WithName("Order")
                .AddProperty("email", nameof(Order.Email))
                .AddProperty("status", nameof(Order.Status));
        }
    }
    #endregion

    public class BuildTest : XUnitTest
    {
        private ApiSchema? ApiSchema { get; set; }

        protected override void Act()
        {
            var builder = new ApiSchemaBuilder()
                .WithName("TestSchema")
                .WithVersion("v1")
                .AddScalar(typeof(EmailAddress), new EmailAddressConfiguration())
                .AddEnum(typeof(OrderStatus), new OrderStatusConfiguration())
                .AddObject(typeof(Order), new OrderConfiguration());
            this.ApiSchema = builder.Build();
        }

        protected override void Assert()
        {
            this.ApiSchema.Should().NotBeNull();
            this.ApiSchema!.ApiName.Should().Be("TestSchema");
            this.ApiSchema.ApiVersion.Should().Be("v1");
            this.ApiSchema.TryGetApiScalarType(typeof(EmailAddress), out var scalar).Should().BeTrue();
            scalar!.ApiName.Should().Be("Email");
            this.ApiSchema.TryGetApiEnumType(typeof(OrderStatus), out var enumType).Should().BeTrue();
            enumType!.ApiEnumValues.Should().HaveCount(2);
            this.ApiSchema.TryGetApiObjectType(typeof(Order), out var objectType).Should().BeTrue();
            objectType!.ApiProperties.Should().HaveCount(2);
        }
    }

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = "Builds schema with configured scalar, enum and object types"
        }
    ];

    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
}

