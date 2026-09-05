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
///     Defines error and warning codes used during API schema compilation.
/// </summary>
/// <remarks>
///     These codes identify specific validation issues encountered when compiling API schema elements,
///     such as missing required values, duplicate names, unresolved references, or invalid configurations.
/// </remarks>
[JsonConverter(typeof(EnumJsonConverter<ApiSchemaCompilationCode>))]
public enum ApiSchemaCompilationCode
{
    #region ApiCollectionType Compilation Codes
    /// <summary>
    ///     The collection type's item type expression is null.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_TYPE_NULL_ITEM_TYPE")]
    ApiCollectionTypeNullItemType,

    /// <summary>
    ///     The collection type's item type expression could not be resolved to a valid API type.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_TYPE_UNRESOLVED_ITEM_TYPE")]
    ApiCollectionTypeUnresolvedItemType,

    /// <summary>
    ///     The collection type's item type modifiers could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_TYPE_INVALID_API_ITEM_TYPE_MODIFIERS")]
    ApiCollectionTypeInvalidApiItemTypeModifiers,

    /// <summary>
    ///     The collection item is declared Required but the CLR element type is nullable.
    ///     The API contract demands a value, but the CLR element type permits null.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_ITEM_REQUIRED_NULLABLE_MISMATCH")]
    ApiCollectionItemRequiredNullableMismatch,

    /// <summary>
    ///     The collection item is declared Optional but the CLR element type is a non-nullable reference type.
    ///     An absent Optional item may assign null to a CLR element that cannot hold it.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_ITEM_OPTIONAL_NON_NULLABLE_MISMATCH")]
    ApiCollectionItemOptionalNonNullableMismatch,
    #endregion

    #region ApiEnumType Compilation Codes
    /// <summary>
    ///     Multiple enum values have the same API name.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_DUPLICATE_VALUE_API_NAME")]
    ApiEnumTypeDuplicateValueApiName,

    /// <summary>
    ///     Multiple enum values have the same CLR name.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_DUPLICATE_VALUE_CLR_NAME")]
    ApiEnumTypeDuplicateValueClrName,

    /// <summary>
    ///     Multiple enum values have the same CLR ordinal value.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_DUPLICATE_VALUE_CLR_ORDINAL")]
    ApiEnumTypeDuplicateValueClrOrdinal,

    /// <summary>
    ///     The CLR type is not a valid enumeration type.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_INVALID_CLR_TYPE")]
    ApiEnumTypeInvalidClrType,

    /// <summary>
    ///     The enum type has no values defined.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_NULL_OR_EMPTY_VALUES")]
    ApiEnumTypeNullOrEmptyValues,
    #endregion

    #region ApiEnumValue Compilation Codes
    /// <summary>
    ///     The enum value's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_ENUM_VALUE_INVALID_API_NAME")]
    ApiEnumValueInvalidApiName,

    /// <summary>
    ///     The enum value's CLR name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_ENUM_VALUE_INVALID_CLR_NAME")]
    ApiEnumValueInvalidClrName,
    #endregion

    #region ApiKeyPath Compilation Codes
    /// <summary>
    ///     An <see cref="ApiKeyPath"/> has no segments. At least one segment is required to identify a scalar property.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_EMPTY_SEGMENTS")]
    ApiKeyPathEmptySegments,

    /// <summary>
    ///     A non-terminal (navigation) segment of an <see cref="ApiKeyPath"/> resolved to a property whose type is not
    ///     an <see cref="ApiObjectType"/>. Navigation segments must refer to object-typed properties.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_NAVIGATION_SEGMENT_INVALID_TYPE")]
    ApiKeyPathNavigationSegmentInvalidType,

    /// <summary>
    ///     The terminal (scalar) segment of an <see cref="ApiKeyPath"/> resolved to a property whose type is not
    ///     an <see cref="ApiScalarType"/>. The last segment must refer to a scalar-typed property.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_SCALAR_SEGMENT_INVALID_TYPE")]
    ApiKeyPathScalarSegmentInvalidType,

    /// <summary>
    ///     An <see cref="ApiKeyPath"/>'s root CLR type is not registered as an <see cref="ApiObjectType"/> in the schema.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_UNRESOLVED_ROOT_TYPE")]
    ApiKeyPathUnresolvedRootType,

    /// <summary>
    ///     An <see cref="ApiKeyPath"/> declares no explicit root CLR type and no owning
    ///     <see cref="ApiObjectType"/> or <see cref="ApiRelationshipElement"/> could supply one.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_UNINFERABLE_ROOT_TYPE")]
    ApiKeyPathUninferableRootType,
    #endregion

