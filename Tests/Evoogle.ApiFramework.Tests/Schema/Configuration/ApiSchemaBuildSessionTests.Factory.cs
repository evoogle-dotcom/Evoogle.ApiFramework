// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Annotations;
using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.Reflection;
using Evoogle.Extension;

using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace Evoogle.ApiFramework.Schema.Configuration;

[DynamicLinqType]
public static class ApiSchemaBuildSessionTestsFactory
{
    #region CLR Types
    public sealed class Customer
    {
        #region Properties
        public int Id { get; set; }
        public int Revision { get; set; }
        #endregion
    }
    public sealed class Transient
    {
        #region Properties
        public int Id { get; set; }
        #endregion
    }
    public enum LifecycleState
    {
        #region Values
        Pending
        #endregion
    }

    public sealed class NestedConfiguration : IApiObjectTypeConfiguration<Customer>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<Customer> builder)
            => builder
            .AddKey("Key", static key => key.AddPath("Revision")).WithVersion("Id");
        #endregion
    }

    private sealed class PrefixConvention(string prefix) :
        IApiObjectTypeConvention, IApiScalarTypeConvention, IApiEnumTypeConvention,
        IApiEnumValueConvention, IApiPropertyConvention
    {
        #region Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
        #endregion

        #region Convention Methods
        public void Apply(ApiObjectTypeBuilder builder)
            => builder.SetApiNameConvention(prefix + builder.ApiName);
        public void Apply(ApiScalarTypeBuilder builder)
            => builder.SetApiNameConvention(prefix + builder.ApiName);
        public void Apply(ApiEnumTypeBuilder builder)
            => builder.SetApiNameConvention(prefix + builder.ApiName);
        public void Apply(ApiEnumValueBuilder builder, ApiEnumValueConventionContext context)
            => builder.SetApiNameConvention(prefix + builder.ApiName);
        public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
            => builder.SetApiNameConvention(prefix + builder.ApiName);
        #endregion
    }

    private sealed class MutableAnnotationReader : IApiTypeAnnotationReader, IApiPropertyAnnotationReader,
        IApiEnumValueAnnotationReader, IApiKeyAnnotationReader, IApiVersionAnnotationReader
    {
        #region Properties
        public string Prefix { get; set; } = "First";
        public bool IsEnabled { get; set; } = true;
        #endregion

        #region Annotation Reader Methods
        public IReadOnlyList<ApiTypeAnnotationResult> ReadObjectTypeAnnotations(Type clrType)
            => this.IsEnabled ? [new(this.Prefix + "Customer")] : [];
        public IReadOnlyList<ApiTypeAnnotationResult> ReadScalarTypeAnnotations(Type clrType)
            => this.IsEnabled ? [new(this.Prefix + "Int32")] : [];
        public IReadOnlyList<ApiTypeAnnotationResult> ReadEnumTypeAnnotations(Type clrType)
            => this.IsEnabled ? [new(this.Prefix + "State")] : [];
        public IReadOnlyList<ApiPropertyAnnotationResult> ReadPropertyAnnotations
            (MemberInfo clrMember, ClrMemberKind clrMemberKind, MemberNullableInfo clrNullabilityInfo)
            => this.IsEnabled ? [new(this.Prefix + clrMember.Name, null)] : [];
        public IReadOnlyList<ApiEnumValueAnnotationResult> ReadEnumValueAnnotations(FieldInfo clrField)
            => this.IsEnabled ? [new(this.Prefix + clrField.Name)] : [];
        public IReadOnlyList<ApiKeyAnnotationResult> ReadKeyAnnotations(Type clrType)
            => this.IsEnabled ? [new(this.Prefix + "Key", 0, clrType, ["Id"])] : [];
        public IReadOnlyList<ApiVersionAnnotationResult> ReadVersionAnnotations(Type clrType)
            => this.IsEnabled ? [new(this.Prefix == "First" ? "Id" : "Revision")] : [];
        #endregion
    }

    private sealed class FailOnceDiscovery : IApiSchemaConvention
    {
        #region Properties
        public bool HasFailed
        {
            get; private set;
        }
        public ApiConventionPhase Phase => ApiConventionPhase.Discovery;
        #endregion

        #region Convention Methods
        public void Apply(ApiSchemaBuilder builder)
        {
            if (this.HasFailed)
            {
                return;
            }
            this.HasFailed = true;
            builder.AddObject<Transient>(static transient => transient.AddProperty("Id", "Id"));
            builder.AddQuerySource("Transient", static source => source.WithOutput<int>());
            throw new InvalidOperationException("Discovery failed after adding declarations.");
        }
        #endregion
    }

    private sealed class ConditionalDiscovery(Func<bool> isEnabled) : IApiSchemaConvention
    {
        #region Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Discovery;
        #endregion

        #region Convention Methods
        public void Apply(ApiSchemaBuilder builder)
        {
            if (isEnabled())
            {
                builder.AddQuerySource("Transient", static source => source.WithOutput<int>());
            }
        }
        #endregion
    }

    private sealed class LabelExtension(int label) : IApiSchemaExtension
    {
        #region Properties
        public int Label { get; } = label;
        #endregion

        #region IApiSchemaExtension Methods
        public IApiSchemaExtension CreateFrozenSnapshot()
            => new LabelExtension(this.Label);
        #endregion
    }

    private sealed class RecoveringExtension : IApiSchemaExtension
    {
        #region Properties
        public bool IsValid { get; set; }
        #endregion

        #region IApiSchemaExtension Methods
        public IApiSchemaExtension CreateFrozenSnapshot()
            => this
            .IsValid ? new RecoveringExtension { IsValid = true } : this;
        #endregion
    }

    private sealed class RelationshipExtensionConvention(
        ApiRelationshipOneToManyRegistration registration) : IApiRelationshipConvention
    {
        #region Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Relationship;
        #endregion

        #region Convention Methods
        public void Apply(ApiSchemaBuilder builder)
            => builder.ConfigureRelationship(registration, static relationship =>
            relationship.From<Customer>(static end => end.AddRelationshipPrincipalEndExtension(
                typeof(LabelExtension), new LabelExtension(2))));
        #endregion
    }
    #endregion

    #region Factory Methods
    private static ApiSchemaBuilder CreateBuilder()
        => new ApiSchemaBuilder().WithName("Sessions").AddScalar<int>()
        .AddObject<Customer>(static customer => customer.AddProperty("Id", "Id").AddProperty("Revision", "Revision"));
    #endregion

    #region Convention and Annotation Scenarios
    public static string ObserveConventionChanges()
    {
        var builder = new ApiSchemaBuilder().WithName("Sessions").AddScalar<int>().AddEnum<LifecycleState>()
            .AddObject<Customer>().UseDefaultConventions()
            .UseConventions(static conventions => conventions.AddConvention(new PrefixConvention("First")));
        var first = builder.Build();
        builder.UseDefaultConventions().UseConventions(static conventions => conventions.AddConvention(
            new PrefixConvention("Second")));
        var second = builder.Build();
        return $"{first.ApiScalarTypes.Single().ApiName}|{second.ApiScalarTypes.Single().ApiName}|" +
            $"{first.ApiEnumTypes.Single().ApiEnumValues.Single().ApiName}|" +
                $"{second.ApiEnumTypes.Single().ApiEnumValues.Single().ApiName}|" +
            $"{first.ApiObjectTypes.Single().ApiName}|{second.ApiObjectTypes.Single().ApiName}|" +
            $"{first.ApiObjectTypes.Single().ApiProperties[0].ApiName}" +
                $"|{second.ApiObjectTypes.Single().ApiProperties[0].ApiName}";
    }

    public static string ObserveAnnotationChanges()
    {
        var reader = new MutableAnnotationReader();
        var builder = new ApiSchemaBuilder().WithName("Sessions").AddScalar<int>().AddEnum<LifecycleState>()
            .AddObject<Customer>()
            .UseDefaultConventions().UseAnnotations(readers => readers.AddReader(reader));
        var first = builder.Build();
        reader.Prefix = "Second";
        var second = builder.Build();
        var firstKeys = string.Join(",", first.ApiObjectTypes.Single().ApiKeys.Select(static key => key.ApiName)
            .Order(StringComparer.Ordinal));
        var secondKeys = string.Join(",", second.ApiObjectTypes.Single().ApiKeys.Select(static key => key.ApiName)
            .Order(StringComparer.Ordinal));
        return $"{first.ApiScalarTypes.Single().ApiName}|{second.ApiScalarTypes.Single().ApiName}|" +
            $"{first.ApiEnumTypes.Single().ApiEnumValues.Single().ApiName}|" +
                $"{second.ApiEnumTypes.Single().ApiEnumValues.Single().ApiName}|" +
            $"{firstKeys}" +
                $"|" +
                $"{secondKeys}" +
                    $"|" +
            $"{first.ApiObjectTypes.Single().ApiVersion!.ApiProperty!.ApiName}" +
                $"|{second.ApiObjectTypes.Single().ApiVersion!.ApiProperty!.ApiName}";
    }

    public static string ObserveRemovedAnnotations()
    {
        var reader = new MutableAnnotationReader();
        var builder = new ApiSchemaBuilder().WithName("Sessions").AddScalar<int>().AddObject<Customer>()
            .UseDefaultConventions().UseAnnotations(readers => readers.AddReader(reader));
        var first = builder.Build();
        reader.IsEnabled = false;
        var second = builder.Build();
        return $"{first.ApiObjectTypes.Single().ApiName}|{second.ApiObjectTypes.Single().ApiName}|" +
            $"{first.ApiObjectTypes.Single().HasVersion}|{second.ApiObjectTypes.Single().HasVersion}";
    }

    public static string ObserveAssemblyFilterChanges()
    {
        var includesTransient = true;
        var builder = CreateBuilder().UseAssemblyTypeInference(typeof(Transient).Assembly,
            type => includesTransient && type == typeof(Transient));
        var first = builder.Build();
        includesTransient = false;
        var second = builder.Build();
        return $"{first.ApiObjectTypes.Length}|{second.ApiObjectTypes.Length}";
    }

    public static string ObserveNestedConfigurationIsolation()
    {
        var isEnabled = true;
        var builder = CreateBuilder().AddObject<Customer>(static customer => customer
            .AddKey("Key", static key => key.AddPath("Id")).WithVersion("Revision"))
            .UseConfigurationsFromAssembly(typeof(NestedConfiguration).Assembly,
                type => isEnabled && type == typeof(NestedConfiguration));
        var first = builder.Build().ApiObjectTypes.Single();
        isEnabled = false;
        var second = builder.Build().ApiObjectTypes.Single();
        return $"{first.ApiKeys.Single().ApiKeyPaths.Length}|{second.ApiKeys.Single().ApiKeyPaths.Length}|" +
            $"{first.ApiVersion!.ApiProperty!.ApiName}|{second.ApiVersion!.ApiProperty!.ApiName}";
    }

    public static string ObserveRelationshipNestedIsolation()
    {
        var builder = CreateBuilder().AddObject<Transient>(static transient => transient.AddProperty("Id", "Id"));
        var registration = builder.DeclareOneToManyRelationship("Relationship", static relationship => relationship
            .From<Customer>(static end => end.AddRelationshipPrincipalEndExtension(typeof(LabelExtension),
                new LabelExtension(1)))
            .To<Transient>());
        builder.UseConventions(conventions => conventions.AddConvention(new RelationshipExtensionConvention(
            registration)));
        var first = (ApiRelationshipOneToMany)builder.Build().ApiRelationships.Single();
        builder.UseConventions(static conventions => conventions.RemoveConvention<RelationshipExtensionConvention>());
        var second = (ApiRelationshipOneToMany)builder.Build().ApiRelationships.Single();
        first.ApiPrincipalEnd.TryGetExtension<LabelExtension>(out var firstExtension);
        second.ApiPrincipalEnd.TryGetExtension<LabelExtension>(out var secondExtension);
        return $"{firstExtension!.Label}|" +
            $"{secondExtension!.Label}|{ReferenceEquals(first.ApiPrincipalEnd, second.ApiPrincipalEnd)}";
    }
    #endregion

    #region Failure Recovery Scenarios
    public static string ObserveDiscoveryFailureRecovery()
    {
        var builder = CreateBuilder().UseConventions(static conventions => conventions.AddConvention(new FailOnceDiscovery()));
        var hasThrown = false;
        try
        {
            builder.BuildResult();
        }
        catch (InvalidOperationException)
        {
            hasThrown = true;
        }
        var schema = builder.Build();
        return $"{hasThrown}|{schema.ApiObjectTypes.Length}|{schema.ApiQuerySources.Length}";
    }

    public static string ObserveMaterializationFailureRecovery()
    {
        var shouldFail = true;
        var builder = CreateBuilder().AddObject<Customer>(customer => customer.WithOptions(_ =>
        {
            if (shouldFail)
            {
                throw new InvalidOperationException("Options construction failed.");
            }
        })).UseConventions(conventions => conventions.AddConvention(new ConditionalDiscovery(() => shouldFail)));
        var hasThrown = false;
        try
        {
            builder.BuildResult();
        }
        catch (InvalidOperationException)
        {
            hasThrown = true;
        }
        shouldFail = false;
        var schema = builder.Build();
        return $"{hasThrown}|{schema.ApiObjectTypes.Length}|{schema.ApiQuerySources.Length}";
    }

    public static string ObserveCompilationFailureRecovery()
    {
        var builder = CreateBuilder();
        var registration = builder.DeclareQuerySource("Source", static _ => { });
        var failed = builder.BuildResult();
        builder.ConfigureQuerySource(registration, static source => source.WithOutput<int>());
        var successful = builder.BuildResult();
        return $"{failed.IsValid}|{successful.IsValid}|{successful.Schema!.ApiQuerySources.Single().ApiName}";
    }

    public static string ObserveFreezeFailureRecovery()
    {
        var extension = new RecoveringExtension();
        var builder = CreateBuilder().AddSchemaExtension(typeof(RecoveringExtension), extension);
        var failed = builder.BuildResult();
        extension.IsValid = true;
        var successful = builder.BuildResult();
        return $"{failed.IsValid}|{successful.IsValid}";
    }
    #endregion

    #region Callback and Graph Scenarios
    public static string ObserveImmediateCallbacks()
    {
        var calls = 0;
        var builder = CreateBuilder();
        var hasThrown = false;
        try
        {
            builder.AddObject<Customer>(customer =>
            {
                calls++;
                customer.WithName("Retained");
                throw new InvalidOperationException("Immediate authoring failure.");
            });
        }
        catch (InvalidOperationException)
        {
            hasThrown = true;
        }
        var callsBeforeBuild = calls;
        var first = builder.Build();
        var second = builder.Build();
        return $"{hasThrown}|{callsBeforeBuild}|" +
            $"{calls}|{first.ApiObjectTypes.Single().ApiName}|{second.ApiObjectTypes.Single().ApiName}";
    }

    public static string ObserveDeferredOptions()
    {
        var schemaCalls = 0;
        var objectCalls = 0;
        var builder = CreateBuilder().WithOptions(_ => schemaCalls++)
            .AddObject<Customer>(customer => customer.WithOptions(_ => objectCalls++));
        var beforeBuild = $"{schemaCalls}:{objectCalls}";
        builder.Build();
        var afterFirst = $"{schemaCalls}:{objectCalls}";
        builder.Build();
        return $"{beforeBuild}|{afterFirst}|{schemaCalls}:{objectCalls}";
    }

    public static string ObserveRetainedBuilderEdits()
    {
        ApiObjectTypeBuilder<Customer>? retained = null;
        var builder = CreateBuilder().AddObject<Customer>(customer => retained = customer);
        var first = builder.Build();
        retained!.WithName("Changed");
        var second = builder.Build();
        return $"{first.ApiObjectTypes.Single().ApiName}|{second.ApiObjectTypes.Single().ApiName}|" +
            $"{ReferenceEquals(first.ApiObjectTypes.Single(), second.ApiObjectTypes.Single())}";
    }

    public static string ObserveFreshOwnedGraphs()
    {
        var builder = CreateBuilder().AddObject<Customer>(static customer => customer.AddKey("Key", static key => key.AddPath("Id"))
            .WithVersion("Revision")).AddQuerySource("Numbers", static source => source.AddInputParameter<int>("id")
                .WithCollectionOutput<int>());
        var first = builder.Build();
        var second = builder.Build();
        var isKeyPathShared = ReferenceEquals(first.ApiObjectTypes[0].ApiKeys[0].ApiKeyPaths[0],
            second.ApiObjectTypes[0].ApiKeys[0].ApiKeyPaths[0]);
        var isParameterShared = ReferenceEquals(first.ApiQuerySources[0].ApiInputParameters[0],
            second.ApiQuerySources[0].ApiInputParameters[0]);
        return $"{ReferenceEquals(first, second)}|" +
            $"{ReferenceEquals(first.ApiObjectTypes[0].ApiProperties[0], second.ApiObjectTypes[0].ApiProperties[0])}|" +
            $"{isKeyPathShared}" +
                $"|" +
            $"{ReferenceEquals(first.ApiObjectTypes[0].ApiVersion, second.ApiObjectTypes[0].ApiVersion)}|" +
            $"{ReferenceEquals(first.ApiQuerySources[0].ApiOutputType, second.ApiQuerySources[0].ApiOutputType)}|" +
            $"{isParameterShared}";
    }

    public static string ObserveReentrantBuildRecovery()
    {
        var builder = CreateBuilder();
        builder.WithOptions(_ => builder.BuildResult());
        var hasThrown = false;
        try
        {
            builder.BuildResult();
        }
        catch (InvalidOperationException)
        {
            hasThrown = true;
        }
        builder.WithDefaultOptions();
        return $"{hasThrown}|{builder.BuildResult().IsValid}";
    }

    public static string ObserveConcurrentBuildRejection()
    {
        using var entered = new ManualResetEventSlim();
        using var released = new ManualResetEventSlim();
        var builder = CreateBuilder().WithOptions(_ =>
        {
            entered.Set();
            if (!released.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new TimeoutException("Build release timed out.");
            }
        });
        var firstBuild = Task.Run(builder.BuildResult);
        var hasThrown = false;
        try
        {
            if (!entered.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new TimeoutException("Build entry timed out.");
            }
            try
            {
                builder.BuildResult();
            }
            catch (InvalidOperationException)
            {
                hasThrown = true;
            }
        }
        finally
        {
            released.Set();
        }
        return $"{hasThrown}|{firstBuild.GetAwaiter().GetResult().IsValid}";
    }
    #endregion

    #region Test Types
    private sealed class BuildTimeDeclaration : IApiSchemaConvention
    {
        #region Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Discovery;
        #endregion

        #region Convention Methods
        public void Apply(ApiSchemaBuilder builder)
            => builder.DeclareQuerySource("Ephemeral", static source => source.WithOutput<int>());
        #endregion
    }
    #endregion

    #region Additional Regression Scenarios
    public static string ObserveBuildTimeHandleRejection()
    {
        var builder = CreateBuilder().UseConventions
            (static conventions => conventions.AddConvention(new BuildTimeDeclaration()));
        var hasThrown = false;
        try
        {
            builder.BuildResult();
        }
        catch (Exceptions.ApiSchemaConfigurationException)
        {
            hasThrown = true;
        }
        builder.UseConventions(static conventions => conventions.RemoveConvention<BuildTimeDeclaration>());
        return $"{hasThrown}|{builder.BuildResult().IsValid}|{builder.Build().ApiQuerySources.Length}";
    }
    #endregion
}
