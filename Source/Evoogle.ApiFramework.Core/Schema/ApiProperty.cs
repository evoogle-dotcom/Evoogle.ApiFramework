// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extension;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents structural metadata of an API property belonging to an <see cref="ApiObjectType"/>.
///     Each property corresponds to a named data element in an API contract.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiProperty"/> captures type-level structure for fields such as primitives,
///         objects, collections, and complex types. Properties may be referenced by one or more
///         <see cref="ApiRelationship"/> instances to convey semantic meaning (e.g., parent-child, references).
///     </para>
///     <para>
///         Getter and setter accessors are compiled on-demand as lambda expressions and cached per
///         target CLR type using thread-safe <see cref="ConcurrentDictionary{TKey, TValue}"/> instances. This
///         avoids repeated reflection and keeps hot paths allocation-friendly. For reference types, the
///         compiled delegates operate on the runtime type when the declared type is not sealed to honor
///         derived members. For value types (struct targets), a dedicated by-ref setter is provided to
///         ensure mutations affect the original instance.
///     </para>
///     <para>
///         The non-generic Try* methods accept <see cref="object"/> and will box value types by design. Prefer
///         the fully generic overloads to avoid boxing both the target instance and the returned value.
///     </para>
/// </remarks>
/// <param name="apiName">The API name of the property.</param>
/// <param name="apiTypeExpression">The API type expression of the property.</param>
/// <param name="apiTypeModifiers">Modifiers applied to the property (e.g., Required).</param>
/// <param name="clrName">The CLR property name corresponding to this API property.</param>
[JsonConverter(typeof(ApiPropertyJsonConverter))]
public sealed class ApiProperty(string apiName, ApiTypeExpression apiTypeExpression, ApiTypeModifiers apiTypeModifiers, string clrName) : ExtensibleBase
{
    #region Types
    private delegate void ByRefAction<TTarget, in TValue>(ref TTarget target, TValue value);

    private readonly record struct CacheKey(Type TargetType, string MemberName);

    private readonly record struct GetterCacheValue<TObject, TValue>(bool Found, Func<TObject, TValue>? Getter);

    private readonly record struct SetterCacheValue<TObject, TValue>(bool Found, Action<TObject, TValue>? Setter);

    private readonly record struct SetterRefCacheValue<TObject, TValue>(bool Found, ByRefAction<TObject, TValue>? Setter)
        where TObject : struct;

    private static class GetterCache<TObject, TValue>
    {
        public static readonly ConcurrentDictionary<CacheKey, GetterCacheValue<TObject, TValue>> Cache = new();
    }

    private static class SetterCache<TObject, TValue>
    {
        public static readonly ConcurrentDictionary<CacheKey, SetterCacheValue<TObject, TValue>> Cache = new();
    }

    private static class SetterRefCache<TObject, TValue>
        where TObject : struct
    {
        public static readonly ConcurrentDictionary<CacheKey, SetterRefCacheValue<TObject, TValue>> Cache = new();
    }
    #endregion

    #region Properties
    /// <summary>Gets the API name of the property (used in API requests/responses).</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the API type of the property.</summary>
    public ApiType ApiType => this.ApiTypeExpression.ApiType;

    /// <summary>Gets the modifiers applied to this property (e.g., Required).</summary>
    public ApiTypeModifiers ApiTypeModifiers { get; } = apiTypeModifiers;

    /// <summary>Gets the CLR name of the property (matching the C# property name).</summary>
    public string ClrName { get; } = clrName;

    /// <summary>
    ///     Gets the API type expression to the API type of this property.
    ///     May point to a named type or inline type (e.g., collection).
    /// </summary>
    internal ApiTypeExpression ApiTypeExpression { get; } = apiTypeExpression;
    #endregion

