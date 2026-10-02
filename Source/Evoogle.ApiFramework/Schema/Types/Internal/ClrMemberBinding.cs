// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.Extensions;
using Evoogle.MemberAccess;

namespace Evoogle.ApiFramework.Schema.Types.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ClrMemberBinding(ClrMemberReference? clrMemberReference)
{
    #region Fields
    private bool _hasBindingAttempted;
    private MemberInfo? _boundClrMemberInfo;
    private MemberAccessor? _boundClrMemberAccessor;
    #endregion

    #region Properties
    public ClrMemberReference? ClrMemberReference { get; } = clrMemberReference;

    public MemberInfo? BoundClrMemberInfo => _boundClrMemberInfo;

    public MemberAccessor? BoundClrMemberAccessor => _boundClrMemberAccessor;

    public MemberInfo ClrMemberInfo => this.RequireValue(_boundClrMemberInfo);

    public MemberAccessor ClrMemberAccessor => this.RequireValue(_boundClrMemberAccessor);

    public Type ClrMemberType => this.ClrMemberInfo switch
    {
        PropertyInfo propertyInfo => propertyInfo.PropertyType,
        FieldInfo fieldInfo => fieldInfo.FieldType,
        _ => throw new ApiSchemaException("A CLR member binding must contain a property or field.")
    };
    #endregion

    #region Computed Properties
    public bool HasReference => this.ClrMemberReference is not null;

    public bool IsBound => _boundClrMemberInfo is not null;
    #endregion

    #region Methods
    /// <summary>Directly binds the specified CLR member.</summary>
    public void Bind(MemberInfo clrMemberInfo)
    {
        ArgumentNullException.ThrowIfNull(clrMemberInfo);

        if (this.HasReference)
        {
            throw new ApiSchemaConfigurationException($"A directly supplied CLR member cannot be bound when {nameof(this.ClrMemberReference)} is configured.");
        }

        if (!IsPublicInstancePropertyOrField(clrMemberInfo))
        {
            throw new ApiSchemaConfigurationException("A CLR member binding requires a public instance property or field.");
        }

        this.BeginBinding();
        _boundClrMemberInfo = clrMemberInfo;
        _boundClrMemberAccessor = MemberAccessor.Create(clrMemberInfo);
    }

    /// <summary>Prevents access through a resolved member that failed consumer-specific compatibility validation.</summary>
    public void InvalidateAccessor() => _boundClrMemberAccessor = null;

    /// <summary>Attempts to resolve and bind the configured CLR member reference.</summary>
    public bool TryResolveReference
    (
        Type clrObjectType,
        ApiSchemaCompilationContext context,
        string referenceName,
        bool requiresRead,
        bool requiresWrite,
        string? apiPath = null
    )
    {
        ArgumentNullException.ThrowIfNull(clrObjectType);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceName);
        apiPath ??= context.ApiPath;

        if (!this.HasReference)
        {
            throw new ApiSchemaConfigurationException($"An {nameof(this.ClrMemberReference)} must be configured before it can be resolved.");
        }

        this.BeginBinding();

        _boundClrMemberInfo = this.ClrMemberReference!.Resolve
        (
            clrObjectType,
            context,
            apiPath,
            referenceName
        );

        if (_boundClrMemberInfo is not null)
        {
            this.InitializeAccessor(context, apiPath, referenceName, requiresRead, requiresWrite);
        }

