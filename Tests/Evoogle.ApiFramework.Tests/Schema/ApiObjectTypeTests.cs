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

public partial class ApiObjectTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum TryGetMethod
    {
        TryGetKeyTypeByApiName,
        TryGetPropertyByApiName,
        TryGetPropertyByClrName
    }

    private class TryGetTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required TryGetMethod TryGetMethod { get; init; }
        public required string SearchKey { get; init; }
        public required bool ExpectedResult { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private bool? ActualResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:      {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType:  {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"TryGetMethod:   {this.TryGetMethod.SafeToString()}");
            this.WriteLine($"SearchKey:      {this.SearchKey.SafeToString()}");
            this.WriteLine($"ExpectedResult: {this.ExpectedResult.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = this.TryGetMethod switch
            {
                TryGetMethod.TryGetKeyTypeByApiName => this.ApiObjectType!.TryGetKeyTypeByApiName(this.SearchKey!, out _),
                TryGetMethod.TryGetPropertyByApiName => this.ApiObjectType!.TryGetPropertyByApiName(this.SearchKey!, out _),
                TryGetMethod.TryGetPropertyByClrName => this.ApiObjectType!.TryGetPropertyByClrName(this.SearchKey!, out _),
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
}
