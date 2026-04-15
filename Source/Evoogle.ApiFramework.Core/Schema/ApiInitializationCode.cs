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

    #region ApiIdentity Initialization Codes
    /// <summary>
    ///    Multiple identity parts reference the same property API name, which may cause ambiguity.
    /// </summary>
    API_IDENTITY_DUPLICATE_PART_API_PROPERTY_NAME,

    /// <summary>
    ///     The identity's API name is null, empty, or whitespace.
    /// </summary>
    API_IDENTITY_INVALID_API_NAME,

    /// <summary>
    ///    More than one owner identity part is defined, which is not allowed.
    /// </summary>
    API_IDENTITY_MULTIPLE_OWNER_PARTS,

    /// <summary>
    ///     Multiple identity parts reference the same property API name.
    /// </summary>
    API_IDENTITY_NULL_OR_EMPTY_PARTS,
    #endregion

    #region ApiIdentityPart Initialization Codes
    /// <summary>
    ///    The identity part's property reference is of an invalid type for the identity part kind (e.g., non-scalar type for a scalar identity part).
    /// </summary>
    API_IDENTITY_PART_INVALID_API_PROPERTY_TYPE,

    /// <summary>
    ///     The identity part's property CLR name is null, empty, or whitespace.
    /// </summary>
    API_IDENTITY_PART_INVALID_CLR_PROPERTY_NAME,

    /// <summary>
    ///     An identity part uses a property CLR type that may have performance implications for identity operations.
    /// </summary>
    API_IDENTITY_PART_PERFORMANCE_CONCERN,

    /// <summary>
    ///     The identity part's property reference could not be resolved to a defined property.
    /// </summary>
    API_IDENTITY_PART_UNRESOLVED_API_PROPERTY,

    /// <summary>
    ///    The identity part's nested identity reference could not be resolved to a defined identity.
    /// </summary>
    API_IDENTITY_PART_UNRESOLVED_NESTED_IDENTITY,

    /// <summary>
    ///     No owner object type could be found for an owner identity part.
    /// </summary>
    API_IDENTITY_PART_UNRESOLVED_OWNER,

    /// <summary>
    ///     Multiple candidate owner object types were found for an owner identity part and could not be disambiguated.
    /// </summary>
    API_IDENTITY_PART_AMBIGUOUS_OWNER,

    /// <summary>
    ///     A cyclic owner identity reference was detected (e.g., A owns B owns A).
    /// </summary>
    API_IDENTITY_PART_CYCLIC_OWNER,
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

    #region ApiRelationshipEnd Initialization Codes
    /// <summary>
    ///     The relationship end's CLR object type is null.
    /// </summary>
    API_RELATIONSHIP_END_NULL_CLR_OBJECT_TYPE,

    /// <summary>
    ///     The relationship end's object type name could not be resolved to a defined object type in the schema.
    /// </summary>
    API_RELATIONSHIP_END_UNRESOLVED_OBJECT_TYPE,

    /// <summary>
    ///     The principal end's referenced identity could not be resolved, or the principal type has no primary identity.
    /// </summary>
    API_RELATIONSHIP_END_UNRESOLVED_IDENTITY,
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
    ///     The many-to-many relationship's dependent end A is null.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_NULL_DEPENDENT_END_A,

    /// <summary>
    ///     The many-to-many relationship's dependent end B is null.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_NULL_DEPENDENT_END_B,

    /// <summary>
    ///     The many-to-many relationship's association type name is null, empty, or whitespace.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_TYPE_NAME,

    /// <summary>
    ///     The many-to-many relationship's association type name could not be resolved to a defined object type in the schema.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_UNRESOLVED_ASSOCIATION_TYPE,

    /// <summary>
    ///     A dependent end of a many-to-many relationship references an object type that does not match the declared association type.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_DEPENDENT_TYPE_MISMATCH,

    /// <summary>
    ///     A dependent end of a many-to-many relationship has null or empty key paths.
    ///     Purely navigational many-to-many relationships are not supported.
    /// </summary>
    API_RELATIONSHIP_MANY_TO_MANY_EMPTY_KEY_PATHS,
    #endregion

    #region ApiRelationshipKeyPath Initialization Codes
    /// <summary>
    ///    The relationship key path's CLR property name is null, empty, or whitespace.
    /// </summary>
    API_RELATIONSHIP_KEY_PATH_INVALID_CLR_PROPERTY_NAME,

    /// <summary>
    ///    The relationship key path's property reference could not be resolved to a defined property on the declaring object type.
    /// </summary>
    API_RELATIONSHIP_KEY_PATH_UNRESOLVED_API_PROPERTY,

    /// <summary>
    ///     The relationship key path's property reference is of an invalid type for the path kind
    ///     (e.g., a non-object property used as a nested key path navigation property).
    /// </summary>
    API_RELATIONSHIP_KEY_PATH_INVALID_API_PROPERTY_TYPE,

    /// <summary>
    ///     A nested or owner relationship key path contains no child paths.
    /// </summary>
    API_RELATIONSHIP_KEY_PATH_NULL_OR_EMPTY_PATHS,
    #endregion

    #region ApiNamedType Initialization Codes
    /// <summary>
    ///     The named type's API name is null, empty, or whitespace.
    /// </summary>
    API_NAMED_TYPE_INVALID_API_NAME,
    #endregion

    #region ApiObjectType Initialization Codes
    /// <summary>
    ///     Multiple identities have the same API name.
    /// </summary>
    API_OBJECT_TYPE_DUPLICATE_IDENTITY_API_NAME,

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
