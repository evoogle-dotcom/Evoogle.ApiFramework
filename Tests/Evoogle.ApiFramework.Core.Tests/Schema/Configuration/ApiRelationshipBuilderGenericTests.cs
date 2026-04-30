// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Unit tests for the typed relationship end builder overloads:
///     <see cref="ApiRelationshipOneToOneBuilder.WithPrincipalEnd{TPrincipal}"/>,
///     <see cref="ApiRelationshipOneToOneBuilder.WithDependentEnd{TDependent}"/>,
///     <see cref="ApiRelationshipOneToManyBuilder.WithDependentEnd{TDependent}"/>,
///     <see cref="ApiRelationshipManyToManyBuilder.WithDependentEndA{TDependent}"/>, and related overloads.
/// </summary>
public class ApiRelationshipBuilderGenericTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    /// <summary>
    ///     Verifies that typed relationship end builder overloads produce a well-formed
    ///     <see cref="ApiRelationship"/> with the expected CLR type references on each end.
    /// </summary>
    private class BuildRelationshipTest : XUnitTest
    {
        #region User Supplied Properties
        [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiRelationship>))]
        public required Expression<Func<ApiRelationship>> BuildExpected { get; init; }

        [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiRelationship>))]
        public required Expression<Func<ApiRelationship>> BuildActual { get; init; }
        #endregion

        #region Calculated Properties
        private ApiRelationship? Expected { get; set; }
        private ApiRelationship? Actual { get; set; }
        #endregion

        #region Constructors
        // [SetsRequiredMembers]
        public BuildRelationshipTest()
        {
            this.Name = nameof(BuildRelationshipTest);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Relationship;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Expected = this.BuildExpected.Compile()();
            this.WriteLine($"Expected: {this.Expected.SafeToString()}");
        }

        protected override void Act()
        {
            this.Actual = this.BuildActual.Compile()();
            this.WriteLine($"Actual:   {this.Actual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.Actual.Should().NotBeNull();
            this.AssertBeEquivalentTo(this.Actual, this.Expected);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildRelationshipTheoryData =>
    [
        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithPrincipalEnd<Customer> and WithDependentEnd<Order> with AddScalarPath(expr)",
            BuildExpected = static () => ApiRelationshipBuilderGenericTestFactory.BuildExpected_OneToOne_DependentEnd_ScalarPath(),
            BuildActual = static () => ApiRelationshipBuilderGenericTestFactory.BuildActual_OneToOne_DependentEnd_ScalarPath()
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToManyBuilder typed WithPrincipalEnd<Customer> and WithDependentEnd<Order> with AddScalarPath(expr)",
            BuildExpected = static () => ApiRelationshipBuilderGenericTestFactory.BuildExpected_OneToMany_DependentEnd_ScalarPath(),
            BuildActual = static () => ApiRelationshipBuilderGenericTestFactory.BuildActual_OneToMany_DependentEnd_ScalarPath()
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipManyToManyBuilder typed WithPrincipalEndA<Customer> and WithDependentEndA with AddScalarPath(expr)",
            BuildExpected = static () => ApiRelationshipBuilderGenericTestFactory.BuildExpected_ManyToMany_DependentEnds_ScalarPaths(),
            BuildActual = static () => ApiRelationshipBuilderGenericTestFactory.BuildActual_ManyToMany_DependentEnds_ScalarPaths()
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithDependentEnd<Order> with multiple scalar paths",
            BuildExpected = static () => ApiRelationshipBuilderGenericTestFactory.BuildExpected_OneToOne_DependentEnd_MultipleScalarPaths(),
            BuildActual = static () => ApiRelationshipBuilderGenericTestFactory.BuildActual_OneToOne_DependentEnd_MultipleScalarPaths()
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithDependentEnd<CustomerProfile> AddNestedPath drill-down",
            BuildExpected = static () => ApiRelationshipBuilderGenericTestFactory.BuildExpected_OneToOne_NestedPath(),
            BuildActual = static () => ApiRelationshipBuilderGenericTestFactory.BuildActual_OneToOne_NestedPath()
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipOneToOneBuilder typed WithPrincipalEnd<Customer>(configure) threads delete behavior through",
            BuildExpected = static () => ApiRelationshipBuilderGenericTestFactory.BuildExpected_OneToOne_PrincipalConfigure_DeleteBehavior(),
            BuildActual = static () => ApiRelationshipBuilderGenericTestFactory.BuildActual_OneToOne_PrincipalConfigure_DeleteBehavior()
        },

        new BuildRelationshipTest
        {
            Name = "ApiRelationshipManyToManyBuilder WithDependentEndA and WithDependentEndB resolve independent CLR types",
            BuildExpected = static () => ApiRelationshipBuilderGenericTestFactory.BuildExpected_ManyToMany_IndependentEndTypes(),
            BuildActual = static () => ApiRelationshipBuilderGenericTestFactory.BuildActual_ManyToMany_IndependentEndTypes()
        },
    ];
    #endregion

    #region Theory Tests
    [Theory]
    [MemberData(nameof(BuildRelationshipTheoryData))]
    public void BuildRelationship(IXUnitTest test) => test.Execute(this);
    #endregion
}

