// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.Configuration.Query;

public sealed class ApiQuerySourceDeclarationTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] RegisteredAnnotationTheoryData =>
    [
        new ApiDeclarationBuildTest
        {
            Name = "Explicitly registered delegates read their configured annotations without assembly discovery",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildRegisteredAnnotationWithoutScan(),
            ExpectedSourceNames = ["Annotated"], ExpectedParameterNames = ["annotatedId"],
            ExpectedPresence = ApiPresence.Optional, ExpectedComposition = ApiQueryComposition.Projection
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] RenamedFluentParameterTheoryData =>
    [
        new ApiDeclarationBuildTest
        {
            Name = "Pure fluent parameter reconfiguration uses its original declaration name after renaming",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildRenamedFluentParameter(),
            ExpectedParameterNames = ["customerId"], ExpectedPresence = ApiPresence.Optional
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] CollectionFacetTheoryData =>
    [
        new ApiDeclarationBuildTest
        {
            Name = "Collection-item output nullability cannot be applied to scalar output",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInvalidOutputItemFacet(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceConfigurationConflict
        },
        new ApiDeclarationBuildTest
        {
            Name = "Collection-item parameter nullability cannot be applied to a scalar parameter",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInvalidParameterItemFacet(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceConfigurationConflict
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new ApiDeclarationBuildTest
        {
            Name = "Delegate signatures infer CLR types and preserve parameter order",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInferredDelegate(),
            ExpectedSourceNames = ["LookupPair"], ExpectedParameterNames = ["id", "term"],
                ExpectedParameterTypes = [typeof(int), typeof(string)], ExpectedOutputClrType =
                    typeof(ApiQuerySourceDeclarationTestsFactory.Customer)
        },

        new ApiDeclarationBuildTest
        {
            Name = "Delegate collections infer output item nullability",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInferredCollection(),
            ExpectedSourceNames = ["Search"], ExpectedItemNullability = ApiNullability.Nullable,
                ExpectedParameterNames = ["term", "ids"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Public closed generic delegates are supported",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildClosedGeneric(),
            ExpectedSourceNames = ["Generic"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Repeated delegate registrations configure one declaration",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildRepeatedDelegate(),
            ExpectedSourceNames = ["Renamed"], ExpectedComposition = ApiQueryComposition.Projection
        },

        new ApiDeclarationBuildTest
        {
            Name = "Equivalent signatures with different delegate types remain distinct",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildEquivalentSignatures(),
            ExpectedSourceNames = ["First", "Second"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Fluent sources with equivalent signatures remain distinct",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildIndependentFluentSources(),
            ExpectedSourceNames = ["First", "Second"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Same-name fluent registrations fail schema compilation",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildDuplicateFluentNames(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode
                .ApiSchemaDuplicateQuerySourceApiName, ExpectedApiPath = "ApiSchema[\"Declarations\"]"
        },

        new ApiDeclarationBuildTest
        {
            Name = "Different delegate types with the same final name are diagnosed",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildDuplicateDelegateNames(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiSchemaDuplicateQuerySourceApiName
        },

        new ApiDeclarationBuildTest
        {
            Name = "Generic and annotation contributions share identity with explicit precedence",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildExplicitAnnotationFacets(),
            ExpectedSourceNames = ["Explicit"], ExpectedParameterNames = ["explicitId"], ExpectedPresence =
                ApiPresence.Required, ExpectedComposition = ApiQueryComposition.None
        },

        new ApiDeclarationBuildTest
        {
            Name = "Delegate output CLR structure cannot be replaced",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildSignatureOutputConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceDeclarationConflict
        },

        new ApiDeclarationBuildTest
        {
            Name = "Delegate parameter CLR types cannot be replaced",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildSignatureParameterConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceDeclarationConflict
        },

        new ApiDeclarationBuildTest
        {
            Name = "Additional delegate parameters are rejected",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildAdditionalParameterConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceDeclarationConflict
        },

        new ApiDeclarationBuildTest
        {
            Name = "Unknown CLR parameter configuration is diagnosed",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildUnknownParameterConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceDeclarationConflict
        },

        new ApiDeclarationBuildTest
        {
            Name = "Delegate API facets remain independently configurable",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildMutableFacets(),
            ExpectedParameterNames = ["query", "ids"], ExpectedOutputNullability = ApiNullability.Nullable,
                ExpectedItemNullability = ApiNullability.NonNullable, ExpectedPresence = ApiPresence.Optional,
                    ExpectedParameterNullability = ApiNullability.NonNullable
        },

        new ApiDeclarationBuildTest
        {
            Name = "Renaming parameters preserves CLR identity and declaration order",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildRenamedParameterOrder(),
            ExpectedParameterNames = ["Zulu", "Alpha"], ExpectedPresence = ApiPresence.Optional
        },

        new ApiDeclarationBuildTest
        {
            Name = "Generic configuration instances require no API name property",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildGenericConfiguration(),
            ExpectedSourceNames = ["Configured"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Discovery distinguishes closed generic roles and deduplicates repeated scans",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildDiscoveredGenericConfigurations(),
            ExpectedSourceNames = ["First", "Second"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Parameterized non-generic configuration instances remain independent",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory
                .BuildIndependentConfigurationInstances(),
            ExpectedSourceNames = ["First", "Second"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Unanchored fluent source names are convention candidates",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildFluentNamingCandidate(),
            ExpectedSourceNames = ["customer-search"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Explicit WithName overrides conventions without CLR metadata",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildPinnedFluentName(),
            ExpectedSourceNames = ["ExactName"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Invalid NonDelegate produces a configuration diagnostic",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInvalidNonDelegate(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceInvalidDelegate
        },

        new ApiDeclarationBuildTest
        {
            Name = "Invalid VoidDelegate produces a configuration diagnostic",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInvalidVoidDelegate(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceInvalidDelegate
        },

        new ApiDeclarationBuildTest
        {
            Name = "Invalid OpenDelegate produces a configuration diagnostic",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInvalidOpenDelegate(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceInvalidDelegate
        },

        new ApiDeclarationBuildTest
        {
            Name = "Invalid PrivateDelegate produces a configuration diagnostic",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInvalidPrivateDelegate(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceInvalidDelegate
        },

        new ApiDeclarationBuildTest
        {
            Name = "Invalid RefDelegate produces a configuration diagnostic",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInvalidRefDelegate(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceInvalidDelegate
        },

        new ApiDeclarationBuildTest
        {
            Name = "Invalid AsyncDelegate produces a configuration diagnostic",
            BuildExpression = static () => ApiQuerySourceDeclarationTestsFactory.BuildInvalidAsyncDelegate(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiQuerySourceInvalidDelegate
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] HandleTheoryData =>
    [
        new ApiBuildObservationTest
        {
            Name = "Registration handles survive sequential builds and preserve earlier schemas",
            ObserveExpression = static () => ApiQuerySourceDeclarationTestsFactory.ObserveHandleAcrossBuilds(),
            ExpectedObservation = "First|Second|False"
        },
        new ApiBuildObservationTest
        {
            Name = "Foreign query-source handles fail immediately",
            ObserveExpression = static () => ApiQuerySourceDeclarationTestsFactory.ConfigureForeignHandle(),
            ExpectedExceptionType = typeof(ApiSchemaConfigurationException)
        },
        new ApiBuildObservationTest
        {
            Name = "Query-source handle JSON serialization is unsupported",
            ObserveExpression = static () => ApiQuerySourceDeclarationTestsFactory.SerializeHandle(),
            ExpectedExceptionType = typeof(NotSupportedException)
        },
        new ApiBuildObservationTest
        {
            Name = "Query-source handle JSON deserialization is unsupported",
            ObserveExpression = static () => ApiQuerySourceDeclarationTestsFactory.DeserializeHandle(),
            ExpectedExceptionType = typeof(NotSupportedException)
        },
    ];
    #endregion

    #region Theory Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    [MemberData(nameof(CollectionFacetTheoryData))]
    [MemberData(nameof(RenamedFluentParameterTheoryData))]
    [MemberData(nameof(RegisteredAnnotationTheoryData))]
    public void BuildResolvesStableDeclarations(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(HandleTheoryData))]
    public void HandlesPreserveOwnershipAndLifetime(IXUnitTest test)
        => test.Execute(this);
    #endregion
}
