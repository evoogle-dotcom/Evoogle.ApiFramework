// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extension;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiIdentitySet
(
    IEnumerable<ApiIdentity> apiIdentities,
    string apiPrimaryIdentityName
) : ExtensibleBase
{
    #region Fields
    private ApiSchemaContext? _apiSchemaContext = null;

    private Dictionary<string, ApiIdentity>? _apiIdentityApiNameLookup = null;

    private ApiIdentity? _apiResolvedPrimaryIdentity = null;
    #endregion

    #region Properties
    public ApiIdentity[] ApiIdentities { get; } = apiIdentities.SafeToArray();

    public string ApiPrimaryIdentityName => apiPrimaryIdentityName;

    public ApiIdentity ApiPrimaryIdentity => this.ThrowIfNotInitialized(_apiResolvedPrimaryIdentity);

    /// <summary>Gets the schema context for this identity set.</summary>
    internal ApiSchemaContext ApiSchemaContext => this.ThrowIfNotInitialized(_apiSchemaContext);

    private Dictionary<string, ApiIdentity> ApiIdentityApiNameLookup => this.ThrowIfNotInitialized(_apiIdentityApiNameLookup);
    #endregion

    #region ApiIdentitySet Methods
    public bool TryGetIdentityByApiName(string apiName, out ApiIdentity? apiIdentity) => this.ApiIdentityApiNameLookup.TryGetValue(apiName, out apiIdentity);

    internal void Initialize(ApiSchema apiSchema, ApiSchemaContext apiSchemaContext, ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(apiSchemaContext);
        ArgumentNullException.ThrowIfNull(apiObjectType);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        _apiSchemaContext = apiSchemaContext;

        this.InitializeLookupDictionaries(apiValidationPath, ref results);

        this.InitializeApiIdentities(apiSchema, apiSchemaContext, apiObjectType, apiValidationPath, ref results);
        this.InitializeApiPrimaryIdentity(apiValidationPath, ref results);
    }

    private void InitializeLookupDictionaries(string apiValidationPath, ref List<ValidationResult>? results)
    {
        _apiIdentityApiNameLookup = null;

        var anyIdentityApiNameDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiIdentities, x => x.ApiName, apiValidationPath, nameof(ApiIdentity.ApiName), ref results);
        if (!anyIdentityApiNameDuplicates)
        {
            _apiIdentityApiNameLookup = this.ApiIdentities.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        }
    }

    private void InitializeApiIdentities(ApiSchema apiSchema, ApiSchemaContext apiSchemaContext, ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        // Must not be null
        if (this.ApiIdentities is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiIdentities)} cannot be null.", [nameof(this.ApiIdentities)]));
            return;
        }

        // Must have at least one identity
        var apiIdentitiesCount = this.ApiIdentities.Length;
        if (apiIdentitiesCount == 0)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiIdentities)} must contain at least one identity.", [nameof(this.ApiIdentities)]));
            return;
        }

        // Initialize each identity
        for (var i = 0; i < apiIdentitiesCount; ++i)
        {
            apiValidationPath = $"{apiValidationPath}.{nameof(this.ApiIdentities)}[{i}]";

            var apiIdentity = this.ApiIdentities[i];
            if (apiIdentity is null)
            {
                results ??= [];
                results.Add(new ValidationResult($"{apiValidationPath} cannot be null.", [nameof(this.ApiIdentities)]));
                continue;
            }

            apiIdentity.Initialize(apiSchema, apiSchemaContext, apiObjectType, apiValidationPath, ref results);
        }
    }

    private void InitializeApiPrimaryIdentity(string apiValidationPath, ref List<ValidationResult>? results)
    {
        _apiResolvedPrimaryIdentity = null;

        if (string.IsNullOrWhiteSpace(this.ApiPrimaryIdentityName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiPrimaryIdentityName)} cannot be null or whitespace.", [nameof(this.ApiPrimaryIdentityName)]));
        }
        else
        {
            if (this.ApiIdentityApiNameLookup.TryGetValue(this.ApiPrimaryIdentityName, out var apiResolvedPrimaryIdentity))
            {
                _apiResolvedPrimaryIdentity = apiResolvedPrimaryIdentity;
            }
        }

        if (_apiResolvedPrimaryIdentity is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiPrimaryIdentityName)} unable to resolve {nameof(this.ApiPrimaryIdentityName)}[\"{this.ApiPrimaryIdentityName.SafeToString()}\"].", [nameof(this.ApiPrimaryIdentityName)]));
        }
    }
    #endregion
}
