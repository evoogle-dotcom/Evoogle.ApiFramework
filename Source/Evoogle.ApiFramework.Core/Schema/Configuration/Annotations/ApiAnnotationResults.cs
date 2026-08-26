// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

#region Result Types
/// <summary>Describes a type-level annotation result.</summary>
/// <param name="ApiName">The optional API name supplied by the annotation.</param>
public sealed record ApiTypeAnnotationResult(string? ApiName);

/// <summary>Describes a property-level annotation result.</summary>
/// <param name="ApiName">The optional API property name.</param>
/// <param name="Modifiers">The optional API type modifiers.</param>
public sealed record ApiPropertyAnnotationResult(string? ApiName, ApiTypeModifiers? Modifiers);

/// <summary>Describes an enum-value annotation result.</summary>
/// <param name="ApiName">The optional API enum-value name.</param>
public sealed record ApiEnumValueAnnotationResult(string? ApiName);

/// <summary>Describes a key path contribution to a named key type.</summary>
/// <param name="ApiName">The API name of the key type.</param>
/// <param name="Order">The zero-based order of the path within the key.</param>
/// <param name="ClrRootType">The CLR type from which the path begins.</param>
/// <param name="ClrPropertyNames">The ordered CLR property names in the path.</param>
public sealed record ApiKeyAnnotationResult
(
    string ApiName,
    int Order,
    Type ClrRootType,
    IReadOnlyList<string> ClrPropertyNames
);

/// <summary>Describes a one-to-many relationship annotation result.</summary>
/// <param name="ApiName">The schema-unique relationship API name.</param>
/// <param name="PrincipalType">The principal CLR object type.</param>
/// <param name="DependentType">The dependent CLR object type.</param>
/// <param name="ForeignKey">The optional dependent CLR foreign-key member name.</param>
/// <param name="DeleteBehavior">The configured delete behavior.</param>
public sealed record ApiOneToManyRelationshipAnnotationResult
(
    string ApiName,
    Type PrincipalType,
    Type DependentType,
    string? ForeignKey,
    ApiRelationshipDeleteBehavior DeleteBehavior
);

/// <summary>Describes a one-to-one relationship annotation result.</summary>
/// <param name="ApiName">The schema-unique relationship API name.</param>
/// <param name="PrincipalType">The principal CLR object type.</param>
/// <param name="DependentType">The dependent CLR object type.</param>
/// <param name="ForeignKey">The optional dependent CLR foreign-key member name.</param>
/// <param name="DeleteBehavior">The configured delete behavior.</param>
public sealed record ApiOneToOneRelationshipAnnotationResult
(
    string ApiName,
    Type PrincipalType,
    Type DependentType,
    string? ForeignKey,
    ApiRelationshipDeleteBehavior DeleteBehavior
);

/// <summary>Describes a many-to-many relationship annotation result.</summary>
/// <param name="ApiName">The schema-unique relationship API name.</param>
/// <param name="PrincipalTypeA">The first principal CLR object type.</param>
/// <param name="PrincipalTypeB">The second principal CLR object type.</param>
/// <param name="AssociationType">The CLR association object type.</param>
/// <param name="ForeignKeyA">The optional association member for principal A.</param>
/// <param name="ForeignKeyB">The optional association member for principal B.</param>
public sealed record ApiManyToManyRelationshipAnnotationResult
(
    string ApiName,
    Type PrincipalTypeA,
    Type PrincipalTypeB,
    Type AssociationType,
    string? ForeignKeyA,
    string? ForeignKeyB
);

/// <summary>Describes a CLR type discovered during assembly annotation scanning.</summary>
/// <param name="ClrType">The discovered CLR type.</param>
/// <param name="ApiKind">The API type kind to register.</param>
public sealed record ApiTypeDiscoveryAnnotationResult(Type ClrType, ApiTypeKind ApiKind);
#endregion