    #region ApiKeyPathSegment Compilation Codes
    /// <summary>
    ///     An <see cref="ApiKeyPath"/> segment's CLR property name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_SEGMENT_INVALID_CLR_PROPERTY_NAME")]
    ApiKeyPathSegmentInvalidClrPropertyName,

    /// <summary>
    ///     An <see cref="ApiKeyPath"/> segment's CLR property name could not be resolved to a defined property on the current object type.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_SEGMENT_UNRESOLVED_API_PROPERTY")]
    ApiKeyPathSegmentUnresolvedApiProperty,
    #endregion

    #region ApiKeyType and ApiNamedKeyType Compilation Codes
    /// <summary>
    ///     An <see cref="ApiKeyType"/> has no key paths defined. At least one <see cref="ApiKeyPath"/> is required.
    /// </summary>
    [EnumMember(Value = "API_KEY_TYPE_NULL_OR_EMPTY_PATHS")]
    ApiKeyTypeNullOrEmptyPaths,

    /// <summary>
    ///     An <see cref="ApiNamedKeyType"/>'s API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_NAMED_KEY_TYPE_INVALID_API_NAME")]
    ApiNamedKeyTypeInvalidApiName,
    #endregion

    #region ApiNamedType Compilation Codes
    /// <summary>
    ///     The named type's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_NAMED_TYPE_INVALID_API_NAME")]
    ApiNamedTypeInvalidApiName,
    #endregion

    #region ApiObjectType Compilation Codes
    /// <summary>
    ///     Multiple key types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_DUPLICATE_KEY_TYPE_API_NAME")]
    ApiObjectTypeDuplicateKeyTypeApiName,

    /// <summary>
    ///     Multiple properties have the same API name.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_DUPLICATE_PROPERTY_API_NAME")]
    ApiObjectTypeDuplicatePropertyApiName,

    /// <summary>
    ///     Multiple properties have the same CLR name.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_DUPLICATE_PROPERTY_CLR_NAME")]
    ApiObjectTypeDuplicatePropertyClrName,

    /// <summary>
    ///     The object type has no properties defined.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_NULL_OR_EMPTY_PROPERTIES")]
    ApiObjectTypeNullOrEmptyProperties,

    /// <summary>
    ///     The object type's key null-handling option could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_INVALID_API_KEY_NULL_HANDLING")]
    ApiObjectTypeInvalidApiKeyNullHandling,
    #endregion

    #region ApiProperty Compilation Codes
    /// <summary>
    ///     The property's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_API_NAME")]
    ApiPropertyInvalidApiName,

    /// <summary>
    ///     The property's CLR name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_CLR_NAME")]
    ApiPropertyInvalidClrName,

    /// <summary>
    ///     The property's CLR member is not a valid field or property.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_CLR_MEMBER")]
    ApiPropertyInvalidClrMember,

    /// <summary>
    ///     The property's type modifiers could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_API_TYPE_MODIFIERS")]
    ApiPropertyInvalidApiTypeModifiers,

    /// <summary>
    ///     The property's field getter could not be created or is invalid.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_FIELD_GETTER")]
    ApiPropertyInvalidFieldGetter,

    /// <summary>
    ///     The property's field setter could not be created or is invalid.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_FIELD_SETTER")]
    ApiPropertyInvalidFieldSetter,

    /// <summary>
    ///     The property's property getter could not be created or is invalid.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_PROPERTY_GETTER")]
    ApiPropertyInvalidPropertyGetter,

    /// <summary>
    ///     The property's property setter could not be created or is invalid.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_PROPERTY_SETTER")]
    ApiPropertyInvalidPropertySetter,

    /// <summary>
    ///     The property's CLR member (field or property) could not be found on the CLR type.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_MISSING_CLR_MEMBER")]
    ApiPropertyMissingClrMember,

    /// <summary>
    ///     The property's type expression is null.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_NULL_TYPE")]
    ApiPropertyNullType,

    /// <summary>
    ///     The property's type expression could not be resolved to a valid API type.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_UNRESOLVED_TYPE")]
    ApiPropertyUnresolvedType,

    /// <summary>
    ///     The property is declared Required but the CLR member is nullable.
    ///     The API contract demands a value, but the CLR type permits null.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_REQUIRED_NULLABLE_MISMATCH")]
    ApiPropertyRequiredNullableMismatch,

    /// <summary>
    ///     The property is declared Optional but the CLR member is a non-nullable reference type.
    ///     An absent Optional property may assign null to a CLR member that cannot hold it.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_OPTIONAL_NON_NULLABLE_MISMATCH")]
    ApiPropertyOptionalNonNullableMismatch,
    #endregion

