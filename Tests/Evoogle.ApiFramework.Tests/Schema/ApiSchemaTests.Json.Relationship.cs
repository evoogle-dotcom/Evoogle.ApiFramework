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
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeRelationshipTheoryData =>
    [
        .. RelationshipJsonTestCases.Select(c => (TheoryDataRow<IXUnitTest>)new JsonDeserializeTest
        {
            Name = c.Name,
            SourceJson = c.Json,
            ExpectedFactoryArgument = c.FactoryArgument
        })
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripRelationshipTheoryData =>
    [
        .. RelationshipJsonTestCases.Select(c => (TheoryDataRow<IXUnitTest>)new JsonRoundtripTest
        {
            Name = c.Name,
            ExpectedFactoryArgument = c.FactoryArgument
        })
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeRelationshipTheoryData =>
    [
        .. RelationshipJsonTestCases.Select(c => (TheoryDataRow<IXUnitTest>)new JsonSerializeTest
        {
            Name = c.Name,
            SourceFactoryArgument = c.FactoryArgument,
            ExpectedJson = c.Json
        })
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeRelationshipTheoryData))]
    public void JsonDeserialize_Relationship(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonRoundtripRelationshipTheoryData))]
    public void JsonRoundtrip_Relationship(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonSerializeRelationshipTheoryData))]
    public void JsonSerialize_Relationship(IXUnitTest test) => test.Execute(this);
    #endregion
}
