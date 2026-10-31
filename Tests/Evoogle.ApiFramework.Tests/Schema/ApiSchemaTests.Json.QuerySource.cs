// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Test Types
    private class JsonSerializeQuerySourceDefaultsTest : JsonSerializeTest
    {
        #region Constructors
        public JsonSerializeQuerySourceDefaultsTest()
        {
            this.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeQuerySourceTheoryData =>
    [
        .. QuerySourceJsonTestCases.Union(QuerySourceDefaultJsonTestCases)
        .Select(c => (TheoryDataRow<IXUnitTest>)new JsonDeserializeTest
        {
            Name = c.Name,
            SourceJson = c.Json,
            ExpectedFactoryArgument = c.FactoryArgument
        })
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripQuerySourceTheoryData =>
    [
        .. QuerySourceJsonTestCases.Union(QuerySourceDefaultJsonTestCases)
        .Select(c => (TheoryDataRow<IXUnitTest>)new JsonRoundtripTest
        {
            Name = c.Name,
            ExpectedFactoryArgument = c.FactoryArgument
        })
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeQuerySourceTheoryData =>
    [
        .. QuerySourceJsonTestCases.Select(c => (TheoryDataRow<IXUnitTest>)new JsonSerializeTest
        {
            Name = c.Name,
            SourceFactoryArgument = c.FactoryArgument,
            ExpectedJson = c.Json
        }),
        .. QuerySourceDefaultJsonTestCases.Select
        (c => (TheoryDataRow<IXUnitTest>)new JsonSerializeQuerySourceDefaultsTest
        {
            Name = c.Name,
            SourceFactoryArgument = c.FactoryArgument,
            ExpectedJson = c.Json
        })
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeQuerySourceTheoryData))]
    public void JsonDeserialize_QuerySource(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonRoundtripQuerySourceTheoryData))]
    public void JsonRoundtrip_QuerySource(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonSerializeQuerySourceTheoryData))]
    public void JsonSerialize_QuerySource(IXUnitTest test) => test.Execute(this);
    #endregion
}
