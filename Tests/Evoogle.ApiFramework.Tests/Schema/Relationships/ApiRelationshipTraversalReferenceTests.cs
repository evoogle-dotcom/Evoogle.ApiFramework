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

public class ApiRelationshipTraversalReferenceTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class ReferenceTest : XUnitTest
    {
        public ApiClrMemberReference? ClrMemberReference { get; init; }

        private ApiRelationshipTraversal? ActualTraversal { get; set; }

        private ApiRelationshipTraversal? RoundtrippedTraversal { get; set; }

        private string? Json { get; set; }

        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.ActualTraversal = new("related", this.ClrMemberReference);
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
            this.ActualTraversal!.HasClrMember.Should().Be(this.ClrMemberReference is not null);
            this.ActualTraversal.ClrMemberReference.Should().Be(this.ClrMemberReference);
            this.RoundtrippedTraversal!.ClrMemberReference.Should().Be(this.ClrMemberReference);

            if (this.ClrMemberReference is null)
            {
                this.Json.Should().NotContain(nameof(ApiRelationshipTraversal.ClrMemberReference));
            }
            else
            {
                this.Json.Should().Contain
                (
                    $"\"{nameof(ApiRelationshipTraversal.ClrMemberReference)}\""
                );
                this.Json.Should().Contain
                (
                    $"\"{nameof(ApiClrMemberReference.ClrMemberName)}\":" +
                    $"\"{this.ClrMemberReference.ClrMemberName}\""
                );
            }
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ReferenceTheoryData =>
    [
        new ReferenceTest
        {
            Name = "Traversal without CLR member reference",
            ClrMemberReference = null
        },
        new ReferenceTest
        {
            Name = "Traversal with CLR property reference",
            ClrMemberReference = new("Related", ClrMemberKind.Property)
        },
        new ReferenceTest
        {
            Name = "Traversal with CLR field reference",
            ClrMemberReference = new("Related", ClrMemberKind.Field)
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ReferenceTheoryData))]
    public void Reference(IXUnitTest test) => test.Execute(this);
    #endregion
}
