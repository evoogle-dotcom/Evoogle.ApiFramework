// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Immutable;
using System.Reflection;

using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Key;

public partial class ApiKeyDefinitionTests
{
    #region Test Types
    private class TypeHierarchyTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiName { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyDefinition? ApiKeyDefinition { get; set; }
        private ApiNamedKeyDefinition? ApiNamedKeyDefinition { get; set; }
        private PropertyInfo? ApiKeyDefinitionApiNameProperty { get; set; }
        private PropertyInfo? ApiNamedKeyDefinitionApiNameProperty { get; set; }
        private NullabilityState? ApiNamedKeyDefinitionApiNameNullability { get; set; }
        private ParameterInfo? ApiNameConstructorParameter { get; set; }
        private NullabilityState? ApiNameConstructorParameterNullability { get; set; }
        private Type? ApiObjectKeysPropertyType { get; set; }
        private ConstructorInfo[]? ApiRelationshipKeyBindingPublicConstructors { get; set; }
        private ConstructorInfo? ApiRelationshipKeyBindingInternalConstructor { get; set; }
        private Type[]? ApiRelationshipKeyBindingConstructorParameterTypes { get; set; }
        private Type? ApiRelationshipPrincipalKeyPropertyType { get; set; }
        private NullabilityState? ApiRelationshipPrincipalKeyNameNullability { get; set; }
        private Type[]? ApiRelationshipForeignKeyPropertyTypes { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiKeyDefinition = new ApiKeyDefinition([]);
            this.ApiNamedKeyDefinition = new ApiNamedKeyDefinition(this.ApiName, []);
        }

        protected override void Act()
        {
            this.ApiKeyDefinitionApiNameProperty = typeof(ApiKeyDefinition).GetProperty
            (
                nameof(this.ApiNamedKeyDefinition.ApiName),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
            );
            this.ApiNamedKeyDefinitionApiNameProperty = typeof(ApiNamedKeyDefinition).GetProperty
            (
                nameof(this.ApiNamedKeyDefinition.ApiName),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
            );

            this.ApiNamedKeyDefinitionApiNameNullability = this.ApiNamedKeyDefinitionApiNameProperty is not null
                ? new NullabilityInfoContext()
                    .Create(this.ApiNamedKeyDefinitionApiNameProperty)
                    .ReadState
                : null;
            this.ApiNameConstructorParameter = typeof(ApiNamedKeyDefinition)
                .GetConstructors()
                .Single()
                .GetParameters()
                .Single(parameter => parameter.Name == "apiName");
            this.ApiNameConstructorParameterNullability = new NullabilityInfoContext()
                .Create(this.ApiNameConstructorParameter)
                .ReadState;

            this.ApiObjectKeysPropertyType = typeof(ApiObjectType)
                .GetProperty(nameof(ApiObjectType.ApiKeys))
                ?.PropertyType;
            this.ApiRelationshipKeyBindingPublicConstructors = typeof(ApiRelationshipKeyBinding)
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            this.ApiRelationshipKeyBindingInternalConstructor = typeof(ApiRelationshipKeyBinding)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .SingleOrDefault();
            this.ApiRelationshipKeyBindingConstructorParameterTypes =
                this.ApiRelationshipKeyBindingInternalConstructor
                    ?.GetParameters()
                    .Select(static parameter => parameter.ParameterType)
                    .ToArray();
            this.ApiRelationshipPrincipalKeyPropertyType = typeof(ApiRelationshipKeyBinding)
                .GetProperty(nameof(ApiRelationshipKeyBinding.ApiPrincipalKey))
                ?.PropertyType;

            var apiPrincipalKeyNameProperty = typeof(ApiRelationshipKeyBinding)
                .GetProperty(nameof(ApiRelationshipKeyBinding.ApiPrincipalKeyName));
            this.ApiRelationshipPrincipalKeyNameNullability =
                apiPrincipalKeyNameProperty is not null
                    ? new NullabilityInfoContext()
                        .Create(apiPrincipalKeyNameProperty)
                        .ReadState
                    : null;

            this.ApiRelationshipForeignKeyPropertyTypes =
            [
                typeof(ApiRelationshipDependentEnd)
                    .GetProperty(nameof(ApiRelationshipDependentEnd.ApiForeignKey))!
                    .PropertyType,
                typeof(ApiRelationshipAssociation)
                    .GetProperty(nameof(ApiRelationshipAssociation.ApiForeignKeyA))!
                    .PropertyType,
                typeof(ApiRelationshipAssociation)
                    .GetProperty(nameof(ApiRelationshipAssociation.ApiForeignKeyB))!
                    .PropertyType,
                typeof(ApiRelationshipKeyBinding)
                    .GetProperty(nameof(ApiRelationshipKeyBinding.ApiForeignKey))!
                    .PropertyType,
            ];
        }

        protected override void Assert()
        {
            this.ApiKeyDefinition.Should().BeOfType<ApiKeyDefinition>();
            this.ApiNamedKeyDefinition.Should().BeOfType<ApiNamedKeyDefinition>();
            this.ApiNamedKeyDefinition.Should().BeAssignableTo<ApiKeyDefinition>();
            this.ApiNamedKeyDefinition!.ApiName.Should().Be(this.ApiName);

            this.ApiKeyDefinitionApiNameProperty.Should().BeNull();
            this.ApiNamedKeyDefinitionApiNameProperty.Should().NotBeNull();
            this.ApiNamedKeyDefinitionApiNameProperty!.PropertyType.Should().Be<string>();
            this.ApiNamedKeyDefinitionApiNameNullability.Should().Be(NullabilityState.NotNull);
            this.ApiNameConstructorParameter.Should().NotBeNull();
            this.ApiNameConstructorParameter!.IsOptional.Should().BeFalse();
            this.ApiNameConstructorParameter.ParameterType.Should().Be<string>();
            this.ApiNameConstructorParameterNullability.Should().Be(NullabilityState.NotNull);

            this.ApiObjectKeysPropertyType.Should()
                .Be<ImmutableArray<ApiNamedKeyDefinition>>();
            this.ApiRelationshipKeyBindingPublicConstructors.Should().BeEmpty();
            this.ApiRelationshipKeyBindingInternalConstructor.Should().NotBeNull();
            this.ApiRelationshipKeyBindingInternalConstructor!.IsAssembly.Should().BeTrue();
            this.ApiRelationshipKeyBindingConstructorParameterTypes.Should().Equal
            (
                typeof(ApiRelationshipPrincipalEnd),
                typeof(ApiNamedKeyDefinition),
                typeof(ApiKeyDefinition),
                typeof(ApiRelationshipPrincipalKeyResolutionSource)
            );
            this.ApiRelationshipPrincipalKeyPropertyType.Should().Be<ApiNamedKeyDefinition>();
            this.ApiRelationshipPrincipalKeyNameNullability
                .Should()
                .Be(NullabilityState.NotNull);
            this.ApiRelationshipForeignKeyPropertyTypes.Should().OnlyContain
            (
                propertyType => propertyType == typeof(ApiKeyDefinition)
            );
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TypeHierarchyTheoryData =>
    [
        new TypeHierarchyTest
        {
            Name = $"{nameof(ApiNamedKeyDefinition)} owns a required non-nullable " +
                $"{nameof(ApiNamedKeyDefinition.ApiName)}, object and resolved principal keys " +
                "are named, " +
                $"and relationship foreign keys retain {nameof(ApiKeyDefinition)}",
            ApiName = "PrimaryKey"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TypeHierarchyTheoryData))]
    public void TypeHierarchy(IXUnitTest test) => test.Execute(this);
    #endregion
}
