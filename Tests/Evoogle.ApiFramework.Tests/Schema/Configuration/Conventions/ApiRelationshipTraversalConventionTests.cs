// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Exceptions;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

public sealed class ApiRelationshipTraversalConventionTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new ApiDeclarationBuildTest
        {
            Name = "Custom traversal conventions run without a naming base class or query-source conventions",
            BuildExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.BuildCustomTraversalConvention(),
            ExpectedRelationshipNames = ["Orders"],
            ExpectedTraversals = ["Customer.CustomOrders:Orders", "Order.CustomCustomer:Customer"]
        },
        new ApiDeclarationBuildTest
        {
            Name = "Traversal conventions compose in registration order at convention precedence",
            BuildExpression = static () => ApiRelationshipTraversalConventionTestsFactory.BuildOrderedConventions(),
            ExpectedTraversals = ["Customer.SecondFirstOrders:Orders", "Order.SecondFirstCustomer:Customer"]
        },
        new ApiDeclarationBuildTest
        {
            Name = "Traversals added by relationship inference receive naming conventions",
            BuildExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.BuildAfterRelationshipInference(),
            ExpectedTraversals = ["Customer.orders:Orders", "Order.customer:Customer"]
        },
        new ApiDeclarationBuildTest
        {
            Name = "Explicit traversal names retain precedence over custom and built-in conventions",
            BuildExpression = static () => ApiRelationshipTraversalConventionTestsFactory.BuildExplicitTraversalNames(),
            ExpectedTraversals = ["Customer.Purchases:Orders", "Order.Buyer:Customer"]
        },
        new ApiDeclarationBuildTest
        {
            Name = "Query-source naming targets do not change traversal names",
            BuildExpression = static () => ApiRelationshipTraversalConventionTestsFactory.BuildOtherNamingTarget(),
            ExpectedTraversals = ["Customer.Orders:Orders", "Order.Customer:Customer"]
        },
        new ApiDeclarationBuildTest
        {
            Name = "The pipeline uses the traversal collection when the query-source collection is empty",
            BuildExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.BuildIndependentConventionList(),
            ExpectedTraversals = ["Customer.orders:Orders", "Order.customer:Customer"]
        },
        new ApiDeclarationBuildTest
        {
            Name = "Attribute navigation names remain convention candidates after annotation binding",
            BuildExpression = static () => ApiRelationshipTraversalConventionTestsFactory.BuildAnnotatedNavigations(),
            ExpectedTraversals = ["Customer.orders:Orders", "Order.customer:Customer"]
        },
        new ApiDeclarationBuildTest
        {
            Name = "Convention-created traversals support API-only references without CLR metadata",
            BuildExpression = static () => ApiRelationshipTraversalConventionTestsFactory.BuildWithoutClrNavigation(),
            ExpectedTraversals = ["Customer.orders:"]
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] ObservationTheoryData =>
    [
        new ApiBuildObservationTest
        {
            Name = "Traversal registration survives copying and removal including default naming conventions",
            ObserveExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.ObserveConventionRegistration(),
            ExpectedObservation = "True|True|0|0|True|True|0"
        },
        new ApiBuildObservationTest
        {
            Name = "Both endpoints of all relationship kinds receive bound property and field contexts",
            ObserveExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.ObserveAllNavigationContexts(),
            ExpectedObservation = "Customer.OrderField:Field|Customer.PrimaryOrder:Property|Customer.Tags:Property|" +
                "Order.Customer:Property|Order.OtherCustomer:Property|Tag.Customers:Property"
        },
        new ApiBuildObservationTest
        {
            Name = "Naming contexts carry traversal ownership and original CLR navigation metadata",
            ObserveExpression = static () => ApiRelationshipTraversalConventionTestsFactory.ObserveNamingContext(),
            ExpectedObservation = "RelationshipTraversal:Customer:Orders:null|RelationshipTraversal:Order:Customer:null"
        },
        new ApiBuildObservationTest
        {
            Name = "Unbound naming contexts carry API ownership with nullable CLR metadata",
            ObserveExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.ObserveUnboundNamingContext(),
            ExpectedObservation = "RelationshipTraversal:null:null:Customer"
        },
        new ApiBuildObservationTest
        {
            Name = "Repeated builds and convention removal preserve independent published traversals",
            ObserveExpression = static () => ApiRelationshipTraversalConventionTestsFactory.ObserveRepeatedBuilds(),
            ExpectedObservation = "NamedOrders|NamedOrders|Orders|False|NamedOrders"
        },
        new ApiBuildObservationTest
        {
            Name = "A retained principal traversal builder cannot mutate authoring state or published schemas",
            ObserveExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.ObserveRetainedPrincipalTraversal(),
            ExpectedObservation = "Original|Original"
        },
        new ApiBuildObservationTest
        {
            Name = "A retained dependent traversal builder cannot mutate authoring state or published schemas",
            ObserveExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.ObserveRetainedDependentTraversal(),
            ExpectedObservation = "Original|Original"
        },
        new ApiBuildObservationTest
        {
            Name = "A traversal convention exception does not retain earlier convention state in the next build",
            ObserveExpression = static () => ApiRelationshipTraversalConventionTestsFactory.ObserveFailureRecovery(),
            ExpectedObservation = "True|RecoveredFirstOrders"
        },
        new ApiBuildObservationTest
        {
            Name = "Traversal convention lists are snapshotted before discovery callbacks run",
            ObserveExpression = static () => ApiRelationshipTraversalConventionTestsFactory.ObserveScheduleSnapshot(),
            ExpectedObservation = "FirstOrders|SecondFirstOrders"
        },
        new ApiBuildObservationTest
        {
            Name = "Traversal conventions reject the discovery phase through normal schedule validation",
            ObserveExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.ObserveInvalidDiscoveryPhase(),
            ExpectedExceptionType = typeof(ApiSchemaConfigurationException)
        },
        new ApiBuildObservationTest
        {
            Name = "Traversal conventions reject the relationship phase through normal schedule validation",
            ObserveExpression = static () =>
                ApiRelationshipTraversalConventionTestsFactory.ObserveInvalidRelationshipPhase(),
            ExpectedExceptionType = typeof(ApiSchemaConfigurationException)
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(ObservationTheoryData))]
    public void Observe(IXUnitTest test)
        => test.Execute(this);
    #endregion
}
