// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;

using BuildTest = Evoogle.ApiFramework.Schema.TestData.ApiSchemaKindFluentRoundTripBuildTest;

namespace Evoogle.ApiFramework.Schema.Configuration;

public partial class ApiSchemaBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Simple}' API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
        },

        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Key}' API schema",
            ApiSchemaKind = ApiSchemaKind.Key,
        },

        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Relationship}' API schema",
            ApiSchemaKind = ApiSchemaKind.Relationship,
        },

        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Commerce}' API schema",
            ApiSchemaKind = ApiSchemaKind.Commerce,
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}