    #region ApiRelationship Compilation Codes
    /// <summary>
    ///     The relationship's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_INVALID_API_NAME")]
    ApiRelationshipInvalidApiName,

    /// <summary>
    ///     The relationship's delete behavior could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_INVALID_API_DELETE_BEHAVIOR")]
    ApiRelationshipInvalidApiDeleteBehavior,

    /// <summary>
    ///     The relationship's principal end is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_NULL_PRINCIPAL_END")]
    ApiRelationshipNullPrincipalEnd,

    /// <summary>
    ///     The relationship's dependent end is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_NULL_DEPENDENT_END")]
    ApiRelationshipNullDependentEnd,
    #endregion

    #region ApiRelationshipElement Compilation Codes
    /// <summary>
    ///     The relationship element's CLR object type is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_ELEMENT_NULL_CLR_OBJECT_TYPE")]
    ApiRelationshipElementNullClrObjectType,

    /// <summary>
    ///     The relationship element's object type name could not be resolved to a defined object type in the schema.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_ELEMENT_UNRESOLVED_OBJECT_TYPE")]
    ApiRelationshipElementUnresolvedObjectType,
    #endregion

    #region ApiRelationshipEnd Compilation Codes
    /// <summary>
    ///     The principal end's explicitly referenced principal key type could not be resolved.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_END_UNRESOLVED_KEY_TYPE")]
    ApiRelationshipEndUnresolvedKeyType,

    /// <summary>
    ///     A principal key type name was supplied for a navigational relationship that has no foreign key binding.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_END_PRINCIPAL_KEY_WITHOUT_FOREIGN_KEY")]
    ApiRelationshipEndPrincipalKeyWithoutForeignKey,
    #endregion

    #region ApiRelationshipManyToMany Compilation Codes
    /// <summary>
    ///     The many-to-many relationship's principal end A is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_A")]
    ApiRelationshipManyToManyNullPrincipalEndA,

    /// <summary>
    ///     The many-to-many relationship's principal end B is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_B")]
    ApiRelationshipManyToManyNullPrincipalEndB,

    /// <summary>
    ///     The many-to-many relationship's association is null.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_NULL_ASSOCIATION")]
    ApiRelationshipManyToManyNullAssociation,

    /// <summary>
    ///     The number of scalar leaves in the association's key paths for end A
    ///     does not match the number of scalar leaves in principal end A's key type.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_A_COUNT")]
    ApiRelationshipManyToManyInvalidAssociationKeyPathsACount,

    /// <summary>
    ///     The number of scalar leaves in the association's key paths for end B
    ///     does not match the number of scalar leaves in principal end B's key type.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_B_COUNT")]
    ApiRelationshipManyToManyInvalidAssociationKeyPathsBCount,
    #endregion

    #region ApiRelationshipOneTo Compilation Codes
    /// <summary>
    ///     The number of scalar leaves in the dependent end's key paths does not match
    ///     the number of scalar leaves in the principal end's key type in either
    ///     one-to-one or one-to-many relationships.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_ONE_TO_INVALID_DEPENDENT_KEY_PATHS_COUNT")]
    ApiRelationshipOneToInvalidDependentKeyPathsCount,

    /// <summary>
    ///     The principal end's key type cannot be automatically determined because multiple key types
    ///     on the principal object type are compatible with the foreign key type.
    ///     Specify the principal key type explicitly using <see cref="ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName"/>.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_AMBIGUOUS_PRINCIPAL_KEY")]
    ApiRelationshipAmbiguousPrincipalKey,

    /// <summary>
    ///     The principal end's key type could not be matched to the foreign key type because their ordered scalar
    ///     leaf types are incompatible.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_INCOMPATIBLE_PRINCIPAL_FOREIGN_KEY")]
    ApiRelationshipIncompatiblePrincipalForeignKey,
    #endregion

    #region ApiSchemaElement Compilation Codes
    /// <summary>
    ///     The same schema element instance appears in more than one structural ownership position
    ///     or schema tree.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_ELEMENT_DUPLICATE_OWNERSHIP")]
    ApiSchemaElementDuplicateOwnership,

    /// <summary>
    ///     The schema element ownership graph contains a cycle.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_ELEMENT_OWNERSHIP_CYCLE")]
    ApiSchemaElementOwnershipCycle,
    #endregion

    #region ApiSchema Compilation Codes
    /// <summary>
    ///     Multiple enum types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_ENUM_TYPE_API_NAME")]
    ApiSchemaDuplicateEnumTypeApiName,

