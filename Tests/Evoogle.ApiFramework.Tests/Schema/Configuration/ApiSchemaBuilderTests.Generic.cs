// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;

using BuildTest = Evoogle.ApiFramework.Schema.TestData.ApiSchemaKindExpressionBuildTest;

namespace Evoogle.ApiFramework.Schema.Configuration;

public partial class ApiSchemaBuilderTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildGenericTheoryData =>
    [
        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Simple}' API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiSchemaActualBuildExpression = static () => ApiSchemaBuilderTestsGenericTestFactory.BuildSimpleApiSchema(),
        },

        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Key}' API schema",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiSchemaActualBuildExpression = static () => ApiSchemaBuilderTestsGenericTestFactory.BuildKeyApiSchema(),
        },

        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Relationship}' API schema",
            ApiSchemaKind = ApiSchemaKind.Relationship,
            ApiSchemaActualBuildExpression = static () => ApiSchemaBuilderTestsGenericTestFactory.BuildRelationshipApiSchema(),
        },

        new BuildTest
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
