// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Internal;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>Identifies a declared <see cref="ApiProperty"/> by API name or CLR name.</summary>
/// <remarks>
///     Exactly one identity form is active. The reference is resolved relative to an
///     <see cref="ApiObjectType"/> during schema compilation; CLR member-kind metadata remains
///     owned by the resolved property declaration.
/// </remarks>
[JsonConverter(typeof(ApiPropertyReferenceJsonConverter))]
public sealed class ApiPropertyReference : IEquatable<ApiPropertyReference>
{
    #region Properties
    /// <summary>Gets the API name when this is an API-named reference.</summary>
    public string? ApiName { get; }

    /// <summary>Gets the CLR name when this is a CLR-named reference.</summary>
    public string? ClrName { get; }
    #endregion

    #region Computed Properties
    /// <summary>Gets whether this reference identifies a property by API name.</summary>
    public bool IsApiNamedReference => this.ApiName is not null && this.ClrName is null;

    /// <summary>Gets whether this reference identifies a property by CLR name.</summary>
    public bool IsClrNamedReference => this.ClrName is not null && this.ApiName is null;

    internal string ReferenceLabel => this.ApiName ?? this.ClrName ?? throw new ApiSchemaException("Both API name and CLR name are null for the property reference.");
    #endregion

    #region Constructors
    internal ApiPropertyReference(string? apiName, string? clrName)
    {
        this.ApiName = apiName;
        this.ClrName = clrName;
    }
    #endregion

    #region Factory Methods
    /// <summary>Creates a property reference using an API name.</summary>
    /// <param name="apiName">The API property name.</param>
    /// <returns>A new API-named property reference.</returns>
    public static ApiPropertyReference ApiRef(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        return new(apiName, null);
    }

    /// <summary>Creates a property reference using a CLR name.</summary>
    /// <param name="clrName">The CLR property or field name.</param>
    /// <returns>A new CLR-named property reference.</returns>
    public static ApiPropertyReference ClrRef(string clrName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrName);
        return new(null, clrName);
    }
    #endregion

    #region Equality Methods
    /// <inheritdoc/>
    public bool Equals(ApiPropertyReference? other) => other is not null
        && ApiNameComparer.Instance.Equals(this.ApiName, other.ApiName)
        && ClrNameComparer.Instance.Equals(this.ClrName, other.ClrName);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as ApiPropertyReference);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine
    (
        this.ApiName is null ? 0 : ApiNameComparer.Instance.GetHashCode(this.ApiName),
        this.ClrName is null ? 0 : ClrNameComparer.Instance.GetHashCode(this.ClrName)
    );

    /// <summary>Determines whether two references identify the same API property.</summary>
    public static bool operator ==(ApiPropertyReference? left, ApiPropertyReference? right) =>
        EqualityComparer<ApiPropertyReference>.Default.Equals(left, right);

    /// <summary>Determines whether two references identify different API properties.</summary>
    public static bool operator !=(ApiPropertyReference? left, ApiPropertyReference? right) =>
        !(left == right);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrName = this.ClrName.SafeToString();
        return $"{nameof(ApiPropertyReference)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ClrName)}={clrName}}}";
    }
    #endregion

    #region Resolve Methods
    internal ApiProperty? Resolve
    (
        ApiObjectType apiObjectType,
        ApiSchemaCompilationContext context,
        ApiSchemaCompilationCode unresolvedCode,
        string referenceName
    )
    {
        ArgumentNullException.ThrowIfNull(apiObjectType);
        ArgumentNullException.ThrowIfNull(context);

        // Attempt to resolve the property reference by API name if applicable.
        if (this.IsApiNamedReference)
        {
            // Attempt to resolve by API name
            if (apiObjectType.TryGetPropertyByApiName(this.ApiName!, out var apiProperty))
            {
                return apiProperty;
            }

            // If the API-named reference could not be resolved, add an unresolved issue.
            var identity = $"{nameof(this.ApiName)}='{this.ApiName.SafeToString}'";
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = unresolvedCode;
            var description = $"{referenceName} could not be resolved for {identity} on object type '{apiObjectType.ApiName}'";
            var remediation = $"Reference a declared {nameof(ApiProperty)} on '{apiObjectType.ApiName}'";

            context.AddIssue(severity, code, description, remediation);
            return null;
        }

        // Attempt to resolve the property reference by CLR name if applicable.
        if (this.IsClrNamedReference)
        {
            // Attempt to resolve by CLR name
            if (apiObjectType.TryGetPropertyByClrName(this.ClrName!, out var apiProperty))
            {
                return apiProperty;
            }

            // If the CLR-named reference could not be resolved, add an unresolved issue.
            var identity = $"{nameof(this.ClrName)}='{this.ClrName.SafeToString()}'";
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = unresolvedCode;
            var description = $"{referenceName} could not be resolved for {identity} on object type '{apiObjectType.ApiName}'";
            var remediation = $"Reference a declared {nameof(ApiProperty)} on '{apiObjectType.ApiName}'";

            context.AddIssue(severity, code, description, remediation);
            return null;
        }

        // If the property reference is neither API-named nor CLR-named, it is invalid.
        this.AddInvalidFormIssue(context);
        return null;
    }
    #endregion

    #region Implementation Methods
    private void AddInvalidFormIssue(ApiSchemaCompilationContext context)
    {
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiPropertyReferenceInvalidForm;
        var description = $"An API property reference must specify either {nameof(this.ApiName)} or {nameof(this.ClrName)}";
        var remediation = $"Specify either {nameof(this.ApiName)}, or {nameof(this.ClrName)}, but not both";

        context.AddIssue(severity, code, description, remediation);
    }
    #endregion
}
