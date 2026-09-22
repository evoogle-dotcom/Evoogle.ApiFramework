// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Internal;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Key.Internal;
using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Version;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Represents metadata of an API object type that defines a set of named structural properties
///     for API input/output.
/// </summary>
/// <param name="apiName">The API name of the object type.</param>
/// <param name="apiOptions">The configuration options for the object type.</param>
/// <param name="apiProperties">The collection of API properties defined on this object type.</param>
/// <param name="apiKeys">
///     The collection of named API key definitions defined for this object type.
/// </param>
/// <param name="apiVersion">The optional version metadata for this object type.</param>
/// <param name="clrObjectType">The CLR type representing this API object.</param>
public sealed partial class ApiObjectType
(
    string apiName,
    ApiObjectTypeOptions? apiOptions,
    IEnumerable<ApiProperty>? apiProperties,
    IEnumerable<ApiNamedKeyDefinition>? apiKeys,
    ApiVersionDefinition? apiVersion,
    Type clrObjectType
) : ApiNamedType(apiName, clrObjectType), IApiKeyPathRootProvider
{
    #region ApiObjectType Fields
    private FrozenDictionary<string, ApiNamedKeyDefinition>? _apiKeyApiNameLookup = null;
    private ImmutableArray<string> _apiKeyApiNames = [];

    private FrozenDictionary<string, ApiProperty>? _apiPropertyApiNameLookup = null;
    private FrozenDictionary<string, ApiProperty>? _apiPropertyClrNameLookup = null;

    private ImmutableArray<ApiRelationshipEnd> _apiRelationshipEnds = [];
    private ImmutableArray<ApiRelationshipPrincipalEnd> _apiPrincipalRelationshipEnds = [];
    private ImmutableArray<ApiRelationshipDependentEnd> _apiDependentRelationshipEnds = [];
    private ImmutableArray<ApiRelationshipAssociation> _apiRelationshipAssociations = [];

    private ImmutableArray<ApiRelationshipTraversal> _apiRelationshipTraversals = [];
    private FrozenDictionary<string, ApiRelationshipTraversal>? _apiRelationshipTraversalApiNameLookup = null;
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
    /// <summary>Gets the immutable snapshot of named key definitions for the object type.</summary>
    public ImmutableArray<ApiNamedKeyDefinition> ApiKeys { get; } =
        [.. apiKeys.EmptyIfNull().Where(x => x is not null)];

    /// <summary>Gets the configuration options for the object type.</summary>
    public ApiObjectTypeOptions? ApiOptions { get; } = apiOptions;

    /// <summary>Gets the immutable snapshot of properties defined on this object type.</summary>
    public ImmutableArray<ApiProperty> ApiProperties { get; } =
        [.. apiProperties.EmptyIfNull().Where(x => x is not null)];

    /// <summary>Gets the optional version metadata defined for this object type.</summary>
    public ApiVersionDefinition? ApiVersion { get; } = apiVersion;

    /// <summary>
    ///     Gets the immutable snapshot of relationship ends where this object type participates.
    ///     Populated during <see cref="ApiSchema"/> compilation. Returns an empty array before
    ///     compilation completes.
    /// </summary>
    public ImmutableArray<ApiRelationshipEnd> ApiRelationshipEnds => _apiRelationshipEnds;

    /// <summary>
    ///     Gets the immutable snapshot of relationship ends where this object type acts as the
    ///     principal and provides the principal key. Populated during <see cref="ApiSchema"/>
    ///     compilation. Returns an empty array before compilation completes.
    /// </summary>
    public ImmutableArray<ApiRelationshipPrincipalEnd> ApiRelationshipPrincipalEnds =>
        _apiPrincipalRelationshipEnds;

    /// <summary>
    ///     Gets the immutable snapshot of relationship ends where this object type acts as the
    ///     dependent and may provide a foreign key role binding. Populated during
    ///     <see cref="ApiSchema"/> compilation. Returns an empty array before compilation.
    /// </summary>
    public ImmutableArray<ApiRelationshipDependentEnd> ApiRelationshipDependentEnds =>
        _apiDependentRelationshipEnds;

    /// <summary>
    ///     Gets the immutable snapshot of M:N associations where this object type acts as the join
    ///     table. Populated during <see cref="ApiSchema"/> compilation. Returns an empty array
    ///     before compilation completes.
    /// </summary>
    public ImmutableArray<ApiRelationshipAssociation> ApiRelationshipAssociations =>
        _apiRelationshipAssociations;

    /// <summary>Gets the relationship traversals exposed from this object type.</summary>
    public ImmutableArray<ApiRelationshipTraversal> ApiRelationshipTraversals => _apiRelationshipTraversals;

    private FrozenDictionary<string, ApiNamedKeyDefinition> ApiKeyApiNameLookup => this.RequireValue(_apiKeyApiNameLookup);

    private FrozenDictionary<string, ApiProperty> ApiPropertyApiNameLookup => this.RequireValue(_apiPropertyApiNameLookup);
    private FrozenDictionary<string, ApiProperty> ApiPropertyClrNameLookup => this.RequireValue(_apiPropertyClrNameLookup);

    private FrozenDictionary<string, ApiRelationshipTraversal> ApiRelationshipTraversalApiNameLookup => this.RequireValue(_apiRelationshipTraversalApiNameLookup);
    #endregion

    #region ApiObjectType Computed Properties
    /// <summary>Indicates whether this object type has any API keys.</summary>
    public bool HasKeys => this.ApiKeys.Length > 0;

    /// <summary>Indicates whether this object type defines version metadata.</summary>
    public bool HasVersion => this.ApiVersion is not null;

    /// <summary>Indicates whether this object type participates in any relationships.</summary>
    public bool HasRelationshipEnds => !_apiRelationshipEnds.IsDefaultOrEmpty;

    /// <summary>Indicates whether this object type acts as a join table in any M:N relationships.</summary>
    public bool HasAssociationRole => !_apiRelationshipAssociations.IsDefaultOrEmpty;
    #endregion

    #region IApiKeyPathRootProvider Properties
    ApiObjectType? IApiKeyPathRootProvider.RootObjectType => this;

    string? IApiKeyPathRootProvider.RootLabel => this.ApiName;
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
    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        foreach (var apiProperty in this.ApiProperties)
        {
            yield return apiProperty;
        }

        foreach (var apiKeyDefinition in this.ApiKeys)
        {
            yield return apiKeyDefinition;
        }

        if (this.ApiVersion is not null)
        {
            yield return this.ApiVersion;
        }
    }

    /// <inheritdoc />
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ValidateApiOptions(context);
        this.BuildLookupDictionaries(context);
        this.CompileApiProperties(context);
    }

    /// <summary>
    ///    Compiles the API keys defined for this object type.
    /// </summary>
    /// <param name="context">The compilation context.</param>
    internal void CompileKeys(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.ThrowIfFrozen();

        this.CompileApiKeys(context);
    }

    /// <summary>Compiles the optional API version definition for this object type.</summary>
    /// <param name="context">The compilation context.</param>
    internal void CompileVersion(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.ThrowIfFrozen();
        this.ApiVersion?.Compile(context);
    }
    #endregion

    #region ApiObjectType Methods
    /// <summary>
    ///     Attempts to retrieve an API key by its API name.
    /// </summary>
    /// <param name="apiName">The name of the key to retrieve.</param>
    /// <param name="apiKey">
    ///     When this method returns, contains the <see cref="ApiNamedKeyDefinition"/> if found;
    ///     otherwise, null.
    /// </param>
    /// <returns>True if the key was found; otherwise, false.</returns>
    public bool TryGetKeyByApiName(string apiName, [NotNullWhen(true)] out ApiNamedKeyDefinition? apiKey)
        => this.ApiKeyApiNameLookup.TryGetValue(apiName, out apiKey);

    /// <summary>
    ///     Attempts to retrieve an API property by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetPropertyByApiName(string apiName, [NotNullWhen(true)] out ApiProperty? value)
        => this.ApiPropertyApiNameLookup.TryGetValue(apiName, out value);

    /// <summary>
    ///     Attempts to retrieve an API property by its CLR name.
    /// </summary>
    /// <param name="clrName">The CLR name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetPropertyByClrName(string clrName, [NotNullWhen(true)] out ApiProperty? value)
        => this.ApiPropertyClrNameLookup.TryGetValue(clrName, out value);

    /// <summary>Finds an exposed relationship traversal by API name.</summary>
    /// <param name="apiName">The traversal name on this object type.</param>
    /// <param name="traversal">The matching traversal, or null when absent.</param>
    /// <returns>True when a matching traversal exists.</returns>
    public bool TryGetTraversalByApiName(string apiName, [NotNullWhen(true)] out ApiRelationshipTraversal? traversal)
        => this.ApiRelationshipTraversalApiNameLookup.TryGetValue(apiName, out traversal);
    #endregion

    #region ApiObjectType Key Methods
    /// <summary>Gets the precomputed immutable API names of all keys in declaration order.</summary>
    public ImmutableArray<string> ApiKeyApiNames => _apiKeyApiNames;

    /// <summary>
    ///     Checks if this object type has a specific key by API name.
    /// </summary>
    /// <param name="apiKeyName">The API name of the key to check for.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    public bool HasKeyByApiName(string apiKeyName)
    {
        if (!this.HasKeys || string.IsNullOrWhiteSpace(apiKeyName))
        {
            return false;
        }

        return this.ApiKeyApiNameLookup.ContainsKey(apiKeyName);
    }
    #endregion

    #region Implementation Methods
    internal void SetRelationshipAssociations
    (
        ImmutableArray<ApiRelationshipAssociation> associations
    )
    {
        this.ThrowIfFrozen();
        _apiRelationshipAssociations = associations.IsDefault ? [] : associations;
    }

    internal void SetRelationshipEnds
    (
        ImmutableArray<ApiRelationshipEnd> ends,
        ImmutableArray<ApiRelationshipPrincipalEnd> principalEnds,
        ImmutableArray<ApiRelationshipDependentEnd> dependentEnds
    )
    {
        this.ThrowIfFrozen();
        _apiRelationshipEnds = ends.IsDefault ? [] : ends;
        _apiPrincipalRelationshipEnds = principalEnds.IsDefault ? [] : principalEnds;
        _apiDependentRelationshipEnds = dependentEnds.IsDefault ? [] : dependentEnds;
    }

    internal void SetRelationshipTraversals
    (
        ApiSchemaCompilationContext context,
        ImmutableArray<ApiRelationshipTraversal> traversals
    )
    {
        this.ThrowIfFrozen();
        _apiRelationshipTraversals = traversals.IsDefault ? [] : traversals;

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: _apiRelationshipTraversals,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiRelationshipTraversal.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiRelationshipTraversalDuplicateApiName,
            session: context.Session,
            keyComparer: ApiNameComparer.Instance,
            lookupDictionary: out _apiRelationshipTraversalApiNameLookup
        );

        foreach (var traversal in _apiRelationshipTraversals)
        {
            if (this.ApiProperties.Any(apiProperty => ApiNameComparer.Instance.Equals(apiProperty.ApiName, traversal.ApiName)))
            {
                var apiPath = traversal.ApiPath;
                var severity = ApiSchemaCompilationSeverity.Error;
                var code = ApiSchemaCompilationCode.ApiRelationshipTraversalDuplicateApiName;
                var description = $"Traversal name '{traversal.ApiName}' conflicts with another property on '{this.ApiName}'";
                var remediation = "Choose a unique API property name on the source object type";

                context.AddIssue(apiPath, severity, code, description, remediation);
            }

            if (traversal.ClrMemberReference is { } clrMemberReference &&
                this.ApiProperties.Any(apiProperty => ClrNameComparer.Instance.Equals
                (
                    apiProperty.ClrName,
                    clrMemberReference.ClrName
                )))
            {
                var apiPath = traversal.ApiPath;
                var severity = ApiSchemaCompilationSeverity.Error;
                var code = ApiSchemaCompilationCode.ApiRelationshipTraversalClrMemberConflict;
                var description = $"CLR member '{clrMemberReference.ClrName}' is bound to "
                    + $"both a contained property and traversal on '{this.ApiName}'";
                var remediation = "Bind the traversal to a distinct CLR member";

                context.AddIssue(apiPath, severity, code, description, remediation);
            }
        }
    }

    private void BuildLookupDictionaries(ApiSchemaCompilationContext context)
    {
        // Compile lookup dictionaries for lookup of:
        // - Property by API name and CLR name
        _apiKeyApiNames = [.. this.ApiKeys.Select(key => key.ApiName)];

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiKeys,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiNamedKeyDefinition.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiObjectTypeDuplicateKeyApiName,
            session: context.Session,
            keyComparer: ApiNameComparer.Instance,
            lookupDictionary: out _apiKeyApiNameLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiProperties,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiProperty.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiObjectTypeDuplicatePropertyApiName,
            session: context.Session,
            keyComparer: ApiNameComparer.Instance,
            lookupDictionary: out _apiPropertyApiNameLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiProperties,
            partKeySelector: x => x.ClrName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiProperty.ClrName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiObjectTypeDuplicatePropertyClrName,
            session: context.Session,
            keyComparer: ClrNameComparer.Instance,
            lookupDictionary: out _apiPropertyClrNameLookup
        );
    }

    private void CompileApiKeys(ApiSchemaCompilationContext context)
    {
        if (this.ApiKeys.Length == 0)
        {
            // No keys defined; this is acceptable because keys are optional.
            return;
        }

        // Compile each key definition.
        var apiKeysCount = this.ApiKeys.Length;
        for (var i = 0; i < apiKeysCount; ++i)
        {
            var apiKeyDefinition = this.ApiKeys[i];

            apiKeyDefinition.Compile(context);
        }
    }

    private void CompileApiProperties(ApiSchemaCompilationContext context)
    {
        if (this.ApiProperties.Length == 0)
        {
            var severity = ApiSchemaCompilationSeverity.Warning;
            var code = ApiSchemaCompilationCode.ApiObjectTypeNullOrEmptyProperties;
            var description = $"{nameof(this.ApiProperties)} is null or empty";

            var remediation = $"Add at least one {nameof(ApiProperty)} to {nameof(ApiObjectType)}[\"{this.ApiName}\"]";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        var apiPropertiesCount = this.ApiProperties.Length;
        for (var i = 0; i < apiPropertiesCount; ++i)
        {
            var apiProperty = this.ApiProperties[i];

            apiProperty.Compile(context);
        }
    }

    private void ValidateApiOptions(ApiSchemaCompilationContext context)
    {
        if (this.ApiOptions?.HasInvalidApiKeyNullHandling != true)
        {
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiObjectTypeInvalidApiKeyNullHandling;
        var description = $"{nameof(this.ApiOptions)}.{nameof(ApiObjectTypeOptions.ApiKeyNullHandling)} must be a valid {nameof(ApiKeyNullHandling)} value";
        var remediation = $"Specify a valid {nameof(ApiObjectTypeOptions.ApiKeyNullHandling)} value";

        context.AddIssue(severity, code, description, remediation);
    }
    #endregion
}
