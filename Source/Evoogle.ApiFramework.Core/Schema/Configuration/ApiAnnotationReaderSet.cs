// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Holds an ordered list of <see cref="IApiAnnotationReader"/> instances and applies
///     them to individual builders under configuration-pipeline coordination.
/// </summary>
public sealed class ApiAnnotationReaderSet
{
    #region Fields
    private readonly IReadOnlyList<IApiAnnotationReader> _readers;
    #endregion

    #region Constructors
    internal ApiAnnotationReaderSet(IReadOnlyList<IApiAnnotationReader> readers)
    {
        _readers = readers;
    }
    #endregion

    #region Type Annotation Methods
    /// <summary>Applies object-type annotations to one builder.</summary>
    internal void ApplyObjectTypeAnnotations(ApiObjectTypeBuilder builder)
    {
        var clrType = builder.ClrType;

        foreach (var reader in _readers)
        {
            reader.ApplyObjectTypeAnnotations(clrType, builder);
        }
    }

    /// <summary>Applies scalar-type annotations to one builder.</summary>
    internal void ApplyScalarTypeAnnotations(ApiScalarTypeBuilder builder)
    {
        var clrType = builder.ClrType;

        foreach (var reader in _readers)
        {
            reader.ApplyScalarTypeAnnotations(clrType, builder);
        }
    }

    /// <summary>Applies enum-type annotations to one builder.</summary>
    internal void ApplyEnumTypeAnnotations(ApiEnumTypeBuilder builder)
    {
        var clrType = builder.ClrType;

        foreach (var reader in _readers)
        {
            reader.ApplyEnumTypeAnnotations(clrType, builder);
        }
    }

    /// <summary>Applies enum-value annotations to one builder.</summary>
    internal void ApplyEnumValueAnnotations
    (
        ApiEnumTypeBuilder enumTypeBuilder,
        ApiEnumValueBuilder enumValueBuilder
    )
    {
        var clrField = TypeReflection.GetField(enumTypeBuilder.ClrType, enumValueBuilder.ClrName);
        if (clrField == null)
        {
            return;
        }

        foreach (var reader in _readers)
        {
            reader.ApplyEnumValueAnnotations(clrField, enumTypeBuilder, enumValueBuilder);
        }
    }
    #endregion

    #region Property Annotation Methods
    /// <summary>Applies member annotations to one property builder.</summary>
    internal void ApplyPropertyAnnotations
    (
        ApiPropertyBuilder propertyBuilder,
        ApiObjectTypeBuilder objectBuilder
    )
    {
        var clrType = objectBuilder.ClrType;
        var clrName = propertyBuilder.ClrName;

        var propertyInfo = TypeReflection.GetProperty(clrType, clrName);
        if (propertyInfo != null)
        {
            var nullabilityInfo = PropertyReflection.GetNullabilityInfo(propertyInfo);

            foreach (var reader in _readers)
            {
                reader.ApplyPropertyAnnotations
                (
                    propertyInfo,
                    ClrMemberKind.Property,
                    nullabilityInfo,
                    objectBuilder,
                    propertyBuilder
                );
            }

            return;
        }

        var fieldInfo = TypeReflection.GetField(clrType, clrName);
        if (fieldInfo == null)
        {
            return;
        }

        var fieldNullabilityInfo = FieldReflection.GetNullabilityInfo(fieldInfo);

        foreach (var reader in _readers)
        {
            reader.ApplyPropertyAnnotations
            (
                fieldInfo,
                ClrMemberKind.Field,
                fieldNullabilityInfo,
                objectBuilder,
                propertyBuilder
            );
        }
    }
    #endregion

    #region Relationship Annotation Methods
    /// <summary>Reads and applies relationship annotations from one settled object type.</summary>
    internal void ApplyRelationshipAnnotations
    (
        ApiSchemaBuilder schemaBuilder,
        Type clrType
    )
    {
        foreach (var reader in _readers)
        {
            foreach (var (apiName, configure) in reader.ReadOneToManyRelationships(clrType))
            {
                schemaBuilder.AddOneToManyRelationshipCore
                (
                    apiName,
                    configure,
                    ApiConfigurationSource.DataAnnotation
                );
            }

            foreach (var (apiName, configure) in reader.ReadOneToOneRelationships(clrType))
            {
                schemaBuilder.AddOneToOneRelationshipCore
                (
                    apiName,
                    configure,
                    ApiConfigurationSource.DataAnnotation
                );
            }

            foreach (var (apiName, configure) in reader.ReadManyToManyRelationships(clrType))
            {
                schemaBuilder.AddManyToManyRelationshipCore
                (
                    apiName,
                    configure,
                    ApiConfigurationSource.DataAnnotation
                );
            }
        }
    }
    #endregion
}
