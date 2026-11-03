// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;
using Evoogle.ApiFramework.Schema.Query;
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
        where TExtension : class, IApiSchemaExtension, new()
    {
        var builder = CreateCommerceApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildCommerceApiSchema<TExtension1, TExtension2>()
        where TExtension1 : class, IApiSchemaExtension, new()
        where TExtension2 : class, IApiSchemaExtension, new()
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
        where TExtension : class, IApiSchemaExtension, new()
    {
        var builder = CreateKeyApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildKeyApiSchema<TExtension1, TExtension2>()
        where TExtension1 : class, IApiSchemaExtension, new()
        where TExtension2 : class, IApiSchemaExtension, new()
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
        where TExtension : class, IApiSchemaExtension, new()
    {
        var builder = CreateRelationshipApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildRelationshipApiSchema<TExtension1, TExtension2>()
        where TExtension1 : class, IApiSchemaExtension, new()
        where TExtension2 : class, IApiSchemaExtension, new()
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
        where TExtension : class, IApiSchemaExtension, new()
    {
        var builder = CreateSimpleApiSchemaBuilder()
            .AddSchemaExtension(new TExtension());

        return builder.Build();
    }

    public static ApiSchema BuildSimpleApiSchema<TExtension1, TExtension2>()
        where TExtension1 : class, IApiSchemaExtension, new()
        where TExtension2 : class, IApiSchemaExtension, new()
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
            .AddEnum<CountryCode>(static e => e
                .AddValue(CountryCode.US)
                .AddValue(CountryCode.CA)
                .AddValue(CountryCode.GB))

            .AddEnum<OrderStatus>(static e => e
                .AddValue(OrderStatus.Pending)
                .AddValue(OrderStatus.Paid)
                .AddValue(OrderStatus.Shipped)
                .AddValue(OrderStatus.Cancelled)
                .AddValue(OrderStatus.Returned))

            .AddEnum<PaymentMethod>(static e => e
                .AddValue(PaymentMethod.Card)
                .AddValue(PaymentMethod.Cash)
                .AddValue(PaymentMethod.Wire)
                .AddValue(PaymentMethod.Crypto))

            .AddEnum<UserRole>(static e => e
                .AddValue(UserRole.None)
                .AddValue(UserRole.Reader)
                .AddValue(UserRole.Editor)
                .AddValue(UserRole.Admin))

            // Value Objects
            .AddObject<Address>(static o => o
                .AddProperty(static p => p.Line1)
                .AddProperty(static p => p.Line2)
                .AddProperty(static p => p.City)
                .AddProperty(static p => p.State)
                .AddProperty(static p => p.Postal)
                .AddProperty(static p => p.Country))

            .AddObject<EmailAddress>(static o => o
                .AddProperty(static p => p.Value))

            .AddObject<Money>(static o => o
                .AddProperty(static p => p.Amount)
                .AddProperty(static p => p.Currency))

            .AddObject<Quantity>(static o => o
                .AddProperty(static p => p.Value)
                .AddProperty(static p => p.Unit))

            // Customer Objects
            .AddObject<Customer>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.Email)
                .AddProperty(static p => p.PrimaryAddress)
                .AddProperty(static p => p.Addresses)
                .AddProperty(static p => p.Orders)
                .AddKey("PK_Customer", static p => p.Id)
                .AddKey("AK_Customer_Email", static p => p.Email.Value))

            .AddObject<CustomerProfile>(static o => o
                .AddProperty(static p => p.Biography))

            // Product Objects
            .AddObject<Category>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.ParentId)
                .AddProperty(static p => p.Parent)
                .AddProperty(static p => p.Children)
                .AddKey("PK_Category", static p => p.Id)
                .AddKey("AK_Category_Name", static p => p.Name))

            .AddObject<DigitalProduct>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Sku)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.Price)
                .AddProperty(static p => p.Tags)
                .AddProperty(static p => p.Category)
                .AddProperty(static p => p.CategoryId)
                .AddProperty(static p => p.DownloadUrl)
                .AddProperty(static p => p.Bytes)
                .AddKey("PK_DigitalProduct", static p => p.Id)
                .AddKey("AK_DigitalProduct_Sku", static p => p.Sku))

            .AddObject<PhysicalProduct>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Sku)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.Price)
                .AddProperty(static p => p.Tags)
                .AddProperty(static p => p.Category)
                .AddProperty(static p => p.CategoryId)
                .AddProperty(static p => p.Weight)
                .AddProperty(static p => p.Size)
                .AddKey("PK_PhysicalProduct", static p => p.Id)
                .AddKey("AK_PhysicalProduct_Sku", static p => p.Sku))

            .AddObject<Tag>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name)
                .AddKey("PK_Tag", static p => p.Id)
                .AddKey("AK_Tag_Name", static p => p.Name))

            .AddObject<DigitalProductTag>(static o => o
                .AddProperty(static p => p.DigitalProductId)
                .AddProperty(static p => p.TagId)
                .AddKey("PK_DigitalProductTag", static p => p.DigitalProductId, static p => p.TagId))

            .AddObject<PhysicalProductTag>(static o => o
                .AddProperty(static p => p.PhysicalProductId)
                .AddProperty(static p => p.TagId)
                .AddKey("PK_PhysicalProductTag", static p => p.PhysicalProductId, static p => p.TagId))

            // Order Objects
            .AddObject<Order>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Customer)
                .AddProperty(static p => p.PlacedAt)
                .AddProperty(static p => p.Status)
                .AddProperty(static p => p.Lines)
                .AddProperty(static p => p.Payment)
                .AddProperty(static p => p.Total)
                .AddKey("PK_Order", static p => p.Id)
                .WithRepositoryVersion<long>())

            .AddObject<OrderLine>(static o => o
                .AddProperty(static p => p.OrderId)
                .AddProperty(static p => p.LineNumber)
                .AddProperty(static p => p.Qty)
                .AddProperty(static p => p.UnitPrice)
                .AddProperty(static p => p.LineTotal)
                .AddKey("PK_OrderLine", static k => k
                    .AddPathFrom<Order>(static p => p.Id)
                    .AddPath(static p => p.LineNumber)))

            // Payment Objects
            .AddObject<Payment>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Method)
                .AddProperty(static p => p.Amount)
                .AddProperty(static p => p.CapturedAt)
                .AddKey("PK_Payment", static p => p.Id))

            // Relationships
            .AddOneToManyRelationship("REL_Customer_Order_1toN", static r => r
                .From<Customer>()
                .To<Order>(static de => de.WithForeignKey(static p => p.Customer.Id)))

            .AddOneToOneRelationship("REL_Customer_Profile_1to1", static r => r
                .From<Customer>()
                .To<CustomerProfile>())

            .AddOneToManyRelationship("REL_Order_OrderLine_1toN", static r => r
                .From<Order>()
                .To<OrderLine>(static de => de.WithForeignKeyFrom<Order>(static p => p.Id))
                .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete))

            .AddOneToOneRelationship("REL_Payment_Order_1to1", static r => r
                .From<Payment>()
                .To<Order>(static de => de.WithForeignKey(static p => p.Payment!.Id)))

            .AddOneToManyRelationship("REL_Category_Category_1toN", static r => r
                .From<Category>()
                .To<Category>(static de => de.WithForeignKey(static p => p.ParentId)))

            .AddOneToManyRelationship("REL_Category_DigitalProduct_1toN", static r => r
                .From<Category>()
                .To<DigitalProduct>(static de => de.WithForeignKey(static p => p.CategoryId)))

            .AddOneToManyRelationship("REL_Category_PhysicalProduct_1toN", static r => r
                .From<Category>("PK_Category")
                .To<PhysicalProduct>(static de => de.WithForeignKey(static p => p.CategoryId)))

            .AddManyToManyRelationship("REL_DigitalProduct_Tag_NtoN", static r => r
                .Between<DigitalProduct>()
                .And<Tag>()
                .WithAssociation<DigitalProductTag>(static a => a
                    .WithForeignKeyA(static p => p.DigitalProductId)
                    .WithForeignKeyB(static p => p.TagId)))

            .AddManyToManyRelationship("REL_PhysicalProduct_Tag_NtoN", static r => r
                .Between<PhysicalProduct>("PK_PhysicalProduct")
                .And<Tag>("PK_Tag")
                .WithAssociation<PhysicalProductTag>(static a => a
                    .WithForeignKeyA(static p => p.PhysicalProductId)
                    .WithForeignKeyB(static p => p.TagId)))

            // Query Sources
            .AddQuerySource("SearchCustomers", static s => s
                .AddInputParameter<string>("name")
                .AddInputParameter<OrderStatus>("status", static p => p
                    .AsRequired()
                    .AsNonNullable())
                .WithCollectionOutput<Customer>(
                    apiCollectionNullability: ApiNullability.NonNullable,
                    apiItemNullability: ApiNullability.NonNullable)
                .WithOutputComposition(ApiQueryComposition.Projection
                    | ApiQueryComposition.Filtering
                    | ApiQueryComposition.Ordering
                    | ApiQueryComposition.OffsetPagination))

            .AddQuerySource("GetCustomerById", static s => s
                .AddInputParameter<Ulid>("id", static p => p
                    .AsRequired()
                    .AsNonNullable())
                .WithOutput<Customer>()
                .WithOutputComposition(ApiQueryComposition.Projection))

            .AddQuerySource("CountOrders", static s => s
                .WithOutput<int>(ApiNullability.NonNullable)
                .WithOutputComposition(ApiQueryComposition.None))

            .AddQuerySource("GetOrderTotal", static s => s
                .AddInputParameter<Ulid>("orderId", static p => p
                    .AsRequired()
                    .AsNonNullable())
                .WithOutput<Money>(ApiNullability.NonNullable)
                .WithOutputComposition(ApiQueryComposition.None))

            .AddQuerySource("ListCategories", static s => s
                .WithCollectionOutput<Category>(
                    apiCollectionNullability: ApiNullability.NonNullable,
                    apiItemNullability: ApiNullability.NonNullable)
                .WithOutputComposition(ApiQueryComposition.Filtering | ApiQueryComposition.Ordering))

            .AddQuerySource("SearchOrders", static s => s
                .AddInputParameter<OrderStatus>("status")
                .AddInputParameter<decimal>("minimumTotal")
                .WithCollectionOutput<Order>(
                    apiCollectionNullability: ApiNullability.NonNullable,
                    apiItemNullability: ApiNullability.NonNullable)
                .WithOutputComposition(ApiQueryComposition.Projection
                    | ApiQueryComposition.Filtering
                    | ApiQueryComposition.Ordering
                    | ApiQueryComposition.OffsetPagination));

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
            .AddObject<KeyOneScalarPart>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name)
                .AddKey("PK_KeyOneScalarPart", static p => p.Id)
                .AddKey("AK_KeyOneScalarPart", static p => p.Name)
                .WithCanonicalKey("AK_KeyOneScalarPart"))

            .AddObject<KeyTwoScalarPartComposite>(static o => o
                .AddProperty(static p => p.Id1)
                .AddProperty(static p => p.Id2)
                .AddProperty(static p => p.Description)
                .AddKey("PK_KeyTwoScalarPartComposite", static p => p.Id1, static p => p.Id2))

            .AddObject<KeyThreeScalarPartComposite>(static o => o
                .AddProperty(static p => p.Id1)
                .AddProperty(static p => p.Id2)
                .AddProperty(static p => p.Id3)
                .AddProperty(static p => p.Description)
                .AddKey("PK_KeyThreeScalarPartComposite", static p => p.Id1, static p => p.Id2, static p => p.Id3))

            .AddObject<KeyNested>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Description)
                .AddKey("PK_KeyNestedPart", static p => p.Id)
                .WithRepositoryVersion<Guid>())

            .AddObject<KeyNestedComposite>(static o => o
                .AddProperty(static p => p.NestedPart)
                .AddProperty(static p => p.Name)
                .AddKey("PK_KeyNestedComposite", static p => p.NestedPart.Id, static p => p.Name))

            .AddObject<KeyOwner>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Description)
                .AddProperty(static p => p.Dependents)
                .AddProperty(static p => p.Dependent)
                .AddKey("PK_KeyOwner", static p => p.Id))

            .AddObject<KeyOwnedComposite>(static o => o
                .AddProperty(static p => p.LineNumber)
                .AddProperty(static p => p.Description)
                .AddKey("PK_KeyOwnedComposite", static k => k
                    .AddPathFrom<KeyOwner>(static p => p.Id)
                    .AddPath(static p => p.LineNumber)))

            .AddObject<KeyOwnedDependent>(static o => o
                .AddProperty(static p => p.Description)
                .AddKeyFrom<KeyOwner>("PK_KeyOwnedDependent", static p => p.Id));

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
            .AddEnum<Gender>(static e => e
                .AddValue(Gender.Unspecified)
                .AddValue(Gender.Male)
                .AddValue(Gender.Female))

            .AddEnum<StopLight>(static e => e
                .AddValue(StopLight.None)
                .AddValue(StopLight.Green)
                .AddValue(StopLight.Yellow)
                .AddValue(StopLight.Red))

            // Objects
            .AddObject<Empty>(static o => o
                .WithOptions(static opt => opt.ThrowOnNullKeyPart()))

            .AddObject<Point>(static o => o
                .WithOptions(static opt => opt.ThrowOnNullKeyPart())
                .AddProperty(static p => p.X)
                .AddProperty(static p => p.Y)
                .AddProperty(static p => p.Note)
                .WithVersion(nameof(Point.X)))

            .AddObject<ScalarsOnly>(static o => o
                .WithOptions(static opt => opt.ThrowOnNullKeyPart())
                .AddProperty(static p => p.RequiredName)
                .AddProperty(static p => p.RequiredNumber)
                .AddProperty(static p => p.RequiredPredicate)
                .AddProperty(static p => p.OptionalName)
                .AddProperty(static p => p.OptionalNumber)
                .AddProperty(static p => p.OptionalPredicate))

            .AddObject<Person>(static o => o
                .WithOptions(static opt => opt.ThrowOnNullKeyPart())
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.Age)
                .AddProperty(static p => p.Gender)
                .AddProperty(static p => p.Hobbies)
                .AddProperty(static p => p.CompanyId)
                .AddKey("PK_Person_Id", static p => p.Id)
                .AddKey("AK_Person_Name", static p => p.Name)
                .WithCanonicalKey("PK_Person_Id"))

            .AddObject<Company>(static o => o
                .WithOptions(static opt => opt.ThrowOnNullKeyPart())
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.Owner)
                .AddProperty(static p => p.Employees)
                .AddKey("PK_Company_Id", static p => p.Id)
                .AddKey("AK_Company_Name", static p => p.Name)
                .WithCanonicalKey("AK_Company_Name"));

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
            .AddObject<RelationshipUserRef>(static o => o
                .AddProperty(static p => p.UserId))

            .AddObject<RelationshipPostRef>(static o => o
                .AddProperty(static p => p.PostId))

            .AddObject<RelationshipCatalogKey>(static o => o
                .AddProperty(static p => p.Sku)
                .AddProperty(static p => p.Revision))

            // RelationshipUser
            .AddObject<RelationshipUser>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.UserName)
                .AddProperty(static p => p.Profile)
                .AddProperty(static p => p.Posts)
                .AddKey("PK_RelationshipUser", static p => p.Id))

            // RelationshipUserProfile
            .AddObject<RelationshipUserProfile>(static o => o
                .AddProperty(static p => p.UserId)
                .AddProperty(static p => p.UserRef)
                .AddProperty(static p => p.DisplayName)
                .AddProperty(static p => p.User)
                .AddKey("PK_RelationshipUserProfile", static p => p.UserId))

            // RelationshipPost
            .AddObject<RelationshipPost>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.AuthorUserId)
                .AddProperty(static p => p.AuthorUserRef)
                .AddProperty(static p => p.Title)
                .AddProperty(static p => p.Comments)
                .AddProperty(static p => p.Tags)
                .AddProperty(static p => p.User)
                .AddKey("PK_RelationshipPost", static p => p.Id))

            // RelationshipComment
            .AddObject<RelationshipComment>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.PostId)
                .AddProperty(static p => p.PostRef)
                .AddProperty(static p => p.Body)
                .AddProperty(static p => p.Post)
                .AddKey("PK_RelationshipComment", static p => p.Id))

            // RelationshipTag
            .AddObject<RelationshipTag>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.Posts)
                .AddKey("PK_RelationshipTag", static p => p.Id))

            // RelationshipPostTag
            .AddObject<RelationshipPostTag>(static o => o
                .AddProperty(static p => p.PostId)
                .AddProperty(static p => p.TagId)
                .AddKey("PK_RelationshipPostTag", static p => p.PostId, static p => p.TagId))

            // RelationshipCatalogItem
            .AddObject<RelationshipCatalogItem>(static o => o
                .AddProperty(static p => p.Sku)
                .AddProperty(static p => p.Revision)
                .AddProperty(static p => p.Name)
                .AddKey("PK_RelationshipCatalogItem", static p => p.Sku, static p => p.Revision)
                .WithVersion(nameof(RelationshipCatalogItem.Revision)))

            // RelationshipOrder
            .AddObject<RelationshipOrder>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Lines)
                .AddKey("PK_RelationshipOrder", static p => p.Id))

            // RelationshipOrderLine
            .AddObject<RelationshipOrderLine>(static o => o
                .AddProperty(static p => p.OrderId)
                .AddProperty(static p => p.LineNumber)
                .AddProperty(static p => p.ProductSku)
                .AddProperty(static p => p.ProductRevision)
                .AddProperty(static p => p.ProductKey)
                .AddKey("PK_RelationshipOrderLine", static p => p.OrderId, static p => p.LineNumber))

            // RelationshipOwnedLine
            .AddObject<RelationshipOwnedLine>(static o => o
                .AddProperty(static p => p.LineNumber)
                .AddProperty(static p => p.Notes)
                .AddKey("PK_RelationshipOwnedLine", static k => k
                    .AddPathFrom<RelationshipOrder>(static p => p.Id)
                    .AddPath(static p => p.LineNumber)))

            // RelationshipOrgUnit
            .AddObject<RelationshipOrgUnit>(static o => o
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.ParentId)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.Children)
                .AddKey("PK_RelationshipOrgUnit", static p => p.Id))

            // Relationships
            .AddOneToOneRelationship("REL_User_UserProfile_1to1ViaScalar", static r => r
                .From<RelationshipUser>()
                .To<RelationshipUserProfile>(static de => de.WithForeignKey(static p => p.UserId)))

            .AddOneToOneRelationship("REL_User_UserProfile_1to1ViaNested", static r => r
                .From<RelationshipUser>()
                .To<RelationshipUserProfile>(static de => de.WithForeignKey(static p => p.UserRef.UserId)))

            .AddOneToManyRelationship("REL_User_Post_1toN_ViaScalar", static r => r
                .From<RelationshipUser>()
                .To<RelationshipPost>(static de => de.WithForeignKey(static p => p.AuthorUserId)))

            .AddOneToManyRelationship("REL_User_Post_1toN_ViaNested", static r => r
                .From<RelationshipUser>()
                .To<RelationshipPost>(static de => de.WithForeignKey(static p => p.AuthorUserRef.UserId)))

            .AddOneToManyRelationship("REL_Post_Comment_1toN_ViaScalar", static r => r
                .From<RelationshipPost>()
                .To<RelationshipComment>(static de => de.WithForeignKey(static p => p.PostId)))

            .AddOneToManyRelationship("REL_Post_Comment_1toN_ViaNested", static r => r
                .From<RelationshipPost>()
                .To<RelationshipComment>(static de => de.WithForeignKey(static p => p.PostRef.PostId)))

            .AddManyToManyRelationship("REL_Post_Tag_NtoN_ViaPostTag", static r => r
                .Between<RelationshipPost>()
                .And<RelationshipTag>()
                .WithAssociation<RelationshipPostTag>(static a => a
                    .WithForeignKeyA(static p => p.PostId)
                    .WithForeignKeyB(static p => p.TagId)))

            .AddOneToManyRelationship("REL_CatalogItem_OrderLine_1toN_ViaScalarComposite", static r => r
                .From<RelationshipCatalogItem>()
                .To<RelationshipOrderLine>(static de => de.WithForeignKey(static p => p.ProductSku, static p => p.ProductRevision)))

            .AddOneToManyRelationship("REL_CatalogItem_OrderLine_1toN_ViaNestedComposite", static r => r
                .From<RelationshipCatalogItem>()
                .To<RelationshipOrderLine>(static de => de.WithForeignKey(static p => p.ProductKey.Sku, static p => p.ProductKey.Revision)))

            .AddOneToManyRelationship("REL_Order_OwnedLine_1toN_ViaOwnerKeyPath", static r => r
                .From<RelationshipOrder>()
                .To<RelationshipOwnedLine>(static de => de.WithForeignKeyFrom<RelationshipOrder>(static p => p.Id)))

            .AddOneToManyRelationship("REL_OrgUnit_OrgUnit_1toN", static r => r
                .From<RelationshipOrgUnit>()
                .To<RelationshipOrgUnit>(static de => de.WithForeignKey(static p => p.ParentId)));

        return builder;
    }
    #endregion
}
