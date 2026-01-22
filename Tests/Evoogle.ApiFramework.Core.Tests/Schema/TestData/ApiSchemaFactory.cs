// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.TestData;

namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>
///     Produces a compact but expressive ApiSchema suitable for most unit tests.
/// </summary>
public static class ApiSchemaFactory
{
    #region Types
    public record ApiSchemaDescriptor
    (
        ApiSchemaConfig ApiSchema,
        List<ApiTypeDescriptor>? ApiNamedTypes = null
    );

    public record ApiTypeDescriptor
    (
        ApiTypeConfig ApiType,

        ApiNamedTypeConfig? ApiNamedType = null,
        ApiCollectionTypeConfig? ApiCollectionType = null,
        ApiObjectTypeConfig? ApiObjectType = null
    );

    public record ApiSchemaConfig
    (
        string ApiName,
        string? ApiVersion = null,
        ApiSchemaOptionsConfig? ApiOptions = null,
        List<Type>? ExtensionTypes = null
    );

    public record ApiSchemaOptionsConfig
    (
        ApiIdentityNullHandling ApiIdentityNullHandling
    );

    public record ApiTypeConfig
    (
        ApiTypeKind ApiKind,
        Type ClrType,
        List<Type>? ExtensionTypes = null
    );

    public record ApiNamedTypeConfig
    (
        string ApiName
    );

    public record ApiCollectionTypeConfig
    (
        ApiTypeExpression ApiItemTypeExpression,
        ApiTypeModifiers ApiItemTypeModifiers
    );

    public record ApiObjectTypeConfig
    (
        ApiObjectTypeOptionsConfig? ApiOptions = null,
        List<ApiIdentityConfig>? ApiIdentities = null,
        List<ApiPropertyConfig>? ApiProperties = null,
        List<ApiRelationshipConfig>? ApiRelationships = null
    );

    public record ApiObjectTypeOptionsConfig
    (
        ApiIdentityNullHandling? ApiIdentityNullHandling = null
    );

    public record ApiIdentityConfig
    (
        string ApiName,
        List<ApiIdentityPartConfig> ApiIdentityParts
    );

    public record ApiIdentityPartConfig
    (
        string ApiPropertyName,
        Type? ClrConfiguredIdType = null
    );

    public record ApiPropertyConfig
    (
        string ApiName,
        ApiTypeExpression ApiTypeExpression,
        ApiTypeModifiers ApiTypeModifiers,
        string ClrName,
        ClrMemberKind ClrMemberKind
    );

    public record ApiRelationshipConfig
    (
        string ApiName,
        string? ApiPropertyName = null
    );
    #endregion

    #region Fields
    private static readonly Lazy<ApiSchema> _commerceApiSchema = new(() => BuildCommerceApiSchema());
    private static readonly Lazy<ApiSchema> _identityApiSchema = new(() => BuildIdentityApiSchema());
    private static readonly Lazy<ApiSchema> _simpleApiSchema = new(() => BuildSimpleApiSchema());
    #endregion

    #region Properties
    public static ApiSchema CommerceApiSchema => _commerceApiSchema.Value;
    public static ApiSchema IdentityApiSchema => _identityApiSchema.Value;
    public static ApiSchema SimpleApiSchema => _simpleApiSchema.Value;
    #endregion

