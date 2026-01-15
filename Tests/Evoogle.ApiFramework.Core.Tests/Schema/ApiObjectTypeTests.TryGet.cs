// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiObjectTypeTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryGetTheoryData =>
    [
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
            Name = $"{nameof(ApiObjectType.TryGetPropertyByApiName)} returns true when {nameof(ApiProperty)} exists but with case-insensitive search",
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
            Name = $"{nameof(ApiObjectType.TryGetPropertyByClrName)} returns true when {nameof(ApiProperty)} exists but with case-insensitive search",
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

        // TryGetRelationshipByApiName
        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetRelationshipByApiName)} returns true when {nameof(ApiRelationship)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            SearchKey = "Company_Owner",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetRelationshipByApiName)} returns true when {nameof(ApiRelationship)} exists but with case-insensitive search",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            SearchKey = "COMPANY_OWNER",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetRelationshipByApiName)} returns false when {nameof(ApiRelationship)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            SearchKey = "Unknown_Relationship",
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