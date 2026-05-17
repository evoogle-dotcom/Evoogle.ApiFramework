// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.TestData;

namespace Evoogle.ApiFramework.Schema.TestData;

public static partial class ApiSchemaFactory
{
    #region Built-In Schema Cache
    private static readonly Lazy<ApiSchema> _commerceApiSchema = new(() => BuildCommerceApiSchema(nameof(ApiSchemaKind.Commerce)));
    private static readonly Lazy<ApiSchema> _identityApiSchema = new(() => BuildIdentityApiSchema(nameof(ApiSchemaKind.Identity)));
    private static readonly Lazy<ApiSchema> _relationshipApiSchema = new(() => BuildRelationshipApiSchema(nameof(ApiSchemaKind.Relationship)));
    private static readonly Lazy<ApiSchema> _simpleApiSchema = new(() => BuildSimpleApiSchema(nameof(ApiSchemaKind.Simple)));
    #endregion

    #region Built-In Schema Accessors
    public static ApiSchema CommerceApiSchema => _commerceApiSchema.Value;
    public static ApiSchema IdentityApiSchema => _identityApiSchema.Value;
    public static ApiSchema SimpleApiSchema => _simpleApiSchema.Value;
    public static ApiSchema RelationshipApiSchema => _relationshipApiSchema.Value;
    #endregion

