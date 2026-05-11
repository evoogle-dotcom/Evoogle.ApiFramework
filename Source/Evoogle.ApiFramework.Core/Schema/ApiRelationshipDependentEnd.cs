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
///     Represents the dependent end of an <see cref="ApiRelationship"/>.
///
///     A dependent end is either <em>navigational</em> — where no FK binding is declared
///     at the schema level — or <em>key-bound</em>, where <see cref="ApiKeyPaths"/>
///     explicitly maps scalar leaves of the principal <see cref="ApiIdentity"/> to
///     properties on the dependent object graph.
/// </summary>
/// <remarks>
///     Use <see cref="HasKeyBinding"/> to determine which state applies before
///     accessing <see cref="ApiKeyPaths"/>.
/// 
///     The state is fixed at construction and does not change after initialization.
/// </remarks>
[JsonConverter(typeof(ApiRelationshipDependentEndJsonConverter))]
public sealed class ApiRelationshipDependentEnd : ApiRelationshipEnd
{
    #region ApiRelationshipDependentEnd Fields
    private readonly ApiRelationshipKeyPath[]? _apiKeyPaths;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipDependentEnd);
    #endregion

    #region ApiRelationshipEnd Properties
    /// <inheritdoc/>
    public override ApiRelationshipEndKind ApiKind => ApiRelationshipEndKind.Dependent;
    #endregion

    #region ApiRelationshipDependentEnd Properties
    /// <summary>
    ///     Gets the FK key paths that map scalar leaves of the principal's <see cref="ApiIdentity"/>
    ///     to properties on this dependent object graph.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="IsNavigational"/> is <see langword="true"/>.
    ///     Check <see cref="HasKeyBinding"/> before accessing this property.
    /// </exception>
    public ApiRelationshipKeyPath[] ApiKeyPaths => this.HasKeyBinding
        ? _apiKeyPaths!
        : throw new ApiSchemaException("No key binding declared for this dependent end of the relationship.");
    #endregion

    #region ApiRelationshipDependentEnd Computed Properties
    /// <summary>
    ///    Gets a value indicating whether this dependent end has an explicit key binding declared at the schema level.
    /// </summary>
    public bool HasKeyBinding => _apiKeyPaths is not null;

    /// <summary>
    ///   Gets a value indicating whether this dependent end is navigational (i.e. has no explicit key binding declared at the schema level).
    /// </summary>
    public bool IsNavigational => !this.HasKeyBinding;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a navigational dependent end with no FK binding declared at the schema level.
    ///
    ///     Use when cardinality or delete behavior needs to be expressed but FK property mapping is
    ///     intentionally left to the downstream layer.
    /// </summary>
    /// <param name="clrObjectType">The CLR type of the dependent <see cref="ApiObjectType"/>.</param>
    public ApiRelationshipDependentEnd(Type clrObjectType)
        : base(clrObjectType)
    {
        _apiKeyPaths = null;
    }

    /// <summary>
    ///     Initializes a key-bound dependent end with explicit FK key paths that map the principal
    ///     identity's scalar leaves to properties on the dependent object graph.
    /// </summary>
    /// <param name="clrObjectType">The CLR type of the dependent <see cref="ApiObjectType"/>.</param>
    /// <param name="apiKeyPaths">
    ///     The FK key paths that map the principal identity's scalar leaves to properties
    ///     on the dependent object graph. Must be non-null and contain at least one non-null entry.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="apiKeyPaths"/> is <see langword="null"/>.</exception>
    public ApiRelationshipDependentEnd(Type clrObjectType, IEnumerable<ApiRelationshipKeyPath> apiKeyPaths)
        : base(clrObjectType)
    {
        ArgumentNullException.ThrowIfNull(apiKeyPaths);

        ApiRelationshipKeyPath[] paths = [.. apiKeyPaths.Where(x => x is not null)];
        _apiKeyPaths = paths.Length > 0 ? paths : null; // Treat empty collection as null (i.e. no key binding).
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var hasKeyBinding = this.HasKeyBinding;
        var apiKeyPathsCount = hasKeyBinding ? _apiKeyPaths!.Length.SafeToString() : "None";
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipDependentEnd)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.HasKeyBinding)}={hasKeyBinding}, {nameof(this.ApiKeyPaths)}Count={apiKeyPathsCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
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

    #region Implementation Methods
    private void InitializeApiKeyPaths(ApiInitializationContext context)
    {
        if (this.IsNavigational)
        {
            // No key binding declared — purely navigational relationship.
            return;
        }

        // ApiObjectType is resolved by the base class.
        // Key paths resolve their properties against the dependent object type.
        var apiObjectType = this.ResolvedObjectType;
        if (apiObjectType is null)
        {
            // ClrObjectType is null or failed to resolve — base already recorded the error.
            return;
        }

        var pathContext = context
            .WithDeclaringSchemaElement(this)
            .WithDeclaringObjectTypeOnly(apiObjectType);

        foreach (var keyPath in this.ApiKeyPaths)
        {
            keyPath.Initialize(pathContext);
        }
    }
    #endregion
}
