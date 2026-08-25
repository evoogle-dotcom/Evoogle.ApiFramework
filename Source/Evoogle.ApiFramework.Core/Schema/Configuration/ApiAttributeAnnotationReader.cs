// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Annotations;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Default <see cref="IApiAnnotationReader"/> implementation that reads the framework's
///     built-in attribute set (<see cref="ApiObjectAttribute"/>, <see cref="ApiPropertyAttribute"/>,
///     <see cref="ApiEnumValueAttribute"/>, <see cref="ApiKeyAttribute"/>,
///     <see cref="ApiIgnoreAttribute"/>, and the relationship attributes).
/// </summary>
public sealed class ApiAttributeAnnotationReader : IApiAnnotationReader
{
    #region IApiAnnotationReader — Type Level
    /// <inheritdoc />
    public void ApplyObjectTypeAnnotations(Type clrType, ApiObjectTypeBuilder builder)
    {
        var attr = clrType.GetCustomAttribute<ApiObjectAttribute>(inherit: false);
        if (attr?.ApiName != null)
        {
            builder.SetApiNameDataAnnotation(attr.ApiName);
        }

        var keyAttrs = clrType.GetCustomAttributes<ApiKeyAttribute>(inherit: true)
            .OrderBy(a => a.Order)
            .ToList();

        foreach (var keyAttr in keyAttrs)
        {
            var clrRootType = keyAttr.ClrRootType ?? builder.ClrType;
            var clrPropertyNames = GetClrPath(keyAttr, clrType, memberName: null);

            builder.AddKeyOrAppendPath(keyAttr.ApiName, clrRootType, clrPropertyNames);
        }
    }

    /// <inheritdoc />
    public void ApplyScalarTypeAnnotations(Type clrType, ApiScalarTypeBuilder builder)
    {
        var attr = clrType.GetCustomAttribute<ApiScalarAttribute>(inherit: false);
        if (attr?.ApiName != null)
        {
            builder.SetApiNameDataAnnotation(attr.ApiName);
        }
    }

    /// <inheritdoc />
    public void ApplyEnumTypeAnnotations(Type clrType, ApiEnumTypeBuilder builder)
    {
        var attr = clrType.GetCustomAttribute<ApiEnumAttribute>(inherit: false);
        if (attr?.ApiName != null)
        {
            builder.SetApiNameDataAnnotation(attr.ApiName);
        }
    }

    /// <inheritdoc />
    public void ApplyEnumValueAnnotations
    (
        FieldInfo clrField,
        ApiEnumTypeBuilder enumTypeBuilder,
        ApiEnumValueBuilder enumValueBuilder
    )
    {
        var attr = clrField.GetCustomAttribute<ApiEnumValueAttribute>(inherit: false);
        if (attr?.ApiName != null)
        {
            enumValueBuilder.SetApiNameDataAnnotation(attr.ApiName);
        }
    }
    #endregion

    #region IApiAnnotationReader — Property Level
    /// <inheritdoc />
    public void ApplyPropertyAnnotations
    (
        MemberInfo clrMember,
        ClrMemberKind clrMemberKind,
        MemberNullableInfo clrNullabilityInfo,
        ApiObjectTypeBuilder objectTypeBuilder,
        ApiPropertyBuilder propertyBuilder
    )
    {
        var propAttr = clrMember.GetCustomAttribute<ApiPropertyAttribute>(inherit: true);
        if (propAttr != null)
        {
            if (propAttr.ApiName != null)
            {
                propertyBuilder.SetApiNameDataAnnotation(propAttr.ApiName);
            }

            if (propAttr.IsRequired)
            {
                propertyBuilder.SetModifiersDataAnnotation(static m => m.Required());
            }
            else if (propAttr.IsOptional)
            {
                propertyBuilder.SetModifiersDataAnnotation(static m => m.Optional());
            }
        }

        // [ApiKey] — register key paths on the owning object type builder.
        var keyAttrs = clrMember.GetCustomAttributes<ApiKeyAttribute>(inherit: true)
            .OrderBy(a => a.Order)
            .ToList();

        if (keyAttrs.Count > 0)
        {
            foreach (var keyAttr in keyAttrs)
            {
                var clrRootType = keyAttr.ClrRootType ?? objectTypeBuilder.ClrType;
                var clrPropertyNames = GetClrPath
                (
                    keyAttr,
                    objectTypeBuilder.ClrType,
                    clrMember.Name
                );

                objectTypeBuilder.AddKeyOrAppendPath
                (
                    keyAttr.ApiName,
                    clrRootType,
                    clrPropertyNames
                );
            }
        }
    }
    #endregion

