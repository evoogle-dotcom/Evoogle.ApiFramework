// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Types.Internal;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>Provides schema-aware convenience methods for accessing the property's CLR member.</summary>
/// <remarks>
///     These methods forward to a Core member accessor. Generic operations use direct access when their types are
///     assignment compatible and otherwise use the schema's configured type coercion and coercion context.
/// </remarks>
public sealed partial class ApiProperty
{
    #region Properties
    private ApiClrMemberAccessor? ClrValueAccessor { get; set; }
    #endregion

    #region Get Methods
    /// <summary>Gets the member value, optionally coercing it to <paramref name="clrValueType"/>.</summary>
    public object? GetValue(object clrObject, Type? clrValueType = null)
        => this.RequireClrValueAccessor(GetClrObjectType(clrObject)).GetValue(clrObject, clrValueType);

    /// <summary>Gets the member value using the requested generic types.</summary>
    public TValue? GetValue<TObject, TValue>(TObject clrObject) where TObject : notnull
        => this.RequireClrValueAccessor(typeof(TObject)).GetValue<TObject, TValue>(clrObject);
    #endregion

    #region Set Methods
    /// <summary>Sets the member value, coercing it when needed.</summary>
    public void SetValue(object clrObject, object? clrValue)
        => this.RequireClrValueAccessor(GetClrObjectType(clrObject)).SetValue(clrObject, clrValue);

    /// <summary>Sets the member value using the requested generic types.</summary>
    public void SetValue<TObject, TValue>(TObject clrObject, TValue? clrValue) where TObject : notnull
        => this.RequireClrValueAccessor(typeof(TObject)).SetValue(clrObject, clrValue);

    /// <summary>Sets a struct member by reference using the requested generic types.</summary>
    public void SetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue) where TObject : struct
        => this.RequireClrValueAccessor(typeof(TObject)).SetValueByRef(ref clrObject, clrValue);
    #endregion

    #region TryGet Methods
    /// <summary>Attempts to get the member value, optionally coercing it to <paramref name="clrValueType"/>.</summary>
    public bool TryGetValue(object? clrObject, out object? clrValue, Type? clrValueType = null)
    {
        if (this.ClrValueAccessor is not { } clrValueAccessor)
        {
            clrValue = default;
            return false;
        }

        return clrValueAccessor.TryGetValue(clrObject, out clrValue, clrValueType);
    }

    /// <summary>Attempts to get the member value using the requested generic types.</summary>
    public bool TryGetValue<TObject, TValue>(TObject? clrObject, out TValue? clrValue)
    {
        if (this.ClrValueAccessor is not { } clrValueAccessor)
        {
            clrValue = default;
            return false;
        }

        return clrValueAccessor.TryGetValue(clrObject, out clrValue);
    }
    #endregion

    #region TrySet Methods
    /// <summary>Attempts to set the member value, coercing it when needed.</summary>
    public bool TrySetValue(object? clrObject, object? clrValue)
        => this.ClrValueAccessor is { } clrValueAccessor && clrValueAccessor.TrySetValue(clrObject, clrValue);

    /// <summary>Attempts to set the member value using the requested generic types.</summary>
    public bool TrySetValue<TObject, TValue>(TObject? clrObject, TValue? clrValue)
        => this.ClrValueAccessor is { } clrValueAccessor && clrValueAccessor.TrySetValue(clrObject, clrValue);

    /// <summary>Attempts to set a struct member by reference using the requested generic types.</summary>
    public bool TrySetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue) where TObject : struct
        => this.ClrValueAccessor is { } clrValueAccessor && clrValueAccessor.TrySetValueByRef(ref clrObject, clrValue);
    #endregion

    #region Initialization Methods
    private void InitializeClrValueAccessor(ApiSchemaContext schemaContext)
    {
        ArgumentNullException.ThrowIfNull(schemaContext);

        this.ThrowIfFrozen();
        if (this.ClrValueAccessor is not null)
        {
            throw new ApiSchemaConfigurationException($"The {nameof(this.ClrValueAccessor)} can only be initialized once.");
        }

        var memberAccessor = _clrValueMemberBinding.BoundClrMemberAccessor;
        var description = ApiClrMemberAccessDescription.ForProperty(this.ClrName);

        this.ClrValueAccessor = new ApiClrMemberAccessor(memberAccessor, schemaContext, description);
    }

    private ApiClrMemberAccessor RequireClrValueAccessor(Type clrObjectType)
    {
        if (this.ClrValueAccessor is not null)
        {
            return this.ClrValueAccessor;
        }

        throw new ApiSchemaException
        (
            $"Cannot access property '{this.ClrName}' on an object of type " +
            $"'{clrObjectType.FullName}': the CLR value accessor has not been initialized by schema compilation."
        );
    }

    private static Type GetClrObjectType(object clrObject)
    {
        ArgumentNullException.ThrowIfNull(clrObject);
        return clrObject.GetType();
    }
    #endregion
}
