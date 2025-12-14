// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;
using System.Reflection;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Partial class containing generic CLR accessor compilation methods.
///     These methods build type-safe lambda expressions for property/field access with minimal boxing.
/// </summary>
public sealed partial class ApiProperty
{
    #region Generic Accessor Methods
    /// <summary>
    ///     Builds a compiled type-safe property/field getter for the specified object and value types.
    ///     Uses direct conversion when types are assignable, otherwise uses TypeCoercion.
    /// </summary>
    /// <typeparam name="TObject">The type of the object containing the member.</typeparam>
    /// <typeparam name="TValue">The expected return type of the member value.</typeparam>
    /// <param name="objectType">The actual CLR type (may differ from TObject for inheritance scenarios).</param>
    /// <param name="memberName">The name of the property or field to access.</param>
    /// <returns>A cache value containing the compiled getter delegate, or null if the member cannot be accessed.</returns>
    private static ClrGetterCacheValue<TObject, TValue> BuildGenericClrGetter<TObject, TValue>(Type objectType, string memberName)
    {
        if (!TryResolveMember(objectType, memberName, forWrite: false, out var memberInfo, out var memberType))
        {
            return new ClrGetterCacheValue<TObject, TValue>(null);
        }

        // Parameters: (TObject obj, ApiSchemaContext context)
        var objectParameterExpression = Expression.Parameter(typeof(TObject), "obj");
        var contextParameterExpression = Expression.Parameter(typeof(ApiSchemaContext), "context");

        var castObjectExpression = MakeObjectExpression<TObject>(objectParameterExpression, objectType);

        var memberAccessExpression = memberInfo switch
        {
            PropertyInfo pi => Expression.Property(castObjectExpression, pi),
            FieldInfo fi => Expression.Field(castObjectExpression, fi),
            _ => null
        };

        if (memberAccessExpression is null)
        {
            return new ClrGetterCacheValue<TObject, TValue>(null);
        }

        // If the member type is already assignable to TValue, just use it directly
        if (typeof(TValue).IsAssignableFrom(memberType))
        {
            Expression convertedMemberValueExpression = memberAccessExpression;
            if (memberAccessExpression.Type != typeof(TValue))
            {
                convertedMemberValueExpression = Expression.Convert(memberAccessExpression, typeof(TValue));
            }

            var directGetterLambdaExpression = Expression.Lambda<Func<TObject, ApiSchemaContext, TValue?>>(convertedMemberValueExpression, objectParameterExpression, contextParameterExpression);
            var directGetterDelegate = directGetterLambdaExpression.Compile();
            return new ClrGetterCacheValue<TObject, TValue>(directGetterDelegate);
        }

        // Otherwise, we need to use TypeCoercion to coerce the value
        // context.TypeCoercion.Coerce<memberType, TValue>(memberValue, context.TypeCoercionContext)
        var typeCoercionPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercion));
        var typeCoercionContextPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercionContext));

        // Use generic Coerce<,> overload for type-safe coercion with compile-time types
        var genericCoerceMethod = MakeGenericCoerceMethod(memberType, typeof(TValue));

        var coerceMethodCallExpression = Expression.Call(
            typeCoercionPropertyExpression,
            genericCoerceMethod,
            memberAccessExpression,
            typeCoercionContextPropertyExpression);

        var coerceGetterLambdaExpression = Expression.Lambda<Func<TObject, ApiSchemaContext, TValue?>>(coerceMethodCallExpression, objectParameterExpression, contextParameterExpression);
        var coerceGetterDelegate = coerceGetterLambdaExpression.Compile();

        return new ClrGetterCacheValue<TObject, TValue>(coerceGetterDelegate);
    }

    /// <summary>
    ///     Builds a compiled type-safe property/field setter for the specified object and value types.
    ///     Uses direct assignment when types are assignable, otherwise uses TypeCoercion.
    /// </summary>
    /// <typeparam name="TObject">The type of the object containing the member.</typeparam>
    /// <typeparam name="TValue">The type of the value to assign.</typeparam>
    /// <param name="objectType">The actual CLR type (may differ from TObject for inheritance scenarios).</param>
    /// <param name="memberName">The name of the property or field to modify.</param>
    /// <returns>A cache value containing the compiled setter delegate, or null if the member cannot be modified.</returns>
    private static ClrSetterCacheValue<TObject, TValue> BuildGenericClrSetter<TObject, TValue>(Type objectType, string memberName)
    {
        if (!TryResolveMember(objectType, memberName, forWrite: true, out var memberInfo, out var memberType))
        {
            return new ClrSetterCacheValue<TObject, TValue>(null);
        }

        // Parameters: (TObject obj, ApiSchemaContext context, TValue value)
        var objectParameterExpression = Expression.Parameter(typeof(TObject), "obj");
        var contextParameterExpression = Expression.Parameter(typeof(ApiSchemaContext), "context");
        var valueParameterExpression = Expression.Parameter(typeof(TValue), "value");

        var castObjectExpression = MakeObjectExpression<TObject>(objectParameterExpression, objectType);

        var memberAccessExpression = memberInfo switch
        {
            PropertyInfo pi => Expression.Property(castObjectExpression, pi),
            FieldInfo fi => Expression.Field(castObjectExpression, fi),
            _ => null
        };

        if (memberAccessExpression is null)
        {
            return new ClrSetterCacheValue<TObject, TValue>(null);
        }

        // If TValue is already assignable to the member type, just use it directly
        if (memberType.IsAssignableFrom(typeof(TValue)))
        {
            Expression convertedValueExpression = valueParameterExpression;
            if (valueParameterExpression.Type != memberType)
            {
                convertedValueExpression = Expression.Convert(valueParameterExpression, memberType);
            }

            var directAssignExpression = Expression.Assign(memberAccessExpression, convertedValueExpression);
            var directSetterLambdaExpression = Expression.Lambda<Action<TObject, ApiSchemaContext, TValue?>>(directAssignExpression, objectParameterExpression, contextParameterExpression, valueParameterExpression);
            var directSetterDelegate = directSetterLambdaExpression.Compile();
            return new ClrSetterCacheValue<TObject, TValue>(directSetterDelegate);
        }

        // Otherwise, we need to use TypeCoercion to coerce the value
        // var coercedValue = context.TypeCoercion.Coerce<TValue, MemberType>(value, context.TypeCoercionContext);
        // member = coercedValue;
        var typeCoercionPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercion));
        var typeCoercionContextPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercionContext));

        // Use generic Coerce<,> overload for type-safe coercion with compile-time types
        var genericCoerceMethod = MakeGenericCoerceMethod(typeof(TValue), memberType);

        var coerceMethodCallExpression = Expression.Call(
            typeCoercionPropertyExpression,
            genericCoerceMethod,
            valueParameterExpression,
            typeCoercionContextPropertyExpression);

        var coerceAssignExpression = Expression.Assign(memberAccessExpression, coerceMethodCallExpression);
        var coerceSetterLambdaExpression = Expression.Lambda<Action<TObject, ApiSchemaContext, TValue?>>(coerceAssignExpression, objectParameterExpression, contextParameterExpression, valueParameterExpression);
        var coerceSetterDelegate = coerceSetterLambdaExpression.Compile();

        return new ClrSetterCacheValue<TObject, TValue>(coerceSetterDelegate);
    }

    /// <summary>
    ///     Builds a compiled type-safe property/field setter for struct types passed by reference.
    ///     This ensures mutations affect the original instance rather than a copy.
    /// </summary>
    /// <typeparam name="TObject">The struct type containing the member.</typeparam>
    /// <typeparam name="TValue">The type of the value to assign.</typeparam>
    /// <param name="objectType">The CLR type (must exactly match typeof(TObject) for structs).</param>
    /// <param name="memberName">The name of the property or field to modify.</param>
    /// <returns>A cache value containing the compiled by-ref setter delegate, or null if the member cannot be modified.</returns>
    private static ClrSetterByRefCacheValue<TObject, TValue> BuildGenericClrSetterByRef<TObject, TValue>(Type objectType, string memberName)
        where TObject : struct
    {
        // objectType must be exactly typeof(TObject) for structs
        if (objectType != typeof(TObject))
        {
            return new ClrSetterByRefCacheValue<TObject, TValue>(null);
        }

        if (!TryResolveMember(objectType, memberName, forWrite: true, out var memberInfo, out var memberType))
        {
            return new ClrSetterByRefCacheValue<TObject, TValue>(null);
        }

        // Parameters: (ref TObject obj, ApiSchemaContext context, TValue value)
        var objectByRefParameterExpression = Expression.Parameter(typeof(TObject).MakeByRefType(), "obj");
        var contextParameterExpression = Expression.Parameter(typeof(ApiSchemaContext), "context");
        var valueParameterExpression = Expression.Parameter(typeof(TValue), "value");

        var memberAccessExpression = memberInfo switch
        {
            PropertyInfo pi => Expression.Property(objectByRefParameterExpression, pi),
            FieldInfo fi => Expression.Field(objectByRefParameterExpression, fi),
            _ => null
        };

        if (memberAccessExpression is null)
        {
            return new ClrSetterByRefCacheValue<TObject, TValue>(null);
        }

        // If TValue is already assignable to the member type, just use it directly
        if (memberType.IsAssignableFrom(typeof(TValue)))
        {
            Expression convertedValueExpression = valueParameterExpression;
            if (valueParameterExpression.Type != memberType)
            {
                convertedValueExpression = Expression.Convert(valueParameterExpression, memberType);
            }

            var directAssignExpression = Expression.Assign(memberAccessExpression, convertedValueExpression);
            var directSetterLambdaExpression = Expression.Lambda<ClrByRefAction<TObject, TValue?>>(directAssignExpression, objectByRefParameterExpression, contextParameterExpression, valueParameterExpression);
            var directSetterDelegate = directSetterLambdaExpression.Compile();
            return new ClrSetterByRefCacheValue<TObject, TValue>(directSetterDelegate);
        }

        // Otherwise, we need to use TypeCoercion to coerce the value
        // var coercedValue = context.TypeCoercion.Coerce<TValue, memberType>(value, context.TypeCoercionContext);
        // member = coercedValue;
        var typeCoercionPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercion));
        var typeCoercionContextPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercionContext));

        // Use generic Coerce<,> overload for type-safe coercion with compile-time types
        var genericCoerceMethod = MakeGenericCoerceMethod(typeof(TValue), memberType);

        var coerceMethodCallExpression = Expression.Call(
            typeCoercionPropertyExpression,
            genericCoerceMethod,
            valueParameterExpression,
            typeCoercionContextPropertyExpression);

        var coerceAssignExpression = Expression.Assign(memberAccessExpression, coerceMethodCallExpression);
        var coerceSetterLambdaExpression = Expression.Lambda<ClrByRefAction<TObject, TValue?>>(coerceAssignExpression, objectByRefParameterExpression, contextParameterExpression, valueParameterExpression);
        var coerceSetterDelegate = coerceSetterLambdaExpression.Compile();

        return new ClrSetterByRefCacheValue<TObject, TValue>(coerceSetterDelegate);
    }

    /// <summary>
    ///     Creates and caches a generic MethodInfo for TypeCoercion.Coerce&lt;TInput, TOutput&gt;.
    /// </summary>
    /// <param name="inputType">The input type for coercion.</param>
    /// <param name="outputType">The output type for coercion.</param>
    /// <returns>A MethodInfo for the generic coercion method.</returns>
    private static MethodInfo MakeGenericCoerceMethod(Type inputType, Type outputType)
    {
        var key = new CoerceMethodCacheKey(inputType, outputType);
        return _coerceMethodCache.GetOrAdd(key, k => GenericCoerceMethodDefinition.MakeGenericMethod(k.ClrInputType, k.ClrOutputType));
    }

    /// <summary>
    ///     Creates an expression that properly casts or uses a parameter expression based on type matching.
    /// </summary>
    /// <typeparam name="TObject">The compile-time type of the parameter.</typeparam>
    /// <param name="parameterExpression">The parameter expression to potentially cast.</param>
    /// <param name="objectType">The runtime type needed.</param>
    /// <returns>Either the original parameter or a cast expression.</returns>
    private static Expression MakeObjectExpression<TObject>(ParameterExpression parameterExpression, Type objectType)
    {
        return objectType == typeof(TObject)
            ? parameterExpression
            : Expression.Convert(parameterExpression, objectType);
    }

    /// <summary>
    ///     Attempts to resolve a property or field by name on the specified type.
    ///     Prefers properties over fields and validates accessibility based on the operation type.
    /// </summary>
    /// <param name="objectType">The CLR type to search for the member.</param>
    /// <param name="memberName">The name of the property or field.</param>
    /// <param name="forWrite">True if the member will be written to; false for read-only access.</param>
    /// <param name="memberInfo">The resolved member info, or default if not found.</param>
    /// <param name="memberType">The type of the resolved member, or default if not found.</param>
    /// <returns>True if the member was successfully resolved; otherwise false.</returns>
    private static bool TryResolveMember(Type objectType, string memberName, bool forWrite, out MemberInfo memberInfo, out Type memberType)
    {
        // Prefer property, then field
        var propertyInfo = TypeReflection.GetProperty(objectType, memberName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is not null)
        {
            // Exclude indexers
            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                memberInfo = default!;
                memberType = default!;
                return false;
            }

            // For write operations, require CanWrite
            if (forWrite && !propertyInfo.CanWrite)
            {
                memberInfo = default!;
                memberType = default!;
                return false;
            }

            memberInfo = propertyInfo;
            memberType = propertyInfo.PropertyType;
            return true;
        }

        var fieldInfo = TypeReflection.GetField(objectType, memberName, BindingFlags.Public | BindingFlags.Instance);
        if (fieldInfo is not null)
        {
            // For write operations, exclude init-only fields
            if (forWrite && fieldInfo.IsInitOnly)
            {
                memberInfo = default!;
                memberType = default!;
                return false;
            }

            memberInfo = fieldInfo;
            memberType = fieldInfo.FieldType;
            return true;
        }

        memberInfo = default!;
        memberType = default!;
        return false;
    }
    #endregion
}
