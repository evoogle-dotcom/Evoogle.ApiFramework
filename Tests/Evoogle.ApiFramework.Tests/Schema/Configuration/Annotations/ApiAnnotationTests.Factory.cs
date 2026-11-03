// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using static Evoogle.ApiFramework.Schema.Configuration.Annotations.ApiAnnotationTests;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

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
            .AddObject<PersonAnnotated>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.Email))
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
            .AddEnum<OrderStatusAnnotated>(static x => x
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
            .AddEnum<OrderStatusValueAnnotated>(static x => x
                .AddValue("explicit_pending", "Pending", 0)
                .AddValue("Shipped", "Shipped", 1))
            .Build();
    }
    #endregion

    #region Property and Field Attribute Factory Methods
    public static ApiSchema BuildWithApiPropertyAttributesConfigureNameAndNullability()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithPropertyAnnotations>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name)
                .AddProperty(static p => p.NonNullableButOptional)
                .AddProperty(static p => p.RequiredWins))
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
            .AddObject<PersonWithKeyAnnotation>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreatePrimaryAndAlternateScalarKeys()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<ScalarKeyAnnotationObject>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name))
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
            .AddObject<CompositeKeyObject>(static x => x
                .AddProperty(static p => p.OrderId)
                .AddProperty(static p => p.LineItemNumber)
                .AddProperty(static p => p.Description))
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
            .AddObject<ThreePartCompositeKeyObject>(static x => x
                .AddProperty(static p => p.Id1)
                .AddProperty(static p => p.Id2)
                .AddProperty(static p => p.Id3)
                .AddProperty(static p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributeCreatesNestedTypePrimaryKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<NestedKeyPartAnnotation>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributeCreatesOwnerTypePrimaryKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<OwnerKeyAnnotation>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Description))
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
            .AddObject<AnnotationPrimaryKeyObject>()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreateNestedCompositeKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<NestedKeyPartAnnotation>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Description))
            .AddObject<NestedCompositeKeyAnnotation>(static x => x
                .AddProperty(static p => p.NestedPart)
                .AddProperty(static p => p.Name))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreateOwnedCompositeKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<OwnerKeyAnnotation>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Description))
            .AddObject<OwnedCompositeKeyAnnotation>(static x => x
                .AddProperty(static p => p.LineNumber)
                .AddProperty(static p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributeCreatesOwnedDependentKey()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<OwnerKeyAnnotation>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Description))
            .AddObject<OwnedDependentKeyAnnotation>(static x => x
                .AddProperty(static p => p.Description))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiKeyAttributesCreateDuplicatePath()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<DuplicatePathKeyAnnotation>(static x => x.AddProperty(static p => p.Id))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithMissingTypeLevelKeyPathAnnotation()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<MissingTypeLevelKeyPathAnnotation>(static x => x.AddProperty(static p => p.Id))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithMalformedTypeLevelKeyPathAnnotation()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<MalformedTypeLevelKeyPathAnnotation>(static x => x.AddProperty(static p => p.Id))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithUnresolvedTypeLevelKeyPathAnnotation()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<UnresolvedTypeLevelKeyPathAnnotation>(static x => x.AddProperty(static p => p.Id))
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
                ApiName = apiName!,
                Kind = ApiRelationshipKind.OneToMany
            },
            nameof(ApiRelationshipDefinitionAttribute) => new ApiRelationshipDefinitionAttribute
            {
                ApiName = apiName!,
                PrincipalType = typeof(PersonAnnotated),
                Kind = ApiRelationshipKind.OneToMany,
                DependentType = typeof(OrderStatusAnnotated)
            },
            nameof(ApiManyToManyRelationshipAttribute) => new ApiManyToManyRelationshipAttribute
            {
                ApiName = apiName!,
                AssociationType = typeof(EmailValueAnnotated),
                OtherPrincipalType = typeof(PersonAnnotated)
            },
            nameof(ApiManyToManyRelationshipDefinitionAttribute) =>
                new ApiManyToManyRelationshipDefinitionAttribute
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
            .UseAnnotations(static x => x.AddReader(new BuildObservableEnumValueAnnotationReader()))
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
            .AddObject<Customer>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.Name))
            .AddObject<Order>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.CustomerId)
                .AddProperty(static p => p.Total))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiRelationshipDefinitionAttributeAtTypeLevel()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<decimal>()
            .AddObject<Order>(static x => x
                .AddProperty(static p => p.Id))
            .AddObject<Invoice>(static x => x
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.OrderId)
                .AddProperty(static p => p.Amount))
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
            .AddObject<Product>(static x => x
                .AddProperty(static p => p.Id))
            .AddObject<Tag>(static x => x
                .AddProperty(static p => p.Id))
            .AddObject<ProductTag>(static x => x
                .AddProperty(static p => p.ProductId)
                .AddProperty(static p => p.TagId))
            .UseDefaultAnnotations()
            .Build();
    }

    public static ApiSchema BuildWithApiManyToManyRelationshipDefinitionAttributeAtTypeLevel()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddObject<Category>(static x => x
                .AddProperty(static p => p.Id))
            .AddObject<Label>(static x => x
                .AddProperty(static p => p.Id))
            .AddObject<ProductTagFromType>(static x => x
                .AddProperty(static p => p.ProductId)
                .AddProperty(static p => p.TagId))
            .UseDefaultAnnotations()
            .Build();
    }
    #endregion
}
