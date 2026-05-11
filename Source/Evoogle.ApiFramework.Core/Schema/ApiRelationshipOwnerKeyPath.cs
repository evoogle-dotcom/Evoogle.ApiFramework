// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a key path that navigates to the owning object type of the dependent object
///     to locate FK scalar values for the principal identity.
/// </summary>
/// <remarks>
///     <para>
///         Two forms are available:
///         <list type="bullet">
///             <item>
///                 <description>
///                     <strong>Convention-based</strong> (<see cref="ApiRelationshipOwnerKeyPath()"/>):
///                     binds to all scalar leaves of the owner's primary identity automatically.
///                     Check <see cref="UseConvention"/> to query this state.
///                 </description>
///             </item>
///             <item>
///                 <description>
///                     <strong>Explicitly specified</strong> (<see cref="ApiRelationshipOwnerKeyPath(IEnumerable{ApiRelationshipKeyPath})"/>):
///                     binds to the scalar leaves identified by the provided child key paths.
///                     Check <see cref="HasKeyBinding"/> to query this state.
///                 </description>
///             </item>
///         </list>
///     </para>
///     <para>
///         Owner resolution follows the same two-phase initialization model as <see cref="ApiIdentityOwnerPart"/>.
///         During the standard <see cref="ApiSchemaElement.Initialize"/> call, no owner resolution occurs.
///         The owner type is resolved in a second schema-wide deferred pass after all object types and
///         relationships have been initialized, via <c>ApiSchema.ResolveOwnerRelationshipKeyPaths</c>.
///     </para>
///     <para>
///         Accessing <see cref="ApiObjectType"/> before schema initialization completes will throw.
///     </para>
/// </remarks>
public sealed class ApiRelationshipOwnerKeyPath : ApiRelationshipKeyPath
{
    #region ApiRelationshipOwnerKeyPath Fields
    private readonly ApiRelationshipKeyPath[]? _apiKeyPaths;

    private ApiObjectType? _apiResolvedObjectType = null;
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
    ///     Gets the explicitly specified child key paths that resolve FK scalar values within the owner object type.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="HasKeyBinding"/> is <see langword="false"/>.
    ///     Check <see cref="HasKeyBinding"/> before accessing this property.
    /// </exception>
    public ApiRelationshipKeyPath[] ApiKeyPaths => this.HasKeyBinding
        ? _apiKeyPaths!
        : throw new ApiSchemaException("No key binding declared for this relationship owner key path, use all scalar leaves of the principal's primary identity of the relationship.");

    /// <summary>Gets the resolved owner <see cref="Schema.ApiObjectType"/>. Available after schema initialization.</summary>
    public ApiObjectType ApiObjectType => this.ThrowIfNotInitialized(_apiResolvedObjectType);
    #endregion

    #region ApiRelationshipOwnerKeyPath Computed Properties
    /// <summary>
    ///     Gets a value indicating whether this owner key path has explicit child key paths declared.
    ///     When <see langword="false"/>, <see cref="UseConvention"/> is <see langword="true"/> and the binding
    ///     spans all scalar leaves of the owner's primary identity automatically.
    /// </summary>
    public bool HasKeyBinding => _apiKeyPaths is not null;

    /// <summary>
    ///     Gets a value indicating whether this owner key path uses all scalar leaves of the owner's
    ///     primary identity by convention, with no explicit child key paths declared.
    ///     The inverse of <see cref="HasKeyBinding"/>.
    /// </summary>
    public bool UseConvention => !this.HasKeyBinding;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a convention-based owner key path that binds to all scalar leaves of the
    ///     owner's primary identity automatically. Use when no explicit FK property selection is needed.
    /// </summary>
    public ApiRelationshipOwnerKeyPath()
    {
        _apiKeyPaths = null;
    }

    /// <summary>
    ///     Initializes an owner key path with explicitly specified child key paths that resolve
    ///     FK scalar values within the owner object type.
    /// </summary>
    /// <param name="apiKeyPaths">
    ///     The child key paths that identify which scalar leaves within the owner object type
    ///     serve as FK values for the principal identity. If all elements are <see langword="null"/>
    ///     or the collection is empty, falls back to convention-based behavior, equivalent to the
    ///     parameterless constructor.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="apiKeyPaths"/> is <see langword="null"/>.
    /// </exception>
    public ApiRelationshipOwnerKeyPath(IEnumerable<ApiRelationshipKeyPath> apiKeyPaths)
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
        var hasKeyBinding = this.HasKeyBinding;
        var apiKeyPathsCount = hasKeyBinding ? _apiKeyPaths!.Length.SafeToString() : "None";
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipOwnerKeyPath)} {{{nameof(this.HasKeyBinding)}={hasKeyBinding}, {nameof(this.ApiKeyPaths)}Count={apiKeyPathsCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
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

        _apiResolvedObjectType = ownerType;

        if (this.UseConvention)
        {
            // No explicit key paths declared — bind to all scalar leaves of the owner's primary identity by convention.
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
