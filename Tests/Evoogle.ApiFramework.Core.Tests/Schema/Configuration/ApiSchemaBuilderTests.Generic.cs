// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

namespace Evoogle.ApiFramework.Schema.Configuration;

public partial class ApiSchemaBuilderTests
{
    #region Test Classes
    private class BuildTestGeneric : BuildTestCore
    {
        #region User Supplied Properties
        [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchema>))]
        public required Expression<Func<ApiSchema>> ApiSchemaActualBuildExpression { get; init; } = null!;
        #endregion

        #region Constructors
        [SetsRequiredMembers]
        public BuildTestGeneric()
        {
            this.Name = nameof(BuildTestGeneric);
        }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            var apiSchemaActualBuildLambda = this.ApiSchemaActualBuildExpression.Compile();
            this.ApiSchemaActual = apiSchemaActualBuildLambda();

            this.WriteLine("ApiSchemaActual:");
            this.WriteLine($"{this.ApiSchemaActual.SafeToJson(_defaultToJsonOptions)}");
        }
        #endregion
    }

    [method: SetsRequiredMembers]
    private class BuildTestGeneric<TExtension>() : BuildTestGeneric
    {
        #region BuildTestCore Methods
        protected override IEnumerable<Type> GetExtensionTypes()
        {
            yield return typeof(TExtension);
        }
        #endregion
    }

    [method: SetsRequiredMembers]
    private class BuildTestGeneric<TExtension1, TExtension2>() : BuildTestGeneric
    {
        #region BuildTestCore Methods
        protected override IEnumerable<Type> GetExtensionTypes()
        {
            yield return typeof(TExtension1);
            yield return typeof(TExtension2);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildGenericTheoryData =>
    [
        new BuildTestGeneric
        {
            Name = $"Build '{ApiSchemaKind.Simple}' API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiSchemaActualBuildExpression = static () => ApiSchemaBuilderTestsGenericTestFactory.BuildSimpleApiSchema(),
        },

        new BuildTestGeneric
        {
            Name = $"Build '{ApiSchemaKind.Key}' API schema",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiSchemaActualBuildExpression = static () => ApiSchemaBuilderTestsGenericTestFactory.BuildKeyApiSchema(),
        },

        new BuildTestGeneric
        {
            Name = $"Build '{ApiSchemaKind.Relationship}' API schema",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ApiSchemaActualBuildExpression = static () => ApiSchemaBuilderTestsGenericTestFactory.BuildRelationshipApiSchema(),
        },

        new BuildTestGeneric
        {
            Name = $"Build '{ApiSchemaKind.Commerce}' API schema",
            ApiSchemaKind = ApiSchemaKind.Commerce,
            ApiSchemaActualBuildExpression = static () => ApiSchemaBuilderTestsGenericTestFactory.BuildCommerceApiSchema(),
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildGenericTheoryData))]
    public void BuildGeneric(IXUnitTest test) => test.Execute(this);
    #endregion
}
