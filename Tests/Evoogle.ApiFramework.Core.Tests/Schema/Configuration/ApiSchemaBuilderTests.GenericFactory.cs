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
        where TExtension : class, new()
    {
        var builder = CreateCommerceApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildCommerceApiSchema<TExtension1, TExtension2>()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
    {
        var builder = CreateCommerceApiSchemaBuilder()
            .AddSchemaExtension(new TExtension1())
            .AddSchemaExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiSchema BuildKeyApiSchema()
    {
        var builder = CreateKeyApiSchemaBuilder();

        return builder.Build();
    }

    public static ApiSchema BuildKeyApiSchema<TExtension>()
        where TExtension : class, new()
    {
        var builder = CreateKeyApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildKeyApiSchema<TExtension1, TExtension2>()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
    {
        var builder = CreateKeyApiSchemaBuilder()
            .AddSchemaExtension(new TExtension1())
            .AddSchemaExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiSchema BuildRelationshipApiSchema()
    {
        var builder = CreateRelationshipApiSchemaBuilder();

        return builder.Build();
    }

    public static ApiSchema BuildRelationshipApiSchema<TExtension>()
        where TExtension : class, new()
    {
        var builder = CreateRelationshipApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildRelationshipApiSchema<TExtension1, TExtension2>()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
    {
        var builder = CreateRelationshipApiSchemaBuilder()
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
        where TExtension : class, new()
    {
        var builder = CreateSimpleApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildSimpleApiSchema<TExtension1, TExtension2>()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
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
                .AddProperty(p => p.Value))

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
                .AddKey("PK_Customer", p => p.Id)
                .AddKey("AK_Customer_Email", p => p.Email.Value))

            // Product Objects
            .AddObject<Category>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.ParentId)
                .AddProperty(p => p.Parent)
                .AddProperty(p => p.Children)
                .AddKey("PK_Category", p => p.Id)
                .AddKey("AK_Category_Name", p => p.Name))

            .AddObject<DigitalProduct>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Sku)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Price)
                .AddProperty(p => p.Tags)
                .AddProperty(p => p.Category)
                .AddProperty(p => p.CategoryId)
                .AddProperty(p => p.DownloadUrl)
                .AddProperty(p => p.Bytes)
                .AddKey("PK_DigitalProduct", p => p.Id)
                .AddKey("AK_DigitalProduct_Sku", p => p.Sku))

            .AddObject<PhysicalProduct>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Sku)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Price)
                .AddProperty(p => p.Tags)
                .AddProperty(p => p.Category)
                .AddProperty(p => p.CategoryId)
                .AddProperty(p => p.Weight)
                .AddProperty(p => p.Size)
                .AddKey("PK_PhysicalProduct", p => p.Id)
                .AddKey("AK_PhysicalProduct_Sku", p => p.Sku))

            .AddObject<Tag>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddKey("PK_Tag", p => p.Id)
                .AddKey("AK_Tag_Name", p => p.Name))

            .AddObject<DigitalProductTag>(o => o
                .AddProperty(p => p.DigitalProductId)
                .AddProperty(p => p.TagId)
                .AddKey("PK_DigitalProductTag", p => p.DigitalProductId, p => p.TagId))

            .AddObject<PhysicalProductTag>(o => o
                .AddProperty(p => p.PhysicalProductId)
                .AddProperty(p => p.TagId)
                .AddKey("PK_PhysicalProductTag", p => p.PhysicalProductId, p => p.TagId))

            // Order Objects
            .AddObject<Order>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Customer)
                .AddProperty(p => p.PlacedAt)
                .AddProperty(p => p.Status)
                .AddProperty(p => p.Lines)
                .AddProperty(p => p.Payment)
                .AddProperty(p => p.Total)
                .AddKey("PK_Order", p => p.Id))

            .AddObject<OrderLine>(o => o
                .AddProperty(p => p.OrderId)
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Qty)
                .AddProperty(p => p.UnitPrice)
                .AddProperty(p => p.LineTotal)
                .AddKey("PK_OrderLine", k => k
                    .AddPathFrom<Order>(p => p.Id)
                    .AddPath(p => p.LineNumber)))

            // Payment Objects
            .AddObject<Payment>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Method)
                .AddProperty(p => p.Amount)
                .AddProperty(p => p.CapturedAt)
                .AddKey("PK_Payment", p => p.Id))

            // Relationships
            .AddOneToManyRelationship("REL_Customer_Order_1toN", r => r
                .From<Customer>()
                .To<Order>(de => de.WithForeignKey(p => p.Id)))

            .AddOneToManyRelationship("REL_Order_OrderLine_1toN", r => r
                .From<Order>()
                .To<OrderLine>(de => de.WithForeignKeyFrom<Order>(p => p.Id))
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete))

            .AddOneToOneRelationship("REL_Payment_Order_1to1", r => r
                .From<Payment>()
                .To<Order>(de => de.WithForeignKey(p => p.Payment.Id)))

            .AddOneToManyRelationship("REL_Category_Category_1toN", r => r
                .From<Category>()
                .To<Category>(de => de.WithForeignKey(p => p.ParentId)))

            .AddOneToManyRelationship("REL_Category_DigitalProduct_1toN", r => r
                .From<Category>()
                .To<DigitalProduct>(de => de.WithForeignKey(p => p.CategoryId)))

            .AddOneToManyRelationship("REL_Category_PhysicalProduct_1toN", r => r
                .From<Category>("PK_Category")
                .To<PhysicalProduct>(de => de.WithForeignKey(p => p.CategoryId)))

            .AddManyToManyRelationship("REL_DigitalProduct_Tag_NtoN", r => r
                .Between<DigitalProduct>()
                .And<Tag>()
                .WithAssociation<DigitalProductTag>(a => a
                    .WithForeignKeyA(p => p.DigitalProductId)
                    .WithForeignKeyB(p => p.TagId)))

            .AddManyToManyRelationship("REL_PhysicalProduct_Tag_NtoN", r => r
                .Between<PhysicalProduct>("PK_PhysicalProduct")
                .And<Tag>("PK_Tag")
                .WithAssociation<PhysicalProductTag>(a => a
                    .WithForeignKeyA(p => p.PhysicalProductId)
                    .WithForeignKeyB(p => p.TagId)));

        return builder;
    }

    private static ApiSchemaBuilder CreateKeyApiSchemaBuilder()
    {
        var builder = new ApiSchemaBuilder()
            .WithName(nameof(ApiSchemaKind.Key))

            // Scalars
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddScalar<int>()

            // Enums

            // Objects
            .AddObject<KeyOneScalarPart>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddKey("PK_KeyOneScalarPart", p => p.Id)
                .AddKey("AK_KeyOneScalarPart", p => p.Name))

            .AddObject<KeyTwoScalarPartComposite>(o => o
                .AddProperty(p => p.Id1)
                .AddProperty(p => p.Id2)
                .AddProperty(p => p.Description)
                .AddKey("PK_KeyTwoScalarPartComposite", p => p.Id1, p => p.Id2))

            .AddObject<KeyThreeScalarPartComposite>(o => o
                .AddProperty(p => p.Id1)
                .AddProperty(p => p.Id2)
                .AddProperty(p => p.Id3)
                .AddProperty(p => p.Description)
                .AddKey("PK_KeyThreeScalarPartComposite", p => p.Id1, p => p.Id2, p => p.Id3))

            .AddObject<KeyNested>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description)
                .AddKey("PK_KeyNestedPart", p => p.Id))

            .AddObject<KeyNestedComposite>(o => o
                .AddProperty(p => p.NestedPart)
                .AddProperty(p => p.Name)
                .AddKey("PK_KeyNestedComposite", p => p.NestedPart.Id, p => p.Name))

            .AddObject<KeyOwner>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description)
                .AddProperty(p => p.Dependents)
                .AddProperty(p => p.Dependent)
                .AddKey("PK_KeyOwner", p => p.Id))

            .AddObject<KeyOwnedComposite>(o => o
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Description)
                .AddKey("PK_KeyOwnedComposite", k => k
                    .AddPathFrom<KeyOwner>(p => p.Id)
                    .AddPath(p => p.LineNumber)))

            .AddObject<KeyOwnedDependent>(o => o
                .AddProperty(p => p.Description)
                .AddKeyFrom<KeyOwner>("PK_KeyOwnedDependent", p => p.Id));

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
                .AddKey("PK_Person_Id", p => p.Id)
                .AddKey("AK_Person_Name", p => p.Name))

            .AddObject<Company>(o => o
                .WithOptions(opt => opt.ThrowOnNullKeyPart())
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Owner)
                .AddProperty(p => p.Employees)
                .AddKey("PK_Company_Id", p => p.Id)
                .AddKey("AK_Company_Name", p => p.Name));

        return builder;
    }

    private static ApiSchemaBuilder CreateRelationshipApiSchemaBuilder()
    {
        var builder = new ApiSchemaBuilder()
            .WithName(nameof(ApiSchemaKind.Relationship))

            // Scalars
            .AddScalar<int>()
            .AddScalar<string>()
            .AddScalar<Ulid>()

            // Enums

            // Value Objects
            .AddObject<RelationshipUserRef>(o => o
                .AddProperty(p => p.UserId))

            .AddObject<RelationshipPostRef>(o => o
                .AddProperty(p => p.PostId))

            .AddObject<RelationshipCatalogKey>(o => o
                .AddProperty(p => p.Sku)
                .AddProperty(p => p.Revision))

            // RelationshipUser
            .AddObject<RelationshipUser>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.UserName)
                .AddProperty(p => p.Profile)
                .AddProperty(p => p.Posts)
                .AddKey("PK_RelationshipUser", p => p.Id))

            // RelationshipUserProfile
            .AddObject<RelationshipUserProfile>(o => o
                .AddProperty(p => p.UserId)
                .AddProperty(p => p.UserRef)
                .AddProperty(p => p.DisplayName)
                .AddProperty(p => p.User)
                .AddKey("PK_RelationshipUserProfile", p => p.UserId))

            // RelationshipPost
            .AddObject<RelationshipPost>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.AuthorUserId)
                .AddProperty(p => p.AuthorUserRef)
                .AddProperty(p => p.Title)
                .AddProperty(p => p.Comments)
                .AddProperty(p => p.Tags)
                .AddProperty(p => p.User)
                .AddKey("PK_RelationshipPost", p => p.Id))

            // RelationshipComment
            .AddObject<RelationshipComment>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.PostId)
                .AddProperty(p => p.PostRef)
                .AddProperty(p => p.Body)
                .AddProperty(p => p.Post)
                .AddKey("PK_RelationshipComment", p => p.Id))

            // RelationshipTag
            .AddObject<RelationshipTag>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Posts)
                .AddKey("PK_RelationshipTag", p => p.Id))

            // RelationshipPostTag
            .AddObject<RelationshipPostTag>(o => o
                .AddProperty(p => p.PostId)
                .AddProperty(p => p.TagId)
                .AddKey("PK_RelationshipPostTag", p => p.PostId, p => p.TagId))

            // RelationshipCatalogItem
            .AddObject<RelationshipCatalogItem>(o => o
                .AddProperty(p => p.Sku)
                .AddProperty(p => p.Revision)
                .AddProperty(p => p.Name)
                .AddKey("PK_RelationshipCatalogItem", p => p.Sku, p => p.Revision))

            // RelationshipOrder
            .AddObject<RelationshipOrder>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Lines)
                .AddKey("PK_RelationshipOrder", p => p.Id))

            // RelationshipOrderLine
            .AddObject<RelationshipOrderLine>(o => o
                .AddProperty(p => p.OrderId)
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.ProductSku)
                .AddProperty(p => p.ProductRevision)
                .AddProperty(p => p.ProductKey)
                .AddKey("PK_RelationshipOrderLine", p => p.OrderId, p => p.LineNumber))

            // RelationshipOwnedLine
            .AddObject<RelationshipOwnedLine>(o => o
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Notes)
                .AddKey("PK_RelationshipOwnedLine", k => k
                    .AddPathFrom<RelationshipOrder>(p => p.Id)
                    .AddPath(p => p.LineNumber)))

            // RelationshipOrgUnit
            .AddObject<RelationshipOrgUnit>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.ParentId)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Children)
                .AddKey("PK_RelationshipOrgUnit", p => p.Id))

            // Relationships
            .AddOneToOneRelationship("REL_User_UserProfile_1to1ViaScalar", r => r
                .From<RelationshipUser>()
                .To<RelationshipUserProfile>(de => de.WithForeignKey(p => p.UserId)))

            .AddOneToOneRelationship("REL_User_UserProfile_1to1ViaNested", r => r
                .From<RelationshipUser>()
                .To<RelationshipUserProfile>(de => de.WithForeignKey(p => p.UserRef.UserId)))

            .AddOneToManyRelationship("REL_User_Post_1toN_ViaScalar", r => r
                .From<RelationshipUser>()
                .To<RelationshipPost>(de => de.WithForeignKey(p => p.AuthorUserId)))

            .AddOneToManyRelationship("REL_User_Post_1toN_ViaNested", r => r
                .From<RelationshipUser>()
                .To<RelationshipPost>(de => de.WithForeignKey(p => p.AuthorUserRef.UserId)))

            .AddOneToManyRelationship("REL_Post_Comment_1toN_ViaScalar", r => r
                .From<RelationshipPost>()
                .To<RelationshipComment>(de => de.WithForeignKey(p => p.PostId)))

            .AddOneToManyRelationship("REL_Post_Comment_1toN_ViaNested", r => r
                .From<RelationshipPost>()
                .To<RelationshipComment>(de => de.WithForeignKey(p => p.PostRef.PostId)))

            .AddManyToManyRelationship("REL_Post_Tag_NtoN_ViaPostTag", r => r
                .Between<RelationshipPost>()
                .And<RelationshipTag>()
                .WithAssociation<RelationshipPostTag>(a => a
                    .WithForeignKeyA(p => p.PostId)
                    .WithForeignKeyB(p => p.TagId)))

            .AddOneToManyRelationship("REL_CatalogItem_OrderLine_1toN_ViaScalarComposite", r => r
                .From<RelationshipCatalogItem>()
                .To<RelationshipOrderLine>(de => de.WithForeignKey(p => p.ProductSku, p => p.ProductRevision)))

            .AddOneToManyRelationship("REL_CatalogItem_OrderLine_1toN_ViaNestedComposite", r => r
                .From<RelationshipCatalogItem>()
                .To<RelationshipOrderLine>(de => de.WithForeignKey(p => p.ProductKey.Sku, p => p.ProductKey.Revision)))

            .AddOneToManyRelationship("REL_Order_OwnedLine_1toN_ViaOwnerKeyPath", r => r
                .From<RelationshipOrder>()
                .To<RelationshipOwnedLine>(de => de.WithForeignKeyFrom<RelationshipOrder>(p => p.Id)))

            .AddOneToManyRelationship("REL_OrgUnit_OrgUnit_1toN", r => r
                .From<RelationshipOrgUnit>()
                .To<RelationshipOrgUnit>(de => de.WithForeignKey(p => p.ParentId)));

        return builder;
    }
    #endregion
}
