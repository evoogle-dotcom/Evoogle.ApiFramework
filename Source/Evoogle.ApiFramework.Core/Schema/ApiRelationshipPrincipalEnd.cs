// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the principal end of an <see cref="ApiRelationship"/>.
///     The principal end owns the join key: its <see cref="ApiIdentity"/> uniquely identifies
///     the objects on this side and is referenced by the <see cref="ApiRelationshipDependentEnd"/>.
/// </summary>
/// <param name="apiObjectTypeName">The API name of the principal <see cref="ApiObjectType"/>.</param>
/// <param name="apiIdentityName">
///     The optional name of the <see cref="ApiIdentity"/> on the principal type that serves as the join key.
///     When <see langword="null"/>, the primary identity (<see cref="ApiObjectType.ApiPrimaryIdentity"/>) is used.
/// </param>
/// <param name="apiDeleteBehavior">
///     The delete behavior applied to dependent objects when an object on this end is deleted.
///     Defaults to <see cref="ApiRelationshipDeleteBehavior.None"/>.
/// </param>
public sealed class ApiRelationshipPrincipalEnd
(
    string apiObjectTypeName,
    string? apiIdentityName = null,
    ApiRelationshipDeleteBehavior apiDeleteBehavior = ApiRelationshipDeleteBehavior.None
) : ApiRelationshipEnd(apiObjectTypeName, apiDeleteBehavior)
{
    #region ApiRelationshipPrincipalEnd Fields
    private ApiIdentity? _apiResolvedIdentity = null;
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
    ///     Gets the optional explicit identity name used to select a specific identity on the principal object type as the join key.
    ///     When <see langword="null"/>, the primary identity of the principal type is used.
    ///     Follows the same null-means-primary convention as <see cref="ApiIdentityNestedPart.ApiIdentityName"/>
    ///     and <see cref="ApiIdentityOwnerPart.ApiIdentityName"/>.
    /// </summary>
    public string? ApiIdentityName { get; } = apiIdentityName;

    /// <summary>
    ///     Gets the resolved join-key <see cref="ApiIdentity"/> from the principal object type. Available after initialization.
    /// </summary>
    public ApiIdentity ApiIdentity => this.ThrowIfNotInitialized(_apiResolvedIdentity);

    /// <summary>
    ///     Gets the strongly-typed dependent end of the relationship. Available after initialization.
    /// </summary>
    public ApiRelationshipDependentEnd ApiDependentEnd => (ApiRelationshipDependentEnd)this.ApiOppositeEnd;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiObjectTypeName = this.ApiObjectTypeName.SafeToString();
        var apiIdentityName = this.ApiIdentityName.SafeToString();
        var apiDeleteBehavior = this.ApiDeleteBehavior.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipPrincipalEnd)} {{{nameof(this.ApiObjectTypeName)}={apiObjectTypeName}, {nameof(this.ApiIdentityName)}={apiIdentityName}, {nameof(this.ApiDeleteBehavior)}={apiDeleteBehavior}, {nameof(this.ExtensionCount)}={extensionCount}}}";
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
            // Resolve by explicit name — same pattern as ApiIdentityNestedPart.InitializeApiIdentity.
            if (apiObjectType.TryGetIdentityByApiName(this.ApiIdentityName, out var apiResolvedIdentity))
            {
                _apiResolvedIdentity = apiResolvedIdentity;
                return;
            }

            var availableIdentities = string.Join(", ", apiObjectType.ApiIdentities.Select(i => $"'{i.ApiName}'"));
            var remediation = !string.IsNullOrEmpty(availableIdentities)
                ? $"Use one of the available identities: {availableIdentities}"
                : $"Define an identity on '{apiObjectType.ApiName}' or remove {nameof(this.ApiIdentityName)}";

            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_IDENTITY,
                $"Referenced identity '{this.ApiIdentityName}' could not be found on object type '{apiObjectType.ApiName}'",
                remediation);
            return;
        }

        // Use primary identity by convention.
        _apiResolvedIdentity = apiObjectType.ApiPrimaryIdentity;

        if (_apiResolvedIdentity is null)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_IDENTITY,
                $"Object type '{apiObjectType.ApiName}' has no primary identity and cannot act as a principal end",
                $"Define a primary identity on '{apiObjectType.ApiName}' or specify {nameof(this.ApiIdentityName)} explicitly");
        }
    }
    #endregion
}
