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
///     The principal end provides the key type used for relationship matching: its <see cref="ApiKeyType"/> uniquely identifies
///     the objects on this side and is referenced by the <see cref="ApiRelationshipDependentEnd"/>.
/// </summary>
/// <param name="clrObjectType">The CLR type of the principal <see cref="ApiObjectType"/>.</param>
/// <param name="apiIdentityName">
///     The optional name of the <see cref="ApiKeyType"/> on the principal type that serves as the join key.
///     When <see langword="null"/>, the primary key type (<see cref="ApiObjectType.ApiPrimaryKeyType"/>) is used.
/// </param>
[JsonConverter(typeof(ApiRelationshipPrincipalEndJsonConverter))]
public sealed class ApiRelationshipPrincipalEnd
(
    Type clrObjectType,
    string? apiIdentityName = null
) : ApiRelationshipEnd(clrObjectType)
{
    #region ApiRelationshipPrincipalEnd Fields
    private ApiKeyType? _apiResolvedIdentity = null;
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
    ///     When <see langword="null"/>, the primary key type of the principal type is used.
    /// </summary>
    public string? ApiIdentityName { get; } = apiIdentityName;

    /// <summary>
    ///     Gets the resolved principal <see cref="ApiKeyType"/> used for relationship matching. Available after initialization.
    /// </summary>
    public ApiKeyType ApiIdentity => this.ThrowIfNotInitialized(_apiResolvedIdentity);

    /// <summary>Gets the resolved principal <see cref="ApiKeyType"/>, or <see langword="null"/> if initialization failed or has not yet run.</summary>
    internal ApiKeyType? ResolvedIdentity => _apiResolvedIdentity;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiIdentityName = this.ApiIdentityName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipPrincipalEnd)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiIdentityName)}={apiIdentityName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiIdentity(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiIdentity(ApiInitializationContext context)
    {
        _apiResolvedIdentity = null;

        // ApiObjectType is resolved by the base class. If it didn't resolve, we cannot proceed.
        var apiObjectType = this.ResolvedObjectType;
        if (apiObjectType is null)
        {
            return;
        }

        if (this.ApiIdentityName is not null)
        {
            // Resolve by explicit key type name.
            if (apiObjectType.TryGetKeyTypeByApiName(this.ApiIdentityName, out var apiResolvedIdentity))
            {
                _apiResolvedIdentity = apiResolvedIdentity;
                return;
            }

            var availableIdentities = string.Join(", ", apiObjectType.ApiKeyTypes.Select(i => $"'{i.ApiName}'"));
            var remediation = !string.IsNullOrEmpty(availableIdentities)
                ? $"Use one of the available key types: {availableIdentities}"
                : $"Define a key type on '{apiObjectType.ApiName}' or remove {nameof(this.ApiIdentityName)}";

            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_IDENTITY;
            var description = $"Referenced key type '{this.ApiIdentityName}' could not be found on object type '{apiObjectType.ApiName}'";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        // Use primary key type by convention.
        _apiResolvedIdentity = apiObjectType.ApiPrimaryKeyType;

        if (_apiResolvedIdentity is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_IDENTITY;
            var description = $"Object type '{apiObjectType.ApiName}' has no primary key type and cannot act as a principal end";
            var remediation = $"Define a primary key type on '{apiObjectType.ApiName}' or specify {nameof(this.ApiIdentityName)} explicitly";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
