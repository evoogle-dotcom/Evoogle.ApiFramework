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
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
        [.. JsonTestCases
            .Select(c => (TheoryDataRow<IXUnitTest>)new JsonDeserializeTest
            {
                Name = c.Name,
                SourceJson = c.Json,
                ExpectedFactoryArgument = c.FactoryArgument
            })];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeTheoryData))]
    public void JsonDeserialize(IXUnitTest test) => test.Execute(this);
    #endregion
}
