// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using static Evoogle.ApiFramework.Schema.TestData.ApiTestSchemaUtils;

namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>
///     Produces a compact but expressive ApiSchema suitable for most unit tests.
/// </summary>
public static class TestSchemaFactory
{
    #region Methods
    /// <summary>
    ///     Builds the reusable “Commerce” schema:
    ///     Scalars, Enums, Value Objects, Entities, Relationships, Polymorphism, Recursion, and M2M.
    /// </summary>
    public static ApiSchema BuildCommerceSchema(string name = "Commerce")
    {
        // 1) Scalars
        var scalars = new List<ApiScalarType>
        {
            S("Ulid",           typeof(Ulid)),
            S("Guid",           typeof(Guid)),
            S("String",         typeof(string)),
            S("Int32",          typeof(int)),
            S("Int64",          typeof(long)),
            S("Decimal",        typeof(decimal)),
            S("Double",         typeof(double)),
            S("Boolean",        typeof(bool)),
            S("DateOnly",       typeof(DateOnly)),
            S("TimeOnly",       typeof(TimeOnly)),
            S("DateTimeOffset", typeof(DateTimeOffset)),
            S("Uri",            typeof(Uri)),
            S("Email",          typeof(string)),
            S("Bytes",          typeof(byte[]))
        };

        // 2) Enums
        var enums = new List<ApiEnumType>
        {
            E("OrderStatus",   typeof(OrderStatus),   [EV("Pending", 0), EV("Paid", 1), EV("Shipped", 2), EV("Cancelled", 3), EV("Returned", 4)]),
            E("PaymentMethod", typeof(PaymentMethod), [EV("Card", 0), EV("Cash", 1), EV("Wire", 2), EV("Crypto", 3)]),
            E("UserRole",      typeof(UserRole),      [EV("None", 0), EV("Reader", 1), EV("Editor", 2), EV("Admin", 3)]),
            E("CountryCode",   typeof(CountryCode),   [EV("US", 0), EV("CA", 1), EV("GB", 2)])
        };

        // 3) Value Objects (objects w/o relationships)
        var money = O("Money", typeof(Money),
        [
            P("Amount",    TE.ClrRef<decimal>(),    required: true),
            P("Currency",  TE.ClrRef<string>(),     required: true)
        ]);

        var quantity = O("Quantity", typeof(Quantity),
        [
            P("Value",     TE.ClrRef<decimal>(),    required: true),
            P("Unit",      TE.ClrRef<string>(),     required: true)
        ]);

        var emailAddress = O("EmailAddress", typeof(EmailAddress),
        [
            P("Value",     TE.ClrRef<string>(), required: true)
        ]);

        var address = O("Address", typeof(Address),
        [
            P("Line1",     TE.ClrRef<string>(),         required: true),
            P("Line2",     TE.ClrRef<string>(),         required: false),
            P("City",      TE.ClrRef<string>(),         required: true),
            P("State",     TE.ClrRef<string>(),         required: true),
            P("Postal",    TE.ClrRef<string>(),         required: true),
            P("Country",   TE.ClrRef<CountryCode>(),    required: true)
        ]);

        // 4) Entity/Object Types (with relationships)
        // Category (self-referential)
        var category = O("Category", typeof(Category),
        [
            P("Id",        TE.ClrRef<Ulid>(),                   required: true),
            P("Name",      TE.ClrRef<string>(),                 required: true),
            P("Parent",    TE.ClrRef<Category>(),               required: false),
            P("Children",  TE.ListOf<Category>(required: true), required: true)
        ],
        [
            R("Category_Children", "Children"),
            R("Category_Children", "Parent")
        ]);

        // Tag (M2M with ProductBase)
        var tag = O("Tag", typeof(Tag),
        [
            P("Id",        TE.ClrRef<Ulid>(),                       required: true),
            P("Name",      TE.ClrRef<string>(),                     required: true),
            P("Products",  TE.ListOf<ProductBase>(required:true),   required: true)
        ],
        [
            R("Product_Tags", "Products")
        ]);

        // // Abstract ProductBase + two derived types (polymorphism)
        // var productBase = O("ProductBase", typeof(ProductBase),
        // [
        //     P("Id",        TE.ClrRef<Ulid>(),       required: true),
        //     P("Sku",       TE.ClrRef<string>(),     required: true),
        //     P("Name",      TE.ClrRef<string>(),     required: true),
        //     P("Price",     TE.ClrRef<Money>(),      required: true)
        // ],
        // extensions: Ext(new() { ["discriminator"] = "kind" })); // Example: your framework’s discriminator key

        var physicalProduct = O("PhysicalProduct", typeof(PhysicalProduct),
        [
            P("Id",        TE.ClrRef<Ulid>(),               required: true),
            P("Sku",       TE.ClrRef<string>(),             required: true),
            P("Name",      TE.ClrRef<string>(),             required: true),
            P("Price",     TE.ClrRef<Money>(),              required: true),
            P("Tags",      TE.ListOf<Tag>(required:true),   required: false),
            P("Category",  TE.ClrRef<Category>(),           required: false),
            P("Weight",    TE.ClrRef<decimal>(),            required: true),
            P("Size",      TE.ClrRef<Quantity>(),           required: false),
        ],
        [
            R("Product_Tags", "Tags")
        ]);

        var digitalProduct = O("DigitalProduct", typeof(DigitalProduct),
        [
            P("Id",             TE.ClrRef<Ulid>(),              required: true),
            P("Sku",            TE.ClrRef<string>(),            required: true),
            P("Name",           TE.ClrRef<string>(),            required: true),
            P("Price",          TE.ClrRef<Money>(),             required: true),
            P("Tags",           TE.ListOf<Tag>(required:true),  required: false),
            P("Category",       TE.ClrRef<Category>(),          required: false),
            P("DownloadUrl",    TE.ClrRef<Uri>(),               required: false),
            P("Bytes",          TE.ClrRef<long>(),              required: false),
        ],
        [
            R("Product_Tags", "Tags")
        ]);

        // OrderLine
        var orderLine = O("OrderLine", typeof(OrderLine),
        [
            P("Id",        TE.ClrRef<Ulid>(),           required: true),
            P("Product",   TE.ClrRef<ProductBase>(),    required: true),
            P("Qty",       TE.ClrRef<Quantity>(),       required: true),
            P("UnitPrice", TE.ClrRef<Money>(),          required: true),
            P("LineTotal", TE.ClrRef<Money>(),          required: true)
        ]);

        // Payment
        var payment = O("Payment", typeof(Payment),
        [
            P("Id",         TE.ClrRef<Ulid>(),              required: true),
            P("Method",     TE.ClrRef<PaymentMethod>(),     required: true),
            P("Amount",     TE.ClrRef<Money>(),             required: true),
            P("CapturedAt", TE.ClrRef<DateTimeOffset>(),    required: false),
            // Dictionary<string,string>
            //P("Metadata",  TE.DictOf(TE.ClrRef<string>(), TE.ClrRef<string>(), valueNullable:true), required: true)
        ]);

        // Order
        var order = O("Order", typeof(Order),
        [
            P("Id",        TE.ClrRef<Ulid>(),                   required: true),
            P("Customer",  TE.ClrRef<Customer>(),               required: true),
            P("PlacedAt",  TE.ClrRef<DateTimeOffset>(),         required: true),
            P("Status",    TE.ClrRef<OrderStatus>(),            required: true),
            P("Lines",     TE.ListOf<OrderLine>(required:true), required: true),
            P("Payment",   TE.ClrRef<Payment>(),                required: false),
            P("Total",     TE.ClrRef<Money>(),                  required: true)
        ],
        [
            R("Customer_Orders", "Customer"),
            R("Order_Payment", "Payment")
        ]);

        // Customer (has collections, nested value objects)
        var customer = O("Customer", typeof(Customer),
        [
            P("Id",             TE.ClrRef<Ulid>(),                  required: true),
            P("Name",           TE.ClrRef<string>(),                required: true),
            P("Email",          TE.ClrRef<EmailAddress>(),          required: true),
            P("PrimaryAddress", TE.ClrRef<Address>(),               required: true),
            P("Addresses",      TE.ListOf<Address>(required:true),  required: true),
            P("Orders",         TE.ListOf<Order>(required:true),    required: true)
        ],
        [
            R("Customer_Orders", "Orders")
        ]);

        // 5) Relationships (optional if you encode via properties; include here when you want explicit cardinality tests)
        var relationships = new List<ApiRelationship>
        {
            // Customer 1–many Orders
            R("Customer_Orders", "Orders"),

            // Order optional 1–1 Payment
            R("Order_Payment", "Payment"),

            // ProductBase many–many Tag
            R("Product_Tags", "Tags"),

            // Category self-reference
            R("Category_Children", "Children"),
        };

        // 6) Objects list (include value objects + entities + abstract base + derived)
        var objects = new List<ApiObjectType>
        {
            money, quantity, emailAddress, address,
            category, tag,
            physicalProduct, digitalProduct,
            orderLine, payment, order, customer
        };

        // 7) Assemble schema
        var schema = new ApiSchema(
            apiName: name,
            apiScalarTypes: scalars,
            apiEnumTypes: enums,
            apiObjectTypes: objects
        );

        var result = schema.Initialize();
        result.ThrowIfInvalid();

        return schema;
    }
    #endregion
}
