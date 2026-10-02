// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.MemberAccess;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>Provides schema-aware convenience methods for accessing the property's CLR member.</summary>
/// <remarks>
///     These methods forward to a Core <see cref="MemberAccessor"/>. Generic operations use direct access when
///     their types are assignment compatible and otherwise use the schema's configured type coercion and
///     coercion context. Throwing methods translate Core failures to <see cref="ApiSchemaException"/>;
///     corresponding Try methods return <see langword="false"/> with default output and no mutation.
/// </remarks>
public sealed partial class ApiProperty
{
    #region Get Methods
    /// <summary>Gets the member value, optionally coercing it to <paramref name="clrValueType"/>.</summary>
    public object? GetValue(object clrObject, Type? clrValueType = null)
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        var accessor = this.RequireClrMemberAccessor(canRead: true);
        try
        {
            return clrValueType is null
                ? accessor.GetValue(clrObject)
                : accessor.GetValue(clrObject, clrValueType, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
        }
        catch (Exception ex)
        {
            var message = $"Failed to get value for property '{this.ClrName}' from object of type '{clrObject.GetType().SafeToName()}': {ex.Message}";
            throw new ApiSchemaException(message, ex);
        }
    }

    /// <summary>Gets the member value using the requested generic types.</summary>
    public TValue? GetValue<TObject, TValue>(TObject clrObject)
        where TObject : notnull
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        var accessor = this.RequireClrMemberAccessor(canRead: true);
        try
        {
            return typeof(TValue).IsAssignableFrom(accessor.MemberType)
                ? accessor.GetValue<TObject, TValue>(clrObject)
                : accessor.GetValue<TObject, TValue>(clrObject, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
        }
        catch (Exception ex)
        {
            var message = $"Failed to get value for property '{this.ClrName}' from object of type '{typeof(TObject).SafeToName()}': {ex.Message}";
            throw new ApiSchemaException(message, ex);
        }
    }
    #endregion

    #region Set Methods
    /// <summary>Sets the member value, coercing it when needed.</summary>
    public void SetValue(object clrObject, object? clrValue)
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        var accessor = this.RequireClrMemberAccessor(canRead: false);
        try
        {
            accessor.SetValue(clrObject, clrValue, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
        }
        catch (Exception ex)
        {
            var valueTypeName = clrValue?.GetType().SafeToName() ?? "null";
            var message = $"Failed to set value for property '{this.ClrName}' on object of type '{clrObject.GetType().SafeToName()}' with value of type '{valueTypeName}': {ex.Message}";
            throw new ApiSchemaException(message, ex);
        }
    }

    /// <summary>Sets the member value using the requested generic types.</summary>
    public void SetValue<TObject, TValue>(TObject clrObject, TValue? clrValue)
        where TObject : notnull
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        var accessor = this.RequireClrMemberAccessor(canRead: false);
        try
        {
            if (accessor.MemberType.IsAssignableFrom(typeof(TValue)))
            {
                accessor.SetValue(clrObject, clrValue);
            }
            else
            {
                accessor.SetValue(clrObject, clrValue, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
            }
        }
        catch (Exception ex)
        {
            var valueTypeName = clrValue?.GetType().SafeToName() ?? "null";
            var message = $"Failed to set value for property '{this.ClrName}' on object of type '{typeof(TObject).SafeToName()}' with value of type '{valueTypeName}': {ex.Message}";
            throw new ApiSchemaException(message, ex);
        }
    }

