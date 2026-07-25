// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Reads CLR metadata (attributes or other sources) from registered types and their members,
///     and applies schema configuration to the corresponding builders at
///     <see cref="Internal.ApiConfigurationSource.DataAnnotation"/> precedence.
///     Implement this interface to integrate third-party attribute sets with the annotation pass.
/// </summary>
public interface IApiAnnotationReader
{
    #region Methods
    /// <summary>
    ///     Applies annotations from the CLR type to the supplied object type builder.
    /// </summary>
    /// <param name="clrType">The CLR type being configured.</param>
    /// <param name="builder">The object type builder to configure.</param>
    void ApplyObjectTypeAnnotations(Type clrType, ApiObjectTypeBuilder builder);

    /// <summary>
    ///     Applies annotations from the CLR type to the supplied scalar type builder.
    /// </summary>
    /// <param name="clrType">The CLR type being configured.</param>
    /// <param name="builder">The scalar type builder to configure.</param>
    void ApplyScalarTypeAnnotations(Type clrType, ApiScalarTypeBuilder builder);

    /// <summary>
    ///     Applies annotations from the CLR type to the supplied enum type builder.
    /// </summary>
    /// <param name="clrType">The CLR type being configured.</param>
    /// <param name="builder">The enum type builder to configure.</param>
    void ApplyEnumTypeAnnotations(Type clrType, ApiEnumTypeBuilder builder);

    /// <summary>
    ///     Applies annotations from the CLR member to the supplied property builder.
    /// </summary>
    /// <param name="clrMember">The CLR property or field being configured.</param>
    /// <param name="clrMemberKind">Whether <paramref name="clrMember"/> is a property or field.</param>
    /// <param name="nullabilityInfo">
    ///     Pre-computed nullability info from <see cref="PropertyReflection.GetNullabilityInfo"/>
    ///     or <see cref="FieldReflection.GetNullabilityInfo"/>.
    /// </param>
    /// <param name="builder">The property builder to configure.</param>
    /// <param name="objectTypeBuilder">The object type builder that owns the property.</param>
    void ApplyPropertyAnnotations
    (
        MemberInfo clrMember,
        ClrMemberKind clrMemberKind,
        MemberNullableInfo nullabilityInfo,
        ApiPropertyBuilder builder,
        ApiObjectTypeBuilder objectTypeBuilder
    );

    /// <summary>
    ///     Reads any one-to-many relationship declarations from the CLR type.
    ///     The reader checks both navigation-property-level attributes and type-level attributes.
    /// </summary>
    /// <param name="clrType">The CLR type to inspect.</param>
    /// <returns>
    ///     A list of (name, configure) pairs; each pair will be passed to
    ///     <see cref="ApiSchemaBuilder.AddOneToManyRelationship(string, Action{ApiRelationshipOneToManyBuilder})"/>.
    ///     Return an empty list when no relationships are declared on this type.
    /// </returns>
    IReadOnlyList<(string Name, Action<ApiRelationshipOneToManyBuilder> Configure)>
        ReadOneToManyRelationships(Type clrType);

    /// <summary>
    ///     Reads any one-to-one relationship declarations from the CLR type.
    /// </summary>
    /// <param name="clrType">The CLR type to inspect.</param>
    /// <returns>
    ///     A list of (name, configure) pairs; return an empty list when none are declared.
    /// </returns>
    IReadOnlyList<(string Name, Action<ApiRelationshipOneToOneBuilder> Configure)>
        ReadOneToOneRelationships(Type clrType);

    /// <summary>
    ///     Reads any many-to-many relationship declarations from the CLR type.
    /// </summary>
    /// <param name="clrType">The CLR type to inspect.</param>
    /// <returns>
    ///     A list of (name, configure) pairs; return an empty list when none are declared.
    /// </returns>
    IReadOnlyList<(string Name, Action<ApiRelationshipManyToManyBuilder> Configure)>
        ReadManyToManyRelationships(Type clrType);
    #endregion
}
