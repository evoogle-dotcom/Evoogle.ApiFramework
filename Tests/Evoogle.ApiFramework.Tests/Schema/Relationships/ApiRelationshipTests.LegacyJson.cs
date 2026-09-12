// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Relationships;

public partial class ApiRelationshipTests
{
    #region Test Types
    private sealed class LegacyJsonMemberTest : XUnitTest
    {
        #region Calculated Properties
        private ApiRelationshipAssociation? Association { get; set; }
        private ApiRelationshipDependentEnd? DependentEnd { get; set; }
        private ApiRelationshipPrincipalEnd? PrincipalEnd { get; set; }
        private string? AssociationJson { get; set; }
        private string? DependentEndJson { get; set; }
        private string? PrincipalEndJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var foreignKeyA = CreateForeignKey();
            var foreignKeyB = CreateForeignKey();

            this.DependentEndJson = JsonSerializer.Serialize
            (
                new ApiRelationshipDependentEnd(typeof(KeyOneScalarPart), CreateForeignKey())
            ).Replace("\"ApiForeignKey\"", "\"ApiForeignKeyType\"");

            this.PrincipalEndJson = JsonSerializer.Serialize
            (
                new ApiRelationshipPrincipalEnd(typeof(KeyOneScalarPart), "PrimaryKey")
            ).Replace("\"ApiPrincipalKeyName\"", "\"ApiPrincipalKeyTypeName\"");

            this.AssociationJson = JsonSerializer.Serialize
            (
                new ApiRelationshipAssociation
                (
                    typeof(KeyOneScalarPart),
                    foreignKeyA,
                    foreignKeyB
                )
            )
                .Replace("\"ApiForeignKeyA\"", "\"ApiForeignKeyTypeA\"")
                .Replace("\"ApiForeignKeyB\"", "\"ApiForeignKeyTypeB\"");
        }

        protected override void Act()
        {
            this.DependentEnd = JsonSerializer.Deserialize<ApiRelationshipDependentEnd>
            (
                this.DependentEndJson!
            );
            this.PrincipalEnd = JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>
            (
                this.PrincipalEndJson!
            );
            this.Association = JsonSerializer.Deserialize<ApiRelationshipAssociation>
            (
                this.AssociationJson!
            );
        }

        protected override void Assert()
        {
            this.DependentEnd.Should().NotBeNull();
            this.DependentEnd!.HasForeignKey.Should().BeFalse();
            this.PrincipalEnd.Should().NotBeNull();
            this.PrincipalEnd!.ApiPrincipalKeyName.Should().BeNull();
            this.Association.Should().NotBeNull();
            this.Association!.HasForeignKeys.Should().BeFalse();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryData<IXUnitTest> LegacyJsonMemberTheoryData => new()
    {
        new LegacyJsonMemberTest
        {
            Name = "Ignore Legacy Relationship Key JSON Members"
        }
    };
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(LegacyJsonMemberTheoryData))]
    public void LegacyJsonMembers(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Factory Methods
    private static ApiKeyDefinition CreateForeignKey() => new
    (
        [
            new ApiKeyPath
            (
                typeof(KeyOneScalarPart),
                [new ApiKeyPathSegment(nameof(KeyOneScalarPart.Id))]
            )
        ]
    );
    #endregion
}
