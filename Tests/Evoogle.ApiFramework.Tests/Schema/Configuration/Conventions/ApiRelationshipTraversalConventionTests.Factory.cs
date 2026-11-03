// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

using System.Linq.Dynamic.Core.CustomTypeProviders;

using Customer =
    Evoogle.ApiFramework.Schema.Configuration.Relationships.ApiRelationshipDeclarationTestsFactory.Customer;
using Order = Evoogle.ApiFramework.Schema.Configuration.Relationships.ApiRelationshipDeclarationTestsFactory.Order;
using Tag = Evoogle.ApiFramework.Schema.Configuration.Relationships.ApiRelationshipDeclarationTestsFactory.Tag;
using Link = Evoogle.ApiFramework.Schema.Configuration.Relationships.ApiRelationshipDeclarationTestsFactory.Link;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

[DynamicLinqType]
public static class ApiRelationshipTraversalConventionTestsFactory
{
    #region Test Types
    private sealed class PrefixTraversalConvention
    (
        string prefix,
        ApiConventionPhase phase = ApiConventionPhase.Configuration
    ) : IApiRelationshipTraversalConvention
    {
        #region IApiConvention Properties
        public ApiConventionPhase Phase => phase;
        #endregion

        #region IApiRelationshipTraversalConvention Methods
        public void Apply(ApiRelationshipTraversalBuilder builder, ApiRelationshipTraversalConventionContext context)
            => builder.WithName(prefix + builder.ApiName);
        #endregion
    }

    private sealed class InferRelationshipConvention : IApiRelationshipConvention
    {
        #region IApiConvention Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Relationship;
        #endregion

        #region IApiRelationshipConvention Methods
        public void Apply(ApiSchemaBuilder builder)
            => builder.AddOneToManyRelationship<Customer, Order>
            ("Inferred", static customer => customer.Orders, static order => order.Customer);
        #endregion
    }

    private sealed class CaptureTraversalConvention(List<string> observations) : IApiRelationshipTraversalConvention
    {
        #region IApiConvention Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
        #endregion

        #region IApiRelationshipTraversalConvention Methods
        public void Apply(ApiRelationshipTraversalBuilder builder, ApiRelationshipTraversalConventionContext context)
        {
            if (context.ApiObjectTypeReference.ClrType != context.ClrDeclaringType ||
                context.ApiRelationshipBuilder is null || context.ApiSchemaBuilder is null ||
                builder.ClrNavigationMember?.ClrName != context.ClrMemberInfo?.Name)
            {
                throw new InvalidOperationException("The traversal convention received inconsistent metadata.");
            }
            observations.Add($"{context.ClrDeclaringType?.Name}.{context.ClrMemberInfo?.Name}:" +
                $"{context.ClrMemberInfo?.MemberType}");
        }
        #endregion
    }

    private sealed class CaptureNamingConvention(List<string> observations)
        : ApiNamingConvention(ApiNamingConventionTargets.RelationshipTraversal)
    {
        #region IApiConvention Properties
        public override ApiConventionPhase Phase => ApiConventionPhase.Configuration;
        #endregion

        #region IApiNamingConvention Methods
        public override string ConvertName(string apiName, ApiNamingConventionContext context)
        {
            var traversalContext = context.ApiRelationshipTraversalConventionContext
                ?? throw new InvalidOperationException("Missing relationship traversal naming context.");
            observations.Add($"{context.Target}:{context.ClrType?.Name ?? "null"}:" +
                $"{context.ClrName ?? "null"}:{traversalContext.ApiObjectTypeReference.ApiName ?? "null"}");
            return apiName.ToLowerInvariant();
        }
        #endregion
    }

    private sealed class RetainTraversalConvention(Action<ApiRelationshipTraversalBuilder> retain)
        : IApiRelationshipTraversalConvention
    {
        #region IApiConvention Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
        #endregion

        #region IApiRelationshipTraversalConvention Methods
        public void Apply(ApiRelationshipTraversalBuilder builder, ApiRelationshipTraversalConventionContext context)
            => retain(builder);
        #endregion
    }

    private sealed class FailOnceTraversalConvention : IApiRelationshipTraversalConvention
    {
        #region Fields
        private bool _hasFailed;
        #endregion