    #region Methods
    public static ApiSchema BuildTestApiSchema(ApiSchemaKind kind)
    {
        return kind switch
        {
            ApiSchemaKind.Simple => SimpleApiSchema,
            ApiSchemaKind.Commerce => CommerceApiSchema,
            ApiSchemaKind.Identity => IdentityApiSchema,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }

    public static ApiSchema? BuildTestApiSchema(ApiSchemaDescriptor? descriptor)
    {
        if (descriptor == null)
        {
            return default;
        }

        var apiSchemaConfig = descriptor.ApiSchema;
        var apiName = apiSchemaConfig.ApiName;
        var apiVersion = apiSchemaConfig.ApiVersion;

        var apiOptions = BuildApiSchemaOptions(apiSchemaConfig);

        var apiNamedTypeDescriptors = descriptor.ApiNamedTypes ?? [];
        var apiNamedTypes = apiNamedTypeDescriptors
            .Select(x => BuildTestApiType(x))
            .Where(x => x != null)
            .Cast<ApiNamedType>()
            .ToList();

        var extensionTypeAndInstances = default(List<(Type ExtensionType, object ExtensionInstance)>?);

        var extensionTypes = apiSchemaConfig.ExtensionTypes;
        if (extensionTypes != null)
        {
            extensionTypeAndInstances = [];

            foreach (var extensionType in extensionTypes)
            {
                var extensionInstance = Activator.CreateInstance(extensionType)!;
                extensionTypeAndInstances.Add((extensionType, extensionInstance));
            }
        }

        var schema = ApiSchema.Create
        (
            apiName: apiName,
            apiVersion: apiVersion,
            apiOptions: apiOptions,
            apiNamedTypes: apiNamedTypes,
            extensionTypeAndInstances: extensionTypeAndInstances
        );

        return schema;
    }

    public static ApiType? BuildTestApiType(ApiTypeDescriptor? descriptor)
    {
        if (descriptor == null)
        {
            return default;
        }

        var apiTypeConfig = descriptor.ApiType;
        var kind = apiTypeConfig.ApiKind;

        var apiType = default(ApiType?);
        switch (kind)
        {
            case ApiTypeKind.Collection:
                {
                    var apiCollectionTypeConfig = descriptor.ApiCollectionType ?? throw new InvalidOperationException("ApiCollectionTypeFactoryArgument is required for ApiTypeKind.Collection");
                    apiType = BuildApiCollectionType(apiTypeConfig, apiCollectionTypeConfig);
                    break;
                }

            case ApiTypeKind.Enum:
                {
                    var apiNamedTypeConfig = descriptor.ApiNamedType ?? throw new InvalidOperationException("ApiNamedTypeFactoryArgument is required for ApiTypeKind.Enum");
                    apiType = BuildApiEnumType(apiTypeConfig, apiNamedTypeConfig);
                    break;
                }

            case ApiTypeKind.Object:
                {
                    var apiNamedTypeConfig = descriptor.ApiNamedType ?? throw new InvalidOperationException("ApiNamedTypeFactoryArgument is required for ApiTypeKind.Object");
                    var apiObjectTypeConfig = descriptor.ApiObjectType ?? throw new InvalidOperationException("ApiObjectTypeFactoryArgument is required for ApiTypeKind.Object");
                    apiType = BuildApiObjectType(apiTypeConfig, apiNamedTypeConfig, apiObjectTypeConfig);
                    break;
                }

            case ApiTypeKind.Scalar:
                {
                    var apiNamedTypeConfig = descriptor.ApiNamedType ?? throw new InvalidOperationException("ApiNamedTypeFactoryArgument is required for ApiTypeKind.Scalar");
                    apiType = BuildApiScalarType(apiTypeConfig, apiNamedTypeConfig);
                    break;
                }

            default:
                {
                    throw new InvalidOperationException($"Unsupported ApiTypeKind: {kind}");
                }
        }

        if (apiType == null)
        {
            throw new InvalidOperationException($"{nameof(ApiType)} creation failed");
        }

        var extensionTypes = apiTypeConfig.ExtensionTypes;
        if (extensionTypes != null)
        {
            foreach (var extensionType in extensionTypes)
            {
                var extensionInstance = Activator.CreateInstance(extensionType)!;
                apiType.AttachExtension(extensionType, extensionInstance);
            }
        }

        return apiType;
    }
    #endregion

    #region Built-In Schema Builders
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

    private static ApiIdentity I(string name, IEnumerable<string> propertyNames)
        => new(name, propertyNames.Select(pn => new ApiIdentityPart(pn)));

    private static ApiObjectType O(string name, Type clr, IEnumerable<ApiProperty> properties, IEnumerable<ApiIdentity>? identities = null, IEnumerable<ApiRelationship>? relationships = null, ApiObjectTypeOptions? options = null, OrderedDictionary<Type, object>? extensions = null)
        => new(name, options, identities, properties, relationships ?? [], clr)
        {
            Extensions = extensions
        };

    private static ApiObjectTypeOptions OO(ApiIdentityNullHandling identityNullHandling)
        => new()
        {
            ApiIdentityNullHandling = identityNullHandling
        };

    private static ApiProperty P(string name, ApiTypeExpression expression, bool required, ClrMemberKind clrMemberKind = ClrMemberKind.Property, OrderedDictionary<Type, object>? extensions = null)
        => new(name, expression, required ? ApiTypeModifiers.Required : ApiTypeModifiers.None, name, clrMemberKind)
        {
            Extensions = extensions
        };

    private static ApiRelationship R(string name, string propertyName, OrderedDictionary<Type, object>? extensions = null)
        => new(name, propertyName)
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

    /// <summary>
    ///     Builds the reusable “Commerce” API schema:
    ///     Scalars, Enums, Value Objects, Entities, Relationships, Polymorphism, Recursion, and M2M.
    /// </summary>
    private static ApiSchema BuildCommerceApiSchema(string name = "Commerce")
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
            I("PK_Customer", [nameof(Customer.Id)]),
            I("AK_Customer_Email", [nameof(Customer.Email)])
        ], relationships:
        [
            R(name: "Customer_Orders", propertyName: nameof(Customer.Orders))
        ]);

        // Product Object Types

        // Category (self-referential)
        var category = O(name: nameof(Category), clr: typeof(Category), properties:
        [
            P(name: nameof(Category.Id),        expression: TE.ClrRef<Ulid>(),                   required: true),
            P(name: nameof(Category.Name),      expression: TE.ClrRef<string>(),                 required: true),
            P(name: nameof(Category.Parent),    expression: TE.ClrRef<Category>(),               required: false),
            P(name: nameof(Category.Children),  expression: TE.ListOf<Category>(required: true), required: true)
        ], identities:
        [
            I("PK_Category", [nameof(Category.Id)]),
            I("AK_Category_Name", [nameof(Category.Name)])
        ], relationships:
        [
            R(name: "Category_Children", propertyName: nameof(Category.Children)),
            R(name: "Category_Parent", propertyName: nameof(Category.Parent))
        ]);

        // Tag (M2M with ProductBase)
        var tag = O(name: nameof(Tag), clr: typeof(Tag), properties:
        [
            P(name: nameof(Tag.Id),        expression: TE.ClrRef<Ulid>(),                      required: true),
            P(name: nameof(Tag.Name),      expression: TE.ClrRef<string>(),                    required: true),
            P(name: nameof(Tag.Products),  expression: TE.ListOf<ProductBase>(required: true), required: true)
        ], identities:
        [
            I("PK_Tag", [nameof(Tag.Id)]),
            I("AK_Tag_Name", [nameof(Tag.Name)])
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
        ], identities:
        [
            I("PK_DigitalProduct", [nameof(DigitalProduct.Id)]),
            I("AK_DigitalProduct_Sku", [nameof(DigitalProduct.Sku)])
        ], relationships:
        [
            R(name: "Product_Tags", propertyName: nameof(DigitalProduct.Tags))
        ]);

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
        ], identities:
        [
            I("PK_PhysicalProduct", [nameof(PhysicalProduct.Id)]),
            I("AK_PhysicalProduct_Sku", [nameof(PhysicalProduct.Sku)])
        ], relationships:
        [
            R(name: "Product_Tags", propertyName: nameof(PhysicalProduct.Tags))
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
            I("PK_Order", [nameof(Order.Id)])
        ], relationships:
        [
            R(name: "Customer_Orders", propertyName: nameof(Order.Customer)),
            R(name: "Order_Payment", propertyName: nameof(Order.Payment))
        ]);

        // OrderLine
        var orderLine = O(name: nameof(OrderLine), clr: typeof(OrderLine), properties:
        [
            P(name: nameof(OrderLine.OrderId),      expression: TE.ClrRef<Ulid>(),        required: true),
            P(name: nameof(OrderLine.LineNumber),   expression: TE.ClrRef<int>(),         required: true),
            P(name: nameof(OrderLine.Product),      expression: TE.ClrRef<ProductBase>(), required: true),
            P(name: nameof(OrderLine.Qty),          expression: TE.ClrRef<Quantity>(),    required: true),
            P(name: nameof(OrderLine.UnitPrice),    expression: TE.ClrRef<Money>(),       required: true),
            P(name: nameof(OrderLine.LineTotal),    expression: TE.ClrRef<Money>(),       required: true)
        ], identities:
        [
            I("PK_OrderLine", [nameof(OrderLine.OrderId), nameof(OrderLine.LineNumber)])
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
            I("PK_Payment", [nameof(Payment.Id)])
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
            customer,
            category, physicalProduct, digitalProduct, tag,
            order, orderLine, payment
        };

        // 7) Assemble schema
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
    ///    Builds the reusable “Simple” API schema: Scalars, Enums, and Object Types.
    /// </summary>
    private static ApiSchema BuildSimpleApiSchema(string name = "Simple")
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
            P(name: nameof(ScalarsOnly.OptionalName),      expression: TE.ClrRef<string>(), required: false, ClrMemberKind.Field),
            P(name: nameof(ScalarsOnly.OptionalNumber),    expression: TE.ClrRef<long>(),   required: false, ClrMemberKind.Field),
            P(name: nameof(ScalarsOnly.OptionalPredicate), expression: TE.ClrRef<bool>(),   required: false, ClrMemberKind.Field)
        ]);

        var person = O(name: nameof(Person), clr: typeof(Person), options: OO(ApiIdentityNullHandling.ThrowException), identities:
        [
            I("PK_Person_Id", [nameof(Person.Id)]),
            I("AK_Person_Name", [nameof(Person.Name)])
        ], properties:
        [
            P(name: nameof(Person.Id),        expression: TE.ClrRef<int>(),                  required: true),
            P(name: nameof(Person.Name),      expression: TE.ClrRef<string>(),               required: true),
            P(name: nameof(Person.Age),       expression: TE.ClrRef<int>(),                  required: false),
            P(name: nameof(Person.Gender),    expression: TE.ClrRef<Gender>(),               required: false),
            P(name: nameof(Person.Hobbies),   expression: TE.ListOf<string>(required: true), required: false),
            P(name: nameof(Person.CompanyId), expression: TE.ClrRef<Ulid>(),                 required: false),
        ]);

        var company = O(name: nameof(Company), clr: typeof(Company), options: OO(ApiIdentityNullHandling.ThrowException), identities:
        [
            I("PK_Company_Id", [nameof(Company.Id)]),
            I("AK_Company_Name", [nameof(Company.Name)])
        ], properties:
        [
            P(name: nameof(Company.Id),        expression: TE.ClrRef<Ulid>(),                 required: true),
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
    private static ApiSchema BuildIdentityApiSchema(string name = "Identity")
    {
        var scalars = new List<ApiScalarType>
        {
            S(name: nameof(Guid),   clr: typeof(Guid)),
            S(name: nameof(String), clr: typeof(string)),
            S(name: nameof(Int32),  clr: typeof(int))
        };

        // ProductInventory: Composite identity (int + string + Guid)
        var productInventory = O(
            name: nameof(ProductInventory),
            clr: typeof(ProductInventory),
            identities:
            [
                I(name: "PK_ProductInventory", propertyNames: [nameof(ProductInventory.WarehouseId), nameof(ProductInventory.ProductCode), nameof(ProductInventory.BatchId)])
            ],
            properties:
            [
                P(name: nameof(ProductInventory.WarehouseId),  expression: TE.ClrRef<int>(),    required: true),
                P(name: nameof(ProductInventory.ProductCode),  expression: TE.ClrRef<string>(), required: true),
                P(name: nameof(ProductInventory.BatchId),      expression: TE.ClrRef<Guid>(),   required: true),
                P(name: nameof(ProductInventory.Quantity),     expression: TE.ClrRef<int>(),    required: true)
            ]
        );

        // CompositeNullable: Composite identity with nullable parts (ReturnEmpty)
        var compositeNullable = O(
            name: nameof(CompositeNullable),
            clr: typeof(CompositeNullable),
            options: OO(ApiIdentityNullHandling.ReturnEmpty),
            identities:
            [
                I(name: "PK_CompositeNullable", propertyNames: [nameof(CompositeNullable.Part1), nameof(CompositeNullable.Part2)])
            ],
            properties:
            [
                P(name: nameof(CompositeNullable.Part1), expression: TE.ClrRef<int>(),    required: true),
                P(name: nameof(CompositeNullable.Part2), expression: TE.ClrRef<string>(), required: false)
            ]
        );

        // CompositeStrict: Composite identity with nullable parts (ThrowException)
        var compositeStrict = O(
            name: nameof(CompositeStrict),
            clr: typeof(CompositeStrict),
            options: OO(ApiIdentityNullHandling.ThrowException),
            identities:
            [
                I(name: "PK_CompositeStrict", propertyNames: [nameof(CompositeStrict.Part1), nameof(CompositeStrict.Part2)])
            ],
            properties:
            [
                P(name: nameof(CompositeStrict.Part1), expression: TE.ClrRef<int>(),    required: true),
                P(name: nameof(CompositeStrict.Part2), expression: TE.ClrRef<string>(), required: false)
            ]
        );

        // User: Primary + alternate identities
        var user = O(
            name: nameof(User),
            clr: typeof(User),
            identities:
            [
                I(name: "PK_User",        propertyNames: [nameof(User.UserId)]),
                I(name: "AK_User_Email",  propertyNames: [nameof(User.Email)]),
                I(name: "AK_User_Username", propertyNames: [nameof(User.Username)])
            ],
            properties:
            [
                P(name: nameof(User.UserId),   expression: TE.ClrRef<int>(),    required: true),
                P(name: nameof(User.Email),    expression: TE.ClrRef<string>(), required: true),
                P(name: nameof(User.Username), expression: TE.ClrRef<string>(), required: true)
            ]
        );

        var objects = new List<ApiObjectType> { productInventory, compositeNullable, compositeStrict, user };

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
    #endregion

    #region Dynamic Schema Builders
    private static ApiType? BuildApiCollectionType
    (
        ApiTypeConfig apiTypeFactoryArgument,
        ApiCollectionTypeConfig apiCollectionTypeConfig
    )
    {
        var apiItemTypeExpression = apiCollectionTypeConfig.ApiItemTypeExpression;
        var apiItemTypeModifiers = apiCollectionTypeConfig.ApiItemTypeModifiers;
        var clrCollectionType = apiTypeFactoryArgument.ClrType;

        var apiCollectionType = (ApiType)new ApiCollectionType(apiItemTypeExpression, apiItemTypeModifiers, clrCollectionType);

        return apiCollectionType;
    }

    private static ApiType? BuildApiEnumType
    (
        ApiTypeConfig apiTypeConfig,
        ApiNamedTypeConfig apiNamedTypeConfig
    )
    {
        var apiName = apiNamedTypeConfig.ApiName;
        var clrEnumType = apiTypeConfig.ClrType;

        var clrEnumValues = Enum.GetValues(clrEnumType);
        var apiEnumValues = clrEnumValues
            .Cast<int>()
            .Select(x =>
            {
                var clrName = Enum.GetName(clrEnumType, x)!;
                var clrOrdinal = x;
                var apiEnumValue = new ApiEnumValue(apiName: clrName, clrName: clrName, clrOrdinal: clrOrdinal);

                return apiEnumValue;
            })
            .ToList();

        var apiEnumType = (ApiType)new ApiEnumType(apiName, apiEnumValues, clrEnumType);

        return apiEnumType;
    }

    private static ApiType? BuildApiObjectType
    (
        ApiTypeConfig apiTypeConfig,
        ApiNamedTypeConfig apiNamedTypeConfig,
        ApiObjectTypeConfig apiObjectTypeConfig
    )
    {
        var apiName = apiNamedTypeConfig.ApiName;
        var clrObjectType = apiTypeConfig.ClrType;

        var apiOptions = BuildApiObjectTypeOptions(apiObjectTypeConfig);

        var apiIdentities = apiObjectTypeConfig.ApiIdentities?
                .Select(x =>
                {
                    var apiIdentity = new ApiIdentity
                    (
                        apiName: x.ApiName,
                        apiIdentityParts: x.ApiIdentityParts.Select(part => new ApiIdentityPart(part.ApiPropertyName, part.ClrConfiguredIdType))
                    );
                    return apiIdentity;
                });

        var apiProperties = apiObjectTypeConfig.ApiProperties?
                .Select(x =>
                {
                    var apiProperty = new ApiProperty
                    (
                        apiName: x.ApiName,
                        apiTypeExpression: x.ApiTypeExpression,
                        apiTypeModifiers: x.ApiTypeModifiers,
                        clrName: x.ClrName,
                        clrMemberKind: x.ClrMemberKind
                    );
                    return apiProperty;
                });

        var apiRelationships = apiObjectTypeConfig.ApiRelationships?
                .Select(x =>
                {
                    var apiRelationship = new ApiRelationship
                    (
                        apiName: x.ApiName,
                        apiPropertyName: x.ApiPropertyName
                    );
                    return apiRelationship;
                });

        var apiObjectType = (ApiType)new ApiObjectType
        (
            apiName,
            apiOptions,
            apiIdentities,
            apiProperties,
            apiRelationships,
            clrObjectType
        );
        return apiObjectType;
    }

    private static ApiObjectTypeOptions? BuildApiObjectTypeOptions
    (
        ApiObjectTypeConfig apiObjectTypeConfig
    )
    {
        var apiOptions = apiObjectTypeConfig.ApiOptions != null
            ? new ApiObjectTypeOptions
            {
                ApiIdentityNullHandling = apiObjectTypeConfig.ApiOptions.ApiIdentityNullHandling
            }
            : null;
        return apiOptions;
    }

    private static ApiType? BuildApiScalarType
    (
        ApiTypeConfig apiTypeConfig,
        ApiNamedTypeConfig apiNamedTypeConfig
    )
    {
        var apiName = apiNamedTypeConfig.ApiName;
        var clrScalarType = apiTypeConfig.ClrType;

        var apiScalarType = (ApiType)new ApiScalarType(apiName, clrScalarType);
        return apiScalarType;
    }

    private static ApiSchemaOptions? BuildApiSchemaOptions
    (
        ApiSchemaConfig apiSchemaConfig
    )
    {
        var apiOptions = apiSchemaConfig.ApiOptions != null
            ? new ApiSchemaOptions
            {
                ApiIdentityNullHandling = apiSchemaConfig.ApiOptions.ApiIdentityNullHandling
            }
            : null;
        return apiOptions;
    }
    #endregion
}
