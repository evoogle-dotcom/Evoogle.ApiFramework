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
///     Represents metadata of an API object type that defines a set of named structural properties
///     for API input/output.
/// </summary>
/// <param name="apiName">The API name of the object type.</param>
/// <param name="apiOptions">The configuration options for the object type.</param>
/// <param name="apiProperties">The collection of API properties defined on this object type.</param>
/// <param name="apiKeyTypes">The collection of API key types defined for this object type.</param>
/// <param name="clrObjectType">The CLR type representing this API object.</param>
public sealed partial class ApiObjectType
(
    string apiName,
    ApiObjectTypeOptions? apiOptions,
    IEnumerable<ApiProperty>? apiProperties,
    IEnumerable<ApiKeyType>? apiKeyTypes,
    Type clrObjectType
) : ApiNamedType(apiName, clrObjectType)
{
    #region ApiObjectType Fields
    private Dictionary<string, ApiKeyType>? _apiKeyTypeApiNameLookup = null;
    private string[]? _apiKeyTypeApiNames = null;

    private Dictionary<string, ApiProperty>? _apiPropertyApiNameLookup = null;
    private Dictionary<string, ApiProperty>? _apiPropertyClrNameLookup = null;

    private ApiRelationshipEnd[]? _apiRelationshipEnds = null;
    private ApiRelationshipPrincipalEnd[]? _apiPrincipalRelationshipEnds = null;
    private ApiRelationshipDependentEnd[]? _apiDependentRelationshipEnds = null;
    private ApiRelationshipAssociation[]? _apiRelationshipAssociations = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiObjectType);
    #endregion

    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind ApiKind => ApiTypeKind.Object;
    #endregion

    #region ApiObjectType Properties
    /// <summary>Gets all key types defined for the object type.</summary>
    public ApiKeyType[] ApiKeyTypes { get; } = [.. apiKeyTypes.EmptyIfNull().Where(x => x is not null)];

    /// <summary>Gets the configuration options for the object type.</summary>
    public ApiObjectTypeOptions? ApiOptions { get; } = apiOptions;

    /// <summary>Gets the collection of API properties defined on this object type.</summary>
    public ApiProperty[] ApiProperties { get; } = [.. apiProperties.EmptyIfNull().Where(x => x is not null)];

    /// <summary>
    ///     Gets all relationship ends where this object type participates, whether as principal or dependent.
    ///     Populated during <see cref="ApiSchema"/> initialization. Returns an empty array before initialization completes.
    /// </summary>
    public ApiRelationshipEnd[] ApiRelationshipEnds => _apiRelationshipEnds is not null ? _apiRelationshipEnds : [];

    /// <summary>
    ///     Gets all relationship ends where this object type acts as the principal and provides the referenced key type.
    ///     Populated during <see cref="ApiSchema"/> initialization. Returns an empty array before initialization completes.
    /// </summary>
    public ApiRelationshipPrincipalEnd[] ApiRelationshipPrincipalEnds => _apiPrincipalRelationshipEnds is not null ? _apiPrincipalRelationshipEnds : [];

    /// <summary>
    ///     Gets all relationship ends where this object type acts as the dependent and may provide a foreign key role binding.
    ///     Populated during <see cref="ApiSchema"/> initialization. Returns an empty array before initialization completes.
    /// </summary>
    public ApiRelationshipDependentEnd[] ApiRelationshipDependentEnds => _apiDependentRelationshipEnds is not null ? _apiDependentRelationshipEnds : [];

    /// <summary>
    ///     Gets all M:N relationship associations where this object type acts as the join table.
    ///     Populated during <see cref="ApiSchema"/> initialization. Returns an empty array before initialization completes.
    /// </summary>
    public ApiRelationshipAssociation[] ApiRelationshipAssociations => _apiRelationshipAssociations is not null ? _apiRelationshipAssociations : [];

    private Dictionary<string, ApiKeyType> ApiKeyTypeApiNameLookup => this.ThrowIfNotInitialized(_apiKeyTypeApiNameLookup);
    private Dictionary<string, ApiProperty> ApiPropertyApiNameLookup => this.ThrowIfNotInitialized(_apiPropertyApiNameLookup);
    private Dictionary<string, ApiProperty> ApiPropertyClrNameLookup => this.ThrowIfNotInitialized(_apiPropertyClrNameLookup);
    #endregion

    #region ApiObjectType Computed Properties
    /// <summary>Indicates whether this object type has any API key types.</summary>
    public bool HasKeyTypes => this.ApiKeyTypes.Length > 0;

    /// <summary>Indicates whether this object type participates in any relationships.</summary>
    public bool HasRelationshipEnds => _apiRelationshipEnds?.Length > 0;

    /// <summary>Indicates whether this object type acts as a join table in any M:N relationships.</summary>
    public bool HasAssociationRole => _apiRelationshipAssociations?.Length > 0;
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

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeLookupDictionaries(context);
        this.InitializeApiProperties(context);
    }

    /// <summary>
    ///    Initializes the API key types defined for this object type.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    internal void InitializeKeyTypes(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        this.InitializeApiKeyTypes(context);
    }
    #endregion

    #region ApiObjectType Methods
    /// <summary>
    ///     Attempts to retrieve an API key type by its API name.
    /// </summary>
    /// <param name="apiName">The name of the key type to retrieve.</param>
    /// <param name="apiKeyType">When this method returns, contains the <see cref="ApiKeyType"/> if found; otherwise, null.</param>
    /// <returns>True if the key type was found; otherwise, false.</returns>
    public bool TryGetKeyTypeByApiName(string apiName, [NotNullWhen(true)] out ApiKeyType? apiKeyType) => this.ApiKeyTypeApiNameLookup.TryGetValue(apiName, out apiKeyType);

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

    #endregion

    #region ApiObjectType KeyType Methods
    /// <summary>
    ///     Gets the API names of all key types defined on this object type.
    /// </summary>
    /// <returns>An array of key type API names, or an empty array if no key types are defined.</returns>
    public string[] GetKeyTypeApiNames()
    {
        if (!this.HasKeyTypes)
        {
            return [];
        }

        return _apiKeyTypeApiNames ??= [.. this.ApiKeyTypeApiNameLookup.Keys];
    }

    /// <summary>
    ///     Checks if this object type has a specific key type by API name.
    /// </summary>
    /// <param name="apiKeyTypeName">The API name of the key type to check for.</param>
    /// <returns><c>true</c> if the key type exists; otherwise, <c>false</c>.</returns>
    public bool HasKeyTypeByApiName(string apiKeyTypeName)
    {
        if (!this.HasKeyTypes || string.IsNullOrWhiteSpace(apiKeyTypeName))
        {
            return false;
        }

        return this.ApiKeyTypeApiNameLookup.ContainsKey(apiKeyTypeName);
    }
    #endregion

    #region Implementation Methods
    internal void SetRelationshipEnds
    (
        ApiRelationshipEnd[] ends,
        ApiRelationshipPrincipalEnd[] principalEnds,
        ApiRelationshipDependentEnd[] dependentEnds
    )
    {
        _apiRelationshipEnds = ends;
        _apiPrincipalRelationshipEnds = principalEnds;
        _apiDependentRelationshipEnds = dependentEnds;
    }

    internal void SetRelationshipAssociations(ApiRelationshipAssociation[] associations)
    {
        _apiRelationshipAssociations = associations;
    }

    internal void ClearRelationshipEnds()
    {
        _apiRelationshipEnds = null;
        _apiPrincipalRelationshipEnds = null;
        _apiDependentRelationshipEnds = null;
        _apiRelationshipAssociations = null;
    }

    private void InitializeApiKeyTypes(ApiInitializationContext context)
    {
        if (this.ApiKeyTypes.Length == 0)
        {
            // No key types defined; this is acceptable as key types are optional.
            return;
        }

        // Initialize each key type
        var apiKeyTypesCount = this.ApiKeyTypes.Length;
        for (var i = 0; i < apiKeyTypesCount; ++i)
        {
            var apiKeyType = this.ApiKeyTypes[i];

            var childContext = context.WithDeclaringObjectType(this);
            apiKeyType.Initialize(childContext);
        }
    }

    private void InitializeApiProperties(ApiInitializationContext context)
    {
        if (this.ApiProperties is null || this.ApiProperties.Length == 0)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Warning;
            var code = ApiInitializationCode.API_OBJECT_TYPE_NULL_OR_EMPTY_PROPERTIES;
            var description = $"{nameof(this.ApiProperties)} is null or empty";

            var remediation = $"Add at least one {nameof(ApiProperty)} to {nameof(ApiObjectType)}[\"{this.ApiName}\"]";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var apiPropertiesCount = this.ApiProperties.Length;
        for (var i = 0; i < apiPropertiesCount; ++i)
        {
            var apiProperty = this.ApiProperties[i];

            var childContext = context.WithDeclaringObjectType(this);
            apiProperty.Initialize(childContext);
        }
    }

    private void InitializeLookupDictionaries(ApiInitializationContext context)
    {
        // Initialize lookup dictionaries for lookup of:
        // - Property by API name and CLR name
        _apiKeyTypeApiNameLookup = null;
        _apiPropertyApiNameLookup = null;
        _apiPropertyClrNameLookup = null;

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiKeyTypes,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiKeyType.ApiName),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_KEY_TYPE_API_NAME,
            context: context,
            lookupDictionary: out _apiKeyTypeApiNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiProperties,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiProperty.ApiName),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_PROPERTY_API_NAME,
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
            duplicatePartCode: ApiInitializationCode.API_OBJECT_TYPE_DUPLICATE_PROPERTY_CLR_NAME,
            context: context,
            lookupDictionary: out _apiPropertyClrNameLookup
        );
    }

    #endregion
}