    /// <summary>Sets a struct member by reference using the requested generic types.</summary>
    public void SetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue)
        where TObject : struct
    {
        var accessor = this.RequireClrMemberAccessor(canRead: false);
        try
        {
            if (accessor.MemberType.IsAssignableFrom(typeof(TValue)))
            {
                accessor.SetValueByRef(ref clrObject, clrValue);
            }
            else
            {
                accessor.SetValueByRef(ref clrObject, clrValue, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
            }
        }
        catch (Exception ex)
        {
            var valueTypeName = clrValue?.GetType().SafeToName() ?? "null";
            var message = $"Failed to set value for property '{this.ClrName}' on struct of type '{typeof(TObject).SafeToName()}' with value of type '{valueTypeName}': {ex.Message}";
            throw new ApiSchemaException(message, ex);
        }
    }
    #endregion

    #region TryGet Methods
    /// <summary>Attempts to get the member value, optionally coercing it to <paramref name="clrValueType"/>.</summary>
    public bool TryGetValue(object? clrObject, out object? clrValue, Type? clrValueType = null)
    {
        var accessor = _clrMemberBinding.BoundClrMemberAccessor;
        if (clrObject is null || accessor?.CanRead != true)
        {
            clrValue = default;
            return false;
        }

        return clrValueType is null
            ? accessor.TryGetValue(clrObject, out clrValue, valueType: null, coercion: null, context: null)
            : accessor.TryGetValue(clrObject, out clrValue, clrValueType, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
    }

    /// <summary>Attempts to get the member value using the requested generic types.</summary>
    public bool TryGetValue<TObject, TValue>(TObject? clrObject, out TValue? clrValue)
    {
        var accessor = _clrMemberBinding.BoundClrMemberAccessor;
        if (clrObject is null || accessor?.CanRead != true)
        {
            clrValue = default;
            return false;
        }

        return typeof(TValue).IsAssignableFrom(accessor.MemberType)
            ? accessor.TryGetValue(clrObject, out clrValue)
            : accessor.TryGetValue(clrObject, out clrValue, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
    }
    #endregion

    #region TrySet Methods
    /// <summary>Attempts to set the member value, coercing it when needed.</summary>
    public bool TrySetValue(object? clrObject, object? clrValue)
    {
        var accessor = _clrMemberBinding.BoundClrMemberAccessor;
        return clrObject is not null
            && accessor?.CanWrite == true
            && accessor.TrySetValue(clrObject, clrValue, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
    }

    /// <summary>Attempts to set the member value using the requested generic types.</summary>
    public bool TrySetValue<TObject, TValue>(TObject? clrObject, TValue? clrValue)
    {
        var accessor = _clrMemberBinding.BoundClrMemberAccessor;
        if (clrObject is null || accessor?.CanWrite != true)
        {
            return false;
        }

        return accessor.MemberType.IsAssignableFrom(typeof(TValue))
            ? accessor.TrySetValue(clrObject, clrValue)
            : accessor.TrySetValue(clrObject, clrValue, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
    }

    /// <summary>Attempts to set a struct member by reference using the requested generic types.</summary>
    public bool TrySetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue)
        where TObject : struct
    {
        var accessor = _clrMemberBinding.BoundClrMemberAccessor;
        if (accessor?.CanWrite != true)
        {
            return false;
        }

        return accessor.MemberType.IsAssignableFrom(typeof(TValue))
            ? accessor.TrySetValueByRef(ref clrObject, clrValue)
            : accessor.TrySetValueByRef(ref clrObject, clrValue, this.ApiSchemaContext.TypeCoercion, this.ApiSchemaContext.TypeCoercionContext);
    }
    #endregion

    #region Value Coercion Methods
    /// <summary>Coerces a value using the schema's configured type coercion service.</summary>
    public object? CoerceValue(object? rawValue, Type? clrValueType = null)
    {
        ArgumentNullException.ThrowIfNull(this.ApiSchemaContext);

        var targetType = clrValueType ?? this.ApiType.ClrType;
        try
        {
            return this.ApiSchemaContext.TypeCoercion.Coerce(rawValue, targetType, this.ApiSchemaContext.TypeCoercionContext);
        }
        catch (Exception ex)
        {
            throw new ApiSchemaException($"Failed to coerce value for property '{this.ClrName}' from type '{rawValue?.GetType().Name ?? "null"}' to '{targetType.Name}'.", ex);
        }
    }

    /// <summary>Attempts to coerce a value using the schema's configured type coercion service.</summary>
    public bool TryCoerceValue(object? rawValue, out object? coercedValue, Type? clrValueType = null)
    {
        coercedValue = null;
        if (this.ApiSchemaContext is null)
        {
            return false;
        }

        var targetType = clrValueType ?? (this.ApiTypeExpression.IsResolved ? this.ApiType.ClrType : null);
        if (targetType is null)
        {
            return false;
        }

        try
        {
            coercedValue = this.ApiSchemaContext.TypeCoercion.Coerce(rawValue, targetType, this.ApiSchemaContext.TypeCoercionContext);
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region Implementation Methods
    private MemberAccessor RequireClrMemberAccessor(bool canRead)
    {
        var accessor = _clrMemberBinding.BoundClrMemberAccessor;
        var isAvailable = canRead ? accessor?.CanRead == true : accessor?.CanWrite == true;
        if (isAvailable)
        {
            return accessor!;
        }

        var operation = canRead ? "get" : "set";
        var accessorKind = canRead ? "getter" : "setter";
        throw new ApiSchemaException($"Cannot {operation} value for property '{this.ClrName}': no compiled {accessorKind} available. The CLR member may not support the operation, or the schema may not have compiled successfully.");
    }
    #endregion
}