        #region IApiConvention Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
        #endregion

        #region IApiRelationshipTraversalConvention Methods
        public void Apply(ApiRelationshipTraversalBuilder builder, ApiRelationshipTraversalConventionContext context)
        {
            if (!_hasFailed)
            {
                _hasFailed = true;
                throw new InvalidOperationException("Traversal convention failed after an earlier naming convention.");
            }
            builder.WithName("Recovered" + builder.ApiName);
        }
        #endregion
    }

    private sealed class MutateConventionSetConvention(Action mutate) : IApiSchemaConvention
    {
        #region IApiConvention Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Discovery;
        #endregion

        #region IApiSchemaConvention Methods
        public void Apply(ApiSchemaBuilder builder)
            => mutate();
        #endregion
    }

    private sealed class ApiOnlyRelationshipConvention : IApiRelationshipConvention
    {
        #region IApiConvention Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Relationship;
        #endregion

        #region IApiRelationshipConvention Methods
        public void Apply(ApiSchemaBuilder builder)
            => builder.AddOneToManyRelationship
        (
            "Unbound",
            static relationship => relationship
                .From(new ApiTypeReference(ApiTypeKind.Object, "Customer"), static end => end.WithTraversal("Orders"))
                .To(new ApiTypeReference(ApiTypeKind.Object, "Order"))
        );
        #endregion
    }
    #endregion

    #region Factory Methods
    private static ApiSchemaBuilder CreateBuilder()
        => new ApiSchemaBuilder().WithName("TraversalConventions")
        .AddScalar<int>()
        .AddObject<Customer>(static customer => customer.WithName("Customer").AddProperty("Id", "Id")
            .AddKey("Id", static key => key.AddPath("Id")))
        .AddObject<Order>(static order => order.WithName("Order").AddProperty("Id", "Id"))
        .AddObject<Tag>(static tag => tag.WithName("Tag").AddProperty("Id", "Id"))
        .AddObject<Link>(static link => link.WithName("Link"));

    private static ApiSchemaBuilder CreateOrdersBuilder()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>("Orders", static customer => customer.Orders, static order => order.Customer);

    private static string ReadCustomerTraversal(ApiSchema schema)
        => schema.ApiObjectTypes
        .Single(static type => type.ClrType == typeof(Customer)).ApiRelationshipTraversals.Single().ApiName;
    #endregion

    #region Build Scenarios
    public static ApiSchemaCompilationResult BuildCustomTraversalConvention()
        => CreateOrdersBuilder()
        .UseConventions(static conventions => conventions.AddConvention(new PrefixTraversalConvention("Custom")))
        .BuildResult();

    public static ApiSchemaCompilationResult BuildOrderedConventions()
        => CreateOrdersBuilder()
        .UseConventions(static conventions => conventions
            .AddConvention(new PrefixTraversalConvention("First"))
            .AddConvention(new PrefixTraversalConvention("Second")))
        .BuildResult();

    public static ApiSchemaCompilationResult BuildAfterRelationshipInference()
        => CreateBuilder()
        .UseConventions(static conventions => conventions.AddConvention(new InferRelationshipConvention()))
        .UseCamelCaseNaming(ApiNamingConventionTargets.RelationshipTraversal)
        .BuildResult();

    public static ApiSchemaCompilationResult BuildExplicitTraversalNames()
        => CreateBuilder()
        .AddOneToManyRelationship<Customer, Order>
        (
            "Explicit",
            static customer => customer.Orders,
            static order => order.Customer,
            static relationship => relationship
                .From<Customer>(static end => end.WithTraversal("Purchases"))
                .To<Order>(static end => end.WithTraversal("Buyer"))
        )
        .UseConventions(static conventions => conventions.AddConvention(new PrefixTraversalConvention("Ignored")))
        .UseCamelCaseNaming(ApiNamingConventionTargets.RelationshipTraversal)
        .BuildResult();

    public static ApiSchemaCompilationResult BuildOtherNamingTarget()
        => CreateOrdersBuilder()
        .UseCamelCaseNaming(ApiNamingConventionTargets.QuerySource)
        .BuildResult();

