// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;
using System.Runtime.CompilerServices;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Annotations;

public partial class ApiAnnotationTests
{
    #region Test Methods
    [Fact]
    public void AnnotationConstructorsAreParameterless()
    {
        var annotationTypes = new[]
        {
            typeof(ApiObjectAttribute),
            typeof(ApiScalarAttribute),
            typeof(ApiEnumAttribute),
            typeof(ApiEnumValueAttribute),
            typeof(ApiPropertyAttribute),
            typeof(ApiKeyAttribute),
            typeof(ApiRelationshipAttribute),
            typeof(ApiRelationshipTypeAttribute),
            typeof(ApiManyToManyRelationshipAttribute),
            typeof(ApiManyToManyRelationshipTypeAttribute)
        };

        foreach (var annotationType in annotationTypes)
        {
            var constructors = annotationType.GetConstructors
            (
                BindingFlags.Instance | BindingFlags.Public
            );

            constructors.Should().ContainSingle();
            constructors[0].GetParameters().Should().BeEmpty();
        }
    }

    [Fact]
    public void AnnotationPropertiesUseInitAccessors()
    {
        var annotationTypes = new[]
        {
            typeof(ApiObjectAttribute),
            typeof(ApiScalarAttribute),
            typeof(ApiEnumAttribute),
            typeof(ApiEnumValueAttribute),
            typeof(ApiPropertyAttribute),
            typeof(ApiKeyAttribute),
            typeof(ApiRelationshipAttribute),
            typeof(ApiRelationshipTypeAttribute),
            typeof(ApiManyToManyRelationshipAttribute),
            typeof(ApiManyToManyRelationshipTypeAttribute)
        };

        foreach (var annotationType in annotationTypes)
        {
            var properties = annotationType.GetProperties
            (
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly
            );

            foreach (var property in properties)
            {
                property.GetMethod.Should().NotBeNull();

                var setter = property.GetSetMethod(nonPublic: true);
                setter.Should().NotBeNull();
                setter!.ReturnParameter
                    .GetRequiredCustomModifiers()
                    .Should()
                    .Contain(typeof(IsExternalInit));
            }
        }

        var baseApiName = typeof(ApiNamedElementAttribute).GetProperty
        (
            nameof(ApiNamedElementAttribute.ApiName)
        );

        baseApiName.Should().NotBeNull();
        baseApiName!.GetSetMethod(nonPublic: true).Should().NotBeNull();
        baseApiName.GetSetMethod(nonPublic: true)!.ReturnParameter
            .GetRequiredCustomModifiers()
            .Should()
            .Contain(typeof(IsExternalInit));
    }

    [Fact]
    public void RequiredAnnotationPropertiesExposeRequiredMetadata()
    {
        var requiredProperties = new[]
        {
            (typeof(ApiRelationshipAttribute), nameof(ApiRelationshipAttribute.ApiName)),
            (typeof(ApiRelationshipTypeAttribute), nameof(ApiRelationshipTypeAttribute.ApiName)),
            (
                typeof(ApiRelationshipTypeAttribute),
                nameof(ApiRelationshipTypeAttribute.PrincipalType)
            ),
            (
                typeof(ApiRelationshipTypeAttribute),
                nameof(ApiRelationshipTypeAttribute.DependentType)
            ),
            (
                typeof(ApiManyToManyRelationshipAttribute),
                nameof(ApiManyToManyRelationshipAttribute.ApiName)
            ),
            (
                typeof(ApiManyToManyRelationshipAttribute),
                nameof(ApiManyToManyRelationshipAttribute.AssociationType)
            ),
            (
                typeof(ApiManyToManyRelationshipAttribute),
                nameof(ApiManyToManyRelationshipAttribute.OtherPrincipalType)
            ),
            (
                typeof(ApiManyToManyRelationshipTypeAttribute),
                nameof(ApiManyToManyRelationshipTypeAttribute.ApiName)
            ),
            (
                typeof(ApiManyToManyRelationshipTypeAttribute),
                nameof(ApiManyToManyRelationshipTypeAttribute.PrincipalTypeA)
            ),
            (
                typeof(ApiManyToManyRelationshipTypeAttribute),
                nameof(ApiManyToManyRelationshipTypeAttribute.PrincipalTypeB)
            ),
            (
                typeof(ApiManyToManyRelationshipTypeAttribute),
                nameof(ApiManyToManyRelationshipTypeAttribute.AssociationType)
            )
        };

        foreach (var (annotationType, propertyName) in requiredProperties)
        {
            var property = annotationType.GetProperty(propertyName);

            property.Should().NotBeNull();
            property!.GetCustomAttribute<RequiredMemberAttribute>().Should().NotBeNull();
        }
    }