    #region Built-In Schema Methods
    public static ApiSchema BuildTestApiSchema(ApiSchemaKind kind)
    {
        return kind switch
        {
            ApiSchemaKind.Simple => SimpleApiSchema,
            ApiSchemaKind.Commerce => CommerceApiSchema,
            ApiSchemaKind.Identity => IdentityApiSchema,
            ApiSchemaKind.Relationship => RelationshipApiSchema,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }
    #endregion

    #region Built-In Schema Presets
    private static ApiScalarType S(string name, Type clr, OrderedDictionary<Type, object>? extensions = null)
        => new(name, clr)
        {
            Extensions = extensions
        };

    private static ApiEnumType E(string name, Type clr, IEnumerable<ApiEnumValue> values, OrderedDictionary<Type, object>? extensions = null)
        => new(name, values, clr)
        {
            Extensions = extensions
        };

    private static ApiEnumValue EV(string name, int ordinal, OrderedDictionary<Type, object>? extensions = null)
        => new(name, name, ordinal)
        {
            Extensions = extensions
        };

    private static ApiIdentity I(string name, IEnumerable<ApiIdentityPart> parts)
        => new(name, parts);

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private static ApiIdentityPart IPS(string propertyName, Type? type = null)
        => new ApiIdentityScalarPart(propertyName, type);

    private static ApiIdentityPart IPN(string propertyName, string? identityName = null)
        => new ApiIdentityNestedPart(propertyName, identityName);

    private static ApiIdentityPart IPO(string? identityName = null)
        => new ApiIdentityOwnerPart(identityName);
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

    private static ApiObjectType O(string name, Type clr, IEnumerable<ApiProperty> properties, IEnumerable<ApiIdentity>? identities = null, ApiObjectTypeOptions? options = null, OrderedDictionary<Type, object>? extensions = null)
        => new(name, options, identities, properties, clr)
        {
            Extensions = extensions
        };

    private static ApiObjectTypeOptions OO(ApiIdentityPartNullHandling identityPartNullHandling)
        => new()
        {
            ApiIdentityPartNullHandling = identityPartNullHandling
        };

    private static ApiProperty P(string name, ApiTypeExpression expression, bool required, ClrMemberKind clrMemberKind = ClrMemberKind.Property, OrderedDictionary<Type, object>? extensions = null)
        => new(name, expression, required ? ApiTypeModifiers.Required : ApiTypeModifiers.None, name, clrMemberKind)
        {
            Extensions = extensions
        };

    // private static IDictionary<string, object?> Ext(IDictionary<string, object?> dict) => dict;

    private static class TE
    {
        public static ApiTypeExpression ClrRef<T>() => ApiTypeExpression.ClrRef<T>();

        public static ApiTypeExpression HashSetOf<T>(bool required) => ApiTypeExpression.HashSetOf<T>(required ? ApiTypeModifiers.Required : ApiTypeModifiers.None);

        public static ApiTypeExpression ListOf<T>(bool required) => ApiTypeExpression.ListOf<T>(required ? ApiTypeModifiers.Required : ApiTypeModifiers.None);
    }

    // Relationship Presets

    private static ApiRelationshipOneToMany R1M(string name, ApiRelationshipPrincipalEnd principal, ApiRelationshipDependentEnd dependent, ApiRelationshipDeleteBehavior deleteBehavior = ApiRelationshipOneToMany.DefaultDeleteBehavior)
        => new(name, principal, dependent, deleteBehavior);

    private static ApiRelationshipOneToOne R11(string name, ApiRelationshipPrincipalEnd principal, ApiRelationshipDependentEnd dependent, ApiRelationshipDeleteBehavior deleteBehavior = ApiRelationshipOneToOne.DefaultDeleteBehavior)
        => new(name, principal, dependent, deleteBehavior);

    private static ApiRelationshipManyToMany RMN(string name, ApiRelationshipPrincipalEnd principalA, ApiRelationshipPrincipalEnd principalB, ApiRelationshipAssociation association, ApiRelationshipDeleteBehavior deleteBehavior = ApiRelationshipManyToMany.DefaultDeleteBehavior)
        => new(name, principalA, principalB, association, deleteBehavior);

    private static ApiRelationshipPrincipalEnd RPE(Type clr, string? identityName = null)
        => new(clr, identityName);

    private static ApiRelationshipDependentEnd RDE(Type clr, IEnumerable<ApiRelationshipKeyPath> keyPaths)
        => new(clr, keyPaths);

    private static ApiRelationshipAssociation RAS(Type clr, IEnumerable<ApiRelationshipKeyPath> keyPathsA, IEnumerable<ApiRelationshipKeyPath> keyPathsB)
        => new(clr, keyPathsA, keyPathsB);

    private static ApiRelationshipKeyPath RKS(string propertyName)
        => new ApiRelationshipScalarKeyPath(propertyName);

    private static ApiRelationshipKeyPath RKN(string propertyName, IEnumerable<ApiRelationshipKeyPath> keyPaths)
        => new ApiRelationshipNestedKeyPath(propertyName, keyPaths);

    private static ApiRelationshipKeyPath RKO()
        => new ApiRelationshipOwnerKeyPath();

    private static ApiRelationshipKeyPath RKO(IEnumerable<ApiRelationshipKeyPath> keyPaths)
        => new ApiRelationshipOwnerKeyPath(keyPaths);

    /// <summary>
    ///     Builds the reusable "Commerce" API schema:
    ///     Scalars, Enums, Value Objects, Entities, Relationships, Polymorphism, Recursion, and M2M.
    /// </summary>
    private static ApiSchema BuildCommerceApiSchema(string name)
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
                EV(name: nameof(UserRole.Admin),  ordinal: 4)
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
        ], identities:
        [
            I("PK_EmailAddress", [IPS(nameof(EmailAddress.Value))])
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

        // Customer Object Types

        // Customer (has collections, nested value objects)
        var customer = O(name: nameof(Customer), clr: typeof(Customer), properties:
        [
            P(name: nameof(Customer.Id),             expression: TE.ClrRef<Ulid>(),                  required: true),
            P(name: nameof(Customer.Name),           expression: TE.ClrRef<string>(),                required: true),
            P(name: nameof(Customer.Email),          expression: TE.ClrRef<EmailAddress>(),          required: true),
            P(name: nameof(Customer.PrimaryAddress), expression: TE.ClrRef<Address>(),               required: true),
            P(name: nameof(Customer.Addresses),      expression: TE.ListOf<Address>(required: true), required: true),
            P(name: nameof(Customer.Orders),         expression: TE.ListOf<Order>(required: true),   required: true)
        ], identities:
        [
            I("PK_Customer", [IPS(nameof(Customer.Id))]),
            I("AK_Customer_Email", [IPN(nameof(Customer.Email))])
        ]);

        // Product Object Types

        // Category (self-referential)
        var category = O(name: nameof(Category), clr: typeof(Category), properties:
        [
            P(name: nameof(Category.Id),       expression: TE.ClrRef<Ulid>(),                   required: true),
            P(name: nameof(Category.Name),     expression: TE.ClrRef<string>(),                 required: true),
            P(name: nameof(Category.ParentId), expression: TE.ClrRef<Ulid>(),                   required: false),
            P(name: nameof(Category.Parent),   expression: TE.ClrRef<Category>(),               required: false),
            P(name: nameof(Category.Children), expression: TE.ListOf<Category>(required: true), required: true)
        ], identities:
        [
            I("PK_Category", [IPS(nameof(Category.Id))]),
            I("AK_Category_Name", [IPS(nameof(Category.Name))])
        ]);

        // Tag (M2M with ProductBase)
        var tag = O(name: nameof(Tag), clr: typeof(Tag), properties:
        [
            P(name: nameof(Tag.Id),        expression: TE.ClrRef<Ulid>(),                      required: true),
            P(name: nameof(Tag.Name),      expression: TE.ClrRef<string>(),                    required: true),
            // P(name: nameof(Tag.Products),  expression: TE.ListOf<ProductBase>(required: true), required: true)
        ], identities:
        [
            I("PK_Tag", [IPS(nameof(Tag.Id))]),
            I("AK_Tag_Name", [IPS(nameof(Tag.Name))])
        ]);

        // // Abstract ProductBase + two derived types (polymorphism)
        // var productBase = O("ProductBase", typeof(ProductBase),
        // [
        //     P("Id",        TE.ClrRef<Ulid>(),       required: true),
        //     P("Sku",       TE.ClrRef<string>(),     required: true),
        //     P("Name",      TE.ClrRef<string>(),     required: true),
        //     P("Price",     TE.ClrRef<Money>(),      required: true)
        // ],
        // extensions: Ext(new() { ["discriminator"] = "kind" })); // Example: your framework's discriminator key

        var digitalProduct = O(name: nameof(DigitalProduct), clr: typeof(DigitalProduct), properties:
        [
            // ProductBase properties
            P(name: nameof(ProductBase.Id),     expression: TE.ClrRef<Ulid>(),      required: true),
            P(name: nameof(ProductBase.Sku),    expression: TE.ClrRef<string>(),    required: true),
            P(name: nameof(ProductBase.Name),   expression: TE.ClrRef<string>(),    required: true),
            P(name: nameof(ProductBase.Price),  expression: TE.ClrRef<Money>(),     required: true),

            // DigitalProduct properties
            P(name: nameof(DigitalProduct.Tags),        expression: TE.ListOf<Tag>(required: true), required: false),
            P(name: nameof(DigitalProduct.Category),    expression: TE.ClrRef<Category>(),          required: false),
            P(name: nameof(DigitalProduct.CategoryId),  expression: TE.ClrRef<Ulid>(),              required: false),
            P(name: nameof(DigitalProduct.DownloadUrl), expression: TE.ClrRef<Uri>(),               required: false),
            P(name: nameof(DigitalProduct.Bytes),       expression: TE.ClrRef<long>(),              required: false),
        ], identities:
        [
            I("PK_DigitalProduct", [IPS(nameof(DigitalProduct.Id))]),
            I("AK_DigitalProduct_Sku", [IPS(nameof(DigitalProduct.Sku))])
        ]);

        var physicalProduct = O(name: nameof(PhysicalProduct), clr: typeof(PhysicalProduct), properties:
        [
            // ProductBase properties
            P(name: nameof(ProductBase.Id),     expression: TE.ClrRef<Ulid>(),      required: true),
            P(name: nameof(ProductBase.Sku),    expression: TE.ClrRef<string>(),    required: true),
            P(name: nameof(ProductBase.Name),   expression: TE.ClrRef<string>(),    required: true),
            P(name: nameof(ProductBase.Price),  expression: TE.ClrRef<Money>(),     required: true),

            // PhysicalProduct properties
            P(name: nameof(PhysicalProduct.Tags),       expression: TE.ListOf<Tag>(required: true), required: false),
            P(name: nameof(PhysicalProduct.Category),   expression: TE.ClrRef<Category>(),          required: false),
            P(name: nameof(PhysicalProduct.CategoryId), expression: TE.ClrRef<Ulid>(),              required: false),
            P(name: nameof(PhysicalProduct.Weight),     expression: TE.ClrRef<decimal>(),           required: true),
            P(name: nameof(PhysicalProduct.Size),       expression: TE.ClrRef<Quantity>(),          required: false),
        ], identities:
        [
            I("PK_PhysicalProduct", [IPS(nameof(PhysicalProduct.Id))]),
            I("AK_PhysicalProduct_Sku", [IPS(nameof(PhysicalProduct.Sku))])
        ]);

        // Order/Payment Object Types

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
        ], identities:
        [
            I("PK_Order", [IPS(nameof(Order.Id))])
        ]);

        // OrderLine
        var orderLine = O(name: nameof(OrderLine), clr: typeof(OrderLine), properties:
        [
            P(name: nameof(OrderLine.OrderId),      expression: TE.ClrRef<Ulid>(),        required: true),
            P(name: nameof(OrderLine.LineNumber),   expression: TE.ClrRef<int>(),         required: true),
            // P(name: nameof(OrderLine.Product),      expression: TE.ClrRef<ProductBase>(), required: true),
            P(name: nameof(OrderLine.Qty),          expression: TE.ClrRef<Quantity>(),    required: true),
            P(name: nameof(OrderLine.UnitPrice),    expression: TE.ClrRef<Money>(),       required: true),
            P(name: nameof(OrderLine.LineTotal),    expression: TE.ClrRef<Money>(),       required: true)
        ], identities:
        [
            I("PK_OrderLine", [IPO(), IPS(nameof(OrderLine.LineNumber))])
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
        ], identities:
        [
            I("PK_Payment", [IPS(nameof(Payment.Id))])
        ]);

        // Product Join Types (M:M association tables)
        var digitalProductTag = O(name: nameof(DigitalProductTag), clr: typeof(DigitalProductTag), properties:
        [
            P(name: nameof(DigitalProductTag.DigitalProductId), expression: TE.ClrRef<Ulid>(), required: true),
            P(name: nameof(DigitalProductTag.TagId),            expression: TE.ClrRef<Ulid>(), required: true)
        ], identities:
        [
            I("PK_DigitalProductTag", [IPS(nameof(DigitalProductTag.DigitalProductId)), IPS(nameof(DigitalProductTag.TagId))])
        ]);

        var physicalProductTag = O(name: nameof(PhysicalProductTag), clr: typeof(PhysicalProductTag), properties:
        [
            P(name: nameof(PhysicalProductTag.PhysicalProductId), expression: TE.ClrRef<Ulid>(), required: true),
            P(name: nameof(PhysicalProductTag.TagId),             expression: TE.ClrRef<Ulid>(), required: true)
        ], identities:
        [
            I("PK_PhysicalProductTag", [IPS(nameof(PhysicalProductTag.PhysicalProductId)), IPS(nameof(PhysicalProductTag.TagId))])
        ]);

        // 5) Objects list (include value objects + entities + abstract base + derived)
        var objects = new List<ApiObjectType>
        {
            money, quantity, emailAddress, address,
            customer,
            category, physicalProduct, digitalProduct, tag,
            order, orderLine, payment,
            digitalProductTag, physicalProductTag
        };

        // 6) Relationships

        // Customer → Order (1:M, nested FK via Order.Customer.Id)
        var customerToOrder = R1M("REL_Customer_Order_1toN",
            RPE(typeof(Customer)),
            RDE(typeof(Order), [RKN(nameof(Order.Customer), [RKS(nameof(Customer.Id))])]));

        // Order → OrderLine (1:M, owner key path)
        var orderToOrderLine = R1M("REL_Order_OrderLine_1toN",
            RPE(typeof(Order)),
            RDE(typeof(OrderLine), [RKO()]),
            ApiRelationshipDeleteBehavior.Delete);

        // Payment → Order (1:1, nested FK via Order.Payment.Id)
        var paymentToOrder = R11("REL_Payment_Order_1to1",
            RPE(typeof(Payment)),
            RDE(typeof(Order), [RKN(nameof(Order.Payment), [RKS(nameof(Payment.Id))])]));

        // Category → Category (1:M self-referential, scalar FK via Category.ParentId)
        var categoryToCategory = R1M("REL_Category_Category_1toN",
            RPE(typeof(Category)),
            RDE(typeof(Category), [RKS(nameof(Category.ParentId))]));

        // Category → DigitalProduct (1:M, scalar FK via DigitalProduct.CategoryId)
        var categoryToDigitalProduct = R1M("REL_Category_DigitalProduct_1toN",
            RPE(typeof(Category)),
            RDE(typeof(DigitalProduct), [RKS(nameof(DigitalProduct.CategoryId))]));

        // Category → PhysicalProduct (1:M, scalar FK via PhysicalProduct.CategoryId)
        var categoryToPhysicalProduct = R1M("REL_Category_PhysicalProduct_1toN",
            RPE(typeof(Category)),
            RDE(typeof(PhysicalProduct), [RKS(nameof(PhysicalProduct.CategoryId))]));

        // DigitalProduct ↔ Tag (M:M via DigitalProductTag)
        var digitalProductToTag = RMN("REL_DigitalProduct_Tag_NtoN",
            RPE(typeof(DigitalProduct)),
            RPE(typeof(Tag)),
            RAS(typeof(DigitalProductTag),
                [RKS(nameof(DigitalProductTag.DigitalProductId))],
                [RKS(nameof(DigitalProductTag.TagId))]));

        // PhysicalProduct ↔ Tag (M:M via PhysicalProductTag)
        var physicalProductToTag = RMN("REL_PhysicalProduct_Tag_NtoN",
            RPE(typeof(PhysicalProduct)),
            RPE(typeof(Tag)),
            RAS(typeof(PhysicalProductTag),
                [RKS(nameof(PhysicalProductTag.PhysicalProductId))],
                [RKS(nameof(PhysicalProductTag.TagId))]));

        var relationships = new List<ApiRelationship>
        {
            customerToOrder, orderToOrderLine, paymentToOrder,
            categoryToCategory, categoryToDigitalProduct, categoryToPhysicalProduct, digitalProductToTag, physicalProductToTag
        };

        // 7) Assemble schema
        var schema = ApiSchema.Create
        (
            apiName: name,
            apiVersion: null,
            apiOptions: null,
            apiScalarTypes: scalars,
            apiEnumTypes: enums,
            apiObjectTypes: objects,
            apiRelationships: relationships
        );

        return schema;
    }

    /// <summary>
    ///    Builds the reusable "Simple" API schema: Scalars, Enums, and Object Types.
    /// </summary>
    private static ApiSchema BuildSimpleApiSchema(string name)
    {
        // 1) Scalars
        var scalars = new List<ApiScalarType>
        {
            S(name: nameof(String),  clr: typeof(string)),
            S(name: nameof(Int32),   clr: typeof(int)),
            S(name: nameof(Int64),   clr: typeof(long)),
            S(name: nameof(Boolean), clr: typeof(bool)),
            S(name: nameof(Ulid),    clr: typeof(Ulid))
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
        var empty = O(name: nameof(Empty), clr: typeof(Empty), options: OO(ApiIdentityPartNullHandling.ThrowOnNull), properties: []);

        var point = O(name: nameof(Point), clr: typeof(Point), options: OO(ApiIdentityPartNullHandling.ThrowOnNull), properties:
        [
            P(name: nameof(Point.X),    expression: TE.ClrRef<long>(),   required: true, ClrMemberKind.Field),
            P(name: nameof(Point.Y),    expression: TE.ClrRef<long>(),   required: true),
            P(name: nameof(Point.Note), expression: TE.ClrRef<string>(), required: false)
        ]);

        var scalarsOnly = O(name: nameof(ScalarsOnly), clr: typeof(ScalarsOnly), options: OO(ApiIdentityPartNullHandling.ThrowOnNull), properties:
        [
            P(name: nameof(ScalarsOnly.RequiredName),      expression: TE.ClrRef<string>(), required: true),
            P(name: nameof(ScalarsOnly.RequiredNumber),    expression: TE.ClrRef<long>(),   required: true),
            P(name: nameof(ScalarsOnly.RequiredPredicate), expression: TE.ClrRef<bool>(),   required: true),
            P(name: nameof(ScalarsOnly.OptionalName),      expression: TE.ClrRef<string>(), required: false, ClrMemberKind.Field),
            P(name: nameof(ScalarsOnly.OptionalNumber),    expression: TE.ClrRef<long>(),   required: false, ClrMemberKind.Field),
            P(name: nameof(ScalarsOnly.OptionalPredicate), expression: TE.ClrRef<bool>(),   required: false, ClrMemberKind.Field)
        ]);

        var person = O(name: nameof(Person), clr: typeof(Person), options: OO(ApiIdentityPartNullHandling.ThrowOnNull), identities:
        [
            I("PK_Person_Id", [IPS(nameof(Person.Id))]),
            I("AK_Person_Name", [IPS(nameof(Person.Name))])
        ], properties:
        [
            P(name: nameof(Person.Id),        expression: TE.ClrRef<int>(),                  required: true),
            P(name: nameof(Person.Name),      expression: TE.ClrRef<string>(),               required: true),
            P(name: nameof(Person.Age),       expression: TE.ClrRef<int>(),                  required: false),
            P(name: nameof(Person.Gender),    expression: TE.ClrRef<Gender>(),               required: false),
            P(name: nameof(Person.Hobbies),   expression: TE.ListOf<string>(required: true), required: false),
            P(name: nameof(Person.CompanyId), expression: TE.ClrRef<Ulid>(),                 required: false),
        ]);

        var company = O(name: nameof(Company), clr: typeof(Company), options: OO(ApiIdentityPartNullHandling.ThrowOnNull), identities:
        [
            I("PK_Company_Id", [IPS(nameof(Company.Id))]),
            I("AK_Company_Name", [IPS(nameof(Company.Name))])
        ], properties:
        [
            P(name: nameof(Company.Id),        expression: TE.ClrRef<Ulid>(),                 required: true),
            P(name: nameof(Company.Name),      expression: TE.ClrRef<string>(),               required: true),
            P(name: nameof(Company.Owner),     expression: TE.ClrRef<Person>(),               required: false),
            P(name: nameof(Company.Employees), expression: TE.ListOf<Person>(required: true), required: false)
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
        var schema = ApiSchema.Create
        (
            apiName: name,
            apiVersion: null,
            apiOptions: null,
            apiScalarTypes: scalars,
            apiEnumTypes: enums,
            apiObjectTypes: objects
        );

        return schema;
    }

    /// <summary>
    ///    Builds a consolidated test schema for identity testing.
    ///    Supports composite identities, nullable handling (both ReturnEmpty and ThrowException),
    ///    and alternate identities for comprehensive identity testing.
    /// </summary>
    private static ApiSchema BuildIdentityApiSchema(string name)
    {
        var scalars = new List<ApiScalarType>
        {
            S(name: nameof(Guid),   clr: typeof(Guid)),
            S(name: nameof(Int32),  clr: typeof(int)),
            S(name: nameof(String), clr: typeof(string)),
        };

        // IdentityScalar: Single scalar identity
        var identityScalar = O(
            name: nameof(IdentityScalar),
            clr: typeof(IdentityScalar),
            identities:
            [
                I(name: "PK_IdentityScalar", parts: [IPS(nameof(IdentityScalar.Id))]),
                I(name: "AK_IdentityScalar", parts: [IPS(nameof(IdentityScalar.Name))])
            ],
            properties:
            [
                P(name: nameof(IdentityScalar.Id), expression: TE.ClrRef<int>(), required: true),
                P(name: nameof(IdentityScalar.Name), expression: TE.ClrRef<string>(), required: true)
            ]
        );

        // IdentityTwoScalarPartComposite: Composite identity (int + string)
        var identityTwoScalarPartComposite = O(
            name: nameof(IdentityTwoScalarPartComposite),
            clr: typeof(IdentityTwoScalarPartComposite),
            identities:
            [
                I(name: "PK_IdentityTwoScalarPartComposite", parts: [IPS(nameof(IdentityTwoScalarPartComposite.Id1)), IPS(nameof(IdentityTwoScalarPartComposite.Id2))])
            ],
            properties:
            [
                P(name: nameof(IdentityTwoScalarPartComposite.Id1), expression: TE.ClrRef<int>(), required: true),
                P(name: nameof(IdentityTwoScalarPartComposite.Id2), expression: TE.ClrRef<string>(), required: false),
                P(name: nameof(IdentityTwoScalarPartComposite.Description), expression: TE.ClrRef<string>(), required: false)
            ]
        );

        // IdentityThreeScalarPartComposite: Composite identity (int + string + Guid)
        var identityThreeScalarPartComposite = O(
            name: nameof(IdentityThreeScalarPartComposite),
            clr: typeof(IdentityThreeScalarPartComposite),
            identities:
            [
                I(name: "PK_IdentityThreeScalarPartComposite", parts: [IPS(nameof(IdentityThreeScalarPartComposite.Id1)), IPS(nameof(IdentityThreeScalarPartComposite.Id2)), IPS(nameof(IdentityThreeScalarPartComposite.Id3))])
            ],
            properties:
            [
                P(name: nameof(IdentityThreeScalarPartComposite.Id1), expression: TE.ClrRef<int>(), required: true),
                P(name: nameof(IdentityThreeScalarPartComposite.Id2), expression: TE.ClrRef<string>(), required: false),
                P(name: nameof(IdentityThreeScalarPartComposite.Id3), expression: TE.ClrRef<Guid>(), required: true),
                P(name: nameof(IdentityThreeScalarPartComposite.Description), expression: TE.ClrRef<string>(), required: false)
            ]
        );

        // IdentityNested: Nested part for identity testing
        var identityNested = O(
            name: nameof(IdentityNested),
            clr: typeof(IdentityNested),
            identities:
            [
                I(name: "PK_IdentityNestedPart", parts: [IPS(nameof(IdentityNested.Id))])
            ],
            properties:
            [
                P(name: nameof(IdentityNested.Id), expression: TE.ClrRef<int>(), required: true),
                P(name: nameof(IdentityNested.Description), expression: TE.ClrRef<string>(), required: false)
            ]
        );

        // IdentityNestedComposite: Composite identity with nested part
        var identityNestedComposite = O(
            name: nameof(IdentityNestedComposite),
            clr: typeof(IdentityNestedComposite),
            identities:
            [
                I(name: "PK_IdentityNestedComposite", parts: [IPN(nameof(IdentityNestedComposite.NestedPart)), IPS(nameof(IdentityNestedComposite.Name))])
            ],
            properties:
            [
                P(name: nameof(IdentityNestedComposite.NestedPart), expression: TE.ClrRef<IdentityNested>(), required: true),
                P(name: nameof(IdentityNestedComposite.Name), expression: TE.ClrRef<string>(), required: true)
            ]
        );

        // IdentityOwner: Owner identity reference for testing owner identity handling
        var identityOwner = O(
            name: nameof(IdentityOwner),
            clr: typeof(IdentityOwner),
            identities:
            [
                I(name: "PK_IdentityOwner", parts: [IPS(nameof(IdentityOwner.Id))])
            ],
            properties:
            [
                P(name: nameof(IdentityOwner.Id), expression: TE.ClrRef<int>(), required: true),
                P(name: nameof(IdentityOwner.Description), expression: TE.ClrRef<string>(), required: false),
                P(name: nameof(IdentityOwner.Dependents), expression: TE.ListOf<IdentityOwnedComposite>(required: true), required: true),
                P(name: nameof(IdentityOwner.Dependent), expression: TE.ClrRef<IdentityOwnedDependent>(), required: true)
            ]
        );

        // IdentityOwnedComposite: Composite identity with owner identity and secondary child identity
        var identityOwnedComposite = O(
            name: nameof(IdentityOwnedComposite),
            clr: typeof(IdentityOwnedComposite),
            identities:
            [
                I(name: "PK_IdentityOwnedComposite", parts: [IPO(), IPS(nameof(IdentityOwnedComposite.LineNumber))])
            ],
            properties:
            [
                P(name: nameof(IdentityOwnedComposite.LineNumber), expression: TE.ClrRef<int>(), required: true),
                P(name: nameof(IdentityOwnedComposite.Description), expression: TE.ClrRef<string>(), required: true),
            ]
        );

        // IdentityOwnedDependent: Composite identity with owner identity only
        var identityOwnedDependent = O(
            name: nameof(IdentityOwnedDependent),
            clr: typeof(IdentityOwnedDependent),
            identities:
            [
                I(name: "PK_IdentityOwnedDependent", parts: [IPO()])
            ],
            properties:
            [
                P(name: nameof(IdentityOwnedDependent.Description), expression: TE.ClrRef<string>(), required: true)
            ]
        );

        var objects = new List<ApiObjectType>
        {
            identityScalar,
            identityTwoScalarPartComposite,
            identityThreeScalarPartComposite,
            identityNested,
            identityNestedComposite,
            identityOwner,
            identityOwnedComposite,
            identityOwnedDependent
        };

        var schema = ApiSchema.Create
        (
            apiName: name,
            apiVersion: null,
            apiOptions: null,
            apiScalarTypes: scalars,
            apiEnumTypes: [],
            apiObjectTypes: objects
        );

        return schema;
    }

    private static ApiSchema BuildRelationshipApiSchema(string name)
    {
        // 1) Scalars
        var scalars = new List<ApiScalarType>
        {
            S(name: nameof(Ulid),   clr: typeof(Ulid)),
            S(name: nameof(String), clr: typeof(string)),
            S(name: nameof(Int32),  clr: typeof(int)),
        };

        // 2) Enums - none for this schema
        var enums = new List<ApiEnumType>();

        // 3) Object Types

        // RelationshipUser
        var relationshipUser = O(name: nameof(RelationshipUser), clr: typeof(RelationshipUser), properties:
        [
            P(name: nameof(RelationshipUser.Id),       expression: TE.ClrRef<Ulid>(),                          required: true),
            P(name: nameof(RelationshipUser.UserName), expression: TE.ClrRef<string>(),                        required: true),
            P(name: nameof(RelationshipUser.Profile),  expression: TE.ClrRef<RelationshipUserProfile>(),       required: false),
            P(name: nameof(RelationshipUser.Posts), expression:    TE.ListOf<RelationshipPost>(required:true), required: true),
        ], identities:
        [
            I("PK_RelationshipUser", [IPS(nameof(RelationshipUser.Id))]),
        ]);

        // RelationshipUserProfile
        var relationshipUserProfile = O(name: nameof(RelationshipUserProfile), clr: typeof(RelationshipUserProfile), properties:
        [
            P(name: nameof(RelationshipUserProfile.UserId),      expression: TE.ClrRef<Ulid>(),                required: true),
            P(name: nameof(RelationshipUserProfile.UserRef),     expression: TE.ClrRef<RelationshipUserRef>(), required: true),
            P(name: nameof(RelationshipUserProfile.DisplayName), expression: TE.ClrRef<string>(),              required: true),
            P(name: nameof(RelationshipUserProfile.User),        expression: TE.ClrRef<RelationshipUser>(),    required: true),
        ], identities:
        [
            I("PK_RelationshipUserProfile", [IPS(nameof(RelationshipUserProfile.UserId))]),
        ]);

        // RelationshipUserRef
        var relationshipUserRef = O(name: nameof(RelationshipUserRef), clr: typeof(RelationshipUserRef), properties:
        [
            P(name: nameof(RelationshipUserRef.UserId), expression: TE.ClrRef<Ulid>(), required: true),
        ]);

        // RelationshipPost
        var relationshipPost = O(name: nameof(RelationshipPost), clr: typeof(RelationshipPost), properties:
        [
            P(name: nameof(RelationshipPost.Id),           expression: TE.ClrRef<Ulid>(),                              required: true),
            P(name: nameof(RelationshipPost.AuthorUserId), expression: TE.ClrRef<Ulid>(),                              required: true),
            P(name: nameof(RelationshipPost.AuthorUserRef),    expression: TE.ClrRef<RelationshipUserRef>(),               required: true),
            P(name: nameof(RelationshipPost.Title),        expression: TE.ClrRef<string>(),                            required: true),
            P(name: nameof(RelationshipPost.Comments),     expression: TE.ListOf<RelationshipComment>(required: true), required: true),
            P(name: nameof(RelationshipPost.Tags),         expression: TE.ListOf<RelationshipTag>(required: true),     required: true),
            P(name: nameof(RelationshipPost.User),         expression: TE.ClrRef<RelationshipUser>(),                  required: true),
        ], identities:
        [
            I("PK_RelationshipPost", [IPS(nameof(RelationshipPost.Id))]),
        ]);

        // RelationshipPostRef
        var relationshipPostRef = O(name: nameof(RelationshipPostRef), clr: typeof(RelationshipPostRef), properties:
        [
            P(name: nameof(RelationshipPostRef.PostId), expression: TE.ClrRef<Ulid>(), required: true),
        ]);

        // RelationshipComment
        var relationshipComment = O(name: nameof(RelationshipComment), clr: typeof(RelationshipComment), properties:
        [
            P(name: nameof(RelationshipComment.Id),      expression: TE.ClrRef<Ulid>(),                required: true),
            P(name: nameof(RelationshipComment.PostId),  expression: TE.ClrRef<Ulid>(),                required: true),
            P(name: nameof(RelationshipComment.PostRef), expression: TE.ClrRef<RelationshipPostRef>(), required: true),
            P(name: nameof(RelationshipComment.Body),    expression: TE.ClrRef<string>(),              required: true),
            P(name: nameof(RelationshipComment.Post),    expression: TE.ClrRef<RelationshipPost>(),    required: true),
        ], identities:
        [
            I("PK_RelationshipComment", [IPS(nameof(RelationshipComment.Id))]),
        ]);

        // RelationshipTag
        var relationshipTag = O(name: nameof(RelationshipTag), clr: typeof(RelationshipTag), properties:
        [
            P(name: nameof(RelationshipTag.Id),    expression: TE.ClrRef<Ulid>(),                          required: true),
            P(name: nameof(RelationshipTag.Name),  expression: TE.ClrRef<string>(),                        required: true),
            P(name: nameof(RelationshipTag.Posts), expression: TE.ListOf<RelationshipPost>(required:true), required: true),
        ], identities:
        [
            I("PK_RelationshipTag", [IPS(nameof(RelationshipTag.Id))]),
        ]);

        // RelationshipPostTag
        var relationshipPostTag = O(name: nameof(RelationshipPostTag), clr: typeof(RelationshipPostTag), properties:
        [
            P(name: nameof(RelationshipPostTag.PostId), expression: TE.ClrRef<Ulid>(), required: true),
            P(name: nameof(RelationshipPostTag.TagId),  expression: TE.ClrRef<Ulid>(), required: true),
        ], identities:
        [
            I("PK_RelationshipPostTag", [IPS(nameof(RelationshipPostTag.PostId)), IPS(nameof(RelationshipPostTag.TagId))]),
        ]);

        // RelationshipCatalogItem
        var relationshipCatalogItem = O(name: nameof(RelationshipCatalogItem), clr: typeof(RelationshipCatalogItem), properties:
        [
            P(name: nameof(RelationshipCatalogItem.Sku),      expression: TE.ClrRef<string>(), required: true),
            P(name: nameof(RelationshipCatalogItem.Revision), expression: TE.ClrRef<int>(), required: true),
            P(name: nameof(RelationshipCatalogItem.Name),     expression: TE.ClrRef<string>(), required: true),
        ], identities:
        [
            I("PK_RelationshipCatalogItem", [IPS(nameof(RelationshipCatalogItem.Sku)), IPS(nameof(RelationshipCatalogItem.Revision))]),
        ]);

        // RelationshipCatalogKey
        var relationshipCatalogKey = O(name: nameof(RelationshipCatalogKey), clr: typeof(RelationshipCatalogKey), properties:
        [
            P(name: nameof(RelationshipCatalogKey.Sku),      expression: TE.ClrRef<string>(), required: true),
            P(name: nameof(RelationshipCatalogKey.Revision), expression: TE.ClrRef<int>(), required: true),
        ]);

        // RelationshipOrder
        var relationshipOrder = O(name: nameof(RelationshipOrder), clr: typeof(RelationshipOrder), properties:
        [
            P(name: nameof(RelationshipOrder.Id),    expression: TE.ClrRef<Ulid>(),                               required: true),
            P(name: nameof(RelationshipOrder.Lines), expression: TE.ListOf<RelationshipOwnedLine>(required:true), required: true),
        ], identities:
        [
            I("PK_RelationshipOrder", [IPS(nameof(RelationshipOrder.Id))]),
        ]);

        // RelationshipOrderLine
        var relationshipOrderLine = O(name: nameof(RelationshipOrderLine), clr: typeof(RelationshipOrderLine), properties:
        [
            P(name: nameof(RelationshipOrderLine.OrderId),         expression: TE.ClrRef<Ulid>(),                   required: true),
            P(name: nameof(RelationshipOrderLine.LineNumber),      expression: TE.ClrRef<int>(),                    required: true),
            P(name: nameof(RelationshipOrderLine.ProductSku),      expression: TE.ClrRef<string>(),                 required: true),
            P(name: nameof(RelationshipOrderLine.ProductRevision), expression: TE.ClrRef<int>(),                    required: true),
            P(name: nameof(RelationshipOrderLine.ProductKey),      expression: TE.ClrRef<RelationshipCatalogKey>(), required: true),
        ], identities:
        [
            I("PK_RelationshipOrderLine", [IPS(nameof(RelationshipOrderLine.OrderId)), IPS(nameof(RelationshipOrderLine.LineNumber))]),
        ]);

        // RelationshipOwnedLine
        var relationshipOwnedLine = O(name: nameof(RelationshipOwnedLine), clr: typeof(RelationshipOwnedLine), properties:
        [
            P(name: nameof(RelationshipOwnedLine.LineNumber), expression: TE.ClrRef<int>(),     required: true),
            P(name: nameof(RelationshipOwnedLine.Notes),      expression: TE.ClrRef<string?>(), required: false),
        ], identities:
        [
            I("PK_RelationshipOwnedLine", [IPO(), IPS(nameof(RelationshipOwnedLine.LineNumber))]),
        ]);

        // RelationshipOrgUnit
        var relationshipOrgUnit = O(name: nameof(RelationshipOrgUnit), clr: typeof(RelationshipOrgUnit), properties:
        [
            P(name: nameof(RelationshipOrgUnit.Id),       expression: TE.ClrRef<Ulid>(),                             required: true),
            P(name: nameof(RelationshipOrgUnit.ParentId), expression: TE.ClrRef<Ulid?>(),                            required: false),
            P(name: nameof(RelationshipOrgUnit.Name),     expression: TE.ClrRef<string>(),                           required: true),
            P(name: nameof(RelationshipOrgUnit.Children), expression: TE.ListOf<RelationshipOrgUnit>(required:true), required: true),
        ], identities:
        [
            I("PK_RelationshipOrgUnit", [IPS(nameof(RelationshipOrgUnit.Id))]),
        ]);

        var objects = new List<ApiObjectType>
        {
            relationshipUser,
            relationshipUserProfile,
            relationshipUserRef,
            relationshipPost,
            relationshipPostRef,
            relationshipComment,
            relationshipTag,
            relationshipPostTag,
            relationshipCatalogItem,
            relationshipCatalogKey,
            relationshipOrder,
            relationshipOrderLine,
            relationshipOwnedLine,
            relationshipOrgUnit,
        };

        // 4) Relationships

        // User → UserProfile (1:1, scalar FK via UserProfile.UserId)
        var userToUserProfileViaScalar = R11("REL_User_UserProfile_1to1ViaScalar",
            RPE(typeof(RelationshipUser)),
            RDE(typeof(RelationshipUserProfile), [RKS(nameof(RelationshipUserProfile.UserId))]));

        // User → UserProfile (1:1, nested FK via UserProfile.UserRef.UserId)
        var userToUserProfileViaNested = R11("REL_User_UserProfile_1to1ViaNested",
            RPE(typeof(RelationshipUser)),
            RDE(typeof(RelationshipUserProfile), [RKN(nameof(RelationshipUserProfile.UserRef), [RKS(nameof(RelationshipUserRef.UserId))])]));

        // User -> Post (1:M, scalar FK via Post.AuthorUserId)
        var userToPostViaScalar = R1M("REL_User_Post_1toN_ViaScalar",
            RPE(typeof(RelationshipUser)),
            RDE(typeof(RelationshipPost), [RKS(nameof(RelationshipPost.AuthorUserId))]));

        // User -> Post (1:M, nested FK via Post.AuthorUserRef.UserId)
        var userToPostViaNested = R1M("REL_User_Post_1toN_ViaNested",
            RPE(typeof(RelationshipUser)),
            RDE(typeof(RelationshipPost), [RKN(nameof(RelationshipPost.AuthorUserRef), [RKS(nameof(RelationshipUserRef.UserId))])]));

        // Post → Comment (1:M, scalar FK via Comment.PostId)
        var postToCommentViaScalar = R1M("REL_Post_Comment_1toN_ViaScalar",
            RPE(typeof(RelationshipPost)),
            RDE(typeof(RelationshipComment), [RKS(nameof(RelationshipComment.PostId))]));

        // Post → Comment (1:M, nested FK via Comment.PostRef.PostId)
        var postToCommentViaNested = R1M("REL_Post_Comment_1toN_ViaNested",
            RPE(typeof(RelationshipPost)),
            RDE(typeof(RelationshipComment), [RKN(nameof(RelationshipComment.PostRef), [RKS(nameof(RelationshipPostRef.PostId))])]));

        // Post ↔ Tag (M:M via PostTag)
        var postToTagViaPostTag = RMN("REL_Post_Tag_NtoN_ViaPostTag",
            RPE(typeof(RelationshipPost)),
            RPE(typeof(RelationshipTag)),
            RAS(typeof(RelationshipPostTag),
                [RKS(nameof(RelationshipPostTag.PostId))],
                [RKS(nameof(RelationshipPostTag.TagId))]));

        // CatalogItem → OrderLine (1:M, composite FK via OrderLine.ProductSku + OrderLine.ProductRevision → CatalogItem.Sku + CatalogItem.Revision)
        var catalogItemToOrderLineViaScalarComposite = R1M("REL_CatalogItem_OrderLine_1toN_ViaScalarComposite",
            RPE(typeof(RelationshipCatalogItem)),
            RDE(typeof(RelationshipOrderLine), [RKS(nameof(RelationshipOrderLine.ProductSku)), RKS(nameof(RelationshipOrderLine.ProductRevision))]));

        // CatalogItem → OrderLine (1:M, composite FK via nested ProductKey → CatalogKey)
        var catalogItemToOrderLineViaNestedComposite = R1M("REL_CatalogItem_OrderLine_1toN_ViaNestedComposite",
            RPE(typeof(RelationshipCatalogItem)),
            RDE(typeof(RelationshipOrderLine), [RKN(nameof(RelationshipOrderLine.ProductKey), [RKS(nameof(RelationshipCatalogKey.Sku)), RKS(nameof(RelationshipCatalogKey.Revision))])]));

        // Order → OrderLine (1:M, owner key path via nested Order.Lines[LineNumber] → OrderLine.LineNumber)
        var orderToOrderLineViaOwnerKeyPath = R1M("REL_Order_OwnedLine_1toN_ViaOwnerKeyPath",
            RPE(typeof(RelationshipOrder)),
            RDE(typeof(RelationshipOwnedLine), [RKO([RKS(nameof(RelationshipOrder.Id))]), RKS(nameof(RelationshipOwnedLine.LineNumber))]));

        // OrgUnit self-referential relationship (1:M, scalar FK via ParentId)
        var orgUnitToOrgUnit = R1M("REL_OrgUnit_OrgUnit_1toN",
            RPE(typeof(RelationshipOrgUnit)),
            RDE(typeof(RelationshipOrgUnit), [RKS(nameof(RelationshipOrgUnit.ParentId))]));

        var relationships = new List<ApiRelationship>
        {
            userToUserProfileViaScalar, userToUserProfileViaNested, userToPostViaScalar, userToPostViaNested,
            postToCommentViaScalar, postToCommentViaNested, postToTagViaPostTag,
            catalogItemToOrderLineViaScalarComposite, catalogItemToOrderLineViaNestedComposite,
            orderToOrderLineViaOwnerKeyPath, orgUnitToOrgUnit
        };

        var schema = ApiSchema.Create
        (
            apiName: name,
            apiVersion: null,
            apiOptions: null,
            apiScalarTypes: scalars,
            apiEnumTypes: enums,
            apiObjectTypes: objects,
            apiRelationships: relationships
        );

        return schema;
    }
    #endregion
}