    public static ApiSchemaCompilationResult BuildIndependentConventionList()
    {
        var builder = CreateOrdersBuilder();
        var conventions = new ApiConventionSet();
        conventions.RelationshipTraversalConventions.Add
            (new ApiNamingCamelCaseConvention(ApiNamingConventionTargets.RelationshipTraversal));
        new ApiSchemaConfigurationPipeline(conventions, null, builder.Context, builder).Run();
        return builder.BuildResult();
    }

    public static ApiSchemaCompilationResult BuildAnnotatedNavigations()
        => new ApiSchemaBuilder()
        .WithName("AnnotatedTraversals").AddScalar<int>()
        .AddObject<ApiRelationshipDeclarationTestsFactory.AnnotatedCustomer>(static customer =>
            customer.WithName("Customer").AddProperty("Id", "Id"))
        .AddObject<ApiRelationshipDeclarationTestsFactory.AnnotatedOrder>(static order =>
            order.WithName("Order").AddProperty("Id", "Id"))
        .UseDefaultAnnotations()
        .UseCamelCaseNaming(ApiNamingConventionTargets.RelationshipTraversal)
        .BuildResult();

    public static ApiSchemaCompilationResult BuildWithoutClrNavigation()
        => CreateBuilder()
        .UseConventions(static conventions => conventions.AddConvention(new ApiOnlyRelationshipConvention()))
        .UseCamelCaseNaming(ApiNamingConventionTargets.RelationshipTraversal)
        .BuildResult();
    #endregion

    #region Observation Scenarios
    public static string ObserveConventionRegistration()
    {
        var convention = new PrefixTraversalConvention("Custom");
        var registered = new ApiConventionSetBuilder().AddConvention(convention).Build();
        var copied = new ApiConventionSetBuilder(registered).Build();
        var removed = new ApiConventionSetBuilder(copied).RemoveConvention<PrefixTraversalConvention>().Build();
        var naming = new ApiNamingCamelCaseConvention();
        var defaults = ApiConventionSet.CreateDefault(naming);
        var defaultCopy = new ApiConventionSetBuilder(defaults).Build();
        var defaultRemoved = new ApiConventionSetBuilder(defaultCopy)
            .RemoveConvention<ApiNamingCamelCaseConvention>().Build();
        return $"{ReferenceEquals(registered.RelationshipTraversalConventions.Single(), convention)}|" +
            $"{ReferenceEquals(copied.RelationshipTraversalConventions.Single(), convention)}|" +
            $"{removed.RelationshipTraversalConventions.Count}|{registered.QuerySourceConventions.Count}|" +
            $"{ReferenceEquals(defaults.RelationshipTraversalConventions.Single(), naming)}|" +
            $"{ReferenceEquals(defaultCopy.RelationshipTraversalConventions.Single(), naming)}|" +
            $"{defaultRemoved.RelationshipTraversalConventions.Count}";
    }

    public static string ObserveAllNavigationContexts()
    {
        var observations = new List<string>();
        CreateBuilder()
            .AddOneToOneRelationship<Customer, Order>("Primary", static customer => customer.PrimaryOrder,
                static order => order.OtherCustomer)
            .AddOneToManyRelationship<Customer, Order>("Orders", static customer => customer.OrderField,
                static order => order.Customer)
            .AddManyToManyRelationship<Customer, Tag>("Tags", static customer => customer.Tags, static tag => tag.Customers,
                static relationship => relationship.WithAssociation<Link>())
            .UseConventions(conventions => conventions.AddConvention(new CaptureTraversalConvention(observations)))
            .Build();
        return string.Join("|", observations.Order(StringComparer.Ordinal));
    }

    public static string ObserveNamingContext()
    {
        var observations = new List<string>();
        CreateOrdersBuilder().UseConventions(conventions =>
            conventions.AddConvention(new CaptureNamingConvention(observations))).Build();
        return string.Join("|", observations);
    }

    public static string ObserveUnboundNamingContext()
    {
        var observations = new List<string>();
        CreateBuilder().UseConventions(conventions => conventions
            .AddConvention(new ApiOnlyRelationshipConvention())
            .AddConvention(new CaptureNamingConvention(observations))).Build();
        return string.Join("|", observations);
    }

