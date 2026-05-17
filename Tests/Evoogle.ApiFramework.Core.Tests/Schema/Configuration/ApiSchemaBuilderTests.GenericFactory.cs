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
                .AddProperty(p => p.ParentId)
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
                .AddProperty(p => p.CategoryId)
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
                .AddProperty(p => p.CategoryId)
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

            .AddObject<DigitalProductTag>(o => o
                .AddProperty(p => p.DigitalProductId)
                .AddProperty(p => p.TagId)
                .AddIdentity("PK_DigitalProductTag", i => i
                    .AddScalarPart(p => p.DigitalProductId)
                    .AddScalarPart(p => p.TagId)))

            .AddObject<PhysicalProductTag>(o => o
                .AddProperty(p => p.PhysicalProductId)
                .AddProperty(p => p.TagId)
                .AddIdentity("PK_PhysicalProductTag", i => i
                    .AddScalarPart(p => p.PhysicalProductId)
                    .AddScalarPart(p => p.TagId)))

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
                    .AddOwnerPart()
                    .AddScalarPart(p => p.LineNumber)))

            // Payment Objects
            .AddObject<Payment>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Method)
                .AddProperty(p => p.Amount)
                .AddProperty(p => p.CapturedAt)
                .AddIdentity("PK_Payment", i => i
                    .AddScalarPart(p => p.Id)))

            // Relationships
            .AddOneToManyRelationship("REL_Customer_Order_1toN", r => r
                .WithPrincipalEnd<Customer>()
                .WithDependentEnd<Order>(np => np.AddNestedPath(np => np.Customer, sp => sp.AddScalarPath(p => p.Id))))

            .AddOneToManyRelationship("REL_Order_OrderLine_1toN", r => r
                .WithPrincipalEnd<Order>()
                .WithDependentEnd<OrderLine>(np => np.AddOwnerPath())
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete))

            .AddOneToOneRelationship("REL_Payment_Order_1to1", r => r
                .WithPrincipalEnd<Payment>()
                .WithDependentEnd<Order>(np => np.AddNestedPath(np => np.Payment, sp => sp.AddScalarPath(p => p.Id))))

            .AddOneToManyRelationship("REL_Category_Category_1toN", r => r
                .WithPrincipalEnd<Category>()
                .WithDependentEnd<Category>(sp => sp.AddScalarPath(p => p.ParentId)))

            .AddOneToManyRelationship("REL_Category_DigitalProduct_1toN", r => r
                .WithPrincipalEnd<Category>()
                .WithDependentEnd<DigitalProduct>(sp => sp.AddScalarPath(p => p.CategoryId)))

            .AddOneToManyRelationship("REL_Category_PhysicalProduct_1toN", r => r
                .WithPrincipalEnd<Category>()
                .WithDependentEnd<PhysicalProduct>(sp => sp.AddScalarPath(p => p.CategoryId)))

            .AddManyToManyRelationship("REL_DigitalProduct_Tag_NtoN", r => r
                .WithPrincipalEndA<DigitalProduct>()
                .WithPrincipalEndB<Tag>()
                .WithAssociation<DigitalProductTag>(sp => sp
                    .AddScalarPathA(p => p.DigitalProductId)
                    .AddScalarPathB(p => p.TagId)))

            .AddManyToManyRelationship("REL_PhysicalProduct_Tag_NtoN", r => r
                .WithPrincipalEndA<PhysicalProduct>()
                .WithPrincipalEndB<Tag>()
                .WithAssociation<PhysicalProductTag>(sp => sp
                    .AddScalarPathA(p => p.PhysicalProductId)
                    .AddScalarPathB(p => p.TagId)));

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
                .AddIdentity("PK_RelationshipUser", i => i.AddScalarPart(p => p.Id)))

            // RelationshipUserProfile
            .AddObject<RelationshipUserProfile>(o => o
                .AddProperty(p => p.UserId)
                .AddProperty(p => p.UserRef)
                .AddProperty(p => p.DisplayName)
                .AddProperty(p => p.User)
                .AddIdentity("PK_RelationshipUserProfile", i => i.AddScalarPart(p => p.UserId)))

            // RelationshipPost
            .AddObject<RelationshipPost>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.AuthorUserId)
                .AddProperty(p => p.AuthorUserRef)
                .AddProperty(p => p.Title)
                .AddProperty(p => p.Comments)
                .AddProperty(p => p.Tags)
                .AddProperty(p => p.User)
                .AddIdentity("PK_RelationshipPost", i => i.AddScalarPart(p => p.Id)))

            // RelationshipComment
            .AddObject<RelationshipComment>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.PostId)
                .AddProperty(p => p.PostRef)
                .AddProperty(p => p.Body)
                .AddProperty(p => p.Post)
                .AddIdentity("PK_RelationshipComment", i => i.AddScalarPart(p => p.Id)))

            // RelationshipTag
            .AddObject<RelationshipTag>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Posts)
                .AddIdentity("PK_RelationshipTag", i => i.AddScalarPart(p => p.Id)))

            // RelationshipPostTag
            .AddObject<RelationshipPostTag>(o => o
                .AddProperty(p => p.PostId)
                .AddProperty(p => p.TagId)
                .AddIdentity("PK_RelationshipPostTag", i => i
                    .AddScalarPart(p => p.PostId)
                    .AddScalarPart(p => p.TagId)))

            // RelationshipCatalogItem
            .AddObject<RelationshipCatalogItem>(o => o
                .AddProperty(p => p.Sku)
                .AddProperty(p => p.Revision)
                .AddProperty(p => p.Name)
                .AddIdentity("PK_RelationshipCatalogItem", i => i
                    .AddScalarPart(p => p.Sku)
                    .AddScalarPart(p => p.Revision)))

            // RelationshipOrder
            .AddObject<RelationshipOrder>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Lines)
                .AddIdentity("PK_RelationshipOrder", i => i.AddScalarPart(p => p.Id)))

            // RelationshipOrderLine
            .AddObject<RelationshipOrderLine>(o => o
                .AddProperty(p => p.OrderId)
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.ProductSku)
                .AddProperty(p => p.ProductRevision)
                .AddProperty(p => p.ProductKey)
                .AddIdentity("PK_RelationshipOrderLine", i => i
                    .AddScalarPart(p => p.OrderId)
                    .AddScalarPart(p => p.LineNumber)))

            // RelationshipOwnedLine
            .AddObject<RelationshipOwnedLine>(o => o
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Notes)
                .AddIdentity("PK_RelationshipOwnedLine", i => i
                    .AddOwnerPart()
                    .AddScalarPart(p => p.LineNumber)))

            // RelationshipOrgUnit
            .AddObject<RelationshipOrgUnit>(o => o
                .AddProperty(p => p.Id)
                .AddProperty(p => p.ParentId)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Children)
                .AddIdentity("PK_RelationshipOrgUnit", i => i.AddScalarPart(p => p.Id))

            // Relationships
            .AddOneToOneRelationship("REL_User_UserProfile_1to1ViaScalar", r => r
                .WithPrincipalEnd<RelationshipUser>()
                .WithDependentEnd<RelationshipUserProfile>(de => de.AddScalarPath(p => p.UserId)))

            .AddOneToOneRelationship("REL_User_UserProfile_1to1ViaNested", r => r
                .WithPrincipalEnd<RelationshipUser>()
                .WithDependentEnd<RelationshipUserProfile>(de => de.AddNestedPath(p => p.UserRef, sp => sp.AddScalarPath(p => p.UserId))))

            .AddOneToManyRelationship("REL_User_Post_1toN_ViaScalar", r => r
                .WithPrincipalEnd<RelationshipUser>()
                .WithDependentEnd<RelationshipPost>(de => de.AddScalarPath(p => p.AuthorUserId)))

            .AddOneToManyRelationship("REL_User_Post_1toN_ViaNested", r => r
                .WithPrincipalEnd<RelationshipUser>()
                .WithDependentEnd<RelationshipPost>(de => de.AddNestedPath(p => p.AuthorUserRef, sp => sp.AddScalarPath(p => p.UserId))))

            .AddOneToManyRelationship("REL_Post_Comment_1toN_ViaScalar", r => r
                .WithPrincipalEnd<RelationshipPost>()
                .WithDependentEnd<RelationshipComment>(de => de.AddScalarPath(p => p.PostId)))

            .AddOneToManyRelationship("REL_Post_Comment_1toN_ViaNested", r => r
                .WithPrincipalEnd<RelationshipPost>()
                .WithDependentEnd<RelationshipComment>(de => de.AddNestedPath(p => p.PostRef, sp => sp.AddScalarPath(p => p.PostId))))

            .AddManyToManyRelationship("REL_Post_Tag_NtoN_ViaPostTag", r => r
                .WithPrincipalEndA<RelationshipPost>()
                .WithPrincipalEndB<RelationshipTag>()
                .WithAssociation<RelationshipPostTag>(a => a
                    .AddScalarPathA(p => p.PostId)
                    .AddScalarPathB(p => p.TagId)))

            .AddOneToManyRelationship("REL_CatalogItem_OrderLine_1toN_ViaScalarComposite", r => r
                .WithPrincipalEnd<RelationshipCatalogItem>()
                .WithDependentEnd<RelationshipOrderLine>(de => de
                    .AddScalarPath(p => p.ProductSku)
                    .AddScalarPath(p => p.ProductRevision)))

            .AddOneToManyRelationship("REL_CatalogItem_OrderLine_1toN_ViaNestedComposite", r => r
                .WithPrincipalEnd<RelationshipCatalogItem>()
                .WithDependentEnd<RelationshipOrderLine>(de => de
                    .AddNestedPath(p => p.ProductKey, sp => sp
                        .AddScalarPath(p => p.Sku)
                        .AddScalarPath(p => p.Revision))))

            .AddOneToManyRelationship("REL_Order_OwnedLine_1toN_ViaOwnerKeyPath", r => r
                .WithPrincipalEnd<RelationshipOrder>()
                .WithDependentEnd<RelationshipOwnedLine>(de => de
                    .AddOwnerPath<RelationshipOrder>(sp => sp.AddScalarPath(p => p.Id))
                    .AddScalarPath(p => p.LineNumber)))

            .AddOneToManyRelationship("REL_OrgUnit_OrgUnit_1toN", r => r
                .WithPrincipalEnd<RelationshipOrgUnit>()
                .WithDependentEnd<RelationshipOrgUnit>(de => de.AddScalarPath(p => p.ParentId))));

        return builder;
    }
    #endregion
}
