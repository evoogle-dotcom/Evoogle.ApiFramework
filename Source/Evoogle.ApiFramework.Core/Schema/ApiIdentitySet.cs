// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiIdentitySet(IEnumerable<ApiIdentity> apiIdentities, string apiPrimaryIdentityName) : ApiSchemaElement
{
    #region Fields
    private Dictionary<string, ApiIdentity>? _apiIdentityApiNameLookup = null;

    private ApiIdentity? _apiResolvedPrimaryIdentity = null;
    #endregion

    #region Properties
    public ApiIdentity[] ApiIdentities { get; } = [.. apiIdentities.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

    public string ApiPrimaryIdentityName => apiPrimaryIdentityName;

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
            code: ApiInitializationCode.API_IDENTITY_SET_DUPLICATE_IDENTITY_API_NAME,
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
            var description = $"{nameof(this.ApiIdentities)} is either null or empty";

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
            var code = ApiInitializationCode.API_IDENTITY_SET_INVALID_PRIMARY_IDENTITY_API_NAME;
            var description = $"{nameof(this.ApiPrimaryIdentityName)} cannot be null, empty, or whitespace";
            var remediation = $"Provide a valid {nameof(this.ApiPrimaryIdentityName)}";

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
            var code = ApiInitializationCode.API_IDENTITY_SET_UNRESOLVED_PRIMARY_IDENTITY;
            var description = $"{nameof(this.ApiPrimaryIdentity)} is unresolved for {nameof(this.ApiPrimaryIdentityName)}={this.ApiPrimaryIdentityName.SafeToString()}";
            var remediation = $"Ensure that an {nameof(this.ApiPrimaryIdentity)} is declared in the {nameof(this.ApiIdentities)} for {nameof(this.ApiPrimaryIdentityName)}={this.ApiPrimaryIdentityName.SafeToString()}";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
