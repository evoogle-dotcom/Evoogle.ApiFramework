// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Relationships;

public class ApiRelationshipTraversalClrNavigationMemberTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class ClrNavigationMemberTest : XUnitTest
    {
        public ClrMemberReference? ClrNavigationMember { get; init; }

        private ApiRelationshipTraversal? ActualTraversal { get; set; }

        private ApiRelationshipTraversal? RoundtrippedTraversal { get; set; }

        private string? Json { get; set; }

        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.ActualTraversal = new("related", this.ClrNavigationMember);
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            this.Json = JsonSerializer.Serialize(this.ActualTraversal, options);
            this.RoundtrippedTraversal = JsonSerializer.Deserialize<ApiRelationshipTraversal>
            (
                this.Json,
                options
            );
        }

        protected override void Assert()
        {
            this.ActualTraversal!.HasClrNavigationMember.Should().Be(this.ClrNavigationMember is not null);
            this.ActualTraversal.ClrNavigationMember.Should().Be(this.ClrNavigationMember);
            this.RoundtrippedTraversal!.ClrNavigationMember.Should().Be(this.ClrNavigationMember);

            if (this.ClrNavigationMember is null)
            {
                this.Json.Should().NotContain(nameof(ApiRelationshipTraversal.ClrNavigationMember));
            }
            else
            {
                this.Json.Should().Contain
                (
                    $$"""
                    "{{nameof(ApiRelationshipTraversal.ClrNavigationMember)}}"
                    """
                );
                this.Json.Should().Contain
                (
                    $$"""
                    "{{nameof(ClrMemberReference.ClrName)}}":"{{this.ClrNavigationMember.ClrName}}"
                    """
                );
            }
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ClrNavigationMemberTheoryData =>
    [
        new ClrNavigationMemberTest
        {
            Name = "Traversal without CLR navigation member",
            ClrNavigationMember = null
        },
        new ClrNavigationMemberTest
        {
            Name = "Traversal with CLR property navigation member",
            ClrNavigationMember = new(ClrMemberKind.Property, "Related")
        },
        new ClrNavigationMemberTest
        {
            Name = "Traversal with CLR field navigation member",
            ClrNavigationMember = new(ClrMemberKind.Field, "Related")
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ClrNavigationMemberTheoryData))]
    public void ClrNavigationMember(IXUnitTest test) => test.Execute(this);
    #endregion
}