    /// <summary>
    ///     Multiple enum types have the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_ENUM_TYPE_CLR_TYPE")]
    ApiSchemaDuplicateEnumTypeClrType,

    /// <summary>
    ///     Multiple named types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_NAMED_TYPE_API_NAME")]
    ApiSchemaDuplicateNamedTypeApiName,

    /// <summary>
    ///     Multiple named types have the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_NAMED_TYPE_CLR_TYPE")]
    ApiSchemaDuplicateNamedTypeClrType,

    /// <summary>
    ///     Multiple object types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_OBJECT_TYPE_API_NAME")]
    ApiSchemaDuplicateObjectTypeApiName,

    /// <summary>
    ///     Multiple object types have the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_OBJECT_TYPE_CLR_TYPE")]
    ApiSchemaDuplicateObjectTypeClrType,

    /// <summary>
    ///     Multiple relationships have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_RELATIONSHIP_API_NAME")]
    ApiSchemaDuplicateRelationshipApiName,

    /// <summary>
    ///     Multiple scalar types have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_SCALAR_TYPE_API_NAME")]
    ApiSchemaDuplicateScalarTypeApiName,

    /// <summary>
    ///     Multiple scalar types have the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_SCALAR_TYPE_CLR_TYPE")]
    ApiSchemaDuplicateScalarTypeClrType,

    /// <summary>
    ///     The schema's API name is null, empty, or whitespace.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_INVALID_NAME")]
    ApiSchemaInvalidName,

    /// <summary>
    ///     The schema's key null-handling option could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_INVALID_API_KEY_NULL_HANDLING")]
    ApiSchemaInvalidApiKeyNullHandling,

    /// <summary>
    ///     An attached schema extension does not implement the frozen snapshot contract.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_EXTENSION_UNSUPPORTED")]
    ApiSchemaExtensionUnsupported,

    /// <summary>
    ///     A schema extension failed while creating its frozen snapshot.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_EXTENSION_SNAPSHOT_FAILED")]
    ApiSchemaExtensionSnapshotFailed,

    /// <summary>
    ///     A schema extension returned an invalid frozen snapshot.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_EXTENSION_INVALID_SNAPSHOT")]
    ApiSchemaExtensionInvalidSnapshot,
    #endregion

    #region ApiTypeExpression Compilation Codes
    /// <summary>
    ///     A type expression's API kind could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_TYPE_EXPRESSION_INVALID_API_KIND")]
    ApiTypeExpressionInvalidApiKind,
    #endregion

    #region ApiType Compilation Codes
    /// <summary>
    ///     The type's CLR type is null.
    /// </summary>
    [EnumMember(Value = "API_TYPE_NULL_CLR_TYPE")]
    ApiTypeNullClrType,
    #endregion

    #region ApiConfiguration Compilation Codes
    /// <summary>
    ///     A discovered configuration could not be activated.
    /// </summary>
    [EnumMember(Value = "API_CONFIGURATION_ACTIVATION_FAILED")]
    ApiConfigurationActivationFailed,

    /// <summary>
    ///     A discovered configuration threw while exposing its identity or applying its configuration.
    /// </summary>
    [EnumMember(Value = "API_CONFIGURATION_EXECUTION_FAILED")]
    ApiConfigurationExecutionFailed,
    #endregion

    #region ApiAssembly Compilation Codes
    /// <summary>
    ///     Assembly type discovery failed while scanning an assembly or evaluating a candidate
    ///     filter.
    /// </summary>
    [EnumMember(Value = "API_ASSEMBLY_DISCOVERY_FAILED")]
    ApiAssemblyDiscoveryFailed,
    #endregion

    #region ApiAnnotation Compilation Codes
    /// <summary>
    ///     An annotation reader threw while reading metadata for a schema target.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_READER_EXECUTION_FAILED")]
    ApiAnnotationReaderExecutionFailed,

    /// <summary>
    ///     An annotation reader returned an invalid or unsupported contribution.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_INVALID_CONTRIBUTION")]
    ApiAnnotationInvalidContribution,

    /// <summary>
    ///     A CLR type has conflicting built-in API type marker annotations.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_TYPE_MARKER_CONFLICT")]
    ApiAnnotationTypeMarkerConflict,

    /// <summary>
    ///     Multiple annotation readers discovered different API type kinds for the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_TYPE_DISCOVERY_CONFLICT")]
    ApiAnnotationTypeDiscoveryConflict,

    /// <summary>
    ///     Multiple annotation key paths use the same order within one named key type.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_KEY_ORDER_CONFLICT")]
    ApiAnnotationKeyOrderConflict,
    #endregion
}
