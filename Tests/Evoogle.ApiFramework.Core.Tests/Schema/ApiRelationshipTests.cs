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

namespace Evoogle.ApiFramework.Schema;

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
            this.ApiSchema = apiSchema
                ?? throw new InvalidOperationException($"{this.ApiSchemaKind} creation failed.");

            this.WriteLine($"ApiSchema:               {this.ApiSchema.SafeToString()}");
            this.WriteLine
            (
                $"ExpectedApiRelationship: {this.ExpectedApiRelationshipDef.SafeToString()}"
            );
            this.WriteLine();
        }
        #endregion

        protected override void Act()
        {
            var actualApiRelationship = this.ApiSchema?.GetRelationshipByApiName
            (
                this.ExpectedApiRelationshipDef.ApiName
            );
            this.ActualApiRelationship = actualApiRelationship
                ?? throw new InvalidOperationException
                (
                    $"{nameof(ApiRelationship)} creation failed."
                );
            this.WriteLine
            (
                $"ActualApiRelationship:   {this.ActualApiRelationship.SafeToString()}"
            );
        }

        protected override void Assert()
        {
            this.ActualApiRelationship.Should().NotBeNull();

            var actualApiRelationship = this.ActualApiRelationship!;

            actualApiRelationship.ApiName.Should().Be(this.ExpectedApiRelationshipDef.ApiName);

            switch (this.ExpectedApiRelationshipDef, actualApiRelationship)
            {
                case
                (
                    ApiRelationshipOneToOneDef expectedRelationshipDef,
                    ApiRelationshipOneToOne actualRelationship
                ):
                    actualRelationship.ApiKind.Should().Be(ApiRelationshipKind.OneToOne);
                    actualRelationship.ApiDeleteBehavior
                        .Should().Be(expectedRelationshipDef.ApiDeleteBehavior);

                    AssertOneToRelationshipBinding
                    (
                        expectedRelationshipDef.PrincipalEnd,
                        expectedRelationshipDef.DependentEnd,
                        actualRelationship
                    );
                    break;

                case
                (
                    ApiRelationshipOneToManyDef expectedRelationshipDef,
                    ApiRelationshipOneToMany actualRelationship
                ):
                    actualRelationship.ApiKind.Should().Be(ApiRelationshipKind.OneToMany);
                    actualRelationship.ApiDeleteBehavior
                        .Should().Be(expectedRelationshipDef.ApiDeleteBehavior);

                    AssertOneToRelationshipBinding
                    (
                        expectedRelationshipDef.PrincipalEnd,
                        expectedRelationshipDef.DependentEnd,
                        actualRelationship
                    );
                    break;

                case
                (
                    ApiRelationshipManyToManyDef expectedRelationshipDef,
                    ApiRelationshipManyToMany actualRelationship
                ):
                    actualRelationship.ApiKind.Should().Be(ApiRelationshipKind.ManyToMany);
                    actualRelationship.ApiDeleteBehavior
                        .Should().Be(expectedRelationshipDef.ApiDeleteBehavior);

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
                    throw new InvalidOperationException
                    (
                        $"Unsupported {nameof(ApiRelationship)} type: {actualRelationshipTypeName}"
                    );
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

            var expectedHasKeyBinding = expectedDependentEnd.ApiForeignKeyType is not null;

            actualRelationship.IsNavigational.Should().Be(!expectedHasKeyBinding);
            actualRelationship.HasKeyBinding.Should().Be(expectedHasKeyBinding);

            if (!expectedHasKeyBinding)
            {
                Action getKeyBinding = () => _ = actualRelationship.ApiKeyBinding;
                getKeyBinding.Should().Throw<ApiSchemaException>();
                return;
            }

            AssertKeyType
            (
                expectedDependentEnd.ApiForeignKeyType!,
                actualRelationship.ApiDependentEnd.ApiForeignKeyType
            );

            AssertRelationshipKeyBinding
            (
                expectedPrincipalEnd,
                actualRelationship.ApiPrincipalEnd,
                actualRelationship.ApiDependentEnd.ApiForeignKeyType,
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

            var expectedHasKeyBindings = expectedAssociation.ApiForeignKeyTypeA is not null &&
                expectedAssociation.ApiForeignKeyTypeB is not null;

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

            AssertKeyType
            (
                expectedAssociation.ApiForeignKeyTypeA!,
                actualRelationship.ApiAssociation.ApiForeignKeyTypeA
            );
            AssertKeyType
            (
                expectedAssociation.ApiForeignKeyTypeB!,
                actualRelationship.ApiAssociation.ApiForeignKeyTypeB
            );

            AssertRelationshipKeyBinding
            (
                expectedPrincipalEndA,
                actualRelationship.ApiPrincipalEndA,
                actualRelationship.ApiAssociation.ApiForeignKeyTypeA,
                actualRelationship.ApiKeyBindingA
            );

            AssertRelationshipKeyBinding
            (
                expectedPrincipalEndB,
                actualRelationship.ApiPrincipalEndB,
                actualRelationship.ApiAssociation.ApiForeignKeyTypeB,
                actualRelationship.ApiKeyBindingB
            );
        }

        private static void AssertRelationshipKeyBinding
        (
            ApiRelationshipPrincipalEndDef expectedPrincipalEnd,
            ApiRelationshipPrincipalEnd actualPrincipalEnd,
            ApiKeyType actualForeignKeyType,
            ApiRelationshipKeyBinding actualKeyBinding
        )
        {
            var expectedPrincipalKeyType = ResolveExpectedPrincipalKeyType
            (
                expectedPrincipalEnd,
                actualPrincipalEnd,
                actualForeignKeyType
            );

            var expectedResolutionSource = expectedPrincipalEnd.ApiPrincipalKeyTypeName is null
                ? ApiRelationshipPrincipalKeyResolutionSource.Inferred
                : ApiRelationshipPrincipalKeyResolutionSource.Explicit;

            actualKeyBinding.ApiPrincipalEnd.Should().BeSameAs(actualPrincipalEnd);
            actualKeyBinding.ApiPrincipalKeyType.Should().BeSameAs(expectedPrincipalKeyType);
            actualKeyBinding.ApiForeignKeyType.Should().BeSameAs(actualForeignKeyType);
            actualKeyBinding.ApiPrincipalKeyTypeName.Should().Be(expectedPrincipalKeyType.ApiName);
            actualKeyBinding.ApiPrincipalKeyResolutionSource.Should().Be(expectedResolutionSource);
        }

        private static ApiKeyType ResolveExpectedPrincipalKeyType
        (
            ApiRelationshipPrincipalEndDef expectedPrincipalEnd,
            ApiRelationshipPrincipalEnd actualPrincipalEnd,
            ApiKeyType actualForeignKeyType
        )
        {
            if (expectedPrincipalEnd.ApiPrincipalKeyTypeName is { } apiPrincipalKeyTypeName)
            {
                return actualPrincipalEnd.ApiObjectType
                    .GetKeyTypeByApiName(apiPrincipalKeyTypeName);
            }

            var compatiblePrincipalKeyTypes = actualPrincipalEnd.ApiObjectType.ApiKeyTypes
                .Where(apiKeyType => HaveCompatibleLeafTypes(apiKeyType, actualForeignKeyType))
                .ToArray();

            var compatiblePrincipalKeyType = compatiblePrincipalKeyTypes
                .Should().ContainSingle().Which;
            return compatiblePrincipalKeyType;
        }

        private static bool HaveCompatibleLeafTypes
        (
            ApiKeyType principalKeyType,
            ApiKeyType foreignKeyType
        )
        {
            var principalLeafTypes = GetKeyLeafTypes(principalKeyType);
            var foreignLeafTypes = GetKeyLeafTypes(foreignKeyType);

            return principalLeafTypes.SequenceEqual(foreignLeafTypes);
        }

        private static Type[] GetKeyLeafTypes(ApiKeyType keyType)
        {
            return
            [
                .. keyType.ApiKeyPaths.Select
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
            actualEnd.ClrObjectType.Should().Be(expectedEnd.ClrObjectType);
            actualEnd.ApiPrincipalKeyTypeName.Should().Be(expectedEnd.ApiPrincipalKeyTypeName);
            actualEnd.ApiObjectType.ClrType.Should().Be(expectedEnd.ClrObjectType);
        }

        private static void AssertDependentEnd
        (
            ApiRelationshipDependentEndDef expectedEnd,
            ApiRelationshipDependentEnd actualEnd
        )
        {
            actualEnd.ClrObjectType.Should().Be(expectedEnd.ClrObjectType);
            actualEnd.HasForeignKey.Should().Be(expectedEnd.ApiForeignKeyType is not null);
            actualEnd.ApiObjectType.ClrType.Should().Be(expectedEnd.ClrObjectType);
        }

        private static void AssertAssociation
        (
            ApiRelationshipAssociationDef expectedAssociation,
            ApiRelationshipAssociation actualAssociation
        )
        {
            actualAssociation.ClrObjectType.Should().Be(expectedAssociation.ClrObjectType);
            actualAssociation.HasForeignKeys.Should().Be
            (
                expectedAssociation.ApiForeignKeyTypeA is not null &&
                expectedAssociation.ApiForeignKeyTypeB is not null
            );
            actualAssociation.ApiObjectType.ClrType.Should().Be(expectedAssociation.ClrObjectType);
        }

        private static void AssertKeyType(ApiKeyTypeDef expectedKeyType, ApiKeyType actualKeyType)
        {
            actualKeyType.ApiName.Should().Be(expectedKeyType.ApiName);
            actualKeyType.ApiKeyPaths.Should().HaveCount(expectedKeyType.ApiKeyPaths.Count);

            for (var i = 0; i < expectedKeyType.ApiKeyPaths.Count; i++)
            {
                AssertKeyPath(expectedKeyType.ApiKeyPaths[i], actualKeyType.ApiKeyPaths[i]);
            }
        }

        private static void AssertKeyPath(ApiKeyPathDef expectedKeyPath, ApiKeyPath actualKeyPath)
        {
            actualKeyPath.ClrRootType.Should().Be(expectedKeyPath.ClrRootType);
            actualKeyPath.ApiSegments.Should().HaveCount(expectedKeyPath.ApiKeyPathSegments.Count);

            for (var i = 0; i < expectedKeyPath.ApiKeyPathSegments.Count; i++)
            {
                actualKeyPath.ApiSegments[i].ClrPropertyName.Should().Be
                (
                    expectedKeyPath.ApiKeyPathSegments[i].ClrPropertyName
                );
            }
        }
    }
    #endregion
}
