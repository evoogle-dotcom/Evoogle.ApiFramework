// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema.Relationships;

public partial class ApiRelationshipTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private class KeyBindingTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required ApiRelationshipDef ExpectedApiRelationshipDef { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiRelationship? ActualApiRelationship { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{this.ApiSchemaKind} creation failed.");

            this.WriteLine($"ApiSchema:               {this.ApiSchema.SafeToString()}");
            this.WriteLine($"ExpectedApiRelationship: {this.ExpectedApiRelationshipDef.SafeToString()}");
            this.WriteLine();
        }
        #endregion

        protected override void Act()
        {
            var actualApiRelationship = this.ApiSchema?.GetRelationshipByApiName(this.ExpectedApiRelationshipDef.ApiName);
            this.ActualApiRelationship = actualApiRelationship ?? throw new InvalidOperationException($"{nameof(ApiRelationship)} creation failed.");
            this.WriteLine($"ActualApiRelationship:   {this.ActualApiRelationship.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualApiRelationship.Should().NotBeNull();

            var actualApiRelationship = this.ActualApiRelationship!;

            actualApiRelationship.ApiName.Should().Be(this.ExpectedApiRelationshipDef.ApiName);

            switch (this.ExpectedApiRelationshipDef, actualApiRelationship)
            {
                case (ApiRelationshipOneToOneDef expectedRelationshipDef, ApiRelationshipOneToOne actualRelationship) :
                    actualRelationship.ApiKind.Should().Be(ApiRelationshipKind.OneToOne);
                    actualRelationship.ApiDeleteBehavior.Should().Be(expectedRelationshipDef.ApiDeleteBehavior);

                    AssertOneToRelationshipBinding
                    (
                        expectedRelationshipDef.PrincipalEnd,
                        expectedRelationshipDef.DependentEnd,
                        actualRelationship
                    );
                    break;

                case (ApiRelationshipOneToManyDef expectedRelationshipDef, ApiRelationshipOneToMany actualRelationship) :
                    actualRelationship.ApiKind.Should().Be(ApiRelationshipKind.OneToMany);
                    actualRelationship.ApiDeleteBehavior.Should().Be(expectedRelationshipDef.ApiDeleteBehavior);

                    AssertOneToRelationshipBinding
                    (
                        expectedRelationshipDef.PrincipalEnd,
                        expectedRelationshipDef.DependentEnd,
                        actualRelationship
                    );
                    break;

                case (ApiRelationshipManyToManyDef expectedRelationshipDef, ApiRelationshipManyToMany actualRelationship) :
                    actualRelationship.ApiKind.Should().Be(ApiRelationshipKind.ManyToMany);
                    actualRelationship.ApiDeleteBehavior.Should().Be(expectedRelationshipDef.ApiDeleteBehavior);

                    AssertManyToManyRelationshipBinding
                    (
                        expectedRelationshipDef.PrincipalEndA,
                        expectedRelationshipDef.PrincipalEndB,
                        expectedRelationshipDef.Association,
                        actualRelationship
                    );
                    break;

                default:
                    var actualRelationshipTypeName = actualApiRelationship.GetType().Name;
                    throw new InvalidOperationException($"Unsupported {nameof(ApiRelationship)} type: {actualRelationshipTypeName}");
            }
        }

        private static void AssertOneToRelationshipBinding
        (
            ApiRelationshipPrincipalEndDef expectedPrincipalEnd,
            ApiRelationshipDependentEndDef expectedDependentEnd,
            ApiRelationshipOneTo actualRelationship
        )
        {
            AssertPrincipalEnd(expectedPrincipalEnd, actualRelationship.ApiPrincipalEnd);
            AssertDependentEnd(expectedDependentEnd, actualRelationship.ApiDependentEnd);

            var expectedHasKeyBinding = expectedDependentEnd.ApiForeignKey is not null;

            actualRelationship.IsNavigational.Should().Be(!expectedHasKeyBinding);
            actualRelationship.HasKeyBinding.Should().Be(expectedHasKeyBinding);

            if (!expectedHasKeyBinding)
            {
                Action getKeyBinding = () => _ = actualRelationship.ApiKeyBinding;
                getKeyBinding.Should().Throw<ApiSchemaException>();
                return;
            }

            AssertKeyDefinition
            (
                expectedDependentEnd.ApiForeignKey!,
                actualRelationship.ApiDependentEnd.ApiForeignKey
            );

            AssertRelationshipKeyBinding
            (
                expectedPrincipalEnd,
                actualRelationship.ApiPrincipalEnd,
                actualRelationship.ApiDependentEnd.ApiForeignKey,
                actualRelationship.ApiKeyBinding
            );
        }

        private static void AssertManyToManyRelationshipBinding
        (
            ApiRelationshipPrincipalEndDef expectedPrincipalEndA,
            ApiRelationshipPrincipalEndDef expectedPrincipalEndB,
            ApiRelationshipAssociationDef expectedAssociation,
            ApiRelationshipManyToMany actualRelationship
        )
        {
            AssertPrincipalEnd(expectedPrincipalEndA, actualRelationship.ApiPrincipalEndA);
            AssertPrincipalEnd(expectedPrincipalEndB, actualRelationship.ApiPrincipalEndB);
            AssertAssociation(expectedAssociation, actualRelationship.ApiAssociation);

            var expectedHasKeyBindings = expectedAssociation.ApiForeignKeyA is not null && expectedAssociation.ApiForeignKeyB is not null;

            actualRelationship.IsNavigational.Should().Be(!expectedHasKeyBindings);
            actualRelationship.HasKeyBindings.Should().Be(expectedHasKeyBindings);

            if (!expectedHasKeyBindings)
            {
                Action getKeyBindingA = () => _ = actualRelationship.ApiKeyBindingA;
                Action getKeyBindingB = () => _ = actualRelationship.ApiKeyBindingB;

                getKeyBindingA.Should().Throw<ApiSchemaException>();
                getKeyBindingB.Should().Throw<ApiSchemaException>();
                return;
            }

            AssertKeyDefinition
            (
                expectedAssociation.ApiForeignKeyA!,
                actualRelationship.ApiAssociation.ApiForeignKeyA
            );
            AssertKeyDefinition
            (
                expectedAssociation.ApiForeignKeyB!,
                actualRelationship.ApiAssociation.ApiForeignKeyB
            );

            AssertRelationshipKeyBinding
            (
                expectedPrincipalEndA,
                actualRelationship.ApiPrincipalEndA,
                actualRelationship.ApiAssociation.ApiForeignKeyA,
                actualRelationship.ApiKeyBindingA
            );

            AssertRelationshipKeyBinding
            (
                expectedPrincipalEndB,
                actualRelationship.ApiPrincipalEndB,
                actualRelationship.ApiAssociation.ApiForeignKeyB,
                actualRelationship.ApiKeyBindingB
            );
        }

        private static void AssertRelationshipKeyBinding
        (
            ApiRelationshipPrincipalEndDef expectedPrincipalEnd,
            ApiRelationshipPrincipalEnd actualPrincipalEnd,
            ApiKeyDefinition actualForeignKey,
            ApiRelationshipKeyBinding actualKeyBinding
        )
        {
            var expectedPrincipalKey = ResolveExpectedPrincipalKey
            (
                expectedPrincipalEnd,
                actualPrincipalEnd,
                actualForeignKey
            );

            var expectedResolutionSource = expectedPrincipalEnd.ApiPrincipalKeyName is null
                ? ApiRelationshipPrincipalKeyResolutionSource.Inferred
                : ApiRelationshipPrincipalKeyResolutionSource.Explicit;

            actualKeyBinding.ApiPrincipalEnd.Should().BeSameAs(actualPrincipalEnd);
            actualKeyBinding.ApiPrincipalKey.Should().BeSameAs(expectedPrincipalKey);
            actualKeyBinding.ApiForeignKey.Should().BeSameAs(actualForeignKey);
            actualKeyBinding.ApiPrincipalKeyName.Should().Be(expectedPrincipalKey.ApiName);
            actualKeyBinding.ApiPrincipalKeyResolutionSource.Should().Be(expectedResolutionSource);
        }

        private static ApiNamedKeyDefinition ResolveExpectedPrincipalKey
        (
            ApiRelationshipPrincipalEndDef expectedPrincipalEnd,
            ApiRelationshipPrincipalEnd actualPrincipalEnd,
            ApiKeyDefinition actualForeignKey
        )
        {
            if (expectedPrincipalEnd.ApiPrincipalKeyName is { } apiPrincipalKeyName)
            {
                return actualPrincipalEnd.ApiObjectType.GetKeyByApiName(apiPrincipalKeyName);
            }

            var compatiblePrincipalKeys = actualPrincipalEnd.ApiObjectType.ApiKeys
                .Where(apiKeyDefinition => HaveCompatibleLeafTypes(apiKeyDefinition, actualForeignKey))
                .ToArray();

            var compatiblePrincipalKey = compatiblePrincipalKeys.Should().ContainSingle().Which;
            return compatiblePrincipalKey;
        }

        private static bool HaveCompatibleLeafTypes
        (
            ApiKeyDefinition principalKey,
            ApiKeyDefinition foreignKey
        )
        {
            var principalLeafTypes = GetKeyLeafTypes(principalKey);
            var foreignLeafTypes = GetKeyLeafTypes(foreignKey);

            return principalLeafTypes.SequenceEqual(foreignLeafTypes);
        }

        private static Type[] GetKeyLeafTypes(ApiKeyDefinition keyDefinition)
        {
            return
            [
                .. keyDefinition.ApiKeyPaths.Select
                (
                    static apiKeyPath => apiKeyPath.ApiScalarSegment.ApiProperty.ApiType.ClrType
                )
            ];
        }

        private static void AssertPrincipalEnd
        (
            ApiRelationshipPrincipalEndDef expectedEnd,
            ApiRelationshipPrincipalEnd actualEnd
        )
        {
            actualEnd.ApiObjectTypeReference.ApiKind.Should().Be(expectedEnd.ApiObjectTypeReference.ApiKind);
            actualEnd.ApiObjectTypeReference.ApiName.Should().Be(expectedEnd.ApiObjectTypeReference.ApiName);
            actualEnd.ApiObjectTypeReference.ClrType.Should().Be(expectedEnd.ApiObjectTypeReference.ClrType);
            actualEnd.ApiPrincipalKeyName.Should().Be(expectedEnd.ApiPrincipalKeyName);
            actualEnd.ApiObjectType.ClrType.Should().Be(expectedEnd.ApiObjectTypeReference.ClrType);
        }

        private static void AssertDependentEnd
        (
            ApiRelationshipDependentEndDef expectedEnd,
            ApiRelationshipDependentEnd actualEnd
        )
        {
            actualEnd.ApiObjectTypeReference.ApiKind.Should().Be(expectedEnd.ApiObjectTypeReference.ApiKind);
            actualEnd.ApiObjectTypeReference.ApiName.Should().Be(expectedEnd.ApiObjectTypeReference.ApiName);
            actualEnd.ApiObjectTypeReference.ClrType.Should().Be(expectedEnd.ApiObjectTypeReference.ClrType);
            actualEnd.HasForeignKey.Should().Be(expectedEnd.ApiForeignKey is not null);
            actualEnd.ApiObjectType.ClrType.Should().Be(expectedEnd.ApiObjectTypeReference.ClrType);
        }

        private static void AssertAssociation
        (
            ApiRelationshipAssociationDef expectedAssociation,
            ApiRelationshipAssociation actualAssociation
        )
        {
            actualAssociation.ApiObjectTypeReference.ApiKind.Should().Be(expectedAssociation.ApiObjectTypeReference.ApiKind);
            actualAssociation.ApiObjectTypeReference.ApiName.Should().Be(expectedAssociation.ApiObjectTypeReference.ApiName);
            actualAssociation.ApiObjectTypeReference.ClrType.Should().Be(expectedAssociation.ApiObjectTypeReference.ClrType);
            actualAssociation.HasForeignKeys.Should().Be
            (
                expectedAssociation.ApiForeignKeyA is not null &&
                expectedAssociation.ApiForeignKeyB is not null
            );
            actualAssociation.ApiObjectType.ClrType.Should().Be
                (expectedAssociation.ApiObjectTypeReference.ClrType);
        }

        private static void AssertKeyDefinition(ApiKeyDef expectedKey, ApiKeyDefinition actualKey)
        {
            if (actualKey is ApiNamedKeyDefinition actualNamedKey)
            {
                actualNamedKey.ApiName.Should().Be(expectedKey.ApiName);
            }
            else
            {
                actualKey.Should().BeOfType<ApiKeyDefinition>();
            }

            actualKey.ApiKeyPaths.Should().HaveCount(expectedKey.ApiKeyPaths.Count);

            for (var i = 0; i < expectedKey.ApiKeyPaths.Count; i++)
            {
                AssertKeyPath(expectedKey.ApiKeyPaths[i], actualKey.ApiKeyPaths[i]);
            }
        }

        private static void AssertKeyPath(ApiKeyPathDef expectedKeyPath, ApiKeyPath actualKeyPath)
        {
            actualKeyPath.ApiRootObjectTypeReference?.ApiKind.Should().Be(expectedKeyPath.ApiRootObjectTypeReference?.ApiKind);
            actualKeyPath.ApiRootObjectTypeReference?.ApiName.Should().Be(expectedKeyPath.ApiRootObjectTypeReference?.ApiName);
            actualKeyPath.ApiRootObjectTypeReference?.ClrType.Should().Be(expectedKeyPath.ApiRootObjectTypeReference?.ClrType);
            if (expectedKeyPath.ApiRootObjectTypeReference is null)
            {
                actualKeyPath.ClrRootType.Should().NotBeNull();
            }
            else
            {
                actualKeyPath.ClrRootType.Should().Be(expectedKeyPath.ApiRootObjectTypeReference.ClrType);
            }

            actualKeyPath.ApiSegments.Should().HaveCount(expectedKeyPath.ApiKeyPathSegments.Count);

            for (var i = 0; i < expectedKeyPath.ApiKeyPathSegments.Count; i++)
            {
                actualKeyPath.ApiSegments[i].ClrMemberName.Should().Be(expectedKeyPath.ApiKeyPathSegments[i].ClrMemberName);
            }
        }
    }
    #endregion
}
