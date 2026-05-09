// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Test Types
    private class InitializeRelationshipCrossReferencesTest : XUnitTest
    {
        #region User Supplied Properties
        public required Func<(ApiSchema Schema, ApiRelationship Relationship)> Build { get; init; }
        public required Action<ApiSchema, ApiRelationship> Assertions { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? Schema { get; set; }
        private ApiRelationship? Relationship { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var (schema, relationship) = this.Build();
            this.Schema = schema;
            this.Relationship = relationship;

            this.WriteLine($"Schema:       {this.Schema}");
            this.WriteLine($"Relationship: {this.Relationship}");
        }

        protected override void Assert()
        {
            this.Schema.Should().NotBeNull();
            this.Relationship.Should().NotBeNull();
            this.Assertions(this.Schema!, this.Relationship!);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] InitializeRelationshipCrossReferencesTheoryData =>
    [
        // 1:1 — principal end and dependent end back-ref point to the owning relationship
        new InitializeRelationshipCrossReferencesTest
        {
            Name = $"{nameof(ApiRelationshipOneToOne)} principal and dependent end {nameof(ApiRelationshipEnd.ApiRelationship)} back-ref resolves to the owning relationship",
            Build = () =>
            {
                var ulidScalarType = new ApiScalarType(nameof(Ulid), typeof(Ulid));
                var stringScalarType = new ApiScalarType(nameof(String), typeof(string));

                var userType = new ApiObjectType(
                    nameof(RelationshipUser),
                    null,
                    [new ApiIdentity("PK_User_Id", [new ApiIdentityScalarPart(nameof(RelationshipUser.Id))])],
                    [
                        new ApiProperty(nameof(RelationshipUser.Id), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.Required, nameof(RelationshipUser.Id), ClrMemberKind.Property),
                        new ApiProperty(nameof(RelationshipUser.UserName), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(String)), ApiTypeModifiers.Required, nameof(RelationshipUser.UserName), ClrMemberKind.Property),
                    ],
                    typeof(RelationshipUser));

                var userProfileType = new ApiObjectType(
                    nameof(RelationshipUserProfile),
                    null,
                    null,
                    [
                        new ApiProperty(nameof(RelationshipUserProfile.UserId), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.Required, nameof(RelationshipUserProfile.UserId), ClrMemberKind.Property),
                    ],
                    typeof(RelationshipUserProfile));

                var relationship = new ApiRelationshipOneToOne(
                    "REL_User_Profile_1to1",
                    new ApiRelationshipPrincipalEnd(typeof(RelationshipUser)),
                    new ApiRelationshipDependentEnd(typeof(RelationshipUserProfile),
                    [
                        new ApiRelationshipScalarKeyPath(nameof(RelationshipUserProfile.UserId))
                    ]));

                var schema = ApiSchema.Create(
                    "Test",
                    [ulidScalarType, stringScalarType, userType, userProfileType],
                    apiRelationships: [relationship]);

                return (schema, relationship);
            },
            Assertions = (schema, rel) =>
            {
                var oneTo = (ApiRelationshipOneToOne)rel;
                oneTo.ApiPrincipalEnd.ApiRelationship.Should().BeSameAs(rel);
                oneTo.ApiDependentEnd.ApiRelationship.Should().BeSameAs(rel);
            }
        },

        // 1:M — principal end and dependent end back-ref point to the owning relationship
        new InitializeRelationshipCrossReferencesTest
        {
            Name = $"{nameof(ApiRelationshipOneToMany)} principal and dependent end {nameof(ApiRelationshipEnd.ApiRelationship)} back-ref resolves to the owning relationship",
            Build = () =>
            {
                var ulidScalarType = new ApiScalarType(nameof(Ulid), typeof(Ulid));
                var stringScalarType = new ApiScalarType(nameof(String), typeof(string));

                var userType = new ApiObjectType(
                    nameof(RelationshipUser),
                    null,
                    [new ApiIdentity("PK_User_Id", [new ApiIdentityScalarPart(nameof(RelationshipUser.Id))])],
                    [
                        new ApiProperty(nameof(RelationshipUser.Id), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.Required, nameof(RelationshipUser.Id), ClrMemberKind.Property),
                    ],
                    typeof(RelationshipUser));

                var postType = new ApiObjectType(
                    nameof(RelationshipPost),
                    null,
                    null,
                    [
                        new ApiProperty(nameof(RelationshipPost.AuthorUserId), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.Required, nameof(RelationshipPost.AuthorUserId), ClrMemberKind.Property),
                    ],
                    typeof(RelationshipPost));

                var relationship = new ApiRelationshipOneToMany(
                    "REL_User_Post_1toN",
                    new ApiRelationshipPrincipalEnd(typeof(RelationshipUser)),
                    new ApiRelationshipDependentEnd(typeof(RelationshipPost),
                    [
                        new ApiRelationshipScalarKeyPath(nameof(RelationshipPost.AuthorUserId))
                    ]));

                var schema = ApiSchema.Create(
                    "Test",
                    [ulidScalarType, stringScalarType, userType, postType],
                    apiRelationships: [relationship]);

                return (schema, relationship);
            },
            Assertions = (schema, rel) =>
            {
                var oneTo = (ApiRelationshipOneToMany)rel;
                oneTo.ApiPrincipalEnd.ApiRelationship.Should().BeSameAs(rel);
                oneTo.ApiDependentEnd.ApiRelationship.Should().BeSameAs(rel);
            }
        },

        // M:N — both principal ends and the association back-ref all point to the owning relationship
        new InitializeRelationshipCrossReferencesTest
        {
            Name = $"{nameof(ApiRelationshipManyToMany)} both principal ends and {nameof(ApiRelationshipAssociation)} {nameof(ApiRelationshipEnd.ApiRelationship)} back-ref resolves to the owning relationship",
            Build = () =>
            {
                var ulidScalarType = new ApiScalarType(nameof(Ulid), typeof(Ulid));

                var postType = new ApiObjectType(
                    nameof(RelationshipPost),
                    null,
                    [new ApiIdentity("PK_Post_Id", [new ApiIdentityScalarPart(nameof(RelationshipPost.Id))])],
                    [
                        new ApiProperty(nameof(RelationshipPost.Id), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.Required, nameof(RelationshipPost.Id), ClrMemberKind.Property),
                    ],
                    typeof(RelationshipPost));

                var tagType = new ApiObjectType(
                    nameof(RelationshipTag),
                    null,
                    [new ApiIdentity("PK_Tag_Id", [new ApiIdentityScalarPart(nameof(RelationshipTag.Id))])],
                    [
                        new ApiProperty(nameof(RelationshipTag.Id), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.Required, nameof(RelationshipTag.Id), ClrMemberKind.Property),
                    ],
                    typeof(RelationshipTag));

                var postTagType = new ApiObjectType(
                    nameof(RelationshipPostTag),
                    null,
                    null,
                    [
                        new ApiProperty(nameof(RelationshipPostTag.PostId), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.Required, nameof(RelationshipPostTag.PostId), ClrMemberKind.Property),
                        new ApiProperty(nameof(RelationshipPostTag.TagId), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.Required, nameof(RelationshipPostTag.TagId), ClrMemberKind.Property),
                    ],
                    typeof(RelationshipPostTag));

                var relationship = new ApiRelationshipManyToMany(
                    "REL_Post_Tag_NtoN",
                    new ApiRelationshipPrincipalEnd(typeof(RelationshipPost)),
                    new ApiRelationshipPrincipalEnd(typeof(RelationshipTag)),
                    new ApiRelationshipAssociation(typeof(RelationshipPostTag),
                        [new ApiRelationshipScalarKeyPath(nameof(RelationshipPostTag.PostId))],
                        [new ApiRelationshipScalarKeyPath(nameof(RelationshipPostTag.TagId))]));

                var schema = ApiSchema.Create(
                    "Test",
                    [ulidScalarType, postType, tagType, postTagType],
                    apiRelationships: [relationship]);

                return (schema, relationship);
            },
            Assertions = (schema, rel) =>
            {
                var manyToMany = (ApiRelationshipManyToMany)rel;
                manyToMany.ApiPrincipalEndA.ApiRelationship.Should().BeSameAs(rel);
                manyToMany.ApiPrincipalEndB.ApiRelationship.Should().BeSameAs(rel);
                manyToMany.ApiAssociation.ApiRelationshipManyToMany.Should().BeSameAs(rel);
            }
        },

        // Self-referential 1:M — both ends (same CLR type) back-ref point to the same relationship
        new InitializeRelationshipCrossReferencesTest
        {
            Name = $"{nameof(ApiRelationshipOneToMany)} self-referential both ends {nameof(ApiRelationshipEnd.ApiRelationship)} back-ref resolves to the same owning relationship",
            Build = () =>
            {
                var ulidScalarType = new ApiScalarType(nameof(Ulid), typeof(Ulid));

                var orgUnitType = new ApiObjectType(
                    nameof(RelationshipOrgUnit),
                    null,
                    [new ApiIdentity("PK_OrgUnit_Id", [new ApiIdentityScalarPart(nameof(RelationshipOrgUnit.Id))])],
                    [
                        new ApiProperty(nameof(RelationshipOrgUnit.Id), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.Required, nameof(RelationshipOrgUnit.Id), ClrMemberKind.Property),
                        new ApiProperty(nameof(RelationshipOrgUnit.ParentId), new ApiTypeExpression(ApiTypeKind.Scalar, nameof(Ulid)), ApiTypeModifiers.None, nameof(RelationshipOrgUnit.ParentId), ClrMemberKind.Property),
                    ],
                    typeof(RelationshipOrgUnit));

                var relationship = new ApiRelationshipOneToMany(
                    "REL_OrgUnit_OrgUnit_1toN",
                    new ApiRelationshipPrincipalEnd(typeof(RelationshipOrgUnit)),
                    new ApiRelationshipDependentEnd(typeof(RelationshipOrgUnit),
                    [
                        new ApiRelationshipScalarKeyPath(nameof(RelationshipOrgUnit.ParentId))
                    ]));

                var schema = ApiSchema.Create(
                    "Test",
                    [ulidScalarType, orgUnitType],
                    apiRelationships: [relationship]);

                return (schema, relationship);
            },
            Assertions = (schema, rel) =>
            {
                var oneTo = (ApiRelationshipOneToMany)rel;
                oneTo.ApiPrincipalEnd.ApiRelationship.Should().BeSameAs(rel);
                oneTo.ApiDependentEnd.ApiRelationship.Should().BeSameAs(rel);
            }
        },
    ];
    #endregion

    #region Theory Tests
    [Theory]
    [MemberData(nameof(InitializeRelationshipCrossReferencesTheoryData))]
    public void InitializeRelationshipCrossReferences(IXUnitTest unitTest) => unitTest.Execute(this);
    #endregion
}
