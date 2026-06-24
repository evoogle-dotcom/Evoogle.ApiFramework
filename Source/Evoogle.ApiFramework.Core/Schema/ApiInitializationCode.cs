// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines error and warning codes used during API schema initialization.
/// </summary>
/// <remarks>
///     These codes identify specific validation issues encountered when initializing API schema elements,
///     such as missing required values, duplicate names, unresolved references, or invalid configurations.
/// </remarks>
[JsonConverter(typeof(EnumJsonConverter<ApiInitializationCode>))]
public enum ApiInitializationCode
{
    #region ApiCollectionType Initialization Codes
    /// <summary>
    ///     The collection type's item type expression is null.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_TYPE_NULL_ITEM_TYPE")]
    ApiCollectionTypeNullItemType = 0,

    /// <summary>
    ///     The collection type's item type expression could not be resolved to a valid API type.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_TYPE_UNRESOLVED_ITEM_TYPE")]
    ApiCollectionTypeUnresolvedItemType = 1,

    /// <summary>
    ///     The collection item is declared Required but the CLR element type is nullable.
    ///     The API contract demands a value, but the CLR element type permits null.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_ITEM_REQUIRED_NULLABLE_MISMATCH")]
    ApiCollectionItemRequiredNullableMismatch = 2,

    /// <summary>
    ///     The collection item is declared Optional but the CLR element type is a non-nullable reference type.
    ///     An absent Optional item may assign null to a CLR element that cannot hold it.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_ITEM_OPTIONAL_NON_NULLABLE_MISMATCH")]
    ApiCollectionItemOptionalNonNullableMismatch = 3,
    #endregion

    #region ApiEnumType Initialization Codes
    /// <summary>
    ///     Multiple enum values have the same API name.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_DUPLICATE_VALUE_API_NAME")]
    ApiEnumTypeDuplicateValueApiName = 4,

    /// <summary>
    ///     Multiple enum values have the same CLR name.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_DUPLICATE_VALUE_CLR_NAME")]
    ApiEnumTypeDuplicateValueClrName = 5,

    /// <summary>
    ///     Multiple enum values have the same CLR ordinal value.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_DUPLICATE_VALUE_CLR_ORDINAL")]
    ApiEnumTypeDuplicateValueClrOrdinal = 6,

    /// <summary>
    ///     The CLR type is not a valid enumeration type.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_INVALID_CLR_TYPE")]
    ApiEnumTypeInvalidClrType = 7,

    /// <summary>
    ///     The enum type has no values defined.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_NULL_OR_EMPTY_VALUES")]
    ApiEnumTypeNullOrEmptyValues = 8,
    #endregion

    #region ApiEnumValue Initialization Codes
    /// <summary>
    ///     The enum value's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_ENUM_VALUE_INVALID_API_NAME")]
    ApiEnumValueInvalidApiName = 9,

    /// <summary>
    ///     The enum value's CLR name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_ENUM_VALUE_INVALID_CLR_NAME")]
    ApiEnumValueInvalidClrName = 10,
    #endregion

    #region ApiKeyPath Initialization Codes
    /// <summary>
    ///     An <see cref="ApiKeyPath"/> has no segments. At least one segment is required to identify a scalar property.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_EMPTY_SEGMENTS")]
    ApiKeyPathEmptySegments = 11,

    /// <summary>
    ///     A non-terminal (navigation) segment of an <see cref="ApiKeyPath"/> resolved to a property whose type is not
    ///     an <see cref="ApiObjectType"/>. Navigation segments must refer to object-typed properties.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_NAVIGATION_SEGMENT_INVALID_TYPE")]
    ApiKeyPathNavigationSegmentInvalidType = 12,

    /// <summary>
    ///     The terminal (scalar) segment of an <see cref="ApiKeyPath"/> resolved to a property whose type is not
    ///     an <see cref="ApiScalarType"/>. The last segment must refer to a scalar-typed property.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_SCALAR_SEGMENT_INVALID_TYPE")]
    ApiKeyPathScalarSegmentInvalidType = 13,

    /// <summary>
    ///     An <see cref="ApiKeyPath"/>'s root CLR type is not registered as an <see cref="ApiObjectType"/> in the schema.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_UNRESOLVED_ROOT_TYPE")]
    ApiKeyPathUnresolvedRootType = 14,
    #endregion

    #region ApiKeyPathSegment Initialization Codes
    /// <summary>
    ///     An <see cref="ApiKeyPath"/> segment's CLR property name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_SEGMENT_INVALID_CLR_PROPERTY_NAME")]
    ApiKeyPathSegmentInvalidClrPropertyName = 15,

    /// <summary>
    ///     An <see cref="ApiKeyPath"/> segment's CLR property name could not be resolved to a defined property on the current object type.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_SEGMENT_UNRESOLVED_API_PROPERTY")]
    ApiKeyPathSegmentUnresolvedApiProperty = 16,
    #endregion