    #region IApiAnnotationReader — Relationship
    /// <inheritdoc />
    public IReadOnlyList<(string ApiName, Action<ApiRelationshipOneToManyBuilder> Configure)>
        ReadOneToManyRelationships(Type clrType)
    {
        var results = new List<(string, Action<ApiRelationshipOneToManyBuilder>)>();

        // Navigation-property-level attributes.
        foreach (var member in GetPublicInstanceMembers(clrType))
        {
            var attr = member.GetCustomAttribute<ApiRelationshipAttribute>(inherit: true);
            if (attr == null || attr.Kind != ApiRelationshipKind.OneToMany)
            {
                continue;
            }

            var memberType = GetMemberType(member);
            var dependentType = GetCollectionElementType(memberType) ?? memberType;
            var foreignKey = attr.ForeignKey;
            var deleteBehavior = attr.DeleteBehavior;
            var apiName = attr.ApiName;

            results.Add((apiName, builder =>
            {
                builder
                    .WithDeleteBehavior(deleteBehavior)
                    .From(clrType)
                    .To(dependentType, foreignKey != null
                        ? d => d.WithForeignKey(b => b.AddPath(dependentType, foreignKey))
                        : null);
            }
            ));
        }

        // Type-level attributes.
        foreach
        (
            var attr in clrType.GetCustomAttributes<ApiRelationshipDefinitionAttribute>
            (
                inherit: false
            )
        )
        {
            if (attr.Kind != ApiRelationshipKind.OneToMany)
            {
                continue;
            }

            var principalType = attr.PrincipalType;
            var dependentType = attr.DependentType;
            var foreignKey = attr.ForeignKey;
            var deleteBehavior = attr.DeleteBehavior;
            var apiName = attr.ApiName;

            results.Add((apiName, builder =>
            {
                builder
                    .WithDeleteBehavior(deleteBehavior)
                    .From(principalType)
                    .To(dependentType, foreignKey != null
                        ? d => d.WithForeignKey(b => b.AddPath(dependentType, foreignKey))
                        : null);
            }
            ));
        }

        return results;
    }

    /// <inheritdoc />
    public IReadOnlyList<(string ApiName, Action<ApiRelationshipOneToOneBuilder> Configure)>
        ReadOneToOneRelationships(Type clrType)
    {
        var results = new List<(string, Action<ApiRelationshipOneToOneBuilder>)>();

        // Navigation-property-level attributes.
        foreach (var member in GetPublicInstanceMembers(clrType))
        {
            var attr = member.GetCustomAttribute<ApiRelationshipAttribute>(inherit: true);
            if (attr == null || attr.Kind != ApiRelationshipKind.OneToOne)
            {
                continue;
            }

            var dependentType = GetMemberType(member);
            var foreignKey = attr.ForeignKey;
            var deleteBehavior = attr.DeleteBehavior;
            var apiName = attr.ApiName;

            results.Add((apiName, builder =>
            {
                builder
                    .WithDeleteBehavior(deleteBehavior)
                    .From(clrType)
                    .To(dependentType, foreignKey != null
                        ? d => d.WithForeignKey(b => b.AddPath(dependentType, foreignKey))
                        : null);
            }
            ));
        }

        // Type-level attributes.
        foreach
        (
            var attr in clrType.GetCustomAttributes<ApiRelationshipDefinitionAttribute>
            (
                inherit: false
            )
        )
        {
            if (attr.Kind != ApiRelationshipKind.OneToOne)
            {
                continue;
            }

            var principalType = attr.PrincipalType;
            var dependentType = attr.DependentType;
            var foreignKey = attr.ForeignKey;
            var deleteBehavior = attr.DeleteBehavior;
            var apiName = attr.ApiName;

            results.Add((apiName, builder =>
            {
                builder
                    .WithDeleteBehavior(deleteBehavior)
                    .From(principalType)
                    .To(dependentType, foreignKey != null
                        ? d => d.WithForeignKey(b => b.AddPath(dependentType, foreignKey))
                        : null);
            }
            ));
        }

        return results;
    }

