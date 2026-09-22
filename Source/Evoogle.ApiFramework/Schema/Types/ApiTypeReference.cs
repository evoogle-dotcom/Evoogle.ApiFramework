// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Internal;
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
    public bool IsApiNamedReference => this.ApiKind is not null && !string.IsNullOrWhiteSpace(this.ApiName)
        && this.ClrType is null;

    /// <summary>Gets a value indicating whether this is a complete CLR-backed reference.</summary>
    public bool IsClrTypeReference => this.ApiKind is null && this.ApiName is null
        && this.ClrType is not null;

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
    /// <summary>Creates an API-named reference.</summary>
    /// <param name="apiKind">The expected kind of the referenced API type.</param>
    /// <param name="apiName">The API name of the referenced type.</param>
    /// <returns>A new API-named reference.</returns>
    public static ApiTypeReference ApiRef(ApiTypeKind apiKind, string apiName) => new(apiKind, apiName);

    /// <summary>Creates a CLR-backed reference.</summary>
    /// <param name="clrType">The CLR type of the referenced API type.</param>
    /// <returns>A new CLR-backed reference.</returns>
    public static ApiTypeReference ClrRef(Type clrType) => new(clrType);

    /// <summary>Creates a CLR-backed reference for <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The CLR type to reference.</typeparam>
    /// <returns>A new CLR-backed reference.</returns>
    public static ApiTypeReference ClrRef<T>() => new(typeof(T));
    #endregion

    #region Equality Methods
    /// <inheritdoc/>
    public bool Equals(ApiTypeReference? other) => other is not null
        && this.ApiKind == other.ApiKind
        && ApiNameComparer.Instance.Equals(this.ApiName, other.ApiName)
        && this.ClrType == other.ClrType;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as ApiTypeReference);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine
    (
        this.ApiKind,
        this.ApiName is null ? 0 : ApiNameComparer.Instance.GetHashCode(this.ApiName),
        this.ClrType
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

    #region Resolve Methods
    internal ApiType? Resolve
    (
        ApiSchemaCompilationContext context,
        ApiSchemaCompilationCode unresolvedCode,
        string referenceName
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        // Validate the API kind if it was specified no matter the type of reference.
        // This way it gets validated so developers receive immediate feedback if the API kind is incorrect.
        if (!this.ValidateApiKind(context))
        {
            return null;
        }

        // Attempt to resolve the reference as a CLR type if applicable.
        if (this.IsClrTypeReference)
        {
            // Attempt to resolve the reference using the CLR type.
            if (context.ApiSchema.TryGetTypeByClrType(this.ClrType!, out var apiType))
            {
                return apiType;
            }

            // If the CLR-typed reference could not be resolved, add an unresolved issue.
            var clrTypeIdentity = $"{nameof(this.ClrType)}='{this.ClrType.SafeToName()}'";
            this.AddUnresolvedIssue
            (
                context,
                unresolvedCode,
                referenceName,
                clrTypeIdentity
            );
            return null;
        }

        // Attempt to resolve the reference as an API-named type if applicable.
        if (this.IsApiNamedReference)
        {
            // Attempt to resolve the API-named type based on the API kind.
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

            // If the API-named reference could not be resolved, add an unresolved issue.
            var apiNamedIdentity = $"{nameof(this.ApiKind)}='{this.ApiKind.SafeToString()}' and {nameof(this.ApiName)}='{this.ApiName.SafeToString()}'";
            this.AddUnresolvedIssue
            (
                context,
                unresolvedCode,
                referenceName,
                apiNamedIdentity
            );
            return null;
        }

        // If the reference is neither a valid CLR type nor a valid API-named type, it is considered invalid.
        this.AddInvalidFormIssue(context);
        return null;
    }
    #endregion

    #region Implementation Methods
    private void AddInvalidFormIssue(ApiSchemaCompilationContext context)
    {
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiTypeReferenceInvalidForm;
        var description = $"A type reference must specify exactly one complete API-named reference "
            + $"({nameof(this.ApiKind)} and {nameof(this.ApiName)}) or CLR reference "
            + $"({nameof(this.ClrType)})";
        var remediation = $"Specify either {nameof(this.ApiKind)} and {nameof(this.ApiName)}, or "
            + $"{nameof(this.ClrType)}, but not both";

        context.AddIssue(severity, code, description, remediation);
    }

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

    private bool ValidateApiKind(ApiSchemaCompilationContext context)
    {
        if (_hasInvalidApiKind || this.ApiKind is ApiTypeKind apiKind && !Enum.IsDefined(apiKind))
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiTypeReferenceInvalidApiKind;
            var description = $"{nameof(this.ApiKind)} must be a valid {nameof(ApiTypeKind)} value";
            var remediation = $"Specify a valid {nameof(this.ApiKind)} value";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        return true;
    }
    #endregion
}
