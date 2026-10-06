// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.MemberAccess;

namespace Evoogle.ApiFramework.Schema.Types.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiClrMemberAccessor
(
    MemberAccessor? memberAccessor,
    ApiSchemaContext schemaContext,
    ApiClrMemberAccessDescription description
)
{
    #region Fields
    private readonly MemberAccessor? _memberAccessor = memberAccessor;
    private readonly ApiSchemaContext _schemaContext = schemaContext;
    private readonly ApiClrMemberAccessDescription _description = description;
    #endregion

    #region Get Methods
    public object? GetValue(object clrObject, Type? clrValueType = null)
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        var clrObjectType = clrObject.GetType();
        var memberAccessor = this.RequireMemberAccessor(canRead: true, clrObjectType);

        try
        {
            return clrValueType is null
                ? memberAccessor.GetValue(target: clrObject)
                : memberAccessor.GetValue
                (
                    target: clrObject,
                    valueType: clrValueType,
                    coercion: this.SchemaContext.TypeCoercion,
                    context: this.SchemaContext.TypeCoercionContext
                );
        }
        catch (Exception exception)
        {
            throw this.CreateGetException(clrObjectType, exception);
        }
    }

    public TValue? GetValue<TObject, TValue>(TObject clrObject)
        where TObject : notnull
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        var clrObjectType = typeof(TObject);
        var memberAccessor = this.RequireMemberAccessor(canRead: true, clrObjectType);

        try
        {
            return typeof(TValue).IsAssignableFrom(memberAccessor.MemberType)
                ? memberAccessor.GetValue<TObject, TValue>(target: clrObject)
                : memberAccessor.GetValue<TObject, TValue>
                (
                    target: clrObject,
                    coercion: this.SchemaContext.TypeCoercion,
                    context: this.SchemaContext.TypeCoercionContext
                );
        }
        catch (Exception exception)
        {
            throw this.CreateGetException(clrObjectType, exception);
        }
    }
    #endregion

    #region Set Methods
    public void SetValue(object clrObject, object? clrValue)
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        var clrObjectType = clrObject.GetType();
        var memberAccessor = this.RequireMemberAccessor
        (
            canRead: false,
            clrObjectType,
            clrValue,
            isStruct: false
        );

        try
        {
            memberAccessor.SetValue
            (
                target: clrObject,
                value: clrValue,
                coercion: this.SchemaContext.TypeCoercion,
                context: this.SchemaContext.TypeCoercionContext
            );
        }
        catch (Exception exception)
        {
            throw this.CreateSetException(clrObjectType, clrValue, isStruct: false, exception);
        }
    }

    public void SetValue<TObject, TValue>(TObject clrObject, TValue? clrValue)
        where TObject : notnull
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        var clrObjectType = typeof(TObject);
        var memberAccessor = this.RequireMemberAccessor
        (
            canRead: false,
            clrObjectType,
            clrValue,
            isStruct: false
        );

        try
        {
            var clrValueType = typeof(TValue);
            if (memberAccessor.MemberType.IsAssignableFrom(clrValueType))
            {
                memberAccessor.SetValue(target: clrObject, value: clrValue);
            }
            else
            {
                memberAccessor.SetValue
                (
                    target: clrObject,
                    value: clrValue,
                    coercion: this.SchemaContext.TypeCoercion,
                    context: this.SchemaContext.TypeCoercionContext
                );
            }
        }
        catch (Exception exception)
        {
            throw this.CreateSetException(clrObjectType, clrValue, isStruct: false, exception);
        }
    }

    public void SetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue)
        where TObject : struct
    {
        var clrObjectType = typeof(TObject);
        var memberAccessor = this.RequireMemberAccessor
        (
            canRead: false,
            clrObjectType,
            clrValue,
            isStruct: true
        );

        try
        {
            var clrValueType = typeof(TValue);
            if (memberAccessor.MemberType.IsAssignableFrom(clrValueType))
            {
                memberAccessor.SetValueByRef(target: ref clrObject, value: clrValue);
            }
            else
            {
                memberAccessor.SetValueByRef
                (
                    target: ref clrObject,
                    value: clrValue,
                    coercion: this.SchemaContext.TypeCoercion,
                    context: this.SchemaContext.TypeCoercionContext
                );
            }
        }
        catch (Exception exception)
        {
            throw this.CreateSetException(clrObjectType, clrValue, isStruct: true, exception);
        }
    }
    #endregion

    #region TryGet Methods
    public bool TryGetValue(object? clrObject, out object? clrValue, Type? clrValueType = null)
    {
        if (clrObject is null || _memberAccessor?.CanRead != true)
        {
            clrValue = default;
            return false;
        }

        return clrValueType is null
            ? _memberAccessor.TryGetValue
            (
                target: clrObject,
                value: out clrValue,
                valueType: null,
                coercion: null,
                context: null
            )
            : _memberAccessor.TryGetValue
            (
                target: clrObject,
                value: out clrValue,
                valueType: clrValueType,
                coercion: this.SchemaContext.TypeCoercion,
                context: this.SchemaContext.TypeCoercionContext
            );
    }

    public bool TryGetValue<TObject, TValue>(TObject? clrObject, out TValue? clrValue)
    {
        if (clrObject is null || _memberAccessor?.CanRead != true)
        {
            clrValue = default;
            return false;
        }

        return typeof(TValue).IsAssignableFrom(_memberAccessor.MemberType)
            ? _memberAccessor.TryGetValue(target: clrObject, value: out clrValue)
            : _memberAccessor.TryGetValue
            (
                target: clrObject,
                value: out clrValue,
                coercion: this.SchemaContext.TypeCoercion,
                context: this.SchemaContext.TypeCoercionContext
            );
    }
    #endregion

    #region TrySet Methods
    public bool TrySetValue(object? clrObject, object? clrValue) =>
        clrObject is not null &&
        _memberAccessor?.CanWrite == true &&
        _memberAccessor.TrySetValue
        (
            target: clrObject,
            value: clrValue,
            coercion: this.SchemaContext.TypeCoercion,
            context: this.SchemaContext.TypeCoercionContext
        );

    public bool TrySetValue<TObject, TValue>(TObject? clrObject, TValue? clrValue)
    {
        if (clrObject is null || _memberAccessor?.CanWrite != true)
        {
            return false;
        }

        var clrValueType = typeof(TValue);
        return _memberAccessor.MemberType.IsAssignableFrom(clrValueType)
            ? _memberAccessor.TrySetValue(target: clrObject, value: clrValue)
            : _memberAccessor.TrySetValue
            (
                target: clrObject,
                value: clrValue,
                coercion: this.SchemaContext.TypeCoercion,
                context: this.SchemaContext.TypeCoercionContext
            );
    }

    public bool TrySetValueByRef<TObject, TValue>(ref TObject clrObject, TValue? clrValue)
        where TObject : struct
    {
        if (_memberAccessor?.CanWrite != true)
        {
            return false;
        }

        var clrValueType = typeof(TValue);
        return _memberAccessor.MemberType.IsAssignableFrom(clrValueType)
            ? _memberAccessor.TrySetValueByRef(target: ref clrObject, value: clrValue)
            : _memberAccessor.TrySetValueByRef
            (
                target: ref clrObject,
                value: clrValue,
                coercion: this.SchemaContext.TypeCoercion,
                context: this.SchemaContext.TypeCoercionContext
            );
    }
    #endregion

    #region Implementation Methods
    private ApiSchemaException CreateGetException(Type clrObjectType, Exception exception)
    {
        var message = $"Failed to get value for {_description.Subject} from object of type " +
            $"'{clrObjectType.SafeToName()}': {exception.Message}";
        return new ApiSchemaException(message, exception);
    }

    private ApiSchemaContext SchemaContext => _schemaContext;

    private ApiSchemaException CreateSetException(Type clrObjectType, object? clrValue, bool isStruct, Exception exception)
    {
        var clrObjectKind = isStruct ? "struct" : "object";
        var clrValueTypeName = clrValue?.GetType().SafeToName() ?? "null";
        var message = $"Failed to set value for {_description.Subject} on {clrObjectKind} of type " +
            $"'{clrObjectType.SafeToName()}' with value of type '{clrValueTypeName}': {exception.Message}";
        return new ApiSchemaException(message, exception);
    }

    private MemberAccessor RequireMemberAccessor
    (
        bool canRead,
        Type clrObjectType,
        object? clrValue = null,
        bool isStruct = false
    )
    {
        var isAvailable = canRead ? _memberAccessor?.CanRead == true : _memberAccessor?.CanWrite == true;
        if (isAvailable)
        {
            return _memberAccessor!;
        }

        var operation = canRead ? "get" : "set";
        var accessorKind = canRead ? "getter" : "setter";
        var exception = new ApiSchemaException(_description.CreateUnavailableMessage(operation, accessorKind));
        if (!_description.WrapUnavailableException)
        {
            throw exception;
        }

        throw canRead
            ? this.CreateGetException(clrObjectType, exception)
            : this.CreateSetException(clrObjectType, clrValue, isStruct, exception);
    }
    #endregion
}
