// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Annotations;

public partial class ApiAnnotationTests
{
    #region Test Methods
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void RequiredAnnotationApiNamesRejectInvalidValues(string? apiName)
    {
        Action key = () => new ApiKeyAttribute(apiName!);
        Action relationship = () => new ApiRelationshipAttribute(apiName!);
        Action relationshipType = () => new ApiRelationshipTypeAttribute
        (
            apiName!,
            typeof(PersonAnnotated),
            typeof(OrderStatusAnnotated)
        );
        Action manyToManyRelationship = () => new ApiManyToManyRelationshipAttribute
        (
            apiName!,
            typeof(EmailValueAnnotated),
            typeof(PersonAnnotated)
        );
        Action manyToManyRelationshipType = () => new ApiManyToManyRelationshipTypeAttribute
        (
            apiName!,
            typeof(PersonAnnotated),
            typeof(OrderStatusAnnotated),
            typeof(EmailValueAnnotated)
        );

        key.Should().Throw<ArgumentException>().WithParameterName("apiName");
        relationship.Should().Throw<ArgumentException>().WithParameterName("apiName");
        relationshipType.Should().Throw<ArgumentException>().WithParameterName("apiName");
        manyToManyRelationship.Should().Throw<ArgumentException>().WithParameterName("apiName");
        manyToManyRelationshipType.Should().Throw<ArgumentException>().WithParameterName("apiName");
    }

    [Fact]
    public void RequiredNamedAnnotationsExposeReadOnlyApiNames()
    {
        var attributeTypes = new[]
        {
            typeof(ApiKeyAttribute),
            typeof(ApiRelationshipAttribute),
            typeof(ApiRelationshipTypeAttribute),
            typeof(ApiManyToManyRelationshipAttribute),
            typeof(ApiManyToManyRelationshipTypeAttribute)
        };

        foreach (var attributeType in attributeTypes)
        {
            var apiNameProperty = attributeType.GetProperty
            (
                nameof(ApiNamedElementAttribute.ApiName)
            );

            apiNameProperty.Should().NotBeNull();
            apiNameProperty!.PropertyType.Should().Be(typeof(string));
            apiNameProperty.CanWrite.Should().BeFalse();
        }
    }
    #endregion
}
