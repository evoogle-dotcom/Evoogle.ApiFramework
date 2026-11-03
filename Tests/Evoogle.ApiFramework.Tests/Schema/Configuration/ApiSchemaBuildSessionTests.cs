// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiSchemaBuildSessionTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] DurableHandleTheoryData =>
    [
        new ApiBuildObservationTest
        {
            Name = "Build-local declarations cannot escape as apparently durable registration handles",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveBuildTimeHandleRejection(),
            ExpectedObservation = "True|True|0"
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] IsolationTheoryData =>
    [
        new ApiBuildObservationTest
        {
            Name = "Naming conventions recompute scalar enum value object and property names",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveConventionChanges(),
            ExpectedObservation =
                "FirstInt32|SecondInt32|FirstPending|SecondPending|FirstCustomer|SecondCustomer|FirstId|SecondId"
        },

        new ApiBuildObservationTest
        {
            Name = "Mutable annotation results recompute type member key and version state",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveAnnotationChanges(),
            ExpectedObservation =
                "FirstInt32|SecondInt32|FirstPending|SecondPending|FirstK" +
                    "ey,PrimaryKey|PrimaryKey,SecondKey|FirstId|SecondRevision"
        },

        new ApiBuildObservationTest
        {
            Name = "Removed annotations do not retain prior derived configuration",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveRemovedAnnotations(),
            ExpectedObservation = "FirstCustomer|Customer|True|False"
        },

        new ApiBuildObservationTest
        {
            Name = "Changed assembly filters remove obsolete discovered types",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveAssemblyFilterChanges(),
            ExpectedObservation = "2|1"
        },

        new ApiBuildObservationTest
        {
            Name = "Discovered generic configuration mutates only copied key paths and versions",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveNestedConfigurationIsolation(),
            ExpectedObservation = "2|1|Id|Revision"
        },

        new ApiBuildObservationTest
        {
            Name = "Relationship end extension mutations do not leak into retained authoring",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveRelationshipNestedIsolation(),
            ExpectedObservation = "2|1|False"
        },

        new ApiBuildObservationTest
        {
            Name = "Discovery exceptions discard partially discovered declarations",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveDiscoveryFailureRecovery(),
            ExpectedObservation = "True|1|0"
        },

        new ApiBuildObservationTest
        {
            Name = "Materialization exceptions discard build-local declarations",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveMaterializationFailureRecovery(),
            ExpectedObservation = "True|1|0"
        },

        new ApiBuildObservationTest
        {
            Name = "Failed compilation preserves durable handles and accepts later corrections",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveCompilationFailureRecovery(),
            ExpectedObservation = "False|True|Source"
        },

        new ApiBuildObservationTest
        {
            Name = "Failed extension freezing does not poison a later build",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveFreezeFailureRecovery(),
            ExpectedObservation = "False|True"
        },

        new ApiBuildObservationTest
        {
            Name = "Fluent callbacks execute immediately and are never replayed",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveImmediateCallbacks(),
            ExpectedObservation = "True|1|1|Retained|Retained"
        },

        new ApiBuildObservationTest
        {
            Name = "Schema and object options callbacks remain deferred per build",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveDeferredOptions(),
            ExpectedObservation = "0:0|1:1|2:2"
        },

        new ApiBuildObservationTest
        {
            Name = "Edits through retained fluent builders apply only to later schemas",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveRetainedBuilderEdits(),
            ExpectedObservation = "Customer|Changed|False"
        },

        new ApiBuildObservationTest
        {
            Name = "Each build owns fresh properties keys versions inline types and parameters",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveFreshOwnedGraphs(),
            ExpectedObservation = "False|False|False|False|False|False"
        },

        new ApiBuildObservationTest
        {
            Name = "Reentrant builds are rejected and release the build guard",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveReentrantBuildRecovery(),
            ExpectedObservation = "True|True"
        },

        new ApiBuildObservationTest
        {
            Name = "Overlapping builds are rejected before mutating working state",
            ObserveExpression = static () => ApiSchemaBuildSessionTestsFactory.ObserveConcurrentBuildRejection(),
            ExpectedObservation = "True|True"
        },
    ];
    #endregion

    #region Theory Methods
    [Theory]
    [MemberData(nameof(IsolationTheoryData))]
    [MemberData(nameof(DurableHandleTheoryData))]
    public void BuildsIsolateDerivedConfigurationAndPreserveCallbackTiming(IXUnitTest test)
        => test.Execute(this);
    #endregion
}
