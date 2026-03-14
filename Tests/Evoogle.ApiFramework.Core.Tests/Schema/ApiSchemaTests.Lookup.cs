// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryGetByApiNameTheoryData =>
    [
        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false when {nameof(ApiNamedType)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "UnknownType",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiEnumType)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "Gender",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiEnumType)} exists but case mismatch",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "GENDER",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiObjectType)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "ScalarsOnly",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiObjectType)} exists but case mismatch",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "scalarsonly",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiScalarType)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "Boolean",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiScalarType)} exists but case mismatch",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "BOOLEAN",
            ExpectedResult = false
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetByClrTypeTheoryData =>
    [
        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns false when {nameof(ApiType)} is not registered in schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = typeof(Order),
            ExpectedResult = false
        },

        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns true for registered {nameof(ApiEnumType)} CLR type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = typeof(Gender),
            ExpectedResult = true
        },

        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns true for registered {nameof(ApiObjectType)} CLR type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = typeof(ScalarsOnly),
            ExpectedResult = true
        },

        new TryGetByClrTypeTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByClrType)} returns true for registered {nameof(ApiScalarType)} CLR type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = typeof(bool),
            ExpectedResult = true
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TryGetByApiNameTheoryData))]
    public void TryGetByApiName(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetByClrTypeTheoryData))]
    public void TryGetByClrType(IXUnitTest test) => test.Execute(this);
    #endregion
}
