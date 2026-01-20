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
    ///     Multiple identity parts reference the same property API name.
    /// </summary>
    API_IDENTITY_DUPLICATE_PART_API_PROPERTY_NAME,

    /// <summary>
    ///     The identity's API name is null, empty, or whitespace.
    /// </summary>
    API_IDENTITY_INVALID_API_NAME,

    /// <summary>
    ///     The identity has no parts defined.
    /// </summary>
    API_IDENTITY_NULL_OR_EMPTY_PARTS,
    #endregion

    #region ApiIdentityPart Initialization Codes
    /// <summary>
    ///     An identity part references a property whose type has an identity that creates a circular dependency.
    /// </summary>
    API_IDENTITY_PART_CIRCULAR_REFERENCE,

    /// <summary>
    ///     The identity part's property API name is null, empty, or whitespace.
    /// </summary>
    API_IDENTITY_PART_INVALID_API_PROPERTY_NAME,

    /// <summary>
    ///     The scalar CLR type specified for an <see cref="ApiIdentityPart"/> is not compatible with <see cref="ApiId"/>.
    /// </summary>
    API_IDENTITY_PART_INVALID_SCALAR_TYPE,

    /// <summary>
    ///     An identity part uses a property CLR type that may have performance implications for identity operations.
    /// </summary>
    API_IDENTITY_PART_PERFORMANCE_CONCERN,

    /// <summary>
    ///     The identity part's property reference could not be resolved to a defined property.
    /// </summary>
    API_IDENTITY_PART_UNRESOLVED_PROPERTY,
    #endregion

    #region ApiNamedType Initialization Codes
    /// <summary>
    ///     The named type's API name is null, empty, or whitespace.
    /// </summary>
    API_NAMED_TYPE_INVALID_API_NAME,
    #endregion

    #region ApiObjectType Initialization Codes
    /// <summary>
    ///     Multiple identities may be ambiguous due to using the same property set.
    /// </summary>
    API_OBJECT_TYPE_AMBIGUOUS_IDENTITIES,

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
    ///     Multiple relationships have the same API name.
    /// </summary>
    API_OBJECT_TYPE_DUPLICATE_RELATIONSHIP_API_NAME,

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
    #endregion

    #region ApiRelationship Initialization Codes
    /// <summary>
    ///     The relationship's API name is null, empty, or whitespace.
    /// </summary>
    API_RELATIONSHIP_INVALID_NAME,

    /// <summary>
    ///     The relationship's property reference could not be resolved to a defined property.
    /// </summary>
    API_RELATIONSHIP_UNRESOLVED_PROPERTY,
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
