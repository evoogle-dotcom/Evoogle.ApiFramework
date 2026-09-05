// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Identifies the concrete built-in kind of an <see cref="ApiSchemaElement"/>.
/// </summary>
public enum ApiSchemaElementKind
{
    #region Values
    /// <summary>Represents an <see cref="ApiCollectionType"/>.</summary>
    CollectionType,

    /// <summary>Represents an <see cref="ApiEnumType"/>.</summary>
    EnumType,

    /// <summary>Represents an <see cref="ApiEnumValue"/>.</summary>
    EnumValue,

    /// <summary>Represents an <see cref="ApiKeyPath"/>.</summary>
    KeyPath,

    /// <summary>Represents an <see cref="ApiKeyPathSegment"/>.</summary>
    KeyPathSegment,

    /// <summary>Represents an <see cref="ApiKeyType"/>.</summary>
    KeyType,

    /// <summary>Represents an <see cref="ApiNamedKeyType"/>.</summary>
    NamedKeyType,

    /// <summary>Represents an <see cref="ApiObjectType"/>.</summary>
    ObjectType,

    /// <summary>Represents an <see cref="ApiProperty"/>.</summary>
    Property,

    /// <summary>Represents an <see cref="ApiRelationshipAssociation"/>.</summary>
    RelationshipAssociation,

    /// <summary>Represents an <see cref="ApiRelationshipDependentEnd"/>.</summary>
    RelationshipDependentEnd,

    /// <summary>Represents an <see cref="ApiRelationshipManyToMany"/>.</summary>
    RelationshipManyToMany,

    /// <summary>Represents an <see cref="ApiRelationshipOneToMany"/>.</summary>
    RelationshipOneToMany,

    /// <summary>Represents an <see cref="ApiRelationshipOneToOne"/>.</summary>
    RelationshipOneToOne,

    /// <summary>Represents an <see cref="ApiRelationshipPrincipalEnd"/>.</summary>
    RelationshipPrincipalEnd,

    /// <summary>Represents an <see cref="ApiScalarType"/>.</summary>
    ScalarType,

    /// <summary>Represents an <see cref="ApiSchema"/>.</summary>
    Schema
    #endregion
}
