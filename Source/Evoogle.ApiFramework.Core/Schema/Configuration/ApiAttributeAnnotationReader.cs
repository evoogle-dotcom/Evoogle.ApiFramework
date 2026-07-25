// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;
using Evoogle.ApiFramework.Schema.Annotations;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Default <see cref="IApiAnnotationReader"/> implementation that reads the framework's
///     built-in attribute set (<see cref="ApiObjectTypeAttribute"/>, <see cref="ApiPropertyAttribute"/>,
///     <see cref="ApiKeyAttribute"/>, <see cref="ApiIgnoreAttribute"/>, and the relationship attributes).
/// </summary>
public sealed class ApiAttributeAnnotationReader : IApiAnnotationReader
{
    #region IApiAnnotationReader — type-level
    /// <inheritdoc />
    public void ApplyObjectTypeAnnotations(Type clrType, ApiObjectTypeBuilder builder)
    {
        var attr = clrType.GetCustomAttribute<ApiObjectTypeAttribute>(inherit: false);
        if (attr?.Name != null)
        {
            builder.SetApiNameDataAnnotation(attr.Name);
        }
    }

    /// <inheritdoc />
    public void ApplyScalarTypeAnnotations(Type clrType, ApiScalarTypeBuilder builder)
    {
        var attr = clrType.GetCustomAttribute<ApiScalarTypeAttribute>(inherit: false);
        if (attr?.Name != null)
        {
            builder.SetApiNameDataAnnotation(attr.Name);
        }
    }

    /// <inheritdoc />
    public void ApplyEnumTypeAnnotations(Type clrType, ApiEnumTypeBuilder builder)
    {
        var attr = clrType.GetCustomAttribute<ApiEnumTypeAttribute>(inherit: false);
        if (attr?.Name != null)
        {
            builder.SetApiNameDataAnnotation(attr.Name);
        }
    }
    #endregion

    #region IApiAnnotationReader — property-level
    /// <inheritdoc />
    public void ApplyPropertyAnnotations(
        MemberInfo clrMember,
        ClrMemberKind clrMemberKind,
        MemberNullableInfo nullabilityInfo,
        ApiPropertyBuilder builder,
        ApiObjectTypeBuilder objectTypeBuilder)
    {
        var propAttr = clrMember.GetCustomAttribute<ApiPropertyAttribute>(inherit: true);
        if (propAttr != null)
        {
            if (propAttr.Name != null)
            {
                builder.SetApiNameDataAnnotation(propAttr.Name);
            }

            if (propAttr.IsRequired)
            {
                builder.SetModifiersDataAnnotation(static m => m.Required());
            }
            else if (propAttr.IsOptional)
            {
                builder.SetModifiersDataAnnotation(static m => m.Optional());
            }
        }

        // [ApiKey] — register key paths on the owning object type builder.
        var keyAttrs = clrMember.GetCustomAttributes<ApiKeyAttribute>(inherit: true)
            .OrderBy(a => a.Order)
            .ToList();

        if (keyAttrs.Count > 0)
        {
            var clrName = clrMember.Name;
            var declaringClrType = objectTypeBuilder.ClrType;

            foreach (var keyAttr in keyAttrs)
            {
                // Add the path to an existing or newly created key type builder.
                objectTypeBuilder.AddKeyOrAppendPath(keyAttr.KeyName, declaringClrType, clrName);
            }
        }
    }
    #endregion

    #region IApiAnnotationReader — relationship
    /// <inheritdoc />
    public IReadOnlyList<(string Name, Action<ApiRelationshipOneToManyBuilder> Configure)>
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
            var name = attr.Name;

            results.Add((name, builder =>
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
        foreach (var attr in clrType.GetCustomAttributes<ApiRelationshipTypeAttribute>(inherit: false))
        {
            if (attr.Kind != ApiRelationshipKind.OneToMany)
            {
                continue;
            }

            var principalType = attr.PrincipalType;
            var dependentType = attr.DependentType;
            var foreignKey = attr.ForeignKey;
            var deleteBehavior = attr.DeleteBehavior;
            var name = attr.Name;

            results.Add((name, builder =>
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
    public IReadOnlyList<(string Name, Action<ApiRelationshipOneToOneBuilder> Configure)>
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
            var name = attr.Name;

            results.Add((name, builder =>
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
        foreach (var attr in clrType.GetCustomAttributes<ApiRelationshipTypeAttribute>(inherit: false))
        {
            if (attr.Kind != ApiRelationshipKind.OneToOne)
            {
                continue;
            }

            var principalType = attr.PrincipalType;
            var dependentType = attr.DependentType;
            var foreignKey = attr.ForeignKey;
            var deleteBehavior = attr.DeleteBehavior;
            var name = attr.Name;

            results.Add((name, builder =>
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
    public IReadOnlyList<(string Name, Action<ApiRelationshipManyToManyBuilder> Configure)>
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
            var name = attr.Name;

            results.Add((name, builder =>
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
        foreach (var attr in clrType.GetCustomAttributes<ApiManyToManyRelationshipTypeAttribute>(inherit: false))
        {
            var principalTypeA = attr.PrincipalTypeA;
            var principalTypeB = attr.PrincipalTypeB;
            var associationType = attr.AssociationType;
            var fkA = attr.ForeignKeyA;
            var fkB = attr.ForeignKeyB;
            var name = attr.Name;

            results.Add((name, builder =>
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
