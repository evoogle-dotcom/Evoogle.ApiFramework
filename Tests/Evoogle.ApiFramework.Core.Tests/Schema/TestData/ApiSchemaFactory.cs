// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.TestData;

namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>
///     Produces a compact but expressive ApiSchema suitable for most unit tests.
/// </summary>
public static class ApiSchemaFactory
{
    #region Types

    // ApiSchema
    public record ApiSchemaSpec
    (
        ApiSchemaParams ApiSchema,
        List<ApiTypeSpec>? ApiNamedTypes = null
    );

    public record ApiSchemaParams
    (
        string ApiName,
        string? ApiVersion = null,
        ApiSchemaOptionsParams? ApiOptions = null,
        List<Type>? ExtensionTypes = null
    );

    public record ApiSchemaOptionsParams
    (
        ApiIdentityPartNullHandling ApiIdentityPartNullHandling
    );

    // ApiType
    public record ApiTypeSpec
    (
        ApiTypeParams ApiType,

        ApiNamedTypeParams? ApiNamedType = null,
        ApiCollectionTypeParams? ApiCollectionType = null,
        ApiObjectTypeParams? ApiObjectType = null
    );

    public record ApiTypeParams
    (
        ApiTypeKind ApiKind,
        Type ClrType,
        List<Type>? ExtensionTypes = null
    );

    public record ApiNamedTypeParams
    (
        string ApiName
    );

    public record ApiCollectionTypeParams
    (
        ApiTypeExpression ApiItemTypeExpression,
        ApiTypeModifiers ApiItemTypeModifiers
    );

    public record ApiObjectTypeParams
    (
        ApiObjectTypeOptionsParams? ApiOptions = null,
        List<ApiIdentityParams>? ApiIdentities = null,
        List<ApiPropertyParams>? ApiProperties = null
    );

    public record ApiObjectTypeOptionsParams
    (
        ApiIdentityPartNullHandling? ApiIdentityPartNullHandling = null
    );

    // ApiIdentity
    public record ApiIdentityParams
    (
        string ApiName,
        List<ApiIdentityPartParams> ApiIdentityParts
    );

    public record ApiIdentityPartParams
    (
        ApiIdentityPartKind ApiKind,
        string? ApiPropertyName = null,
        string? ApiIdentityName = null,
        Type? ClrScalarTypeHint = null
    );

    // ApiProperty
    public record ApiPropertyParams
    (
        string ApiName,
        ApiTypeExpression ApiTypeExpression,
        ApiTypeModifiers ApiTypeModifiers,
        string ClrName,
        ClrMemberKind ClrMemberKind
    );

    // ApiRelationship
    public record ApiRelationashipSpec
    (
        ApiRelationashipParams ApiRelationship,

        ApiRelationashipOneToParams? ApiRelationshipOneToOne = null,
        ApiRelationashipOneToParams? ApiRelationshipOneToMany = null,
        ApiRelationashipManyToManyParams? ApiRelationshipManyToMany = null
    );

    public record ApiRelationashipParams
    (
        string ApiName,
        ApiRelationshipKind ApiKind
    );

    public record ApiRelationashipOneToParams
    (
        ApiRelationashipEndSpec ApiPrincipalEnd,
        ApiRelationashipEndSpec ApiDependentEnd
    );

    public record ApiRelationashipManyToManyParams
    (
        ApiRelationashipEndSpec ApiPrincipalEndA,
        ApiRelationashipEndSpec ApiPrincipalEndB,
        ApiRelationashipEndSpec ApiDependentEndA,
        ApiRelationashipEndSpec ApiDependentEndB,
        Type ClrAssociationObjectType
    );

    // ApiRelationshipEnd
    public record ApiRelationashipEndSpec
    (
        ApiRelationashipEndParams ApiRelationashipEnd,

        ApiRelationashipPrincipalEndParams? ApiRelationashipPrincipalEnd = null,
        ApiRelationashipDependentEndParams? ApiRelationashipDependentEnd = null
    );

    public record ApiRelationashipEndParams
    (
        ApiRelationshipEndKind ApiKind,
        Type ClrObjectType,
        ApiRelationshipDeleteBehavior ApiDeleteBehavior
    );

    public record ApiRelationashipPrincipalEndParams
    (
        string? ApiIdentityName
    );

    public record ApiRelationashipDependentEndParams
    (
        IEnumerable<ApiRelationshipKeyPath>? ApiKeyPaths = null,
        ApiRelationshipDeleteBehavior? ApiForcedDeleteBehavior = null
    );
    #endregion

    #region Fields
    private static readonly Lazy<ApiSchema> _commerceApiSchema = new(() => BuildCommerceApiSchema(nameof(ApiSchemaKind.Commerce)));
    private static readonly Lazy<ApiSchema> _identityApiSchema = new(() => BuildIdentityApiSchema(nameof(ApiSchemaKind.Identity)));
    private static readonly Lazy<ApiSchema> _simpleApiSchema = new(() => BuildSimpleApiSchema(nameof(ApiSchemaKind.Simple)));
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

