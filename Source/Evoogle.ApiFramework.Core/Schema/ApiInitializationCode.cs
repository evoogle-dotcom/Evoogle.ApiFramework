// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines error and warning codes used during API schema initialization.
/// </summary>
/// <remarks>
///     These codes identify specific validation issues encountered when initializing API schema elements,
///     such as missing required values, duplicate names, unresolved references, or invalid configurations.
/// </remarks>
public enum ApiInitializationCode
{
    #region ApiCollectionType Initialization Codes
    /// <summary>
    ///     The collection type's item type expression is null.
    /// </summary>
    API_COLLECTION_TYPE_NULL_ITEM_TYPE,

    /// <summary>
    ///     The collection type's item type expression could not be resolved to a valid API type.
    /// </summary>
    API_COLLECTION_TYPE_UNRESOLVED_ITEM_TYPE,

    /// <summary>
    ///     The collection item is declared Required but the CLR element type is nullable.
    ///     The API contract demands a value, but the CLR element type permits null.
    /// </summary>
    API_COLLECTION_ITEM_REQUIRED_NULLABLE_MISMATCH,

    /// <summary>
    ///     The collection item is declared Optional but the CLR element type is a non-nullable reference type.
    ///     An absent Optional item may assign null to a CLR element that cannot hold it.
    /// </summary>
    API_COLLECTION_ITEM_OPTIONAL_NON_NULLABLE_MISMATCH,
    #endregion

    #region ApiEnumType Initialization Codes
    /// <summary>
    ///     Multiple enum values have the same API name.
    /// </summary>
    API_ENUM_TYPE_DUPLICATE_VALUE_API_NAME,

    /// <summary>
    ///     Multiple enum values have the same CLR name.
    /// </summary>
    API_ENUM_TYPE_DUPLICATE_VALUE_CLR_NAME,

    /// <summary>
    ///     Multiple enum values have the same CLR ordinal value.
    /// </summary>
    API_ENUM_TYPE_DUPLICATE_VALUE_CLR_ORDINAL,

    /// <summary>
    ///     The CLR type is not a valid enumeration type.
    /// </summary>
    API_ENUM_TYPE_INVALID_CLR_TYPE,

    /// <summary>
    ///     The enum type has no values defined.
    /// </summary>
    API_ENUM_TYPE_NULL_OR_EMPTY_VALUES,
    #endregion

    #region ApiEnumValue Initialization Codes
    /// <summary>
    ///     The enum value's API name is null, empty, or whitespace.
    /// </summary>
    API_ENUM_VALUE_INVALID_API_NAME,

    /// <summary>
    ///     The enum value's CLR name is null, empty, or whitespace.
    /// </summary>
    API_ENUM_VALUE_INVALID_CLR_NAME,
    #endregion

    #region ApiKeyPath Initialization Codes
    /// <summary>
    ///     An <see cref="ApiKeyPath"/> has no segments. At least one segment is required to identify a scalar property.
    /// </summary>
    API_KEY_PATH_EMPTY_SEGMENTS,

    /// <summary>
    ///     A non-terminal (navigation) segment of an <see cref="ApiKeyPath"/> resolved to a property whose type is not
    ///     an <see cref="ApiObjectType"/>. Navigation segments must refer to object-typed properties.
    /// </summary>
    API_KEY_PATH_NAVIGATION_SEGMENT_INVALID_TYPE,

    /// <summary>
    ///     The terminal (scalar) segment of an <see cref="ApiKeyPath"/> resolved to a property whose type is not
    ///     an <see cref="ApiScalarType"/>. The last segment must refer to a scalar-typed property.
    /// </summary>
    API_KEY_PATH_SCALAR_SEGMENT_INVALID_TYPE,

    /// <summary>
    ///     An <see cref="ApiKeyPath"/>'s root CLR type is not registered as an <see cref="ApiObjectType"/> in the schema.
    /// </summary>
    API_KEY_PATH_UNRESOLVED_ROOT_TYPE,
    #endregion

    #region ApiKeyPathSegment Initialization Codes
    /// <summary>
    ///     An <see cref="ApiKeyPath"/> segment's CLR property name is null, empty, or whitespace.
    /// </summary>
    API_KEY_PATH_SEGMENT_INVALID_CLR_PROPERTY_NAME,

    /// <summary>
    ///     An <see cref="ApiKeyPath"/> segment's CLR property name could not be resolved to a defined property on the current object type.
    /// </summary>
    API_KEY_PATH_SEGMENT_UNRESOLVED_API_PROPERTY,
    #endregion

