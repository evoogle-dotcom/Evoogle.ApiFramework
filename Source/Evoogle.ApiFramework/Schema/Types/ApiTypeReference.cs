// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Identifies a declared API type by either its API kind and name or its CLR type.
/// </summary>
[JsonConverter(typeof(ApiTypeReferenceJsonConverter))]
public sealed class ApiTypeReference : IEquatable<ApiTypeReference>
{
    #region Fields
    private readonly bool _hasInvalidApiKind;
    #endregion

    #region Properties
    /// <summary>Gets the kind of an API-named reference.</summary>
    public ApiTypeKind? ApiKind { get; }

    /// <summary>Gets the name of an API-named reference.</summary>
    public string? ApiName { get; }

    /// <summary>Gets the CLR type of a CLR-backed reference.</summary>
    public Type? ClrType { get; }
    #endregion

    #region Computed Properties
    /// <summary>Gets a value indicating whether this is a complete API-named reference.</summary>
    public bool IsApiNamedReference =>
        this.ApiKind is not null &&
        !string.IsNullOrWhiteSpace(this.ApiName) &&
        this.ClrType is null;

    /// <summary>Gets a value indicating whether this is a complete CLR-backed reference.</summary>
    public bool IsClrTypeReference =>
        this.ApiKind is null &&
        this.ApiName is null &&
        this.ClrType is not null;

    internal string? ApiReferenceLabel => this.IsApiNamedReference
        ? this.ApiName
        : this.ClrType?.Name;
    #endregion

    #region Constructors
    /// <summary>Creates an API-named reference.</summary>
    /// <param name="apiKind">The expected kind of the referenced API type.</param>
    /// <param name="apiName">The API name of the referenced type.</param>
    public ApiTypeReference(ApiTypeKind apiKind, string apiName)
        : this(apiKind, apiName, null, false)
    {
    }

    /// <summary>Creates a CLR-backed reference.</summary>
    /// <param name="clrType">The CLR type of the referenced API type.</param>
    public ApiTypeReference(Type clrType)
        : this
        (
            null,
            null,
            clrType is not null && TypeReflection.IsNullableType(clrType) ? Nullable.GetUnderlyingType(clrType) : clrType,
            false
        )
    {
    }

    internal ApiTypeReference
    (
        ApiTypeKind? apiKind,
        string? apiName,
        Type? clrType,
        bool hasInvalidApiKind
    )
    {
        this.ApiKind = apiKind;
        this.ApiName = apiName;
        this.ClrType = clrType;
        _hasInvalidApiKind = hasInvalidApiKind;
    }
    #endregion

    #region Factory Methods
    /// <summary>Creates a CLR-backed reference for <typeparamref name="TClr"/>.</summary>
    /// <typeparam name="TClr">The CLR type to reference.</typeparam>
    /// <returns>A new CLR-backed reference.</returns>
    public static ApiTypeReference ClrRef<TClr>() => new(typeof(TClr));
    #endregion

    #region Internal Methods
    internal ApiType? Resolve
    (
        ApiSchemaCompilationContext context,
        ApiSchemaCompilationCode parentUnresolvedCode,
        string parentUnresolvedName
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        // Validate the API kind before attempting to resolve the reference.
        if
        (
            _hasInvalidApiKind ||
            this.ApiKind is ApiTypeKind apiKind && !Enum.IsDefined(apiKind)
        )
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiTypeReferenceInvalidApiKind;
            var description = $"{nameof(this.ApiKind)} must be a valid {nameof(ApiTypeKind)} value";
            var remediation = $"Specify a valid {nameof(this.ApiKind)} value";

            context.AddIssue(severity, code, description, remediation);
            return null;
        }

        // Ensure that the reference is either an API-named reference or a CLR type reference.
        if (!this.IsApiNamedReference && !this.IsClrTypeReference)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiTypeReferenceInvalidForm;
            var description = $"A type reference must specify exactly one complete API-named reference "
                + $"({nameof(this.ApiKind)} and {nameof(this.ApiName)}) or CLR reference "
                + $"({nameof(this.ClrType)})";
            var remediation = $"Specify either {nameof(this.ApiKind)} and {nameof(this.ApiName)}, or "
                + $"{nameof(this.ClrType)}, but not both";

