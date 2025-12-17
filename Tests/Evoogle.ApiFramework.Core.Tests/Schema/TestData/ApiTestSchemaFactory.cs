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
public static class ApiTestSchemaFactory
{
    #region Fields
    private static readonly Lazy<ApiSchema> _commerceSchema = new(() => BuildCommerceSchema());
    private static readonly Lazy<ApiSchema> _simpleSchema = new(() => BuildSimpleSchema());
    #endregion

    #region Properties
    public static ApiSchema CommerceSchema => _commerceSchema.Value;
    public static ApiSchema SimpleSchema => _simpleSchema.Value;
    #endregion

    #region Methods
    public static ApiSchema BuildTestSchema(ApiTestSchemaKind kind)
    {
        return kind switch
        {
            ApiTestSchemaKind.Simple => SimpleSchema,
            ApiTestSchemaKind.Commerce => CommerceSchema,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }

    /// <summary>
    ///     Builds the reusable “Commerce” schema:
    ///     Scalars, Enums, Value Objects, Entities, Relationships, Polymorphism, Recursion, and M2M.
    /// </summary>
    public static ApiSchema BuildCommerceSchema(string name = "Commerce")
    {
        // 1) Scalars
        var scalars = new List<ApiScalarType>
        {
            S(name: nameof(Ulid),           clr: typeof(Ulid)),
            S(name: nameof(Guid),           clr: typeof(Guid)),
            S(name: nameof(String),         clr: typeof(string)),
            S(name: nameof(Int32),          clr: typeof(int)),
            S(name: nameof(Int64),          clr: typeof(long)),
            S(name: nameof(Decimal),        clr: typeof(decimal)),
            S(name: nameof(Double),         clr: typeof(double)),
            S(name: nameof(Boolean),        clr: typeof(bool)),
            S(name: nameof(DateOnly),       clr: typeof(DateOnly)),
            S(name: nameof(TimeOnly),       clr: typeof(TimeOnly)),
            S(name: nameof(DateTimeOffset), clr: typeof(DateTimeOffset)),
            S(name: nameof(Uri),            clr: typeof(Uri))
        };

        // 2) Enums
        var enums = new List<ApiEnumType>
        {
            E(name: nameof(OrderStatus), clr: typeof(OrderStatus), values:
            [
                EV(name: nameof(OrderStatus.Pending),   ordinal: 0),
                EV(name: nameof(OrderStatus.Paid),      ordinal: 1),
                EV(name: nameof(OrderStatus.Shipped),   ordinal: 2),
                EV(name: nameof(OrderStatus.Cancelled), ordinal: 3),
                EV(name: nameof(OrderStatus.Returned),  ordinal: 4)
            ]),

            E(name: nameof(PaymentMethod), clr: typeof(PaymentMethod), values:
            [
                EV(name: nameof(PaymentMethod.Card),   ordinal: 0),
                EV(name: nameof(PaymentMethod.Cash),   ordinal: 1),
                EV(name: nameof(PaymentMethod.Wire),   ordinal: 2),
                EV(name: nameof(PaymentMethod.Crypto), ordinal: 3)
            ]),

            E(name: nameof(UserRole), clr: typeof(UserRole), values:
            [
                EV(name: nameof(UserRole.None),   ordinal: 0),
                EV(name: nameof(UserRole.Reader), ordinal: 1),
                EV(name: nameof(UserRole.Editor), ordinal: 2),
                EV(name: nameof(UserRole.Admin),  ordinal: 3)
            ]),

            E(name: nameof(CountryCode), clr: typeof(CountryCode), values:
            [
                EV(name: nameof(CountryCode.US), ordinal: 0),
                EV(name: nameof(CountryCode.CA), ordinal: 1),
                EV(name: nameof(CountryCode.GB), ordinal: 2)
            ])
        };

        // 3) Value Objects (objects w/o relationships)
        var money = O(name: nameof(Money), clr: typeof(Money), properties:
        [
            P(name: nameof(Money.Amount),   expression: TE.ClrRef<decimal>(), required: true),
            P(name: nameof(Money.Currency), expression: TE.ClrRef<string>(),  required: true)
        ]);

        var quantity = O(name: nameof(Quantity), clr: typeof(Quantity), properties:
        [
            P(name: nameof(Quantity.Value), expression: TE.ClrRef<decimal>(), required: true),
            P(name: nameof(Quantity.Unit),  expression: TE.ClrRef<string>(),  required: true)
        ]);

        var emailAddress = O(name: nameof(EmailAddress), clr: typeof(EmailAddress), properties:
        [
            P(name: nameof(EmailAddress.Value), expression: TE.ClrRef<string>(), required: true)
        ]);

        var address = O(name: nameof(Address), clr: typeof(Address), properties:
        [
            P(name: nameof(Address.Line1),   expression: TE.ClrRef<string>(),      required: true),
            P(name: nameof(Address.Line2),   expression: TE.ClrRef<string>(),      required: false),
            P(name: nameof(Address.City),    expression: TE.ClrRef<string>(),      required: true),
            P(name: nameof(Address.State),   expression: TE.ClrRef<string>(),      required: true),
            P(name: nameof(Address.Postal),  expression: TE.ClrRef<string>(),      required: true),
            P(name: nameof(Address.Country), expression: TE.ClrRef<CountryCode>(), required: true)
        ]);

        // 4) Entity/Object Types (with relationships)
        // Category (self-referential)
        var category = O(name: nameof(Category), clr: typeof(Category), properties:
        [
            P(name: nameof(Category.Id),        expression: TE.ClrRef<Ulid>(),                   required: true),
            P(name: nameof(Category.Name),      expression: TE.ClrRef<string>(),                 required: true),
            P(name: nameof(Category.Parent),    expression: TE.ClrRef<Category>(),               required: false),
            P(name: nameof(Category.Children),  expression: TE.ListOf<Category>(required: true), required: true)
        ], relationships:
        [
            R(name: "Category_Children", propertyName: nameof(Category.Children)),
            R(name: "Category_Children", propertyName: nameof(Category.Parent))
        ]);

        // Tag (M2M with ProductBase)
        var tag = O(name: nameof(Tag), clr: typeof(Tag), properties:
        [
            P(name: nameof(Tag.Id),        expression: TE.ClrRef<Ulid>(),                      required: true),
            P(name: nameof(Tag.Name),      expression: TE.ClrRef<string>(),                    required: true),
            P(name: nameof(Tag.Products),  expression: TE.ListOf<ProductBase>(required: true), required: true)
        ], relationships:
        [
            R(name: "Product_Tags", propertyName: "Products")
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

        var physicalProduct = O(name: nameof(PhysicalProduct), clr: typeof(PhysicalProduct), properties:
        [
            P(name: nameof(PhysicalProduct.Id),        expression: TE.ClrRef<Ulid>(),              required: true),
            P(name: nameof(PhysicalProduct.Sku),       expression: TE.ClrRef<string>(),            required: true),
            P(name: nameof(PhysicalProduct.Name),      expression: TE.ClrRef<string>(),            required: true),
            P(name: nameof(PhysicalProduct.Price),     expression: TE.ClrRef<Money>(),             required: true),
            P(name: nameof(PhysicalProduct.Tags),      expression: TE.ListOf<Tag>(required: true), required: false),
            P(name: nameof(PhysicalProduct.Category),  expression: TE.ClrRef<Category>(),          required: false),
            P(name: nameof(PhysicalProduct.Weight),    expression: TE.ClrRef<decimal>(),           required: true),
            P(name: nameof(PhysicalProduct.Size),      expression: TE.ClrRef<Quantity>(),          required: false),
        ], relationships:
        [
            R(name: "Product_Tags", propertyName: nameof(PhysicalProduct.Tags))
        ]);

        var digitalProduct = O(name: nameof(DigitalProduct), clr: typeof(DigitalProduct), properties:
        [
            P(name: nameof(DigitalProduct.Id),             expression: TE.ClrRef<Ulid>(),              required: true),
            P(name: nameof(DigitalProduct.Sku),            expression: TE.ClrRef<string>(),            required: true),
            P(name: nameof(DigitalProduct.Name),           expression: TE.ClrRef<string>(),            required: true),
            P(name: nameof(DigitalProduct.Price),          expression: TE.ClrRef<Money>(),             required: true),
            P(name: nameof(DigitalProduct.Tags),           expression: TE.ListOf<Tag>(required: true), required: false),
            P(name: nameof(DigitalProduct.Category),       expression: TE.ClrRef<Category>(),          required: false),
            P(name: nameof(DigitalProduct.DownloadUrl),    expression: TE.ClrRef<Uri>(),               required: false),
            P(name: nameof(DigitalProduct.Bytes),          expression: TE.ClrRef<long>(),              required: false),
        ], relationships:
        [
            R(name: "Product_Tags", propertyName: nameof(DigitalProduct.Tags))
        ]);

        // OrderLine
        var orderLine = O(name: nameof(OrderLine), clr: typeof(OrderLine), properties:
        [
            P(name: nameof(OrderLine.Id),        expression: TE.ClrRef<Ulid>(),        required: true),
            P(name: nameof(OrderLine.Product),   expression: TE.ClrRef<ProductBase>(), required: true),
            P(name: nameof(OrderLine.Qty),       expression: TE.ClrRef<Quantity>(),    required: true),
            P(name: nameof(OrderLine.UnitPrice), expression: TE.ClrRef<Money>(),       required: true),
            P(name: nameof(OrderLine.LineTotal), expression: TE.ClrRef<Money>(),       required: true)
        ]);

        // Payment
        var payment = O(name: nameof(Payment), clr: typeof(Payment), properties:
        [
            P(name: nameof(Payment.Id),         expression: TE.ClrRef<Ulid>(),           required: true),
            P(name: nameof(Payment.Method),     expression: TE.ClrRef<PaymentMethod>(),  required: true),
            P(name: nameof(Payment.Amount),     expression: TE.ClrRef<Money>(),          required: true),
            P(name: nameof(Payment.CapturedAt), expression: TE.ClrRef<DateTimeOffset>(), required: false),
            // Dictionary<string,string>
            //P(name: "Metadata",  expression: TE.DictOf(TE.ClrRef<string>(), TE.ClrRef<string>(), valueNullable: true), required: true)
        ]);

        // Order
        var order = O(name: nameof(Order), clr: typeof(Order), properties:
        [
            P(name: nameof(Order.Id),        expression: TE.ClrRef<Ulid>(),                    required: true),
            P(name: nameof(Order.Customer),  expression: TE.ClrRef<Customer>(),                required: true),
            P(name: nameof(Order.PlacedAt),  expression: TE.ClrRef<DateTimeOffset>(),          required: true),
            P(name: nameof(Order.Status),    expression: TE.ClrRef<OrderStatus>(),             required: true),
            P(name: nameof(Order.Lines),     expression: TE.ListOf<OrderLine>(required: true), required: true),
            P(name: nameof(Order.Payment),   expression: TE.ClrRef<Payment>(),                 required: false),
            P(name: nameof(Order.Total),     expression: TE.ClrRef<Money>(),                   required: true)
        ], relationships:
        [
            R(name: "Customer_Orders", propertyName: nameof(Order.Customer)),
            R(name: "Order_Payment", propertyName: nameof(Order.Payment))
        ]);

        // Customer (has collections, nested value objects)
        var customer = O(name: nameof(Customer), clr: typeof(Customer), properties:
        [
            P(name: nameof(Customer.Id),             expression: TE.ClrRef<Ulid>(),                  required: true),
            P(name: nameof(Customer.Name),           expression: TE.ClrRef<string>(),                required: true),
            P(name: nameof(Customer.Email),          expression: TE.ClrRef<EmailAddress>(),          required: true),
            P(name: nameof(Customer.PrimaryAddress), expression: TE.ClrRef<Address>(),               required: true),
            P(name: nameof(Customer.Addresses),      expression: TE.ListOf<Address>(required: true), required: true),
            P(name: nameof(Customer.Orders),         expression: TE.ListOf<Order>(required: true),   required: true)
        ], relationships:
        [
            R(name: "Customer_Orders", propertyName: nameof(Customer.Orders))
        ]);

        // 5) Relationships (optional if you encode via properties; include here when you want explicit cardinality tests)
        var relationships = new List<ApiRelationship>
        {
            // Customer 1–many Orders
            R(name: "Customer_Orders", propertyName: nameof(Customer.Orders)),

            // Order optional 1–1 Payment
            R(name: "Order_Payment", propertyName: nameof(Order.Payment)),

            // ProductBase many–many Tag
            R(name: "Product_Tags", propertyName: nameof(ProductBase.Tags)),

            // Category self-reference
            R(name: "Category_Children", propertyName: nameof(Category.Children)),
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

    /// <summary>
    ///    Builds the reusable “Simple” schema: Scalars, Enums, and Object Types.
    /// </summary>
    public static ApiSchema BuildSimpleSchema(string name = "Simple")
    {
        // 1) Scalars
        var scalars = new List<ApiScalarType>
        {
            S(name: nameof(String),  clr: typeof(string)),
            S(name: nameof(Int32),   clr: typeof(int)),
            S(name: nameof(Int64),   clr: typeof(long)),
            S(name: nameof(Boolean), clr: typeof(bool))
        };

        // 2) Enums
        var enums = new List<ApiEnumType>
        {
            E(name: nameof(Gender), clr: typeof(Gender), values:
            [
                EV(name: nameof(Gender.Unspecified), ordinal: 0),
                EV(name: nameof(Gender.Male),        ordinal: 1),
                EV(name: nameof(Gender.Female),      ordinal: 2)
            ]),

            E(name: nameof(StopLight), clr: typeof(StopLight), values:
            [
                EV(name: nameof(StopLight.None),   ordinal: 0),
                EV(name: nameof(StopLight.Green),  ordinal: 1),
                EV(name: nameof(StopLight.Yellow), ordinal: 2),
                EV(name: nameof(StopLight.Red),    ordinal: 3)
            ])
        };

        // 3) Object Types
        var empty = O(name: nameof(Empty), clr: typeof(Empty), options: OO(ApiIdentityNullHandling.ThrowException), properties: []);

        var point = O(name: nameof(Point), clr: typeof(Point), options: OO(ApiIdentityNullHandling.ThrowException), properties:
        [
            P(name: nameof(Point.X),    expression: TE.ClrRef<long>(),   required: true),
            P(name: nameof(Point.Y),    expression: TE.ClrRef<long>(),   required: true),
            P(name: nameof(Point.Note), expression: TE.ClrRef<string>(), required: false)
        ]);

        var scalarsOnly = O(name: nameof(ScalarsOnly), clr: typeof(ScalarsOnly), options: OO(ApiIdentityNullHandling.ThrowException), properties:
        [
            P(name: nameof(ScalarsOnly.RequiredName),      expression: TE.ClrRef<string>(), required: true),
            P(name: nameof(ScalarsOnly.RequiredNumber),    expression: TE.ClrRef<long>(),   required: true),
            P(name: nameof(ScalarsOnly.RequiredPredicate), expression: TE.ClrRef<bool>(),   required: true),
            P(name: nameof(ScalarsOnly.OptionalName),      expression: TE.ClrRef<string>(), required: false),
            P(name: nameof(ScalarsOnly.OptionalNumber),    expression: TE.ClrRef<long>(),   required: false),
            P(name: nameof(ScalarsOnly.OptionalPredicate), expression: TE.ClrRef<bool>(),   required: false)
        ]);

        var person = O(name: nameof(Person), clr: typeof(Person), options: OO(ApiIdentityNullHandling.ThrowException), properties:
        [
            P(name: nameof(Person.Name),    expression: TE.ClrRef<string>(),               required: true),
            P(name: nameof(Person.Age),     expression: TE.ClrRef<int>(),                  required: false),
            P(name: nameof(Person.Gender),  expression: TE.ClrRef<Gender>(),               required: false),
            P(name: nameof(Person.Hobbies), expression: TE.ListOf<string>(required: true), required: false)
        ]);
        var company = O(name: nameof(Company), clr: typeof(Company), options: OO(ApiIdentityNullHandling.ThrowException), properties:
        [
            P(name: nameof(Company.Name),      expression: TE.ClrRef<string>(),               required: true),
            P(name: nameof(Company.Owner),     expression: TE.ClrRef<Person>(),               required: false),
            P(name: nameof(Company.Employees), expression: TE.ListOf<Person>(required: true), required: false)
        ], relationships:
        [
            R(name: "Company_Owner",      propertyName: nameof(Company.Owner)),
            R(name: "Company_Employees",  propertyName: nameof(Company.Employees))
        ]);

        // 4) Objects list
        var objects = new List<ApiObjectType>
        {
            empty,
            point,
            scalarsOnly,
            person,
            company
        };

        // 5) Assemble schema
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