    #region ApiKeyType Initialization Codes
    /// <summary>
    ///     An <see cref="ApiKeyType"/>'s API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_KEY_TYPE_INVALID_API_NAME")]
    ApiKeyTypeInvalidApiName = 17,

    /// <summary>
    ///     An <see cref="ApiKeyType"/> has no key paths defined. At least one <see cref="ApiKeyPath"/> is required.
    /// </summary>
    [EnumMember(Value = "API_KEY_TYPE_NULL_OR_EMPTY_PATHS")]
    ApiKeyTypeNullOrEmptyPaths = 18,
    #endregion

    #region ApiNamedType Initialization Codes
    /// <summary>
    ///     The named type's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_NAMED_TYPE_INVALID_API_NAME")]
    ApiNamedTypeInvalidApiName = 19,
    #endregion

    #region ApiObjectType Initialization Codes
    /// <summary>
    ///     Multiple key types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_DUPLICATE_KEY_TYPE_API_NAME")]
    ApiObjectTypeDuplicateKeyTypeApiName = 20,

    /// <summary>
    ///     Multiple properties have the same API name.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_DUPLICATE_PROPERTY_API_NAME")]
    ApiObjectTypeDuplicatePropertyApiName = 21,

    /// <summary>
    ///     Multiple properties have the same CLR name.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_DUPLICATE_PROPERTY_CLR_NAME")]
    ApiObjectTypeDuplicatePropertyClrName = 22,

    /// <summary>
    ///     The object type has no properties defined.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_NULL_OR_EMPTY_PROPERTIES")]
    ApiObjectTypeNullOrEmptyProperties = 23,
    #endregion

    #region ApiProperty Initialization Codes
    /// <summary>
    ///     The property's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_API_NAME")]
    ApiPropertyInvalidApiName = 24,

    /// <summary>
    ///     The property's CLR name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_CLR_NAME")]
    ApiPropertyInvalidClrName = 25,

    /// <summary>
    ///     The property's CLR member is not a valid field or property.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_CLR_MEMBER")]
    ApiPropertyInvalidClrMember = 26,

    /// <summary>
    ///     The property's field getter could not be created or is invalid.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_FIELD_GETTER")]
    ApiPropertyInvalidFieldGetter = 27,

    /// <summary>
    ///     The property's field setter could not be created or is invalid.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_FIELD_SETTER")]
    ApiPropertyInvalidFieldSetter = 28,

    /// <summary>
    ///     The property's property getter could not be created or is invalid.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_PROPERTY_GETTER")]
    ApiPropertyInvalidPropertyGetter = 29,

    /// <summary>
    ///     The property's property setter could not be created or is invalid.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_PROPERTY_SETTER")]
    ApiPropertyInvalidPropertySetter = 30,

    /// <summary>
    ///     The property's CLR member (field or property) could not be found on the CLR type.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_MISSING_CLR_MEMBER")]
    ApiPropertyMissingClrMember = 31,

    /// <summary>
    ///     The property's type expression is null.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_NULL_TYPE")]
    ApiPropertyNullType = 32,

    /// <summary>
    ///     The property's type expression could not be resolved to a valid API type.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_UNRESOLVED_TYPE")]
    ApiPropertyUnresolvedType = 33,

    /// <summary>
    ///     The property is declared Required but the CLR member is nullable.
    ///     The API contract demands a value, but the CLR type permits null.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_REQUIRED_NULLABLE_MISMATCH")]
    ApiPropertyRequiredNullableMismatch = 34,

    /// <summary>
    ///     The property is declared Optional but the CLR member is a non-nullable reference type.
    ///     An absent Optional property may assign null to a CLR member that cannot hold it.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_OPTIONAL_NON_NULLABLE_MISMATCH")]
    ApiPropertyOptionalNonNullableMismatch = 35,
    #endregion

    #region ApiRelationship Initialization Codes
    /// <summary>
    ///     The relationship's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_INVALID_API_NAME")]
    ApiRelationshipInvalidApiName = 36,

    /// <summary>
    ///     The relationship's principal end is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_NULL_PRINCIPAL_END")]
    ApiRelationshipNullPrincipalEnd = 37,

    /// <summary>
    ///     The relationship's dependent end is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_NULL_DEPENDENT_END")]
    ApiRelationshipNullDependentEnd = 38,
    #endregion

    #region ApiRelationshipElement Initialization Codes
    /// <summary>
    ///     The relationship element's CLR object type is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_ELEMENT_NULL_CLR_OBJECT_TYPE")]
    ApiRelationshipElementNullClrObjectType = 39,

    /// <summary>
    ///     The relationship element's object type name could not be resolved to a defined object type in the schema.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_ELEMENT_UNRESOLVED_OBJECT_TYPE")]
    ApiRelationshipElementUnresolvedObjectType = 40,
    #endregion

