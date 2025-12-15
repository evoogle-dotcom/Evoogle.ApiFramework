// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
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
    public ApiProperty[] ApiProperties { get; } = [.. apiProperties.EmptyIfNull().Where(x => x is not null)];

    /// <summary>
    ///     Gets the collection of API relationships defined on this object type.
    /// </summary>
    public ApiRelationship[] ApiRelationships { get; } = [.. apiRelationships.EmptyIfNull().Where(x => x is not null)];

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

    public ApiObjectTypeOptions ApiObjectTypeOptions { get; init; } = new ApiObjectTypeOptions();

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
        this.InitializeApiIdentitySet(context);
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
    private void InitializeApiIdentitySet(ApiInitializationContext context)
    {
        if (this.ApiIdentitySet is null)
        {
            // No identity set defined; this is acceptable as identity sets are optional.
            return;
        }

        var childContext = context.WithParentObjectType(this);
        this.ApiIdentitySet.Initialize(childContext);
    }

    private void InitializeApiProperties(ApiInitializationContext context)
    {
        if (this.ApiProperties is null || this.ApiProperties.Length == 0)
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiProperties)}";
            var severity = ApiInitializationSeverity.Warning;
            var code = ApiInitializationCode.API_OBJECT_TYPE_NULL_OR_EMPTY_PROPERTIES;
            var description = $"{nameof(this.ApiProperties)} must not be null or empty";

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
            partKeyFilter: x => !string.IsNullOrWhiteSpace(x),
            partKeyPropertyName: nameof(ApiProperty.ApiName),
            path: $"{this.ApiPath}.{nameof(this.ApiProperties)}",
            code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_PROPERTY_API_NAME,
            context: context,
            lookupDictionary: out _apiPropertyApiNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiProperties,
            partKeySelector: x => x.ClrName,
            partKeyFilter: x => !string.IsNullOrWhiteSpace(x),
            partKeyPropertyName: nameof(ApiProperty.ClrName),
            path: $"{this.ApiPath}.{nameof(this.ApiProperties)}",
            code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_PROPERTY_CLR_NAME,
            context: context,
            lookupDictionary: out _apiPropertyClrNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiRelationships,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => !string.IsNullOrWhiteSpace(x),
            partKeyPropertyName: nameof(ApiRelationship.ApiName),
            path: $"{this.ApiPath}.{nameof(this.ApiRelationships)}",
            code: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_RELATIONSHIP_API_NAME,
            context: context,
            lookupDictionary: out _apiRelationshipApiNameLookup
        );
    }
    #endregion
}
