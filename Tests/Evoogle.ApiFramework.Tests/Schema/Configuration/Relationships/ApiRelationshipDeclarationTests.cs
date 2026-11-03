// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Exceptions;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships;

public sealed class ApiRelationshipDeclarationTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] AnnotationOriginTheoryData =>
    [
        new ApiDeclarationBuildTest
        {
            Name = "Independent custom reader declarations are not merged by occurrence ordinal",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildIndependentAnnotationReaders(),
            ExpectedRelationshipNames = ["First", "Second"]
        },
        new ApiDeclarationBuildTest
        {
            Name = "Repeated built-in readers preserve each physical attribute occurrence once",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildRepeatedAttributeOccurrences(),
            ExpectedRelationshipNames = ["First", "Second"]
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] AdditionalResolutionTheoryData =>
    [
        new ApiDeclarationBuildTest
        {
            Name = "One navigation member cannot bind both endpoint roles",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildSameMemberBothRoles(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict
        },
        new ApiDeclarationBuildTest
        {
            Name = "A claimed endpoint cannot acquire a different authoritative member",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildOccupiedEndpointConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict,
            ExpectedApiPath = "ApiSchema[\"Relationships\"].ApiRelationshipOneToMany[\"First\"]"
        },
        new ApiDeclarationBuildTest
        {
            Name = "Many-to-many declarations resolve paired locators supplied in inverse order",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildReversedManyToMany(),
            ExpectedRelationshipNames = ["Second"], ExpectedTraversals = ["Customer.Tags:Tags",
                "Tag.Customers:Customers"]
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new ApiDeclarationBuildTest
        {
            Name = "Same endpoint types do not merge unbound relationships",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildIndependentNames(),
            ExpectedRelationshipNames = ["First", "Second"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Same-name unbound declarations remain independent until compilation",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildDuplicateNames(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiSchemaDuplicateRelationshipApiName
        },

        new ApiDeclarationBuildTest
        {
            Name = "Same-name declarations of different kinds produce a final name conflict",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildDuplicateNamesAcrossKinds(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiSchemaDuplicateRelationshipApiName
        },

        new ApiDeclarationBuildTest
        {
            Name = "Two explicit navigation members locate one relationship",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildPairedMembers(),
            ExpectedRelationshipNames = ["Orders"], ExpectedTraversals = ["Customer.Orders:Orders",
                "Order.Customer:Customer"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Configuration through an inverse navigation targets the same relationship",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildConfigureThroughInverse(),
            ExpectedRelationshipNames = ["Renamed"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Repeated member declarations attach unclaimed inverse locators",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildRepeatedMemberDeclarations(),
            ExpectedRelationshipNames = ["Second"], ExpectedTraversals = ["Customer.Orders:Orders",
                "Order.Customer:Customer"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Explicit inverse association attaches an unclaimed locator",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildAttachInverse(),
            ExpectedRelationshipNames = ["Orders"], ExpectedTraversals = ["Customer.Orders:Orders",
                "Order.Customer:Customer"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Different navigation members preserve distinct relationships between the same endpoints",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildDistinctMemberRelationships(),
            ExpectedRelationshipNames = ["First", "Second"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Locators identifying two existing symbols are never merged",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildSplitLocatorConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode
                .ApiRelationshipDeclarationConflict, ExpectedApiPath =
                    "ApiSchema[\"Relationships\"].ApiRelationship[\"Combined\"]"
        },

        new ApiDeclarationBuildTest
        {
            Name = "Inverse association rejects a locator already claimed by another relationship",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildClaimedInverseConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipDeclarationConflict
        },

        new ApiDeclarationBuildTest
        {
            Name = "Equal-precedence contributions cannot change a locator relationship kind",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildKindConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict
        },

        new ApiDeclarationBuildTest
        {
            Name = "Lower-precedence incompatible relationship kinds are ignored",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildLowerKindIgnored(),
            ExpectedRelationshipNames = ["Orders"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "A collection member cannot define a to-one traversal",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildCardinalityConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict
        },

        new ApiDeclarationBuildTest
        {
            Name = "A navigation target CLR type is authoritative",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildTargetConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict
        },

        new ApiDeclarationBuildTest
        {
            Name = "A later endpoint replacement cannot override a navigation locator",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildEndpointConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict
        },

        new ApiDeclarationBuildTest
        {
            Name = "A later CLR traversal replacement cannot override a navigation locator",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildBindingConflict(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict,
            ExpectedApiPath = "ApiSchema[\"Relationships\"].ApiRelationshipOneToMany[\"Invalid\"]"
        },

        new ApiDeclarationBuildTest
        {
            Name = "Explicit traversal names preserve authoritative CLR binding",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildRenamedTraversal(),
            ExpectedTraversals = ["Customer.Purchases:Orders"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Traversal naming conventions preserve member identity and explicit relationship names",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildConventionTraversal(),
            ExpectedRelationshipNames = ["ExactRelationship"], ExpectedTraversals = ["Customer.orders:Orders",
                "Order.customer:Customer"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Later traversal configuration never silently establishes declaration identity",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildLaterBindingDoesNotLocate(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipTraversalDuplicateApiName
        },

        new ApiDeclarationBuildTest
        {
            Name = "One-to-one navigation locators bind both ends",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildOneToOne(),
            ExpectedRelationshipNames = ["Primary"], ExpectedTraversals = [
                "Customer.PrimaryOrder:PrimaryOrder", "Order.Customer:Customer"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Many-to-many locators support configuration from either endpoint",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildManyToMany(),
            ExpectedRelationshipNames = ["Membership"], ExpectedTraversals = ["Customer.Tags:Tags",
                "Tag.Customers:Customers"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Public instance fields can locate relationships",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildFieldNavigation(),
            ExpectedTraversals = ["Customer.OrderField:OrderField"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Inherited members retain their reflected declaration identity",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildInheritedNavigation(),
            ExpectedTraversals = ["Derived.Orders:Orders"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Hidden members have identities distinct from base members",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildHiddenNavigation(),
            ExpectedRelationshipNames = ["Base", "Hidden"], ExpectedTraversals = ["Customer.Orders:Orders",
                "Hidden.Orders:Orders"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Self-referencing relationships retain two distinct navigation roles",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildSelfReference(),
            ExpectedTraversals = ["Employee.Manager:Manager", "Employee.Reports:Reports"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Relationship attributes explicitly associate inverse navigation members",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildAttributeInverse(),
            ExpectedRelationshipNames = ["AnnotatedOrders"], ExpectedTraversals = ["Customer.Orders:Orders",
                "Order.Customer:Customer"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Member annotations converge with explicit configuration by locator",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildAttributeExplicitConvergence(),
            ExpectedRelationshipNames = ["Explicit"], ExpectedTraversals = ["Customer.Orders:Orders",
                "Order.Customer:Customer"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Many-to-many attributes bind their explicit inverse member",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildAttributeManyToMany(),
            ExpectedRelationshipNames = ["AnnotatedTags"], ExpectedTraversals = ["Customer.Tags:Tags",
                "Tag.Customers:Customers"]
        },

        new ApiDeclarationBuildTest
        {
            Name = "Unresolved inverse attribute names produce navigation diagnostics",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildInvalidInverse(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipInvalidNavigationLocator
        },

        new ApiDeclarationBuildTest
        {
            Name = "Blank inverse attribute names produce navigation diagnostics",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildBlankInverse(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipInvalidNavigationLocator
        },

        new ApiDeclarationBuildTest
        {
            Name = "Type-level relationship occurrences remain independent declarations",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildTypeDefinitions(),
            ExpectedRelationshipNames = ["First", "Second"], ExpectedTraversals = []
        },

        new ApiDeclarationBuildTest
        {
            Name = "Member-targeted configuration requires an existing relationship",
            BuildExpression = static () => ApiRelationshipDeclarationTestsFactory.BuildUnresolvedMemberConfiguration(),
            ExpectedSuccess = false, ExpectedCode = ApiSchemaCompilationCode.ApiRelationshipInvalidNavigationLocator
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] AdditionalHandleTheoryData =>
    [
        new ApiBuildObservationTest
        {
            Name = "One-to-one registration handles survive sequential builds",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.ObserveOneToOneHandleAcrossBuilds(),
            ExpectedObservation = "First|Second"
        },
        new ApiBuildObservationTest
        {
            Name = "Many-to-many registration handles survive sequential builds",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory
                .ObserveManyToManyHandleAcrossBuilds(),
            ExpectedObservation = "First|Second"
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] HandleTheoryData =>
    [
        new ApiBuildObservationTest
        {
            Name = "Relationship registration handles survive sequential builds",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.ObserveHandleAcrossBuilds(),
            ExpectedObservation = "First|Second|False"
        },
        new ApiBuildObservationTest
        {
            Name = "Nested navigation selectors fail immediately",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.RejectNestedSelector(),
            ExpectedExceptionType = typeof(ArgumentException)
        },
        new ApiBuildObservationTest
        {
            Name = "ConfigureForeign OneToOne registration handles fail at the call boundary",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.ConfigureForeignOneToOne(),
            ExpectedExceptionType = typeof(ApiSchemaConfigurationException)
        },

        new ApiBuildObservationTest
        {
            Name = "Serialize OneToOne registration handles fail at the call boundary",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.SerializeOneToOne(),
            ExpectedExceptionType = typeof(NotSupportedException)
        },

        new ApiBuildObservationTest
        {
            Name = "Deserialize OneToOne registration handles fail at the call boundary",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.DeserializeOneToOne(),
            ExpectedExceptionType = typeof(NotSupportedException)
        },

        new ApiBuildObservationTest
        {
            Name = "ConfigureForeign OneToMany registration handles fail at the call boundary",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.ConfigureForeignOneToMany(),
            ExpectedExceptionType = typeof(ApiSchemaConfigurationException)
        },

        new ApiBuildObservationTest
        {
            Name = "Serialize OneToMany registration handles fail at the call boundary",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.SerializeOneToMany(),
            ExpectedExceptionType = typeof(NotSupportedException)
        },

        new ApiBuildObservationTest
        {
            Name = "Deserialize OneToMany registration handles fail at the call boundary",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.DeserializeOneToMany(),
            ExpectedExceptionType = typeof(NotSupportedException)
        },

        new ApiBuildObservationTest
        {
            Name = "ConfigureForeign ManyToMany registration handles fail at the call boundary",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.ConfigureForeignManyToMany(),
            ExpectedExceptionType = typeof(ApiSchemaConfigurationException)
        },

        new ApiBuildObservationTest
        {
            Name = "Serialize ManyToMany registration handles fail at the call boundary",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.SerializeManyToMany(),
            ExpectedExceptionType = typeof(NotSupportedException)
        },

        new ApiBuildObservationTest
        {
            Name = "Deserialize ManyToMany registration handles fail at the call boundary",
            ObserveExpression = static () => ApiRelationshipDeclarationTestsFactory.DeserializeManyToMany(),
            ExpectedExceptionType = typeof(NotSupportedException)
        },
    ];
    #endregion

    #region Theory Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    [MemberData(nameof(AdditionalResolutionTheoryData))]
    [MemberData(nameof(AnnotationOriginTheoryData))]
    public void BuildResolvesNavigationDeclarations(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(HandleTheoryData))]
    [MemberData(nameof(AdditionalHandleTheoryData))]
    public void HandlesAndSelectorsEnforceTheirContracts(IXUnitTest test)
        => test.Execute(this);
    #endregion
}
