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

    public static ApiSchema BuildKeyApiSchema()
    {
        var builder = CreateKeyApiSchemaBuilder();

        return builder.Build();
    }

    public static ApiSchema BuildKeyApiSchema<TExtension>()
        where TExtension : notnull, new()
    {
        var builder = CreateKeyApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildKeyApiSchema<TExtension1, TExtension2>()
        where TExtension1 : notnull, new()
        where TExtension2 : notnull, new()
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
        where TExtension : notnull, new()
    {
        var builder = CreateRelationshipApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildRelationshipApiSchema<TExtension1, TExtension2>()
        where TExtension1 : notnull, new()
        where TExtension2 : notnull, new()
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
                .AddKeyType("PK_Customer", k => k.AddKeyPath(p => p.Id))
                .AddKeyType("AK_Customer_Email", k => k.AddKeyPath(p => p.Email.Value)))

            // Product Objects
            .AddObject<Category>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.ParentId)
                .AddProperty(p => p.Parent)
                .AddProperty(p => p.Children)
                .AddKeyType("PK_Category", k => k.AddKeyPath(p => p.Id))
                .AddKeyType("AK_Category_Name", k => k.AddKeyPath(p => p.Name)))

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
                .AddKeyType("PK_DigitalProduct", k => k.AddKeyPath(p => p.Id))
                .AddKeyType("AK_DigitalProduct_Sku", k => k.AddKeyPath(p => p.Sku)))

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
                .AddKeyType("PK_PhysicalProduct", k => k.AddKeyPath(p => p.Id))
                .AddKeyType("AK_PhysicalProduct_Sku", k => k.AddKeyPath(p => p.Sku)))

            .AddObject<Tag>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddKeyType("PK_Tag", k => k.AddKeyPath(p => p.Id))
                .AddKeyType("AK_Tag_Name", k => k.AddKeyPath(p => p.Name)))

            .AddObject<DigitalProductTag>(o => o
                .AddProperty(p => p.DigitalProductId)
                .AddProperty(p => p.TagId)
                .AddKeyType("PK_DigitalProductTag", k => k
                    .AddKeyPath(p => p.DigitalProductId)
                    .AddKeyPath(p => p.TagId)))

            .AddObject<PhysicalProductTag>(o => o
                .AddProperty(p => p.PhysicalProductId)
                .AddProperty(p => p.TagId)
                .AddKeyType("PK_PhysicalProductTag", k => k
                    .AddKeyPath(p => p.PhysicalProductId)
                    .AddKeyPath(p => p.TagId)))

            // Order Objects
            .AddObject<Order>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Customer)
                .AddProperty(p => p.PlacedAt)
                .AddProperty(p => p.Status)
                .AddProperty(p => p.Lines)
                .AddProperty(p => p.Payment)
                .AddProperty(p => p.Total)
                .AddKeyType("PK_Order", k => k.AddKeyPath(p => p.Id)))

            .AddObject<OrderLine>(o => o
                .AddProperty(p => p.OrderId)
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Qty)
                .AddProperty(p => p.UnitPrice)
                .AddProperty(p => p.LineTotal)
                .AddKeyType("PK_OrderLine", k => k
                    .AddKeyPath<Order, Ulid>(p => p.Id)
                    .AddKeyPath(p => p.LineNumber)))

            // Payment Objects
            .AddObject<Payment>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Method)
                .AddProperty(p => p.Amount)
                .AddProperty(p => p.CapturedAt)
                .AddKeyType("PK_Payment", k => k
                    .AddKeyPath(p => p.Id)))

            // Relationships
            .AddOneToManyRelationship("REL_Customer_Order_1toN", r => r
                .WithPrincipalEnd<Customer>()
                .WithDependentEnd<Order>(de => de.WithForeignKeyType("FK_Customer_Order", fk => fk.AddKeyPath(p => p.Customer.Id))))

            .AddOneToManyRelationship("REL_Order_OrderLine_1toN", r => r
                .WithPrincipalEnd<Order>()
                .WithDependentEnd<OrderLine>(de => de.WithForeignKeyType("FK_Order_OrderLine", fk => fk.AddKeyPath<Order, Ulid>(p => p.Id)))
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete))

            .AddOneToOneRelationship("REL_Payment_Order_1to1", r => r
                .WithPrincipalEnd<Payment>()
                .WithDependentEnd<Order>(de => de.WithForeignKeyType("FK_Payment_Order", fk => fk.AddKeyPath(p => p.Payment.Id))))

            .AddOneToManyRelationship("REL_Category_Category_1toN", r => r
                .WithPrincipalEnd<Category>()
                .WithDependentEnd<Category>(de => de.WithForeignKeyType("FK_Category_Category_ParentId", fk => fk.AddKeyPath(p => p.ParentId))))

            .AddOneToManyRelationship("REL_Category_DigitalProduct_1toN", r => r
                .WithPrincipalEnd<Category>()
                .WithDependentEnd<DigitalProduct>(de => de.WithForeignKeyType("FK_Category_DigitalProduct", fk => fk.AddKeyPath(p => p.CategoryId))))

            .AddOneToManyRelationship("REL_Category_PhysicalProduct_1toN", r => r
                .WithPrincipalEnd<Category>()
                .WithDependentEnd<PhysicalProduct>(de => de.WithForeignKeyType("FK_Category_PhysicalProduct", fk => fk.AddKeyPath(p => p.CategoryId))))

            .AddManyToManyRelationship("REL_DigitalProduct_Tag_NtoN", r => r
                .WithPrincipalEndA<DigitalProduct>()
                .WithPrincipalEndB<Tag>()
                .WithAssociation<DigitalProductTag>(a => a
                    .WithForeignKeyTypeA("FK_DigitalProduct_DigitalProductTag", fk => fk.AddKeyPath(p => p.DigitalProductId))
                    .WithForeignKeyTypeB("FK_Tag_DigitalProductTag", fk => fk.AddKeyPath(p => p.TagId))))

            .AddManyToManyRelationship("REL_PhysicalProduct_Tag_NtoN", r => r
                .WithPrincipalEndA<PhysicalProduct>()
                .WithPrincipalEndB<Tag>()
                .WithAssociation<PhysicalProductTag>(a => a
                    .WithForeignKeyTypeA("FK_PhysicalProduct_PhysicalProductTag", fk => fk.AddKeyPath(p => p.PhysicalProductId))
                    .WithForeignKeyTypeB("FK_Tag_PhysicalProductTag", fk => fk.AddKeyPath(p => p.TagId))));

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
                .AddKeyType("PK_KeyOneScalarPart", k => k.AddKeyPath(p => p.Id))
                .AddKeyType("AK_KeyOneScalarPart", k => k.AddKeyPath(p => p.Name)))

            .AddObject<KeyTwoScalarPartComposite>(o => o
                .AddProperty(p => p.Id1)
                .AddProperty(p => p.Id2)
                .AddProperty(p => p.Description)
                .AddKeyType("PK_KeyTwoScalarPartComposite", k => k
                    .AddKeyPath(p => p.Id1)
                    .AddKeyPath(p => p.Id2)))

            .AddObject<KeyThreeScalarPartComposite>(o => o
                .AddProperty(p => p.Id1)
                .AddProperty(p => p.Id2)
                .AddProperty(p => p.Id3)
                .AddProperty(p => p.Description)
                .AddKeyType("PK_KeyThreeScalarPartComposite", k => k
                    .AddKeyPath(p => p.Id1)
                    .AddKeyPath(p => p.Id2)
                    .AddKeyPath(p => p.Id3)))

            .AddObject<KeyNested>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description)
                .AddKeyType("PK_KeyNestedPart", k => k
                    .AddKeyPath(p => p.Id)))

            .AddObject<KeyNestedComposite>(o => o
                .AddProperty(p => p.NestedPart)
                .AddProperty(p => p.Name)
                .AddKeyType("PK_KeyNestedComposite", k => k
                    .AddKeyPath(p => p.NestedPart.Id)
                    .AddKeyPath(p => p.Name)))

            .AddObject<KeyOwner>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description)
                .AddProperty(p => p.Dependents)
                .AddProperty(p => p.Dependent)
                .AddKeyType("PK_KeyOwner", k => k
                    .AddKeyPath(p => p.Id)))

            .AddObject<KeyOwnedComposite>(o => o
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Description)
                .AddKeyType("PK_KeyOwnedComposite", k => k
                    .AddKeyPath<KeyOwner, int>(p => p.Id)
                    .AddKeyPath(p => p.LineNumber)))

            .AddObject<KeyOwnedDependent>(o => o
                .AddProperty(p => p.Description)
                .AddKeyType("PK_KeyOwnedDependent", k => k
                    .AddKeyPath<KeyOwner, int>(p => p.Id)));

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
                .AddKeyType("PK_Person_Id", k => k
                    .AddKeyPath(p => p.Id))
                .AddKeyType("AK_Person_Name", k => k
                    .AddKeyPath(p => p.Name)))

            .AddObject<Company>(o => o
                .WithOptions(opt => opt.ThrowOnNullKeyPart())
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Owner)
                .AddProperty(p => p.Employees)
                .AddKeyType("PK_Company_Id", k => k
                    .AddKeyPath(p => p.Id))
                .AddKeyType("AK_Company_Name", k => k
                    .AddKeyPath(p => p.Name)));

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
                .AddKeyType("PK_RelationshipUser", k => k
                    .AddKeyPath(p => p.Id)))

            // RelationshipUserProfile
            .AddObject<RelationshipUserProfile>(o => o
                .AddProperty(p => p.UserId)
                .AddProperty(p => p.UserRef)
                .AddProperty(p => p.DisplayName)
                .AddProperty(p => p.User)
                .AddKeyType("PK_RelationshipUserProfile", k => k
                    .AddKeyPath(p => p.UserId)))

            // RelationshipPost
            .AddObject<RelationshipPost>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.AuthorUserId)
                .AddProperty(p => p.AuthorUserRef)
                .AddProperty(p => p.Title)
                .AddProperty(p => p.Comments)
                .AddProperty(p => p.Tags)
                .AddProperty(p => p.User)
                .AddKeyType("PK_RelationshipPost", k => k
                    .AddKeyPath(p => p.Id)))

            // RelationshipComment
            .AddObject<RelationshipComment>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.PostId)
                .AddProperty(p => p.PostRef)
                .AddProperty(p => p.Body)
                .AddProperty(p => p.Post)
                .AddKeyType("PK_RelationshipComment", k => k
                    .AddKeyPath(p => p.Id)))

            // RelationshipTag
            .AddObject<RelationshipTag>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Posts)
                .AddKeyType("PK_RelationshipTag", k => k
                    .AddKeyPath(p => p.Id)))

            // RelationshipPostTag
            .AddObject<RelationshipPostTag>(o => o
                .AddProperty(p => p.PostId)
                .AddProperty(p => p.TagId)
                .AddKeyType("PK_RelationshipPostTag", k => k
                    .AddKeyPath(p => p.PostId)
                    .AddKeyPath(p => p.TagId)))

            // RelationshipCatalogItem
            .AddObject<RelationshipCatalogItem>(o => o
                .AddProperty(p => p.Sku)
                .AddProperty(p => p.Revision)
                .AddProperty(p => p.Name)
                .AddKeyType("PK_RelationshipCatalogItem", k => k
                    .AddKeyPath(p => p.Sku)
                    .AddKeyPath(p => p.Revision)))

            // RelationshipOrder
            .AddObject<RelationshipOrder>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Lines)
                .AddKeyType("PK_RelationshipOrder", k => k.AddKeyPath(p => p.Id)))

            // RelationshipOrderLine
            .AddObject<RelationshipOrderLine>(o => o
                .AddProperty(p => p.OrderId)
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.ProductSku)
                .AddProperty(p => p.ProductRevision)
                .AddProperty(p => p.ProductKey)
                .AddKeyType("PK_RelationshipOrderLine", k => k
                    .AddKeyPath(p => p.OrderId)
                    .AddKeyPath(p => p.LineNumber)))

            // RelationshipOwnedLine
            .AddObject<RelationshipOwnedLine>(o => o
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Notes)
                .AddKeyType("PK_RelationshipOwnedLine", k => k
                    .AddKeyPath<RelationshipOrder, Ulid>(p => p.Id)
                    .AddKeyPath(p => p.LineNumber)))

            // RelationshipOrgUnit
            .AddObject<RelationshipOrgUnit>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.ParentId)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Children)
                .AddKeyType("PK_RelationshipOrgUnit", k => k.AddKeyPath(p => p.Id)))

            // Relationships
            .AddOneToOneRelationship("REL_User_UserProfile_1to1ViaScalar", r => r
                .WithPrincipalEnd<RelationshipUser>()
                .WithDependentEnd<RelationshipUserProfile>(de => de.WithForeignKeyType("FK_User_UserProfile_UserId", fk => fk.AddKeyPath(p => p.UserId))))

            .AddOneToOneRelationship("REL_User_UserProfile_1to1ViaNested", r => r
                .WithPrincipalEnd<RelationshipUser>()
                .WithDependentEnd<RelationshipUserProfile>(de => de.WithForeignKeyType("FK_User_UserProfile_UserRef_UserId", fk => fk.AddKeyPath(p => p.UserRef.UserId))))

            .AddOneToManyRelationship("REL_User_Post_1toN_ViaScalar", r => r
                .WithPrincipalEnd<RelationshipUser>()
                .WithDependentEnd<RelationshipPost>(de => de.WithForeignKeyType("FK_User_Post_AuthorUserId", fk => fk.AddKeyPath(p => p.AuthorUserId))))

            .AddOneToManyRelationship("REL_User_Post_1toN_ViaNested", r => r
                .WithPrincipalEnd<RelationshipUser>()
                .WithDependentEnd<RelationshipPost>(de => de.WithForeignKeyType("FK_User_Post_AuthorUserRef_UserId", fk => fk.AddKeyPath(p => p.AuthorUserRef.UserId))))

            .AddOneToManyRelationship("REL_Post_Comment_1toN_ViaScalar", r => r
                .WithPrincipalEnd<RelationshipPost>()
                .WithDependentEnd<RelationshipComment>(de => de.WithForeignKeyType("FK_Post_Comment_PostId", fk => fk.AddKeyPath(p => p.PostId))))

            .AddOneToManyRelationship("REL_Post_Comment_1toN_ViaNested", r => r
                .WithPrincipalEnd<RelationshipPost>()
                .WithDependentEnd<RelationshipComment>(de => de.WithForeignKeyType("FK_Post_Comment_PostRef_PostId", fk => fk.AddKeyPath(p => p.PostRef.PostId))))

            .AddManyToManyRelationship("REL_Post_Tag_NtoN_ViaPostTag", r => r
                .WithPrincipalEndA<RelationshipPost>()
                .WithPrincipalEndB<RelationshipTag>()
                .WithAssociation<RelationshipPostTag>(a => a
                    .WithForeignKeyTypeA("FK_Post_PostTag_PostId", fk => fk.AddKeyPath(p => p.PostId))
                    .WithForeignKeyTypeB("FK_Tag_PostTag_TagId", fk => fk.AddKeyPath(p => p.TagId))))

            .AddOneToManyRelationship("REL_CatalogItem_OrderLine_1toN_ViaScalarComposite", r => r
                .WithPrincipalEnd<RelationshipCatalogItem>()
                .WithDependentEnd<RelationshipOrderLine>(de => de.WithForeignKeyType("FK_CatalogItem_OrderLine_ProductKeys", fk => fk
                    .AddKeyPath(p => p.ProductSku)
                    .AddKeyPath(p => p.ProductRevision))))

            .AddOneToManyRelationship("REL_CatalogItem_OrderLine_1toN_ViaNestedComposite", r => r
                .WithPrincipalEnd<RelationshipCatalogItem>()
                .WithDependentEnd<RelationshipOrderLine>(de => de.WithForeignKeyType("FK_CatalogItem_OrderLine_ProductKey", fk => fk
                    .AddKeyPath(p => p.ProductKey.Sku)
                    .AddKeyPath(p => p.ProductKey.Revision))))

            .AddOneToManyRelationship("REL_Order_OwnedLine_1toN_ViaOwnerKeyPath", r => r
                .WithPrincipalEnd<RelationshipOrder>()
                .WithDependentEnd<RelationshipOwnedLine>())

            .AddOneToManyRelationship("REL_OrgUnit_OrgUnit_1toN", r => r
                .WithPrincipalEnd<RelationshipOrgUnit>()
                .WithDependentEnd<RelationshipOrgUnit>(de => de.WithForeignKeyType("FK_OrgUnit_OrgUnit_ParentId", fk => fk.AddKeyPath(p => p.ParentId))));

        return builder;
    }
    #endregion
}