        return this.IsBound;
    }
    #endregion

    #region Implementation Methods
    private void BeginBinding()
    {
        if (_hasBindingAttempted)
        {
            throw new ApiSchemaConfigurationException($"An {nameof(ClrMemberBinding)} can only be bound once.");
        }

        _hasBindingAttempted = true;
    }

    private void InitializeAccessor
    (
        ApiSchemaCompilationContext context,
        string bindingApiPath,
        string referenceName,
        bool requiresRead,
        bool requiresWrite
    )
    {
        var clrMemberInfo = this.ClrMemberInfo;
        var clrMemberType = this.ClrMemberType;
        if (clrMemberType.IsByRefLike)
        {
            var apiPath = bindingApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ClrMemberIncompatible;
            var description = $"{referenceName} '{clrMemberInfo.Name}' has unsupported by-ref-like CLR type '{clrMemberType.SafeToName()}'";
            var remediation = "Bind a CLR member whose value can be boxed and accessed through object";

            context.AddIssue(apiPath, severity, code, description, remediation);
            return;
        }

        if (clrMemberInfo is PropertyInfo propertyInfo && propertyInfo.GetIndexParameters().Length > 0)
        {
            var apiPath = bindingApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ClrMemberIncompatible;
            var description = $"{referenceName} '{clrMemberInfo.Name}' is an indexer, which is not supported";
            var remediation = "Bind a non-indexed public instance CLR property or field";

            context.AddIssue(apiPath, severity, code, description, remediation);
            return;
        }

        try
        {
            _boundClrMemberAccessor = MemberAccessor.Create(clrMemberInfo);
        }
        catch (Exception exception)
        {
            var apiPath = bindingApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ClrMemberIncompatible;
            var description = $"{referenceName} '{clrMemberInfo.Name}' could not create a CLR member accessor: " + GetRootCauseMessage(exception);
            var remediation = "Bind a readable or writable public instance CLR property or field";

            context.AddIssue(apiPath, severity, code, description, remediation);
            return;
        }

        var clrMemberAccessor = this.ClrMemberAccessor;
        var hasRequiredAccess = true;
        if
        (
            requiresRead && !clrMemberAccessor.CanRead ||
            requiresWrite && !clrMemberAccessor.CanWrite
        )
        {
            var requiredAccess = requiresRead && requiresWrite
                ? "readable and writable"
                : requiresRead ? "readable" : "writable";
            var apiPath = bindingApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ClrMemberIncompatible;
            var description = $"{referenceName} '{clrMemberInfo.Name}' must be {requiredAccess}";
            var remediation = $"Bind a {requiredAccess} public instance CLR property or field";

            context.AddIssue(apiPath, severity, code, description, remediation);
            hasRequiredAccess = false;
        }

        var isGetterValid = ValidateGetter(context, bindingApiPath, referenceName, clrMemberAccessor);
        var isSetterValid = ValidateSetter(context, bindingApiPath, referenceName, clrMemberAccessor);
        if (!hasRequiredAccess || !isGetterValid || !isSetterValid)
        {
            _boundClrMemberAccessor = null;
        }
    }

    private static string GetGetterRootCauseMessage(Exception exception, Type clrMemberType)
    {
        if (clrMemberType.IsPointer)
        {
            return $"No coercion operator is defined between types '{clrMemberType}' and '{typeof(object)}'";
        }

        return GetRootCauseMessage(exception);
    }

    private static string GetRootCauseMessage(Exception exception) =>
        exception.GetBaseException().Message.TrimEnd('.');

    private static bool ValidateGetter
    (
        ApiSchemaCompilationContext context,
        string bindingApiPath,
        string referenceName,
        MemberAccessor clrMemberAccessor
    )
    {
        if (!clrMemberAccessor.CanRead)
        {
            return true;
        }

        try
        {
            MemberAccessorFactory.CreateGetter(clrMemberAccessor.MemberInfo);
        }
        catch (Exception exception)
        {
            var apiPath = bindingApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ClrMemberInvalidGetter;
            var rootCause = GetGetterRootCauseMessage(exception, clrMemberAccessor.MemberType);
            var description = $"Failed to compile getter for {referenceName} '{clrMemberAccessor.MemberName}': " + rootCause;
            var remediation = $"Verify that CLR member '{clrMemberAccessor.MemberName}' is readable and can be used in expression trees";

            context.AddIssue(apiPath, severity, code, description, remediation);
            return false;
        }

        return true;
    }

    private static bool ValidateSetter
    (
        ApiSchemaCompilationContext context,
        string bindingApiPath,
        string referenceName,
        MemberAccessor clrMemberAccessor
    )
    {
        if (!clrMemberAccessor.CanWrite || clrMemberAccessor.DeclaringType.IsValueType)
        {
            return true;
        }

        try
        {
            MemberAccessorFactory.CreateCoercingSetter(clrMemberAccessor.MemberInfo);
        }
        catch (Exception exception)
        {
            var apiPath = bindingApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ClrMemberInvalidSetter;
            var description = $"Failed to compile setter for {referenceName} '{clrMemberAccessor.MemberName}': " + GetRootCauseMessage(exception);
            var remediation = $"Verify that CLR member '{clrMemberAccessor.MemberName}' is writable and can be used in expression trees";

            context.AddIssue(apiPath, severity, code, description, remediation);
            return false;
        }

        return true;
    }

    private static bool IsPublicInstancePropertyOrField(MemberInfo clrMemberInfo) => clrMemberInfo switch
    {
        PropertyInfo propertyInfo =>
            propertyInfo.GetMethod?.IsPublic == true && propertyInfo.GetMethod.IsStatic == false ||
            propertyInfo.SetMethod?.IsPublic == true && propertyInfo.SetMethod.IsStatic == false,
        FieldInfo fieldInfo => fieldInfo.IsPublic && !fieldInfo.IsStatic,
        _ => false
    };
    #endregion
}
