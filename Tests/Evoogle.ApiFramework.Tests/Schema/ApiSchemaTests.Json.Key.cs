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
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeKeyTheoryData =>
    [
        .. KeyJsonTestCases.Select(c => (TheoryDataRow<IXUnitTest>)new JsonDeserializeTest
        {
            Name = c.Name,
            SourceJson = c.Json,
            ExpectedFactoryArgument = c.FactoryArgument
        })
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripKeyTheoryData =>
    [
        .. KeyJsonTestCases.Select(c => (TheoryDataRow<IXUnitTest>)new JsonRoundtripTest
        {
            Name = c.Name,
            ExpectedFactoryArgument = c.FactoryArgument
        })
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeKeyTheoryData =>
    [
        .. KeyJsonTestCases.Select(c => (TheoryDataRow<IXUnitTest>)new JsonSerializeTest
        {
            Name = c.Name,
            SourceFactoryArgument = c.FactoryArgument,
            ExpectedJson = c.Json
        })
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeKeyTheoryData))]
    public void JsonDeserialize_Key(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonRoundtripKeyTheoryData))]
    public void JsonRoundtrip_Key(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonSerializeKeyTheoryData))]
    public void JsonSerialize_Key(IXUnitTest test) => test.Execute(this);
    #endregion
}
