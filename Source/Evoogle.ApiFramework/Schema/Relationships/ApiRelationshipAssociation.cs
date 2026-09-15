// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Relationships;

/// <summary>
///     Represents the association element of an <see cref="ApiRelationshipManyToMany"/> relationship.
///
///     The association identifies the join-table <see cref="ApiObjectType"/> whose properties hold key values
///     that link the two outer principal object types.
///
///     An association may declare <see cref="ApiForeignKeyA"/> and <see cref="ApiForeignKeyB"/>
///     to map the scalar leaves of each principal key to properties on the association object type.
/// </summary>
/// <remarks>
///     Use <see cref="HasForeignKeys"/> before accessing <see cref="ApiForeignKeyA"/> or
///     <see cref="ApiForeignKeyB"/>.
///
///     The state is symmetric: both sides are either declared together or omitted together.
///
///     When no foreign keys are declared, the owning many-to-many relationship is navigational at the schema level.
/// </remarks>
[JsonConverter(typeof(ApiRelationshipAssociationJsonConverter))]
public sealed class ApiRelationshipAssociation : ApiRelationshipElement
{
    #region ApiRelationshipAssociation Fields
    private readonly ApiKeyDefinition? _apiForeignKeyA;
    private readonly ApiKeyDefinition? _apiForeignKeyB;

    private const string _noForeignKeysDeclaredMessage = "No foreign keys declared for this association of the many-to-many relationship.";
    private const string _ownershipErrorMessage = $"An {nameof(ApiRelationshipAssociation)} must be owned by an {nameof(ApiRelationshipManyToMany)}.";
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind =>
        ApiSchemaElementKind.RelationshipAssociation;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipAssociation);
    #endregion

    #region ApiRelationshipAssociation Properties
    /// <summary>
    ///     Gets the <see cref="ApiRelationshipManyToMany"/> that owns this association.
    ///     Derived from <see cref="ApiSchemaElement.Parent"/> and available after topology construction.
    /// </summary>
    public ApiRelationshipManyToMany ApiRelationshipManyToMany =>
        this.Parent as ApiRelationshipManyToMany ?? throw new ApiSchemaException(_ownershipErrorMessage);

    /// <summary>
    ///     Gets the A-side foreign key role's <see cref="ApiKeyDefinition"/> that maps scalar leaves of principal end A's key
    ///     to properties on the association object type.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="HasForeignKeys"/> is <see langword="false"/>.
    /// </exception>
    public ApiKeyDefinition ApiForeignKeyA => this.HasForeignKeys
        ? _apiForeignKeyA! : throw new ApiSchemaException(_noForeignKeysDeclaredMessage);

    /// <summary>
    ///     Gets the B-side foreign key role's <see cref="ApiKeyDefinition"/> that maps scalar leaves of principal end B's key
    ///     to properties on the association object type.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="HasForeignKeys"/> is <see langword="false"/>.
    /// </exception>
    public ApiKeyDefinition ApiForeignKeyB => this.HasForeignKeys
        ? _apiForeignKeyB! : throw new ApiSchemaException(_noForeignKeysDeclaredMessage);
    #endregion

    #region ApiRelationshipAssociation Computed Properties
    /// <summary>
    ///     Gets a value indicating whether this association has explicit foreign keys declared for both principal ends.
    ///     When <see langword="true"/>, both <see cref="ApiForeignKeyA"/> and <see cref="ApiForeignKeyB"/> are available.
    /// </summary>
    public bool HasForeignKeys => _apiForeignKeyA is not null && _apiForeignKeyB is not null;
    #endregion

    #region Constructors
    /// <summary>
    ///     Creates an association with no foreign key binding declared at the schema level for either principal end.
    ///     Use when the join-table object type needs to be identified but key property mapping
    ///     is intentionally left to the downstream layer.
    /// </summary>
    /// <param name="apiObjectTypeReference">The reference to the association <see cref="ApiObjectType"/>.</param>
    public ApiRelationshipAssociation(ApiTypeReference apiObjectTypeReference)
        : base(apiObjectTypeReference)
    {
    }

    /// <summary>
    ///     Creates a key-bound association with explicit <see cref="ApiKeyDefinition"/> instances for both foreign key roles.
    /// </summary>
    /// <param name="apiObjectTypeReference">The reference to the association <see cref="ApiObjectType"/>.</param>
    /// <param name="apiForeignKeyA">
    ///     The <see cref="ApiKeyDefinition"/> that maps the scalar leaves of principal end A's key
    ///     to properties on the association object type.
    /// </param>
    /// <param name="apiForeignKeyB">
    ///     The <see cref="ApiKeyDefinition"/> that maps the scalar leaves of principal end B's key
    ///     to properties on the association object type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="apiForeignKeyA"/> or <paramref name="apiForeignKeyB"/> is <see langword="null"/>.
    /// </exception>
    public ApiRelationshipAssociation
    (
        ApiTypeReference apiObjectTypeReference,
        ApiKeyDefinition apiForeignKeyA,
        ApiKeyDefinition apiForeignKeyB
    )
        : base(apiObjectTypeReference)
    {
        ArgumentNullException.ThrowIfNull(apiForeignKeyA);
        ArgumentNullException.ThrowIfNull(apiForeignKeyB);

        _apiForeignKeyA = apiForeignKeyA;
        _apiForeignKeyB = apiForeignKeyB;
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiObjectTypeReference = this.ApiObjectTypeReference.SafeToString();
        var apiForeignKeyA = _apiForeignKeyA.SafeToString();
        var apiForeignKeyB = _apiForeignKeyB.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipAssociation)} "
            + $"{{{nameof(this.ApiObjectTypeReference)}={apiObjectTypeReference}, "
            + $"{nameof(this.ApiForeignKeyA)}={apiForeignKeyA}, "
            + $"{nameof(this.ApiForeignKeyB)}={apiForeignKeyB}, "
            + $"{nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        if (_apiForeignKeyA is not null)
        {
            yield return _apiForeignKeyA;
        }

        if (_apiForeignKeyB is not null)
        {
            yield return _apiForeignKeyB;
        }
    }

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.CompileApiForeignKeys(context);
    }
    #endregion

    #region Implementation Methods
    private void CompileApiForeignKeys(ApiSchemaCompilationContext context)
    {
        if (!this.HasForeignKeys)
        {
            // No foreign keys declared; the owning relationship is navigational.
            return;
        }

        var locationA = ApiSchemaCompilationLocation.ForRole(nameof(this.ApiForeignKeyA));
        _apiForeignKeyA!.Compile(context, locationA);

        var locationB = ApiSchemaCompilationLocation.ForRole(nameof(this.ApiForeignKeyB));
        _apiForeignKeyB!.Compile(context, locationB);
    }
    #endregion
}
