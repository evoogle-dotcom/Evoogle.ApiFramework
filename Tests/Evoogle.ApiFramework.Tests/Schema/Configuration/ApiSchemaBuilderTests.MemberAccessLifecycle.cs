// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public partial class ApiSchemaBuilderTests
{
    #region Member Access Lifecycle Test Types
    private sealed class NavigationTarget
    {
        #region Properties
        public long Id { get; set; }
        public NavigationTarget? Related { get; set; }
        #endregion
    }

    private enum TraversalEndScenario
    {
        PrincipalEnd,
        DependentEnd,
        MergedDependentEnd
    }

    private sealed class RepeatedEndTraversalBuildTest : XUnitTest
    {
        #region User Supplied Properties
        public required TraversalEndScenario EndScenario { get; init; }
        public required string ExpectedApiName { get; init; }
        #endregion

        #region Calculated Properties
        private ApiRelationshipTraversal? FirstTraversal { get; set; }
        private ApiRelationshipTraversal? SecondTraversal { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:     An end builder configured with a traversal is built more than once");
            this.WriteLine($"End Scenario: {this.EndScenario}");
            this.WriteLine($"Expected:     distinct traversal instances, each named '{this.ExpectedApiName}'");
            this.WriteLine();
        }

        protected override void Act()
        {
            var clrNavigationMemberName = nameof(NavigationTarget.Related);
            switch (this.EndScenario)
            {
                case TraversalEndScenario.PrincipalEnd:
                    var principalBuilder = new ApiRelationshipPrincipalEndBuilder(typeof(NavigationTarget));
                    principalBuilder.WithTraversal(this.ExpectedApiName, clrNavigationMemberName);

                    this.FirstTraversal = principalBuilder.Build().ApiTraversal;
                    this.SecondTraversal = principalBuilder.Build().ApiTraversal;
                    break;

                case TraversalEndScenario.DependentEnd:
                    var dependentBuilder = new ApiRelationshipDependentEndBuilder(typeof(NavigationTarget));
                    dependentBuilder.WithTraversal(this.ExpectedApiName, clrNavigationMemberName);

                    this.FirstTraversal = dependentBuilder.Build().ApiTraversal;
                    this.SecondTraversal = dependentBuilder.Build().ApiTraversal;
                    break;

                case TraversalEndScenario.MergedDependentEnd:
                    var mergedFromBuilder = new ApiRelationshipDependentEndBuilder(typeof(NavigationTarget));
                    mergedFromBuilder.WithTraversal(this.ExpectedApiName, clrNavigationMemberName);

                    var mergedIntoBuilder = new ApiRelationshipDependentEndBuilder(typeof(NavigationTarget));
                    mergedIntoBuilder.MergeConfigurationFrom(mergedFromBuilder);

                    this.FirstTraversal = mergedFromBuilder.Build().ApiTraversal;
                    this.SecondTraversal = mergedIntoBuilder.Build().ApiTraversal;
                    break;
            }

            this.WriteLine($"First Traversal:  {this.FirstTraversal.SafeToString()}");
            this.WriteLine($"Second Traversal: {this.SecondTraversal.SafeToString()}");
        }

        protected override void Assert()
        {
            this.FirstTraversal.Should().NotBeNull();
            this.SecondTraversal.Should().NotBeNull();
            this.FirstTraversal.Should().NotBeSameAs(this.SecondTraversal);
            this.FirstTraversal.ApiName.Should().Be(this.ExpectedApiName);
            this.SecondTraversal.ApiName.Should().Be(this.ExpectedApiName);
            this.SecondTraversal.ClrName.Should().Be(nameof(NavigationTarget.Related));
        }
        #endregion
    }
    #endregion

    #region Member Access Lifecycle Theory Data
    public static TheoryDataRow<IXUnitTest>[] RepeatedEndTraversalBuildTheoryData =>
    [
        new RepeatedEndTraversalBuildTest
        {
            Name = "Principal end builds a distinct traversal on each build",
            EndScenario = TraversalEndScenario.PrincipalEnd,
            ExpectedApiName = "Related"
        },
        new RepeatedEndTraversalBuildTest
        {
            Name = "Dependent end builds a distinct traversal on each build",
            EndScenario = TraversalEndScenario.DependentEnd,
            ExpectedApiName = "Related"
        },
        new RepeatedEndTraversalBuildTest
        {
            Name = "Merged dependent end does not share the source builder's traversal",
            EndScenario = TraversalEndScenario.MergedDependentEnd,
            ExpectedApiName = "Related"
        }
    ];
    #endregion

    #region Member Access Lifecycle Test Methods
    [Theory]
    [MemberData(nameof(RepeatedEndTraversalBuildTheoryData))]
    public void RepeatedEndBuildsCreateDistinctTraversals(IXUnitTest test) => test.Execute(this);
    #endregion
}
