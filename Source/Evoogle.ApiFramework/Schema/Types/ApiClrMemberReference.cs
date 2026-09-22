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
    private readonly ClrMemberKind? _clrMemberKind;
    private readonly bool _hasInvalidClrMemberKind;
    #endregion

    #region Properties
    /// <summary>Gets the CLR member name.</summary>
    public string ClrMemberName { get; }

    /// <summary>Gets the CLR member kind.</summary>
    public ClrMemberKind ClrMemberKind => this.RequireValue(_clrMemberKind);
    #endregion

    #region Constructors
    /// <summary>Creates a CLR member reference.</summary>
    /// <param name="clrMemberName">The CLR member name.</param>
    /// <param name="clrMemberKind">The CLR member kind.</param>
    public ApiClrMemberReference(string clrMemberName, ClrMemberKind clrMemberKind)
        : this(clrMemberName, clrMemberKind, false)
    {
    }

    internal ApiClrMemberReference(string clrMemberName, ClrMemberKind? clrMemberKind, bool hasInvalidClrMemberKind)
    {
        this.ClrMemberName = clrMemberName;
        _clrMemberKind = clrMemberKind;
        _hasInvalidClrMemberKind = hasInvalidClrMemberKind;
    }
    #endregion

    #region Equality Methods
    /// <inheritdoc/>
    public bool Equals(ApiClrMemberReference? other) => other is not null
        && ClrNameComparer.Instance.Equals(this.ClrMemberName, other.ClrMemberName)
        && _clrMemberKind == other._clrMemberKind;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as ApiClrMemberReference);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine
    (
        this.ClrMemberName is null ? 0 : ClrNameComparer.Instance.GetHashCode(this.ClrMemberName),
        _clrMemberKind
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
        var clrMemberName = this.ClrMemberName.SafeToString();
        var clrMemberKind = _clrMemberKind.SafeToString();
        return $"{nameof(ApiClrMemberReference)} {{{nameof(this.ClrMemberName)}={clrMemberName}, {nameof(this.ClrMemberKind)}={clrMemberKind}}}";
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
        var isClrMemberNameInvalid = !this.ValidateClrMemberName(context);
        var isClrMemberKindInvalid = !this.ValidateClrMemberKind(context);
        if (isClrMemberNameInvalid || isClrMemberKindInvalid)
        {
            return null;
        }

        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        MemberInfo? clrMemberInfo = this.ClrMemberKind switch
        {
            ClrMemberKind.Property => clrObjectType.GetProperty(this.ClrMemberName, bindingFlags),
            ClrMemberKind.Field => clrObjectType.GetField(this.ClrMemberName, bindingFlags),
            _ => null
        };
        if (clrMemberInfo is not null)
        {
            return clrMemberInfo;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = unresolvedCode;
        var description = $"{referenceName} could not resolve CLR {this.ClrMemberKind.ToString().ToLowerInvariant()} '{this.ClrMemberName}' on CLR type '{clrObjectType}'";
        var remediation = $"Reference an existing public instance CLR {this.ClrMemberKind.ToString().ToLowerInvariant()}";

        context.AddIssue(severity, code, description, remediation);
        return null;
    }
    #endregion

    #region Implementation Methods
    private bool ValidateClrMemberName(ApiSchemaCompilationContext context)
    {
        var isClrMemberNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ClrMemberName);
        if (isClrMemberNameInvalid)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiClrMemberReferenceInvalidClrMemberName;
            var description = $"{nameof(this.ClrMemberName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ClrMemberName)} value";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        return true;
    }

    private bool ValidateClrMemberKind(ApiSchemaCompilationContext context)
    {
        if (_hasInvalidClrMemberKind || _clrMemberKind is ClrMemberKind clrMemberKind && !Enum.IsDefined(clrMemberKind))
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiClrMemberReferenceInvalidClrMemberKind;
            var description = $"{nameof(this.ClrMemberKind)} must be a valid {nameof(Types.ClrMemberKind)} value";
            var remediation = $"Specify a valid {nameof(this.ClrMemberKind)} value";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        return true;
    }
    #endregion
}
