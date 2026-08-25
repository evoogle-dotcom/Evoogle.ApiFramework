// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

public partial class ApiAnnotationTests
{
    #region Test Domain Types — Type-Level Attributes
    [ApiObject(ApiName = "RenamedPerson")]
    public class PersonAnnotated
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    [ApiScalar(ApiName = "EmailValue")]
    public record struct EmailValueAnnotated(string Value);

    [ApiEnum(ApiName = "OrderState")]
    public enum OrderStatusAnnotated { Pending, Shipped }

    public enum OrderStatusValueAnnotated
    {
        [ApiEnumValue(ApiName = "awaiting_payment")]
        Pending,

        [ApiEnumValue]
        Shipped,
    }
    #endregion

    #region Test Domain Types — Property-Level Attributes
    public class PersonWithPropertyAnnotations
    {
        public Guid Id { get; set; }

        [ApiProperty(ApiName = "display_name", IsRequired = true)]
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

        [ApiKey(ApiName = "AlternateKey")]
        public string Name { get; set; } = string.Empty;
    }

    public class CompositeKeyType
    {
        [ApiKey(ApiName = "OrderLineKey", Order = 0)]
        public Guid OrderId { get; set; }

        [ApiKey(ApiName = "OrderLineKey", Order = 1)]
        public long LineItemNumber { get; set; }

        public string Description { get; set; } = string.Empty;
    }

    public class ThreePartCompositeKeyType
    {
        [ApiKey(ApiName = "ThreePartKey", Order = 0)]
        public int Id1 { get; set; }

        [ApiKey(ApiName = "ThreePartKey", Order = 1)]
        public string? Id2 { get; set; }

        [ApiKey(ApiName = "ThreePartKey", Order = 2)]
        public Guid Id3 { get; set; }

        public string? Description { get; set; }
    }

    public class NestedKeyPartAnnotation
    {
        [ApiKey(ApiName = "NestedPartKey")]
        public int Id { get; set; }

        public string? Description { get; set; }
    }

    public class OwnerKeyAnnotation
    {
        [ApiKey(ApiName = "OwnerKey")]
        public int Id { get; set; }

        public string? Description { get; set; }
    }

    public class AnnotationPrimaryKeyType
    {
        public Guid Id { get; set; }

        [ApiKey]
        public string Code { get; set; } = string.Empty;
    }

    [ApiKey(ApiName = "NestedCompositeKey", Order = 0, ClrPath = nameof(NestedPart) + "." + nameof(NestedKeyPartAnnotation.Id))]
    [ApiKey(ApiName = "NestedCompositeKey", Order = 1, ClrPath = nameof(Name))]
    public class NestedCompositeKeyAnnotation
    {
        public NestedKeyPartAnnotation NestedPart { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
    }

    [ApiKey(ApiName = "OwnedCompositeKey", Order = 0, ClrRootType = typeof(OwnerKeyAnnotation), ClrPath = nameof(OwnerKeyAnnotation.Id))]
    [ApiKey(ApiName = "OwnedCompositeKey", Order = 1, ClrPath = nameof(LineNumber))]
    public class OwnedCompositeKeyAnnotation
    {
        public int LineNumber { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    [ApiKey(ApiName = "OwnedDependentKey", Order = 0, ClrRootType = typeof(OwnerKeyAnnotation), ClrPath = nameof(OwnerKeyAnnotation.Id))]
    public class OwnedDependentKeyAnnotation
    {
        public string Description { get; set; } = string.Empty;
    }

    [ApiKey(ApiName = "MissingPathKey")]
    public class MissingTypeLevelKeyPathAnnotation
    {
        public int Id { get; set; }
    }

    [ApiKey(ApiName = "MalformedPathKey", ClrPath = "Nested..Id")]
    public class MalformedTypeLevelKeyPathAnnotation
    {
        public int Id { get; set; }
    }

    [ApiKey(ApiName = "UnresolvedPathKey", ClrPath = "Missing")]
    public class UnresolvedTypeLevelKeyPathAnnotation
    {
        public int Id { get; set; }
    }

    [ApiKey(ApiName = "DuplicatePathKey", Order = 0, ClrPath = nameof(Id))]
    public class DuplicatePathKeyAnnotation
    {
        [ApiKey(ApiName = "DuplicatePathKey", Order = 1, ClrPath = nameof(Id))]
        public int Id { get; set; }
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
            ApiName = "CustomerHasOrders",
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

        [ApiRelationship
        (
            ApiName = "ConventionFilledCustomerOrders",
            Kind = ApiRelationshipKind.OneToMany
        )]
        public List<Order> Orders { get; set; } = [];
    }

    [ApiRelationshipDefinition
    (
        ApiName = "InvoiceForOrder",
        PrincipalType = typeof(Order),
        DependentType = typeof(Invoice),
        Kind = ApiRelationshipKind.OneToOne,
        ForeignKey = "OrderId"
    )]
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
        [ApiProperty(ApiName = "field_code", IsRequired = true)]
        [ApiKey(ApiName = "FieldKey")]
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
            ApiName = "ProductHasTags",
            AssociationType = typeof(ProductTag),
            OtherPrincipalType = typeof(Tag),
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

    [ApiManyToManyRelationshipDefinition
    (
        ApiName = "ProductHasTagsFromType",
        PrincipalTypeA = typeof(Category),
        PrincipalTypeB = typeof(Label),
        AssociationType = typeof(ProductTagFromType),
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