    public static ApiSchema? BuildTestApiSchema(ApiSchemaSpec? apiSchemaSpec)
    {
        if (apiSchemaSpec == null)
        {
            return default;
        }

        var apiSchemaParams = apiSchemaSpec.ApiSchema;
        var apiName = apiSchemaParams.ApiName;
        var apiVersion = apiSchemaParams.ApiVersion;

        var apiOptions = BuildApiSchemaOptions(apiSchemaParams);

        var apiNamedTypeSpecs = apiSchemaSpec.ApiNamedTypes ?? [];
        var apiNamedTypes = apiNamedTypeSpecs
            .Select(spec => BuildTestApiType(spec))
            .Where(apiType => apiType != null)
            .Cast<ApiNamedType>()
            .ToList();

        var extensionTypeAndInstances = default(List<(Type ExtensionType, object ExtensionInstance)>?);

        var extensionTypes = apiSchemaParams.ExtensionTypes;
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

    public static ApiType? BuildTestApiType(ApiTypeSpec? apiTypeSpec)
    {
        if (apiTypeSpec == null)
        {
            return default;
        }

        var apiTypeParams = apiTypeSpec.ApiType;
        var kind = apiTypeParams.ApiKind;

        var apiType = default(ApiType?);
        switch (kind)
        {
            case ApiTypeKind.Collection:
                {
                    var apiCollectionTypeParams = apiTypeSpec.ApiCollectionType ?? throw new InvalidOperationException("ApiCollectionTypeFactoryArgument is required for ApiTypeKind.Collection");
                    apiType = BuildApiCollectionType(apiTypeParams, apiCollectionTypeParams);
                    break;
                }

            case ApiTypeKind.Enum:
                {
                    var apiNamedTypeParams = apiTypeSpec.ApiNamedType ?? throw new InvalidOperationException("ApiNamedTypeFactoryArgument is required for ApiTypeKind.Enum");
                    apiType = BuildApiEnumType(apiTypeParams, apiNamedTypeParams);
                    break;
                }

            case ApiTypeKind.Object:
                {
                    var apiNamedTypeParams = apiTypeSpec.ApiNamedType ?? throw new InvalidOperationException("ApiNamedTypeFactoryArgument is required for ApiTypeKind.Object");
                    var apiObjectTypeParams = apiTypeSpec.ApiObjectType ?? throw new InvalidOperationException("ApiObjectTypeFactoryArgument is required for ApiTypeKind.Object");
                    apiType = BuildApiObjectType(apiTypeParams, apiNamedTypeParams, apiObjectTypeParams);
                    break;
                }

            case ApiTypeKind.Scalar:
                {
                    var apiNamedTypeParams = apiTypeSpec.ApiNamedType ?? throw new InvalidOperationException("ApiNamedTypeFactoryArgument is required for ApiTypeKind.Scalar");
                    apiType = BuildApiScalarType(apiTypeParams, apiNamedTypeParams);
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

        var extensionTypes = apiTypeParams.ExtensionTypes;
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

    /// <summary>
    ///     Builds the reusable “Commerce” API schema:
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
            P(name: nameof(Category.Id),        expression: TE.ClrRef<Ulid>(),                   required: true),
            P(name: nameof(Category.Name),      expression: TE.ClrRef<string>(),                 required: true),
            P(name: nameof(Category.Parent),    expression: TE.ClrRef<Category>(),               required: false),
            P(name: nameof(Category.Children),  expression: TE.ListOf<Category>(required: true), required: true)
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
        // extensions: Ext(new() { ["discriminator"] = "kind" })); // Example: your framework’s discriminator key

        var digitalProduct = O(name: nameof(DigitalProduct), clr: typeof(DigitalProduct), properties:
        [
            // ProductBase properties
            P(name: nameof(ProductBase.Id),     expression: TE.ClrRef<Ulid>(),      required: true),
            P(name: nameof(ProductBase.Sku),    expression: TE.ClrRef<string>(),    required: true),
            P(name: nameof(ProductBase.Name),   expression: TE.ClrRef<string>(),    required: true),
            P(name: nameof(ProductBase.Price),  expression: TE.ClrRef<Money>(),     required: true),

            // DigitalProduct properties
            P(name: nameof(DigitalProduct.Tags),           expression: TE.ListOf<Tag>(required: true), required: false),
            P(name: nameof(DigitalProduct.Category),       expression: TE.ClrRef<Category>(),          required: false),
            P(name: nameof(DigitalProduct.DownloadUrl),    expression: TE.ClrRef<Uri>(),               required: false),
            P(name: nameof(DigitalProduct.Bytes),          expression: TE.ClrRef<long>(),              required: false),
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
            P(name: nameof(PhysicalProduct.Tags),      expression: TE.ListOf<Tag>(required: true), required: false),
            P(name: nameof(PhysicalProduct.Category),  expression: TE.ClrRef<Category>(),          required: false),
            P(name: nameof(PhysicalProduct.Weight),    expression: TE.ClrRef<decimal>(),           required: true),
            P(name: nameof(PhysicalProduct.Size),      expression: TE.ClrRef<Quantity>(),          required: false),
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
            I("PK_OrderLine", [IPS(nameof(OrderLine.OrderId)), IPS(nameof(OrderLine.LineNumber))])
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

        // 5) Objects list (include value objects + entities + abstract base + derived)
        var objects = new List<ApiObjectType>
        {
            money, quantity, emailAddress, address,
            customer,
            category, physicalProduct, digitalProduct, tag,
            order, orderLine, payment
        };

        // 6) Assemble schema
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
    #endregion

    #region Dynamic Schema Builders
    private static ApiType? BuildApiCollectionType
    (
        ApiTypeParams apiTypeParams,
        ApiCollectionTypeParams apiCollectionTypeParams
    )
    {
        var apiItemTypeExpression = apiCollectionTypeParams.ApiItemTypeExpression;
        var apiItemTypeModifiers = apiCollectionTypeParams.ApiItemTypeModifiers;
        var clrCollectionType = apiTypeParams.ClrType;

        var apiCollectionType = (ApiType)new ApiCollectionType(apiItemTypeExpression, apiItemTypeModifiers, clrCollectionType);

        return apiCollectionType;
    }

    private static ApiType? BuildApiEnumType
    (
        ApiTypeParams apiTypeParams,
        ApiNamedTypeParams apiNamedTypeParams
    )
    {
        var apiName = apiNamedTypeParams.ApiName;
        var clrEnumType = apiTypeParams.ClrType;

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
        ApiTypeParams apiTypeParams,
        ApiNamedTypeParams apiNamedTypeParams,
        ApiObjectTypeParams apiObjectTypeParams
    )
    {
        var apiName = apiNamedTypeParams.ApiName;
        var clrObjectType = apiTypeParams.ClrType;

        var apiOptions = BuildApiObjectTypeOptions(apiObjectTypeParams);

        var apiIdentities = apiObjectTypeParams.ApiIdentities?
                .Select(identity =>
                {
                    var apiIdentity = new ApiIdentity
                    (
                        apiName: identity.ApiName,
                        apiIdentityParts: identity.ApiIdentityParts.Select(part =>
                        {
                            var apiKind = part.ApiKind;
                            ApiIdentityPart apiIdentityPart = apiKind switch
                            {
                                ApiIdentityPartKind.Scalar => new ApiIdentityScalarPart(part.ApiPropertyName!, part.ClrScalarTypeHint),
                                ApiIdentityPartKind.Nested => new ApiIdentityNestedPart(part.ApiPropertyName!, part.ApiIdentityName),
                                ApiIdentityPartKind.Owner => new ApiIdentityOwnerPart(part.ApiIdentityName),
                                _ => throw new InvalidOperationException($"Unsupported ApiIdentityPartKind: {apiKind}"),
                            };
                            return apiIdentityPart;
                        })
                    );
                    return apiIdentity;
                });

        var apiProperties = apiObjectTypeParams.ApiProperties?
                .Select(property =>
                {
                    var apiProperty = new ApiProperty
                    (
                        apiName: property.ApiName,
                        apiTypeExpression: property.ApiTypeExpression,
                        apiTypeModifiers: property.ApiTypeModifiers,
                        clrName: property.ClrName,
                        clrMemberKind: property.ClrMemberKind
                    );
                    return apiProperty;
                });

        var apiObjectType = (ApiType)new ApiObjectType
        (
            apiName,
            apiOptions,
            apiIdentities,
            apiProperties,
            clrObjectType
        );
        return apiObjectType;
    }

    private static ApiObjectTypeOptions? BuildApiObjectTypeOptions(ApiObjectTypeParams apiObjectTypeParams)
    {
        var apiObjectTypeOptions = apiObjectTypeParams.ApiOptions != null
            ? new ApiObjectTypeOptions
            {
                ApiIdentityPartNullHandling = apiObjectTypeParams.ApiOptions.ApiIdentityPartNullHandling
            }
            : null;
        return apiObjectTypeOptions;
    }

    private static ApiType? BuildApiScalarType
    (
        ApiTypeParams apiTypeConfig,
        ApiNamedTypeParams apiNamedTypeConfig
    )
    {
        var apiName = apiNamedTypeConfig.ApiName;
        var clrScalarType = apiTypeConfig.ClrType;

        var apiScalarType = (ApiType)new ApiScalarType(apiName, clrScalarType);
        return apiScalarType;
    }

    private static ApiSchemaOptions? BuildApiSchemaOptions(ApiSchemaParams apiSchemaParams)
    {
        var apiSchemaOptions = apiSchemaParams.ApiOptions != null
            ? new ApiSchemaOptions
            {
                ApiIdentityPartNullHandling = apiSchemaParams.ApiOptions.ApiIdentityPartNullHandling
            }
            : null;
        return apiSchemaOptions;
    }
    #endregion
}
