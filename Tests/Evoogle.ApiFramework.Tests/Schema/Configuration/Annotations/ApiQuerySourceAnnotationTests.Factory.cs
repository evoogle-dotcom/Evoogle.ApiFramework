// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.ApiFramework.Schema.Configuration.Query;
using Evoogle.ApiFramework.Schema.Query;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

[DynamicLinqType]
public static class ApiQuerySourceAnnotationTestsFactory
{
    #region Query Source Declarations
    [ApiQuerySource
    (
        ApiOutputComposition = ApiQueryComposition.Projection | ApiQueryComposition.Filtering
    )]
    public delegate IReadOnlyList<QueryAnnotationCustomer> CustomerSearch
    (
        [ApiQuerySourceParameter
        (
            ApiNullability = ApiNullability.Nullable,
            ApiPresence = ApiPresence.Optional
        )]
        string? customerName,
        IReadOnlyList<IReadOnlyList<int?>?> ids
    );

    [ApiQuerySource]
    public delegate Task<QueryAnnotationCustomer> InvalidTaskQuery();

    [ApiQuerySource]
    public delegate ValueTask<QueryAnnotationCustomer> InvalidValueTaskQuery();

    [ApiQuerySource]
    public delegate IAsyncEnumerable<QueryAnnotationCustomer> InvalidAsyncEnumerableQuery();

    [ApiQuerySource]
    public delegate QueryAnnotationTask InvalidDerivedTaskQuery();

    [ApiQuerySource]
    public delegate QueryAnnotationStream InvalidAsyncEnumerableImplementationQuery();

    [ApiQuerySource]
    public delegate void InvalidVoidQuery();

    [ApiQuerySource]
    public delegate QueryAnnotationCustomer InvalidGenericQuery<T>();

    [ApiQuerySource]
    public delegate ref QueryAnnotationCustomer InvalidRefReturnQuery();

    [ApiQuerySource]
    public delegate QueryAnnotationCustomer InvalidRefParameterQuery(ref int id);

    [ApiQuerySource]
    public unsafe delegate int* InvalidPointerReturnQuery();

    [ApiQuerySource]
    public unsafe delegate QueryAnnotationCustomer InvalidPointerParameterQuery(int* id);

    [ApiQuerySource(ApiName = "Duplicate")]
    public delegate QueryAnnotationCustomer FirstDuplicate();

    [ApiQuerySource(ApiName = "Duplicate")]
    public delegate QueryAnnotationCustomer SecondDuplicate();

    [ApiQuerySource
    (
        ApiName = "ExactQuery",
        ApiOutputComposition = ApiQueryComposition.Projection | ApiQueryComposition.Filtering
    )]
    public delegate IReadOnlyList<QueryAnnotationCustomer> ExplicitNamesQuery
    (
        [ApiQuerySourceParameter
        (
            ApiName = "ExactParameter",
            ApiNullability = ApiNullability.Nullable,
            ApiPresence = ApiPresence.Optional
        )]
        string? customerName, IReadOnlyList<IReadOnlyList<int?>?> ids
    );

    [ApiQuerySource]
    public delegate IReadOnlyList<QueryAnnotationCustomer> AnnotatedConventionQuery(string filter);

    public delegate IReadOnlyList<QueryAnnotationCustomer> CustomReaderQuery
    (
        string? customerName,
        IReadOnlyList<IReadOnlyList<int?>?> ids
    );
    #endregion

    #region Convention Types
    private sealed class TestQuerySourceConvention(bool expectsClrMetadata) : IApiQuerySourceConvention
    {
        #region Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
        #endregion

        #region Convention Methods
        public void Apply(ApiQuerySourceBuilder builder, ApiQuerySourceConventionContext context)
        {
            if ((context.ClrDelegateType is not null) != expectsClrMetadata)
            {
                throw new InvalidOperationException("The query-source CLR context did not match the declaration.");
            }

            builder.WithOutputComposition(ApiQueryComposition.Filtering);
            if (context.ClrDelegateType is null)
            {
                builder.AddInputParameter<string>("addedByConvention");
            }
        }
        #endregion
    }

