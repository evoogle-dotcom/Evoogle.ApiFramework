// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiObjectTypeTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryGetTheoryData =>
    [
        // TryGetIdentityByApiName
        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetIdentityByApiName)} returns true when {nameof(ApiIdentity)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(IdentityScalar),
            TryGetMethod = TryGetMethod.TryGetIdentityByApiName,
            SearchKey = "PK_IdentityScalar",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetIdentityByApiName)} returns false when {nameof(ApiIdentity)} exists but case mismatch",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(IdentityScalar),
            TryGetMethod = TryGetMethod.TryGetIdentityByApiName,
            SearchKey = "pk_identityscalar",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiObjectType.TryGetIdentityByApiName)} returns false when {nameof(ApiIdentity)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(IdentityScalar),
            TryGetMethod = TryGetMethod.TryGetIdentityByApiName,
            SearchKey = "Unknown_Identity",
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