    #region ApiRelationshipEnd Initialization Codes
    /// <summary>
    ///     The principal end's explicitly referenced principal key type could not be resolved.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_END_UNRESOLVED_KEY_TYPE")]
    ApiRelationshipEndUnresolvedKeyType = 41,

    /// <summary>
    ///     A principal key type name was supplied for a navigational relationship that has no foreign key binding.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_END_PRINCIPAL_KEY_WITHOUT_FOREIGN_KEY")]
    ApiRelationshipEndPrincipalKeyWithoutForeignKey = 42,
    #endregion

    #region ApiRelationshipManyToMany Initialization Codes
    /// <summary>
    ///     The many-to-many relationship's principal end A is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_A")]
    ApiRelationshipManyToManyNullPrincipalEndA = 43,

    /// <summary>
    ///     The many-to-many relationship's principal end B is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_B")]
    ApiRelationshipManyToManyNullPrincipalEndB = 44,

    /// <summary>
    ///     The many-to-many relationship's association is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_NULL_ASSOCIATION")]
    ApiRelationshipManyToManyNullAssociation = 45,

    /// <summary>
    ///     The number of scalar leaves in the association's key paths for end A
    ///     does not match the number of scalar leaves in principal end A's key type.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_A_COUNT")]
    ApiRelationshipManyToManyInvalidAssociationKeyPathsACount = 46,

    /// <summary>
    ///     The number of scalar leaves in the association's key paths for end B
    ///     does not match the number of scalar leaves in principal end B's key type.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_B_COUNT")]
    ApiRelationshipManyToManyInvalidAssociationKeyPathsBCount = 47,
    #endregion

    #region ApiRelationshipOneTo Initialization Codes
    /// <summary>
    ///     The number of scalar leaves in the dependent end's key paths does not match
    ///     the number of scalar leaves in the principal end's key type in either
    ///     one-to-one or one-to-many relationships.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_ONE_TO_INVALID_DEPENDENT_KEY_PATHS_COUNT")]
    ApiRelationshipOneToInvalidDependentKeyPathsCount = 48,

    /// <summary>
    ///     The principal end's key type cannot be automatically determined because multiple key types
    ///     on the principal object type are compatible with the foreign key type.
    ///     Specify the principal key type explicitly using <see cref="ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName"/>.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_AMBIGUOUS_PRINCIPAL_KEY")]
    ApiRelationshipAmbiguousPrincipalKey = 49,

    /// <summary>
    ///     The principal end's key type could not be matched to the foreign key type because their ordered scalar
    ///     leaf types are incompatible.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_INCOMPATIBLE_PRINCIPAL_FOREIGN_KEY")]
    ApiRelationshipIncompatiblePrincipalForeignKey = 50,
    #endregion

    #region ApiSchema Initialization Codes
    /// <summary>
    ///     Multiple enum types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_ENUM_TYPE_API_NAME")]
    ApiSchemaDuplicateEnumTypeApiName = 51,

    /// <summary>
    ///     Multiple enum types have the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_ENUM_TYPE_CLR_TYPE")]
    ApiSchemaDuplicateEnumTypeClrType = 52,

    /// <summary>
    ///     Multiple named types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_NAMED_TYPE_API_NAME")]
    ApiSchemaDuplicateNamedTypeApiName = 53,

    /// <summary>
    ///     Multiple named types have the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_NAMED_TYPE_CLR_TYPE")]
    ApiSchemaDuplicateNamedTypeClrType = 54,

    /// <summary>
    ///     Multiple object types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_OBJECT_TYPE_API_NAME")]
    ApiSchemaDuplicateObjectTypeApiName = 55,

    /// <summary>
    ///     Multiple object types have the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_OBJECT_TYPE_CLR_TYPE")]
    ApiSchemaDuplicateObjectTypeClrType = 56,

    /// <summary>
    ///     Multiple relationships have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_RELATIONSHIP_API_NAME")]
    ApiSchemaDuplicateRelationshipApiName = 57,

    /// <summary>
    ///     Multiple scalar types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_SCALAR_TYPE_API_NAME")]
    ApiSchemaDuplicateScalarTypeApiName = 58,

    /// <summary>
    ///     Multiple scalar types have the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_SCALAR_TYPE_CLR_TYPE")]
    ApiSchemaDuplicateScalarTypeClrType = 59,

    /// <summary>
    ///     The schema's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_INVALID_NAME")]
    ApiSchemaInvalidName = 60,
    #endregion

    #region ApiType Initialization Codes
    /// <summary>
    ///     The type's CLR type is null.
    /// </summary>
    [EnumMember(Value = "API_TYPE_NULL_CLR_TYPE")]
    ApiTypeNullClrType = 61
    #endregion
}
