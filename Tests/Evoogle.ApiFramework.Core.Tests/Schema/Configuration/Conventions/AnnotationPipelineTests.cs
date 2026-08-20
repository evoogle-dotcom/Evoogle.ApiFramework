// // Copyright (c) 2024-2025 Evoogle.com
// // SPDX-License-Identifier: MIT
// //
// // This file is licensed under the MIT License.
// // See the LICENSE file in the project root for more information.
// using Evoogle.ApiFramework.Schema.Annotations;
// using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;
// using Evoogle.XUnit;

// using FluentAssertions;

// namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

// /// <summary>
// ///     Tests for the annotation pipeline: CLR attributes, annotation-reader precedence, and
// ///     interactions between annotations and conventions.
// /// </summary>
// public class AnnotationPipelineTests(ITestOutputHelper output) : XUnitTests(output)
// {
//     #region Test Domain Types — Type-Level Attributes
//     [ApiObjectType(ApiName = "RenamedPerson")]
//     private class PersonAnnotated
//     {
//         public Guid Id { get; set; }
//         public string Name { get; set; } = string.Empty;
//         public string? Email { get; set; }
//     }

//     [ApiScalarType(ApiName = "EmailValue")]
//     private record struct EmailValueAnnotated(string Value);

//     [ApiEnumType(ApiName = "OrderState")]
//     private enum OrderStatusAnnotated { Pending, Shipped }
//     #endregion

//     #region Test Domain Types — Property-Level Attributes
//     private class PersonWithPropertyAnnotations
//     {
//         public Guid Id { get; set; }

//         [ApiProperty(ApiName = "display_name", IsRequired = true)]
//         public string? Name { get; set; }

//         [ApiIgnore]
//         public string InternalTag { get; set; } = string.Empty;

//         [ApiProperty(IsOptional = true)]
//         public string NonNullableButOptional { get; set; } = string.Empty;
//     }
//     #endregion

//     #region Test Domain Types — ApiKey Attributes
//     private class PersonWithKeyAnnotation
//     {
//         [ApiKey]
//         public Guid Id { get; set; }
//         public string Name { get; set; } = string.Empty;
//     }

//     private class CompositeKeyType
//     {
//         [ApiKey(order: 0)]
//         public Guid OrderId { get; set; }

//         [ApiKey(order: 1)]
//         public long LineItemNumber { get; set; }

//         public string Description { get; set; } = string.Empty;
//     }

//     private class AnnotationPrimaryKeyType
//     {
//         public Guid Id { get; set; }

//         [ApiKey]
//         public string Code { get; set; } = string.Empty;
//     }
//     #endregion

//     #region Test Domain Types — Relationship Attributes
//     private class Customer
//     {
//         public Guid Id { get; set; }
//         public string Name { get; set; } = string.Empty;

//         [ApiRelationship("CustomerHasOrders",
//             Kind = ApiRelationshipKind.OneToMany,
//             ForeignKey = "CustomerId")]
//         public List<Order> Orders { get; set; } = [];
//     }

//     private class Order
//     {
//         public Guid Id { get; set; }
//         public Guid? CustomerId { get; set; }
//         public decimal Total { get; set; }
//     }

//     private class ConventionFilledCustomer
//     {
//         public Guid Id { get; set; }

//         [ApiRelationship(
//             "ConventionFilledCustomerOrders",
//             Kind = ApiRelationshipKind.OneToMany)]
//         public List<Order> Orders { get; set; } = [];
//     }

//     [ApiRelationshipType(
//         "InvoiceForOrder",
//         principalType: typeof(Order),
//         dependentType: typeof(Invoice),
//         Kind = ApiRelationshipKind.OneToOne,
//         ForeignKey = "OrderId")]
//     private class Invoice
//     {
//         public Guid Id { get; set; }
//         public Guid? OrderId { get; set; }
//         public decimal Amount { get; set; }
//     }
//     #endregion

