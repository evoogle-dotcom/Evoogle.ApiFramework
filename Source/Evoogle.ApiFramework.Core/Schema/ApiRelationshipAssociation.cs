// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the association element of an <see cref="ApiRelationshipManyToMany"/> relationship.
///
///     The association identifies the join-table <see cref="ApiObjectType"/> whose properties hold key values
///     that link the two outer principal object types.
///
///     An association may declare <see cref="ApiForeignKeyTypeA"/> and <see cref="ApiForeignKeyTypeB"/>
///     to map the scalar leaves of each principal key type to properties on the association object type.
/// </summary>
/// <remarks>
///     Use <see cref="HasForeignKeys"/> before accessing <see cref="ApiForeignKeyTypeA"/> or
///     <see cref="ApiForeignKeyTypeB"/>.
///
///     The state is symmetric: both sides are either declared together or omitted together.
///
///     When no foreign keys are declared, the owning many-to-many relationship is navigational at the schema level.
/// </remarks>
[JsonConverter(typeof(ApiRelationshipAssociationJsonConverter))]
public sealed class ApiRelationshipAssociation : ApiRelationshipElement
{
    #region ApiRelationshipAssociation Fields
    private readonly ApiKeyType? _apiForeignKeyTypeA;
    private readonly ApiKeyType? _apiForeignKeyTypeB;

    private ApiRelationshipManyToMany? _apiResolvedRelationshipManyToMany;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipAssociation);
    #endregion

    #region ApiRelationshipAssociation Properties
    /// <summary>
    ///     Gets the <see cref="ApiRelationshipManyToMany"/> that owns this association.
    ///     Available after schema initialization.
    /// </summary>
    public ApiRelationshipManyToMany ApiRelationshipManyToMany => this.ThrowIfNotInitialized(_apiResolvedRelationshipManyToMany);

    /// <summary>
    ///     Gets the A-side foreign key role's <see cref="ApiKeyType"/> that maps scalar leaves of principal end A's key type
    ///     to properties on the association object type.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="HasForeignKeys"/> is <see langword="false"/>.
    /// </exception>
    public ApiKeyType ApiForeignKeyTypeA => this.HasForeignKeys
        ? _apiForeignKeyTypeA!
        : throw new ApiSchemaException("No foreign keys declared for this association of the many-to-many relationship.");

    /// <summary>
    ///     Gets the B-side foreign key role's <see cref="ApiKeyType"/> that maps scalar leaves of principal end B's key type
    ///     to properties on the association object type.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="HasForeignKeys"/> is <see langword="false"/>.
    /// </exception>
    public ApiKeyType ApiForeignKeyTypeB => this.HasForeignKeys
        ? _apiForeignKeyTypeB!
        : throw new ApiSchemaException("No foreign keys declared for this association of the many-to-many relationship.");
    #endregion

    #region ApiRelationshipAssociation Computed Properties
    /// <summary>
    ///     Gets a value indicating whether this association has explicit foreign keys declared for both principal ends.
    ///     When <see langword="true"/>, both <see cref="ApiForeignKeyTypeA"/> and <see cref="ApiForeignKeyTypeB"/> are available.
    /// </summary>
    public bool HasForeignKeys => _apiForeignKeyTypeA is not null && _apiForeignKeyTypeB is not null;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes an association with no foreign key binding declared at the schema level for either principal end.
    ///     Use when the join-table object type needs to be identified but key property mapping
    ///     is intentionally left to the downstream layer.
    /// </summary>
    /// <param name="clrObjectType">The CLR type of the association <see cref="ApiObjectType"/>.</param>
    public ApiRelationshipAssociation(Type clrObjectType)
        : base(clrObjectType)
    {
        _apiForeignKeyTypeA = null;
        _apiForeignKeyTypeB = null;
    }

    /// <summary>
    ///     Initializes a key-bound association with explicit <see cref="ApiKeyType"/> instances for both foreign key roles.
    /// </summary>
    /// <param name="clrObjectType">The CLR type of the association <see cref="ApiObjectType"/>.</param>
    /// <param name="apiForeignKeyTypeA">
    ///     The <see cref="ApiKeyType"/> that maps the scalar leaves of principal end A's key type
    ///     to properties on the association object type.
    /// </param>
    /// <param name="apiForeignKeyTypeB">
    ///     The <see cref="ApiKeyType"/> that maps the scalar leaves of principal end B's key type
    ///     to properties on the association object type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="apiForeignKeyTypeA"/> or <paramref name="apiForeignKeyTypeB"/> is <see langword="null"/>.
    /// </exception>
    public ApiRelationshipAssociation
    (
        Type clrObjectType,
        ApiKeyType apiForeignKeyTypeA,
        ApiKeyType apiForeignKeyTypeB
    )
        : base(clrObjectType)
    {
        ArgumentNullException.ThrowIfNull(apiForeignKeyTypeA);
        ArgumentNullException.ThrowIfNull(apiForeignKeyTypeB);

        _apiForeignKeyTypeA = apiForeignKeyTypeA;
        _apiForeignKeyTypeB = apiForeignKeyTypeB;
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiForeignKeyTypeA = _apiForeignKeyTypeA.SafeToString();
        var apiForeignKeyTypeB = _apiForeignKeyTypeB.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipAssociation)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiForeignKeyTypeA)}={apiForeignKeyTypeA}, {nameof(this.ApiForeignKeyTypeB)}={apiForeignKeyTypeB}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiForeignKeyTypes(context);
    }
    #endregion

    #region ApiRelationshipAssociation Methods
    internal void SetRelationship(ApiRelationshipManyToMany relationship)
    {
        ArgumentNullException.ThrowIfNull(relationship);
        _apiResolvedRelationshipManyToMany = relationship;
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiForeignKeyTypes(ApiInitializationContext context)
    {
        if (!this.HasForeignKeys)
        {
            // No foreign keys declared; the owning relationship is navigational.
            return;
        }

        var fkContext = context.WithDeclaringSchemaElement(this);
        _apiForeignKeyTypeA!.Initialize(fkContext);
        _apiForeignKeyTypeB!.Initialize(fkContext);
    }
    #endregion
}