    /// <inheritdoc />
    public IReadOnlyList<(string ApiName, Action<ApiRelationshipManyToManyBuilder> Configure)>
        ReadManyToManyRelationships(Type clrType)
    {
        var results = new List<(string, Action<ApiRelationshipManyToManyBuilder>)>();

        // Navigation-property-level attributes.
        foreach (var member in GetPublicInstanceMembers(clrType))
        {
            var attr = member.GetCustomAttribute<ApiManyToManyRelationshipAttribute>(inherit: true);
            if (attr == null)
            {
                continue;
            }

            var principalTypeA = clrType;
            var principalTypeB = attr.OtherPrincipalType;
            var associationType = attr.AssociationType;
            var fkA = attr.ForeignKeyA;
            var fkB = attr.ForeignKeyB;
            var apiName = attr.ApiName;

            results.Add((apiName, builder =>
            {
                builder
                    .Between(principalTypeA)
                    .And(principalTypeB)
                    .WithAssociation(associationType, a =>
                    {
                        if (fkA != null)
                        {
                            a.WithForeignKeyA(b => b.AddPath(associationType, fkA));
                        }

                        if (fkB != null)
                        {
                            a.WithForeignKeyB(b => b.AddPath(associationType, fkB));
                        }
                    });
            }
            ));
        }

        // Type-level attributes.
        foreach
        (
            var attr in clrType.GetCustomAttributes<ApiManyToManyRelationshipDefinitionAttribute>
            (
                inherit: false
            )
        )
        {
            var principalTypeA = attr.PrincipalTypeA;
            var principalTypeB = attr.PrincipalTypeB;
            var associationType = attr.AssociationType;
            var fkA = attr.ForeignKeyA;
            var fkB = attr.ForeignKeyB;
            var apiName = attr.ApiName;

            results.Add((apiName, builder =>
            {
                builder
                    .Between(principalTypeA)
                    .And(principalTypeB)
                    .WithAssociation(associationType, a =>
                    {
                        if (fkA != null)
                        {
                            a.WithForeignKeyA(b => b.AddPath(associationType, fkA));
                        }

                        if (fkB != null)
                        {
                            a.WithForeignKeyB(b => b.AddPath(associationType, fkB));
                        }
                    });
            }
            ));
        }

        return results;
    }
    #endregion

    #region Implementation Methods
    private static IReadOnlyList<string> GetClrPath
    (
        ApiKeyAttribute keyAttribute,
        Type clrType,
        string? memberName
    )
    {
        if (keyAttribute.ClrPath == null)
        {
            if (memberName != null)
            {
                return [memberName];
            }

            throw new ApiSchemaConfigurationException
            (
                $"The {nameof(ApiKeyAttribute)} on CLR type '{clrType.FullName}' must specify " +
                $"{nameof(ApiKeyAttribute.ClrPath)} when applied at type level."
            );
        }

        var clrPropertyNames = keyAttribute.ClrPath.Split('.');
        if (clrPropertyNames.Length == 0 || clrPropertyNames.Any(string.IsNullOrWhiteSpace))
        {
            throw new ApiSchemaConfigurationException
            (
                $"The {nameof(ApiKeyAttribute)} on CLR type '{clrType.FullName}' has an invalid " +
                $"{nameof(ApiKeyAttribute.ClrPath)} value '{keyAttribute.ClrPath}'. CLR paths must " +
                "contain one or more non-empty dot-delimited member names."
            );
        }

        return clrPropertyNames;
    }

    private static IEnumerable<MemberInfo> GetPublicInstanceMembers(Type clrType)
    {
        return clrType
            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m is PropertyInfo or FieldInfo);
    }

    private static Type GetMemberType(MemberInfo member) => member switch
    {
        PropertyInfo p => p.PropertyType,
        FieldInfo f => f.FieldType,
        _ => typeof(object),
    };

    private static Type? GetCollectionElementType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        foreach (var iface in type.GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return iface.GetGenericArguments()[0];
            }
        }

        return null;
    }
    #endregion
}
