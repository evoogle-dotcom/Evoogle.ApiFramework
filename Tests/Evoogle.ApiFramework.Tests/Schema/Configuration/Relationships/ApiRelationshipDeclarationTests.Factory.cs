// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Text.Json;
using Evoogle.ApiFramework.Schema.Configuration.Annotations;
using Evoogle.ApiFramework.Schema.Configuration.Conventions;

using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships;

[DynamicLinqType]
public static class ApiRelationshipDeclarationTestsFactory
{
    #region CLR Declarations
    public class Customer
    {
        #region Properties
        public int Id { get; set; }
        public List<Order> Orders { get; set; } = [];
        public List<Order> FavoriteOrders { get; set; } = [];
        public Order? PrimaryOrder { get; set; }
        public List<Tag> Tags { get; set; } = [];
        #endregion

        #region Fields
        public List<Order> OrderField = [];
        #endregion
    }
    public sealed class Order
    {
        #region Properties
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public Customer? OtherCustomer { get; set; }
        #endregion
    }
    public sealed class Tag
    {
        #region Properties
        public int Id { get; set; }
        public List<Customer> Customers { get; set; } = [];
        #endregion
    }
    public sealed class Link
    {
        #region Properties
        public int CustomerId { get; set; }
        public int TagId { get; set; }
        #endregion
    }
    public sealed class Employee
    {
        #region Properties
        public int Id { get; set; }
        public Employee? Manager { get; set; }
        public List<Employee> Reports { get; set; } = [];
        #endregion
    }
    public sealed class DerivedCustomer : Customer
    {
        #region Properties
        public int DerivedId { get; set; }
        #endregion
    }
    public sealed class HiddenCustomer : Customer
    {
        #region Properties
        public new List<Order> Orders { get; set; } = [];
        public int HiddenId { get; set; }
        #endregion
    }
    public sealed class AnnotatedCustomer
    {
        #region Properties
        public int Id { get; set; }
        [ApiRelationship(ApiName = "AnnotatedOrders", Kind = ApiRelationshipKind.OneToMany,
            InverseNavigation = nameof(AnnotatedOrder.Customer))]
        public List<AnnotatedOrder> Orders { get; set; } = [];
        #endregion
    }
    public sealed class AnnotatedOrder
    {
        #region Properties
        public int Id { get; set; }
        public AnnotatedCustomer? Customer { get; set; }
        #endregion
    }
    public sealed class InvalidInverseCustomer
    {
        #region Properties
        public int Id { get; set; }
        [ApiRelationship(ApiName = "Invalid", Kind = ApiRelationshipKind.OneToMany, InverseNavigation = "Missing")]
        public List<Order> Orders { get; set; } = [];
        #endregion
    }
    public sealed class BlankInverseCustomer
    {
        #region Properties
        public int Id { get; set; }
        [ApiRelationship(ApiName = "Invalid", Kind = ApiRelationshipKind.OneToMany, InverseNavigation = " ")]
        public List<Order> Orders { get; set; } = [];
        #endregion
    }
    public sealed class AnnotatedManyCustomer
    {
        #region Properties
        public int Id { get; set; }
        [ApiManyToManyRelationship(ApiName = "AnnotatedTags", AssociationType = typeof(AnnotatedLink),
            OtherPrincipalType = typeof(AnnotatedTag), ForeignKeyA = nameof(AnnotatedLink.CustomerId),
            ForeignKeyB = nameof(AnnotatedLink.TagId), InverseNavigation = nameof(AnnotatedTag.Customers))]
        public List<AnnotatedTag> Tags { get; set; } = [];
        #endregion
    }
    public sealed class AnnotatedTag
    {
        #region Properties
        public int Id { get; set; }
        [ApiManyToManyRelationship(ApiName = "AnnotatedTags", AssociationType = typeof(AnnotatedLink),
            OtherPrincipalType = typeof(AnnotatedManyCustomer), ForeignKeyA = nameof(AnnotatedLink.TagId),
            ForeignKeyB = nameof(AnnotatedLink.CustomerId), InverseNavigation = nameof(AnnotatedManyCustomer.Tags))]
        public List<AnnotatedManyCustomer> Customers { get; set; } = [];
        #endregion
    }
    public sealed class AnnotatedLink
    {
        #region Properties
        public int CustomerId { get; set; }
        public int TagId { get; set; }
        #endregion
    }
    [ApiRelationshipDefinition(ApiName = "First", PrincipalType = typeof(Customer), DependentType = typeof(Order),
        Kind = ApiRelationshipKind.OneToMany)]
    [ApiRelationshipDefinition(ApiName = "Second", PrincipalType = typeof(Customer), DependentType = typeof(Order),
        Kind = ApiRelationshipKind.OneToMany)]
    public sealed class DefinitionContainer
    {
        #region Properties
        public int Id { get; set; }
        #endregion
    }

