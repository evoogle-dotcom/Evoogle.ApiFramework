// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.TestData;
using Evoogle.Extension;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiKeyTypeTests
{
    #region Test Types
    private class JsonContractTest : XUnitTest
    {
        #region User Supplied Properties
        public required string? ApiName { get; init; }
        public required string ExpectedJson { get; init; }
        #endregion

        #region Calculated Properties
        private string? ActualJson { get; set; }
        private ApiKeyType? ActualKeyType { get; set; }
        private Type? DeclaredType { get; set; }
        private ApiKeyType? SourceKeyType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiKeyPath = new ApiKeyPath
            (
                typeof(KeyOneScalarPart),
                [new ApiKeyPathSegment(nameof(KeyOneScalarPart.Id))]
            );

            this.SourceKeyType = this.ApiName is null
                ? new ApiKeyType([apiKeyPath])
                : new ApiNamedKeyType(this.ApiName, [apiKeyPath]);
            this.SourceKeyType.AttachExtension(new GraphQlExtension());
            this.DeclaredType = this.SourceKeyType.GetType();
        }

        protected override void Act()
        {
            this.ActualJson = JsonSerializer.Serialize(this.SourceKeyType, this.DeclaredType!);
            this.ActualKeyType = (ApiKeyType?)JsonSerializer.Deserialize
            (
                this.ExpectedJson,
                this.DeclaredType!
            );
        }

        protected override void Assert()
        {
            this.ActualJson.RemoveWhitespace().Should().Be(this.ExpectedJson.RemoveWhitespace());

            this.ActualKeyType.Should().NotBeNull();
            this.ActualKeyType!.GetType().Should().Be(this.DeclaredType);
            this.ActualKeyType.ApiKeyPaths.Should().ContainSingle();

            var actualApiKeyPath = this.ActualKeyType.ApiKeyPaths.Single();
            actualApiKeyPath.ClrRootType.Should().Be(typeof(KeyOneScalarPart));
            actualApiKeyPath.ApiSegments.Should().ContainSingle();
            actualApiKeyPath.ApiSegments.Single().ClrPropertyName.Should().Be
            (
                nameof(KeyOneScalarPart.Id)
            );

            this.ActualKeyType.Extensions.Should().NotBeNull();
            this.ActualKeyType.Extensions!.Should().ContainKey(typeof(GraphQlExtension));
            this.ActualKeyType.Extensions[typeof(GraphQlExtension)].Should().BeEquivalentTo
            (
                new GraphQlExtension()
            );

            if (this.ApiName is null)
            {
                this.ActualKeyType.Should().BeOfType<ApiKeyType>();
            }
            else
            {
                this.ActualKeyType.Should().BeOfType<ApiNamedKeyType>()
                    .Which.ApiName.Should().Be(this.ApiName);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonContractTheoryData =>
    [
        new JsonContractTest
        {
            Name = $"{nameof(ApiKeyType)} JSON contract",
            ApiName = null,
            ExpectedJson = @"
            {
                ""ApiKeyPaths"": [
                    {
                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOneScalarPart, " +
                            @"Evoogle.ApiFramework.Tests"",
                        ""ClrPath"": ""Id""
                    }
                ],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, " +
                        @"Evoogle.ApiFramework.Tests"": {
                        ""Count"": 42
                    }
                }
            }"
        },
        new JsonContractTest
        {
            Name = $"{nameof(ApiNamedKeyType)} JSON contract",
            ApiName = "PrimaryKey",
            ExpectedJson = @"
            {
                ""ApiName"": ""PrimaryKey"",
                ""ApiKeyPaths"": [
                    {
                        ""ClrRootType"": ""Evoogle.ApiFramework.TestData.KeyOneScalarPart, " +
                            @"Evoogle.ApiFramework.Tests"",
                        ""ClrPath"": ""Id""
                    }
                ],
                ""Extensions"": {
                    ""Evoogle.ApiFramework.TestData.GraphQlExtension, " +
                        @"Evoogle.ApiFramework.Tests"": {
                        ""Count"": 42
                    }
                }
            }"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonContractTheoryData))]
    public void JsonContract(IXUnitTest test) => test.Execute(this);
    #endregion
}
