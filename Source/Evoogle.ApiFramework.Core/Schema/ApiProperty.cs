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

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Coercion;
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
///         For optimal performance, getter and setter accessors are compiled once during initialization
///         as lambda expressions using the owning <see cref="ApiObjectType"/>'s CLR type. The compiled
///         delegates are stored directly on each <see cref="ApiProperty"/> instance and provide near-native
///         performance (~10-20x faster than reflection) for property/field access operations.
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
    private delegate void ByRefAction<TObject, in TValue>(ref TObject clrObject, ApiSchemaContext apiSchemaContext, TValue? clrValue);

    private readonly record struct ClrCacheKey(Type ClrObjectType, string ClrMemberName);

    private readonly record struct ClrGetterCacheValue<TObject, TValue>(Func<TObject, ApiSchemaContext, TValue?>? ClrGetter);

    private readonly record struct ClrSetterCacheValue<TObject, TValue>(Action<TObject, ApiSchemaContext, TValue?>? ClrSetter);

    private readonly record struct ClrSetterByRefCacheValue<TObject, TValue>(ByRefAction<TObject, TValue?>? ClrSetterByRef)
        where TObject : struct;

    private static class ClrGetterCache<TObject, TValue>
    {
        public static readonly ConcurrentDictionary<ClrCacheKey, ClrGetterCacheValue<TObject, TValue>> Cache = new();
    }

    private static class ClrSetterCache<TObject, TValue>
    {
        public static readonly ConcurrentDictionary<ClrCacheKey, ClrSetterCacheValue<TObject, TValue>> Cache = new();
    }

    private static class ClrSetterByRefCache<TObject, TValue>
        where TObject : struct
    {
        public static readonly ConcurrentDictionary<ClrCacheKey, ClrSetterByRefCacheValue<TObject, TValue>> Cache = new();
    }

    private readonly record struct CoerceMethodCacheKey(Type ClrInputType, Type ClrOutputType);
    #endregion

    #region Fields
    private ApiSchemaContext? _apiSchemaContext = null;

    private Func<object, ApiSchemaContext, object?>? _clrGetter;

    private Action<object, ApiSchemaContext, object?>? _clrSetter;

    private static readonly ConcurrentDictionary<CoerceMethodCacheKey, MethodInfo> _coerceMethodCache = new();

    private static readonly MethodInfo? _genericCoerceMethodDefinition = TypeReflection.GetGenericMethodDefinition
    (
        type: typeof(TypeCoercion),
        methodName: nameof(TypeCoercion.Coerce),
        bindingFlags: BindingFlags.Public | BindingFlags.Instance,
        parameterCount: 2
    );

    private static readonly MethodInfo? _nonGenericCoerceMethod = TypeReflection.GetMethod
    (
        type: typeof(TypeCoercion),
        methodName: nameof(TypeCoercion.Coerce),
        bindingFlags: BindingFlags.Public | BindingFlags.Instance,
        parameterTypes: [typeof(object), typeof(Type), typeof(TypeCoercionContext)]
    );
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

    /// <summary>Gets the schema context for this property.</summary>
    internal ApiSchemaContext ApiSchemaContext => this.ThrowIfNotInitialized(_apiSchemaContext);

    private static MethodInfo GenericCoerceMethodDefinition => _genericCoerceMethodDefinition
        ?? throw new InvalidOperationException($"Failed to locate generic method definition for {nameof(TypeCoercion)}.{nameof(TypeCoercion.Coerce)}.");

    private static MethodInfo NonGenericCoerceMethod => _nonGenericCoerceMethod
        ?? throw new InvalidOperationException($"Failed to locate non-generic method for {nameof(TypeCoercion)}.{nameof(TypeCoercion.Coerce)}.");
    #endregion

    #region Computed Properties
    /// <summary>Gets a value indicating whether this property is optional (not required).</summary>
    public bool IsOptional => !this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    /// <summary>Gets a value indicating whether this property is required.</summary>
    public bool IsRequired => this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);
    #endregion

    #region ApiProperty Methods
    /// <summary>
    ///     Gets the value of this property from the specified object.
    /// </summary>
    /// <param name="clrObject">The object instance to get the property value from.</param>
    /// <returns>The property value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrObject"/> is null.</exception>
    /// <exception cref="ApiSchemaException">Thrown when the property has no compiled getter or the getter fails to execute.</exception>
    /// <remarks>
    ///     This method uses a pre-compiled lambda expression for optimal performance.
    ///     The return value will be boxed if the property type is a value type.
    /// </remarks>
    public object? GetValue(object clrObject)
    {
        ArgumentNullException.ThrowIfNull(clrObject, nameof(clrObject));

        if (_clrGetter is null)
        {
            var errorMessage =
                $"Cannot get value for property '{this.ClrName}': no compiled getter available. " +
                $"This may indicate the property does not exist on the target type, is write-only, or the schema was not initialized properly.";
            throw new ApiSchemaException(errorMessage);
        }

        try
        {
            return _clrGetter(clrObject, this.ApiSchemaContext);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Failed to get value for property '{this.ClrName}' from object of type '{clrObject.GetType().SafeToName()}': {ex.Message}";
            throw new ApiSchemaException(errorMessage, ex);
        }
    }

    /// <summary>
    ///     Gets the value of this property from the specified object with type safety.
    /// </summary>
    /// <typeparam name="TObject">The type of the object containing the property.</typeparam>
    /// <typeparam name="TValue">The expected return type of the property value.</typeparam>
    /// <param name="clrObject">The object instance to get the property value from.</param>
    /// <returns>The property value typed as <typeparamref name="TValue"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrObject"/> is null.</exception>
    /// <exception cref="ApiSchemaException">Thrown when the typed getter cannot be compiled or the getter fails to execute.</exception>
    /// <remarks>
    ///     <para>
    ///         This method compiles and caches a typed lambda expression per unique combination of
    ///         <typeparamref name="TObject"/> and <typeparamref name="TValue"/>. The compiled delegate
    ///         is cached for subsequent calls, providing near-native performance.
    ///     </para>
    ///     <para>
    ///         Prefer this generic overload over the non-generic version when the types are known
    ///         at compile time to avoid boxing overhead for value types.
    ///     </para>
    /// </remarks>
    public TValue? GetValue<TObject, TValue>(TObject clrObject)
        where TObject : notnull
    {
        ArgumentNullException.ThrowIfNull(clrObject, nameof(clrObject));

        var clrObjectType = typeof(TObject);
        var clrCacheKey = new ClrCacheKey(clrObjectType, this.ClrName);
        var clrCacheValue = ClrGetterCache<TObject, TValue>.Cache.GetOrAdd(clrCacheKey, static k => BuildGenericClrGetter<TObject, TValue>(k.ClrObjectType, k.ClrMemberName));
        var clrGetter = clrCacheValue.ClrGetter;

        if (clrGetter is null)
        {
            var errorMessage =
                $"Cannot get value for property '{this.ClrName}' on type '{typeof(TObject).SafeToName()}': " +
                $"failed to compile typed getter for return type '{typeof(TValue).SafeToName()}'. " +
                $"Verify the property exists, is readable, and the return type is compatible.";
            throw new ApiSchemaException(errorMessage);
        }

        try
        {
            return clrGetter(clrObject, this.ApiSchemaContext);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Failed to get value for property '{this.ClrName}' from object of type '{typeof(TObject).SafeToName()}': {ex.Message}";
            throw new ApiSchemaException(errorMessage, ex);
        }
    }

    /// <summary>
    ///     Sets the value of this property on the specified object.
    /// </summary>
    /// <param name="clrObject">The object instance to set the property value on.</param>
    /// <param name="clrValue">The value to assign to the property.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrObject"/> is null.</exception>
    /// <exception cref="ApiSchemaException">Thrown when the property has no compiled setter, is read-only, or the setter fails to execute.</exception>
    /// <remarks>
    ///     This method uses a pre-compiled lambda expression for optimal performance.
    ///     For struct targets passed by value, this method will modify a copy. Use the generic
    ///     <see cref="SetValueByRef{TObject, TValue}(ref TObject, TValue)"/> method for struct mutation.
    /// </remarks>
    public void SetValue(object clrObject, object? clrValue)
    {
        ArgumentNullException.ThrowIfNull(clrObject, nameof(clrObject));

        if (_clrSetter is null)
        {
            var errorMessage =
                $"Cannot set value for property '{this.ClrName}': no compiled setter available. " +
                $"This property may be read-only, write-only (getter missing), not exist on the target type, or the schema was not initialized properly.";
            throw new ApiSchemaException(errorMessage);
        }

        try
        {
            _clrSetter(clrObject, this.ApiSchemaContext, clrValue);
        }
        catch (Exception ex)
        {
            var valueTypeName = clrValue?.GetType().SafeToName() ?? "null";
            var errorMessage =
                $"Failed to set value for property '{this.ClrName}' on object of type '{clrObject.GetType().SafeToName()}' " +
                $"with value of type '{valueTypeName}': {ex.Message}";
            throw new ApiSchemaException(errorMessage, ex);
        }
    }

    /// <summary>
    ///     Sets the value of this property on the specified object with type safety.
    /// </summary>
    /// <typeparam name="TObject">The type of the object containing the property.</typeparam>
    /// <typeparam name="TValue">The type of the value to assign.</typeparam>
    /// <param name="clrObject">The object instance to set the property value on.</param>
    /// <param name="clrValue">The value to assign to the property.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrObject"/> is null.</exception>
    /// <exception cref="ApiSchemaException">Thrown when the typed setter cannot be compiled, the property is read-only, or the setter fails to execute.</exception>
    /// <remarks>
    ///     <para>
    ///         This method compiles and caches a typed lambda expression per unique combination of
    ///         <typeparamref name="TObject"/> and <typeparamref name="TValue"/>. The compiled delegate
    ///         is cached for subsequent calls, providing near-native performance.
    ///     </para>
    ///     <para>
    ///         For struct targets passed by value, this method will modify a copy. Use
    ///         <see cref="SetValueByRef{TObject, TValue}(ref TObject, TValue)"/> for struct mutation.
    ///     </para>
    /// </remarks>
    public void SetValue<TObject, TValue>(TObject clrObject, TValue? clrValue)
        where TObject : notnull
    {
        ArgumentNullException.ThrowIfNull(clrObject, nameof(clrObject));

        var clrObjectType = typeof(TObject);
        var clrCacheKey = new ClrCacheKey(clrObjectType, this.ClrName);
        var clrCacheValue = ClrSetterCache<TObject, TValue>.Cache.GetOrAdd(clrCacheKey, static k => BuildGenericClrSetter<TObject, TValue>(k.ClrObjectType, k.ClrMemberName));
        var clrSetter = clrCacheValue.ClrSetter;

        if (clrSetter is null)
        {
            var errorMessage =
                $"Cannot set value for property '{this.ClrName}' on type '{typeof(TObject).SafeToName()}': " +
                $"failed to compile typed setter for value type '{typeof(TValue).SafeToName()}'. " +
                $"Verify the property exists, is writable, and the value type is compatible.";
            throw new ApiSchemaException(errorMessage);
        }

        try
        {
            clrSetter(clrObject, this.ApiSchemaContext, clrValue);
        }
        catch (Exception ex)
        {
            var valueTypeName = clrValue?.GetType().SafeToName() ?? "null";
            var errorMessage =
                $"Failed to set value for property '{this.ClrName}' on object of type '{typeof(TObject).SafeToName()}' " +
                $"with value of type '{valueTypeName}': {ex.Message}";
            throw new ApiSchemaException(errorMessage, ex);
        }
    }

    /// <summary>
    ///     Sets the value of this property on the specified struct by reference, ensuring mutations affect the original instance.
    /// </summary>
    /// <typeparam name="TObject">The struct type of the object containing the property.</typeparam>
    /// <typeparam name="TValue">The type of the value to assign.</typeparam>
    /// <param name="clrObject">The struct instance passed by reference to set the property value on.</param>
    /// <param name="clrValue">The value to assign to the property.</param>
    /// <exception cref="ApiSchemaException">Thrown when the by-ref setter cannot be compiled, the property is read-only, or the setter fails to execute.</exception>
    /// <remarks>
    ///     <para>
    ///         This method compiles and caches a by-reference lambda expression for struct mutation.
    ///         Unlike <see cref="SetValue{TObject, TValue}(TObject, TValue)"/>, this method ensures
    ///         the original struct instance is modified rather than a copy.
    ///     </para>
    ///     <para>
    ///         Use this method when you need to mutate struct properties in place, such as when
    ///         working with struct collections or ref locals.
    ///     </para>
    /// </remarks>
    public void SetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue)
        where TObject : struct
    {
        var clrObjectType = typeof(TObject);
        var clrCacheKey = new ClrCacheKey(clrObjectType, this.ClrName);
        var clrCacheValue = ClrSetterByRefCache<TObject, TValue>.Cache.GetOrAdd(clrCacheKey, static k => BuildGenericClrSetterByRef<TObject, TValue>(k.ClrObjectType, k.ClrMemberName));
        var clrSetterByRef = clrCacheValue.ClrSetterByRef;

        if (clrSetterByRef is null)
        {
            var errorMessage =
                $"Cannot set value for property '{this.ClrName}' on struct type '{typeof(TObject).SafeToName()}': " +
                $"failed to compile by-ref setter for value type '{typeof(TValue).SafeToName()}'. " +
                $"Verify the property exists on the struct, is writable (not readonly/init-only), and the value type is compatible.";
            throw new ApiSchemaException(errorMessage);
        }

        try
        {
            clrSetterByRef(ref clrObject, this.ApiSchemaContext, clrValue);
        }
        catch (Exception ex)
        {
            var valueTypeName = clrValue?.GetType().SafeToName() ?? "null";
            var errorMessage =
                $"Failed to set value for property '{this.ClrName}' on struct of type '{typeof(TObject).SafeToName()}' " +
                $"with value of type '{valueTypeName}': {ex.Message}";
            throw new ApiSchemaException(errorMessage, ex);
        }
    }

    /// <summary>
    ///     Attempts to read the CLR member value identified by <see cref="ClrName"/> from the specified <paramref name="clrObject"/>.
    ///     This non-generic overload returns the value as <see cref="object"/>, which will box value types.
    /// </summary>
    /// <param name="clrObject">The object instance containing the member.</param>
    /// <param name="clrValue">When this method returns, contains the member value if successful; otherwise, the default value.</param>
    /// <returns><c>true</c> if the value was retrieved successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         This method never throws exceptions. It returns <c>false</c> if:
    ///         <list type="bullet">
    ///             <item><description>The <paramref name="clrObject"/> is null</description></item>
    ///             <item><description>The property has no compiled getter</description></item>
    ///             <item><description>An exception occurs during property access</description></item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         Prefer this method over <see cref="GetValue(object)"/> when failure is expected
    ///         or performance is critical, as it avoids exception overhead.
    ///     </para>
    /// </remarks>
    public bool TryGetValue(object? clrObject, out object? clrValue)
    {
        if (clrObject is null || _clrGetter is null)
        {
            clrValue = default;
            return false;
        }

        try
        {
            clrValue = _clrGetter(clrObject, this.ApiSchemaContext);
            return true;
        }
        catch
        {
            clrValue = default;
            return false;
        }
    }

    /// <summary>
    ///     Attempts to read the CLR member value identified by <see cref="ClrName"/> from the specified <paramref name="clrObject"/>
    ///     with type safety and minimal boxing overhead.
    /// </summary>
    /// <typeparam name="TObject">The static type of the target instance.</typeparam>
    /// <typeparam name="TValue">The desired return type for the member value.</typeparam>
    /// <param name="clrObject">The object instance containing the member.</param>
    /// <param name="clrValue">When this method returns, contains the member value if successful; otherwise, the default value.</param>
    /// <returns><c>true</c> if the value was retrieved successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         This method compiles and caches a typed lambda expression per unique combination of
    ///         <typeparamref name="TObject"/> and <typeparamref name="TValue"/>. Subsequent calls with
    ///         the same type parameters reuse the cached delegate for optimal performance.
    ///     </para>
    ///     <para>
    ///         This method never throws exceptions. It returns <c>false</c> if:
    ///         <list type="bullet">
    ///             <item><description>The <paramref name="clrObject"/> is null</description></item>
    ///             <item><description>The typed getter cannot be compiled</description></item>
    ///             <item><description>An exception occurs during property access</description></item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         Prefer this generic overload over the non-generic version when types are known at
    ///         compile time to avoid boxing value types.
    ///     </para>
    /// </remarks>
    public bool TryGetValue<TObject, TValue>(TObject? clrObject, out TValue? clrValue)
    {
        if (clrObject is null)
        {
            clrValue = default;
            return false;
        }

        var clrObjectType = typeof(TObject);
        var clrMemberName = this.ClrName;

        var clrCacheKey = new ClrCacheKey(clrObjectType, clrMemberName);
        var clrCacheValue = ClrGetterCache<TObject, TValue>.Cache.GetOrAdd(clrCacheKey, static k => BuildGenericClrGetter<TObject, TValue>(k.ClrObjectType, k.ClrMemberName));
        var clrGetter = clrCacheValue.ClrGetter;

        if (clrGetter is null)
        {
            clrValue = default;
            return false;
        }

        try
        {
            clrValue = clrGetter(clrObject, this.ApiSchemaContext);
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
    /// </summary>
    /// <param name="clrObject">The object instance containing the member to set.</param>
    /// <param name="clrValue">The value to assign to the member.</param>
    /// <returns><c>true</c> if the assignment succeeded; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         This method never throws exceptions. It returns <c>false</c> if:
    ///         <list type="bullet">
    ///             <item><description>The <paramref name="clrObject"/> is null</description></item>
    ///             <item><description>The property has no compiled setter or is read-only</description></item>
    ///             <item><description>An exception occurs during property assignment</description></item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         For struct targets passed by value, this method modifies a copy. Use
    ///         <see cref="TrySetValueByRef{TObject, TValue}(ref TObject, TValue)"/> for struct mutation.
    ///     </para>
    ///     <para>
    ///         Prefer this method over <see cref="SetValue(object, object)"/> when failure is expected
    ///         or performance is critical, as it avoids exception overhead.
    ///     </para>
    /// </remarks>
    public bool TrySetValue(object? clrObject, object? clrValue)
    {
        if (clrObject is null || _clrSetter is null)
        {
            return false;
        }

        try
        {
            _clrSetter(clrObject, this.ApiSchemaContext, clrValue);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Attempts to set the CLR member identified by <see cref="ClrName"/> on the specified <paramref name="clrObject"/>
    ///     to <paramref name="clrValue"/> with type safety and minimal boxing overhead.
    /// </summary>
    /// <typeparam name="TObject">The static type of the target instance.</typeparam>
    /// <typeparam name="TValue">The type of the value to assign.</typeparam>
    /// <param name="clrObject">The object instance containing the member to set.</param>
    /// <param name="clrValue">The value to assign to the member.</param>
    /// <returns><c>true</c> if the assignment succeeded; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         This method compiles and caches a typed lambda expression per unique combination of
    ///         <typeparamref name="TObject"/> and <typeparamref name="TValue"/>. Subsequent calls with
    ///         the same type parameters reuse the cached delegate for optimal performance.
    ///     </para>
    ///     <para>
    ///         This method never throws exceptions. It returns <c>false</c> if:
    ///         <list type="bullet">
    ///             <item><description>The <paramref name="clrObject"/> is null</description></item>
    ///             <item><description>The typed setter cannot be compiled or the property is read-only</description></item>
    ///             <item><description>An exception occurs during property assignment</description></item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         For struct targets passed by value, this method modifies a copy. Use
    ///         <see cref="TrySetValueByRef{TObject, TValue}(ref TObject, TValue)"/> for struct mutation.
    ///     </para>
    ///     <para>
    ///         Prefer this generic overload over the non-generic version when types are known at
    ///         compile time to avoid boxing value types.
    ///     </para>
    /// </remarks>
    public bool TrySetValue<TObject, TValue>(TObject? clrObject, TValue? clrValue)
    {
        if (clrObject is null)
        {
            return false;
        }

        var clrObjectType = typeof(TObject);
        var clrMemberName = this.ClrName;

        var clrCacheKey = new ClrCacheKey(clrObjectType, clrMemberName);
        var clrCacheValue = ClrSetterCache<TObject, TValue>.Cache.GetOrAdd(clrCacheKey, static k => BuildGenericClrSetter<TObject, TValue>(k.ClrObjectType, k.ClrMemberName));
        var clrSetter = clrCacheValue.ClrSetter;

        if (clrSetter is null)
        {
            return false;
        }

        try
        {
            clrSetter(clrObject, this.ApiSchemaContext, clrValue);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Attempts to set the CLR member identified by <see cref="ClrName"/> on the specified struct <paramref name="clrObject"/>
    ///     by reference, ensuring mutations affect the original instance.
    /// </summary>
    /// <typeparam name="TObject">The struct type of the target instance.</typeparam>
    /// <typeparam name="TValue">The type of the value to assign.</typeparam>
    /// <param name="clrObject">The struct instance passed by reference whose member will be set.</param>
    /// <param name="clrValue">The value to assign to the member.</param>
    /// <returns><c>true</c> if the assignment succeeded; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         This method compiles and caches a by-reference lambda expression for struct mutation.
    ///         Unlike <see cref="TrySetValue{TObject, TValue}(TObject, TValue)"/>, this method ensures
    ///         the original struct instance is modified rather than a copy.
    ///     </para>
    ///     <para>
    ///         This method never throws exceptions. It returns <c>false</c> if:
    ///         <list type="bullet">
    ///             <item><description>The by-ref setter cannot be compiled or the property is read-only</description></item>
    ///             <item><description>An exception occurs during property assignment</description></item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         Use this method when you need to mutate struct properties in place, such as when
    ///         working with struct collections or ref locals.
    ///     </para>
    /// </remarks>
    public bool TrySetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue)
        where TObject : struct
    {
        var clrObjectType = typeof(TObject);
        var clrMemberName = this.ClrName;

        var clrCacheKey = new ClrCacheKey(clrObjectType, clrMemberName);
        var clrCacheValue = ClrSetterByRefCache<TObject, TValue>.Cache.GetOrAdd(clrCacheKey, static k => BuildGenericClrSetterByRef<TObject, TValue>(k.ClrObjectType, k.ClrMemberName));
        var clrSetterByRef = clrCacheValue.ClrSetterByRef;

        if (clrSetterByRef is null)
        {
            return false;
        }

        try
        {
            clrSetterByRef(ref clrObject, this.ApiSchemaContext, clrValue);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Validates the specified CLR value against this property's constraints with type safety.
    /// </summary>
    /// <param name="apiValidationPath">The validation path used for error reporting.</param>
    /// <param name="clrValue">The CLR value to validate.</param>
    /// <returns>A <see cref="ValidationResult"/> if validation fails; otherwise, <c>null</c>.</returns>
    /// <remarks>
    ///     This method validates whether a value can be assigned to this property based on the <see cref="ApiTypeModifiers"/>.
    ///     Currently validates that required properties receive non-null values.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="apiValidationPath"/> is null or whitespace.</exception>
    public ValidationResult? ValidateValue(string apiValidationPath, object? clrValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        if (clrValue is null && this.IsRequired)
        {
            return new ValidationResult($"'{apiValidationPath}' is required and cannot be set to null.", [this.ApiName]);
        }
        return null;
    }

    /// <summary>
    ///     Validates the specified CLR value against this property's constraints with type safety.
    /// </summary>
    /// <typeparam name="TValue">The type of the CLR value to validate.</typeparam>
    /// <param name="apiValidationPath">The validation path used for error reporting.</param>
    /// <param name="clrValue">The CLR value to validate.</param>
    /// <returns>A <see cref="ValidationResult"/> if validation fails; otherwise, <c>null</c>.</returns>
    /// <remarks>
    ///     This method validates whether a value can be assigned to this property based on the <see cref="ApiTypeModifiers"/>.
    ///     Currently validates that required properties receive non-null values.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="apiValidationPath"/> is null or whitespace.</exception>
    public ValidationResult? ValidateValue<TValue>(string apiValidationPath, TValue? clrValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        if (clrValue is null && this.IsRequired)
        {
            return new ValidationResult($"'{apiValidationPath}' is required and cannot be set to null.", [this.ApiName]);
        }
        return null;
    }

    internal string GetValidationPath(string parentPath) => $"{parentPath.SafeToString()}.{nameof(ApiProperty)}[\"{this.ApiName.SafeToString()}\"]";

    internal void Initialize(ApiSchema apiSchema, ApiSchemaContext apiSchemaContext, ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(apiSchemaContext);
        ArgumentNullException.ThrowIfNull(apiObjectType);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        _apiSchemaContext = apiSchemaContext;

        this.InitializeApiName(apiValidationPath, ref results);
        this.InitializeApiTypeExpression(apiSchema, apiValidationPath, ref results);
        this.InitializeClrName(apiValidationPath, ref results);
        this.InitializeClrGetterAndSetter(apiObjectType, apiValidationPath, ref results);
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

    private void InitializeClrGetterAndSetter(ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        var clrObjectType = apiObjectType.ClrType;
        var clrMemberName = this.ClrName;

        try
        {
            // Prefer property, then field
            var clrPropertyInfo = TypeReflection.GetProperty(clrObjectType, clrMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (clrPropertyInfo is not null)
            {
                // Exclude indexers
                if (clrPropertyInfo.GetIndexParameters().Length > 0)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} refers to an indexer property, which is not supported.", [nameof(this.ClrName)]));
                    return;
                }

                // Build compiled property getter and setter
                try
                {
                    _clrGetter = BuildNonGenericClrPropertyGetter(clrObjectType, clrPropertyInfo);
                }
                catch (Exception ex)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda property getter: {ex.Message}", [nameof(this.ClrName)]));
                }

                try
                {
                    _clrSetter = BuildNonGenericClrPropertySetter(clrObjectType, clrPropertyInfo);
                }
                catch (Exception ex)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda property setter: {ex.Message}", [nameof(this.ClrName)]));
                }

                return; // Found valid property
            }

            var clrFieldInfo = TypeReflection.GetField(clrObjectType, clrMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (clrFieldInfo is not null)
            {
                // Build compiled field getter and setter
                try
                {
                    _clrGetter = BuildNonGenericClrFieldGetter(clrObjectType, clrFieldInfo);
                }
                catch (Exception ex)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda field getter: {ex.Message}", [nameof(this.ClrName)]));
                }

                try
                {
                    _clrSetter = BuildNonGenericClrFieldSetter(clrObjectType, clrFieldInfo);
                }
                catch (Exception ex)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda field setter: {ex.Message}", [nameof(this.ClrName)]));
                }

                return; // Found valid field
            }

            // Member not found
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} '{clrMemberName}' could not be found on {nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}'.", [nameof(this.ClrName)]));
        }
        catch (Exception ex)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda getter or setter accessor: {ex.Message}", [nameof(this.ClrName)]));
        }
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

    #region Non-Generic Accessor Methods
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

    #region Generic Accessor Methods
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
            var directSetterLambdaExpression = Expression.Lambda<ByRefAction<TObject, TValue?>>(directAssignExpression, objectByRefParameterExpression, contextParameterExpression, valueParameterExpression);
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
        var coerceSetterLambdaExpression = Expression.Lambda<ByRefAction<TObject, TValue?>>(coerceAssignExpression, objectByRefParameterExpression, contextParameterExpression, valueParameterExpression);
        var coerceSetterDelegate = coerceSetterLambdaExpression.Compile();

        return new ClrSetterByRefCacheValue<TObject, TValue>(coerceSetterDelegate);
    }

    private static MethodInfo MakeGenericCoerceMethod(Type inputType, Type outputType)
    {
        var key = new CoerceMethodCacheKey(inputType, outputType);
        return _coerceMethodCache.GetOrAdd(key, k => GenericCoerceMethodDefinition.MakeGenericMethod(k.ClrInputType, k.ClrOutputType));
    }

    private static Expression MakeObjectExpression<TObject>(ParameterExpression parameterExpression, Type objectType)
    {
        return objectType == typeof(TObject)
            ? parameterExpression
            : Expression.Convert(parameterExpression, objectType);
    }

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
