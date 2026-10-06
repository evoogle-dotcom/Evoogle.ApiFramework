// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Types.Internal;

namespace Evoogle.ApiFramework.Schema.Relationships;

/// <summary>Provides schema-aware convenience methods for accessing the traversal's CLR navigation member.</summary>
/// <remarks>
///     These methods access only the configured CLR navigation member. Reading a value does not establish whether
///     the relationship was loaded. Generic operations use direct access when their types are assignment compatible
///     and otherwise use the schema's configured type coercion and coercion context.
/// </remarks>
public sealed partial class ApiRelationshipTraversal
{
    #region Properties
    private ApiClrMemberAccessor? ClrNavigationAccessor { get; set; }
    #endregion

    #region Get Methods
    /// <summary>Gets the navigation member value, optionally coercing it to <paramref name="clrValueType"/>.</summary>
    public object? GetValue(object clrObject, Type? clrValueType = null)
        => this.RequireClrNavigationAccessor(GetClrObjectType(clrObject)).GetValue(clrObject, clrValueType);

    /// <summary>Gets the navigation member value using the requested generic types.</summary>
    public TValue? GetValue<TObject, TValue>(TObject clrObject) where TObject : notnull
        => this.RequireClrNavigationAccessor(typeof(TObject)).GetValue<TObject, TValue>(clrObject);
    #endregion

    #region Set Methods
    /// <summary>Sets the navigation member value, coercing it when needed.</summary>
    public void SetValue(object clrObject, object? clrValue)
        => this.RequireClrNavigationAccessor(GetClrObjectType(clrObject), clrValue).SetValue(clrObject, clrValue);

    /// <summary>Sets the navigation member value using the requested generic types.</summary>
    public void SetValue<TObject, TValue>(TObject clrObject, TValue? clrValue) where TObject : notnull
        => this.RequireClrNavigationAccessor(typeof(TObject), clrValue).SetValue(clrObject, clrValue);

    /// <summary>Sets a struct navigation member by reference using the requested generic types.</summary>
    public void SetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue) where TObject : struct
        => this.RequireClrNavigationAccessor(typeof(TObject), clrValue).SetValueByRef(ref clrObject, clrValue);
    #endregion

    #region TryGet Methods
    /// <summary>Attempts to get the navigation member value, optionally coercing it.</summary>
    public bool TryGetValue(object? clrObject, out object? clrValue, Type? clrValueType = null)
    {
        if (this.ClrNavigationAccessor is not { } clrNavigationAccessor)
        {
            clrValue = default;
            return false;
        }

        return clrNavigationAccessor.TryGetValue(clrObject, out clrValue, clrValueType);
    }

    /// <summary>Attempts to get the navigation member value using the requested generic types.</summary>
    public bool TryGetValue<TObject, TValue>(TObject? clrObject, out TValue? clrValue)
    {
        if (this.ClrNavigationAccessor is not { } clrNavigationAccessor)
        {
            clrValue = default;
            return false;
        }

        return clrNavigationAccessor.TryGetValue(clrObject, out clrValue);
    }
    #endregion

    #region TrySet Methods
    /// <summary>Attempts to set the navigation member value, coercing it when needed.</summary>
    public bool TrySetValue(object? clrObject, object? clrValue)
        => this.ClrNavigationAccessor is { } clrNavigationAccessor && clrNavigationAccessor.TrySetValue(clrObject, clrValue);

    /// <summary>Attempts to set the navigation member value using the requested generic types.</summary>
    public bool TrySetValue<TObject, TValue>(TObject? clrObject, TValue? clrValue)
        => this.ClrNavigationAccessor is { } clrNavigationAccessor && clrNavigationAccessor.TrySetValue(clrObject, clrValue);

    /// <summary>Attempts to set a struct navigation member by reference using the requested generic types.</summary>
    public bool TrySetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue) where TObject : struct
        => this.ClrNavigationAccessor is { } clrNavigationAccessor && clrNavigationAccessor.TrySetValueByRef(ref clrObject, clrValue);
    #endregion

    #region Initialization Methods
    private void InitializeClrNavigationAccessor(ApiSchemaContext schemaContext)
    {
        ArgumentNullException.ThrowIfNull(schemaContext);

        this.ThrowIfFrozen();
        if (this.ClrNavigationAccessor is not null)
        {
            throw new ApiSchemaConfigurationException($"The {nameof(this.ClrNavigationAccessor)} can only be initialized once.");
        }

        var memberAccessor = _clrNavigationMemberBinding.BoundClrMemberAccessor;
        var description = ApiClrMemberAccessDescription.ForTraversal(this.ApiName, this.ClrName ?? "<unbound>");

        this.ClrNavigationAccessor = new ApiClrMemberAccessor(memberAccessor, schemaContext, description);
    }

    private ApiClrMemberAccessor RequireClrNavigationAccessor(Type clrObjectType, object? clrValue = null)
    {
        if (this.ClrNavigationAccessor is not null)
        {
            return this.ClrNavigationAccessor;
        }

        var clrValueTypeName = clrValue?.GetType().FullName ?? "null";
        throw new ApiSchemaException
        (
            $"Cannot access traversal '{this.ApiName}' CLR navigation member " +
            $"'{this.ClrName ?? "<unbound>"}' on an object of type '{clrObjectType.FullName}': " +
            $"the CLR navigation accessor has not been initialized by schema compilation; value type is " +
            $"'{clrValueTypeName}'."
        );
    }

    private static Type GetClrObjectType(object clrObject)
    {
        ArgumentNullException.ThrowIfNull(clrObject);
        return clrObject.GetType();
    }
    #endregion
}
