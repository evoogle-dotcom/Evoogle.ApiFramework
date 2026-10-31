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
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;

        public void Apply(ApiQuerySourceBuilder builder, ApiQuerySourceConventionContext context)
        {
            if ((context.ClrDelegateType is not null) != expectsClrMetadata)
            {
                throw new InvalidOperationException("The query-source CLR context did not match the declaration.");
            }

            builder.WithOutputComposition(ApiQueryComposition.Filtering);
            builder.AddInputParameter<string>("addedByConvention");
        }
    }

    private sealed class TestQuerySourceParameterConvention(bool expectsSourceClrMetadata) :
        IApiQuerySourceParameterConvention
    {
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;

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
    }

    private sealed class TestQuerySourceAnnotationReader : IApiQuerySourceAnnotationReader
    {
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
                        ApiQueryComposition.Projection | ApiQueryComposition.Filtering,
                        [
                            new(parameters[0], "customerName", ApiNullability.Nullable, ApiPresence.Optional),
                            new(parameters[1], "ids", null, null)
                        ]
                    )
                ],
                []
            );
        }
    }
    #endregion

    #region Factory Methods
    public static ApiSchemaCompilationResult BuildValidDeclaration() => BuildValidDeclaration(typeof(CustomerSearch));

    public static ApiSchemaCompilationResult BuildNamespaceDeclaration() => BuildValidDeclaration
        (typeof(NamespaceCustomerSearch));

    public static ApiSchemaCompilationResult BuildOrdinaryNestedDeclaration() => BuildValidDeclaration
        (typeof(OrdinaryQuerySourceDeclarations.CustomerSearch));

    public static ApiSchemaCompilationResult BuildExplicitNamesDeclaration() => BuildValidDeclaration
        (typeof(ExplicitNamesQuery));

    public static ApiSchemaCompilationResult BuildMissingReferencedTypes() => new ApiSchemaBuilder()
        .WithName("MissingQuerySourceTypes")
        .UseDefaultAnnotations()
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            type => type == typeof(CustomerSearch)
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildCustomReaderDeclaration() => new ApiSchemaBuilder()
        .WithName("CustomQuerySourceAnnotationReader")
        .AddScalar<int>()
        .AddScalar<string>()
        .AddObject<QueryAnnotationCustomer>(customer => customer.AddProperty(property => property.Id))
        .UseAnnotations(readers => readers.AddReader(new TestQuerySourceAnnotationReader()))
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            type => type == typeof(CustomReaderQuery)
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildSourceNamingOnly() => BuildValidDeclaration
        (
            typeof(CustomerSearch),
            ApiNamingConventionTargets.QuerySource,
            useKebabCaseNaming: true
        );

    public static ApiSchemaCompilationResult BuildParameterNamingOnly() => BuildValidDeclaration
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
            .AddObject<QueryAnnotationCustomer>(customer => customer.AddProperty(property => property.Id))
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

    public static ApiSchemaCompilationResult BuildInvalidTaskDeclaration() => new ApiSchemaBuilder()
        .WithName("InvalidTaskQuerySourceAnnotation")
        .UseDefaultAnnotations()
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            type => type == typeof(InvalidTaskQuery)
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildInvalidValueTaskDeclaration() => BuildInvalidDeclaration
        (typeof(InvalidValueTaskQuery));

    public static ApiSchemaCompilationResult BuildInvalidAsyncEnumerableDeclaration() => BuildInvalidDeclaration
        (typeof(InvalidAsyncEnumerableQuery));

    public static ApiSchemaCompilationResult BuildInvalidVoidDeclaration() => BuildInvalidDeclaration
        (typeof(InvalidVoidQuery));

    public static ApiSchemaCompilationResult BuildInvalidGenericDeclaration() => BuildInvalidDeclaration
        (typeof(InvalidGenericQuery<>));

    public static ApiSchemaCompilationResult BuildInvalidRefReturnDeclaration() => BuildInvalidDeclaration
        (typeof(InvalidRefReturnQuery));

    public static ApiSchemaCompilationResult BuildInvalidRefParameterDeclaration() => BuildInvalidDeclaration
        (typeof(InvalidRefParameterQuery));

    public static ApiSchemaCompilationResult BuildInvalidPointerReturnDeclaration() => BuildInvalidDeclaration
        (typeof(InvalidPointerReturnQuery));

    public static ApiSchemaCompilationResult BuildInvalidPointerParameterDeclaration() => BuildInvalidDeclaration
        (typeof(InvalidPointerParameterQuery));

    private static ApiSchemaCompilationResult BuildInvalidDeclaration(Type clrDelegateType) => new ApiSchemaBuilder()
        .WithName("InvalidQuerySourceAnnotation")
        .UseDefaultAnnotations()
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            type => type == clrDelegateType
        )
        .BuildResult();

    public static ApiSchemaCompilationResult BuildDuplicateDeclarations() => new ApiSchemaBuilder()
        .WithName("DuplicateQuerySourceAnnotations")
        .AddScalar<int>()
        .AddObject<QueryAnnotationCustomer>(customer => customer.AddProperty(property => property.Id))
        .UseDefaultAnnotations()
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            type => type == typeof(FirstDuplicate) || type == typeof(SecondDuplicate)
        )
        .BuildResult();

    public static ApiSchema BuildExplicitPrecedence() => new ApiSchemaBuilder()
        .WithName("ExplicitQuerySourcePrecedence")
        .AddScalar<int>()
        .AddScalar<string>()
        .AddObject<QueryAnnotationCustomer>(customer => customer.AddProperty(property => property.Id))
        .AddQuerySource("customerSearch", source => source
            .AddInputParameter<string>("customerName", parameter => parameter.AsRequired().AsNonNullable())
            .WithOutput<string>(ApiNullability.Nullable)
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
            type => type == typeof(CustomerSearch)
        )
        .Build();

    public static ApiSchema BuildCustomConventions() => new ApiSchemaBuilder()
        .WithName("CustomQuerySourceConventions")
        .AddScalar<string>()
        .AddQuerySource("Values", source => source.WithCollectionOutput<string>())
        .UseConventions(conventions => conventions
            .AddConvention(new TestQuerySourceConvention(expectsClrMetadata: false))
            .AddConvention(new TestQuerySourceParameterConvention(expectsSourceClrMetadata: false)))
        .Build();

    public static ApiSchema BuildAnnotatedCustomConventions() => new ApiSchemaBuilder()
        .WithName("AnnotatedCustomQuerySourceConventions")
        .AddScalar<int>()
        .AddScalar<string>()
        .AddObject<QueryAnnotationCustomer>(customer => customer.AddProperty(property => property.Id))
        .UseDefaultAnnotations()
        .UseConventions(conventions => conventions
            .AddConvention(new TestQuerySourceConvention(expectsClrMetadata: true))
            .AddConvention(new TestQuerySourceParameterConvention(expectsSourceClrMetadata: true)))
        .UseAssemblyAnnotationScanning
        (
            typeof(ApiQuerySourceAnnotationTestsFactory).Assembly,
            type => type == typeof(AnnotatedConventionQuery)
        )
        .Build();

    public static ApiSchema BuildRepeatedScan()
    {
        var assembly = typeof(ApiQuerySourceAnnotationTestsFactory).Assembly;
        return new ApiSchemaBuilder()
            .WithName("RepeatedQuerySourceScan")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<QueryAnnotationCustomer>(customer => customer.AddProperty(property => property.Id))
            .UseDefaultAnnotations()
            .UseCamelCaseNaming
            (
                ApiNamingConventionTargets.QuerySource |
                ApiNamingConventionTargets.QuerySourceParameter
            )
            .UseAssemblyAnnotationScanning(assembly, type => type == typeof(CustomerSearch))
            .UseAssemblyAnnotationScanning(assembly, type => type == typeof(CustomerSearch))
            .Build();
    }
    #endregion
}

public sealed class QueryAnnotationCustomer
{
    public int Id { get; set; }
}

[ApiQuerySource(ApiOutputComposition = ApiQueryComposition.Projection | ApiQueryComposition.Filtering)]
public delegate IReadOnlyList<QueryAnnotationCustomer> NamespaceCustomerSearch
(
    [ApiQuerySourceParameter(ApiNullability = ApiNullability.Nullable, ApiPresence = ApiPresence.Optional)]
    string? customerName, IReadOnlyList<IReadOnlyList<int?>?> ids
);

public sealed class OrdinaryQuerySourceDeclarations
{
    [ApiQuerySource(ApiOutputComposition = ApiQueryComposition.Projection | ApiQueryComposition.Filtering)]
    public delegate IReadOnlyList<QueryAnnotationCustomer> CustomerSearch
    (
        [ApiQuerySourceParameter(ApiNullability = ApiNullability.Nullable, ApiPresence = ApiPresence.Optional)]
        string? customerName, IReadOnlyList<IReadOnlyList<int?>?> ids
    );
}
