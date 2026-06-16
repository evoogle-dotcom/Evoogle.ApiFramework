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
///     The principal end identifies the object type that provides the principal key type used for relationship matching.
///     Key-bound relationships either select a named principal key explicitly or infer the compatible principal key type from the
///     corresponding foreign key binding.
/// </summary>
/// <param name="clrObjectType">The CLR type of the principal <see cref="ApiObjectType"/>.</param>
/// <param name="apiKeyTypeName">
///     The optional name of the <see cref="ApiKeyType"/> on the principal type that serves as the join key.
///     When <see langword="null"/>, relationship initialization uses the foreign key binding, when present,
///     to infer the best compatible key type on the principal object type.
/// </param>
[JsonConverter(typeof(ApiRelationshipPrincipalEndJsonConverter))]
public sealed class ApiRelationshipPrincipalEnd(Type clrObjectType, string? apiKeyTypeName = null) : ApiRelationshipEnd(clrObjectType)
{
    #region ApiRelationshipPrincipalEnd Fields
    private ApiKeyType? _apiResolvedPrincipalKeyType = null;
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
    ///     Gets the optional explicit principal key type name used to select a specific key type on the principal object type as the join key.
    ///     When <see langword="null"/>, key-bound relationships infer the best compatible principal key type from the
    ///     corresponding foreign key; navigational relationships have no key binding declared at the schema level.
    /// </summary>
    public string? ApiPrincipalKeyTypeName { get; } = apiKeyTypeName;

    /// <summary>
    ///     Gets the resolved principal <see cref="ApiKeyType"/> used for relationship matching.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="IsNavigational"/> is <see langword="true"/>.
    ///     Check <see cref="HasPrincipalKey"/> before accessing this property.
    /// </exception>
    public ApiKeyType ApiPrincipalKeyType => this.HasPrincipalKey
        ? _apiResolvedPrincipalKeyType!
        : throw new ApiSchemaException("No principal key declared or resolved for this principal end of the relationship.");

    /// <summary>
    ///     Gets a value indicating whether this principal end has a resolved principal key declared explicitly or inferred from a foreign key.
    /// </summary>
    public bool HasPrincipalKey => _apiResolvedPrincipalKeyType is not null;

    /// <summary>
    ///     Gets a value indicating whether this principal end is navigational (i.e. has no resolved principal key at the schema level).
    /// </summary>
    public bool IsNavigational => !this.HasPrincipalKey;

    /// <summary>Gets the resolved principal <see cref="ApiKeyType"/>, or <see langword="null"/> if initialization failed or has not yet run.</summary>
    internal ApiKeyType? ResolvedPrincipalKeyType => _apiResolvedPrincipalKeyType;

    /// <summary>Gets the resolved <see cref="ApiObjectType"/> for this end, or <see langword="null"/> if initialization failed or has not yet run.</summary>
    internal ApiObjectType? ResolvedApiObjectType => this.ResolvedObjectType;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiKeyTypeName = this.ApiPrincipalKeyTypeName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipPrincipalEnd)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiPrincipalKeyTypeName)}={apiKeyTypeName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiPrincipalKeyType(context);
    }
    #endregion

    #region Implementation Methods
    /// <summary>Overrides the resolved principal key type; used by shape-match disambiguation during relationship initialization.</summary>
    internal void OverrideResolvedPrincipalKeyType(ApiKeyType keyType)
    {
        _apiResolvedPrincipalKeyType = keyType;
    }

    private void InitializeApiPrincipalKeyType(ApiInitializationContext context)
    {
        _apiResolvedPrincipalKeyType = null;

        // ApiObjectType is resolved by the base class. If it didn't resolve, we cannot proceed.
        var apiObjectType = this.ResolvedObjectType;
        if (apiObjectType is null)
        {
            return;
        }

        if (this.ApiPrincipalKeyTypeName is not null)
        {
            // Resolve by explicit principal key type name.
            if (apiObjectType.TryGetKeyTypeByApiName(this.ApiPrincipalKeyTypeName, out var apiResolvedPrincipalKeyType))
            {
                _apiResolvedPrincipalKeyType = apiResolvedPrincipalKeyType;
                return;
            }

            var availableKeyTypes = string.Join(", ", apiObjectType.GetKeyTypeApiNames().Select(k => $"'{k}'"));
            var remediation = !string.IsNullOrEmpty(availableKeyTypes)
                ? $"Use one of the available key types: {availableKeyTypes}"
                : $"Define a key type on '{apiObjectType.ApiName}' or remove {nameof(this.ApiPrincipalKeyTypeName)}";

            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_KEY_TYPE;
            var description = $"Referenced principal key type '{this.ApiPrincipalKeyTypeName}' could not be found on object type '{apiObjectType.ApiName}'";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        // No explicit principal key type name was supplied. Key-bound relationship initialization may infer
        // the compatible principal key type from the corresponding foreign key binding; navigational relationships
        // intentionally leave the principal key unresolved.
    }
    #endregion
}
