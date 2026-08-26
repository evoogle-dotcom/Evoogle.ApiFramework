// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Annotations;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>
///     Default annotation reader for the framework's built-in attribute set.
/// </summary>
public sealed class ApiAttributeAnnotationReader :
    IApiTypeAnnotationReader,
    IApiPropertyAnnotationReader,
    IApiEnumValueAnnotationReader,
    IApiKeyAnnotationReader,
    IApiRelationshipAnnotationReader
{
    #region IApiTypeAnnotationReader Methods
    /// <inheritdoc/>
    public IReadOnlyList<ApiTypeAnnotationResult> ReadObjectTypeAnnotations(Type clrType)
    {
        var attr = clrType.GetCustomAttribute<ApiObjectAttribute>(inherit: false);
        return attr?.ApiName == null ? [] : [new(attr.ApiName)];
    }

    /// <inheritdoc/>
    public IReadOnlyList<ApiTypeAnnotationResult> ReadScalarTypeAnnotations(Type clrType)
    {
        var attr = clrType.GetCustomAttribute<ApiScalarAttribute>(inherit: false);
        return attr?.ApiName == null ? [] : [new(attr.ApiName)];
    }

    /// <inheritdoc/>
    public IReadOnlyList<ApiTypeAnnotationResult> ReadEnumTypeAnnotations(Type clrType)
    {
        var attr = clrType.GetCustomAttribute<ApiEnumAttribute>(inherit: false);
        return attr?.ApiName == null ? [] : [new(attr.ApiName)];
    }
    #endregion

    #region IApiEnumValueAnnotationReader Methods
    /// <inheritdoc/>
    public IReadOnlyList<ApiEnumValueAnnotationResult> ReadEnumValueAnnotations(FieldInfo clrField)
    {
        var attr = clrField.GetCustomAttribute<ApiEnumValueAttribute>(inherit: false);
        return attr?.ApiName == null ? [] : [new(attr.ApiName)];
    }
    #endregion

    #region IApiPropertyAnnotationReader Methods
    /// <inheritdoc/>
    public IReadOnlyList<ApiPropertyAnnotationResult> ReadPropertyAnnotations
    (
        MemberInfo clrMember,
        ClrMemberKind clrMemberKind,
        MemberNullableInfo clrNullabilityInfo
    )
    {
        var attr = clrMember.GetCustomAttribute<ApiPropertyAttribute>(inherit: true);
        if (attr == null)
        {
            return [];
        }

        var modifiers = attr.IsRequired
            ? ApiTypeModifiers.Required
            : attr.IsOptional
                ? ApiTypeModifiers.None
                : (ApiTypeModifiers?)null;

        return [new(attr.ApiName, modifiers)];
    }
    #endregion

    #region IApiKeyAnnotationReader Methods
    /// <inheritdoc/>
    public IReadOnlyList<ApiKeyAnnotationResult> ReadKeyAnnotations(Type clrType)
    {
        var results = new List<ApiKeyAnnotationResult>();

        foreach (var keyAttribute in clrType.GetCustomAttributes<ApiKeyAttribute>(inherit: true))
        {
            var clrRootType = keyAttribute.ClrRootType ?? clrType;
            results.Add
            (
                new
                (
                    keyAttribute.ApiName,
                    keyAttribute.Order,
                    clrRootType,
                    GetClrPath(keyAttribute, clrType, memberName: null)
                )
            );
        }

        foreach (var member in GetPublicInstanceMembers(clrType))
        {
            foreach (var keyAttribute in member.GetCustomAttributes<ApiKeyAttribute>(inherit: true))
            {
                var clrRootType = keyAttribute.ClrRootType ?? clrType;
                results.Add
                (
                    new
                    (
                        keyAttribute.ApiName,
                        keyAttribute.Order,
                        clrRootType,
                        GetClrPath(keyAttribute, clrType, member.Name)
                    )
                );
            }
        }

        return results;
    }
    #endregion

    #region IApiRelationshipAnnotationReader Methods
    /// <inheritdoc/>
    public IReadOnlyList<ApiOneToManyRelationshipAnnotationResult> ReadOneToManyRelationships(Type clrType)
    {
        var results = new List<ApiOneToManyRelationshipAnnotationResult>();

        foreach (var member in GetPublicInstanceMembers(clrType))
        {
            var attr = member.GetCustomAttribute<ApiRelationshipAttribute>(inherit: true);
            if (attr == null || attr.Kind != ApiRelationshipKind.OneToMany)
            {
                continue;
            }

            var memberType = GetMemberType(member);
            var dependentType = GetCollectionElementType(memberType) ?? memberType;
            results.Add
            (
                new
                (
                    attr.ApiName,
                    clrType,
                    dependentType,
                    attr.ForeignKey,
                    attr.DeleteBehavior
                )
            );
        }

        foreach (var attr in clrType.GetCustomAttributes<ApiRelationshipDefinitionAttribute>(inherit: false))
        {
            if (attr.Kind == ApiRelationshipKind.OneToMany)
            {
                results.Add
                (
                    new
                    (
                        attr.ApiName,
                        attr.PrincipalType,
                        attr.DependentType,
                        attr.ForeignKey,
                        attr.DeleteBehavior
                    )
                );
            }
        }

        return results;
    }

    /// <inheritdoc/>
    public IReadOnlyList<ApiOneToOneRelationshipAnnotationResult> ReadOneToOneRelationships(Type clrType)
    {
        var results = new List<ApiOneToOneRelationshipAnnotationResult>();

        foreach (var member in GetPublicInstanceMembers(clrType))
        {
            var attr = member.GetCustomAttribute<ApiRelationshipAttribute>(inherit: true);
            if (attr == null || attr.Kind != ApiRelationshipKind.OneToOne)
            {
                continue;
            }

            results.Add
            (
                new
                (
                    attr.ApiName,
                    clrType,
                    GetMemberType(member),
                    attr.ForeignKey,
                    attr.DeleteBehavior
                )
            );
        }

        foreach (var attr in clrType.GetCustomAttributes<ApiRelationshipDefinitionAttribute>(inherit: false))
        {
            if (attr.Kind == ApiRelationshipKind.OneToOne)
            {
                results.Add
                (
                    new
                    (
                        attr.ApiName,
                        attr.PrincipalType,
                        attr.DependentType,
                        attr.ForeignKey,
                        attr.DeleteBehavior
                    )
                );
            }
        }

        return results;
    }

    /// <inheritdoc/>
    public IReadOnlyList<ApiManyToManyRelationshipAnnotationResult> ReadManyToManyRelationships(Type clrType)
    {
        var results = new List<ApiManyToManyRelationshipAnnotationResult>();

        foreach (var member in GetPublicInstanceMembers(clrType))
        {
            var attr = member.GetCustomAttribute<ApiManyToManyRelationshipAttribute>(inherit: true);
            if (attr == null)
            {
                continue;
            }

            results.Add
            (
                new
                (
                    attr.ApiName,
                    clrType,
                    attr.OtherPrincipalType,
                    attr.AssociationType,
                    attr.ForeignKeyA,
                    attr.ForeignKeyB
                )
            );
        }

        foreach (var attr in clrType.GetCustomAttributes<ApiManyToManyRelationshipDefinitionAttribute>(inherit: false))
        {
            results.Add
            (
                new
                (
                    attr.ApiName,
                    attr.PrincipalTypeA,
                    attr.PrincipalTypeB,
                    attr.AssociationType,
                    attr.ForeignKeyA,
                    attr.ForeignKeyB
                )
            );
        }

        return results;
    }
    #endregion

    #region Private Methods
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

            throw new InvalidOperationException
            (
                $"The {nameof(ApiKeyAttribute)} on CLR type '{clrType.FullName}' must specify " +
                $"{nameof(ApiKeyAttribute.ClrPath)} when applied at type level."
            );
        }

        var clrPropertyNames = keyAttribute.ClrPath.Split('.');
        if (clrPropertyNames.Length == 0 || clrPropertyNames.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationException
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
        PropertyInfo property => property.PropertyType,
        FieldInfo field => field.FieldType,
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