    private sealed class TestQuerySourceParameterConvention(bool expectsSourceClrMetadata) :
        IApiQuerySourceParameterConvention
    {
        #region Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
        #endregion

        #region Convention Methods
        public void Apply
        (
            ApiQuerySourceParameterBuilder builder,
            ApiQuerySourceParameterConventionContext context
        )
        {
            if ((context.ApiQuerySourceContext.ClrDelegateType is not null) != expectsSourceClrMetadata)
            {
                throw new InvalidOperationException
                    ("The parameter's source CLR context did not match the declaration.");
            }

            builder.AsOptional().AsNullable();
        }
        #endregion
    }

    private sealed class TestQuerySourceAnnotationReader
    (
        ApiQueryComposition apiOutputComposition =
            ApiQueryComposition.Projection | ApiQueryComposition.Filtering
    ) : IApiQuerySourceAnnotationReader
    {
        #region Annotation Reader Methods
        public ApiAnnotationReaderResult<ApiQuerySourceAnnotationResult> ReadQuerySourceAnnotations
        (
            Assembly assembly,
            Func<Type, bool>? filter
        )
        {
            var clrDelegateType = typeof(CustomReaderQuery);
            if (assembly != clrDelegateType.Assembly || filter?.Invoke(clrDelegateType) == false)
            {
                return new([], []);
            }

            var parameters = clrDelegateType.GetMethod(nameof(Action.Invoke))!.GetParameters();
            return new
            (
                [
                    new ApiQuerySourceAnnotationResult
                    (
                        clrDelegateType,
                        "customerSearch",
                        ApiNullability.NonNullable,
                        apiOutputComposition,
                        [
                            new(parameters[0], "customerName", ApiNullability.Nullable, ApiPresence.Optional),
                            new(parameters[1], "ids", null, null)
                        ]
                    )
                ],
                []
            );
        }
        #endregion
    }

    private sealed class FailOnceQuerySourceAnnotationReader : IApiQuerySourceAnnotationReader
    {
        #region Properties
        private bool HasFailed { get; set; }
        #endregion

        #region Annotation Reader Methods
        public ApiAnnotationReaderResult<ApiQuerySourceAnnotationResult> ReadQuerySourceAnnotations
        (
            Assembly assembly,
            Func<Type, bool>? filter
        )
        {
            if (this.HasFailed)
            {
                return new([], []);
            }

            this.HasFailed = true;
            return new
            (
                [],
                [
                    new ApiAnnotationReaderDiagnostic
                    (
                        ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration,
                        assembly.GetName().Name!,
                        "Intentional first-build annotation diagnostic"
                    )
                ]
            );
        }
        #endregion
    }
    #endregion

    #region Factory Methods
    public static ApiSchemaCompilationResult BuildValidDeclaration()
        => BuildValidDeclaration(typeof(CustomerSearch));

    public static ApiSchemaCompilationResult BuildNamespaceDeclaration()
        => BuildValidDeclaration
        (typeof(NamespaceCustomerSearch));

    public static ApiSchemaCompilationResult BuildOrdinaryNestedDeclaration()
        => BuildValidDeclaration
        (typeof(OrdinaryQuerySourceDeclarations.CustomerSearch));

    public static ApiSchemaCompilationResult BuildExplicitNamesDeclaration()
        => BuildValidDeclaration
        (typeof(ExplicitNamesQuery));

