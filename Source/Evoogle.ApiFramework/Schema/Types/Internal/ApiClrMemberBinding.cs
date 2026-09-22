// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;

namespace Evoogle.ApiFramework.Schema.Types.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiClrMemberBinding(ApiClrMemberReference? clrMemberReference)
{
    #region Fields
    private bool _hasBindingAttempted;
    private MemberInfo? _boundClrMemberInfo;
    #endregion

    #region Properties
    public ApiClrMemberReference? ClrMemberReference { get; } = clrMemberReference;

    public MemberInfo ClrMemberInfo => this.RequireValue(_boundClrMemberInfo);

    public MemberInfo? BoundClrMemberInfo => _boundClrMemberInfo;

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
    }

    /// <summary>Attempts to resolve and bind the configured CLR member reference.</summary>
    public bool TryResolveReference
    (
        Type clrObjectType,
        ApiSchemaCompilationContext context,
        ApiSchemaCompilationCode unresolvedCode,
        string referenceName
    )
    {
        ArgumentNullException.ThrowIfNull(clrObjectType);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceName);

        if (!this.HasReference)
        {
            throw new ApiSchemaConfigurationException($"An {nameof(this.ClrMemberReference)} must be configured before it can be resolved.");
        }

        this.BeginBinding();

        _boundClrMemberInfo = this.ClrMemberReference!.Resolve
        (
            clrObjectType,
            context,
            unresolvedCode,
            referenceName
        );
        return this.IsBound;
    }
    #endregion

    #region Implementation Methods
    private void BeginBinding()
    {
        if (_hasBindingAttempted)
        {
            throw new ApiSchemaConfigurationException($"An {nameof(ApiClrMemberBinding)} can only be bound once.");
        }

        _hasBindingAttempted = true;
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