    private sealed class IncompatibleConvention : IApiRelationshipConvention
    {
        #region Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Relationship;
        #endregion

        #region Convention Methods
        public void Apply(ApiSchemaBuilder builder)
            => builder.AddOneToOneRelationship<Customer, Order>
            ("Ignored", static customer => customer.Orders, null, static _ => throw new InvalidOperationException("Must be ignored"));
        #endregion
    }
    #endregion

    #region Factory Methods
    private static ApiSchemaBuilder CreateBuilder()
        => new ApiSchemaBuilder().WithName("Relationships").AddScalar<int>()
        .AddObject<Customer>(static customer => customer.WithName("Customer").AddProperty("Id", "Id")
            .AddKey("Id", static key => key.AddPath("Id")))
        .AddObject<Order>(static order => order.WithName("Order").AddProperty("Id", "Id").AddProperty("CustomerId",
            "CustomerId"))
        .AddObject<Tag>(static tag => tag.WithName("Tag").AddProperty("Id", "Id").AddKey("Id", static key => key.AddPath("Id")))
        .AddObject<Link>(static link => link.WithName("Link").AddProperty("CustomerId", "CustomerId").AddProperty(
            "TagId", "TagId"));
    #endregion

    #region Declaration Scenarios
    public static ApiSchemaCompilationResult BuildIndependentNames()
        => CreateBuilder()
        .AddOneToManyRelationship("First", static relationship => relationship.From<Customer>().To<Order>())
        .AddOneToManyRelationship("Second", static relationship => relationship.From<Customer>().To<Order>()).BuildResult();
    public static ApiSchemaCompilationResult BuildDuplicateNames()
        => CreateBuilder()
        .AddOneToManyRelationship("Duplicate", static relationship => relationship.From<Customer>().To<Order>())
        .AddOneToManyRelationship("Duplicate", static relationship => relationship.From<Customer>().To<Order>()).BuildResult();
    public static ApiSchemaCompilationResult BuildDuplicateNamesAcrossKinds()
        => CreateBuilder()
        .AddOneToOneRelationship("Duplicate", static relationship => relationship.From<Customer>().To<Order>())
        .AddOneToManyRelationship("Duplicate", static relationship => relationship.From<Customer>().To<Order>()).BuildResult();
    public static ApiSchemaCompilationResult BuildPairedMembers()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Orders", static customer => customer.Orders, static order => order
            .Customer).BuildResult();
    public static ApiSchemaCompilationResult BuildConfigureThroughInverse()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Orders", static customer => customer.Orders, static order => order.Customer)
        .ConfigureOneToManyRelationship<Order>(static order => order.Customer, static relationship => relationship.WithName(
            "Renamed"))
        .BuildResult();
    public static ApiSchemaCompilationResult BuildRepeatedMemberDeclarations()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("First", static customer => customer.Orders, null)
        .AddOneToManyRelationship<Customer, Order>("Second", static customer => customer.Orders, static order => order.Customer)
        .BuildResult();
    public static ApiSchemaCompilationResult BuildAttachInverse()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Orders", static customer => customer.Orders, null)
        .AssociateInverseNavigation<Customer, Order>(static customer => customer.Orders, static order => order.Customer)
            .BuildResult();
    public static ApiSchemaCompilationResult BuildDistinctMemberRelationships()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("First", static customer => customer.Orders, static order => order.Customer)
        .AddOneToManyRelationship<Customer, Order>("Second", static customer => customer.FavoriteOrders, static order =>
            order.OtherCustomer)
        .BuildResult();
    public static ApiSchemaCompilationResult BuildSplitLocatorConflict()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("First", static customer => customer.Orders, null)
        .AddOneToManyRelationship<Customer, Order>("Second", null, static order => order.Customer)
        .AddOneToManyRelationship<Customer, Order>("Combined", static customer => customer.Orders, static order => order.Customer)
        .BuildResult();
    public static ApiSchemaCompilationResult BuildClaimedInverseConflict()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("First", static customer => customer.Orders, null)
        .AddOneToManyRelationship<Customer, Order>("Second", null, static order => order.Customer)
        .AssociateInverseNavigation<Customer, Order>(static customer => customer.Orders, static order => order.Customer)
            .BuildResult();
    public static ApiSchemaCompilationResult BuildKindConflict()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Orders", static customer => customer.Orders, null)
        .AddOneToOneRelationship<Customer, Order>("Conflicting", static customer => customer.Orders, null).BuildResult();
    public static ApiSchemaCompilationResult BuildLowerKindIgnored()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Orders", static customer => customer.Orders, null)
        .UseConventions(static conventions => conventions.AddConvention(new IncompatibleConvention())).BuildResult();
    public static ApiSchemaCompilationResult BuildCardinalityConflict()
        => CreateBuilder()
        .AddOneToOneRelationship<Customer, Order>("Invalid", static customer => customer.Orders, null).BuildResult();
    public static ApiSchemaCompilationResult BuildTargetConflict()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Tag>("Invalid", static customer => customer.Orders, null).BuildResult();
    public static ApiSchemaCompilationResult BuildEndpointConflict()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Invalid", static customer => customer.Orders, null,
            static relationship => relationship.To<Tag>()).BuildResult();
    public static ApiSchemaCompilationResult BuildBindingConflict()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Invalid", static customer => customer.Orders, null,
            static relationship => relationship.From<Customer>(static end => end.WithTraversal("Wrong", nameof(Customer
                .FavoriteOrders))))
        .BuildResult();
    public static ApiSchemaCompilationResult BuildRenamedTraversal()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Orders", static customer => customer.Orders, null,
            static relationship => relationship.From<Customer>(static end => end.WithTraversal("Purchases")))
        .UseCamelCaseNaming(ApiNamingConventionTargets.RelationshipTraversal).BuildResult();
    public static ApiSchemaCompilationResult BuildConventionTraversal()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("ExactRelationship", static customer => customer.Orders, static order =>
            order.Customer)
        .UseCamelCaseNaming(ApiNamingConventionTargets.RelationshipTraversal).BuildResult();
    public static ApiSchemaCompilationResult BuildLaterBindingDoesNotLocate()
        => CreateBuilder()
        .AddOneToManyRelationship("First", static relationship => relationship.From<Customer>(static end =>
            end.WithTraversal("Orders", nameof(Customer.Orders))).To<Order>())
        .AddOneToManyRelationship<Customer, Order>("Second", static customer => customer.Orders, null).BuildResult();
    public static ApiSchemaCompilationResult BuildOneToOne()
        => CreateBuilder()
        .AddOneToOneRelationship<Customer, Order>("Primary", static customer => customer.PrimaryOrder, static order => order.Customer)
        .ConfigureOneToOneRelationship<Order>(static order => order.Customer, static relationship => relationship.WithName("Primary"))
        .BuildResult();
    public static ApiSchemaCompilationResult BuildManyToMany()
        => CreateBuilder()
        .AddManyToManyRelationship<Customer, Tag>("Tags", static customer => customer.Tags, static tag => tag.Customers,
            static relationship => relationship.WithAssociation<Link>(static association => association
                .WithForeignKeyA(static key => key.AddPath("CustomerId")).WithForeignKeyB(static key => key.AddPath("TagId"))))
        .ConfigureManyToManyRelationship<Tag>(static tag => tag.Customers, static relationship => relationship.WithName("Membership"))
        .BuildResult();
    public static ApiSchemaCompilationResult BuildFieldNavigation()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Orders", static customer => customer.OrderField, null).BuildResult();
    public static ApiSchemaCompilationResult BuildInheritedNavigation()
        => CreateBuilder()
        .AddObject<DerivedCustomer>(static customer => customer.WithName("Derived").AddProperty("DerivedId", "DerivedId"))
        .AddOneToManyRelationship<DerivedCustomer, Order>("Inherited", static customer => customer.Orders, null).BuildResult();
    public static ApiSchemaCompilationResult BuildHiddenNavigation()
        => CreateBuilder()
        .AddObject<HiddenCustomer>(static customer => customer.WithName("Hidden").AddProperty("HiddenId", "HiddenId"))
        .AddOneToManyRelationship<Customer, Order>("Base", static customer => customer.Orders, null)
        .AddOneToManyRelationship<HiddenCustomer, Order>("Hidden", static customer => customer.Orders, null).BuildResult();
    public static ApiSchemaCompilationResult BuildSelfReference()
        => CreateBuilder()
        .AddObject<Employee>(static employee => employee.WithName("Employee").AddProperty("Id", "Id"))
        .AddOneToManyRelationship<Employee, Employee>("Reports", static employee => employee.Reports, static employee =>
            employee.Manager)
        .BuildResult();
    public static ApiSchemaCompilationResult BuildAttributeInverse()
        => new ApiSchemaBuilder().WithName("Relationships")
        .AddScalar<int>().AddObject<AnnotatedCustomer>(static customer => customer.WithName("Customer").AddProperty(
            "Id", "Id"))
        .AddObject<AnnotatedOrder>(static order => order.WithName("Order").AddProperty("Id", "Id"))
        .UseDefaultAnnotations().BuildResult();
    public static ApiSchemaCompilationResult BuildAttributeExplicitConvergence()
        => new ApiSchemaBuilder()
        .WithName("Relationships")
        .AddScalar<int>().AddObject<AnnotatedCustomer>(static customer => customer.WithName("Customer").AddProperty(
            "Id", "Id"))
        .AddObject<AnnotatedOrder>(static order => order.WithName("Order").AddProperty("Id", "Id"))
        .AddOneToManyRelationship<AnnotatedCustomer, AnnotatedOrder>("Explicit", static customer => customer.Orders, null)
        .UseDefaultAnnotations().BuildResult();
    public static ApiSchemaCompilationResult BuildAttributeManyToMany()
        => new ApiSchemaBuilder().WithName(
        "Relationships")
        .AddScalar<int>()
        .AddObject<AnnotatedManyCustomer>(static customer => customer.WithName("Customer").AddProperty("Id", "Id")
            .AddKey("Id", static key => key.AddPath("Id")))
        .AddObject<AnnotatedTag>(static tag => tag.WithName("Tag").AddProperty("Id", "Id").AddKey("Id", static key => key
            .AddPath("Id")))
        .AddObject<AnnotatedLink>(static link => link.AddProperty("CustomerId", "CustomerId").AddProperty("TagId", "TagId"))
        .UseDefaultAnnotations().BuildResult();
    public static ApiSchemaCompilationResult BuildInvalidInverse()
        => CreateBuilder()
        .AddObject<InvalidInverseCustomer>(static customer => customer.AddProperty("Id", "Id"))
        .UseDefaultAnnotations().BuildResult();
    public static ApiSchemaCompilationResult BuildBlankInverse()
        => CreateBuilder()
        .AddObject<BlankInverseCustomer>(static customer => customer.AddProperty("Id", "Id"))
        .UseDefaultAnnotations().BuildResult();
    public static ApiSchemaCompilationResult BuildTypeDefinitions()
        => CreateBuilder()
        .AddObject<DefinitionContainer>(static container => container.AddProperty("Id", "Id"))
        .UseDefaultAnnotations().BuildResult();
    public static ApiSchemaCompilationResult BuildUnresolvedMemberConfiguration()
        => CreateBuilder()
        .ConfigureOneToManyRelationship<Customer>(static customer => customer.Orders, static relationship => relationship
            .WithName("Missing"))
        .BuildResult();
    #endregion

    #region Handle Observations
    public static string ObserveHandleAcrossBuilds()
    {
        var builder = CreateBuilder();
        var registration = builder.DeclareOneToManyRelationship("First", static relationship => relationship
            .From<Customer>().To<Order>());
        var first = builder.Build().ApiRelationships.Single();
        builder.ConfigureRelationship(registration, static relationship => relationship.WithName("Second"));
        var second = builder.Build().ApiRelationships.Single();
        return $"{first.ApiName}|{second.ApiName}|{ReferenceEquals(first, second)}";
    }
    public static string ConfigureForeignOneToOne()
    {
        var handle = CreateBuilder().DeclareOneToOneRelationship("Source", static relationship => relationship
            .From<Customer>().To<Order>());
        CreateBuilder().ConfigureRelationship(handle, static relationship => relationship.WithName("Other"));
        return "Unexpected completion";
    }
    public static string ConfigureForeignOneToMany()
    {
        var handle = CreateBuilder().DeclareOneToManyRelationship("Source", static relationship => relationship
            .From<Customer>().To<Order>());
        CreateBuilder().ConfigureRelationship(handle, static relationship => relationship.WithName("Other"));
        return "Unexpected completion";
    }
    public static string ConfigureForeignManyToMany()
    {
        var handle = CreateBuilder().DeclareManyToManyRelationship("Source", static relationship => relationship
            .Between<Customer>().And<Tag>());
        CreateBuilder().ConfigureRelationship(handle, static relationship => relationship.WithName("Other"));
        return "Unexpected completion";
    }
    public static string SerializeOneToOne()
        => JsonSerializer.Serialize(CreateBuilder().DeclareOneToOneRelationship
        ("Source", static relationship => relationship.From<Customer>().To<Order>()));
    public static string SerializeOneToMany()
        => JsonSerializer.Serialize(CreateBuilder().DeclareOneToManyRelationship
        ("Source", static relationship => relationship.From<Customer>().To<Order>()));
    public static string SerializeManyToMany()
        => JsonSerializer.Serialize(CreateBuilder().DeclareManyToManyRelationship
        ("Source", static relationship => relationship.Between<Customer>().And<Tag>()));
    public static string DeserializeOneToOne()
        => JsonSerializer
        .Deserialize<ApiRelationshipOneToOneRegistration>("{}")!.ToString()!;
    public static string DeserializeOneToMany()
        => JsonSerializer
        .Deserialize<ApiRelationshipOneToManyRegistration>("{}")!.ToString()!;
    public static string DeserializeManyToMany()
        => JsonSerializer
        .Deserialize<ApiRelationshipManyToManyRegistration>("{}")!.ToString()!;
    public static string RejectNestedSelector()
    {
        CreateBuilder().AddOneToManyRelationship<Customer, Order>("Nested", static customer => customer.PrimaryOrder!
            .Customer, null);
        return "Unexpected completion";
    }
    #endregion

    #region Additional Regression Scenarios
    public static string ObserveOneToOneHandleAcrossBuilds()
    {
        var builder = CreateBuilder();
        var registration = builder.DeclareOneToOneRelationship("First", static relationship => relationship
            .From<Customer>().To<Order>());
        var first = builder.Build().ApiRelationships.Single();
        builder.ConfigureRelationship(registration, static relationship => relationship.WithName("Second"));
        return $"{first.ApiName}|{builder.Build().ApiRelationships.Single().ApiName}";
    }

    public static string ObserveManyToManyHandleAcrossBuilds()
    {
        var builder = CreateBuilder();
        var registration = builder.DeclareManyToManyRelationship("First", static relationship => relationship
            .Between<Customer>().And<Tag>()
            .WithAssociation<Link>(static association => association.WithForeignKeyA(static key => key.AddPath("CustomerId"))
                .WithForeignKeyB(static key => key.AddPath("TagId"))));
        var first = builder.Build().ApiRelationships.Single();
        builder.ConfigureRelationship(registration, static relationship => relationship.WithName("Second"));
        return $"{first.ApiName}|{builder.Build().ApiRelationships.Single().ApiName}";
    }

    public static ApiSchemaCompilationResult BuildSameMemberBothRoles()
        => CreateBuilder()
        .AddManyToManyRelationship<Customer, Customer>("Invalid", static customer => customer.Orders, static customer =>
            customer.Orders)
        .BuildResult();

    public static ApiSchemaCompilationResult BuildOccupiedEndpointConflict()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("First", static customer => customer.Orders, static order => order.Customer)
        .AddOneToManyRelationship<Customer, Order>("Conflicting", static customer => customer.FavoriteOrders,
            static order => order.Customer)
        .BuildResult();

    public static ApiSchemaCompilationResult BuildReversedManyToMany()
        => CreateBuilder()
        .AddManyToManyRelationship<Customer, Tag>("First", static customer => customer.Tags, static tag => tag.Customers,
            static relationship => relationship.WithAssociation<Link>(static association => association
                .WithForeignKeyA(static key => key.AddPath("CustomerId")).WithForeignKeyB(static key => key.AddPath("TagId"))))
        .AddManyToManyRelationship<Tag, Customer>("Second", static tag => tag.Customers, static customer => customer.Tags)
        .BuildResult();
    #endregion

    #region Test Types
    private sealed class IndependentTypeReader(string apiName) : IApiRelationshipAnnotationReader
    {
        #region Annotation Reader Methods
        public IReadOnlyList<ApiOneToManyRelationshipAnnotationResult> ReadOneToManyRelationships(Type clrType)
            => clrType == typeof(Customer) ? [new(apiName, typeof(Customer), typeof(Order), null,
                ApiRelationshipDeleteBehavior.None)] : [];
        public IReadOnlyList<ApiOneToOneRelationshipAnnotationResult> ReadOneToOneRelationships(Type clrType)
            => [];
        public IReadOnlyList<ApiManyToManyRelationshipAnnotationResult> ReadManyToManyRelationships(Type clrType)
            => [];
        #endregion
    }
    #endregion

    #region Additional Regression Scenarios
    public static ApiSchemaCompilationResult BuildIndependentAnnotationReaders()
        => CreateBuilder()
        .UseAnnotations(static readers => readers.AddReader(new IndependentTypeReader("First"))
            .AddReader(new IndependentTypeReader("Second"))).BuildResult();

    public static ApiSchemaCompilationResult BuildRepeatedAttributeOccurrences()
        => CreateBuilder()
        .AddObject<DefinitionContainer>(static container => container.AddProperty("Id", "Id"))
        .UseAnnotations(static readers => readers.AddReader(new ApiAttributeAnnotationReader())
            .AddReader(new ApiAttributeAnnotationReader())).BuildResult();
    #endregion
}