    #region ApiKeyType Initialization Codes
    /// <summary>
    ///     An <see cref="ApiKeyType"/>'s API name is null, empty, or whitespace.
    /// </summary>
    API_KEY_TYPE_INVALID_API_NAME,

    /// <summary>
    ///     An <see cref="ApiKeyType"/> has no key paths defined. At least one <see cref="ApiKeyPath"/> is required.
    /// </summary>
    API_KEY_TYPE_NULL_OR_EMPTY_PATHS,
    #endregion

    #region ApiNamedType Initialization Codes
    /// <summary>
    ///     The named type's API name is null, empty, or whitespace.
    /// </summary>
    API_NAMED_TYPE_INVALID_API_NAME,
    #endregion

    #region ApiObjectType Initialization Codes
    /// <summary>
    ///     Multiple key types have the same API name.
    /// </summary>
    API_OBJECT_TYPE_DUPLICATE_KEY_TYPE_API_NAME,

    /// <summary>
    ///     Multiple properties have the same API name.
    /// </summary>
    API_OBJECT_TYPE_DUPLICATE_PROPERTY_API_NAME,

    /// <summary>
    ///     Multiple properties have the same CLR name.
    /// </summary>
    API_OBJECT_TYPE_DUPLICATE_PROPERTY_CLR_NAME,

    /// <summary>
    ///     The object type has no properties defined.
    /// </summary>
    API_OBJECT_TYPE_NULL_OR_EMPTY_PROPERTIES,
    #endregion

    #region ApiProperty Initialization Codes
    /// <summary>
    ///     The property's API name is null, empty, or whitespace.
    /// </summary>
    API_PROPERTY_INVALID_API_NAME,

    /// <summary>
    ///     The property's CLR name is null, empty, or whitespace.
    /// </summary>
    API_PROPERTY_INVALID_CLR_NAME,

    /// <summary>
    ///     The property's CLR member is not a valid field or property.
    /// </summary>
    API_PROPERTY_INVALID_CLR_MEMBER,

    /// <summary>
    ///     The property's field getter could not be created or is invalid.
    /// </summary>
    API_PROPERTY_INVALID_FIELD_GETTER,

    /// <summary>
    ///     The property's field setter could not be created or is invalid.
    /// </summary>
    API_PROPERTY_INVALID_FIELD_SETTER,

    /// <summary>
    ///     The property's property getter could not be created or is invalid.
    /// </summary>
    API_PROPERTY_INVALID_PROPERTY_GETTER,

    /// <summary>
    ///     The property's property setter could not be created or is invalid.
    /// </summary>
    API_PROPERTY_INVALID_PROPERTY_SETTER,

    /// <summary>
    ///     The property's CLR member (field or property) could not be found on the CLR type.
    /// </summary>
    API_PROPERTY_MISSING_CLR_MEMBER,

    /// <summary>
    ///     The property's type expression is null.
    /// </summary>
    API_PROPERTY_NULL_TYPE,

    /// <summary>
    ///     The property's type expression could not be resolved to a valid API type.
    /// </summary>
    API_PROPERTY_UNRESOLVED_TYPE,

    /// <summary>
    ///     The property is declared Required but the CLR member is nullable.
    ///     The API contract demands a value, but the CLR type permits null.
    /// </summary>
    API_PROPERTY_REQUIRED_NULLABLE_MISMATCH,

    /// <summary>
    ///     The property is declared Optional but the CLR member is a non-nullable reference type.
    ///     An absent Optional property may assign null to a CLR member that cannot hold it.
    /// </summary>
    API_PROPERTY_OPTIONAL_NON_NULLABLE_MISMATCH,
    #endregion

    #region ApiRelationship Initialization Codes
    /// <summary>
    ///     The relationship's API name is null, empty, or whitespace.
    /// </summary>
    API_RELATIONSHIP_INVALID_API_NAME,

    /// <summary>
    ///     The relationship's principal end is null.
    /// </summary>
    API_RELATIONSHIP_NULL_PRINCIPAL_END,

    /// <summary>
    ///     The relationship's dependent end is null.
    /// </summary>
    API_RELATIONSHIP_NULL_DEPENDENT_END,
    #endregion

