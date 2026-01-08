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

public class ApiEnumTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum TryGetMethodKind
    {
        TryGetValueByApiName,
        TryGetValueByClrName,
        TryGetValueByClrOrdinal
    }

    private class TryGetTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiEnumTypeName { get; init; }
        public required TryGetMethodKind TryGetMethod { get; init; }
        public required object SearchKey { get; init; }
        public required bool ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        protected ApiSchema? ApiSchema { get; set; }
        private ApiEnumType? ApiEnumType { get; set; }
        private bool? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiEnumType = this.ApiSchema.GetEnumTypeByApiName(this.ApiEnumTypeName);
            this.ApiEnumType = apiEnumType ?? throw new InvalidOperationException($"{nameof(Schema.ApiEnumType)} '{this.ApiEnumTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:      {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiEnumType:    {this.ApiEnumType.ApiName.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"TryGetMethod:   {this.TryGetMethod.SafeToString()}");
            this.WriteLine($"SearchKey:      {this.SearchKey.SafeToString()}");
            this.WriteLine($"ExpectedResult: {this.ExpectedResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.TryGetMethod switch
            {
                TryGetMethodKind.TryGetValueByApiName => this.ApiEnumType!.TryGetValueByApiName((string)this.SearchKey!, out _),
                TryGetMethodKind.TryGetValueByClrName => this.ApiEnumType!.TryGetValueByClrName((string)this.SearchKey!, out _),
                TryGetMethodKind.TryGetValueByClrOrdinal => this.ApiEnumType!.TryGetValueByClrOrdinal((int)this.SearchKey!, out _),
                _ => throw new InvalidOperationException($"Unknown {nameof(this.TryGetMethod)}: {this.TryGetMethod}"),
            };

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
    public static TheoryDataRow<IXUnitTest>[] TryGetTheoryData =>
    [
        // TryGetValueByApiName
        new TryGetTest
        {
            Name = $"{nameof(ApiEnumType.TryGetValueByApiName)} returns true when {nameof(ApiEnumValue)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiEnumTypeName = nameof(Gender),
            TryGetMethod = TryGetMethodKind.TryGetValueByApiName,
            SearchKey = "Male",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiEnumType.TryGetValueByApiName)} returns false when {nameof(ApiEnumValue)} exists but with case-insensitive search",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiEnumTypeName = nameof(Gender),
            TryGetMethod = TryGetMethodKind.TryGetValueByApiName,
            SearchKey = "MALE",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiEnumType.TryGetValueByApiName)} returns false when {nameof(ApiEnumValue)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiEnumTypeName = nameof(Gender),
            TryGetMethod = TryGetMethodKind.TryGetValueByApiName,
            SearchKey = "alien",
            ExpectedResult = false
        },

        // TryGetValueByClrName
        new TryGetTest
        {
            Name = $"{nameof(ApiEnumType.TryGetValueByClrName)} returns true when {nameof(ApiEnumValue)} exists with exact case match",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiEnumTypeName = nameof(Gender),
            TryGetMethod = TryGetMethodKind.TryGetValueByClrName,
            SearchKey = "Male",
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiEnumType.TryGetValueByClrName)} returns false when {nameof(ApiEnumValue)} exists but with case-insensitive search",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiEnumTypeName = nameof(Gender),
            TryGetMethod = TryGetMethodKind.TryGetValueByClrName,
            SearchKey = "MALE",
            ExpectedResult = false
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiEnumType.TryGetValueByClrName)} returns false when {nameof(ApiEnumValue)} does not exist",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiEnumTypeName = nameof(Gender),
            TryGetMethod = TryGetMethodKind.TryGetValueByClrName,
            SearchKey = "Alien",
            ExpectedResult = false
        },

        // TryGetValueByClrOrdinal
        new TryGetTest
        {
            Name = $"{nameof(ApiEnumType.TryGetValueByClrOrdinal)} returns true when {nameof(ApiEnumValue)} exists for given ordinal",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiEnumTypeName = nameof(Gender),
            TryGetMethod = TryGetMethodKind.TryGetValueByClrOrdinal,
            SearchKey = 1,
            ExpectedResult = true
        },

        new TryGetTest
        {
            Name = $"{nameof(ApiEnumType.TryGetValueByClrOrdinal)} returns false when {nameof(ApiEnumValue)} does not exist for given ordinal",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiEnumTypeName = nameof(Gender),
            TryGetMethod = TryGetMethodKind.TryGetValueByClrOrdinal,
            SearchKey = 3,
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
