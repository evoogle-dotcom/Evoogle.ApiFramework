// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents an identity part that sources its value from the identity of the owning object in an owner-owned relationship.
///     This applies to both 1-to-many containment (e.g., Order → OrderLine) and 1-to-1 dependent identity (e.g., Person → PersonProfile).
/// </summary>
/// <param name="apiIdentityName">The optional explicit name of the identity to use on the owner object type. When <see langword="null"/>, the primary identity of the owner type is used.</param>
/// <remarks>
///     <para>
///         Owner resolution follows a two-phase initialization model. During the standard
///         <see cref="ApiSchemaElement.Initialize"/> call, no property-level resolution occurs for this
///         part type — unlike <see cref="ApiIdentityScalarPart"/> and <see cref="ApiIdentityNestedPart"/>
///         which resolve their backing properties in the standard phase.
///     </para>
///     <para>
///         Owner identity is instead resolved in a second schema-wide pass (via
///         <c>ApiSchema.ResolveOwnerIdentityParts</c>) that runs after all object types have been
///         initialized, because determining the owner requires inspecting the full object-type graph
///         for types that hold a collection or direct property of the owned type.
///     </para>
///     <para>
///         Accessing <see cref="ApiOwnerIdentity"/> or <see cref="ApiOwnerType"/> before schema
///         initialization completes will throw.
///     </para>
/// </remarks>
public sealed class ApiIdentityOwnerPart(string? apiIdentityName = null) : ApiIdentityPart
{
    #region ApiIdentityOwnerPart Fields
    private ApiIdentity? _apiResolvedOwnerIdentity = null;
    private ApiObjectType? _apiResolvedOwnerType = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiIdentityOwnerPart);
    #endregion

    #region ApiIdentityPart Properties
    /// <inheritdoc/>
    public override ApiIdentityPartKind ApiKind => ApiIdentityPartKind.Owner;
    #endregion

    #region ApiIdentityOwnerPart Properties
    /// <summary>
    ///     Gets the optional explicit identity name used to select a specific identity on the owner object type.
    ///     When <see langword="null"/>, the primary identity of the owner type is used.
    /// </summary>
    public string? ApiIdentityName { get; } = apiIdentityName;

    /// <summary>Gets the resolved owner <see cref="ApiIdentity"/> from the owning object type. Available after schema initialization.</summary>
    public ApiIdentity ApiOwnerIdentity => this.ThrowIfNotInitialized(_apiResolvedOwnerIdentity);

    /// <summary>Gets the resolved owner <see cref="ApiObjectType"/>. Available after schema initialization.</summary>
    public ApiObjectType ApiOwnerType => this.ThrowIfNotInitialized(_apiResolvedOwnerType);
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiIdentityName = this.ApiIdentityName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiIdentityOwnerPart)} {{{nameof(this.ApiIdentityName)}={apiIdentityName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: null);
    #endregion

    #region Implementation Methods
    internal void ResolveOwnerIdentity
    (
        ApiObjectType ownedType,
        List<ApiObjectType> candidateOwners,
        ApiInitializationContext context
    )
    {
        _apiResolvedOwnerIdentity = null;
        _apiResolvedOwnerType = null;

        if (candidateOwners.Count == 0)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_OWNER;
            var description = $"No owner object type was found for '{ownedType.ApiName}' — no other object type has a collection or direct object property of this type";
            var remediation = $"Ensure another {nameof(ApiObjectType)} has a collection or direct object property typed as '{ownedType.ApiName}', or remove the {nameof(ApiIdentityOwnerPart)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        if (candidateOwners.Count > 1)
        {
            var ownerNames = string.Join(", ", candidateOwners.Select(o => $"'{o.ApiName}'"));
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_PART_AMBIGUOUS_OWNER;
            var description = $"Multiple candidate owner object types found for '{ownedType.ApiName}': {ownerNames}";
            var remediation = $"Disambiguate by ensuring only one {nameof(ApiObjectType)} holds a collection or direct object property of '{ownedType.ApiName}'";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var ownerType = candidateOwners[0];

        if (this.ApiIdentityName is not null)
        {
            // Resolve by explicit name
            if (ownerType.TryGetIdentityByApiName(this.ApiIdentityName, out var namedIdentity))
            {
                _apiResolvedOwnerIdentity = namedIdentity;
                _apiResolvedOwnerType = ownerType;
            }
            else
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Error;
                var code = ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_OWNER;
                var availableIdentities = string.Join(", ", ownerType.ApiIdentities.Select(i => $"'{i.ApiName}'"));
                var description = $"Referenced identity '{this.ApiIdentityName}' could not be found on owner type '{ownerType.ApiName}'";
                var remediation = !string.IsNullOrEmpty(availableIdentities)
                    ? $"Use one of the available identities: {availableIdentities}"
                    : $"Define an identity on '{ownerType.ApiName}' or remove {nameof(this.ApiIdentityName)}";

                context.AddIssue(path, severity, code, description, remediation);
            }
        }
        else
        {
            // Use primary identity (first by convention)
            _apiResolvedOwnerIdentity = ownerType.ApiPrimaryIdentity;
            _apiResolvedOwnerType = ownerType;

            if (_apiResolvedOwnerIdentity is null)
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Error;
                var code = ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_OWNER;
                var description = $"Owner type '{ownerType.ApiName}' has no primary identity";
                var remediation = $"Define a primary identity on '{ownerType.ApiName}' or specify {nameof(this.ApiIdentityName)} explicitly";

                context.AddIssue(path, severity, code, description, remediation);
            }
        }
    }
    #endregion
}
