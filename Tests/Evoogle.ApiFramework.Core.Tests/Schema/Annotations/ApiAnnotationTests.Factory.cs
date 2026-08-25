// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using Evoogle.ApiFramework.Schema.Configuration;

using static Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests;

namespace Evoogle.ApiFramework.Schema.Annotations;

[DynamicLinqType]
public static class ApiAnnotationTestsFactory
{
    #region Type-Level Attribute Factory Methods
    public static ApiSchema BuildWithApiObjectAttributeOverridesApiName()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonAnnotated>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email))
            .UseDefaultAnnotations()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithApiScalarAttributeOverridesApiName()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<EmailValueAnnotated>()
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiEnumAttributeOverridesApiName()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<OrderStatusAnnotated>(x => x
                .AddValue("Pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiEnumValueAttributeOverridesApiName()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .UseDefaultConventions()
            .UseDefaultAnnotations()
            .AddEnum<OrderStatusValueAnnotated>()
            .Build();
    }

    public static ApiSchema BuildWithExplicitEnumValueNameOverridesApiEnumValueAttribute()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .UseDefaultAnnotations()
            .AddEnum<OrderStatusValueAnnotated>(x => x
                .AddValue("explicit_pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1))
            .Build();
    }
    #endregion

    #region Property and Field Attribute Factory Methods
    public static ApiSchema BuildWithApiPropertyAttributesConfigureNameAndModifiers()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithPropertyAnnotations>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.NonNullableButOptional)
                .AddProperty(p => p.RequiredWins))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiFieldAttributesConfigureNameKeyAndIgnore()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .UsePropertyDiscovery()
            .UseDefaultAnnotations()
            .AddObject<FieldAnnotationsType>()
            .Build();
    }
    #endregion

    #region Key Attribute Factory Methods
    public static ApiSchema BuildWithApiKeyAttributeCreatesPrimaryKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithKeyAnnotation>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreatePrimaryAndAlternateScalarKeys()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<ScalarKeyTypeAnnotation>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreateOrderedNamedCompositeKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<long>()
            .AddScalar<string>()
            .AddObject<CompositeKeyType>(x => x
                .AddProperty(p => p.OrderId)
                .AddProperty(p => p.LineItemNumber)
                .AddProperty(p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreateThreePartScalarCompositeKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<ThreePartCompositeKeyType>(x => x
                .AddProperty(p => p.Id1)
                .AddProperty(p => p.Id2)
                .AddProperty(p => p.Id3)
                .AddProperty(p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributeCreatesNestedTypePrimaryKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<NestedKeyPartAnnotation>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributeCreatesOwnerTypePrimaryKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<OwnerKeyAnnotation>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributeRunsBeforePrimaryKeyInference()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .UseDefaultConventions()
            .UseDefaultAnnotations()
            .AddObject<AnnotationPrimaryKeyType>()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreateNestedCompositeKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<NestedKeyPartAnnotation>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description))
            .AddObject<NestedCompositeKeyAnnotation>(x => x
                .AddProperty(p => p.NestedPart)
                .AddProperty(p => p.Name))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreateOwnedCompositeKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<OwnerKeyAnnotation>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description))
            .AddObject<OwnedCompositeKeyAnnotation>(x => x
                .AddProperty(p => p.LineNumber)
                .AddProperty(p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributeCreatesOwnedDependentKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<OwnerKeyAnnotation>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Description))
            .AddObject<OwnedDependentKeyAnnotation>(x => x
                .AddProperty(p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreateDuplicatePath()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<DuplicatePathKeyAnnotation>(x => x.AddProperty(p => p.Id))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithMissingTypeLevelKeyPathAnnotation()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<MissingTypeLevelKeyPathAnnotation>(x => x.AddProperty(p => p.Id))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithMalformedTypeLevelKeyPathAnnotation()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<MalformedTypeLevelKeyPathAnnotation>(x => x.AddProperty(p => p.Id))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithUnresolvedTypeLevelKeyPathAnnotation()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<UnresolvedTypeLevelKeyPathAnnotation>(x => x.AddProperty(p => p.Id))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithInvalidRequiredAnnotationApiName
    (
        string annotationType,
        string? apiName
    )
    {
        Attribute attribute = annotationType switch
        {
            nameof(ApiKeyAttribute) => new ApiKeyAttribute { ApiName = apiName! },
            nameof(ApiRelationshipAttribute) => new ApiRelationshipAttribute
            {
                ApiName = apiName!
            },
            nameof(ApiRelationshipTypeAttribute) => new ApiRelationshipTypeAttribute
            {
                ApiName = apiName!,
                PrincipalType = typeof(PersonAnnotated),
                DependentType = typeof(OrderStatusAnnotated)
            },
            nameof(ApiManyToManyRelationshipAttribute) => new ApiManyToManyRelationshipAttribute
            {
                ApiName = apiName!,
                AssociationType = typeof(EmailValueAnnotated),
                OtherPrincipalType = typeof(PersonAnnotated)
            },
            nameof(ApiManyToManyRelationshipTypeAttribute) =>
                new ApiManyToManyRelationshipTypeAttribute
            {
                ApiName = apiName!,
                PrincipalTypeA = typeof(PersonAnnotated),
                PrincipalTypeB = typeof(OrderStatusAnnotated),
                AssociationType = typeof(EmailValueAnnotated)
            },
            _ => throw new ArgumentException("Unknown annotation type.", nameof(annotationType))
        };
        _ = attribute;

        return null!;
    }

    public static ApiSchema BuildWithCustomEnumValueAnnotationReader()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .UseDefaultConventions()
            .UseAnnotations(x => x.AddReader(new BuildObservableEnumValueAnnotationReader()))
            .AddEnum<OrderStatusValueAnnotated>()
            .Build();
    }
    #endregion

    #region Relationship Attribute Factory Methods
    public static ApiSchema BuildWithApiRelationshipAttributeOnNavigationProperty()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddScalar<decimal>()
            .AddObject<Customer>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name))
            .AddObject<Order>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.CustomerId)
                .AddProperty(p => p.Total))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiRelationshipTypeAttributeAtTypeLevel()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<decimal>()
            .AddObject<Order>(x => x
                .AddProperty(p => p.Id))
            .AddObject<Invoice>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.OrderId)
                .AddProperty(p => p.Amount))
            .UseDefaultAnnotations()
            .Build();
    }
    #endregion

    #region Many-to-Many Attribute Factory Methods
    public static ApiSchema BuildWithApiManyToManyRelationshipAttributeOnNavigationProperty()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddObject<Product>(x => x
                .AddProperty(p => p.Id))
            .AddObject<Tag>(x => x
                .AddProperty(p => p.Id))
            .AddObject<ProductTag>(x => x
                .AddProperty(p => p.ProductId)
                .AddProperty(p => p.TagId))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiManyToManyRelationshipTypeAttributeAtTypeLevel()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddObject<Category>(x => x
                .AddProperty(p => p.Id))
            .AddObject<Label>(x => x
                .AddProperty(p => p.Id))
            .AddObject<ProductTagFromType>(x => x
                .AddProperty(p => p.ProductId)
                .AddProperty(p => p.TagId))
            .UseDefaultAnnotations()
            .Build();
    }
    #endregion
}
