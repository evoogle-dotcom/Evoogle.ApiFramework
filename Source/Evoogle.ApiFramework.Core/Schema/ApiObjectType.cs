// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API object type that defines a set of named structural properties and semantic relationships
///     for API input/output.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiObjectType"/> distinguishes between:
///         <list type="bullet">
///             <item><description><see cref="ApiProperty"/> — structural metadata describing individual named data members (e.g., strings, numbers, collections) of the object.</description></item>
///             <item><description><see cref="ApiRelationship"/> — semantic metadata describing navigation or linkage from one object type to another, based on one of its properties and expressing cardinality.</description></item>
///         </list>
///     </para>
/// </remarks>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiObjectType"/> class.
/// </remarks>
/// <param name="apiName">The API name of the object type.</param>
/// <param name="apiProperties">The collection of API properties defined on this object type.</param>
/// <param name="apiRelationships">The collection of API relationships defined on this object type.</param>
/// <param name="clrObjectType">The CLR type representing this API object.</param>
public sealed partial class ApiObjectType
(
    string apiName,
    IEnumerable<ApiProperty> apiProperties,
    IEnumerable<ApiRelationship> apiRelationships,
    Type clrObjectType,
    ApiIdentitySet? apiIdentitySet = null
) : ApiNamedType(apiName, clrObjectType)
{
    #region ApiObjectType Fields
    private Dictionary<string, ApiProperty>? _apiPropertyApiNameLookup = null;
    private Dictionary<string, ApiProperty>? _apiPropertyClrNameLookup = null;
    private Dictionary<string, ApiRelationship>? _apiRelationshipApiNameLookup = null;
    #endregion

    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind Kind => ApiTypeKind.Object;

    /// <inheritdoc/>
    protected override string ApiTypeName => nameof(ApiObjectType);
    #endregion

    #region ApiObject Properties
    /// <summary>
    ///     Gets the collection of API properties defined on this object type.
    /// </summary>
    public ApiProperty[] ApiProperties { get; } = apiProperties.SafeToArray();

    /// <summary>
    ///     Gets the collection of API relationships defined on this object type.
    /// </summary>
    public ApiRelationship[] ApiRelationships { get; } = apiRelationships.SafeToArray();

    /// <summary>
    ///     Gets the collection of API identities defined on this object type.
    /// </summary>
    public ApiIdentitySet? ApiIdentitySet => apiIdentitySet;

    /// <summary>
    ///     Gets the primary API identity for this object type, if available.
    /// </summary>
    public ApiIdentity? ApiPrimaryIdentity => this.ApiIdentitySet?.ApiPrimaryIdentity;

    /// <summary>
    ///     Indicates whether this object type has any API identities defined.
    /// </summary>
    public bool HasIdentity => this.ApiIdentitySet is not null;

    private Dictionary<string, ApiProperty> ApiPropertyApiNameLookup => this.ThrowIfNotInitialized(_apiPropertyApiNameLookup);
    private Dictionary<string, ApiProperty> ApiPropertyClrNameLookup => this.ThrowIfNotInitialized(_apiPropertyClrNameLookup);
    private Dictionary<string, ApiRelationship> ApiRelationshipApiNameLookup => this.ThrowIfNotInitialized(_apiRelationshipApiNameLookup);
    #endregion

    #region ApiType Methods
    internal override void Initialize(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);

        base.Initialize(apiSchema, ref results);

        this.InitializeLookupDictionaries(ref results);

        this.InitializeApiProperties(apiSchema, ref results);
        this.InitializeApiRelationships(apiSchema, ref results);
        this.InitializeApiIdentitySet(apiSchema, ref results);
    }
    #endregion

    #region ApiObjectType Methods
    /// <summary>
    ///     Attempts to retrieve an API identity by its API name.
    /// </summary>
    /// <param name="name">The name of the identity to retrieve.</param>
    /// <param name="apiIdentity">When this method returns, contains the <see cref="ApiIdentity"/> if found; otherwise, null.</param>
    /// <returns>True if the identity was found; otherwise, false.</returns>
    public bool TryGetIdentityByApiName(string apiName, out ApiIdentity? apiIdentity)
    {
        if (!this.HasIdentity)
        {
            apiIdentity = null;
            return false;
        }
        return this.ApiIdentitySet!.TryGetIdentityByApiName(apiName, out apiIdentity);
    }

    /// <summary>
    ///     Attempts to retrieve an API property by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetPropertyByApiName(string apiName, out ApiProperty? value) => this.ApiPropertyApiNameLookup.TryGetValue(apiName, out value);

    /// <summary>
    ///     Attempts to retrieve an API property by its CLR name.
    /// </summary>
    /// <param name="clrName">The CLR name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetPropertyByClrName(string clrName, out ApiProperty? value) => this.ApiPropertyClrNameLookup.TryGetValue(clrName, out value);

    /// <summary>
    ///     Attempts to retrieve an API relationship by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the relationship to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiRelationship"/> if found; otherwise, null.</param>
    /// <returns>True if the relationship was found; otherwise, false.</returns>
    public bool TryGetRelationshipByApiName(string apiName, out ApiRelationship? value) => this.ApiRelationshipApiNameLookup.TryGetValue(apiName, out value);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiObjectType)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ExtensionCount)}={extensionCount}}} [{clrType}]";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiIdentitySet(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        if (this.ApiIdentitySet is null)
        {
            return; // optional (no identity for this object type)
        }

        var apiValidationPath = $"{this.ValidationPath}.Identities";
        this.ApiIdentitySet.Initialize(apiSchema, this, apiValidationPath, ref results);
    }

    private void InitializeApiProperties(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        if (this.ApiProperties is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{this.ValidationPath}.{nameof(this.ApiProperties)} cannot be null.", [nameof(this.ApiProperties)]));
            return;
        }

        var apiPropertiesCount = this.ApiProperties.Length;
        for (var i = 0; i < apiPropertiesCount; ++i)
        {
            var apiValidationPath = $"{this.ValidationPath}.{nameof(this.ApiProperties)}[{i}]";

            var apiProperty = this.ApiProperties[i];
            if (apiProperty is null)
            {
                results ??= [];
                results.Add(new ValidationResult($"{apiValidationPath} cannot be null.", [nameof(this.ApiProperties)]));
                continue;
            }

            apiProperty.Initialize(apiSchema, apiValidationPath, ref results);
        }
    }

    private void InitializeApiRelationships(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        if (this.ApiRelationships is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{this.ValidationPath}.{nameof(this.ApiRelationships)} cannot be null.", [nameof(this.ApiRelationships)]));
            return;
        }

        var apiRelationshipsCount = this.ApiRelationships.Length;
        for (var i = 0; i < apiRelationshipsCount; ++i)
        {
            var apiValidationPath = $"{this.ValidationPath}.{nameof(this.ApiRelationships)}[{i}]";

            var apiRelationship = this.ApiRelationships[i];
            if (apiRelationship is null)
            {
                results ??= [];
                results.Add(new ValidationResult($"{apiValidationPath} cannot be null.", [nameof(this.ApiRelationships)]));
                continue;
            }

            apiRelationship.Initialize(apiSchema, this, apiValidationPath, ref results);
        }
    }

    private void InitializeLookupDictionaries(ref List<ValidationResult>? results)
    {
        _apiPropertyApiNameLookup = null;
        _apiPropertyClrNameLookup = null;
        _apiRelationshipApiNameLookup = null;

        var anyPropertyApiNameDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiProperties, x => x.ApiName, this.ValidationPath, nameof(ApiProperty.ApiName), ref results);
        var anyPropertyClrNameDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiProperties, x => x.ClrName, this.ValidationPath, nameof(ApiProperty.ClrName), ref results);
        var anyRelationshipApiNameDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiRelationships, x => x.ApiName, this.ValidationPath, nameof(ApiRelationship.ApiName), ref results);

        if (!anyPropertyApiNameDuplicates)
        {
            _apiPropertyApiNameLookup = this.ApiProperties.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        }

        if (!anyPropertyClrNameDuplicates)
        {
            _apiPropertyClrNameLookup = this.ApiProperties.ToDictionary(x => x.ClrName, StringComparer.OrdinalIgnoreCase);
        }

        if (!anyRelationshipApiNameDuplicates)
        {
            _apiRelationshipApiNameLookup = this.ApiRelationships.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        }
    }
    #endregion
}
