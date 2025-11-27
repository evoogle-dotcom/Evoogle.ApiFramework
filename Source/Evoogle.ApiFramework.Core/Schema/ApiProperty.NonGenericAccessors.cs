// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;
using System.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Partial class containing non-generic CLR accessor compilation methods.
///     These methods build lambda expressions for object-based property/field access.
/// </summary>
public sealed partial class ApiProperty
{
    #region Non-Generic Accessor Methods
    /// <summary>
    ///     Builds a compiled non-generic property getter that returns values as <see cref="object"/>.
    ///     Uses direct conversion without TypeCoercion since all types are assignable to object.
    /// </summary>
    /// <param name="objectType">The CLR type that owns the property.</param>
    /// <param name="propertyInfo">The property metadata.</param>
    /// <returns>A compiled getter delegate, or null if the property is not readable.</returns>
    private static Func<object, ApiSchemaContext, object?>? BuildNonGenericClrPropertyGetter(Type objectType, PropertyInfo propertyInfo)
    {
        // Build property getter if readable: (object obj, ApiSchemaContext context) => ((OwningType)obj).PropertyName
        // All types are assignable to object, so direct conversion is sufficient
        if (!propertyInfo.CanRead)
        {
            return null;
        }

        var objectParameterExpression = Expression.Parameter(typeof(object), "obj");
        var contextParameterExpression = Expression.Parameter(typeof(ApiSchemaContext), "context");
        var castObjectExpression = Expression.Convert(objectParameterExpression, objectType);
        var propertyAccessExpression = Expression.Property(castObjectExpression, propertyInfo);

        // Direct path: All types are assignable to object (reference types upcast, value types box)
        // Direct conversion is always sufficient - no TypeCoercion needed
        var valueAsObjectExpression = (Expression)propertyAccessExpression;
        if (propertyInfo.PropertyType != typeof(object))
        {
            valueAsObjectExpression = Expression.Convert(propertyAccessExpression, typeof(object));
        }

        var directGetterLambdaExpression = Expression.Lambda<Func<object, ApiSchemaContext, object?>>(valueAsObjectExpression, objectParameterExpression, contextParameterExpression);
        var directGetterDelegate = directGetterLambdaExpression.Compile();
        return directGetterDelegate;
    }