    public static string ObserveRepeatedBuilds()
    {
        var builder = CreateOrdersBuilder().UseConventions(static conventions =>
            conventions.AddConvention(new PrefixTraversalConvention("Named")));
        var first = builder.Build();
        var second = builder.Build();
        builder.UseConventions(static conventions => conventions.RemoveConvention<PrefixTraversalConvention>());
        var third = builder.Build();
        var firstTraversal = first.ApiObjectTypes.Single(static type => type.ClrType == typeof(Customer))
            .ApiRelationshipTraversals.Single();
        var secondTraversal = second.ApiObjectTypes.Single(static type => type.ClrType == typeof(Customer))
            .ApiRelationshipTraversals.Single();
        return $"{ReadCustomerTraversal(first)}|{ReadCustomerTraversal(second)}|{ReadCustomerTraversal(third)}|" +
            $"{ReferenceEquals(firstTraversal, secondTraversal)}|{ReadCustomerTraversal(first)}";
    }

    public static string ObserveRetainedPrincipalTraversal()
        => ObserveRetainedTraversal(true);

    public static string ObserveRetainedDependentTraversal()
        => ObserveRetainedTraversal(false);

    private static string ObserveRetainedTraversal(bool isPrincipal)
    {
        ApiRelationshipTraversalBuilder? retainedTraversal = null;
        var builder = CreateBuilder().AddOneToManyRelationship("Retained", relationship =>
        {
            if (isPrincipal)
            {
                relationship.From<Customer>(static end => end.WithTraversal("Original", nameof(Customer.Orders)))
                    .To<Order>();
            }
            else
            {
                relationship.From<Customer>()
                    .To<Order>(static end => end.WithTraversal("Original", nameof(Order.Customer)));
            }
        }).UseConventions(conventions => conventions.AddConvention
            (new RetainTraversalConvention(traversal => retainedTraversal = traversal)));
        var first = builder.Build();
        retainedTraversal!.WithName("ChangedAfterPublication");
        builder.UseConventions(static conventions => conventions.RemoveConvention<RetainTraversalConvention>());
        var second = builder.Build();
        return $"{first.ApiObjectTypes.SelectMany(static type => type.ApiRelationshipTraversals).Single().ApiName}|" +
            $"{second.ApiObjectTypes.SelectMany(static type => type.ApiRelationshipTraversals).Single().ApiName}";
    }

    public static string ObserveFailureRecovery()
    {
        var builder = CreateOrdersBuilder().UseConventions(static conventions => conventions
            .AddConvention(new PrefixTraversalConvention("First"))
            .AddConvention(new FailOnceTraversalConvention()));
        var hasThrown = false;
        try
        {
            builder.BuildResult();
        }
        catch (InvalidOperationException)
        {
            hasThrown = true;
        }
        return $"{hasThrown}|{ReadCustomerTraversal(builder.Build())}";
    }

    public static string ObserveScheduleSnapshot()
    {
        var conventions = new ApiConventionSet();
        conventions.RelationshipTraversalConventions.Add(new PrefixTraversalConvention("First"));
        conventions.DiscoveryConventions.Add(new MutateConventionSetConvention(() =>
            conventions.RelationshipTraversalConventions.Add(new PrefixTraversalConvention("Second"))));
        var firstBuilder = CreateOrdersBuilder();
        new ApiSchemaConfigurationPipeline(conventions, null, firstBuilder.Context, firstBuilder).Run();
        conventions.DiscoveryConventions.Clear();
        var secondBuilder = CreateOrdersBuilder();
        new ApiSchemaConfigurationPipeline(conventions, null, secondBuilder.Context, secondBuilder).Run();
        return $"{ReadCustomerTraversal(firstBuilder.Build())}|{ReadCustomerTraversal(secondBuilder.Build())}";
    }

    public static string ObserveInvalidDiscoveryPhase()
        => ObserveInvalidPhase(ApiConventionPhase.Discovery);

    public static string ObserveInvalidRelationshipPhase()
        => ObserveInvalidPhase(ApiConventionPhase.Relationship);

    private static string ObserveInvalidPhase(ApiConventionPhase phase)
    {
        CreateOrdersBuilder().UseConventions(conventions =>
            conventions.AddConvention(new PrefixTraversalConvention("Invalid", phase))).Build();
        return "Unexpected successful build";
    }
    #endregion
}