//     #region Test Conventions
//     private sealed class CustomerOrdersRelationshipConvention : IApiRelationshipConvention
//     {
//         #region IApiConvention
//         public ApiConventionPhase Phase => ApiConventionPhase.Relationship;
//         #endregion

//         #region IApiRelationshipConvention
//         public void Apply(ApiSchemaBuilder builder)
//         {
//             builder.AddOneToManyRelationship
//             (
//                 "CustomerHasOrders",
//                 relationship => relationship
//                     .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
//                     .From<Customer>()
//                     .To<Order>(dependent => dependent
//                         .WithForeignKey(key => key.AddPath(order => order.Id)))
//             );
//         }
//         #endregion
//     }

//     private sealed class ConventionFilledCustomerRelationshipConvention
//         : IApiRelationshipConvention
//     {
//         #region IApiConvention
//         public ApiConventionPhase Phase => ApiConventionPhase.Relationship;
//         #endregion

//         #region IApiRelationshipConvention
//         public void Apply(ApiSchemaBuilder builder)
//         {
//             builder.AddOneToManyRelationship
//             (
//                 "ConventionFilledCustomerOrders",
//                 relationship => relationship
//                     .From<ConventionFilledCustomer>()
//                     .To<Order>(dependent => dependent
//                         .WithForeignKey(key => key.AddPath(order => order.CustomerId)))
//             );
//         }
//         #endregion
//     }

//     private sealed class ConflictingCustomerOrdersRelationshipConvention
//         : IApiRelationshipConvention
//     {
//         #region IApiConvention
//         public ApiConventionPhase Phase => ApiConventionPhase.Relationship;
//         #endregion

//         #region IApiRelationshipConvention
//         public void Apply(ApiSchemaBuilder builder)
//         {
//             builder.AddOneToOneRelationship
//             (
//                 "CustomerHasOrders",
//                 relationship => relationship
//                     .From<Customer>()
//                     .To<Order>()
//             );
//         }
//         #endregion
//     }

//     private sealed class CustomerOrdersObjectTypeConvention : IApiObjectTypeConvention
//     {
//         #region IApiConvention
//         public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
//         #endregion

//         #region IApiObjectTypeConvention
//         public void Apply(ApiObjectTypeBuilder builder)
//         {
//             if (builder.ClrType != typeof(Customer))
//             {
//                 return;
//             }

//             builder.AddOneToManyRelationship
//             (
//                 "CustomerHasOrders",
//                 relationship => relationship
//                     .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
//                     .From<Customer>()
//                     .To<Order>(dependent => dependent
//                         .WithForeignKey(key => key.AddPath(order => order.Id)))
//             );
//         }
//         #endregion
//     }
//     #endregion

//     #region Type-Level Attribute Tests
//     [Fact]
//     public void ApiObjectTypeAttributeOverridesApiName()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .AddObject<PersonAnnotated>()
//             .Build();

//         schema.TryGetObjectTypeByClrType(typeof(PersonAnnotated), out var objectType).Should().BeTrue();
//         objectType!.ApiName.Should().Be("RenamedPerson");
//     }

//     [Fact]
//     public void ApiScalarTypeAttributeOverridesApiName()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .AddScalar<EmailValueAnnotated>()
//             .Build();

//         schema.TryGetScalarTypeByClrType(typeof(EmailValueAnnotated), out var scalarType).Should().BeTrue();
//         scalarType!.ApiName.Should().Be("EmailValue");
//     }

//     [Fact]
//     public void ApiEnumTypeAttributeOverridesApiName()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .AddEnum<OrderStatusAnnotated>(x => x
//                 .AddValue("Pending", "Pending", 0)
//                 .AddValue("Shipped", "Shipped", 1))
//             .Build();

//         schema.TryGetEnumTypeByClrType(typeof(OrderStatusAnnotated), out var enumType).Should().BeTrue();
//         enumType!.ApiName.Should().Be("OrderState");
//     }
//     #endregion

