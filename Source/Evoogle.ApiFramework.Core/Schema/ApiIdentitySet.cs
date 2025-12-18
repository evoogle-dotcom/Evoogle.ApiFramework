// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a collection of identities for an <see cref="ApiObjectType"/>, including a primary identity.
/// </summary>
/// <remarks>
///     An object type can have multiple identities (e.g., primary key, alternate keys),
///     with one designated as the primary identity.
/// </remarks>
/// <param name="apiIdentities">The collection of identities for the object type.</param>
/// <param name="apiPrimaryIdentityName">The name of the primary identity within the collection.</param>
public sealed class ApiIdentitySet
(
    IEnumerable<ApiIdentity> apiIdentities,
    string apiPrimaryIdentityName
) : ApiSchemaElement
{
    #region Fields
    private Dictionary<string, ApiIdentity>? _apiIdentityApiNameLookup = null;

    private ApiIdentity? _apiResolvedPrimaryIdentity = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets all identities defined for the object type.
    /// </summary>
    public ApiIdentity[] ApiIdentities { get; } = [.. apiIdentities.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

    /// <summary>
    ///     Gets the name of the primary identity.
    /// </summary>
    public string ApiPrimaryIdentityName => apiPrimaryIdentityName;

    /// <summary>
    ///     Gets the primary identity for the object type.
    /// </summary>
    /// <remarks>
    ///     This property is available after initialization.
    /// </remarks>
    public ApiIdentity ApiPrimaryIdentity => this.ThrowIfNotInitialized(_apiResolvedPrimaryIdentity);

    private Dictionary<string, ApiIdentity> ApiIdentityApiNameLookup => this.ThrowIfNotInitialized(_apiIdentityApiNameLookup);
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiParentPath)
        => ApiSchemaHelpers.BuildPath(apiParentPath, apiChildPath: nameof(ApiIdentitySet), apiApiName: null);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeLookupDictionaries(context);

        this.InitializeApiIdentities(context);
        this.InitializeApiPrimaryIdentity(context);
    }
    #endregion

    #region ApiIdentitySet Methods
    /// <summary>
    ///     Attempts to retrieve an identity by its API name.
    /// </summary>
    /// <param name="apiName">The name of the identity to retrieve.</param>
    /// <param name="apiIdentity">When this method returns, contains the identity if found; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the identity was found; otherwise, <c>false</c>.</returns>
    public bool TryGetIdentityByApiName(string apiName, out ApiIdentity? apiIdentity) => this.ApiIdentityApiNameLookup.TryGetValue(apiName, out apiIdentity);
    #endregion

    #region Implementation Methods
    private void InitializeLookupDictionaries(ApiInitializationContext context)
    {
        _apiIdentityApiNameLookup = null;

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiIdentities,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => !string.IsNullOrWhiteSpace(x),
            partKeyPropertyName: nameof(ApiIdentity.ApiName),
            path: $"{this.ApiPath}.{nameof(this.ApiIdentities)}",
            code: ApiInitializationCode.API_IDENTITY_SET_DUPLICATE_API_NAME,
            context: context,
            lookupDictionary: out _apiIdentityApiNameLookup
        );
    }

    private void InitializeApiIdentities(ApiInitializationContext context)
    {
        if (this.ApiIdentities is null || this.ApiIdentities.Length == 0)
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiIdentities)}";
            var severity = ApiInitializationSeverity.Warning;
            var code = ApiInitializationCode.API_IDENTITY_SET_NULL_OR_EMPTY_IDENTITIES;
            var description = $"{nameof(this.ApiIdentities)} must not be null or empty";

            context.AddIssue(path, severity, code, description, remediation: null);
            return;
        }

        // Initialize each identity
        var apiIdentitiesCount = this.ApiIdentities.Length;
        for (var i = 0; i < apiIdentitiesCount; ++i)
        {
            var apiIdentity = this.ApiIdentities[i];

            var childContext = context.WithParentSchemaElement(this);
            apiIdentity.Initialize(childContext);
        }
    }

    private void InitializeApiPrimaryIdentity(ApiInitializationContext context)
    {
        _apiResolvedPrimaryIdentity = null;

        if (string.IsNullOrWhiteSpace(this.ApiPrimaryIdentityName))
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiPrimaryIdentityName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_SET_INVALID_PRIMARY_API_NAME;
            var description = $"{nameof(this.ApiPrimaryIdentityName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiPrimaryIdentityName)} value";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        if (this.ApiIdentityApiNameLookup.TryGetValue(this.ApiPrimaryIdentityName, out var apiResolvedPrimaryIdentity))
        {
            _apiResolvedPrimaryIdentity = apiResolvedPrimaryIdentity;
        }

        if (_apiResolvedPrimaryIdentity is null)
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiPrimaryIdentity)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_SET_UNRESOLVED_PRIMARY;
            var description = $"{nameof(this.ApiPrimaryIdentity)} could not be resolved for {nameof(this.ApiPrimaryIdentityName)}='{this.ApiPrimaryIdentityName.SafeToString()}'";
            var remediation = $"Verify that an {nameof(ApiIdentity)} is declared in {nameof(this.ApiIdentities)} for {nameof(this.ApiPrimaryIdentityName)}='{this.ApiPrimaryIdentityName.SafeToString()}'";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
