// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Partial class containing public accessor methods for getting and setting property values.
/// </summary>
public sealed partial class ApiProperty
{
    #region Get Methods
    /// <summary>
    ///     Gets the value of this property from the specified object.
    /// </summary>
    /// <param name="clrObject">The object instance to get the property value from.</param>
    /// <param name="clrValueType">
    ///     The optional desired output type for coercion. If <c>null</c> (the default), no coercion is performed
    ///     and the value is returned directly as <see cref="object"/>. If specified, the retrieved value is
    ///     coerced to the specified type using <see cref="ApiSchemaContext.TypeCoercion"/>.
    /// </param>
    /// <returns>The property value, optionally coerced to <paramref name="clrValueType"/> if specified.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrObject"/> is null.</exception>
    /// <exception cref="ApiSchemaException">Thrown when the property has no compiled getter or the getter fails to execute.</exception>
    /// <remarks>
    ///     <para>
    ///         This method uses a pre-compiled lambda expression for optimal performance.
    ///         The return value will be boxed if the property type is a value type.
    ///     </para>
    ///     <para>
    ///         When <paramref name="clrValueType"/> is specified, the retrieved value is coerced using the
    ///         schema's <see cref="Coercion.TypeCoercion"/> instance. This handles type conversions
    ///         such as unboxing, downcasting, enum conversions, and custom coercion definitions.
    ///     </para>
    /// </remarks>
    public object? GetValue(object clrObject, Type? clrValueType = null)
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
            var clrValue = _clrGetter(clrObject, this.ApiSchemaContext);

            // If no desired output type specified, return the value directly (no coercion)
            if (clrValueType is null)
            {
                return clrValue;
            }

            // Coerce the value to the desired output type
            var clrCoercedValue = this.ApiSchemaContext.TypeCoercion.Coerce(clrValue, clrValueType, this.ApiSchemaContext.TypeCoercionContext);
            return clrCoercedValue;
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
    #endregion

    #region Set Methods
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
    #endregion

    #region TryGet Methods
    /// <summary>
    ///     Attempts to read the CLR member value identified by <see cref="ClrName"/> from the specified <paramref name="clrObject"/>.
    ///     This non-generic overload returns the value as <see cref="object"/>, which will box value types.
    /// </summary>
    /// <param name="clrObject">The object instance containing the member.</param>
    /// <param name="clrValue">When this method returns, contains the member value if successful; otherwise, the default value.</param>
    /// <param name="clrValueType">
    ///     The optional desired output type for coercion. If <c>null</c> (the default), no coercion is performed
    ///     and the value is returned directly as <see cref="object"/>. If specified, the retrieved value is
    ///     coerced to the specified type using <see cref="ApiSchemaContext.TypeCoercion"/>.
    /// </param>
    /// <returns><c>true</c> if the value was retrieved (and optionally coerced) successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>
    ///         This method never throws exceptions. It returns <c>false</c> if:
    ///         <list type="bullet">
    ///             <item><description>The <paramref name="clrObject"/> is null</description></item>
    ///             <item><description>The property has no compiled getter</description></item>
    ///             <item><description>An exception occurs during property access</description></item>
    ///             <item><description>Coercion to <paramref name="clrValueType"/> fails (when specified)</description></item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         Prefer this method over <see cref="GetValue(object, Type?)"/> when failure is expected
    ///         or performance is critical, as it avoids exception overhead.
    ///     </para>
    ///     <para>
    ///         When <paramref name="clrValueType"/> is specified, the retrieved value is coerced using the
    ///         schema's <see cref="Coercion.TypeCoercion"/> instance. This handles type conversions
    ///         such as unboxing, downcasting, enum conversions, and custom coercion definitions.
    ///     </para>
    /// </remarks>
    public bool TryGetValue(object? clrObject, out object? clrValue, Type? clrValueType = null)
    {
        if (clrObject is null || _clrGetter is null)
        {
            clrValue = default;
            return false;
        }

        try
        {
            var clrRawValue = _clrGetter(clrObject, this.ApiSchemaContext);

            // If no desired output type specified, return the value directly (no coercion)
            if (clrValueType is null)
            {
                clrValue = clrRawValue;
                return true;
            }

            // Coerce the value to the desired output type
            clrValue = this.ApiSchemaContext.TypeCoercion.Coerce(clrRawValue, clrValueType, this.ApiSchemaContext.TypeCoercionContext);
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
    #endregion

    #region TrySet Methods
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
    #endregion

    #region Value Coercion Methods
    /// <summary>
    ///     Coerces a raw value to the property's target type using the property's compiled coercion logic.
    /// </summary>
    /// <param name="rawValue">The raw value to coerce.</param>
    /// <param name="clrValueType">The target type to coerce to. If <c>null</c>, uses the property's CLR type.</param>
    /// <returns>The coerced value.</returns>
    /// <exception cref="ApiSchemaException">Thrown when coercion fails.</exception>
    /// <remarks>
    ///     <para>
    ///         This method provides access to the property's optimized type coercion without requiring
    ///         an actual CLR instance. Useful for coercing dictionary values, query parameters, or
    ///         deserialized data before assignment.
    ///     </para>
    ///     <para>
    ///         The coercion uses the same cached compiled expressions as the property accessors,
    ///         providing better performance than direct <see cref="Coercion.TypeCoercion"/> calls for frequently-used type pairs.
    ///     </para>
    /// </remarks>
    public object? CoerceValue(object? rawValue, Type? clrValueType = null)
    {
        ArgumentNullException.ThrowIfNull(this.ApiSchemaContext);

        var targetType = clrValueType ?? this.ApiTypeExpression.ClrType ?? throw new ApiSchemaException($"Cannot coerce value for property '{this.ClrName}' because neither a target type was provided nor does the property have a resolved CLR type.");

        try
        {
            return this.ApiSchemaContext.TypeCoercion.Coerce
            (
                rawValue,
                targetType,
                this.ApiSchemaContext.TypeCoercionContext
            );
        }
        catch (Exception ex)
        {
            throw new ApiSchemaException(
                $"Failed to coerce value for property '{this.ClrName}' from type '{rawValue?.GetType().Name ?? "null"}' to '{targetType.Name}'.",
                ex);
        }
    }

    /// <summary>
    ///     Attempts to coerce a raw value to the property's target type without throwing exceptions.
    /// </summary>
    /// <param name="rawValue">The raw value to coerce.</param>
    /// <param name="coercedValue">When this method returns, contains the coerced value if successful; otherwise, <c>null</c>.</param>
    /// <param name="clrValueType">The target type to coerce to. If <c>null</c>, uses the property's CLR type.</param>
    /// <returns><c>true</c> if coercion was successful; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     This method never throws exceptions and returns <c>false</c> on any failure.
    ///     Use <see cref="CoerceValue"/> if you need exception details.
    /// </remarks>
    public bool TryCoerceValue(object? rawValue, out object? coercedValue, Type? clrValueType = null)
    {
        coercedValue = null;

        if (this.ApiSchemaContext is null)
        {
            return false;
        }

        var targetType = clrValueType ?? this.ApiTypeExpression.ClrType;

        if (targetType is null)
        {
            return false;
        }

        try
        {
            coercedValue = this.ApiSchemaContext.TypeCoercion.Coerce
            (
                rawValue,
                targetType,
                this.ApiSchemaContext.TypeCoercionContext
            );
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
}
