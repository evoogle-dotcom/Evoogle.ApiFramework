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
///     mediated by an <see cref="ApiRelationshipAssociation"/> join-table object type.
/// </summary>
/// <remarks>
///     <para>
///         Unlike one-to-one and one-to-many relationships, a many-to-many has two symmetric
///         <see cref="ApiRelationshipPrincipalEnd"/> instances — <see cref="ApiPrincipalEndA"/> and
///         <see cref="ApiPrincipalEndB"/> — and no dependent end.
///         Each principal end provides a referenced primary key type that is mapped to the association object type
///         through the corresponding key path collection on <see cref="ApiAssociation"/>.
///     </para>
///     <para>
///         Self-referential many-to-many relationships are supported by setting both principal ends
///         to the same <see cref="ApiRelationshipElement.ClrObjectType"/>.
///     </para>
/// </remarks>
/// <param name="apiName">The API name that uniquely identifies this relationship within the schema.</param>
/// <param name="apiPrincipalEndA">The first principal end of the relationship, which provides the A-side referenced primary key type.</param>
/// <param name="apiPrincipalEndB">The second principal end of the relationship, which provides the B-side referenced primary key type.</param>
/// <param name="apiAssociation">
///     The association element that mediates the relationship and may provide the foreign key role's key path trees
///     for both principal ends.
/// </param>
/// <param name="apiDeleteBehavior">
///     The delete behavior that governs what happens to the association objects when either principal end is deleted.
///     Defaults to <see cref="DefaultDeleteBehavior"/>.
/// </param>
public sealed class ApiRelationshipManyToMany
(
    string apiName,
    ApiRelationshipPrincipalEnd apiPrincipalEndA,
    ApiRelationshipPrincipalEnd apiPrincipalEndB,
    ApiRelationshipAssociation apiAssociation,
    ApiRelationshipDeleteBehavior apiDeleteBehavior = ApiRelationshipManyToMany.DefaultDeleteBehavior
) : ApiRelationship(apiName, apiDeleteBehavior)
{
    #region ApiRelationshipManyToMany Fields
    /// <summary>
    ///     The default delete behavior for many-to-many relationships.
    ///     Association rows are deleted automatically when either principal is removed.
    /// </summary>
    public const ApiRelationshipDeleteBehavior DefaultDeleteBehavior = ApiRelationshipDeleteBehavior.Delete;
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
    /// <summary>Gets principal end A of the relationship, which provides the referenced primary key type for the first outer type.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEndA { get; } = apiPrincipalEndA;

    /// <summary>Gets principal end B of the relationship, which provides the referenced primary key type for the second outer type.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEndB { get; } = apiPrincipalEndB;

    /// <summary>Gets the association element that mediates the relationship.</summary>
    public ApiRelationshipAssociation ApiAssociation { get; } = apiAssociation;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiPrincipalEndA = this.ApiPrincipalEndA.SafeToString();
        var apiPrincipalEndB = this.ApiPrincipalEndB.SafeToString();
        var apiAssociation = this.ApiAssociation.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipManyToMany)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiPrincipalEndA)}={apiPrincipalEndA}, {nameof(this.ApiPrincipalEndB)}={apiPrincipalEndB}, {nameof(this.ApiAssociation)}={apiAssociation}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiPrincipalEndA(context);
        this.InitializeApiPrincipalEndB(context);
        this.InitializeApiAssociation(context);
        this.InitializeApiAssociationKeyPathAlignment(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiAssociationKeyPathAlignment(ApiInitializationContext context)
    {
        if (this.ApiPrincipalEndA is not null && this.ApiAssociation is not null && this.ApiAssociation.HasForeignKeys)
        {
            this.InitializeApiAssociationKeyPathAlignment
            (
                context,
                this.ApiPrincipalEndA,
                this.ApiAssociation.ApiForeignKeyTypeA,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_A_COUNT,
                nameof(ApiRelationshipAssociation.ApiForeignKeyTypeA),
                "A"
            );
        }

        if (this.ApiPrincipalEndB is not null && this.ApiAssociation is not null && this.ApiAssociation.HasForeignKeys)
        {
            this.InitializeApiAssociationKeyPathAlignment
            (
                context,
                this.ApiPrincipalEndB,
                this.ApiAssociation.ApiForeignKeyTypeB,
                ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_INVALID_ASSOCIATION_KEY_PATHS_B_COUNT,
                nameof(ApiRelationshipAssociation.ApiForeignKeyTypeB),
                "B"
            );
        }
    }

    private void InitializeApiAssociationKeyPathAlignment
    (
        ApiInitializationContext context,
        ApiRelationshipPrincipalEnd principalEnd,
        ApiKeyType foreignKeyType,
        ApiInitializationCode countMismatchCode,
        string foreignKeyPropertyName,
        string principalEndName
    )
    {
        var primaryKeyDesc = principalEnd.ApiPrimaryKeyTypeName is not null ? $"primary key type '{principalEnd.ApiPrimaryKeyTypeName}'" : "primary key type";
        var foreignKeyPath = $"{nameof(this.ApiAssociation)}.{foreignKeyPropertyName}";

        ApiRelationshipKeyAlignment.ValidatePrincipalForeignKeyAlignment
        (
            context: context,
            relationshipPath: this.ApiPath,
            principalEnd: principalEnd,
            foreignKeyType: foreignKeyType,
            countMismatchCode: countMismatchCode,
            foreignKeyPath: foreignKeyPath,
            principalCountLabel: $"principal end {principalEndName} {primaryKeyDesc}",
            principalCompatibilityLabel: $"principal end {principalEndName} {primaryKeyDesc}",
            principalEndQualifier: $"for principal end {principalEndName}",
            explicitKeyTarget: $"{nameof(ApiRelationshipPrincipalEnd.ApiPrimaryKeyTypeName)} on principal end {principalEndName}",
            inferredForeignKeyLabel: $"{principalEndName}-side foreign key",
            countMismatchRemediationTarget: $"principal end {principalEndName}'s primary key type",
            compatibilityRemediation: $"Ensure {foreignKeyPath} paths are ordered to match principal end {principalEndName}'s primary key type and use compatible scalar types"
        );
    }

    private void InitializeApiAssociation(ApiInitializationContext context)
    {
        if (this.ApiAssociation is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_ASSOCIATION;
            var description = $"{nameof(this.ApiAssociation)} must not be null";
            var remediation = $"Provide a valid {nameof(ApiRelationshipAssociation)} for the association between the two principal ends";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var assocationContext = context.WithDeclaringSchemaElement(this);
        this.ApiAssociation.Initialize(assocationContext);
    }

    private void InitializeApiPrincipalEndA(ApiInitializationContext context)
    {
        if (this.ApiPrincipalEndA is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_A;
            var description = $"{nameof(this.ApiPrincipalEndA)} must not be null";
            var remediation = $"Provide a valid {nameof(ApiRelationshipPrincipalEnd)} for end A";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiPrincipalEndA.Initialize(endContext);
    }

    private void InitializeApiPrincipalEndB(ApiInitializationContext context)
    {
        if (this.ApiPrincipalEndB is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_MANY_TO_MANY_NULL_PRINCIPAL_END_B;
            var description = $"{nameof(this.ApiPrincipalEndB)} must not be null";
            var remediation = $"Provide a valid {nameof(ApiRelationshipPrincipalEnd)} for end B";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiPrincipalEndB.Initialize(endContext);
    }

    #endregion
}
