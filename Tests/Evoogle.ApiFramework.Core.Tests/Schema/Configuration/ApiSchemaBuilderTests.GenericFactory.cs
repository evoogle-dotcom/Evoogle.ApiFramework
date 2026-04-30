// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;

namespace Evoogle.ApiFramework.Schema.Configuration;

[DynamicLinqType]
public static class ApiSchemaBuilderTestsGenericTestFactory
{
    #region Generic Factory Methods
    public static ApiSchema BuildCommerceApiSchema()
    {
        var builder = CreateCommerceApiSchemaBuilder();

        return builder.Build();
    }

    public static ApiSchema BuildCommerceApiSchema<TExtension>()
        where TExtension : notnull, new()
    {
        var builder = CreateCommerceApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildCommerceApiSchema<TExtension1, TExtension2>()
        where TExtension1 : notnull, new()
        where TExtension2 : notnull, new()
    {
        var builder = CreateCommerceApiSchemaBuilder()
            .AddSchemaExtension(new TExtension1())
            .AddSchemaExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiSchema BuildIdentityApiSchema()
    {
        var builder = CreateIdentityApiSchemaBuilder();

        return builder.Build();
    }

    public static ApiSchema BuildIdentityApiSchema<TExtension>()
        where TExtension : notnull, new()
    {
        var builder = CreateIdentityApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildIdentityApiSchema<TExtension1, TExtension2>()
        where TExtension1 : notnull, new()
        where TExtension2 : notnull, new()
    {
        var builder = CreateIdentityApiSchemaBuilder()
            .AddSchemaExtension(new TExtension1())
            .AddSchemaExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiSchema BuildSimpleApiSchema()
    {
        var builder = CreateSimpleApiSchemaBuilder();

        return builder.Build();
    }

    public static ApiSchema BuildSimpleApiSchema<TExtension>()
        where TExtension : notnull, new()
    {
        var builder = CreateSimpleApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildSimpleApiSchema<TExtension1, TExtension2>()
        where TExtension1 : notnull, new()
        where TExtension2 : notnull, new()
    {
        var builder = CreateSimpleApiSchemaBuilder()
            .AddSchemaExtension(new TExtension1())
            .AddSchemaExtension(new TExtension2());

        return builder.Build();
    }
    #endregion

    #region Implementation Methods
    private static ApiSchemaBuilder CreateCommerceApiSchemaBuilder()
    {
        var builder = new ApiSchemaBuilder()
            .WithName(nameof(ApiSchemaKind.Commerce))

            // Scalars
            .AddScalar<bool>()
            .AddScalar<DateOnly>()
            .AddScalar<DateTimeOffset>()
            .AddScalar<decimal>()
            .AddScalar<double>()
            .AddScalar<Guid>()
            .AddScalar<int>()
            .AddScalar<long>()
            .AddScalar<string>()
            .AddScalar<TimeOnly>()
            .AddScalar<Ulid>()
            .AddScalar<Uri>()

            // Enums
            .AddEnum<CountryCode>(e => e
                .AddValue(CountryCode.US)
                .AddValue(CountryCode.CA)
                .AddValue(CountryCode.GB))

            .AddEnum<OrderStatus>(e => e
                .AddValue(OrderStatus.Pending)
                .AddValue(OrderStatus.Paid)
                .AddValue(OrderStatus.Shipped)
                .AddValue(OrderStatus.Cancelled)
                .AddValue(OrderStatus.Returned))

            .AddEnum<PaymentMethod>(e => e
                .AddValue(PaymentMethod.Card)
                .AddValue(PaymentMethod.Cash)
                .AddValue(PaymentMethod.Wire)
                .AddValue(PaymentMethod.Crypto))

            .AddEnum<UserRole>(e => e
                .AddValue(UserRole.None)
                .AddValue(UserRole.Reader)
                .AddValue(UserRole.Editor)
                .AddValue(UserRole.Admin))

            // Value Objects
            .AddObject<Address>(o => o
                .AddProperty(p => p.Line1)
                .AddProperty(p => p.Line2)
                .AddProperty(p => p.City)
                .AddProperty(p => p.State)
                .AddProperty(p => p.Postal)
                .AddProperty(p => p.Country))

            .AddObject<EmailAddress>(o => o
                .AddProperty(p => p.Value)
                .AddIdentity("PK_EmailAddress", i => i
                    .AddScalarPart(p => p.Value)))

            .AddObject<Money>(o => o
                .AddProperty(p => p.Amount)
                .AddProperty(p => p.Currency))

            .AddObject<Quantity>(o => o
                .AddProperty(p => p.Value)
                .AddProperty(p => p.Unit))

            // Customer Objects
            .AddObject<Customer>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email)
                .AddProperty(p => p.PrimaryAddress)
                .AddProperty(p => p.Addresses)
                .AddProperty(p => p.Orders)
                .AddIdentity("PK_Customer", i => i
                    .AddScalarPart(p => p.Id))
                .AddIdentity("AK_Customer_Email", i => i
                    .AddNestedPart(p => p.Email)))

            // Product Objects
            .AddObject<Category>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Parent)
                .AddProperty(p => p.Children)
                .AddIdentity("PK_Category", i => i
                    .AddScalarPart(p => p.Id))
                .AddIdentity("AK_Category_Name", i => i
                    .AddScalarPart(p => p.Name)))

            .AddObject<DigitalProduct>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Sku)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Price)
                .AddProperty(p => p.Tags)
                .AddProperty(p => p.Category)
                .AddProperty(p => p.DownloadUrl)
                .AddProperty(p => p.Bytes)
                .AddIdentity("PK_DigitalProduct", i => i
                    .AddScalarPart(p => p.Id))
                .AddIdentity("AK_DigitalProduct_Sku", i => i
                    .AddScalarPart(p => p.Sku)))

            .AddObject<PhysicalProduct>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Sku)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Price)
                .AddProperty(p => p.Tags)
                .AddProperty(p => p.Category)
                .AddProperty(p => p.Weight)
                .AddProperty(p => p.Size)
                .AddIdentity("PK_PhysicalProduct", i => i
                    .AddScalarPart(p => p.Id))
                .AddIdentity("AK_PhysicalProduct_Sku", i => i
                    .AddScalarPart(p => p.Sku)))

            .AddObject<Tag>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddIdentity("PK_Tag", i => i
                    .AddScalarPart(p => p.Id))
                .AddIdentity("AK_Tag_Name", i => i
                    .AddScalarPart(p => p.Name)))

            // Order Objects
            .AddObject<Order>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Customer)
                .AddProperty(p => p.PlacedAt)
                .AddProperty(p => p.Status)
                .AddProperty(p => p.Lines)
                .AddProperty(p => p.Payment)
                .AddProperty(p => p.Total)
                .AddIdentity("PK_Order", i => i
                    .AddScalarPart(p => p.Id)))

            .AddObject<OrderLine>(o => o
                .AddProperty(p => p.OrderId)
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Qty)
                .AddProperty(p => p.UnitPrice)
                .AddProperty(p => p.LineTotal)
                .AddIdentity("PK_OrderLine", i => i
                    .AddScalarPart(p => p.OrderId)
                    .AddScalarPart(p => p.LineNumber)))

            // Payment Objects
            .AddObject<Payment>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Method)
                .AddProperty(p => p.Amount)
                .AddProperty(p => p.CapturedAt)
                .AddIdentity("PK_Payment", i => i
                    .AddScalarPart(p => p.Id)));

        return builder;
    }

    private static ApiSchemaBuilder CreateIdentityApiSchemaBuilder()
    {
        var builder = new ApiSchemaBuilder()
            .WithName(nameof(ApiSchemaKind.Identity))

            // Scalars
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddScalar<int>()

            // Enums

            // Objects
            .AddObject<IdentityScalar>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddIdentity("PK_IdentityScalar", i => i
                    .AddScalarPart(p => p.Id))
                .AddIdentity("AK_IdentityScalar", i => i
                    .AddScalarPart(p => p.Name)))

            .AddObject<IdentityTwoScalarPartComposite>(o => o
                .AddProperty(p => p.Id1)
                .AddProperty(p => p.Id2)
                .AddProperty(p => p.Description)
                .AddIdentity("PK_IdentityTwoScalarPartComposite", i => i
                    .AddScalarPart(p => p.Id1)
                    .AddScalarPart(p => p.Id2)))

            .AddObject<IdentityThreeScalarPartComposite>(o => o
                .AddProperty(p => p.Id1)
                .AddProperty(p => p.Id2)
                .AddProperty(p => p.Id3)
                .AddProperty(p => p.Description)
                .AddIdentity("PK_IdentityThreeScalarPartComposite", i => i
                    .AddScalarPart(p => p.Id1)
                    .AddScalarPart(p => p.Id2)
                    .AddScalarPart(p => p.Id3)))

            .AddObject<IdentityNested>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description)
                .AddIdentity("PK_IdentityNestedPart", i => i
                    .AddScalarPart(p => p.Id)))

            .AddObject<IdentityNestedComposite>(o => o
                .AddProperty(p => p.NestedPart)
                .AddProperty(p => p.Name)
                .AddIdentity("PK_IdentityNestedComposite", i => i
                    .AddNestedPart(p => p.NestedPart)
                    .AddScalarPart(p => p.Name)))

            .AddObject<IdentityOwner>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description)
                .AddProperty(p => p.Dependents)
                .AddProperty(p => p.Dependent)
                .AddIdentity("PK_IdentityOwner", i => i
                    .AddScalarPart(p => p.Id)))

            .AddObject<IdentityOwnedComposite>(o => o
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Description)
                .AddIdentity("PK_IdentityOwnedComposite", i => i
                    .AddOwnerPart()
                    .AddScalarPart(p => p.LineNumber)))

            .AddObject<IdentityOwnedDependent>(o => o
                .AddProperty(p => p.Description)
                .AddIdentity("PK_IdentityOwnedDependent", i => i
                    .AddOwnerPart()));

        return builder;
    }

    private static ApiSchemaBuilder CreateSimpleApiSchemaBuilder()
    {
        var builder = new ApiSchemaBuilder()
            .WithName(nameof(ApiSchemaKind.Simple))

            // Scalars
            .AddScalar<string>()
            .AddScalar<int>()
            .AddScalar<long>()
            .AddScalar<bool>()
            .AddScalar<Ulid>()

            // Enums
            .AddEnum<Gender>(e => e
                .AddValue(Gender.Unspecified)
                .AddValue(Gender.Male)
                .AddValue(Gender.Female))

            .AddEnum<StopLight>(e => e
                .AddValue(StopLight.None)
                .AddValue(StopLight.Green)
                .AddValue(StopLight.Yellow)
                .AddValue(StopLight.Red))

            // Objects
            .AddObject<Empty>(o => o
                .WithOptions(opt => opt.ThrowOnNullKeyPart()))

            .AddObject<Point>(o => o
                .WithOptions(opt => opt.ThrowOnNullKeyPart())
                .AddProperty(p => p.X)
                .AddProperty(p => p.Y)
                .AddProperty(p => p.Note))

            .AddObject<ScalarsOnly>(o => o
                .WithOptions(opt => opt.ThrowOnNullKeyPart())
                .AddProperty(p => p.RequiredName)
                .AddProperty(p => p.RequiredNumber)
                .AddProperty(p => p.RequiredPredicate)
                .AddProperty(p => p.OptionalName)
                .AddProperty(p => p.OptionalNumber)
                .AddProperty(p => p.OptionalPredicate))

            .AddObject<Person>(o => o
                .WithOptions(opt => opt.ThrowOnNullKeyPart())
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Age)
                .AddProperty(p => p.Gender)
                .AddProperty(p => p.Hobbies)
                .AddProperty(p => p.CompanyId)
                .AddIdentity("PK_Person_Id", i => i
                    .AddScalarPart(p => p.Id))
                .AddIdentity("AK_Person_Name", i => i
                    .AddScalarPart(p => p.Name)))

            .AddObject<Company>(o => o
                .WithOptions(opt => opt.ThrowOnNullKeyPart())
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Owner)
                .AddProperty(p => p.Employees)
                .AddIdentity("PK_Company_Id", i => i
                    .AddScalarPart(p => p.Id))
                .AddIdentity("AK_Company_Name", i => i
                    .AddScalarPart(p => p.Name)));

        return builder;
    }
    #endregion
}
