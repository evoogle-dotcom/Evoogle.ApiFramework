// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the principal end of an <see cref="ApiRelationship"/>.
///
///     The principal end identifies the object type that provides the primary key type used for relationship matching.
///     Key-bound relationships either select a named primary key explicitly or infer the compatible primary key type from the
///     corresponding foreign key binding.
/// </summary>
/// <param name="clrObjectType">The CLR type of the principal <see cref="ApiObjectType"/>.</param>
/// <param name="apiPrimaryKeyTypeName">
///     The optional name of the <see cref="ApiKeyType"/> on the principal type that serves as the join key.
///     When <see langword="null"/>, relationship initialization uses the foreign key binding, when present,
///     to infer the best compatible key type on the principal object type.
/// </param>
[JsonConverter(typeof(ApiRelationshipPrincipalEndJsonConverter))]
public sealed class ApiRelationshipPrincipalEnd(Type clrObjectType, string? apiPrimaryKeyTypeName = null) : ApiRelationshipEnd(clrObjectType)
{
    #region ApiRelationshipPrincipalEnd Fields
    private ApiKeyType? _apiResolvedPrimaryKeyType = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipPrincipalEnd);
    #endregion

    #region ApiRelationshipEnd Properties
    /// <inheritdoc/>
    public override ApiRelationshipEndKind ApiKind => ApiRelationshipEndKind.Principal;
    #endregion

    #region ApiRelationshipPrincipalEnd Properties
    /// <summary>
    ///     Gets the optional explicit primary key type name used to select a specific key type on the principal object type as the join key.
    ///     When <see langword="null"/>, key-bound relationships infer the best compatible principal primary key type from the
    ///     corresponding foreign key; navigational relationships have no key binding declared at the schema level.
    /// </summary>
    public string? ApiPrimaryKeyTypeName { get; } = apiPrimaryKeyTypeName;

    /// <summary>
    ///     Gets the resolved principal primary <see cref="ApiKeyType"/> used for relationship matching.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="IsNavigational"/> is <see langword="true"/>.
    ///     Check <see cref="HasPrimaryKey"/> before accessing this property.
    /// </exception>
    public ApiKeyType ApiPrimaryKeyType => this.HasPrimaryKey
        ? _apiResolvedPrimaryKeyType!
        : throw new ApiSchemaException("No primary key declared or resolved for this principal end of the relationship.");

    /// <summary>
    ///     Gets a value indicating whether this principal end has a resolved primary key declared explicitly or inferred from a foreign key.
    /// </summary>
    public bool HasPrimaryKey => _apiResolvedPrimaryKeyType is not null;

    /// <summary>
    ///     Gets a value indicating whether this principal end is navigational (i.e. has no resolved primary key at the schema level).
    /// </summary>
    public bool IsNavigational => !this.HasPrimaryKey;

    /// <summary>Gets the resolved principal primary <see cref="ApiKeyType"/>, or <see langword="null"/> if initialization failed or has not yet run.</summary>
    internal ApiKeyType? ResolvedPrimaryKeyType => _apiResolvedPrimaryKeyType;

    /// <summary>Gets the resolved <see cref="ApiObjectType"/> for this end, or <see langword="null"/> if initialization failed or has not yet run.</summary>
    internal ApiObjectType? ResolvedApiObjectType => this.ResolvedObjectType;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiPrimaryKeyTypeName = this.ApiPrimaryKeyTypeName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipPrincipalEnd)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiPrimaryKeyTypeName)}={apiPrimaryKeyTypeName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiPrimaryKeyType(context);
    }
    #endregion

    #region Implementation Methods
    /// <summary>Overrides the resolved primary key type; used by shape-match disambiguation during relationship initialization.</summary>
    internal void OverrideResolvedPrimaryKeyType(ApiKeyType keyType)
    {
        _apiResolvedPrimaryKeyType = keyType;
    }

    private void InitializeApiPrimaryKeyType(ApiInitializationContext context)
    {
        _apiResolvedPrimaryKeyType = null;

        // ApiObjectType is resolved by the base class. If it didn't resolve, we cannot proceed.
        var apiObjectType = this.ResolvedObjectType;
        if (apiObjectType is null)
        {
            return;
        }

        if (this.ApiPrimaryKeyTypeName is not null)
        {
            // Resolve by explicit primary key type name.
            if (apiObjectType.TryGetKeyTypeByApiName(this.ApiPrimaryKeyTypeName, out var apiResolvedPrimaryKeyType))
            {
                _apiResolvedPrimaryKeyType = apiResolvedPrimaryKeyType;
                return;
            }

            var availableKeyTypes = string.Join(", ", apiObjectType.GetKeyTypeApiNames().Select(k => $"'{k}'"));
            var remediation = !string.IsNullOrEmpty(availableKeyTypes)
                ? $"Use one of the available key types: {availableKeyTypes}"
                : $"Define a key type on '{apiObjectType.ApiName}' or remove {nameof(this.ApiPrimaryKeyTypeName)}";

            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_KEY_TYPE;
            var description = $"Referenced primary key type '{this.ApiPrimaryKeyTypeName}' could not be found on object type '{apiObjectType.ApiName}'";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        // No explicit primary key type name was supplied. Key-bound relationship initialization may infer
        // the compatible primary key type from the corresponding foreign key binding; navigational relationships
        // intentionally leave the principal primary key unresolved.
    }
    #endregion
}
