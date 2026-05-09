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

[JsonConverter(typeof(ApiRelationshipAssociationJsonConverter))]
public sealed class ApiRelationshipAssociation
(
    Type clrObjectType,
    IEnumerable<ApiRelationshipKeyPath>? apiKeyPathsA = null,
    IEnumerable<ApiRelationshipKeyPath>? apiKeyPathsB = null
) : ApiRelationshipElement(clrObjectType)
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipAssociation);
    #endregion

    #region ApiRelationshipAssociation Properties
    public ApiRelationshipKeyPath[]? ApiKeyPathsA { get; } = apiKeyPathsA is not null
        ? [.. apiKeyPathsA.Where(x => x is not null)]
        : null;

    public ApiRelationshipKeyPath[]? ApiKeyPathsB { get; } = apiKeyPathsB is not null
        ? [.. apiKeyPathsB.Where(x => x is not null)]
        : null;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiKeyPathsACount = (this.ApiKeyPathsA?.Length ?? 0).SafeToString();
        var apiKeyPathsBCount = (this.ApiKeyPathsB?.Length ?? 0).SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipAssociation)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiKeyPathsA)}Count={apiKeyPathsACount}, {nameof(this.ApiKeyPathsB)}Count={apiKeyPathsBCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiKeyPaths(context, this.ApiKeyPathsA);
        this.InitializeApiKeyPaths(context, this.ApiKeyPathsB);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiKeyPaths(ApiInitializationContext context, ApiRelationshipKeyPath[]? apiKeyPaths)
    {
        if (apiKeyPaths is null || apiKeyPaths.Length == 0)
        {
            // Null or empty is valid — purely navigational relationship.
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

        foreach (var keyPath in apiKeyPaths)
        {
            keyPath.Initialize(pathContext);
        }
    }
    #endregion
}