    public static ApiSchemaCompilationResult BuildMissingReferencedTypes()
        => new ApiSchemaBuilder()
        .WithName("MissingQuerySourceTypes")
        .UseDefaultAnnotations()
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            static type => type == typeof(CustomerSearch)
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildCustomReaderDeclaration()
        => new ApiSchemaBuilder()
        .WithName("CustomQuerySourceAnnotationReader")
        .AddScalar<int>()
        .AddScalar<string>()
        .AddObject<QueryAnnotationCustomer>(static customer => customer.AddProperty(static property => property.Id))
        .UseAnnotations(static readers => readers.AddReader(new TestQuerySourceAnnotationReader()))
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            static type => type == typeof(CustomReaderQuery)
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildSourceNamingOnly()
        => BuildValidDeclaration
        (
            typeof(CustomerSearch),
            ApiNamingConventionTargets.QuerySource,
            useKebabCaseNaming: true
        );

    public static ApiSchemaCompilationResult BuildParameterNamingOnly()
        => BuildValidDeclaration
        (
            typeof(CustomerSearch),
            ApiNamingConventionTargets.QuerySourceParameter,
            useKebabCaseNaming: true
        );

    private static ApiSchemaCompilationResult BuildValidDeclaration
    (
        Type clrDelegateType,
        ApiNamingConventionTargets namingTargets =
            ApiNamingConventionTargets.QuerySource | ApiNamingConventionTargets.QuerySourceParameter,
        bool useKebabCaseNaming = false
    )
    {
        var builder = new ApiSchemaBuilder()
            .WithName("QuerySourceAnnotations")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<QueryAnnotationCustomer>(static customer => customer.AddProperty(static property => property.Id))
            .UseDefaultAnnotations();

        if (useKebabCaseNaming)
        {
            builder.UseKebabCaseNaming(namingTargets);
        }
        else
        {
            builder.UseCamelCaseNaming(namingTargets);
        }

        return builder.UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            type => type == clrDelegateType
        )
        .BuildResult();
    }

    public static ApiSchemaCompilationResult BuildInvalidTaskDeclaration()
        => new ApiSchemaBuilder()
        .WithName("InvalidTaskQuerySourceAnnotation")
        .UseDefaultAnnotations()
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            static type => type == typeof(InvalidTaskQuery)
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildInvalidValueTaskDeclaration()
        => BuildInvalidDeclaration
        (typeof(InvalidValueTaskQuery));

    public static ApiSchemaCompilationResult BuildInvalidAsyncEnumerableDeclaration()
        => BuildInvalidDeclaration
        (typeof(InvalidAsyncEnumerableQuery));

    public static ApiSchemaCompilationResult BuildInvalidDerivedTaskDeclaration()
        => BuildInvalidDeclaration
        (typeof(InvalidDerivedTaskQuery));

    public static ApiSchemaCompilationResult BuildInvalidAsyncEnumerableImplementationDeclaration()
        => BuildInvalidDeclaration(typeof(InvalidAsyncEnumerableImplementationQuery));

    public static ApiSchemaCompilationResult BuildInvalidVoidDeclaration()
        => BuildInvalidDeclaration
        (typeof(InvalidVoidQuery));

    public static ApiSchemaCompilationResult BuildInvalidGenericDeclaration()
        => BuildInvalidDeclaration
        (typeof(InvalidGenericQuery<>));

    public static ApiSchemaCompilationResult BuildInvalidRefReturnDeclaration()
        => BuildInvalidDeclaration
        (typeof(InvalidRefReturnQuery));

    public static ApiSchemaCompilationResult BuildInvalidRefParameterDeclaration()
        => BuildInvalidDeclaration
        (typeof(InvalidRefParameterQuery));

    public static ApiSchemaCompilationResult BuildInvalidPointerReturnDeclaration()
        => BuildInvalidDeclaration
        (typeof(InvalidPointerReturnQuery));

    public static ApiSchemaCompilationResult BuildInvalidPointerParameterDeclaration()
        => BuildInvalidDeclaration
        (typeof(InvalidPointerParameterQuery));

    private static ApiSchemaCompilationResult BuildInvalidDeclaration(Type clrDelegateType)
        => new ApiSchemaBuilder()
        .WithName("InvalidQuerySourceAnnotation")
        .UseDefaultAnnotations()
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            type => type == clrDelegateType
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildDuplicateDeclarations()
        => new ApiSchemaBuilder()
        .WithName("DuplicateQuerySourceAnnotations")
        .AddScalar<int>()
        .AddObject<QueryAnnotationCustomer>(static customer => customer.AddProperty(static property => property.Id))
        .UseDefaultAnnotations()
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            static type => type == typeof(FirstDuplicate) || type == typeof(SecondDuplicate)
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildRepeatedReaders()
        => new ApiSchemaBuilder()
        .WithName("RepeatedQuerySourceReaders")
        .AddScalar<int>()
        .AddScalar<string>()
        .AddObject<QueryAnnotationCustomer>(static customer => customer.AddProperty(static property => property.Id))
        .UseDefaultAnnotations()
        .UseDefaultAnnotations()
        .UseCamelCaseNaming
        (
            ApiNamingConventionTargets.QuerySource |
            ApiNamingConventionTargets.QuerySourceParameter
        )
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            static type => type == typeof(CustomerSearch)
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildConflictingReaders()
        => new ApiSchemaBuilder()
        .WithName("ConflictingQuerySourceReaders")
        .AddScalar<int>()
        .AddScalar<string>()
        .AddObject<QueryAnnotationCustomer>(static customer => customer.AddProperty(static property => property.Id))
        .UseAnnotations(static readers => readers
            .AddReader(new TestQuerySourceAnnotationReader(ApiQueryComposition.Projection))
            .AddReader(new TestQuerySourceAnnotationReader(ApiQueryComposition.Filtering)))
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            static type => type == typeof(CustomReaderQuery)
        )
        .BuildResult();

    public static ApiSchema BuildExplicitPrecedence()
        => new ApiSchemaBuilder()
        .WithName("ExplicitQuerySourcePrecedence")
        .AddScalar<int>()
        .AddScalar<string>()
        .AddObject<QueryAnnotationCustomer>(static customer => customer.AddProperty(static property => property.Id))
        .AddQuerySource<CustomerSearch>(static source => source
            .ConfigureInputParameter("customerName", static parameter => parameter.AsRequired().AsNonNullable())
            .WithOutputNullability(ApiNullability.Nullable)
            .WithOutputComposition(ApiQueryComposition.None))
        .UseDefaultAnnotations()
        .UseCamelCaseNaming
        (
            ApiNamingConventionTargets.QuerySource |
            ApiNamingConventionTargets.QuerySourceParameter
        )
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            static type => type == typeof(CustomerSearch)
        )
        .Build();

    public static ApiSchema BuildCustomConventions()
        => new ApiSchemaBuilder()
        .WithName("CustomQuerySourceConventions")
        .AddScalar<string>()
        .AddQuerySource("Values", static source => source.WithCollectionOutput<string>())
        .UseConventions(static conventions => conventions
            .AddConvention(new TestQuerySourceConvention(expectsClrMetadata: false))
            .AddConvention(new TestQuerySourceParameterConvention(expectsSourceClrMetadata: false)))
        .Build();

    public static ApiSchema BuildAnnotatedCustomConventions()
        => new ApiSchemaBuilder()
        .WithName("AnnotatedCustomQuerySourceConventions")
        .AddScalar<int>()
        .AddScalar<string>()
        .AddObject<QueryAnnotationCustomer>(static customer => customer.AddProperty(static property => property.Id))
        .UseDefaultAnnotations()
        .UseConventions(static conventions => conventions
            .AddConvention(new TestQuerySourceConvention(expectsClrMetadata: true))
            .AddConvention(new TestQuerySourceParameterConvention(expectsSourceClrMetadata: true)))
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            static type => type == typeof(AnnotatedConventionQuery)
        )
        .Build();

    public static ApiSchema BuildRepeatedScan()
    {
        var assembly = typeof(ApiQuerySourceAnnotationTestsFactory).Assembly;
        return new ApiSchemaBuilder()
            .WithName("RepeatedQuerySourceScan")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<QueryAnnotationCustomer>(static customer => customer.AddProperty(static property => property.Id))
            .UseDefaultAnnotations()
            .UseCamelCaseNaming
            (
                ApiNamingConventionTargets.QuerySource |
                ApiNamingConventionTargets.QuerySourceParameter
            )
            .UseAssemblyAnnotationScanning(assembly, static type => type == typeof(CustomerSearch))
            .UseAssemblyAnnotationScanning(assembly, static type => type == typeof(CustomerSearch))
            .Build();
    }

    public static string BuildAfterNamingConventionChange()
    {
        var builder = CreateRepeatedBuildBuilder();
        var firstSchema = builder.Build();

        builder.UseCamelCaseNaming
        (
            ApiNamingConventionTargets.QuerySource |
            ApiNamingConventionTargets.QuerySourceParameter
        );
        var secondSchema = builder.Build();

        return $"{JoinQuerySourceNames(firstSchema)}|{JoinQuerySourceNames(secondSchema)}";
    }

    public static string BuildAfterFailedAnnotationBuild()
    {
        var builder = CreateRepeatedBuildBuilder()
            .UseAnnotations(static readers => readers.AddReader(new FailOnceQuerySourceAnnotationReader()));
        var failedResult = builder.BuildResult();
        if (failedResult.IsValid)
        {
            throw new InvalidOperationException("The first schema build was expected to fail.");
        }

        builder.UseCamelCaseNaming
        (
            ApiNamingConventionTargets.QuerySource |
            ApiNamingConventionTargets.QuerySourceParameter
        );
        var schema = builder.Build();
        return JoinQuerySourceNames(schema);
    }

    private static ApiSchemaBuilder CreateRepeatedBuildBuilder()
        => new ApiSchemaBuilder()
        .WithName("RepeatedQuerySourceBuild")
        .AddScalar<int>()
        .AddScalar<string>()
        .AddObject<QueryAnnotationCustomer>(static customer => customer.AddProperty(static property => property.Id))
        .AddQuerySource("ExplicitSource", static source => source.WithName("ExplicitSource").WithOutput<string>())
        .UseDefaultAnnotations()
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            static type => type == typeof(CustomerSearch)
        );

    private static string JoinQuerySourceNames(ApiSchema schema)
        => string.Join(",", schema.ApiQuerySources.Select(static source => source.ApiName));
    #endregion
}

