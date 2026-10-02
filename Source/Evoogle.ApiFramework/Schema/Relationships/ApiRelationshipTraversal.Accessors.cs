// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.MemberAccess;

namespace Evoogle.ApiFramework.Schema.Relationships;

/// <summary>Provides schema-aware convenience methods for accessing the traversal's CLR navigation member.</summary>
/// <remarks>
///     These methods access only the configured CLR navigation member. Reading a value does not establish whether
///     the relationship was loaded. Generic operations use direct access when their types are assignment compatible
///     and otherwise use the schema's configured type coercion and coercion context.
/// </remarks>
public sealed partial class ApiRelationshipTraversal
{
    #region Get Methods
    /// <summary>Gets the navigation member value, optionally coercing it to <paramref name="clrValueType"/>.</summary>
    public object? GetValue(object clrObject, Type? clrValueType = null)
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        try
        {
            var accessor = this.RequireClrNavigationMemberAccessor(canRead: true);
            return clrValueType is null
                ? accessor.GetValue(clrObject)
                : accessor.GetValue
                (
                    clrObject,
                    clrValueType,
                    this.ApiSchemaContext.TypeCoercion,
                    this.ApiSchemaContext.TypeCoercionContext
                );
        }
        catch (Exception exception)
        {
            var message = $"Failed to get value for traversal '{this.ApiName}' CLR navigation member " +
                $"'{this.GetClrNavigationMemberName()}' from object of type '{clrObject.GetType().SafeToName()}': " +
                exception.Message;
            throw new ApiSchemaException(message, exception);
        }
    }

    /// <summary>Gets the navigation member value using the requested generic types.</summary>
    public TValue? GetValue<TObject, TValue>(TObject clrObject)
        where TObject : notnull
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        try
        {
            var accessor = this.RequireClrNavigationMemberAccessor(canRead: true);
            return typeof(TValue).IsAssignableFrom(accessor.MemberType)
                ? accessor.GetValue<TObject, TValue>(clrObject)
                : accessor.GetValue<TObject, TValue>
                (
                    clrObject,
                    this.ApiSchemaContext.TypeCoercion,
                    this.ApiSchemaContext.TypeCoercionContext
                );
        }
        catch (Exception exception)
        {
            var message = $"Failed to get value for traversal '{this.ApiName}' CLR navigation member " +
                $"'{this.GetClrNavigationMemberName()}' from object of type '{typeof(TObject).SafeToName()}': " +
                exception.Message;
            throw new ApiSchemaException(message, exception);
        }
    }
    #endregion

    #region Set Methods
    /// <summary>Sets the navigation member value, coercing it when needed.</summary>
    public void SetValue(object clrObject, object? clrValue)
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        try
        {
            var accessor = this.RequireClrNavigationMemberAccessor(canRead: false);
            accessor.SetValue
            (
                clrObject,
                clrValue,
                this.ApiSchemaContext.TypeCoercion,
                this.ApiSchemaContext.TypeCoercionContext
            );
        }
        catch (Exception exception)
        {
            var valueTypeName = clrValue?.GetType().SafeToName() ?? "null";
            var message = $"Failed to set value for traversal '{this.ApiName}' CLR navigation member " +
                $"'{this.GetClrNavigationMemberName()}' on object of type '{clrObject.GetType().SafeToName()}' with value " +
                $"of type '{valueTypeName}': {exception.Message}";
            throw new ApiSchemaException(message, exception);
        }
    }

    /// <summary>Sets the navigation member value using the requested generic types.</summary>
    public void SetValue<TObject, TValue>(TObject clrObject, TValue? clrValue)
        where TObject : notnull
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        try
        {
            var accessor = this.RequireClrNavigationMemberAccessor(canRead: false);
            if (accessor.MemberType.IsAssignableFrom(typeof(TValue)))
            {
                accessor.SetValue(clrObject, clrValue);
            }
            else
            {
                accessor.SetValue
                (
                    clrObject,
                    clrValue,
                    this.ApiSchemaContext.TypeCoercion,
                    this.ApiSchemaContext.TypeCoercionContext
                );
            }
        }
        catch (Exception exception)
        {
            var valueTypeName = clrValue?.GetType().SafeToName() ?? "null";
            var message = $"Failed to set value for traversal '{this.ApiName}' CLR navigation member " +
                $"'{this.GetClrNavigationMemberName()}' on object of type '{typeof(TObject).SafeToName()}' with value " +
                $"of type '{valueTypeName}': {exception.Message}";
            throw new ApiSchemaException(message, exception);
        }
    }

    /// <summary>Sets a struct navigation member by reference using the requested generic types.</summary>
    public void SetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue)
        where TObject : struct
    {
        try
        {
            var accessor = this.RequireClrNavigationMemberAccessor(canRead: false);
            if (accessor.MemberType.IsAssignableFrom(typeof(TValue)))
            {
                accessor.SetValueByRef(ref clrObject, clrValue);
            }
            else
            {
                accessor.SetValueByRef
                (
                    ref clrObject,
                    clrValue,
                    this.ApiSchemaContext.TypeCoercion,
                    this.ApiSchemaContext.TypeCoercionContext
                );
            }
        }
        catch (Exception exception)
        {
            var valueTypeName = clrValue?.GetType().SafeToName() ?? "null";
            var message = $"Failed to set value for traversal '{this.ApiName}' CLR navigation member " +
                $"'{this.GetClrNavigationMemberName()}' on struct of type '{typeof(TObject).SafeToName()}' with value " +
                $"of type '{valueTypeName}': {exception.Message}";
            throw new ApiSchemaException(message, exception);
        }
    }
    #endregion

    #region TryGet Methods
    /// <summary>Attempts to get the navigation member value, optionally coercing it.</summary>
    public bool TryGetValue(object? clrObject, out object? clrValue, Type? clrValueType = null)
    {
        var accessor = _clrNavigationMemberBinding.BoundClrMemberAccessor;
        if (clrObject is null || accessor?.CanRead != true)
        {
            clrValue = default;
            return false;
        }

        return clrValueType is null
            ? accessor.TryGetValue(clrObject, out clrValue, valueType: null, coercion: null, context: null)
            : accessor.TryGetValue
            (
                clrObject,
                out clrValue,
                clrValueType,
                this.ApiSchemaContext.TypeCoercion,
                this.ApiSchemaContext.TypeCoercionContext
            );
    }

    /// <summary>Attempts to get the navigation member value using the requested generic types.</summary>
    public bool TryGetValue<TObject, TValue>(TObject? clrObject, out TValue? clrValue)
    {
        var accessor = _clrNavigationMemberBinding.BoundClrMemberAccessor;
        if (clrObject is null || accessor?.CanRead != true)
        {
            clrValue = default;
            return false;
        }

        return typeof(TValue).IsAssignableFrom(accessor.MemberType)
            ? accessor.TryGetValue(clrObject, out clrValue)
            : accessor.TryGetValue
            (
                clrObject,
                out clrValue,
                this.ApiSchemaContext.TypeCoercion,
                this.ApiSchemaContext.TypeCoercionContext
            );
    }
    #endregion

    #region TrySet Methods
    /// <summary>Attempts to set the navigation member value, coercing it when needed.</summary>
    public bool TrySetValue(object? clrObject, object? clrValue)
    {
        var accessor = _clrNavigationMemberBinding.BoundClrMemberAccessor;
        return clrObject is not null && accessor?.CanWrite == true && accessor.TrySetValue
        (
            clrObject,
            clrValue,
            this.ApiSchemaContext.TypeCoercion,
            this.ApiSchemaContext.TypeCoercionContext
        );
    }

    /// <summary>Attempts to set the navigation member value using the requested generic types.</summary>
    public bool TrySetValue<TObject, TValue>(TObject? clrObject, TValue? clrValue)
    {
        var accessor = _clrNavigationMemberBinding.BoundClrMemberAccessor;
        if (clrObject is null || accessor?.CanWrite != true)
        {
            return false;
        }

        return accessor.MemberType.IsAssignableFrom(typeof(TValue))
            ? accessor.TrySetValue(clrObject, clrValue)
            : accessor.TrySetValue
            (
                clrObject,
                clrValue,
                this.ApiSchemaContext.TypeCoercion,
                this.ApiSchemaContext.TypeCoercionContext
            );
    }

    /// <summary>Attempts to set a struct navigation member by reference using the requested generic types.</summary>
    public bool TrySetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue)
        where TObject : struct
    {
        var accessor = _clrNavigationMemberBinding.BoundClrMemberAccessor;
        if (accessor?.CanWrite != true)
        {
            return false;
        }

        return accessor.MemberType.IsAssignableFrom(typeof(TValue))
            ? accessor.TrySetValueByRef(ref clrObject, clrValue)
            : accessor.TrySetValueByRef
            (
                ref clrObject,
                clrValue,
                this.ApiSchemaContext.TypeCoercion,
                this.ApiSchemaContext.TypeCoercionContext
            );
    }
    #endregion

    #region Implementation Methods
    private string GetClrNavigationMemberName() => this.ClrNavigationMember?.ClrName ?? "<unbound>";

    private MemberAccessor RequireClrNavigationMemberAccessor(bool canRead)
    {
        var accessor = _clrNavigationMemberBinding.BoundClrMemberAccessor;
        var isAvailable = canRead ? accessor?.CanRead == true : accessor?.CanWrite == true;
        if (isAvailable)
        {
            return accessor!;
        }

        var operation = canRead ? "get" : "set";
        var accessorKind = canRead ? "getter" : "setter";
        throw new ApiSchemaException
        (
            $"Cannot {operation} value for traversal '{this.ApiName}' CLR navigation member '{this.GetClrNavigationMemberName()}': " +
            $"no compiled {accessorKind} is available. The traversal may not have a CLR navigation member reference, the CLR " +
            "navigation member may not support the operation, or the schema may not have compiled successfully."
        );
    }
    #endregion
}
