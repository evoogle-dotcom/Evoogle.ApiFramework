// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{

    #region Test Types
    private class TryGetByApiNameTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string SearchKey { get; init; }
        public required bool ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private bool? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            this.WriteLine($"ApiSchema:      {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"SearchKey:      {this.SearchKey.SafeToString()}");
            this.WriteLine($"ExpectedResult: {this.ExpectedResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiSchema!.TryGetTypeByApiName(this.SearchKey, out _);

            this.WriteLine($"ActualResult: {this.ActualResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().NotBeNull();
            this.ActualResult.Should().Be(this.ExpectedResult);
        }
        #endregion
    }

    private class TryGetByClrTypeTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required Type SearchKey { get; init; }
        public required bool ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private bool? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            this.WriteLine($"ApiSchema:      {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"SearchKey:      {this.SearchKey.SafeToString()}");
            this.WriteLine($"ExpectedResult: {this.ExpectedResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.ApiSchema!.TryGetTypeByClrType(this.SearchKey, out _);

            this.WriteLine($"ActualResult: {this.ActualResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().NotBeNull();
            this.ActualResult.Should().Be(this.ExpectedResult);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryGetByApiNameTheoryData =>
    [
        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false when {nameof(ApiNamedType)} does not exist in schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "UnknownType",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiEnumType)} with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "Gender",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiEnumType)} with case-insensitive search (uppercase)",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "GENDER",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiObjectType)} with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "ScalarsOnly",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiObjectType)} with case-insensitive search (lowercase)",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "scalarsonly",
            ExpectedResult = false
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns true for {nameof(ApiScalarType)} with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            SearchKey = "Boolean",
            ExpectedResult = true
        },

        new TryGetByApiNameTest
        {
            Name = $"{nameof(ApiSchema.TryGetTypeByApiName)} returns false for {nameof(ApiScalarType)} with case-insensitive search (uppercase)",
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
