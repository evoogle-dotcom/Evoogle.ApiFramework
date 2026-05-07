// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the dependent end of an <see cref="ApiRelationship"/>.
///     The dependent end holds the FK binding: its <see cref="ApiKeyPaths"/> describe how scalar leaves
///     of the principal <see cref="ApiIdentity"/> map to properties on the dependent object graph.
/// </summary>
/// <param name="clrObjectType">The CLR type of the dependent <see cref="ApiObjectType"/>.</param>
/// <param name="apiKeyPaths">
///     The optional FK key paths that map the principal identity's scalar leaves to properties
///     on the dependent object graph. When <see langword="null"/>, the relationship is purely
///     navigational with no explicit FK binding declared at the schema level.
/// </param>
[JsonConverter(typeof(ApiRelationshipDependentEndJsonConverter))]
public sealed class ApiRelationshipDependentEnd
(
    Type clrObjectType,
    IEnumerable<ApiRelationshipKeyPath>? apiKeyPaths = null
) : ApiRelationshipEnd(clrObjectType)
{
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
    ///     <see langword="null"/> when the relationship is purely navigational.
    /// </summary>
    public ApiRelationshipKeyPath[]? ApiKeyPaths { get; } = apiKeyPaths is not null
        ? [.. apiKeyPaths.Where(x => x is not null)]
        : null;

    /// <summary>
    ///     Gets the strongly-typed principal end of the relationship. Available after initialization.
    /// </summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEnd => (ApiRelationshipPrincipalEnd)this.ApiOppositeEnd;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiKeyPathsCount = (this.ApiKeyPaths?.Length ?? 0).SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipDependentEnd)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiKeyPaths)}Count={apiKeyPathsCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
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
        if (this.ApiKeyPaths is null || this.ApiKeyPaths.Length == 0)
        {
            // Null or empty is valid — purely navigational relationship.
            return;
        }

        // ApiObjectType is resolved by the base class. Key paths resolve their
        // properties against the dependent object type.
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