public sealed class QueryAnnotationTask : Task
{
    #region Constructors
    public QueryAnnotationTask()
        : base(static () => { })
    {
    }
    #endregion
}

public abstract class QueryAnnotationStream : IAsyncEnumerable<QueryAnnotationCustomer>
{
    #region IAsyncEnumerable<QueryAnnotationCustomer> Methods
    public abstract IAsyncEnumerator<QueryAnnotationCustomer> GetAsyncEnumerator
        (CancellationToken cancellationToken = default);
    #endregion
}

public sealed class QueryAnnotationCustomer
{
    #region Properties
    public int Id { get; set; }
    #endregion
}

[ApiQuerySource(ApiOutputComposition = ApiQueryComposition.Projection | ApiQueryComposition.Filtering)]
public delegate IReadOnlyList<QueryAnnotationCustomer> NamespaceCustomerSearch
(
    [ApiQuerySourceParameter(ApiNullability = ApiNullability.Nullable, ApiPresence = ApiPresence.Optional)]
    string? customerName, IReadOnlyList<IReadOnlyList<int?>?> ids
);

public sealed class OrdinaryQuerySourceDeclarations
{
    #region Test Types
    [ApiQuerySource(ApiOutputComposition = ApiQueryComposition.Projection | ApiQueryComposition.Filtering)]
    public delegate IReadOnlyList<QueryAnnotationCustomer> CustomerSearch
    (
        [ApiQuerySourceParameter(ApiNullability = ApiNullability.Nullable, ApiPresence = ApiPresence.Optional)]
        string? customerName, IReadOnlyList<IReadOnlyList<int?>?> ids
    );
    #endregion
}
