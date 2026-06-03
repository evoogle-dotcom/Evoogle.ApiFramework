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
///     An association is either <em>navigational</em> — where no foreign key binding is declared at the schema level —
///     or <em>key-bound</em>, where <see cref="ApiForeignKeyTypeA"/> and <see cref="ApiForeignKeyTypeB"/> explicitly
///     map the scalar leaves of each principal key type to properties on the association object type.
/// </summary>
/// <remarks>
///     Use <see cref="HasKeyBinding"/> to determine which state applies before accessing
///     <see cref="ApiForeignKeyTypeA"/> or <see cref="ApiForeignKeyTypeB"/>.
///
///     The state is symmetric: both sides are either bound or navigational together.
///
///     The state is fixed at construction and does not change after initialization.
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
    ///     Thrown when <see cref="IsNavigational"/> is <see langword="true"/>.
    ///     Check <see cref="HasKeyBinding"/> before accessing this property.
    /// </exception>
    public ApiKeyType ApiForeignKeyTypeA => this.HasKeyBinding
        ? _apiForeignKeyTypeA!
        : throw new ApiSchemaException("No key binding declared for this association of the many-to-many relationship.");

    /// <summary>
    ///     Gets the B-side foreign key role's <see cref="ApiKeyType"/> that maps scalar leaves of principal end B's key type
    ///     to properties on the association object type.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="IsNavigational"/> is <see langword="true"/>.
    ///     Check <see cref="HasKeyBinding"/> before accessing this property.
    /// </exception>
    public ApiKeyType ApiForeignKeyTypeB => this.HasKeyBinding
        ? _apiForeignKeyTypeB!
        : throw new ApiSchemaException("No key binding declared for this association of the many-to-many relationship.");
    #endregion

    #region ApiRelationshipAssociation Computed Properties
    /// <summary>
    ///     Gets a value indicating whether this association has explicit key bindings declared for both principal ends.
    ///     When <see langword="true"/>, both <see cref="ApiForeignKeyTypeA"/> and <see cref="ApiForeignKeyTypeB"/> are available.
    /// </summary>
    public bool HasKeyBinding => _apiForeignKeyTypeA is not null && _apiForeignKeyTypeB is not null;

    /// <summary>
    ///     Gets a value indicating whether this association is navigational (i.e. has no explicit key binding
    ///     declared at the schema level for either principal end).
    /// </summary>
    public bool IsNavigational => !this.HasKeyBinding;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a navigational association with no foreign key binding declared at the schema level for either principal end.
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
        var hasKeyBinding = this.HasKeyBinding;
        var apiForeignKeyTypeA = hasKeyBinding ? _apiForeignKeyTypeA!.SafeToString() : "None";
        var apiForeignKeyTypeB = hasKeyBinding ? _apiForeignKeyTypeB!.SafeToString() : "None";
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipAssociation)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.HasKeyBinding)}={hasKeyBinding}, {nameof(this.ApiForeignKeyTypeA)}={apiForeignKeyTypeA}, {nameof(this.ApiForeignKeyTypeB)}={apiForeignKeyTypeB}, {nameof(this.ExtensionCount)}={extensionCount}}}";
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
        if (this.IsNavigational)
        {
            // No key binding declared — purely navigational relationship.
            return;
        }

        var fkContext = context.WithDeclaringSchemaElement(this);
        _apiForeignKeyTypeA!.Initialize(fkContext);
        _apiForeignKeyTypeB!.Initialize(fkContext);
    }
    #endregion
}
