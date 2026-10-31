// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Query;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

public sealed class ApiQuerySourceAnnotationTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class BuildAnnotatedQuerySourceTest : XUnitTest
    {
        [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchemaCompilationResult>))]
        public required Expression<Func<ApiSchemaCompilationResult>> BuildExpression { get; init; }

        public required bool ExpectedSuccess { get; init; }

        public ApiSchemaCompilationCode? ExpectedCode { get; init; }
        public string ExpectedApiName { get; init; } = "customerSearch";
        public string ExpectedFirstParameterApiName { get; init; } = "customerName";

        private Expression<Func<ApiSchemaCompilationResult>> RestoredBuildExpression { get; set; } = null!;
        private ApiSchemaCompilationResult ActualResult { get; set; } = null!;

        protected override void Arrange()
        {
            this.WriteLine($"Scenario:         {this.Name}");
            this.WriteLine($"Build Expression: {this.BuildExpression}");
            this.WriteLine();
            this.WriteLine($"Expected Success: {this.ExpectedSuccess}");

            var serializer = new XUnitTestSerializer();
            var restored = (BuildAnnotatedQuerySourceTest)serializer.Deserialize
                (this.GetType(), serializer.Serialize(this));
            this.RestoredBuildExpression = restored.BuildExpression;
        }

        protected override void Act()
        {
            this.ActualResult = this.RestoredBuildExpression.Compile()();

            this.WriteLine();
            this.WriteLine($"Actual Success: {this.ActualResult.IsValid}");
            this.WriteLine($"Issue Count:    {this.ActualResult.Issues.Length}");
            foreach (var issue in this.ActualResult.Issues)
            {
                this.WriteLine($"Issue:          {issue.Code}: {issue.Description}");
            }
        }

        protected override void Assert()
        {
            this.ActualResult.IsValid.Should().Be(this.ExpectedSuccess);
            if (!this.ExpectedSuccess)
            {
                this.ActualResult.Issues.Should().Contain
                    (issue => issue.Code == this.ExpectedCode);
                return;
            }

            var source = this.ActualResult.Schema!.ApiQuerySources.Should().ContainSingle().Subject;
            source.ApiName.Should().Be(this.ExpectedApiName);
            source.ApiOutputNullability.Should().Be(ApiNullability.NonNullable);
            source.ApiOutputComposition.Should().Be
                (ApiQueryComposition.Projection | ApiQueryComposition.Filtering);

            var outputType = source.ApiOutputType.Should().BeOfType<ApiCollectionType>().Subject;
            outputType.ApiItemType.ClrType.Should().Be<QueryAnnotationCustomer>();
            outputType.ApiItemNullability.Should().Be(ApiNullability.NonNullable);

            source.ApiInputParameters.Should().HaveCount(2);
            var name = source.ApiInputParameters[0];
            name.ApiName.Should().Be(this.ExpectedFirstParameterApiName);
            name.ApiNullability.Should().Be(ApiNullability.Nullable);
            name.ApiPresence.Should().Be(ApiPresence.Optional);

            var ids = source.ApiInputParameters[1];
            ids.ApiName.Should().Be("ids");
            var idsType = ids.ApiType.Should().BeOfType<ApiCollectionType>().Subject;
            idsType.ApiItemNullability.Should().Be(ApiNullability.Nullable);
            var nestedIdsType = idsType.ApiItemType.Should().BeOfType<ApiCollectionType>().Subject;
            nestedIdsType.ApiItemType.ClrType.Should().Be<int>();
            nestedIdsType.ApiItemNullability.Should().Be(ApiNullability.Nullable);
        }
    }

    private sealed class BuildQuerySourcePrecedenceTest : XUnitTest
    {
        [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchema>))]
        public required Expression<Func<ApiSchema>> BuildExpression { get; init; }

        public required string Scenario { get; init; }

        private Expression<Func<ApiSchema>> RestoredBuildExpression { get; set; } = null!;
        private ApiSchema ActualSchema { get; set; } = null!;

        protected override void Arrange()
        {
            this.WriteLine($"Scenario:         {this.Scenario}");
            this.WriteLine($"Build Expression: {this.BuildExpression}");
            this.WriteLine();
            this.WriteLine("Expected Exception: None");

            var serializer = new XUnitTestSerializer();
            var restored = (BuildQuerySourcePrecedenceTest)serializer.Deserialize
                (this.GetType(), serializer.Serialize(this));
            this.RestoredBuildExpression = restored.BuildExpression;
        }

        protected override void Act()
        {
            this.ActualSchema = this.RestoredBuildExpression.Compile()();

            this.WriteLine();
            this.WriteLine($"Query Source Count: {this.ActualSchema.ApiQuerySources.Length}");
            this.WriteLine($"Query Source:       {this.ActualSchema.ApiQuerySources.Single().ApiName}");
        }

        protected override void Assert()
        {
            var source = this.ActualSchema.ApiQuerySources.Should().ContainSingle().Subject;
            if (this.Scenario == "Explicit")
            {
                source.ApiOutputType.ClrType.Should().Be<string>();
                source.ApiOutputNullability.Should().Be(ApiNullability.Nullable);
                source.ApiOutputComposition.Should().Be(ApiQueryComposition.None);
                source.ApiInputParameters[0].ApiPresence.Should().Be(ApiPresence.Required);
                source.ApiInputParameters[0].ApiNullability.Should().Be(ApiNullability.NonNullable);
                return;
            }

            if (this.Scenario == "Convention")
            {
                source.ApiOutputComposition.Should().Be(ApiQueryComposition.Filtering);
                source.ApiInputParameters.Single().ApiPresence.Should().Be(ApiPresence.Optional);
                source.ApiInputParameters.Single().ApiNullability.Should().Be(ApiNullability.Nullable);
                return;
            }

            if (this.Scenario == "AnnotatedConvention")
            {
                source.ApiOutputComposition.Should().Be(ApiQueryComposition.Filtering);
                source.ApiInputParameters.Should().HaveCount(2);
                source.ApiInputParameters.Should().OnlyContain
                    (parameter => parameter.ApiPresence == ApiPresence.Optional);
                return;
            }

            source.ApiOutputComposition.Should().Be
                (ApiQueryComposition.Projection | ApiQueryComposition.Filtering);
            source.ApiInputParameters.Should().HaveCount(2);
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildAnnotatedQuerySourceTheoryData =>
    [
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Public delegate builds a named query source with inferred collection types",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildValidDeclaration(),
            ExpectedSuccess = true
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Namespace-level public delegate is discovered",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildNamespaceDeclaration(),
            ExpectedSuccess = true,
            ExpectedApiName = "namespaceCustomerSearch"
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Public delegate nested in an ordinary class is discovered",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildOrdinaryNestedDeclaration(),
            ExpectedSuccess = true
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Explicit annotation names bypass naming conventions",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildExplicitNamesDeclaration(),
            ExpectedSuccess = true,
            ExpectedApiName = "ExactQuery",
            ExpectedFirstParameterApiName = "ExactParameter"
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Custom annotation reader contributes a delegate declaration",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildCustomReaderDeclaration(),
            ExpectedSuccess = true
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Query-source naming target does not transform parameters",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildSourceNamingOnly(),
            ExpectedSuccess = true,
            ExpectedApiName = "customer-search",
            ExpectedFirstParameterApiName = "customerName"
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Query-source-parameter naming target does not transform sources",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildParameterNamingOnly(),
            ExpectedSuccess = true,
            ExpectedApiName = "CustomerSearch",
            ExpectedFirstParameterApiName = "customer-name"
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Delegate discovery does not register referenced schema types",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildMissingReferencedTypes(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiCollectionTypeUnresolvedItemType
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Task output reports an invalid annotation declaration",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildInvalidTaskDeclaration(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "ValueTask output reports an invalid annotation declaration",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildInvalidValueTaskDeclaration(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Async enumerable output reports an invalid annotation declaration",
            BuildExpression = static () =>
                ApiQuerySourceAnnotationTestsFactory.BuildInvalidAsyncEnumerableDeclaration(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Void output reports an invalid annotation declaration",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildInvalidVoidDeclaration(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Generic delegate reports an invalid annotation declaration",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildInvalidGenericDeclaration(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "By-reference output reports an invalid annotation declaration",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildInvalidRefReturnDeclaration(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "By-reference parameter reports an invalid annotation declaration",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildInvalidRefParameterDeclaration(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Pointer output reports an invalid annotation declaration",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildInvalidPointerReturnDeclaration(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Pointer parameter reports an invalid annotation declaration",
            BuildExpression = static () =>
                ApiQuerySourceAnnotationTestsFactory.BuildInvalidPointerParameterDeclaration(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationInvalidDeclaration
        },
        new BuildAnnotatedQuerySourceTest
        {
            Name = "Duplicate resolved API names report an annotation discovery conflict",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildDuplicateDeclarations(),
            ExpectedSuccess = false,
            ExpectedCode = ApiSchemaCompilationCode.ApiAnnotationQuerySourceDiscoveryConflict
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildQuerySourcePrecedenceTheoryData =>
    [
        new BuildQuerySourcePrecedenceTest
        {
            Name = "Explicit fluent settings override an annotation declaration",
            Scenario = "Explicit",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildExplicitPrecedence()
        },
        new BuildQuerySourcePrecedenceTest
        {
            Name = "Custom query conventions configure fluent defaults with null CLR contexts",
            Scenario = "Convention",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildCustomConventions()
        },
        new BuildQuerySourcePrecedenceTest
        {
            Name = "Custom query conventions receive annotation CLR contexts and added parameters",
            Scenario = "AnnotatedConvention",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildAnnotatedCustomConventions()
        },
        new BuildQuerySourcePrecedenceTest
        {
            Name = "Repeated assembly annotation scanning is idempotent",
            Scenario = "Repeated",
            BuildExpression = static () => ApiQuerySourceAnnotationTestsFactory.BuildRepeatedScan()
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildAnnotatedQuerySourceTheoryData))]
    public void BuildDiscoversQuerySourceDelegates(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildQuerySourcePrecedenceTheoryData))]
    public void BuildPreservesQuerySourcePrecedenceAndConventionBehavior(IXUnitTest test)
        => test.Execute(this);
    #endregion
}