    #region ApiProperty Methods
    /// <summary>
    ///     Attempts to read the CLR member value identified by <see cref="ClrName"/> from the specified <paramref name="clrObject"/>.
    ///     This non-generic overload returns the value as <see cref="object"/>, which will box value types.
    /// </summary>
    /// <param name="clrObject">The object instance containing the member.</param>
    /// <param name="clrValue">When this method returns, contains the member value if found; otherwise the default.</param>
    /// <returns><c>true</c> if the value was retrieved successfully; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(object clrObject, out object? clrValue)
    {
        if (clrObject is null)
        {
            clrValue = default;
            return false;
        }

        var ok = this.TryGetValue<object, object?>(clrObject, out var result);
        clrValue = result;
        return ok;
    }

    /// <summary>
    ///     Attempts to read the CLR member value identified by <see cref="ClrName"/> from the specified <paramref name="clrObject"/> without unnecessary boxing when possible.
    ///     Compiles and caches a typed <see cref="Func{T, TResult}"/> delegate per target type.
    /// </summary>
    /// <typeparam name="TObject">The static type of the target instance.</typeparam>
    /// <typeparam name="TValue">The desired return type for the member value.</typeparam>
    /// <param name="clrObject">The object instance containing the member.</param>
    /// <param name="clrValue">When this method returns, contains the member value if found; otherwise the default.</param>
    /// <returns><c>true</c> if the value was retrieved successfully; otherwise, <c>false</c>.</returns>
    public bool TryGetValue<TObject, TValue>(TObject clrObject, out TValue? clrValue)
    {
        if (clrObject is null)
        {
            clrValue = default;
            return false;
        }

        // Use runtime type for non-sealed reference types to respect derived members
        var targetType = typeof(TObject);
        if (!targetType.IsValueType && !targetType.IsSealed)
        {
            targetType = clrObject.GetType();
        }

        var key = new CacheKey(targetType, this.ClrName);
        var cacheValue = GetterCache<TObject, TValue>.Cache.GetOrAdd(key, static k => BuildTypedGetter<TObject, TValue>(k.TargetType, k.MemberName));

        if (!cacheValue.Found || cacheValue.Getter is null)
        {
            clrValue = default;
            return false;
        }

        try
        {
            clrValue = cacheValue.Getter(clrObject);
            return true;
        }
        catch
        {
            clrValue = default;
            return false;
        }
    }

    /// <summary>
    ///     Attempts to set the CLR member identified by <see cref="ClrName"/> on the specified <paramref name="clrObject"/> to <paramref name="clrValue"/>.
    ///     This non-generic overload accepts <see cref="object"/> parameters and will box value types by design.
    ///     For struct targets, this method sets a copy and will not mutate the caller's instance.
    /// </summary>
    /// <param name="clrObject">The object instance containing the member to set.</param>
    /// <param name="clrValue">The value to assign to the member.</param>
    /// <returns><c>true</c> if the assignment succeeded; otherwise, <c>false</c>.</returns>
    public bool TrySetValue(object clrObject, object? clrValue)
    {
        if (clrObject is null)
        {
            return false;
        }

        return this.TrySetValue<object, object?>(clrObject, clrValue);
    }