//     #region Property-Level Attribute Tests
//     [Fact]
//     public void ApiPropertyAnnotationsOverrideConventionAppliedValues()
//     {
//         // Conventions discover and name properties via camelCase; annotations then override.
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultConventions()
//             .UseDefaultAnnotations()
//             .AddObject<PersonWithPropertyAnnotations>()
//             .Build();

//         schema.TryGetObjectTypeByClrType(typeof(PersonWithPropertyAnnotations), out var objectType).Should().BeTrue();

//         // Annotation renames "name" (convention camelCase) to "display_name" and forces Required.
//         var nameProperty = objectType!.ApiProperties.Single(p => p.ClrName == "Name");
//         nameProperty.ApiName.Should().Be("display_name");
//         nameProperty.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required).Should().BeTrue();

//         // Annotation forces Optional on a non-nullable CLR property.
//         var optionalProperty = objectType.ApiProperties.Single(p => p.ClrName == "NonNullableButOptional");
//         optionalProperty.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required).Should().BeFalse();
//     }

//     [Fact]
//     public void ApiIgnoreAttributeExcludesPropertyFromConventionDiscovery()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UsePropertyDiscovery()
//             .UseDefaultAnnotations()
//             .AddObject<PersonWithPropertyAnnotations>()
//             .Build();

//         schema.TryGetObjectTypeByClrType(typeof(PersonWithPropertyAnnotations), out var objectType).Should().BeTrue();
//         objectType!.ApiProperties.Any(p => p.ClrName == "InternalTag").Should().BeFalse();
//     }

//     [Fact]
//     public void ApiKeyAttributeCreatesKeyTypeOnOwningObjectType()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UsePropertyDiscovery()
//             .UseDefaultAnnotations()
//             .AddObject<PersonWithKeyAnnotation>()
//             .Build();

//         schema.TryGetObjectTypeByClrType(typeof(PersonWithKeyAnnotation), out var objectType).Should().BeTrue();
//         objectType!.HasKeyTypes.Should().BeTrue();
//         objectType.ApiKeyTypes.Should().ContainSingle(k => k.ApiName == "PrimaryKey");
//     }

//     [Fact]
//     public void MultipleApiKeyAttributesWithOrderCreateCompositeKey()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UsePropertyDiscovery()
//             .UseDefaultAnnotations()
//             .AddObject<CompositeKeyType>()
//             .Build();

//         schema.TryGetObjectTypeByClrType(typeof(CompositeKeyType), out var objectType).Should().BeTrue();
//         objectType!.HasKeyTypes.Should().BeTrue();

//         var primaryKey = objectType.ApiKeyTypes.SingleOrDefault(k => k.ApiName == "PrimaryKey");
//         primaryKey.Should().NotBeNull();
//         primaryKey!.ApiKeyPaths.Should().HaveCount(2);
//     }

//     [Fact]
//     public void ApiKeyAnnotationRunsBeforePrimaryKeyInference()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultConventions()
//             .UseDefaultAnnotations()
//             .AddObject<AnnotationPrimaryKeyType>()
//             .Build();

//         schema.TryGetObjectTypeByClrType(typeof(AnnotationPrimaryKeyType), out var objectType)
//             .Should().BeTrue();

//         var primaryKey = objectType!.ApiKeyTypes
//             .Should().ContainSingle(key => key.ApiName == "PrimaryKey").Which;
//         var keyPath = primaryKey.ApiKeyPaths.Should().ContainSingle().Which;

//         keyPath.ApiSegments.Should().ContainSingle()
//             .Which.ClrPropertyName.Should().Be(nameof(AnnotationPrimaryKeyType.Code));
//     }
//     #endregion

//     #region Relationship Attribute Tests
//     [Fact]
//     public void ApiRelationshipAttributeOnNavigationPropertyRegistersRelationship()
//     {
//         // Customer has a nav property Orders decorated with [ApiRelationship].
//         // Principal type (Customer) needs a PrimaryKey for relationship initialization to succeed.
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .AddObject<Customer>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("name", "Name")
//                 .AddProperty("orders", "Orders")
//                 .AddKey("PrimaryKey", b => b.AddPath(typeof(Customer), "Id")))
//             .AddObject<Order>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("customerId", "CustomerId"))
//             .Build();

