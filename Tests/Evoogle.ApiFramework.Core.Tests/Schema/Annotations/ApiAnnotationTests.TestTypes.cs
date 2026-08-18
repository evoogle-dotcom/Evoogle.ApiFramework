// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

public partial class ApiAnnotationTests
{
    #region Test Domain Types — Type-Level Attributes
    [ApiObjectType(Name = "RenamedPerson")]
    public class PersonAnnotated
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    [ApiScalarType(Name = "EmailValue")]
    public record struct EmailValueAnnotated(string Value);

    [ApiEnumType(Name = "OrderState")]
    public enum OrderStatusAnnotated { Pending, Shipped }
    #endregion

    #region Test Domain Types — Property-Level Attributes
    public class PersonWithPropertyAnnotations
    {
        public Guid Id { get; set; }

        [ApiProperty(Name = "display_name", IsRequired = true)]
        public string? Name { get; set; }

        [ApiIgnore]
        public string InternalTag { get; set; } = string.Empty;

        [ApiProperty(IsOptional = true)]
        public string NonNullableButOptional { get; set; } = string.Empty;

        [ApiProperty(IsRequired = true, IsOptional = true)]
        public string? RequiredWins { get; set; }
    }
    #endregion

    #region Test Domain Types — ApiKey Attributes
    public class PersonWithKeyAnnotation
    {
        [ApiKey]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ScalarKeyTypeAnnotation
    {
        [ApiKey]
        public int Id { get; set; }

        [ApiKey("AlternateKey")]
        public string Name { get; set; } = string.Empty;
    }

    public class CompositeKeyType
    {
        [ApiKey("OrderLineKey", order: 0)]
        public Guid OrderId { get; set; }

        [ApiKey("OrderLineKey", order: 1)]
        public long LineItemNumber { get; set; }

        public string Description { get; set; } = string.Empty;
    }

    public class ThreePartCompositeKeyType
    {
        [ApiKey("ThreePartKey", order: 0)]
        public int Id1 { get; set; }

        [ApiKey("ThreePartKey", order: 1)]
        public string? Id2 { get; set; }

        [ApiKey("ThreePartKey", order: 2)]
        public Guid Id3 { get; set; }

        public string? Description { get; set; }
    }

    public class NestedKeyPartAnnotation
    {
        [ApiKey("NestedPartKey")]
        public int Id { get; set; }

        public string? Description { get; set; }
    }

    public class OwnerKeyAnnotation
    {
        [ApiKey("OwnerKey")]
        public int Id { get; set; }

        public string? Description { get; set; }
    }

    public class AnnotationPrimaryKeyType
    {
        public Guid Id { get; set; }

        [ApiKey]
        public string Code { get; set; } = string.Empty;
    }
    #endregion

    #region Test Domain Types — Relationship Attributes
    public class Customer
    {
        [ApiKey]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [ApiRelationship
        (
            "CustomerHasOrders",
            Kind = ApiRelationshipKind.OneToMany,
            ForeignKey = "CustomerId",
            DeleteBehavior = ApiRelationshipDeleteBehavior.Delete
        )]
        public List<Order> Orders { get; set; } = [];
    }

    public class Order
    {
        [ApiKey]
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal Total { get; set; }
    }

    public class ConventionFilledCustomer
    {
        public Guid Id { get; set; }

        [ApiRelationship("ConventionFilledCustomerOrders",Kind = ApiRelationshipKind.OneToMany)]
        public List<Order> Orders { get; set; } = [];
    }

    [ApiRelationshipType("InvoiceForOrder", principalType: typeof(Order), dependentType: typeof(Invoice), Kind = ApiRelationshipKind.OneToOne, ForeignKey = "OrderId")]
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid? OrderId { get; set; }
        public decimal Amount { get; set; }
    }
    #endregion

    #region Test Domain Types — Field and Many-to-Many Attributes
    public class FieldAnnotationsType
    {
        [ApiProperty(Name = "field_code", IsRequired = true)]
        [ApiKey("FieldKey")]
        public Guid Code;

        [ApiIgnore]
        public string InternalValue = string.Empty;
    }

    public class Product
    {
        [ApiKey]
        public Guid Id { get; set; }

        [ApiManyToManyRelationship
        (
            "ProductHasTags",
            associationType: typeof(ProductTag),
            otherPrincipalType: typeof(Tag),
            ForeignKeyA = "ProductId",
            ForeignKeyB = "TagId"
        )]
        public List<Tag> Tags { get; set; } = [];
    }

    public class Tag
    {
        [ApiKey]
        public Guid Id { get; set; }
    }

    public class ProductTag
    {
        public Guid ProductId { get; set; }
        public Guid TagId { get; set; }
    }

    [ApiManyToManyRelationshipType
    (
        "ProductHasTagsFromType",
        principalTypeA: typeof(Category),
        principalTypeB: typeof(Label),
        associationType: typeof(ProductTagFromType),
        ForeignKeyA = "ProductId",
        ForeignKeyB = "TagId"
    )]
    public class ProductTagFromType
    {
        public Guid ProductId { get; set; }
        public Guid TagId { get; set; }
    }

    public class Category
    {
        [ApiKey]
        public Guid Id { get; set; }
    }

    public class Label
    {
        [ApiKey]
        public Guid Id { get; set; }
    }
    #endregion
}
