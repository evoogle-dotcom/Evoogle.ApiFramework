// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Text.Json;
using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.ApiFramework.Schema.Query;

using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace Evoogle.ApiFramework.Schema.Configuration.Query;

[DynamicLinqType]
public static class ApiQuerySourceDeclarationTestsFactory
{
    #region Declarations
    public sealed class Customer
    {
        #region Properties
        public int Id { get; set; }
        #endregion
    }
    public delegate Customer Lookup(int id);
    public delegate Customer LookupTwin(int id);
    public delegate Customer LookupPair(int id, string? term);
    public delegate IReadOnlyList<Customer?> Search(string? term, int[] ids);
    public delegate Customer GenericLookup<T>(T id);
    public delegate Customer RefLookup(ref int id);
    private delegate Customer PrivateLookup(int id);

    [ApiQuerySource(ApiName = "Annotated", ApiOutputComposition = ApiQueryComposition.Projection)]
    public delegate Customer AnnotatedLookup
        ([ApiQuerySourceParameter(ApiName = "annotatedId", ApiPresence = ApiPresence.Optional)] int id);

    public sealed class LookupConfiguration : IApiQuerySourceConfiguration<Lookup>
    {
        #region Configuration Methods
        public void Configure(ApiQuerySourceBuilder builder)
            => builder.WithName("Configured");
        #endregion
    }

    public sealed class MultipleDelegateConfiguration :
        IApiQuerySourceConfiguration<Lookup>, IApiQuerySourceConfiguration<LookupTwin>
    {
        #region Configuration Methods
        void IApiQuerySourceConfiguration<Lookup>.Configure(ApiQuerySourceBuilder builder)
            => builder.WithName("First");
        void IApiQuerySourceConfiguration<LookupTwin>.Configure(ApiQuerySourceBuilder builder)
            => builder
            .WithName("Second");
        #endregion
    }

    private sealed class NamedConfiguration(string apiName) : IApiQuerySourceConfiguration
    {
        #region Properties
        public string ApiName => apiName;
        #endregion

        #region Configuration Methods
        public void Configure(ApiQuerySourceBuilder builder)
            => builder.WithOutput<Customer>();
        #endregion
    }
    #endregion

    #region Factory Methods
    private static ApiSchemaBuilder CreateBuilder()
        => new ApiSchemaBuilder().WithName("Declarations")
        .AddScalar<int>().AddScalar<string>()
        .AddObject<Customer>(static customer => customer.WithName("Customer").AddProperty(static member => member.Id));
    #endregion

