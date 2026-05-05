// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
        [.. SimpleJsonTestCases.Union(IdentityJsonTestCases).Union(RelationshipJsonTestCases)
            .Select(c => (TheoryDataRow<IXUnitTest>)new JsonRoundtripTest
            {
                Name = c.Name,
                ExpectedFactoryArgument = c.FactoryArgument
            })];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtrip(IXUnitTest test) => test.Execute(this);
    #endregion
}