            context.AddIssue(severity, code, description, remediation);
            return null;
        }

        // Attempt to resolve the reference as a CLR type if applicable.
        if (this.IsClrTypeReference)
        {
            if (context.ApiSchema.TryGetTypeByClrType(this.ClrType!, out var apiType))
            {
                return apiType;
            }

            this.AddUnresolvedIssue
            (
                context,
                parentUnresolvedCode,
                parentUnresolvedName,
                $"{nameof(this.ClrType)}='{this.ClrType.SafeToName()}'"
            );
            return null;
        }

        // Attempt to resolve the reference as an API-named type if applicable.
        ApiType? apiNamedType = this.ApiKind switch
        {
            ApiTypeKind.Scalar => context.ApiSchema.TryGetScalarTypeByApiName(this.ApiName!, out var apiScalarType) ? apiScalarType : null,
            ApiTypeKind.Enum => context.ApiSchema.TryGetEnumTypeByApiName(this.ApiName!, out var apiEnumType) ? apiEnumType : null,
            ApiTypeKind.Object => context.ApiSchema.TryGetObjectTypeByApiName(this.ApiName!, out var apiObjectType) ? apiObjectType : null,
            ApiTypeKind.Collection => null,
            _ => null,
        };

        // Return the resolved API-named type if found.
        if (apiNamedType is not null)
        {
            return apiNamedType;
        }

        // If the API-named type could not be resolved, add an unresolved issue.
        var identity = $"{nameof(this.ApiKind)}='{this.ApiKind.SafeToString()}' and {nameof(this.ApiName)}='{this.ApiName.SafeToString()}'";
        this.AddUnresolvedIssue(context, parentUnresolvedCode, parentUnresolvedName, identity);
        return null;
    }
    #endregion

    #region Equality Methods
    /// <inheritdoc/>
    public bool Equals(ApiTypeReference? other) =>
        other is not null &&
        this.ApiKind == other.ApiKind &&
        string.Equals(this.ApiName, other.ApiName, StringComparison.Ordinal) &&
        this.ClrType == other.ClrType &&
        _hasInvalidApiKind == other._hasInvalidApiKind;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as ApiTypeReference);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine
    (
        this.ApiKind,
        this.ApiName is null ? 0 : StringComparer.Ordinal.GetHashCode(this.ApiName),
        this.ClrType,
        _hasInvalidApiKind
    );

    /// <summary>Determines whether two references have equal declaration identities.</summary>
    public static bool operator ==(ApiTypeReference? left, ApiTypeReference? right) =>
        EqualityComparer<ApiTypeReference>.Default.Equals(left, right);

    /// <summary>Determines whether two references have different declaration identities.</summary>
    public static bool operator !=(ApiTypeReference? left, ApiTypeReference? right) => !(left == right);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiKind = this.ApiKind.SafeToString();
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToName();
        return $"{nameof(ApiTypeReference)} {{{nameof(this.ApiKind)}={apiKind}, {nameof(this.ApiName)}={apiName}, {nameof(this.ClrType)}={clrType}}}";
    }
    #endregion

    #region Implementation Methods
    private void AddUnresolvedIssue
    (
        ApiSchemaCompilationContext context,
        ApiSchemaCompilationCode code,
        string referenceName,
        string identity
    )
    {
        var collectionDetail = this.ApiKind == ApiTypeKind.Collection
            ? $" because {nameof(ApiTypeKind.Collection)} types must be defined inline"
            : string.Empty;

        var severity = ApiSchemaCompilationSeverity.Error;
        var description = $"{referenceName} could not be resolved for {identity}{collectionDetail}";
        var remediation = $"Verify that a compatible type is declared in the schema for {identity}";

        context.AddIssue(severity, code, description, remediation);
    }
    #endregion
}
