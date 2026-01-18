// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

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
/// <param name="apiOptions">The configuration options for the object type.</param>
/// <param name="apiIdentities">The collection of API identities defined for this object type.</param>
/// <param name="apiProperties">The collection of API properties defined on this object type.</param>
/// <param name="apiRelationships">The collection of API relationships defined on this object type.</param>
/// <param name="clrObjectType">The CLR type representing this API object.</param>
public sealed partial class ApiObjectType
(
    string apiName,
    ApiObjectTypeOptions? apiOptions,
    IEnumerable<ApiIdentity>? apiIdentities,
    IEnumerable<ApiProperty>? apiProperties,
    IEnumerable<ApiRelationship>? apiRelationships,
    Type clrObjectType
) : ApiNamedType(apiName, clrObjectType)
{
    #region ApiObjectType Fields
    private Dictionary<string, ApiIdentity>? _apiIdentityApiNameLookup = null;
    private Dictionary<string, ApiProperty>? _apiPropertyApiNameLookup = null;
    private Dictionary<string, ApiProperty>? _apiPropertyClrNameLookup = null;
    private Dictionary<string, ApiRelationship>? _apiRelationshipApiNameLookup = null;
    #endregion

    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind ApiKind => ApiTypeKind.Object;

    /// <inheritdoc/>
    protected override string ApiTypeName => nameof(ApiObjectType);
    #endregion

    #region ApiObject Properties
    /// <summary>
    ///     Gets all identities defined for the object type.
    /// </summary>
    public ApiIdentity[] ApiIdentities { get; } = [.. apiIdentities.EmptyIfNull().Where(x => x is not null)];

    /// <summary>
    ///     Gets the primary API identity for this object type, if available.
    /// </summary>
    /// <remarks>
    ///     By convention, the first identity in <see cref="ApiIdentities"/> is the primary identity.
    ///     Returns null if no identities are configured.
    /// </remarks>
    public ApiIdentity? ApiPrimaryIdentity => this.ApiIdentities.Length > 0 ? this.ApiIdentities[0] : null;

    /// <summary>
    ///     Gets the configuration options for the object type.
    /// </summary>
    public ApiObjectTypeOptions? ApiOptions => apiOptions;

    /// <summary>
    ///     Gets the collection of API properties defined on this object type.
    /// </summary>
    public ApiProperty[] ApiProperties { get; } = [.. apiProperties.EmptyIfNull().Where(x => x is not null)];

    /// <summary>
    ///     Gets the collection of API relationships defined on this object type.
    /// </summary>
    public ApiRelationship[] ApiRelationships { get; } = [.. apiRelationships.EmptyIfNull().Where(x => x is not null)];

    /// <summary>
    ///     Indicates whether this object type has any API identities defined.
    /// </summary>
    public bool HasIdentity => this.ApiIdentities.Length > 0;

    private Dictionary<string, ApiIdentity> ApiIdentityApiNameLookup => this.ThrowIfNotInitialized(_apiIdentityApiNameLookup);
    private Dictionary<string, ApiProperty> ApiPropertyApiNameLookup => this.ThrowIfNotInitialized(_apiPropertyApiNameLookup);
    private Dictionary<string, ApiProperty> ApiPropertyClrNameLookup => this.ThrowIfNotInitialized(_apiPropertyClrNameLookup);
    private Dictionary<string, ApiRelationship> ApiRelationshipApiNameLookup => this.ThrowIfNotInitialized(_apiRelationshipApiNameLookup);
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeLookupDictionaries(context);

        this.InitializeApiProperties(context);
        this.InitializeApiRelationships(context);
        this.InitializeApiIdentities(context);
    }
    #endregion

    #region ApiObjectType Methods
    /// <summary>
    ///     Attempts to retrieve an API identity by its API name.
    /// </summary>
    /// <param name="apiName">The name of the identity to retrieve.</param>
    /// <param name="apiIdentity">When this method returns, contains the <see cref="ApiIdentity"/> if found; otherwise, null.</param>
    /// <returns>True if the identity was found; otherwise, false.</returns>
    public bool TryGetIdentityByApiName(string apiName, [NotNullWhen(true)] out ApiIdentity? apiIdentity) => this.ApiIdentityApiNameLookup.TryGetValue(apiName, out apiIdentity);

    /// <summary>
    ///     Attempts to retrieve an API property by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetPropertyByApiName(string apiName, [NotNullWhen(true)] out ApiProperty? value) => this.ApiPropertyApiNameLookup.TryGetValue(apiName, out value);

    /// <summary>
    ///     Attempts to retrieve an API property by its CLR name.
    /// </summary>
    /// <param name="clrName">The CLR name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetPropertyByClrName(string clrName, [NotNullWhen(true)] out ApiProperty? value) => this.ApiPropertyClrNameLookup.TryGetValue(clrName, out value);

    /// <summary>
    ///     Attempts to retrieve an API relationship by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the relationship to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiRelationship"/> if found; otherwise, null.</param>
    /// <returns>True if the relationship was found; otherwise, false.</returns>
    public bool TryGetRelationshipByApiName(string apiName, [NotNullWhen(true)] out ApiRelationship? value) => this.ApiRelationshipApiNameLookup.TryGetValue(apiName, out value);
    #endregion

    #region ApiObjectType Identity Methods
    /// <summary>
    ///     Gets the API names of all properties that constitute a specific identity.
    /// </summary>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>A read-only list of property API names, or an empty list if the identity is not found.</returns>
    /// <remarks>
    ///     <para><b>Use Cases:</b></para>
    ///     <list type="bullet">
    ///         <item><description>UI form generation - Mark identity fields as required</description></item>
    ///         <item><description>Query building - Generate WHERE clauses based on identity properties</description></item>
    ///         <item><description>Validation - Auto-generate validation rules for identity fields</description></item>
    ///         <item><description>Serialization - Optimize identity property handling</description></item>
    ///     </list>
    /// </remarks>
    public string[] GetIdentityApiPropertyNames(string? apiIdentityName = null)
    {
        if (!this.HasIdentity)
        {
            return [];
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            return [];
        }

        return [.. identity.ApiIdentityParts.Select(part => part.ApiPropertyName)];
    }

    /// <summary>
    ///     Gets the scalar CLR type for a specific identity part property.
    /// </summary>
    /// <param name="apiPropertyName">The API name of the property.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>The scalar target type for the identity part, or null if the property is not part of the identity.</returns>
    /// <remarks>
    ///     This returns the resolved scalar target type that the property value will be coerced to when building the identity.
    /// </remarks>
    public Type? GetIdentityPartClrType(string apiPropertyName, string? apiIdentityName = null)
    {
        if (!this.HasIdentity || string.IsNullOrWhiteSpace(apiPropertyName))
        {
            return null;
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            return null;
        }

        var part = identity.ApiIdentityParts
            .FirstOrDefault(p => string.Equals(p.ApiPropertyName, apiPropertyName, StringComparison.OrdinalIgnoreCase));

        return part?.ClrIdType;
    }

    /// <summary>
    ///     Checks if a property is part of any identity on this object type.
    /// </summary>
    /// <param name="apiPropertyName">The API name of the property to check.</param>
    /// <returns><c>true</c> if the property is part of at least one identity; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para><b>Use Cases:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Validation - Mark identity properties as required in forms</description></item>
    ///         <item><description>Security - Prevent modification of identity properties</description></item>
    ///         <item><description>Change tracking - Detect identity changes that require special handling</description></item>
    ///     </list>
    /// </remarks>
    public bool IsIdentityProperty(string apiPropertyName)
    {
        if (!this.HasIdentity || string.IsNullOrWhiteSpace(apiPropertyName))
        {
            return false;
        }

        return this.ApiIdentities
            .Any(identity => identity.ApiIdentityParts
                .Any(part => string.Equals(part.ApiPropertyName, apiPropertyName, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    ///     Gets the API names of all identities defined on this object type.
    /// </summary>
    /// <returns>A read-only list of identity API names, or an empty list if no identities are defined.</returns>
    /// <remarks>
    ///     <para>The returned list includes both the primary identity and any alternate identities.</para>
    ///     <para><b>Use Cases:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Runtime discovery - Allow users to choose which identity to use</description></item>
    ///         <item><description>API documentation - Generate documentation for available identity schemes</description></item>
    ///         <item><description>Query optimization - Select most efficient identity for lookup operations</description></item>
    ///     </list>
    /// </remarks>
    public string[] GetIdentityApiNames()
    {
        if (!this.HasIdentity)
        {
            return [];
        }

        return [.. this.ApiIdentities.Select(identity => identity.ApiName)];
    }

    /// <summary>
    ///     Checks if this object type has a specific identity by API name.
    /// </summary>
    /// <param name="apiIdentityName">The API name of the identity to check for.</param>
    /// <returns><c>true</c> if the identity exists; otherwise, <c>false</c>.</returns>
    public bool HasIdentityByApiName(string apiIdentityName)
    {
        if (!this.HasIdentity || string.IsNullOrWhiteSpace(apiIdentityName))
        {
            return false;
        }

        return this.TryGetIdentityByApiName(apiIdentityName, out _);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiOptions = this.ApiOptions.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiObjectType)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiOptions)}={apiOptions}, {nameof(this.ExtensionCount)}={extensionCount}}} [{clrType}]";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiIdentities(ApiInitializationContext context)
    {
        // Initialize identity lookup dictionary
        _apiIdentityApiNameLookup = null;

        if (this.ApiIdentities.Length == 0)
        {
            // No identities defined; this is acceptable as identities are optional.
            return;
        }

        // Initialize identity lookup dictionary
        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiIdentities,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiIdentity.ApiName),
            path: this.ApiPath,
            code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_IDENTITY_API_NAME,
            context: context,
            lookupDictionary: out _apiIdentityApiNameLookup
        );

        // Initialize each identity
        var apiIdentitiesCount = this.ApiIdentities.Length;
        for (var i = 0; i < apiIdentitiesCount; ++i)
        {
            var apiIdentity = this.ApiIdentities[i];

            var childContext = context.WithParentObjectType(this);
            apiIdentity.Initialize(childContext);
        }

        // Perform additional validation after identities initialization
        this.ValidateIdentityConfiguration(context);
    }

    private void InitializeApiProperties(ApiInitializationContext context)
    {
        if (this.ApiProperties is null || this.ApiProperties.Length == 0)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Warning;
            var code = ApiInitializationCode.API_OBJECT_TYPE_NULL_OR_EMPTY_PROPERTIES;
            var description = $"{nameof(this.ApiProperties)} is null or empty";

            context.AddIssue(path, severity, code, description, remediation: null);
            return;
        }

        var apiPropertiesCount = this.ApiProperties.Length;
        for (var i = 0; i < apiPropertiesCount; ++i)
        {
            var apiProperty = this.ApiProperties[i];

            var childContext = context.WithParentObjectType(this);
            apiProperty.Initialize(childContext);
        }
    }

    private void InitializeApiRelationships(ApiInitializationContext context)
    {
        if (this.ApiRelationships is null || this.ApiRelationships.Length == 0)
        {
            // No relationships defined; this is acceptable as relationships are optional.
            return;
        }

        var apiRelationshipsCount = this.ApiRelationships.Length;
        for (var i = 0; i < apiRelationshipsCount; ++i)
        {
            var apiRelationship = this.ApiRelationships[i];

            var childContext = context.WithParentObjectType(this);
            apiRelationship.Initialize(childContext);
        }
    }

    private void InitializeLookupDictionaries(ApiInitializationContext context)
    {
        // Initialize lookup dictionaries for lookup of:
        // - Property by API name and CLR name
        // - Relationship by API name
        _apiPropertyApiNameLookup = null;
        _apiPropertyClrNameLookup = null;
        _apiRelationshipApiNameLookup = null;

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiProperties,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiProperty.ApiName),
            path: this.ApiPath,
            code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_PROPERTY_API_NAME,
            context: context,
            lookupDictionary: out _apiPropertyApiNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiProperties,
            partKeySelector: x => x.ClrName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiProperty.ClrName),
            path: this.ApiPath,
            code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_PROPERTY_CLR_NAME,
            context: context,
            lookupDictionary: out _apiPropertyClrNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiRelationships,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiRelationship.ApiName),
            path: this.ApiPath,
            code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_RELATIONSHIP_API_NAME,
            context: context,
            lookupDictionary: out _apiRelationshipApiNameLookup
        );
    }

    private void ValidateIdentityConfiguration(ApiInitializationContext context)
    {
        if (this.ApiIdentities.Length <= 1)
        {
            // No validation needed for single or no identities
            return;
        }

        // Check for ambiguous identities (same property sets)
        var identities = this.ApiIdentities;
        for (var i = 0; i < identities.Length; i++)
        {
            for (var j = i + 1; j < identities.Length; j++)
            {
                var identity1 = identities[i];
                var identity2 = identities[j];

                var props1 = identity1.ApiIdentityParts
                    .Select(p => p.ApiPropertyName)
                    .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                var props2 = identity2.ApiIdentityParts
                    .Select(p => p.ApiPropertyName)
                    .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (props1.SequenceEqual(props2, StringComparer.OrdinalIgnoreCase))
                {
                    var path = this.ApiPath;
                    var severity = ApiInitializationSeverity.Warning;
                    var code = ApiInitializationCode.API_OBJECT_TYPE_AMBIGUOUS_IDENTITIES;
                    var description = $"Identities '{identity1.ApiName}' and '{identity2.ApiName}' use the same property set [{string.Join(", ", props1)}], which may cause ambiguity";
                    var remediation = "Consider using different property combinations for each identity, or remove one of the duplicate identities";

                    context.AddIssue(path, severity, code, description, remediation);
                }
            }
        }
    }
    #endregion
}
