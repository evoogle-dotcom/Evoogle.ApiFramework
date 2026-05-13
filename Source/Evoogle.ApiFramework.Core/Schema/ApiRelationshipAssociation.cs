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
///     The association identifies the join-table <see cref="ApiObjectType"/> whose properties hold the FK values
///     that link the two outer principal object types.
///
///     An association is either <em>navigational</em> — where no FK binding is declared at the schema level —
///     or <em>key-bound</em>, where <see cref="ApiKeyPathsA"/> and <see cref="ApiKeyPathsB"/> explicitly
///     map the scalar leaves of each principal's join-key identity to properties on the association object type.
/// </summary>
/// <remarks>
///     Use <see cref="HasKeyBinding"/> to determine which state applies before accessing
///     <see cref="ApiKeyPathsA"/> or <see cref="ApiKeyPathsB"/>.
///
///     The state is symmetric: both sides are either bound or navigational together.
///
///     The state is fixed at construction and does not change after initialization.
/// </remarks>
[JsonConverter(typeof(ApiRelationshipAssociationJsonConverter))]
public sealed class ApiRelationshipAssociation : ApiRelationshipElement
{
    #region ApiRelationshipAssociation Fields
    private readonly ApiRelationshipKeyPath[]? _apiKeyPathsA;
    private readonly ApiRelationshipKeyPath[]? _apiKeyPathsB;

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
    ///     Gets the FK key paths that map scalar leaves of principal end A's join-key <see cref="ApiIdentity"/>
    ///     to properties on the association object type.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="IsNavigational"/> is <see langword="true"/>.
    ///     Check <see cref="HasKeyBinding"/> before accessing this property.
    /// </exception>
    public ApiRelationshipKeyPath[] ApiKeyPathsA => this.HasKeyBinding
        ? _apiKeyPathsA!
        : throw new ApiSchemaException("No key binding declared for this association of the many-to-many relationship.");

    /// <summary>
    ///     Gets the FK key paths that map scalar leaves of principal end B's join-key <see cref="ApiIdentity"/>
    ///     to properties on the association object type.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="IsNavigational"/> is <see langword="true"/>.
    ///     Check <see cref="HasKeyBinding"/> before accessing this property.
    /// </exception>
    public ApiRelationshipKeyPath[] ApiKeyPathsB => this.HasKeyBinding
        ? _apiKeyPathsB!
        : throw new ApiSchemaException("No key binding declared for this association of the many-to-many relationship.");
    #endregion

    #region ApiRelationshipAssociation Computed Properties
    /// <summary>
    ///     Gets a value indicating whether this association has explicit FK key paths declared for both principal ends.
    ///     When <see langword="true"/>, both <see cref="ApiKeyPathsA"/> and <see cref="ApiKeyPathsB"/> are available.
    /// </summary>
    public bool HasKeyBinding => _apiKeyPathsA is not null && _apiKeyPathsB is not null;

    /// <summary>
    ///     Gets a value indicating whether this association is navigational (i.e. has no explicit FK key binding
    ///     declared at the schema level for either principal end).
    /// </summary>
    public bool IsNavigational => !this.HasKeyBinding;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a navigational association with no FK binding declared at the schema level for either principal end.
    ///     Use when the join-table object type needs to be identified but FK property mapping
    ///     is intentionally left to the downstream layer.
    /// </summary>
    /// <param name="clrObjectType">The CLR type of the association <see cref="ApiObjectType"/>.</param>
    public ApiRelationshipAssociation(Type clrObjectType)
        : base(clrObjectType)
    {
        _apiKeyPathsA = null;
        _apiKeyPathsB = null;
    }

    /// <summary>
    ///     Initializes a key-bound association with explicit FK key paths for both principal ends.
    /// </summary>
    /// <param name="clrObjectType">The CLR type of the association <see cref="ApiObjectType"/>.</param>
    /// <param name="apiKeyPathsA">
    ///     The FK key paths that map the scalar leaves of principal end A's join-key identity
    ///     to properties on the association object type. Must be non-null and contain at least one non-null entry.
    /// </param>
    /// <param name="apiKeyPathsB">
    ///     The FK key paths that map the scalar leaves of principal end B's join-key identity
    ///     to properties on the association object type. Must be non-null and contain at least one non-null entry.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="apiKeyPathsA"/> or <paramref name="apiKeyPathsB"/> is <see langword="null"/>.
    /// </exception>
    public ApiRelationshipAssociation
    (
        Type clrObjectType,
        IEnumerable<ApiRelationshipKeyPath> apiKeyPathsA,
        IEnumerable<ApiRelationshipKeyPath> apiKeyPathsB
    )
        : base(clrObjectType)
    {
        ArgumentNullException.ThrowIfNull(apiKeyPathsA);
        ArgumentNullException.ThrowIfNull(apiKeyPathsB);

        ApiRelationshipKeyPath[] pathsA = [.. apiKeyPathsA.Where(x => x is not null)];
        _apiKeyPathsA = pathsA.Length > 0 ? pathsA : null; // Treat empty collection as null (i.e. no key binding).

        ApiRelationshipKeyPath[] pathsB = [.. apiKeyPathsB.Where(x => x is not null)];
        _apiKeyPathsB = pathsB.Length > 0 ? pathsB : null; // Treat empty collection as null (i.e. no key binding).

        // Enforce symmetry: both sides must be bound or both navigational.
        if (_apiKeyPathsA is null || _apiKeyPathsB is null)
        {
            _apiKeyPathsA = null;
            _apiKeyPathsB = null;
        }
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var hasKeyBinding = this.HasKeyBinding;
        var apiKeyPathsACount = hasKeyBinding ? _apiKeyPathsA!.Length.SafeToString() : "None";
        var apiKeyPathsBCount = hasKeyBinding ? _apiKeyPathsB!.Length.SafeToString() : "None";
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipAssociation)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.HasKeyBinding)}={hasKeyBinding}, {nameof(this.ApiKeyPathsA)}Count={apiKeyPathsACount}, {nameof(this.ApiKeyPathsB)}Count={apiKeyPathsBCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiKeyPaths(context);
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
    private void InitializeApiKeyPaths(ApiInitializationContext context)
    {
        if (this.IsNavigational)
        {
            // No key binding declared — purely navigational relationship.
            return;
        }

        // ApiObjectType is resolved by the base class.
        // Key paths resolve their properties against the assocation object type.
        var apiObjectType = this.ResolvedObjectType;
        if (apiObjectType is null)
        {
            // ClrObjectType is null or failed to resolve — base already recorded the error.
            return;
        }

        var pathContext = context
            .WithDeclaringSchemaElement(this)
            .WithDeclaringObjectTypeOnly(apiObjectType);

        foreach (var keyPathA in this.ApiKeyPathsA)
        {
            keyPathA.Initialize(pathContext);
        }

        foreach (var keyPathB in this.ApiKeyPathsB)
        {
            keyPathB.Initialize(pathContext);
        }
    }
    #endregion
}
