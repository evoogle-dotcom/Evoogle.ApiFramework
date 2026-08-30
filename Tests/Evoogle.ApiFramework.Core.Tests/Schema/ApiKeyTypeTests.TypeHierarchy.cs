// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiKeyTypeTests
{
    #region Test Types
    private class TypeHierarchyTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiName { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyType? ApiKeyType { get; set; }
        private ApiNamedKeyType? ApiNamedKeyType { get; set; }
        private PropertyInfo? ApiKeyTypeApiNameProperty { get; set; }
        private PropertyInfo? ApiNamedKeyTypeApiNameProperty { get; set; }
        private NullabilityState? ApiNamedKeyTypeApiNameNullability { get; set; }
        private ParameterInfo? ApiNameConstructorParameter { get; set; }
        private NullabilityState? ApiNameConstructorParameterNullability { get; set; }
        private Type? ApiObjectKeyTypesPropertyType { get; set; }
        private ConstructorInfo[]? ApiRelationshipKeyBindingPublicConstructors { get; set; }
        private ConstructorInfo? ApiRelationshipKeyBindingInternalConstructor { get; set; }
        private Type[]? ApiRelationshipKeyBindingConstructorParameterTypes { get; set; }
        private Type? ApiRelationshipPrincipalKeyTypePropertyType { get; set; }
        private NullabilityState? ApiRelationshipPrincipalKeyTypeNameNullability { get; set; }
        private Type[]? ApiRelationshipForeignKeyTypePropertyTypes { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiKeyType = new ApiKeyType([]);
            this.ApiNamedKeyType = new ApiNamedKeyType(this.ApiName, []);
        }

        protected override void Act()
        {
            this.ApiKeyTypeApiNameProperty = typeof(ApiKeyType).GetProperty
            (
                nameof(this.ApiNamedKeyType.ApiName),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
            );
            this.ApiNamedKeyTypeApiNameProperty = typeof(ApiNamedKeyType).GetProperty
            (
                nameof(this.ApiNamedKeyType.ApiName),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
            );

            this.ApiNamedKeyTypeApiNameNullability = this.ApiNamedKeyTypeApiNameProperty is not null
                ? new NullabilityInfoContext()
                    .Create(this.ApiNamedKeyTypeApiNameProperty)
                    .ReadState
                : null;
            this.ApiNameConstructorParameter = typeof(ApiNamedKeyType)
                .GetConstructors()
                .Single()
                .GetParameters()
                .Single(parameter => parameter.Name == "apiName");
            this.ApiNameConstructorParameterNullability = new NullabilityInfoContext()
                .Create(this.ApiNameConstructorParameter)
                .ReadState;

            this.ApiObjectKeyTypesPropertyType = typeof(ApiObjectType)
                .GetProperty(nameof(ApiObjectType.ApiKeyTypes))
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
            this.ApiRelationshipPrincipalKeyTypePropertyType = typeof(ApiRelationshipKeyBinding)
                .GetProperty(nameof(ApiRelationshipKeyBinding.ApiPrincipalKeyType))
                ?.PropertyType;

            var apiPrincipalKeyTypeNameProperty = typeof(ApiRelationshipKeyBinding)
                .GetProperty(nameof(ApiRelationshipKeyBinding.ApiPrincipalKeyTypeName));
            this.ApiRelationshipPrincipalKeyTypeNameNullability =
                apiPrincipalKeyTypeNameProperty is not null
                    ? new NullabilityInfoContext()
                        .Create(apiPrincipalKeyTypeNameProperty)
                        .ReadState
                    : null;

            this.ApiRelationshipForeignKeyTypePropertyTypes =
            [
                typeof(ApiRelationshipDependentEnd)
                    .GetProperty(nameof(ApiRelationshipDependentEnd.ApiForeignKeyType))!
                    .PropertyType,
                typeof(ApiRelationshipAssociation)
                    .GetProperty(nameof(ApiRelationshipAssociation.ApiForeignKeyTypeA))!
                    .PropertyType,
                typeof(ApiRelationshipAssociation)
                    .GetProperty(nameof(ApiRelationshipAssociation.ApiForeignKeyTypeB))!
                    .PropertyType,
                typeof(ApiRelationshipKeyBinding)
                    .GetProperty(nameof(ApiRelationshipKeyBinding.ApiForeignKeyType))!
                    .PropertyType,
            ];
        }

        protected override void Assert()
        {
            this.ApiKeyType.Should().BeOfType<ApiKeyType>();
            this.ApiNamedKeyType.Should().BeOfType<ApiNamedKeyType>();
            this.ApiNamedKeyType.Should().BeAssignableTo<ApiKeyType>();
            this.ApiNamedKeyType!.ApiName.Should().Be(this.ApiName);

            this.ApiKeyTypeApiNameProperty.Should().BeNull();
            this.ApiNamedKeyTypeApiNameProperty.Should().NotBeNull();
            this.ApiNamedKeyTypeApiNameProperty!.PropertyType.Should().Be<string>();
            this.ApiNamedKeyTypeApiNameNullability.Should().Be(NullabilityState.NotNull);
            this.ApiNameConstructorParameter.Should().NotBeNull();
            this.ApiNameConstructorParameter!.IsOptional.Should().BeFalse();
            this.ApiNameConstructorParameter.ParameterType.Should().Be<string>();
            this.ApiNameConstructorParameterNullability.Should().Be(NullabilityState.NotNull);

            this.ApiObjectKeyTypesPropertyType.Should().Be<ApiNamedKeyType[]>();
            this.ApiRelationshipKeyBindingPublicConstructors.Should().BeEmpty();
            this.ApiRelationshipKeyBindingInternalConstructor.Should().NotBeNull();
            this.ApiRelationshipKeyBindingInternalConstructor!.IsAssembly.Should().BeTrue();
            this.ApiRelationshipKeyBindingConstructorParameterTypes.Should().Equal
            (
                typeof(ApiRelationshipPrincipalEnd),
                typeof(ApiNamedKeyType),
                typeof(ApiKeyType),
                typeof(ApiRelationshipPrincipalKeyResolutionSource)
            );
            this.ApiRelationshipPrincipalKeyTypePropertyType.Should().Be<ApiNamedKeyType>();
            this.ApiRelationshipPrincipalKeyTypeNameNullability
                .Should()
                .Be(NullabilityState.NotNull);
            this.ApiRelationshipForeignKeyTypePropertyTypes.Should().OnlyContain
            (
                propertyType => propertyType == typeof(ApiKeyType)
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
            Name = $"{nameof(ApiNamedKeyType)} owns a required non-nullable " +
                $"{nameof(ApiNamedKeyType.ApiName)}, object and resolved principal keys " +
                "are named, " +
                $"and relationship foreign keys retain {nameof(ApiKeyType)}",
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
