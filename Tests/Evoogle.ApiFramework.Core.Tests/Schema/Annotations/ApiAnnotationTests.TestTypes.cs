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
    }
    #endregion

    #region Test Domain Types — ApiKey Attributes
    public class PersonWithKeyAnnotation
    {
        [ApiKey]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CompositeKeyType
    {
        [ApiKey(order: 0)]
        public Guid OrderId { get; set; }

        [ApiKey(order: 1)]
        public long LineItemNumber { get; set; }

        public string Description { get; set; } = string.Empty;
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
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [ApiRelationship("CustomerHasOrders", Kind = ApiRelationshipKind.OneToMany, ForeignKey = "CustomerId")]
        public List<Order> Orders { get; set; } = [];
    }

    public class Order
    {
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
}
