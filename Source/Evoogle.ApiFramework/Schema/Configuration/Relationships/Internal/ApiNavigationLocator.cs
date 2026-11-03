// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Linq.Expressions;
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;

internal sealed record ApiNavigationLocator(Type SourceType, MemberInfo ClrMember, Type TargetType, bool IsMany)
{
    #region Properties
    internal (Type DeclaringType, MemberInfo Member) Identity => (this.ClrMember.DeclaringType!, this.ClrMember);
    internal ClrMemberReference Reference => new
        (this.ClrMember is PropertyInfo ? ClrMemberKind.Property : ClrMemberKind.Field, this.ClrMember.Name);
    #endregion

    #region Factory Methods
    internal static ApiNavigationLocator FromExpression<TSource>(Expression<Func<TSource, object?>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var body = expression.Body;
        while (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } conversion)
        {
            body = conversion.Operand;
        }
        if (body is not MemberExpression member || member.Expression != expression.Parameters[0])
        {
            throw new ArgumentException(
                "A navigation selector must identify one direct instance property or field.", nameof(expression));
        }
        return FromMember(typeof(TSource), member.Member);
    }

    internal static ApiNavigationLocator FromName(Type clrType, string clrMemberName)
    {
        if (string.IsNullOrWhiteSpace(clrMemberName) || clrMemberName.Contains('.'))
        {
            throw new ApiSchemaConfigurationException("An inverse navigation must name one public instance member.");
        }
        var members = clrType.GetMember(clrMemberName, BindingFlags.Public | BindingFlags.Instance)
            .Where(static member => member is FieldInfo or PropertyInfo).ToArray();
        if (members.Length != 1)
        {
            throw new ApiSchemaConfigurationException(
                $"Navigation '{clrType.FullName}.{clrMemberName}' is unresolved or ambiguous.");
        }
        return FromMember(clrType, members[0]);
    }

    internal static ApiNavigationLocator FromMember(Type clrType, MemberInfo member)
    {
        var memberType = member switch
        {
            PropertyInfo { GetMethod.IsPublic: true, GetMethod.IsStatic: false } property
                            when property.GetIndexParameters().Length == 0 => property.PropertyType,
            FieldInfo { IsPublic: true, IsStatic: false } field => field.FieldType,
            _ => throw new ApiSchemaConfigurationException(
                                "A navigation must be a readable public instance property or field."),
        };
        if (member.DeclaringType is null || !member.DeclaringType.IsAssignableFrom(clrType))
        {
            throw new ApiSchemaConfigurationException(
                "The navigation member does not belong to the declared source CLR type.");
        }
        var collectionInterfaces = memberType.GetInterfaces().Prepend(memberType)
            .Where(static candidate => candidate.IsGenericType &&
                candidate.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(static candidate => candidate.GenericTypeArguments[0]).Distinct().ToArray();
        var isMany = memberType != typeof(string) && (memberType.IsArray || collectionInterfaces.Length > 0);
        if (collectionInterfaces.Length > 1)
        {
            throw new ApiSchemaConfigurationException(
                "A navigation collection must have one unambiguous CLR item type.");
        }
        var targetType = memberType.IsArray ? memberType.GetElementType()!
            : isMany ? collectionInterfaces[0] : Nullable.GetUnderlyingType(memberType) ?? memberType;
        return new(clrType, member, targetType, isMany);
    }
    #endregion
}

internal sealed record ApiRelationshipNavigationBinding(ApiNavigationLocator Locator, int EndIndex);
