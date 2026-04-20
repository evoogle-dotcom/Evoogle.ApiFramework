// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a key path that navigates to the owning object type of the dependent object
///     to locate FK scalar values for the principal identity.
/// </summary>
/// <param name="apiKeyPaths">
///     The optional child key paths that resolve FK scalar values within the owner object type.
///     When <see langword="null"/>, the binding implicitly spans all scalar leaves of the owner's primary identity.
/// </param>
/// <remarks>
///     <para>
///         Owner resolution follows the same two-phase initialization model as <see cref="ApiIdentityOwnerPart"/>.
///         During the standard <see cref="ApiSchemaElement.Initialize"/> call, no owner resolution occurs.
///         The owner type is resolved in a second schema-wide deferred pass after all object types and
///         relationships have been initialized, via <c>ApiSchema.ResolveOwnerRelationshipKeyPaths</c>.
///     </para>
///     <para>
///         Accessing <see cref="ApiOwnerType"/> before schema initialization completes will throw.
///     </para>
/// </remarks>
public sealed class ApiRelationshipOwnerKeyPath(IEnumerable<ApiRelationshipKeyPath>? apiKeyPaths = null) : ApiRelationshipKeyPath
{
    #region ApiRelationshipOwnerKeyPath Fields
    private ApiObjectType? _apiResolvedOwnerType = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipOwnerKeyPath);
    #endregion

    #region ApiRelationshipKeyPath Properties
    /// <inheritdoc/>
    public override ApiRelationshipKeyPathKind ApiKind => ApiRelationshipKeyPathKind.Owner;
    #endregion

    #region ApiRelationshipOwnerKeyPath Properties
    /// <summary>
    ///     Gets the optional child key paths within the owner object type.
    ///     When <see langword="null"/>, the binding implicitly spans all scalar leaves of the owner's primary identity.
    /// </summary>
    public ApiRelationshipKeyPath[]? ApiKeyPaths { get; } = apiKeyPaths is not null
        ? [.. apiKeyPaths.Where(x => x is not null)]
        : null;

    /// <summary>Gets the resolved owner <see cref="ApiObjectType"/>. Available after schema initialization.</summary>
    public ApiObjectType ApiOwnerType => this.ThrowIfNotInitialized(_apiResolvedOwnerType);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipOwnerKeyPath)} {{{nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: null);
    #endregion

    #region Implementation Methods
    internal void ResolveOwnerType(ApiObjectType ownerType, ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(ownerType);
        ArgumentNullException.ThrowIfNull(context);

        _apiResolvedOwnerType = ownerType;

        if (this.ApiKeyPaths is null || this.ApiKeyPaths.Length == 0)
        {
            return;
        }

        // Child paths resolve against the owner's type.
        var ownerContext = context
            .WithDeclaringSchemaElement(this)
            .WithDeclaringObjectTypeOnly(ownerType);
        foreach (var keyPath in this.ApiKeyPaths)
        {
            keyPath.Initialize(ownerContext);
        }
    }
    #endregion
}
