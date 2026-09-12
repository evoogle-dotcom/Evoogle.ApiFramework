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

namespace Evoogle.ApiFramework.Schema.Key;

public partial class ApiKeyDefinitionTests
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
        private ApiKeyDefinition? ActualKeyDefinition { get; set; }
        private Type? DeclaredType { get; set; }
        private ApiKeyDefinition? SourceKeyDefinition { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiKeyPath = new ApiKeyPath
            (
                typeof(KeyOneScalarPart),
                [new ApiKeyPathSegment(nameof(KeyOneScalarPart.Id))]
            );

            this.SourceKeyDefinition = this.ApiName is null
                ? new ApiKeyDefinition([apiKeyPath])
                : new ApiNamedKeyDefinition(this.ApiName, [apiKeyPath]);
            this.SourceKeyDefinition.AttachExtension(new GraphQlExtension());
            this.DeclaredType = this.SourceKeyDefinition.GetType();
        }

        protected override void Act()
        {
            this.ActualJson = JsonSerializer.Serialize(this.SourceKeyDefinition, this.DeclaredType!);
            this.ActualKeyDefinition = (ApiKeyDefinition?)JsonSerializer.Deserialize
            (
                this.ExpectedJson,
                this.DeclaredType!
            );
        }

        protected override void Assert()
        {
            this.ActualJson.RemoveWhitespace().Should().Be(this.ExpectedJson.RemoveWhitespace());

            this.ActualKeyDefinition.Should().NotBeNull();
            this.ActualKeyDefinition!.GetType().Should().Be(this.DeclaredType);
            this.ActualKeyDefinition.ApiKeyPaths.Should().ContainSingle();

            var actualApiKeyPath = this.ActualKeyDefinition.ApiKeyPaths.Single();
            actualApiKeyPath.ClrRootType.Should().Be(typeof(KeyOneScalarPart));
            actualApiKeyPath.ApiSegments.Should().ContainSingle();
            actualApiKeyPath.ApiSegments.Single().ClrMemberName.Should().Be
            (
                nameof(KeyOneScalarPart.Id)
            );

            this.ActualKeyDefinition.Extensions.Should().NotBeNull();
            this.ActualKeyDefinition.Extensions!.Should().ContainKey(typeof(GraphQlExtension));
            this.ActualKeyDefinition.Extensions[typeof(GraphQlExtension)].Should().BeEquivalentTo
            (
                new GraphQlExtension()
            );

            if (this.ApiName is null)
            {
                this.ActualKeyDefinition.Should().BeOfType<ApiKeyDefinition>();
            }
            else
            {
                this.ActualKeyDefinition.Should().BeOfType<ApiNamedKeyDefinition>()
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
            Name = $"{nameof(ApiKeyDefinition)} JSON contract",
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
            Name = $"{nameof(ApiNamedKeyDefinition)} JSON contract",
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