    #region ApiRelationshipElement Initialization Codes
    /// <summary>
    ///     The relationship element's CLR object type is null.
    /// </summary>
    API_RELATIONSHIP_ELEMENT_NULL_CLR_OBJECT_TYPE,

    /// <summary>
    ///     The relationship element's object type name could not be resolved to a defined object type in the schema.
    /// </summary>
    API_RELATIONSHIP_ELEMENT_UNRESOLVED_OBJECT_TYPE,
    #endregion

    #region ApiRelationshipEnd Initialization Codes
    /// <summary>
    ///     The principal end's referenced key type could not be resolved, or the principal type has no primary key type.
    /// </summary>
    API_RELATIONSHIP_END_UNRESOLVED_KEY_TYPE,
    #endregion

    #region ApiRelationshipManyToMany Initialization Codes
    /// <summary>
    ///     The many-to-many relationship's principal end A is null.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_A,

    /// <summary>
    ///     The many-to-many relationship's principal end B is null.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_B,

    /// <summary>
    ///     The many-to-many relationship's association is null.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_NULL_ASSOCIATION,

    /// <summary>
    ///     The number of scalar leaves in the association's key paths for end A
    ///     does not match the number of scalar leaves in principal end A's key type.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_A_COUNT,

    /// <summary>
    ///     The number of scalar leaves in the association's key paths for end B
    ///     does not match the number of scalar leaves in principal end B's key type.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_B_COUNT,

    #endregion

    #region ApiRelationshipOneTo Initialization Codes
    /// <summary>
    ///     The number of scalar leaves in the dependent end's key paths does not match
    ///     the number of scalar leaves in the principal end's key type in either
    ///     one-to-one or one-to-many relationships.
    /// </summary>
    API_RELATIONSHIP_ONE_TO_INVALID_DEPENDENT_KEY_PATHS_COUNT,

    /// <summary>
    ///     The principal end's key type cannot be automatically determined because multiple key types
    ///     on the principal object type are compatible with the foreign key type.
    ///     Specify the key type explicitly using <see cref="ApiRelationshipPrincipalEnd.ApiKeyTypeName"/>.
    /// </summary>
    API_RELATIONSHIP_AMBIGUOUS_PRINCIPAL_KEY,

    /// <summary>
    ///     The principal end's key type could not be matched to the foreign key type because their ordered scalar
    ///     leaf types are incompatible.
    /// </summary>
    API_RELATIONSHIP_INCOMPATIBLE_PRINCIPAL_FOREIGN_KEY,
    #endregion

    #region ApiSchema Initialization Codes
    /// <summary>
    ///     Multiple enum types have the same API name.
    /// </summary>
    API_SCHEMA_DUPLICATE_ENUM_TYPE_API_NAME,

    /// <summary>
    ///     Multiple enum types have the same CLR type.
    /// </summary>
    API_SCHEMA_DUPLICATE_ENUM_TYPE_CLR_TYPE,

    /// <summary>
    ///     Multiple named types have the same API name.
    /// </summary>
    API_SCHEMA_DUPLICATE_NAMED_TYPE_API_NAME,

    /// <summary>
    ///     Multiple named types have the same CLR type.
    /// </summary>
    API_SCHEMA_DUPLICATE_NAMED_TYPE_CLR_TYPE,

    /// <summary>
    ///     Multiple object types have the same API name.
    /// </summary>
    API_SCHEMA_DUPLICATE_OBJECT_TYPE_API_NAME,

    /// <summary>
    ///     Multiple object types have the same CLR type.
    /// </summary>
    API_SCHEMA_DUPLICATE_OBJECT_TYPE_CLR_TYPE,

    /// <summary>
    ///     Multiple relationships have the same API name.
    /// </summary>
    API_SCHEMA_DUPLICATE_RELATIONSHIP_API_NAME,

    /// <summary>
    ///     Multiple scalar types have the same API name.
    /// </summary>
    API_SCHEMA_DUPLICATE_SCALAR_TYPE_API_NAME,

    /// <summary>
    ///     Multiple scalar types have the same CLR type.
    /// </summary>
    API_SCHEMA_DUPLICATE_SCALAR_TYPE_CLR_TYPE,

    /// <summary>
    ///     The schema's API name is null, empty, or whitespace.
    /// </summary>
    API_SCHEMA_INVALID_NAME,
    #endregion

    #region ApiType Initialization Codes
    /// <summary>
    ///     The type's CLR type is null.
    /// </summary>
    API_TYPE_NULL_CLR_TYPE,
    #endregion
}
