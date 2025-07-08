// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public static class Dummy
{
    public record struct MailAddress(string Address)
    {
        public static implicit operator MailAddress(string address) => new(address);
        public static implicit operator string(MailAddress mailAddress) => mailAddress.Address;
    }

    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new();
    }

    public class Order
    {
        public Guid OrderId { get; set; }
        public decimal Total { get; set; }
    }

    public static void DummyMethod()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("CustomerOrdersAPI")
            .WithVersion("v1")
            .AddScalar("Email", typeof(MailAddress))
            .AddObject("Customer", customer => customer
                .WithClrType(typeof(Customer))
                .AddProperty("email", "Email", typeof(MailAddress), m => m.Required())
                .AddRelationship("orders"))
            .AddObject("Order", order => order
                .WithClrType(typeof(Order))
                .AddProperty("id", "Id", typeof(Guid))
                .AddProperty("total", "Total", typeof(decimal)))
            .Build();
    }
}