    [Fact]
    public void OptionalAnnotationPropertiesExposeNullableMetadata()
    {
        var optionalProperties = new[]
        {
            (typeof(ApiNamedElementAttribute), nameof(ApiNamedElementAttribute.ApiName)),
            (typeof(ApiObjectAttribute), nameof(ApiObjectAttribute.ApiName)),
            (typeof(ApiScalarAttribute), nameof(ApiScalarAttribute.ApiName)),
            (typeof(ApiEnumAttribute), nameof(ApiEnumAttribute.ApiName)),
            (typeof(ApiEnumValueAttribute), nameof(ApiEnumValueAttribute.ApiName)),
            (typeof(ApiPropertyAttribute), nameof(ApiPropertyAttribute.ApiName)),
            (typeof(ApiKeyAttribute), nameof(ApiKeyAttribute.ClrRootType)),
            (typeof(ApiKeyAttribute), nameof(ApiKeyAttribute.ClrPath)),
            (typeof(ApiRelationshipAttribute), nameof(ApiRelationshipAttribute.ForeignKey)),
            (
                typeof(ApiRelationshipTypeAttribute),
                nameof(ApiRelationshipTypeAttribute.ForeignKey)
            ),
            (
                typeof(ApiManyToManyRelationshipAttribute),
                nameof(ApiManyToManyRelationshipAttribute.ForeignKeyA)
            ),
            (
                typeof(ApiManyToManyRelationshipAttribute),
                nameof(ApiManyToManyRelationshipAttribute.ForeignKeyB)
            ),
            (
                typeof(ApiManyToManyRelationshipTypeAttribute),
                nameof(ApiManyToManyRelationshipTypeAttribute.ForeignKeyA)
            ),
            (
                typeof(ApiManyToManyRelationshipTypeAttribute),
                nameof(ApiManyToManyRelationshipTypeAttribute.ForeignKeyB)
            )
        };

        var nullabilityContext = new NullabilityInfoContext();

        foreach (var (annotationType, propertyName) in optionalProperties)
        {
            var property = annotationType.GetProperty(propertyName);

            property.Should().NotBeNull();
            nullabilityContext.Create(property!).ReadState.Should().Be(NullabilityState.Nullable);
        }
    }

    [Fact]
    public void ParameterlessApiKeyPreservesDefaults()
    {
        var attribute = new ApiKeyAttribute();

        attribute.ApiName.Should().Be("PrimaryKey");
        attribute.Order.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void RequiredAnnotationApiNamesRejectInvalidValues(string? apiName)
    {
        var actions = new Action[]
        {
            () => new ApiKeyAttribute { ApiName = apiName! },
            () => new ApiRelationshipAttribute { ApiName = apiName! },
            () => new ApiRelationshipTypeAttribute
            {
                ApiName = apiName!,
                PrincipalType = typeof(PersonAnnotated),
                DependentType = typeof(OrderStatusAnnotated)
            },
            () => new ApiManyToManyRelationshipAttribute
            {
                ApiName = apiName!,
                AssociationType = typeof(EmailValueAnnotated),
                OtherPrincipalType = typeof(PersonAnnotated)
            },
            () => new ApiManyToManyRelationshipTypeAttribute
            {
                ApiName = apiName!,
                PrincipalTypeA = typeof(PersonAnnotated),
                PrincipalTypeB = typeof(OrderStatusAnnotated),
                AssociationType = typeof(EmailValueAnnotated)
            }
        };

        foreach (var action in actions)
        {
            action.Should().Throw<ArgumentException>().WithParameterName("apiName");
        }
    }

    [Fact]
    public void RequiredAnnotationClrTypesRejectNullValues()
    {
        var actions = new Action[]
        {
            () => new ApiRelationshipTypeAttribute
            {
                ApiName = "Relationship",
                PrincipalType = null!,
                DependentType = typeof(OrderStatusAnnotated)
            },
            () => new ApiRelationshipTypeAttribute
            {
                ApiName = "Relationship",
                PrincipalType = typeof(PersonAnnotated),
                DependentType = null!
            },
            () => new ApiManyToManyRelationshipAttribute
            {
                ApiName = "Relationship",
                AssociationType = null!,
                OtherPrincipalType = typeof(PersonAnnotated)
            },
            () => new ApiManyToManyRelationshipAttribute
            {
                ApiName = "Relationship",
                AssociationType = typeof(EmailValueAnnotated),
                OtherPrincipalType = null!
            },
            () => new ApiManyToManyRelationshipTypeAttribute
            {
                ApiName = "Relationship",
                PrincipalTypeA = null!,
                PrincipalTypeB = typeof(OrderStatusAnnotated),
                AssociationType = typeof(EmailValueAnnotated)
            },
            () => new ApiManyToManyRelationshipTypeAttribute
            {
                ApiName = "Relationship",
                PrincipalTypeA = typeof(PersonAnnotated),
                PrincipalTypeB = null!,
                AssociationType = typeof(EmailValueAnnotated)
            },
            () => new ApiManyToManyRelationshipTypeAttribute
            {
                ApiName = "Relationship",
                PrincipalTypeA = typeof(PersonAnnotated),
                PrincipalTypeB = typeof(OrderStatusAnnotated),
                AssociationType = null!
            }
        };

        foreach (var action in actions)
        {
            action.Should().Throw<ArgumentNullException>();
        }
    }
    #endregion
}
