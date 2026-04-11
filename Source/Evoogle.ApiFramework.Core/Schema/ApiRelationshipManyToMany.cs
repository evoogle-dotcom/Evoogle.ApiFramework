// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a many-to-many relationship between two <see cref="ApiObjectType"/> instances,
///     mediated by an explicit association <see cref="ApiObjectType"/> that holds FK references to both sides.
/// </summary>
/// <remarks>
///     <para>
///         The relationship is modeled through four ends:
///         <see cref="ApiPrincipalEndA"/> and <see cref="ApiPrincipalEndB"/> identify the two outer types
///         by their join-key identities, while <see cref="ApiDependentEndA"/> and <see cref="ApiDependentEndB"/>
///         both target the association type and carry the FK key paths pointing back to their respective principal sides.
///     </para>
///     <para>
///         The association type must be a registered <see cref="ApiObjectType"/> in the schema.
///         It may carry only FK columns (pure mapping) or additional payload properties describing
///         the relationship itself — both are fully supported since the association type is a first-class object type.
///     </para>
///     <para>
///         Association instances cannot exist independently: deleting either principal always cascades
///         to the association type.  This behavior is fixed and cannot be overridden by the developer.
///     </para>
/// </remarks>
/// <param name="apiName">The schema-unique API name of the relationship.</param>
/// <param name="apiPrincipalEndA">Principal end A, which owns the join key identity for the first outer type.</param>
/// <param name="apiPrincipalEndB">Principal end B, which owns the join key identity for the second outer type.</param>
/// <param name="apiDependentEndA">
///     Dependent end A, targeting the association type and holding FK key paths back to principal end A.
/// </param>
/// <param name="apiDependentEndB">
///     Dependent end B, targeting the association type and holding FK key paths back to principal end B.
/// </param>
/// <param name="apiAssociationTypeName">
///     The API name of the association <see cref="ApiObjectType"/> that mediates the relationship.
///     Must be a registered object type in the schema; both dependent ends must reference this type.
/// </param>
/// <param name="apiDisplayName">The optional human-readable display name for this relationship.</param>
/// <param name="apiDescription">The optional description for this relationship.</param>
public sealed class ApiRelationshipManyToMany
(
    string apiName,
    ApiRelationshipPrincipalEnd apiPrincipalEndA,
    ApiRelationshipPrincipalEnd apiPrincipalEndB,
    ApiRelationshipDependentEnd apiDependentEndA,
    ApiRelationshipDependentEnd apiDependentEndB,
    string apiAssociationTypeName,
    string? apiDisplayName = null,
    string? apiDescription = null
) : ApiRelationship(apiName, apiDisplayName, apiDescription)
{
    #region ApiRelationshipManyToMany Fields
    private ApiObjectType? _apiResolvedAssociationObjectType = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipManyToMany);
    #endregion

    #region ApiRelationship Properties
    /// <inheritdoc/>
    public override ApiRelationshipKind ApiKind => ApiRelationshipKind.ManyToMany;
    #endregion

    #region ApiRelationshipManyToMany Properties
    /// <summary>Gets principal end A of the relationship, which owns the join key identity for the first outer type.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEndA { get; } = apiPrincipalEndA;

    /// <summary>Gets principal end B of the relationship, which owns the join key identity for the second outer type.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEndB { get; } = apiPrincipalEndB;

    /// <summary>
    ///     Gets dependent end A, which targets the association type and carries FK key paths back to principal end A.
    ///     Delete behavior is fixed to <see cref="ApiRelationshipDeleteBehavior.Cascade"/> and is not developer-configurable.
    /// </summary>
    public ApiRelationshipDependentEnd ApiDependentEndA { get; } = apiDependentEndA;

    /// <summary>
    ///     Gets dependent end B, which targets the association type and carries FK key paths back to principal end B.
    ///     Delete behavior is fixed to <see cref="ApiRelationshipDeleteBehavior.Cascade"/> and is not developer-configurable.
    /// </summary>
    public ApiRelationshipDependentEnd ApiDependentEndB { get; } = apiDependentEndB;

    /// <summary>
    ///     Gets the API name of the association <see cref="ApiObjectType"/> that mediates the relationship.
    ///     Both <see cref="ApiDependentEndA"/> and <see cref="ApiDependentEndB"/> must reference this type.
    /// </summary>
    public string ApiAssociationTypeName { get; } = apiAssociationTypeName;

    /// <summary>Gets the resolved association <see cref="ApiObjectType"/>. Available after initialization.</summary>
    public ApiObjectType ApiAssociationObjectType => this.ThrowIfNotInitialized(_apiResolvedAssociationObjectType);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var typeA = this.ApiPrincipalEndA?.ApiObjectTypeName.SafeToString();
        var typeB = this.ApiPrincipalEndB?.ApiObjectTypeName.SafeToString();
        var associationType = this.ApiAssociationTypeName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipManyToMany)} {{{nameof(this.ApiName)}={apiName}, A={typeA}, B={typeB}, Association={associationType}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiAssociationTypeName(context);
        this.InitializeApiPrincipalEndA(context);
        this.InitializeApiDependentEndA(context);
        this.ValidateDependentEndAAssociationType(context);
        this.ValidateDependentEndAKeyPaths(context);
        this.InitializeApiPrincipalEndB(context);
        this.InitializeApiDependentEndB(context);
        this.ValidateDependentEndBAssociationType(context);
        this.ValidateDependentEndBKeyPaths(context);
        this.WireBackReferences();
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiAssociationTypeName(ApiInitializationContext context)
    {
        _apiResolvedAssociationObjectType = null;

        if (ApiSchemaHelpers.IsNameInvalid(this.ApiAssociationTypeName))
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_TYPE_NAME,
                $"{nameof(this.ApiAssociationTypeName)} must not be null, empty, or whitespace",
                $"Specify a valid {nameof(this.ApiAssociationTypeName)} value");
            return;
        }

        if (context.ApiSchema.TryGetObjectTypeByApiName(this.ApiAssociationTypeName, out var apiObjectType))
        {
            _apiResolvedAssociationObjectType = apiObjectType;
            return;
        }

        var availableTypes = string.Join(", ", context.ApiSchema.ApiObjectTypes.Select(t => $"'{t.ApiName}'"));
        var remediation = !string.IsNullOrEmpty(availableTypes)
            ? $"Use one of the available object types: {availableTypes}"
            : $"Define an {nameof(ApiObjectType)} with name '{this.ApiAssociationTypeName}' in the schema";

        context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
            ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_UNRESOLVED_ASSOCIATION_TYPE,
            $"Association type '{this.ApiAssociationTypeName}' could not be found in the schema",
            remediation);
    }

    private void InitializeApiPrincipalEndA(ApiInitializationContext context)
    {
        if (this.ApiPrincipalEndA is null)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_A,
                $"{nameof(this.ApiPrincipalEndA)} must not be null",
                $"Provide a valid {nameof(ApiRelationshipPrincipalEnd)} for end A");
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiPrincipalEndA.Initialize(endContext);
    }

    private void InitializeApiDependentEndA(ApiInitializationContext context)
    {
        if (this.ApiDependentEndA is null)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_DEPENDENT_END_A,
                $"{nameof(this.ApiDependentEndA)} must not be null",
                $"Provide a valid {nameof(ApiRelationshipDependentEnd)} for end A");
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiDependentEndA.Initialize(endContext);
    }

    private void ValidateDependentEndAAssociationType(ApiInitializationContext context)
    {
        if (this.ApiDependentEndA is null)
        {
            return;
        }

        if (ApiSchemaHelpers.IsNameInvalid(this.ApiAssociationTypeName))
        {
            return;
        }

        if (!string.Equals(this.ApiDependentEndA.ApiObjectTypeName, this.ApiAssociationTypeName, StringComparison.Ordinal))
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_DEPENDENT_TYPE_MISMATCH,
                $"{nameof(this.ApiDependentEndA)}.{nameof(ApiRelationshipDependentEnd.ApiObjectTypeName)} " +
                $"'{this.ApiDependentEndA.ApiObjectTypeName}' must match {nameof(this.ApiAssociationTypeName)} '{this.ApiAssociationTypeName}'",
                $"Set dependent end A's object type name to '{this.ApiAssociationTypeName}'");
        }
    }

    private void ValidateDependentEndAKeyPaths(ApiInitializationContext context)
    {
        if (this.ApiDependentEndA is null)
        {
            return;
        }

        if (this.ApiDependentEndA.ApiKeyPaths is null || this.ApiDependentEndA.ApiKeyPaths.Length == 0)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_EMPTY_KEY_PATHS,
                $"{nameof(this.ApiDependentEndA)} must declare at least one FK key path — purely navigational many-to-many relationships are not supported",
                $"Add at least one scalar, nested, or owner key path to {nameof(this.ApiDependentEndA)}");
        }
    }

    private void InitializeApiPrincipalEndB(ApiInitializationContext context)
    {
        if (this.ApiPrincipalEndB is null)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_B,
                $"{nameof(this.ApiPrincipalEndB)} must not be null",
                $"Provide a valid {nameof(ApiRelationshipPrincipalEnd)} for end B");
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiPrincipalEndB.Initialize(endContext);
    }

    private void InitializeApiDependentEndB(ApiInitializationContext context)
    {
        if (this.ApiDependentEndB is null)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_DEPENDENT_END_B,
                $"{nameof(this.ApiDependentEndB)} must not be null",
                $"Provide a valid {nameof(ApiRelationshipDependentEnd)} for end B");
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiDependentEndB.Initialize(endContext);
    }

    private void ValidateDependentEndBAssociationType(ApiInitializationContext context)
    {
        if (this.ApiDependentEndB is null)
        {
            return;
        }

        if (ApiSchemaHelpers.IsNameInvalid(this.ApiAssociationTypeName))
        {
            return;
        }

        if (!string.Equals(this.ApiDependentEndB.ApiObjectTypeName, this.ApiAssociationTypeName, StringComparison.Ordinal))
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_DEPENDENT_TYPE_MISMATCH,
                $"{nameof(this.ApiDependentEndB)}.{nameof(ApiRelationshipDependentEnd.ApiObjectTypeName)} " +
                $"'{this.ApiDependentEndB.ApiObjectTypeName}' must match {nameof(this.ApiAssociationTypeName)} '{this.ApiAssociationTypeName}'",
                $"Set dependent end B's object type name to '{this.ApiAssociationTypeName}'");
        }
    }

    private void ValidateDependentEndBKeyPaths(ApiInitializationContext context)
    {
        if (this.ApiDependentEndB is null)
        {
            return;
        }

        if (this.ApiDependentEndB.ApiKeyPaths is null || this.ApiDependentEndB.ApiKeyPaths.Length == 0)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_EMPTY_KEY_PATHS,
                $"{nameof(this.ApiDependentEndB)} must declare at least one FK key path — purely navigational many-to-many relationships are not supported",
                $"Add at least one scalar, nested, or owner key path to {nameof(this.ApiDependentEndB)}");
        }
    }

    private void WireBackReferences()
    {
        if (this.ApiPrincipalEndA is not null && this.ApiDependentEndA is not null)
        {
            this.ApiPrincipalEndA.WireBackReferences(this, this.ApiDependentEndA);
            this.ApiDependentEndA.WireBackReferences(this, this.ApiPrincipalEndA);
        }

        if (this.ApiPrincipalEndB is not null && this.ApiDependentEndB is not null)
        {
            this.ApiPrincipalEndB.WireBackReferences(this, this.ApiDependentEndB);
            this.ApiDependentEndB.WireBackReferences(this, this.ApiPrincipalEndB);
        }
    }
    #endregion
}
