// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema.Compilation;

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
    #region ApiAnnotation Compilation Codes
    /// <summary>
    ///     Multiple annotation key paths use the same order within one named key.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_DUPLICATE_KEY_PATH_ORDER")]
    ApiAnnotationDuplicateKeyPathOrder,

    /// <summary>
    ///     An annotation declaration is invalid or unsupported for its target.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_INVALID_DECLARATION")]
    ApiAnnotationInvalidDeclaration,

    /// <summary>
    ///     An annotation reader returned multiple valid version declarations for one object type.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_MULTIPLE_VERSION_DECLARATIONS")]
    ApiAnnotationMultipleVersionDeclarations,

    /// <summary>
    ///     Multiple annotated delegates resolve to the same query-source API name.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_QUERY_SOURCE_DISCOVERY_CONFLICT")]
    ApiAnnotationQuerySourceDiscoveryConflict,

    /// <summary>
    ///     An annotation reader threw while reading metadata for a schema target.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_READER_EXECUTION_FAILED")]
    ApiAnnotationReaderExecutionFailed,

    /// <summary>
    ///     Multiple annotation readers discovered different API type kinds for the same CLR type.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_TYPE_DISCOVERY_CONFLICT")]
    ApiAnnotationTypeDiscoveryConflict,

    /// <summary>
    ///     A CLR type has conflicting built-in API type marker annotations.
    /// </summary>
    [EnumMember(Value = "API_ANNOTATION_TYPE_MARKER_CONFLICT")]
    ApiAnnotationTypeMarkerConflict,
    #endregion

    #region ApiAssembly Compilation Codes
    /// <summary>
    ///     Assembly type discovery failed while scanning an assembly or evaluating a candidate
    ///     filter.
    /// </summary>
    [EnumMember(Value = "API_ASSEMBLY_DISCOVERY_FAILED")]
    ApiAssemblyDiscoveryFailed,
    #endregion

    #region ApiCollectionType Compilation Codes
    /// <summary>
    ///     The collection item's API nullability conflicts with the CLR element nullability.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_ITEM_NULLABILITY_MISMATCH")]
    ApiCollectionItemNullabilityMismatch,

    /// <summary>
    ///     The collection type's item nullability is invalid.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_TYPE_INVALID_API_ITEM_NULLABILITY")]
    ApiCollectionTypeInvalidApiItemNullability,

    /// <summary>
    ///     The collection type has no item type.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_TYPE_MISSING_ITEM_TYPE")]
    ApiCollectionTypeMissingItemType,

    /// <summary>
    ///     The collection type's item type could not be resolved to a declared API type.
    /// </summary>
    [EnumMember(Value = "API_COLLECTION_TYPE_UNRESOLVED_ITEM_TYPE")]
    ApiCollectionTypeUnresolvedItemType,
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

    #region ApiEnumType Compilation Codes
    /// <summary>
    ///     The enum type's CLR type does not represent an enumeration.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_CLR_TYPE_NOT_ENUM")]
    ApiEnumTypeClrTypeNotEnum,

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
    ///     The enum type has no values.
    /// </summary>
    [EnumMember(Value = "API_ENUM_TYPE_MISSING_VALUES")]
    ApiEnumTypeMissingValues,
    #endregion

    #region ApiEnumValue Compilation Codes
    /// <summary>
    ///     The enum value does not have a valid API name.
    /// </summary>
    [EnumMember(Value = "API_ENUM_VALUE_INVALID_API_NAME")]
    ApiEnumValueInvalidApiName,

    /// <summary>
    ///     The enum value does not have a valid CLR name.
    /// </summary>
    [EnumMember(Value = "API_ENUM_VALUE_INVALID_CLR_NAME")]
    ApiEnumValueInvalidClrName,
    #endregion

    #region ApiKeyDefinition Compilation Codes
    /// <summary>
    ///     An <see cref="ApiKeyDefinition"/> has no key paths defined. At least one <see cref="ApiKeyPath"/> is required.
    /// </summary>
    [EnumMember(Value = "API_KEY_DEFINITION_MISSING_PATHS")]
    ApiKeyDefinitionMissingPaths,
    #endregion

    #region ApiKeyPath Compilation Codes
    /// <summary>
    ///     An <see cref="ApiKeyPath"/> declares no explicit root reference and no enclosing
    ///     <see cref="ApiObjectType"/> or <see cref="ApiRelationshipElement"/> can provide an
    ///     inferred root.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_MISSING_ROOT_TYPE")]
    ApiKeyPathMissingRootType,

    /// <summary>
    ///     An <see cref="ApiKeyPath"/> has no segments. At least one segment is required to identify a scalar property.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_MISSING_SEGMENTS")]
    ApiKeyPathMissingSegments,

    /// <summary>
    ///     A non-terminal (navigation) segment of an <see cref="ApiKeyPath"/> resolved to a property whose type is not
    ///     an <see cref="ApiObjectType"/>. Navigation segments must refer to object-typed properties.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_NAVIGATION_SEGMENT_NOT_OBJECT_TYPE")]
    ApiKeyPathNavigationSegmentNotObjectType,

    /// <summary>
    ///     The terminal (scalar) segment of an <see cref="ApiKeyPath"/> resolved to a property whose type is not
    ///     an <see cref="ApiScalarType"/>. The last segment must refer to a scalar-typed property.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_TERMINAL_SEGMENT_NOT_SCALAR_TYPE")]
    ApiKeyPathTerminalSegmentNotScalarType,

    /// <summary>
    ///     An <see cref="ApiKeyPath"/>'s explicit root reference could not be resolved to an
    ///     <see cref="ApiObjectType"/> in the schema.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_UNRESOLVED_ROOT_TYPE")]
    ApiKeyPathUnresolvedRootType,
    #endregion

    #region ApiKeyPathSegment Compilation Codes
    /// <summary>
    ///     An <see cref="ApiKeyPath"/> segment's property reference could not be resolved to an
    ///     <see cref="ApiProperty"/> on the current object type.
    /// </summary>
    [EnumMember(Value = "API_KEY_PATH_SEGMENT_UNRESOLVED_PROPERTY")]
    ApiKeyPathSegmentUnresolvedProperty,
    #endregion

    #region ApiNamedKeyDefinition Compilation Codes
    /// <summary>
    ///     An <see cref="ApiNamedKeyDefinition"/> does not have a valid API name.
    /// </summary>
    [EnumMember(Value = "API_NAMED_KEY_DEFINITION_INVALID_API_NAME")]
    ApiNamedKeyDefinitionInvalidApiName,
    #endregion

    #region ApiNamedType Compilation Codes
    /// <summary>
    ///     The named type does not have a valid API name.
    /// </summary>
    [EnumMember(Value = "API_NAMED_TYPE_INVALID_API_NAME")]
    ApiNamedTypeInvalidApiName,
    #endregion

    #region ApiObjectType Compilation Codes
    /// <summary>
    ///     Multiple keys have the same API name.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_DUPLICATE_KEY_API_NAME")]
    ApiObjectTypeDuplicateKeyApiName,

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
    ///     The object type's key null-handling option could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_INVALID_API_KEY_NULL_HANDLING")]
    ApiObjectTypeInvalidApiKeyNullHandling,

    /// <summary>
    ///     The object type has no properties.
    /// </summary>
    [EnumMember(Value = "API_OBJECT_TYPE_MISSING_PROPERTIES")]
    ApiObjectTypeMissingProperties,

    /// <summary>The canonical-key designation does not resolve to a key declared by the object type.</summary>
    [EnumMember(Value = "API_OBJECT_TYPE_UNRESOLVED_CANONICAL_KEY")]
    ApiObjectTypeUnresolvedCanonicalKey,
    #endregion

    #region ApiProperty Compilation Codes
    /// <summary>
    ///     The property does not have a valid API name.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_API_NAME")]
    ApiPropertyInvalidApiName,

    /// <summary>
    ///     The property's nullability is invalid.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_INVALID_API_NULLABILITY")]
    ApiPropertyInvalidApiNullability,

    /// <summary>
    ///     The property has no type.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_MISSING_TYPE")]
    ApiPropertyMissingType,

    /// <summary>
    ///     The property's API nullability conflicts with the CLR member nullability.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_NULLABILITY_MISMATCH")]
    ApiPropertyNullabilityMismatch,

    /// <summary>
    ///     The property's type could not be resolved to a declared API type.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_UNRESOLVED_TYPE")]
    ApiPropertyUnresolvedType,
    #endregion

    #region ApiPropertyReference Compilation Codes
    /// <summary>
    ///     An API property reference does not have exactly one API-name or CLR-name identity.
    /// </summary>
    [EnumMember(Value = "API_PROPERTY_REFERENCE_INVALID_IDENTITY")]
    ApiPropertyReferenceInvalidIdentity,
    #endregion

    #region ApiQuerySource Compilation Codes
    /// <summary>
    ///     Multiple parameters of one source have the same API name.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_DUPLICATE_PARAMETER_API_NAME")]
    ApiQuerySourceDuplicateParameterApiName,

    /// <summary>
    ///     The query source has an invalid API name.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_INVALID_API_NAME")]
    ApiQuerySourceInvalidApiName,

    /// <summary>
    ///     The allowed composition is invalid or incompatible with the output type.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_INVALID_API_OUTPUT_COMPOSITION")]
    ApiQuerySourceInvalidApiOutputComposition,

    /// <summary>
    ///     The query source output nullability is invalid.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_INVALID_API_OUTPUT_NULLABILITY")]
    ApiQuerySourceInvalidApiOutputNullability,

    /// <summary>
    ///     The query source has no output type.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_MISSING_OUTPUT_TYPE")]
    ApiQuerySourceMissingOutputType,

    /// <summary>
    ///     The query source parameter has an invalid API name.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_PARAMETER_INVALID_API_NAME")]
    ApiQuerySourceParameterInvalidApiName,

    /// <summary>
    ///     The query source parameter nullability is invalid.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_PARAMETER_INVALID_API_NULLABILITY")]
    ApiQuerySourceParameterInvalidApiNullability,

    /// <summary>The query source parameter presence requirement is invalid.</summary>
    [EnumMember(Value = "API_QUERY_SOURCE_PARAMETER_INVALID_API_PRESENCE")]
    ApiQuerySourceParameterInvalidApiPresence,

    /// <summary>
    ///     The query source parameter has no type.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_PARAMETER_MISSING_TYPE")]
    ApiQuerySourceParameterMissingType,

    /// <summary>
    ///     The query source parameter type could not be resolved to a declared API type.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_PARAMETER_UNRESOLVED_TYPE")]
    ApiQuerySourceParameterUnresolvedType,

    /// <summary>
    ///     The query source output type could not be resolved to a declared API type.
    /// </summary>
    [EnumMember(Value = "API_QUERY_SOURCE_UNRESOLVED_OUTPUT_TYPE")]
    ApiQuerySourceUnresolvedOutputType,
    #endregion

    #region ApiRelationship Compilation Codes
    /// <summary>
    ///     The relationship's delete behavior could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_INVALID_API_DELETE_BEHAVIOR")]
    ApiRelationshipInvalidApiDeleteBehavior,

    /// <summary>
    ///     The relationship does not have a valid API name.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_INVALID_API_NAME")]
    ApiRelationshipInvalidApiName,

    /// <summary>
    ///     The relationship has no dependent end.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MISSING_DEPENDENT_END")]
    ApiRelationshipMissingDependentEnd,

    /// <summary>
    ///     The relationship has no principal end.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MISSING_PRINCIPAL_END")]
    ApiRelationshipMissingPrincipalEnd,
    #endregion

    #region ApiRelationshipElement Compilation Codes
    /// <summary>
    ///     The relationship element has no object type.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_ELEMENT_MISSING_OBJECT_TYPE")]
    ApiRelationshipElementMissingObjectType,

    /// <summary>
    ///     The relationship element's object type reference could not be resolved to an object type
    ///     in the schema.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_ELEMENT_UNRESOLVED_OBJECT_TYPE")]
    ApiRelationshipElementUnresolvedObjectType,
    #endregion

    #region ApiRelationshipEnd Compilation Codes
    /// <summary>
    ///     A principal key name was supplied for a navigational relationship that has no foreign key binding.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_END_PRINCIPAL_KEY_REQUIRES_FOREIGN_KEY")]
    ApiRelationshipEndPrincipalKeyRequiresForeignKey,

    /// <summary>
    ///     The principal end's explicitly referenced principal key could not be resolved.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_END_UNRESOLVED_PRINCIPAL_KEY")]
    ApiRelationshipEndUnresolvedPrincipalKey,
    #endregion

    #region ApiRelationshipManyToMany Compilation Codes
    /// <summary>
    ///     The number of scalar leaves in the association's key paths for end A
    ///     does not match the number of scalar leaves in principal end A's key.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_FOREIGN_KEY_A_PATH_COUNT_MISMATCH")]
    ApiRelationshipManyToManyForeignKeyAPathCountMismatch,

    /// <summary>
    ///     The number of scalar leaves in the association's key paths for end B
    ///     does not match the number of scalar leaves in principal end B's key.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_FOREIGN_KEY_B_PATH_COUNT_MISMATCH")]
    ApiRelationshipManyToManyForeignKeyBPathCountMismatch,

    /// <summary>
    ///     The many-to-many relationship has no association.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_MISSING_ASSOCIATION")]
    ApiRelationshipManyToManyMissingAssociation,

    /// <summary>
    ///     The many-to-many relationship has no principal end A.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_MISSING_PRINCIPAL_END_A")]
    ApiRelationshipManyToManyMissingPrincipalEndA,

    /// <summary>
    ///     The many-to-many relationship has no principal end B.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_MANY_TO_MANY_MISSING_PRINCIPAL_END_B")]
    ApiRelationshipManyToManyMissingPrincipalEndB,
    #endregion

    #region ApiRelationshipOneTo Compilation Codes
    /// <summary>
    ///     The principal end's key cannot be automatically determined because multiple keys
    ///     on the principal object type are compatible with the foreign key.
    ///     Specify the principal key explicitly using <see cref="ApiRelationshipPrincipalEnd.ApiPrincipalKeyName"/>.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_AMBIGUOUS_PRINCIPAL_KEY")]
    ApiRelationshipAmbiguousPrincipalKey,

    /// <summary>
    ///     The number of scalar leaves in the dependent end's key paths does not match
    ///     the number of scalar leaves in the principal end's key in either
    ///     one-to-one or one-to-many relationships.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_FOREIGN_KEY_PATH_COUNT_MISMATCH")]
    ApiRelationshipForeignKeyPathCountMismatch,

    /// <summary>
    ///     The principal end's key could not be matched to the foreign key because their ordered scalar
    ///     leaf types are incompatible.
    /// </summary>
    [EnumMember(Value = "API_RELATIONSHIP_PRINCIPAL_FOREIGN_KEY_TYPE_MISMATCH")]
    ApiRelationshipPrincipalForeignKeyTypeMismatch,
    #endregion

    #region ApiRelationshipTraversal Compilation Codes
    /// <summary>A CLR member is bound to both a contained property and a traversal.</summary>
    [EnumMember(Value = "API_RELATIONSHIP_TRAVERSAL_CLR_MEMBER_CONFLICT")]
    ApiRelationshipTraversalClrMemberConflict,

    /// <summary>Multiple relationship traversals have the same API name on one source object type.</summary>
    [EnumMember(Value = "API_RELATIONSHIP_TRAVERSAL_DUPLICATE_API_NAME")]
    ApiRelationshipTraversalDuplicateApiName,

    /// <summary>A relationship traversal has an invalid API name.</summary>
    [EnumMember(Value = "API_RELATIONSHIP_TRAVERSAL_INVALID_API_NAME")]
    ApiRelationshipTraversalInvalidApiName,

    /// <summary>A relationship traversal and contained property have the same API name.</summary>
    [EnumMember(Value = "API_RELATIONSHIP_TRAVERSAL_PROPERTY_API_NAME_CONFLICT")]
    ApiRelationshipTraversalPropertyApiNameConflict,
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
    ///     Multiple query sources have the same API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_DUPLICATE_QUERY_SOURCE_API_NAME")]
    ApiSchemaDuplicateQuerySourceApiName,

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
    ///     A schema extension returned an invalid frozen snapshot.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_EXTENSION_INVALID_SNAPSHOT")]
    ApiSchemaExtensionInvalidSnapshot,

    /// <summary>
    ///     An attached schema extension does not implement the frozen snapshot contract.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_EXTENSION_NOT_FREEZABLE")]
    ApiSchemaExtensionNotFreezable,

    /// <summary>
    ///     A schema extension failed while creating its frozen snapshot.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_EXTENSION_SNAPSHOT_FAILED")]
    ApiSchemaExtensionSnapshotFailed,

    /// <summary>
    ///     The schema's key null-handling option could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_INVALID_API_KEY_NULL_HANDLING")]
    ApiSchemaInvalidApiKeyNullHandling,

    /// <summary>
    ///     The schema does not have a valid API name.
    /// </summary>
    [EnumMember(Value = "API_SCHEMA_INVALID_API_NAME")]
    ApiSchemaInvalidApiName,
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

    #region ApiType Compilation Codes
    /// <summary>
    ///     The API type has no CLR type.
    /// </summary>
    [EnumMember(Value = "API_TYPE_MISSING_CLR_TYPE")]
    ApiTypeMissingClrType,
    #endregion

    #region ApiTypeExpression Compilation Codes
    /// <summary>
    ///     A type expression does not define exactly one inline or referenced type source.
    /// </summary>
    [EnumMember(Value = "API_TYPE_EXPRESSION_INVALID_DEFINITION")]
    ApiTypeExpressionInvalidDefinition,
    #endregion

    #region ApiTypeReference Compilation Codes
    /// <summary>
    ///     A type reference's API kind could not be read from schema JSON.
    /// </summary>
    [EnumMember(Value = "API_TYPE_REFERENCE_INVALID_API_KIND")]
    ApiTypeReferenceInvalidApiKind,

    /// <summary>
    ///     A type reference does not have exactly one complete API or CLR identity.
    /// </summary>
    [EnumMember(Value = "API_TYPE_REFERENCE_INVALID_IDENTITY")]
    ApiTypeReferenceInvalidIdentity,
    #endregion

    #region ApiVersionDefinition Compilation Codes
    /// <summary>The version member and scalar API type use different CLR types.</summary>
    [EnumMember(Value = "API_VERSION_DEFINITION_CLR_TYPE_MISMATCH")]
    ApiVersionDefinitionClrTypeMismatch,

    /// <summary>The repository-backed version definition has no CLR type.</summary>
    [EnumMember(Value = "API_VERSION_DEFINITION_MISSING_CLR_TYPE")]
    ApiVersionDefinitionMissingClrType,

    /// <summary>The schema has no scalar type for the version CLR type.</summary>
    [EnumMember(Value = "API_VERSION_DEFINITION_MISSING_SCALAR_TYPE")]
    ApiVersionDefinitionMissingScalarType,

    /// <summary>The property-backed version refers to a non-scalar API property.</summary>
    [EnumMember(Value = "API_VERSION_DEFINITION_NON_SCALAR_PROPERTY")]
    ApiVersionDefinitionNonScalarProperty,

    /// <summary>The version definition uses a nullable CLR value type.</summary>
    [EnumMember(Value = "API_VERSION_DEFINITION_NULLABLE_CLR_TYPE")]
    ApiVersionDefinitionNullableClrType,

    /// <summary>The property-backed version refers to a nullable API property.</summary>
    [EnumMember(Value = "API_VERSION_DEFINITION_NULLABLE_PROPERTY")]
    ApiVersionDefinitionNullableProperty,

    /// <summary>No API property matches the property-backed version property reference.</summary>
    [EnumMember(Value = "API_VERSION_DEFINITION_UNRESOLVED_PROPERTY")]
    ApiVersionDefinitionUnresolvedProperty,
    #endregion

    #region ClrMemberReference Compilation Codes
    /// <summary>
    ///     A resolved CLR member is incompatible with its schema role.
    /// </summary>
    [EnumMember(Value = "CLR_MEMBER_BINDING_INCOMPATIBLE")]
    ClrMemberBindingIncompatible,

    /// <summary>
    ///     The framework could not create read access for a CLR member.
    /// </summary>
    [EnumMember(Value = "CLR_MEMBER_READ_ACCESS_FAILED")]
    ClrMemberReadAccessFailed,

    /// <summary>
    ///     A CLR member reference has an invalid CLR kind.
    /// </summary>
    [EnumMember(Value = "CLR_MEMBER_REFERENCE_INVALID_CLR_KIND")]
    ClrMemberReferenceInvalidClrKind,

    /// <summary>
    ///     A CLR member reference has an invalid CLR name.
    /// </summary>
    [EnumMember(Value = "CLR_MEMBER_REFERENCE_INVALID_CLR_NAME")]
    ClrMemberReferenceInvalidClrName,

    /// <summary>
    ///     A CLR member reference could not resolve its property or field.
    /// </summary>
    [EnumMember(Value = "CLR_MEMBER_REFERENCE_UNRESOLVED")]
    ClrMemberReferenceUnresolved,

    /// <summary>
    ///     The framework could not create write access for a CLR member.
    /// </summary>
    [EnumMember(Value = "CLR_MEMBER_WRITE_ACCESS_FAILED")]
    ClrMemberWriteAccessFailed,
    #endregion
}
