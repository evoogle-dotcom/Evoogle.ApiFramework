// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the principal end of an <see cref="ApiRelationship"/>.
///
///     The principal end provides the key type used for relationship matching:
///     its <see cref="ApiKeyType"/> uniquely identifies the objects on this side and is referenced
///     by the <see cref="ApiRelationshipDependentEnd"/>.
/// </summary>
/// <param name="clrObjectType">The CLR type of the principal <see cref="ApiObjectType"/>.</param>
/// <param name="apiKeyTypeName">
///     The optional name of the <see cref="ApiKeyType"/> on the principal type that serves as the join key.
///     When <see langword="null"/>, relationship initialization uses the foreign key binding, when present,
///     to infer the best compatible key type on the principal object type.
/// </param>
[JsonConverter(typeof(ApiRelationshipPrincipalEndJsonConverter))]
public sealed class ApiRelationshipPrincipalEnd
(
    Type clrObjectType,
    string? apiKeyTypeName = null
) : ApiRelationshipEnd(clrObjectType)
{
    #region ApiRelationshipPrincipalEnd Fields
    private ApiKeyType? _apiResolvedKeyType = null;
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
    ///     Gets the optional explicit key type name used to select a specific key type on the principal object type as the join key.
    ///     When <see langword="null"/>, key-bound relationships infer the best compatible principal key type from the
    ///     corresponding foreign key; navigational relationships use the principal type's primary key by convention.
    /// </summary>
    public string? ApiKeyTypeName { get; } = apiKeyTypeName;

    /// <summary>
    ///     Gets the resolved principal <see cref="ApiKeyType"/> used for relationship matching. Available after initialization.
    /// </summary>
    public ApiKeyType ApiKeyType => this.ThrowIfNotInitialized(_apiResolvedKeyType);

    /// <summary>Gets the resolved principal <see cref="ApiKeyType"/>, or <see langword="null"/> if initialization failed or has not yet run.</summary>
    internal ApiKeyType? ResolvedKeyType => _apiResolvedKeyType;

    /// <summary>Gets the resolved <see cref="ApiObjectType"/> for this end, or <see langword="null"/> if initialization failed or has not yet run.</summary>
    internal ApiObjectType? ResolvedApiObjectType => this.ResolvedObjectType;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiKeyTypeName = this.ApiKeyTypeName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipPrincipalEnd)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiKeyTypeName)}={apiKeyTypeName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiKeyType(context);
    }
    #endregion

    #region Implementation Methods
    /// <summary>Overrides the resolved key type; used by shape-match disambiguation during relationship initialization.</summary>
    internal void OverrideResolvedKeyType(ApiKeyType keyType)
    {
        _apiResolvedKeyType = keyType;
    }

    private void InitializeApiKeyType(ApiInitializationContext context)
    {
        _apiResolvedKeyType = null;

        // ApiObjectType is resolved by the base class. If it didn't resolve, we cannot proceed.
        var apiObjectType = this.ResolvedObjectType;
        if (apiObjectType is null)
        {
            return;
        }

        if (this.ApiKeyTypeName is not null)
        {
            // Resolve by explicit key type name.
            if (apiObjectType.TryGetKeyTypeByApiName(this.ApiKeyTypeName, out var apiResolvedKeyType))
            {
                _apiResolvedKeyType = apiResolvedKeyType;
                return;
            }

            var availableKeyTypes = string.Join(", ", apiObjectType.ApiKeyTypes.Keys.Select(k => $"'{k}'"));
            var remediation = !string.IsNullOrEmpty(availableKeyTypes)
                ? $"Use one of the available key types: {availableKeyTypes}"
                : $"Define a key type on '{apiObjectType.ApiName}' or remove {nameof(this.ApiKeyTypeName)}";

            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_KEY_TYPE;
            var description = $"Referenced key type '{this.ApiKeyTypeName}' could not be found on object type '{apiObjectType.ApiName}'";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        // Use primary key type by convention. Key-bound relationships may override this during alignment
        // when another principal key type is the best compatible match for the foreign key.
        _apiResolvedKeyType = apiObjectType.ApiPrimaryKeyType;

        if (_apiResolvedKeyType is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_KEY_TYPE;
            var description = $"Object type '{apiObjectType.ApiName}' has no primary key type and cannot act as a principal end";
            var remediation = $"Define a primary key type on '{apiObjectType.ApiName}' or specify {nameof(this.ApiKeyTypeName)} explicitly";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
