// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Internal;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>Identifies one public instance CLR property or field for later schema compilation.</summary>
/// <remarks>
///     The name and member kind form one complete identity.
///
///     A schema component represents an optional CLR binding by making this reference nullable,
///     rather than by making either part of the identity nullable.
/// </remarks>
[JsonConverter(typeof(ApiClrMemberReferenceJsonConverter))]
public sealed class ApiClrMemberReference : IEquatable<ApiClrMemberReference>
{
    #region Fields
    private readonly ClrMemberKind? _clrKind;
    private readonly bool _hasInvalidClrKind;
    #endregion

    #region Properties
    /// <summary>Gets the CLR member name.</summary>
    public string ClrName { get; }

    /// <summary>Gets the CLR member kind.</summary>
    public ClrMemberKind ClrKind => this.RequireValue(_clrKind);
    #endregion

    #region Constructors
    /// <summary>Creates a CLR member reference.</summary>
    /// <param name="clrName">The CLR member name.</param>
    /// <param name="clrKind">The CLR member kind.</param>
    public ApiClrMemberReference(string clrName, ClrMemberKind clrKind)
        : this(clrName, clrKind, false)
    {
    }

    internal ApiClrMemberReference(string clrName, ClrMemberKind? clrKind, bool hasInvalidClrKind)
    {
        this.ClrName = clrName;
        _clrKind = clrKind;
        _hasInvalidClrKind = hasInvalidClrKind;
    }
    #endregion

    #region Equality Methods
    /// <inheritdoc/>
    public bool Equals(ApiClrMemberReference? other) => other is not null
        && ClrNameComparer.Instance.Equals(this.ClrName, other.ClrName)
        && _clrKind == other._clrKind;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as ApiClrMemberReference);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine
    (
        this.ClrName is null ? 0 : ClrNameComparer.Instance.GetHashCode(this.ClrName),
        _clrKind
    );

    /// <summary>Determines whether two references identify the same CLR member.</summary>
    public static bool operator ==(ApiClrMemberReference? left, ApiClrMemberReference? right) =>
        EqualityComparer<ApiClrMemberReference>.Default.Equals(left, right);

    /// <summary>Determines whether two references identify different CLR members.</summary>
    public static bool operator !=(ApiClrMemberReference? left, ApiClrMemberReference? right) =>
        !(left == right);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrName = this.ClrName.SafeToString();
        var clrKind = _clrKind.SafeToString();
        return $"{nameof(ApiClrMemberReference)} {{{nameof(this.ClrName)}={clrName}, {nameof(this.ClrKind)}={clrKind}}}";
    }
    #endregion

    #region Resolve Methods
    internal MemberInfo? Resolve
    (
        Type clrObjectType,
        ApiSchemaCompilationContext context,
        ApiSchemaCompilationCode unresolvedCode,
        string referenceName
    )
    {
        ArgumentNullException.ThrowIfNull(clrObjectType);
        ArgumentNullException.ThrowIfNull(context);

        // Validate the CLR member name and kind before attempting to resolve the reference.
        var isClrNameInvalid = !this.ValidateClrName(context);
        var isClrKindInvalid = !this.ValidateClrKind(context);
        if (isClrNameInvalid || isClrKindInvalid)
        {
            return null;
        }

        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        MemberInfo? clrMemberInfo = this.ClrKind switch
        {
            ClrMemberKind.Property => clrObjectType.GetProperty(this.ClrName, bindingFlags),
            ClrMemberKind.Field => clrObjectType.GetField(this.ClrName, bindingFlags),
            _ => null
        };
        if (clrMemberInfo is not null)
        {
            return clrMemberInfo;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = unresolvedCode;
        var description = $"{referenceName} could not resolve CLR {this.ClrKind.ToString().ToLowerInvariant()} '{this.ClrName}' on CLR type '{clrObjectType}'";
        var remediation = $"Reference an existing public instance CLR {this.ClrKind.ToString().ToLowerInvariant()}";

        context.AddIssue(severity, code, description, remediation);
        return null;
    }
    #endregion

    #region Implementation Methods
    private bool ValidateClrName(ApiSchemaCompilationContext context)
    {
        var isClrNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ClrName);
        if (isClrNameInvalid)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiClrMemberReferenceInvalidClrName;
            var description = $"{nameof(this.ClrName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ClrName)} value";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        return true;
    }

    private bool ValidateClrKind(ApiSchemaCompilationContext context)
    {
        if (_hasInvalidClrKind || _clrKind is ClrMemberKind clrKind && !Enum.IsDefined(clrKind))
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiClrMemberReferenceInvalidClrKind;
            var description = $"{nameof(this.ClrKind)} must be a valid {nameof(Types.ClrMemberKind)} value";
            var remediation = $"Specify a valid {nameof(this.ClrKind)} value";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        return true;
    }
    #endregion
}