    /// <summary>
    ///     Builds a compiled non-generic property setter that accepts values as <see cref="object"/>.
    ///     Uses TypeCoercion to convert object to the target property type (handles unboxing, downcasting, etc.).
    /// </summary>
    /// <param name="objectType">The CLR type that owns the property.</param>
    /// <param name="propertyInfo">The property metadata.</param>
    /// <returns>A compiled setter delegate, or null if the property is not writable.</returns>
    private static Action<object, ApiSchemaContext, object?>? BuildNonGenericClrPropertySetter(Type objectType, PropertyInfo propertyInfo)
    {
        // Build property setter if writable: (object obj, ApiSchemaContext context, object value) => ((OwningType)obj).PropertyName = context.TypeCoercion.Coerce<object, PropertyType>(value, context.TypeCoercionContext)
        if (!propertyInfo.CanWrite)
        {
            return null;
        }

        var objectParameterExpression = Expression.Parameter(typeof(object), "obj");
        var contextParameterExpression = Expression.Parameter(typeof(ApiSchemaContext), "context");
        var valueParameterExpression = Expression.Parameter(typeof(object), "value");
        var castObjectExpression = Expression.Convert(objectParameterExpression, objectType);
        var propertyAccessExpression = Expression.Property(castObjectExpression, propertyInfo);

        // Direct path: If property type is assignable from object (only if property type IS object)
        if (propertyInfo.PropertyType == typeof(object))
        {
            var directAssignExpression = Expression.Assign(propertyAccessExpression, valueParameterExpression);
            var directSetterLambdaExpression = Expression.Lambda<Action<object, ApiSchemaContext, object?>>(directAssignExpression, objectParameterExpression, contextParameterExpression, valueParameterExpression);
            var directSetterDelegate = directSetterLambdaExpression.Compile();
            return directSetterDelegate;
        }

        // Coercion path: Use TypeCoercion to convert object → specific type
        // This handles unboxing, downcasting, and custom conversions
        var typeCoercionPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercion));
        var typeCoercionContextPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercionContext));
        var targetPropertyTypeConstantExpression = Expression.Constant(propertyInfo.PropertyType, typeof(Type));

        var coerceMethodCallExpression = Expression.Call(
            typeCoercionPropertyExpression,
            NonGenericCoerceMethod,
            valueParameterExpression,
            targetPropertyTypeConstantExpression,
            typeCoercionContextPropertyExpression);

        // Convert coerce result back to property type
        var coercedValueExpression = Expression.Convert(coerceMethodCallExpression, propertyInfo.PropertyType);

        var coerceAssignExpression = Expression.Assign(propertyAccessExpression, coercedValueExpression);
        var coerceSetterLambdaExpression = Expression.Lambda<Action<object, ApiSchemaContext, object?>>(coerceAssignExpression, objectParameterExpression, contextParameterExpression, valueParameterExpression);
        var coerceSetterDelegate = coerceSetterLambdaExpression.Compile();
        return coerceSetterDelegate;
    }

    /// <summary>
    ///     Builds a compiled non-generic field getter that returns values as <see cref="object"/>.
    ///     Uses direct conversion without TypeCoercion since all types are assignable to object.
    /// </summary>
    /// <param name="objectType">The CLR type that owns the field.</param>
    /// <param name="fieldInfo">The field metadata.</param>
    /// <returns>A compiled getter delegate.</returns>
    private static Func<object, ApiSchemaContext, object?>? BuildNonGenericClrFieldGetter(Type objectType, FieldInfo fieldInfo)
    {
        // Build field getter: (object obj, ApiSchemaContext context) => ((OwningType)obj).FieldName
        // All types are assignable to object, so direct conversion is sufficient
        var objectParameterExpression = Expression.Parameter(typeof(object), "obj");
        var contextParameterExpression = Expression.Parameter(typeof(ApiSchemaContext), "context");
        var castObjectExpression = Expression.Convert(objectParameterExpression, objectType);
        var fieldAccessExpression = Expression.Field(castObjectExpression, fieldInfo);

        // Direct path: All types are assignable to object (reference types upcast, value types box)
        // Direct conversion is always sufficient - no TypeCoercion needed
        var valueAsObjectExpression = (Expression)fieldAccessExpression;
        if (fieldInfo.FieldType != typeof(object))
        {
            valueAsObjectExpression = Expression.Convert(fieldAccessExpression, typeof(object));
        }

        var directGetterLambdaExpression = Expression.Lambda<Func<object, ApiSchemaContext, object?>>(valueAsObjectExpression, objectParameterExpression, contextParameterExpression);
        var directGetterDelegate = directGetterLambdaExpression.Compile();
        return directGetterDelegate;
    }

    /// <summary>
    ///     Builds a compiled non-generic field setter that accepts values as <see cref="object"/>.
    ///     Uses TypeCoercion to convert object to the target field type (handles unboxing, downcasting, etc.).
    /// </summary>
    /// <param name="objectType">The CLR type that owns the field.</param>
    /// <param name="fieldInfo">The field metadata.</param>
    /// <returns>A compiled setter delegate, or null if the field is init-only.</returns>
    private static Action<object, ApiSchemaContext, object?>? BuildNonGenericClrFieldSetter(Type objectType, FieldInfo fieldInfo)
    {
        // Build field setter if writable: (object obj, ApiSchemaContext context, object value) => ((OwningType)obj).FieldName = context.TypeCoercion.Coerce<object, FieldType>(value, context.TypeCoercionContext)
        if (fieldInfo.IsInitOnly)
        {
            return null;
        }

        var objectParameterExpression = Expression.Parameter(typeof(object), "obj");
        var contextParameterExpression = Expression.Parameter(typeof(ApiSchemaContext), "context");
        var valueParameterExpression = Expression.Parameter(typeof(object), "value");
        var castObjectExpression = Expression.Convert(objectParameterExpression, objectType);
        var fieldAccessExpression = Expression.Field(castObjectExpression, fieldInfo);

        // Direct path: If field type is assignable from object (only if field type IS object)
        if (fieldInfo.FieldType == typeof(object))
        {
            var directAssignExpression = Expression.Assign(fieldAccessExpression, valueParameterExpression);
            var directSetterLambdaExpression = Expression.Lambda<Action<object, ApiSchemaContext, object?>>(directAssignExpression, objectParameterExpression, contextParameterExpression, valueParameterExpression);
            var directSetterDelegate = directSetterLambdaExpression.Compile();
            return directSetterDelegate;
        }

        // Coercion path: Use TypeCoercion to convert object → specific type
        // This handles unboxing, downcasting, and custom conversions
        var typeCoercionPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercion));
        var typeCoercionContextPropertyExpression = Expression.Property(contextParameterExpression, nameof(ApiSchemaContext.TypeCoercionContext));
        var targetFieldTypeConstantExpression = Expression.Constant(fieldInfo.FieldType, typeof(Type));

        var coerceMethodCallExpression = Expression.Call(
            typeCoercionPropertyExpression,
            NonGenericCoerceMethod,
            valueParameterExpression,
            targetFieldTypeConstantExpression,
            typeCoercionContextPropertyExpression);

        // Convert coerce result back to field type
        var coercedValueExpression = Expression.Convert(coerceMethodCallExpression, fieldInfo.FieldType);

        var coerceAssignExpression = Expression.Assign(fieldAccessExpression, coercedValueExpression);
        var coerceSetterLambdaExpression = Expression.Lambda<Action<object, ApiSchemaContext, object?>>(coerceAssignExpression, objectParameterExpression, contextParameterExpression, valueParameterExpression);
        var coerceSetterDelegate = coerceSetterLambdaExpression.Compile();
        return coerceSetterDelegate;
    }
    #endregion
}