    #region Build Scenarios
    public static ApiSchemaCompilationResult BuildInferredDelegate()
        => CreateBuilder()
        .AddQuerySource<LookupPair>().BuildResult();
    public static ApiSchemaCompilationResult BuildInferredCollection()
        => CreateBuilder()
        .AddQuerySource<Search>().BuildResult();
    public static ApiSchemaCompilationResult BuildClosedGeneric()
        => CreateBuilder()
        .AddQuerySource<GenericLookup<int>>(static source => source.WithName("Generic")).BuildResult();
    public static ApiSchemaCompilationResult BuildRepeatedDelegate()
        => CreateBuilder()
        .AddQuerySource<Lookup>(static source => source.WithName("First"))
        .AddQuerySource<Lookup>(static source => source.WithName("Renamed").WithOutputComposition(ApiQueryComposition
            .Projection))
        .BuildResult();
    public static ApiSchemaCompilationResult BuildEquivalentSignatures()
        => CreateBuilder()
        .AddQuerySource<Lookup>(static source => source.WithName("First"))
        .AddQuerySource<LookupTwin>(static source => source.WithName("Second")).BuildResult();
    public static ApiSchemaCompilationResult BuildIndependentFluentSources()
        => CreateBuilder()
        .AddQuerySource("First", static source => source.WithOutput<Customer>())
        .AddQuerySource("Second", static source => source.WithOutput<Customer>()).BuildResult();
    public static ApiSchemaCompilationResult BuildDuplicateFluentNames()
        => CreateBuilder()
        .AddQuerySource("Duplicate", static source => source.WithOutput<Customer>())
        .AddQuerySource("Duplicate", static source => source.WithOutput<Customer>()).BuildResult();
    public static ApiSchemaCompilationResult BuildDuplicateDelegateNames()
        => CreateBuilder()
        .AddQuerySource<Lookup>(static source => source.WithName("Duplicate"))
        .AddQuerySource<LookupTwin>(static source => source.WithName("Duplicate")).BuildResult();
    public static ApiSchemaCompilationResult BuildExplicitAnnotationFacets()
        => CreateBuilder()
        .AddQuerySource<AnnotatedLookup>(static source => source.WithName("Explicit")
            .ConfigureInputParameter("id", static parameter => parameter.WithName("explicitId").AsRequired())
            .WithOutputComposition(ApiQueryComposition.None))
        .UseDefaultAnnotations().UseAssemblyAnnotationScanning(typeof(AnnotatedLookup).Assembly,
            static type => type == typeof(AnnotatedLookup)).BuildResult();
    public static ApiSchemaCompilationResult BuildSignatureOutputConflict()
        => CreateBuilder()
        .AddQuerySource<Lookup>(static source => source.WithOutput<string>()).BuildResult();
    public static ApiSchemaCompilationResult BuildSignatureParameterConflict()
        => CreateBuilder()
        .AddQuerySource<Lookup>(static source => source.AddInputParameter<string>("id")).BuildResult();
    public static ApiSchemaCompilationResult BuildAdditionalParameterConflict()
        => CreateBuilder()
        .AddQuerySource<Lookup>(static source => source.AddInputParameter<int>("extra")).BuildResult();
    public static ApiSchemaCompilationResult BuildUnknownParameterConflict()
        => CreateBuilder()
        .AddQuerySource<Lookup>(static source => source.ConfigureInputParameter("missing", static parameter => parameter
            .AsOptional()))
        .BuildResult();
    public static ApiSchemaCompilationResult BuildMutableFacets()
        => CreateBuilder()
        .AddQuerySource<Search>(static source => source.WithOutputNullability(ApiNullability.Nullable)
            .WithOutputItemNullability(ApiNullability.NonNullable)
            .ConfigureInputParameter("term", static parameter => parameter.WithName("query").AsOptional().AsNonNullable()))
        .BuildResult();
    public static ApiSchemaCompilationResult BuildRenamedParameterOrder()
        => CreateBuilder()
        .AddQuerySource<LookupPair>(static source => source.ConfigureInputParameter("id", static parameter => parameter
            .WithName("Zulu"))
            .ConfigureInputParameter("term", static parameter => parameter.WithName("Alpha")))
        .AddQuerySource<LookupPair>(static source => source.ConfigureInputParameter("id", static parameter => parameter.AsOptional()))
        .BuildResult();
    public static ApiSchemaCompilationResult BuildGenericConfiguration()
        => CreateBuilder()
        .AddQuerySource(new LookupConfiguration()).BuildResult();
    public static ApiSchemaCompilationResult BuildDiscoveredGenericConfigurations()
        => CreateBuilder()
        .UseConfigurationsFromAssembly(typeof(Lookup).Assembly, static type => type == typeof(MultipleDelegateConfiguration))
        .UseConfigurationsFromAssembly(typeof(Lookup).Assembly, static type => type == typeof(MultipleDelegateConfiguration))
        .BuildResult();
    public static ApiSchemaCompilationResult BuildIndependentConfigurationInstances()
        => CreateBuilder()
        .AddQuerySource(new NamedConfiguration("First")).AddQuerySource(new NamedConfiguration("Second")).BuildResult();
    public static ApiSchemaCompilationResult BuildFluentNamingCandidate()
        => CreateBuilder()
        .AddQuerySource("CustomerSearch", static source => source.WithOutput<Customer>())
        .UseKebabCaseNaming(ApiNamingConventionTargets.QuerySource).BuildResult();
    public static ApiSchemaCompilationResult BuildPinnedFluentName()
        => CreateBuilder()
        .AddQuerySource("CustomerSearch", static source => source.WithName("ExactName").WithOutput<Customer>())
        .UseKebabCaseNaming(ApiNamingConventionTargets.QuerySource).BuildResult();
    public static ApiSchemaCompilationResult BuildInvalidNonDelegate()
        => CreateBuilder().AddQuerySource(
        typeof(string)).BuildResult();
    public static ApiSchemaCompilationResult BuildInvalidVoidDelegate()
        => CreateBuilder()
        .AddQuerySource<Action>().BuildResult();
    public static ApiSchemaCompilationResult BuildInvalidOpenDelegate()
        => CreateBuilder().AddQuerySource(
        typeof(GenericLookup<>)).BuildResult();
    public static ApiSchemaCompilationResult BuildInvalidPrivateDelegate()
        => CreateBuilder().AddQuerySource(
        typeof(PrivateLookup)).BuildResult();
    public static ApiSchemaCompilationResult BuildInvalidRefDelegate()
        => CreateBuilder()
        .AddQuerySource<RefLookup>().BuildResult();
    public static ApiSchemaCompilationResult BuildInvalidAsyncDelegate()
        => CreateBuilder()
        .AddQuerySource<Func<Task<Customer>>>().BuildResult();
    #endregion