    /// <summary>
    ///     Attempts to set the CLR member identified by <see cref="ClrName"/> on the specified <paramref name="clrObject"/> to <paramref name="clrValue"/> using a compiled setter.
    ///     For struct targets passed by value, this assigns to a copy and will not mutate the caller.
    ///     Use <see cref="TrySetValueRef{TObject, TValue}(ref TObject, TValue)"/> for structs when mutation is required.
    /// </summary>
    /// <typeparam name="TObject">The static type of the target instance.</typeparam>
    /// <typeparam name="TValue">The type of the value to assign.</typeparam>
    /// <param name="clrObject">The object instance containing the member to set.</param>
    /// <param name="clrValue">The value to assign to the member.</param>
    /// <returns><c>true</c> if the assignment succeeded; otherwise, <c>false</c>.</returns>
    public bool TrySetValue<TObject, TValue>(TObject clrObject, TValue clrValue)
    {
        if (clrObject is null)
        {
            return false;
        }

        // Use runtime type for non-sealed reference types
        var targetType = typeof(TObject);
        if (!targetType.IsValueType && !targetType.IsSealed)
        {
            targetType = clrObject.GetType();
        }

        var key = new CacheKey(targetType, this.ClrName);
        var cacheValue = SetterCache<TObject, TValue>.Cache.GetOrAdd(key, static k => BuildTypedSetter<TObject, TValue>(k.TargetType, k.MemberName));

        if (!cacheValue.Found || cacheValue.Setter is null)
        {
            return false;
        }

        try
        {
            cacheValue.Setter(clrObject, clrValue);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Attempts to set the CLR member identified by <see cref="ClrName"/> on the specified struct <paramref name="clrObject"/> by reference so that mutations affect the original instance.
    ///     This overload compiles and caches a by-ref setter delegate.
    /// </summary>
    /// <typeparam name="TObject">The struct type of the target instance.</typeparam>
    /// <typeparam name="TValue">The type of the value to assign.</typeparam>
    /// <param name="clrObject">The struct instance passed by reference whose member will be set.</param>
    /// <param name="clrValue">The value to assign to the member.</param>
    /// <returns><c>true</c> if the assignment succeeded; otherwise, <c>false</c>.</returns>
    public bool TrySetValueRef<TObject, TValue>(ref TObject clrObject, TValue clrValue)
        where TObject : struct
    {
        var targetType = typeof(TObject); // for structs, compile-time type is exact

        var key = new CacheKey(targetType, this.ClrName);
        var cacheValue = SetterRefCache<TObject, TValue>.Cache.GetOrAdd(key, static k => BuildTypedSetterByRef<TObject, TValue>(k.TargetType, k.MemberName));

        if (!cacheValue.Found || cacheValue.Setter is null)
        {
            return false;
        }

        try
        {
            cacheValue.Setter(ref clrObject, clrValue);
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal void Initialize(ApiSchema apiSchema, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        this.InitializeApiName(apiValidationPath, ref results);
        this.InitializeClrName(apiValidationPath, ref results);
        this.InitializeApiTypeExpression(apiSchema, apiValidationPath, ref results);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiTypeExpression = this.ApiTypeExpression.SafeToString();
        var apiTypeModifiers = this.ApiTypeModifiers.SafeToString();
        var clrName = this.ClrName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiProperty)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiTypeExpression)}={apiTypeExpression}, {nameof(this.ApiTypeModifiers)}={apiTypeModifiers}, {nameof(this.ClrName)}={clrName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region Initialize Methods
    private void InitializeApiName(string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    private void InitializeApiTypeExpression(ApiSchema apiSchema, string apiParentValidationPath, ref List<ValidationResult>? results)
    {
        if (this.ApiTypeExpression is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiParentValidationPath}.{nameof(this.ApiTypeExpression)} cannot be null.", [nameof(this.ApiTypeExpression)]));
            return;
        }

        var apiChildValidationPath = $"{apiParentValidationPath}.{nameof(this.ApiTypeExpression)}";
        this.ApiTypeExpression.Initialize(apiSchema, apiChildValidationPath, ref results);
    }

    private void InitializeClrName(string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ClrName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} cannot be null or whitespace.", [nameof(this.ClrName)]));
        }
    }
    #endregion

    #region Cache Methods
    private static GetterCacheValue<TObject, TValue> BuildTypedGetter<TObject, TValue>(Type targetType, string memberName)
    {
        if (!TryResolveMember(targetType, memberName, forWrite: false, out var memberInfo, out var memberType))
        {
            return new GetterCacheValue<TObject, TValue>(false, null);
        }

        var objExpression = Expression.Parameter(typeof(TObject), "obj");
        var targetExpression = MakeTargetExpression<TObject>(objExpression, targetType);

        var valueExpression = memberInfo switch
        {
            PropertyInfo pi => Expression.Property(targetExpression, pi),
            FieldInfo fi => Expression.Field(targetExpression, fi),
            _ => null
        };

        if (valueExpression is null)
        {
            return new GetterCacheValue<TObject, TValue>(false, null);
        }

        if (!TryConvertExpression(valueExpression, typeof(TValue), out var bodyExpression))
        {
            return new GetterCacheValue<TObject, TValue>(false, null);
        }

        var lambdaExpression = Expression.Lambda<Func<TObject, TValue>>(bodyExpression, objExpression);
        var lambda = lambdaExpression.Compile();

        return new GetterCacheValue<TObject, TValue>(true, lambda);
    }

    private static SetterCacheValue<TObject, TValue> BuildTypedSetter<TObject, TValue>(Type targetType, string memberName)
    {
        if (!TryResolveMember(targetType, memberName, forWrite: true, out var memberInfo, out var memberType))
        {
            return new SetterCacheValue<TObject, TValue>(false, null);
        }

        var objExpression = Expression.Parameter(typeof(TObject), "obj");
        var targetExpression = MakeTargetExpression<TObject>(objExpression, targetType);

        var valueExpression = Expression.Parameter(typeof(TValue), "value");

        var memberAccessExpression = memberInfo switch
        {
            PropertyInfo pi => Expression.Property(targetExpression, pi),
            FieldInfo fi => Expression.Field(targetExpression, fi),
            _ => null
        };

        if (memberAccessExpression is null)
        {
            return new SetterCacheValue<TObject, TValue>(false, null);
        }

        if (!TryConvertExpression(valueExpression, memberType, out var rhsExpression))
        {
            return new SetterCacheValue<TObject, TValue>(false, null);
        }

        var assignExpression = Expression.Assign(memberAccessExpression, rhsExpression);
        var lambdaExpression = Expression.Lambda<Action<TObject, TValue>>(assignExpression, objExpression, valueExpression);
        var lambda = lambdaExpression.Compile();

        return new SetterCacheValue<TObject, TValue>(true, lambda);
    }

    private static SetterRefCacheValue<TObject, TValue> BuildTypedSetterByRef<TObject, TValue>(Type targetType, string memberName)
        where TObject : struct
    {
        // targetType must be exactly typeof(TObject) for structs
        if (targetType != typeof(TObject))
        {
            return new SetterRefCacheValue<TObject, TValue>(false, null);
        }

        if (!TryResolveMember(targetType, memberName, forWrite: true, out var memberInfo, out var memberType))
        {
            return new SetterRefCacheValue<TObject, TValue>(false, null);
        }

        var objExpression = Expression.Parameter(typeof(TObject).MakeByRefType(), "obj");
        var valueExpression = Expression.Parameter(typeof(TValue), "value");

        var memberAccessExpression = memberInfo switch
        {
            PropertyInfo pi => Expression.Property(objExpression, pi),
            FieldInfo fi => Expression.Field(objExpression, fi),
            _ => null
        };

        if (memberAccessExpression is null)
        {
            return new SetterRefCacheValue<TObject, TValue>(false, null);
        }

        if (!TryConvertExpression(valueExpression, memberType, out var rhsExpression))
        {
            return new SetterRefCacheValue<TObject, TValue>(false, null);
        }

        var assignExpression = Expression.Assign(memberAccessExpression, rhsExpression);
        var lambdaExpression = Expression.Lambda<ByRefAction<TObject, TValue>>(assignExpression, objExpression, valueExpression);
        var lambda = lambdaExpression.Compile();

        return new SetterRefCacheValue<TObject, TValue>(true, lambda);
    }

    private static Expression MakeTargetExpression<TObject>(ParameterExpression objParam, Type targetType)
    {
        return targetType == typeof(TObject)
            ? objParam
            : Expression.Convert(objParam, targetType);
    }

    private static bool TryConvertExpression(Expression source, Type destinationType, out Expression converted)
    {
        if (destinationType.IsAssignableFrom(source.Type))
        {
            converted = source;
            return true;
        }

        try
        {
            converted = Expression.Convert(source, destinationType);
            return true;
        }
        catch
        {
            converted = default!;
            return false;
        }
    }

    // Shared helpers to DRY member resolution and expression conversions
    private static bool TryResolveMember(Type targetType, string memberName, bool forWrite, out MemberInfo memberInfo, out Type memberType)
    {
        // Prefer property, then field (case-insensitive)
        var propertyInfo = TypeReflection.GetProperty(targetType, memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

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

        var fieldInfo = TypeReflection.GetField(targetType, memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
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
