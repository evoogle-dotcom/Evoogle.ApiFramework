// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.Types;

public partial class ApiObjectTypeTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryGetTheoryData =>
    [
        // TryGetKeyByApiName
        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetKeyByApiName)} returns true when " +
                $"{nameof(ApiNamedKeyDefinition)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            TryGetMethod = TryGetMethod.TryGetKeyByApiName,
            SearchKey = "PK_KeyOneScalarPart",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetKeyByApiName)} returns false when " +
                $"{nameof(ApiNamedKeyDefinition)} exists but case mismatch",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            TryGetMethod = TryGetMethod.TryGetKeyByApiName,
            SearchKey = "pk_keyonescalarpart",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetKeyByApiName)} returns false when " +
                $"{nameof(ApiNamedKeyDefinition)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            TryGetMethod = TryGetMethod.TryGetKeyByApiName,
            SearchKey = "Unknown_Key",
            ExpectedResult = false
        },

        // TryGetPropertyByApiName
        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByApiName)} returns true when {nameof(ApiProperty)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            SearchKey = "RequiredName",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByApiName)} returns false when {nameof(ApiProperty)} exists but case mismatch",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            SearchKey = "requiredname",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByApiName)} returns false when {nameof(ApiProperty)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            SearchKey = "Unknown_Property",
            ExpectedResult = false
        },

        // TryGetPropertyByClrName
        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByClrName)} returns true when {nameof(ApiProperty)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            SearchKey = "OptionalName",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByClrName)} returns false when {nameof(ApiProperty)} exists but case mismatch",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            SearchKey = "OPTIONALNAME",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetPropertyByClrName)} returns false when {nameof(ApiProperty)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            SearchKey = "Unknown_Property",
            ExpectedResult = false
        },

    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TryGetTheoryData))]
    public void TryGet(IXUnitTest test) => test.Execute(this);
    #endregion
}