    #region Handle Scenarios
    public static string ObserveHandleAcrossBuilds()
    {
        var builder = CreateBuilder();
        var registration = builder.DeclareQuerySource("First", static source => source.WithOutput<Customer>());
        var first = builder.Build().ApiQuerySources.Single();
        builder.ConfigureQuerySource(registration, static source => source.WithName("Second"));
        var second = builder.Build().ApiQuerySources.Single();
        return $"{first.ApiName}|{second.ApiName}|{ReferenceEquals(first, second)}";
    }
    public static string ConfigureForeignHandle()
    {
        var owner = CreateBuilder();
        var registration = owner.DeclareQuerySource("First", static source => source.WithOutput<Customer>());
        CreateBuilder().ConfigureQuerySource(registration, static source => source.WithName("Second"));
        return "Unexpected completion";
    }
    public static string SerializeHandle()
    {
        var registration = CreateBuilder().DeclareQuerySource("Source", static source => source.WithOutput<Customer>());
        return JsonSerializer.Serialize(registration);
    }
    public static string DeserializeHandle()
        => JsonSerializer.Deserialize<ApiQuerySourceRegistration>("{}")!
        .ToString()!;
    #endregion

    #region Additional Regression Scenarios
    public static ApiSchemaCompilationResult BuildInvalidOutputItemFacet()
        => CreateBuilder()
        .AddQuerySource<Lookup>(static source => source.WithOutputItemNullability(ApiNullability.Nullable)).BuildResult();

    public static ApiSchemaCompilationResult BuildInvalidParameterItemFacet()
        => CreateBuilder()
        .AddQuerySource<Lookup>(static source => source.ConfigureInputParameter("id", static parameter =>
            parameter.WithItemNullability(ApiNullability.Nullable))).BuildResult();

    public static ApiSchemaCompilationResult BuildRenamedFluentParameter()
        => CreateBuilder()
        .AddQuerySource("Source", static source => source
            .AddInputParameter<int>("id", static parameter => parameter.WithName("customerId"))
            .AddInputParameter<int>("id", static parameter => parameter.AsOptional())
            .WithOutput<Customer>()).BuildResult();

    public static ApiSchemaCompilationResult BuildRegisteredAnnotationWithoutScan()
        => CreateBuilder()
        .AddQuerySource<AnnotatedLookup>().UseDefaultAnnotations().BuildResult();
    #endregion
}