//         schema.TryGetRelationshipByApiName("CustomerHasOrders", out var relationship).Should().BeTrue();
//         relationship.Should().BeOfType<ApiRelationshipOneToMany>();
//     }

//     [Fact]
//     public void ApiRelationshipTypeAttributeAtTypeLevelRegistersRelationship()
//     {
//         // Invoice has [ApiRelationshipType] at the class level.
//         // Principal type (Order) needs a PrimaryKey for relationship initialization.
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .AddObject<Order>(x => x
//                 .AddProperty("id", "Id")
//                 .AddKey("PrimaryKey", b => b.AddPath(typeof(Order), "Id")))
//             .AddObject<Invoice>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("orderId", "OrderId"))
//             .Build();

//         schema.TryGetRelationshipByApiName("InvoiceForOrder", out var relationship).Should().BeTrue();
//         relationship.Should().BeOfType<ApiRelationshipOneToOne>();
//     }

//     [Fact]
//     public void RelationshipAnnotationOverridesRelationshipConvention()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .UseConventions(c => c.AddConvention(new CustomerOrdersRelationshipConvention()))
//             .AddObject<Customer>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("orders", "Orders")
//                 .AddKey("PrimaryKey", key => key.AddPath(typeof(Customer), "Id")))
//             .AddObject<Order>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("customerId", "CustomerId"))
//             .Build();

//         schema.TryGetRelationshipByApiName("CustomerHasOrders", out var relationship)
//             .Should().BeTrue();

//         var oneToMany = relationship.Should().BeOfType<ApiRelationshipOneToMany>().Which;
//         oneToMany.ApiDeleteBehavior.Should().Be(ApiRelationshipDeleteBehavior.None);
//         oneToMany.ApiDependentEnd.ApiForeignKeyType.ApiKeyPaths
//             .Should().ContainSingle().Which.ApiSegments
//             .Should().ContainSingle().Which.ClrPropertyName
//             .Should().Be(nameof(Order.CustomerId));
//     }

//     [Fact]
//     public void ExplicitRelationshipConfigurationOverridesRelationshipAnnotation()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .AddObject<Customer>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("orders", "Orders")
//                 .AddKey("PrimaryKey", key => key.AddPath(typeof(Customer), "Id")))
//             .AddObject<Order>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("customerId", "CustomerId"))
//             .AddOneToManyRelationship
//             (
//                 "CustomerHasOrders",
//                 relationship => relationship
//                     .WithDeleteBehavior(ApiRelationshipDeleteBehavior.Delete)
//                     .From<Customer>()
//                     .To<Order>(dependent => dependent
//                         .WithForeignKey(key => key.AddPath(order => order.Id)))
//             )
//             .Build();

//         schema.TryGetRelationshipByApiName("CustomerHasOrders", out var relationship)
//             .Should().BeTrue();

//         var oneToMany = relationship.Should().BeOfType<ApiRelationshipOneToMany>().Which;
//         oneToMany.ApiDeleteBehavior.Should().Be(ApiRelationshipDeleteBehavior.Delete);
//         oneToMany.ApiDependentEnd.ApiForeignKeyType.ApiKeyPaths
//             .Should().ContainSingle().Which.ApiSegments
//             .Should().ContainSingle().Which.ClrPropertyName
//             .Should().Be(nameof(Order.Id));
//     }

//     [Fact]
//     public void RelationshipConventionFillsFacetNotConfiguredByAnnotation()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .UseConventions(c => c.AddConvention
//             (
//                 new ConventionFilledCustomerRelationshipConvention()
//             ))
//             .AddObject<ConventionFilledCustomer>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("orders", "Orders")
//                 .AddKey
//                 (
//                     "PrimaryKey",
//                     key => key.AddPath(typeof(ConventionFilledCustomer), "Id")
//                 ))
//             .AddObject<Order>(x => x
//                 .AddProperty("customerId", "CustomerId"))
//             .Build();

//         schema.TryGetRelationshipByApiName
//         (
//             "ConventionFilledCustomerOrders",
//             out var relationship
//         ).Should().BeTrue();

//         var oneToMany = relationship.Should().BeOfType<ApiRelationshipOneToMany>().Which;
//         oneToMany.ApiDependentEnd.ApiForeignKeyType.ApiKeyPaths
//             .Should().ContainSingle().Which.ApiSegments
//             .Should().ContainSingle().Which.ClrPropertyName
//             .Should().Be(nameof(Order.CustomerId));
//     }

//     [Fact]
//     public void RelationshipAnnotationsRunBeforeRelationshipConventions()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .UseConventions(c => c.AddConvention
//             (
//                 new ConflictingCustomerOrdersRelationshipConvention()
//             ))
//             .AddObject<Customer>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("orders", "Orders")
//                 .AddKey("PrimaryKey", key => key.AddPath(typeof(Customer), "Id")))
//             .AddObject<Order>(x => x
//                 .AddProperty("customerId", "CustomerId"))
//             .Build();

//         schema.TryGetRelationshipByApiName("CustomerHasOrders", out var relationship)
//             .Should().BeTrue();
//         relationship.Should().BeOfType<ApiRelationshipOneToMany>();
//     }

//     [Fact]
//     public void RelationshipCreatedByObjectConventionUsesConventionPrecedence()
//     {
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultAnnotations()
//             .UseConventions(c => c.AddConvention(new CustomerOrdersObjectTypeConvention()))
//             .AddObject<Customer>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("orders", "Orders")
//                 .AddKey("PrimaryKey", key => key.AddPath(typeof(Customer), "Id")))
//             .AddObject<Order>(x => x
//                 .AddProperty("id", "Id")
//                 .AddProperty("customerId", "CustomerId"))
//             .Build();

//         schema.TryGetRelationshipByApiName("CustomerHasOrders", out var relationship)
//             .Should().BeTrue();

//         var oneToMany = relationship.Should().BeOfType<ApiRelationshipOneToMany>().Which;
//         oneToMany.ApiDeleteBehavior.Should().Be(ApiRelationshipDeleteBehavior.None);
//         oneToMany.ApiDependentEnd.ApiForeignKeyType.ApiKeyPaths
//             .Should().ContainSingle().Which.ApiSegments
//             .Should().ContainSingle().Which.ClrPropertyName
//             .Should().Be(nameof(Order.CustomerId));
//     }
//     #endregion

//     #region Precedence Tests
//     [Fact]
//     public void DataAnnotationOverridesConvention()
//     {
//         // The annotation sets "RenamedPerson" before the lower-precedence naming convention runs.
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultConventions()
//             .UseDefaultAnnotations()
//             .AddObject<PersonAnnotated>()
//             .Build();

//         schema.TryGetObjectTypeByClrType(typeof(PersonAnnotated), out var objectType).Should().BeTrue();
//         objectType!.ApiName.Should().Be("RenamedPerson");
//     }

//     [Fact]
//     public void ExplicitConfigurationOverridesDataAnnotation()
//     {
//         // [ApiObjectType(ApiName="RenamedPerson")] annotation is beaten by explicit WithName.
//         var schema = new ApiSchemaBuilder()
//             .WithName("Test")
//             .WithTestScalars()
//             .UseDefaultConventions()
//             .UseDefaultAnnotations()
//             .AddObject<PersonAnnotated>(x => x.WithName("ActualPerson"))
//             .Build();

//         schema.TryGetObjectTypeByClrType(typeof(PersonAnnotated), out var objectType).Should().BeTrue();
//         objectType!.ApiName.Should().Be("ActualPerson");
//     }
//     #endregion
// }
